using LofiHollow.Entities;
using SadConsole;
using SadConsole.Input;
using SadConsole.UI;
using SadRogue.Primitives;
using System.Collections.Generic;
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

        public UI_Minigame(int width, int height, string title) {
            MinigameWindow = new Window(width, height);
            MinigameWindow.CanDrag = false;
            MinigameWindow.Position = new Point(11, 6);

            int invConWidth = width - 2;
            int invConHeight = height - 2;

            MinigameConsole = new SadConsole.Console(invConWidth, invConHeight);
            MinigameConsole.Position = new Point(1, 1);
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

        }

        public void MinigameInput() {
            Point mousePos = new MouseScreenObjectState(MinigameConsole, GameHost.Instance.Mouse).CellPosition;

            if (CurrentGame == "Fishing") {
                if (HookedFish != null) {
                    FishingInput();
                }
            }
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
                    LineStress += 5;
                } else {
                    FishDistance -= 2;
                }

                if (FishDistance <= 13) {
                    FinishFishing();
                } 
            }
        }


        public void InitiateFishing(string Season, string Area, int CurrentTime, int FishingLevel) {
            List<FishDef> validFish = new List<FishDef>();

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
            ToggleMinigame();
        }

        public void FinishFishing() {
            Item caughtFish = new Item(new Color(HookedFish.colR, HookedFish.colG, HookedFish.colB), HookedFish.glyph);
            caughtFish.IsStackable = false;
            caughtFish.SubID = HookedFish.FishID;
            caughtFish.Weight = (float) (HookedFish.MinWeightKG + HookedFish.MaxWeightKG) / 2;
            caughtFish.AverageValue = (int) (caughtFish.Weight * HookedFish.CoppersPerKG);
            caughtFish.Name = HookedFish.Name + " (" + caughtFish.Weight + " kg)";
            caughtFish.ItemCategory = 8;
            caughtFish.ItemID = 2;
            caughtFish.ForegroundR = HookedFish.colR;
            caughtFish.ForegroundG = HookedFish.colG;
            caughtFish.ForegroundB = HookedFish.colB;
            caughtFish.ItemGlyph = HookedFish.glyph;
            caughtFish.ItemQuantity = 1;

            GameLoop.CommandManager.AddItemToInv(GameLoop.World.Player, caughtFish);
            ToggleMinigame();
            ColoredString caught = new ColoredString("You caught a ", Color.Cyan, Color.Black);
            caught += new ColoredString(HookedFish.Name, caughtFish.Appearance.Foreground, Color.Black);
            caught += new ColoredString(" (" + caughtFish.Weight + " kg)!", Color.Cyan, Color.Black);

            GameLoop.UIManager.AddMsg(caught);
            GameLoop.World.Player.Skills["Fishing"].GrantExp(HookedFish.GrantedExp);
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
            }
        }
    }
}
