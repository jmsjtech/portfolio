using System;
using SadConsole.Components;
using SadRogue.Primitives;
using System.Collections.Generic;

using LofiHollow.Entities;
using System.IO;
using Newtonsoft.Json;
using LofiHollow.Entities.NPC;

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
        public Dictionary<int, NPC> npcLibrary = new Dictionary<int, NPC>();
        public Dictionary<string, Template> templateLibrary = new Dictionary<string, Template>();


        public bool DoneInitializing = false;
         
        public Player Player { get; set; }

         
        public World() {
            LoadTemplateDefinitions();
            LoadSkillDefinitions(); 
            LoadTileDefinitions();
            LoadItemDefinitions(); 
            LoadRaceDefinitions();
            LoadClassDefinitions(); 
            LoadMonsterDefinitions();
            LoadNPCDefinitions();
        }

        public void InitPlayer() {
            CreatePlayer();
            CreateMap(Player.MapPos);

            if (Directory.Exists("./data/")) {
                string json = System.IO.File.ReadAllText("./data/minimap.dat");
                Dictionary<string, MinimapTile> minimap = JsonConvert.DeserializeObject<Dictionary<string, MinimapTile>>(json);

                foreach (KeyValuePair<string, MinimapTile> kv in minimap) {
                    string posString = kv.Key.Substring(1, kv.Key.Length - 2);
                    string[] coords = posString.Split(",");
                    Point3D pos = new Point3D(Int32.Parse(coords[0]), Int32.Parse(coords[1]), Int32.Parse(coords[2]));
                    GameLoop.UIManager.minimap.Add(pos, kv.Value);
                }
            } 
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

        public void LoadTemplateDefinitions() {
            if (Directory.Exists("./data/templates/")) {
                string[] tileFiles = Directory.GetFiles("./data/templates/");

                foreach (string fileName in tileFiles) {
                    string json = File.ReadAllText(fileName);

                    Template template = JsonConvert.DeserializeObject<Template>(json);

                    templateLibrary.Add(template.Name, template);
                }
            }
        }

        public void LoadNPCDefinitions() {
            if (Directory.Exists("./data/npcs/")) {
                string[] tileFiles = Directory.GetFiles("./data/npcs/");

                foreach (string fileName in tileFiles) {
                    string json = File.ReadAllText(fileName);

                    NPC npc = JsonConvert.DeserializeObject<NPC>(json);

                    npcLibrary.Add(npc.npcID, npc);
                }
            }

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

        public bool LoadMapAt(Point3D mapPos) {
            if (Directory.Exists("./data/maps/")) {
                string json = System.IO.File.ReadAllText("./data/maps/" + mapPos.X + "," + mapPos.Y + "," + mapPos.Z + ".dat");
                Map map = JsonConvert.DeserializeObject<Map>(json);
                maps.Add(mapPos, map);
                return true;
            }

            return false;
        }

        public void LoadExistingMaps() {
            if (Directory.Exists("./data/maps/")) {
                string[] mapFiles = Directory.GetFiles("./data/maps/");

                foreach (string fileName in mapFiles) {
                    string json = File.ReadAllText(fileName);
                    Map map = JsonConvert.DeserializeObject<Map>(json);

                    string[] strings = fileName.Split("/");
                    string posString = strings[3].Substring(0, strings[3].Length - 4);

                    int x = Int32.Parse(posString.Split(',')[0]);
                    int y = Int32.Parse(posString.Split(',')[1]);
                    int z = Int32.Parse(posString.Split(',')[2]);

                    maps.Add(new Point3D(x, y, z), map);
                     
                }
            } else {
                System.Console.WriteLine("Failed to load map directory");
            }
        }

        public void SaveMapToFile(Map map, Point3D pos) {
            string path = "./data/maps/" + pos.X + "," + pos.Y + "," + pos.Z + ".dat";

            using (StreamWriter output = new StreamWriter(path)) {
                string jsonString = JsonConvert.SerializeObject(map, Formatting.Indented);
                output.WriteLine(jsonString);
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


            GameLoop.UIManager.LoadMap(Player.MapPos, false);
            GameLoop.UIManager.EntityRenderer.Add(Player); 
        }



        public void CreateMap(Point3D pos) {
            if (!maps.ContainsKey(pos)) {
                if (!LoadMapAt(pos)) {
                    Map newMap = new Map(GameLoop.MapWidth, GameLoop.MapHeight);

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
        } 

        private void CreatePlayer() {
            Player = new Player(Color.Yellow);
            Player.Position = new Point(25, 25);
            Player.MapPos = new Point3D(3, 1, 0);
            Player.Name = "Player";


            GameLoop.UIManager.LoadMap(Player.MapPos, true);
            GameLoop.UIManager.EntityRenderer.Add(Player);
            Player.ZIndex = 10;

            DoneInitializing = true;
                 
            Player.MaxHP = 0;
            Player.CurrentHP = Player.MaxHP;
            
        }

        public void FreshStart() {
            Player.MaxHP = Int32.Parse(Player.ClassLevels[0].HitDie.Split("d")[1]) + Player.GetMod("CON");
            Player.CurrentHP = Player.MaxHP;
            LoadMapAt(Player.MapPos);
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
