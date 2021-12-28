﻿using System;
using SadRogue.Primitives;
using SadConsole;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace LofiHollow.Entities {
    [JsonObject(MemberSerialization.OptIn)]
    public class Item : Entity {
        [JsonProperty]
        public int ItemID = 0;
        [JsonProperty]
        public int SubID = 0;
        [JsonProperty]
        public int ItemQuantity = 0;
        [JsonProperty]
        public bool IsStackable = false;
        [JsonProperty]
        public string Description = "";
        [JsonProperty]
        public int AverageValue = 0;
        [JsonProperty]
        public float Weight = 0.0f;
        [JsonProperty]
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
        // 11: Consumable
        [JsonProperty]
        public int EquipSlot = -1;
        // -1: Not equippable
        // 0: Main hand
        // 1: Off hand
        // 2: Head / Helmet
        // 3: Headband
        // 4: Eyes
        // 5: Shoulders
        // 6: Neck
        // 7: Chest
        // 8: Body
        // 9: Armor
        // 10: Belt
        // 11: Wrists
        // 12: Hands
        // 13: Ring 1
        // 14: Ring 2
        // 15: Feet


        [JsonProperty]
        public Weapon Weapon;

        [JsonProperty]
        public int ForegroundR = 0;
        [JsonProperty]
        public int ForegroundG = 0;
        [JsonProperty]
        public int ForegroundB = 0;
        [JsonProperty]
        public int ItemGlyph = 0;
         

        [JsonConstructor]
        public Item(Color foreground, int glyph) : base(foreground, Color.Transparent, glyph) { 
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context) {
            Appearance.Foreground = new Color(ForegroundR, ForegroundG, ForegroundB);
            Appearance.Glyph = ItemGlyph;
        }


        public Item(int ID) : base(Color.Black, Color.Transparent, 32) {
            if (GameLoop.World != null && GameLoop.World.itemLibrary != null && GameLoop.World.itemLibrary.ContainsKey(ID)) {
                Item temp = GameLoop.World.itemLibrary[ID];

                Name = temp.Name;
                ItemID = temp.ItemID;
                SubID = temp.SubID;
                ItemCategory = temp.ItemCategory;
                EquipSlot = temp.EquipSlot;
                IsStackable = temp.IsStackable; 
                Description = temp.Description;

                Weight = temp.Weight;
                AverageValue = temp.AverageValue;

                ForegroundR = temp.ForegroundR; 
                ForegroundG = temp.ForegroundG; 
                ForegroundB = temp.ForegroundB;
                ItemGlyph = temp.ItemGlyph;

                Appearance.Foreground = new Color(ForegroundR, ForegroundG, ForegroundB);
                Appearance.Glyph = ItemGlyph;

                Weapon = temp.Weapon;

                if (ID == 0)
                    ItemQuantity = 0;
                else
                    ItemQuantity = 1;
            }
        }


        public ColoredGlyph AsColoredGlyph() {
            return new ColoredGlyph(Appearance.Foreground, Color.Black, Appearance.Glyph);
        }
    }
}
