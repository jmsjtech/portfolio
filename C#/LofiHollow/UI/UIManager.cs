using SadRogue.Primitives;
using SadConsole; 
using System;
using Key = SadConsole.Input.Keys;
using LofiHollow.Entities;
using SadConsole.UI;
using System.Linq;
using SadConsole.Input;
using System.Collections.Generic;
using SadRex;
using Color = SadRogue.Primitives.Color;
using System.IO;
using LofiHollow.Entities.NPC;
using LofiHollow.TileData;
using Newtonsoft.Json;

namespace LofiHollow.UI {
    public class UIManager : ScreenObject {
        public SadConsole.UI.Colors CustomColors;

        public SadConsole.Console MainMenuConsole;
        public SadConsole.UI.ControlsConsole NameConsole;
        public SadConsole.UI.Controls.TextBox NameBox;
        public Window MainMenuWindow;
        public SadRex.Image MenuImage;
        
        public SadConsole.Console MapConsole;
        public Window MapWindow;
        public MessageLogWindow MessageLog;
        public Window SidebarWindow;
        public SadConsole.Console SidebarConsole;

        public SadConsole.Console BattleConsole;
        public SadConsole.Console BattleLog;
        public Window BattleWindow;
        public SadConsole.Console InvConsole;
        public Window InvWindow;

        public SadConsole.Console DialogueConsole;
        public Window DialogueWindow;

        public SadConsole.Console SignConsole;
        public Window SignWindow;

        public SadConsole.Console InventoryConsole;
        public Window InventoryWindow;

        public SadConsole.Entities.Renderer EntityRenderer;


        public GoRogue.FOV FOV;
        public bool LimitedVision = true;

        public Dictionary<Point3D, MinimapTile> minimap = new Dictionary<Point3D, MinimapTile>();


        public Point targetDir = new Point(0, 0);
        public bool BuyingFromShop = true;
        public List<Item> BuyingItems = new List<Item>();
        public List<Item> SellingItems = new List<Item>();
        public int BuyCopper = 0;
        public int BuySilver = 0;
        public int BuyGold = 0;
        public int BuyJade = 0;
        public string targetType = "None";
        public string selectedMenu = "None";
        public string battleResult = "None";
        public string dialogueOption = "None";
        public string dialogueLatest = "";
        public string chitChat1 = "";
        public string chitChat2 = "";
        public string chitChat3 = "";
        public string chitChat4 = "";
        public string chitChat5 = "";
        public bool battleDone = false;
        public string moveMenu = "None";
        public string signText = "";
        public bool AlreadyUsedItem = false;
        public bool PlayerFirst = false;

        public List<Item> dropTable = new List<Item>();

        public bool flying = false;
        public int tileIndex = 0;

        public int hotbarSelect = 0; 
        public int invMoveIndex = -1;
        public NPC DialogueNPC = null;

        public int charCreationStage = 0;
        public int ccPointBuyLeft = 20;
        public int ccClassTop = 0;
        public int ccClassSelect = -1;
        public int ccRaceSelect = 0;
        public int ccRaceTop = 0;
        public string[] Names;

        public UIManager() {
            // must be set to true
            // or will not call each child's Draw method
            IsVisible = true;
            IsFocused = true;

            // The UIManager becomes the only
            // screen that SadConsole processes
            Parent = GameHost.Instance.Screen;
        }

        public override void Update(TimeSpan timeElapsed) {
            if (selectedMenu == "MainMenu" || selectedMenu == "CharCreation" || selectedMenu == "LoadFile") {
                RenderMainMenu();
                CaptureMainMenuClicks();

            } else {
                if (GameLoop.World != null && GameLoop.World.DoneInitializing)
                    RenderSidebar();

                if (selectedMenu == "Sign") {
                    RenderSign();
                }

                if (selectedMenu == "Inventory") {
                    RenderInventory();
                }

                if (selectedMenu == "Battle" || selectedMenu == "TurnWait" || selectedMenu == "BattleDone") {
                    RenderBattle();

                    if (InvWindow.IsVisible) {
                        RenderBattleInv();
                        CaptureInvClicks();
                    } else {
                        CaptureBattleClicks();
                    }
                } else {
                    if (selectedMenu != "Dialogue") {
                        CheckKeyboard();
                        UpdateNPCs();
                    } else {
                        RenderDialogue();
                        CaptureDialogueClicks();
                    }
                }

                RenderOverlays();

                CheckFall();
            }
            base.Update(timeElapsed);
        } 

        public void Init() {
            SetupCustomColors();

            CreateConsoles(); 
            CreateSidebarWindow(28, GameLoop.GameHeight, "");
            CreateBattleWindow(72, 42, ""); 
            CreateInventoryWindow(GameLoop.GameWidth / 2, GameLoop.GameHeight / 2, "");
            CreateSignWindow((GameLoop.MapWidth / 2) - 1, GameLoop.MapHeight / 2, "");
            CreateDialogueWindow(72, 42, "");

            CreateMainMenu();

            MessageLog = new MessageLogWindow(72, 18, "Message Log");
            Children.Add(MessageLog);
            MessageLog.Show();
            MessageLog.Position = new Point(0, 42);
            
            MessageLog.IsVisible = false;

            EntityRenderer = new SadConsole.Entities.Renderer();
             
            UseMouse = true;
            selectedMenu = "MainMenu";
        }


        public void UpdateNPCs() {
            for (int i = 0; i < GameLoop.World.npcLibrary.Count; i++) {
                GameLoop.World.npcLibrary[i].Update(false);
            }

            if (LimitedVision) {
                for (int i = 0; i < EntityRenderer.Entities.Count; i++) {
                    Entity ent = (Entity) EntityRenderer.Entities[i];

                    if (FOV.CurrentFOV.Contains(new GoRogue.Coord(ent.Position.X, ent.Position.Y))) {
                        ent.IsVisible = true;
                    } else {
                        ent.IsVisible = false;
                    }

                    if (ent.MapPos.Z < GameLoop.World.Player.MapPos.Z) {
                        int depth = GameLoop.World.Player.MapPos.Z - ent.MapPos.Z;

                        Color shaded = new Color(ent.Appearance.Foreground.R, ent.Appearance.Foreground.G, ent.Appearance.Foreground.B, 255 - (depth * 51));

                        ent.Appearance.Foreground = shaded;
                    }
                }
            }
        }



        private void RenderMainMenu() {
            Point mousePos = new MouseScreenObjectState(MainMenuConsole, GameHost.Instance.Mouse).CellPosition;

            if (selectedMenu == "MainMenu") {
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




                MainMenuConsole.DrawBox(new Rectangle(40, 20, 20, 10), ShapeParameters.CreateStyledBoxFilled(ICellSurface.ConnectedLineThin, new ColoredGlyph(Color.White, Color.Black), new ColoredGlyph(Color.Black, Color.Black)));
                MainMenuConsole.Print(46, 22, new ColoredString("New Game", mousePos.Y == 22 ? Color.Yellow : Color.White, Color.Black));
                MainMenuConsole.Print(45, 23, new ColoredString("Load Game", mousePos.Y == 23 ? Color.Yellow : Color.White, Color.Black));
            } else if (selectedMenu == "CharCreation") {
                MainMenuConsole.DrawBox(new Rectangle(25, 5, 50, 50), ShapeParameters.CreateStyledBoxFilled(ICellSurface.ConnectedLineThin, new ColoredGlyph(Color.White, Color.Black), new ColoredGlyph(Color.Black, Color.Black)));

                int CreateX = 26;
                int CreateY = 6;

                MainMenuConsole.DrawLine(new Point(CreateX + 13, CreateY), new Point(CreateX + 13, CreateY + 47), (char)179, Color.White, Color.Black);
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

                MainMenuConsole.Print(CreateX + 1, CreateY + 1, new ColoredString("STR: ", Color.White, Color.Black));
                MainMenuConsole.Print(CreateX + 6, CreateY + 1, new ColoredString("- ", mousePos == new Point(CreateX + 6, CreateY + 1) ? Color.Yellow : Color.White, Color.Black));
                MainMenuConsole.Print(CreateX + 8, CreateY + 1, new ColoredString(STR, Color.White, Color.Black));
                MainMenuConsole.Print(CreateX + 10, CreateY + 1, new ColoredString(" +", mousePos == new Point(CreateX + 11, CreateY + 1) ? Color.Yellow : Color.White, Color.Black));

                MainMenuConsole.Print(CreateX + 1, CreateY + 2, new ColoredString("DEX: ", Color.White, Color.Black));
                MainMenuConsole.Print(CreateX + 6, CreateY + 2, new ColoredString("- ", mousePos == new Point(CreateX + 6, CreateY + 2) ? Color.Yellow : Color.White, Color.Black));
                MainMenuConsole.Print(CreateX + 8, CreateY + 2, new ColoredString(DEX, Color.White, Color.Black));
                MainMenuConsole.Print(CreateX + 10, CreateY + 2, new ColoredString(" +", mousePos == new Point(CreateX + 11, CreateY + 2) ? Color.Yellow : Color.White, Color.Black));

                MainMenuConsole.Print(CreateX + 1, CreateY + 3, new ColoredString("CON: ", Color.White, Color.Black));
                MainMenuConsole.Print(CreateX + 6, CreateY + 3, new ColoredString("- ", mousePos == new Point(CreateX + 6, CreateY + 3) ? Color.Yellow : Color.White, Color.Black));
                MainMenuConsole.Print(CreateX + 8, CreateY + 3, new ColoredString(CON, Color.White, Color.Black));
                MainMenuConsole.Print(CreateX + 10, CreateY + 3, new ColoredString(" +", mousePos == new Point(CreateX + 11, CreateY + 3) ? Color.Yellow : Color.White, Color.Black));
                 
                MainMenuConsole.Print(CreateX + 1, CreateY + 4, new ColoredString("INT: ", Color.White, Color.Black));
                MainMenuConsole.Print(CreateX + 6, CreateY + 4, new ColoredString("- ", mousePos == new Point(CreateX + 6, CreateY + 4) ? Color.Yellow : Color.White, Color.Black));
                MainMenuConsole.Print(CreateX + 8, CreateY + 4, new ColoredString(INT, Color.White, Color.Black));
                MainMenuConsole.Print(CreateX + 10, CreateY + 4, new ColoredString(" +", mousePos == new Point(CreateX + 11, CreateY + 4) ? Color.Yellow : Color.White, Color.Black));

                MainMenuConsole.Print(CreateX + 1, CreateY + 5, new ColoredString("WIS: ", Color.White, Color.Black));
                MainMenuConsole.Print(CreateX + 6, CreateY + 5, new ColoredString("- ", mousePos == new Point(CreateX + 6, CreateY + 5) ? Color.Yellow : Color.White, Color.Black));
                MainMenuConsole.Print(CreateX + 8, CreateY + 5, new ColoredString(WIS, Color.White, Color.Black));
                MainMenuConsole.Print(CreateX + 10, CreateY + 5, new ColoredString(" +", mousePos == new Point(CreateX + 11, CreateY + 5) ? Color.Yellow : Color.White, Color.Black));

                MainMenuConsole.Print(CreateX + 1, CreateY + 6, new ColoredString("CHA: ", Color.White, Color.Black));
                MainMenuConsole.Print(CreateX + 6, CreateY + 6, new ColoredString("- ", mousePos == new Point(CreateX + 6, CreateY + 6) ? Color.Yellow : Color.White, Color.Black));
                MainMenuConsole.Print(CreateX + 8, CreateY + 6, new ColoredString(CHA, Color.White, Color.Black));
                MainMenuConsole.Print(CreateX + 10, CreateY + 6, new ColoredString(" +", mousePos == new Point(CreateX + 11, CreateY + 6) ? Color.Yellow : Color.White, Color.Black));


                ccPointBuyLeft = 20 - (PointBuyCost(GameLoop.World.Player.STR) + PointBuyCost(GameLoop.World.Player.DEX) + PointBuyCost(GameLoop.World.Player.CON) +
                    PointBuyCost(GameLoop.World.Player.INT) + PointBuyCost(GameLoop.World.Player.WIS) + PointBuyCost(GameLoop.World.Player.CHA));

                MainMenuConsole.Print(CreateX + 1, CreateY + 8, new ColoredString("Points: " + ccPointBuyLeft + " ", ccPointBuyLeft > 0 ? Color.Lime : ccPointBuyLeft == 0 ? Color.White : Color.Red, Color.Black));

                MainMenuConsole.DrawLine(new Point(CreateX, CreateY + 10), new Point(CreateX + 12, CreateY + 10), (char)196, Color.White, Color.Black);


                // Class Picker
                MainMenuConsole.DrawBox(new Rectangle(CreateX + 13, CreateY - 1, 36, 15), ShapeParameters.CreateStyledBoxFilled(ICellSurface.ConnectedLineThin, new ColoredGlyph(Color.White, Color.Black), new ColoredGlyph(Color.Black, Color.Black)));
                 
                MainMenuConsole.DrawLine(new Point(CreateX + 25, CreateY), new Point(CreateX + 25, CreateY + 12), (char)124, Color.White, Color.Black);
                MainMenuConsole.DrawLine(new Point(CreateX + 31, CreateY), new Point(CreateX + 31, CreateY + 12), (char)124, Color.White, Color.Black);
                MainMenuConsole.DrawLine(new Point(CreateX + 36, CreateY), new Point(CreateX + 36, CreateY + 12), (char)124, Color.White, Color.Black);
                MainMenuConsole.DrawLine(new Point(CreateX + 38, CreateY), new Point(CreateX + 38, CreateY + 12), (char)124, Color.White, Color.Black);
                MainMenuConsole.DrawLine(new Point(CreateX + 40, CreateY), new Point(CreateX + 40, CreateY + 12), (char)124, Color.White, Color.Black);
                MainMenuConsole.DrawLine(new Point(CreateX + 42, CreateY), new Point(CreateX + 42, CreateY + 12), (char)124, Color.White, Color.Black);
                MainMenuConsole.DrawLine(new Point(CreateX + 14, CreateY + 1), new Point(CreateX + 47, CreateY + 1), (char)196, Color.White, Color.Black);


                MainMenuConsole.Print(CreateX + 14, CreateY, new ColoredString("Class Name".Align(HorizontalAlignment.Center, 11), Color.White, Color.Black));
                MainMenuConsole.Print(CreateX + 26, CreateY, new ColoredString("BAB".Align(HorizontalAlignment.Center, 5), Color.White, Color.Black));
                MainMenuConsole.Print(CreateX + 32, CreateY, new ColoredString("HD".Align(HorizontalAlignment.Center, 4), Color.White, Color.Black));
                MainMenuConsole.Print(CreateX + 37, CreateY, new ColoredString("F", Color.White, Color.Black));
                MainMenuConsole.Print(CreateX + 39, CreateY, new ColoredString("R", Color.White, Color.Black));
                MainMenuConsole.Print(CreateX + 41, CreateY, new ColoredString("W", Color.White, Color.Black));

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
                        MainMenuConsole.Print(CreateX + 14, CreateY + i + 2, new ColoredString(temp.Name.Align(HorizontalAlignment.Center, 11), temp.Name == classString ? Color.Lime : (mousePos.Y == CreateY + i + 2 && mousePos.X > CreateX + 13)  ? Color.Yellow : Color.White, Color.Black));
                        MainMenuConsole.Print(CreateX + 26, CreateY + i + 2, new ColoredString(temp.BABperLevel.ToString().Align(HorizontalAlignment.Center, 5), temp.Name == classString ? Color.Lime : (mousePos.Y == CreateY + i + 2 && mousePos.X > CreateX + 13) ? Color.Yellow : Color.White, Color.Black));
                        MainMenuConsole.Print(CreateX + 32, CreateY + i + 2, new ColoredString(temp.HitDie.Align(HorizontalAlignment.Center, 4), temp.Name == classString ? Color.Lime : (mousePos.Y == CreateY + i + 2 && mousePos.X > CreateX + 13) ? Color.Yellow : Color.White, Color.Black));
                        MainMenuConsole.Print(CreateX + 37, CreateY + i + 2, new ColoredString(temp.FortSaveProg.Substring(0, 1), temp.Name == classString ? Color.Lime : (mousePos.Y == CreateY + i + 2 && mousePos.X > CreateX + 13) ? Color.Yellow : Color.White, Color.Black));
                        MainMenuConsole.Print(CreateX + 39, CreateY + i + 2, new ColoredString(temp.RefSaveProg.Substring(0, 1), temp.Name == classString ? Color.Lime : (mousePos.Y == CreateY + i + 2 && mousePos.X > CreateX + 13) ? Color.Yellow : Color.White, Color.Black));
                        MainMenuConsole.Print(CreateX + 41, CreateY + i + 2, new ColoredString(temp.WillSaveProg.Substring(0, 1), temp.Name == classString ? Color.Lime : (mousePos.Y == CreateY + i + 2 && mousePos.X > CreateX + 13) ? Color.Yellow : Color.White, Color.Black));
                    }
                }


