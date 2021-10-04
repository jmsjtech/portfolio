using System;
using SadConsole;
using Console = SadConsole.Console;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using SadConsole.Components;

namespace Jawara {
    class GameLoop {
        public const int Width = 59;
        public const int Height = 30;
        public static Font RegularSize;
        public static Color CyberBlue = new Color(51, 153, 255);

        private static Player player;

        public static Map GameMap;
        private static int _mapWidth = 100;
        private static int _mapHeight = 100;
        private static int _maxRooms = 500;
        private static int _minRoomSize = 4;
        private static int _maxRoomSize = 15;

        private static ScrollingConsole startingConsole;

        static void Main(string[] args) {
            SadConsole.Game.Create(Width, Height);

            SadConsole.Game.OnInitialize = Init;
            SadConsole.Game.OnUpdate = Update;

            // Start the game.
            SadConsole.Game.Instance.Run();
            SadConsole.Game.Instance.Dispose();
        }

        private static void Update(GameTime time) {
            CheckKeyboard();

        }

        private static void CheckKeyboard() {
            if (Global.KeyboardState.IsKeyPressed(Keys.NumPad8) || Global.KeyboardState.IsKeyPressed(Keys.W)) { player.MoveBy(new Point(0, -1)); CenterOnActor(player); }
            if (Global.KeyboardState.IsKeyPressed(Keys.NumPad2) || Global.KeyboardState.IsKeyPressed(Keys.S)) { player.MoveBy(new Point(0, 1)); CenterOnActor(player); }
            if (Global.KeyboardState.IsKeyPressed(Keys.NumPad4) || Global.KeyboardState.IsKeyPressed(Keys.A)) { player.MoveBy(new Point(-1, 0)); CenterOnActor(player); }
            if (Global.KeyboardState.IsKeyPressed(Keys.NumPad6) || Global.KeyboardState.IsKeyPressed(Keys.D)) { player.MoveBy(new Point(1, 0)); CenterOnActor(player); }
        }

        private static void Init() {
            GameMap = new Map(_mapWidth, _mapHeight);
            MapGenerator mapGen = new MapGenerator();
            GameMap = mapGen.GenerateMap(_mapWidth, _mapHeight, _maxRooms, _minRoomSize, _maxRoomSize);

            SetFonts();
            startingConsole = new ScrollingConsole(GameMap.Width, GameMap.Height, Global.FontDefault, new Rectangle(0, 0, Width, Height), GameMap.Tiles);

            SadConsole.Global.CurrentScreen = startingConsole;

            //PrintJaCo("jahheemihloh jahjahtahroh", 4, 4, Color.White, Color.Transparent, startingConsole);
            //PrintJaCo("fahbae", 4, 6, Color.White, Color.Transparent, startingConsole);
            //PrintJaCo("sahsah kae", 4, 10, Color.White, Color.Transparent, startingConsole);


            CreatePlayer();
            startingConsole.Children.Add(player);
        }


        private static void PrintJaCo(string romanized, int x, int y, Color fg, Color bg, Console console)  {
            string[] words = romanized.Split(' ');

            Dictionary<string, char> consonants = new Dictionary<string, char>();
            consonants.Add("_", (char)128);
            consonants.Add("b", (char)129);
            consonants.Add("d", (char)130); // Actually the dzh sound
            consonants.Add("h", (char)131);
            consonants.Add("j", (char)132);
            consonants.Add("k", (char)133);
            consonants.Add("l", (char)134);
            consonants.Add("m", (char)135);
            consonants.Add("n", (char)136);
            consonants.Add("p", (char)137);
            consonants.Add("r", (char)138);
            consonants.Add("t", (char)139);
            consonants.Add("s", (char)140);
            consonants.Add("w", (char)141);
            consonants.Add("f", (char)142);

            Dictionary<string, char> vowels = new Dictionary<string, char>();
            vowels.Add("ah", (char)144); 
            vowels.Add("ae", (char)145);
            vowels.Add("eh", (char)146);
            vowels.Add("ih", (char)147);
            vowels.Add("oh", (char)148);
            vowels.Add("oo", (char)149);
            vowels.Add("uh", (char)150);
            vowels.Add("ee", (char)151);

            
            for (int i = 0; i < words.Length; i++) {
                for (int j = 0; j < words[i].Length; j = j + 3) {
                    char con = consonants[words[i].Substring(j, 1)];
                    char vow = vowels[words[i].Substring(j + 1, 2)];

                    console.SetCellAppearance(x, y, new Cell(fg, bg, con));
                    console.SetDecorator(x, y, 1, new CellDecorator(fg, vow, new SpriteEffects()));
                    x++;
                }
                x++;
            }


        }

        private static void SetFonts() {
            SadConsole.Themes.WindowTheme windowTheme = new SadConsole.Themes.WindowTheme();
            windowTheme.BorderLineStyle = CellSurface.ConnectedLineThick;
            SadConsole.Themes.Library.Default.WindowTheme = windowTheme;


            var fontMaster = SadConsole.Global.LoadFont("res/JawaranCommon.font");
            RegularSize = fontMaster.GetFont(Font.FontSizes.One);

            SadConsole.Themes.Library.Default.Colors.TitleText = CyberBlue;
            SadConsole.Themes.Library.Default.Colors.Lines = CyberBlue;
            SadConsole.Themes.Library.Default.Colors.ControlHostBack = Color.Black;

            SadConsole.Global.FontDefault = RegularSize;
            Global.FontDefault.ResizeGraphicsDeviceManager(SadConsole.Global.GraphicsDeviceManager, Width, Height, 0, 0);
            Global.ResetRendering();
        }

        private static void CreatePlayer() {
            player = new Player(Color.Yellow, Color.Transparent);
            player.Position = new Point(5, 5);
            player.Components.Add(new EntityViewSyncComponent());
        }

        public static void CenterOnActor(Actor actor) {
            startingConsole.CenterViewPortOnPoint(actor.Position);
        }
    }
}