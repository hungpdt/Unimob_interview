using UnityEngine;
using UnityEngine.AI;

namespace Farm
{
    public abstract class CharacterBaseController : MonoBehaviour
    {
        private const string AnimIsMove = "IsMove";
        private const string AnimIsCarryMove = "IsCarryMove";
        private const string AnimIsEmpty = "IsEmpty";

        protected NavMeshAgent _agent;
        protected Animator _animator;

        protected void InitCharacter(float moveSpeed)
        {
            _agent = GetComponent<NavMeshAgent>();
            _animator = GetComponentInChildren<Animator>();
            _agent.speed = moveSpeed;
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
