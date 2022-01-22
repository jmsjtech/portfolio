using LofiHollow.Entities;
using LofiHollow.Managers;
using LofiHollow.Minigames.Mining;
using SadConsole;
using SadConsole.Input;
using SadConsole.UI;
using SadRogue.Primitives;
using System.Collections.Generic;
using System.Linq;
using Key = SadConsole.Input.Keys;

namespace LofiHollow.UI {
    public class UI_Minigame {
        public SadConsole.Console MinigameConsole;
        public Window MinigameWindow;
        public string CurrentGame = "None";

        public int FishDistance = 30;
        public FishDef HookedFish;
        public bool FishFighting = false;
        public int LineStress = 0;
        public double ReeledTime = 0;
        public double FishRunTime = 0;
        public int FishFightLeft = 0;

        public Mine MountainMine;
        public Mine LakeMine;
        public bool MineJumpUp = false;
        public bool MineJumpApex = false;
        public int MineJump = 0;
        public GoRogue.FOV MiningFOV;
        public SadConsole.Entities.Renderer MiningEntities;

        public UI_Minigame(int width, int height, string title) {
            MinigameWindow = new(width, height);
            MinigameWindow.CanDrag = false;
            MinigameWindow.Position = new Point(11, 6);

            int invConWidth = width - 2;
            int invConHeight = height - 2;

            MinigameConsole = new(invConWidth, invConHeight);
            MinigameConsole.Position = new(1, 1);
            MinigameWindow.Title = title.Align(HorizontalAlignment.Center, invConWidth, (char)196);


            MinigameWindow.Children.Add(MinigameConsole);
            GameLoop.UIManager.Children.Add(MinigameWindow);

            MinigameWindow.Show();
            MinigameWindow.IsVisible = false;
        }


        public void RenderMinigame() {
            Point mousePos = new MouseScreenObjectState(MinigameConsole, GameHost.Instance.Mouse).CellPosition;

            MinigameConsole.Clear();

            if (CurrentGame == "Fishing") {
                if (HookedFish != null) {
                    FishingDraw();
                }
            }

            if (CurrentGame == "Mining") {
                if (GameLoop.World.Player.MineLocation == "Mountain") {
                    MiningDraw(MountainMine);
                } else if (GameLoop.World.Player.MineLocation == "Lake") {
                    MiningDraw(LakeMine);
                }
            }

        }

        public void MinigameInput() {
            if (CurrentGame == "Fishing") {
                if (HookedFish != null) {
                    FishingInput();
                }
            }

            if (CurrentGame == "Mining") {  
                if (GameLoop.World.Player.MineLocation == "Mountain") {
                    MiningInput(MountainMine);
                } else if (GameLoop.World.Player.MineLocation == "Lake") {
                    MiningInput(LakeMine);
                }
            }
        }

