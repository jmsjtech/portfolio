using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using RogueSharp.DiceNotation;
using TearsInRain.Serializers;
using TearsInRain.src;
using TearsInRain.Tiles;

namespace TearsInRain.Entities {
    
    [JsonConverter(typeof(ActorJsonConverter))]
    public class Actor : Entity { 
        public UInt64 TimeLastActed { get; set; }
        public Dictionary<string, SadConsole.CellDecorator> decorators = new Dictionary<string, SadConsole.CellDecorator>();
        public string HealthState = "healthy";
        public bool IsCrouched = false;
        public int Speed = 10;
   

        public Actor(Color foreground, Color background, int glyph, int width = 1, int height = 1) : base(foreground, background, width, height, glyph) {
            Animation.CurrentFrame[0].Foreground = foreground;
            Animation.CurrentFrame[0].Background = background;
            Animation.CurrentFrame[0].Glyph = glyph;

        }

        public bool MoveBy(Point positionChange) {
            if (Position.X + positionChange.X < 0 || Position.X + positionChange.X > 100 || Position.Y + positionChange.Y < 0 || Position.Y + positionChange.Y > 100) {
                return false;
            }


            TileBase tile = GameLoop.World.CurrentMap.GetTileAt<TileBase>(Position.X + positionChange.X, Position.Y + positionChange.Y);


            if (tile != null) {
                Point justVert = new Point(0, positionChange.Y);
                Point justHori = new Point(positionChange.X, 0);

                if (GameLoop.World.CurrentMap.IsTileWalkable(Position + positionChange) || tile is TileDoor) {
                    Actor monster = GameLoop.World.CurrentMap.GetEntityAt<Actor>(Position + positionChange);

                    foreach (KeyValuePair<long, Player> player in GameLoop.World.players) {
                        if (player.Value.Position == Position + positionChange) {
                            monster = player.Value;
                        }
                    }

                    if (monster != null) {
                        //GameLoop.CommandManager.Attack(this, monster);
                        return false;
                    } 
                    
                    //else if (tile.Name.ToLower().Contains("door") && !tile.IsOpen) {
                    //    GameLoop.CommandManager.OpenDoor(this, door, Position + positionChange);
                    //    return true;
                    //}


                    Position += positionChange;
                    return true;
                } else if (GameLoop.World.CurrentMap.IsTileWalkable(Position + justVert) || tile is TileDoor) {
                    Actor monster = GameLoop.World.CurrentMap.GetEntityAt<Actor>(Position + justVert);

                    foreach (KeyValuePair<long, Player> player in GameLoop.World.players) {
                        if (player.Value.Position == Position + justVert) {
                            monster = player.Value;
                        }
                    }

                    if (monster != null) {
                       // GameLoop.CommandManager.Attack(this, monster);
                        return true;
                    } else if (tile is TileDoor door && !door.IsOpen) {
                        GameLoop.CommandManager.OpenDoor(this, door, Position + justVert);
                        return true;
                    }


                    Position += justVert;
                    return true;
                } else if (GameLoop.World.CurrentMap.IsTileWalkable(Position + justHori) || tile is TileDoor) {
                    Actor monster = GameLoop.World.CurrentMap.GetEntityAt<Actor>(Position + justHori);

                    foreach (KeyValuePair<long, Player> player in GameLoop.World.players) {
                        if (player.Value.Position == Position + justHori) {
                            monster = player.Value;
                        }
                    }

                    if (monster != null) {
                       // GameLoop.CommandManager.Attack(this, monster);
                        return true;
                    } else if (tile is TileDoor door && !door.IsOpen) {
                        GameLoop.CommandManager.OpenDoor(this, door, Position + justHori);
                        return true;
                    }


                    Position += justHori;
                    return true;
                } else {
                    return false;
                }
            } else {
                return false;
            }
        }

        public bool MoveTo(Point newPosition) {
            Position = newPosition;
            return true;
        }

    }
}