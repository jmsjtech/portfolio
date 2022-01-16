using Newtonsoft.Json;
using SadConsole;
using SadRogue.Primitives;
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

        [JsonProperty]
        public List<string> Uses = new List<string>();

        public int ExpToLevel() {
            return (int) (75 * Math.Pow(2, Level / 7));
        }

        public void GrantExp(int gained) {
            Experience += gained;
            if (Experience >= ExpToLevel()) {
                Experience -= ExpToLevel();
                Level++;
                GameLoop.UIManager.AddMsg(new ColoredString("You leveled " + Name + " to " + Level + "!", Color.Cyan, Color.Black));
            }
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