        public void MiningInput(Mine CurrentMine) {
            Point mousePos = new MouseScreenObjectState(MinigameConsole, GameHost.Instance.Mouse).CellPosition;

            if (GameHost.Instance.Mouse.ScrollWheelValueChange > 0) {
                if (GameLoop.UIManager.Sidebar.hotbarSelect + 1 < GameLoop.World.Player.Inventory.Length)
                    GameLoop.UIManager.Sidebar.hotbarSelect++;
            } else if (GameHost.Instance.Mouse.ScrollWheelValueChange < 0) {
                if (GameLoop.UIManager.Sidebar.hotbarSelect > 0)
                    GameLoop.UIManager.Sidebar.hotbarSelect--;
            }

            if (GameHost.Instance.Keyboard.IsKeyDown(Key.Space) && !MineJumpApex) {
                MineJumpUp = true;
            } else {
                MineJumpUp = false;
            }

            if (GameHost.Instance.Keyboard.IsKeyReleased(Key.Space)) {
                MineJumpApex = true;
                MineJump = 0;
            }

            if (GameHost.Instance.Mouse.LeftButtonDown) {
                int AgilityLevel = 0;

                if (GameLoop.World.Player.TimeLastActed + (120 - (AgilityLevel)) < SadConsole.GameHost.Instance.GameRunningTotalTime.TotalMilliseconds) {
                    GameLoop.World.Player.TimeLastActed = SadConsole.GameHost.Instance.GameRunningTotalTime.TotalMilliseconds;

                    CurrentMine.BreakTileAt(GameLoop.World.Player, GameLoop.World.Player.MineDepth, mousePos);
                }
            }


            if (GameHost.Instance.Keyboard.IsKeyDown(Key.A)) {
                if (!CollisionLeft(GameLoop.World.Player, CurrentMine.Levels[GameLoop.World.Player.MineDepth])) {
                    bool moved = CurrentMine.MovePlayerTo(GameLoop.World.Player, GameLoop.World.Player.MineDepth, GameLoop.World.Player.Position + new Point(-2, 0), false);
                    if (moved) {
                        if (GameLoop.NetworkManager != null && GameLoop.NetworkManager.lobbyManager != null) {
                            GameLoop.NetworkManager.BroadcastMsg("updatePlayerMine;" + GameLoop.NetworkManager.ownID + ";" + GameLoop.World.Player.Position.X + ";" + GameLoop.World.Player.Position.Y + ";"
                                + GameLoop.World.Player.MineLocation + ";" + GameLoop.World.Player.MineDepth);
                        }
                    }
                }
            }

            if (GameHost.Instance.Keyboard.IsKeyDown(Key.D)) {
                if (!CollisionRight(GameLoop.World.Player, CurrentMine.Levels[GameLoop.World.Player.MineDepth])) {
                    bool moved = CurrentMine.MovePlayerTo(GameLoop.World.Player, GameLoop.World.Player.MineDepth, GameLoop.World.Player.Position + new Point(2, 0), false);
                    if (moved) {
                        if (GameLoop.NetworkManager != null && GameLoop.NetworkManager.lobbyManager != null) {
                            GameLoop.NetworkManager.BroadcastMsg("updatePlayerMine;" + GameLoop.NetworkManager.ownID + ";" + GameLoop.World.Player.Position.X + ";" + GameLoop.World.Player.Position.Y + ";"
                                + GameLoop.World.Player.MineLocation + ";" + GameLoop.World.Player.MineDepth);
                        }
                    }
                }
            }

            if (GameHost.Instance.Keyboard.IsKeyDown(Key.G)) {
                CurrentMine.PickupItem(GameLoop.World.Player);
            }

            if (GameHost.Instance.Keyboard.IsKeyDown(Key.LeftShift) && GameHost.Instance.Keyboard.IsKeyPressed(Key.OemComma)) {
                if (GameLoop.World.Player.Position / 12 == new Point(35, 4) || GameLoop.World.Player.Position / 12 == new Point(34, 4) || GameLoop.World.Player.Position / 12 == new Point(36, 4)) {
                    GameLoop.UIManager.selectedMenu = "None";
                    CurrentGame = "None";
                    GameLoop.World.Player.UsePixelPositioning = false;
                    GameLoop.World.Player.Position = GameLoop.World.Player.MineEnteredAt;
                    ToggleMinigame();
                }
            }
        }
         
        public void SyncMiningEntities(Mine CurrentMine) {
            if (CurrentMine != null) {
                MinigameConsole.ForceRendererRefresh = true;
                MiningEntities.RemoveAll();

                if (GameLoop.World.Player.ScreenAppearance == null)
                    GameLoop.World.Player.UpdateAppearance();
                MiningEntities.Add(GameLoop.World.Player);

                foreach (Entity entity in CurrentMine.Levels[GameLoop.World.Player.MineDepth].Entities.Items) { 
                    MiningEntities.Add(entity); 
                } 

                foreach (KeyValuePair<long, Player> kv in GameLoop.World.otherPlayers) {
                    if (kv.Value.ScreenAppearance == null)
                        kv.Value.UpdateAppearance();

                    if (kv.Value.MineLocation == GameLoop.World.Player.MineLocation && kv.Value.MineDepth == GameLoop.World.Player.MineDepth) {
                        MiningEntities.Add(kv.Value);
                    }
                }

                MiningFOV = new GoRogue.FOV(CurrentMine.Levels[GameLoop.World.Player.MineDepth].MapFOV);

                UpdateMiningVision(CurrentMine);
            }
        }


