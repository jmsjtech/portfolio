using LofiHollow.Entities;
using LofiHollow.Managers;
using Newtonsoft.Json;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LofiHollow.Minigames.Mining {
    [JsonObject(MemberSerialization.OptIn)]
    public class Mine {
        [JsonProperty]
        public string Location = "";
        [JsonProperty]
        public Dictionary<int, MineLevel> Levels = new();

        public bool SyncedFromHost = false;

        public Mine(string loc) {
            Location = loc;

            MineLevel zero = new(0, loc);
            Levels.Add(0, zero);
        }


        public void PickupItem(Player player) {
            Item item = Levels[player.MineDepth].GetEntityAt<Item>(player.Position / 12);
            if (item != null) { 
                for (int i = 0; i < player.Inventory.Length; i++) {
                    if (player.Inventory[i].ItemID == item.ItemID && player.Inventory[i].SubID == item.SubID && item.IsStackable) {
                        player.Inventory[i].ItemQuantity++;

                        DestroyItem(item, player.MineDepth);
                        SendPickup(item, player.MineDepth);

                        return;
                    }
                }

                for (int i = 0; i < player.Inventory.Length; i++) {
                    if (player.Inventory[i].ItemID == 0) {
                        player.Inventory[i] = item;
                        DestroyItem(item, player.MineDepth);
                        SendPickup(item, player.MineDepth);
                        break;
                    }
                } 
            }
        }

        public void AddItemToInv(Player player, Item item) {
            if (item != null) {
                for (int i = 0; i < player.Inventory.Length; i++) {
                    if (player.Inventory[i].ItemID == item.ItemID && player.Inventory[i].SubID == item.SubID && item.IsStackable) {
                        player.Inventory[i].ItemQuantity += item.ItemQuantity;
                        return;
                    }
                }

                for (int i = 0; i < player.Inventory.Length; i++) {
                    if (player.Inventory[i].ItemID == 0) {
                        player.Inventory[i] = item;
                        return;
                    }
                }
            }

            item.Position = player.Position / 12;
            SpawnItem(item, player.MineDepth);
        }

        public void SpawnItem(Item item, int Depth) {
            if (Levels.ContainsKey(Depth)) {
                Levels[Depth].Add(item);
            }
        }

        public void SendPickup(Item item, int Depth) {
            string json = JsonConvert.SerializeObject(item, Formatting.Indented);
            string msg = "mineDestroy;" + Depth + ";" + json;
            if (GameLoop.NetworkManager != null && GameLoop.NetworkManager.lobbyManager != null)
                GameLoop.NetworkManager.BroadcastMsg(msg);
        }

        public void DestroyItem(Item item, int Depth) {
            if (Levels.ContainsKey(Depth)) { 
                Item localCopy = Levels[Depth].GetEntityAt<Item>(item.Position, item.Name);
                if (localCopy != null) {
                    Levels[Depth].Remove(localCopy); 
                }
            } 
        }



        public bool MovePlayerTo(Player player, int depth, Point newPos, bool refreshMap) {
            if (!Levels.ContainsKey(depth)) {
                MineLevel newLevel = new(depth, Location);
                Levels.Add(depth, newLevel);
            }

            player.Position = newPos;
            player.MineDepth = depth;

            GameLoop.UIManager.Minigames.MinigameWindow.Title = Location + " Mine - Depth: " + (depth * -50);
            GameLoop.UIManager.Minigames.MinigameWindow.TitleAlignment = SadConsole.HorizontalAlignment.Center;


            if (refreshMap)
                if (player == GameLoop.World.Player)
                    GameLoop.UIManager.Minigames.MiningFOV = new GoRogue.FOV(Levels[player.MineDepth].MapFOV);


            return true;
        }

        public bool BreakTileAt(Player player, int depth, Point breakPos) { 
            if (breakPos.X < 0 || breakPos.X > 70)
                return false;
            if (breakPos.Y < 0 || breakPos.Y > 40)
                return false;
            int newDist = 10;
            if (Levels[depth].MapPath.ShortestPath(new GoRogue.Coord(player.Position.X / 12, player.Position.Y / 12), new GoRogue.Coord(breakPos.X, breakPos.Y)) != null)
                newDist = Levels[depth].MapPath.ShortestPath(new GoRogue.Coord(player.Position.X / 12, player.Position.Y / 12), new GoRogue.Coord(breakPos.X, breakPos.Y)).Length;

            if (newDist < 6) {
                if (Levels[depth].GetTile(breakPos).Name != "Air") {
                    int ToolTier = player.GetToolTier(2);
                    if (ToolTier >= Levels[player.MineDepth].GetTile(breakPos).RequiredTier) {
                        Levels[player.MineDepth].GetTile(breakPos).Damage(ToolTier);
                        if (Levels[player.MineDepth].GetTile(breakPos).TileHP <= 0) {
                            Item item = new(Levels[player.MineDepth].GetTile(breakPos).OutputID);
                            AddItemToInv(player, item);
                            player.Skills["Mining"].GrantExp(Levels[player.MineDepth].GetTile(breakPos).GrantedExp);
                            Levels[player.MineDepth].TileToAir(breakPos);
                        }

                        if (GameLoop.NetworkManager != null && GameLoop.NetworkManager.lobbyManager != null) {
                            string msg = "updateMine;" + Location + ";" + player.MineDepth + ";" +
                                breakPos.X + ";" + breakPos.Y + ";" +
                                JsonConvert.SerializeObject(Levels[player.MineDepth].GetTile(breakPos), Formatting.Indented);
                            GameLoop.NetworkManager.BroadcastMsg(msg);
                        }

                        return true;
                    } else {
                        return false;
                    }
                }
            }

            return false;
        }

        public bool MovePlayerBy(Player player, int depth, Point newPos) {
            Point newPosition = player.Position + newPos;

            if (newPosition.X / 12 < 0 || newPosition.X / 12 >= 70)
                return false;

            if (newPosition.Y < 0 && depth > 0) { 
               // int newDepth = depth - 1;
               // MovePlayerTo(player, newDepth, newPosition.WithY(39 * 12), true);
                // return true;
                return false;
            }

            if (newPosition.Y / 12 > 39) {
                //   int newDepth = depth + 1;
                //    MovePlayerTo(player, newDepth, newPosition.WithY(0), true);
                //   return true;
                return false;
            }

            

            if (Levels.ContainsKey(player.MineDepth)) {
                if (Levels[player.MineDepth].GetTile(new Point((newPosition.X + 6) / 12, (newPosition.Y + 12) / 12)).Name == "Air") {
                    player.Position = newPosition;
                    return true;
                } else {
                    return false;
                }
            }

            return false;
        }
    }
}
