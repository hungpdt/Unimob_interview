using System;
using System.Collections.Generic;

namespace Farm
{
    // Formula: Final = Base * (1 + Σ Additive) * Π Multiplicative.
    public class StatCollection
    {
        public event Action OnChanged;

        public StatType Stat { get; }

        private readonly List<StatModifier> _modifiers = new List<StatModifier>();
        private double _baseValue;
        private double _cachedValue;
        private bool _isDirty = true;

        public StatCollection(StatType stat, double baseValue)
        {
            Stat = stat;
            _baseValue = baseValue;
        }

        public double BaseValue
        {
            get { return _baseValue; }
            set
            {
                _baseValue = value;
                MarkDirty();
            }
        }

        public double Value
        {
            get
            {
                if (_isDirty)
                {
                    _cachedValue = Recalculate();
                    _isDirty = false;
                }
                return _cachedValue;
            }
        }

        public void AddModifier(StatModifier mod)
        {
            _modifiers.Add(mod);
            MarkDirty();
        }

        public bool RemoveModifier(StatModifier mod)
        {
            bool removed = _modifiers.Remove(mod);
            if (removed)
            {
                MarkDirty();
            }
            return removed;
        }

        public int RemoveAllFromSource(object source)
        {
            if (source == null)
            {
                return 0;
            }

            int removed = _modifiers.RemoveAll(m => Equals(m.Source, source));
            if (removed > 0)
            {
                MarkDirty();
            }
            return removed;
        }

        public void ClearModifiers()
        {
            if (_modifiers.Count == 0)
            {
                return;
            }

            _modifiers.Clear();
            MarkDirty();
        }

        private void MarkDirty()
        {
            _isDirty = true;
            OnChanged?.Invoke();
        }

        private double Recalculate()
        {
            double additive = 1.0;
            double mult = 1.0;

            foreach (StatModifier mod in _modifiers)
            {
                if (mod.Type == ModifierType.Additive)
                {
                    additive += mod.Value;
                }
                else
                {
                    mult *= mod.Value;
                }
            }

            return _baseValue * additive * mult;
        }
    }
}
