﻿using System;
using SadConsole;
using Console = SadConsole.Console;
using SadRogue.Primitives;
using Microsoft.Xna.Framework.Graphics;

using LofiHollow.UI;
using LofiHollow.Commands;
using LofiHollow.Entities;

namespace LofiHollow {
    class GameLoop {
        public const int GameWidth = 100;
        public const int GameHeight = 60;  
        public const int MapWidth = 70;
        public const int MapHeight = 40;

        public static UIManager UIManager;
        public static World World;
        public static CommandManager CommandManager;
        public static BattleManager BattleManager;


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
            World.Player.TimeLastTicked++;
            if (World.Player.TimeLastTicked >= 60) {
                World.Player.TimeLastTicked = 0;
                World.Player.TickTime();
            }
        }

        private static void Init() { 
            rand = new Random();
            World = new World();
            UIManager = new UIManager();
            UIManager.Init();

            
            CommandManager = new CommandManager();
            BattleManager = new BattleManager();


            World.LoadExistingMaps();

            World.InitPlayer();
             

            SadConsole.Game.Instance.MonoGameInstance.Window.Title = "Lofi Hollow";
        }
    }
}
