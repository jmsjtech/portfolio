﻿using SadRogue.Primitives;
using SadConsole; 
using System;
using Key = SadConsole.Input.Keys;
using SadConsole.UI;
using Color = SadRogue.Primitives.Color;
using LofiHollow.Entities;
using SadConsole.Input;

namespace LofiHollow.UI {
    public class UIManager : ScreenObject {
        public SadConsole.UI.Colors CustomColors;

        public UI_DialogueWindow DialogueWindow;
        public UI_Sidebar Sidebar; 
        public UI_MainMenu MainMenu;
        public UI_Inventory Inventory;
        public UI_Map Map;
        public UI_Skills Skills;
        public UI_Minigame Minigames;
          
        public SadConsole.Console SignConsole;
        public Window SignWindow; 
        public string signText = ""; 

        public Point targetDir = new Point(0, 0); 
        public string targetType = "None";
        public string selectedMenu = "None"; 
        public bool flying = false;

        public bool clientAndConnected = true;
         
        public UIManager() { 
            IsVisible = true;
            IsFocused = true; 
            Parent = GameHost.Instance.Screen;
        }

        public void AddMsg(string msg) { Map.MessageLog.Add(msg); }
        public void AddMsg(ColoredString msg) { Map.MessageLog.Add(msg); }

        public override void Update(TimeSpan timeElapsed) {
            if (GameLoop.NetworkManager != null)
                GameLoop.NetworkManager.discord.RunCallbacks();

            if (clientAndConnected) {
                if (selectedMenu == "MainMenu" || selectedMenu == "CharCreation" || selectedMenu == "LoadFile" || selectedMenu == "ConnectOrHost") {
                    MainMenu.RenderMainMenu();
                    MainMenu.CaptureMainMenuClicks();
                } else {
                    if (GameLoop.World != null && GameLoop.World.DoneInitializing) {
                        Sidebar.RenderSidebar();
                    }

                    if (selectedMenu == "Inventory") {
                        Inventory.RenderInventory();
                        Inventory.InventoryInput();
                    } else if (selectedMenu == "Map Editor") {
                        Sidebar.MapEditorInput();
                    } else if (selectedMenu == "Skills") {
                        Skills.RenderSkills();
                        Skills.SkillInput();
                    } else if (selectedMenu == "Minigame") {
                        Minigames.RenderMinigame();
                        Minigames.MinigameInput();
                    } else {
                        if (selectedMenu != "Dialogue") {
                            if (selectedMenu == "Sign")
                                RenderSign();
                            
                             CheckKeyboard();
                            Sidebar.SidebarInput();
                            Map.UpdateNPCs();
                        } else {
                            DialogueWindow.RenderDialogue();
                            DialogueWindow.CaptureDialogueClicks();
                        }
                    }

                    Map.RenderOverlays();

                    CheckFall();
                }
            }

            if (GameLoop.NetworkManager != null && GameLoop.NetworkManager.lobbyManager != null)
                GameLoop.NetworkManager.lobbyManager.FlushNetwork();
            
            base.Update(timeElapsed);
        } 

        public void Init() {
            SetupCustomColors();

            Map = new UI_Map(72, 42);
            Sidebar = new UI_Sidebar(28, GameLoop.GameHeight, ""); 
            Inventory = new UI_Inventory(GameLoop.GameWidth / 2, GameLoop.GameHeight / 2, "");
            CreateSignWindow((GameLoop.MapWidth / 2) - 1, GameLoop.MapHeight / 2, "");

            DialogueWindow = new UI_DialogueWindow(72, 42, ""); 
            MainMenu = new UI_MainMenu();
            Skills = new UI_Skills(72, 42, "[SKILLS]");
            Minigames = new UI_Minigame(72, 42, "");
             
            UseMouse = true;
            selectedMenu = "MainMenu";
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
                        Map.UpdateVision();
                    }
                    if (GameHost.Instance.Keyboard.IsKeyDown(Key.S)) { 
                        GameLoop.CommandManager.MoveActorBy(GameLoop.World.Player, new Point(0, 1));
                        Map.UpdateVision();
                    }
                    if (GameHost.Instance.Keyboard.IsKeyDown(Key.A)) { 
                        GameLoop.CommandManager.MoveActorBy(GameLoop.World.Player, new Point(-1, 0));
                        Map.UpdateVision();
                    }
                    if (GameHost.Instance.Keyboard.IsKeyDown(Key.D)) {
                        GameLoop.CommandManager.MoveActorBy(GameLoop.World.Player, new Point(1, 0));
                        Map.UpdateVision();
                    }
                    if (GameHost.Instance.Keyboard.IsKeyDown(Key.LeftShift) && GameHost.Instance.Keyboard.IsKeyPressed(Key.OemPeriod)) {
                        if (GameLoop.World.maps[GameLoop.World.Player.MapPos].GetTile(GameLoop.World.Player.Position).Name == "Down Stairs") {
                            GameLoop.CommandManager.MoveActorTo(GameLoop.World.Player, GameLoop.World.Player.Position, GameLoop.World.Player.MapPos + new Point3D(0, 0, -1));
                            Map.UpdateVision();
                        }
                    }

