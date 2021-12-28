using System;
using SadConsole.Components;
using SadRogue.Primitives;
using System.Collections.Generic;

using LofiHollow.Entities;
using System.IO;
using Newtonsoft.Json;

namespace LofiHollow {
    public class World { 
        public Dictionary<int, TileBase> tileLibrary = new Dictionary<int, TileBase>();
        public Dictionary<int, Item> itemLibrary = new Dictionary<int, Item>();
        public Dictionary<int, Monster> monsterLibrary = new Dictionary<int, Monster>();
        public Dictionary<int, Move> moveLibrary = new Dictionary<int, Move>();
        public Dictionary<Point3D, Map> maps = new Dictionary<Point3D, Map>();
        public Dictionary<int, ClassDef> classLibrary = new Dictionary<int, ClassDef>();
        public Dictionary<int, Race> raceLibrary = new Dictionary<int, Race>();
        public Dictionary<int, Skill> skillLibrary = new Dictionary<int, Skill>();


        public bool DoneInitializing = false;
         
        public Player Player { get; set; }
         
        public World() {
            LoadSkillDefinitions(); 
            LoadTileDefinitions();
            LoadItemDefinitions();
            LoadMoveDefinitions();
            LoadRaceDefinitions();
            LoadClassDefinitions(); 
            LoadMonsterDefinitions();
             
        }

        public void InitPlayer() {
            CreatePlayer();
            CreateMap(Player.MapPos);

        }

        public void MakeShipWreckage() {
            Item woodBit1 = new Item(1);
            woodBit1.Position = new Point(23, 24);
            maps[new Point3D(3, 1, 0)].Entities.Add(woodBit1, new GoRogue.Coord(woodBit1.Position.X, woodBit1.Position.Y));
            GameLoop.UIManager.EntityRenderer.Add(woodBit1);

            Item woodBit2 = new Item(1);
            woodBit2.Position = new Point(26, 23);
            maps[new Point3D(3, 1, 0)].Entities.Add(woodBit2, new GoRogue.Coord(woodBit2.Position.X, woodBit2.Position.Y));
            GameLoop.UIManager.EntityRenderer.Add(woodBit2);

            Item woodBit3 = new Item(1);
            woodBit3.Position = new Point(25, 22);
            maps[new Point3D(3, 1, 0)].Entities.Add(woodBit3, new GoRogue.Coord(woodBit3.Position.X, woodBit3.Position.Y));
            GameLoop.UIManager.EntityRenderer.Add(woodBit3);

            Item woodBit4 = new Item(1);
            woodBit4.Position = new Point(24, 25);
            maps[new Point3D(3, 1, 0)].Entities.Add(woodBit4, new GoRogue.Coord(woodBit4.Position.X, woodBit4.Position.Y));
            GameLoop.UIManager.EntityRenderer.Add(woodBit4);
        }

        public void LoadItemDefinitions() {
            if (Directory.Exists("./data/items/")) {
                string[] itemFiles = Directory.GetFiles("./data/items/");

                foreach (string fileName in itemFiles) {
                    string json = File.ReadAllText(fileName);

                    Item item = JsonConvert.DeserializeObject<Item>(json);

                    itemLibrary.Add(item.ItemID, item);
                }
            } 
        }
         

        public void LoadTileDefinitions() {
            if (Directory.Exists("./data/tiles/")) {
                string[] tileFiles = Directory.GetFiles("./data/tiles/");

                foreach (string fileName in tileFiles) {
                    string json = File.ReadAllText(fileName);

                    TileBase tile = JsonConvert.DeserializeObject<TileBase>(json);

                    tileLibrary.Add(tile.TileID, tile);
                }
            }
            /*
            string path = "./data/tiles/" + tile.TileID + "," + tile.Name + ".dat";

            using (StreamWriter output = new StreamWriter(path)) {
                string jsonString = JsonConvert.SerializeObject(tile, Formatting.Indented);
                output.WriteLine(jsonString);
            }*/
        }

        public void LoadSkillDefinitions() { 
            if (Directory.Exists("./data/skills/")) {
                string[] skillFiles = Directory.GetFiles("./data/skills/");

                foreach (string fileName in skillFiles) {
                    string json = File.ReadAllText(fileName);

                    Skill skill = JsonConvert.DeserializeObject<Skill>(json);

                    skillLibrary.Add(skill.SkillID, skill);
                }
            } 
        }

