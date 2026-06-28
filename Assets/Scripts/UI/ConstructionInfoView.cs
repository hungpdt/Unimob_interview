using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Farm
{
    public class ConstructionInfoView : MonoBehaviour
    {
        [SerializeField] private Image _productIcon;
        [SerializeField] private TMP_Text _amountText;
        [SerializeField] private TMP_Text _perMinText;

        private ConstructionController _target;
        private Camera _camera;

        public void Bind(ConstructionController construction)
        {
            if (construction == null)
            {
                Debug.LogError("[ConstructionInfoView] construction is null.", this);
                return;
            }

            if (_target != null)
            {
                _target.OnStatsChanged -= OnStatsChanged;
            }

            _target = construction;
            _target.OnStatsChanged += OnStatsChanged;

            if (_productIcon != null)
            {
                _productIcon.sprite = construction.Config.Icon;
                _productIcon.enabled = construction.Config.Icon != null;
            }

            Refresh();
        }

        private void OnDestroy()
        {
            if (_target != null)
            {
                _target.OnStatsChanged -= OnStatsChanged;
            }
        }

        private void OnStatsChanged(ConstructionController construction)
        {
            Refresh();
        }

        private void Refresh()
        {
            if (_target == null || _target.Config == null)
            {
                Debug.LogError("[ConstructionInfoView] _target or its Config is null.", this);
                return;
            }

            double profit = _target.CurrentProfit;

            if (_amountText != null)
            {
                _amountText.text = NumberFormatter.Format(profit);
            }

            if (_perMinText != null)
            {
                // ProduceTime is seconds per cycle, so per-cycle profit scales to per-minute.
                float produceTime = _target.Config.ProduceTime;
                double perMin = produceTime > 0f ? profit * (60.0 / produceTime) : 0.0;
                _perMinText.text = NumberFormatter.Format(perMin) + "/min";
            }
        }

        private void LateUpdate()
        {
            if (_camera == null)
            {
                _camera = Camera.main;
            }

            if (_camera != null)
            {
                transform.rotation = _camera.transform.rotation;
            }
        }
    }
}
