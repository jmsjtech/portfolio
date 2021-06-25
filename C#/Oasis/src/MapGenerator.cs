using System;
using System.Collections.Generic;
using System.Linq;
using DefaultEcs;
using Microsoft.Xna.Framework;

namespace CritterQuest {
    public class MapGenerator {
        public MapGenerator() {
        }

        Map _map; 

        public Map GenerateMap(int mapWidth, int mapHeight, int maxRooms, int minRoomSize, int maxRoomSize) {
            _map = new Map(mapWidth, mapHeight);

            List<Rectangle> Rooms = new List<Rectangle>();

            for (int i = 0; i < maxRooms; i++) {
                int newRoomWidth = GameLoop.GlobalRand.Next(minRoomSize, maxRoomSize);
                int newRoomHeight = GameLoop.GlobalRand.Next(minRoomSize, maxRoomSize);

                int newRoomX = GameLoop.GlobalRand.Next(0, mapWidth - newRoomWidth - 1);
                int newRoomY = GameLoop.GlobalRand.Next(0, mapHeight - newRoomHeight - 1);

                Rectangle newRoom = new Rectangle(newRoomX, newRoomY, newRoomWidth, newRoomHeight);

                bool newRoomIntersects = Rooms.Any(room => newRoom.Intersects(room));

                if (!newRoomIntersects) {
                    Rooms.Add(newRoom);
                }
            }

            FloodWalls();

            foreach (Rectangle room in Rooms) {
                CreateRoom(room);
            }

            // carve out tunnels between all rooms
            // based on the Positions of their centers
            for (int r = 1; r < Rooms.Count; r++) {
                //for all remaining rooms get the center of the room and the previous room
                Point previousRoomCenter = Rooms[r - 1].Center;
                Point currentRoomCenter = Rooms[r].Center;

                // give a 50/50 chance of which 'L' shaped connecting hallway to tunnel out
                if (GameLoop.GlobalRand.Next(1, 2) == 1) {
                    CreateHorizontalTunnel(previousRoomCenter.X, currentRoomCenter.X, previousRoomCenter.Y);
                    CreateVerticalTunnel(previousRoomCenter.Y, currentRoomCenter.Y, currentRoomCenter.X);
                } else {
                    CreateVerticalTunnel(previousRoomCenter.Y, currentRoomCenter.Y, previousRoomCenter.X);
                    CreateHorizontalTunnel(previousRoomCenter.X, currentRoomCenter.X, currentRoomCenter.Y);
                }
            }

            foreach (Rectangle room in Rooms) {
                CreateDoor(room);
            }

            // spit out the final map
            return _map;
        }

        // Fills the map with walls
        private void FloodWalls() {
            for (int i = 0; i < _map.Tiles.Length; i++) {
                _map.Tiles[i] = CreateWall(i.ToPoint());
            }
        }

        // Builds a room composed of walls and floors using the supplied Rectangle
        // which determines its size and position on the map
        // Walls are placed at the perimeter of the room
        // Floors are placed in the interior area of the room
        private void CreateRoom(Rectangle room) {
            // Place floors in interior area
            for (int x = room.Left + 1; x < room.Right; x++) {
                for (int y = room.Top + 1; y < room.Bottom; y++) {
                    CreateFloor(new Point(x, y));
                }
            }

            // Place walls at perimeter
            List<Point> perimeter = GetBorderCellLocations(room);
            foreach (Point location in perimeter) {
                CreateWall(location);
            }
        }

        // Creates a Floor tile at the specified X/Y location
        private void CreateFloor(Point location) {
            _map.Tiles[location.ToIndex(_map.Width)] = new TileFloor();
        }

        // Creates a Wall tile at the specified X/Y location
        private void CreateWall(Point location) {
            _map.Tiles[location.ToIndex(_map.Width)] = new TileWall();
        }

        private void CreateDoor(Rectangle room) {
            List<Point> borderCells = GetBorderCellLocations(room);

            //go through every border cell and look for potential door candidates
            foreach (Point location in borderCells) {
                int locationIndex = location.ToIndex(_map.Width);
                if (IsPotentialDoor(location)) {
                    // Create a new door that is closed and unlocked.
                    Entity newDoor = GameLoop.gs.ecs.CreateEntity();
                    newDoor.Set(new Door { is_locked = false, is_open = false });
                    newDoor.Set(new BlocksMovement { });
                    newDoor.Set(new BlocksVisibility { });
                    newDoor.Set(new Name { name = "Door" });
                    newDoor.Set(new Render { sce = new SadConsole.Entities.Entity(1, 1) });
                    newDoor.Get<Render>().Init(_map.idx_xy(locationIndex), '+', Color.Brown);

                    _map.Tiles[locationIndex].IsBlockingLOS = true;
                }
            }
        }

        private bool IsPotentialDoor(Point location) {
            //if the target location is not walkable
            //then it's a wall and not a good place for a door
            int locationIndex = location.ToIndex(_map.Width);
            if (_map.Tiles[locationIndex] != null && _map.Tiles[locationIndex] is TileWall) {
                return false;
            }

            //store references to all neighbouring cells
            Point right = new Point(location.X + 1, location.Y);
            Point left = new Point(location.X - 1, location.Y);
            Point top = new Point(location.X, location.Y - 1);
            Point bottom = new Point(location.X, location.Y + 1);

            // check to see if there is a door already in the target
            // location, or above/below/right/left of the target location
            // If it detects a door there, return false.
            if (_map.GetEntityAt<Door>(location) != null ||
                _map.GetEntityAt<Door>(right) != null ||
                _map.GetEntityAt<Door>(left) != null ||
                _map.GetEntityAt<Door>(top) != null ||
                _map.GetEntityAt<Door>(bottom) != null
               ) {
                return false;
            }

            //if all the prior checks are okay, make sure that the door is placed along a horizontal wall
            if (!_map.Tiles[right.ToIndex(_map.Width)].IsBlockingMove && !_map.Tiles[left.ToIndex(_map.Width)].IsBlockingMove && _map.Tiles[top.ToIndex(_map.Width)].IsBlockingMove && _map.Tiles[bottom.ToIndex(_map.Width)].IsBlockingMove) {
                return true;
            }
            //or make sure that the door is placed along a vertical wall
            if (_map.Tiles[right.ToIndex(_map.Width)].IsBlockingMove && _map.Tiles[left.ToIndex(_map.Width)].IsBlockingMove && !_map.Tiles[top.ToIndex(_map.Width)].IsBlockingMove && !_map.Tiles[bottom.ToIndex(_map.Width)].IsBlockingMove) {
                return true;
            }
            return false;
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

        // carve a tunnel out of the map parallel to the x-axis
        private void CreateHorizontalTunnel(int xStart, int xEnd, int yPosition) {
            for (int x = Math.Min(xStart, xEnd); x <= Math.Max(xStart, xEnd); x++) {
                CreateFloor(new Point(x, yPosition));
            }
        }

        // carve a tunnel using the y-axis
        private void CreateVerticalTunnel(int yStart, int yEnd, int xPosition) {
            for (int y = Math.Min(yStart, yEnd); y <= Math.Max(yStart, yEnd); y++) {
                CreateFloor(new Point(xPosition, y));
            }
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
