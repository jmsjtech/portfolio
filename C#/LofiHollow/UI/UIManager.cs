using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Controls;
using System;
using Key = Microsoft.Xna.Framework.Input.Keys;
using LofiHollow.Entities;

namespace LofiHollow.UI {
    public class UIManager : ContainerConsole {
        public SadConsole.Themes.Colors CustomColors;
        
        public SadConsole.Console MapConsole;
        public Window MapWindow;
        public MessageLogWindow MessageLog;
        public Window SidebarWindow;
        public SadConsole.Console SidebarConsole;
        public SadConsole.Controls.TextBox minimapNameBox;

        public string selectedMenu = "None";
        public int tileIndex = 0;

        public UIManager() {
            // must be set to true
            // or will not call each child's Draw method
            IsVisible = true;
            IsFocused = true;

            // The UIManager becomes the only
            // screen that SadConsole processes
            Parent = SadConsole.Global.CurrentScreen;
        }

        public override void Update(TimeSpan timeElapsed) {
            CheckKeyboard();
            RenderSidebar();

            base.Update(timeElapsed);
        }

        public void Init() {
            SetupCustomColors();

            CreateConsoles(); 
            CreateSidebarWindow(28, GameLoop.GameHeight, "");

            MessageLog = new MessageLogWindow(72, 18, "Message Log");
            Children.Add(MessageLog);
            MessageLog.Show();
            MessageLog.Position = new Point(0, 42);
            MessageLog.Add("Testing 123");


            LoadMap(GameLoop.World.maps[GameLoop.World.Player.MapPos]);

           // CreateMapWindow(72, 42, "Game Map");
            UseMouse = true;
        }

        private void CheckKeyboard() {
            if (selectedMenu != "Map Editor") {
                if (Global.KeyboardState.IsKeyDown(Key.W) || Global.KeyboardState.IsKeyDown(Key.NumPad8)) { GameLoop.CommandManager.MoveActorBy(GameLoop.World.Player, new Point(0, -1)); }
                if (Global.KeyboardState.IsKeyDown(Key.S) || Global.KeyboardState.IsKeyDown(Key.NumPad2)) { GameLoop.CommandManager.MoveActorBy(GameLoop.World.Player, new Point(0, 1)); }
                if (Global.KeyboardState.IsKeyDown(Key.A) || Global.KeyboardState.IsKeyDown(Key.NumPad4)) { GameLoop.CommandManager.MoveActorBy(GameLoop.World.Player, new Point(-1, 0)); }
                if (Global.KeyboardState.IsKeyDown(Key.D) || Global.KeyboardState.IsKeyDown(Key.NumPad6)) { GameLoop.CommandManager.MoveActorBy(GameLoop.World.Player, new Point(1, 0)); }

                if (Global.KeyboardState.IsKeyReleased(Key.P)) {
                    GameLoop.World.SaveMapToFile(GameLoop.World.maps[GameLoop.World.Player.MapPos], GameLoop.World.Player.MapPos);
                }
            } else {
                MinimapTile thisMap = GameLoop.World.maps[GameLoop.World.Player.MapPos].MinimapTile;

                foreach (var key in Global.KeyboardState.KeysReleased) {
                    if (key.Character >= 'A' && key.Character <= 'z') {
                        thisMap.name += key.Character;
                    }
                }
                if (Global.KeyboardState.IsKeyReleased(Key.Back)) {
                    if (thisMap.name.Length > 0) { thisMap.name = thisMap.name.Substring(0, thisMap.name.Length - 1); }
                } else if (Global.KeyboardState.IsKeyReleased(Key.Space)) {
                    thisMap.name += " ";
                }

                if (Global.MouseState.ScrollWheelValueChange < 0) {
                    tileIndex++;
                } else if (Global.MouseState.ScrollWheelValueChange > 0) {
                    if (tileIndex > 0)
                        tileIndex--;
                }

                if (Global.MouseState.LeftButtonDown) {
                    Point mousePos = Global.MouseState.ScreenPosition.PixelLocationToConsole(12, 12);
                    // 1,1 to 70,40 is game map

                    if (mousePos.X >= 1 && mousePos.X <= 70 && mousePos.Y >= 1 && mousePos.Y <= 40) {
                        Point mapPos = mousePos - new Point(1, 1);
                        if (GameLoop.World.tileLibrary.ContainsKey(tileIndex)) {
                            TileBase tile = GameLoop.World.tileLibrary[tileIndex];
                            GameLoop.World.maps[GameLoop.World.Player.MapPos].Tiles[mapPos.ToIndex(GameLoop.World._mapWidth)] = tile;
                            MapConsole.Print(mapPos.X, mapPos.Y, tile.AsColoredGlyph());
                            MapConsole.IsDirty = true;
                        } else {
                            MessageLog.Add("No tile found");
                        }
                    }
                }

                if (Global.MouseState.LeftClicked) {
                    LoadMap(GameLoop.World.maps[GameLoop.World.Player.MapPos]);
                }
            }

            if (Global.KeyboardState.IsKeyReleased(Key.F1)) {
                if (selectedMenu == "Map Editor") {
                    selectedMenu = "None";
                } else {
                    selectedMenu = "Map Editor";
                }
            }
        }

