using logic.debug;

namespace game
{

	public enum TerrainType
	{
		Void,
		Rock,
		Sand,
		Dirt,
		Grass,
		Forest,
		Count
	}

	public static class LogicTile
	{
		//32 bit configuration mapping

		const int BITS_TERRAIN = 0x000000FF;
		const int BITS_HEIGHT = 0x0000FF00;
        const int BITS_ROOM = 0x00FF0000;
        const int BIT_EXPLORED = 0x10000000;
        const int BIT_WALKABLE = 0x20000000;

        const int BITS_SHIFT_TERRAIN = 0;
        const int BITS_SHIFT_HEIGHT = 8;
        const int BITS_SHIFT_ROOM = 16;

		public const int MAX_TERRAIN = 255;
        public const int MAX_HEIGHT = 255;
        public const int MAX_ROOM = 255;

        public static int SetTerrain(int tile, TerrainType terrain)
		{
			int val = (int)terrain;
			Assert(val >= 0 && val <= MAX_TERRAIN);
			return (tile & ~BITS_TERRAIN) | ((val << BITS_SHIFT_TERRAIN) & BITS_TERRAIN);
		}
		public static TerrainType GetTerrain(int tile)
		{
			return (TerrainType)((tile & BITS_TERRAIN) >> BITS_SHIFT_TERRAIN);
		}

        public static int SetHeight(int tile, int val)
		{
			Assert(val >= 0 && val <= MAX_HEIGHT);
			return (tile & ~BITS_HEIGHT) | ((val << BITS_SHIFT_HEIGHT) & BITS_HEIGHT);
		}
		public static int GetHeight(int tile)
		{
			return (int)((tile & BITS_HEIGHT) >> BITS_SHIFT_HEIGHT);
		}

        public static int SetRoom(int tile, int val)
        {
            Assert(val >= 0 && val <= MAX_ROOM);
            return (tile & ~BITS_ROOM) | ((val << BITS_SHIFT_ROOM) & BITS_ROOM);
        }
        public static int GetRoom(int tile)
        {
            return (int)((tile & BITS_ROOM) >> BITS_SHIFT_ROOM);
        }

        public static int SetExplored(int tile, bool val)
		{
			if (val)
				return (tile | BIT_EXPLORED);
			else
				return (tile & ~BIT_EXPLORED);
		}
		public static bool IsExplored(int tile)
		{
			return (tile & BIT_EXPLORED) != 0;
		}

		public static void DebugPrintBits(int tile)
		{
			string bits = "";
			int num = tile;
			for (int bit = 0; bit < 32; bit++)
			{
				bits += string.Format("%d", num & 0x01);
				num = num >> 1;
			}

			Debugger.print(bits);
		}

		public static void DebugUnitTest()
		{
			int tile = 0;

			//Setting
			tile = SetTerrain(tile, TerrainType.Rock);
			Assert(GetTerrain(tile) == TerrainType.Rock);

			tile = SetHeight(tile, MAX_HEIGHT);
			Assert(GetHeight(tile) == MAX_HEIGHT);

			tile = SetExplored(tile, true);
			Assert(IsExplored(tile) == true);

			//resetting
			tile = SetTerrain(tile, TerrainType.Void);
			Assert(GetTerrain(tile) == TerrainType.Void);

			tile = SetHeight(tile, 0);
			Assert(GetHeight(tile) == 0);

			tile = SetExplored(tile, false);
			Assert(IsExplored(tile) == false);

			Assert(tile == 0);
		}

		public static void DebugPrint(int data)
		{
			Debugger.print(string.Format("Terrain:%d Height:%d Explored:%d", GetTerrain(data), GetHeight(data), IsExplored(data) ? 1 : 0));
		}

		static void Assert(bool test)
		{
			if (!test)
				Debugger.error("Tile assert failed!");
		}


	};



}
