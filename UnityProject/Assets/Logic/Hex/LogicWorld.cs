using logic.math;
using logic.util;


namespace game
{

    
	public class LogicWorld
	{
        LogicRandom random = new LogicRandom(0);

		public int foo = 0;
		public LogicHexMap map = new LogicHexMap(19, 19);

		const int HEIGHT_SCALE = 8;
			
		public void Start()
		{
			UpdateMap();
		}

		int seed = 0;
        public void SetSeed(int seed)
        {
            this.seed = seed;
        }

        public int GetSeed()
        {
            return seed;
        }

        public const int TILE_SIZE = 150;
        public const int HALF_TILE_SIZE = TILE_SIZE / 2;
        public const int HEXAGON_OFFSET = 130;

        public LogicPoint3 HexToLogicPos(LogicHex hex)
        {
            int tile = map.Get(hex.q,hex.r);
            int height = LogicTile.GetHeight(tile);
            ///int tile = LogicTile.SetTerrain(0, TerrainType.Grass);

            return HexToLogicPos(hex, height);
        }

        public static LogicPoint3 HexToLogicPos(LogicHex hex, int height)
        {
            LogicPoint3 p = new LogicPoint3();
            p.x = (hex.r * TILE_SIZE) + (hex.s * HALF_TILE_SIZE);
            p.y = height * HEIGHT_SCALE;
            p.z = -hex.s * HEXAGON_OFFSET;

            return p;
        }
        
        public static LogicHex LogicToHexPos(LogicPoint3 point)
        {
            int s = -point.z / HEXAGON_OFFSET;
            int r = (point.x - (s * HALF_TILE_SIZE)) / TILE_SIZE;

            return new LogicHex(-s - r, r, s);
        }

        LogicHex touch1 = new LogicHex(9, 9);
        LogicHex touch2 = new LogicHex(9, 9);
        public void TouchHex(LogicHex hex)
        {
            //tiles.Clear();

            int tile = LogicTile.SetTerrain(0, TerrainType.Sand);

            //LogicHex origo = new LogicHex(9, 9);
            //tiles.Set(origo, tile);

            touch1 = hex;

            UpdateMap();

            //map.Set(hex, tile);
        }

        public void Touch2Hex(LogicHex hex)
        {
            //tiles.Clear();

            int tile = LogicTile.SetTerrain(0, TerrainType.Rock);

            //LogicHex origo = new LogicHex(9, 9);
            //tiles.Set(origo, tile);
            touch2 = hex;

            UpdateMap();

            //map.Set(hex, tile);
        }

        public void UpdateMap()
		{
            random.setIteratedRandomSeed(seed);

            map.Clear();

            int tile = LogicTile.SetTerrain(0, TerrainType.Sand);

            LogicHexUtils.Line(map, touch1, touch2, tile);

            Noise(map, seed);

            /*
            HexGuideGrid();
            GenerateRooms();

            int tile = LogicTile.SetTerrain(0, TerrainType.Sand);
            LogicHex origo = new LogicHex(9,9);

            tiles.Set(origo, tile);

            /*LogicHex a = LogicHex.northWest.Multiply(3);
            for(int i = 0; i < seed; ++i)
            {
                a = a.RotateRight();
            }

            tiles.Set(origo.Add(a), tile);*/

            //tiles.Set(origo.Add(LogicHex.ClockDirection(seed)), tile);

        }




        public void HexGuideGrid()
        {
            int tile = LogicTile.SetTerrain(0, TerrainType.Grass);

            for (int r = 0; r < map.height; ++r)
            {
                for (int q = 0; q < map.width; ++q)
                {
                    if (q % 2 == 0 && r % 2 == 0)
                        map.Set(q, r, tile);
                }
            }
        }


        public static void Noise(LogicHexMap tiles, int seed)
        {
            int i = 0;
            for (int r = 0; r < tiles.height; ++r)
            {
                for (int q = 0; q < tiles.width; ++q)
                {
                    int tile = tiles.Get(q, r);
                    LogicPoint3 pos = HexToLogicPos(new LogicHex(q,r), LogicTile.GetHeight(tile) * 8);


                    int noise = 0;
                    //Two octaves
                    noise += LogicPerlinNoise.Noise((pos.x << 6) + seed, (pos.z << 6) + seed, seed);
                    noise += LogicPerlinNoise.Noise((pos.x << 5) + seed, (pos.z << 5) + seed, seed);
                    noise /= 2;

                    int height = ScaleNoise(noise, LogicTile.MAX_HEIGHT);
                    tile = LogicTile.SetHeight(tile, height);

                    int room = LogicTile.GetRoom(tile);

                    /*if (room == 0)
                        tile = LogicTile.SetTerrain(tile, TerrainType.Grass);
                        */

                    /*if (height > 167)
						tile = LogicTile.SetTerrain(tile, TerrainType.Rock);
                    else if (height > 127)
                        tile = LogicTile.SetTerrain(tile, TerrainType.Sand);
                    else if (height > 87)
                        tile = LogicTile.SetTerrain(tile, TerrainType.Dirt);
                    else*/
                    //tile = LogicTile.SetTerrain(tile, TerrainType.Grass);

                    tiles.Set(q, r, tile);
                    ++i;
                }
            }
        }


