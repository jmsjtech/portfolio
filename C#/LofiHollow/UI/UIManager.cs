using SadRogue.Primitives;
using SadConsole; 
using System;
using Key = SadConsole.Input.Keys;
using LofiHollow.Entities;
using SadConsole.UI;
using System.Linq;
using SadConsole.Input;
using System.Collections.Generic;

namespace LofiHollow.UI {
    public class UIManager : ScreenObject {
        public SadConsole.UI.Colors CustomColors;
        
        public SadConsole.Console MapConsole;
        public Window MapWindow;
        public MessageLogWindow MessageLog;
        public Window SidebarWindow;
        public SadConsole.Console SidebarConsole;

        public SadConsole.Console BattleConsole;
        public SadConsole.Console BattleLog;
        public Window BattleWindow;
        public SadConsole.Console MoveConsole;
        public Window MoveWindow;

        public SadConsole.Console SignConsole;
        public Window SignWindow;

        public SadConsole.Console InventoryConsole;
        public Window InventoryWindow;

        public SadConsole.Entities.Renderer EntityRenderer;

        public Point targetDir = new Point(0, 0);
        public string targetType = "None";
        public string selectedMenu = "None";
        public string battleResult = "None";
        public bool battleDone = false;
        public string moveMenu = "None";
        public string signText = "";

        public List<Item> dropTable = new List<Item>();

        public bool flying = false;
        public int tileIndex = 0;

        public int hotbarSelect = 0;
        public int moveIndex = 0;

        public int vitChange = 0;
        public int spdChange = 0;
        public int atkChange = 0;
        public int defChange = 0;
        public int matkChange = 0;
        public int mdefChange = 0;

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

                if (MoveWindow.IsVisible) {
                    RenderMoves();
                    CaptureMoveClicks();
                } else {
                    CaptureBattleClicks();
                } 
            } else {
                CheckKeyboard();
            }

            RenderOverlays();

            CheckFall();

