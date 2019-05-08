using logic.debug;
using logic.math;
using logic.util;
using System.Collections.Generic;

namespace game
{

    
	public class LogicWorld
	{
        LogicRandom random = new LogicRandom(0);

		public int foo = 0;
		public LogicHexMap map = new LogicHexMap(64);

		const int ALTITUDE_SCALE = 16;
			
		public void Start()
		{
			UpdateMap();
		}

        int step = 10;
        public void StepForward()
        {
            ++step;
        }

        public void StepBackward()
        {
            --step;
            if (step < 1)
                step = 1;
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
            int height = LogicTile.GetAltitude(tile);
            ///int tile = LogicTile.SetTerrain(0, TerrainType.Grass);

            return HexToLogicPos(hex, height);
        }

        public static LogicPoint3 HexToLogicPos(LogicHex hex, int height)
        {
            LogicPoint3 p = new LogicPoint3();
            p.x = (hex.r * TILE_SIZE) + (hex.s * HALF_TILE_SIZE);
            p.y = height * ALTITUDE_SCALE;
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

        //static int min = int.MaxValue;
        //static int max = int.MinValue;

        public void UpdateMap()
		{
            int initTile = LogicTile.SetTerrain(0, TerrainType.Grass);
            initTile = LogicTile.SetAltitude(initTile, LogicTile.MAX_ALTITUDE / 2);
            map.Clear(initTile);

            GenerateMap();

            
           
            
            //AltitudeAbsNoise(map, 5, seed, 100);
            //AltitudeNoise(map, 5, seed, 40);
            //AltitudeNoise(map, 6, seed, 20);


            //Noise(map, seed);

            /*
            int gridSize = 8;
            int gridRowCount = 5;
            LogicHex center = new LogicHex(map.width/2,map.height/2);
            LogicHex start = center.Sub(new LogicHex(gridSize * gridRowCount / 2, gridSize * gridRowCount / 2));

            List<LogicHex> nodes = CreateGrid(gridSize, gridRowCount);
            OffsetNodes(nodes, start);
            RandomOffsetNodes(nodes, 7, seed);

            PlotNodes(map, nodes, 3, TerrainType.Sand);
            */
            //int min, max;
            //GetAltitudeRange(map, out min, out max);
            /*
            if (min < LogicWorld.min)
                LogicWorld.min = min;

            if (max > LogicWorld.max)
                LogicWorld.max = max;

            Debugger.Log("Altitude range 1 [" + LogicWorld.min + " - " + LogicWorld.max + "]");
            Debugger.Log("Altitude range 2 [" + LogicPerlinNoise.minNoise + " - " + LogicPerlinNoise.maxNoise + "]");
            */
            /*for(int i = -LogicPerlinNoise.SCALE; i < LogicPerlinNoise.SCALE; ++i)
            {
                int altitude = LogicPerlinNoise.ScaleNoise(i,LogicTile.MAX_ALTITUDE);

                if (altitude < min)
                    min = altitude;

                if (altitude > max)
                    max = altitude;

            }*/

            //Debugger.Log("Altitude range 3 [" + min + " - " + max + "]");

        }

        public void GenerateMap()
        {
            random.setIteratedRandomSeed(seed);

            GeneratedTile tile = new GeneratedTile();

            for (int r = 0; r < map.height; ++r)
            {
                for (int q = 0; q < map.width; ++q)
                {

                    //int tile = 0;
                    LogicPoint3 pos = HexToLogicPos(new LogicHex(q, r), 0);
                    GenerateTile(tile, pos.x, pos.z, seed, step);

                    map.Set(q, r, tile.GetLogicTile());
                }
            }

        }

        public static void GenerateTile(GeneratedTile tile, int x, int y, int seed, int step)
        {
            tile.terrain = TerrainType.Grass;
            tile.altitude = LogicTween.SCALE_HALF;

            if (step == 1) return;

            int noise1 = Noise(x, y, 3, seed, LogicTween.SCALE);
            int noise1b = Noise(x, y, 3, seed + 12345, LogicTween.SCALE);

            int noise2 = Noise(x, y, 5, seed, LogicTween.SCALE);
            int noise2b = Noise(x, y, 5, seed + 12345, LogicTween.SCALE);

            int noise3 = Noise(x, y, 6, seed, LogicTween.SCALE);
            int noise3b = Noise(x, y, 6, seed + 12345, LogicTween.SCALE);

            tile.altitude = noise1;

            if (step == 2) return;

            tile.altitude = Lerp(tile.altitude, noise2, 70);

            if (step == 3) return;

            tile.altitude = Lerp(tile.altitude, noise3, 10);

            if (step == 4) return;

            tile.altitude = Plateau(tile.altitude, LogicTween.SCALE_HALF);

            if (step == 5) return;

            if (tile.altitude < LogicTween.SCALE_HALF - 10)
                tile.altitude = Lerp(tile.altitude, tile.altitude / 2, 20);

            if (step == 6) return;

            const int errorMargin = 16;
            if (tile.altitude < LogicTween.SCALE_HALF - errorMargin)
            {
                tile.terrain = TerrainType.Sand;
            }
            else if(tile.altitude < LogicTween.SCALE_HALF + errorMargin)
            {
                tile.terrain = TerrainType.Dirt;
            }

            if (step == 7) return;

            tile.altitude = Lerp(tile.altitude, noise2b, 10);
        }

        public static int Noise(int x, int y, int octave, int seed, int scale)
        {
            return LogicPerlinNoise.Noise((x << octave) + seed, (y << octave) + seed, seed << 2, scale);
        }
        
        public static int FlipCut(int value, int cut)
        {
            value -= cut;
            if (value < 0)
                return -value;
            else
                return value;
        }


        public static int Plateau2(int value, int height)
        {

            int distance = (value - height);

            bool flip = false;
            if (distance < 0)
            {
                distance = -distance;
                flip = true;
            }

            distance = LogicTween.QuadIn(distance * 2) / 2;
            
            if (flip)
            {
                distance = -distance;
            }

            return height + distance;
        }

        public static int Plateau(int value, int height)
        {

            int distance = (value - height);

            bool flip = false;
            if (distance < 0)
            {
                distance = -distance;
                flip = true;
            }

            if(distance < height/10)
                distance = LogicTween.QuadInOut(distance * 2) / 2;

            if (flip)
            {
                distance = -distance;
            }

            return height + distance;
        }




        public static void GetAltitudeRange(LogicHexMap map, out int min, out int max)
        {
            max = int.MinValue;
            min = int.MaxValue;
            for (int r = 0; r < map.height; ++r)
            {
                for (int q = 0; q < map.width; ++q)
                {
                    int tile = map.Get(q, r);
                    int altitude = LogicTile.GetAltitude(tile);

                    if (altitude < min)
                        min = altitude;

                    if (altitude > max)
                        max = altitude;
                }
            }
        }


        public static List<LogicHex> CreateGrid(int gridSize, int gridRowCount)
        {
            List<LogicHex> nodes = new List<LogicHex>(gridRowCount * gridRowCount);

            for (int r = 0; r < gridRowCount; ++r)
            {
                for (int q = 0; q < gridRowCount; ++q)
                {
                    nodes.Add(new LogicHex(q * gridSize, r * gridSize));
                }
            }

            return nodes;
        }

        public static void OffsetNodes(List<LogicHex> nodes, LogicHex offset)
        {
            for (int i = 0; i < nodes.Count; ++i)
            {
                nodes[i] = nodes[i].Add(offset);
            }
        }

        public static void RandomOffsetNodes(List<LogicHex> nodes, int maxDistance, int seed)
        {
            for (int i = 0; i < nodes.Count; ++i)
            {
                LogicHex node = nodes[i];

                int qNoise = LogicPerlinNoise.Noise((node.q << 12) + seed, (node.s << 15) + seed, seed * 7, maxDistance);
                int rNoise = LogicPerlinNoise.Noise((node.q << 14) + seed, (node.s << 13) + seed, seed * 10, maxDistance);

                nodes[i] = nodes[i].Add(new LogicHex(qNoise, rNoise));
            }
        }

        public static void PlotNodes(LogicHexMap map, List<LogicHex> nodes, int radius, TerrainType terrain)
        {
            for (int i = 0; i < nodes.Count; ++i)
            {
                LogicHex node = nodes[i];

                int tile = map.Get(node);
                tile = LogicTile.SetTerrain(tile,terrain);

                map.Set(node, tile);
            }
        }

        public static int Lerp(int a, int b, int tPercentage)
        {
            return (a * (100-tPercentage) +  b * tPercentage) / 100; 
        }

        public static int EaseIn(int a, int b, int tPercentage)
        {
            return (a * (100 - tPercentage) + b * tPercentage) / 100;
        }
        
        public static void AltitudeNoise(LogicHexMap map, int octave, int seed, int percent)
        {
            for (int r = 0; r < map.height; ++r)
            {
                for (int q = 0; q < map.width; ++q)
                {
                    int tile = map.Get(q, r);
                    int altitude = LogicTile.GetAltitude(tile);
                    LogicPoint3 pos = HexToLogicPos(new LogicHex(q, r), altitude * ALTITUDE_SCALE);

                    int noise = LogicPerlinNoise.Noise((pos.x << octave) + seed, (pos.z << octave) + seed, seed << 2, LogicTile.MAX_ALTITUDE);

                    altitude = Lerp(altitude, noise, percent);
                    tile = LogicTile.SetAltitude(tile, altitude);

                    map.Set(q, r, tile);
                }
            }
        }

        public static void AltitudeFlip(LogicHexMap map, int cutoff)
        {
            for (int r = 0; r < map.height; ++r)
            {
                for (int q = 0; q < map.width; ++q)
                {
                    int tile = map.Get(q, r);
                    int altitude = LogicTile.GetAltitude(tile);

                    

                    tile = LogicTile.SetAltitude(tile,altitude);
                    map.Set(q, r, tile);
                }
            }
        }

        public static void AltitudeLevels(LogicHexMap map, int middle, int octave, int seed, int percent)
        {
            for (int r = 0; r < map.height; ++r)
            {
                for (int q = 0; q < map.width; ++q)
                {
                    int tile = map.Get(q, r);
                    int altitude = LogicTile.GetAltitude(tile);
                    LogicPoint3 pos = HexToLogicPos(new LogicHex(q, r), altitude * ALTITUDE_SCALE);


                    int noise = LogicPerlinNoise.Noise((pos.x << octave) + seed, (pos.z << octave) + seed, seed << 2, LogicTween.SCALE);


                    int distanceToMiddle = (noise - LogicTween.SCALE_HALF);
                    bool flip = false;
                    if (distanceToMiddle < 0)
                    {
                        distanceToMiddle = -distanceToMiddle;
                        flip = true;
                    }
                    distanceToMiddle = LogicTween.QuadIn(distanceToMiddle * 2) / 2;
                    if (flip)
                    {
                        distanceToMiddle = -distanceToMiddle;
                    }

                    noise = LogicTween.SCALE_HALF + distanceToMiddle;
                    noise = noise / (LogicTween.SCALE / LogicTile.MAX_ALTITUDE);



                    /*int noise = LogicPerlinNoise.Noise((pos.x << octave) + seed, (pos.z << octave) + seed, seed << 2, LogicTile.MAX_ALTITUDE*2);
                    noise -= LogicTile.MAX_ALTITUDE;
                    if (noise < 0)
                        noise = -noise;


                    LogicTween.QuadIn();
                    */

                    //noise = LogicPerlinNoise.ScaleNoise(noise, LogicTile.MAX_ALTITUDE);

                    altitude = Lerp(altitude, noise, percent);
                    tile = LogicTile.SetAltitude(tile, altitude);






                    map.Set(q, r, tile);
                }
            }
        }


        public static void AltitudeAbsNoise(LogicHexMap map, int octave, int seed, int percent)
        {
            for (int r = 0; r < map.height; ++r)
            {
                for (int q = 0; q < map.width; ++q)
                {
                    int tile = map.Get(q, r);
                    int altitude = LogicTile.GetAltitude(tile);
                    LogicPoint3 pos = HexToLogicPos(new LogicHex(q, r), altitude * ALTITUDE_SCALE);


                    int noise = LogicPerlinNoise.Noise((pos.x << octave) + seed, (pos.z << octave) + seed, seed << 2, LogicTween.SCALE);


                    int distanceToMiddle = (noise - LogicTween.SCALE_HALF);
                    bool flip = false;
                    if (distanceToMiddle < 0)
                    {
                        distanceToMiddle = -distanceToMiddle;
                        flip = true;
                    }
                    distanceToMiddle = LogicTween.QuadIn(distanceToMiddle*2) / 2;
                    if (flip)
                    {
                        distanceToMiddle = -distanceToMiddle;
                    }
                    
                    noise = LogicTween.SCALE_HALF + distanceToMiddle;
                    noise = noise / (LogicTween.SCALE / LogicTile.MAX_ALTITUDE); 



                    /*int noise = LogicPerlinNoise.Noise((pos.x << octave) + seed, (pos.z << octave) + seed, seed << 2, LogicTile.MAX_ALTITUDE*2);
                    noise -= LogicTile.MAX_ALTITUDE;
                    if (noise < 0)
                        noise = -noise;


                    LogicTween.QuadIn();
                    */

                    //noise = LogicPerlinNoise.ScaleNoise(noise, LogicTile.MAX_ALTITUDE);

                    altitude = Lerp(altitude, noise, percent);
                    tile = LogicTile.SetAltitude(tile, altitude);






                    map.Set(q, r, tile);
                }
            }
        }

        /*public static void Noise(LogicHexMap tiles, int seed)
        {
            int i = 0;
            for (int r = 0; r < tiles.height; ++r)
            {
                for (int q = 0; q < tiles.width; ++q)
                {
                    int tile = tiles.Get(q, r);
                    LogicPoint3 pos = HexToLogicPos(new LogicHex(q,r), LogicTile.GetAltitude(tile) * 8);


                    int noise = 0;
                    //Two octaves
                    noise += LogicPerlinNoise.Noise((pos.x << 6) + seed, (pos.z << 6) + seed, seed);
                    noise += LogicPerlinNoise.Noise((pos.x << 5) + seed, (pos.z << 5) + seed, seed);
                    noise /= 2;

                    int height = LogicPerlinNoise.ScaleNoise(noise, LogicTile.MAX_ALTITUDE);
                    tile = LogicTile.SetAltitude(tile, height);

                    int room = LogicTile.GetRoom(tile);

                    if (room == 0)
                        tile = LogicTile.SetTerrain(tile, TerrainType.Grass);
                        

                    if (height > 167)
						tile = LogicTile.SetTerrain(tile, TerrainType.Rock);
                    else if (height > 127)
                        tile = LogicTile.SetTerrain(tile, TerrainType.Sand);
                    else if (height > 87)
                        tile = LogicTile.SetTerrain(tile, TerrainType.Dirt);
                    else
                    //tile = LogicTile.SetTerrain(tile, TerrainType.Grass);

                    tiles.Set(q, r, tile);
                    ++i;
                }
            }
        }*/


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
                    sum += LogicTile.GetAltitude(tiles.Get(x, y));
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
            tile = LogicTile.SetAltitude(tile, tileHeight);

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

	}

    public class GeneratedTile
    {
        const int TILE_ALTITUDE_SCALE = LogicTween.SCALE / LogicTile.MAX_ALTITUDE;

        public int altitude = 0;
        public TerrainType terrain = TerrainType.Grass;

        public int GetLogicTile()
        {
            int tile = LogicTile.SetAltitude(0, altitude / TILE_ALTITUDE_SCALE);
            return LogicTile.SetTerrain(tile, terrain);
        }
    }
    
}
