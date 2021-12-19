using System;
using SadRogue.Primitives;
using SadConsole;

namespace LofiHollow {
    public class MinimapTile {
        public string name = "Void";
        public char ch = ' ';
        public Color fg = Color.White;
        public Color bg = Color.Black;

        public MinimapTile(char inCh, Color inFG, Color inBG) {
            ch = inCh;
            fg = inFG;
            bg = inBG;
        }

        public ColoredGlyph AsColoredGlyph() {
            return new ColoredGlyph(fg, bg, ch);
        }

    }
}
