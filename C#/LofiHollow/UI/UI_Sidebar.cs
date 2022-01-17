using LofiHollow.Entities;
using Newtonsoft.Json;
using SadConsole;
using SadConsole.Input;
using SadConsole.UI;
using SadRogue.Primitives; 
using System.Collections.Generic;
using System.Linq;
using Key = SadConsole.Input.Keys;

namespace LofiHollow.UI {
    public class UI_Sidebar {
        public Window SidebarWindow;
        public SadConsole.Console SidebarConsole;

        public int tileIndex = 0;
        public int monIndex = 0;
        public int hotbarSelect = 0;
        public int ChargeBar = 0;
        public Point LureSpot;

        public FishingLure LocalLure;

        public Point tileSelected = new Point(0, 0);

        public Dictionary<Point3D, MinimapTile> minimap = new Dictionary<Point3D, MinimapTile>();



        public UI_Sidebar(int width, int height, string title) {
            SidebarConsole = new SadConsole.Console(28, GameLoop.GameHeight);
            SidebarWindow = new Window(width, height);
            SidebarWindow.CanDrag = false;
            SidebarWindow.Position = new Point(72, 0);

            int sidebarConsoleWidth = width - 2;
            int sidebarConsoleHeight = height - 2;

            SidebarConsole.Position = new Point(1, 1);
            SidebarWindow.Title = title.Align(HorizontalAlignment.Center, sidebarConsoleWidth, (char)196);


            SidebarWindow.Children.Add(SidebarConsole);
            GameLoop.UIManager.Children.Add(SidebarWindow);

            SidebarWindow.Show();

            SidebarWindow.IsVisible = false;
        }

