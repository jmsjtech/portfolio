using System;
using SadConsole.Components;
using SadRogue.Primitives;
using System.Collections.Generic;

using LofiHollow.Entities;
using System.IO;

namespace LofiHollow {
    public class World { 
        public Dictionary<int, TileBase> tileLibrary = new Dictionary<int, TileBase>();
        public Dictionary<int, Item> itemLibrary = new Dictionary<int, Item>();
        public Dictionary<int, Monster> monsterLibrary = new Dictionary<int, Monster>();
        public Dictionary<int, Move> moveLibrary = new Dictionary<int, Move>();
        public Dictionary<Point3D, Map> maps = new Dictionary<Point3D, Map>(); 

        
              
        public int Hours = 11;
        public int Minutes = 30;
        public int Month = 12;
        public int Day = 28;
        public int Year = 1;
        public bool AM = false;

        public double TimeLastTicked = 0;
        public bool DoneInitializing = false;
         
        public Player Player { get; set; }
         
        public World() {
            LoadTileDefinitions();
            LoadItemDefinitions();
            LoadMoveDefinitions();
            LoadMonsterDefinitions();
            
            LoadExistingMaps(); 
        }

        public void InitPlayer() {
            CreatePlayer(true);
            CreateMap(Player.MapPos);
        }

        public void LoadItemDefinitions() {
            string[] lines = File.ReadAllLines("./items.dat");

            foreach (string line in lines) {
                string[] header = line.Split('|');
                if (header[0][0] == '#') {
                    continue;
                }

                int ItemID = Int32.Parse(header[0]);
                string Name = header[1];
                int Glyph = (char) Int32.Parse(header[2]);
                
                int fgR = Int32.Parse(header[3]);
                int fgG = Int32.Parse(header[4]);
                int fgB = Int32.Parse(header[5]); 

                Color Foreground = new Color(fgR, fgG, fgB); 

                int ItemCategory = Int32.Parse(header[6]);
                int EquipSlot = Int32.Parse(header[7]);
                bool IsStackable = header[8] == "true" ? true : false;
                int numericalBonus = Int32.Parse(header[9]);

                Item item = new Item(Foreground, Color.Black, Glyph, Name, ItemID, ItemCategory, EquipSlot, IsStackable, numericalBonus);

                itemLibrary.Add(item.ItemID, item);
            }
        }

        public void LoadMoveDefinitions() {
            string[] lines = File.ReadAllLines("./moves.dat");

            foreach (string line in lines) {
                string[] header = line.Split('|');
                if (header[0][0] == '#') {
                    continue;
                }

                int MoveID = Int32.Parse(header[0]);
                string Name = header[1];
                string Type = header[2];
                bool IsPhysical = header[3] == "true" ? true : false;

                int Cost = Int32.Parse(header[4]);
                int Acc = Int32.Parse(header[5]);
                int Power = Int32.Parse(header[6]); 
                 

                Move move = new Move(MoveID, Name, Type, IsPhysical, Cost, Acc, Power);

                moveLibrary.Add(move.MoveID, move);
            }
        }

        public void LoadTileDefinitions() {
            string[] lines = File.ReadAllLines("./tiles.dat");

            foreach (string line in lines) {
                TileBase tile = new TileBase(Color.Green, Color.Black, ',');

                string[] header = line.Split('|');

                if (header[0][0] == '#') {
                    continue;
                }

                tile.TileID = Int32.Parse(header[0]);
                tile.Name = header[1];
                tile.IsBlockingMove = header[2] == "true" ? true : false;

                tile.Glyph = Int32.Parse(header[3]);


                int fgR = Int32.Parse(header[4]);
                int fgG = Int32.Parse(header[5]);
                int fgB = Int32.Parse(header[6]);

                int bgR = Int32.Parse(header[7]);
                int bgG = Int32.Parse(header[8]);
                int bgB = Int32.Parse(header[9]);

                tile.Foreground = new Color(fgR, fgG, fgB);
                tile.Background = new Color(bgR, bgG, bgB);

                tile.SpawnsMonsters = header[10] == "true" ? true : false;

                tileLibrary.Add(tile.TileID, tile);
            }
        }

