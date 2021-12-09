using Lidgren.Network;
using System;
using System.Collections.Generic;


namespace ROServer {
    struct playerDesc {
        public int x;
        public int y;
    }


    class Server {
        public static Dictionary<int, playerDesc> players;
        public static int newestID = 10000;


        static void Main(string[] args) {
            var config = new NetPeerConfiguration("Rainfall Online") { Port = 12345 };
            var server = new NetServer(config);
            server.Start();

            Console.WriteLine("[SERVER] Started!");
            players = new Dictionary<int, playerDesc>();

            while (true) {
                NetIncomingMessage message;
                while ((message = server.ReadMessage()) != null) {
                    switch (message.MessageType) {
                        case NetIncomingMessageType.Data:
                            // handle custom messages
                            var data = message.ReadString();
                            if (data == "move") {
                                int pID = message.ReadInt32();

                                if (players.TryGetValue(pID, out playerDesc player)) {
                                    player.x = message.ReadInt32();
                                    player.y = message.ReadInt32();

                                    players[pID] = player;

                                    List<NetConnection> all = server.Connections;

                                    var outMsg = server.CreateMessage("move");
                                    outMsg.Write(pID);
                                    outMsg.Write(player.x);
                                    outMsg.Write(player.y);
                                    server.SendMessage(outMsg, all, NetDeliveryMethod.ReliableOrdered, 0);
                                } else {
                                    Console.WriteLine("[SERVER] Player " + pID + " doesn't exist");
                                }
                            }

                            else if (data == "createPlayer") {
                                int pID = message.ReadInt32();

                                if (!players.ContainsKey(pID)) {
                                    playerDesc newPlayer = new playerDesc();
                                    newPlayer.x = 10;
                                    newPlayer.y = 10;

                                    players.Add(pID, newPlayer);
                                    Console.WriteLine("[SERVER] Created a player entry for " + pID);

                                    foreach(KeyValuePair<int, playerDesc> kvp in players) {
                                        Console.WriteLine(kvp.Key);
                                    }

                                    List<NetConnection> all = server.Connections;

                                    var outMsg = server.CreateMessage("createPlayer"); 
                                    outMsg.Write(pID);
                                    outMsg.Write(newPlayer.x);
                                    outMsg.Write(newPlayer.y);
                                    server.SendMessage(outMsg, all, NetDeliveryMethod.ReliableOrdered, 0);
                                }
                            }

                            else {
                                Console.WriteLine("[SERVER] Received message with initial data: " + data);
                            }


                            break;

                        case NetIncomingMessageType.StatusChanged:
                            // handle connection status messages
                            switch (message.SenderConnection.Status) {
                                case NetConnectionStatus.Connected: {
                                    var idMsg = server.CreateMessage();
                                    idMsg.Write("yourID");
                                    idMsg.Write(newestID);
                                    server.SendMessage(idMsg, message.SenderConnection, NetDeliveryMethod.ReliableOrdered);
                                    Console.WriteLine("[SERVER] Client connected. Assigned ID: " + newestID);
                                    newestID++;
                                    break;
                                }

                                case NetConnectionStatus.Disconnected: {
                                    Console.WriteLine("[SERVER] Client disconnected.");
                                    break;
                                }
                            }
                            break;

                        case NetIncomingMessageType.DebugMessage:
                            // handle debug messages
                            // (only received when compiled in DEBUG mode)
                            Console.WriteLine(message.ReadString());
                            break;

                        /* .. */
                        default:
                            Console.WriteLine("unhandled message with type: "
                                + message.MessageType);
                            break;
                    }
                }
            }
        }

    }
}
