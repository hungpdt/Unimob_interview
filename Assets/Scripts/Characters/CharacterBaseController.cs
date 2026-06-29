using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Farm
{
    public abstract class CharacterBaseController : MonoBehaviour
    {
        private const string AnimIsMove = "IsMove";
        private const string AnimIsCarryMove = "IsCarryMove";
        private const string AnimIsEmpty = "IsEmpty";

        [Header("Fruit Animation")]
        [SerializeField] private float _fruitStackOffset = 0.3f;
        [SerializeField] private float _fruitFlyDuration = 0.6f;
        [SerializeField] private float _fruitTransferStagger = 0.08f;

        [Header("References")]
        [SerializeField] protected Transform _fruitAnchor;

        protected NavMeshAgent _agent;
        protected Animator _animator;
        protected GameObject[] _carriedFruits;

        private Coroutine[] _fruitCoroutines;
        private int _pendingFruitCount;
        private System.Action _onFruitArrived;
        private System.Action _onAllFruitsArrived;
        private WaitForSeconds[] _staggerWaits;

        public bool IsFruitAnimating { get; private set; }

        protected void InitCharacter(float moveSpeed)
        {
            _agent = GetComponent<NavMeshAgent>();
            _animator = GetComponentInChildren<Animator>();
            _agent.speed = moveSpeed;
            _onFruitArrived = OnFruitArrived;
            _staggerWaits = new WaitForSeconds[8];
            for (int i = 1; i < _staggerWaits.Length; i++)
            {
                _staggerWaits[i] = new WaitForSeconds(i * _fruitTransferStagger);
            }
        }

        public void SetDestination(Vector3 position)
        {
            if (_agent == null || !_agent.isOnNavMesh)
            {
                return;
            }

            _agent.isStopped = false;
            _agent.SetDestination(position);
        }

        public void StopMovement()
        {
            if (_agent == null || !_agent.isOnNavMesh)
            {
                return;
            }

            _agent.isStopped = true;
            _agent.velocity = Vector3.zero;
        }

        public bool HasArrived(float tolerance)
        {
            if (_agent == null || !_agent.isOnNavMesh || _agent.pathPending)
            {
                return false;
            }

            float reach = Mathf.Max(tolerance, _agent.stoppingDistance);
            if (_agent.remainingDistance > reach)
            {
                return false;
            }

            return !_agent.hasPath || _agent.velocity.sqrMagnitude < 0.01f;
        }

        public void FaceToward(Vector3 worldPosition)
        {
            Vector3 direction = worldPosition - transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.0001f)
            {
                return;
            }

            transform.rotation = Quaternion.LookRotation(direction);
        }

        public void AttachFruits(GameObject[] fruits, System.Action onAllArrived = null)
        {
            StopAllFruitAnimation();
            _carriedFruits = fruits;

            if (fruits == null)
            {
                onAllArrived?.Invoke();
                return;
            }

            if (_fruitAnchor == null)
            {
                Debug.LogError($"[{GetType().Name}] _fruitAnchor is null", this);
                onAllArrived?.Invoke();
                return;
            }

            int nonNull = 0;
            for (int i = 0; i < fruits.Length; i++)
            {
                if (fruits[i] != null)
                {
                    nonNull++;
                }
            }

            if (nonNull == 0)
            {
                onAllArrived?.Invoke();
                return;
            }

            IsFruitAnimating = true;
            _pendingFruitCount = nonNull;
            _onAllFruitsArrived = onAllArrived;
            _fruitCoroutines = new Coroutine[fruits.Length];

            for (int i = 0; i < fruits.Length; i++)
            {
                if (fruits[i] == null)
                {
                    continue;
                }

                fruits[i].SetActive(true);
                _fruitCoroutines[i] = StartCoroutine(
                    FlyFruitToAnchor(fruits[i], _fruitAnchor, Vector3.up * (_fruitStackOffset * i),
                        _fruitFlyDuration, i, _onFruitArrived));
            }
        }

        private void OnFruitArrived()
        {
            _pendingFruitCount--;
            if (_pendingFruitCount <= 0)
            {
                IsFruitAnimating = false;
                System.Action cb = _onAllFruitsArrived;
                _onAllFruitsArrived = null;
                cb?.Invoke();
            }
        }

        public void DestroyCarriedFruits()
        {
            StopAllFruitAnimation();
            if (_carriedFruits == null)
            {
                return;
            }

            foreach (GameObject fruit in _carriedFruits)
            {
                if (fruit != null)
                {
                    Destroy(fruit);
                }
            }

            _carriedFruits = null;
        }

        protected GameObject[] TakeCarriedFruits()
        {
            StopAllFruitAnimation();
            if (_carriedFruits != null && _fruitAnchor != null)
            {
                for (int i = 0; i < _carriedFruits.Length; i++)
                {
                    if (_carriedFruits[i] == null)
                    {
                        continue;
                    }

                    _carriedFruits[i].transform.SetParent(_fruitAnchor, false);
                    _carriedFruits[i].transform.localPosition = Vector3.up * (_fruitStackOffset * i);
                    _carriedFruits[i].transform.SetParent(null, true);
                }
            }

            GameObject[] fruits = _carriedFruits;
            _carriedFruits = null;
            return fruits;
        }

        private void StopAllFruitAnimation()
        {
            IsFruitAnimating = false;
            _pendingFruitCount = 0;
            _onAllFruitsArrived = null;

            if (_fruitCoroutines != null)
            {
                foreach (Coroutine c in _fruitCoroutines)
                {
                    if (c != null)
                    {
                        StopCoroutine(c);
                    }
                }

                _fruitCoroutines = null;
            }
        }

        private IEnumerator FlyFruitToAnchor(GameObject fruit, Transform anchor, Vector3 targetLocal, float duration, int fruitIndex, System.Action onArrived = null)
        {
            if (fruitIndex > 0 && fruitIndex < _staggerWaits.Length)
            {
                yield return _staggerWaits[fruitIndex];
            }

            if (fruit == null)
            {
                onArrived?.Invoke();
                yield break;
            }

            Vector3 startWorld = fruit.transform.position;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                if (fruit == null)
                {
                    onArrived?.Invoke();
                    yield break;
                }

                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / duration));
                fruit.transform.position = Vector3.Lerp(startWorld, anchor.TransformPoint(targetLocal), t);
                yield return null;
            }

            if (fruit == null)
            {
                onArrived?.Invoke();
                yield break;
            }

            fruit.transform.SetParent(anchor, false);
            fruit.transform.localPosition = targetLocal;
            onArrived?.Invoke();
        }

        public void SetLocomotion(bool moving, bool carrying)
        {
            if (_animator == null)
            {
                return;
            }

            _animator.SetBool(AnimIsEmpty, !carrying);
            _animator.SetBool(AnimIsMove, moving && !carrying);
            _animator.SetBool(AnimIsCarryMove, moving && carrying);
        }
    }
}
