using System;
using SadConsole.Components;
using SadRogue.Primitives;
using System.Collections.Generic;

using LofiHollow.Entities;
using System.IO;

namespace LofiHollow {
    public class World { 
        public Dictionary<int, TileBase> tileLibrary = new Dictionary<int, TileBase>();
        public Dictionary<Point3D, Map> maps = new Dictionary<Point3D, Map>();

        
              
        public int Hours = 11;
        public int Minutes = 30;
        public int Month = 12;
        public int Day = 28;
        public int Year = 1;
        public bool AM = false;

        public double TimeLastTicked = 0;
         
        public Player Player { get; set; }
         
        public World() {
            LoadTileDefinitions();
            LoadExistingMaps();
            CreatePlayer(); 
            CreateMap(Player.MapPos);

            GameLoop.UIManager.LoadMap(maps[Player.MapPos], true);
            GameLoop.UIManager.EntityRenderer.Add(Player);
        }

        public void LoadTileDefinitions() {
            string[] lines = File.ReadAllLines("./tiles.dat");

            foreach (string line in lines) {
                TileBase tile = new TileBase(Color.Green, Color.Black, ',');

                string[] header = line.Split('|');
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

        private void CreatePlayer() {
            Player = new Player(Color.Yellow, Color.Transparent);
            Player.Position = new Point(5, 5);
            Player.MapPos = new Point3D(0, 1, 0);

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
