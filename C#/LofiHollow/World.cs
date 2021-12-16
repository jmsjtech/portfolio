using System;
using SadConsole.Components;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using LofiHollow.Entities;
using System.IO;

namespace LofiHollow {
    public class World { 
        public Dictionary<int, TileBase> tileLibrary = new Dictionary<int, TileBase>();
        public int _mapWidth = 70;
        public int _mapHeight = 40;

        public Dictionary<Point, Map> maps = new Dictionary<Point, Map>();
              
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
            CreateMonsters();
        }

        public void LoadTileDefinitions() {
            string[] lines = File.ReadAllLines("./tiles.dat");

            foreach (string line in lines) {
                TileBase tile = new TileBase(Color.Green, Color.Black, ',');

                string[] header = line.Split('|');
                int id = Int32.Parse(header[0]);
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

                tileLibrary.Add(id, tile);
            }
        }

        public void LoadExistingMaps() {
            if (Directory.Exists("./maps/")) {
                string[] mapFiles = Directory.GetFiles("./maps/");

                foreach (string fileName in mapFiles) {
                    string posString = fileName.Substring(7, fileName.Length - 11);
                    int x = Int32.Parse(posString.Split(',')[0]);
                    int y = Int32.Parse(posString.Split(',')[1]);

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

                        } else {
                            tile = tileLibrary[Int32.Parse(line)];

                            newMap.Tiles[index] = tile;
                        }
                        index++;

                    }

                    System.Console.WriteLine("Loaded a map");

                    maps.Add(new Point(x, y), newMap);
                }
            } else {
                System.Console.WriteLine("Failed to load map directory");
            }
        }

        public void SaveMapToFile(Map map, Point pos) {
            string path = "./maps/" + pos.X + "," + pos.Y + ".dat";

            using (StreamWriter output = new StreamWriter(path)) {
                MinimapTile minimap = map.MinimapTile;
                output.WriteLine(minimap.name + "|" + minimap.ch + "|" + minimap.fg.R + "|" + minimap.fg.G + "|" + minimap.fg.B + "|" + minimap.bg.R + "|" + minimap.bg.G + "|" + minimap.bg.B);
                foreach (TileBase tile in map.Tiles) {
                    output.WriteLine(tile.TileID);
                }
            }
        }
         
        public void CreateMap(Point pos) {
            if (!maps.ContainsKey(pos)) {
                Map newMap  = new Map(_mapWidth, _mapHeight);
                maps.Add(pos, newMap);
            }
        } 

        private void CreatePlayer() {
            Player = new Player(Color.Yellow, Color.Transparent);
            Player.Position = new Point(5, 5); 
            Player.Components.Add(new EntityViewSyncComponent());
        }

        private void CreateMonsters() { 
            int numMonsters = 10;
             
            Random rndNum = new Random();
             
            for (int i = 0; i < numMonsters; i++) {
                int monsterPosition = 0;
                Monster newMonster = new Monster(Color.Blue, Color.Transparent);
                newMonster.Components.Add(new EntityViewSyncComponent());

                monsterPosition = rndNum.Next(0, maps[Player.MapPos].Tiles.Length);

                newMonster.Name = "troll";

                newMonster.Position = SadConsole.Helpers.GetPointFromIndex(monsterPosition, _mapWidth);
                maps[Player.MapPos].Add(newMonster);
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
