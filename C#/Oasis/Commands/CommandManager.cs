using System;
using DefaultEcs;
using Microsoft.Xna.Framework;
using System.Text;
using GoRogue.DiceNotation;
using SadConsole.Components;

namespace Oasis.Commands {
    public class CommandManager {
        private Point _lastMoveEntityPoint;
        private Entity _lastMoveEntity;

        public CommandManager() {

        }

        public bool MoveEntityBy(Entity entity, Point position) {
            _lastMoveEntity = entity;
            _lastMoveEntityPoint = position;

            if (GameLoop.World.CurrentMap.GetEntityAt<Monster>(entity.Get<Render>().GetPosition() + position) != null) {
                Entity monster = GameLoop.World.CurrentMap.GetEntityAt<Monster>(entity.Get<Render>().GetPosition() + position).Value;
                GameLoop.CommandManager.Attack(entity, monster);
                return true;
            }

            return entity.Get<Render>().Move(position.X, position.Y);
        }

        public bool RedoMoveEntityBy() {
            if (_lastMoveEntity != null) {
                return MoveEntityBy(_lastMoveEntity, _lastMoveEntityPoint);
            } else return false;
        }

        public bool UndoMoveEntityBy() {
            if (_lastMoveEntity != null) {
                _lastMoveEntityPoint = new Point(-_lastMoveEntityPoint.X, -_lastMoveEntityPoint.Y);

                if (MoveEntityBy(_lastMoveEntity, _lastMoveEntityPoint)) {
                    _lastMoveEntityPoint = new Point(0, 0);
                    return true;
                } else return false;
            } else return false;
        }

        public void Attack(Entity attacker, Entity defender) { 
            StringBuilder attackMessage = new StringBuilder();
            StringBuilder defenseMessage = new StringBuilder();
             
            ResolveAttack(attacker, defender, attackMessage);
             
            GameLoop.UIManager.MessageLog.Add(attackMessage.ToString());

            
        }

        private static void ResolveAttack(Entity attacker, Entity defender, StringBuilder attackMessage) { 
            attackMessage.AppendFormat("{0} attacks {1},", attacker.Get<Name>().name, defender.Get<Name>().name);
             
            int diceOutcome = Dice.Roll("3d6"); 
            if (diceOutcome <= attacker.Get<Stats>().Attack) {
                ResolveDefense(defender, Dice.Roll(attacker.Get<Stats>().Damage), attackMessage);
            } else {
                attackMessage.AppendFormat(" but misses.");
            }
        }

        private static void ResolveDefense(Entity defender, int damage, StringBuilder attackMessage) {
            int diceOutcome = Dice.Roll("3d6");

            if (diceOutcome <= defender.Get<Stats>().Defense) {
                attackMessage.AppendFormat(" but {0} dodges!", defender.Get<Name>().name);
            } else {
                attackMessage.AppendFormat(" dealing {0} damage.", damage);
                ResolveDamage(defender, damage);
            }
        }


        private static void ResolveDamage(Entity defender, int damage) {
            if (damage > 0) {
                defender.Get<Stats>().Health = defender.Get<Stats>().Health - damage;
                if (defender.Get<Stats>().Health <= 0) {
                    ResolveDeath(defender);
                }
            } else {
                GameLoop.UIManager.MessageLog.Add($"{ defender.Get<Name>().name} blocked all damage!");
            }
        }

        private static void ResolveDeath(Entity defender) {
            if (defender.Has<Player>()) {
                GameLoop.UIManager.MessageLog.Add($" {defender.Get<Name>().name} was killed.");
            } else if (defender.Has<Monster>()) {
                GameLoop.UIManager.MessageLog.Add($"{defender.Get<Name>().name} died and dropped {defender.Get<Stats>().Gold} gold coins.");
            }

            foreach (Entity item in GameLoop.gs.ecs.GetEntities().With<Item>().With<InBackpack>().AsEnumerable()) {
                if (item.Get<InBackpack>().owner == defender) {
                    item.Set(new Render { sce = new SadConsole.Entities.Entity(1, 1) });
                    item.Get<Render>().SimpleSet(item.Get<Item>().glyph, item.Get<Item>().fg, item.Get<Item>().bg);
                    item.Get<Render>().SetPosition(defender.Get<Render>().GetPosition());
                    item.Get<Render>().sce.Components.Add(new EntityViewSyncComponent());
                    item.Remove<InBackpack>();
                }
            }
            
            defender.Dispose();


            GameLoop.UIManager.SyncMapEntities(GameLoop.World.CurrentMap);
        }

        public void Pickup(Entity actor, Entity item) {
            GameLoop.UIManager.MessageLog.Add($"{actor.Get<Name>().name} picked up {item.Get<Name>().name}");
            item.Remove<Render>();
            item.Set(new InBackpack { owner = actor });
        }
    }
}