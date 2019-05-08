
using logic.debug;
using System.Collections.Generic;

namespace logic.util
{

	public class LogicIntArray2 : LogicArray2<int>
	{
		public LogicIntArray2(int width, int height) : this(width, height, 0)
		{
		}

		public LogicIntArray2(int width, int height, int defaultValue) : base(width, height, defaultValue)
		{
		}
	}

	public class LogicArray2<T>
	{

		public enum WrapMethod
		{
			Error,  //Will give error if index is out of bouds
			Default,//Will ignore out of bounds and return default value
			Clamp,  //Clamp to min max index
			Repeat //Will wrap around
				   //Mirror  //TODO
		}

		public List<T> array;
		public int width;
		public int height;
		public WrapMethod wrapMethod;
		public T defaultValue;

		public LogicArray2(int width, int height, T defaultValue)
		{
			this.width = width;
			this.height = height;
			array = new List<T>(width * height);
			wrapMethod = WrapMethod.Error;
			this.defaultValue = defaultValue;

			Init(width, height);
		}

		private void Init(int width, int height)
		{
			this.width = width;
			this.height = height;

			int size = width * height;
			array.Clear();
			for (int i = 0; i < size; ++i)
				array.Add(defaultValue);
		}

		public void Resize(int width, int height)
		{
			WrapMethod originalWrapMethod = wrapMethod;
			if (wrapMethod == WrapMethod.Error)
				wrapMethod = WrapMethod.Default;

			int newWidth = width;
			int newHeight = height;
			List<T> newArray = new List<T>(width*height);
			for (int y = 0; y < height; ++y)
			{
				for (int x = 0; x < width; ++x)
				{
					newArray.Add(Get(x,y));
				}
			}
			this.array = newArray;
			this.width = width;
			this.height = height;

			wrapMethod = originalWrapMethod;
		}

		public void Clear()
		{
			Clear(defaultValue);
		}

		public void Clear(T value)
		{
			int size = width * height;
			for (int i = 0; i < size; ++i)
				array[i] = value;
		}

		public bool IsInside(int x, int y)
		{
			return x >= 0 && x < width && y >= 0 && y < height;
		}

		public virtual T Get(int x, int y)
		{
			if (x < 0 || x >= width)
			{
				switch (wrapMethod)
				{
					case WrapMethod.Repeat:
						x = PMod(x, width);
						break;
					case WrapMethod.Default:
						return defaultValue;
					case WrapMethod.Clamp:
						x = Clamp(x, 0, width - 1);
						break;
					case WrapMethod.Error:
					default:
						Debugger.error(string.Format("LogicArray2::get({0},{1}) x out of bounds!", x, y));
						return defaultValue;
				}
			}

			if (y < 0 || y >= height)
			{
				switch (wrapMethod)
				{
					case WrapMethod.Repeat:
						y = PMod(y, height);
						break;
					case WrapMethod.Default:
						return defaultValue;
					case WrapMethod.Clamp:
						y = Clamp(y, 0, height - 1);
						break;
					case WrapMethod.Error:
					default:
						Debugger.error(string.Format("LogicArray2::get({0},{1}) y out of bounds!", x, y));
						return defaultValue;
				}
			}

			return array[x + y * width];
		}

		public virtual void Set(int x, int y, T value)
		{
			if (x < 0 || x >= width)
			{
				switch (wrapMethod)
				{
					case WrapMethod.Repeat:
						x = PMod(x, width);
						break;
					case WrapMethod.Default:
						return;
					case WrapMethod.Clamp:
						x = Clamp(x, 0, width - 1);
						break;
					case WrapMethod.Error:
					default:
						Debugger.error(string.Format("LogicArray2::set({0},{1}) x out of bounds!", x, y));
						return;
				}
			}

			if (y < 0 || y >= height)
			{
				switch (wrapMethod)
				{
					case WrapMethod.Repeat:
						y = PMod(y, height);
						break;
					case WrapMethod.Default:
						return;
					case WrapMethod.Clamp:
						y = Clamp(y, 0, height - 1);
						break;
					case WrapMethod.Error:
					default:
						Debugger.error(string.Format("LogicArray2::set({0},{1}) y out of bounds!", x, y));
						return;
				}
			}

			array[x + y * width] = value;
		}

        private static int PMod(int value, int modulo)
		{
			value = value % modulo;
			if (value < 0)
				return value + modulo;
			return value;
		}

		private static int Clamp(int value, int min, int max)
		{
			if (value < min)
				return min;

			if (value > max)
				return max;

			return value;
		}


	}


}
