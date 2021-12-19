using SadRogue.Primitives;
using SadConsole; 
using System;
using Key = SadConsole.Input.Keys;
using LofiHollow.Entities;
using SadConsole.UI;
using System.Linq;

namespace LofiHollow.UI {
    public class UIManager : ScreenObject {
        public SadConsole.UI.Colors CustomColors;
        
        public SadConsole.Console MapConsole;
        public Window MapWindow;
        public MessageLogWindow MessageLog;
        public Window SidebarWindow;
        public SadConsole.Console SidebarConsole;

        public SadConsole.Console BattleConsole;
        public Window BattleWindow;

        public SadConsole.Console SignConsole;
        public Window SignWindow;

        public SadConsole.Entities.Renderer EntityRenderer;

        public Point targetDir = new Point(0, 0);
        public string targetType = "None";
        public string selectedMenu = "None";
        public string signText = "";
        public bool flying = false;
        public int tileIndex = 0;

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
            CheckKeyboard();
            RenderSidebar();

            if (selectedMenu == "Sign") {
                RenderSign();
            }

            CheckFall();

            base.Update(timeElapsed);
        } 

        public void Init() {
            SetupCustomColors();

            CreateConsoles(); 
            CreateSidebarWindow(28, GameLoop.GameHeight, "");
            CreateBattleWindow(GameLoop.GameWidth / 2, GameLoop.GameHeight / 2, "");
            CreateSignWindow((GameLoop.MapWidth / 2) - 1, GameLoop.MapHeight / 2, "");

            MessageLog = new MessageLogWindow(72, 18, "Message Log");
            Children.Add(MessageLog);
            MessageLog.Show();
            MessageLog.Position = new Point(0, 42);
            MessageLog.Add("Testing 123");

            EntityRenderer = new SadConsole.Entities.Renderer();

           // LoadMap(GameLoop.World.maps[GameLoop.World.Player.MapPos], true);

           // CreateMapWindow(72, 42, "Game Map");
            UseMouse = true;

        }

