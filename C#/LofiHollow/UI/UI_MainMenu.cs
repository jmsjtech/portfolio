using SadRogue.Primitives;
using SadConsole;
using SadConsole.UI;
using SadConsole.Input;
using System.IO;
using System;
using System.Collections.Generic;
using Key = SadConsole.Input.Keys;

namespace LofiHollow.UI {
    public class UI_MainMenu {
        public SadConsole.Console MainMenuConsole;
        public SadConsole.UI.ControlsConsole NameConsole;
        public SadConsole.UI.Controls.TextBox NameBox;
        public Window MainMenuWindow;
        public SadRex.Image MenuImage;
        
        public SadConsole.Console MenuConsole;


        public int charCreationStage = 0;
        public int ccPointBuyLeft = 20;
        public int ccClassTop = 0;
        public int ccClassSelect = -1;
        public int ccRaceSelect = 0;
        public int ccRaceTop = 0;
        public string LobbyCode = "";
        public string[] Names;
        public string joinError = "";

        

        public UI_MainMenu() {
            MainMenuWindow = new Window(GameLoop.GameWidth, GameLoop.GameHeight);
            MainMenuWindow.CanDrag = false;
            MainMenuWindow.Position = new Point(0, 0);

            int menuConWidth = GameLoop.GameWidth;

            Stream menuXP = new FileStream("./data/trees.xp", FileMode.Open);
            MenuImage = SadRex.Image.Load(menuXP);


            ColoredGlyph[] cells = new ColoredGlyph[100 * 60];

            for (int i = 0; i < MenuImage.Layers[0].Cells.Count; i++) {
                var cell = MenuImage.Layers[0].Cells[i];
                Color convertedFG = new Color(cell.Foreground.R, cell.Foreground.G, cell.Foreground.B);
                Color convertedBG = new Color(cell.Background.R, cell.Background.G, cell.Background.B);

                cells[i] = new ColoredGlyph(Color.Transparent, convertedFG, MenuImage.Layers[0].Cells[i].Character);
            }

            MainMenuConsole = new SadConsole.Console(GameLoop.GameWidth, GameLoop.GameHeight, cells);
            MainMenuWindow.Children.Add(MainMenuConsole);

            MainMenuConsole.Position = new Point(0, 0);
            MainMenuWindow.Title = "".Align(HorizontalAlignment.Center, menuConWidth, (char)196);

            GameLoop.UIManager.Children.Add(MainMenuWindow);

            MainMenuWindow.Show();
            MainMenuWindow.IsVisible = true;

            MenuConsole = new SadConsole.Console(20, 20);
            MenuConsole.Position = new Point(40, 20);
            MainMenuWindow.Children.Add(MenuConsole);

            NameConsole = new ControlsConsole(13, 1);
            NameBox = new SadConsole.UI.Controls.TextBox(13);
            NameConsole.Controls.Add(NameBox);
            NameConsole.Position = new Point(1, 13);

            MenuConsole.Children.Add(NameConsole);
            NameConsole.IsVisible = false;
            NameBox.TextChanged += NameChanged; 
        }

        private void NameChanged(object sender, EventArgs e) {
            GameLoop.World.Player.Name = NameBox.Text;
        }
         

        public void RemakeMenu() {
            for (int i = 0; i < MenuImage.Layers[0].Cells.Count; i++) {
                var cell = MenuImage.Layers[0].Cells[i];
                Color convertedFG = new Color(cell.Foreground.R, cell.Foreground.G, cell.Foreground.B);
                Color convertedBG = new Color(cell.Background.R, cell.Background.G, cell.Background.B);

                MainMenuConsole.SetCellAppearance(i % GameLoop.GameWidth, i / GameLoop.GameWidth, new ColoredGlyph(Color.Transparent, convertedFG, MenuImage.Layers[0].Cells[i].Character));
            }
        }


