using System;
using Microsoft.Xna.Framework;

namespace Jawara.Entities {
    // Creates a new player
    // Default colour is LightYellow and glyph is @
    public class Player : Actor {
        public Player(Color foreground, Color background) : base(foreground, background, '@') {

        }
    }
}
