using System;
using System.Collections.Generic;
using System.Linq;
using DefaultEcs;
using Microsoft.Xna.Framework;

namespace NeonCity {
    public class MapGenerator {
        public MapGenerator() {
        }

        Map _map;

        public Map GenerateMap(int mapWidth, int mapHeight) {
            _map = new Map(mapWidth, mapHeight);

            List<Rectangle> Rooms = new List<Rectangle>();


            FloodGrass();

            Trees();

            Ores();

            // spit out the final map
            return _map;
        }

        // Fills the map with grass
        private void FloodGrass() {
            for (int i = 0; i < _map.Tiles.Length; i++) {
                Entity newTile = GameLoop.gs.ecs.CreateEntity();
                newTile.Set(new Tile { cell = new SadConsole.Cell(Color.Lime, Color.Transparent, '"') });
                newTile.Get<Tile>().SetPosition(i, _map.Width);
                newTile.Set(new Name { name = "Grass" });

                _map._tiles[i] = newTile;
                _map.Tiles[i] = newTile.Get<Tile>().cell;
            }
        }

        private void Trees() {
            int num_trees = (_map.Width * _map.Height) / 10;
            for (int i = 0; i < num_trees; i++) {
                int index = GameLoop.GlobalRand.Next(0, _map.Tiles.Length);
                CreateTree(index.ToPoint(_map.Width));
            }
        }

        private void CreateTree(Point location) {
            if (_map._tiles[location.ToIndex(_map.Width)] != null) {
                _map._tiles[location.ToIndex(_map.Width)].Dispose();
            }

            Entity newTile = GameLoop.gs.ecs.CreateEntity();
            newTile.Set(new Tile { cell = new SadConsole.Cell(Color.Lime, Color.Transparent, (char) 6) });
            newTile.Get<Tile>().SetPosition(location);
            newTile.Set(new Name { name = "Tree" });
            newTile.Set(new Resources("Health", 100));
            newTile.Get<Resources>().Add("Wood", 20);

            _map._tiles[location.ToIndex(_map.Width)] = newTile;
            _map.Tiles[location.ToIndex(_map.Width)] = newTile.Get<Tile>().cell;
        }

        private void Ores() {
            int stone_deposits = 10;
            int iron_deposits = 10;
            int copper_deposits = 10;
            int coal_deposits = 10;

            for (int i = 0; i < stone_deposits; i++) {
                int index = GameLoop.GlobalRand.Next(0, _map.Tiles.Length);
                OreDeposit(index.ToPoint(_map.Width), "Stone", Color.DarkGray, GameLoop.GlobalRand.Next(5, 50), GameLoop.GlobalRand.Next(3, 10));
            }

            for (int i = 0; i < iron_deposits; i++) {
                int index = GameLoop.GlobalRand.Next(0, _map.Tiles.Length);
                OreDeposit(index.ToPoint(_map.Width), "Iron", Color.Silver, GameLoop.GlobalRand.Next(5, 50), GameLoop.GlobalRand.Next(3, 10));
            }

            for (int i = 0; i < copper_deposits; i++) {
                int index = GameLoop.GlobalRand.Next(0, _map.Tiles.Length);
                OreDeposit(index.ToPoint(_map.Width), "Copper", Color.Orange, GameLoop.GlobalRand.Next(5, 50), GameLoop.GlobalRand.Next(3, 10));
            }

            for (int i = 0; i < coal_deposits; i++) {
                int index = GameLoop.GlobalRand.Next(0, _map.Tiles.Length);
                OreDeposit(index.ToPoint(_map.Width), "Coal", Color.DarkSlateGray, GameLoop.GlobalRand.Next(5, 50), GameLoop.GlobalRand.Next(3, 10));
            }
        }

