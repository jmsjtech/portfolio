using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LofiHollow {
    [JsonObject(MemberSerialization.OptIn)] 
    public class Skill {
        [JsonProperty]
        public int SkillID = -1;
        [JsonProperty]
        public string Name = "";
        [JsonProperty]
        public int Level = 1;
        [JsonProperty]
        public int Experience = 0;

        public int ExpToLevel() {
            return (int) (75 * Math.Pow(2, Level / 7));
        }

        [JsonConstructor]
        public Skill() {

        }


        public Skill(Skill other) {
            SkillID = other.SkillID;
            Name = other.Name;
            Level = other.Level;
            Experience = other.Experience;
        }
    }
}
