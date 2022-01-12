using System;
using System.Linq;

namespace Loupedeck.KeyLightPlugin.Helpers
{
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
        
        public static int ToKelvin(int index)
        {
            index = Range(index, 143, 344);
            index -= 143;

            return _lookupTable[index];
        }

        public static int NextTemperatureIndex(int value, int ticks)
        {
            var currentKelvin = ToKelvin(value);
            var nextValue = value + ticks;

            if (ticks < 0)
            {
                for (var i = nextValue; i > 143; i--)
                {
                    var nextKelvin = ToKelvin(i);
                    if (nextKelvin > currentKelvin)
                        return i;
                }

                return 143;
            }
            else
            {
                for (var i = nextValue; i < 344; i++)
                {
                    var nextKelvin = ToKelvin(i);
                    if (nextKelvin < currentKelvin)
                        return i;
                }

                return 344;
            }
        }

        private static readonly int[] _lookupTable =
        {
            7000, 6950, 6900, 6850, 6800, 6750, 6700, 6650, 6600, 6600, 6550, 6500,
            6450, 6400, 6350, 6350, 6300, 6250, 6200, 6150, 6150, 6100, 6050, 6000,
            6000, 5950, 5900, 5900, 5850, 5800, 5800, 5750, 5700, 5700, 5650, 5600,
            5600, 5550, 5500, 5500, 5450, 5450, 5400, 5400, 5350, 5300, 5300, 5250,
            5250, 5200, 5200, 5150, 5150, 5100, 5100, 5050, 5050, 5000, 5000, 4950,
            4950, 4900, 4900, 4850, 4850, 4800, 4800, 4750, 4750, 4700, 4700, 4650,
            4650, 4650, 4600, 4600, 4550, 4550, 4500, 4500, 4500, 4450, 4450, 4400,
            4400, 4400, 4350, 4350, 4350, 4300, 4300, 4250, 4250, 4250, 4200, 4200,
            4200, 4150, 4150, 4150, 4100, 4100, 4100, 4050, 4050, 4050, 4000, 4000,
            4000, 3950, 3950, 3950, 3900, 3900, 3900, 3900, 3850, 3850, 3850, 3800,
            3800, 3800, 3750, 3750, 3750, 3750, 3700, 3700, 3700, 3700, 3650, 3650,
            3600, 3600, 3600, 3600, 3600, 3550, 3550, 3550, 3550, 3500, 3500, 3500,
            3500, 3450, 3450, 3450, 3450, 3400, 3400, 3400, 3400, 3400, 3350, 3350,
            3350, 3350, 3300, 3300, 3300, 3300, 3300, 3250, 3250, 3250, 3250, 3250,
            3200, 3200, 3200, 3200, 3150, 3150, 3150, 3150, 3150, 3150, 3100, 3100,
            3100, 3100, 3100, 3050, 3050, 3050, 3050, 3050, 3000, 3000, 3000, 3000,
            3000, 3000, 2950, 2950, 2950, 2950, 2950, 2900, 2900, 2900
        };
    }
}
