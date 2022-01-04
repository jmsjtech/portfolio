using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LofiHollow {
    [JsonObject(MemberSerialization.OptIn)]
    public class ClassFeature {
        [JsonProperty]
        public string Name = "";
        [JsonProperty]
        public string Selection = "";
        [JsonProperty]
        public string BonusTo = "";
        [JsonProperty]
        public int NumericalBonus = 0;
    }
}
