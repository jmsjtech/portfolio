using LofiHollow.Entities;
using LofiHollow.EntityData;
using LofiHollow.Managers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LofiHollow {
    [JsonObject(MemberSerialization.OptIn)]
    public class CraftingRecipe {
        [JsonProperty]
        public string Name = "";
        [JsonProperty]
        public int FinishedID = 0;
        [JsonProperty]
        public int FinishedQty = 1;
        [JsonProperty]
        public string Skill = "";
        [JsonProperty]
        public int RequiredLevel = 1;
        [JsonProperty]
        public int ExpGranted = 0;

        [JsonProperty]
        public List<ConstructionMaterial> SpecificMaterials = new();
        [JsonProperty]
        public List<CraftComponent> GenericMaterials = new();
        [JsonProperty]
        public List<ToolData> RequiredTools = new();

        public void Craft(Actor act, int quantity) {
            if (ActorCanCraft(act, Skill, quantity)) {
                for (int i = 0; i < SpecificMaterials.Count; i++) {
                    int quantityLeft = SpecificMaterials[i].ItemQuantity * quantity;
                    for (int j = 0; j < act.Inventory.Length; j++) {
                        if (act.Inventory[j].ItemID == SpecificMaterials[i].ItemID && act.Inventory[j].SubID == SpecificMaterials[i].SubID) {
                            if (act.Inventory[j].ItemQuantity > quantityLeft) {
                                act.Inventory[j].ItemQuantity -= quantityLeft;
                                quantityLeft = 0;
                            } else if (act.Inventory[j].ItemQuantity == quantityLeft) {
                                quantityLeft = 0;
                                act.Inventory[j] = new(0);
                            } else {
                                quantityLeft -= act.Inventory[j].ItemQuantity;
                                act.Inventory[j] = new(0);
                            }
                        }

                        if (quantityLeft == 0) {
                            break;
                        }
                    }
                }

                for (int i = 0; i < GenericMaterials.Count; i++) {
                    int amountLeft = GenericMaterials[i].Quantity * quantity;
                    if (GenericMaterials[i].CountsAsMultiple)
                        amountLeft *= GenericMaterials[i].Tier;

                    for (int j = 0; j < act.Inventory.Length; j++) {
                        if (act.Inventory[j].Craft != null) {
                            for (int k = 0; k < act.Inventory[j].Craft.Count; k++) {
                                if (act.Inventory[j].Craft[k].Property == GenericMaterials[i].Property) {
                                    if (act.Inventory[j].Craft[k].CountsAsMultiple) {
                                        if (act.Inventory[j].Craft[k].Tier > amountLeft) {
                                            amountLeft = 0;
                                            if (act.Inventory[j].ItemQuantity > 1)
                                                act.Inventory[j].ItemQuantity--;
                                            else
                                                act.Inventory[j] = new(0);
                                        } else if (act.Inventory[j].Craft[k].Tier == amountLeft) {
                                            amountLeft = 0;
                                            if (act.Inventory[j].ItemQuantity > 1)
                                                act.Inventory[j].ItemQuantity--;
                                            else
                                                act.Inventory[j] = new(0);
                                        } else {
                                            if (act.Inventory[j].ItemQuantity * act.Inventory[j].Craft[k].Tier > amountLeft) {
                                                amountLeft = 0;
                                                int amountRequired = (int) Math.Ceiling((double) amountLeft / (double) act.Inventory[j].Craft[k].Tier);
                                                act.Inventory[j].ItemQuantity -= amountRequired;
                                                if (act.Inventory[j].ItemQuantity == 0)
                                                    act.Inventory[j] = new(0); 
                                            } else if (act.Inventory[j].ItemQuantity * act.Inventory[j].Craft[k].Tier == amountLeft) {
                                                amountLeft = 0;
                                                act.Inventory[j] = new(0);
                                            } else {
                                                amountLeft -= act.Inventory[j].Craft[k].Tier * act.Inventory[j].ItemQuantity;
                                                act.Inventory[j] = new(0);
                                            }
                                        } 
                                    } else {
                                        if (act.Inventory[j].ItemQuantity > amountLeft) {
                                            act.Inventory[j].ItemQuantity -= amountLeft;
                                            amountLeft = 0; 
                                        } else if (act.Inventory[j].ItemQuantity == amountLeft) {
                                            amountLeft = 0;
                                            act.Inventory[j] = new(0);
                                        } else {
                                            amountLeft -= act.Inventory[j].ItemQuantity;
                                            act.Inventory[j] = new(0);
                                        }
                                    }
                                }

                                if (amountLeft <= 0) {
                                    break;
                                }
                            }
                        }

                        if (amountLeft <= 0) {
                            break;
                        }
                    }
                }

                Item finished = new(FinishedID);
                finished.ItemQuantity = (FinishedQty * quantity);
                CommandManager.AddItemToInv(act, finished);

                act.Skills[Skill].GrantExp(ExpGranted * quantity);
            }
        }


        public bool ActorCanCraft(Actor act, string menuSkill, int quantity) {
            if (menuSkill != Skill)
                return false;
            if (!act.Skills.ContainsKey(Skill))
                return false;
            if (act.Skills[Skill].Level < RequiredLevel)
                return false;

            for (int i = 0; i < RequiredTools.Count; i++) {
                if (!RequiredTools[i].ActorHasTool(act))
                    return false;
            }

            for (int i = 0; i < GenericMaterials.Count; i++) {
                if (!GenericMaterials[i].ActorHasComponent(act, quantity))
                    return false;
            }

            for (int i = 0; i < SpecificMaterials.Count; i++) {
                if (!SpecificMaterials[i].ActorHasComponent(act, quantity))
                    return false;
            }

            return true;
        }
    }
}
