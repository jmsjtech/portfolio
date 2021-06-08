using DefaultEcs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SadConsole;
using System;
using SadConsole.Controls;


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

        public override void Update(TimeSpan timeElapsed) {
            CheckKeyboard();
            base.Update(timeElapsed);
        }

        private void CheckKeyboard() {
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

            if (Global.KeyboardState.IsKeyPressed(Keys.G)) {
                if (GameLoop.World.CurrentMap.GetEntityAt<Item>(GameLoop.World.player.Get<Render>().GetPosition()) != null) {
                    Entity item = GameLoop.World.CurrentMap.GetEntityAt<Item>(GameLoop.World.player.Get<Render>().GetPosition()).Value;
                    GameLoop.CommandManager.Pickup(GameLoop.World.player, item);
                    SyncMapEntities(GameLoop.World.CurrentMap);
                    CenterOnActor(GameLoop.World.player);
                }
            }


            if (Global.KeyboardState.IsKeyPressed(Keys.NumPad8)) {
                GameLoop.CommandManager.MoveEntityBy(GameLoop.World.player, new Point(0, -1));
                CenterOnActor(GameLoop.World.player);
            }
            if (Global.KeyboardState.IsKeyPressed(Keys.NumPad2)) {
                GameLoop.CommandManager.MoveEntityBy(GameLoop.World.player, new Point(0, 1));
                CenterOnActor(GameLoop.World.player);
            }
            if (Global.KeyboardState.IsKeyPressed(Keys.NumPad4)) {
                GameLoop.CommandManager.MoveEntityBy(GameLoop.World.player, new Point(-1, 0));
                CenterOnActor(GameLoop.World.player);
            }
            if (Global.KeyboardState.IsKeyPressed(Keys.NumPad6)) {
                GameLoop.CommandManager.MoveEntityBy(GameLoop.World.player, new Point(1, 0));
                CenterOnActor(GameLoop.World.player);
            }

            if (Global.KeyboardState.IsKeyPressed(Keys.NumPad7)) {
               GameLoop.CommandManager.MoveEntityBy(GameLoop.World.player, new Point(-1, -1));
                CenterOnActor(GameLoop.World.player);
            }
            if (Global.KeyboardState.IsKeyPressed(Keys.NumPad9)) {
                GameLoop.CommandManager.MoveEntityBy(GameLoop.World.player, new Point(1, -1));
                CenterOnActor(GameLoop.World.player);
            }
            if (Global.KeyboardState.IsKeyPressed(Keys.NumPad1)) {
                GameLoop.CommandManager.MoveEntityBy(GameLoop.World.player, new Point(-1, 1));
                CenterOnActor(GameLoop.World.player);
            }
            if (Global.KeyboardState.IsKeyPressed(Keys.NumPad3)) {
                GameLoop.CommandManager.MoveEntityBy(GameLoop.World.player, new Point(1, 1));
                CenterOnActor(GameLoop.World.player);
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