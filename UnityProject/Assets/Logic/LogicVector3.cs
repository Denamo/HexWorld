using System;

class LogicVector3
{
    public int x, y, z;

    public LogicVector3(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public LogicVector3(LogicVector3 v)
    {
        x = v.x;
        y = v.y;
        z = v.z;
    }

    public int getLength()
    {
        return LogicMath.sqrt(x * x + y * y + z * z);
    }

    public void normalize()
    {
        int l = (1 / LogicMath.sqrt(x * x + y * y + z * z));
        x *= l;
        y *= l;
        z *= l;
    }

    public void add(LogicVector3 v)
    {
        x += v.x;
        y += v.y;
        z += v.z;
    }

    public static LogicVector3 add(LogicVector3 v1, LogicVector3 v2)
    {
        return new LogicVector3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
    }

    public void subtract(LogicVector3 v)
    {
        x -= v.x;
        y -= v.y;
        z -= v.z;
    }

    public static LogicVector3 subtract(LogicVector3 v1, LogicVector3 v2)
    {
        return new LogicVector3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
    }

    public void scale(int s)
    {
        x *= s;
        y *= s;
        z *= s;
    }

    public static LogicVector3 scale(LogicVector3 v, int s)
    {
        return new LogicVector3(s * v.x, s * v.y, s * v.z);
    }

    public int dot(LogicVector3 v)
    {
        return x * v.x + y * v.y + z * v.z;
    }

    public static int dot(LogicVector3 v1, LogicVector3 v2)
    {
        return v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;
    }

    public LogicVector3 cross(LogicVector3 v)
    {
        return new LogicVector3(y * v.z - z * v.y, z * v.x - x * v.z, x * v.y - y * v.x);
    }

    public static LogicVector3 cross(LogicVector3 v1, LogicVector3 v2)
    {
        return new LogicVector3(v1.y * v2.z - v1.z * v2.y, v1.z * v2.x - v1.x * v2.z, v1.x * v2.y - v1.y * v2.x);
    }

}