using System;
using System.Collections.Generic;
using Discord;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using RogueSharp.DiceNotation;
using SadConsole;
using SadConsole.Controls;
using SadConsole.Input;
using TearsInRain.Entities;
using Entity = TearsInRain.Entities.Entity;
using TearsInRain.Serializers;
using TearsInRain.Tiles;
using Console = SadConsole.Console;
using Utils = TearsInRain.Utils;
using GoRogue;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using System.IO;
using TearsInRain.src;

namespace TearsInRain.UI {
    public class UIManager : ContainerConsole {
        public ScrollingConsole MapConsole;
        public Console StatusConsole;
        public Console SplashConsole;
        public Console SplashRain;
        public Console MenuConsole;
        
        public Window MapWindow;
        public MessageLogWindow MessageLog;
        public Window EscapeMenuWindow;
        public Console EscapeMenuConsole;
        public Window StatusWindow;


        public Button hostButton;
        public Button closeButton;
        public Button joinButton;

        public ControlsConsole joinPrompt;
        public TextBox joinBox;

        public Window LoadWindow;
        public Console LoadConsole;
        public string selectedWorldName = "";
        public bool tryDelete = false;


        public List<Player> currPlayerChars;

        public bool NoWorldChar = false;

        public Color[] skinColors;
        public int skinIndex;
        public Color[] hairColors;
        public int hairIndex;
        public int hairStyleIndex;
        public TextBox PlayerNameBox;
        public Player characterToLoad;
        

        public Point Mouse;
        public string STATE = "MENU";
        public string JOIN_CODE = "";

       
        public bool chat = false;
        public long tempUID = 0;
        public int invContextIndex = -1;

        public List<Entity> rain = new List<Entity>();
        public int raindrops = 0;

        public string waitingForCommand = "";
        public Point viewOffset = new Point(0, 0);
        public Font hold = GameLoop.MapQuarter;
        public int zoom = 1;

        public UIManager() {
            IsVisible = true;
            IsFocused = true;

            Parent = SadConsole.Global.CurrentScreen;
        }

        public void checkResize(int newX, int newY) {
            this.Resize(newX, newY, false);

            if (MapConsole != null) {
                ResizeMap();

                MapConsole.IsDirty = true;
                MapWindow.IsDirty = true;
                CenterOnActor(GameLoop.World.players[GameLoop.NetworkingManager.myUID]);

                int windowPixelsWidth = SadConsole.Game.Instance.Window.ClientBounds.Width - (240);
                int windowPixelsHeight = SadConsole.Game.Instance.Window.ClientBounds.Height - (240);

                int newW = (windowPixelsWidth / 12);
                int newH = (windowPixelsHeight / 12);

                MapWindow.Resize(newW, newH, false, new Rectangle(0, 0, newW, newH));
                MapWindow.Title = "[GAME MAP]".Align(HorizontalAlignment.Center, GameLoop.GameWidth - 22, (char)205);

                StatusWindow.Position = new Point(MapWindow.Width, 0);
                MessageLog.Position = new Point(0, MapWindow.Height);

                SyncMapEntities();
            }
        }
        
        public void Init() {
            UseMouse = true;
            LoadSplash();

            IsFocused = true;

            joinPrompt = new ControlsConsole(8, 1, Global.FontDefault);
            joinPrompt.IsVisible = true;
            joinPrompt.Position = new Point(42, 53);

            joinBox = new TextBox(5);
            joinBox.Position = new Point(0,0);
            joinBox.UseKeyboard = true;
            joinBox.IsVisible = true;
 
            joinPrompt.Add(joinBox);
            
            Children.Add(joinPrompt);

            joinPrompt.FocusOnMouseClick = true;


            
            EscapeMenuWindow = new Window(20, 20, Global.FontDefault);
            EscapeMenuWindow.Position = new Point(GameLoop.GameWidth / 2 - 10, GameLoop.GameHeight / 2 - 10);
            EscapeMenuWindow.CanDrag = true;
            EscapeMenuWindow.IsVisible = false;
            EscapeMenuWindow.UseMouse = true;
            EscapeMenuWindow.Title = "[MENU]".Align(HorizontalAlignment.Center, 38, (char)205);

            EscapeMenuConsole = new Console(38, 18, Global.FontDefault);
            EscapeMenuConsole.Position = new Point(1, 1);
            EscapeMenuConsole.IsVisible = false;

            EscapeMenuWindow.Children.Add(EscapeMenuConsole);
            Children.Add(EscapeMenuWindow);
        }

        public override void Update(TimeSpan timeElapsed) {
            CheckKeyboard();

            base.Update(timeElapsed);
            if (STATE == "GAME") {
                GameLoop.World.CalculateFov(GameLoop.CommandManager.lastPeek);


                if (StatusWindow.IsVisible) { UpdateStatusWindow(); }

            }

            if (STATE == "MENU") {
                SplashAnim();
            }


        }

        public void CreateConsoles() {
            MapConsole = new ScrollingConsole(60, (GameLoop.GameHeight / 3) * 2); 
            StatusConsole = new Console(20, 40);
        }


