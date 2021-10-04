using Discord;
using Discord.Commands;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace NeonBot {
    
    [Group("cradle")]
    public class CradleModule : ModuleBase<SocketCommandContext> {
        Random rand = new Random();

        [Command("")]
        public async Task DetailDefaultAsync() {
            string detailList = "<@" + Context.User.Id + ">, the possible commands for `!cradle` are: `path`.";
            await ReplyAsync(detailList);
        }


        [Command("path")]
        public async Task PathAsync() {
            Random rand = new Random();

            string[] madraTypes = {
                "Fire", "Water", "Stone", "Wind", "Light", "Shadow", "Force",
                "Life", "Death", "Blood", "Dream", "Sword", "Destruction", "Spatial",
                "Hunger", "Cloud", "Luck", "Poison", "Electric", "Joy", "Despair",
                "Anger", "Fear", "Intoxication", "Ice", "Inertia" };

            string output = "";
            int[] amounts = { 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 3, 3, 4 };
            int amount = amounts[rand.Next(0, amounts.Length)];

            for (int i = 0; i < amount; i++) {
                if (i == 0) { output = "Path Auras: "; }
                if (i != 0) {
                    output += ", ";
                }
                if (i == amount - 1 && amount != 1) {
                    output += "and ";
                }

                output += madraTypes[rand.Next(0, madraTypes.Length)];

                
            }



            await ReplyAsync(output);
        }

        [Command("name")]
        public async Task NameAsync([Remainder] string number) {
            Random rand = new Random();

            string[] nouns = {
                "Act", "Air", "Animal", "Art", "Balance", "Blood", "Body", "Burst", "Chance",
                "Cloth", "Copy", "Crack", "Current", "Day", "Death", "Detail", "Direction", "Disease",
                "Drink", "Dust", "Earth", "Edge", "Expanse", "Field", "Flame", "Flight", "Flower", "Front",
                "Glass", "Grain", "Grass", "Heat", "Harmony", "Impulse", "Ink", "Land", "Liquid", "Mark",
                "Memory", "Mind", "Mine", "Mist", "Morning", "Mountain", "Motion", "Music", "Night",
                "Order", "Page", "Pain", "Peace", "Plant", "Poison", "Rain", "River", "Salt", "Sand",
                "Sea", "Self", "Shade", "Silk", "Sky", "Snow", "Smoke", "Society", "Space", "Steam",
                "Steel", "Story", "Time", "Water", "Wax", "Weight", "Wind", "Ant", "Apple", "Army",
                "Basket", "Bee", "Bell", "Berry", "Bird", "Blade", "Board", "Bone", "Branch", "Bridge",
                "Card", "Cat", "Clock", "Cloud", "Dog", "Door", "Eye", "Face", "Feather", "Garden", 
                "Hammer", "Hand", "Heart", "Hook", "Island", "Jewel", "Key", "Knot", "Leaf", "Lock",
                "Map", "Monkey", "Moon", "Needle", "Net", "Ring", "Screw", "Seed", "Snake", "Star",
                "Sun", "Thread", "Tooth", "Tree", "Wheel", "Fox", "Viper", "Shield", "Spear", "Sword",
                "Oath", "Abyss", "Guardian", "Crown", "King", "Rabbit", "Tiger", "Pig", "Goat", "Ox",
                "Dragon", "Horse", "Rooster", "Rat"
            };

            string[] verbs = {
                "Hot", "Mental", "Old", "Amazing", "Amazed", "Charming", "Charmed", "Confusing",
                "Exciting", "Excited", "Inspiring", "Inspired", "Pleased", "Relaxed", "Satisfying", "Satisfied",
                "Tiring", "Touched", "Abnormal", "Adventurous", "Arrogant", "Beautiful", "Bitter", "Bleak",
                "Bold", "Bright", "Calm", "Cautious", "Clever", "Clear", "Common", "Cruel", "Defiant", "Deep",
                "Diligent", "Dim", "Dreaming", "Elegant", "Energetic", "Even", "Far", "Fatal", "Ferocious",
                "Fervent", "Fierce", "Furious", "Generous", "Gentle", "Graceful", "Innocent", "Irritable",
                "Just", "Keen", "Kind", "Light", "Limp", "Loyal", "Majestic", "Natural", "Mysterious", "Perfect",
                "Playful", "Powerful", "Quick", "Quiet", "Random", "Lucky", "Righteous", "Rigid", "Safe",
                "Silent", "Smooth", "Solemn", "Solid", "Swift", "Triumphant", "Ultimate", "Unnatural", "Vicious",
                "Warm", "Weak", "Zealous", "Beaming", "Brilliant", "Colorful", "Deep", "Delicate", "Electric",
                "Festive", "Fiery", "Fresh", "Glistening", "Glittering", "Harmonious", "Iridescent", "Opalescent",
                "Radiant", "Vibrant", "Vivid", "Ashy", "Brash", "Cold", "Dark", "Harsh", "Loud", "Opaque", "Gaudy",
                "Stained", "Faded", "Crimson", "Crimson", "Golden", "Rose", "Ivory", "Olive", "Lime", "Lavender", 
                "Violet", "Silver", "Black", "Copper", "Iron", "Steel", "Brass", "Cobalt", "Violent", "Sapphire",
                "Ruby", "Emerald", "Jade", "Diamond", "Pearl", "Platinum", "Crystal", "Broken", "Unstained", 
                "Endless", "Celestial", "Hungry", "White", "Twin", "Stellar", "Twisting", "Last", "First",
                "Grasping", "Whispering", "Flowing", "Dawn", "Storm", "Hollow"
            };

            string output = "";

            if (Int32.TryParse(number, out int multiple)) {
                Console.WriteLine(multiple);
                if (multiple > 0) {
                    for (int i = 0; i < multiple; i++) {
                        output += "The Path of the " + verbs[rand.Next(0, verbs.Length)] + " " + nouns[rand.Next(0, nouns.Length)] + "\n";
                    }
                } else {
                    output = (verbs.Length * nouns.Length).ToString();
                }
            }



            await ReplyAsync(output);
        }




        //[Command("building")]
        //public async Task BuildingAsync() {
        //    var userInfo = Context.User;
        //    string output = "<@" + userInfo.Id + ">:\n";


        //    await ReplyAsync(output);
        //}
    }
}