        public void LoadRaceDefinitions() {
            if (Directory.Exists("./data/races/")) {
                string[] tileFiles = Directory.GetFiles("./data/races/");

                foreach (string fileName in tileFiles) {
                    string json = File.ReadAllText(fileName);

                    Race race = JsonConvert.DeserializeObject<Race>(json);

                    raceLibrary.Add(race.RaceID, race);
                }
            }
        }

        public void LoadClassDefinitions() {
            if (Directory.Exists("./data/classes/")) {
                string[] tileFiles = Directory.GetFiles("./data/classes/");

                foreach (string fileName in tileFiles) {
                    string json = File.ReadAllText(fileName);

                    ClassDef classDef = JsonConvert.DeserializeObject<ClassDef>(json);

                    classLibrary.Add(classDef.ClassID, classDef);
                }
            } 
        }

        public void LoadMonsterDefinitions() {
            if (Directory.Exists("./data/monsters/")) {
                string[] monsterFiles = Directory.GetFiles("./data/monsters/");

                foreach (string fileName in monsterFiles) {
                    string json = File.ReadAllText(fileName);

                    Monster monster = JsonConvert.DeserializeObject<Monster>(json);

                    monsterLibrary.Add(monster.MonsterID, monster);
                }
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
                            tile = new TileBase(tileLibrary[Int32.Parse(line)]);
                            
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

        public void SavePlayer() {
            System.IO.Directory.CreateDirectory("./saves/" + Player.Name + "/");
            string path = "./saves/" + Player.Name + "/player.dat";

            using (StreamWriter output = new StreamWriter(path)) {
                string jsonString = JsonConvert.SerializeObject(Player, Formatting.Indented);
                output.WriteLine(jsonString);
            }
        }

        public void LoadPlayer(string playerName) {  

            if (Directory.Exists("./saves/" + playerName + "/")) {
                string[] monsterFiles = Directory.GetFiles("./saves/" + playerName + "/");

                foreach (string fileName in monsterFiles) {
                    string json = File.ReadAllText(fileName);

                    string[] name = fileName.Split("/");

                    if (name[name.Length - 1] == "player.dat") {
                        Player = JsonConvert.DeserializeObject<Player>(json);
                    } 
                }
            }


            GameLoop.UIManager.LoadMap(maps[Player.MapPos], false);
            GameLoop.UIManager.EntityRenderer.Add(Player); 
        }



        public void CreateMap(Point3D pos) {
            if (!maps.ContainsKey(pos)) {
                Map newMap  = new Map(GameLoop.MapWidth, GameLoop.MapHeight);

                if (pos.Z < 0) {
                    for (int i = 0; i < newMap.Tiles.Length; i++) {  
                        newMap.Tiles[i] = new TileBase(31);
                    }

                    if (Player.MapPos.Z > pos.Z) { 
                        newMap.Tiles[Player.Position.ToIndex(GameLoop.MapWidth)] = new TileBase(29);
                    }
                }

                if (pos.Z > 0) {
                    for (int i = 0; i < newMap.Tiles.Length; i++) { 
                        newMap.Tiles[i] = new TileBase(32);
                    }

                    if (Player.MapPos.Z < pos.Z) {  
                        newMap.Tiles[Player.Position.ToIndex(GameLoop.MapWidth)] = new TileBase(30);
                    }
                }

                maps.Add(pos, newMap);
            }
        } 

        private void CreatePlayer() {
            Player = new Player(Color.Yellow);
            Player.Position = new Point(25, 25);
            Player.MapPos = new Point3D(3, 1, 0);
            Player.Name = "Player"; 
 
            GameLoop.UIManager.LoadMap(maps[Player.MapPos], true);
            GameLoop.UIManager.EntityRenderer.Add(Player);
            Player.ZIndex = 10;

            DoneInitializing = true;
                 
            Player.MaxHP = 0;
            Player.CurrentHP = Player.MaxHP;
            
        }

        public void FreshStart() {
            Player.MaxHP = Int32.Parse(Player.ClassLevels[0].HitDie.Split("d")[1]) + Player.GetMod("CON");
            Player.CurrentHP = Player.MaxHP;
            MakeShipWreckage();

            Player.Inventory[0] = new Item(36);

            Player.Skills = new Dictionary<string, Skill>();
            
            for (int i = 0; i < skillLibrary.Count; i++) {
                Skill skill = new Skill(skillLibrary[i]);
                Player.Skills.Add(skill.Name, skill);
            }
        }
    }
}
