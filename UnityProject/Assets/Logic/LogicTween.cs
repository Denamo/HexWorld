using System;

namespace logic.math
{
    public static class LogicTween
    {
        public const int SCALE_SHIFT = 10;
        public const int SCALE = 1 << SCALE_SHIFT;
        public const int SCALE_2 = SCALE * 2;
        public const int SCALE_HALF = SCALE / 2;


        public static int QuadIn(int k)
        {
            return (k * k) / SCALE;
        }

        public static int QuadOut(int k)
        {
            return (k * (SCALE_2 - k)) / SCALE;
        }

        public static int QuadInOut(int k)
        {
            if (k < SCALE_HALF)
                return QuadIn(k * 2) / 2;
            else
                return SCALE_HALF + QuadOut((k - SCALE_HALF) * 2) / 2;
        }

    }
}