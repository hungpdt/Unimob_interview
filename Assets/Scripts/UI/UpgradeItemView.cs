using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Farm
{
    public class UpgradeItemView : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _descText;
        [SerializeField] private TMP_Text _costText;
        [SerializeField] private Button _button;

        public void Bind(UpgradeConfig config, double cost, Action onPurchase)
        {
            if (config == null)
            {
                Debug.LogError("[UpgradeItemView] config is null.", this);
                return;
            }

            if (_icon != null)
            {
                _icon.sprite = config.Icon;
                _icon.enabled = config.Icon != null;
            }

            if (_titleText != null)
            {
                _titleText.text = config.Title;
            }

            if (_descText != null)
            {
                _descText.text = config.Description;
            }

            if (_costText != null)
            {
                _costText.text = NumberFormatter.Format(cost);
            }

            if (_button != null)
            {
                _button.onClick.RemoveAllListeners();
                if (onPurchase != null)
                {
                    _button.onClick.AddListener(() => onPurchase());
                }
            }
        }

        public void Refresh(double cost, bool canAfford)
        {
            if (_costText != null)
            {
                _costText.text = NumberFormatter.Format(cost);
            }

            if (_button != null)
            {
                _button.interactable = canAfford;
            }
        }
    }
}
