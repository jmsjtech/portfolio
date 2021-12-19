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
          
    }
}
