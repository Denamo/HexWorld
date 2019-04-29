using game;
using logic.debug;
using logic.math;

namespace logic.util
{
    
	public struct LogicHex
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
            //Debugger.DoAssert(other != null, "Other is null!");

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
        
        private static int PMod(int value, int modulo)
        {
            value = value % modulo;
            if (value < 0)
                return value + modulo;
            return value;
        }


    };

}