            base.Update(timeElapsed);
        } 

        public void Init() {
            SetupCustomColors();

            CreateConsoles(); 
            CreateSidebarWindow(28, GameLoop.GameHeight, "");
            CreateBattleWindow(72, 42, ""); 
            CreateInventoryWindow(GameLoop.GameWidth / 2, GameLoop.GameHeight / 2, "");
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
                    if (GameHost.Instance.Keyboard.IsKeyDown(Key.W)) { GameLoop.CommandManager.MoveActorBy(GameLoop.World.Player, new Point(0, -1)); }
                    if (GameHost.Instance.Keyboard.IsKeyDown(Key.S)) { GameLoop.CommandManager.MoveActorBy(GameLoop.World.Player, new Point(0, 1)); }
                    if (GameHost.Instance.Keyboard.IsKeyDown(Key.A)) { GameLoop.CommandManager.MoveActorBy(GameLoop.World.Player, new Point(-1, 0)); }
                    if (GameHost.Instance.Keyboard.IsKeyDown(Key.D)) { GameLoop.CommandManager.MoveActorBy(GameLoop.World.Player, new Point(1, 0)); }
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
                                        if (moveIndex == -1)
                                            moveIndex = slot;
                                        else {
                                            Item tempID = GameLoop.World.Player.Inventory[moveIndex];
                                            GameLoop.World.Player.Inventory[moveIndex] = GameLoop.World.Player.Inventory[slot];
                                            GameLoop.World.Player.Inventory[slot] = tempID;
                                            moveIndex = -1;
                                        }
                                    } else if (x > 35 && x < 43) {
                                        GameLoop.CommandManager.EquipItem(GameLoop.World.Player, slot, GameLoop.World.Player.Inventory[slot].ItemID);
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

                Point mousePos = new MouseScreenObjectState(SidebarConsole, GameHost.Instance.Mouse).CellPosition - new Point(0, 26);
                if (mousePos.X > 0) { // Clicked in Sidebar
                    if (GameHost.Instance.Mouse.LeftClicked) { 
                        int slot = mousePos.Y;
                        if (slot >= 0 && slot <= 6)
                            GameLoop.CommandManager.UnequipItem(GameLoop.World.Player, slot); 
                    }
                }
            } else if (selectedMenu == "Targeting") { 
                if (GameHost.Instance.Keyboard.IsKeyReleased(Key.S)) { targetDir = new Point(0, 1); } 
                if (GameHost.Instance.Keyboard.IsKeyReleased(Key.A)) { targetDir = new Point(-1, 0); } 
                if (GameHost.Instance.Keyboard.IsKeyReleased(Key.D)) { targetDir = new Point(1, 0); } 
                if (GameHost.Instance.Keyboard.IsKeyReleased(Key.W)) { targetDir = new Point(0, -1); } 

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

        private void RenderOverlays() {
            for (int i = 0; i < GameLoop.World.maps[GameLoop.World.Player.MapPos].Tiles.Length; i++) {
                if (GameLoop.World.maps[GameLoop.World.Player.MapPos].Tiles[i].Name == "Bed") {
                    MapConsole.AddDecorator(i, 1, new CellDecorator(Color.White, 14, Mirror.None));
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
            InventoryConsole.Clear();
            InventoryConsole.Print((InventoryConsole.Width / 2) - 4, 0, "BACKPACK");

            for (int i = 0; i < 27; i++) {
                if (i < GameLoop.World.Player.Inventory.Length) {
                    Item item = GameLoop.World.Player.Inventory[i];
                     
                    InventoryConsole.Print(0, i + 1, item.AsColoredGlyph());
                    if (!item.IsStackable || (item.IsStackable && item.ItemQuantity == 1))
                        InventoryConsole.Print(2, i + 1, new ColoredString(item.Name, moveIndex == i ? Color.Yellow : item.Name == "(EMPTY)" ? Color.DarkSlateGray : Color.White, Color.Black));
                    else
                        InventoryConsole.Print(2, i + 1, new ColoredString(("(" + item.ItemQuantity + ") " + item.Name), i == hotbarSelect ? Color.Yellow : item.Name == "(EMPTY)" ? Color.DarkSlateGray : Color.White, Color.TransparentBlack));
                    
                    InventoryConsole.Print(28, i + 1, new ColoredString("MOVE | EQUIP | DROP", item.Name == "(EMPTY)" ? Color.DarkSlateGray : Color.White, Color.Black));
                } else {
                    InventoryConsole.Print(2, i + 1, new ColoredString("[LOCKED]", Color.DarkSlateGray, Color.Black));
                }
            } 

        }

        private ColoredString DrawBar(int curr, int max, Color on, Color off) {
            ColoredGlyph[] parts = new ColoredGlyph[10];

            float percent = ((float) curr / (float) max) * 100;
             

            for (int i = 0; i < 10; i++) {
                if (percent >= 10) {
                    percent -= 10;
                    parts[i] = new ColoredGlyph(on, Color.Black, 254);
                } else {
                    parts[i] = new ColoredGlyph(off, Color.Black, 254);
                }
            }

            return new ColoredString(parts);
        }

        private void RenderBattle() { 
            BattleConsole.Clear();
            BattleConsole.IsFocused = true;

            if ((selectedMenu == "Battle" || selectedMenu == "TurnWait")) {
                string enemyTitle = "";

                if (GameLoop.BattleManager.Enemy.Descriptor != "")
                    enemyTitle = GameLoop.BattleManager.Enemy.Descriptor + " " + GameLoop.BattleManager.Enemy.Name + " (Lv." + GameLoop.BattleManager.Enemy.Level + ")";
                else
                    enemyTitle = GameLoop.BattleManager.Enemy.Name + " (Lv." + GameLoop.BattleManager.Enemy.Level + ")";
                BattleConsole.Print(BattleConsole.Width - 31, 0, enemyTitle.Align(HorizontalAlignment.Right, 30));
                BattleConsole.Print(BattleConsole.Width - 1, 1, new ColoredString(((char)3).ToString(), Color.Red, Color.Black));
                BattleConsole.Print(BattleConsole.Width - 11, 1, DrawBar(GameLoop.BattleManager.Enemy.HitPoints, GameLoop.BattleManager.Enemy.MaxHP, Color.Red, Color.Black));

                BattleConsole.Print(0, 0, GameLoop.World.Player.Name + " (Lv. " + GameLoop.World.Player.Level + ")");
                BattleConsole.Print(0, 1, new ColoredString(((char)3).ToString(), Color.Red, Color.Black));
                BattleConsole.Print(0, 2, new ColoredString(((char)175).ToString(), Color.Lime, Color.Black));
                BattleConsole.Print(0, 3, new ColoredString(((char)168).ToString(), Color.Cyan, Color.Black));
                BattleConsole.Print(1, 1, DrawBar(GameLoop.World.Player.HitPoints, GameLoop.World.Player.MaxHP, Color.Red, Color.Black));
                BattleConsole.Print(1, 2, DrawBar(GameLoop.World.Player.Energy, GameLoop.World.Player.MaxEnergy, Color.LimeGreen, Color.Black));
                BattleConsole.Print(1, 3, DrawBar(GameLoop.World.Player.Mana, GameLoop.World.Player.MaxMana, Color.Cyan, Color.Black));


                BattleConsole.DrawLine(new Point(0, 30), new Point(BattleConsole.Width - 1, 30), (char)196, new Color(0, 127, 0), Color.Black);
                BattleConsole.Print(10, 29, GameLoop.World.Player.Appearance);
                BattleConsole.Print(BattleConsole.Width - 11, 29, GameLoop.BattleManager.Enemy.Appearance);

                BattleConsole.DrawLine(new Point(0, 31), new Point(BattleConsole.Width - 1, 31), (char)196, Color.Orange, Color.Black);
                BattleConsole.Print(1, 32, GameLoop.World.moveLibrary[moveIndex].Name.ToUpper().Align(HorizontalAlignment.Center, 16));
                BattleConsole.Print(1, 33, new ColoredString("CHANGE".Align(HorizontalAlignment.Center, 16), Color.Gray, Color.Black));
                BattleConsole.Print(1, 34, "ITEM".Align(HorizontalAlignment.Center, 16));
                //BattleConsole.Print(BattleConsole.Width - 7, BattleConsole.Height - 4, (GameLoop.BattleManager.fleePercent + "%").Align(HorizontalAlignment.Right, 5));
                BattleConsole.Print(1, BattleConsole.Height - 1, "FLEE".Align(HorizontalAlignment.Center, 16));


                BattleConsole.DrawLine(new Point(18, 31), new Point(18, BattleConsole.Height - 1), (char)179, Color.Orange, Color.Black);

                if (selectedMenu == "TurnWait")
                    BattleLog.Print((BattleLog.Width / 2) - 3, 4, "[OKAY]");
            } else if (selectedMenu == "BattleDone" && battleResult == "Victory") {
                BattleConsole.Print(0, 8, "Victory!".Align(HorizontalAlignment.Center, BattleConsole.Width - 2));
                BattleConsole.Print(0, 10, ("You got " + GameLoop.BattleManager.Enemy.ExpGranted + " exp.").Align(HorizontalAlignment.Center, BattleConsole.Width - 2));
                BattleConsole.Print(0, 25, "[Close]".Align(HorizontalAlignment.Center, BattleConsole.Width - 2));
            } else if (selectedMenu == "BattleDone" && battleResult == "Level") {
                BattleConsole.Print(0, 8, "Victory!".Align(HorizontalAlignment.Center, BattleConsole.Width - 2));
                BattleConsole.Print(0, 10, ("You got " + GameLoop.BattleManager.Enemy.ExpGranted + " exp and levelled up!").Align(HorizontalAlignment.Center, BattleConsole.Width - 2));
                BattleConsole.Print(0, 12, "Pick a stat to recieve a boost:".Align(HorizontalAlignment.Center, BattleConsole.Width - 2));
                BattleConsole.Print(0, 16, "Vitality".Align(HorizontalAlignment.Center, BattleConsole.Width - 2));
                BattleConsole.Print(0, 18, "Speed".Align(HorizontalAlignment.Center, BattleConsole.Width - 2));
                BattleConsole.Print(0, 20, "Attack".Align(HorizontalAlignment.Center, BattleConsole.Width - 2));
                BattleConsole.Print(0, 22, "Defense".Align(HorizontalAlignment.Center, BattleConsole.Width - 2));
                BattleConsole.Print(0, 24, "Magic Attack".Align(HorizontalAlignment.Center, BattleConsole.Width - 2));
                BattleConsole.Print(0, 26, "Magic Defense".Align(HorizontalAlignment.Center, BattleConsole.Width - 2));
            } else if (selectedMenu == "BattleDone" && battleResult == "LevelDone") {
                BattleConsole.Print(0, 8, "Victory!".Align(HorizontalAlignment.Center, BattleConsole.Width - 2));
                BattleConsole.Print(0, 10, ("You got " + GameLoop.BattleManager.Enemy.ExpGranted + " exp and levelled up!").Align(HorizontalAlignment.Center, BattleConsole.Width - 2));

                BattleConsole.Print(0, 16, ("Vitality  +" + vitChange).Align(HorizontalAlignment.Center, BattleConsole.Width - 2));
                BattleConsole.Print(0, 17, ("Speed  +" + spdChange).Align(HorizontalAlignment.Center, BattleConsole.Width - 2));
                BattleConsole.Print(0, 18, ("Attack  +" + atkChange).Align(HorizontalAlignment.Center, BattleConsole.Width - 2));
                BattleConsole.Print(0, 19, ("Defense  +" + defChange).Align(HorizontalAlignment.Center, BattleConsole.Width - 2));
                BattleConsole.Print(0, 20, ("Magic Attack  +" + matkChange).Align(HorizontalAlignment.Center, BattleConsole.Width - 2));
                BattleConsole.Print(0, 21, ("Magic Defense  +" + mdefChange).Align(HorizontalAlignment.Center, BattleConsole.Width - 2));
                BattleConsole.Print(0, 25, "[Close]".Align(HorizontalAlignment.Center, BattleConsole.Width - 2));
            } else if (selectedMenu == "BattleDone" && battleResult == "Drops") {
                BattleConsole.Print(0, 7, ("The " + GameLoop.BattleManager.Enemy.Name + " dropped items!").Align(HorizontalAlignment.Center, BattleConsole.Width - 2));
                BattleConsole.Print(0, 8, "(click to take)".Align(HorizontalAlignment.Center, BattleConsole.Width - 2));
                for (int i = 0; i < dropTable.Count; i++) {
                    Item item = dropTable[i];
                    if (!item.IsStackable || (item.IsStackable && item.ItemQuantity == 1))
                        BattleConsole.Print(0, 10 + (i), item.Name.Align(HorizontalAlignment.Center, BattleConsole.Width - 2));
                    else
                        BattleConsole.Print(0, 10 + (i), ("(" + item.ItemQuantity + ") " + item.Name).Align(HorizontalAlignment.Center, BattleConsole.Width - 2));
                }

                if (dropTable.Count == 0) {
                    BattleWindow.IsVisible = false;
                    BattleWindow.IsFocused = false;
                    selectedMenu = "None";
                    battleResult = "None";
                    dropTable.Clear();
                }

                BattleConsole.Print(0, 30, "[DONE]".Align(HorizontalAlignment.Center, BattleConsole.Width - 2));
            } else if (selectedMenu == "BattleDone") {
                BattleConsole.Print(0, 10, "You escaped!".Align(HorizontalAlignment.Center, BattleConsole.Width - 2));
                BattleConsole.Print(0, 25, "[Close]".Align(HorizontalAlignment.Center, BattleConsole.Width - 2));
            }
        }

        private void RenderMoves() {
            MoveConsole.Clear();
            MoveConsole.Print(0, 0, "        NAME        | PHY | COST | ACC | POW ");
            MoveConsole.DrawLine(new Point(0, 1), new Point(MoveConsole.Width - 1, 1), (char)196, Color.Orange, Color.Black);

            for (int i = 0; i < GameLoop.World.Player.KnownMoves.Count; i++) {
                Move move = GameLoop.World.moveLibrary[GameLoop.World.Player.KnownMoves[i]];
                string name = move.Name.Align(HorizontalAlignment.Left, 20);
                string phys = move.IsPhysical ? "  Y  " : "  N  ";
                string cost = move.Cost.ToString().Align(HorizontalAlignment.Center, 6);
                string acc = move.Accuracy.ToString().Align(HorizontalAlignment.Center, 5);
                string pow = move.Power.ToString().Align(HorizontalAlignment.Center, 5);

                string full = name + "|" + phys + "|" + cost + "|" + acc + "|" + pow;

                MoveConsole.Print(0, i + 2, full);
            }
        }

        private void CaptureMoveClicks() {
            Point mousePos = new MouseScreenObjectState(MoveConsole, GameHost.Instance.Mouse).CellPosition - new Point (0, 2);
                //.PixelLocationToSurface(12, 12) - new Point(24, 14);
             

            if (mousePos.X >= 0 && mousePos.X <= 47 && mousePos.Y >= 0 && mousePos.Y <= 30) {
                if (GameHost.Instance.Mouse.LeftClicked) {
                    if (GameLoop.World.Player.KnownMoves.Count > mousePos.Y) {
                        moveIndex = GameLoop.World.Player.KnownMoves[mousePos.Y];
                        MoveWindow.IsVisible = false;
                    }
                }
            }
        }



        private void CaptureBattleClicks() {
            if (selectedMenu == "Battle") {
                Point mousePos = new MouseScreenObjectState(BattleConsole, GameHost.Instance.Mouse).CellPosition; 
                if (GameHost.Instance.Mouse.LeftClicked) {
                    if (mousePos.Y == 39 && mousePos.X >= 0 && mousePos.X <= 18) {
                        GameLoop.BattleManager.ResolveTurn("Flee", 0);
                    }

                    if (mousePos.Y == 32) {
                        GameLoop.BattleManager.ResolveTurn("Attack", moveIndex);
                    }

                    if (mousePos.Y == 33) {
                        MoveWindow.IsVisible = true;
                    }

                    if (mousePos.Y == 34) {
                        MessageLog.Add("Tried to open inventory");
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
                        vitChange = GameLoop.rand.Next(5) + 1;
                        spdChange = GameLoop.rand.Next(5) + 1;
                        atkChange = GameLoop.rand.Next(5) + 1;
                        defChange = GameLoop.rand.Next(5) + 1;
                        matkChange = GameLoop.rand.Next(5) + 1;
                        mdefChange = GameLoop.rand.Next(5) + 1;

                        if (mousePos.Y == 16) {
                            vitChange += 3;
                            battleResult = "LevelDone";
                        } else if (mousePos.Y == 18) {
                            spdChange += 3;
                            battleResult = "LevelDone";
                        } else if (mousePos.Y == 20) {
                            atkChange += 3;
                            battleResult = "LevelDone";
                        } else if (mousePos.Y == 22) {
                            defChange += 3;
                            battleResult = "LevelDone";
                        } else if (mousePos.Y == 24) {
                            matkChange += 3;
                            battleResult = "LevelDone";
                        } else if (mousePos.Y == 26) {
                            mdefChange += 3;
                            battleResult = "LevelDone";
                        }

                    } else if (battleResult == "LevelDone") {
                        if (mousePos.Y == 25) {
                            GameLoop.World.Player.Vitality += vitChange;
                            GameLoop.World.Player.Speed += spdChange;
                            GameLoop.World.Player.Attack += atkChange;
                            GameLoop.World.Player.Defense += defChange;
                            GameLoop.World.Player.MagicAttack += matkChange;
                            GameLoop.World.Player.MagicDefense += mdefChange;

                            vitChange = 0;
                            spdChange = 0;
                            atkChange = 0;
                            defChange = 0;
                            matkChange = 0;
                            mdefChange = 0;

                            int oldMaxHP = GameLoop.World.Player.MaxHP;
                            GameLoop.World.Player.RecalculateHP();
                            GameLoop.World.Player.HitPoints += GameLoop.World.Player.MaxHP - oldMaxHP;
                             

                            for (int i = 0; i < GameLoop.BattleManager.Enemy.DropTable.Count; i++) {
                                ItemDrop drop = GameLoop.BattleManager.Enemy.DropTable[i];

                                int roll = GameLoop.rand.Next(drop.DropChance);
                                
                                if (roll == 0) {
                                    Item item = new Item(drop.ItemID);

                                    if (item.IsStackable) {
                                        item.ItemQuantity = GameLoop.rand.Next(drop.DropQuantity) + 1;
                                        dropTable.Add(item);
                                    } else {
                                        int qty = GameLoop.rand.Next(drop.DropQuantity) + 1;

                                        for (int j = 0; j < qty; j++) {
                                            Item itemNonStack = new Item(drop.ItemID);
                                            dropTable.Add(itemNonStack);
                                        }
                                    }
                                } 
                            }


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
                            for (int i = 0; i < GameLoop.BattleManager.Enemy.DropTable.Count; i++) {
                                ItemDrop drop = GameLoop.BattleManager.Enemy.DropTable[i];

                                int roll = GameLoop.rand.Next(drop.DropChance);

                                if (roll == 0) {
                                    Item item = new Item(drop.ItemID);

                                    if (item.IsStackable) {
                                        item.ItemQuantity = GameLoop.rand.Next(drop.DropQuantity) + 1;
                                        dropTable.Add(item);
                                    } else {
                                        int qty = GameLoop.rand.Next(drop.DropQuantity) + 1;

                                        for (int j = 0; j < qty; j++) {
                                            Item itemNonStack = new Item(drop.ItemID);
                                            dropTable.Add(itemNonStack);
                                        }
                                    }
                                } 
                            }

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

            int armorSum = GameLoop.World.Player.Equipment[1].NumericalBonus + GameLoop.World.Player.Equipment[2].NumericalBonus + GameLoop.World.Player.Equipment[3].NumericalBonus + GameLoop.World.Player.Equipment[4].NumericalBonus;

            SidebarConsole.Print(0, 7, "+Dmg:" + GameLoop.World.Player.Equipment[0].NumericalBonus);
            SidebarConsole.Print(0, 8, "  DR:" + armorSum);

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
            SidebarConsole.Print(9, 8, new ColoredString(GameLoop.World.Player.Gold.ToString().Align(HorizontalAlignment.Right, 7), Color.Goldenrod, Color.Black));

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
            } else { // Print non-map editor stuff
                if (GameLoop.World != null && GameLoop.World.DoneInitializing) {

                    SidebarConsole.Print(0, 10, "Lv" + GameLoop.World.Player.Level);
                    SidebarConsole.Print(7, 10, ("XP: " + GameLoop.World.Player.Experience).Align(HorizontalAlignment.Right, 19));
                    SidebarConsole.Print(0, 11, ("To Next: " + (GameLoop.World.Player.ExpToNext - GameLoop.World.Player.Experience)).Align(HorizontalAlignment.Right, 26));


                    SidebarConsole.DrawLine(new Point(0, 12), new Point(25, 12), (char)196, Color.Orange, Color.Black);


                    SidebarConsole.Print(0, 13, "Backpack");
                    int y = 14;

                    for (int i = 0; i < 9; i++) {
                        Item item = GameLoop.World.Player.Inventory[i]; 

                        SidebarConsole.Print(0, y + i, "|");
                        SidebarConsole.Print(1, y + i, item.AsColoredGlyph());
                        if (!item.IsStackable || (item.IsStackable && item.ItemQuantity == 1))
                            SidebarConsole.Print(3, y + i, new ColoredString(item.Name, i == hotbarSelect ? Color.Yellow : item.Name == "(EMPTY)" ? Color.DarkSlateGray : Color.White, Color.TransparentBlack));
                        else
                            SidebarConsole.Print(3, y + i, new ColoredString(("(" + item.ItemQuantity + ") " + item.Name), i == hotbarSelect ? Color.Yellow : item.Name == "(EMPTY)" ? Color.DarkSlateGray : Color.White, Color.TransparentBlack));
                    }

                    y = 26;

                    SidebarConsole.Print(0, 25, "Equipment");

                    for (int i = 0; i < 8; i++) {
                        Item item =  GameLoop.World.Player.Equipment[i];

                        SidebarConsole.Print(0, y + i, "|");
                        SidebarConsole.Print(1, y + i, item.AsColoredGlyph());
                        SidebarConsole.Print(3, y + i, new ColoredString(item.Name, item.Name == "(EMPTY)" ? Color.DarkSlateGray : Color.White, Color.TransparentBlack));
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

            BattleLog = new SadConsole.Console(50, 8);
            BattleWindow.Children.Add(BattleLog);
            BattleLog.Position = new Point(20, 33);


            BattleWindow.Show();
            BattleWindow.IsVisible = false;


            MoveWindow = new Window(47, 32);
            MoveWindow.CanDrag = false;
            MoveWindow.Position = new Point((BattleWindow.Width - MoveWindow.Width) / 2, (BattleWindow.Height - MoveWindow.Height) / 2);

            MoveConsole = new SadConsole.Console(45, 30);
            MoveConsole.Position = new Point(1, 1);
            MoveWindow.Title = "".Align(HorizontalAlignment.Center, 20, (char)196);

            MoveWindow.Children.Add(MoveConsole);
            BattleWindow.Children.Add(MoveWindow);
            MoveWindow.Show();
            MoveWindow.IsVisible = false;
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
    }
}
