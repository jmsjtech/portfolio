using System;
using SadConsole;
using Console = SadConsole.Console;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DefaultEcs;
using Microsoft.Xna.Framework.Input;
using SadConsole.Components;
using Oasis.UI;
using Oasis.Commands;

namespace Oasis {
    class GameLoop {

        public const int GameWidth = 80;
        public const int GameHeight = 50;

        public static UIManager UIManager;
        public static World World;
        public static CommandManager CommandManager;
        public static SaveManager SaveManager;
        public static NetworkManager NetworkManager;
        public static Random GlobalRand = new Random();


        public struct State {
            public DefaultEcs.World ecs;
        }

        public static State gs = new State {
            ecs = new DefaultEcs.World()
        };


        static void Main(string[] args) {
            SadConsole.Game.Create(GameWidth, GameHeight);


            SadConsole.Game.OnInitialize = Init;

            SadConsole.Game.OnUpdate = Update;

            SadConsole.Game.Instance.Run();



            SadConsole.Game.Instance.Dispose();
        }

        private static void Update(GameTime time) {
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


            NetworkManager = new NetworkManager();
            UIManager = new UIManager();
            World = new World();
            CommandManager = new CommandManager();
            SaveManager = new SaveManager();

            UIManager.Init();

            UIManager.SyncMapEntities(World.CurrentMap);
        }
    }
}