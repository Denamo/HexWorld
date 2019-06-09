
public class LogicVector2
{
    public int x;
    public int y;

    public LogicVector2(LogicVector2 vector)
    {
        x = vector.x;
        y = vector.y;
    }
    public LogicVector2(int x, int y)
	{
		this.x = x;
		this.y = y;
	}
	public LogicVector2()
	{
        x = 0;
        y = 0;
    }

    public int getLength()
	{
		return LogicMath.sqrt(getLengthSquared());
	}

    public int getLengthSquared()
	{
		return getDistanceSquaredHelper(0, 0);
	}

    public bool isEqual(LogicVector2 pVector2)
	{
		return (pVector2 != null && x == pVector2.x && y == pVector2.y);
	}

    public void set(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

	public void add(LogicVector2 pVector2)
	{
		x += pVector2.x;
		y += pVector2.y;
	}
	public void subtract(LogicVector2 pVector2)
	{
		x -= pVector2.x;
		y -= pVector2.y;
	}
	public void multiply(LogicVector2 pVector2)
	{
		x *= pVector2.x;
		y *= pVector2.y;
	}

	public int getDistance(LogicVector2 pVector2)
	{
		return LogicMath.sqrt(getDistanceSquared(pVector2));
	}

	// Normalizes to the given length
	public void normalize(int length)
	{
		int l = getLength();

		if (l != 0)
		{
			x *= length;
			x /= l;
			y *= length;
			y /= l;
		}
	}

	// Returns angle from 0 to 359.
	// (1, 0) = 0, (0, 1) = 90, (-1, 0) = 180, (0, -1) = 270 		
	public int getAngle()
	{
		return LogicMath.getAngle(x, y);
	}

	// Returns angle from 0 to 180
	public int getAngleBetween(int x, int y)
	{
		return LogicMath.getAngleBetween(getAngle(), LogicMath.getAngle(x, y));
	}
	public void rotate(int angle)
	{
		int newX = LogicMath.getRotatedX(x, y, angle);
		y = LogicMath.getRotatedY(x, y, angle);
		x = newX;
	}

	public LogicVector2 clone()
	{
		return new LogicVector2(x, y);
	}

	public bool isInDistanceXY(int targetX, int targetY, int maxDistance)
	{
		int diff = targetX - x;

		if (diff > maxDistance || diff < -maxDistance)
		{
			return false;
		}

		diff = targetY - y;

		if (diff > maxDistance || diff < -maxDistance)
		{
			return false;
		}

		return getDistanceSquaredHelper(targetX, targetY) <= (maxDistance * maxDistance);
	}

    public bool isInDistance(LogicVector2 pVector2, int maxDistance)
	{
		return isInDistanceXY(pVector2.x, pVector2.y, maxDistance);
	}

    public int getDistanceSquared(LogicVector2 pVector2)
	{
		return getDistanceSquaredHelper(pVector2.x, pVector2.y);
	}

    public int getDistanceSquaredTo(int x, int y)
	{
		return getDistanceSquaredHelper(x, y);
	}

	public bool isInArea(int areaX, int areaY, int areaWidth, int areaHeight)
	{
		return (x >= areaX && y >= areaY && x < (areaX + areaWidth) && y < (areaY + areaHeight));
	}

	public int dot(LogicVector2 pOther)
	{
		return x * pOther.x + y * pOther.y;
	}

    public int getDistanceSquaredHelper(int targetX, int targetY)
	{
		targetX -= x;
		if (targetX > 46340 || targetX < -46340)
		{
			// Avoid overflow
			return LogicMath.INT_MAX_VALUE;
		}

		targetY -= y;
		if (targetY > 46340 || targetY < -46340)
		{
			// Avoid overflow
			return LogicMath.INT_MAX_VALUE;
		}

		targetX *= targetX;
		targetX += targetY * targetY;

		if (targetX < 0)
		{
			// Overflow
			return LogicMath.INT_MAX_VALUE;
		}

		return targetX;
	}
/*
    public void encode(ChecksumEncoder pEncoder)
	{
		pEncoder.writeInt(x);
		pEncoder.writeInt(y);
	}
	public void decode(ByteStream pByteStream)
	{
		x = pByteStream.readInt();
		y = pByteStream.readInt();
	}

    //override Java Object functions to for example be able to use as hashtable key. Please do not change signature.
    public bool equals(object pValue)
	{
		if (pValue == null)
		{
			return false;
		}

		LogicVector2 pLogicVector2 = (LogicVector2)pValue;

		return (y == pLogicVector2.y) && (x == pLogicVector2.x);
	}

    public int hashCode()
	{
		int result = x;
		result = 31 * result + y;
		return result;
	}

    public String toString()
	{
		return String.format("LogicVector2(%d,%d)", x, y);
	}
*/

}



