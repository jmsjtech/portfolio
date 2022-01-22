using LofiHollow.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LofiHollow {
    [JsonObject(MemberSerialization.OptIn)]
    public class ConstructionMaterial {
        [JsonProperty]
        public int ItemID = 0;
        [JsonProperty]
        public int SubID = 0;
        [JsonProperty]
        public int ItemQuantity = 0;
        [JsonProperty]
        public bool Stacks = false;


        [JsonConstructor]
        public ConstructionMaterial() { }

        public ConstructionMaterial(int id, int qty, bool stacks) {
            ItemID = id;
            ItemQuantity = qty;
            Stacks = stacks;
        }

        public bool ActorHasComponent(Actor act, int CraftAmount) {
            int heldQty = 0;

            for (int i = 0; i < act.Inventory.Length; i++) {
                if (act.Inventory[i].ItemID == ItemID && act.Inventory[i].SubID == SubID) {
                    heldQty += act.Inventory[i].ItemQuantity;
                }
            }

            if (heldQty >= ItemQuantity * CraftAmount)
                return true;

            return false;
        }
    }
}
