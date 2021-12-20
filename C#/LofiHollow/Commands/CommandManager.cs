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

        public void Attack(Actor attacker, Actor defender) { 
            StringBuilder attackMessage = new StringBuilder();
            StringBuilder defenseMessage = new StringBuilder();

            int atkDiceOutcome = Dice.Roll("3d6");
        }

        public void DropItem(Actor actor, int slot) {
            if (actor.Inventory.Length > slot && actor.Inventory[slot] != 0) {
                if (GameLoop.World.itemLibrary.ContainsKey(actor.Inventory[slot])) {
                    Item temp = GameLoop.World.itemLibrary[actor.Inventory[slot]];
                    Item item = new Item(temp.Foreground, Color.Black, temp.Glyph, temp.Name, temp.ItemID, temp.ItemCategory, temp.EquipSlot);
                    item.Position = actor.Position;

                    GameLoop.World.maps[actor.MapPos].Entities.Add(item, new GoRogue.Coord(actor.Position.X, actor.Position.Y));
                    GameLoop.UIManager.EntityRenderer.Add(item);
                    actor.Inventory[slot] = 0;
                }
            }
        }

        public void PickupItem(Actor actor) {
            Item item = GameLoop.World.maps[actor.MapPos].GetEntityAt<Item>(actor.Position);
            if (item != null) {
                for (int i = 0; i < actor.Inventory.Length; i++) {
                    if (actor.Inventory[i] == 0) {
                        actor.Inventory[i] = item.ItemID;
                        GameLoop.World.maps[actor.MapPos].Entities.Remove(item);
                        GameLoop.UIManager.EntityRenderer.Remove(item);
                        break;
                    }
                }
            }
        }

        public void EquipItem(Actor actor, int slot, int id) {
            if (actor.Inventory.Length > slot && slot >= 0) {
                if (GameLoop.World.itemLibrary.ContainsKey(id)) {
                    Item item = GameLoop.World.itemLibrary[id];

                    if (item.EquipSlot >= 0 && item.EquipSlot <= 6) {
                        int temp = actor.Equipment[item.EquipSlot];
                        actor.Equipment[item.EquipSlot] = item.ItemID;
                        actor.Inventory[slot] = temp;
                    }
                }
            }
        }

        public void UnequipItem(Actor actor, int slot) {
            if (slot >= 0 && slot <= 6) {
                if (actor.Equipment[slot] != 0) {
                    for (int i = 0; i < actor.Inventory.Length; i++) {
                        if (actor.Inventory[i] == 0) {
                            actor.Inventory[i] = actor.Equipment[slot];
                            actor.Equipment[slot] = 0;
                        }
                    }
                }
            }
        }
    }
}
