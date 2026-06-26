namespace Farm
{
    public readonly struct StatModifier
    {
        public ModifierType Type { get; }
        public float Value { get; }
        public object Source { get; }   // identity token for the buff source — enables remove-by-source

        public StatModifier(ModifierType type, float value, object source)
        {
            Type = type;
            Value = value;
            Source = source;
        }
    }
}
