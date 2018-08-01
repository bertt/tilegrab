using System.Collections.Generic;
using Tiles.Tools;

namespace tilegrab
{
    public class TileHelper
    {
        public static List<Tile> GetTilesInArea(double minx, double miny, double maxx, double maxy)
        {
            var res = new List<Tile>();
            for (var z = 0; z <= 17; z++)
            {
                var tile = Tilebelt.PointToTile(minx, miny, z);
                var tile1 = Tilebelt.PointToTile(maxx, maxy, z);

                for (var x = tile.X; x <= tile1.X; x++)
                {
                    for (var y = tile1.Y; y <= tile.Y; y++)
                    {
                        var t = new Tile(x, y, z);
                        res.Add(t);
                    }

                }
            }
            return res;
        }
    }
}