        public void MiningDraw(Mine CurrentMine) {
            MiningCheckFall(GameLoop.World.Player, CurrentMine);
            UpdateMiningVision(CurrentMine);
               
            for (int x = 0; x < 70; x++) {
                for (int y = 0; y < 40; y++) {
                    if (CurrentMine.Levels[GameLoop.World.Player.MineDepth].GetTile(new Point(x, y)).Visible) { 
                        MinigameConsole.Print(x, y, CurrentMine.Levels[GameLoop.World.Player.MineDepth].GetTile(new Point(x, y)).GetAppearance()); 
                        if (CurrentMine.Levels[GameLoop.World.Player.MineDepth].GetTile(new Point(x, y)).Dec != null) {
                            MinigameConsole.SetDecorator(x, y, 1, CurrentMine.Levels[GameLoop.World.Player.MineDepth].GetTile(new Point(x, y)).Decorator());
                        } else {
                            MinigameConsole.ClearDecorators(x, y, 1);
                        }
                    }
                }
            }
        }

        public void UpdateMiningVision(Mine CurrentMine) { 
            MiningFOV.Calculate(GameLoop.World.Player.Position.X / 12, GameLoop.World.Player.Position.Y / 12, GameLoop.World.Player.Vision);
            foreach (var position in MiningFOV.NewlyUnseen) {
                CurrentMine.Levels[GameLoop.World.Player.MineDepth].GetTile(new Point(position.X, position.Y)).Shade();
            }

            foreach (var position in MiningFOV.CurrentFOV) {
                CurrentMine.Levels[GameLoop.World.Player.MineDepth].GetTile(new Point(position.X, position.Y)).Visible = true;
                CurrentMine.Levels[GameLoop.World.Player.MineDepth].GetTile(new Point(position.X, position.Y)).Unshade();
            } 

            foreach (KeyValuePair<long, Player> kv in GameLoop.World.otherPlayers) {
                if (kv.Value.MineLocation == GameLoop.World.Player.MineLocation && kv.Value.MineDepth == GameLoop.World.Player.MineDepth) {
                    if (MiningFOV.CurrentFOV.Contains(new GoRogue.Coord(kv.Value.Position.X / 12, kv.Value.Position.Y / 12))) {
                        MiningEntities.Add(kv.Value);
                        kv.Value.UsePixelPositioning = true;
                    } else {
                        MiningEntities.Remove(kv.Value);
                    }
                }
            }
        } 


        public bool CollisionTop(Entity ent, MineLevel level) {
            MineTile left = level.GetTile(new Point((ent.Position.X) / 12, (ent.Position.Y - 1) / 12));
            MineTile center = level.GetTile(new Point((ent.Position.X + 5) / 12, (ent.Position.Y - 1) / 12));
            MineTile right = level.GetTile(new Point((ent.Position.X + 11) / 12, (ent.Position.Y - 1) / 12));

            if (left != null && center != null && right != null) {
                if (!left.BlocksMove && !center.BlocksMove && !right.BlocksMove) {
                    return false;
                }
            }
            return true;
        }

        public bool CollisionBottom(Entity ent, MineLevel level) {
            MineTile left = level.GetTile(new Point((ent.Position.X) / 12, (ent.Position.Y + 12) / 12));
            MineTile center = level.GetTile(new Point((ent.Position.X + 5) / 12, (ent.Position.Y + 12) / 12));
            MineTile right = level.GetTile(new Point((ent.Position.X + 11) / 12, (ent.Position.Y + 12) / 12));

            if (left != null && center != null && right != null) {
                if (!left.BlocksMove && !center.BlocksMove && !right.BlocksMove) {
                    return false;
                }
            }
            return true;
        }

        public bool CollisionLeft(Entity ent, MineLevel level) {
            MineTile topLeft = level.GetTile(new Point((ent.Position.X - 1) / 12, (ent.Position.Y + 11) / 12));
            MineTile centerLeft = level.GetTile(new Point((ent.Position.X - 1) / 12, (ent.Position.Y + 5) / 12));
            MineTile bottomLeft = level.GetTile(new Point((ent.Position.X - 1) / 12, (ent.Position.Y) / 12));
            if (topLeft != null && centerLeft != null && bottomLeft != null) {
                if (!topLeft.BlocksMove && !centerLeft.BlocksMove && !bottomLeft.BlocksMove) {
                    return false;
                }
            }
            return true;
        }

