﻿using System;
using SadRogue.Primitives;
using System.Text;
using LofiHollow.Entities;
using GoRogue.DiceNotation;
using SadConsole;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace LofiHollow.Managers {
    public class CommandManager {
        public CommandManager() { } 

        public static bool MoveActorBy(Actor actor, Point position) {
            bool moved = actor.MoveBy(position);
            if (moved) {
                if (GameLoop.NetworkManager != null && GameLoop.NetworkManager.lobbyManager != null) {
                    GameLoop.NetworkManager.BroadcastMsg("movePlayer;" + GameLoop.NetworkManager.ownID + ";" + actor.Position.X + ";" + actor.Position.Y + ";" 
                        + actor.MapPos.X + ";" + actor.MapPos.Y + ";" + actor.MapPos.Z);
                }

                if (actor.ScreenAppearance == null) {
                    actor.UpdateAppearance(); 
                }
                actor.UpdatePosition();
            }

            


            return moved;
        }

        public static bool MoveActorTo(Actor actor, Point position, Point3D mapPos) {
            bool moved = actor.MoveTo(position, mapPos);

            if (moved) {
                if (actor.ScreenAppearance == null) {
                    actor.UpdateAppearance();
                }
                actor.UpdatePosition();
            }

            return moved;
        } 

        public static void DropItem(Actor actor, int slot) {
            if (actor.Inventory.Length > slot && actor.Inventory[slot].ItemID != 0) {
                if (GameLoop.World.itemLibrary.ContainsKey(actor.Inventory[slot].ItemID)) {
                    Item item = actor.Inventory[slot]; 
                    item.Position = actor.Position;
                    item.MapPos = actor.MapPos;

                    
                    actor.Inventory[slot] = new Item(0);

                    SendItem(item);
                    SpawnItem(item);
                }
            }
        }

        public static void SendItem(Item item) {
            string json = JsonConvert.SerializeObject(item, Formatting.Indented);
            string msg = "spawnItem;" + json;
            if (GameLoop.NetworkManager != null && GameLoop.NetworkManager.lobbyManager != null)
                GameLoop.NetworkManager.BroadcastMsg(msg);
        }

        public static void SpawnItem(Item item) {
            if (!GameLoop.World.maps.ContainsKey(item.MapPos))
                GameLoop.World.LoadMapAt(item.MapPos);

            GameLoop.World.maps[item.MapPos].Add(item);

            GameLoop.UIManager.Map.SyncMapEntities(GameLoop.World.maps[GameLoop.World.Player.MapPos]);
        }

        public static void SendPickup(Item item) {
            string json = JsonConvert.SerializeObject(item, Formatting.Indented);
            string msg = "destroyItem;" + json;
            if (GameLoop.NetworkManager != null && GameLoop.NetworkManager.lobbyManager != null)
                GameLoop.NetworkManager.BroadcastMsg(msg);
        }

        public static void DestroyItem(Item item) {
            if (!GameLoop.World.maps.ContainsKey(item.MapPos))
                GameLoop.World.LoadMapAt(item.MapPos);

            Item localCopy = GameLoop.World.maps[item.MapPos].GetEntityAt<Item>(item.Position, item.Name);
            if (localCopy != null) {
                GameLoop.UIManager.Map.EntityRenderer.Remove(localCopy);
                GameLoop.World.maps[item.MapPos].Entities.Remove(localCopy); 
            }

            GameLoop.UIManager.Map.SyncMapEntities(GameLoop.World.maps[GameLoop.World.Player.MapPos]);
        }

        public static void PickupItem(Actor actor) {
            Item item = GameLoop.World.maps[actor.MapPos].GetEntityAt<Item>(actor.Position);
            if (item != null) {
                for (int i = 0; i < actor.Inventory.Length; i++) {
                    if (actor.Inventory[i].ItemID == item.ItemID && actor.Inventory[i].SubID == item.SubID && item.IsStackable) {
                        actor.Inventory[i].ItemQuantity++;

                        DestroyItem(item);
                        SendPickup(item);

                        return;
                    }
                }

                for (int i = 0; i < actor.Inventory.Length; i++) {
                    if (actor.Inventory[i].ItemID == 0) {
                        actor.Inventory[i] = item;
                        DestroyItem(item);
                        SendPickup(item);
                        break;
                    }
                }
            }
        }

        public static void AddItemToInv(Actor actor, Item item) { 
            if (item != null) {
                for (int i = 0; i < actor.Inventory.Length; i++) {
                    if (actor.Inventory[i].ItemID == item.ItemID && actor.Inventory[i].SubID == item.SubID && item.IsStackable) {
                        actor.Inventory[i].ItemQuantity += item.ItemQuantity; 
                        return;
                    }
                }

                for (int i = 0; i < actor.Inventory.Length; i++) {
                    if (actor.Inventory[i].ItemID == 0) {
                        actor.Inventory[i] = item; 
                        return;
                    }
                }
            } 

            item.Position = actor.Position;
            item.MapPos = actor.MapPos;
            SpawnItem(item);
        }

        public static string UseItem(Actor actor, Item item) {
            if (item.Heal != null) {
                if (actor.CurrentHP != actor.MaxHP) {
                    int healAmount = GoRogue.DiceNotation.Dice.Roll(item.Heal.HealAmount);

                    if (actor.CurrentHP + healAmount > actor.MaxHP) {
                        healAmount = actor.MaxHP - actor.CurrentHP;
                    }

                    actor.CurrentHP += healAmount;
                    return "t|Healed " + healAmount + " hit points!";

                } else {
                    return "f|You're already at max HP!";
                }
            }

            return "f|Item usage not implemented.";
        }

        public static void EquipItem(Actor actor, int slot, Item item) {
            if (actor.Inventory.Length > slot && slot >= 0) {

                if (item.EquipSlot >= 0 && item.EquipSlot <= 6) {
                    Item temp = actor.Equipment[item.EquipSlot];
                    actor.Equipment[item.EquipSlot] = item;
                    actor.Inventory[slot] = temp;
                } 
            }
        }

        public static void UnequipItem(Actor actor, int slot) {
            if (slot >= 0 && slot <= 15) {
                if (actor.Equipment[slot].ItemID != 0) {
                    for (int i = 0; i < actor.Inventory.Length; i++) {
                        if (actor.Inventory[i].ItemID == 0) {
                            actor.Inventory[i] = actor.Equipment[slot];
                            actor.Equipment[slot] = new Item(0);
                        }
                    }
                }
            }
        }

        public static int RemoveOneItem(Actor actor, int slot) {
            int returnID = -1;
            if (slot >= 0 && slot <= actor.Inventory.Length) {
                if (actor.Inventory[slot].ItemID != 0) {
                    if (!actor.Inventory[slot].IsStackable || (actor.Inventory[slot].IsStackable && actor.Inventory[slot].ItemQuantity == 1)) {
                        returnID = actor.Inventory[slot].ItemID;
                        actor.Inventory[slot] = new Item(0);
                    } else if (actor.Inventory[slot].IsStackable && actor.Inventory[slot].ItemQuantity > 1) {
                        actor.Inventory[slot].ItemQuantity--;
                        returnID = actor.Inventory[slot].ItemID;
                    }
                }
            }

            return returnID;
        }

        public static void SendMonster(Monster monster) {
            string json = JsonConvert.SerializeObject(monster, Formatting.Indented);
            string msg = "spawnMonster;" + json;
            if (GameLoop.NetworkManager != null && GameLoop.NetworkManager.lobbyManager != null)
                GameLoop.NetworkManager.BroadcastMsg(msg);
        }


        public static void SpawnMonster(Monster monster) {
            GameLoop.World.maps[monster.MapPos].Add(monster);

            monster.UpdateAppearance();
            if (monster.MapPos == GameLoop.World.Player.MapPos) {
                //  GameLoop.UIManager.Map.EntityRenderer.Add(monster);
                if (monster.ScreenAppearance == null)
                    monster.UpdateAppearance();
                GameLoop.UIManager.Map.MapConsole.Children.Add(monster.ScreenAppearance);
                GameLoop.UIManager.Map.SyncMapEntities(GameLoop.World.maps[GameLoop.World.Player.MapPos]);
            }
        }

        public static void MoveMonster(string id, Point3D MapPos, Point newPos) {
            foreach (Entity ent in GameLoop.World.maps[MapPos].Entities.Items) {
                if (ent is Monster mon) { 
                    if (mon.UniqueID == id) {
                        mon.MoveTo(newPos, MapPos);
                    }
                }
            }
        }

        public static void DamageMonster(string id, Point3D MapPos, int damage, string battleString, string color) {
            foreach (Entity ent in GameLoop.World.maps[MapPos].Entities.Items) {
                if (ent is Monster mon) {
                    if (mon.UniqueID == id) {
                        Color stringColor = color == "Green" ? Color.Green : color == "Red" ? Color.Red : Color.White;

                        mon.CurrentHP -= damage;
                        if (MapPos == GameLoop.World.Player.MapPos) {
                            GameLoop.UIManager.AddMsg(new ColoredString(battleString, stringColor, Color.Black));
                        }


                        if (mon.CurrentHP <= 0) {
                            mon.Death(false);
                        }
                    }
                }
            }
        }

        public static void DamagePlayer(long id, int damage, string battleString, string color) {
            Color hitColor = color == "Green" ? Color.Green : color == "Red" ? Color.Red : Color.White;
            if (!GameLoop.World.otherPlayers.ContainsKey(id)) {
                if (GameLoop.NetworkManager.ownID == id) {
                    GameLoop.World.Player.CurrentHP -= damage;
                   
                    
                    GameLoop.UIManager.AddMsg(new ColoredString(battleString, hitColor, Color.Black));
                } else {
                    return;
                }
            } else { 
                GameLoop.World.otherPlayers[id].CurrentHP -= damage;

                if (GameLoop.World.otherPlayers[id].CurrentHP <= 0) {
                    GameLoop.World.otherPlayers[id].PlayerDied(); 
                }

                if (GameLoop.World.otherPlayers[id].MapPos == GameLoop.World.Player.MapPos)
                    GameLoop.UIManager.AddMsg(new ColoredString(battleString, hitColor, Color.Black));
            }
        }



        public static void Attack(Actor attacker, Actor defender, bool melee) {
            if (attacker == GameLoop.World.Player || defender == GameLoop.World.Player) {
                string damageType = melee ? attacker.GetDamageType() : "Range";
                int attackRoll = attacker.AttackRoll(damageType);
                int defRoll = defender.DefenceRoll(damageType);



                int newDamage = 0;

                float hitChance;

                if (attackRoll > defRoll) {
                    hitChance = 1 - ((defRoll + 2) / 2 * (attackRoll + 1));
                } else {
                    hitChance = attackRoll / 2 * (defRoll + 1);
                }

                hitChance *= 100;

                int roll = GameLoop.rand.Next(100) + 1;

                ColoredString battleString;
                string battleColor = "White";

                if (roll < hitChance) {
                    newDamage = attacker.DamageRoll(damageType); 
                     
                    if (newDamage < 0)
                        newDamage = 0; 

                    if (newDamage > 0) {
                        battleString = attacker.GetAppearance() + new ColoredString(" " + newDamage + " " + ((char) 20) + " ", Color.Red, Color.Black) + defender.GetAppearance();
                    } else {
                        battleString = attacker.GetAppearance() + new ColoredString(" 0 " + ((char) 20) + " ", Color.White, Color.Black) + defender.GetAppearance();
                    }

                    defender.CurrentHP -= newDamage;

                    if (attacker is Player && newDamage > 0) {
                        attacker.CombatExp(newDamage);
                        if (!((Monster) defender).AlwaysAggro) {
                            ((Monster)defender).AlwaysAggro = true;
                        }
                    }
                } else {
                    battleString = attacker.GetAppearance() + new ColoredString(" 0 " + ((char)20) + " ", Color.White, Color.Black) + defender.GetAppearance();
                }

                if (GameLoop.NetworkManager != null && GameLoop.NetworkManager.lobbyManager != null) {
                    if (defender is Monster mon) {
                        string netmsg = "damageMonster;" + mon.UniqueID + ";" + mon.MapPos.X + ";" + mon.MapPos.Y + ";" + mon.MapPos.Z + ";" + newDamage + ";" + battleString.String + ";" + battleColor;
                        GameLoop.NetworkManager.BroadcastMsg(netmsg);
                    } else if (defender is Player player) {
                        if (player == GameLoop.World.Player) {
                            string netmsg = "damagePlayer;" + GameLoop.NetworkManager.ownID + ";" + newDamage + ";" + battleString.String + ";" + battleColor;
                            GameLoop.NetworkManager.BroadcastMsg(netmsg);
                        } else {
                            foreach (KeyValuePair<long, Player> kv in GameLoop.World.otherPlayers) {
                                if (player == kv.Value) {
                                    string netmsg = "damagePlayer;" + kv.Key + ";" + newDamage + ";" + battleString.String + ";" + battleColor;
                                    GameLoop.NetworkManager.BroadcastMsg(netmsg);
                                    break;
                                }
                            }
                        }
                    }
                }

                if (attacker.MapPos == GameLoop.World.Player.MapPos)
                    GameLoop.UIManager.BattleMsg(battleString);

                if (defender.CurrentHP <= 0) {
                    if (defender is Monster) {
                        defender.Death();
                        ((Player)attacker).killList.Push(defender.GetAppearance());
                    } else if (defender is Player player) {
                        player.PlayerDied();
                    }
                }
            }
        }
    }
}