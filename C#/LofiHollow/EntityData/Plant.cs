using LofiHollow.Entities;
using LofiHollow.EntityData;
using LofiHollow.Managers;
using Newtonsoft.Json;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LofiHollow.EntityData {
    [JsonObject(MemberSerialization.OptIn)]
    public class Plant {
        [JsonProperty]
        public int CurrentStage = 0;

        [JsonProperty]
        public int DayCounter = 0;

        [JsonProperty]
        public string GrowthSeason = "";

        [JsonProperty]
        public int HarvestRevert = -1;

        [JsonProperty]
        public string ProduceName = "";

        [JsonProperty]
        public int ProduceValue = 0;

        [JsonProperty]
        public double ProduceWeight = 0;

        [JsonProperty]
        public int RequiredLevel = 0;

        [JsonProperty]
        public int ExpOnHarvest = 0;

        [JsonProperty]
        public int ExpPerExtra = 0;

        [JsonProperty]
        public int ProducePerHarvestMin = 0;
        [JsonProperty]
        public int ProducePerHarvestMax = 0;

        [JsonProperty]
        public int ProduceGlyph = 0;

        [JsonProperty]
        public int ProduceR = 0;
        [JsonProperty]
        public int ProduceG = 0;
        [JsonProperty]
        public int ProduceB = 0;
        [JsonProperty]
        public Decorator ProduceDec;

        [JsonProperty]
        public bool ProduceIsSeed = false;

        [JsonProperty]
        public bool WateredToday = false;



        [JsonProperty]
        public List<PlantStage> Stages = new();


        [JsonConstructor]
        public Plant() {

        }

        public Plant(Plant other) {
            CurrentStage = 0;
            DayCounter = 0;
            GrowthSeason = other.GrowthSeason;
            HarvestRevert = other.HarvestRevert;
            ProduceName = other.ProduceName;
            ProduceValue = other.ProduceValue;
            ProduceWeight = other.ProduceWeight;
            ProducePerHarvestMin = other.ProducePerHarvestMin;
            ProducePerHarvestMax = other.ProducePerHarvestMax;
            ProduceIsSeed = other.ProduceIsSeed;
            ProduceGlyph = other.ProduceGlyph;
            ProduceR = other.ProduceR;
            ProduceG = other.ProduceG;
            ProduceB = other.ProduceB;
            WateredToday = false;
            Stages = other.Stages;
            ProduceDec = other.ProduceDec;
        }

        public void DayUpdate() {
            if (GameLoop.World.Player.Clock.GetSeason() == GrowthSeason || GrowthSeason == "Any") {
                if (WateredToday) { 
                    DayCounter++;
                    if (DayCounter > Stages[CurrentStage].DaysToNext) {
                        if (Stages.Count > CurrentStage + 1) { 
                            CurrentStage++;
                            DayCounter = 1;
                        }
                    }
                    WateredToday = false;
                }
            }
        }

        public void Harvest(Actor harvester) {
            if (CurrentStage != -1 && Stages[CurrentStage].HarvestItem != -1) {
                Item produce = new(Stages[CurrentStage].HarvestItem);
                produce.Name = ProduceName;
                produce.Weight = (float) ProduceWeight;
                produce.AverageValue = ProduceValue;
                produce.ItemQuantity = ProducePerHarvestMin;
                if (ProducePerHarvestMax > ProducePerHarvestMin)
                    produce.ItemQuantity += GameLoop.rand.Next(ProducePerHarvestMax - ProducePerHarvestMin);
                produce.Position = harvester.Position;
                produce.MapPos = harvester.MapPos;

                if (ProduceIsSeed) {
                    produce.Plant = new(this);
                    produce.Plant.CurrentStage = 0;
                    produce.Plant.DayCounter = 0;
                }

                produce.ItemGlyph = ProduceGlyph;
                produce.ForegroundR = ProduceR;
                produce.ForegroundG = ProduceG;
                produce.ForegroundB = ProduceB;
                produce.Dec = ProduceDec;
                produce.UpdateAppearance();
                

                CommandManager.AddItemToInv(harvester, produce);

                CurrentStage = HarvestRevert;
                DayCounter = 0; 
            }
        }
    }
}
