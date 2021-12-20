using System;
using SadRogue.Primitives;
using SadConsole;

namespace LofiHollow.Entities {
    public class Item : Entity {
        public int ItemID = 0;
        public char Glyph = (char) 32;
        public Color Foreground = Color.Black;
        public int ItemCategory = -1;
        // -1: Debug / Empty
        // 0: Weapon
        // 1: Watering Can
        // 2: Pickaxe
        // 3: Axe
        // 4: Hoe
        // 5: Hammer
        // 6: Helmet
        // 7: Chestplate
        // 8: Leggings
        // 9: Ring
        // 10: Amulet
        
        public int EquipSlot = -1;
        // -1: Not equippable
        // 0: Main hand
        // 1: Off hand
        // 2: Head
        // 3: Torso
        // 4: Legs
        // 5: Ring
        // 6: Amulet

        public Item(Color foreground, Color background, int glyph, string name, int id, int itemCat, int equip) : base(foreground, background, glyph) {
            Name = name;
            Foreground = foreground;
            Glyph = (char) glyph;
            ItemID = id;
            ItemCategory = itemCat;
            EquipSlot = equip;
        }


        public ColoredGlyph AsColoredGlyph() {
            return new ColoredGlyph(Foreground, Color.Black, Glyph);
        }
    }
}
