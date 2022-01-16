using System;
using SadConsole;
using Console = SadConsole.Console;
using SadRogue.Primitives;
using Microsoft.Xna.Framework.Graphics;

using LofiHollow.UI;
using LofiHollow.Commands;
using LofiHollow.Entities;
using System.Collections.Generic;
using System.Linq;

namespace LofiHollow {
    class GameLoop {
        public const int GameWidth = 100;
        public const int GameHeight = 60;  
        public const int MapWidth = 70;
        public const int MapHeight = 40;

        public static UIManager UIManager;
        public static World World;
        public static CommandManager CommandManager;

        public static NetworkManager NetworkManager;


        public static Random rand;

        public static void Main(string[] args) {
            // Setup the engine and create the main window.
            SadConsole.Game.Create(GameWidth, GameHeight, "./fonts/Cheepicus48.font");
             

            // Hook the start event so we can add consoles to the system.
            GameHost.Instance.OnStart = Init;
            GameHost.Instance.FrameUpdate += Update;
            
            // Start the game.
            SadConsole.Game.Instance.Run(); 
            SadConsole.Game.Instance.Dispose(); 
        } 

        private static void Update(object sender, GameHost e) {
            if (UIManager != null) {
                if (NetworkManager == null || NetworkManager.lobbyManager == null || NetworkManager.isHost) {
                    World.Player.TimeLastTicked++;
                    if (World.Player.TimeLastTicked >= 60) {
                        World.Player.TimeLastTicked = 0;
                        World.Player.Clock.TickTime();
                    }
                }

                List<Point3D> UpdatedMaps = new List<Point3D>();

                if (World.maps[World.Player.MapPos].Entities.Count > 0) {
                    var entities = World.maps[World.Player.MapPos].Entities.Items.ToList();
                    foreach (Entity ent in entities) {
                        if (ent is Monster) {
                            ((Monster)ent).Update();
                        }
                    } 

                    UpdatedMaps.Add(World.Player.MapPos);
                }
                 
                foreach (KeyValuePair<long, Player> kv in World.otherPlayers) {
                    if (!UpdatedMaps.Contains(kv.Value.MapPos)) {
                        if (World.maps[kv.Value.MapPos].Entities.Count > 0) {
                            foreach (Entity ent in World.maps[kv.Value.MapPos].Entities.Items) {
                                if (ent is Monster) {
                                    ((Monster)ent).Update();
                                }
                            }
                            UpdatedMaps.Add(kv.Value.MapPos);
                        }
                    }
                } 

                UpdatedMaps.Clear(); 
            }
        }

        private static void Init() { 
            rand = new Random();
            World = new World();
            UIManager = new UIManager();
            UIManager.Init();

            
            CommandManager = new CommandManager();

            //  World.LoadExistingMaps();
            World.LoadMapAt(new Point3D(1, 3, 0));
            World.InitPlayer(); 

            SadConsole.Game.Instance.MonoGameInstance.Window.Title = "Lofi Hollow";


        }
    }
}