        public void CreateStatusWindow(int width, int height, string title) {
            StatusWindow = new Window(width, height);

            int statusConsoleWidth = width - 2;
            int statusConsoleHeight = height - 2;

            StatusConsole.Position = new Point(1, 1);

            StatusWindow.Title = title.Align(HorizontalAlignment.Center, statusConsoleWidth, (char) 205);
            StatusWindow.Children.Add(StatusConsole);
            StatusWindow.Position = new Point(60, 0);

            Children.Add(StatusWindow);

            StatusWindow.CanDrag = true;
            StatusWindow.IsVisible = true;
        }

       

        public void UpdateStatusWindow() { 
            StatusConsole.Clear();

            Player player = null;

            if (GameLoop.World.players.ContainsKey(GameLoop.NetworkingManager.myUID)) {
                player = GameLoop.World.players[GameLoop.NetworkingManager.myUID];
            }

            

            StatusConsole.Print(0, 3, (("" + (char) 205).Align(HorizontalAlignment.Center, 18, (char) 205)).CreateColored(GameLoop.CyberBlue));


            if (player != null) {
                
                StatusConsole.Print(0, 10, (("" + (char)205).Align(HorizontalAlignment.Center, 18, (char)205)).CreateColored(GameLoop.CyberBlue));


                Point Mouse = GameLoop.MouseLoc.PixelLocationToConsole(12, 12);
                Mouse = Mouse - player.PositionOffset - new Point(1, 1) ;
                TileBase hovered = GameLoop.World.CurrentMap.GetTileAt<TileBase>(Mouse.X, Mouse.Y);

                StatusConsole.Print(0, 19, "GROUND".Align(HorizontalAlignment.Center, 18, (char)205).CreateColored(GameLoop.CyberBlue));
                if (hovered != null && hovered.Background.A == 255 && hovered.IsVisible) {
                    ColoredString hovName = new ColoredString(hovered.Name.Align(HorizontalAlignment.Center, 18, ' '), hovered.Foreground, Color.Transparent);
                    StatusConsole.Print(0, 20, hovName);
                    

                    int drawLineAt = 21;

                    List<TerrainFeature> tfsAtMouse = GameLoop.World.CurrentMap.GetEntitiesAt<TerrainFeature>(Mouse);
                    if (tfsAtMouse.Count > 0) {
                        drawLineAt++;
                        StatusConsole.Print(0, drawLineAt, "FEATURES".Align(HorizontalAlignment.Center, 18, (char)205).CreateColored(GameLoop.CyberBlue));
                        drawLineAt++;
                    }

                    for (int i = 0; i < tfsAtMouse.Count; i++) {
                        ColoredString tfCol = new ColoredString(tfsAtMouse[i].Name.Align(HorizontalAlignment.Center, 18, ' '), tfsAtMouse[i].Animation.CurrentFrame[0].Foreground, Color.Transparent);
                        StatusConsole.Print(0, drawLineAt, tfCol);
                        drawLineAt++;
                    }


                    List<Actor> monstersAtTile = GameLoop.World.CurrentMap.GetEntitiesAt<Actor>(Mouse);

                    if (monstersAtTile.Count > 0) {
                        drawLineAt++;
                        StatusConsole.Print(0, drawLineAt, "ENTITIES".Align(HorizontalAlignment.Center, 18, (char)205).CreateColored(GameLoop.CyberBlue));
                        drawLineAt++;
                    }

                    for (int i = 0; i < monstersAtTile.Count; i++) {
                        Color entHealth = Color.White;

                        if (monstersAtTile[i].HealthState == "healthy") { entHealth = Color.Green; }
                        else if (monstersAtTile[i].HealthState == "injured") { entHealth = Color.Yellow; } 
                        else if (monstersAtTile[i].HealthState == "dying") { entHealth = Color.Red; }


                        ColoredString enHealth = new ColoredString(monstersAtTile[i].Name.Align(HorizontalAlignment.Center, 18, ' '), entHealth, Color.Transparent);
                        StatusConsole.Print(0, drawLineAt, enHealth);
                        drawLineAt++;
                    }

                }
            }
        }

        public void ResizeMap() {
            int cellX = 12 * zoom;
            int cellY = 12 * zoom;

            int windowPixelsWidth = SadConsole.Game.Instance.Window.ClientBounds.Width - (264);
            int windowPixelsHeight = SadConsole.Game.Instance.Window.ClientBounds.Height - (264);

            int newW = (windowPixelsWidth / cellX);
            int newH = (windowPixelsHeight / cellY);

            //MapConsole.Resize(newW, newH, false);
            MapConsole.ViewPort = new Rectangle(0, 0, newW, newH);

            CenterOnActor(GameLoop.World.players[GameLoop.NetworkingManager.myUID]);
            
            MapConsole.IsDirty = true;
        }

