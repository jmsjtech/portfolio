using DefaultEcs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SadConsole;
using System;
using SadConsole.Controls;
using System.Collections.Generic;

namespace NeonCity.UI {
    public class UIManager : ContainerConsole {
        public ScrollingConsole MapConsole;
        public Window MapWindow;
        public Window StatWindow;
        public SadConsole.Console StatConsole;
        public MessageLogWindow MessageLog;
        public SadConsole.Themes.Colors CustomColors;

        public Entity selectedEntity;
        public bool moving = false;
        public Point clicked = new Point(0, 0);

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

            MapConsole.Children.Add(GameLoop.World.playerCursor.Get<Render>().sce);

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

            CenterOnActor(GameLoop.World.playerCursor);
        }

        public void CenterOnActor(Entity player) {
            MapConsole.CenterViewPortOnPoint(player.Get<Render>().sce.Position);
        }

        public void CenterOnPoint(Point pos) {
            MapConsole.CenterViewPortOnPoint(pos);
        }

        public override void Update(TimeSpan timeElapsed) {
            CheckKeyboard();

            Point mousePos = Global.MouseState.ScreenPosition.PixelLocationToConsole(12, 12);
            mousePos -= new Point(3, 3);
            mousePos += MapConsole.ViewPort.Location;

            GameLoop.World.playerCursor.Get<Render>().SetPosition(mousePos);







            if (Global.MouseState.RightButtonDown) {
                if (moving == false) {
                    moving = true;
                    clicked = MapConsole.ViewPort.Location + new Point(MapConsole.ViewPort.Width / 2, MapConsole.ViewPort.Height / 2);
                }

                if (moving) {
                    if (clicked != mousePos + new Point (2, 2)) {
                        Point difference = clicked - mousePos + new Point(2, 2);
                        if (difference.X < 0) { difference.X = -1; }
                        if (difference.X > 0) { difference.X = 1; }
                        if (difference.Y < 0) { difference.Y = -1; }
                        if (difference.Y > 0) { difference.Y = 1; }

                        clicked -= difference;
                    }


                    CenterOnPoint(clicked);
                }
            }


            if (Global.MouseState.RightClicked) {
                moving = false;
            }

            Entity? entity = GameLoop.World.CurrentMap.GetEntityAt<Tile>(mousePos + new Point(2, 2));
            if (entity.HasValue && selectedEntity != entity.Value) {
                selectedEntity = entity.Value;
            }


            if (Global.MouseState.LeftClicked) {
                ClickHandler(mousePos + new Point(2, 2));
            }


            UpdateStats();


            base.Update(timeElapsed);
        }

        public void UpdateStats() {
            StatConsole.Clear();

            StatConsole.Print(0, 1, " Gold: " + GameLoop.World.playerCursor.Get<Player>().gold, Color.Gold);
            StatConsole.Print(0, 2, " Wood: " + GameLoop.World.playerCursor.Get<Player>().wood, Color.SandyBrown);
            StatConsole.Print(0, 3, "Stone: " + GameLoop.World.playerCursor.Get<Player>().stone, Color.Gray);



            if (selectedEntity != null) {
                int printy = 10;
                if (selectedEntity.Has<Name>()) {
                    StatConsole.Print(0, printy, selectedEntity.Get<Name>().name);
                    printy++;
                }

                if (selectedEntity.Has<Resources>()) {
                    foreach(KeyValuePair<string, int> resource in selectedEntity.Get<Resources>().resources) {
                        StatConsole.Print(0, printy, resource.Key + ": " + resource.Value);
                        printy++;
                    }
                }
            }
        }


        private void ClickHandler(Point pos) {
            Entity? entity = GameLoop.World.CurrentMap.GetEntityAt<Tile>(pos);

            if (entity.HasValue) {
                if (entity.Value.Get<Name>().name == "Tree") {
                    if (entity.Value.Get<Resources>().resources["Health"] - 25 <= 0) {
                        entity.Value.Get<Name>().name = "Dirt";
                        entity.Value.Get<Tile>().SimpleSet('"', Color.Brown);
                        GameLoop.World.playerCursor.Get<Player>().wood += entity.Value.Get<Resources>().resources["Wood"];
                        entity.Value.Remove<Resources>();
                    } else {
                        entity.Value.Get<Resources>().resources["Health"] -= 25;
                    }
                }
            }

        }


        private void CheckKeyboard() {
            if (SadConsole.Global.KeyboardState.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.F5)) {
                SadConsole.Settings.ToggleFullScreen();
            }


            if (Global.KeyboardState.IsKeyReleased(Keys.T)) {
                GameLoop.UIManager.MessageLog.Add("space");
                Entity newItem = GameLoop.gs.ecs.CreateEntity();
                newItem.Set(new Render { sce = new SadConsole.Entities.Entity(1, 1) });
                newItem.Set(new Name { name = "Fancy Shirt" });

                newItem.Get<Render>().Init(GameLoop.World.playerCursor.Get<Render>().GetPosition(), 'L', Color.Green);
                SyncMapEntities(GameLoop.World.CurrentMap);
            }


            if (Global.KeyboardState.IsKeyPressed(Keys.NumPad8)) {
                GameLoop.CommandManager.MoveEntityBy(GameLoop.World.playerCursor, new Point(0, -1));
                CenterOnActor(GameLoop.World.playerCursor);
            }
            if (Global.KeyboardState.IsKeyPressed(Keys.NumPad2)) {
                GameLoop.CommandManager.MoveEntityBy(GameLoop.World.playerCursor, new Point(0, 1));
                CenterOnActor(GameLoop.World.playerCursor);
            }
            if (Global.KeyboardState.IsKeyPressed(Keys.NumPad4)) {
                GameLoop.CommandManager.MoveEntityBy(GameLoop.World.playerCursor, new Point(-1, 0));
                CenterOnActor(GameLoop.World.playerCursor);
            }
            if (Global.KeyboardState.IsKeyPressed(Keys.NumPad6)) {
                GameLoop.CommandManager.MoveEntityBy(GameLoop.World.playerCursor, new Point(1, 0));
                CenterOnActor(GameLoop.World.playerCursor);
            }

            if (Global.KeyboardState.IsKeyPressed(Keys.NumPad7)) {
                GameLoop.CommandManager.MoveEntityBy(GameLoop.World.playerCursor, new Point(-1, -1));
                CenterOnActor(GameLoop.World.playerCursor);
            }
            if (Global.KeyboardState.IsKeyPressed(Keys.NumPad9)) {
                GameLoop.CommandManager.MoveEntityBy(GameLoop.World.playerCursor, new Point(1, -1));
                CenterOnActor(GameLoop.World.playerCursor);
            }
            if (Global.KeyboardState.IsKeyPressed(Keys.NumPad1)) {
                GameLoop.CommandManager.MoveEntityBy(GameLoop.World.playerCursor, new Point(-1, 1));
                CenterOnActor(GameLoop.World.playerCursor);
            }
            if (Global.KeyboardState.IsKeyPressed(Keys.NumPad3)) {
                GameLoop.CommandManager.MoveEntityBy(GameLoop.World.playerCursor, new Point(1, 1));
                CenterOnActor(GameLoop.World.playerCursor);
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
