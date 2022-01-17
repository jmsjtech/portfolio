using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using LofiHollow.Entities;
using LofiHollow.Entities.NPC;
using Newtonsoft.Json;
using SadConsole;
using SadRogue.Primitives;

namespace LofiHollow {
	public class NetworkManager {
		public Discord.Discord discord;
		public LobbyManager lobbyManager;
		public UserManager userManager;
		public long lobbyID;
		public long ownID;
		public bool isHost = false;

		public bool HostOwnsFarm = false;

		public NetworkManager(bool second = false) {
			
			if (second) {
				System.Environment.SetEnvironmentVariable("DISCORD_INSTANCE_ID", "1");
				discord = new Discord.Discord(579827348665532425, (UInt64)Discord.CreateFlags.Default);
			} else {
				System.Environment.SetEnvironmentVariable("DISCORD_INSTANCE_ID", "0");
				discord = new Discord.Discord(579827348665532425, (UInt64)Discord.CreateFlags.Default);
			}
			
			//discord = new Discord.Discord(579827348665532425, (UInt64)Discord.CreateFlags.Default);
		} 

		public void BroadcastMsg(string msg) {
			if (lobbyManager != null) {
				for (int i = 0; i < lobbyManager.MemberCount(lobbyID); i++) {
					var userID = lobbyManager.GetMemberUserId(lobbyID, i);
					lobbyManager.SendNetworkMessage(lobbyID, userID, 0, Encoding.UTF8.GetBytes(msg));
				}
			}
		}

