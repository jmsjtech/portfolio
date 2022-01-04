using System;
using System.Linq;
using SadRogue.Primitives;
using LofiHollow.Entities;
using Newtonsoft.Json;
using LofiHollow.TileData;

namespace LofiHollow {
    [JsonObject(MemberSerialization.OptIn)]
    public class Map {
        [JsonProperty]
        public TileBase[] Tiles;
        [JsonProperty]
        public int Width;
        [JsonProperty]
        public int Height;

        [JsonProperty]
        public MinimapTile MinimapTile = new MinimapTile(',', new Color(0, 127, 0), Color.Black);

        [JsonProperty]
        public bool PlayerCanBuild = false;
        [JsonProperty]
        public bool AmbientMonsters = false;

        public GoRogue.MultiSpatialMap<Entity> Entities;
        public GoRogue.Pathing.FastAStar MapPath;
        public static GoRogue.IDGenerator IDGenerator = new GoRogue.IDGenerator();

        public GoRogue.MapViews.LambdaMapView<bool> MapFOV;


        public Map(int width, int height) {
            Width = width;
            Height = height;
            Tiles = new TileBase[width * height];

            for (int i = 0; i < Tiles.Length; i++) {
                Tiles[i] = new TileBase(); 
            }

            Entities = new GoRogue.MultiSpatialMap<Entity>();

            var MapView = new GoRogue.MapViews.LambdaMapView<bool>(width, height, pos => IsTileWalkable(new Point(pos.X, pos.Y)));
            MapFOV = new GoRogue.MapViews.LambdaMapView<bool>(GameLoop.MapWidth, GameLoop.MapHeight, pos => BlockingLOS(new Point(pos.X, pos.Y)));
            MapPath = new GoRogue.Pathing.FastAStar(MapView, GoRogue.Distance.CHEBYSHEV);
        }
         
        public bool IsTileWalkable(Point location) { 
            if (location.X < 0 || location.Y < 0 || location.X >= Width || location.Y >= Height)
                return false; 
            return !Tiles[location.Y * Width + location.X].IsBlockingMove;
        }

        public bool BlockingLOS(Point location) {
            if (location.X < 0 || location.Y < 0 || location.X >= Width || location.Y >= Height)
                return true;
            return !Tiles[location.Y * Width + location.X].IsBlockingLOS;
        }

        public void ToggleDoor(Point location) {
            if (location.X < 0 || location.Y < 0 || location.X >= Width || location.Y >= Height)
                return;
            if (Tiles[location.Y * Width + location.X].Lock != null) {
                LockOwner lockData = Tiles[location.Y * Width + location.X].Lock;

                if (lockData.Closed)
                    Tiles[location.Y * Width + location.X].Glyph = lockData.OpenedGlyph; 
                else
                    Tiles[location.Y * Width + location.X].Glyph = lockData.ClosedGlyph;

                Tiles[location.Y * Width + location.X].Lock.Closed = !Tiles[location.Y * Width + location.X].Lock.Closed;
                Tiles[location.Y * Width + location.X].IsBlockingMove = lockData.Closed;
                Tiles[location.Y * Width + location.X].IsBlockingLOS = lockData.Closed;

                return;
            }

            return;
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
