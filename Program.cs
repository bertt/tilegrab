using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace tilegrab
{
    class Program
    {
        async static Task Main(string[] args)
        {
            // for complete nl
            //var xmin = 3.31497114423;
            //var ymin = 50.803721015;
            //var xmax = 7.09205325687;
            //var ymax = 53.5104033474;

            // bounding box (amsterdam as sample) tip use bboxfinder.com
            var xmin = 4.814930;
            var ymin = 52.320757;
            var xmax = 4.988308;
            var ymax = 52.396763;
            var levelFrom = 0;
            var levelTo = 17;

            // remote server to download from
            var url = $"https://geodata.nationaalgeoregister.nl/beta/topotiles/";
            var extension = "pbf";

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var tiles = TileHelper.GetTilesInArea(xmin, ymin, xmax, ymax, levelFrom, levelTo);
            Console.WriteLine("total tiles: " + tiles.Count);
            var sep = Path.DirectorySeparatorChar;
            var httpClient = new HttpClient();

            Downloadfile(httpClient, $"tiles{sep}/metadata.json", url + "/metadata.json");

            var i = 1;
            tiles.AsParallel().ForAll(t =>
            {
                i++;
                var perc = Math.Round(((double)i / tiles.Count) * 100, 2);
                Console.Write($"\rtile {i}/{tiles.Count} - {perc:F}%");

                var di = Directory.CreateDirectory($"tiles{sep}{t.Z}{sep}{t.X}");
                var filename = di.FullName + sep + t.Y + "." + extension;
                var downloadfile = url + $"{ t.Z}/{ t.X}/{t.Y}.{extension}";

                Downloadfile(httpClient, filename, downloadfile);

            });

            httpClient.Dispose();
            Console.WriteLine();

            stopwatch.Stop();
            Console.WriteLine("Total duration: " + stopwatch.Elapsed.TotalSeconds);
            Console.WriteLine("Request/tile: " + stopwatch.Elapsed.TotalSeconds/tiles.Count);

            Console.WriteLine("TileGrab is ready... Press any key to exit");
            Console.ReadKey();
        }

        private static void Downloadfile(HttpClient httpClient, string filename, string downloadfile)
        {
            var responseResult = httpClient.GetAsync(downloadfile);
            using (var memStream = responseResult.Result.Content.ReadAsStreamAsync().Result)
            {
                using (var fileStream = File.Create($"{filename}"))
                {
                    memStream.CopyTo(fileStream);
                }

            }
        }
    }
}
