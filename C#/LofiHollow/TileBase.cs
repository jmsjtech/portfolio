using System;
using SadRogue.Primitives;
using SadConsole;

namespace LofiHollow {
    // TileBase is an abstract base class 
    // representing the most basic form of of all Tiles used.
    public class TileBase : ColoredGlyph {
        public bool IsBlockingMove = false;
        public bool IsBlockingLOS = false;
        public bool SpawnsMonsters = false;

        public string Name = "Grass";

        public int TileID = 0;

         
        public TileBase(Color fg, Color bg, int glyph, bool blockingMove = false, bool blockingLOS = false, string name = "", int id = 0, bool spawns = false) : base(fg, bg, glyph) {
            IsBlockingMove = blockingMove;
            IsBlockingLOS = blockingLOS;
            Name = name;
            TileID = id;
            SpawnsMonsters = spawns;
        }

        public void SetNewFG(Color fg, int glyph) {
            Foreground = fg;
            Glyph = glyph;
        }
    }
}