        public void CreateMapWindow(int width, int height, string title, bool reset = false) {
            MapWindow = new Window(width, height);

            int mapConsoleWidth = width - 2;
            int mapConsoleHeight = height - 2;


            MapConsole.ViewPort = new Rectangle(0, 0, mapConsoleWidth, mapConsoleHeight);
            MapConsole.Position = new Point(1, 1);
            
            
            MapWindow.Title = title.Align(HorizontalAlignment.Center, mapConsoleWidth, (char) 205);
            MapWindow.Children.Add(MapConsole);
            
            Children.Add(MapWindow);
            
            MapConsole.MouseButtonClicked += mapClick;

            MapWindow.CanDrag = true;
            MapWindow.IsVisible = true;

            
        }
        
        private void mapClick(object sender, MouseEventArgs e) {
            if (GameLoop.World.players.ContainsKey(GameLoop.NetworkingManager.myUID)) {
                Player player = GameLoop.World.players[GameLoop.NetworkingManager.myUID];
                Point offset = player.PositionOffset;
                Point modifiedClick = e.MouseState.ConsoleCellPosition - offset;

                int range = (int) Distance.CHEBYSHEV.Calculate(player.Position, modifiedClick);
                Monster monster = GameLoop.World.CurrentMap.GetEntityAt<Monster>(modifiedClick); 

                if (monster != null) {
                   
                } else {
                    if (range <= 1) {
                        TileBase tile = GameLoop.World.CurrentMap.GetTileAt<TileBase>(modifiedClick.X, modifiedClick.Y);
                        

                        if (tile is TileDoor door) {
                            if (!door.IsOpen)
                                GameLoop.CommandManager.OpenDoor(player, door, modifiedClick);
                            else
                                GameLoop.CommandManager.CloseDoor(player, modifiedClick, true);

                        }
                    }
                }
            }
        }

          
        private void hostButtonClick(object sender, SadConsole.Input.MouseEventArgs e) {
            GameLoop.NetworkingManager.changeClientTarget("0"); // HAS TO BE DISABLED ON LIVE BUILD, ONLY FOR TESTING TWO CLIENTS ON ONE COMPUTER

            var lobbyManager = GameLoop.NetworkingManager.discord.GetLobbyManager(); 
            string possibleChars = "ABCDEFGHIJKLMNPQRSTUVWXYZ0123456789";
            string code = "";
            
            code = "";

            for (int i = 0; i < 4; i++) {
                code += possibleChars[GameLoop.Random.Next(0, possibleChars.Length)];
            }



            var txn = lobbyManager.GetLobbyCreateTransaction();
            txn.SetCapacity(6);
            txn.SetType(Discord.LobbyType.Public);
            txn.SetMetadata("code", code);


            lobbyManager.CreateLobby(txn, (Result result, ref Lobby lobby) => {
                if (result == Result.Ok) {
                    MessageLog.Add("Created lobby! Code has been copied to clipboard.");
                    

                    GameLoop.NetworkingManager.InitNetworking(lobby.Id);
                    lobbyManager.OnMemberConnect += onPlayerConnected;
                    lobbyManager.OnMemberDisconnect += onPlayerDisconnected;
                    
                    JOIN_CODE = code;

                    EscapeMenuWindow.Title = ("[MENU: " + code + "]").Align(HorizontalAlignment.Center, EscapeMenuWindow.Width, (char) 205);
                } else {
                    MessageLog.Add("Error: " + result);
                }
            });
        }

        private void onPlayerDisconnected(long lobbyId, long userId) {
            var userManager = GameLoop.NetworkingManager.discord.GetUserManager();
            userManager.GetUser(userId, (Result result, ref User user) => {
                if (result == Discord.Result.Ok) {
                    MessageLog.Add("User disconnected: " + user.Username);
                    GameLoop.World.CurrentMap.Remove(GameLoop.World.players[user.Id]);
                    GameLoop.World.players.Remove(user.Id);
                }
            });
        }

        private void onPlayerConnected(long lobbyId, long userId) {
            var userManager = GameLoop.NetworkingManager.discord.GetUserManager();
            userManager.GetUser(userId, (Result result, ref User user) => {
                if (result == Discord.Result.Ok) {
                    MessageLog.Add("User connected: " + user.Username);
                    kickstartNet();

                    GameLoop.World.CreatePlayer(userId, new Player(Color.Yellow, Color.Transparent));
                    SyncMapEntities(true);
                    GameLoop.NetworkingManager.SendNetMessage(2, System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(GameLoop.World, Formatting.Indented, new WorldJsonConverter())));

                    
                    MapConsole.IsDirty = true;
                }
            });
        }

        private void kickstartNet() {
            GameLoop.NetworkingManager.SendNetMessage(0, System.Text.Encoding.UTF8.GetBytes("a"));
            GameLoop.NetworkingManager.SendNetMessage(1, System.Text.Encoding.UTF8.GetBytes("a"));
            GameLoop.NetworkingManager.SendNetMessage(2, System.Text.Encoding.UTF8.GetBytes("a"));
        }

