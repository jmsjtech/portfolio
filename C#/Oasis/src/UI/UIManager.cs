using MonoGame.Extended.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SadConsole;
using System;
using SadConsole.Controls;
using GoRogue.Pathing;

namespace Oasis.UI {
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

            MapWindow.Title = title.Align(HorizontalAlignment.Center, mapConsoleWidth, (char)205);

            MapWindow.Children.Add(MapConsole);

            Children.Add(MapWindow);

            MapConsole.Children.Add(GameLoop.World.players[GameLoop.NetworkManager.myUID].Get<Render>().sce);

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

            StatWindow.Title = title.Align(HorizontalAlignment.Center, statConsoleW, (char)205);
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
            MessageLog.Position = new Point(0, GameLoop.GameHeight - 15);


            LoadMap(GameLoop.World.CurrentMap);

            CreateMapWindow(GameLoop.GameWidth - 20, GameLoop.GameHeight - 15, "[MAP]");
            CreateStatWindow(20, GameLoop.GameHeight - 15, "[STATS]");
            UseMouse = true;

            CenterOnActor(GameLoop.World.players[GameLoop.NetworkManager.myUID]);
        }

        public void CenterOnActor(Entity player) {
            MapConsole.CenterViewPortOnPoint(player.Get<Render>().sce.Position);
        }


        public override void Update(TimeSpan timeElapsed) {
            CheckKeyboard();


            if (GameLoop.World.players[GameLoop.NetworkManager.myUID].Get<Viewshed>().view == null) {
                GameLoop.World.players[GameLoop.NetworkManager.myUID].Get<Viewshed>().view = new GoRogue.FOV(GameLoop.World.CurrentMap.sightMap);
            }
            GameLoop.World.players[GameLoop.NetworkManager.myUID].Get<Viewshed>().view.Calculate(GameLoop.World.players[GameLoop.NetworkManager.myUID].Get<Render>().sce.Position, GameLoop.World.players[GameLoop.NetworkManager.myUID].Get<Viewshed>().radius);

            base.Update(timeElapsed);
        }

        private void CheckKeyboard() {
            bool acted = false;

            if (SadConsole.Global.KeyboardState.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.F5)) {
                SadConsole.Settings.ToggleFullScreen();
            }

            if (Global.KeyboardState.IsKeyPressed(Keys.Z)) {
                GameLoop.CommandManager.UndoMoveEntityBy();
                CenterOnActor(GameLoop.World.players[GameLoop.NetworkManager.myUID]);
            }
            if (Global.KeyboardState.IsKeyPressed(Keys.X)) {
                GameLoop.CommandManager.RedoMoveEntityBy();
                CenterOnActor(GameLoop.World.players[GameLoop.NetworkManager.myUID]);
            }

            if (Global.KeyboardState.IsKeyReleased(Keys.T)) {
                GameLoop.UIManager.MessageLog.Add("space");
                Entity newItem = GameLoop.ecs.CreateEntity();
                newItem.Attach(new Render { sce = new SadConsole.Entities.Entity(1, 1) });
                newItem.Attach(new Item { condition = 100, weight = 2, glyph = 'L', fg = Color.Green, value = 2 });
                newItem.Attach(new Name { name = "Fancy Shirt" });

                Helper.Render_Init(newItem.Get<Render>(), GameLoop.World.players[GameLoop.NetworkManager.myUID].Get<Render>().sce.Position, 'L', Color.Green);
                SyncMapEntities(GameLoop.World.CurrentMap);
            }


            if (Global.KeyboardState.IsKeyPressed(Keys.G)) {
                if (GameLoop.World.CurrentMap.GetEntityAt<Item>(GameLoop.World.players[GameLoop.NetworkManager.myUID].Get<Render>().sce.Position) != null) {
                    Entity item = GameLoop.World.CurrentMap.GetEntityAt<Item>(GameLoop.World.players[GameLoop.NetworkManager.myUID].Get<Render>().sce.Position).Value;
                    GameLoop.CommandManager.Pickup(GameLoop.World.players[GameLoop.NetworkManager.myUID], item);
                    SyncMapEntities(GameLoop.World.CurrentMap);
                    CenterOnActor(GameLoop.World.players[GameLoop.NetworkManager.myUID]);
                }
            }

            if (GameLoop.World.players[GameLoop.NetworkManager.myUID].Get<LastActed>().last_action + GameLoop.World.players[GameLoop.NetworkManager.myUID].Get<LastActed>().speed_in_ms <= DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()) {

                if (Global.KeyboardState.IsKeyPressed(Keys.NumPad8)) {
                    acted = GameLoop.CommandManager.MoveEntityBy(GameLoop.World.players[GameLoop.NetworkManager.myUID], new Point(0, -1));
                    CenterOnActor(GameLoop.World.players[GameLoop.NetworkManager.myUID]);
                }
                if (Global.KeyboardState.IsKeyPressed(Keys.NumPad2)) {
                    acted = GameLoop.CommandManager.MoveEntityBy(GameLoop.World.players[GameLoop.NetworkManager.myUID], new Point(0, 1));
                    CenterOnActor(GameLoop.World.players[GameLoop.NetworkManager.myUID]);
                }
                if (Global.KeyboardState.IsKeyPressed(Keys.NumPad4)) {
                    acted = GameLoop.CommandManager.MoveEntityBy(GameLoop.World.players[GameLoop.NetworkManager.myUID], new Point(-1, 0));
                    CenterOnActor(GameLoop.World.players[GameLoop.NetworkManager.myUID]);
                }
                if (Global.KeyboardState.IsKeyPressed(Keys.NumPad6)) {
                    acted = GameLoop.CommandManager.MoveEntityBy(GameLoop.World.players[GameLoop.NetworkManager.myUID], new Point(1, 0));
                    CenterOnActor(GameLoop.World.players[GameLoop.NetworkManager.myUID]);
                }

                if (Global.KeyboardState.IsKeyPressed(Keys.NumPad7)) {
                    acted = GameLoop.CommandManager.MoveEntityBy(GameLoop.World.players[GameLoop.NetworkManager.myUID], new Point(-1, -1));
                    CenterOnActor(GameLoop.World.players[GameLoop.NetworkManager.myUID]);
                }
                if (Global.KeyboardState.IsKeyPressed(Keys.NumPad9)) {
                    acted = GameLoop.CommandManager.MoveEntityBy(GameLoop.World.players[GameLoop.NetworkManager.myUID], new Point(1, -1));
                    CenterOnActor(GameLoop.World.players[GameLoop.NetworkManager.myUID]);
                }
                if (Global.KeyboardState.IsKeyPressed(Keys.NumPad1)) {
                    acted = GameLoop.CommandManager.MoveEntityBy(GameLoop.World.players[GameLoop.NetworkManager.myUID], new Point(-1, 1));
                    CenterOnActor(GameLoop.World.players[GameLoop.NetworkManager.myUID]);
                }
                if (Global.KeyboardState.IsKeyPressed(Keys.NumPad3)) {
                    acted = GameLoop.CommandManager.MoveEntityBy(GameLoop.World.players[GameLoop.NetworkManager.myUID], new Point(1, 1));
                    CenterOnActor(GameLoop.World.players[GameLoop.NetworkManager.myUID]);
                }

                if (Global.KeyboardState.IsKeyPressed(Keys.S)) {
                    GameLoop.SaveManager.save();
                }

                if (Global.KeyboardState.IsKeyPressed(Keys.L)) {
                    GameLoop.SaveManager.load();
                }

                if (acted) {
                    GameLoop.World.players[GameLoop.NetworkManager.myUID].Get<LastActed>().last_action = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                }
            }
        }

        public void SyncMapEntities(Map map) {
            MapConsole.Children.Clear();

            foreach (Entity entity in GameLoop.ecs.GetEntities().With<Render>().AsEnumerable()) {
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