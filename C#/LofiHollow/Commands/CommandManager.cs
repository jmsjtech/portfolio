using System;
using SadRogue.Primitives;
using System.Text;
using LofiHollow.Entities;
using GoRogue.DiceNotation;

namespace LofiHollow.Commands {
    public class CommandManager {
        public CommandManager() {

        } 

        public bool MoveActorBy(Actor actor, Point position) {
            return actor.MoveBy(position);
        }

        public bool MoveActorTo(Actor actor, Point position, Point3D mapPos) {
            return actor.MoveTo(position, mapPos);
        } 

        public void DropItem(Actor actor, int slot) {
            if (actor.Inventory.Length > slot && actor.Inventory[slot].ItemID != 0) {
                if (GameLoop.World.itemLibrary.ContainsKey(actor.Inventory[slot].ItemID)) {
                    Item item = actor.Inventory[slot]; 
                    item.Position = actor.Position;

                    GameLoop.World.maps[actor.MapPos].Entities.Add(item, new GoRogue.Coord(actor.Position.X, actor.Position.Y));
                    GameLoop.UIManager.EntityRenderer.Add(item);
                    actor.Inventory[slot] = new Item(0);
                }
            }
        }

        public void PickupItem(Actor actor) {
            Item item = GameLoop.World.maps[actor.MapPos].GetEntityAt<Item>(actor.Position);
            if (item != null) {
                for (int i = 0; i < actor.Inventory.Length; i++) {
                    if (actor.Inventory[i].ItemID == item.ItemID && actor.Inventory[i].SubID == item.SubID && item.IsStackable) {
                        actor.Inventory[i].ItemQuantity++;

                        GameLoop.World.maps[actor.MapPos].Entities.Remove(item);
                        GameLoop.UIManager.EntityRenderer.Remove(item);

                        return;
                    }
                }

                for (int i = 0; i < actor.Inventory.Length; i++) {
                    if (actor.Inventory[i].ItemID == 0) {
                        actor.Inventory[i] = item;
                        GameLoop.World.maps[actor.MapPos].Entities.Remove(item);
                        GameLoop.UIManager.EntityRenderer.Remove(item);
                        break;
                    }
                }
            }
        }

        public void AddItemToInv(Actor actor, Item item) {
            bool putInInv = false;
            if (item != null) {
                for (int i = 0; i < actor.Inventory.Length; i++) {
                    if (actor.Inventory[i].ItemID == item.ItemID && actor.Inventory[i].SubID == item.SubID && item.IsStackable) {
                        actor.Inventory[i].ItemQuantity++;

                        GameLoop.UIManager.dropTable.Remove(item);
                        putInInv = true;
                        return;
                    }
                }

                for (int i = 0; i < actor.Inventory.Length; i++) {
                    if (actor.Inventory[i].ItemID == 0) {
                        actor.Inventory[i] = item;
                        GameLoop.UIManager.dropTable.Remove(item);
                        putInInv = true;
                        return;
                    }
                }
            }

            if (!putInInv) {
                item.Position = actor.Position;
                GameLoop.World.maps[actor.MapPos].Entities.Add(item, new GoRogue.Coord(actor.Position.X, actor.Position.Y));
                GameLoop.UIManager.EntityRenderer.Add(item);
            }
        }

        public string UseItem(Actor actor, Item item) {
            if (item.ItemID == 7) {
                if (actor.CurrentHP != actor.MaxHP) {
                    int healAmount = GoRogue.DiceNotation.Dice.Roll("1d8");

                    if (actor.CurrentHP + healAmount > actor.MaxHP) {
                        healAmount = actor.MaxHP - actor.CurrentHP;
                    }

                    actor.CurrentHP += healAmount;
                    return "t|Healed " + healAmount + " hit points!";

                } else {
                    return "f|You're already at max HP!";
                }
            } else if (item.ItemID == 8) {
                if (actor.CurrentHP != actor.MaxHP) {
                    int healAmount = GoRogue.DiceNotation.Dice.Roll("2d8");

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

        public void EquipItem(Actor actor, int slot, Item item) {
            if (actor.Inventory.Length > slot && slot >= 0) {

                if (item.EquipSlot >= 0 && item.EquipSlot <= 6) {
                    Item temp = actor.Equipment[item.EquipSlot];
                    actor.Equipment[item.EquipSlot] = item;
                    actor.Inventory[slot] = temp;
                } 
            }
        }

        public void UnequipItem(Actor actor, int slot) {
            if (slot >= 0 && slot <= 6) {
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
    }
}