        private void joinButtonClick(object sender, SadConsole.Input.MouseEventArgs e) {
            GameLoop.NetworkingManager.changeClientTarget("1"); // HAS TO BE DISABLED ON LIVE BUILD, ONLY FOR TESTING TWO CLIENTS ON ONE COMPUTER

            joinBox.IsFocused = false;
            

            var lobbyManager = GameLoop.NetworkingManager.discord.GetLobbyManager();
            LobbySearchQuery searchQ = lobbyManager.GetSearchQuery();
            searchQ.Filter("metadata.code", LobbySearchComparison.Equal, LobbySearchCast.String, joinBox.Text);

            string acSec = "";

            lobbyManager.Search(searchQ, (resultSearch) => {
                if (resultSearch == Discord.Result.Ok) {
                    var count = lobbyManager.LobbyCount();

                    acSec = lobbyManager.GetLobbyActivitySecret(lobbyManager.GetLobbyId(0));



                    lobbyManager.ConnectLobbyWithActivitySecret(acSec, (Result result, ref Lobby lobby) => {
                        if (result == Discord.Result.Ok) {
                            MessageLog.Add("Connected to lobby successfully!");
                            GameLoop.NetworkingManager.InitNetworking(lobby.Id);
                            kickstartNet();
                            GameLoop.World.CreatePlayer(GameLoop.NetworkingManager.myUID, new Player(Color.Yellow, Color.Transparent));
                        } else {
                            MessageLog.Add("Encountered error: " + result);
                        }
                    });
                }
            });
        }

        public void CenterOnActor(Actor actor) {
            MapConsole.CenterViewPortOnPoint(actor.Position);
            RefreshMap(actor.Position);
        }

        public void RefreshMap(Point pos) {
            MapConsole.SetSurface(GameLoop.World.CurrentMap.GetTileRegion(pos), 101, 101);
            MapConsole.IsDirty = true;

            MapConsole.ViewPort = MapConsole.ViewPort;
        }

        public void SyncMapEntities(bool keepOldView = false) {
            MapConsole.Children.Clear();

            foreach (Entity entity in GameLoop.World.CurrentMap.Entities.Items) {
                MapConsole.Children.Add(entity);
            }

            foreach (KeyValuePair<long, Player> player in GameLoop.World.players) {
                MapConsole.Children.Add(player.Value);
            }

            GameLoop.World.CurrentMap.Entities.ItemAdded += OnMapEntityAdded;
            GameLoop.World.CurrentMap.Entities.ItemRemoved += OnMapEntityRemoved;

            GameLoop.World.ResetFOV(keepOldView);
        }

        public void OnMapEntityAdded (object sender, GoRogue.ItemEventArgs<Entity> args) {
            MapConsole.Children.Add(args.Item);
        }

        public void OnMapEntityRemoved(object sender, GoRogue.ItemEventArgs<Entity> args) {
            MapConsole.Children.Remove(args.Item);
        }

        public void LoadMap(Map map) {
            GameLoop.World.CurrentMap = map;

            Point center = new Point(0, 0);

            if (GameLoop.World.players.ContainsKey(GameLoop.NetworkingManager.myUID)) {
                center = GameLoop.World.players[GameLoop.NetworkingManager.myUID].Position;
            }



            MapConsole.Clear();
            MapConsole.Children.Clear();


            //MapConsole = new ScrollingConsole(map.Width, map.Height, Global.FontDefault, new Rectangle(0, 0, MapWindow.Width - 2, MapWindow.Height - 2), GameLoop.World.CurrentMap.Tiles);
            MapConsole = new ScrollingConsole(101, 101, Global.FontDefault, new Rectangle(0, 0, MapWindow.Width - 2, MapWindow.Height - 2), GameLoop.World.CurrentMap.GetTileRegion(center));
            MapConsole.Parent = MapWindow;
            MapConsole.Position = new Point(1, 1);
            MapConsole.MouseButtonClicked += mapClick;

            SyncMapEntities(false);
        }

        public void InitNewMap(string worldName = "Unnamed", bool justInit = false) {
            GameLoop.World.WorldName = worldName;


            CreateConsoles();

            MessageLog = new MessageLogWindow(60, 20, "[MESSAGE LOG]");
            MessageLog.Title.Align(HorizontalAlignment.Center, MessageLog.Title.Length);
            Children.Add(MessageLog);
            MessageLog.IsVisible = false;
            MessageLog.Position = new Point(0, (GameLoop.GameHeight / 3) * 2); 
            
            CreateMapWindow(GameLoop.GameWidth - 20, GameLoop.GameHeight - 20, "[GAME MAP]");
            CreateStatusWindow(20, (GameLoop.GameHeight / 3) * 2, "[PLAYER INFO]"); 


            MapWindow.IsVisible = false;
            MapConsole.IsVisible = false;
            StatusWindow.IsVisible = false;
            StatusConsole.IsVisible = false;



            if (!justInit) {
                GameLoop.World.CreatePlayer(GameLoop.NetworkingManager.myUID, new Player(Color.Yellow, Color.Transparent));
                LoadMap(GameLoop.World.CurrentMap);
                MapConsole.Font = GameLoop.RegularSize;
                CenterOnActor(GameLoop.World.players[GameLoop.NetworkingManager.myUID]);
                ChangeState("GAME");
            }
        }