                MainMenuConsole.Print(CreateX + 1, CreateY + 11, new ColoredString("Name:", Color.White, Color.Black));

                MainMenuConsole.Print(CreateX + 1, CreateY + 14, new ColoredString(classString, Color.White, Color.Black));
                MainMenuConsole.Print(CreateX + 1, CreateY + 14, new ColoredString(classString, Color.White, Color.Black));


                MainMenuConsole.Print(CreateX + 14, CreateY + 28, new ColoredString("EXP Track:", Color.White, Color.Black));
                MainMenuConsole.Print(CreateX + 25, CreateY + 28, new ColoredString("Slow", GameLoop.World.Player.ExpTrack == "Slow" ? Color.Lime : (mousePos.X <= CreateX + 28 && mousePos.X >= CreateX + 25 && mousePos.Y == CreateY + 28) ? Color.Yellow : Color.White, Color.Black));
                MainMenuConsole.Print(CreateX + 30, CreateY + 28, new ColoredString("Medium", GameLoop.World.Player.ExpTrack == "Medium" ? Color.Lime : (mousePos.X <= CreateX + 35 && mousePos.X >= CreateX + 30 && mousePos.Y == CreateY + 28) ? Color.Yellow : Color.White, Color.Black));
                MainMenuConsole.Print(CreateX + 37, CreateY + 28, new ColoredString("Fast", GameLoop.World.Player.ExpTrack == "Fast" ? Color.Lime : (mousePos.X <= CreateX + 40 && mousePos.X >= CreateX + 37 && mousePos.Y == CreateY + 28) ? Color.Yellow : Color.White, Color.Black));

                // Race Box
                MainMenuConsole.DrawBox(new Rectangle(CreateX + 13, CreateY + 13, 36, 15), ShapeParameters.CreateStyledBoxFilled(ICellSurface.ConnectedLineThin, new ColoredGlyph(Color.White, Color.Black), new ColoredGlyph(Color.Black, Color.Black)));
                MainMenuConsole.Print(CreateX + 14, CreateY + 14, new ColoredString("Name".Align(HorizontalAlignment.Center, 10), Color.White, Color.Black));
                MainMenuConsole.Print(CreateX + 24, CreateY + 14, new ColoredString("Description".Align(HorizontalAlignment.Center, 24), Color.White, Color.Black));
                MainMenuConsole.DrawLine(new Point(CreateX + 14, CreateY + 15), new Point(CreateX + 47, CreateY + 15), (char)196, Color.White, Color.Black);

                for (int i = 0; i < 10; i++) {
                    if (GameLoop.World.raceLibrary.ContainsKey(i + ccRaceTop)) {
                        Race temp = GameLoop.World.raceLibrary[i + ccRaceTop];
                        MainMenuConsole.Print(CreateX + 14, CreateY + i + 16, new ColoredString(temp.Name.Align(HorizontalAlignment.Center, 9), GameLoop.World.Player.Race.Name == temp.Name ? Color.Lime : (mousePos.Y == CreateY + i + 16 && mousePos.X > CreateX + 13) ? Color.Yellow : Color.White, Color.Black));
                        MainMenuConsole.Print(CreateX + 24, CreateY + i + 16, new ColoredString(temp.Description.Align(HorizontalAlignment.Center, 24), GameLoop.World.Player.Race.Name == temp.Name ? Color.Lime : (mousePos.Y == CreateY + i + 16 && mousePos.X > CreateX + 13) ? Color.Yellow : Color.White, Color.Black));
                    }
                }

                MainMenuConsole.DrawLine(new Point(CreateX + 23, CreateY + 14), new Point(CreateX + 23, CreateY + 26), (char)124, Color.White, Color.Black);


