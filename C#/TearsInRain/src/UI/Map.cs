using TearsInRain.Tiles;
using SadConsole;
using TearsInRain.Entities;
using System.Linq;
using Point = Microsoft.Xna.Framework.Point;
using GoRogue.MapViews;
using GoRogue;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TearsInRain.Serializers;
using Newtonsoft.Json;

namespace TearsInRain {

    [JsonConverter(typeof(MapJsonConverter))]
    public class Map{
        //TileBase[] _tiles;
        public int Width { get; }
        public int Height { get; }

        public TileBase[] Tiles;


        public GoRogue.MultiSpatialMap<Entity> Entities;
        public static GoRogue.IDGenerator IDGenerator = new GoRogue.IDGenerator();
        
        public Map(int width, int height) {
            Width = width;
            Height = height;

            if (GameLoop.ReceivedEntities != null) {
                Entities = GameLoop.ReceivedEntities;
            } else {
                Entities = new GoRogue.MultiSpatialMap<Entity>();
            }
        }

        public Map(TileBase[] tiles, int W = 100, int H = 100) {
            Width = W;
            Height = H;

            for (int i = 0; i < tiles.Length; i++) {
                Tiles[i] = tiles[i];
            }

            if (GameLoop.ReceivedEntities != null) {
                Entities = GameLoop.ReceivedEntities;
            } else {
                Entities = new GoRogue.MultiSpatialMap<Entity>();
            }
        }


        public bool IsTileWalkable(Point location) {
            TerrainFeature terrain = GetEntityAt<TerrainFeature>(location);

            if (terrain != null) {
                return !GetTileAt<TileBase>(location).IsBlockingMove && !terrain.IsBlockingMove;
            }

            return !GetTileAt<TileBase>(location).IsBlockingMove;
        }

        public T GetTileAt<T>(int x, int y) where T : TileBase {
            if (Tiles[xy_idx(x, y)] is T) {
                return (T)Tiles[xy_idx(x, y)];
            } else return null;
        }


        public T GetTileAt<T>(Point loc) where T : TileBase {
            if (Tiles[xy_idx(loc.X, loc.Y)] != null && Tiles[xy_idx(loc.X, loc.Y)] is T) {
                return (T) Tiles[xy_idx(loc.X, loc.Y)];
            } else return null;
        }

        public TileBase[] GetTileRegion(Point center) {
            int startY = center.Y - 50;
            int startX = center.X - 50;

            TileBase[] region = new TileBase[10201];
            
            for (int y = 0; y < 101; y++) {
                for (int x = 0; x < 101; x++) {
                    region[new Point(x, y).ToIndex(101)] = GetTileAt<TileBase>(x + startX, y + startY);
                }
            }
            return region;
        }
        
        public T GetEntityAt<T>(Point location) where T : Entity {
            return Entities.GetItems(location).OfType<T>().FirstOrDefault();
        }

        public List<T> GetEntitiesAt<T>(Point location) where T : Entity {
            return Entities.GetItems(location).OfType<T>().ToList<T>();
        }

        public void Remove(Entity entity) {
            Entities.Remove(entity);
            
            entity.Moved -= OnEntityMoved;
        }
        
        public void Add(Entity entity) {
            Entities.Add(entity, entity.Position);
            
            entity.Moved += OnEntityMoved;
        }
        
        private void OnEntityMoved(object sender, Entity.EntityMovedEventArgs args) {
            Entities.Move(args.Entity as Entity, args.Entity.Position);
            
            if (GameLoop.World.players.ContainsKey(GameLoop.NetworkingManager.myUID)) {
                GameLoop.UIManager.CenterOnActor(GameLoop.World.players[GameLoop.NetworkingManager.myUID]);
            }
        }

        public void SetTile(Point pos, TileBase tile) {
            Tiles[xy_idx(pos.X, pos.Y)] = tile;
        }

        public int xy_idx(int x, int y) {
            return (y * Width) + x;
        }

        public Point idx_xy(int idx) {
            return new Point(idx / Width, idx % Width);
        }