        public void RenderMainMenu() {
            MenuConsole.Clear();
            Point mousePos = new MouseScreenObjectState(MenuConsole, GameHost.Instance.Mouse).CellPosition;

            if (GameLoop.UIManager.selectedMenu == "MainMenu") {
                int leftEdge = 32;
                int topEdge = 10;

                // L
                for (int i = 0; i < 5; i++) {
                    MainMenuConsole.SetDecorator(leftEdge + 1, topEdge + i, 1, new CellDecorator(Color.MediumPurple, 240, Mirror.None));
                }
                // MainMenuConsole.SetDecorator(28, 7, 3, new CellDecorator(Color.MediumPurple, 240, Mirror.None)); 



                // O
                MainMenuConsole.SetDecorator(leftEdge + 3, topEdge + 2, 3, new CellDecorator(Color.MediumPurple, 240, Mirror.None));
                MainMenuConsole.SetDecorator(leftEdge + 3, topEdge + 3, 1, new CellDecorator(Color.MediumPurple, 240, Mirror.None));
                MainMenuConsole.SetDecorator(leftEdge + 5, topEdge + 3, 1, new CellDecorator(Color.MediumPurple, 240, Mirror.None));
                MainMenuConsole.SetDecorator(leftEdge + 3, topEdge + 4, 3, new CellDecorator(Color.MediumPurple, 240, Mirror.None));

                // F
                MainMenuConsole.SetDecorator(leftEdge + 8, topEdge + 1, 1, new CellDecorator(Color.MediumPurple, 240, Mirror.None));
                MainMenuConsole.SetDecorator(leftEdge + 7, topEdge + 2, 1, new CellDecorator(Color.MediumPurple, 240, Mirror.None));
                MainMenuConsole.SetDecorator(leftEdge + 7, topEdge + 3, 2, new CellDecorator(Color.MediumPurple, 240, Mirror.None));
                MainMenuConsole.SetDecorator(leftEdge + 7, topEdge + 4, 1, new CellDecorator(Color.MediumPurple, 240, Mirror.None));

                // I
                MainMenuConsole.SetDecorator(leftEdge + 10, topEdge + 1, 1, new CellDecorator(Color.MediumPurple, 240, Mirror.None));
                MainMenuConsole.SetDecorator(leftEdge + 10, topEdge + 3, 1, new CellDecorator(Color.MediumPurple, 240, Mirror.None));
                MainMenuConsole.SetDecorator(leftEdge + 10, topEdge + 4, 1, new CellDecorator(Color.MediumPurple, 240, Mirror.None));



                // H
                MainMenuConsole.SetDecorator(leftEdge + 14, topEdge, 1, new CellDecorator(Color.MediumPurple, 240, Mirror.None));
                MainMenuConsole.SetDecorator(leftEdge + 14, topEdge + 1, 1, new CellDecorator(Color.MediumPurple, 240, Mirror.None));
                MainMenuConsole.SetDecorator(leftEdge + 14, topEdge + 2, 2, new CellDecorator(Color.MediumPurple, 240, Mirror.None));
                MainMenuConsole.SetDecorator(leftEdge + 14, topEdge + 3, 1, new CellDecorator(Color.MediumPurple, 240, Mirror.None));
                MainMenuConsole.SetDecorator(leftEdge + 14, topEdge + 4, 1, new CellDecorator(Color.MediumPurple, 240, Mirror.None));
                MainMenuConsole.SetDecorator(leftEdge + 16, topEdge + 3, 1, new CellDecorator(Color.MediumPurple, 240, Mirror.None));
                MainMenuConsole.SetDecorator(leftEdge + 16, topEdge + 4, 1, new CellDecorator(Color.MediumPurple, 240, Mirror.None));

                // O
                MainMenuConsole.SetDecorator(leftEdge + 18, topEdge + 2, 3, new CellDecorator(Color.MediumPurple, 240, Mirror.None));
                MainMenuConsole.SetDecorator(leftEdge + 18, topEdge + 3, 1, new CellDecorator(Color.MediumPurple, 240, Mirror.None));
                MainMenuConsole.SetDecorator(leftEdge + 20, topEdge + 3, 1, new CellDecorator(Color.MediumPurple, 240, Mirror.None));
                MainMenuConsole.SetDecorator(leftEdge + 18, topEdge + 4, 3, new CellDecorator(Color.MediumPurple, 240, Mirror.None));

                // LL
                for (int i = 0; i < 5; i++) {
                    MainMenuConsole.SetDecorator(leftEdge + 22, topEdge + i, 1, new CellDecorator(Color.MediumPurple, 240, Mirror.None));
                    MainMenuConsole.SetDecorator(leftEdge + 24, topEdge + i, 1, new CellDecorator(Color.MediumPurple, 240, Mirror.None));
                }

                // O
                MainMenuConsole.SetDecorator(leftEdge + 26, topEdge + 2, 3, new CellDecorator(Color.MediumPurple, 240, Mirror.None));
                MainMenuConsole.SetDecorator(leftEdge + 26, topEdge + 3, 1, new CellDecorator(Color.MediumPurple, 240, Mirror.None));
                MainMenuConsole.SetDecorator(leftEdge + 28, topEdge + 3, 1, new CellDecorator(Color.MediumPurple, 240, Mirror.None));
                MainMenuConsole.SetDecorator(leftEdge + 26, topEdge + 4, 3, new CellDecorator(Color.MediumPurple, 240, Mirror.None));

                // W
                MainMenuConsole.SetDecorator(leftEdge + 30, topEdge + 2, 1, new CellDecorator(Color.MediumPurple, 240, Mirror.None));
                MainMenuConsole.SetDecorator(leftEdge + 34, topEdge + 2, 1, new CellDecorator(Color.MediumPurple, 240, Mirror.None));
                MainMenuConsole.SetDecorator(leftEdge + 30, topEdge + 3, 1, new CellDecorator(Color.MediumPurple, 240, Mirror.None));
                MainMenuConsole.SetDecorator(leftEdge + 32, topEdge + 3, 1, new CellDecorator(Color.MediumPurple, 240, Mirror.None));
                MainMenuConsole.SetDecorator(leftEdge + 34, topEdge + 3, 1, new CellDecorator(Color.MediumPurple, 240, Mirror.None));
                MainMenuConsole.SetDecorator(leftEdge + 30, topEdge + 4, 2, new CellDecorator(Color.MediumPurple, 240, Mirror.None));
                MainMenuConsole.SetDecorator(leftEdge + 33, topEdge + 4, 2, new CellDecorator(Color.MediumPurple, 240, Mirror.None));



                MenuConsole.DrawBox(new Rectangle(0, 0, 20, 20), ShapeParameters.CreateStyledBoxFilled(ICellSurface.ConnectedLineThin, new ColoredGlyph(Color.White, Color.Black), new ColoredGlyph(Color.Black, Color.Black)));
                MenuConsole.Print(6, 2, new ColoredString("New Game", mousePos.Y == 2 ? Color.Yellow : Color.White, Color.Black));
                MenuConsole.Print(5, 3, new ColoredString("Load Game", mousePos.Y == 3 ? Color.Yellow : Color.White, Color.Black));
            } else if (GameLoop.UIManager.selectedMenu == "CharCreation") {
                MenuConsole.DrawBox(new Rectangle(0, 0, 50, 50), ShapeParameters.CreateStyledBoxFilled(ICellSurface.ConnectedLineThin, new ColoredGlyph(Color.White, Color.Black), new ColoredGlyph(Color.Black, Color.Black)));

                int CreateX = 1;
                int CreateY = 1;

                MenuConsole.DrawLine(new Point(CreateX + 13, CreateY), new Point(CreateX + 13, CreateY + 47), (char)179, Color.White, Color.Black);
                // Attribute window

                string STR = GameLoop.World.Player.STR.ToString();
                if (GameLoop.World.Player.STR < 10)
                    STR = "0" + STR;

                string DEX = GameLoop.World.Player.DEX.ToString();
                if (GameLoop.World.Player.DEX < 10)
                    DEX = "0" + DEX;

                string CON = GameLoop.World.Player.CON.ToString();
                if (GameLoop.World.Player.CON < 10)
                    CON = "0" + CON;

                string INT = GameLoop.World.Player.INT.ToString();
                if (GameLoop.World.Player.INT < 10)
                    INT = "0" + INT;

                string WIS = GameLoop.World.Player.WIS.ToString();
                if (GameLoop.World.Player.WIS < 10)
                    WIS = "0" + WIS;

                string CHA = GameLoop.World.Player.CHA.ToString();
                if (GameLoop.World.Player.CHA < 10)
                    CHA = "0" + CHA;

                MenuConsole.Print(CreateX + 1, CreateY + 1, new ColoredString("STR: ", Color.White, Color.Black));
                MenuConsole.Print(CreateX + 6, CreateY + 1, new ColoredString("- ", mousePos == new Point(CreateX + 6, CreateY + 1) ? Color.Yellow : Color.White, Color.Black));
                MenuConsole.Print(CreateX + 8, CreateY + 1, new ColoredString(STR, Color.White, Color.Black));
                MenuConsole.Print(CreateX + 10, CreateY + 1, new ColoredString(" +", mousePos == new Point(CreateX + 11, CreateY + 1) ? Color.Yellow : Color.White, Color.Black));

                MenuConsole.Print(CreateX + 1, CreateY + 2, new ColoredString("DEX: ", Color.White, Color.Black));
                MenuConsole.Print(CreateX + 6, CreateY + 2, new ColoredString("- ", mousePos == new Point(CreateX + 6, CreateY + 2) ? Color.Yellow : Color.White, Color.Black));
                MenuConsole.Print(CreateX + 8, CreateY + 2, new ColoredString(DEX, Color.White, Color.Black));
                MenuConsole.Print(CreateX + 10, CreateY + 2, new ColoredString(" +", mousePos == new Point(CreateX + 11, CreateY + 2) ? Color.Yellow : Color.White, Color.Black));

                MenuConsole.Print(CreateX + 1, CreateY + 3, new ColoredString("CON: ", Color.White, Color.Black));
                MenuConsole.Print(CreateX + 6, CreateY + 3, new ColoredString("- ", mousePos == new Point(CreateX + 6, CreateY + 3) ? Color.Yellow : Color.White, Color.Black));
                MenuConsole.Print(CreateX + 8, CreateY + 3, new ColoredString(CON, Color.White, Color.Black));
                MenuConsole.Print(CreateX + 10, CreateY + 3, new ColoredString(" +", mousePos == new Point(CreateX + 11, CreateY + 3) ? Color.Yellow : Color.White, Color.Black));

                MenuConsole.Print(CreateX + 1, CreateY + 4, new ColoredString("INT: ", Color.White, Color.Black));
                MenuConsole.Print(CreateX + 6, CreateY + 4, new ColoredString("- ", mousePos == new Point(CreateX + 6, CreateY + 4) ? Color.Yellow : Color.White, Color.Black));
                MenuConsole.Print(CreateX + 8, CreateY + 4, new ColoredString(INT, Color.White, Color.Black));
                MenuConsole.Print(CreateX + 10, CreateY + 4, new ColoredString(" +", mousePos == new Point(CreateX + 11, CreateY + 4) ? Color.Yellow : Color.White, Color.Black));

                MenuConsole.Print(CreateX + 1, CreateY + 5, new ColoredString("WIS: ", Color.White, Color.Black));
                MenuConsole.Print(CreateX + 6, CreateY + 5, new ColoredString("- ", mousePos == new Point(CreateX + 6, CreateY + 5) ? Color.Yellow : Color.White, Color.Black));
                MenuConsole.Print(CreateX + 8, CreateY + 5, new ColoredString(WIS, Color.White, Color.Black));
                MenuConsole.Print(CreateX + 10, CreateY + 5, new ColoredString(" +", mousePos == new Point(CreateX + 11, CreateY + 5) ? Color.Yellow : Color.White, Color.Black));

                MenuConsole.Print(CreateX + 1, CreateY + 6, new ColoredString("CHA: ", Color.White, Color.Black));
                MenuConsole.Print(CreateX + 6, CreateY + 6, new ColoredString("- ", mousePos == new Point(CreateX + 6, CreateY + 6) ? Color.Yellow : Color.White, Color.Black));
                MenuConsole.Print(CreateX + 8, CreateY + 6, new ColoredString(CHA, Color.White, Color.Black));
                MenuConsole.Print(CreateX + 10, CreateY + 6, new ColoredString(" +", mousePos == new Point(CreateX + 11, CreateY + 6) ? Color.Yellow : Color.White, Color.Black));


                ccPointBuyLeft = 20 - (PointBuyCost(GameLoop.World.Player.STR) + PointBuyCost(GameLoop.World.Player.DEX) + PointBuyCost(GameLoop.World.Player.CON) +
                    PointBuyCost(GameLoop.World.Player.INT) + PointBuyCost(GameLoop.World.Player.WIS) + PointBuyCost(GameLoop.World.Player.CHA));

                MenuConsole.Print(CreateX + 1, CreateY + 8, new ColoredString("Points: " + ccPointBuyLeft + " ", ccPointBuyLeft > 0 ? Color.Lime : ccPointBuyLeft == 0 ? Color.White : Color.Red, Color.Black));

                MenuConsole.DrawLine(new Point(CreateX, CreateY + 10), new Point(CreateX + 12, CreateY + 10), (char)196, Color.White, Color.Black);


                // Class Picker
                MenuConsole.DrawBox(new Rectangle(CreateX + 13, CreateY - 1, 36, 15), ShapeParameters.CreateStyledBoxFilled(ICellSurface.ConnectedLineThin, new ColoredGlyph(Color.White, Color.Black), new ColoredGlyph(Color.Black, Color.Black)));

                MenuConsole.DrawLine(new Point(CreateX + 25, CreateY), new Point(CreateX + 25, CreateY + 12), (char)124, Color.White, Color.Black);
                MenuConsole.DrawLine(new Point(CreateX + 31, CreateY), new Point(CreateX + 31, CreateY + 12), (char)124, Color.White, Color.Black);
                MenuConsole.DrawLine(new Point(CreateX + 36, CreateY), new Point(CreateX + 36, CreateY + 12), (char)124, Color.White, Color.Black);
                MenuConsole.DrawLine(new Point(CreateX + 38, CreateY), new Point(CreateX + 38, CreateY + 12), (char)124, Color.White, Color.Black);
                MenuConsole.DrawLine(new Point(CreateX + 40, CreateY), new Point(CreateX + 40, CreateY + 12), (char)124, Color.White, Color.Black);
                MenuConsole.DrawLine(new Point(CreateX + 42, CreateY), new Point(CreateX + 42, CreateY + 12), (char)124, Color.White, Color.Black);
                MenuConsole.DrawLine(new Point(CreateX + 14, CreateY + 1), new Point(CreateX + 47, CreateY + 1), (char)196, Color.White, Color.Black);

                MenuConsole.Print(CreateX + 14, CreateY, new ColoredString("Class Name".Align(HorizontalAlignment.Center, 11), Color.White, Color.Black));
                MenuConsole.Print(CreateX + 26, CreateY, new ColoredString("BAB".Align(HorizontalAlignment.Center, 5), Color.White, Color.Black));
                MenuConsole.Print(CreateX + 32, CreateY, new ColoredString("HD".Align(HorizontalAlignment.Center, 4), Color.White, Color.Black));
                MenuConsole.Print(CreateX + 37, CreateY, new ColoredString("F", Color.White, Color.Black));
                MenuConsole.Print(CreateX + 39, CreateY, new ColoredString("R", Color.White, Color.Black));
                MenuConsole.Print(CreateX + 41, CreateY, new ColoredString("W", Color.White, Color.Black));

                string classString = "No Class";

                if (ccClassSelect != -1) {
                    if (GameLoop.World.classLibrary.ContainsKey(ccClassSelect)) {
                        ClassDef temp = GameLoop.World.classLibrary[ccClassSelect];
                        classString = temp.Name;
                    }
                }


                for (int i = 0; i < 10; i++) {
                    if (GameLoop.World.classLibrary.ContainsKey(i + ccClassTop)) {
                        ClassDef temp = GameLoop.World.classLibrary[i + ccClassTop];
                        MenuConsole.Print(CreateX + 14, CreateY + i + 2, new ColoredString(temp.Name.Align(HorizontalAlignment.Center, 11), temp.Name == classString ? Color.Lime : (mousePos.Y == CreateY + i + 2 && mousePos.X > CreateX + 13) ? Color.Yellow : Color.White, Color.Black));
                        MenuConsole.Print(CreateX + 26, CreateY + i + 2, new ColoredString(temp.BABperLevel.ToString().Align(HorizontalAlignment.Center, 5), temp.Name == classString ? Color.Lime : (mousePos.Y == CreateY + i + 2 && mousePos.X > CreateX + 13) ? Color.Yellow : Color.White, Color.Black));
                        MenuConsole.Print(CreateX + 32, CreateY + i + 2, new ColoredString(temp.HitDie.Align(HorizontalAlignment.Center, 4), temp.Name == classString ? Color.Lime : (mousePos.Y == CreateY + i + 2 && mousePos.X > CreateX + 13) ? Color.Yellow : Color.White, Color.Black));
                        MenuConsole.Print(CreateX + 37, CreateY + i + 2, new ColoredString(temp.FortSaveProg.Substring(0, 1), temp.Name == classString ? Color.Lime : (mousePos.Y == CreateY + i + 2 && mousePos.X > CreateX + 13) ? Color.Yellow : Color.White, Color.Black));
                        MenuConsole.Print(CreateX + 39, CreateY + i + 2, new ColoredString(temp.RefSaveProg.Substring(0, 1), temp.Name == classString ? Color.Lime : (mousePos.Y == CreateY + i + 2 && mousePos.X > CreateX + 13) ? Color.Yellow : Color.White, Color.Black));
                        MenuConsole.Print(CreateX + 41, CreateY + i + 2, new ColoredString(temp.WillSaveProg.Substring(0, 1), temp.Name == classString ? Color.Lime : (mousePos.Y == CreateY + i + 2 && mousePos.X > CreateX + 13) ? Color.Yellow : Color.White, Color.Black));
                    }
                }


                MenuConsole.Print(CreateX + 1, CreateY + 11, new ColoredString("Name:", Color.White, Color.Black));

                MenuConsole.Print(CreateX + 1, CreateY + 14, new ColoredString(classString, Color.White, Color.Black));
                MenuConsole.Print(CreateX + 1, CreateY + 14, new ColoredString(classString, Color.White, Color.Black));


                MenuConsole.Print(CreateX + 14, CreateY + 28, new ColoredString("EXP Track:", Color.White, Color.Black));
                MenuConsole.Print(CreateX + 25, CreateY + 28, new ColoredString("Slow", GameLoop.World.Player.ExpTrack == "Slow" ? Color.Lime : (mousePos.X <= CreateX + 28 && mousePos.X >= CreateX + 25 && mousePos.Y == CreateY + 28) ? Color.Yellow : Color.White, Color.Black));
                MenuConsole.Print(CreateX + 30, CreateY + 28, new ColoredString("Medium", GameLoop.World.Player.ExpTrack == "Medium" ? Color.Lime : (mousePos.X <= CreateX + 35 && mousePos.X >= CreateX + 30 && mousePos.Y == CreateY + 28) ? Color.Yellow : Color.White, Color.Black));
                MenuConsole.Print(CreateX + 37, CreateY + 28, new ColoredString("Fast", GameLoop.World.Player.ExpTrack == "Fast" ? Color.Lime : (mousePos.X <= CreateX + 40 && mousePos.X >= CreateX + 37 && mousePos.Y == CreateY + 28) ? Color.Yellow : Color.White, Color.Black));

                // Race Box
                MenuConsole.DrawBox(new Rectangle(CreateX + 13, CreateY + 13, 36, 15), ShapeParameters.CreateStyledBoxFilled(ICellSurface.ConnectedLineThin, new ColoredGlyph(Color.White, Color.Black), new ColoredGlyph(Color.Black, Color.Black)));
                MenuConsole.Print(CreateX + 14, CreateY + 14, new ColoredString("Name".Align(HorizontalAlignment.Center, 10), Color.White, Color.Black));
                MenuConsole.Print(CreateX + 24, CreateY + 14, new ColoredString("Description".Align(HorizontalAlignment.Center, 24), Color.White, Color.Black));
                MenuConsole.DrawLine(new Point(CreateX + 14, CreateY + 15), new Point(CreateX + 47, CreateY + 15), (char)196, Color.White, Color.Black);

                for (int i = 0; i < 10; i++) {
                    if (GameLoop.World.raceLibrary.ContainsKey(i + ccRaceTop)) {
                        Race temp = GameLoop.World.raceLibrary[i + ccRaceTop];
                        MenuConsole.Print(CreateX + 14, CreateY + i + 16, new ColoredString(temp.Name.Align(HorizontalAlignment.Center, 9), GameLoop.World.Player.Race.Name == temp.Name ? Color.Lime : (mousePos.Y == CreateY + i + 16 && mousePos.X > CreateX + 13) ? Color.Yellow : Color.White, Color.Black));
                        MenuConsole.Print(CreateX + 24, CreateY + i + 16, new ColoredString(temp.Description.Align(HorizontalAlignment.Center, 24), GameLoop.World.Player.Race.Name == temp.Name ? Color.Lime : (mousePos.Y == CreateY + i + 16 && mousePos.X > CreateX + 13) ? Color.Yellow : Color.White, Color.Black));
                    }
                }

                MenuConsole.DrawLine(new Point(CreateX + 23, CreateY + 14), new Point(CreateX + 23, CreateY + 26), (char)124, Color.White, Color.Black);


                MenuConsole.Print(2, MenuConsole.Height - 2, new ColoredString("DONE", (mousePos.Y == MenuConsole.Height - 2 && mousePos.X <= 6 && mousePos.X >= 2) ? Color.Yellow : Color.White, Color.Black));
            } else if (GameLoop.UIManager.selectedMenu == "LoadFile") {
                int fileSize = Names.Length + 2;
                if (fileSize < 20)
                    fileSize = 20;

                MenuConsole.DrawBox(new Rectangle(0, 0, 20, fileSize), ShapeParameters.CreateStyledBoxFilled(ICellSurface.ConnectedLineThin, new ColoredGlyph(Color.White, Color.Black), new ColoredGlyph(Color.Black, Color.Black)));
                MenuConsole.Print(1, 1, new ColoredString("Select a Save File", Color.White, Color.Black));
                MenuConsole.DrawLine(new Point(1, 2), new Point(18, 2), (char)196, Color.White, Color.Black);

                for (int i = 0; i < Names.Length; i++) {
                    MenuConsole.Print(1, 3 + i, new ColoredString(Names[i].Align(HorizontalAlignment.Center, 18), mousePos.Y == 3 + i ? Color.Yellow : Color.White, Color.Black));
                }

                MenuConsole.Print(1, 1 + fileSize - 3, new ColoredString("[BACK]".Align(HorizontalAlignment.Center, 18), mousePos.Y == (1 + fileSize - 3) ? Color.Yellow : Color.White, Color.Black));
            } else if (GameLoop.UIManager.selectedMenu == "ConnectOrHost") {
                MenuConsole.DrawBox(new Rectangle(0, 0, 20, 10), ShapeParameters.CreateStyledBoxFilled(ICellSurface.ConnectedLineThin, new ColoredGlyph(Color.White, Color.Black), new ColoredGlyph(Color.Black, Color.Black)));
                MenuConsole.Print(1, 1, new ColoredString("Singleplayer", mousePos.Y == 1 ? Color.Yellow : Color.White, Color.Black));
                MenuConsole.Print(1, 2, new ColoredString("Host and Play", mousePos.Y == 2 ? Color.Yellow : Color.White, Color.Black));

                string buff = LobbyCode;

                for (int i = buff.Length; i < 6; i++) {
                    buff += "-";
                }

                MenuConsole.Print(1, 4, new ColoredString(buff, Color.White, Color.Black));
                MenuConsole.Print(8, 4, new ColoredString("[JOIN]", mousePos.Y == 4 ? Color.Yellow : Color.White, Color.Black));


                MenuConsole.Print(1, 7, new ColoredString(joinError, Color.White, Color.Black));
            }
        }

