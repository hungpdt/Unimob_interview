using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Farm
{
    public class BuildTimerView : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private TMP_Text _label;

        public void SetProgress(float fraction01, float remainingSeconds)
        {
            if (_slider == null || _label == null)
            {
                Debug.LogError("[BuildTimerView] _slider or _label is null", this);
                return;
            }

            _slider.value = Mathf.Clamp01(fraction01);
            _label.text = remainingSeconds.ToString("0.0") + "s";
        }
        
        private void LateUpdate()
        {
            if (Camera.main != null)
            {
                transform.rotation = Camera.main.transform.rotation;
            }
        }
    }
}
