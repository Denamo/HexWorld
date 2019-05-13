using logic.debug;
using logic.math;
using logic.util;
using System;
using System.Collections.Generic;

namespace game
{

    [Serializable]
    public class LogicWorldConfig
    {
        public int mapSize = 64;
        public int seed = 0;
        public List<LogicTileMod> tileMods;
    }

    public enum TileModType
    {
        Noise,
        Plateau,
        TerrainSet,
        Count
    }



    [Serializable]
    public class LogicTileMod
    {
        public bool enabled = true;
        public TileModType type;
        public int p1;
        public int p2;
        public int p3;

        public void Execute(GeneratedTile tile, int x, int y, int seed)
        {
            if (!enabled)
                return;

            switch (type)
            {
                case TileModType.Noise:
                    int noise = LogicWorld.Noise(x, y, p1, seed + p3 << 6, LogicTween.SCALE);
                    tile.altitude = LogicWorld.Lerp(tile.altitude, noise, p2);
                    break;
                case TileModType.Plateau:
                    int plateau = LogicWorld.Plateau(tile.altitude, LogicTween.SCALE * p1 / 100);
                    tile.altitude = LogicWorld.Lerp(tile.altitude, plateau, p2);
                    break;
                case TileModType.TerrainSet:
                    if (tile.altitude > (LogicTween.SCALE * p1 / 100) && tile.altitude < (LogicTween.SCALE * p2 / 100))
                    {
                        tile.terrain = (TerrainType)p3;
                    }
                    break;
            }
        }

    }



}
