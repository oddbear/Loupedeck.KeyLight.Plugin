namespace Loupedeck.KeyLightPlugin.Helpers
{
    using System;

    public static class RangeHelper
    {
        public static Int32 Range(Int32 value, Int32 min, Int32 max)
        {
            if (value < min)
                return min;

            if (value > max)
                return max;

            return value;
        }
    }
}
