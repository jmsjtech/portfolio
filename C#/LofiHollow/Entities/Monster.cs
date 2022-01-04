using System;
using Newtonsoft.Json;
using SadRogue.Primitives;

namespace LofiHollow.Entities {
    public class Monster : Actor {
        [JsonProperty]
        public int MonsterID = -1;

        [JsonConstructor]
        public Monster() : base(Color.White, 'e') { 
        }


        public Monster(Color foreground, int glyph, int ID, string name) : base(foreground, glyph) {
            MonsterID = ID;
            Name = name;
        }

        public Monster(int ID, Color fg, int glyph) : base(fg, glyph) {
            if (GameLoop.World.monsterLibrary != null && GameLoop.World.monsterLibrary.ContainsKey(ID)) {
                Monster temp = GameLoop.World.monsterLibrary[ID];

                Name = temp.Name;
                Appearance.Foreground = temp.Appearance.Foreground;
                Appearance.Glyph = temp.Appearance.Glyph;
                MonsterID = temp.MonsterID;

                SetAttribs(temp.STR, temp.DEX, temp.CON, temp.INT, temp.WIS, temp.CHA);

                LandSpeed = temp.LandSpeed;
                BaseAttackBonus = temp.BaseAttackBonus;
                SizeMod = temp.SizeMod;
                HitDice = temp.HitDice;
                CR = temp.CR;
                InitiativeMod = temp.InitiativeMod;
                Templates = temp.Templates;
            }
        }
    }
}
