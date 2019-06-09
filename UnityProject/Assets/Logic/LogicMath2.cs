
public class LogicMath
{

	public const int FIXED_SHIFT = 10;
	public const int FIXED_ACCURACY = 1024;

	public static int max(int a, int b)
	{
		return (a > b) ? a : b;
	}
	public static int min(int a, int b)
	{
		return (a < b) ? a : b;
	}

    public static int clamp(int value, int low, int high)
	{
		if (value <= low)
		{
			return low;
		}
		if (value >= high)
		{
			return high;
		}
		return value;
	}

	/*
	 Does not support floating point values.
	 Use Utility::abs(float) if you are running client-side (non-logic) code!
	 */
	public static int abs(int value)
	{
		return ((value >= 0) ? value : -value);
	}

    public static int sign(int value)
    {
        if (value > 0)
            return 1;
        else if (value < 0)
            return -1;
        else
            return 0;
    }

    // Return value is always is between 0 and 359.
    public static int normalizeAngle360(int angle)
	{
		angle = (angle % 360);

		if (angle < 0)
		{
			return angle + 360;
		}

		return angle;
	}

	// Return value is always is between -180 and 179	 
	public static int normalizeAngle180(int angle)
	{
		angle = normalizeAngle360(angle);

		if (angle >= 180)
		{
			return angle - 360;
		}

		return angle;
	}

	// Return value is bit shifted with FIXED_SHIFT	 
	public static int sin(int angle)
	{
		angle = normalizeAngle360(angle);

		if (angle < 180)
		{
			if (angle > 90)
			{
				angle = 180 - angle;
			}

			return SIN_TABLE[angle];
		}

		angle -= 180;

		if (angle > 90)
		{
			angle = 180 - angle;
		}

		return -SIN_TABLE[angle];
	}

	// Return value is bit shifted with FIXED_SHIFT
	public static int cos(int angle)
	{
		return sin(angle + 90);
	}

	// Return value is in same accuracy as radius
	public static int sin(int angle, int radius)
	{
		return sin(angle) * radius / FIXED_ACCURACY;
	}
	public static int cos(int angle, int radius)
	{
		return cos(angle) * radius / FIXED_ACCURACY;
	}

	public static int sqrt(int x)
	{
		int xn;

		if (x >= 0x10000)
		{
			if (x >= 0x1000000)
			{
				if (x >= 0x10000000)
				{
					if (x >= 0x40000000)
					{
						if (x >= INT_MAX_VALUE)
						{
							return 65535;
						}

						xn = SQRT_TABLE[x >> 24] << 8;
					}
					else
					{
						xn = SQRT_TABLE[x >> 22] << 7;
					}
				}
				else
				{
					if (x >= 0x4000000)
					{
						xn = SQRT_TABLE[x >> 20] << 6;
					}
					else
					{
						xn = SQRT_TABLE[x >> 18] << 5;
					}
				}

				xn = (xn + 1 + (x / xn)) >> 1;
				xn = (xn + 1 + (x / xn)) >> 1;
				return ((xn * xn) > x) ?--xn : xn;
			}
			else
			{
				if (x >= 0x100000)
				{
					if (x >= 0x400000)
					{
						xn = SQRT_TABLE[x >> 16] << 4;
					}
					else
					{
						xn = SQRT_TABLE[x >> 14] << 3;
					}
				}
				else
				{
					if (x >= 0x40000)
					{
						xn = SQRT_TABLE[x >> 12] << 2;
					}
					else
					{
						xn = SQRT_TABLE[x >> 10] << 1;
					}
				}

				xn = (xn + 1 + (x / xn)) >> 1;

				return ((xn * xn) > x) ?--xn : xn;
			}
		}
		else
		{
			if (x >= 0x100)
			{
				if (x >= 0x1000)
				{
					if (x >= 0x4000)
					{
						xn = SQRT_TABLE[x >> 8] + 1;
					}
					else
					{
						xn = (SQRT_TABLE[x >> 6] >> 1) + 1;
					}
				}
				else
				{
					if (x >= 0x400)
					{
						xn = (SQRT_TABLE[x >> 4] >> 2) + 1;
					}
					else
					{
						xn = (SQRT_TABLE[x >> 2] >> 3) + 1;
					}
				}

				return ((xn * xn) > x) ?--xn : xn;
			}
			else
			{
				if (x >= 0)
				{
					return (SQRT_TABLE[x]) >> 4;
				}

				return -1;
			}
		}
	}

	// Returns value is between 0 and 359
	// (1, 0) = 0, (0, 1) = 90, (-1, 0) = 180, (0, -1) = 270
	public static int getAngle(int x, int y)
	{
		if (x == 0 && y == 0)
		{
			return 0;
		}
		if (x > 0 && y >= 0) // 0 - 90
		{
			if (y < x)
			{
				return ATAN_TABLE[((y << 7) / x)];
			}

			return -ATAN_TABLE[((x << 7) / y)] + 90;
		}

		int ax = abs(x);

		if (x <= 0 && y > 0) // 90 - 180
		{
			if (ax < y)
			{
				return ATAN_TABLE[((ax << 7) / y)] + 90;
			}

			return -ATAN_TABLE[((y << 7) / ax)] + 180;
		}

		int ay = abs(y);

		if (x < 0 && y <= 0) // 180 - 270
		{
			if (ay < ax)
			{
				return ATAN_TABLE[((ay << 7) / ax)] + 180;
			}

			if (ay == 0)
			{
				// Division by zero workaround
				return 0;
			}

			return -ATAN_TABLE[((ax << 7) / ay)] + 270;
		}
		else //if    (x >= 0 && y < 0) 270 - 360
		{
			if (ax < ay)
			{
				return ATAN_TABLE[((ax << 7) / ay)] + 270;
			}

			if (ax == 0)
			{
				// Division by zero workaround
				return 0;
			}

			// Make sure the return value is in between 0 and 359 (never 360).
			return normalizeAngle360(-ATAN_TABLE[((ay << 7) / ax)] + 360);
		}
	}