                    if (GameHost.Instance.Keyboard.IsKeyDown(Key.LeftShift) && GameHost.Instance.Keyboard.IsKeyPressed(Key.OemComma)) {
                        if (GameLoop.World.maps[GameLoop.World.Player.MapPos].GetTile(GameLoop.World.Player.Position).Name == "Up Stairs") {
                            GameLoop.CommandManager.MoveActorTo(GameLoop.World.Player, GameLoop.World.Player.Position, GameLoop.World.Player.MapPos + new Point3D(0, 0, 1));
                            Map.UpdateVision();
                        }
                    }

                    

                    if (GameHost.Instance.Keyboard.IsKeyReleased(Key.I)) {
                        Inventory.ToggleInventory(); 
                    }

                    if (GameHost.Instance.Keyboard.IsKeyReleased(Key.K)) {
                        Skills.Toggle();
                    }

                    if (GameHost.Instance.Keyboard.IsKeyReleased(Key.G)) {
                        GameLoop.CommandManager.PickupItem(GameLoop.World.Player);
                    }
                }

                if (GameHost.Instance.Keyboard.IsKeyReleased(Key.F9)) {
                    GameLoop.World.SaveMapToFile(GameLoop.World.maps[GameLoop.World.Player.MapPos], GameLoop.World.Player.MapPos);
                }

                if (GameHost.Instance.Keyboard.IsKeyReleased(Key.F4)) {
                    Item rod = new Item(18);
                    GameLoop.CommandManager.AddItemToInv(GameLoop.World.Player, rod);
                }

                if (GameHost.Instance.Keyboard.IsKeyReleased(Key.F5)) {
                    Map.LimitedVision = !Map.LimitedVision;

                    if (!Map.LimitedVision) {
                        for (int i = 0; i < GameLoop.World.maps[GameLoop.World.Player.MapPos].Tiles.Length; i++) {
                            GameLoop.World.maps[GameLoop.World.Player.MapPos].Tiles[i].Unshade();
                            GameLoop.World.maps[GameLoop.World.Player.MapPos].Tiles[i].IsVisible = true;
                        }
                    } else {
                        Map.UpdateVision();
                    }
                } 

                if (GameHost.Instance.Keyboard.IsKeyReleased(Key.F3)) {
                    GameLoop.World.SavePlayer(); 
                }