		private void ProcessMessage(long lobbyId, long userId, byte channelId, byte[] data) {
			if (lobbyId == lobbyID && userId != ownID) { 
				string msg = Encoding.UTF8.GetString(data);

				string[] splitMsg = msg.Split(";");
				switch (splitMsg[0]) {
					case "createPlayer":
						long id = long.Parse(splitMsg[1]);
						Player newPlayer = JsonConvert.DeserializeObject<Player>(splitMsg[2]);
						if (!GameLoop.World.otherPlayers.ContainsKey(id)) {
							GameLoop.World.otherPlayers.Add(id, newPlayer);
							GameLoop.UIManager.Map.SyncMapEntities(GameLoop.World.maps[GameLoop.World.Player.MapPos]);
						}
						break;
					case "hostFlags":
						bool farm = bool.Parse(splitMsg[1]);



						HostOwnsFarm = farm;
						break;
					case "registerPlayer":
						Player player = JsonConvert.DeserializeObject<Player>(splitMsg[2]);
						if (!GameLoop.World.otherPlayers.ContainsKey(userId)) {
							GameLoop.World.otherPlayers.Add(userId, player);
							GameLoop.UIManager.Map.SyncMapEntities(GameLoop.World.maps[GameLoop.World.Player.MapPos]);
						}
						string idmsg = "yourID;" + userId;
						lobbyManager.SendNetworkMessage(lobbyID, userId, 0, Encoding.UTF8.GetBytes(idmsg));
						break;
					case "requestEntities":
						int requestX = Int32.Parse(splitMsg[1]);
						int requestY = Int32.Parse(splitMsg[2]);
						int requestZ = Int32.Parse(splitMsg[3]);

						Point3D requestPos = new Point3D(requestX, requestY, requestZ);

						if (!GameLoop.World.maps.ContainsKey(requestPos))
							GameLoop.World.LoadMapAt(requestPos);

						if (GameLoop.World.maps.ContainsKey(requestPos)) {
							if (!GameLoop.World.Player.VisitedMaps.Contains(requestPos)) {
								GameLoop.World.maps[requestPos].PopulateMonsters(requestPos);
								GameLoop.World.Player.VisitedMaps.Add(requestPos);
                            }

							foreach (Entity ent in GameLoop.World.maps[requestPos].Entities.Items) {
								string entityMsg = "";
								if (ent is Monster) {
									entityMsg = "spawnMonster;" + JsonConvert.SerializeObject((Monster) ent, Formatting.Indented);
								}

								if (ent is Item) {
									entityMsg = "spawnItem;" + JsonConvert.SerializeObject((Item) ent, Formatting.Indented);
								}

								if (entityMsg != "") { 
									lobbyManager.SendNetworkMessage(lobbyID, userId, 0, Encoding.UTF8.GetBytes(entityMsg));
								}
							}
						}
						break;
					case "yourID":
						ownID = long.Parse(splitMsg[1]);
						GameLoop.UIManager.clientAndConnected = true;
						break;
					case "time":
						GameLoop.World.Player.Clock = JsonConvert.DeserializeObject<TimeKeeper>(splitMsg[1]);
						break;
					case "moveNPC":
						int NPCid = Int32.Parse(splitMsg[1]);
						int posX = Int32.Parse(splitMsg[2]);
						int posY = Int32.Parse(splitMsg[3]);
						int mapX = Int32.Parse(splitMsg[4]);
						int mapY = Int32.Parse(splitMsg[5]);
						int mapZ = Int32.Parse(splitMsg[6]);

						if (GameLoop.World.npcLibrary.ContainsKey(NPCid)) {
							GameLoop.World.npcLibrary[NPCid].MoveTo(new Point(posX, posY), new Point3D(mapX, mapY, mapZ));
                        }
						break;
					case "moveMonster":
						string monMoveID = splitMsg[1];
						int monMoveX = Int32.Parse(splitMsg[2]);
						int monMoveY = Int32.Parse(splitMsg[3]);
						int monMapX = Int32.Parse(splitMsg[4]);
						int monMapY = Int32.Parse(splitMsg[5]);
						int monMapZ = Int32.Parse(splitMsg[6]);
						 
						Point3D monMoveMap = new Point3D(monMapX, monMapY, monMapZ);
						Point monNewPos = new Point(monMoveX, monMoveY);

						GameLoop.CommandManager.MoveMonster(monMoveID, monMoveMap, monNewPos);
						break;
					case "damageMonster":
						string hitId = splitMsg[1];
						int monX = Int32.Parse(splitMsg[2]);
						int monY = Int32.Parse(splitMsg[3]);
						int monZ = Int32.Parse(splitMsg[4]);
						int dmgDealt = Int32.Parse(splitMsg[5]);
						string hitString = splitMsg[6];
						string hitColor = splitMsg[7];

						GameLoop.CommandManager.DamageMonster(hitId, new Point3D(monX, monY, monZ), dmgDealt, hitString, hitColor);
						break;
					case "damagePlayer":
						long playerID = long.Parse(splitMsg[1]); 
						int dmgTaken = Int32.Parse(splitMsg[2]);
						string playerHitString = splitMsg[3];
						string playerHitColor = splitMsg[4];

						GameLoop.CommandManager.DamagePlayer(playerID, dmgTaken, playerHitString, playerHitColor);
						break;
					case "updateTile":
						int tilePosX = Int32.Parse(splitMsg[1]);
						int tilePosY = Int32.Parse(splitMsg[2]);
						int tileMapX = Int32.Parse(splitMsg[3]);
						int tileMapY = Int32.Parse(splitMsg[4]);
						int tileMapZ = Int32.Parse(splitMsg[5]);
						TileBase tile = JsonConvert.DeserializeObject<TileBase>(splitMsg[6]);

						Point3D tileMapPos = new Point3D(tileMapX, tileMapY, tileMapZ);
						Point tilePos = new Point(tilePosX, tilePosY);

						if (!GameLoop.World.maps.ContainsKey(tileMapPos))
							GameLoop.World.LoadMapAt(tileMapPos);

						GameLoop.World.maps[tileMapPos].SetTile(tilePos, tile);
						GameLoop.World.maps[tileMapPos].GetTile(tilePos).UpdateAppearance();
						if (GameLoop.UIManager.Map.FOV.CurrentFOV.Contains(new GoRogue.Coord(tilePos.X, tilePos.Y))) {
							GameLoop.World.maps[tileMapPos].GetTile(tilePos).Unshade();
						} else {
							GameLoop.World.maps[tileMapPos].GetTile(tilePos).Shade(); 
						} 
						break;
					case "spawnItem":
						Item spawnItem = JsonConvert.DeserializeObject<Item>(splitMsg[1]);
						GameLoop.CommandManager.SpawnItem(spawnItem);
						break;
					case "spawnMonster":
						Monster spawnMonster = JsonConvert.DeserializeObject<Monster>(splitMsg[1]);
						GameLoop.CommandManager.SpawnMonster(spawnMonster);
						break;
					case "destroyItem":
						Item destroyItem = JsonConvert.DeserializeObject<Item>(splitMsg[1]);
						GameLoop.CommandManager.DestroyItem(destroyItem);
						break;
					case "movePlayer":
						long moveID = long.Parse(splitMsg[1]);
						int x = Int32.Parse(splitMsg[2]);
						int y = Int32.Parse(splitMsg[3]);
						int mx = Int32.Parse(splitMsg[4]);
						int my = Int32.Parse(splitMsg[5]);
						int mz = Int32.Parse(splitMsg[6]);
						GameLoop.World.otherPlayers[moveID].MoveTo(new Point(x, y), new Point3D(mx, my, mz));

						GameLoop.UIManager.Map.UpdateVision(); 
						break;
					default:
						GameLoop.UIManager.AddMsg(msg);
						break;
				}
			}
		}


		public void CreateLobby() {
			lobbyManager = discord.GetLobbyManager();

			var transaction = lobbyManager.GetLobbyCreateTransaction();
			transaction.SetCapacity(4);
			transaction.SetType(Discord.LobbyType.Public);
			

			string lobbyCode = GetRoomCode();
			transaction.SetMetadata("RoomCode", lobbyCode);
			

			lobbyManager.CreateLobby(transaction, (Discord.Result result, ref Discord.Lobby lobby) => {
				if (result == Discord.Result.Ok) { 
					lobbyManager.ConnectNetwork(lobby.Id);
					lobbyID = lobby.Id;

					lobby.Secret = "123";

					lobbyManager.OpenNetworkChannel(lobbyID, 0, true);
					lobbyManager.OpenNetworkChannel(lobbyID, 1, false);

                    lobbyManager.OnNetworkMessage += ProcessMessage;
                    lobbyManager.OnMemberConnect += MemberConnected;

					isHost = true;

					GameLoop.UIManager.AddMsg(new ColoredString("Created a lobby with code " + lobbyCode, Color.Green, Color.Black));
				}
			});
        }