        public void GenerateRooms()
        {

            //List<LogicPoint2> nodes = new List<LogicPoint2>();

            //int roomCount = 10;
            //int roomMinRadius = 2;
            //int roomMaxRadius = 2;


            //LogicPoint2 start = new LogicPoint2(tiles.width / 4, tiles.height / 4);
            //LogicPoint2 end = new LogicPoint2(tiles.width - tiles.width / 4, tiles.height - tiles.height / 4);
            //LogicPoint2 start = new LogicPoint2(9, 9);
            //LogicPoint2 end = new LogicPoint2(0, 0);
            //int tile = LogicTile.SetTerrain(0,TerrainType.Dirt);

            //LogicHex.Line(tiles, end, start, tile);
            /*
            end = start.Add(LogicHex.northEast.Mul(6).Add(LogicHex.east));
            LogicHex.Line(tiles, end, start, tile);

            end = start.Add(LogicHex.east.Mul(6).Add(LogicHex.southEast));
            LogicHex.Line(tiles, end, start, tile);

            end = start.Add(LogicHex.southEast.Mul(6).Add(LogicHex.southWest));
            LogicHex.Line(tiles, end, start, tile);

            end = start.Add(LogicHex.southWest.Mul(6).Add(LogicHex.west));
            LogicHex.Line(tiles, end, start, tile);

            end = start.Add(LogicHex.west.Mul(6).Add(LogicHex.northWest));
            LogicHex.Line(tiles, end, start, tile);
            
            end = start.Add(LogicHex.northWest.Mul(6).Add(LogicHex.northEast));
            LogicHex.Line(tiles, end, start, tile);
            */
            //tile = LogicTile.SetTerrain(0, TerrainType.Sand);
            /*
            end = start.Add(LogicHex.northEast.Mul(6).Add(LogicHex.east));
            LogicHex.Line(tiles, start, end, tile);

            end = start.Add(LogicHex.east.Mul(6).Add(LogicHex.southEast));
            LogicHex.Line(tiles, start, end, tile);

            end = start.Add(LogicHex.southEast.Mul(6).Add(LogicHex.southWest));
            LogicHex.Line(tiles, start, end, tile);

            end = start.Add(LogicHex.southWest.Mul(6).Add(LogicHex.west));
            LogicHex.Line(tiles, start, end, tile);*/

            /*end = start.Add(LogicHex.west.Mul(6).Add(LogicHex.northWest));
            LogicHex.Line(tiles, start, end, tile);
            */

            //end = start.Add(LogicHex.northWest.Mul(6).Add(LogicHex.northEast));
            //LogicHex.Line(tiles, start, end, tile);
            


            //LogicHex.Line(tiles, end, start, tile);



            /*

            for (int i=0;i<roomCount;++i)
            {

                int radius = roomMinRadius + random.rand((roomMaxRadius-roomMinRadius)+1);

                int width = radius * 2 - 1;
                int height = radius * 2 - 1;

                int x = random.rand(tiles.width - width);
                int y = random.rand(tiles.height - height);

                int avgHeight = GetAverageHeight(tiles, x, y, width, height);

                LogicIntArray2 room = CreateRoom(width, height, avgHeight);

                nodes.Add(new LogicPoint2(x,y));

                LogicArray2Utils.Blit(room, tiles, x, y);

                
            }*/





        }

        public static int GetAverageHeight(LogicIntArray2 tiles, int x0, int y0, int width, int height)
        {
            int sum = 0;
            int count = 0;
            for (int y = y0; y < tiles.height && y < y0+height; ++y)
            { 
                for (int x = x0; x < tiles.width && x < x0+width; ++x)
                {
                    ++count;
                    sum += LogicTile.GetHeight(tiles.Get(x, y));
                }
            }
            if (count == 0)
                return 0;
            return sum / count;
        }

        public LogicIntArray2 CreateRoom(int width, int height, int tileHeight)
        {

            int tile = 0;
            tile = LogicTile.SetTerrain(tile, TerrainType.Dirt);
            tile = LogicTile.SetRoom(tile, 1);
            tile = LogicTile.SetHeight(tile, tileHeight);

            LogicIntArray2 room = new LogicIntArray2(width, height, tile);

            /*
            for (int y = 0; y < width; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    room.Set(x,y,tile);
                }
            }*/

            return room;
        }


        static int ScaleNoise(int noise, int max)
		{
			int k = (LogicPerlinNoise.SCALE*2) / max;

			return (noise + LogicPerlinNoise.SCALE) / k;
		}

	}
    
}
