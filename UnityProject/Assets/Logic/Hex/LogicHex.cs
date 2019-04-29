using game;
using logic.debug;
using logic.math;

namespace logic.util
{
    
	public class LogicHex
    {

        
        public readonly int q;
        public readonly int r;
        public readonly int s;

        public LogicHex(int q, int r) : this(q,r,-q-r)
        {
        }

        public LogicHex(int q, int r, int s)
        {
            this.q = q;
            this.r = r;
            this.s = s;

            Debugger.DoAssert(q + r + s == 0, "Sum must be zero!");
        }

        public LogicHex(LogicHex other)
        {
            Debugger.DoAssert(other != null, "Other is null!");

            q = other.q;
            r = other.r;
            s = other.s;

            Debugger.DoAssert(q + r + s == 0, "Sum must be zero!");
        }

        public LogicHex Clone()
        {
            return new LogicHex(q,r,s);
        }

        public LogicHex Add(LogicHex b)
        {
            return new LogicHex(q + b.q, r + b.r, s + b.s);
        }
        
        public LogicHex Sub(LogicHex b)
        {
            return new LogicHex(q - b.q, r - b.r, s - b.s);
        }

        public LogicHex Mul(int k)
        {
            return new LogicHex(q * k, r * k, s * k);
        }

        public LogicHex Div(int k)
        {
            return new LogicHex(q / k, r / k, s / k);
        }
        
        public LogicHex RotateLeft()
        {
            return new LogicHex(-s, -q, -r);
        }

        public LogicHex RotateRight()
        {
            return new LogicHex(-r, -s, -q);
        }


        public static readonly LogicHex northWest = new LogicHex(1, 0);
        public static readonly LogicHex northEast = new LogicHex(0, 1);
        public static readonly LogicHex east = new LogicHex(-1, 1);
        public static readonly LogicHex southEast = new LogicHex(-1, 0);
        public static readonly LogicHex southWest = new LogicHex(0, -1);
        public static readonly LogicHex west = new LogicHex(1, -1);

        public static LogicHex[] directions = { northWest, northEast, east, southEast, southWest, west };

        public static LogicHex Direction(int direction)
        {
            return directions[PMod(direction,6)];
        }

        public LogicHex Neighbor(int direction)
        {
            return Add(Direction(direction));
        }

        public static LogicHex[] clockDirections = {
            new LogicHex(1,1),
            new LogicHex(0,2),
            new LogicHex(-1,2),
            new LogicHex(-2,2),
            new LogicHex(-2,1),
            new LogicHex(-2,0),
            new LogicHex(-1,-1),
            new LogicHex(0,-2),
            new LogicHex(1,-2),
            new LogicHex(2,-2),
            new LogicHex(2,-1),
            new LogicHex(2,0)
        };

        //Length is allways two.
        public static LogicHex ClockDirection(int hour)
        {
            return clockDirections[PMod(hour,12)];
        }

        public int Length()
        {
            return (LogicMath.abs(q) + LogicMath.abs(r) + LogicMath.abs(s)) / 2;
        }

        public static int Distance(LogicHex a, LogicHex b)
        {
            return b.Sub(a).Length();
        }

        public bool Equals(LogicHex other)
        {
            return other.q == q && other.r==r && other.s==s;
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !GetType().Equals(obj.GetType()))
                return false;

            return Equals((LogicHex)obj);
        }

        public override int GetHashCode()
        {
            return q ^ r << 4 ^ s << 8;
        }

        public override string ToString()
        {
            return string.Format("[{0},{1},{2}]",q,r,s);
        }

        /*public static void Line(LogicIntArray2 tiles, LogicPoint2 start, LogicPoint2 end, int paint)
        {
            LogicPoint2 pos = new LogicPoint2(start);

            string debug = "";

            LogicPoint2 diff = end.Sub(start);
            LogicPoint2 sign = diff.Sign();
            LogicPoint2 abs = diff.Abs();
            LogicPoint2 sum = new LogicPoint2(0,0);



            }



            return;
        }*/



        private static int PMod(int value, int modulo)
        {
            value = value % modulo;
            if (value < 0)
                return value + modulo;
            return value;
        }


    };



}


/*

if (dist.x > 0)
{
    if (dist.y > 0)
    {
        if (dist.x < dist.y)
            pos = pos.Add(northEast);
        else
            pos = pos.Add(east);
    }
    else if (dist.y < 0)
    {
        if (dist.x > dist.y)
            pos = pos.Add(southEast);
        else
            pos = pos.Add(east);
    }
    else
    {
        pos = pos.Add(east);
    }
}
else if (dist.x < 0)
{
    if (dist.y > 0)
    {
        if (dist.x < dist.y)
            pos = pos.Add(northWest);
        else
            pos = pos.Add(west);
    }
    else if (dist.y < 0)
    {
        if (dist.x > dist.y)
            pos = pos.Add(southWest);
        else
            pos = pos.Add(west);
    }
    else
    {
        pos = pos.Add(west);
    }
}
else
{
    if (dist.y > 0)
        pos = pos.Add(northEast);
    else
        pos = pos.Add(southWest);
}

 */


    /*
        public static void LineLow(LogicIntArray2 tiles, LogicPoint2 start, LogicPoint2 end, int paint)
        {
            LogicPoint2 d = end.Sub(start);

            LogicPoint2 inc = new LogicPoint2(1, 1);

            if (d.y < 0) {
                inc.y = -1;
                d.y = -d.y;
            }
            int D = 2*d.y - d.x;

            int y = start.y;
            for(int x = start.x; x<end.x; ++x)
            {
                tiles.Set(x,y,paint);
                if (D > 0)
                {
                    y += inc.y;
                    D = D - 2 * d.x;
                }
                D = D + 2 * d.y;
            }
      
        }

        public static void LineHigh(LogicIntArray2 tiles, LogicPoint2 start, LogicPoint2 end, int paint)
        {
            LogicPoint2 d = end.Sub(start);

            LogicPoint2 inc = new LogicPoint2(1, 1);

            if (d.x < 0)
            {
                inc.x = -1;
                d.y = -d.x;
            }
            int D = 2 * d.x - d.y;

            int x = start.x;
            for (int y = start.y; y < end.y; ++y)
            {
                tiles.Set(x, y, paint);
                if (D > 0)
                {
                    x += inc.x;
                    D = D - 2 * d.y;
                }
                D = D + 2 * d.x;
            }
        }

        public static void Line2(LogicIntArray2 tiles, LogicPoint2 start, LogicPoint2 end, int paint)
        {
            LogicPoint2 diff = end.Sub(start);
            LogicPoint2 abs = diff.Abs();

            if(abs.y < abs.x)
            {
                if (start.x > end.x)
                    LineLow(tiles, end, start, paint);
                else
                    LineLow(tiles, start, end, paint);
            }
            else
            {
                if (start.x > end.x)
                    LineHigh(tiles, end, start, paint);
                else
                    LineHigh(tiles, start, end, paint);
            }
        }
        */