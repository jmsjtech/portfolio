using Newtonsoft.Json;
using SadConsole;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LofiHollow.Entities {
    [JsonObject(MemberSerialization.OptIn)]
    public class FishDef {
        [JsonProperty]
        public int FishID = 0;
        [JsonProperty]
        public string Name = "";
        [JsonProperty]
        public string Season = "";
        [JsonProperty]
        public int EarliestTime = 0;
        [JsonProperty]
        public int LatestTime = 0;
        [JsonProperty]
        public string CatchLocation = "";
        [JsonProperty]
        public double MinWeightKG = 0;
        [JsonProperty]
        public double MaxWeightKG = 0;

        [JsonProperty]
        public int CoppersPerKG = 0;

        [JsonProperty]
        public int RequiredLevel = 0;
        [JsonProperty]
        public int GrantedExp = 0;

        [JsonProperty]
        public int colR = 0;
        [JsonProperty]
        public int colG = 0;
        [JsonProperty]
        public int colB = 0;
        [JsonProperty]
        public int glyph = 0;

        public ColoredString GetAppearance() {
            return new ColoredString(((char) glyph).ToString(), new Color(colR, colG, colB), Color.Black);
        }
    }
}
