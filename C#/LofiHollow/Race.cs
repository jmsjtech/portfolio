using Newtonsoft.Json;
using System.Collections.Generic;

namespace LofiHollow {
    [JsonObject(MemberSerialization.OptIn)]
    public class Race {
        [JsonProperty]
        public int RaceID = 0;
        [JsonProperty]
        public string Name = "";
        [JsonProperty]
        public int SizeMod = 0;
        [JsonProperty]
        public string Description = "";
        [JsonProperty]
        public int StrMod = 0;
        [JsonProperty]
        public int DexMod = 0;
        [JsonProperty]
        public int ConMod = 0;
        [JsonProperty]
        public int IntMod = 0;
        [JsonProperty]
        public int WisMod = 0;
        [JsonProperty]
        public int ChaMod = 0;
        [JsonProperty]
        public int LandSpeed = 30;

        [JsonProperty]
        public List<string> Languages = new List<string>();

        [JsonProperty]
        public List<string> RacialFeatures = new List<string>();

        [JsonConstructor]
        public Race() {

        }

        public void Copy(Race other) {
            RaceID = other.RaceID; 
            Name = other.Name; 
            SizeMod = other.SizeMod; 
            Description = other.Description; 
            StrMod = other.StrMod; 
            DexMod = other.DexMod; 
            ConMod = other.ConMod; 
            IntMod = other.IntMod; 
            WisMod = other.WisMod; 
            ChaMod = other.ChaMod; 
            LandSpeed = other.LandSpeed;
        } 
    }
}
