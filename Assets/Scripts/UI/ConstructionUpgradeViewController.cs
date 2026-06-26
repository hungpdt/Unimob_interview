using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Farm
{
    public class ConstructionUpgradeViewController : BaseViewController
    {
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _costText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private Slider _progressSlider;
        [SerializeField] private Button _upgradeButton;
        [SerializeField] private GameObject _maxLabel;

        private ConstructionController _target;

        public void Bind(ConstructionController construction)
        {
            // TODO: implement
        }

        private void Refresh()
        {
            // TODO: implement
        }
    }
}