        public bool CollisionRight(Entity ent, MineLevel level) {
            MineTile topLeft = level.GetTile(new Point((ent.Position.X + 12) / 12, (ent.Position.Y + 11) / 12));
            MineTile centerLeft = level.GetTile(new Point((ent.Position.X + 12) / 12, (ent.Position.Y + 5) / 12));
            MineTile bottomLeft = level.GetTile(new Point((ent.Position.X + 12) / 12, (ent.Position.Y) / 12));
            if (topLeft != null && centerLeft != null && bottomLeft != null) {
                if (!topLeft.BlocksMove && !centerLeft.BlocksMove && !bottomLeft.BlocksMove) {
                    return false;
                }
            }
            return true;
        }




        public void MiningCheckFall(Player player, Mine CurrentMine) {
            Point oldPos = new Point(player.Position.X, player.Position.Y);
            if (player.Position.Y / 12 < 39) {
                if (!MineJumpUp) {
                    if (!CollisionBottom(player, CurrentMine.Levels[player.MineDepth])) {
                        CurrentMine.MovePlayerTo(player, player.MineDepth, player.Position + new Point(0, 2), false);
                        MineJumpApex = true;
                        MineJump++;
                    } else {
                        MineJumpApex = false;
                        if (MineJump > 0) {
                            float blocksFallen = (float)MineJump / 6f;
                            if (blocksFallen > 8) {
                                GameLoop.World.Player.CurrentHP -= (int)((blocksFallen - 8f) / 2f);
                            } 
                        }

                        MineJump = 0;
                    } 
                } else {
                    if (!CollisionTop(player, CurrentMine.Levels[player.MineDepth])) {
                        MineJump += 4;
                        CurrentMine.MovePlayerBy(player, player.MineDepth, new Point(0, -4));
                        if (MineJump > 40) {
                            MineJumpApex = true;
                            MineJumpUp = false;
                            MineJump = 0;
                        }
                    } else {
                        MineJumpApex = true;
                    }
                }
            } 

            if (oldPos != GameLoop.World.Player.Position) { 
                if (GameLoop.NetworkManager != null && GameLoop.NetworkManager.lobbyManager != null) {
                    GameLoop.NetworkManager.BroadcastMsg("updatePlayerMine;" + GameLoop.NetworkManager.ownID + ";" + GameLoop.World.Player.Position.X + ";" + GameLoop.World.Player.Position.Y + ";"
                        + GameLoop.World.Player.MineLocation + ";" + GameLoop.World.Player.MineDepth);
                } 
            }

            foreach (Entity ent in CurrentMine.Levels[player.MineDepth].Entities.Items) {
                if (ent is Item item) {
                    if (item.Position.Y < 39) {
                        if (CurrentMine.Levels[player.MineDepth].GetTile(item.Position + new Point(0, 1)).Name == "Air") {
                            item.Position += new Point(0, 1);
                        }
                    } else {
                        if (CurrentMine.Levels.ContainsKey(player.MineDepth + 1)) {
                            if (CurrentMine.Levels[player.MineDepth + 1].GetTile(item.Position.WithY(0)).Name == "Air") {
                                item.Position = item.Position.WithY(0);
                                CurrentMine.Levels[player.MineDepth].Remove(item);
                                CurrentMine.Levels[player.MineDepth + 1].Add(item);
                            }
                        }
                    }
                }
            } 
        }

