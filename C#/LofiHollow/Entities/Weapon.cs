using System;
using SadRogue.Primitives;
using SadConsole;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace LofiHollow.Entities {
    public class Weapon {
        public string DamageDice = "1d1";
        public int CritMod = 2;
        public int MinCrit = 20;
        public string WeaponCategory = "";
        public string WeaponClass = "";
        public string DamageType = "";

        [JsonConstructor]
        public Weapon(){

        } 
    }
}