                MainMenuConsole.Print(30, 53, new ColoredString("DONE", (mousePos.Y == 53 && mousePos.X <= 33 && mousePos.X >= 30) ? Color.Yellow : Color.White, Color.Black));
            } else if (selectedMenu == "LoadFile") {
                int fileSize = Names.Length + 2;
                if (fileSize < 20)
                    fileSize = 20;

                MainMenuConsole.DrawBox(new Rectangle(40, 20, 20, fileSize), ShapeParameters.CreateStyledBoxFilled(ICellSurface.ConnectedLineThin, new ColoredGlyph(Color.White, Color.Black), new ColoredGlyph(Color.Black, Color.Black)));
                MainMenuConsole.Print(41, 21, new ColoredString("Select a Save File", Color.White, Color.Black));
                MainMenuConsole.DrawLine(new Point(41, 22), new Point(58, 22), (char)196, Color.White, Color.Black);
                
                for (int i = 0; i < Names.Length; i++) {
                    MainMenuConsole.Print(41, 23 + i, new ColoredString(Names[i].Align(HorizontalAlignment.Center, 18), mousePos.Y == 23 + i ? Color.Yellow : Color.White, Color.Black));
                }

                MainMenuConsole.Print(41, 21 + fileSize - 3, new ColoredString("[BACK]".Align(HorizontalAlignment.Center, 18), mousePos.Y == (21 + fileSize - 3) ? Color.Yellow : Color.White, Color.Black));
            }
        }

        private void CaptureMainMenuClicks() {
            Point mousePos = new MouseScreenObjectState(MainMenuConsole, GameHost.Instance.Mouse).CellPosition;
            if (GameHost.Instance.Mouse.LeftClicked) {
                if (selectedMenu == "MainMenu") {
                    if (mousePos.Y == 22) {
                        selectedMenu = "CharCreation";
                        NameConsole.IsVisible = true;
                    } else if (mousePos.Y == 23) {
                        selectedMenu = "LoadFile";
                        if (Directory.Exists("./saves/")) {
                            Names = Directory.GetDirectories("./saves/"); 

                            for (int i = 0; i < Names.Length; i++) {
                                string[] split = Names[i].Split("/");
                                Names[i] = split[split.Length - 1];
                            }
                        }
                    }
                } else if (selectedMenu == "CharCreation") {
                    int CreateX = 26;
                    int CreateY = 6;
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


                    if (mousePos.Y == 53 && mousePos.X <= 33 && mousePos.X >= 30 && ccPointBuyLeft >= 0 && ccClassSelect != -1) {
                        selectedMenu = "None";
                        MainMenuWindow.IsVisible = false;
                        MapWindow.IsVisible = true;
                        MessageLog.IsVisible = true;
                        SidebarWindow.IsVisible = true;

                        ClassDef selectedClass = new ClassDef();
                        selectedClass.Copy(GameLoop.World.classLibrary[ccClassSelect]);
                        selectedClass.ClassLevels = 1;

                        GameLoop.World.Player.ClassLevels.Add(selectedClass);

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
                        UpdateVision();
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
                } else if (selectedMenu == "LoadFile") {
                    int fileSize = Names.Length + 2;
                    if (fileSize < 20)
                        fileSize = 20;

                    if (mousePos.Y == 21 + fileSize - 3) {
                        RemakeMenu();
                        selectedMenu = "MainMenu";
                    } else {
                        int fileSlot = mousePos.Y - 23;
                        if (Names.Length > fileSlot && fileSlot >= 0) {
                            GameLoop.World.LoadPlayer(Names[fileSlot]);
                            selectedMenu = "None";
                            MainMenuWindow.IsVisible = false;
                            MapWindow.IsVisible = true;
                            MessageLog.IsVisible = true;
                            SidebarWindow.IsVisible = true;
                            UpdateVision();
                        }
                    }
                }
            }
        }



        private void RenderDialogue() {
            DialogueConsole.Clear();
            Point mousePos = new MouseScreenObjectState(DialogueConsole, GameHost.Instance.Mouse).CellPosition;

            if (DialogueNPC != null) {
                DialogueWindow.IsFocused = true;
                int opinion = 0;
                if (GameLoop.World.Player.MetNPCs.ContainsKey(DialogueNPC.Name))
                    opinion = GameLoop.World.Player.MetNPCs[DialogueNPC.Name];
                
                DialogueWindow.Title = (DialogueNPC.Name + ", " + DialogueNPC.Occupation + " - " + DialogueNPC.RelationshipDescriptor() + " (" + opinion + ")").Align(HorizontalAlignment.Center, DialogueWindow.Width - 2, (char)196);

                if (dialogueOption == "None" || dialogueOption == "Goodbye") {
                    if (dialogueLatest.Contains('@') && !GameLoop.World.Player.Name.Contains('@')) {
                        int index = dialogueLatest.IndexOf('@');
                        dialogueLatest = dialogueLatest.Remove(index, 1);
                        dialogueLatest = dialogueLatest.Insert(index, GameLoop.World.Player.Name);
                    }
                    string[] allLines = dialogueLatest.Split("|");

                    for (int i = 0; i < allLines.Length; i++)
                        DialogueConsole.Print(1, 1 + i, new ColoredString(allLines[i], Color.White, Color.Black));
                }

                if (dialogueOption == "None") { 
                    //  DialogueConsole.Print(1, DialogueConsole.Height - 19, new ColoredString("Mission: [A book for Cobalt]", mousePos.Y == DialogueConsole.Height - 19 ? Color.Yellow : Color.White, Color.Black));

                    DialogueConsole.DrawLine(new Point(0, DialogueConsole.Height - 20), new Point(DialogueConsole.Width - 1, DialogueConsole.Height - 20), 196, Color.Orange, Color.Black);
                    
                    if (DialogueNPC.Shop != null && DialogueNPC.Shop.ShopOpen(DialogueNPC))
                        DialogueConsole.Print(1, DialogueConsole.Height - 17, new ColoredString("[Open Store]", mousePos.Y == DialogueConsole.Height - 17 ? Color.Yellow : Color.White, Color.Black));

                    DialogueConsole.Print(1, DialogueConsole.Height - 15, new ColoredString("[Give Item]", mousePos.Y == DialogueConsole.Height - 15 ? Color.Yellow : Color.White, Color.Black));

                    DialogueConsole.Print(1, DialogueConsole.Height - 12, new ColoredString("Chit-chat: " + chitChat1, mousePos.Y == DialogueConsole.Height - 12 ? Color.Yellow : Color.White, Color.Black));
                    DialogueConsole.Print(1, DialogueConsole.Height - 10, new ColoredString("Chit-chat: " + chitChat2, mousePos.Y == DialogueConsole.Height - 10 ? Color.Yellow : Color.White, Color.Black));
                    DialogueConsole.Print(1, DialogueConsole.Height - 8, new ColoredString("Chit-chat: " + chitChat3, mousePos.Y == DialogueConsole.Height - 8 ? Color.Yellow : Color.White, Color.Black));
                    DialogueConsole.Print(1, DialogueConsole.Height - 6, new ColoredString("Chit-chat: " + chitChat4, mousePos.Y == DialogueConsole.Height - 6 ? Color.Yellow : Color.White, Color.Black));
                    DialogueConsole.Print(1, DialogueConsole.Height - 4, new ColoredString("Chit-chat: " + chitChat5, mousePos.Y == DialogueConsole.Height - 4 ? Color.Yellow : Color.White, Color.Black));

                    DialogueConsole.Print(1, DialogueConsole.Height - 1, new ColoredString("Nevermind.", mousePos.Y == DialogueConsole.Height - 1 ? Color.Yellow : Color.White, Color.Black));
                } else if (dialogueOption == "Goodbye") {
                    DialogueConsole.Print(1, DialogueConsole.Height -1, new ColoredString("[Click anywhere to close]", mousePos.Y == DialogueConsole.Height - 1 ? Color.Yellow : Color.White, Color.Black));
                } else if (dialogueOption == "Gift") {
                    int y = 1;
                    DialogueConsole.Print(1, y++, "Give what?");

                    for (int i = 0; i < GameLoop.World.Player.Inventory.Length; i++) {
                        Item item = GameLoop.World.Player.Inventory[i];

                        string nameWithDurability = item.Name;

                        if (item.Durability >= 0)
                            nameWithDurability = "[" + item.Durability + "] " + item.Name;

                        DialogueConsole.Print(1, y, item.AsColoredGlyph());
                        if (!item.IsStackable || (item.IsStackable && item.ItemQuantity == 1))
                            DialogueConsole.Print(3, y, new ColoredString(nameWithDurability, mousePos.Y == y ? Color.Yellow : item.Name == "(EMPTY)" ? Color.DarkSlateGray : Color.White, Color.Black));
                        else
                            DialogueConsole.Print(3, y, new ColoredString(("(" + item.ItemQuantity + ") " + item.Name), mousePos.Y == y ? Color.Yellow : item.Name == "(EMPTY)" ? Color.DarkSlateGray : Color.White, Color.Black));

                        y++;
                    }

                    DialogueConsole.Print(1, y + 2, new ColoredString("Nevermind.", mousePos.Y == y + 2 ? Color.Yellow : Color.White, Color.Black));
                } else if (dialogueOption == "Shop") {
                    
                    ColoredString shopHeader = new ColoredString(DialogueNPC.Shop.ShopName, Color.White, Color.Black); 


                    ColoredString playerCopperString = new ColoredString("CP:" + GameLoop.World.Player.CopperCoins, new Color(184, 115, 51), Color.Black);
                    ColoredString playerSilverString = new ColoredString("SP:" + GameLoop.World.Player.SilverCoins, Color.Silver, Color.Black);
                    ColoredString playerGoldString = new ColoredString("GP:" + GameLoop.World.Player.GoldCoins, Color.Yellow, Color.Black);
                    ColoredString playerJadeString = new ColoredString("JP:" + GameLoop.World.Player.JadeCoins, new Color(0, 168, 107), Color.Black);

                    ColoredString playerMoney = new ColoredString("", Color.White, Color.Black);
                    playerMoney += playerCopperString + new ColoredString(" / ", Color.White, Color.Black);
                    playerMoney += playerSilverString + new ColoredString(" / ", Color.White, Color.Black);
                    playerMoney += playerGoldString + new ColoredString(" / ", Color.White, Color.Black);
                    playerMoney += playerJadeString;



                    DialogueConsole.Print(1, 0, GameLoop.World.Player.Name);
                    DialogueConsole.Print(1, 1, playerMoney);
                    DialogueConsole.Print(DialogueConsole.Width - (shopHeader.Length + 1), 0, shopHeader); 
                    DialogueConsole.DrawLine(new Point(0, 2), new Point(DialogueConsole.Width - 1, 2), 196);
                    if (BuyingFromShop) {
                        DialogueConsole.Print(0, 3, " Buy  |" + "Item Name".Align(HorizontalAlignment.Center, 23) + "|" + "Short Description".Align(HorizontalAlignment.Center, 27) + "|" + "Price".Align(HorizontalAlignment.Center, 11));
                        DialogueConsole.DrawLine(new Point(0, 4), new Point(DialogueConsole.Width - 1, 4), 196);


                        for (int i = 0; i < DialogueNPC.Shop.SoldItems.Count; i++) {
                            Item item = new Item(DialogueNPC.Shop.SoldItems[i]);
                            int price = DialogueNPC.Shop.GetPrice(GameLoop.World.Player.MetNPCs[DialogueNPC.Name], item, false);

                            int buyQuantity = 0;

                            for (int j = 0; j < BuyingItems.Count; j++) {
                                if (BuyingItems[j].ItemID == item.ItemID && BuyingItems[j].SubID == item.SubID) {
                                    buyQuantity += BuyingItems[j].ItemQuantity;
                                }
                            }

                            DialogueConsole.Print(0, 5 + i, new ColoredString("-", mousePos.Y == 5 + i && mousePos.X == 0 ? Color.Red : Color.White, Color.Black));
                            DialogueConsole.Print(1, 5 + i, new ColoredString(buyQuantity.ToString().Align(HorizontalAlignment.Center, 3), Color.White, Color.Black));
                            DialogueConsole.Print(5, 5 + i, new ColoredString("+", mousePos.Y == 5 + i && mousePos.X == 5 ? Color.Lime : Color.White, Color.Black));
                            DialogueConsole.Print(6, 5 + i, new ColoredString("|" + item.Name.Align(HorizontalAlignment.Center, 23) + "|" + item.ShortDesc.Align(HorizontalAlignment.Center, 27) + "|", mousePos.Y == 5 + i ? Color.Yellow : Color.White, Color.Black));
                            DialogueConsole.Print(DialogueConsole.Width - 10, 5 + i, ConvertCoppers(price));
                        }

                        DialogueConsole.DrawLine(new Point(0, DialogueConsole.Height - 7), new Point(DialogueConsole.Width - 1, DialogueConsole.Height - 7), 196);
                        DialogueConsole.Print(1, DialogueConsole.Height - 6, new ColoredString("[View Inventory]", (mousePos.Y == DialogueConsole.Height - 6 && mousePos.X >= 1 && mousePos.X <= 16) ? Color.Yellow : Color.White, Color.Black));
                    } else {
                        DialogueConsole.Print(0, 3, " Sell |" + "Item Name".Align(HorizontalAlignment.Center, 23) + "|" + "Short Description".Align(HorizontalAlignment.Center, 27) + "|" + "Price".Align(HorizontalAlignment.Center, 11));
                        DialogueConsole.DrawLine(new Point(0, 4), new Point(DialogueConsole.Width - 1, 4), 196);


                        for (int i = 0; i < GameLoop.World.Player.Inventory.Length; i++) {
                            Item item = new Item(GameLoop.World.Player.Inventory[i]);
                            item.ItemQuantity = GameLoop.World.Player.Inventory[i].ItemQuantity;
                            int price = DialogueNPC.Shop.GetPrice(GameLoop.World.Player.MetNPCs[DialogueNPC.Name], item, true);

                            int sellQuantity = 0;

                            for (int j = 0; j < SellingItems.Count; j++) {
                                if (SellingItems[j].ItemID == item.ItemID && SellingItems[j].SubID == item.SubID) {
                                    sellQuantity = SellingItems[j].ItemQuantity;
                                }
                            }

                            string name = item.Name;

                            if (item.ItemQuantity > 1) {
                                name = "[" + item.ItemQuantity + "] " + name;
                            }

                            DialogueConsole.Print(0, 5 + i, new ColoredString("-", mousePos.Y == 5 + i && mousePos.X == 0 ? Color.Red : Color.White, Color.Black));
                            DialogueConsole.Print(1, 5 + i, new ColoredString(sellQuantity.ToString().Align(HorizontalAlignment.Center, 3), Color.White, Color.Black));
                            DialogueConsole.Print(5, 5 + i, new ColoredString("+", mousePos.Y == 5 + i && mousePos.X == 5 ? Color.Lime : Color.White, Color.Black));
                            DialogueConsole.Print(6, 5 + i, new ColoredString("|" + name.Align(HorizontalAlignment.Center, 23) + "|" + item.ShortDesc.Align(HorizontalAlignment.Center, 27) + "|", mousePos.Y == 5 + i ? Color.Yellow : Color.White, Color.Black));
                            DialogueConsole.Print(DialogueConsole.Width - 10, 5 + i, ConvertCoppers(price));
                        }

                        DialogueConsole.DrawLine(new Point(0, DialogueConsole.Height - 7), new Point(DialogueConsole.Width - 1, DialogueConsole.Height - 7), 196);
                        DialogueConsole.Print(1, DialogueConsole.Height - 6, new ColoredString("[View Shop]", (mousePos.Y == DialogueConsole.Height - 6 && mousePos.X >= 1 && mousePos.X <= 11) ? Color.Yellow : Color.White, Color.Black));
                    }
                    
                    int buyValue = 0;

                    for (int j = 0; j < BuyingItems.Count; j++) {
                        buyValue += BuyingItems[j].ItemQuantity * DialogueNPC.Shop.GetPrice(GameLoop.World.Player.MetNPCs[DialogueNPC.Name], BuyingItems[j], false);
                    }

                    int sellValue = 0;

                    for (int j = 0; j < SellingItems.Count; j++) {
                        sellValue += SellingItems[j].ItemQuantity * DialogueNPC.Shop.GetPrice(GameLoop.World.Player.MetNPCs[DialogueNPC.Name], SellingItems[j], true);
                    }

                    DialogueConsole.Print(28, DialogueConsole.Height - 6, "Coins");

                    ColoredString buyCopperString = new ColoredString(BuyCopper.ToString(), new Color(184, 115, 51), Color.Black);
                    ColoredString buySilverString = new ColoredString(BuySilver.ToString(), Color.Silver, Color.Black);
                    ColoredString buyGoldString = new ColoredString(BuyGold.ToString(), Color.Yellow, Color.Black);
                    ColoredString buyJadeString = new ColoredString(BuyJade.ToString(), new Color(0, 168, 107), Color.Black);

                    DialogueConsole.Print(30, DialogueConsole.Height - 5, buyCopperString);
                    DialogueConsole.Print(30, DialogueConsole.Height - 4, buySilverString);
                    DialogueConsole.Print(30, DialogueConsole.Height - 3, buyGoldString);
                    DialogueConsole.Print(30, DialogueConsole.Height - 2, buyJadeString);

                    DialogueConsole.Print(28, DialogueConsole.Height - 5, new ColoredString("-", mousePos == new Point(28, DialogueConsole.Height - 5) ? Color.Red : Color.White, Color.Black));
                    DialogueConsole.Print(28, DialogueConsole.Height - 4, new ColoredString("-", mousePos == new Point(28, DialogueConsole.Height - 4) ? Color.Red : Color.White, Color.Black));
                    DialogueConsole.Print(28, DialogueConsole.Height - 3, new ColoredString("-", mousePos == new Point(28, DialogueConsole.Height - 3) ? Color.Red : Color.White, Color.Black));
                    DialogueConsole.Print(28, DialogueConsole.Height - 2, new ColoredString("-", mousePos == new Point(28, DialogueConsole.Height - 2) ? Color.Red : Color.White, Color.Black));

                    DialogueConsole.Print(32, DialogueConsole.Height - 5, new ColoredString("+", mousePos == new Point(32, DialogueConsole.Height - 5) ? Color.Lime : Color.White, Color.Black));
                    DialogueConsole.Print(32, DialogueConsole.Height - 4, new ColoredString("+", mousePos == new Point(32, DialogueConsole.Height - 4) ? Color.Lime : Color.White, Color.Black));
                    DialogueConsole.Print(32, DialogueConsole.Height - 3, new ColoredString("+", mousePos == new Point(32, DialogueConsole.Height - 3) ? Color.Lime : Color.White, Color.Black));
                    DialogueConsole.Print(32, DialogueConsole.Height - 2, new ColoredString("+", mousePos == new Point(32, DialogueConsole.Height - 2) ? Color.Lime : Color.White, Color.Black));

                    sellValue += BuyCopper;
                    sellValue += BuySilver * 10;
                    sellValue += BuyGold * 100;
                    sellValue += BuyJade * 1000;

                    DialogueConsole.Print(27, DialogueConsole.Height - 1, "[EXACT]");

                    DialogueConsole.Print(1, DialogueConsole.Height - 4, new ColoredString("Buy Value: ", Color.White, Color.Black) + ConvertCoppers(buyValue));
                    DialogueConsole.Print(1, DialogueConsole.Height - 3, new ColoredString("Sell Value: ", Color.White, Color.Black) + ConvertCoppers(sellValue));

                    int diff = buyValue - sellValue;

                    string total = diff > 0 ? "You owe " : diff == 0 ? "Trade is equal" : "You are owed ";

                    if (diff < 0)
                        DialogueConsole.Print(1, DialogueConsole.Height - 2, new ColoredString(total, Color.White, Color.Black) + ConvertCoppers(-diff));
                    else if (diff > 0)
                        DialogueConsole.Print(1, DialogueConsole.Height - 2, new ColoredString(total, Color.White, Color.Black) + ConvertCoppers(diff));
                    else 
                        DialogueConsole.Print(1, DialogueConsole.Height - 2, new ColoredString(total, Color.White, Color.Black));

                    if (diff <= 0)
                        DialogueConsole.Print(DialogueConsole.Width - 15, DialogueConsole.Height - 5, new ColoredString("[Accept - Gift]", Color.Green, Color.Black));
                    else
                        DialogueConsole.Print(DialogueConsole.Width - 15, DialogueConsole.Height - 5, new ColoredString("[Accept - Gift]", Color.Red, Color.Black));


                    if (diff <= 0)
                        DialogueConsole.Print(DialogueConsole.Width - 8, DialogueConsole.Height - 3, new ColoredString("[Accept]", Color.Green, Color.Black));
                    else
                        DialogueConsole.Print(DialogueConsole.Width - 8, DialogueConsole.Height - 3, new ColoredString("[Accept]", Color.Red, Color.Black));

                    DialogueConsole.Print(DialogueConsole.Width - 12, DialogueConsole.Height - 1, new ColoredString("[Close Shop]", (mousePos.Y == DialogueConsole.Height - 1 && mousePos.X >= DialogueConsole.Width - 12 && mousePos.X <= DialogueConsole.Width - 1) ? Color.Yellow : Color.White, Color.Black));

                }
            }
        }

        private void CaptureDialogueClicks() {
            Point mousePos = new MouseScreenObjectState(DialogueConsole, GameHost.Instance.Mouse).CellPosition;
            if (GameHost.Instance.Mouse.LeftClicked) {
                if (dialogueOption == "Goodbye") {
                    dialogueOption = "None";
                    selectedMenu = "None";
                    DialogueWindow.IsVisible = false;
                    DialogueNPC = null;
                    dialogueLatest = "";
                    DialogueConsole.Clear();
                } else if (dialogueOption == "Shop") {
                    if (mousePos.Y == DialogueConsole.Height - 1 && mousePos.X >= DialogueConsole.Width - 12 && mousePos.X <= DialogueConsole.Width - 1) {
                        dialogueOption = "None";
                        BuyingItems.Clear();
                        SellingItems.Clear();
                        BuyingFromShop = true;
                    }

                    if (mousePos.X == 28) {
                        if (mousePos.Y == DialogueConsole.Height - 5 && BuyCopper > 0 ) { BuyCopper--; }
                        if (mousePos.Y == DialogueConsole.Height - 4 && BuySilver > 0) { BuySilver--; }
                        if (mousePos.Y == DialogueConsole.Height - 3 && BuyGold > 0) { BuyGold--; }
                        if (mousePos.Y == DialogueConsole.Height - 2 && BuyJade > 0) { BuyJade--; } 
                    }

                    if (mousePos.X == 32) {
                        if (mousePos.Y == DialogueConsole.Height - 5 && BuyCopper < GameLoop.World.Player.CopperCoins) { BuyCopper++; }
                        if (mousePos.Y == DialogueConsole.Height - 4 && BuySilver < GameLoop.World.Player.SilverCoins) { BuySilver++; }
                        if (mousePos.Y == DialogueConsole.Height - 3 && BuyGold < GameLoop.World.Player.GoldCoins) { BuyGold++; }
                        if (mousePos.Y == DialogueConsole.Height - 2 && BuyJade < GameLoop.World.Player.JadeCoins) { BuyJade++; }
                    }

                    int buyValue = 0; 
                    for (int j = 0; j < BuyingItems.Count; j++) {
                        buyValue += BuyingItems[j].ItemQuantity * DialogueNPC.Shop.GetPrice(GameLoop.World.Player.MetNPCs[DialogueNPC.Name], BuyingItems[j], false);
                    }

                    int sellValue = 0; 
                    for (int j = 0; j < SellingItems.Count; j++) {
                        sellValue += SellingItems[j].ItemQuantity * DialogueNPC.Shop.GetPrice(GameLoop.World.Player.MetNPCs[DialogueNPC.Name], SellingItems[j], true);
                    }

                    sellValue += BuyCopper;
                    sellValue += BuySilver * 10;
                    sellValue += BuyGold * 100;
                    sellValue += BuyJade * 1000;

                    int diff = buyValue - sellValue;

                    if (mousePos.X >= 27 && mousePos.X <= 33 && mousePos.Y == DialogueConsole.Height - 1 && diff > 0) {
                        if (diff >= 1000)
                            BuyJade = diff / 1000;
                        diff -= BuyJade * 1000;

                        if (diff >= 100)
                            BuyGold = diff / 100;
                        diff -= BuyGold * 100;

                        if (diff >= 10)
                            BuySilver = diff / 10;
                        diff -= BuySilver * 10;

                        BuyCopper = diff;

                        if (BuyJade > GameLoop.World.Player.JadeCoins) {
                            int jadeOff = BuyJade - GameLoop.World.Player.JadeCoins;
                            BuyJade = GameLoop.World.Player.JadeCoins;
                            BuyGold += jadeOff * 10;
                        }

                        if (BuyGold > GameLoop.World.Player.GoldCoins) {
                            int goldOff = BuyJade - GameLoop.World.Player.GoldCoins;
                            BuyGold = GameLoop.World.Player.GoldCoins;
                            BuySilver += goldOff * 10;
                        }

                        if (BuySilver > GameLoop.World.Player.SilverCoins) {
                            int silverOff = BuyJade - GameLoop.World.Player.SilverCoins;
                            BuySilver = GameLoop.World.Player.SilverCoins;
                            BuyCopper += silverOff * 10;
                        }

                        if (BuyCopper > GameLoop.World.Player.CopperCoins) {
                            BuyCopper = GameLoop.World.Player.CopperCoins;
                        }

                    }

                    if (diff <= 0) {
                        if (mousePos.X >= DialogueConsole.Width - 8 && mousePos.X <= DialogueConsole.Width - 3 && mousePos.Y == DialogueConsole.Height - 3) {
                            GameLoop.World.Player.CopperCoins -= BuyCopper;
                            GameLoop.World.Player.SilverCoins -= BuySilver;
                            GameLoop.World.Player.GoldCoins -= BuyGold;
                            GameLoop.World.Player.JadeCoins -= BuyJade;

                            diff *= -1;
                            int plat = 0;
                            int gold = 0;
                            int silver = 0; 

                            if (diff > 1000)
                                plat = diff / 1000;
                            diff -= plat * 1000;

                            if (diff > 100)
                                gold = diff / 100;
                            diff -= gold * 100;

                            if (diff > 10)
                                silver = diff / 10;
                            diff -= silver * 10;
                             

                            GameLoop.World.Player.CopperCoins += diff;
                            GameLoop.World.Player.SilverCoins += silver;
                            GameLoop.World.Player.GoldCoins += gold;
                            GameLoop.World.Player.JadeCoins += plat;

                            for (int i = 0; i < SellingItems.Count; i++) {
                                for (int j = 0; j < GameLoop.World.Player.Inventory.Length; j++) {
                                    if (GameLoop.World.Player.Inventory[j].ItemID == SellingItems[i].ItemID && GameLoop.World.Player.Inventory[j].SubID == SellingItems[i].SubID) {
                                        GameLoop.World.Player.Inventory[j].ItemQuantity -= SellingItems[i].ItemQuantity;
                                        if (GameLoop.World.Player.Inventory[j].ItemQuantity <= 0) {
                                            GameLoop.World.Player.Inventory[j] = new Item(0);
                                        }
                                        break;
                                    }
                                }
                            }

                            for (int i = 0; i < BuyingItems.Count; i++) {
                                GameLoop.CommandManager.AddItemToInv(GameLoop.World.Player, BuyingItems[i]);
                            }

                            BuyCopper = 0;
                            BuySilver = 0;
                            BuyGold = 0;
                            BuyJade = 0;

                            BuyingItems.Clear();
                            SellingItems.Clear();
                        } else if (mousePos.X >= DialogueConsole.Width - 15 && mousePos.X <= DialogueConsole.Width - 3 && mousePos.Y == DialogueConsole.Height - 5) {
                            GameLoop.World.Player.CopperCoins -= BuyCopper;
                            GameLoop.World.Player.SilverCoins -= BuySilver;
                            GameLoop.World.Player.GoldCoins -= BuyGold;
                            GameLoop.World.Player.JadeCoins -= BuyJade;

                            diff *= -1;
                            
                            if (diff >= 100) {
                                string reaction = DialogueNPC.ReactGift(-2);
                                if (DialogueNPC.GiftResponses.ContainsKey(reaction))
                                    dialogueLatest = DialogueNPC.GiftResponses[reaction];
                                else
                                    dialogueLatest = "Error - No response for " + reaction + " gift.";
                            } else if (diff >= 10) {
                                string reaction = DialogueNPC.ReactGift(-3);
                                if (DialogueNPC.GiftResponses.ContainsKey(reaction))
                                    dialogueLatest = DialogueNPC.GiftResponses[reaction];
                                else
                                    dialogueLatest = "Error - No response for " + reaction + " gift.";
                            }



                            for (int i = 0; i < SellingItems.Count; i++) {
                                for (int j = 0; j < GameLoop.World.Player.Inventory.Length; j++) {
                                    if (GameLoop.World.Player.Inventory[j].ItemID == SellingItems[i].ItemID && GameLoop.World.Player.Inventory[j].SubID == SellingItems[i].SubID) {
                                        GameLoop.World.Player.Inventory[j].ItemQuantity -= SellingItems[i].ItemQuantity;
                                        if (GameLoop.World.Player.Inventory[j].ItemQuantity <= 0) {
                                            GameLoop.World.Player.Inventory[j] = new Item(0);
                                        }
                                        break;
                                    }
                                }
                            }

                            for (int i = 0; i < BuyingItems.Count; i++) {
                                GameLoop.CommandManager.AddItemToInv(GameLoop.World.Player, BuyingItems[i]);
                            }

                            BuyCopper = 0;
                            BuySilver = 0;
                            BuyGold = 0;
                            BuyJade = 0;

                            BuyingItems.Clear();
                            SellingItems.Clear();
                            dialogueOption = "None";
                            BuyingFromShop = false;
                            return;
                        }
                    }

                    if (BuyingFromShop) {
                        if (mousePos.Y == DialogueConsole.Height - 6 && mousePos.X >= 1 && mousePos.X <= 16) {
                            BuyingFromShop = false;
                        } else {
                            int itemSlot = mousePos.Y - 5;
                            if (itemSlot >= 0 && itemSlot <= DialogueNPC.Shop.SoldItems.Count) {
                                if (mousePos.X == 0) {
                                    for (int i = 0; i < BuyingItems.Count; i++) {
                                        if (BuyingItems[i].ItemID == DialogueNPC.Shop.SoldItems[itemSlot]) {
                                            if (BuyingItems[i].IsStackable && BuyingItems[i].ItemQuantity > 1) {
                                                BuyingItems[i].ItemQuantity--;
                                                break;
                                            } else if (!BuyingItems[i].IsStackable || BuyingItems[i].ItemQuantity == 1) {
                                                BuyingItems.RemoveAt(i);
                                                break;
                                            }
                                        }
                                    }
                                } else if (mousePos.X == 5) {
                                    bool alreadyInList = false;
                                    for (int i = 0; i < BuyingItems.Count; i++) {
                                        if (BuyingItems[i].ItemID == DialogueNPC.Shop.SoldItems[itemSlot]) {
                                            if (BuyingItems[i].IsStackable) {
                                                alreadyInList = true;
                                                BuyingItems[i].ItemQuantity++;
                                                break;
                                            } else if (!BuyingItems[i].IsStackable) {
                                                alreadyInList = true;
                                                BuyingItems.Add(new Item(DialogueNPC.Shop.SoldItems[itemSlot]));
                                                break;
                                            }
                                        }
                                    }

                                    if (!alreadyInList) {
                                        BuyingItems.Add(new Item(DialogueNPC.Shop.SoldItems[itemSlot]));
                                    }
                                }
                            }
                        }
                    } else {
                        if (mousePos.Y == DialogueConsole.Height - 6 && mousePos.X >= 1 && mousePos.X <= 11) {
                            BuyingFromShop = true;
                        } else {
                            int itemSlot = mousePos.Y - 5;
                            if (itemSlot >= 0 && itemSlot <= GameLoop.World.Player.Inventory.Length) {
                                if (mousePos.X == 0) {
                                    for (int i = 0; i < SellingItems.Count; i++) {
                                        if (SellingItems[i].ItemID == GameLoop.World.Player.Inventory[itemSlot].ItemID && SellingItems[i].SubID == GameLoop.World.Player.Inventory[itemSlot].SubID) {
                                            if (SellingItems[i].IsStackable && SellingItems[i].ItemQuantity > 1) {
                                                SellingItems[i].ItemQuantity--;
                                                break;
                                            } else if (!SellingItems[i].IsStackable || SellingItems[i].ItemQuantity == 1) {
                                                SellingItems.RemoveAt(i);
                                                break;
                                            }
                                        }
                                    }
                                } else if (mousePos.X == 5) {
                                    bool alreadyInList = false;
                                    
                                    for (int i = 0; i < SellingItems.Count; i++) {
                                        int thisItemCount = 0;
                                        int alreadyInListCount = 0;
                                        if (SellingItems[i].ItemID == GameLoop.World.Player.Inventory[itemSlot].ItemID && SellingItems[i].SubID == GameLoop.World.Player.Inventory[itemSlot].SubID) {
                                            if (SellingItems[i].IsStackable) {
                                                alreadyInList = true;
                                                if (SellingItems[i].ItemQuantity < GameLoop.World.Player.Inventory[itemSlot].ItemQuantity) { 
                                                    SellingItems[i].ItemQuantity++;
                                                    break;
                                                }
                                            } else if (!SellingItems[i].IsStackable) {
                                                for (int j = 0; j < GameLoop.World.Player.Inventory.Length; j++) {
                                                    if (GameLoop.World.Player.Inventory[j].ItemID == SellingItems[i].ItemID && GameLoop.World.Player.Inventory[j].SubID == SellingItems[i].SubID) {
                                                        thisItemCount++;
                                                    }
                                                }

                                                for (int j = 0; j < SellingItems.Count; j++) {
                                                    if (GameLoop.World.Player.Inventory[itemSlot].ItemID == SellingItems[j].ItemID && GameLoop.World.Player.Inventory[itemSlot].SubID == SellingItems[j].SubID) {
                                                        alreadyInListCount++;
                                                    }
                                                }

                                                if (alreadyInListCount < thisItemCount) {
                                                    alreadyInList = true;
                                                    SellingItems.Add(new Item(GameLoop.World.Player.Inventory[itemSlot]));
                                                    break;
                                                } else {
                                                    alreadyInList = true;
                                                }
                                            }
                                        }
                                    }

                                    if (!alreadyInList) {
                                        SellingItems.Add(new Item(GameLoop.World.Player.Inventory[itemSlot]));
                                        SellingItems[SellingItems.Count - 1].ItemQuantity = 1;
                                    }
                                }
                            }
                        }
                    }
                } else if (dialogueOption == "Gift") {
                    if (mousePos.Y == 4 + GameLoop.World.Player.Inventory.Length) {
                        dialogueOption = "None";
                    } else if (mousePos.Y >= 2 && mousePos.Y <= 2 + GameLoop.World.Player.Inventory.Length) {
                        int slot = mousePos.Y - 2;
                        int itemID = GameLoop.CommandManager.RemoveOneItem(GameLoop.World.Player, slot);

                        if (itemID != -1 && itemID != 0) {
                            string reaction = DialogueNPC.ReactGift(itemID);
                            dialogueOption = "None";
                            if (DialogueNPC.GiftResponses.ContainsKey(reaction))
                                dialogueLatest = DialogueNPC.GiftResponses[reaction];
                            else
                                dialogueLatest = "Error - No response for " + reaction + " gift.";
                        }
                    }
                } else if (dialogueOption == "None") {
                    if (mousePos.Y == DialogueConsole.Height - 1) {
                        dialogueOption = "Goodbye";
                        if (DialogueNPC.Farewells.ContainsKey(DialogueNPC.RelationshipDescriptor())) {
                            dialogueLatest = DialogueNPC.Farewells[DialogueNPC.RelationshipDescriptor()];
                        } else {
                            dialogueLatest = "Error: Greeting not found for relationship " + DialogueNPC.RelationshipDescriptor();
                        }
                    } else if (mousePos.Y <= DialogueConsole.Height - 4 && mousePos.Y >= DialogueConsole.Height - 12) {
                        string chat = "";

                        if (mousePos.Y == DialogueConsole.Height - 12)
                            chat = DialogueNPC.ChitChats[chitChat1];
                        else if (mousePos.Y == DialogueConsole.Height - 10)
                            chat = DialogueNPC.ChitChats[chitChat2];
                        else if (mousePos.Y == DialogueConsole.Height - 8)
                            chat = DialogueNPC.ChitChats[chitChat3];
                        else if (mousePos.Y == DialogueConsole.Height - 6)
                            chat = DialogueNPC.ChitChats[chitChat4];
                        else if (mousePos.Y == DialogueConsole.Height - 4)
                            chat = DialogueNPC.ChitChats[chitChat5];

                        if (chat != "") {
                            string[] chatParts = chat.Split("~");

                            if (chatParts.Length == 2) {
                                dialogueLatest = chatParts[1];
                                GameLoop.World.Player.MetNPCs[DialogueNPC.Name] += Int32.Parse(chatParts[0]);
                                DialogueNPC.UpdateChitChats();
                            }
                        }
                    } else if (mousePos.Y == DialogueConsole.Height - 15) { // Give item
                        dialogueOption = "Gift";
                    } else if (mousePos.Y == DialogueConsole.Height - 17) { // Open shop dialogue
                        if (DialogueNPC.Shop != null && DialogueNPC.Shop.ShopOpen(DialogueNPC))
                            dialogueOption = "Shop";
                    }
                }
            }
        }






        private void CheckKeyboard() {
            if (selectedMenu != "Sign" && selectedMenu != "Targeting") { 
                if (GameHost.Instance.Keyboard.IsKeyDown(Key.LeftControl)) {
                    if (GameHost.Instance.Keyboard.IsKeyPressed(Key.W)) { 
                        GameLoop.CommandManager.MoveActorTo(GameLoop.World.Player, GameLoop.World.Player.Position, GameLoop.World.Player.MapPos + new Point3D(0, -1, 0)); 
                    }
                    if (GameHost.Instance.Keyboard.IsKeyPressed(Key.S)) {
                        GameLoop.CommandManager.MoveActorTo(GameLoop.World.Player, GameLoop.World.Player.Position, GameLoop.World.Player.MapPos + new Point3D(0, 1, 0));
                    }
                    if (GameHost.Instance.Keyboard.IsKeyPressed(Key.A)) {
                        GameLoop.CommandManager.MoveActorTo(GameLoop.World.Player, GameLoop.World.Player.Position, GameLoop.World.Player.MapPos + new Point3D(-1, 0, 0));
                    }
                    if (GameHost.Instance.Keyboard.IsKeyPressed(Key.D)) {
                        GameLoop.CommandManager.MoveActorTo(GameLoop.World.Player, GameLoop.World.Player.Position, GameLoop.World.Player.MapPos + new Point3D(1, 0, 0));
                    }
                } else {
                    if (GameHost.Instance.Keyboard.IsKeyDown(Key.W)) { 
                        GameLoop.CommandManager.MoveActorBy(GameLoop.World.Player, new Point(0, -1));
                        GameLoop.UIManager.UpdateVision();
                    }
                    if (GameHost.Instance.Keyboard.IsKeyDown(Key.S)) { 
                        GameLoop.CommandManager.MoveActorBy(GameLoop.World.Player, new Point(0, 1));
                        GameLoop.UIManager.UpdateVision();
                    }
                    if (GameHost.Instance.Keyboard.IsKeyDown(Key.A)) { 
                        GameLoop.CommandManager.MoveActorBy(GameLoop.World.Player, new Point(-1, 0));
                        GameLoop.UIManager.UpdateVision();
                    }
                    if (GameHost.Instance.Keyboard.IsKeyDown(Key.D)) {
                        GameLoop.CommandManager.MoveActorBy(GameLoop.World.Player, new Point(1, 0));
                        GameLoop.UIManager.UpdateVision();
                    }
                    if (GameHost.Instance.Keyboard.IsKeyDown(Key.LeftShift) && GameHost.Instance.Keyboard.IsKeyPressed(Key.OemPeriod)) {
                        if (GameLoop.World.maps[GameLoop.World.Player.MapPos].GetTile(GameLoop.World.Player.Position).Name == "Down Stairs") {
                            GameLoop.CommandManager.MoveActorTo(GameLoop.World.Player, GameLoop.World.Player.Position, GameLoop.World.Player.MapPos + new Point3D(0, 0, -1));
                            GameLoop.UIManager.UpdateVision();
                        }
                    }

                    if (GameHost.Instance.Keyboard.IsKeyDown(Key.LeftShift) && GameHost.Instance.Keyboard.IsKeyPressed(Key.OemComma)) {
                        if (GameLoop.World.maps[GameLoop.World.Player.MapPos].GetTile(GameLoop.World.Player.Position).Name == "Up Stairs") {
                            GameLoop.CommandManager.MoveActorTo(GameLoop.World.Player, GameLoop.World.Player.Position, GameLoop.World.Player.MapPos + new Point3D(0, 0, 1));
                            GameLoop.UIManager.UpdateVision();
                        }
                    }

                    if (GameHost.Instance.Mouse.ScrollWheelValueChange > 0) {
                        if (hotbarSelect + 1 < GameLoop.World.Player.Inventory.Length)
                            hotbarSelect++;
                    } else if (GameHost.Instance.Mouse.ScrollWheelValueChange < 0) {
                        if (hotbarSelect > 0)
                            hotbarSelect--;
                    }

                    if (GameHost.Instance.Keyboard.IsKeyReleased(Key.I)) {
                        if (InventoryWindow.IsVisible) {
                            selectedMenu = "None";
                            InventoryWindow.IsVisible = false;
                            MapConsole.IsFocused = true;
                        } else {
                            selectedMenu = "Inventory";
                            InventoryWindow.IsVisible = true;
                            InventoryWindow.IsFocused = true;
                        }
                    }

                    if (GameHost.Instance.Keyboard.IsKeyReleased(Key.G)) {
                        GameLoop.CommandManager.PickupItem(GameLoop.World.Player);
                    }

                    if (selectedMenu == "Inventory") {
                        Point mousePos = GameHost.Instance.Mouse.ScreenPosition.PixelLocationToSurface(12, 12);
                        if (mousePos.X >= 10 && mousePos.X <= 59 && mousePos.Y >= 5 && mousePos.Y <= 34) {
                            int slot = mousePos.Y - 8;
                            if (slot >= 0 && slot < GameLoop.World.Player.Inventory.Length) {
                                int x = mousePos.X - 10;
                                if (GameHost.Instance.Mouse.LeftClicked) {
                                    if (x < 35) {
                                        if (invMoveIndex == -1)
                                            invMoveIndex = slot;
                                        else {
                                            Item tempID = GameLoop.World.Player.Inventory[invMoveIndex];
                                            GameLoop.World.Player.Inventory[invMoveIndex] = GameLoop.World.Player.Inventory[slot];
                                            GameLoop.World.Player.Inventory[slot] = tempID;
                                            invMoveIndex = -1;
                                        }
                                    } else if (x > 35 && x < 43) {
                                        Item item = GameLoop.World.Player.Inventory[slot];
                                        if (GameLoop.World.Player.Inventory[slot].EquipSlot != -1) {
                                            GameLoop.CommandManager.EquipItem(GameLoop.World.Player, slot, GameLoop.World.Player.Inventory[slot]);
                                        } else if (item.ItemCategory == 11) {
                                            string[] itemResult = GameLoop.CommandManager.UseItem(GameLoop.World.Player, item).Split("|"); ;

                                            if (itemResult[0] != "f") {
                                                if (item.IsStackable && item.ItemQuantity > 1) {
                                                    item.ItemQuantity -= 1;
                                                } else {
                                                    GameLoop.World.Player.Inventory[slot] = new Item(0);
                                                } 
                                                MessageLog.Add(new ColoredString("Used the " + item.Name + ".", Color.AliceBlue, Color.Black));
                                                MessageLog.Add(new ColoredString(itemResult[1], Color.AliceBlue, Color.Black)); 
                                            } else {
                                                MessageLog.Add(new ColoredString("Tried to use the " + item.Name + ".", Color.AliceBlue, Color.Black));
                                                MessageLog.Add(new ColoredString(itemResult[1], Color.AliceBlue, Color.Black)); 
                                            }
                                        }
                                    } else if (x > 43) {
                                        if (slot < GameLoop.World.Player.Inventory.Length && slot >= 0) {
                                            GameLoop.CommandManager.DropItem(GameLoop.World.Player, slot);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (GameHost.Instance.Keyboard.IsKeyReleased(Key.F9)) {
                    GameLoop.World.SaveMapToFile(GameLoop.World.maps[GameLoop.World.Player.MapPos], GameLoop.World.Player.MapPos);
                }

                if (GameHost.Instance.Keyboard.IsKeyReleased(Key.F5)) {
                    LimitedVision = !LimitedVision;

                    if (!LimitedVision) {
                        for (int i = 0; i < GameLoop.World.maps[GameLoop.World.Player.MapPos].Tiles.Length; i++) {
                            GameLoop.World.maps[GameLoop.World.Player.MapPos].Tiles[i].Unshade();
                            GameLoop.World.maps[GameLoop.World.Player.MapPos].Tiles[i].IsVisible = true;
                        }
                    } else {
                        UpdateVision();
                    }
                }

                if (GameHost.Instance.Keyboard.IsKeyReleased(Key.F8)) {
                    GameLoop.World.Player.MetNPCs["Cobalt"] = 100;
                }
                if (GameHost.Instance.Keyboard.IsKeyReleased(Key.F7)) {
                    GameLoop.World.Player.MetNPCs["Cobalt"] = -100;
                }



                if (GameHost.Instance.Keyboard.IsKeyReleased(Key.F3)) {
                    GameLoop.World.SavePlayer(); 
                }

                if (GameHost.Instance.Keyboard.IsKeyReleased(Key.F2)) {
                    MessageLog.Add(GameLoop.World.Player.MapPos.ToString() + " | " + GameLoop.World.Player.Position.ToString());
                }
            } else if (selectedMenu == "Sign") {
                if (GameHost.Instance.Keyboard.HasKeysPressed) {
                    selectedMenu = "None";
                    SignWindow.IsVisible = false;
                    MapConsole.IsFocused = true;
                }
            }

            if (GameHost.Instance.Keyboard.IsKeyReleased(Key.F)) {
                flying = !flying;
            }

            if (selectedMenu != "Map Editor" && selectedMenu != "Targeting") {
                
                if (GameHost.Instance.Keyboard.IsKeyReleased(Key.C)) {
                    selectedMenu = "Targeting";
                    targetType = "Door";
                    MessageLog.Add("Close door where?");
                }

                Point mousePos = new MouseScreenObjectState(SidebarConsole, GameHost.Instance.Mouse).CellPosition - new Point(0, 33);
                if (mousePos.X > 0) { // Clicked in Sidebar
                    if (GameHost.Instance.Mouse.LeftClicked) {
                        int slot = mousePos.Y;
                        if (slot >= 0 && slot <= 15)
                            GameLoop.CommandManager.UnequipItem(GameLoop.World.Player, slot); 
                    }
                }
            } else if (selectedMenu == "Targeting") { 
                if (GameHost.Instance.Keyboard.IsKeyReleased(Key.S)) { targetDir = new Point(0, 1); } 
                if (GameHost.Instance.Keyboard.IsKeyReleased(Key.A)) { targetDir = new Point(-1, 0); } 
                if (GameHost.Instance.Keyboard.IsKeyReleased(Key.D)) { targetDir = new Point(1, 0); } 
                if (GameHost.Instance.Keyboard.IsKeyReleased(Key.W)) { targetDir = new Point(0, -1); } 

                if (targetType == "Door" && targetDir != new Point(0, 0)) { 
                    GameLoop.World.maps[GameLoop.World.Player.MapPos].ToggleDoor(GameLoop.World.Player.Position + targetDir);
                    UpdateVision();
                    targetType = "Done";
                } else if (targetType != "Door") {
                    targetType = "None";
                    targetDir = new Point(0, 0);
                    selectedMenu = "None";
                }
            } else if (selectedMenu == "Map Editor") {
                Map mapData = GameLoop.World.maps[GameLoop.World.Player.MapPos];
                MinimapTile thisMap = mapData.MinimapTile;
                

                foreach (var key in GameHost.Instance.Keyboard.KeysReleased) {
                    if (key.Character >= 'A' && key.Character <= 'z') {
                        thisMap.name += key.Character;
                    }
                }
                if (GameHost.Instance.Keyboard.IsKeyReleased(Key.Back)) {
                    if (thisMap.name.Length > 0) { thisMap.name = thisMap.name.Substring(0, thisMap.name.Length - 1); }
                } else if (GameHost.Instance.Keyboard.IsKeyReleased(Key.Space)) {
                    thisMap.name += " ";
                }



                if (!GameHost.Instance.Keyboard.IsKeyDown(Key.LeftShift)) {
                    if (GameHost.Instance.Mouse.ScrollWheelValueChange < 0) {
                        if (tileIndex < GameLoop.World.tileLibrary.Count)
                            tileIndex++;
                        else
                            tileIndex = 0;
                    } else if (GameHost.Instance.Mouse.ScrollWheelValueChange > 0) {
                        if (tileIndex > 0)
                            tileIndex--;
                        else
                            tileIndex = GameLoop.World.tileLibrary.Count - 1;
                    }
                } else {
                    if (GameHost.Instance.Mouse.ScrollWheelValueChange < 0) {
                        thisMap.ch++;
                    } else if (GameHost.Instance.Mouse.ScrollWheelValueChange > 0) {
                        if (thisMap.ch > 0)
                            thisMap.ch--;
                    }
                }

                Point mousePos = GameHost.Instance.Mouse.ScreenPosition.PixelLocationToSurface(12, 12) - new Point(1, 1);
                if (mousePos.X < 72 && mousePos.Y < 41 && mousePos.X >= 0 && mousePos.Y >= 0) {
                    if (GameHost.Instance.Mouse.LeftButtonDown) { 
                        if (GameLoop.World.tileLibrary.ContainsKey(tileIndex)) {
                            TileBase tile = new TileBase(tileIndex);
                            tile.TileID = tileIndex;
                            if (mousePos.ToIndex(GameLoop.MapWidth) < GameLoop.World.maps[GameLoop.World.Player.MapPos].Tiles.Length) 
                                GameLoop.World.maps[GameLoop.World.Player.MapPos].Tiles[mousePos.ToIndex(GameLoop.MapWidth)] = tile; 
                        } else {
                            MessageLog.Add("No tile found");
                        } 
                    }
                } else {
                    mousePos -= new Point(72, 0);
                    if (GameHost.Instance.Mouse.LeftButtonDown) {  
                        if (mousePos == new Point(8, 14)) { thisMap.fg = new Color(thisMap.fg.R - 1, thisMap.fg.G, thisMap.fg.B); } 
                        if (mousePos == new Point(14, 14)) { thisMap.fg = new Color(thisMap.fg.R + 1, thisMap.fg.G, thisMap.fg.B); }
                        if (mousePos == new Point(8, 15)) { thisMap.fg = new Color(thisMap.fg.R, thisMap.fg.G - 1, thisMap.fg.B); }
                        if (mousePos == new Point(14, 15)) { thisMap.fg = new Color(thisMap.fg.R, thisMap.fg.G + 1, thisMap.fg.B); }
                        if (mousePos == new Point(8, 16)) { thisMap.fg = new Color(thisMap.fg.R, thisMap.fg.G, thisMap.fg.B - 1); }
                        if (mousePos == new Point(14, 16)) { thisMap.fg = new Color(thisMap.fg.R, thisMap.fg.G, thisMap.fg.B + 1); }
                    }

                    if (GameHost.Instance.Mouse.RightClicked) {
                        if (mousePos == new Point(8, 14)) { thisMap.fg = new Color(thisMap.fg.R - 1, thisMap.fg.G, thisMap.fg.B); }
                        if (mousePos == new Point(14, 14)) { thisMap.fg = new Color(thisMap.fg.R + 1, thisMap.fg.G, thisMap.fg.B); }
                        if (mousePos == new Point(8, 15)) { thisMap.fg = new Color(thisMap.fg.R, thisMap.fg.G - 1, thisMap.fg.B); }
                        if (mousePos == new Point(14, 15)) { thisMap.fg = new Color(thisMap.fg.R, thisMap.fg.G + 1, thisMap.fg.B); }
                        if (mousePos == new Point(8, 16)) { thisMap.fg = new Color(thisMap.fg.R, thisMap.fg.G, thisMap.fg.B - 1); }
                        if (mousePos == new Point(14, 16)) { thisMap.fg = new Color(thisMap.fg.R, thisMap.fg.G, thisMap.fg.B + 1); }

                        if (mousePos.Y == 18) { mapData.PlayerCanBuild = !mapData.PlayerCanBuild; }
                        if (mousePos.Y == 19) { mapData.AmbientMonsters = !mapData.AmbientMonsters; }
                    }
                }
            }

            if (GameHost.Instance.Keyboard.IsKeyReleased(Key.F1)) {
                if (selectedMenu == "Map Editor") {
                    selectedMenu = "None";
                } else {
                    selectedMenu = "Map Editor";
                }
            }
        }

        public void LoadMap(Point3D pos, bool firstLoad) {
            if (!GameLoop.World.maps.ContainsKey(pos)) { GameLoop.World.CreateMap(pos); }
            Map map = GameLoop.World.maps[pos];

            if (firstLoad) {
                MapConsole = new SadConsole.Console(GameLoop.MapWidth, GameLoop.MapHeight, map.Tiles);
                CreateMapWindow(72, 42, map.MinimapTile.name);
                
            } else { 
                for (int i = 0; i < map.Tiles.Length; i++) {
                    map.Tiles[i].Unshade();
                    map.Tiles[i].IsVisible = false;

                    int depth = 0;
                    TileBase tile = new TileBase(GameLoop.World.maps[GameLoop.World.Player.MapPos + new Point3D(0, 0, depth)].Tiles[i].TileID);

                    if (map.Tiles[i].Name == "Space") {
                        while (tile.Name == "Space" && GameLoop.World.maps.ContainsKey(GameLoop.World.Player.MapPos + new Point3D(0, 0, depth - 1))) {
                            depth--;
                            tile = new TileBase(GameLoop.World.maps[GameLoop.World.Player.MapPos + new Point3D(0, 0, depth)].Tiles[i].TileID);
                        }

                        float mult = Math.Max(0.0f, 1.0f + (depth * 0.2f));

                        Color shaded = tile.Foreground * mult;

                        map.Tiles[i].ForegroundR = shaded.R;
                        map.Tiles[i].ForegroundG = shaded.G;
                        map.Tiles[i].ForegroundB = shaded.B;
                        map.Tiles[i].TileGlyph = tile.TileGlyph;
                        map.Tiles[i].UpdateAppearance(); 
                    }
                }

                MapConsole.Surface = new CellSurface(GameLoop.MapWidth, GameLoop.MapHeight, map.Tiles);
                MapWindow.Title = GameLoop.World.maps[GameLoop.World.Player.MapPos].MinimapTile.name.Align(HorizontalAlignment.Center, GameLoop.MapWidth - 2, (char)196); 
            }
            
           
            SyncMapEntities(map);
        }

        public void SyncMapEntities(Map map) {
            if (GameLoop.World != null) {
                var entities = EntityRenderer.Entities.ToList(); // Duplicate the list
                foreach (var ent in entities)
                    EntityRenderer.Remove(ent);
                 
                EntityRenderer.Add(GameLoop.World.Player); 

                foreach (Entity entity in map.Entities.Items) {
                    EntityRenderer.Add(entity);
                }

                for (int i = 0; i < GameLoop.World.npcLibrary.Count; i++) {
                    NPC temp = GameLoop.World.npcLibrary[i];
                    if (temp.MapPos == GameLoop.World.Player.MapPos || (temp.MapPos.X == GameLoop.World.Player.MapPos.X && temp.MapPos.Y == GameLoop.World.Player.MapPos.Y && temp.MapPos.Z < GameLoop.World.Player.MapPos.Z)) {
                        EntityRenderer.Add(temp);
                    }
                }

                FOV = new GoRogue.FOV(GameLoop.World.maps[GameLoop.World.Player.MapPos].MapFOV);

                UpdateVision();
            }
        }

        private void RenderOverlays() {
            for (int i = 0; i < GameLoop.World.maps[GameLoop.World.Player.MapPos].Tiles.Length; i++) {
                if (GameLoop.World.maps[GameLoop.World.Player.MapPos].Tiles[i].Dec != null) { 
                    Decorator dec = GameLoop.World.maps[GameLoop.World.Player.MapPos].Tiles[i].Dec;
                    MapConsole.AddDecorator(i, 1, new CellDecorator(new Color(dec.R, dec.G, dec.B, dec.A), dec.Glyph, Mirror.None)); 
                } else {
                    MapConsole.ClearDecorators(i, 1);
                }
            } 
        }

        private void RenderSign() {
            SignConsole.Clear();

            string[] signLines = signText.Split('|');

            int mid = SignConsole.Height / 2;
            int startY = mid - (signLines.Length / 2) - 1;

            for (int i = 0; i < signLines.Length; i++) {
                SignConsole.Print(0, startY + i, signLines[i].Align(HorizontalAlignment.Center, SignConsole.Width));
            }

        }

        private void RenderInventory() {
            Point mousePos = new MouseScreenObjectState(InventoryConsole, GameHost.Instance.Mouse).CellPosition;

            InventoryConsole.Clear();
            InventoryConsole.Print((InventoryConsole.Width / 2) - 4, 0, "BACKPACK");

            for (int i = 0; i < 27; i++) {
                if (i < GameLoop.World.Player.Inventory.Length) {
                    Item item = GameLoop.World.Player.Inventory[i];

                    string nameWithDurability = item.Name;

                    if (item.Durability >= 0)
                        nameWithDurability = "[" + item.Durability + "] " + item.Name;

                    InventoryConsole.Print(0, i + 1, item.AsColoredGlyph());
                    if (!item.IsStackable || (item.IsStackable && item.ItemQuantity == 1))
                        InventoryConsole.Print(2, i + 1, new ColoredString(nameWithDurability, invMoveIndex == i ? Color.Yellow : item.Name == "(EMPTY)" ? Color.DarkSlateGray : Color.White, Color.Black));
                    else
                        InventoryConsole.Print(2, i + 1, new ColoredString(("(" + item.ItemQuantity + ") " + item.Name), invMoveIndex == i ? Color.Yellow : item.Name == "(EMPTY)" ? Color.DarkSlateGray : Color.White, Color.Black));
                    
                    ColoredString Options = new ColoredString("MOVE", (mousePos.Y == i + 1 && mousePos.X < 33) ? Color.Yellow : item.Name == "(EMPTY)" ? Color.DarkSlateGray : Color.White, Color.Black);

                    Options += new ColoredString(" | ", Color.White, Color.Black);

                    if (item.EquipSlot != -1) {
                        Options += new ColoredString("EQUIP", (mousePos.Y == i + 1 && mousePos.X > 33 && mousePos.X < 41) ? Color.Yellow : item.Name == "(EMPTY)" ? Color.DarkSlateGray : Color.White, Color.Black);
                    } else if (item.ItemCategory == 11) {
                        Options += new ColoredString(" USE ", (mousePos.Y == i + 1 && mousePos.X > 33 && mousePos.X < 41) ? Color.Yellow : item.Name == "(EMPTY)" ? Color.DarkSlateGray : Color.White, Color.Black);
                    } else {
                        Options += new ColoredString("     ", item.Name == "(EMPTY)" ? Color.DarkSlateGray : Color.White, Color.Black);
                    }

                    Options += new ColoredString(" | ", Color.White, Color.Black);
                    Options += new ColoredString("DROP", (mousePos.Y == i + 1 && mousePos.X > 41) ? Color.Yellow : item.Name == "(EMPTY)" ? Color.DarkSlateGray : Color.White, Color.Black);

                    InventoryConsole.Print(28, i + 1, Options);
                } else {
                    InventoryConsole.Print(2, i + 1, new ColoredString("[LOCKED]", Color.DarkSlateGray, Color.Black));
                }
            } 

        } 

        private void RenderBattle() { 
            BattleConsole.Clear();
            BattleConsole.IsFocused = true;
            Point mousePos = new MouseScreenObjectState(BattleConsole, GameHost.Instance.Mouse).CellPosition;

            if ((selectedMenu == "Battle" || selectedMenu == "TurnWait")) {
                string enemyTitle = GameLoop.BattleManager.Enemy.Name + " (CR ";

                if (GameLoop.BattleManager.Enemy.CR < 0) {
                    if (GameLoop.BattleManager.Enemy.CR == -2) { enemyTitle += (char)25; }
                    if (GameLoop.BattleManager.Enemy.CR == -3) { enemyTitle += (char)26; }
                    if (GameLoop.BattleManager.Enemy.CR == -4) { enemyTitle += (char)27; }
                    if (GameLoop.BattleManager.Enemy.CR == -6) { enemyTitle += (char)28; }
                    if (GameLoop.BattleManager.Enemy.CR == -8) { enemyTitle += (char)29; }
                } else {
                    enemyTitle += GameLoop.BattleManager.Enemy.CR.ToString();
                }

                enemyTitle += ")";

                BattleConsole.Print(BattleConsole.Width - 31, 0, enemyTitle.Align(HorizontalAlignment.Right, 30));
                BattleConsole.Print(BattleConsole.Width - 1, 1, new ColoredString(((char)3).ToString(), Color.Red, Color.Black));
                BattleConsole.Print(BattleConsole.Width - 11, 1, (GameLoop.BattleManager.Enemy.CurrentHP + "/" + GameLoop.BattleManager.Enemy.MaxHP).Align(HorizontalAlignment.Right, 11), Color.Red, Color.Black);

                BattleConsole.Print(0, 0, GameLoop.World.Player.Name + " (Lv. " + GameLoop.World.Player.Level + ")");
                BattleConsole.Print(0, 1, new ColoredString(((char)3).ToString(), Color.Red, Color.Black));

                string WeaponDura = "Weapon Durability: ";
                if (GameLoop.World.Player.Equipment[0].ItemID != 0 && GameLoop.World.Player.Equipment[0].Durability >= 0) {
                    if (GameLoop.World.Player.Equipment[0].Durability > 0) {
                        WeaponDura += GameLoop.World.Player.Equipment[0].Durability;
                    } else {
                        WeaponDura += "(Broken)";
                    }
                } else {
                    WeaponDura += "(Unarmed)";
                }

                BattleConsole.Print(0, 2, new ColoredString(WeaponDura, Color.White, Color.Black));

                string ArmorDura = "Armor Durability: ";
                if (GameLoop.World.Player.Equipment[9].ItemID != 0 && GameLoop.World.Player.Equipment[9].Durability >= 0) {
                    if (GameLoop.World.Player.Equipment[9].Durability > 0) {
                        ArmorDura += GameLoop.World.Player.Equipment[9].Durability;
                    } else {
                        ArmorDura += "(Broken)";
                    }
                } else {
                    ArmorDura += "(Unarmored)";
                }

                BattleConsole.Print(0, 3, new ColoredString(ArmorDura, Color.White, Color.Black));
                BattleConsole.Print(1, 1, (GameLoop.World.Player.CurrentHP + "/" + GameLoop.World.Player.MaxHP), Color.Red, Color.Black); 


                BattleConsole.DrawLine(new Point(0, 30), new Point(BattleConsole.Width - 1, 30), (char)196, new Color(0, 127, 0), Color.Black);
                BattleConsole.Print(10, 29, GameLoop.World.Player.Appearance);
                BattleConsole.Print(BattleConsole.Width - 11, 29, GameLoop.BattleManager.Enemy.Appearance);

                BattleConsole.DrawLine(new Point(0, 31), new Point(BattleConsole.Width - 1, 31), (char)196, Color.Orange, Color.Black);
                BattleConsole.Print(1, 32, new ColoredString("ATTACK".Align(HorizontalAlignment.Center, 16), mousePos.Y == 32 ? Color.Yellow : Color.White, Color.Black));
                BattleConsole.Print(1, 33, new ColoredString("CHANGE".Align(HorizontalAlignment.Center, 16), mousePos.Y == 33 ? Color.Yellow : Color.White, Color.Black));
                BattleConsole.Print(1, 34, new ColoredString("ITEM".Align(HorizontalAlignment.Center, 16), mousePos.Y == 34 ? Color.Yellow : Color.White, Color.Black));
                //BattleConsole.Print(BattleConsole.Width - 7, BattleConsole.Height - 4, (GameLoop.BattleManager.fleePercent + "%").Align(HorizontalAlignment.Right, 5));
                BattleConsole.Print(1, BattleConsole.Height - 1, new ColoredString("FLEE".Align(HorizontalAlignment.Center, 16), mousePos.Y == BattleConsole.Height-1 ? Color.Yellow : Color.White, Color.Black));


                BattleConsole.DrawLine(new Point(18, 31), new Point(18, BattleConsole.Height - 1), (char)179, Color.Orange, Color.Black);

                if (selectedMenu == "TurnWait")
                    BattleLog.Print(0, 4, "[CLICK ANYWHERE TO CONTINUE]".Align(HorizontalAlignment.Center, BattleLog.Width));
            } else if (selectedMenu == "BattleDone" && battleResult == "Victory") {
                BattleConsole.Print(0, 8, "Victory!".Align(HorizontalAlignment.Center, BattleConsole.Width - 2));
                BattleConsole.Print(0, 10, ("You got " + GameLoop.BattleManager.Enemy.ExpGranted + " exp.").Align(HorizontalAlignment.Center, BattleConsole.Width - 2));
                BattleConsole.Print(0, 25, "[Close]".Align(HorizontalAlignment.Center, BattleConsole.Width - 2));
            } else if (selectedMenu == "BattleDone" && battleResult == "Level") {
                BattleConsole.Print(0, 8, "Victory!".Align(HorizontalAlignment.Center, BattleConsole.Width - 2));
                BattleConsole.Print(0, 10, ("You got " + GameLoop.BattleManager.Enemy.ExpGranted + " exp and leveled up!").Align(HorizontalAlignment.Center, BattleConsole.Width - 2));
                BattleConsole.Print(0, 12, "Pick a class to advance a level in:".Align(HorizontalAlignment.Center, BattleConsole.Width - 2));

                for (int i = 0; i < GameLoop.World.Player.ClassLevels.Count; i++) {
                    BattleConsole.Print(0, 14 + (i * 2), new ColoredString((GameLoop.World.Player.ClassLevels[i].Name + " [" + GameLoop.World.Player.ClassLevels[i].ClassLevels + "]").Align(HorizontalAlignment.Center, BattleConsole.Width - 2), mousePos.Y == 14 + (i * 2) ? Color.Yellow : Color.White, Color.Black));
                }
            } else if (selectedMenu == "BattleDone" && battleResult == "LevelDone") {
                BattleConsole.Print(0, 8, "Victory!".Align(HorizontalAlignment.Center, BattleConsole.Width - 2));
                BattleConsole.Print(0, 10, ("You got " + GameLoop.BattleManager.Enemy.ExpGranted + " exp and levelled up!").Align(HorizontalAlignment.Center, BattleConsole.Width - 2));
                 
                BattleConsole.Print(0, 25, new ColoredString("[Close]".Align(HorizontalAlignment.Center, BattleConsole.Width - 2), mousePos.Y == 25 ? Color.Yellow : Color.White, Color.Black));
            } else if (selectedMenu == "BattleDone" && battleResult == "Drops") {
                BattleConsole.Print(0, 7, ("The " + GameLoop.BattleManager.Enemy.Name + " dropped items!").Align(HorizontalAlignment.Center, BattleConsole.Width - 2));
                BattleConsole.Print(0, 8, "(click to take)".Align(HorizontalAlignment.Center, BattleConsole.Width - 2));
                for (int i = 0; i < dropTable.Count; i++) {
                    Item item = dropTable[i];
                    if (!item.IsStackable || (item.IsStackable && item.ItemQuantity == 1))
                        BattleConsole.Print(0, 10 + (i), new ColoredString(item.Name.Align(HorizontalAlignment.Center, BattleConsole.Width - 2), mousePos.Y == 10 + i ? Color.Yellow : Color.White, Color.Black));
                    else
                        BattleConsole.Print(0, 10 + (i), new ColoredString(("(" + item.ItemQuantity + ") " + item.Name).Align(HorizontalAlignment.Center, BattleConsole.Width - 2), mousePos.Y == 10 + i ? Color.Yellow : Color.White, Color.Black));
                }

                if (dropTable.Count == 0) {
                    BattleWindow.IsVisible = false;
                    BattleWindow.IsFocused = false;
                    selectedMenu = "None";
                    battleResult = "None";
                    dropTable.Clear();
                }

                BattleConsole.Print(0, 30, new ColoredString("[DONE]".Align(HorizontalAlignment.Center, BattleConsole.Width - 2), mousePos.Y == 30 ? Color.Yellow : Color.White, Color.Black));
            } else if (selectedMenu == "BattleDone" && battleResult == "Fled") {
                BattleConsole.Print(0, 10, "You escaped!".Align(HorizontalAlignment.Center, BattleConsole.Width - 2));
                BattleConsole.Print(0, 25, new ColoredString("[Close]".Align(HorizontalAlignment.Center, BattleConsole.Width - 2), mousePos.Y == 25 ? Color.Yellow : Color.White, Color.Black));
            } else if (selectedMenu == "BattleDone" && battleResult == "Died") {
                BattleConsole.Print(0, 10, "You died!".Align(HorizontalAlignment.Center, BattleConsole.Width - 2));
                BattleConsole.Print(0, 25, new ColoredString("[Respawn]".Align(HorizontalAlignment.Center, BattleConsole.Width - 2), mousePos.Y == 25 ? Color.Yellow : Color.White, Color.Black));
            }
        }

        
        private void RenderBattleInv() {
            Point mousePos = new MouseScreenObjectState(InvConsole, GameHost.Instance.Mouse).CellPosition;
            InvConsole.Clear();
            InvConsole.Print(0, 0, "        NAME        | QTY |" + ("DESC").Align(HorizontalAlignment.Center, 36));
            InvConsole.DrawLine(new Point(0, 1), new Point(InvConsole.Width - 1, 1), (char)196, Color.Orange, Color.Black);

            for (int i = 0; i < GameLoop.World.Player.Inventory.Length; i++) {
                Item item = GameLoop.World.Player.Inventory[i];
                string nameWithDurability = item.Name;

                if (item.Durability >= 0)
                    nameWithDurability = "[" + item.Durability + "] " + item.Name;

                string name = nameWithDurability.Align(HorizontalAlignment.Left, 20);
                string qty = item.ItemQuantity.ToString().Align(HorizontalAlignment.Center, 5);
                string desc = item.Description.Align(HorizontalAlignment.Center, 36);

                string full = name + "|" + qty + "|" + desc;

                InvConsole.Print(0, i + 2, new ColoredString(full, mousePos.Y == i + 2 ? Color.Yellow : item.ItemCategory == 11 ? Color.White : item.EquipSlot != -1 ? Color.White : Color.DarkSlateGray, Color.Black));
            }

            InvConsole.Print(0, InvConsole.Height - 1, new ColoredString("[CLOSE]".Align(HorizontalAlignment.Center, InvConsole.Width), mousePos.Y == InvConsole.Height - 1 ? Color.Yellow : Color.White, Color.Black));
        }

        private void CaptureInvClicks() {
            Point mousePos = new MouseScreenObjectState(InvConsole, GameHost.Instance.Mouse).CellPosition - new Point(0, 2);

            if (GameHost.Instance.Mouse.LeftClicked) {
                if (mousePos.Y == InvConsole.Height - 3) {
                    InvWindow.IsVisible = false;
                } else { 
                    if (mousePos.Y >= 0 && mousePos.Y < GameLoop.World.Player.Inventory.Length) {
                        Item item = GameLoop.World.Player.Inventory[mousePos.Y];

                        if (!AlreadyUsedItem) {
                            if (item.ItemCategory == 11) {
                                string[] itemResult = GameLoop.CommandManager.UseItem(GameLoop.World.Player, item).Split("|");

                                if (itemResult[0] != "f") {
                                    if (item.IsStackable && item.ItemQuantity > 1) {
                                        item.ItemQuantity -= 1;
                                    } else {
                                        GameLoop.World.Player.Inventory[mousePos.Y] = new Item(0);
                                    }

                                    AlreadyUsedItem = true;
                                    selectedMenu = "TurnWait";
                                    BattleLog.Clear();
                                    BattleLog.Print(0, 1, ("Used the " + item.Name + ".").Align(HorizontalAlignment.Center, BattleLog.Width));
                                    BattleLog.Print(0, 2, itemResult[1].Align(HorizontalAlignment.Center, BattleLog.Width));
                                    InvWindow.IsVisible = false;
                                } else { 
                                    selectedMenu = "TurnWait";
                                    BattleLog.Clear();
                                    BattleLog.Print(0, 1, ("Tried to use the " + item.Name + ".").Align(HorizontalAlignment.Center, BattleLog.Width));
                                    BattleLog.Print(0, 2, itemResult[1].Align(HorizontalAlignment.Center, BattleLog.Width));
                                    InvWindow.IsVisible = false;
                                }
                            } else if (item.EquipSlot != -1) {
                                GameLoop.CommandManager.EquipItem(GameLoop.World.Player, mousePos.Y, item);
                                AlreadyUsedItem = true;
                                selectedMenu = "TurnWait";
                                BattleLog.Clear();
                                BattleLog.Print(0, 1, ("Equipped the " + item.Name + ".").Align(HorizontalAlignment.Center, BattleLog.Width));
                                InvWindow.IsVisible = false;
                            } else {
                                selectedMenu = "TurnWait";
                                BattleLog.Clear();
                                BattleLog.Print(0, 1, "That item can't be used in combat.".Align(HorizontalAlignment.Center, BattleLog.Width));
                                InvWindow.IsVisible = false;
                            }
                        } else {
                            selectedMenu = "TurnWait";
                            BattleLog.Clear();
                            BattleLog.Print(0, 1, "You already used an item this turn!".Align(HorizontalAlignment.Center, BattleLog.Width));
                            InvWindow.IsVisible = false;
                        }
                    }
                } 
            }
        }

        
        private void CaptureBattleClicks() {
            if (selectedMenu == "Battle") {
                Point mousePos = new MouseScreenObjectState(BattleConsole, GameHost.Instance.Mouse).CellPosition; 
                if (GameHost.Instance.Mouse.LeftClicked) {
                    if (mousePos.Y == 39 && mousePos.X >= 0 && mousePos.X <= 18) {
                        GameLoop.BattleManager.ResolveTurn("Flee", true, PlayerFirst);
                    }

                    if (mousePos.Y == 32) {
                        GameLoop.BattleManager.ResolveTurn("Attack", true, PlayerFirst);
                    }

                    if (mousePos.Y == 34) {
                        InvWindow.IsVisible = true;
                    } 
                } 
            } else if (selectedMenu == "TurnWait") {
                if (GameHost.Instance.Mouse.LeftClicked) {
                    if (!battleDone) {
                        selectedMenu = "Battle";
                        BattleLog.Clear();
                    } else {
                        if (battleResult != "Fled") {
                            GameLoop.BattleManager.BattleState = "None";
                            selectedMenu = "BattleDone";
                            BattleLog.IsVisible = false;
                        } else {
                            BattleWindow.IsVisible = false;
                            BattleWindow.IsFocused = false;
                            selectedMenu = "None";
                            battleResult = "None";
                            dropTable.Clear();
                        }
                    }
                } 
            } else {
                Point mousePos = new MouseScreenObjectState(BattleConsole, GameHost.Instance.Mouse).CellPosition;
                if (GameHost.Instance.Mouse.LeftClicked) {
                    if (battleResult == "Level") {
                        // DO ALL THE LEVEL-UP SELECTIONS

                        if (mousePos.Y >= 14) {
                            int slot = (mousePos.Y / 2) - 7;
                            if (GameLoop.World.Player.ClassLevels.Count > slot && slot >= 0) {
                                GameLoop.World.Player.ApplyLevel(slot);
                                battleResult = "LevelDone";
                            }
                        } 

                    } else if (battleResult == "LevelDone") {
                        if (mousePos.Y == 25) { 
                            // APPLY THE LEVEL-UP

                            int oldMaxHP = GameLoop.World.Player.MaxHP;  
                            GameLoop.World.Player.CurrentHP += GameLoop.World.Player.MaxHP - oldMaxHP;



                            GameLoop.BattleManager.RollDrops(); 
                            battleResult = "Drops";
                        }
                    } else if (battleResult == "Drops") { 
                        if (mousePos.Y >= 10 && mousePos.Y <= 28) {
                            int lootNum = mousePos.Y - 10;

                            if (dropTable.Count > lootNum) {
                                GameLoop.CommandManager.AddItemToInv(GameLoop.World.Player, dropTable[lootNum]); 
                            }
                        } 

                        if (mousePos.Y == 30) {
                            BattleWindow.IsVisible = false;
                            BattleWindow.IsFocused = false;
                            selectedMenu = "None";
                            battleResult = "None";
                            dropTable.Clear();
                        }
                    } else if (battleResult != "Fled") {
                        if (mousePos.Y == 25) {
                            GameLoop.BattleManager.RollDrops();
                            battleResult = "Drops";
                        }
                    } else {
                        if (mousePos.Y == 25) {
                            BattleWindow.IsVisible = false;
                            BattleWindow.IsFocused = false;
                            selectedMenu = "None";
                            battleResult = "None";
                            dropTable.Clear();
                        }
                    }
                }
            } 
        }


        private void RenderSidebar() {
            Point mousePos = new MouseScreenObjectState(SidebarConsole, GameHost.Instance.Mouse).CellPosition;
            SidebarConsole.Clear();

            string timeHour = GameLoop.World.Player.Hours.ToString();
            if (timeHour.Length == 1)
                timeHour = "0" + timeHour;

            string timeMinute = GameLoop.World.Player.Minutes.ToString();
            if (timeMinute.Length == 1)
                timeMinute = "0" + timeMinute;

            string time = timeHour + ":" + timeMinute;


            string[] months = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };


            SidebarConsole.Print(0, 0, "STR: " + GameLoop.World.Player.STR);
            SidebarConsole.Print(0, 1, "DEX: " + GameLoop.World.Player.DEX);
            SidebarConsole.Print(0, 2, "CON: " + GameLoop.World.Player.CON);
            SidebarConsole.Print(0, 3, "INT: " + GameLoop.World.Player.INT);
            SidebarConsole.Print(0, 4, "WIS: " + GameLoop.World.Player.WIS);
            SidebarConsole.Print(0, 5, "CHA: " + GameLoop.World.Player.CHA);


            SidebarConsole.DrawLine(new Point(7, 0), new Point(7, 9), (char) 179, Color.Orange, Color.Black);

            // The time and date area (top-left)
            SidebarConsole.Print(8, 0, time);
            SidebarConsole.Print(14, 0, GameLoop.World.Player.AM ? "AM" : "PM");
            SidebarConsole.Print(8, 2, (months[GameLoop.World.Player.Month - 1] + " " + GameLoop.World.Player.Day).Align(HorizontalAlignment.Right, 8));
            SidebarConsole.Print(9, 3, ("Year " + GameLoop.World.Player.Year).Align(HorizontalAlignment.Right, 7));

            // HP
            SidebarConsole.Print(8, 5, new ColoredString(((char) 3).ToString(), Color.Red, Color.Black)); 
            SidebarConsole.Print(9, 5, new ColoredString((GameLoop.World.Player.CurrentHP + "/" + GameLoop.World.Player.MaxHP).Align(HorizontalAlignment.Right, 7), Color.Red, Color.Black));

            if (GameLoop.World.Player.Race != null)
                SidebarConsole.Print(8, 7, GameLoop.World.Player.Race.Name);
            if (GameLoop.World.Player.ClassLevels.Count > 0)
                SidebarConsole.Print(8, 8, GameLoop.World.Player.ClassLevels[0].Name);
           

            SidebarConsole.DrawLine(new Point(16, 0), new Point(16, 9), (char) 179, Color.Orange, Color.Black);
            // The minimap area (top-right)

            for (int x = GameLoop.World.Player.MapPos.X - 4; x < GameLoop.World.Player.MapPos.X + 5; x++) {
                for (int y = GameLoop.World.Player.MapPos.Y - 4; y < GameLoop.World.Player.MapPos.Y + 5; y++) {
                    if (minimap.ContainsKey(new Point3D(x, y, GameLoop.World.Player.MapPos.Z))) {
                        Point3D modifiedPos = new Point3D(x, y, 0) - GameLoop.World.Player.MapPos;
                        SidebarConsole.Print(modifiedPos.X + 21, modifiedPos.Y + 4, minimap[new Point3D(x, y, GameLoop.World.Player.MapPos.Z)].AsColoredGlyph());
                    }
                }
            }

            SidebarConsole.Print(21, 4, "@", Color.White);
            SidebarConsole.DrawLine(new Point(0, 9), new Point(25, 9), (char) 196, Color.Orange, Color.Black);


            if (selectedMenu == "Map Editor") { 
                MinimapTile thisMap = GameLoop.World.maps[GameLoop.World.Player.MapPos].MinimapTile;
                SidebarConsole.Print(0, 10, "Map Name: " + thisMap.name);
                SidebarConsole.Print(0, 11, "Map Icon: ");
                SidebarConsole.Print(10, 11, thisMap.AsColoredGlyph());
                SidebarConsole.Print(0, 13, "Position: " + GameLoop.World.Player.MapPos.ToString());

                SidebarConsole.Print(0, 14, "Map fR: - " + thisMap.fg.R);
                SidebarConsole.Print(14, 14, "+");
                SidebarConsole.Print(0, 15, "Map fG: - " + thisMap.fg.G);
                SidebarConsole.Print(14, 15, "+");
                SidebarConsole.Print(0, 16, "Map fB: - " + thisMap.fg.B);
                SidebarConsole.Print(14, 16, "+");

                SidebarConsole.Print(0, 17, "Buildable: " + GameLoop.World.maps[GameLoop.World.Player.MapPos].PlayerCanBuild);
                SidebarConsole.Print(0, 18, "Ambient Monsters: " + GameLoop.World.maps[GameLoop.World.Player.MapPos].AmbientMonsters);



                SidebarConsole.Print(0, 26, "Tile Index: " + tileIndex);
                
                if (GameLoop.World.tileLibrary.ContainsKey(tileIndex)) {
                    TileBase tile = GameLoop.World.tileLibrary[tileIndex];

                    SidebarConsole.Print(0, 27, "Tile Name: " + tile.Name);
                    SidebarConsole.Print(0, 28, "Tile Appearance: ");
                    SidebarConsole.Print(17, 28, tile); 
                }
            } else { // Print non-map editor stuff
                if (GameLoop.World != null && GameLoop.World.DoneInitializing) {
                    
                    SidebarConsole.Print(0, 10, "Lv" + GameLoop.World.Player.Level); 
                    SidebarConsole.Print(7, 10, ("XP: " + GameLoop.World.Player.Experience).Align(HorizontalAlignment.Right, 19));
                    SidebarConsole.Print(0, 11, ("To Next: " + (GameLoop.World.Player.ExpToLevel() - GameLoop.World.Player.Experience)).Align(HorizontalAlignment.Right, 26));
                    

                    SidebarConsole.DrawLine(new Point(0, 12), new Point(25, 12), (char)196, Color.Orange, Color.Black);

                    ColoredString copperString = new ColoredString("CP:" + GameLoop.World.Player.CopperCoins, new Color(184, 115, 51), Color.Black);
                    ColoredString silverString = new ColoredString("SP:" + GameLoop.World.Player.SilverCoins, Color.Silver, Color.Black);
                    ColoredString goldString = new ColoredString("GP:" + GameLoop.World.Player.GoldCoins, Color.Yellow, Color.Black);
                    ColoredString JadeString = new ColoredString("JP:" + GameLoop.World.Player.JadeCoins, new Color(0, 168, 107), Color.Black);

                    SidebarConsole.Print(0, 13, copperString);
                    SidebarConsole.Print(0, 14, silverString);
                    SidebarConsole.Print(13, 13, goldString);
                    SidebarConsole.Print(13, 14, JadeString);

                    SidebarConsole.DrawLine(new Point(0, 15), new Point(25, 15), (char)196, Color.Orange, Color.Black);

                    int y = 16;

                    int attackBonus = GameLoop.World.Player.RollAttack(true); 
                    string bonusString = attackBonus > 0 ? "+" + attackBonus : attackBonus.ToString();

                    int damageBonus = GameLoop.World.Player.GetDamageBonus(true);
                    string damageString = damageBonus > 0 ? "+" + damageBonus : damageBonus.ToString();

                    string weaponDice = (GameLoop.World.Player.Equipment[0].Weapon != null && GameLoop.World.Player.Equipment[0].Durability > 0) ? GameLoop.World.Player.Equipment[0].Weapon.DamageDice : GameLoop.World.Player.UnarmedDice;

                    int armorClass = GameLoop.World.Player.GetAC();

                    SidebarConsole.Print(0, y++, "To-Hit: " + bonusString);
                    SidebarConsole.Print(0, y++, "To-Dam: " + damageString);
                    SidebarConsole.Print(0, y++, "Weapon: " + weaponDice);
                    SidebarConsole.Print(0, y++, "    AC: " + armorClass);




                    y++;
                    SidebarConsole.Print(0, y, "Backpack");
                    y++;

                    for (int i = 0; i < 9; i++) {
                        Item item = GameLoop.World.Player.Inventory[i];

                        string nameWithDurability = item.Name;

                        if (item.Durability >= 0)
                            nameWithDurability = "[" + item.Durability + "] " + item.Name;

                        SidebarConsole.Print(0, y, "|");
                        SidebarConsole.Print(1, y, item.AsColoredGlyph());
                        if (!item.IsStackable || (item.IsStackable && item.ItemQuantity == 1))
                            SidebarConsole.Print(3, y, new ColoredString(nameWithDurability, i == hotbarSelect ? Color.Yellow : item.Name == "(EMPTY)" ? Color.DarkSlateGray : Color.White, Color.TransparentBlack));
                        else
                            SidebarConsole.Print(3, y, new ColoredString(("(" + item.ItemQuantity + ") " + item.Name), i == hotbarSelect ? Color.Yellow : item.Name == "(EMPTY)" ? Color.DarkSlateGray : Color.White, Color.TransparentBlack));

                        y++;
                    }


                    y++;
                    SidebarConsole.Print(0, y, "Equipment");
                    y++;
                        
                    for (int i = 0; i < 16; i++) {
                        Item item =  GameLoop.World.Player.Equipment[i];

                        string nameWithDurability = item.Name;

                        if (item.Durability >= 0)
                            nameWithDurability = "[" + item.Durability + "] " + item.Name;


                        SidebarConsole.Print(0, y, "|");
                        SidebarConsole.Print(1, y, item.AsColoredGlyph());
                        SidebarConsole.Print(3, y, new ColoredString(nameWithDurability, mousePos.Y == y ? Color.Yellow : item.Name == "(EMPTY)" ? Color.DarkSlateGray : Color.White, Color.TransparentBlack));
                        y++;
                    }


                }
            }  
        }

        public void CreateConsoles() {
            MapConsole = new SadConsole.Console(GameLoop.MapWidth, GameLoop.MapHeight);
            SidebarConsole = new SadConsole.Console(28, GameLoop.GameHeight);
            BattleConsole = new SadConsole.Console(70, 40);
        }

        public void CreateMapWindow(int width, int height, string title) {
            MapWindow = new Window(width, height);
            MapWindow.CanDrag = false;

            int mapConsoleWidth = width - 2;
            int mapConsoleHeight = height - 2;
             
            MapConsole.Position = new Point(1, 1);
            MapWindow.Title = title.Align(HorizontalAlignment.Center, mapConsoleWidth, (char) 196);
            

            MapWindow.Children.Add(MapConsole);
            Children.Add(MapWindow); 

            MapConsole.SadComponents.Add(EntityRenderer);
            
            MapWindow.Show();
            MapWindow.IsVisible = false;
        }

        public void CreateDialogueWindow(int width, int height, string title) {
            DialogueWindow = new Window(width, height);
            DialogueWindow.CanDrag = false;
            DialogueWindow.Position = new Point(11, 6);

            int diaConWidth = width - 2;
            int diaConHeight = height - 2;

            DialogueConsole = new SadConsole.Console(diaConWidth, diaConHeight);
            DialogueConsole.Position = new Point(1, 1);
            DialogueWindow.Title = title.Align(HorizontalAlignment.Center, diaConWidth, (char)196);


            DialogueWindow.Children.Add(DialogueConsole);
            Children.Add(DialogueWindow); 
             
            DialogueWindow.Show();
            DialogueWindow.IsVisible = false;
        }

        public void CreateSidebarWindow(int width, int height, string title) {
            SidebarWindow = new Window(width, height);
            SidebarWindow.CanDrag = false;
            SidebarWindow.Position = new Point(72, 0);

            int sidebarConsoleWidth = width - 2;
            int sidebarConsoleHeight = height - 2;
             
            SidebarConsole.Position = new Point(1, 1);
            SidebarWindow.Title = title.Align(HorizontalAlignment.Center, sidebarConsoleWidth, (char) 196);
            

            SidebarWindow.Children.Add(SidebarConsole);
            Children.Add(SidebarWindow);

            SidebarWindow.Show();

            SidebarWindow.IsVisible = false;
        }

        public void CreateBattleWindow(int width, int height, string title) {
            BattleWindow = new Window(width, height);
            BattleWindow.CanDrag = false;
            BattleWindow.Position = new Point(11, 6);

            int battleConWidth = width - 2;
            int battleConHeight = height - 2;

            BattleConsole.Position = new Point(1, 1);
            BattleWindow.Title = title.Align(HorizontalAlignment.Center, battleConWidth, (char)196);


            BattleWindow.Children.Add(BattleConsole);
            Children.Add(BattleWindow);

            BattleLog = new SadConsole.Console(50, 8);
            BattleWindow.Children.Add(BattleLog);
            BattleLog.Position = new Point(20, 33);


            BattleWindow.Show();
            BattleWindow.IsVisible = false;


            InvWindow = new Window(67, 32);
            InvWindow.CanDrag = false;
            InvWindow.Position = new Point((BattleWindow.Width - InvWindow.Width) / 2, (BattleWindow.Height - InvWindow.Height) / 2);

            InvConsole = new SadConsole.Console(65, 30);
            InvConsole.Position = new Point(1, 1);
            InvWindow.Title = "".Align(HorizontalAlignment.Center, 20, (char)196);

            InvWindow.Children.Add(InvConsole);
            BattleWindow.Children.Add(InvWindow);
            InvWindow.Show();
            InvWindow.IsVisible = false;
        }

        public void CreateInventoryWindow(int width, int height, string title) {
            InventoryWindow = new Window(width, height);
            InventoryWindow.CanDrag = false;
            InventoryWindow.Position = new Point(11, 6);

            int invConWidth = width - 2;
            int invConHeight = height - 2;

            InventoryConsole = new SadConsole.Console(invConWidth, invConHeight);
            InventoryConsole.Position = new Point(1, 1);
            InventoryWindow.Title = title.Align(HorizontalAlignment.Center, invConWidth, (char)196);


            InventoryWindow.Children.Add(InventoryConsole);
            Children.Add(InventoryWindow);

            InventoryWindow.Show();
            InventoryWindow.IsVisible = false;
        }

        public void RemakeMenu() { 
            for (int i = 0; i < MenuImage.Layers[0].Cells.Count; i++) {
                var cell = MenuImage.Layers[0].Cells[i];
                Color convertedFG = new Color(cell.Foreground.R, cell.Foreground.G, cell.Foreground.B);
                Color convertedBG = new Color(cell.Background.R, cell.Background.G, cell.Background.B);

                MainMenuConsole.SetCellAppearance(i % GameLoop.GameWidth, i / GameLoop.GameWidth, new ColoredGlyph(Color.Transparent, convertedFG, MenuImage.Layers[0].Cells[i].Character));
            } 
        }

        public void CreateMainMenu() {
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

            Children.Add(MainMenuWindow);

            MainMenuWindow.Show();
            MainMenuWindow.IsVisible = true;


            NameConsole = new ControlsConsole(13, 1);
            NameBox = new SadConsole.UI.Controls.TextBox(13);
            NameConsole.Controls.Add(NameBox);
            NameConsole.Position = new Point(26, 18);
            
            MainMenuConsole.Children.Add(NameConsole);
            NameConsole.IsVisible = false;
            NameBox.TextChanged += NameChanged;
        }

        private void NameChanged(object sender, EventArgs e) {
            GameLoop.World.Player.Name = NameBox.Text;
        }

        public void CreateSignWindow(int width, int height, string title) {
            SignWindow = new Window(width, height);
            SignWindow.CanDrag = false;
            SignWindow.Position = new Point(19, 11);

            int signConWidth = width - 2;
            int signConHeight = height - 2;

            SignConsole = new SadConsole.Console(signConWidth, signConHeight);
            SignConsole.Position = new Point(1, 1);
            SignWindow.Title = title.Align(HorizontalAlignment.Center, signConWidth, (char)196);


            SignWindow.Children.Add(SignConsole);
            Children.Add(SignWindow);

            SignWindow.Show();
            SignWindow.IsVisible = false;
        }

        private void MinimapRename(object sender, EventArgs e) {
            GameLoop.World.maps[GameLoop.World.Player.MapPos].MinimapTile.name = e.ToString();
        }

        private void SetupCustomColors() {
            CustomColors = SadConsole.UI.Colors.CreateAnsi();
             
            CustomColors.ControlHostBackground = new AdjustableColor(Color.Black, "Black"); 
           // CustomColors.ControlBackLight = (backgroundColor * 1.4f).FillAlpha(); 
            //CustomColors.ControlBackDark = (backgroundColor * 0.7f).FillAlpha();


            //CustomColors.ControlBackSelected = CustomColors.GrayDark;
            CustomColors.Lines = new AdjustableColor(Color.Orange, "White"); 

            CustomColors.Title = new AdjustableColor(Color.Orange, "White");

            CustomColors.RebuildAppearances();

            SadConsole.UI.Themes.Library.Default.Colors = CustomColors;
            


           // SadConsole.UI.Themes.Library.Default.WindowTheme.BorderLineStyle = SadConsole.UI.Window.ConnectedLineThick;
           
        }

        public void SignText(Point locInMap, Point3D MapLoc) {
            selectedMenu = "Sign";
            SignWindow.IsVisible = true;
            SignWindow.IsFocused = true;

            if (MapLoc == new Point3D(0, 1, 0)) { // Town Center
                if (locInMap == new Point(31, 9)) { signText = "Town Hall"; } 
                else if (locInMap == new Point(21, 9)) { signText = "Blacksmith||Weekdays 9am to 5pm|Closed Weekends"; } 
                else if (locInMap == new Point(12, 20)) { signText = "General Store"; } 
                else if (locInMap == new Point(21, 30)) { signText = "Adventure Guild"; }
                else if (locInMap == new Point(29, 30)) { signText = "Apothecary"; }
                else {
                    MessageLog.Add("Sign at (" + locInMap.X + "," + locInMap.Y + ") has no text.");
                }
            } 
            
            else if (MapLoc == new Point3D(-3, 1, 0)) { signText = "North -> Lake|West -> Mountain Cave|East -> Noonbreeze"; }
            else if (MapLoc == new Point3D(-3, -1, 0)) { signText = "Fisherman's Cabin"; } 
            else if (MapLoc == new Point3D(-5, 1, 0)) { signText = "Mountain Tunnel||Under Construction"; } 
            else { MessageLog.Add("Sign at (" + locInMap.X + "," + locInMap.Y + "), map (" + MapLoc.X + "," + MapLoc.Y + ")"); }
        }

        public void CheckFall() {
            if (GameLoop.World.maps[GameLoop.World.Player.MapPos].GetTile(GameLoop.World.Player.Position).Name == "Space" && !flying) {
                GameLoop.CommandManager.MoveActorTo(GameLoop.World.Player, GameLoop.World.Player.Position, GameLoop.World.Player.MapPos + new Point3D(0, 0, -1));
                MessageLog.Add("You fell down!");
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

        public void UpdateVision() {
            if (LimitedVision) {
                FOV.Calculate(GameLoop.World.Player.Position.X, GameLoop.World.Player.Position.Y, GameLoop.World.Player.Vision);
                foreach (var position in FOV.NewlyUnseen) {
                    GameLoop.World.maps[GameLoop.World.Player.MapPos].GetTile(new Point(position.X, position.Y)).Shade();
                }

                foreach (var position in FOV.NewlySeen) {
                    GameLoop.World.maps[GameLoop.World.Player.MapPos].GetTile(new Point(position.X, position.Y)).IsVisible = true;
                    GameLoop.World.maps[GameLoop.World.Player.MapPos].GetTile(new Point(position.X, position.Y)).Unshade();
                }
            }
        }

        public ColoredString ConvertCoppers(int copperValue) {
            int coinsLeft = copperValue;
            int Jade = copperValue / 1000;

            coinsLeft = coinsLeft - (Jade * 1000);
            int gold = coinsLeft / 100;

            coinsLeft = coinsLeft - (gold * 100);
            int silver = coinsLeft / 10;

            coinsLeft = coinsLeft - (silver * 10);
            int copper = coinsLeft;

            ColoredString build = new ColoredString("", Color.White, Color.Black);

            ColoredString copperString = new ColoredString(copper + "c", new Color(184, 115, 51), Color.Black);
            ColoredString silverString = new ColoredString(silver + "s ", Color.Silver, Color.Black);
            ColoredString goldString = new ColoredString(gold + "g ", Color.Yellow, Color.Black);
            ColoredString JadeString = new ColoredString(Jade + "j ", new Color(0, 168, 107), Color.Black);

            if (Jade > 0)
                build += JadeString;
            if (gold > 0)
                build += goldString;
            if (silver > 0)
                build += silverString;
            if (copper > 0)
                build += copperString;

            return build;
        }
    }
}
