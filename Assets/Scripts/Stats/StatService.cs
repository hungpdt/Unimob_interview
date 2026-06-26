using System.Collections.Generic;

namespace Farm
{
    public class StatService
    {
        public const int GlobalScope = -1;

        private readonly Dictionary<int, StatCollection> _profitBySlot = new Dictionary<int, StatCollection>();

        // BaseValue = 1.0 — contains only Multiplicative modifiers, so Value = Π multipliers
        private readonly StatCollection _globalProfit = new StatCollection(StatType.Profit, 1.0);

        public StatCollection GetProfitStat(int slotIndex)
        {
            if (!_profitBySlot.TryGetValue(slotIndex, out StatCollection col))
            {
                col = new StatCollection(StatType.Profit, 0d);
                _profitBySlot[slotIndex] = col;
            }
            return col;
        }

        public double GetGlobalProfitMultiplier()
        {
            return _globalProfit.Value;
        }

        public void AddGlobalProfitModifier(StatModifier mod)
        {
            _globalProfit.AddModifier(mod);
        }

        public void AddConstructionProfitModifier(int slot, StatModifier mod)
        {
            GetProfitStat(slot).AddModifier(mod);
        }

        public void RemoveBySource(object source)
        {
            if (source == null)
            {
                return;
            }

            _globalProfit.RemoveAllFromSource(source);
            foreach (StatCollection col in _profitBySlot.Values)
            {
                col.RemoveAllFromSource(source);
            }
        }
    }
}
