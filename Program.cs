using MBTiles.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace tilegrab;

class Program
{

    async static Task Main(string[] args)
    {
        var city = "amsterdam";
        var bbox = GetBBox(city);
        var db = $"{city}.mbtiles";
        var xmin = Double.Parse(bbox.Split(",")[0]);
        var ymin = Double.Parse(bbox.Split(",")[1]);
        var xmax = Double.Parse(bbox.Split(",")[2]);
        var ymax = Double.Parse(bbox.Split(",")[3]);

        var levelFrom = 0;
        var levelTo = 17;

        // remote server to download from
        var url = $"https://geodata.nationaalgeoregister.nl/beta/topotiles/";
        var extension = "pbf";

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        if (File.Exists(db))
        {
            File.Delete(db);
        }

        if (!Directory.Exists("tiles"))
        {
            Directory.CreateDirectory("tiles");
        }

        var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(url + "/metadata.json");
        var content = await response.Content.ReadAsStringAsync();
        var metadata = System.Text.Json.JsonSerializer.Deserialize<Metadata>(content);
        metadata.bounds = bbox;
        metadata.center = $"{xmin + (xmax - xmin) / 2},{ymin + (ymax - ymin)/2},{levelTo}";
        metadata.name = city;
        metadata.description = city;
        var conn = MBTilesWriter.CreateDatabase(db, metadata);

        var tiles = TileHelper.GetTilesInArea(xmin, ymin, xmax, ymax, levelFrom, levelTo);
        Console.WriteLine("total tiles: " + tiles.Count);
        var sep = Path.DirectorySeparatorChar;
        var i = 1;
        conn.Open();
        tiles.AsParallel().ForAll(async t =>
        { 
            i++;
            var perc = Math.Round(((double)i / tiles.Count) * 100, 2);
            Console.Write($"\rtile {i}/{tiles.Count} - {perc:F}%");

            var downloadfile = url + $"{ t.Z}/{ t.X}/{t.Y}.{extension}";
            var responseResult = httpClient.GetAsync(downloadfile).Result;
            if (responseResult.StatusCode == HttpStatusCode.OK)
            {
                var content1 = await responseResult.Content.ReadAsByteArrayAsync();
                // File.WriteAllBytes("tiles/" + Path.GetFileName(downloadfile), content1);
                MBTilesWriter.WriteTile(conn, t, content1);
            }
        });

        httpClient.Dispose();
        conn.Close();
        Console.WriteLine();

        stopwatch.Stop();
        Console.WriteLine("Total duration: " + stopwatch.Elapsed.TotalSeconds);
        Console.WriteLine("Request/tile: " + stopwatch.Elapsed.TotalSeconds / tiles.Count);
        Console.WriteLine("TileGrab is ready...");
    }

    private static string GetBBox(string city)
    {
        var dict = new Dictionary<string, string>();
        dict.Add("utrecht", "4.933,52.001,5.303,52.184");
        dict.Add("maastricht", "5.631,50.789,5.773,50.914");
        dict.Add("rotterdam", "3.911,51.737,4.784,52.109");
        dict.Add("thehague", "4.183,52.006,4.427,52.136");
        dict.Add("amsterdam", "4.814930,52.320757,4.988308,52.396763");

        return dict[city];

        // for complete nl
        //var xmin = 3.31497114423;
        //var ymin = 50.803721015;
        //var xmax = 7.09205325687;
        //var ymax = 53.5104033474;

    }
}
