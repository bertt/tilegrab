﻿using System;
using System.IO;
using System.Net;

namespace tilegrab
{
    class Program
    {
        static void Main(string[] args)
        {
            // bounding box (amsterdam as sample) tip use bboxfinder.com
            var xmin = 4.814930;
            var ymin = 52.320757;
            var xmax = 4.988308;
            var ymax = 52.396763;

            // remote server to download from
            var url = $"https://geodata.nationaalgeoregister.nl/beta/topotiles/";
            var extension = "pbf";

            var tiles = TileHelper.GetTilesInArea(xmin, ymin, xmax, ymax);
            var sep = Path.DirectorySeparatorChar;
            var wc = new WebClient();

            foreach (var t in tiles)
            {
                var di = Directory.CreateDirectory($"tiles{sep}{t.Z}{sep}{t.X}");
                var filename = di.FullName + sep + t.Y + "." + extension;
                var downloadfile = url + $"{ t.Z}/{ t.X}/{t.Y}.{extension}";
                wc.DownloadFile(new Uri(downloadfile), filename);
                Console.WriteLine(downloadfile);
            }

            wc.Dispose();
            // file metadata.json is needed for mb-util tool
            File.Copy("metadata.json", $"tiles{sep}/metadata.json", true);
            Console.WriteLine("TileGrab is ready... Press any key to exit");
            Console.ReadKey();
        }
    }
}
