using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LofiHollow {
    [JsonObject(MemberSerialization.OptIn)]
    public class ClassDef {
        [JsonProperty]
        public int ClassID = 0;
        [JsonProperty]
        public string Name = "";
        [JsonProperty]
        public string HitDie = "1d4";
        [JsonProperty]
        public float BABperLevel = 1;
        [JsonProperty]
        public int SkillRanks = 2; 

        [JsonProperty]
        public int ClassLevels = 0;


        [JsonProperty]
        public string FortSaveProg = "Good";
        [JsonProperty]
        public string RefSaveProg = "Bad";
        [JsonProperty]
        public string WillSaveProg = "Bad";


        [JsonConstructor]
        public ClassDef() {

        }

        public void Copy(ClassDef other) {
            ClassID = other.ClassID; 
            Name = other.Name; 
            HitDie = other.HitDie; 
            BABperLevel = other.BABperLevel; 
            SkillRanks = other.SkillRanks;
            ClassLevels = other.ClassLevels; 
            FortSaveProg = other.FortSaveProg; 
            RefSaveProg = other.RefSaveProg; 
            WillSaveProg = other.WillSaveProg;
        }


        public int GetSave(string progression) {
            if (progression == "Good") {
                switch (ClassLevels) {
                    case 1: return 2;
                    case 2: case 3: return 3;
                    case 4: case 5: return 4;
                    case 6: case 7: return 5;
                    case 8: case 9: return 6;
                    case 10: case 11: return 7;
                    case 12: case 13: return 8;
                    case 14: case 15: return 9;
                    case 16: case 17: return 10;
                    case 18: case 19: return 11;
                    case 20: return 12;
                }
            }

            if (progression == "Bad") {
                switch (ClassLevels) {
                    case 1: case 2: return 0;
                    case 3: case 4: case 5: return 1;
                    case 6: case 7: case 8: return 2;
                    case 9: case 10: case 11: return 3;
                    case 12: case 13: case 14: return 4;
                    case 15: case 16: case 17: return 5;
                    case 18: case 19: case 20: return 6;
                }
            }

            return 0;
        }
    }
}
