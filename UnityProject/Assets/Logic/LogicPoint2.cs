
using logic.math;

namespace logic.util
{

	public class LogicPoint2
	{

        public static readonly LogicPoint2 zero = new LogicPoint2(0, 0);

        public int x;
        public int y;

        /*public LogicPoint2()
        {
            x = 0;
            y = 0;
        }*/

        public LogicPoint2(int x, int y)
		{
            this.x = x;
            this.y = y;
		}

        public void Set(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public LogicPoint2(LogicPoint2 other)
        {
            x = other.x;
            y = other.y;
        }

        public LogicPoint2 Mul(int val)
        {
            return new LogicPoint2(x * val, y * val);
        }

        public LogicPoint2 Div(int val)
        {
            return new LogicPoint2(x / val, y / val);
        }

        public LogicPoint2 Add(LogicPoint2 other)
        {
            return new LogicPoint2(x + other.x, y + other.y);
        }

        public LogicPoint2 Sub(LogicPoint2 other)
        {
            return new LogicPoint2(x - other.x, y - other.y);
        }

        public LogicPoint2 Mul(LogicPoint2 other)
        {
            return new LogicPoint2(x * other.x, y * other.y);
        }

        public LogicPoint2 Div(LogicPoint2 other)
        {
            return new LogicPoint2(x / other.x, y / other.y);
        }

        public LogicPoint2 Cross(LogicPoint2 other)
        {
            return new LogicPoint2(x * other.y, y * other.x);
        }

        public LogicPoint2 Sign()
        {
            int signX = 0;
            if (x > 0)
                signX = 1;
            if (x < 0)
                signX = -1;

            int signY = 0;
            if (y > 0)
                signY = 1;
            if (y < 0)
                signY = -1;

            return new LogicPoint2(signX,signY);
        }

        public LogicPoint2 Abs()
        {
            int xAbs = x;
            if (x < 0)
                xAbs = -x;

            int yAbs = y;
            if (y < 0)
                yAbs = -y;

            return new LogicPoint2(xAbs, yAbs);
        }

        public bool Equals(LogicPoint2 other)
        {
            return other.x == x && other.y == y;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is LogicPoint2)) return false;

            return Equals((LogicPoint2)obj);
        }

        public override int GetHashCode()
        {
            return x ^ y;
        }

        public override string ToString()
        {
            return "["+x+","+y+"]";
        }
    }

}
