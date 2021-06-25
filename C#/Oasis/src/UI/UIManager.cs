using DefaultEcs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SadConsole;
using System;
using SadConsole.Controls;
using GoRogue.Pathing;

namespace CritterQuest.UI {
    public class UIManager : ContainerConsole {
        public ScrollingConsole MapConsole;
        public Window MapWindow;
        public Window StatWindow;
        public SadConsole.Console StatConsole;
        public MessageLogWindow MessageLog;
        public SadConsole.Themes.Colors CustomColors;

        public UIManager() { 
            IsVisible = true;
            IsFocused = true; 
            Parent = SadConsole.Global.CurrentScreen;
        }

        public void CreateConsoles() {
            MapConsole = new ScrollingConsole(GameLoop.GameWidth, GameLoop.GameHeight);
        }

        public void CreateMapWindow(int width, int height, string title) {
            MapWindow = new Window(width, height);
            MapWindow.CanDrag = false;
            int mapConsoleWidth = width - 2;
            int mapConsoleHeight = height - 2;

            MapConsole.ViewPort = new Rectangle(0, 0, mapConsoleWidth, mapConsoleHeight);
            MapConsole.Position = new Point(1, 1);

            Button closeButton = new Button(3, 1);
            closeButton.Position = new Point(width, 0);
            closeButton.Text = "[X]";
            MapWindow.Add(closeButton);

            MapWindow.Title = title.Align(HorizontalAlignment.Center, mapConsoleWidth, (char) 205);

            MapWindow.Children.Add(MapConsole);

            Children.Add(MapWindow);

            MapConsole.Children.Add(GameLoop.World.player.Get<Render>().sce);

            MapWindow.Show();
        }

        public void CreateStatWindow(int width, int height, string title) {
            StatWindow = new Window(width, height);
            StatWindow.CanDrag = false;
            
            int statConsoleW = width - 2;
            int statConsoleH = height - 2;

            StatConsole = new SadConsole.Console(statConsoleW, statConsoleH);
            StatConsole.Position = new Point(1, 1);

            Button closeButton = new Button(3, 1);
            closeButton.Position = new Point(width, 0);
            closeButton.Text = "[X]";
            StatWindow.Add(closeButton);

            StatWindow.Title = title.Align(HorizontalAlignment.Center, statConsoleW, (char) 205);
            StatWindow.Position = new Point(GameLoop.GameWidth - 20, 0);
            


            StatWindow.Children.Add(StatConsole);

            Children.Add(StatWindow);

            StatWindow.Show();
        }

        public void Init() {
            SetupCustomColors();
            CreateConsoles();
           

            //Message Log initialization
            MessageLog = new MessageLogWindow(GameLoop.GameWidth, 15, "[MESSAGE LOG]");
            Children.Add(MessageLog);
            MessageLog.Show();
            MessageLog.Position = new Point(0, GameLoop.GameHeight -15);


            LoadMap(GameLoop.World.CurrentMap);

            CreateMapWindow(GameLoop.GameWidth - 20 , GameLoop.GameHeight - 15, "[MAP]");
            CreateStatWindow(20, GameLoop.GameHeight - 15, "[STATS]");
            UseMouse = true;

            CenterOnActor(GameLoop.World.player);
        }

        public void CenterOnActor(Entity player) {
            MapConsole.CenterViewPortOnPoint(player.Get<Render>().sce.Position);
        }


        public void AI_Act(string action, Point goal_loc, Entity monster, AI_GOAL goal) {
            Point pos = monster.Get<Render>().GetPosition();
            Path path;
            if (monster.Get<AI>().CurrentPath != null && monster.Get<AI>().CurrentPath.End == goal_loc) {
                path = monster.Get<AI>().CurrentPath;
            } else {
                path = GameLoop.World.CurrentMap.aStar.ShortestPath(pos.X, pos.Y, goal_loc.X, goal_loc.Y);
            }

            bool acted = false;

            if (action == "Attack") {
                if (path.Length != 0) {
                    acted = GameLoop.CommandManager.MoveEntityTo(monster, path.GetStep(0));
                }
            }

            if (action == "Loot") {
                if (pos != goal_loc) {
                    acted = GameLoop.CommandManager.MoveEntityTo(monster, path.GetStep(0));

                    if (pos == goal_loc) {
                        GameLoop.CommandManager.Pickup(monster, GameLoop.World.CurrentMap.GetEntityAt<Item>(pos).Value);
                    }


                } else {
                    if (GameLoop.World.CurrentMap.GetEntityAt<Item>(pos).HasValue) {
                        GameLoop.CommandManager.Pickup(monster, GameLoop.World.CurrentMap.GetEntityAt<Item>(pos).Value);
                    }
                }

                acted = true;
            }

            if (action == "Flee") {
                if (path != null) {
                    Point away = new Point((path.GetStep(0).X - pos.X) * -1, (path.GetStep(0).Y - pos.Y) * -1);
                    acted = GameLoop.CommandManager.MoveEntityBy(monster, away);
                }
            }

            if (action == "Wander") {
                if (path.Length != 0) {
                    acted = GameLoop.CommandManager.MoveEntityTo(monster, path.GetStep(0));
                }
            }


            if (acted) {
                monster.Get<LastActed>().last_action = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }

            if (pos == goal_loc || action == "Flee") {
                monster.Get<AI>().Goals.Remove(goal);
            }
        }

