using System;
using System.Linq;
using SadRogue.Primitives;
using LofiHollow.Entities;


namespace LofiHollow {
    public class Map {
        TileBase[] _tiles; 
        private int _width;
        private int _height;

        public TileBase[] Tiles { get { return _tiles; } set { _tiles = value; } }
        public MinimapTile MinimapTile = new MinimapTile(',', new Color(0, 127, 0), Color.Black);
        public int Width { get { return _width; } set { _width = value; } }
        public int Height { get { return _height; } set { _height = value; } }

        public bool PlayerCanBuild = false;
        public bool AmbientMonsters = false;

        public GoRogue.MultiSpatialMap<Entity> Entities;  
        public static GoRogue.IDGenerator IDGenerator = new GoRogue.IDGenerator();  
        

        public Map(int width, int height) {
            _width = width;
            _height = height;
            Tiles = new TileBase[width * height];

            for (int i = 0; i < Tiles.Length; i++) {
                Tiles[i] = new TileBase();
            }

            Entities = new GoRogue.MultiSpatialMap<Entity>();
        }
         
        public bool IsTileWalkable(Point location) { 
            if (location.X < 0 || location.Y < 0 || location.X >= Width || location.Y >= Height)
                return false; 
            return !_tiles[location.Y * Width + location.X].IsBlockingMove;
        }

        public bool ToggleDoor(Point location, bool open) {
            if (location.X < 0 || location.Y < 0 || location.X >= Width || location.Y >= Height)
                return false;
            if (_tiles[location.Y * Width + location.X].TileID == 7 && open) {
                _tiles[location.Y * Width + location.X] = GameLoop.World.tileLibrary[8];
               // GameLoop.UIManager.MapConsole.SetRenderCells();
            }

            if (_tiles[location.Y * Width + location.X].TileID == 8 && !open) {
                _tiles[location.Y * Width + location.X] = GameLoop.World.tileLibrary[7];
               // GameLoop.UIManager.MapConsole.SetRenderCells();
            }

            return false;
        }

        public T GetEntityAt<T>(Point location) where T : Entity {
            return Entities.GetItems(new GoRogue.Coord(location.X, location.Y)).OfType<T>().FirstOrDefault();
        }

        public TileBase GetTile(Point location) {
            return Tiles[location.ToIndex(GameLoop.MapWidth)];
        }
         
        public void Remove(Entity entity) { 
            Entities.Remove(entity);
            entity.PositionChanged -= OnPositionChange;
        }
         
        public void Add(Entity entity) { 
            Entities.Add(entity, new GoRogue.Coord(entity.Position.X, entity.Position.Y));
            entity.PositionChanged += OnPositionChange;
        }

        private void OnPositionChange(object sender, SadConsole.ValueChangedEventArgs<Point> e) {
            Entities.Move(sender as Entity, new GoRogue.Coord(e.NewValue.X, e.NewValue.Y)); 
        } 
    }
}