        private void OreDeposit(Point center, string type, Color fg,  int intensity, int radius) {
            GoRogue.RadiusAreaProvider a = new GoRogue.RadiusAreaProvider(center, radius, GoRogue.Distance.EUCLIDEAN);
            foreach (Point location in a.CalculatePositions()) {
                if (location.X > 0 && location.Y > 0 && location.X < _map.Width && location.Y < _map.Height) {
                    if (_map._tiles[location.ToIndex(_map.Width)] != null) {
                        _map._tiles[location.ToIndex(_map.Width)].Dispose();
                    }

                    Entity newTile = GameLoop.gs.ecs.CreateEntity();

                    if (intensity * ((radius - (int)(GoRogue.Distance.EUCLIDEAN.Calculate(center, location))) - GameLoop.GlobalRand.Next(-20, 15)) > 0) {
                        newTile.Set(new Tile { cell = new SadConsole.Cell(fg, Color.Transparent, (char)4) });
                        newTile.Get<Tile>().SetPosition(location);
                        newTile.Set(new Name { name = type });
                        newTile.Set(new Resources("Health", 100));
                        newTile.Get<Resources>().Add(type, intensity * (radius - (int)(GoRogue.Distance.EUCLIDEAN.Calculate(center, location))));
                    } else {
                        newTile.Set(new Tile { cell = new SadConsole.Cell(Color.Lime, Color.Transparent, '"') });
                        newTile.Get<Tile>().SetPosition(location);
                        newTile.Set(new Name { name = "Grass" });
                    }

                    _map._tiles[location.ToIndex(_map.Width)] = newTile;
                    _map.Tiles[location.ToIndex(_map.Width)] = newTile.Get<Tile>().cell;
                }
            }
        }



        // Returns a list of points expressing the perimeter of a rectangle
        private List<Point> GetBorderCellLocations(Rectangle room) {
            //establish room boundaries
            int xMin = room.Left;
            int xMax = room.Right;
            int yMin = room.Top;
            int yMax = room.Bottom;

            // build a list of room border cells using a series of
            // straight lines
            List<Point> borderCells = GetTileLocationsAlongLine(xMin, yMin, xMax, yMin).ToList();
            borderCells.AddRange(GetTileLocationsAlongLine(xMin, yMin, xMin, yMax));
            borderCells.AddRange(GetTileLocationsAlongLine(xMin, yMax, xMax, yMax));
            borderCells.AddRange(GetTileLocationsAlongLine(xMax, yMin, xMax, yMax));

            return borderCells;
        }


        // returns a collection of Points which represent
        // locations along a line
        public IEnumerable<Point> GetTileLocationsAlongLine(int xOrigin, int yOrigin, int xDestination, int yDestination) {
            // prevent line from overflowing
            // boundaries of the map
            xOrigin = ClampX(xOrigin);
            yOrigin = ClampY(yOrigin);
            xDestination = ClampX(xDestination);
            yDestination = ClampY(yDestination);

            int dx = Math.Abs(xDestination - xOrigin);
            int dy = Math.Abs(yDestination - yOrigin);

            int sx = xOrigin < xDestination ? 1 : -1;
            int sy = yOrigin < yDestination ? 1 : -1;
            int err = dx - dy;

            while (true) {

                yield return new Point(xOrigin, yOrigin);
                if (xOrigin == xDestination && yOrigin == yDestination) {
                    break;
                }
                int e2 = 2 * err;
                if (e2 > -dy) {
                    err = err - dy;
                    xOrigin = xOrigin + sx;
                }
                if (e2 < dx) {
                    err = err + dx;
                    yOrigin = yOrigin + sy;
                }
            }
        }

        // sets X coordinate between right and left edges of map
        // to prevent any out-of-bounds errors
        private int ClampX(int x) {
            if (x < 0)
                x = 0;
            else if (x > _map.Width - 1)
                x = _map.Width - 1;
            return x;
            // OR using ternary conditional operators: return (x < 0) ? 0 : (x > _map.Width - 1) ? _map.Width - 1 : x;
        }

        // sets Y coordinate between top and bottom edges of map
        // to prevent any out-of-bounds errors
        private int ClampY(int y) {
            if (y < 0)
                y = 0;
            else if (y > _map.Height - 1)
                y = _map.Height - 1;
            return y;
            // OR using ternary conditional operators: return (y < 0) ? 0 : (y > _map.Height - 1) ? _map.Height - 1 : y;
        }
    }
}
