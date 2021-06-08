using System;
using DefaultEcs;
using Microsoft.Xna.Framework;
using Oasis.Tiles;

namespace Oasis {
    // Stores, manipulates and queries Tile data
    public class Map {
        TileBase[] _tiles; // contain all tile objects
        private int _width;
        private int _height;

        public TileBase[] Tiles { get { return _tiles; } set { _tiles = value; } }
        public int Width { get { return _width; } set { _width = value; } }
        public int Height { get { return _height; } set { _height = value; } }



        //Build a new map with a specified width and height
        public Map(int width, int height) {
            _width = width;
            _height = height;
            Tiles = new TileBase[width * height];
        }
        public bool IsTileWalkable(Point location) {
            if (location.X < 0 || location.Y < 0 || location.X >= Width || location.Y >= Height)
                return false;
            return !_tiles[location.Y * Width + location.X].IsBlockingMove;
        }


        public Entity? GetEntityAt <T> (Point location) where T : struct {
            foreach (Entity entity in GameLoop.gs.ecs.GetEntities().With<Render>().With<T>().AsEnumerable()) {
                if (entity.Get<Render>().GetPosition() == location) {
                    return entity;
                }
            }

            return null;
        }

        public T GetTileAt<T>(int x, int y) where T : TileBase {
            int locationIndex = xy_idx(x, y);
            // make sure the index is within the boundaries of the map!
            if (locationIndex <= Width * Height && locationIndex >= 0) {
                if (Tiles[locationIndex] is T)
                    return (T)Tiles[locationIndex];
                else return null;
            } else return null;
        }


        public int xy_idx (int x, int y) {
            return (y * Height) + x;
        }

        public int xy_idx(Point pos) {
            return (pos.Y * Height) + pos.X;
        }

        public Point idx_xy (int idx) {
            return new Point(idx % Width, idx / Width);
        }
    }
}
