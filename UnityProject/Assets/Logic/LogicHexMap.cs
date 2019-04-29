
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

            int distance = (absQ + absR + absS) / 2;

            LogicHex longAxes;
            LogicHex shortAxes;
            bool flip = false;
            int longLength = 0;
            int shortLength = 0;


            if (absQ > absR)
            {
                if(absR > absS)
                {
                    //q,r,s
                    flip = diff.q < 0;
                    longAxes = new LogicHex(1, -1, 0);
                    shortAxes = new LogicHex(1, 0, -1);
                    longLength = absR;
                    shortLength = absS;
                }
                else
                {
                    if (absS > absQ)
                    {
                        //s,q,r
                        flip = diff.s < 0;
                        longAxes = new LogicHex(-1, 0, 1);
                        shortAxes = new LogicHex(0, -1, 1);
                        longLength = absQ;
                        shortLength = absR;
                    }
                    else
                    {
                        //q,s,r
                        flip = diff.q < 0;
                        longAxes = new LogicHex(1, 0, -1);
                        shortAxes = new LogicHex(1, -1, 0);
                        longLength = absS;
                        shortLength = absR;
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
                        longAxes = new LogicHex(0, 1, -1);
                        shortAxes = new LogicHex(-1, 1, 0);
                        longLength = absS;
                        shortLength = absQ;
                    }
                    else
                    {
                        //r,q,s
                        flip = diff.r < 0;
                        longAxes = new LogicHex(-1, 1, 0);
                        shortAxes = new LogicHex(0, 1, -1);
                        longLength = absQ;
                        shortLength = absS;
                    }
                }
                else
                {
                    //s,r,q
                    flip = diff.s < 0;
                    longAxes = new LogicHex(0, -1 , 1);
                    shortAxes = new LogicHex(-1, 0, 1);
                    longLength = absR;
                    shortLength = absQ;
                }
            }

            if (flip)
            {
                longAxes = longAxes.Mul(-1);
                shortAxes = shortAxes.Mul(-1);
            }


            int dx = longLength;
            int dy = shortLength;


            LogicHex pos = new LogicHex(a);
            /*int D = 2 * dy - dx;

            for (int i = 0; i<distance; ++i)
            {
                
                if (D > 0)
                {
                    //pos = pos.Add(shortAxes);
                    D = D - 2 * dx;
                }
                //pos = pos.Add(longAxes);
                D = D + 2 * dy;
                

                map.Set(pos, value);
            }*/
            /*
 
plotLine(x0,y0, x1,y1)
  dx = x1 - x0
  dy = y1 - y0
  D = 2*dy - dx
  y = y0

  for x from x0 to x1
    plot(x,y)
    if D > 0
       y = y + 1
       D = D - 2*dx
    end if
    D = D + 2*dy

    */


            for (int i = 0; i < longLength; ++i)
            {
                pos = pos.Add(longAxes);
                map.Set(pos, value);
            }

            for (int i = 0; i < shortLength; ++i)
            {
                pos = pos.Add(shortAxes);
                map.Set(pos, value);
            }



        }




    }




}
