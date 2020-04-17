# tilegrab

Grabs the tiles from remote server and create a local mbtile file.

## Installation

```
$ git clone https://github.com/bertt/tilegrab.git
$ cd tilegrab
$ dotnet restore
$ dotnet run
````

## Sample output

```
total tiles: 4066
tile 4066/4066 - 100.00%
Total duration: 72.2130713
Request/tile: 0.01776022412690605
TileGrab is ready...
```

## Parameters 

Use parameters in the program.cs:

- bounding box (xmin, ymin, xmax, ymax)
- Z level from and to
- url remote server
- format of tiles (like 'png' or 'pbf')

Mbtile file is generated in output directory. 