        public void PlaceTrees(int num) {
            TerrainFeature tree = new TerrainFeature(Color.SaddleBrown, Color.Transparent, "tree", (char)272, true, true, 1000, 100, 2, 2, Color.Green, (char)273, null);

            for (int i = 0; i < num; i++) {
                TerrainFeature treeCopy = tree.Clone();

                treeCopy.Position = (GameLoop.Random.Next(0, Width * Height)).ToPoint(Width);

                if (GetEntityAt<TerrainFeature>(treeCopy.Position) == null && GetTileAt<TileBase>(treeCopy.Position.X, treeCopy.Position.Y).Name == "grass")
                   Add(treeCopy);
            }
        }


        public void PlaceFlowers(int num) {
            for (int i = 0; i < num; i++) {
                int flowerType = GameLoop.Random.Next(0, 5);

                switch (flowerType) {
                    case 0:
                        TerrainFeature cornflower = new TerrainFeature(Color.CornflowerBlue, Color.Transparent, "cornflower", (char)266, false, false, 0.01, 100, 1, 1, Color.Green, (char) 282);
                        cornflower.Position = (GameLoop.Random.Next(0, Width * Height)).ToPoint(Width);
                        if (GetEntityAt<TerrainFeature>(cornflower.Position) == null && GetTileAt<TileBase>(cornflower.Position.X, cornflower.Position.Y).Name == "grass")
                            Add(cornflower);
                        break;
                    case 1:
                        TerrainFeature rose = new TerrainFeature(Color.Red, Color.Transparent, "rose", (char)268, false, false, 0.01, 100, 1, 1, Color.Green, (char)284);
                        rose.Position = (GameLoop.Random.Next(0, Width * Height)).ToPoint(Width);
                        if (GetEntityAt<TerrainFeature>(rose.Position) == null && GetTileAt<TileBase>(rose.Position.X, rose.Position.Y).Name == "grass")
                            Add(rose);
                        break;
                    case 2:
                        TerrainFeature violet = new TerrainFeature(Color.Purple, Color.Transparent, "violet", (char)268, false, false, 0.01, 100, 1, 1, Color.Green, (char)284);
                        violet.Position = (GameLoop.Random.Next(0, Width * Height)).ToPoint(Width);
                        if (GetEntityAt<TerrainFeature>(violet.Position) == null && GetTileAt<TileBase>(violet.Position.X, violet.Position.Y).Name == "grass")
                            Add(violet);
                        break;
                    case 3:
                        TerrainFeature dandelion = new TerrainFeature(Color.Yellow, Color.Transparent, "dandelion", (char)267, false, false, 0.01, 100, 1, 1, Color.Green, (char)283);
                        dandelion.Position = (GameLoop.Random.Next(0, Width * Height)).ToPoint(Width);
                        if (GetEntityAt<TerrainFeature>(dandelion.Position) == null && GetTileAt<TileBase>(dandelion.Position.X, dandelion.Position.Y).Name == "grass")
                            Add(dandelion);
                        break;
                    default:
                        TerrainFeature tulip = new TerrainFeature(Color.HotPink, Color.Transparent, "tulip", (char)266, false, false, 0.01, 100, 1, 1, Color.Green, (char)282);
                        tulip.Position = (GameLoop.Random.Next(0, Width * Height)).ToPoint(Width);
                        if (GetEntityAt<TerrainFeature>(tulip.Position) == null && GetTileAt<TileBase>(tulip.Position.X, tulip.Position.Y).Name == "grass")
                            Add(tulip);
                        break;
                }
            }
        }


        public bool IsTransparent (Coord pos) {
            Point position = new Point(pos.X, pos.Y);


            TerrainFeature terrain = GetEntityAt<TerrainFeature>(position);

            if (terrain != null) {
                return !GetTileAt<TileBase>(position).IsBlockingLOS && !terrain.IsBlockingLOS;
            }

            return !GetTileAt<TileBase>(position).IsBlockingLOS;
        }

        public TileBase GenerateTile(float noise) {
            if (noise < 128) {
                return GameLoop.TileLibrary["grass"];
            } else {
                return GameLoop.TileLibrary["grass"];
            }
        }
    }
}