        public void MiningSetup(string loc) {
            GameLoop.World.Player.Position = new Point(35 * 12, 4 * 12);
            GameLoop.World.Player.UsePixelPositioning = true; 

            MiningEntities = new SadConsole.Entities.Renderer();
            MinigameConsole.SadComponents.Add(MiningEntities);
            if (loc == "Mountain") {
                if (MountainMine == null)
                    MountainMine = new("Mountain");

                GameLoop.World.Player.MineLocation = "Mountain";
                MiningFOV = new GoRogue.FOV(MountainMine.Levels[GameLoop.World.Player.MineDepth].MapFOV);
                SyncMiningEntities(MountainMine);

                if (GameLoop.NetworkManager != null && !GameLoop.NetworkManager.isHost) {
                    if (!MountainMine.SyncedFromHost) {
                        // Request the mine
                        string msg = "requestMine;Mountain;0";
                        GameLoop.NetworkManager.BroadcastMsg(msg);
                    }
                }
            }

            if (loc == "Lake") {
                if (LakeMine == null)
                    LakeMine = new("Lake");

                GameLoop.World.Player.MineLocation = "Lake"; 
                MiningFOV = new GoRogue.FOV(LakeMine.Levels[GameLoop.World.Player.MineDepth].MapFOV);
                SyncMiningEntities(LakeMine);

                if (GameLoop.NetworkManager != null && !GameLoop.NetworkManager.isHost) {
                    if (!LakeMine.SyncedFromHost) {
                        // Request the mine
                        string msg = "requestMine;Lake;0";
                        GameLoop.NetworkManager.BroadcastMsg(msg);
                    }
                }
            }

            MinigameWindow.Title = loc + " Mine - Depth: 0";
            MinigameWindow.TitleAlignment = SadConsole.HorizontalAlignment.Center;
        }


        public void FishingDraw() {
            MinigameConsole.DrawLine(new Point(0, 35), new Point(10, 35), 196, new Color(0, 127, 0));
            MinigameConsole.DrawLine(new Point(11, 35), new Point(72, 35), '~', new Color(0, 94, 184));
            MinigameConsole.Print(10, 34, GameLoop.World.Player.GetAppearance());
            MinigameConsole.Print(11, 34, new ColoredString("/", new Color(110, 66, 33), Color.Black));
            MinigameConsole.Print(12, 33, new ColoredString("/", new Color(110, 66, 33), Color.Black));
            MinigameConsole.Print(13, 32, new ColoredString("/", new Color(110, 66, 33), Color.Black));
            MinigameConsole.Print(FishDistance, 36, HookedFish.GetAppearance());
            MinigameConsole.DrawLine(new Point(14, 32), new Point(FishDistance - 1, 36), '~', Color.White);

            MinigameConsole.Print(1, 1, "Line Stress: " + LineStress);
        }

        public void FishingInput() {
            if (GameHost.Instance.Mouse.LeftButtonDown) {
                if (ReeledTime + 100 > SadConsole.GameHost.Instance.GameRunningTotalTime.TotalMilliseconds) {
                    return;
                }
                ReeledTime = SadConsole.GameHost.Instance.GameRunningTotalTime.TotalMilliseconds;

                if (FishFighting) {
                    FishDistance -= 1;
                    LineStress += HookedFish.Strength;
                } else {
                    FishDistance -= 2;
                    if (LineStress >= 2)
                        LineStress -= 2;
                    else
                        LineStress = 0;
                }

                int fightChance = GameLoop.rand.Next(100) + 1;

                if (fightChance < HookedFish.FightChance) {
                    FishFighting = true;
                    FishFightLeft = HookedFish.FightLength;
                } else {
                    FishFightLeft--;
                    if (FishFightLeft <= 0) {
                        FishFighting = false;
                    }
                }

                if (FishDistance <= 13) {
                    FinishFishing(true);
                } 

                if (FishDistance >= 72 || LineStress >= 100) {
                    FinishFishing(false);
                } 
            } else {
                int fightChance = GameLoop.rand.Next(100) + 1;

                if (FishFighting) {
                    if (FishRunTime + 100 > SadConsole.GameHost.Instance.GameRunningTotalTime.TotalMilliseconds) {
                        return;
                    }
                    FishRunTime = SadConsole.GameHost.Instance.GameRunningTotalTime.TotalMilliseconds;

                    FishDistance++;
                    FishFightLeft--;
                } else if (fightChance < HookedFish.FightChance && !FishFighting) {
                    FishFighting = true;
                    FishFightLeft = HookedFish.FightLength;
                }

                if (FishDistance <= 13) {
                    FinishFishing(true);
                }

                if (FishDistance >= 72 || LineStress >= 100) {
                    FinishFishing(false);
                }
            }
        }