        public void LoadMap(Map map) {
            MapConsole = new SadConsole.Console(GameLoop.World._mapWidth, GameLoop.World._mapHeight, Global.FontDefault, map.Tiles);
            CreateMapWindow(72, 42, "Game Map");
            SyncMapEntities(map);
        }

        public void SyncMapEntities(Map map) {
            MapConsole.Children.Clear();

            foreach (Entity entity in map.Entities.Items) {
                MapConsole.Children.Add(entity);
            }

            MapConsole.Children.Add(GameLoop.World.Player);

            map.Entities.ItemAdded += OnMapEntityAdded;
            map.Entities.ItemRemoved += OnMapEntityRemoved;
        }

        // Remove an Entity from the MapConsole every time the Map's Entity collection changes
        public void OnMapEntityRemoved(object sender, GoRogue.ItemEventArgs<Entity> args) {
            MapConsole.Children.Remove(args.Item);
        }

        // Add an Entity to the MapConsole every time the Map's Entity collection changes
        public void OnMapEntityAdded(object sender, GoRogue.ItemEventArgs<Entity> args) {
            MapConsole.Children.Add(args.Item);
        }

        private void RenderSidebar() {
            SidebarConsole.Clear();

            string timeHour = GameLoop.World.Hours.ToString();
            if (timeHour.Length == 1)
                timeHour = "0" + timeHour;

            string timeMinute = GameLoop.World.Minutes.ToString();
            if (timeMinute.Length == 1)
                timeMinute = "0" + timeMinute;

            string time = timeHour + ":" + timeMinute;


            string[] months = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };


            SidebarConsole.Print(0, 0, "STR: " + GameLoop.World.Player.Strength);
            SidebarConsole.Print(0, 1, "VIT: " + GameLoop.World.Player.Vitality);
            SidebarConsole.Print(0, 2, "INT: " + GameLoop.World.Player.Intelligence);
            SidebarConsole.Print(0, 3, "DEX: " + GameLoop.World.Player.Dexterity);

            SidebarConsole.DrawLine(new Point(7, 0), new Point(7, 9), Color.White, Color.Black, (char)186);

            // The time and date area (top-left)
            SidebarConsole.Print(8, 0, time);
            SidebarConsole.Print(14, 0, GameLoop.World.AM ? "AM" : "PM");
            SidebarConsole.Print(8, 2, (months[GameLoop.World.Month - 1] + " " + GameLoop.World.Day).Align(HorizontalAlignment.Right, 8));
            SidebarConsole.Print(9, 3, ("Year " + GameLoop.World.Year).Align(HorizontalAlignment.Right, 7));

            // HP / Energy / Mana / Gold
            SidebarConsole.Print(8, 5, new ColoredString(((char) 3).ToString(), Color.Red, Color.Black));
            SidebarConsole.Print(8, 6, new ColoredString(((char) 175).ToString(), Color.Lime, Color.Black));
            SidebarConsole.Print(8, 7, new ColoredString(((char) 168).ToString(), Color.Cyan, Color.Black));
            SidebarConsole.Print(8, 8, new ColoredString(((char) 1).ToString(), Color.Goldenrod, Color.Black));
            SidebarConsole.Print(9, 5, new ColoredString((GameLoop.World.Player.Health + "/" + GameLoop.World.Player.MaxHealth).Align(HorizontalAlignment.Right, 7), Color.Red, Color.Black));
            SidebarConsole.Print(9, 6, new ColoredString((GameLoop.World.Player.Energy + "/" + GameLoop.World.Player.MaxEnergy).Align(HorizontalAlignment.Right, 7), Color.LimeGreen, Color.Black));
            SidebarConsole.Print(9, 7, new ColoredString((GameLoop.World.Player.Mana + "/" + GameLoop.World.Player.MaxMana).Align(HorizontalAlignment.Right, 7), Color.Cyan, Color.Black));
            SidebarConsole.Print(9, 8, new ColoredString(GameLoop.World.Player.Gold.ToString().Align(HorizontalAlignment.Right, 7), Color.Cyan, Color.Black));

