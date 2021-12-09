using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using SadConsole;
using Console = SadConsole.Console;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Key = Microsoft.Xna.Framework.Input.Keys;
using Lidgren.Network;

namespace RainfallOnline {
    public class Client {
        public class playerDesc {
            public int x;
            public int y;
            public SadConsole.Entities.Entity player;
        }

        public const int Width = 80;
        public const int Height = 25;
        public static Dictionary<int, playerDesc> players;
        public static int myID = 0;
        public static Console startingConsole;
        public static NetClient client;


        public static void Main(string[] args) {
            // Setup the engine and creat the main window.
            SadConsole.Game.Create(Width, Height);

            // Hook the start event so we can add consoles to the system.
            SadConsole.Game.OnInitialize = Init;

        
            // Hook the update event that happens each frame so we can trap keys and respond.
            SadConsole.Game.OnUpdate = Update;

            // Start the game.
            SadConsole.Game.Instance.Run();

            //
            // Code here will not run until the game window closes.
            //

            SadConsole.Game.Instance.Dispose();
        }

        private static void Update(GameTime time) {
            // Called each logic update.
            // As an example, we'll use the F5 key to make the game full screen

            if (Global.KeyboardState.IsKeyReleased(Key.P)) {
                var createMsg = client.CreateMessage("createPlayer");
                createMsg.Write(myID);
                client.SendMessage(createMsg, NetDeliveryMethod.ReliableUnordered);

            }

            if (Global.KeyboardState.IsKeyReleased(Key.W)) { TryMove(0, -1); } 
            if (Global.KeyboardState.IsKeyReleased(Key.S)) { TryMove(0, 1); }
            if (Global.KeyboardState.IsKeyReleased(Key.A)) { TryMove(-1, 0); }
            if (Global.KeyboardState.IsKeyReleased(Key.D)) { TryMove(1, 0); }


            NetIncomingMessage readMsg;
            while ((readMsg = client.ReadMessage()) != null) {
                switch (readMsg.MessageType) {
                    case NetIncomingMessageType.Data: 
                        var data = readMsg.ReadString();
                        System.Console.WriteLine(data);

                        if (data == "yourID") {
                            myID = readMsg.ReadInt32();
                        }

                        if (data == "createPlayer") {
                            playerDesc player = new playerDesc();
                            player.player = new SadConsole.Entities.Entity(1, 1);
                            player.player.Animation.CurrentFrame[0].Glyph = '@';
                            player.player.Position = new Point(player.x, player.y);
                            startingConsole.Children.Add(player.player);

                            int pID = readMsg.ReadInt32();

                            player.x = readMsg.ReadInt32();
                            player.y = readMsg.ReadInt32();
                            players.Add(pID, player);
                        }

                        if (data == "move") {
                            int pID = readMsg.ReadInt32();

                            if (players.ContainsKey(pID)) {
                                playerDesc moved = players[pID];
                                startingConsole.Children.Remove(players[pID].player);

                                moved.x = readMsg.ReadInt32();
                                moved.y = readMsg.ReadInt32();
                                moved.player.Position = new Point(moved.x, moved.y);
                                startingConsole.Children.Add(players[pID].player);
                                players[pID] = moved;
                            } else {
                                System.Console.WriteLine("Tried to move a player that doesn't exist");
                            }
                        }

                        break;

                    case NetIncomingMessageType.StatusChanged:
                        // handle connection status messages
                        switch (readMsg.SenderConnection.Status) {
                            /* .. */
                        }
                        break;

                    case NetIncomingMessageType.DebugMessage:
                        // handle debug messages
                        // (only received when compiled in DEBUG mode)
                        System.Console.WriteLine(readMsg.ReadString());
                        break;

                    /* .. */
                    default:
                        System.Console.WriteLine("unhandled message with type: "
                            + readMsg.MessageType);
                        break;
                }
            }
        }

        public static void TryMove(int xChange, int yChange) {
            if (players.ContainsKey(myID)) {
                var message = client.CreateMessage("move");
                message.Write(myID);
                message.Write(xChange);
                message.Write(yChange);
                client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
            }
        }

        public static void Init() {
            // Any custom loading and prep. We will use a sample console for now
            players = new Dictionary<int, playerDesc>();

            startingConsole = new Console(Width, Height);


            var config = new NetPeerConfiguration("Rainfall Online");
            client = new NetClient(config);
            client.Start();
            client.Connect(host: "127.0.0.1", port: 12345);

            // Set our new console as the thing to render and process
            SadConsole.Global.CurrentScreen = startingConsole;
        }
    }
}