        private void MemberConnected(long lobbyId, long userId) {
			if (lobbyId == lobbyID) {
				foreach (KeyValuePair<long, Player> kv in GameLoop.World.otherPlayers) {
					string json = JsonConvert.SerializeObject(kv.Value, Formatting.Indented);
					string msg = "createPlayer;" + kv.Key + ";" + json;
					lobbyManager.SendNetworkMessage(lobbyId, userId, 0, Encoding.UTF8.GetBytes(msg));
				}
				 
				string ownjson = JsonConvert.SerializeObject(GameLoop.World.Player, Formatting.Indented);
				string ownmsg = "createPlayer;" + ownID + ";" + ownjson;
				lobbyManager.SendNetworkMessage(lobbyId, userId, 0, Encoding.UTF8.GetBytes(ownmsg));

				string hostFlags = "hostFlags;" + GameLoop.World.Player.OwnsFarm;
				lobbyManager.SendNetworkMessage(lobbyId, userId, 0, Encoding.UTF8.GetBytes(hostFlags));

				foreach (KeyValuePair<int, NPC> kv in GameLoop.World.npcLibrary) {
					string msg = "moveNPC;" + kv.Value.npcID + ";" + kv.Value.Position.X + ";" + kv.Value.Position.Y + ";" + kv.Value.MapPos.X + ";" + kv.Value.MapPos.Y + ";" + kv.Value.MapPos.Z;
					lobbyManager.SendNetworkMessage(lobbyId, userId, 0, Encoding.UTF8.GetBytes(msg));
				}
			} 
		}

        public void InitNetworking(Int64 lobbyId) { 
			lobbyManager = discord.GetLobbyManager();
			lobbyManager.ConnectNetwork(lobbyId);
			 
			lobbyManager.OpenNetworkChannel(lobbyId, 0, true);
			lobbyManager.OpenNetworkChannel(lobbyId, 1, false);
			lobbyID = lobbyId;
		}

		public void SearchLobbiesAndJoin(string code) {
			lobbyManager = discord.GetLobbyManager();

			var query = lobbyManager.GetSearchQuery();
			query.Filter("metadata.RoomCode", LobbySearchComparison.Equal, LobbySearchCast.String, code);

			lobbyManager.Search(query, (result) => {
				if (result == Discord.Result.Ok) {
					var count = lobbyManager.LobbyCount();

					GameLoop.UIManager.MainMenu.joinError = "No Lobby Found";
					
					if (count == 1) {


						long connectID = lobbyManager.GetLobbyId(0);
						Discord.Lobby lobby = lobbyManager.GetLobby(connectID);


						lobbyManager.ConnectLobby(connectID, lobby.Secret, (Discord.Result result, ref Discord.Lobby lobby) => {
							if (result == Result.Ok) {
								InitNetworking(lobby.Id);
								lobbyManager.OnNetworkMessage += ProcessMessage;

								lobbyID = lobby.Id;
								 
								string jsonString = JsonConvert.SerializeObject(GameLoop.World.Player, Formatting.Indented); 

								BroadcastMsg("registerPlayer;" + ";" + jsonString);
								GameLoop.UIManager.clientAndConnected = false;

								GameLoop.UIManager.MainMenu.MainMenuWindow.IsVisible = false;
								GameLoop.UIManager.Map.MapWindow.IsVisible = true;
								GameLoop.UIManager.Map.MessageLog.IsVisible = true;
								GameLoop.UIManager.Sidebar.SidebarWindow.IsVisible = true;
								GameLoop.UIManager.selectedMenu = "None";

								string initialRequest = "requestEntities;" + GameLoop.World.Player.MapPos.X + ";" + GameLoop.World.Player.MapPos.Y + ";" + GameLoop.World.Player.MapPos.Z;
								GameLoop.World.maps[GameLoop.World.Player.MapPos].Entities.Clear();
								lobbyManager.SendNetworkMessage(lobby.Id, lobby.OwnerId, 0, Encoding.UTF8.GetBytes(initialRequest));
							}
						}); 

						GameLoop.UIManager.MainMenu.joinError = "Connect failed";  
					}
				}
			});
        }




		public string GetRoomCode() {
			string[] letters = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z".Split(",");

			return letters[GameLoop.rand.Next(letters.Length)] + letters[GameLoop.rand.Next(letters.Length)] + letters[GameLoop.rand.Next(letters.Length)]
				+ letters[GameLoop.rand.Next(letters.Length)] + letters[GameLoop.rand.Next(letters.Length)] + letters[GameLoop.rand.Next(letters.Length)];
		}
	}
}