        public void CaptureMainMenuClicks() {
            Point mousePos = new MouseScreenObjectState(MenuConsole, GameHost.Instance.Mouse).CellPosition;
            if (GameLoop.UIManager.selectedMenu == "ConnectOrHost") {
                if (LobbyCode.Length < 6) {
                    foreach (var key in GameHost.Instance.Keyboard.KeysReleased) {
                        if (key.Character >= 'A' && key.Character <= 'z') {
                            LobbyCode += key.Character;
                        }
                    }

                    if (GameHost.Instance.Keyboard.IsKeyReleased(Key.Space)) {
                        LobbyCode += " ";
                    }
                }

                if (LobbyCode.Length > 0) {
                    if (GameHost.Instance.Keyboard.IsKeyReleased(Key.Back)) {
                        LobbyCode = LobbyCode.Substring(0, LobbyCode.Length - 1);
                    }
                }
            }



            if (GameHost.Instance.Mouse.LeftClicked) {
                if (GameLoop.UIManager.selectedMenu == "MainMenu") {
                    if (mousePos.Y == 2) {
                        GameLoop.UIManager.selectedMenu = "CharCreation";
                        MenuConsole = new SadConsole.Console(50, 50);
                        MainMenuWindow.Children.Add(MenuConsole);
                        MenuConsole.Children.Add(NameConsole);
                        MenuConsole.Position = new Point(25, 5);
                        NameConsole.IsVisible = true;
                    } else if (mousePos.Y == 3) {
                        GameLoop.UIManager.selectedMenu = "LoadFile";
                        if (Directory.Exists("./saves/")) {
                            Names = Directory.GetDirectories("./saves/");

                            for (int i = 0; i < Names.Length; i++) {
                                string[] split = Names[i].Split("/");
                                Names[i] = split[split.Length - 1];
                            }
                        }
                    }
                }
                else if (GameLoop.UIManager.selectedMenu == "CharCreation") {
                    int CreateX = 1;
                    int CreateY = 1;
                    if (mousePos == new Point(CreateX + 6, CreateY + 1))
                        if (GameLoop.World.Player.STR > 7)
                            GameLoop.World.Player.STR--;

                    if (mousePos == new Point(CreateX + 11, CreateY + 1))
                        if (GameLoop.World.Player.STR < 18)
                            GameLoop.World.Player.STR++;

                    if (mousePos == new Point(CreateX + 6, CreateY + 2))
                        if (GameLoop.World.Player.DEX > 7)
                            GameLoop.World.Player.DEX--;

                    if (mousePos == new Point(CreateX + 11, CreateY + 2))
                        if (GameLoop.World.Player.DEX < 18)
                            GameLoop.World.Player.DEX++;

                    if (mousePos == new Point(CreateX + 6, CreateY + 3))
                        if (GameLoop.World.Player.CON > 7)
                            GameLoop.World.Player.CON--;

                    if (mousePos == new Point(CreateX + 11, CreateY + 3))
                        if (GameLoop.World.Player.CON < 18)
                            GameLoop.World.Player.CON++;

                    if (mousePos == new Point(CreateX + 6, CreateY + 4))
                        if (GameLoop.World.Player.INT > 7)
                            GameLoop.World.Player.INT--;

                    if (mousePos == new Point(CreateX + 11, CreateY + 4))
                        if (GameLoop.World.Player.INT < 18)
                            GameLoop.World.Player.INT++;

                    if (mousePos == new Point(CreateX + 6, CreateY + 5))
                        if (GameLoop.World.Player.WIS > 7)
                            GameLoop.World.Player.WIS--;

                    if (mousePos == new Point(CreateX + 11, CreateY + 5))
                        if (GameLoop.World.Player.WIS < 18)
                            GameLoop.World.Player.WIS++;

                    if (mousePos == new Point(CreateX + 6, CreateY + 6))
                        if (GameLoop.World.Player.CHA > 7)
                            GameLoop.World.Player.CHA--;

                    if (mousePos == new Point(CreateX + 11, CreateY + 6))
                        if (GameLoop.World.Player.CHA < 18)
                            GameLoop.World.Player.CHA++;


                    if (mousePos.Y == MenuConsole.Height - 2 && mousePos.X <= 6 && mousePos.X >= 2 && ccPointBuyLeft >= 0 && ccClassSelect != -1) {
                        GameLoop.UIManager.selectedMenu = "ConnectOrHost";

                        MenuConsole = new SadConsole.Console(20, 20);
                        MainMenuWindow.Children.Add(MenuConsole); 
                        MenuConsole.Position = new Point(40, 20);
                        NameConsole.IsVisible = false; 
                        RemakeMenu();

                        ClassDef selectedClass = new ClassDef();
                        selectedClass.Copy(GameLoop.World.classLibrary[ccClassSelect]);
                        selectedClass.ClassLevels = 1;

                        GameLoop.World.Player.ClassLevels.Add(selectedClass);
                        
                        if (GameLoop.World.Player.Name != NameBox.EditingText)
                            GameLoop.World.Player.Name = NameBox.EditingText;

                        GameLoop.World.Player.STR += GameLoop.World.Player.Race.StrMod;
                        GameLoop.World.Player.DEX += GameLoop.World.Player.Race.DexMod;
                        GameLoop.World.Player.CON += GameLoop.World.Player.Race.ConMod;
                        GameLoop.World.Player.INT += GameLoop.World.Player.Race.IntMod;
                        GameLoop.World.Player.WIS += GameLoop.World.Player.Race.WisMod;
                        GameLoop.World.Player.CHA += GameLoop.World.Player.Race.ChaMod;

                        GameLoop.World.Player.SizeMod = GameLoop.World.Player.Race.SizeMod;
                        GameLoop.World.Player.LandSpeed = GameLoop.World.Player.Race.LandSpeed;


                        for (int i = 0; i < GameLoop.World.Player.Race.Languages.Count; i++) {
                            GameLoop.World.Player.KnownLanguages.Add(GameLoop.World.Player.Race.Languages[i]);
                        }

                        GameLoop.World.FreshStart();
                        GameLoop.UIManager.Map.UpdateVision();
                    }

                    if (mousePos.X >= 14 + CreateX && mousePos.X <= 47 + CreateX) {
                        if (mousePos.Y < 13 + CreateY && mousePos.Y > CreateY) {
                            ccClassSelect = (mousePos.Y - CreateY) - 2;

                            if (ccClassSelect < 0)
                                ccClassSelect = -1;
                            if (ccClassSelect > GameLoop.World.classLibrary.Count - 1)
                                ccClassSelect = GameLoop.World.classLibrary.Count - 1;
                        }
                    }

                    if (mousePos.X >= CreateX + 25 && mousePos.X <= CreateX + 28 && mousePos.Y == CreateY + 28) { GameLoop.World.Player.ExpTrack = "Slow"; }
                    if (mousePos.X >= CreateX + 30 && mousePos.X <= CreateX + 35 && mousePos.Y == CreateY + 28) { GameLoop.World.Player.ExpTrack = "Medium"; }
                    if (mousePos.X >= CreateX + 37 && mousePos.X <= CreateX + 40 && mousePos.Y == CreateY + 28) { GameLoop.World.Player.ExpTrack = "Fast"; }

                    if (mousePos.X >= 14 + CreateX && mousePos.X <= 47 + CreateX) {
                        if (mousePos.Y >= 16 + CreateY && mousePos.Y < CreateY + 28) {
                            ccRaceSelect = (mousePos.Y - CreateY) - 16;

                            if (ccRaceSelect < 0)
                                ccRaceSelect = 0;
                            if (ccRaceSelect > GameLoop.World.raceLibrary.Count - 1) {
                                ccRaceSelect = GameLoop.World.raceLibrary.Count - 1;
                            }

                            Race temp = GameLoop.World.raceLibrary[ccRaceSelect];
                            GameLoop.World.Player.Race.Copy(temp);

                        }
                    }
                } 
                else if (GameLoop.UIManager.selectedMenu == "LoadFile") {
                    int fileSize = Names.Length + 2;
                    if (fileSize < 20)
                        fileSize = 20;

                    if (mousePos.Y == 1 + fileSize - 3) {
                        RemakeMenu();
                        GameLoop.UIManager.selectedMenu = "MainMenu";
                    } else {
                        int fileSlot = mousePos.Y - 3;
                        if (Names.Length > fileSlot && fileSlot >= 0) {
                            GameLoop.World.LoadPlayer(Names[fileSlot]);
                            GameLoop.UIManager.selectedMenu = "ConnectOrHost";
                            GameLoop.UIManager.Map.UpdateVision(); 
                        }
                    }
                } 
                else if (GameLoop.UIManager.selectedMenu == "ConnectOrHost") {
                    if (mousePos.Y == 1) {
                        MainMenuWindow.IsVisible = false;
                        GameLoop.UIManager.Map.MapWindow.IsVisible = true;
                        GameLoop.UIManager.Map.MessageLog.IsVisible = true;
                        GameLoop.UIManager.Sidebar.SidebarWindow.IsVisible = true;
                        GameLoop.UIManager.selectedMenu = "None";

                        // Singleplayer
                    } else if (mousePos.Y == 2) {
                        MainMenuWindow.IsVisible = false;
                        GameLoop.UIManager.Map.MapWindow.IsVisible = true;
                        GameLoop.UIManager.Map.MessageLog.IsVisible = true;
                        GameLoop.UIManager.Sidebar.SidebarWindow.IsVisible = true;
                        GameLoop.UIManager.selectedMenu = "None";
                        GameLoop.NetworkManager = new NetworkManager(true);
                        // Host immediately
                        GameLoop.NetworkManager.CreateLobby();
                    } else if (mousePos.Y == 4 && mousePos.X >= 8 && mousePos.X <= 13) {
                        // Join game

                        if (LobbyCode.Length == 6) {
                            GameLoop.NetworkManager = new NetworkManager(false);
                            GameLoop.NetworkManager.SearchLobbiesAndJoin(LobbyCode);
                        } else {
                            joinError = "Enter lobby code first";
                        }
                    }
                }
            }
        }


        public int PointBuyCost(int target) {
            if (target == 7) return -4;
            if (target == 8) return -2;
            if (target == 9) return -1;
            if (target == 10) return 0;
            if (target == 11) return 1;
            if (target == 12) return 2;
            if (target == 13) return 3;
            if (target == 14) return 5;
            if (target == 15) return 7;
            if (target == 16) return 10;
            if (target == 17) return 13;
            if (target == 18) return 17;

            return 99;
        }
    }
}
