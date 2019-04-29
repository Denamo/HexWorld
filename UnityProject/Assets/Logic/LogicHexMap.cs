
using logic.debug;
using logic.math;
using System.Collections.Generic;

namespace logic.util
{

	public class LogicHexMap : LogicArray2<int>
	{
		public LogicHexMap(int sizeQ, int sizeR) : this(sizeQ, sizeR, 0)
		{
		}

		public LogicHexMap(int sizeQ, int sizeR, int defaultValue) : base(sizeQ, sizeR, defaultValue)
		{
		}

        public void Set(LogicHex pos, int value)
        {
            Set(pos.q,pos.r,value);
        }

        public int Get(LogicHex pos)
        {
            return Get(pos.q, pos.r);
        }


    }


    public static class LogicHexUtils
    {

        public static void Line(LogicHexMap map, LogicHex a, LogicHex b, int value)
        {
            map.Set(a, value);

            if (a.Equals(b))
            {
                return;
            }

            LogicHex diff = b.Sub(a);

            int absQ = LogicMath.abs(diff.q);
            int absR = LogicMath.abs(diff.r);
            int absS = LogicMath.abs(diff.s);

            bool flip = false;
            int longDistance;
            int shortDistance;

            int longQ, longR, shortQ, shortR;

            if (absQ > absR)
            {
                if (absR > absS)
                {
                    //q,r,s
                    flip = diff.q < 0;
                    longDistance = absR;
                    shortDistance = absS;

                    longQ = 1;
                    longR = -1;
                    shortQ = 1;
                    shortR = 0;
                }
                else
                {
                    if (absS > absQ)
                    {
                        //s,q,r
                        flip = diff.s < 0;
                        longDistance = absQ;
                        shortDistance = absR;

                        longQ = -1;
                        longR = 0;
                        shortQ = 0;
                        shortR = -1;
                    }
                    else
                    {
                        //q,s,r
                        flip = diff.q < 0;
                        longDistance = absS;
                        shortDistance = absR;

                        longQ = 1;
                        longR = 0;
                        shortQ = 1;
                        shortR = -1;
                    }
                }
            }
            else
            {
                if (absR > absS)
                {
                    if (absS > absQ)
                    {
                        //r,s,q
                        flip = diff.r < 0;
                        longDistance = absS;
                        shortDistance = absQ;

                        longQ = 0;
                        longR = 1;
                        shortQ = -1;
                        shortR = 1;
                    }
                    else
                    {
                        //r,q,s
                        flip = diff.r < 0;
                        longDistance = absQ;
                        shortDistance = absS;

                        longQ = -1;
                        longR = 1;
                        shortQ = 0;
                        shortR = 1;
                    }
                }
                else
                {
                    //s,r,q
                    flip = diff.s < 0;
                    longDistance = absR;
                    shortDistance = absQ;

                    longQ = 0;
                    longR = -1;
                    shortQ = -1;
                    shortR = 0;
                }
            }

            if (flip)
            {
                longQ = -longQ;
                longR = -longR;
                shortQ = -shortQ;
                shortR = -shortR;
            }

            int q = a.q;
            int r = a.r;

            int distance = longDistance + shortDistance;
            int bias = shortDistance - longDistance;
            int longBiasStep = -2 * longDistance;
            int shortBiasStep = 2 * shortDistance;

            for (int i = 0; i < distance; ++i)
            {
                /*if (bias == 0)
                {
                    //This is the case when we hit right between two hex tiles.
                    //Depending on the use case you might want to:
                    //- Draw either
                    //- Draw both
                    //- Draw neither
                    //- Bias axis direction so that a => b is allways same as b => a
                    // Here I have chosen to just prioritice shortAxes
                }*/
                if (bias > 0)
                {
                    bias += longBiasStep;
                    q += shortQ;
                    r += shortR;
                }
                else
                {
                    bias += shortBiasStep;
                    q += longQ;
                    r += longR;
                }

                map.Set(q, r, value);
            }

        }

    }




}