        public void SplashAnim() {
            if (SplashConsole.IsVisible) {
                if (raindrops < 100 && GameLoop.Random.Next(0, 10) < 7) {
                    raindrops++;

                    Entity newRaindrop = new Entity(new Color(GameLoop.CyberBlue, GameLoop.Random.Next(170, 256)), Color.Transparent, '\\', 2, 2);
                    newRaindrop.Font = GameLoop.RegularSize;
                    int hori = GameLoop.Random.Next(0, 4);
                    if (hori == 0 || hori == 1) {
                        newRaindrop.Position = new Point(0, GameLoop.Random.Next(0, SplashConsole.Height - 14));
                    } else {
                        newRaindrop.Position = new Point(GameLoop.Random.Next(0, SplashConsole.Width - 2), 0);
                    }
                    rain.Add(newRaindrop);

                    SplashRain.Children.Add(newRaindrop);
                }


                for (int i = rain.Count - 1; i >= 0; i--) {
                    if (rain[i].Position.X + 1 < SplashConsole.Width - 2 && rain[i].Position.Y + 1 < SplashConsole.Height - 14) {
                        rain[i].Position += new Point(1, 1);
                    } else {
                        int hori = GameLoop.Random.Next(0, 4);
                        if (hori == 0 || hori == 1) {
                            rain[i].Position = new Point(0, GameLoop.Random.Next(0, SplashConsole.Height - 14));
                        } else {
                            rain[i].Position = new Point(GameLoop.Random.Next(0, SplashConsole.Width - 2), 0);
                        }
                    }
                }
            }
        }


        public void LoadSplash() {

            SadRex.Image splashScreen = SadRex.Image.Load(new FileStream("./data/splash.xp", FileMode.Open));
            Cell[] splashCells = new Cell[splashScreen.Width * splashScreen.Height];
            int logoCount = 0;
            int lastY = 2;

            for (int i = 0; i < splashScreen.Layers[0].Cells.Count; i++) {
                SadRex.Cell temp = splashScreen.Layers[0].Cells[i];
                Color fore = new Color(temp.Foreground.R, temp.Foreground.G, temp.Foreground.B);
                Color back = new Color(temp.Background.R, temp.Background.G, temp.Background.B);

                if (fore == new Color(17, 17, 17) || fore == new Color(26, 26, 26)) { fore.A = 150; } 
                if (fore == new Color(35, 35, 35) || fore == new Color(48, 48, 48)) { fore.A = 170; } 
                if (fore == new Color(69, 69, 69) || fore == new Color(78, 78, 78)) { fore.A = 190; } 
                if (fore == new Color(255, 0, 255)) { fore.A = 0; } 
                if (back == new Color(255, 0, 255)) {  back.A = 0; }

                ColorGradient splashGrad = new ColorGradient(Color.DeepPink, Color.Cyan);
                Point cellPos = i.ToPoint(splashScreen.Width);

                if (fore == new Color(178, 0, 178)) {
                    if (lastY < cellPos.Y) {
                        logoCount++;
                        lastY = cellPos.Y;
                    }
                    fore = splashGrad.Lerp((float)logoCount / 5);
                }

                splashCells[i] = new Cell(fore, back, temp.Character);
            }

            SplashConsole = new Console(splashScreen.Width, splashScreen.Height, GameLoop.RegularSize, splashCells);
            SplashConsole.IsVisible = true;
            Children.Add(SplashConsole);

            Cell[] rain = new Cell[(SplashConsole.Width - 2) * (SplashConsole.Height - 14)];

            for (int i = 0; i < rain.Length; i++) {
                rain[i] = new Cell(Color.Transparent, Color.Transparent, ' ');
            }

            SplashRain = new Console(SplashConsole.Width - 2, SplashConsole.Height - 14, Global.FontDefault, rain);
            SplashRain.Position = new Point(1, 1);
            SplashConsole.Children.Add(SplashRain);


            Cell[] menu = new Cell[(SplashConsole.Width - 2) * 12];

            for (int i = 0; i < menu.Length; i++) {
                menu[i] = new Cell(Color.White, Color.Transparent, ' ');
            }

            MenuConsole = new Console(SplashConsole.Width - 2, 12, GameLoop.RegularSize, menu);
            SplashConsole.Children.Add(MenuConsole);

            MenuConsole.Position = new Point(1, SplashConsole.Height - 12);
            // MenuConsole.Print(1, 1, "CREATE NEW WORLD".Align(HorizontalAlignment.Center, 76, ' ').CreateColored(GameLoop.CyberBlue, Color.Transparent));
            // MenuConsole.Print(1, 2, "LOAD WORLD".Align(HorizontalAlignment.Center, 76, ' ').CreateColored(GameLoop.CyberBlue, Color.Transparent));
            // MenuConsole.Print(1, 3, "CREATE CHARACTER".Align(HorizontalAlignment.Center, 76, ' ').CreateColored(GameLoop.CyberBlue, Color.Transparent));

            MenuConsole.Print(1, 1, "START LOBBY".Align(HorizontalAlignment.Center, 76, ' ').CreateColored(GameLoop.CyberBlue, Color.Transparent));
            MenuConsole.Print(1, 2, "FIND LOBBY".Align(HorizontalAlignment.Center, 76, ' ').CreateColored(GameLoop.CyberBlue, Color.Transparent));

            MenuConsole.Print(1, 5, "JOIN:    ".Align(HorizontalAlignment.Center, 76, ' ').CreateColored(GameLoop.CyberBlue, Color.Transparent));

            MenuConsole.Print(1, 10, "EXIT GAME".Align(HorizontalAlignment.Center, 76, ' ').CreateColored(GameLoop.CyberBlue, Color.Transparent));

            MenuConsole.MouseButtonClicked += menuClick; 
        }

        
        private void SaveEverything(bool alsoSavePlayers = false) {
            Directory.CreateDirectory(@"./saves/worlds/" + GameLoop.World.WorldName);

            // string map = Utils.SimpleMapString(GameLoop.World.CurrentMap.Tiles);

            string map = JsonConvert.SerializeObject(GameLoop.World, Formatting.Indented, new WorldJsonConverter());

            File.WriteAllText(@"./saves/worlds/" + GameLoop.World.WorldName + "/map.json", map);

            if (alsoSavePlayers) {
                Directory.CreateDirectory(@"./saves/players");

                foreach (KeyValuePair<long, Player> player in GameLoop.World.players) {
                    string playerJson = JsonConvert.SerializeObject(player.Value, Formatting.Indented, new ActorJsonConverter());
                    File.WriteAllText(@"./saves/players/" + player.Value.Name + "+" + player.Key + ".json", playerJson);
                }
            }
        }

        

