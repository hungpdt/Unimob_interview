using System.Collections.Generic;
using UnityEngine;

namespace Farm
{
    public class UpgradeViewController : BaseViewController
    {
        [SerializeField] private RectTransform _content;
        [SerializeField] private GameObject _itemPrefab;

        private readonly List<UpgradeItemView> _items = new List<UpgradeItemView>();

        public void Bind(IReadOnlyList<UpgradeConfig> configs, UpgradeManager upgrades)
        {
            // TODO: implement
        }

        private void OnPurchase(UpgradeConfig config)
        {
            // TODO: implement
        }
    }
}
