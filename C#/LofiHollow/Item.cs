using System;
using SadRogue.Primitives;
using SadConsole;

namespace LofiHollow.Entities {
    public class Item : Entity {
        public int ItemID = 0;
        public char Glyph = (char) 32;
        public Color Foreground = Color.Black;
        public int ItemCategory = -1;
        public int ItemQuantity = 1;
        public bool IsStackable = false;
        public int NumericalBonus = 0;

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

        public Item(Color foreground, Color background, int glyph, string name, int id, int itemCat, int equip, bool stackable, int num) : base(foreground, background, glyph) {
            Name = name;
            Foreground = foreground;
            Glyph = (char) glyph;
            ItemID = id;
            ItemCategory = itemCat;
            EquipSlot = equip;
            IsStackable = stackable;
            NumericalBonus = num;
        }

        public Item(int ID) : base(Color.Black, Color.Black, 32) {
            if (GameLoop.World != null && GameLoop.World.itemLibrary != null && GameLoop.World.itemLibrary.ContainsKey(ID)) {
                Item temp = GameLoop.World.itemLibrary[ID];

                Name = temp.Name;
                Foreground = temp.Foreground;
                Glyph = temp.Glyph;
                ItemID = temp.ItemID;
                ItemCategory = temp.ItemCategory;
                EquipSlot = temp.EquipSlot;
                IsStackable = temp.IsStackable;
                NumericalBonus = temp.NumericalBonus;
            }
        }


        public ColoredGlyph AsColoredGlyph() {
            return new ColoredGlyph(Foreground, Color.Black, Glyph);
        }
    }
}
