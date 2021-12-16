using System;
using Microsoft.Xna.Framework; 

namespace LofiHollow.Entities {
    public abstract class Actor : Entity {
        public int Health = 10;  
        public int MaxHealth = 10;

        public int Energy = 10;
        public int MaxEnergy = 10;

        public int Mana = 10;
        public int MaxMana = 10;

        public int Strength = 10;
        public int Vitality = 10;
        public int Intelligence = 10;
        public int Dexterity = 10;

        public int Gold = 0;


        public Point MapPos = new Point(0, 0);

        protected Actor(Color foreground, Color background, int glyph, int width = 1, int height = 1) : base(foreground, background, glyph) {
            Animation.CurrentFrame[0].Foreground = foreground;
            Animation.CurrentFrame[0].Background = background;
            Animation.CurrentFrame[0].Glyph = glyph;
        }

        public bool MoveBy(Point positionChange) {
            if (GameLoop.World.maps.TryGetValue(MapPos, out Map map)) {
                if (GameLoop.World.maps[MapPos].IsTileWalkable(Position + positionChange)) {
                    // if there's a monster here,
                    // do a bump attack
                    Monster monster = GameLoop.World.maps[MapPos].GetEntityAt<Monster>(Position + positionChange);
                    if (monster != null) {
                        GameLoop.CommandManager.Attack(this, monster);
                        return true;
                    }

                    Position += positionChange;
                    return true;
                }

                Point newPos = Position + positionChange;
                bool movedMaps = false;
              
                if (newPos.X < 0) {
                    if (!GameLoop.World.maps.ContainsKey(MapPos + new Point(-1, 0))) { GameLoop.World.CreateMap(MapPos + new Point(-1, 0)); }

                    MapPos += new Point(-1, 0);
                    Position = new Point(GameLoop.World._mapWidth, newPos.Y);
                    movedMaps = true;
                }

                if (newPos.X >= GameLoop.World._mapWidth) {
                    if (!GameLoop.World.maps.ContainsKey(MapPos + new Point(1, 0))) { GameLoop.World.CreateMap(MapPos + new Point(1, 0)); }

                    MapPos += new Point(1, 0);
                    Position = new Point(0, newPos.Y);
                    movedMaps = true;
                }

                if (newPos.Y < 0) {
                    if (!GameLoop.World.maps.ContainsKey(MapPos + new Point(0, -1))) { GameLoop.World.CreateMap(MapPos + new Point(0, -1)); }

                    MapPos += new Point(0, -1);
                    Position = new Point(newPos.X, GameLoop.World._mapHeight);
                    movedMaps = true;
                }

                if (newPos.Y >= GameLoop.World._mapHeight) {
                    if (!GameLoop.World.maps.ContainsKey(MapPos + new Point(0, 1))) { GameLoop.World.CreateMap(MapPos + new Point(0, 1)); }

                    MapPos += new Point(0, 1);
                    Position = new Point(newPos.X, 0);
                    movedMaps = true;
                }

                if (movedMaps && ID == GameLoop.World.Player.ID) {
                    GameLoop.UIManager.LoadMap(GameLoop.World.maps[MapPos]);
                   // GameLoop.UIManager.SyncMapEntities(GameLoop.World.maps[MapPos]);
                    GameLoop.UIManager.MessageLog.Add("Moved maps to " + MapPos.X + ", " + MapPos.Y);
                    return true;
                }
            }
            return false;
        }

        public bool MoveTo(Point newPosition) {
            Position = newPosition;
            return true;
        }
    }
}
