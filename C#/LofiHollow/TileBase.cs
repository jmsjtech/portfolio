using System;
using Microsoft.Xna.Framework;
using SadConsole;

namespace LofiHollow {
    // TileBase is an abstract base class 
    // representing the most basic form of of all Tiles used.
    public class TileBase : Cell {
        public bool IsBlockingMove = false;
        public bool IsBlockingLOS = false;

        public string Name = "Grass";

        public int TileID = 0;

         
        public TileBase(Color fg, Color bg, int glyph, bool blockingMove = false, bool blockingLOS = false, string name = "") : base(fg, bg, glyph) {
            IsBlockingMove = blockingMove;
            IsBlockingLOS = blockingLOS;
            Name = name;
        }

        public ColoredGlyph AsColoredGlyph() {
            return new ColoredGlyph(Glyph, Foreground, Background);
        }
    }
}