        public void UpdateGoals(Entity monster) {
            AI monAI = monster.Get<AI>();
            Viewshed viewshed = monster.Get<Viewshed>();
            Point pos = monster.Get<Render>().GetPosition();

            //  monAI.ClearGoals();

            foreach (Entity target in GameLoop.gs.ecs.GetEntities().With<Render>().AsEnumerable()) {
                if (target != monster) {
                    if (viewshed.view.BooleanFOV[target.Get<Render>().GetPosition()] || (viewshed.old_bool != null && viewshed.old_bool[target.Get<Render>().GetPosition()])) {
                        if (target.Has<Item>()) {
                            monAI.NewGoal("Loot", target.Get<Item>().value, 0, monAI.Greed, pos, target.Get<Render>().GetPosition(), target);
                        } else if (target.Has<Monster>() || target.Has<Player>()) {
                            int targetSTR = 1;
                            if (target.Has<Monster>()) { targetSTR = target.Get<AI>().ApparentStrength; } 
                            else { targetSTR = target.Get<Stats>().Attack; }

                            if (targetSTR < monAI.SelfStrength + monAI.Bravery) {
                                monAI.NewGoal("Attack", targetSTR, monAI.Bravery, 1, pos, target.Get<Render>().GetPosition(), target);
                            } else {
                                monAI.NewGoal("Flee", targetSTR, monAI.Bravery, 1, pos, target.Get<Render>().GetPosition(), target);
                            }
                        }
                    }
                }
            }

            if (monAI.Goals.Count > 1) {
                foreach (AI_GOAL goal in monAI.Goals) {
                    if (goal.Action == "Wander") {
                        monAI.Goals.Remove(goal);
                        break;
                    }
                }
            }

            if (monAI.Goals.Count > 0) {
                monAI.SortGoals();
                DateTimeOffset now = DateTimeOffset.UtcNow;
                long nowInMS = now.ToUnixTimeMilliseconds();

                if (monster.Get<LastActed>().last_action + monster.Get<LastActed>().speed_in_ms <= nowInMS) {
                    AI_Act(monAI.Goals.ToArray()[0].Action, monAI.Goals.ToArray()[0].Location, monster, monAI.Goals.ToArray()[0]);
                }
            } else if (monAI.Goals.Count == 0) {
                Point topL = new Point(pos.X - (viewshed.radius / 2), pos.Y - (viewshed.radius / 2));
                Point random = new Point(topL.X + GameLoop.GlobalRand.Next(0, viewshed.radius), topL.Y + GameLoop.GlobalRand.Next(0, viewshed.radius));

                while (!GameLoop.World.CurrentMap.IsTileWalkable(random)) {
                    random = new Point(topL.X + GameLoop.GlobalRand.Next(0, viewshed.radius), topL.Y + GameLoop.GlobalRand.Next(0, viewshed.radius));
                }

                monAI.NewGoal("Wander", 1, 0, 1, pos, random);
            }

            
        }


        public override void Update(TimeSpan timeElapsed) {
            CheckKeyboard();

            foreach (Entity monster in GameLoop.gs.ecs.GetEntities().With<AI>().AsEnumerable()) {
                if (monster.Get<Viewshed>().view == null) {
                    monster.Get<Viewshed>().view = new GoRogue.FOV(GameLoop.World.CurrentMap.sightMap);
                }
                monster.Get<Viewshed>().UpdateFOV(monster.Get<Render>().GetPosition());
                UpdateGoals(monster);
            }

            if (GameLoop.World.player.Get<Viewshed>().view == null) {
                GameLoop.World.player.Get<Viewshed>().view = new GoRogue.FOV(GameLoop.World.CurrentMap.sightMap);
            }
            GameLoop.World.player.Get<Viewshed>().view.Calculate(GameLoop.World.player.Get<Render>().GetPosition(), GameLoop.World.player.Get<Viewshed>().radius);

            base.Update(timeElapsed);
        }

        private void CheckKeyboard() {
            bool acted = false;

            if (SadConsole.Global.KeyboardState.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.F5)) {
                SadConsole.Settings.ToggleFullScreen();
            }

            if (Global.KeyboardState.IsKeyPressed(Keys.Z)) {
                GameLoop.CommandManager.UndoMoveEntityBy();
                CenterOnActor(GameLoop.World.player);
            }
            if (Global.KeyboardState.IsKeyPressed(Keys.X)) {
                GameLoop.CommandManager.RedoMoveEntityBy();
                CenterOnActor(GameLoop.World.player);
            }