        //Directory.CreateDirectory(@"./saves/players"); 
        //string playerJson = JsonConvert.SerializeObject(CreatingChar, Formatting.Indented, new ActorJsonConverter()); 
        //File.WriteAllText(@"./saves/players/" + CreatingChar.Name + "+" + GameLoop.NetworkingManager.myUID + ".json", playerJson);


        private void menuClick(object sender, MouseEventArgs e) {

            if (Utils.PointInArea(new Point(31, 1), new Point(47, 1), e.MouseState.ConsoleCellPosition)) { // Should open world creation dialogue 
                InitNewMap("world", false);
            }


            else if (Utils.PointInArea(new Point(35, 5), new Point(39, 5), e.MouseState.ConsoleCellPosition)) { // This one is probably fine like this, but should be switched so it doesn't make its own world before joining.
                InitNewMap("", true);
                ChangeState("GAME");

                joinButtonClick(null, null);
            }
            
            else if (Utils.PointInArea(new Point(35, 10), new Point(43, 10), e.MouseState.ConsoleCellPosition)) { // Should open a dialogue that lets the player specify whether to use a new world or load an existing world
                SadConsole.Game.Instance.Exit();
            }
        }

        private void ClearWait(Actor actor) {
            waitingForCommand = "";
            GameLoop.CommandManager.ResetPeek(actor);
        }

        private void ChangeState(string newState) {
            STATE = newState;
            if (STATE == "MENU") {
                MapConsole.IsVisible = false;
                MapWindow.IsVisible = false;


                StatusConsole.IsVisible = false;
                StatusWindow.IsVisible = false;


                SplashConsole.IsVisible = true;
                SplashRain.IsVisible = true;
                joinBox.IsVisible = true;
                joinPrompt.IsVisible = true;
            }

            if (STATE == "GAME") {
                MapConsole.IsVisible = true;
                MapWindow.IsVisible = true;
                MessageLog.IsVisible = true;
                
                StatusConsole.IsVisible = true;
                StatusWindow.IsVisible = true;
                
                SplashConsole.IsVisible = false;
                SplashRain.IsVisible = false;
                joinBox.IsVisible = false;
                joinPrompt.IsVisible = false;


            }
        }