        public void SidebarInput() { 
            if (GameHost.Instance.Keyboard.IsKeyReleased(Key.F1)) {
                if (GameLoop.UIManager.selectedMenu == "Map Editor") {
                    GameLoop.UIManager.selectedMenu = "None";
                } else {
                    GameLoop.UIManager.selectedMenu = "Map Editor";
                }
            }

            if (GameHost.Instance.Mouse.ScrollWheelValueChange > 0) {
                if (hotbarSelect + 1 < GameLoop.World.Player.Inventory.Length)
                    hotbarSelect++;
            } else if (GameHost.Instance.Mouse.ScrollWheelValueChange < 0) {
                if (hotbarSelect > 0)
                    hotbarSelect--;
            }

            Point mousePos = new MouseScreenObjectState(SidebarConsole, GameHost.Instance.Mouse).CellPosition - new Point(0, 33);
            if (mousePos.X > 0) { // Clicked in Sidebar
                if (GameHost.Instance.Mouse.LeftClicked) {
                    int slot = mousePos.Y;
                    if (slot >= 0 && slot <= 15)
                        GameLoop.CommandManager.UnequipItem(GameLoop.World.Player, slot);
                }
            }


            if (GameHost.Instance.Keyboard.IsKeyReleased(Key.F6)) {
                for (int i = 0; i < GameLoop.World.maps[GameLoop.World.Player.MapPos].Tiles.Length; i++) {
                    TileBase tile = GameLoop.World.maps[GameLoop.World.Player.MapPos].Tiles[i];
                    if (tile.Name == "Dirt") {
                        tile.ForegroundR = 152;
                        tile.ForegroundG = 118;
                        tile.ForegroundB = 84;
                        tile.UpdateAppearance();
                    }
                }
            }

            if (GameHost.Instance.Mouse.LeftClicked) {
                if (GameLoop.World.Player.Inventory[hotbarSelect].ItemID == 0) {
                    Point mapPos = new MouseScreenObjectState(GameLoop.UIManager.Map.MapConsole, GameHost.Instance.Mouse).CellPosition;

                    int distance = GoRogue.Lines.Get(new GoRogue.Coord(mapPos.X, mapPos.Y), new GoRogue.Coord(GameLoop.World.Player.Position.X, GameLoop.World.Player.Position.Y)).Count();
                    if (distance < 5) {
                        if (mapPos.X >= 0 && mapPos.X <= GameLoop.MapWidth && mapPos.Y >= 0 && mapPos.Y <= GameLoop.MapHeight) {
                            TileBase tile = GameLoop.World.maps[GameLoop.World.Player.MapPos].GetTile(mapPos);
                            if (tile.Plant != null) {
                                tile.Plant.Harvest(GameLoop.World.Player);
                                if (tile.Plant.CurrentStage == -1) {
                                    TileBase tilled = new TileBase(8);
                                    tilled.Name = "Tilled Dirt";
                                    tilled.TileGlyph = 34;
                                    tilled.UpdateAppearance();

                                    GameLoop.World.maps[GameLoop.World.Player.MapPos].SetTile(mapPos, tilled);
                                    GameLoop.UIManager.Map.MapConsole.SetEffect(mapPos.X, mapPos.X, null);
                                    tile.UpdateAppearance();

                                    if (GameLoop.NetworkManager != null && GameLoop.NetworkManager.lobbyManager != null) {
                                        string msg = "updateTile;" + mapPos.X + ";" + mapPos.Y + ";" + GameLoop.World.Player.MapPos.X + ";" +
                                            GameLoop.World.Player.MapPos.Y + ";" + GameLoop.World.Player.MapPos.Z + ";" + JsonConvert.SerializeObject(tilled, Formatting.Indented);
                                        GameLoop.NetworkManager.BroadcastMsg(msg);
                                    }
                                } else {
                                    tile.UpdateAppearance();

                                    if (GameLoop.NetworkManager != null && GameLoop.NetworkManager.lobbyManager != null) {
                                        string msg = "updateTile;" + mapPos.X + ";" + mapPos.Y + ";" + GameLoop.World.Player.MapPos.X + ";" +
                                            GameLoop.World.Player.MapPos.Y + ";" + GameLoop.World.Player.MapPos.Z + ";" + JsonConvert.SerializeObject(tile, Formatting.Indented);
                                        GameLoop.NetworkManager.BroadcastMsg(msg);
                                    }
                                }
                            }
                        }
                    }
                } else { 
                    int tempCat = GameLoop.World.Player.Inventory[hotbarSelect].ItemCategory;

                    if (tempCat == 4 || tempCat == 9) {
                        if (GameLoop.World.Player.MapPos == new Point3D(-1, 0, 0) && !GameLoop.CheckFlag("farm")) { 
                                GameLoop.UIManager.AddMsg(new ColoredString("You need to buy this land from the town hall first.", Color.Red, Color.Black));
                        } else if (GameLoop.World.Player.MapPos != new Point3D(-1, 0, 0)) {
                            GameLoop.UIManager.AddMsg(new ColoredString("You probably shouldn't do that here.", Color.Red, Color.Black));
                        }
                    }
                }
            }

           
            if (GameHost.Instance.Mouse.LeftButtonDown) {
                if (GameLoop.World.Player.Inventory[hotbarSelect].ItemID != 0) {
                    if (GameLoop.World.Player.Inventory[hotbarSelect].ItemCategory == 7) { // Fishing rod
                        if (ChargeBar < 100) {
                            ChargeBar++;
                        }
                    }

                    if (GameLoop.World.Player.Inventory[hotbarSelect].ItemCategory == 4) { // Clicked with a hoe
                        Point mapPos = new MouseScreenObjectState(GameLoop.UIManager.Map.MapConsole, GameHost.Instance.Mouse).CellPosition;

                        int distance = GoRogue.Lines.Get(new GoRogue.Coord(mapPos.X, mapPos.Y), new GoRogue.Coord(GameLoop.World.Player.Position.X, GameLoop.World.Player.Position.Y)).Count();
                        if (distance < 5) {
                            if (mapPos.X >= 0 && mapPos.X <= GameLoop.MapWidth && mapPos.Y >= 0 && mapPos.Y <= GameLoop.MapHeight) {
                                TileBase tile = GameLoop.World.maps[GameLoop.World.Player.MapPos].GetTile(mapPos);
                                if (tile.Name == "Dirt") {
                                    if (GameLoop.World.Player.MapPos == new Point3D(-1, 0, 0) && GameLoop.CheckFlag("farm")) {
                                        tile.Name = "Tilled Dirt";
                                        tile.TileGlyph = 34;
                                        tile.UpdateAppearance();

                                        if (GameLoop.NetworkManager != null && GameLoop.NetworkManager.lobbyManager != null) {
                                            string msg = "updateTile;" + mapPos.X + ";" + mapPos.Y + ";" + GameLoop.World.Player.MapPos.X + ";" +
                                                GameLoop.World.Player.MapPos.Y + ";" + GameLoop.World.Player.MapPos.Z + ";" + JsonConvert.SerializeObject(tile, Formatting.Indented);
                                            GameLoop.NetworkManager.BroadcastMsg(msg);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (GameLoop.World.Player.Inventory[hotbarSelect].ItemCategory == 1) { // Clicked with a watering can
                        Point mapPos = new MouseScreenObjectState(GameLoop.UIManager.Map.MapConsole, GameHost.Instance.Mouse).CellPosition;

                        int distance = GoRogue.Lines.Get(new GoRogue.Coord(mapPos.X, mapPos.Y), new GoRogue.Coord(GameLoop.World.Player.Position.X, GameLoop.World.Player.Position.Y)).Count();
                        if (distance < 5) {
                            if (mapPos.X >= 0 && mapPos.X <= GameLoop.MapWidth && mapPos.Y >= 0 && mapPos.Y <= GameLoop.MapHeight) {
                                TileBase tile = GameLoop.World.maps[GameLoop.World.Player.MapPos].GetTile(mapPos);
                                if (tile.Name == "Well" || tile.Name.ToLower().Contains("water")) {
                                    GameLoop.World.Player.Inventory[hotbarSelect].Durability = GameLoop.World.Player.Inventory[hotbarSelect].MaxDurability;
                                } else {
                                    if (tile.Plant != null) {
                                        if (!tile.Plant.WateredToday && GameLoop.World.Player.Inventory[hotbarSelect].Durability > 0) {
                                            tile.Plant.WateredToday = true;
                                            GameLoop.World.Player.Inventory[hotbarSelect].Durability--;
                                            GameLoop.UIManager.Map.MapConsole.SetEffect(mapPos.X, mapPos.Y, null);
                                            tile.UpdateAppearance();
                                            GameLoop.UIManager.Map.MapConsole.SetCellAppearance(mapPos.X, mapPos.Y, tile);
                                        }

                                        if (GameLoop.NetworkManager != null && GameLoop.NetworkManager.lobbyManager != null) {
                                            string msg = "updateTile;" + mapPos.X + ";" + mapPos.Y + ";" + GameLoop.World.Player.MapPos.X + ";" +
                                                GameLoop.World.Player.MapPos.Y + ";" + GameLoop.World.Player.MapPos.Z + ";" + JsonConvert.SerializeObject(tile, Formatting.Indented);
                                            GameLoop.NetworkManager.BroadcastMsg(msg);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (GameLoop.World.Player.Inventory[hotbarSelect].Plant != null) { // Clicked with a seed
                        Point mapPos = new MouseScreenObjectState(GameLoop.UIManager.Map.MapConsole, GameHost.Instance.Mouse).CellPosition;
                          
                        int distance = GoRogue.Lines.Get(new GoRogue.Coord(mapPos.X, mapPos.Y), new GoRogue.Coord(GameLoop.World.Player.Position.X, GameLoop.World.Player.Position.Y)).Count();
                        if (distance < 5) {
                            if (mapPos.X >= 0 && mapPos.X <= GameLoop.MapWidth && mapPos.Y >= 0 && mapPos.Y <= GameLoop.MapHeight) {
                                TileBase tile = GameLoop.World.maps[GameLoop.World.Player.MapPos].GetTile(mapPos);
                                if (tile.Name == "Tilled Dirt") {
                                    if (GameLoop.World.Player.MapPos == new Point3D(-1, 0, 0) && GameLoop.CheckFlag("farm")) {
                                        tile.Plant = new Plant(GameLoop.World.Player.Inventory[hotbarSelect].Plant);
                                        tile.Name = tile.Plant.ProduceName + " Plant";
                                        tile.UpdateAppearance();
                                        GameLoop.UIManager.Map.MapConsole.SetEffect(mapPos.X, mapPos.Y, new CustomBlink(173, Color.Blue));
                                        GameLoop.CommandManager.RemoveOneItem(GameLoop.World.Player, hotbarSelect); 

                                        if (GameLoop.NetworkManager != null && GameLoop.NetworkManager.lobbyManager != null) {
                                            string msg = "updateTile;" + mapPos.X + ";" + mapPos.Y + ";" + GameLoop.World.Player.MapPos.X + ";" +
                                                GameLoop.World.Player.MapPos.Y + ";" + GameLoop.World.Player.MapPos.Z + ";" + JsonConvert.SerializeObject(tile, Formatting.Indented);
                                            GameLoop.NetworkManager.BroadcastMsg(msg);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (!GameHost.Instance.Mouse.LeftButtonDown) {
                if (ChargeBar != 0) {
                    if (LocalLure != null) {
                        if (GameLoop.World.maps[GameLoop.World.Player.MapPos].GetEntityAt<Item>(LocalLure.Position) != null) {
                            Item item = GameLoop.World.maps[GameLoop.World.Player.MapPos].GetEntityAt<Item>(LocalLure.Position);
                            GameLoop.UIManager.AddMsg("Snagged the " + item.Name + "!");
                            GameLoop.CommandManager.AddItemToInv(GameLoop.World.Player, item);
                            //  GameLoop.CommandManager.DestroyItem(item);
                            item.Position = new Point(-1, -2);
                        } else if (LocalLure.FishOnHook) {
                            GameLoop.UIManager.Minigames.InitiateFishing(GameLoop.World.Player.Clock.GetSeason(), 
                                GameLoop.World.maps[GameLoop.World.Player.MapPos].MinimapTile.name, 
                                GameLoop.World.Player.Clock.GetCurrentTime(),
                                GameLoop.World.Player.Skills["Fishing"].Level);
                        } else { 
                            GameLoop.UIManager.Map.MapConsole.ClearDecorators(LocalLure.Position.X, LocalLure.Position.Y, 1);
                            LocalLure.Position = new Point(-1, -1);
                        }
                    }
                    if (ChargeBar >= 10) {
                        if (GameLoop.World.Player.Inventory[hotbarSelect].ItemID != 0) {
                            if (GameLoop.World.Player.Inventory[hotbarSelect].ItemCategory == 7) { // Fishing rod
                                Point target = LureSpot * new Point(2, 2);
                                int xDist = (int)((double)((target.X / 10) * (ChargeBar / 10)));
                                int yDist = (int)((double)((target.Y / 10) * (ChargeBar / 10)));

                                LocalLure = new FishingLure();
                                LocalLure.Position = GameLoop.World.Player.Position;
                                LocalLure.SetVelocity(xDist, yDist); 
                                GameLoop.UIManager.Map.EntityRenderer.Add(LocalLure);
                            }
                        }
                    }

                    ChargeBar = 0;
                }
            } 
        }

        public void MapEditorInput() {
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

            if (GameHost.Instance.Keyboard.IsKeyReleased(Key.F1)) {
                if (GameLoop.UIManager.selectedMenu == "Map Editor") {
                    GameLoop.UIManager.selectedMenu = "None";
                } else {
                    GameLoop.UIManager.selectedMenu = "Map Editor";
                }
            }

            if (GameHost.Instance.Keyboard.IsKeyReleased(Key.F9)) {
                GameLoop.World.SaveMapToFile(GameLoop.World.maps[GameLoop.World.Player.MapPos], GameLoop.World.Player.MapPos);
            }

            Point sidebarMouse = new MouseScreenObjectState(SidebarConsole, GameHost.Instance.Mouse).CellPosition;
            TileBase selectedTile = GameLoop.World.maps[GameLoop.World.Player.MapPos].GetTile(tileSelected);

            if (selectedTile.Lock != null) {
                if (selectedTile.Lock.LockTime > 1440)
                    selectedTile.Lock.LockTime = 1440;

                if (selectedTile.Lock.UnlockTime > 1440)
                    selectedTile.Lock.UnlockTime = 1440;
            }


            if (GameHost.Instance.Mouse.ScrollWheelValueChange < 0) {
                if (sidebarMouse.Y == 19) {
                    if (monIndex < GameLoop.World.monsterLibrary.Count)
                        monIndex++;
                    else
                        monIndex = 0;
                } else if (sidebarMouse.Y == 26) {
                    if (tileIndex < GameLoop.World.tileLibrary.Count)
                        tileIndex++;
                    else
                        tileIndex = 0;
                } else if (sidebarMouse.Y == 11) {
                    thisMap.ch++;
                } else if (sidebarMouse.Y == 38) {
                    if (selectedTile.Lock != null && selectedTile.Lock.RelationshipUnlock < 100)
                        selectedTile.Lock.RelationshipUnlock++;
                } else if (sidebarMouse.Y == 39) {
                    if (selectedTile.Lock != null)
                        selectedTile.Lock.MissionUnlock++;
                } else if (sidebarMouse.Y == 37) {
                    if (selectedTile.Lock != null) {
                        selectedTile.Lock.OwnerID++;
                        selectedTile.Lock.UpdateOwner();
                    }
                } else if (sidebarMouse.Y == 41) {
                    if (selectedTile.Lock != null)
                        if (GameHost.Instance.Keyboard.IsKeyDown(Key.LeftShift))
                            selectedTile.Lock.UnlockTime += 60;
                        else
                            selectedTile.Lock.UnlockTime++;
                } else if (sidebarMouse.Y == 42) {
                    if (selectedTile.Lock != null)
                        if (GameHost.Instance.Keyboard.IsKeyDown(Key.LeftShift))
                            selectedTile.Lock.LockTime += 60;
                        else
                            selectedTile.Lock.LockTime++;
                } else if (sidebarMouse.Y == 43) {
                    if (selectedTile.Lock != null)
                        selectedTile.Lock.KeySubID++;
                }
            } else if (GameHost.Instance.Mouse.ScrollWheelValueChange > 0) {
                if (sidebarMouse.Y == 19) {
                    if (monIndex > 0)
                        monIndex--;
                    else
                        monIndex = GameLoop.World.monsterLibrary.Count;
                } else if (sidebarMouse.Y == 26) {
                    if (tileIndex > 0)
                        tileIndex--;
                    else
                        tileIndex = GameLoop.World.tileLibrary.Count;
                } else if (sidebarMouse.Y == 11) {
                    if (thisMap.ch > 0)
                        thisMap.ch--;
                } else if (sidebarMouse.Y == 38) {
                    if (selectedTile.Lock != null && selectedTile.Lock.RelationshipUnlock > 0)
                        selectedTile.Lock.RelationshipUnlock--;
                } else if (sidebarMouse.Y == 39) {
                    if (selectedTile.Lock != null && selectedTile.Lock.MissionUnlock > -1)
                        selectedTile.Lock.MissionUnlock--;
                } else if (sidebarMouse.Y == 37) {
                    if (selectedTile.Lock != null && selectedTile.Lock.OwnerID > -1) {
                        selectedTile.Lock.OwnerID--;
                        selectedTile.Lock.UpdateOwner();
                    }
                } else if (sidebarMouse.Y == 41) {
                    if (selectedTile.Lock != null && selectedTile.Lock.UnlockTime > 0)
                        if (GameHost.Instance.Keyboard.IsKeyDown(Key.LeftShift))
                            selectedTile.Lock.UnlockTime -= 60;
                        else
                            selectedTile.Lock.UnlockTime--;
                } else if (sidebarMouse.Y == 42) {
                    if (selectedTile.Lock != null && selectedTile.Lock.LockTime > 0)
                        if (GameHost.Instance.Keyboard.IsKeyDown(Key.LeftShift))
                            selectedTile.Lock.LockTime -= 60;
                        else
                            selectedTile.Lock.LockTime--;
                } else if (sidebarMouse.Y == 43) {
                    if (selectedTile.Lock != null && selectedTile.Lock.KeySubID > -1)
                        selectedTile.Lock.KeySubID--;
                }
            }

            /*
                        SidebarConsole.Print(0, 37, "Owner: " + tile.Lock.Owner);
                        SidebarConsole.Print(0, 38, "Rel Unlock: " + tile.Lock.RelationshipUnlock);
                        SidebarConsole.Print(0, 39, "Mission Unlock: " + tile.Lock.MissionUnlock);
                        SidebarConsole.Print(0, 40, "Always Locked: " + tile.Lock.AlwaysLocked);
                        SidebarConsole.Print(0, 41, "Unlocks at: " + tile.Lock.UnlockTime);
                        SidebarConsole.Print(0, 42, "Locks at: " + tile.Lock.LockTime);
                        SidebarConsole.Print(0, 43, "Key SubID: " + tile.Lock.KeySubID);
            */
             
            if (GameHost.Instance.Keyboard.IsKeyReleased(Key.F4)) {
                GameLoop.World.maps[GameLoop.World.Player.MapPos].MonsterWeights.Clear();
                GameLoop.World.maps[GameLoop.World.Player.MapPos].MonsterWeights.Add(1, 45);
                GameLoop.World.maps[GameLoop.World.Player.MapPos].MonsterWeights.Add(2, 45);
                GameLoop.World.maps[GameLoop.World.Player.MapPos].MonsterWeights.Add(3, 10);
                GameLoop.World.maps[GameLoop.World.Player.MapPos].MinimumMonsters = 5;
                GameLoop.World.maps[GameLoop.World.Player.MapPos].MaximumMonsters = 10;

                GameLoop.World.SaveMapToFile(GameLoop.World.maps[GameLoop.World.Player.MapPos], GameLoop.World.Player.MapPos);
            }

            Point mousePos = GameHost.Instance.Mouse.ScreenPosition.PixelLocationToSurface(12, 12) - new Point(1, 1);
            if (mousePos.X < 72 && mousePos.Y < 41 && mousePos.X >= 0 && mousePos.Y >= 0) {
                if (GameHost.Instance.Mouse.LeftButtonDown) {
                    if (GameLoop.World.tileLibrary.ContainsKey(tileIndex)) {
                        TileBase tile = new TileBase(tileIndex);
                        tile.TileID = tileIndex;
                        if (mousePos.ToIndex(GameLoop.MapWidth) < GameLoop.World.maps[GameLoop.World.Player.MapPos].Tiles.Length) { 
                            GameLoop.World.maps[GameLoop.World.Player.MapPos].Tiles[mousePos.ToIndex(GameLoop.MapWidth)] = tile;

                            if (GameLoop.NetworkManager != null && GameLoop.NetworkManager.lobbyManager != null) {
                                string msg = "updateTile;" + mousePos.X + ";" + mousePos.Y + ";" + GameLoop.World.Player.MapPos.X + ";" + 
                                    GameLoop.World.Player.MapPos.Y + ";" + GameLoop.World.Player.MapPos.Z + ";" + JsonConvert.SerializeObject(tile, Formatting.Indented);
                                GameLoop.NetworkManager.BroadcastMsg(msg); 
                            } 
                        }
                    }
                }

                if (GameHost.Instance.Mouse.RightClicked) {
                    tileSelected = mousePos;
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

                    if (GameLoop.World.maps[GameLoop.World.Player.MapPos].MonsterWeights.ContainsKey(monIndex)) {
                        if (mousePos == new Point(8, 20)) { 
                            GameLoop.World.maps[GameLoop.World.Player.MapPos].MonsterWeights[monIndex]--;
                            if (GameLoop.World.maps[GameLoop.World.Player.MapPos].MonsterWeights[monIndex] == 0) {
                                GameLoop.World.maps[GameLoop.World.Player.MapPos].MonsterWeights.Remove(monIndex);
                            }
                        }
                        if (mousePos == new Point(14, 20)) { GameLoop.World.maps[GameLoop.World.Player.MapPos].MonsterWeights[monIndex]++; }
                    } else {
                        if (mousePos.Y == 20) {
                            GameLoop.World.maps[GameLoop.World.Player.MapPos].MonsterWeights.Add(monIndex, 1);
                        }
                    }
                }

                if (GameHost.Instance.Mouse.RightClicked) {
                    if (mousePos == new Point(8, 14)) { thisMap.fg = new Color(thisMap.fg.R - 1, thisMap.fg.G, thisMap.fg.B); }
                    if (mousePos == new Point(14, 14)) { thisMap.fg = new Color(thisMap.fg.R + 1, thisMap.fg.G, thisMap.fg.B); }
                    if (mousePos == new Point(8, 15)) { thisMap.fg = new Color(thisMap.fg.R, thisMap.fg.G - 1, thisMap.fg.B); }
                    if (mousePos == new Point(14, 15)) { thisMap.fg = new Color(thisMap.fg.R, thisMap.fg.G + 1, thisMap.fg.B); }
                    if (mousePos == new Point(8, 16)) { thisMap.fg = new Color(thisMap.fg.R, thisMap.fg.G, thisMap.fg.B - 1); }
                    if (mousePos == new Point(14, 16)) { thisMap.fg = new Color(thisMap.fg.R, thisMap.fg.G, thisMap.fg.B + 1); }
                    if (mousePos == new Point(14, 17)) { GameLoop.World.maps[GameLoop.World.Player.MapPos].MinimumMonsters--; }
                    if (mousePos == new Point(20, 17)) { GameLoop.World.maps[GameLoop.World.Player.MapPos].MinimumMonsters++; }
                    if (mousePos == new Point(14, 18)) { GameLoop.World.maps[GameLoop.World.Player.MapPos].MaximumMonsters--; }
                    if (mousePos == new Point(20, 18)) { GameLoop.World.maps[GameLoop.World.Player.MapPos].MaximumMonsters++; }

                    if (GameLoop.World.maps[GameLoop.World.Player.MapPos].MonsterWeights.ContainsKey(monIndex)) {
                        if (mousePos == new Point(8, 20)) { 
                            GameLoop.World.maps[GameLoop.World.Player.MapPos].MonsterWeights[monIndex]--; 
                            if (GameLoop.World.maps[GameLoop.World.Player.MapPos].MonsterWeights[monIndex] == 0) {
                                GameLoop.World.maps[GameLoop.World.Player.MapPos].MonsterWeights.Remove(monIndex);
                            }
                        }
                        if (mousePos == new Point(14, 20)) { GameLoop.World.maps[GameLoop.World.Player.MapPos].MonsterWeights[monIndex]++; }
                    }
                   

                    if (mousePos.Y == 18) { mapData.PlayerCanBuild = !mapData.PlayerCanBuild; }
                     
                }
            }
        }

        public void RenderSidebar() {
            Point mousePos = new MouseScreenObjectState(SidebarConsole, GameHost.Instance.Mouse).CellPosition;
            SidebarConsole.Clear();

            if (LocalLure != null)
                LocalLure.Update();

            string timeHour = GameLoop.World.Player.Clock.Hours.ToString();
            if (timeHour.Length == 1)
                timeHour = "0" + timeHour;

            string timeMinute = GameLoop.World.Player.Clock.Minutes.ToString();
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


            SidebarConsole.DrawLine(new Point(7, 0), new Point(7, 9), (char)179, Color.White, Color.Black);

            // The time and date area (top-left)
            SidebarConsole.Print(8, 0, time);
            SidebarConsole.Print(14, 0, GameLoop.World.Player.Clock.AM ? "AM" : "PM");
            SidebarConsole.Print(8, 2, (months[GameLoop.World.Player.Clock.Month - 1] + " " + GameLoop.World.Player.Clock.Day).Align(HorizontalAlignment.Right, 8));
            SidebarConsole.Print(9, 3, ("Year " + GameLoop.World.Player.Clock.Year).Align(HorizontalAlignment.Right, 7));

            // HP
            SidebarConsole.Print(8, 5, new ColoredString(((char)3).ToString(), Color.Red, Color.Black));
            SidebarConsole.Print(9, 5, new ColoredString((GameLoop.World.Player.CurrentHP + "/" + GameLoop.World.Player.MaxHP).Align(HorizontalAlignment.Right, 7), Color.Red, Color.Black));

            if (GameLoop.World.Player.Race != null)
                SidebarConsole.Print(8, 7, GameLoop.World.Player.Race.Name);
            if (GameLoop.World.Player.ClassLevels.Count > 0)
                SidebarConsole.Print(8, 8, GameLoop.World.Player.ClassLevels[0].Name);


            SidebarConsole.DrawLine(new Point(16, 0), new Point(16, 9), (char)179, Color.White, Color.Black);
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
            SidebarConsole.DrawLine(new Point(0, 9), new Point(25, 9), (char)196, Color.White, Color.Black);


            if (GameLoop.UIManager.selectedMenu == "Map Editor") {
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
                
                SidebarConsole.Print(0, 17, "Min Monsters: - " + GameLoop.World.maps[GameLoop.World.Player.MapPos].MinimumMonsters.ToString().Align(HorizontalAlignment.Center, 3) + " +");
                SidebarConsole.Print(0, 18, "Max Monsters: - " + GameLoop.World.maps[GameLoop.World.Player.MapPos].MaximumMonsters.ToString().Align(HorizontalAlignment.Center, 3) + " +");

                SidebarConsole.Print(0, 19, "Monster ID: " + monIndex + "(" + (GameLoop.World.monsterLibrary.ContainsKey(monIndex) ? GameLoop.World.monsterLibrary[monIndex].Name : "None") + ")");
                if (GameLoop.World.maps[GameLoop.World.Player.MapPos].MonsterWeights.ContainsKey(monIndex)) {
                    SidebarConsole.Print(0, 20, "Weight: -" + GameLoop.World.maps[GameLoop.World.Player.MapPos].MonsterWeights[monIndex].ToString().Align(HorizontalAlignment.Center, 5) + "+");
                } else {
                    SidebarConsole.Print(0, 20, "Not on this map [Add]");
                }

                int weight = 0;

                if (GameLoop.World.maps[GameLoop.World.Player.MapPos].MonsterWeights.Count > 1) {
                    foreach (KeyValuePair<int, int> kv in GameLoop.World.maps[GameLoop.World.Player.MapPos].MonsterWeights) {
                        weight += kv.Value;
                    }
                } else if (GameLoop.World.maps[GameLoop.World.Player.MapPos].MonsterWeights.Count == 1) {
                    weight = GameLoop.World.maps[GameLoop.World.Player.MapPos].MonsterWeights.ElementAt(0).Value;
                }

                SidebarConsole.Print(0, 21, "Total Map Weight: " + weight);


                SidebarConsole.Print(0, 26, "Tile Index: " + tileIndex);

                if (GameLoop.World.tileLibrary.ContainsKey(tileIndex)) {
                    TileBase tile = GameLoop.World.tileLibrary[tileIndex];

                    SidebarConsole.Print(0, 27, "Tile Name: " + tile.Name);
                    SidebarConsole.Print(0, 28, "Tile Appearance: ");
                    SidebarConsole.Print(17, 28, tile.AsColoredGlyph());
                }

                if (tileSelected != new Point(-1, -1)) {
                    TileBase tile = GameLoop.World.maps[GameLoop.World.Player.MapPos].GetTile(tileSelected);
                    SidebarConsole.Print(0, 35, "Selected Tile: " + tile.Name);
                    SidebarConsole.Print(0, 36, "Tile Appearance: ");
                    SidebarConsole.Print(17, 36, tile.AsColoredGlyph());
                    if (tile.Lock != null) {
                        SidebarConsole.Print(0, 37, "Owner: " + tile.Lock.Owner);
                        SidebarConsole.Print(0, 38, "Rel Unlock: " + tile.Lock.RelationshipUnlock);
                        SidebarConsole.Print(0, 39, "Mission Unlock: " + tile.Lock.MissionUnlock);
                        SidebarConsole.Print(0, 40, "Always Locked: " + tile.Lock.AlwaysLocked);
                        SidebarConsole.Print(0, 41, "Unlocks at: " + GameLoop.World.Player.Clock.MinutesToTime(tile.Lock.UnlockTime));
                        SidebarConsole.Print(0, 42, "Locks at: " + GameLoop.World.Player.Clock.MinutesToTime(tile.Lock.LockTime));
                        SidebarConsole.Print(0, 43, "Key SubID: " + tile.Lock.KeySubID);
                    }
                }
            } else { // Print non-map editor stuff
                if (GameLoop.World != null && GameLoop.World.DoneInitializing) {

                    SidebarConsole.Print(0, 10, "Lv" + GameLoop.World.Player.Level);
                    SidebarConsole.Print(7, 10, ("XP: " + GameLoop.World.Player.Experience).Align(HorizontalAlignment.Right, 19));
                    SidebarConsole.Print(0, 11, ("To Next: " + (GameLoop.World.Player.ExpToLevel() - GameLoop.World.Player.Experience)).Align(HorizontalAlignment.Right, 26));


                    SidebarConsole.DrawLine(new Point(0, 12), new Point(25, 12), (char)196, Color.White, Color.Black);

                    ColoredString copperString = new ColoredString("CP:" + GameLoop.World.Player.CopperCoins, new Color(184, 115, 51), Color.Black);
                    ColoredString silverString = new ColoredString("SP:" + GameLoop.World.Player.SilverCoins, Color.Silver, Color.Black);
                    ColoredString goldString = new ColoredString("GP:" + GameLoop.World.Player.GoldCoins, Color.Yellow, Color.Black);
                    ColoredString JadeString = new ColoredString("JP:" + GameLoop.World.Player.JadeCoins, new Color(0, 168, 107), Color.Black);

                    SidebarConsole.Print(0, 13, copperString);
                    SidebarConsole.Print(0, 14, silverString);
                    SidebarConsole.Print(13, 13, goldString);
                    SidebarConsole.Print(13, 14, JadeString);

                    SidebarConsole.DrawLine(new Point(0, 15), new Point(25, 15), (char)196, Color.White, Color.Black);

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
                        if (item.Dec != null) {
                            SidebarConsole.SetDecorator(1, y, 1, new CellDecorator(new Color(item.Dec.R, item.Dec.G, item.Dec.B), item.Dec.Glyph, Mirror.None));
                        }

                        if (!item.IsStackable || (item.IsStackable && item.ItemQuantity == 1))
                            SidebarConsole.Print(3, y, new ColoredString(nameWithDurability, i == hotbarSelect ? Color.Yellow : item.Name == "(EMPTY)" ? Color.DarkSlateGray : Color.White, Color.TransparentBlack));
                        else
                            SidebarConsole.Print(3, y, new ColoredString(("(" + item.ItemQuantity + ") " + item.Name), i == hotbarSelect ? Color.Yellow : item.Name == "(EMPTY)" ? Color.DarkSlateGray : Color.White, Color.TransparentBlack));

                        if (i == hotbarSelect && item.ItemCategory == 7) {
                            SidebarConsole.Print(15, y, "[");
                            int chargePercent = ChargeBar / 10;

                            for (int j = 0; j < 10; j++) {
                                if (j < chargePercent) {
                                    SidebarConsole.Print(16 + j, y, "X");
                                } else {
                                    SidebarConsole.Print(16 + j, y, "-");
                                }
                            }

                            SidebarConsole.Print(25, y, "]");
                        }

                        y++;
                    }


                    y++;
                    SidebarConsole.Print(0, y, "Equipment");
                    y++;

                    for (int i = 0; i < 16; i++) {
                        Item item = GameLoop.World.Player.Equipment[i];

                        string nameWithDurability = item.Name;

                        if (item.Durability >= 0)
                            nameWithDurability = "[" + item.Durability + "] " + item.Name;


                        SidebarConsole.Print(0, y, "|");
                        SidebarConsole.Print(1, y, item.AsColoredGlyph());
                        if (item.Dec != null) {
                            SidebarConsole.SetDecorator(1, y, 1, new CellDecorator(new Color(item.Dec.R, item.Dec.G, item.Dec.B), item.Dec.Glyph, Mirror.None));
                        }
                        SidebarConsole.Print(3, y, new ColoredString(nameWithDurability, mousePos.Y == y ? Color.Yellow : item.Name == "(EMPTY)" ? Color.DarkSlateGray : Color.White, Color.TransparentBlack));
                        y++;
                    }


                }
            }
        }
    }
}
