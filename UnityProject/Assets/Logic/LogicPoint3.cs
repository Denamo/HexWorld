
namespace logic.util
{

    public class LogicPoint3
    {

        public int x;
        public int y;
        public int z;

        public LogicPoint3()
        {
            x = 0;
            y = 0;
            z = 0;
        }

        public LogicPoint3(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public void Set(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }


        public override bool Equals(object obj)
        {
            if (!(obj is LogicPoint3)) return false;

            LogicPoint3 casted = (LogicPoint3)obj;

            return casted.x == x && casted.y == y && casted.z == z;
        }

        public override int GetHashCode()
        {
            return x ^ y ^ z;
        }

        public override string ToString()
        {
            return "[" + x + "," + y + "," + z + "]";
        }
    }

}
