using System;
using SadRogue.Primitives;
using SadConsole;
using Newtonsoft.Json;

namespace LofiHollow.Entities {
    public class Armor {
        public int ArmorBonus = 0;
        public int MaxDexBonus = 0;
        public int ArmorCheckPenalty = 0;
        public int ArcaneSpellFailureChance = 0;
        public int SpeedReduction = 0;


        [JsonConstructor]
        public Armor() {

        }

        public Armor(Color fg, int glyph, int AC, int maxDex, int ACP, int ASFail, int Speed) {
            ArmorBonus = AC;
            MaxDexBonus = maxDex;
            ArmorCheckPenalty = ACP;
            ArcaneSpellFailureChance = ASFail;
            SpeedReduction = Speed;
        }
    }
}
