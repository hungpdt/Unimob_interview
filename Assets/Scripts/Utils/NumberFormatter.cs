namespace Farm
{
    public static class NumberFormatter
    {
        private static readonly string[] _suffixes = { "", "K", "M", "B", "T", "Qa", "Qi" };

        public static string Format(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                return "∞";
            }

            if (value < 0)
            {
                return "-" + Format(-value);
            }

            if (value < 1000)
            {
                return ((long)value).ToString();
            }

            int tier = 0;
            double v = value;
            while (v >= 1000d && tier < _suffixes.Length - 1)
            {
                v /= 1000d;
                tier++;
            }

            return v.ToString("0.0") + _suffixes[tier];
        }
    }
}
