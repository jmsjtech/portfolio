using System;
using SadRogue.Primitives;

namespace LofiHollow.Entities {
    public class Monster : Actor {
        public int MonsterID = -1;

        public Monster(Color foreground, int glyph, int ID, string name, int vitality, int speed, int attack, int defense, int matk, int mdef) : base(foreground, Color.Black, glyph) {
            MonsterID = ID;
            Name = name;
            BaseVitality = vitality;
            BaseSpeed = speed;
            BaseAttack = attack;
            BaseDefense = defense;
            BaseMagicAttack = matk;
            BaseMagicDefense = mdef;
        }

        public Monster(int ID, Color fg, int glyph) : base(fg, Color.Black, glyph) {
            if (GameLoop.World.monsterLibrary != null && GameLoop.World.monsterLibrary.ContainsKey(ID)) {
                Monster temp = GameLoop.World.monsterLibrary[ID];

                Name = temp.Name;
                Appearance.Foreground = temp.Appearance.Foreground;
                Appearance.Glyph = temp.Appearance.Glyph;
                MonsterID = temp.MonsterID;
                BaseVitality = temp.BaseVitality;
                BaseSpeed = temp.BaseSpeed;
                BaseAttack = temp.BaseAttack;
                BaseDefense = temp.BaseDefense;
                BaseMagicAttack = temp.BaseMagicAttack;
                BaseMagicDefense = temp.BaseMagicDefense;

            }
        }
    }
}
