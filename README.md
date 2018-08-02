# tilegrab

Grabs the tiles from remote server and create a local tilecache

## Installation

```
$ git clone https://github.com/bertt/tilegrab.git
$ cd tilegrab
$ dotnet restore
$ dotnet run
````

Parameters in the program.cs:

- bounding box (xmin, ymin, xmax, ymax)
- url remote server
- format of tiles (like 'png' or 'pbf')

Tiles are generated in output directory.


## MBTiles support

If you want to generate a MBTile file from the output files, use the command <a href="https://github.com/mapbox/mbutil">mb-util</a> as follows:

```
$ mb-util tiles test2.mbtiles --image_format=pbf
```