        public void InitiateFishing(string Season, string Area, int CurrentTime, int FishingLevel) {
            List<FishDef> validFish = new();

            foreach (KeyValuePair<int, FishDef> kv in GameLoop.World.fishLibrary) {
                if (kv.Value.CatchLocation == Area || kv.Value.CatchLocation == "Any") {
                    if (kv.Value.Season == Season || kv.Value.Season == "Any") {
                        if (kv.Value.EarliestTime < CurrentTime && kv.Value.LatestTime > CurrentTime) {
                            if (kv.Value.RequiredLevel <= FishingLevel) {
                                validFish.Add(kv.Value);
                            }
                        }
                    }
                }
            }

            HookedFish = validFish[GameLoop.rand.Next(validFish.Count)];
            CurrentGame = "Fishing";
            FishDistance = 30;
            LineStress = 0;
            ToggleMinigame();
        }

        public void FinishFishing(bool success) {
            if (success) {
                Item caughtFish = new(new Color(HookedFish.colR, HookedFish.colG, HookedFish.colB), HookedFish.glyph);
                caughtFish.IsStackable = false;
                caughtFish.SubID = HookedFish.FishID;

                int WeightDiff = (int)((HookedFish.MaxWeightKG * 100) - (HookedFish.MinWeightKG * 100));
                caughtFish.Weight = (float)((GameLoop.rand.Next(WeightDiff) / 100) + HookedFish.MinWeightKG); 
                caughtFish.AverageValue = (int)(caughtFish.Weight * HookedFish.CoppersPerKG);
                caughtFish.Name = HookedFish.Name + " (" + caughtFish.Weight + " kg)";
                caughtFish.ItemCategory = 8;
                caughtFish.ItemID = 2;
                caughtFish.ForegroundR = HookedFish.colR;
                caughtFish.ForegroundG = HookedFish.colG;
                caughtFish.ForegroundB = HookedFish.colB;
                caughtFish.ItemGlyph = HookedFish.glyph;
                caughtFish.ItemQuantity = 1;

                CommandManager.AddItemToInv(GameLoop.World.Player, caughtFish);
                ToggleMinigame();
                ColoredString caught = new("You caught a ", Color.Cyan, Color.Black);
                caught += new ColoredString(HookedFish.Name, caughtFish.Appearance.Foreground, Color.Black);
                caught += new ColoredString(" (" + caughtFish.Weight + " kg)!", Color.Cyan, Color.Black);

                GameLoop.UIManager.AddMsg(caught);
                GameLoop.World.Player.Skills["Fishing"].GrantExp(HookedFish.GrantedExp);
            } else {
                ToggleMinigame();
                if (LineStress >= 100) {
                    GameLoop.UIManager.AddMsg(new ColoredString("The line snapped!", Color.Red, Color.Black));
                } else {
                    GameLoop.UIManager.AddMsg(new ColoredString("Looks like it got away...", Color.Red, Color.Black));
                }
            }

            HookedFish = null;
            GameLoop.UIManager.Map.MapConsole.ClearDecorators(GameLoop.UIManager.Sidebar.LocalLure.Position.X, GameLoop.UIManager.Sidebar.LocalLure.Position.Y, 1);
            GameLoop.UIManager.Sidebar.LocalLure.Position = new Point(-1, -1);
            GameLoop.UIManager.Sidebar.LocalLure.FishOnHook = false;
            GameLoop.UIManager.Sidebar.LocalLure = new FishingLure();
            CurrentGame = "None";
        }



        public void ToggleMinigame() {
            if (MinigameWindow.IsVisible) {
                GameLoop.UIManager.selectedMenu = "None";
                MinigameWindow.IsVisible = false;
                GameLoop.UIManager.Map.MapConsole.IsFocused = true;
            } else {
                GameLoop.UIManager.selectedMenu = "Minigame";
                MinigameWindow.IsVisible = true;
                MinigameWindow.IsFocused = true;

                if (CurrentGame == "Mining") {
                    MinigameWindow.Position = new Point(0, 0);
                } else {
                    MinigameWindow.Position = new Point(11, 6);
                }
            }
        }
    }
}