        private void CheckKeyboard() {
            if (STATE == "GAME") {
                
                if (Global.KeyboardState.IsKeyReleased(Keys.F5)) { Settings.ToggleFullScreen(); }

                if (GameLoop.World.players.ContainsKey(GameLoop.NetworkingManager.myUID)) {
                    Player player = GameLoop.World.players[GameLoop.NetworkingManager.myUID];
                    if (Global.KeyboardState.IsKeyPressed(Keys.G)) {
                        waitingForCommand = "g";
                    }

                    if (Global.KeyboardState.IsKeyPressed(Keys.X)) {
                        if (GameLoop.CommandManager.lastPeek == new Point(0, 0)) {
                            waitingForCommand = "x";
                        } else {
                            ClearWait(player);
                        }
                    }

                    if (Global.KeyboardState.IsKeyPressed(Keys.C)) {
                        waitingForCommand = "c";
                    }

                    if (Global.KeyboardState.IsKeyReleased(Keys.OemTilde)) {
                        SaveEverything(true);
                    }

                    if (Global.KeyboardState.IsKeyPressed(Keys.S) && Global.KeyboardState.IsKeyDown(Keys.LeftShift)) {
                        if (!player.IsCrouched) {
                            player.IsCrouched = true;
                            GameLoop.NetworkingManager.SendNetMessage(0, System.Text.Encoding.UTF8.GetBytes("stealth|yes|" + GameLoop.NetworkingManager.myUID));
                        } else {
                            player.IsCrouched = false;
                            GameLoop.NetworkingManager.SendNetMessage(0, System.Text.Encoding.UTF8.GetBytes("stealth|no|" + GameLoop.NetworkingManager.myUID));
                        }
                    }

                    if (Global.KeyboardState.IsKeyReleased(Keys.H)) {
                        if (player.HealthState == "healthy") { player.HealthState = "injured"; }
                        else if (player.HealthState == "injured") { player.HealthState = "dying"; }
                    }

                    if (Global.KeyboardState.IsKeyReleased(Keys.Escape)) {
                        if (EscapeMenuWindow.IsVisible) {
                            EscapeMenuWindow.IsVisible = false;
                            EscapeMenuConsole.IsVisible = false;
                        } else {
                            EscapeMenuWindow.IsVisible = true;
                            EscapeMenuConsole.IsVisible = true;
                        }
                    }

                    if (Global.KeyboardState.IsKeyReleased(Keys.OemPlus)) {
                        if (zoom != 8) {
                            switch (zoom) {
                                case 1:
                                    MapConsole.Font = GameLoop.MapHalf;
                                    hold = GameLoop.MapHalf;
                                    zoom = 2;
                                    ResizeMap();
                                    break;
                                case 2:
                                    MapConsole.Font = GameLoop.MapOne;
                                    hold = GameLoop.MapOne;
                                    zoom = 4;
                                    ResizeMap();
                                    break;
                                case 4:
                                    MapConsole.Font = Global.Fonts["Cheepicus48"].GetFont(Font.FontSizes.Two);
                                    hold = Global.Fonts["Cheepicus48"].GetFont(Font.FontSizes.Two);
                                    zoom = 8;
                                    ResizeMap();
                                    break;
                            }

                            foreach (Entity entity in GameLoop.World.CurrentMap.Entities.Items) {
                                entity.UpdateFontSize(hold.SizeMultiple);
                                entity.Position = entity.Position;
                                entity.IsDirty = true;
                            }

                            foreach (KeyValuePair<long, Player> entity in GameLoop.World.players) {
                                entity.Value.UpdateFontSize(hold.SizeMultiple);
                                entity.Value.Position = entity.Value.Position;
                                entity.Value.IsDirty = true;
                            }

                            if (GameLoop.World.players.ContainsKey(GameLoop.NetworkingManager.myUID)) {
                                CenterOnActor(GameLoop.World.players[GameLoop.NetworkingManager.myUID]);
                            }

                            MapConsole.IsDirty = true;
                        }
                    }

                    if (Global.KeyboardState.IsKeyReleased(Keys.OemMinus)) {
                        if (zoom != 1) {
                            switch (zoom) {
                                case 8:
                                    MapConsole.Font = GameLoop.MapOne;
                                    hold = GameLoop.MapOne;
                                    zoom = 4;
                                    ResizeMap();
                                    break;
                                case 4: 
                                    MapConsole.Font = GameLoop.MapHalf;
                                    hold = GameLoop.MapHalf;
                                    zoom = 2;
                                    ResizeMap();
                                    break;
                                case 2:
                                    MapConsole.Font = GameLoop.MapQuarter;
                                    hold = GameLoop.MapQuarter;
                                    zoom = 1;
                                    ResizeMap();
                                    break;
                            }

                            foreach (Entity entity in GameLoop.World.CurrentMap.Entities.Items) {
                                entity.UpdateFontSize(hold.SizeMultiple);
                                entity.Position = entity.Position;
                                entity.IsDirty = true;
                            }

                            foreach (KeyValuePair<long, Player> entity in GameLoop.World.players) {
                                entity.Value.UpdateFontSize(hold.SizeMultiple);
                                entity.Value.Position = entity.Value.Position;
                                entity.Value.IsDirty = true;
                            }


                            if (GameLoop.World.players.ContainsKey(GameLoop.NetworkingManager.myUID)) {
                                CenterOnActor(GameLoop.World.players[GameLoop.NetworkingManager.myUID]);
                            }

                            MapConsole.IsDirty = true;

                        }
                    }



                    if (player.TimeLastActed + (UInt64)player.Speed <= GameLoop.GameTime) {
                        if (Global.KeyboardState.IsKeyPressed(Keys.NumPad9)) {
                            Point thisDir = Utils.Directions["UR"];
                            if (waitingForCommand == "") {
                                ClearWait(player);
                                GameLoop.CommandManager.MoveActorBy(player, thisDir);
                                CenterOnActor(player);
                            } else if (waitingForCommand == "c") {
                                ClearWait(player);
                                GameLoop.CommandManager.CloseDoor(player, thisDir);
                            } else if (waitingForCommand == "x") {
                                ClearWait(player);
                                GameLoop.CommandManager.Peek(player, thisDir);
                            }

                        } else if (Global.KeyboardState.IsKeyPressed(Keys.W) || Global.KeyboardState.IsKeyPressed(Keys.NumPad8)) {
                            Point thisDir = Utils.Directions["U"];
                            if (waitingForCommand == "") {
                                ClearWait(player);
                                GameLoop.CommandManager.MoveActorBy(player, thisDir);
                                CenterOnActor(player);
                            } else if (waitingForCommand == "c") {
                                ClearWait(player);
                                GameLoop.CommandManager.CloseDoor(player, thisDir);
                            } else if (waitingForCommand == "x") {
                                ClearWait(player);
                                GameLoop.CommandManager.Peek(player, thisDir);
                            }

                        } else if (Global.KeyboardState.IsKeyPressed(Keys.NumPad7)) {
                            Point thisDir = Utils.Directions["UL"];
                            if (waitingForCommand == "") {
                                ClearWait(player);
                                GameLoop.CommandManager.MoveActorBy(player, thisDir);
                                CenterOnActor(player);
                            } else if (waitingForCommand == "c") {
                                ClearWait(player);
                                GameLoop.CommandManager.CloseDoor(player, thisDir);
                            } else if (waitingForCommand == "x") {
                                ClearWait(player);
                                GameLoop.CommandManager.Peek(player, thisDir);
                            }

                        } else if (Global.KeyboardState.IsKeyPressed(Keys.D) || Global.KeyboardState.IsKeyPressed(Keys.NumPad6)) {
                            Point thisDir = Utils.Directions["R"];
                            if (waitingForCommand == "") {
                                ClearWait(player);
                                GameLoop.CommandManager.MoveActorBy(player, thisDir);
                                CenterOnActor(player);
                            } else if (waitingForCommand == "c") {
                                ClearWait(player);
                                GameLoop.CommandManager.CloseDoor(player, thisDir);
                            } else if (waitingForCommand == "x") {
                                ClearWait(player);
                                GameLoop.CommandManager.Peek(player, thisDir);
                            }
                        } else if (Global.KeyboardState.IsKeyPressed(Keys.NumPad5)) {
                            Point thisDir = Utils.Directions["C"];
                            if (waitingForCommand == "") {
                                ClearWait(player);
                                GameLoop.CommandManager.MoveActorBy(player, thisDir);
                                CenterOnActor(player);
                            } else if (waitingForCommand == "c") {
                                ClearWait(player);
                                GameLoop.CommandManager.CloseDoor(player, thisDir);
                            } else if (waitingForCommand == "x") {
                                ClearWait(player);
                                GameLoop.CommandManager.Peek(player, thisDir);
                            }
                        } else if (Global.KeyboardState.IsKeyPressed(Keys.A) || Global.KeyboardState.IsKeyPressed(Keys.NumPad4)) {
                            Point thisDir = Utils.Directions["L"];
                            if (waitingForCommand == "") {
                                ClearWait(player);
                                GameLoop.CommandManager.MoveActorBy(player, thisDir);
                                CenterOnActor(player);
                            } else if (waitingForCommand == "c") {
                                ClearWait(player);
                                GameLoop.CommandManager.CloseDoor(player, thisDir);
                            } else if (waitingForCommand == "x") {
                                ClearWait(player);
                                GameLoop.CommandManager.Peek(player, thisDir);
                            }
                        } else if (Global.KeyboardState.IsKeyPressed(Keys.NumPad3)) {
                            Point thisDir = Utils.Directions["DR"];
                            if (waitingForCommand == "") {
                                ClearWait(player);
                                GameLoop.CommandManager.MoveActorBy(player, thisDir);
                                CenterOnActor(player);
                            } else if (waitingForCommand == "c") {
                                ClearWait(player);
                                GameLoop.CommandManager.CloseDoor(player, thisDir);
                            } else if (waitingForCommand == "x") {
                                ClearWait(player);
                                GameLoop.CommandManager.Peek(player, thisDir);
                            }
                        } else if ((Global.KeyboardState.IsKeyPressed(Keys.S) && !Global.KeyboardState.IsKeyDown(Keys.LeftShift)) || Global.KeyboardState.IsKeyPressed(Keys.NumPad2)) {
                            Point thisDir = Utils.Directions["D"];
                            if (waitingForCommand == "") {
                                ClearWait(player);
                                GameLoop.CommandManager.MoveActorBy(player, thisDir);
                                CenterOnActor(player);
                            } else if (waitingForCommand == "c") {
                                ClearWait(player);
                                GameLoop.CommandManager.CloseDoor(player, thisDir);
                            } else if (waitingForCommand == "x") {
                                ClearWait(player);
                                GameLoop.CommandManager.Peek(player, thisDir);
                            }
                        } else if (Global.KeyboardState.IsKeyPressed(Keys.NumPad1)) {
                            Point thisDir = Utils.Directions["DL"];
                            if (waitingForCommand == "") {
                                ClearWait(player);
                                GameLoop.CommandManager.MoveActorBy(player, thisDir);
                                CenterOnActor(player);
                            } else if (waitingForCommand == "c") {
                                ClearWait(player);
                                GameLoop.CommandManager.CloseDoor(player, thisDir);
                            } else if (waitingForCommand == "x") {
                                ClearWait(player);
                                GameLoop.CommandManager.Peek(player, thisDir);
                            }
                        }

                    }
                }
            } 
        }
    }
}
