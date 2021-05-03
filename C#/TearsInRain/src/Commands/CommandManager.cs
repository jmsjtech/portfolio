using System;
using Microsoft.Xna.Framework;
using TearsInRain.Entities;
using System.Text;
using GoRogue.DiceNotation;
using TearsInRain.Tiles; 

namespace TearsInRain.Commands {
    public class CommandManager {
        public Point lastPeek = new Point(0, 0);


        public CommandManager() { }


        public bool MoveActorBy(Actor actor, Point position) {
            if (actor.MoveBy(position)) {
                actor.TimeLastActed = GameLoop.GameTime;

                string msg = "move_p" + "|" + GameLoop.NetworkingManager.myUID + "|" + actor.Position.X + "|" + actor.Position.Y;
                GameLoop.NetworkingManager.SendNetMessage(0, System.Text.Encoding.UTF8.GetBytes(msg));
                return true;
            }

            return false;
        }


        public void Peek(Actor actor, Point dir) {
            Point newPoint = actor.Position + dir;

            if (!GameLoop.World.CurrentMap.GetTileAt<TileBase>(newPoint.X, newPoint.Y).IsBlockingLOS) {
                lastPeek = dir;
                actor.PositionOffset += dir;
            }
        }

        public void ResetPeek(Actor actor) {
            actor.PositionOffset -= lastPeek;
            lastPeek = new Point(0, 0);
        }


        public void OpenDoor(Actor actor, TileDoor door, Point pos) {
            if (!door.IsLocked) {
                door.Open();

                GameLoop.UIManager.MapConsole.IsDirty = true;
                GameLoop.NetworkingManager.SendNetMessage(0, System.Text.Encoding.UTF8.GetBytes("t_data|door|" + pos.X + "|" + pos.Y + "|open|unlock"));
            } else {
                GameLoop.UIManager.MessageLog.Add("The door is locked.");
            }
        }

        public void CloseDoor(Actor actor, Point pos, bool literalPos = false) {
            Point newPoint;
            if (!literalPos)
                newPoint = actor.Position + pos;
            else
                newPoint = pos;

            TileBase tile = GameLoop.World.CurrentMap.GetTileAt<TileBase>(newPoint.X, newPoint.Y);
            Entity entity = GameLoop.World.CurrentMap.GetEntityAt<Entity>(newPoint);

            if (entity == null) {
                if (tile is TileDoor door) {
                    if (door.IsOpen) {
                        door.Close();

                        GameLoop.UIManager.MapConsole.IsDirty = true;

                        var data = "t_data|door|" + newPoint.X + "|" + newPoint.Y + "|close|";

                        if (door.IsLocked) { data += "lock"; } else if (!door.IsLocked) { data += "unlock"; }

                        GameLoop.NetworkingManager.SendNetMessage(0, System.Text.Encoding.UTF8.GetBytes(data));
                    } else {
                        GameLoop.UIManager.MessageLog.Add("The door is already closed.");
                    }
                } else {
                    GameLoop.UIManager.MessageLog.Add("There's nothing to close there!");
                }
            } else if (entity is Player) {
                GameLoop.UIManager.MessageLog.Add("You try to close the door, but some idiot is standing in the way!");
            } else if (entity is Monster) {
                GameLoop.UIManager.MessageLog.Add("Should have shut it before the monster walked through!");
            }
        }
    }
}