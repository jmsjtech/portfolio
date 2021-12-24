using System;
using System.Collections.Generic;
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

        public int BaseVitality = 10;
        public int BaseSpeed = 10;
        public int BaseAttack = 10;
        public int BaseDefense = 10;
        public int BaseMagicAttack = 10;
        public int BaseMagicDefense = 10;

        public int ExpGranted = 0;
        public int AverageStrength = 0;
        public int MaxStrength = 0;
        public int MinStrength = 0;

        public int Gold = 0;
        public int Level = 1;
        public int Experience = 0;
        public int ExpToNext = 100;
        public string Descriptor = "";

        public int CritChance = 1; // Out of 20

        public double TimeLastActed = 0;
        public Item[] Inventory = new Item[9];
        public Item[] Equipment = new Item[8];

        public List<int> KnownMoves = new List<int>();
        public List<ItemDrop> DropTable = new List<ItemDrop>();


        public Point3D MapPos = new Point3D(0, 0, 0);

        protected Actor(Color foreground, Color background, int glyph, int width = 1, int height = 1) : base(foreground, background, glyph) {
            Appearance.Foreground = foreground;
            Appearance.Background = background;
            Appearance.Glyph = glyph;

            KnownMoves.Add(0);

            for (int i = 0; i < Inventory.Length; i++) {
                Inventory[i] = new Item(0);
            }

            for (int i = 0; i < Equipment.Length; i++) {
                Equipment[i] = new Item(0);
            }
        }

        public void RecalculateHP() {
            MaxHP = (int) Math.Floor(0.01 * (2 * Vitality) * Level) + Level + 10;
        }

        public void RecalculateEXP() {
            ExpToNext = (int) (75 * Math.Pow(2, (double) Level / (double) 7));
        }

        public int StatTotal() {
            return (Vitality + Speed + Attack + Defense + MagicAttack + MagicDefense);
        }

        public Move PickKnownMove() {
            int random = GameLoop.rand.Next(KnownMoves.Count);

            if (GameLoop.World.moveLibrary.ContainsKey(KnownMoves[random])) {
                return GameLoop.World.moveLibrary[KnownMoves[random]];
            }

            return new Move(0);
        }

        public bool MoveBy(Point positionChange) {
            if (TimeLastActed + (100 - BaseSpeed) > SadConsole.GameHost.Instance.GameRunningTotalTime.TotalMilliseconds) {
                return false;
            }

            TimeLastActed = SadConsole.GameHost.Instance.GameRunningTotalTime.TotalMilliseconds;

            if (GameLoop.World.maps.TryGetValue(MapPos, out Map map)) {
                Point newPosition = Position + positionChange;
                if (newPosition.Y < 0 && GameLoop.World.maps.ContainsKey(MapPos - new Point3D(0, -1, 0)) && GameLoop.World.maps[MapPos - new Point3D(0, -1, 0)].MinimapTile.name == "Desert") {
                    GameLoop.UIManager.MessageLog.Add("There's dangerous sandstorms that way, best not go there for now.");
                    return false;
                }

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
                       // GameLoop.CommandManager.Attack(this, monster);
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
                                int monsterID = GameLoop.BattleManager.GetEncounter(map.MinimapTile.name, map.Tiles[Position.ToIndex(GameLoop.MapWidth)].Name);
                                if (monsterID != -1) {
                                    if (GameLoop.World.monsterLibrary.ContainsKey(monsterID)) {
                                        Monster temp = GameLoop.World.monsterLibrary[monsterID];
                                        Monster mon = new Monster(temp.MonsterID, temp.Appearance.Foreground, temp.Appearance.Glyph);
                                        
                                        for (int i = 0; i < temp.DropTable.Count; i++) {
                                            mon.DropTable.Add(temp.DropTable[i]);
                                        }

                                        for (int i = 0; i < temp.KnownMoves.Count; i++) {
                                            mon.KnownMoves.Add(temp.KnownMoves[i]);
                                        }

                                        GameLoop.BattleManager.StartBattle(mon);
                                    } else {
                                        GameLoop.UIManager.MessageLog.Add("Monster ID wasn't in library.");
                                    }
                                } else {
                                    GameLoop.UIManager.MessageLog.Add("Failed to get an encounter for (" + map.MinimapTile.name + ") [" +  map.Tiles[Position.ToIndex(GameLoop.MapWidth)].Name + "]");
                                }
                                
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
