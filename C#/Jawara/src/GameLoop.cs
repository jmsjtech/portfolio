using System;
using SadConsole;
using Console = SadConsole.Console;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using SadConsole.Components;
using Jawara.UI;
using Jawara.Commands;

namespace Jawara {
    class GameLoop {
        public const int GameWidth = 80;
        public const int GameHeight = 25;
        public static Font RegularSize;
        public static Color CyberBlue = new Color(51, 153, 255);

        // Managers
        public static UIManager UIManager;
        public static World World;
        public static CommandManager CommandManager;

        private static ScrollingConsole startingConsole;

        static void Main(string[] args) {
            // Setup the engine and create the main window.
            SadConsole.Game.Create(GameWidth, GameHeight);

            // Hook the start event so we can add consoles to the system.
            SadConsole.Game.OnInitialize = Init;

            // Hook the update event that happens each frame so we can trap keys and respond.
            SadConsole.Game.OnUpdate = Update;

            // Start the game.
            SadConsole.Game.Instance.Run();

            //
            // Code here will not run until the game window closes.
            //

            SadConsole.Game.Instance.Dispose();
        }

        private static void Update(GameTime time) {

        }

        private static void Init() {
            //Instantiate the UIManager
            UIManager = new UIManager();

            // Build the world!
            World = new World();

            // Now let the UIManager create its consoles
            // so they can use the World data
            UIManager.Init();

            //Instantiate a new CommandManager
            CommandManager = new CommandManager();
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
            Global.FontDefault.ResizeGraphicsDeviceManager(SadConsole.Global.GraphicsDeviceManager, GameWidth, GameHeight, 0, 0);
            Global.ResetRendering();
        }
    }
}