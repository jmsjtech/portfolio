using System;
using SadRogue.Primitives; 

namespace LofiHollow.Entities {
    public abstract class Actor : Entity {
        public int HitPoints = 10;  
        public int MaxHP = 10;

        public int Energy = 10;
        public int MaxEnergy = 10;

        public int Mana = 10;
        public int MaxMana = 10;

        public int Vitality = 10;
        public int Speed = 10;
        public int Attack = 10;
        public int Defense = 10;
        public int MagicAttack = 10;
        public int MagicDefense = 10;

        public int Gold = 0;

        public double TimeLastActed = 0;


        public Point3D MapPos = new Point3D(0, 0, 0);

        protected Actor(Color foreground, Color background, int glyph, int width = 1, int height = 1) : base(foreground, background, glyph) {
            Appearance.Foreground = foreground;
            Appearance.Background = background;
            Appearance.Glyph = glyph;

        }

        public bool MoveBy(Point positionChange) {
            if (TimeLastActed + (100 - Speed) > SadConsole.GameHost.Instance.GameRunningTotalTime.TotalMilliseconds) {
                return false;
            }

            TimeLastActed = SadConsole.GameHost.Instance.GameRunningTotalTime.TotalMilliseconds;

            if (GameLoop.World.maps.TryGetValue(MapPos, out Map map)) {
                if (map.GetTile(Position).Name == "Up Slope" && positionChange == new Point(0, -1)) {
                    if (!map.IsTileWalkable(Position + positionChange)) {
                        // This is an up slope, move up a map instead of up on the current map
                        if (!GameLoop.World.maps.ContainsKey(MapPos + new Point3D(0, 0, 1))) { GameLoop.World.CreateMap(MapPos + new Point3D(0, 0, 1)); }
                        MapPos += new Point3D(0, 0, 1);
                    } else {
                        if (!GameLoop.World.maps.ContainsKey(MapPos + new Point3D(0, 0, -1))) { GameLoop.World.CreateMap(MapPos + new Point3D(0, 0, -1)); }
                        MapPos += new Point3D(0, 0, -1);
                    }
                    GameLoop.UIManager.LoadMap(GameLoop.World.maps[MapPos], false);
                    return true;
                }

                if (map.GetTile(Position).Name == "Down Slope" && positionChange == new Point(0, 1)) {
                    if (!map.IsTileWalkable(Position + positionChange)) {
                        // This is an up slope, move up a map instead of up on the current map
                        if (!GameLoop.World.maps.ContainsKey(MapPos + new Point3D(0, 0, 1))) { GameLoop.World.CreateMap(MapPos + new Point3D(0, 0, 1)); }
                        MapPos += new Point3D(0, 0, 1);
                    } else {
                        if (!GameLoop.World.maps.ContainsKey(MapPos + new Point3D(0, 0, -1))) { GameLoop.World.CreateMap(MapPos + new Point3D(0, 0, -1)); }
                        MapPos += new Point3D(0, 0, -1);
                    }
                    GameLoop.UIManager.LoadMap(GameLoop.World.maps[MapPos], false);
                    return true;
                }

                if (map.GetTile(Position).Name == "Left Slope" && positionChange == new Point(-1, 0)) {
                    if (!map.IsTileWalkable(Position + positionChange)) {
                        // This is an up slope, move up a map instead of up on the current map
                        if (!GameLoop.World.maps.ContainsKey(MapPos + new Point3D(0, 0, 1))) { GameLoop.World.CreateMap(MapPos + new Point3D(0, 0, 1)); }
                        MapPos += new Point3D(0, 0, 1);
                    } else {
                        if (!GameLoop.World.maps.ContainsKey(MapPos + new Point3D(0, 0, -1))) { GameLoop.World.CreateMap(MapPos + new Point3D(0, 0, -1)); }
                        MapPos += new Point3D(0, 0, -1);
                    }
                    GameLoop.UIManager.LoadMap(GameLoop.World.maps[MapPos], false);
                    return true;
                }

                if (map.GetTile(Position).Name == "Right Slope" && positionChange == new Point(1, 0)) {
                    if (!map.IsTileWalkable(Position + positionChange)) {
                        // This is an up slope, move up a map instead of up on the current map
                        if (!GameLoop.World.maps.ContainsKey(MapPos + new Point3D(0, 0, 1))) { GameLoop.World.CreateMap(MapPos + new Point3D(0, 0, 1)); }
                        MapPos += new Point3D(0, 0, 1);
                    } else {
                        if (!GameLoop.World.maps.ContainsKey(MapPos + new Point3D(0, 0, -1))) { GameLoop.World.CreateMap(MapPos + new Point3D(0, 0, -1)); }
                        MapPos += new Point3D(0, 0, -1);
                    }
                    GameLoop.UIManager.LoadMap(GameLoop.World.maps[MapPos], false);
                    return true;
                }

                if (map.IsTileWalkable(Position +  positionChange)) {
                    // if there's a monster here,
                    // do a bump attack
                    Monster monster = map.GetEntityAt<Monster>(Position + positionChange);
                    if (monster != null) {
                        GameLoop.CommandManager.Attack(this, monster);
                        return true;
                    }

                    TileBase tile = map.GetTile(Position + positionChange);
                    if (tile.Name == "Sign") {
                        GameLoop.UIManager.SignText(Position + positionChange, MapPos);
                        return true;
                    }

                    Position += positionChange;


                    if (ID == GameLoop.World.Player.ID) {
                        if (map.Tiles[Position.ToIndex(GameLoop.MapWidth)].SpawnsMonsters || map.AmbientMonsters) {
                            if (GameLoop.rand.Next(20) == 0) {
                                GameLoop.UIManager.MessageLog.Add("Monster encounter!");
                            }
                        }
                    }


                    return true;
                }

                if (map.ToggleDoor(Position + positionChange, true)) { 
                    return true;
                }

                Point newPos = Position + positionChange;
                bool movedMaps = false;
                
                if (newPos.X < 0) {
                    if (!GameLoop.World.maps.ContainsKey(MapPos + new Point3D(-1, 0, 0))) { GameLoop.World.CreateMap(MapPos + new Point3D(-1, 0, 0)); }

                    MapPos += new Point3D(-1, 0, 0);
                    Position = new Point(GameLoop.MapWidth-1, newPos.Y);
                    movedMaps = true;
                }
                
                if (newPos.X >= GameLoop.MapWidth) {
                    if (!GameLoop.World.maps.ContainsKey(MapPos + new Point3D(1, 0, 0))) { GameLoop.World.CreateMap(MapPos + new Point3D(1, 0, 0)); }

                    MapPos += new Point3D(1, 0, 0);
                    Position = new Point(0, newPos.Y);
                    movedMaps = true;
                } 
                if (newPos.Y < 0) {
                    if (!GameLoop.World.maps.ContainsKey(MapPos + new Point3D(0, -1, 0))) { GameLoop.World.CreateMap(MapPos + new Point3D(0, -1, 0)); }

                    MapPos += new Point3D(0, -1, 0);
                    Position = new Point(newPos.X, GameLoop.MapHeight - 1);
                    movedMaps = true;
                } 
                
                if (newPos.Y >= GameLoop.MapHeight) {
                    if (!GameLoop.World.maps.ContainsKey(MapPos + new Point3D(0, 1, 0))) { GameLoop.World.CreateMap(MapPos + new Point3D(0, 1, 0)); }

                    MapPos += new Point3D(0, 1, 0);
                    Position = new Point(newPos.X, 0);
                    movedMaps = true;
                } 

                if (movedMaps && ID == GameLoop.World.Player.ID) {
                    GameLoop.UIManager.LoadMap(GameLoop.World.maps[MapPos], false);
                    return true;
                }
            }
            return false;
        }

        public bool MoveTo(Point newPosition, Point3D mapLoc) {
            bool movedMaps = false;
            Position = newPosition;

            if (MapPos != mapLoc) { movedMaps = true; }

            if (!GameLoop.World.maps.ContainsKey(mapLoc)) { GameLoop.World.CreateMap(mapLoc); }
            MapPos = mapLoc;

            if (movedMaps && ID == GameLoop.World.Player.ID) {
                GameLoop.UIManager.LoadMap(GameLoop.World.maps[MapPos], false);
            }

            return true;
        }
    }
}
