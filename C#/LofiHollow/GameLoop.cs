using System;
using SadConsole;
using Console = SadConsole.Console;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using LofiHollow.UI;
using LofiHollow.Commands;
using LofiHollow.Entities;

namespace LofiHollow {
    class GameLoop {
        public const int GameWidth = 100;
        public const int GameHeight = 60; 
        public const int WindowWidthPixels = GameWidth * 12;
        public const int WindowHeightPixels = GameHeight * 12;

        public static UIManager UIManager;
        public static World World;
        public static CommandManager CommandManager;

        public static UInt32 TimeSinceLaunch = 0;

        public static void Main(string[] args) {
            // Setup the engine and create the main window.
            SadConsole.Game.Create(GameWidth, GameHeight);

            // Hook the start event so we can add consoles to the system.
            SadConsole.Game.OnInitialize = Init;
            SadConsole.Game.OnUpdate = Update;

            // Start the game.
            SadConsole.Game.Instance.Run();
            SadConsole.Game.Instance.Dispose();
        }

        private static void Update(GameTime time) {
            TimeSinceLaunch++;
            World.TimeLastTicked++;
            if (World.TimeLastTicked >= 60) {
                World.TimeLastTicked = 0;
                World.TickTime();
            }
        }

        private static void Init() {
            SadConsole.FontMaster fontMaster = SadConsole.Global.LoadFont("./fonts/Cheepicus48.font");
            SadConsole.Global.FontDefault = fontMaster.GetFont(SadConsole.Font.FontSizes.Quarter);

            SadConsole.Global.GraphicsDeviceManager.PreferredBackBufferWidth = WindowWidthPixels;
            SadConsole.Global.GraphicsDeviceManager.PreferredBackBufferHeight = WindowHeightPixels;
            SadConsole.Global.GraphicsDeviceManager.ApplyChanges();
            Global.RenderWidth = WindowWidthPixels;
            Global.RenderHeight = WindowHeightPixels;
            Global.ResetRendering();

            World = new World();
            UIManager = new UIManager();
            CommandManager = new CommandManager(); 

            UIManager.Init();
        }
    }
}
