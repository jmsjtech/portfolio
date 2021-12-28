using System;
using SadRogue.Primitives;
using SadConsole;

namespace LofiHollow.Entities {
    public class Armor : Item {
        public int ArmorBonus = 0;
        public int MaxDexBonus = 0;
        public int ArmorCheckPenalty = 0;
        public int ArcaneSpellFailureChance = 0;
        public int SpeedReduction = 0;


        public Armor(Color foreground, int glyph) : base(foreground, glyph) {

        }

        public Armor(Color fg, int glyph, int AC, int maxDex, int ACP, int ASFail, int Speed) : base(fg, glyph) {
            ArmorBonus = AC;
            MaxDexBonus = maxDex;
            ArmorCheckPenalty = ACP;
            ArcaneSpellFailureChance = ASFail;
            SpeedReduction = Speed;
        }
    }
}
