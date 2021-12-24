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
                    if (actor.Inventory[i].ItemID == item.ItemID && item.IsStackable) {
                        actor.Inventory[i].ItemQuantity++;

                        // Remove item from drop table

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
            if (item != null) {
                for (int i = 0; i < actor.Inventory.Length; i++) {
                    if (actor.Inventory[i].ItemID == item.ItemID && item.IsStackable) {
                        actor.Inventory[i].ItemQuantity++;

                        GameLoop.UIManager.dropTable.Remove(item);
                        return;
                    }
                }

                for (int i = 0; i < actor.Inventory.Length; i++) {
                    if (actor.Inventory[i].ItemID == 0) {
                        actor.Inventory[i] = item;
                        GameLoop.UIManager.dropTable.Remove(item);
                        break;
                    }
                }
            }
        }

        public void EquipItem(Actor actor, int slot, int id) {
            if (actor.Inventory.Length > slot && slot >= 0) { 
                Item item = new Item(id);

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
