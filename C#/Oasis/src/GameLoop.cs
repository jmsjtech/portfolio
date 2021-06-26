using System;
using SadConsole;
using Console = SadConsole.Console;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities;
using Microsoft.Xna.Framework.Input;
using SadConsole.Components;
using Oasis.UI;
using Oasis.Commands;
using System.Collections.Generic;

namespace Oasis {
    class GameLoop {

        public const int GameWidth = 80;
        public const int GameHeight = 50;

        public static UIManager UIManager;
        public static GameWorld World;
        public static CommandManager CommandManager;
        public static SaveManager SaveManager;
        public static NetworkManager NetworkManager;
        public static Random GlobalRand = new Random();
        public static World ecs;
        public static List<int> entityIDs;
        public static EntityManager Components;



        static void Main(string[] args) {
            SadConsole.Game.Create(GameWidth, GameHeight);


            SadConsole.Game.OnInitialize = Init;

            SadConsole.Game.OnUpdate = Update;

            SadConsole.Game.Instance.Run();



            SadConsole.Game.Instance.Dispose();
        }

        private static void Update(GameTime time) {
            ecs.Update(time);
        }

        private static void Init() {
            var fontMaster = SadConsole.Global.LoadFont("fonts/Cheepicus12.font");
            var normalSizedFont = fontMaster.GetFont(SadConsole.Font.FontSizes.One);
            Global.FontDefault = normalSizedFont;
            Global.FontDefault.ResizeGraphicsDeviceManager(Global.GraphicsDeviceManager, 80, 50, 0, 0);


            SadConsole.Global.GraphicsDeviceManager.PreferredBackBufferWidth = 80 * 12;
            SadConsole.Global.GraphicsDeviceManager.PreferredBackBufferHeight = 50 * 12;
            SadConsole.Global.GraphicsDeviceManager.IsFullScreen = true;
            SadConsole.Global.GraphicsDeviceManager.ApplyChanges();

            SadConsole.Global.GraphicsDeviceManager.IsFullScreen = false;
            SadConsole.Global.GraphicsDeviceManager.ApplyChanges();

            ecs = new WorldBuilder()
           //     .AddSystem(new PlayerSystem())
                .Build();

            

            entityIDs = new List<int>();

            NetworkManager = new NetworkManager();
            UIManager = new UIManager();
            World = new GameWorld();
            CommandManager = new CommandManager();
            SaveManager = new SaveManager();

            UIManager.Init();

            UIManager.SyncMapEntities(World.CurrentMap);
        }
    }
}