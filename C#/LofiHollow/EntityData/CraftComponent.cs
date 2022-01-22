using LofiHollow.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LofiHollow.EntityData {
    [JsonObject(MemberSerialization.OptIn)]
    public class CraftComponent {
        [JsonProperty]
        public string Property = "";
        [JsonProperty]
        public int Tier = 0;
        [JsonProperty]
        public int Quantity = 0;
        [JsonProperty]
        public bool Stacks = false;
        [JsonProperty]
        public bool CountsAsMultiple = false;
         
        [JsonConstructor]
        public CraftComponent() { }
         
        public CraftComponent(string prop, int tier, int qty, bool stack, bool countsAsMult) {
            Property = prop;
            Tier = tier;
            Quantity = qty;
            Stacks = stack;
            CountsAsMultiple = countsAsMult;
        }



        public bool ActorHasComponent(Actor act, int CraftAmount) {
            int heldQty = 0;
            int heldTotal = 0;
            for (int i = 0; i < act.Inventory.Length; i++) {
                if (act.Inventory[i].Craft != null) {
                    for (int j = 0; j < act.Inventory[i].Craft.Count; j++) {
                        if (act.Inventory[i].Craft[j].Property == Property && act.Inventory[i].Craft[j].Tier >= Tier) {
                            if (CountsAsMultiple)
                                heldTotal += act.Inventory[i].Craft[j].Tier;
                            heldQty += act.Inventory[i].ItemQuantity;
                        }
                    }
                }
            }

            if (CountsAsMultiple) {
                if (heldTotal >= Tier * CraftAmount)
                    return true;
            } else {
                if (heldQty >= Quantity * CraftAmount)
                    return true;
            }

            return false;
        }
    }
}
