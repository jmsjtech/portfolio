using System;
using Microsoft.Xna.Framework;
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

        public void Attack(Actor attacker, Actor defender) { 
            StringBuilder attackMessage = new StringBuilder();
            StringBuilder defenseMessage = new StringBuilder();

            int atkDiceOutcome = Dice.Roll("3d6");

            if (atkDiceOutcome > attacker.Dexterity) {
                GameLoop.UIManager.MessageLog.Add(attacker.Name + " attacked " + defender.Name + " but missed!");
            } else {
                int defDiceOutcome = Dice.Roll("3d6");
                if (defDiceOutcome > defender.Dexterity) {
                    int damageDice = Dice.Roll("1d6");
                    GameLoop.UIManager.MessageLog.Add(attacker.Name + " hit " + defender.Name + " for " + ResolveDamage(defender, damageDice) + " damage!");
                } else {
                    GameLoop.UIManager.MessageLog.Add(attacker.Name + " attacked " + defender.Name + ", but " + defender.Name + " dodged!");
                }
            }
        }

        private static int ResolveDamage(Actor defender, int damage) {
            if (damage > 0) {
                defender.Health = defender.Health - damage;
                if (defender.Health <= 0) {
                    ResolveDeath(defender);
                }
                return damage;
            } else {
                return 0;
            }
        }

        private static void ResolveDeath(Actor defender) {
            GameLoop.World.maps[GameLoop.World.Player.MapPos].Remove(defender);

            if (defender is Player) {
                GameLoop.UIManager.MessageLog.Add($"{defender.Name} was killed.");
            } else if (defender is Monster) {
                GameLoop.UIManager.MessageLog.Add($"{defender.Name} died and dropped {defender.Gold} gold coins.");
            }
        }
    }
}
