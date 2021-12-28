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
        string Name = "";
        [JsonProperty]
        string Selection = "";
    }
}
