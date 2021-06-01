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
        public MessageLogWindow MessageLog;

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
            MapWindow.CanDrag = true;
            int mapConsoleWidth = width - 2;
            int mapConsoleHeight = height - 2;

            MapConsole.ViewPort = new Rectangle(0, 0, mapConsoleWidth, mapConsoleHeight);
            MapConsole.Position = new Point(1, 1);

            Button closeButton = new Button(3, 1);
            closeButton.Position = new Point(0, 0);
            closeButton.Text = "[X]";
            MapWindow.Add(closeButton);

            MapWindow.Title = title.Align(HorizontalAlignment.Center, mapConsoleWidth);

            MapWindow.Children.Add(MapConsole);

            Children.Add(MapWindow);

            MapConsole.Children.Add(GameLoop.World.player.Get<Render>().sce);

            MapWindow.Show();
        }

        public void Init() {
            CreateConsoles();
           

            //Message Log initialization
            MessageLog = new MessageLogWindow(GameLoop.GameWidth, GameLoop.GameHeight / 2, "Message Log");
            Children.Add(MessageLog);
            MessageLog.Show();
            MessageLog.Position = new Point(0, GameLoop.GameHeight / 2);


            LoadMap(GameLoop.World.CurrentMap);

            CreateMapWindow(GameLoop.GameWidth, GameLoop.GameHeight / 2, "Game Map");
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
        }

        private void LoadMap(Map map) { 
            MapConsole = new SadConsole.ScrollingConsole(GameLoop.World.CurrentMap.Width, GameLoop.World.CurrentMap.Height, Global.FontDefault, new Rectangle(0, 0, GameLoop.GameWidth, GameLoop.GameHeight), map.Tiles);
             
            SyncMapEntities(map);
        }

    }
}