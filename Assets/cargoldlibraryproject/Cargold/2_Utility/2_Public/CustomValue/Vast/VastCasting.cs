namespace Cargold.Infinite
{
    using Cargold.Vast;

    public static partial class InfiniteExtensionMethod
    {
        public static Infinite ToInfinite(this Vast value)
        {
            return Infinite.ToInfinite(value);
        }
    }

    public partial struct Infinite
    {
        public static explicit operator Infinite(Vast value)
        {
            return Infinite.ToInfinite(value);
        }

        public static Infinite ToInfinite(Vast value)
        {
            Infinite _inf = new Infinite();

            int _currentDigit = value.CurrentDigit;
            for (int i = 0; i < 5; ++i)
            {
                int _digitValue = value.GetValue(_currentDigit - i);
                if (0 < _digitValue)
                    _inf.Addition(_digitValue, _currentDigit - i);
            }

            return _inf;
        }
    }
}