                if (GameHost.Instance.Keyboard.IsKeyReleased(Key.F2)) {
                    AddMsg(GameLoop.World.Player.MapPos.ToString() + " | " + GameLoop.World.Player.Position.ToString());
                }
            } else if (selectedMenu == "Sign") {
                if (GameHost.Instance.Keyboard.HasKeysPressed) {
                    selectedMenu = "None";
                    SignWindow.IsVisible = false;
                    Map.MapConsole.IsFocused = true;
                }
            }

            if (GameHost.Instance.Keyboard.IsKeyReleased(Key.F)) {
                flying = !flying;
            }

            if (selectedMenu != "Map Editor" && selectedMenu != "Targeting") {
                
                if (GameHost.Instance.Keyboard.IsKeyReleased(Key.C)) {
                    selectedMenu = "Targeting";
                    targetType = "Door";
                    AddMsg("Close door where?");
                }
            } else if (selectedMenu == "Targeting") { 
                if (GameHost.Instance.Keyboard.IsKeyReleased(Key.S)) { targetDir = new Point(0, 1); } 
                if (GameHost.Instance.Keyboard.IsKeyReleased(Key.A)) { targetDir = new Point(-1, 0); } 
                if (GameHost.Instance.Keyboard.IsKeyReleased(Key.D)) { targetDir = new Point(1, 0); } 
                if (GameHost.Instance.Keyboard.IsKeyReleased(Key.W)) { targetDir = new Point(0, -1); } 

                if (targetType == "Door" && targetDir != new Point(0, 0)) { 
                    GameLoop.World.maps[GameLoop.World.Player.MapPos].ToggleDoor(GameLoop.World.Player.Position + targetDir);
                    Map.UpdateVision();
                    targetType = "Done";
                } else if (targetType != "Door") {
                    targetType = "None";
                    targetDir = new Point(0, 0);
                    selectedMenu = "None";
                }
            } else if (selectedMenu == "Map Editor") {
                Sidebar.MapEditorInput();
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

        private void SetupCustomColors() {
            CustomColors = SadConsole.UI.Colors.CreateAnsi(); 
            CustomColors.ControlHostBackground = new AdjustableColor(Color.Black, "Black");  
            CustomColors.Lines = new AdjustableColor(Color.White, "White");  
            CustomColors.Title = new AdjustableColor(Color.White, "White");

            CustomColors.RebuildAppearances(); 
            SadConsole.UI.Themes.Library.Default.Colors = CustomColors; 
        }

        public void SignText(Point locInMap, Point3D MapLoc) {
            selectedMenu = "Sign";
            SignWindow.IsVisible = true;
            SignWindow.IsFocused = true;

            if (MapLoc == new Point3D(0, 1, 0)) { // Town Center
                if (locInMap == new Point(31, 9)) { signText = "Tom's Bar"; }
                else if (locInMap == new Point(21, 9)) { signText = "Blacksmith||Weekdays 9am to 5pm|Closed Weekends"; } 
                else if (locInMap == new Point(12, 20)) { signText = "Jasper's General Goods"; } 
                else if (locInMap == new Point(21, 30)) { signText = "Adventure Guild"; }
                else if (locInMap == new Point(29, 30)) { signText = "Emerose's Apothecary"; } 
                else if (locInMap == new Point(12, 24)) { signText = "Library"; }
                else if (locInMap == new Point(6, 16)) { signText = "Zephyr's Textiles"; }
                else if (locInMap == new Point(37, 7)) { signText = "Indigo -- Workshop"; }
                else if (locInMap == new Point(49, 9)) { signText = "Indigo -- Residence"; }
                else {
                    AddMsg("Sign at (" + locInMap.X + "," + locInMap.Y + ") has no text.");
                }
            } 

            else if (MapLoc == new Point3D(1, 1, 0)) {
                if (locInMap == new Point(5, 24)) { signText = "Sapphire's Bakery"; } 
                else if (locInMap == new Point(8, 20)) { signText = "Saffron's Farm Supply"; }
                else if (locInMap == new Point(10, 28)) { signText = "Cobalt's House"; }
                else if (locInMap == new Point(16, 28)) { signText = "Tak's House"; }
                else if (locInMap == new Point(21, 24)) { signText = "Clinic"; }
                else if (locInMap == new Point(28, 25)) { signText = "Courier's Guild"; }
                else if (locInMap == new Point(59, 24)) { signText = "Merchant's Guild"; }
                else if (locInMap == new Point(43, 15)) { signText = "Town Hall"; }
                else { AddMsg("Sign at (" + locInMap.X + "," + locInMap.Y + ") has no text."); }
            }
            
            else if (MapLoc == new Point3D(-3, 1, 0)) { signText = "North -- Lake|West -- Mountain Cave|East -- Noonbreeze"; }
            else if (MapLoc == new Point3D(-3, -1, 0)) { signText = "Fisherman's Cabin"; } 
            else if (MapLoc == new Point3D(-5, 1, 0)) { signText = "Mountain Tunnel||Under Construction"; } 
            else { AddMsg("Sign at (" + locInMap.X + "," + locInMap.Y + "), map (" + MapLoc.X + "," + MapLoc.Y + "," + MapLoc.Z + ")"); }
        }

        public void CheckFall() {
            if (GameLoop.World.maps[GameLoop.World.Player.MapPos].GetTile(GameLoop.World.Player.Position).Name == "Space" && !flying) {
                GameLoop.CommandManager.MoveActorTo(GameLoop.World.Player, GameLoop.World.Player.Position, GameLoop.World.Player.MapPos + new Point3D(0, 0, -1));
                AddMsg("You fell down!");
            }
        }
    }
}