        private void CheckKeyboard() {
            if (selectedMenu != "Sign" && selectedMenu != "Targeting") {
                bool movedMaps = false;
                if (GameHost.Instance.Keyboard.IsKeyDown(Key.LeftControl)) {
                    if (GameHost.Instance.Keyboard.IsKeyPressed(Key.NumPad8)) { 
                        GameLoop.CommandManager.MoveActorTo(GameLoop.World.Player, GameLoop.World.Player.Position, GameLoop.World.Player.MapPos + new Point3D(0, -1, 0)); 
                    }
                    if (GameHost.Instance.Keyboard.IsKeyPressed(Key.NumPad2)) {
                        GameLoop.CommandManager.MoveActorTo(GameLoop.World.Player, GameLoop.World.Player.Position, GameLoop.World.Player.MapPos + new Point3D(0, 1, 0));
                    }
                    if (GameHost.Instance.Keyboard.IsKeyPressed(Key.NumPad4)) {
                        GameLoop.CommandManager.MoveActorTo(GameLoop.World.Player, GameLoop.World.Player.Position, GameLoop.World.Player.MapPos + new Point3D(-1, 0, 0));
                    }
                    if (GameHost.Instance.Keyboard.IsKeyPressed(Key.NumPad6)) {
                        GameLoop.CommandManager.MoveActorTo(GameLoop.World.Player, GameLoop.World.Player.Position, GameLoop.World.Player.MapPos + new Point3D(1, 0, 0));
                    }
                } else {
                    if (GameHost.Instance.Keyboard.IsKeyPressed(Key.NumPad8)) { GameLoop.CommandManager.MoveActorBy(GameLoop.World.Player, new Point(0, -1)); }
                    if (GameHost.Instance.Keyboard.IsKeyPressed(Key.NumPad2)) { GameLoop.CommandManager.MoveActorBy(GameLoop.World.Player, new Point(0, 1)); }
                    if (GameHost.Instance.Keyboard.IsKeyPressed(Key.NumPad4)) { GameLoop.CommandManager.MoveActorBy(GameLoop.World.Player, new Point(-1, 0)); }
                    if (GameHost.Instance.Keyboard.IsKeyPressed(Key.NumPad6)) { GameLoop.CommandManager.MoveActorBy(GameLoop.World.Player, new Point(1, 0)); }
                    if (GameHost.Instance.Keyboard.IsKeyDown(Key.LeftShift) && GameHost.Instance.Keyboard.IsKeyPressed(Key.OemPeriod)) {
                        if (GameLoop.World.maps[GameLoop.World.Player.MapPos].GetTile(GameLoop.World.Player.Position).Name == "Down Stairs") {
                            GameLoop.CommandManager.MoveActorTo(GameLoop.World.Player, GameLoop.World.Player.Position, GameLoop.World.Player.MapPos + new Point3D(0, 0, -1));
                        }
                    }

                    if (GameHost.Instance.Keyboard.IsKeyDown(Key.LeftShift) && GameHost.Instance.Keyboard.IsKeyPressed(Key.OemComma)) {
                        if (GameLoop.World.maps[GameLoop.World.Player.MapPos].GetTile(GameLoop.World.Player.Position).Name == "Up Stairs") {
                            GameLoop.CommandManager.MoveActorTo(GameLoop.World.Player, GameLoop.World.Player.Position, GameLoop.World.Player.MapPos + new Point3D(0, 0, 1));
                        }
                    }
                }
                if (GameHost.Instance.Keyboard.IsKeyReleased(Key.F9)) {
                    GameLoop.World.SaveMapToFile(GameLoop.World.maps[GameLoop.World.Player.MapPos], GameLoop.World.Player.MapPos);
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
            } else if (selectedMenu == "Targeting") {
                if (GameHost.Instance.Keyboard.IsKeyReleased(Key.NumPad1)) { targetDir = new Point(-1, 1); }
                if (GameHost.Instance.Keyboard.IsKeyReleased(Key.NumPad2)) { targetDir = new Point(0, 1); }
                if (GameHost.Instance.Keyboard.IsKeyReleased(Key.NumPad3)) { targetDir = new Point(1, 1); }
                if (GameHost.Instance.Keyboard.IsKeyReleased(Key.NumPad4)) { targetDir = new Point(-1, 0); } 
                if (GameHost.Instance.Keyboard.IsKeyReleased(Key.NumPad6)) { targetDir = new Point(1, 0); }
                if (GameHost.Instance.Keyboard.IsKeyReleased(Key.NumPad7)) { targetDir = new Point(-1, -1); }
                if (GameHost.Instance.Keyboard.IsKeyReleased(Key.NumPad8)) { targetDir = new Point(0, -1); }
                if (GameHost.Instance.Keyboard.IsKeyReleased(Key.NumPad9)) { targetDir = new Point(1, -1); }

                if (targetType == "Door" && targetDir != new Point(0, 0)) { 
                    GameLoop.World.maps[GameLoop.World.Player.MapPos].ToggleDoor(GameLoop.World.Player.Position + targetDir, false);
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

                if (GameHost.Instance.Keyboard.IsKeyReleased(Key.F3)) {
                    thisMap.name = "Mountain";
                    thisMap.fg = new Color(127, 127, 127);
                    thisMap.ch = (char) 30;
                }

                if (GameHost.Instance.Keyboard.IsKeyReleased(Key.F4)) {
                    thisMap.name = "Forest";
                    thisMap.fg = new Color(0, 127, 0);
                    thisMap.ch = (char)6;
                }

                if (GameHost.Instance.Keyboard.IsKeyReleased(Key.F6)) {
                    thisMap.name = "Town";
                    thisMap.fg = new Color(100, 100, 100);
                    thisMap.ch = (char)127;
                } 


                if (!GameHost.Instance.Keyboard.IsKeyDown(Key.LeftShift)) {
                    if (GameHost.Instance.Mouse.ScrollWheelValueChange < 0) {
                        tileIndex++;
                    } else if (GameHost.Instance.Mouse.ScrollWheelValueChange > 0) {
                        if (tileIndex > 0)
                            tileIndex--;
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
                            TileBase tile = GameLoop.World.tileLibrary[tileIndex];
                            tile.TileID = tileIndex;
                            if (mousePos.ToIndex(GameLoop.MapWidth) < GameLoop.World.maps[GameLoop.World.Player.MapPos].Tiles.Length) 
                                GameLoop.World.maps[GameLoop.World.Player.MapPos].Tiles[mousePos.ToIndex(GameLoop.MapWidth)] = tile; 
                        } else {
                            MessageLog.Add("No tile found");
                        } 
                    }

                    if (GameHost.Instance.Mouse.LeftClicked) {
                        LoadMap(GameLoop.World.maps[GameLoop.World.Player.MapPos], false);
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

        public void LoadMap(Map map, bool firstLoad) {
            if (firstLoad) {
                MapConsole = new SadConsole.Console(GameLoop.MapWidth, GameLoop.MapHeight, map.Tiles);
                CreateMapWindow(72, 42, map.MinimapTile.name);
            } else { 
                for (int i = 0; i < map.Tiles.Length; i++) { 

                    if (map.Tiles[i].Name == "Space" && GameLoop.World.maps.ContainsKey(GameLoop.World.Player.MapPos - new Point3D(0, 0, 1))) {
                        TileBase below = GameLoop.World.maps[GameLoop.World.Player.MapPos - new Point3D(0, 0, 1)].Tiles[i];
                        map.Tiles[i].SetNewFG(below.Foreground * 0.8f, below.Glyph);

                        if (below.Name == "Space" && GameLoop.World.maps.ContainsKey(GameLoop.World.Player.MapPos - new Point3D(0, 0, 2))) {
                            TileBase newBelow = GameLoop.World.maps[GameLoop.World.Player.MapPos - new Point3D(0, 0, 2)].Tiles[i];
                            map.Tiles[i].SetNewFG(newBelow.Foreground * 0.6f, newBelow.Glyph); 

                            if (newBelow.Name == "Space" && GameLoop.World.maps.ContainsKey(GameLoop.World.Player.MapPos - new Point3D(0, 0, 3))) {
                                TileBase newBelow2 = GameLoop.World.maps[GameLoop.World.Player.MapPos - new Point3D(0, 0, 3)].Tiles[i];
                                map.Tiles[i].SetNewFG(newBelow2.Foreground * 0.4f, newBelow2.Glyph);
                                  
                                if (newBelow2.Name == "Space" && GameLoop.World.maps.ContainsKey(GameLoop.World.Player.MapPos - new Point3D(0, 0, 4))) {
                                    TileBase newBelow3 = GameLoop.World.maps[GameLoop.World.Player.MapPos - new Point3D(0, 0, 4)].Tiles[i];
                                    map.Tiles[i].SetNewFG(newBelow3.Foreground * 0.2f, newBelow3.Glyph);
                                }
                            }
                        } 
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


            SidebarConsole.Print(0, 0, " VIT:" + GameLoop.World.Player.Vitality);
            SidebarConsole.Print(0, 1, " SPD:" + GameLoop.World.Player.Speed);
            SidebarConsole.Print(0, 2, " ATK:" + GameLoop.World.Player.Attack);
            SidebarConsole.Print(0, 3, " DEF:" + GameLoop.World.Player.Defense);
            SidebarConsole.Print(0, 4, "MATK:" + GameLoop.World.Player.MagicAttack);
            SidebarConsole.Print(0, 5, "MDEF:" + GameLoop.World.Player.MagicDefense);

            SidebarConsole.DrawLine(new Point(7, 0), new Point(7, 9), (char) 179, Color.Orange, Color.Black);

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
            SidebarConsole.Print(9, 5, new ColoredString((GameLoop.World.Player.HitPoints + "/" + GameLoop.World.Player.MaxHP).Align(HorizontalAlignment.Right, 7), Color.Red, Color.Black));
            SidebarConsole.Print(9, 6, new ColoredString((GameLoop.World.Player.Energy + "/" + GameLoop.World.Player.MaxEnergy).Align(HorizontalAlignment.Right, 7), Color.LimeGreen, Color.Black));
            SidebarConsole.Print(9, 7, new ColoredString((GameLoop.World.Player.Mana + "/" + GameLoop.World.Player.MaxMana).Align(HorizontalAlignment.Right, 7), Color.Cyan, Color.Black));
            SidebarConsole.Print(9, 8, new ColoredString(GameLoop.World.Player.Gold.ToString().Align(HorizontalAlignment.Right, 7), Color.Cyan, Color.Black));

            SidebarConsole.DrawLine(new Point(16, 0), new Point(16, 9), (char) 179, Color.Orange, Color.Black);
            // The minimap area (top-right)

            for (int x = GameLoop.World.Player.MapPos.X - 4; x < GameLoop.World.Player.MapPos.X + 5; x++) {
                for (int y = GameLoop.World.Player.MapPos.Y - 4; y < GameLoop.World.Player.MapPos.Y + 5; y++) {
                    if (GameLoop.World.maps.ContainsKey(new Point3D(x, y, GameLoop.World.Player.MapPos.Z))) {
                        Point3D modifiedPos = new Point3D(x, y, 0) - GameLoop.World.Player.MapPos;
                        SidebarConsole.Print(modifiedPos.X + 21, modifiedPos.Y + 4, GameLoop.World.maps[new Point3D(x, y, GameLoop.World.Player.MapPos.Z)].MinimapTile.AsColoredGlyph());
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
            }  
        }

        public void CreateConsoles() {
            MapConsole = new SadConsole.Console(GameLoop.MapWidth, GameLoop.MapHeight);
            SidebarConsole = new SadConsole.Console(28, GameLoop.GameHeight);
            BattleConsole = new SadConsole.Console(GameLoop.GameWidth / 2, GameLoop.GameHeight / 2);
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

            BattleWindow.Show();
            BattleWindow.IsVisible = false;
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
            
            else { MessageLog.Add("Sign at (" + locInMap.X + "," + locInMap.Y + "), map (" + MapLoc.X + "," + MapLoc.Y + ")"); }
        }

        public void CheckFall() {
            if (GameLoop.World.maps[GameLoop.World.Player.MapPos].GetTile(GameLoop.World.Player.Position).Name == "Space" && !flying) {
                GameLoop.CommandManager.MoveActorTo(GameLoop.World.Player, GameLoop.World.Player.Position, GameLoop.World.Player.MapPos + new Point3D(0, 0, -1));
                MessageLog.Add("You fell down!");
            }
        }
    }
}
