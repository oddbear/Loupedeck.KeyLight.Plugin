namespace Loupedeck.KeyLightPlugin.Helpers
{
    using System;

    public static class RangeHelper
    {
        public static int Range(int value, int min, int max)
        {
            if (value < min)
                return min;

            if (value > max)
                return max;

            return value;
        }

        public static int ToKelvin(int x)
        {
            //Approximation (off by ~ 50-150K):
            var ticks = 0.0900835 * x * x - 63.5864 * x + 14177.3;

            return (int)(Math.Round(ticks / 50.0) * 50.0);
        }
    }
}
