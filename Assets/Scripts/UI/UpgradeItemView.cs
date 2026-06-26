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
            // TODO: implement
        }

        public void Refresh(double cost, bool canAfford)
        {
            // TODO: implement
        }
    }
}