	public static int getRotatedX(int x, int y, int angle)
	{
		x = x * cos(angle) - y * sin(angle);

		return x >> FIXED_SHIFT;
	}
	public static int getRotatedY(int x, int y, int angle)
	{
		y = x * sin(angle) + y * cos(angle);

		return y >> FIXED_SHIFT;
	}

	// Returns 0-180
	public static int getAngleBetween(int angle1, int angle2)
	{
		return abs(normalizeAngle180(angle1 - angle2));
	}

	public static int pow(int @base, int exp)
	{
		int result = 1;

		while (exp != 0)
		{
			if ((exp & 0x1) != 0)
			{
				result *= @base;
			}
			exp >>= 1;
			@base *= @base;
		}

		return result;
	}

	public static int sqrtApproximate(int dx, int dy)
	{
		dx = (dx < 0) ? -dx : dx;
		dy = (dy < 0) ? -dy : dy;
		return LogicMath.max(dx, dy) + ((LogicMath.min(dx, dy) * 53) >> 7);
	}

	public const int INT_MAX_VALUE = 2147483647;
	public const int INT_MIN_VALUE = (-2147483647 - 1);

	private static readonly int[] SQRT_TABLE = {0, 16, 22, 27, 32, 35, 39, 42, 45, 48, 50, 53, 55, 57, 59, 61, 64, 65, 67, 69, 71, 73, 75, 76, 78, 80, 81, 83, 84, 86, 87, 89, 90, 91, 93, 94, 96, 97, 98, 99, 101, 102, 103, 104, 106, 107, 108, 109, 110, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 128, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 144, 145, 146, 147, 148, 149, 150, 150, 151, 152, 153, 154, 155, 155, 156, 157, 158, 159, 160, 160, 161, 162, 163, 163, 164, 165, 166, 167, 167, 168, 169, 170, 170, 171, 172, 173, 173, 174, 175, 176, 176, 177, 178, 178, 179, 180, 181, 181, 182, 183, 183, 184, 185, 185, 186, 187, 187, 188, 189, 189, 190, 191, 192, 192, 193, 193, 194, 195, 195, 196, 197, 197, 198, 199, 199, 200, 201, 201, 202, 203, 203, 204, 204, 205, 206, 206, 207, 208, 208, 209, 209, 210, 211, 211, 212, 212, 213, 214, 214, 215, 215, 216, 217, 217, 218, 218, 219, 219, 220, 221, 221, 222, 222, 223, 224, 224, 225, 225, 226, 226, 227, 227, 228, 229, 229, 230, 230, 231, 231, 232, 232, 233, 234, 234, 235, 235, 236, 236, 237, 237, 238, 238, 239, 240, 240, 241, 241, 242, 242, 243, 243, 244, 244, 245, 245, 246, 246, 247, 247, 248, 248, 249, 249, 250, 250, 251, 251, 252, 252, 253, 253, 254, 254, 255};
	private static readonly int[] SIN_TABLE = {0, 18, 36, 54, 71, 89, 107, 125, 143, 160, 178, 195, 213, 230, 248, 265, 282, 299, 316, 333, 350, 367, 384, 400, 416, 433, 449, 465, 481, 496, 512, 527, 543, 558, 573, 587, 602, 616, 630, 644, 658, 672, 685, 698, 711, 724, 737, 749, 761, 773, 784, 796, 807, 818, 828, 839, 849, 859, 868, 878, 887, 896, 904, 912, 920, 928, 935, 943, 949, 956, 962, 968, 974, 979, 984, 989, 994, 998, 1002, 1005, 1008, 1011, 1014, 1016, 1018, 1020, 1022, 1023, 1023, 1024, 1024};
	private static readonly int[] ATAN_TABLE = {0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 8, 9, 9, 10, 10, 11, 11, 11, 12, 12, 13, 13, 14, 14, 14, 15, 15, 16, 16, 17, 17, 17, 18, 18, 19, 19, 19, 20, 20, 21, 21, 21, 22, 22, 22, 23, 23, 24, 24, 24, 25, 25, 25, 26, 26, 27, 27, 27, 28, 28, 28, 29, 29, 29, 30, 30, 30, 31, 31, 31, 32, 32, 32, 33, 33, 33, 34, 34, 34, 35, 35, 35, 35, 36, 36, 36, 37, 37, 37, 37, 38, 38, 38, 39, 39, 39, 39, 40, 40, 40, 40, 41, 41, 41, 41, 42, 42, 42, 42, 43, 43, 43, 43, 44, 44, 44, 44, 45, 45, 45};

}







