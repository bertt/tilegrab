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

Tiles are generated in output directory for levels 0..17.
