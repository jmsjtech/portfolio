using System;
using Microsoft.Xna.Framework;

namespace Jawara {
    public abstract class Actor : SadConsole.Entities.Entity {
        private int _health; //current health
        private int _maxHealth; //maximum possible health

        public int Health { get { return _health; } set { _health = value; } } // public getter for current health
        public int MaxHealth { get { return _maxHealth; } set { _maxHealth = value; } } // public setter for current health

        protected Actor(Color foreground, Color background, int glyph, int width = 1, int height = 1) : base(width, height) {
            Animation.CurrentFrame[0].Foreground = foreground;
            Animation.CurrentFrame[0].Background = background;
            Animation.CurrentFrame[0].Glyph = glyph;
        }

        public bool MoveBy(Point positionChange) {
            // Check the map if we can move to this new position
            if (GameLoop.GameMap.IsTileWalkable(Position + positionChange)) {
                Position += positionChange;
                return true;
            } else
                return false;
        }

        public bool MoveTo(Point newPosition) {
            Position = newPosition;
            return true;
        }
    }
}