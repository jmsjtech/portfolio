using System;
using DefaultEcs;
using Microsoft.Xna.Framework;
using SadConsole;

namespace NeonCity {
    // Stores, manipulates and queries Tile data
    public class Map {
        public Entity[] _tiles; // contain all tile objects
        private int _width;
        private int _height;

        public Cell[] Tiles;
        public int Width { get { return _width; } set { _width = value; } }
        public int Height { get { return _height; } set { _height = value; } }
        

        //Build a new map with a specified width and height
        public Map(int width, int height) {
            _width = width;
            _height = height;
            Tiles = new Cell[width * height];
            _tiles = new Entity[width * height];
        }
       


        public Entity? GetEntityAt<T>(Point location) where T : struct {
            foreach (Entity entity in GameLoop.gs.ecs.GetEntities().With<Tile>().With<T>().AsEnumerable()) {
                if (entity.Get<Tile>().GetPosition() == location) {
                    return entity;
                }
            }

            return null;
        }

        public Entity? GetTileAt(int x, int y) {
            int locationIndex = xy_idx(x, y);
            // make sure the index is within the boundaries of the map!
            if (locationIndex <= Width * Height && locationIndex >= 0) {
                if (Tiles[locationIndex] != null)
                    return _tiles[locationIndex];
                else return null;
            } else return null;
        }


        public int xy_idx(int x, int y) {
            return (y * Height) + x;
        }

        public int xy_idx(Point pos) {
            return (pos.Y * Height) + pos.X;
        }

        public Point idx_xy(int idx) {
            return new Point(idx % Width, idx / Width);
        }
    }
}
