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

        public Point targetDir = new Point(0, 0);
        public string targetType = "None";
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


            LoadMap(GameLoop.World.maps[GameLoop.World.Player.MapPos], true);

           // CreateMapWindow(72, 42, "Game Map");
            UseMouse = true;
        }

        private void CheckKeyboard() {
            if (Global.KeyboardState.IsKeyPressed(Key.NumPad8)) { GameLoop.CommandManager.MoveActorBy(GameLoop.World.Player, new Point(0, -1)); }
            if (Global.KeyboardState.IsKeyPressed(Key.NumPad2)) { GameLoop.CommandManager.MoveActorBy(GameLoop.World.Player, new Point(0, 1)); }
            if (Global.KeyboardState.IsKeyPressed(Key.NumPad4)) { GameLoop.CommandManager.MoveActorBy(GameLoop.World.Player, new Point(-1, 0)); }
            if (Global.KeyboardState.IsKeyPressed(Key.NumPad6)) { GameLoop.CommandManager.MoveActorBy(GameLoop.World.Player, new Point(1, 0)); }
            if (Global.KeyboardState.IsKeyReleased(Key.F9)) {
                GameLoop.World.SaveMapToFile(GameLoop.World.maps[GameLoop.World.Player.MapPos], GameLoop.World.Player.MapPos);
            }

            if (selectedMenu != "Map Editor" && selectedMenu != "Targeting") {
                
                if (Global.KeyboardState.IsKeyReleased(Key.C)) {
                    selectedMenu = "Targeting";
                    targetType = "Door";
                    MessageLog.Add("Close door where?");
                } 
            } else if (selectedMenu == "Targeting") {
                if (Global.KeyboardState.IsKeyReleased(Key.NumPad1)) { targetDir = new Point(-1, 1); }
                if (Global.KeyboardState.IsKeyReleased(Key.NumPad2)) { targetDir = new Point(0, 1); }
                if (Global.KeyboardState.IsKeyReleased(Key.NumPad3)) { targetDir = new Point(1, 1); }
                if (Global.KeyboardState.IsKeyReleased(Key.NumPad4)) { targetDir = new Point(-1, 0); } 
                if (Global.KeyboardState.IsKeyReleased(Key.NumPad6)) { targetDir = new Point(1, 0); }
                if (Global.KeyboardState.IsKeyReleased(Key.NumPad7)) { targetDir = new Point(-1, -1); }
                if (Global.KeyboardState.IsKeyReleased(Key.NumPad8)) { targetDir = new Point(0, -1); }
                if (Global.KeyboardState.IsKeyReleased(Key.NumPad9)) { targetDir = new Point(1, -1); }

                if (targetType == "Door" && targetDir != new Point(0, 0)) { 
                    GameLoop.World.maps[GameLoop.World.Player.MapPos].ToggleDoor(GameLoop.World.Player.Position + targetDir, false);
                    targetType = "Done";
                } else if (targetType != "Door") {
                    targetType = "None";
                    targetDir = new Point(0, 0);
                    selectedMenu = "None";
                }
            } else if (selectedMenu == "Map Editor") {
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

                if (Global.KeyboardState.IsKeyReleased(Key.F3)) {
                    thisMap.name = "Mountain";
                    thisMap.fg = new Color(127, 127, 127);
                    thisMap.ch = (char) 30;
                }

                if (Global.KeyboardState.IsKeyReleased(Key.F4)) {
                    thisMap.name = "Forest";
                    thisMap.fg = new Color(0, 127, 0);
                    thisMap.ch = (char)6;
                }

                if (Global.KeyboardState.IsKeyReleased(Key.F6)) {
                    thisMap.name = "Town";
                    thisMap.fg = new Color(100, 100, 100);
                    thisMap.ch = (char)127;
                } 


                if (!Global.KeyboardState.IsKeyDown(Key.LeftShift)) {
                    if (Global.MouseState.ScrollWheelValueChange < 0) {
                        tileIndex++;
                    } else if (Global.MouseState.ScrollWheelValueChange > 0) {
                        if (tileIndex > 0)
                            tileIndex--;
                    }
                } else {
                    if (Global.MouseState.ScrollWheelValueChange < 0) {
                        thisMap.ch++;
                    } else if (Global.MouseState.ScrollWheelValueChange > 0) {
                        if (thisMap.ch > 0)
                            thisMap.ch--;
                    }
                }

                Point mousePos = Global.MouseState.ScreenPosition.PixelLocationToConsole(12, 12);
                if (mousePos.X < 72) {
                    if (Global.MouseState.LeftButtonDown) {
                        var mouse = new SadConsole.Input.MouseConsoleState(MapConsole, Global.MouseState);

                        if (mouse.IsOnConsole) {
                            if (GameLoop.World.tileLibrary.ContainsKey(tileIndex)) {
                                TileBase tile = GameLoop.World.tileLibrary[tileIndex];
                                tile.TileID = tileIndex;
                                GameLoop.World.maps[GameLoop.World.Player.MapPos].Tiles[mouse.CellPosition.ToIndex(GameLoop.World._mapWidth)] = tile;
                                MapConsole.SetRenderCells();
                            } else {
                                MessageLog.Add("No tile found");
                            }
                        }
                    }

                    if (Global.MouseState.LeftClicked) {
                        LoadMap(GameLoop.World.maps[GameLoop.World.Player.MapPos], false);
                    }
                } else {
                    if (Global.MouseState.LeftButtonDown) {
                        var mouse = new SadConsole.Input.MouseConsoleState(SidebarConsole, Global.MouseState);

                        if (mouse.IsOnConsole) {
                            if (mouse.CellPosition == new Point(8, 14)) { thisMap.fg.R--; }
                            if (mouse.CellPosition == new Point(14, 14)) { thisMap.fg.R++; }
                            if (mouse.CellPosition == new Point(8, 15)) { thisMap.fg.G--; }
                            if (mouse.CellPosition == new Point(14, 15)) { thisMap.fg.G++; }
                            if (mouse.CellPosition == new Point(8, 16)) { thisMap.fg.B--; }
                            if (mouse.CellPosition == new Point(14, 16)) { thisMap.fg.B++; }
                        }
                    }

                    if (Global.MouseState.RightClicked) {
                        var mouse = new SadConsole.Input.MouseConsoleState(SidebarConsole, Global.MouseState);

                        if (mouse.IsOnConsole) {
                            if (mouse.CellPosition == new Point(8, 14)) { thisMap.fg.R--; }
                            if (mouse.CellPosition == new Point(14, 14)) { thisMap.fg.R++; }
                            if (mouse.CellPosition == new Point(8, 15)) { thisMap.fg.G--; }
                            if (mouse.CellPosition == new Point(14, 15)) { thisMap.fg.G++; }
                            if (mouse.CellPosition == new Point(8, 16)) { thisMap.fg.B--; }
                            if (mouse.CellPosition == new Point(14, 16)) { thisMap.fg.B++; }
                        }
                    }
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

        public void LoadMap(Map map, bool firstLoad) {
            if (firstLoad) {
                MapConsole = new SadConsole.Console(GameLoop.World._mapWidth, GameLoop.World._mapHeight, Global.FontDefault, map.Tiles);
                CreateMapWindow(72, 42, map.MinimapTile.name);
            } else {
                MapConsole.SetSurface(map.Tiles, GameLoop.World._mapWidth, GameLoop.World._mapHeight);
                MapConsole.SetRenderCells();
                MapWindow.Title = GameLoop.World.maps[GameLoop.World.Player.MapPos].MinimapTile.name.Align(HorizontalAlignment.Center, GameLoop.World._mapWidth - 2, (char)205);
            }
            
           
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

                SidebarConsole.Print(0, 14, "Map fR: - " + thisMap.fg.R);
                SidebarConsole.Print(14, 14, "+");
                SidebarConsole.Print(0, 15, "Map fG: - " + thisMap.fg.G);
                SidebarConsole.Print(14, 15, "+");
                SidebarConsole.Print(0, 16, "Map fB: - " + thisMap.fg.B);
                SidebarConsole.Print(14, 16, "+");


                SidebarConsole.Print(0, 19, "Tile Index: " + tileIndex);
                

                if (GameLoop.World.tileLibrary.ContainsKey(tileIndex)) {
                    TileBase tile = GameLoop.World.tileLibrary[tileIndex];

                    SidebarConsole.Print(0, 20, "Tile Name: " + tile.Name);
                    SidebarConsole.Print(0, 21, "Tile Appearance: ");
                    SidebarConsole.Print(17, 21, tile.AsColoredGlyph());

                }
            }  
        }

        public void CreateConsoles() {
            MapConsole = new SadConsole.Console(GameLoop.World._mapWidth, GameLoop.World._mapHeight, Global.FontDefault);
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
            int sidebarConsoleHeight = height - 2;
             
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