        public void LoadMonsterDefinitions() {
            string[] lines = File.ReadAllLines("./monsters.dat");

            foreach (string line in lines) {
                string[] header = line.Split('|');
                if (header[0][0] == '#') {
                    continue;
                }

                int monID = Int32.Parse(header[0]);
                string name = header[1];

                int vitality = Int32.Parse(header[2]);
                int speed = Int32.Parse(header[3]);
                int attack = Int32.Parse(header[4]);
                int defense = Int32.Parse(header[5]);
                int magicAtk = Int32.Parse(header[6]);
                int magicDef = Int32.Parse(header[7]);

                int glyph = Int32.Parse(header[8]);


                int fgR = Int32.Parse(header[9]);
                int fgG = Int32.Parse(header[10]);
                int fgB = Int32.Parse(header[11]);
                Color fg = new Color(fgR, fgG, fgB);

                Monster monster = new Monster(fg, glyph, monID, name, vitality, speed, attack, defense, magicAtk, magicDef);


                string[] allDrops = header[12].Split(";");
                for (int i = 0; i < allDrops.Length; i++) {
                    string[] thisDrop = allDrops[i].Split(",");

                    int dropID = Int32.Parse(thisDrop[0]);
                    int dropChance = Int32.Parse(thisDrop[1]);
                    int dropQuantity = Int32.Parse(thisDrop[2]);

                    ItemDrop drop = new ItemDrop(dropID, dropChance, dropQuantity);

                    monster.DropTable.Add(drop);
                }

                string[] allMoves = header[13].Split(",");
                for (int i = 0; i < allMoves.Length; i++) {
                    if (moveLibrary.ContainsKey(Int32.Parse(allMoves[i]))) {
                        monster.KnownMoves.Add(Int32.Parse(allMoves[i]));
                    }
                }



                monsterLibrary.Add(monster.MonsterID, monster);
            }
        }

        public void LoadExistingMaps() {
            if (Directory.Exists("./maps/")) {
                string[] mapFiles = Directory.GetFiles("./maps/");

                foreach (string fileName in mapFiles) {
                    string posString = fileName.Substring(7, fileName.Length - 11);
                    int x = Int32.Parse(posString.Split(',')[0]);
                    int y = Int32.Parse(posString.Split(',')[1]);
                    int z = Int32.Parse(posString.Split(',')[2]);
                    bool Resave = false;
                     
                    string[] lines = File.ReadAllLines(fileName);

                    Map newMap = new Map(70, 40);
                    int index = -1;

                    foreach (string line in lines) {
                        TileBase tile;

                        if (index < 0) {
                            string[] header = line.Split('|');
                            newMap.MinimapTile.name = header[0];
                            newMap.MinimapTile.ch = header[1][0];

                            int fgR = Int32.Parse(header[2]);
                            int fgG = Int32.Parse(header[3]);
                            int fgB = Int32.Parse(header[4]);

                            int bgR = Int32.Parse(header[5]);
                            int bgG = Int32.Parse(header[6]);
                            int bgB = Int32.Parse(header[7]);

                            newMap.MinimapTile.fg = new Color(fgR, fgG, fgB);
                            newMap.MinimapTile.bg = new Color(bgR, bgG, bgB);

                            newMap.PlayerCanBuild = header[8] == "true" ? true : false;

                            if (header.Length > 9) {
                                newMap.AmbientMonsters = header[9] == "true" ? true : false;
                            } else { 
                                newMap.AmbientMonsters = false;
                                Resave = true;
                            }
                        } else {
                            TileBase lib = tileLibrary[Int32.Parse(line)];
                            tile = new TileBase(lib.Foreground, lib.Background, lib.Glyph, lib.IsBlockingMove, lib.IsBlockingLOS, lib.Name, lib.TileID, lib.SpawnsMonsters);

                            newMap.Tiles[index] = tile;
                        }
                        index++;

                    }
                    
                    maps.Add(new Point3D(x, y, z), newMap);

                    if (Resave) {
                        GameLoop.UIManager.MessageLog.Add("Resaved map at " + x + ", " + y);
                        SaveMapToFile(newMap, new Point3D(x, y, z));
                    } 
                }
            } else {
                System.Console.WriteLine("Failed to load map directory");
            }
        }