            SidebarConsole.DrawLine(new Point(16, 0), new Point(16, 9), Color.White, Color.Black, (char)186);
            // The minimap area (top-right)

            for (int x = GameLoop.World.Player.MapPos.X - 4; x < GameLoop.World.Player.MapPos.X + 5; x++) {
                for (int y = GameLoop.World.Player.MapPos.Y - 4; y < GameLoop.World.Player.MapPos.Y + 5; y++) {
                    if (GameLoop.World.maps.ContainsKey(new Point(x, y))) {
                        Point modifiedPos = new Point(x, y) - GameLoop.World.Player.MapPos;
                        SidebarConsole.Print(modifiedPos.X + 21, modifiedPos.Y + 4, GameLoop.World.maps[new Point(x, y)].MinimapTile.AsColoredGlyph());
                    }
                }
            }

            SidebarConsole.Print(21, 4, "@", Color.White);
            SidebarConsole.DrawLine(new Point(0, 9), new Point(25, 9), Color.White, Color.Black, (char)205);


            if (selectedMenu == "Map Editor") { 
                MinimapTile thisMap = GameLoop.World.maps[GameLoop.World.Player.MapPos].MinimapTile;
                SidebarConsole.Print(0, 10, "Map Name: " + thisMap.name);
                SidebarConsole.Print(0, 11, "Map Icon: ");
                SidebarConsole.Print(10, 11, thisMap.AsColoredGlyph());
                SidebarConsole.Print(0, 13, "Position: " + GameLoop.World.Player.MapPos.ToString());
                SidebarConsole.Print(0, 17, "Tile Index: " + tileIndex);

                if (GameLoop.World.tileLibrary.ContainsKey(tileIndex)) {
                    TileBase tile = GameLoop.World.tileLibrary[tileIndex];

                    SidebarConsole.Print(0, 18, "Tile Name: " + tile.Name);
                    SidebarConsole.Print(0, 19, "Tile Appearance: ");
                    SidebarConsole.Print(17, 19, tile.AsColoredGlyph());
                }
            }  
        }

        public void CreateConsoles() {
            MapConsole = new SadConsole.ScrollingConsole(GameLoop.World._mapWidth, GameLoop.World._mapHeight, Global.FontDefault);
            SidebarConsole = new SadConsole.Console(28, GameLoop.GameHeight, Global.FontDefault);
        }

        public void CreateMapWindow(int width, int height, string title) {
            MapWindow = new Window(width, height);
            MapWindow.CanDrag = false;

            int mapConsoleWidth = width - 2;
            int mapConsoleHeight = height - 2;
             
            MapConsole.Position = new Point(1, 1);
            MapWindow.Title = title.Align(HorizontalAlignment.Center, mapConsoleWidth, (char) 205);

            MapWindow.Children.Add(MapConsole);
            Children.Add(MapWindow);
            MapConsole.Children.Add(GameLoop.World.Player);

            MapWindow.Show();
        }

        public void CreateSidebarWindow(int width, int height, string title) {
            SidebarWindow = new Window(width, height);
            SidebarWindow.CanDrag = false;
            SidebarWindow.Position = new Point(72, 0);

            int sidebarConsoleWidth = width - 2;
            int sidebarCOnsoleHeight = height - 2;
             
            SidebarConsole.Position = new Point(1, 1);
            SidebarWindow.Title = title.Align(HorizontalAlignment.Center, sidebarConsoleWidth, (char)205);

            SidebarWindow.Children.Add(SidebarConsole);
            Children.Add(SidebarWindow);

            SidebarWindow.Show();
        }

        private void MinimapRename(object sender, EventArgs e) {
            GameLoop.World.maps[GameLoop.World.Player.MapPos].MinimapTile.name = e.ToString();
        }

        private void SetupCustomColors() {
            CustomColors = SadConsole.Themes.Colors.CreateDefault();

            Color backgroundColor = Color.Black;

            CustomColors.ControlHostBack = backgroundColor;
            CustomColors.ControlBack = backgroundColor;

            CustomColors.ControlBackLight = (backgroundColor * 1.4f).FillAlpha(); 
            CustomColors.ControlBackDark = (backgroundColor * 0.7f).FillAlpha();


            CustomColors.ControlBackSelected = CustomColors.GrayDark;
            CustomColors.Lines = Color.White;

            CustomColors.TitleText = Color.White;

            CustomColors.RebuildAppearances();

            SadConsole.Themes.Library.Default.Colors = CustomColors;
            SadConsole.Themes.Library.Default.WindowTheme.BorderLineStyle = SadConsole.Window.ConnectedLineThick;
           
        }
    }
}