            if (Global.KeyboardState.IsKeyReleased(Keys.T)) {
                GameLoop.UIManager.MessageLog.Add("space");
                Entity newItem = GameLoop.gs.ecs.CreateEntity();
                newItem.Set(new Render { sce = new SadConsole.Entities.Entity(1, 1) });
                newItem.Set(new Item { condition = 100, weight = 2, glyph = 'L', fg = Color.Green, value = 2 });
                newItem.Set(new Name { name = "Fancy Shirt" });

                newItem.Get<Render>().Init(GameLoop.World.player.Get<Render>().GetPosition(), 'L', Color.Green);
                SyncMapEntities(GameLoop.World.CurrentMap);
            }


            if (Global.KeyboardState.IsKeyPressed(Keys.G)) {
                if (GameLoop.World.CurrentMap.GetEntityAt<Item>(GameLoop.World.player.Get<Render>().GetPosition()) != null) {
                    Entity item = GameLoop.World.CurrentMap.GetEntityAt<Item>(GameLoop.World.player.Get<Render>().GetPosition()).Value;
                    GameLoop.CommandManager.Pickup(GameLoop.World.player, item);
                    SyncMapEntities(GameLoop.World.CurrentMap);
                    CenterOnActor(GameLoop.World.player);
                }
            }

            if (GameLoop.World.player.Get<LastActed>().last_action + GameLoop.World.player.Get<LastActed>().speed_in_ms <= DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()) {

                if (Global.KeyboardState.IsKeyPressed(Keys.NumPad8)) {
                    acted = GameLoop.CommandManager.MoveEntityBy(GameLoop.World.player, new Point(0, -1));
                    CenterOnActor(GameLoop.World.player);
                }
                if (Global.KeyboardState.IsKeyPressed(Keys.NumPad2)) {
                    acted = GameLoop.CommandManager.MoveEntityBy(GameLoop.World.player, new Point(0, 1));
                    CenterOnActor(GameLoop.World.player);
                }
                if (Global.KeyboardState.IsKeyPressed(Keys.NumPad4)) {
                    acted = GameLoop.CommandManager.MoveEntityBy(GameLoop.World.player, new Point(-1, 0));
                    CenterOnActor(GameLoop.World.player);
                }
                if (Global.KeyboardState.IsKeyPressed(Keys.NumPad6)) {
                    acted = GameLoop.CommandManager.MoveEntityBy(GameLoop.World.player, new Point(1, 0));
                    CenterOnActor(GameLoop.World.player);
                }

                if (Global.KeyboardState.IsKeyPressed(Keys.NumPad7)) {
                    acted = GameLoop.CommandManager.MoveEntityBy(GameLoop.World.player, new Point(-1, -1));
                    CenterOnActor(GameLoop.World.player);
                }
                if (Global.KeyboardState.IsKeyPressed(Keys.NumPad9)) {
                    acted = GameLoop.CommandManager.MoveEntityBy(GameLoop.World.player, new Point(1, -1));
                    CenterOnActor(GameLoop.World.player);
                }
                if (Global.KeyboardState.IsKeyPressed(Keys.NumPad1)) {
                    acted = GameLoop.CommandManager.MoveEntityBy(GameLoop.World.player, new Point(-1, 1));
                    CenterOnActor(GameLoop.World.player);
                }
                if (Global.KeyboardState.IsKeyPressed(Keys.NumPad3)) {
                    acted = GameLoop.CommandManager.MoveEntityBy(GameLoop.World.player, new Point(1, 1));
                    CenterOnActor(GameLoop.World.player);
                }

                if (acted) {
                    GameLoop.World.player.Get<LastActed>().last_action = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                }
            }
        }

        public void SyncMapEntities(Map map) {
            MapConsole.Children.Clear();

            foreach (Entity entity in GameLoop.gs.ecs.GetEntities().With<Render>().AsEnumerable()) {
                MapConsole.Children.Add(entity.Get<Render>().sce);
            }

            MapConsole.IsDirty = true;
        }

        private void LoadMap(Map map) { 
            MapConsole = new SadConsole.ScrollingConsole(GameLoop.World.CurrentMap.Width, GameLoop.World.CurrentMap.Height, Global.FontDefault, new Rectangle(0, 0, GameLoop.GameWidth, GameLoop.GameHeight), map.Tiles);
             
            SyncMapEntities(map);
        }

        private void SetupCustomColors() {
            CustomColors = SadConsole.Themes.Colors.CreateDefault();

            Color backgroundColor = Color.Black;

            CustomColors.ControlHostBack = backgroundColor;
            CustomColors.ControlBack = backgroundColor;

            CustomColors.ControlBackLight = (backgroundColor * 1.3f).FillAlpha();
            CustomColors.ControlBackDark = (backgroundColor * 0.7f).FillAlpha();

            CustomColors.ControlBackSelected = CustomColors.GrayDark;

            CustomColors.TitleText = Color.White;
            CustomColors.Lines = Color.White;

            SadConsole.Themes.WindowTheme windowTheme = new SadConsole.Themes.WindowTheme();
            windowTheme.BorderLineStyle = CellSurface.ConnectedLineThick;
            SadConsole.Themes.Library.Default.WindowTheme = windowTheme;

            CustomColors.RebuildAppearances();

            SadConsole.Themes.Library.Default.Colors = CustomColors;
            
        }

    }
}