        public void SaveMapToFile(Map map, Point3D pos) {
            string path = "./maps/" + pos.X + "," + pos.Y + "," + pos.Z + ".dat";

            using (StreamWriter output = new StreamWriter(path)) {
                MinimapTile minimap = map.MinimapTile;
                output.WriteLine(minimap.name + "|" + minimap.ch + "|" +
                    minimap.fg.R + "|" + minimap.fg.G + "|" + minimap.fg.B + "|" + 
                    minimap.bg.R + "|" + minimap.bg.G + "|" + minimap.bg.B + "|" + 
                    map.PlayerCanBuild.ToString().ToLower() + "|" + map.AmbientMonsters.ToString().ToLower());
                foreach (TileBase tile in map.Tiles) {
                    output.WriteLine(tile.TileID);
                }
            }

        }
         
        public void CreateMap(Point3D pos) {
            if (!maps.ContainsKey(pos)) {
                Map newMap  = new Map(GameLoop.MapWidth, GameLoop.MapHeight);

                if (pos.Z < 0) {
                    for (int i = 0; i < newMap.Tiles.Length; i++) {
                        TileBase lib = tileLibrary[31];
                        TileBase tile = new TileBase(lib.Foreground, lib.Background, lib.Glyph, lib.IsBlockingMove, lib.IsBlockingLOS, lib.Name, lib.TileID, lib.SpawnsMonsters);

                        newMap.Tiles[i] = tile;
                    }

                    if (Player.MapPos.Z > pos.Z) {
                        TileBase lib = tileLibrary[29];
                        TileBase tile = new TileBase(lib.Foreground, lib.Background, lib.Glyph, lib.IsBlockingMove, lib.IsBlockingLOS, lib.Name, lib.TileID, lib.SpawnsMonsters);

                        newMap.Tiles[Player.Position.ToIndex(GameLoop.MapWidth)] = tile;
                    }
                }

                if (pos.Z > 0) {
                    for (int i = 0; i < newMap.Tiles.Length; i++) {
                        TileBase lib = tileLibrary[32];
                        TileBase tile = new TileBase(lib.Foreground, lib.Background, lib.Glyph, lib.IsBlockingMove, lib.IsBlockingLOS, lib.Name, lib.TileID, lib.SpawnsMonsters);

                        newMap.Tiles[i] = tile;
                    }

                    if (Player.MapPos.Z < pos.Z) {
                        TileBase lib = tileLibrary[30];
                        TileBase tile = new TileBase(lib.Foreground, lib.Background, lib.Glyph, lib.IsBlockingMove, lib.IsBlockingLOS, lib.Name, lib.TileID, lib.SpawnsMonsters);

                        newMap.Tiles[Player.Position.ToIndex(GameLoop.MapWidth)] = tile;
                    }
                }

                maps.Add(pos, newMap);
            }
        } 

        private void CreatePlayer(bool freshStart) {
            Player = new Player(Color.Yellow, Color.Transparent);
            if (freshStart) {
                Player.Position = new Point(25, 25);
                Player.MapPos = new Point3D(3, 1, 0);
                Player.Equipment[4] = new Item(3);
                Player.Equipment[3] = new Item(2);
                Player.Name = "Player";
 
                GameLoop.UIManager.LoadMap(maps[Player.MapPos], true);
                GameLoop.UIManager.EntityRenderer.Add(Player);
                Player.ZIndex = 10;

                DoneInitializing = true;

                Player.Inventory[0] = new Item(1);

                Player.KnownMoves.Add(1);
                Player.KnownMoves.Add(2);
                Player.KnownMoves.Add(3);

                Player.RecalculateHP();
                Player.MaxHP = Player.HitPoints;
                Player.RecalculateEXP();
            }
        }

         
        public void TickTime() {
            Minutes++;
            if (Minutes >= 60) {
                Hours++;
                Minutes = 0;
                if (Hours == 12) {
                    AM = !AM;
                    if (AM) {
                        Day++;
                        if (Day > 28) {
                            Day = 1;
                            Month++;

                            if (Month > 12) {
                                Month = 1;
                                Year++;
                            }
                        }
                    }
                }
                if (Hours > 12) {
                    Hours = 1;
                }
            } 
        }
    }
}
