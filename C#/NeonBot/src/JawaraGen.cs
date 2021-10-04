using Discord;
using Discord.Commands;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace NeonBot {

    [Group("space")]
    public class JawaraGen : ModuleBase<SocketCommandContext> {
        Random rand = new Random();

        [Command("")]
        public async Task DetailDefaultAsync() {
            string detailList = "<@" + Context.User.Id + ">, the possible commands for `!jawara` are: `animal`, `mineral`, `plant`, and `word`.";
            await ReplyAsync(detailList);
        }


        [Command("animal")]
        public async Task AnimalAsync() {
            string output = SpaceGen.AtmosphericComposition(SpaceGen.WorldType());

            await ReplyAsync(output);
        }

        [Command("mineral")]
        public async Task MineralAsync() {
            string output = SpaceGen.AtmosphericComposition(SpaceGen.WorldType());

            await ReplyAsync(output);
        }

        [Command("plant")]
        public async Task PlantAsync() {
            string output = SpaceGen.AtmosphericComposition(SpaceGen.WorldType());

            await ReplyAsync(output);
        }

        [Command("word")]
        public async Task WordAsync([Remainder] string number) {
            string output = "";

            if (Int32.TryParse(number, out int multiple)) {
                if (multiple > 0) {
                    for (int i = 0; i < multiple; i++) {
                        if (i == 0) {
                            output += GetSyllable(true, rand);
                        } else {
                            output += GetSyllable(false, rand);
                        }
                    }
                }
            } else {
                output = number;
            }

            await ReplyAsync(output);
        }

        public static String GetSyllable(bool firstSyl, Random rand) {
            string[] consonants = { "_", "b", "zh", "f", "h", "j", "k", "l", "m", "n", "p", "r", "t", "w", "s" };
            string[] vowels = { "ae", "a", "e", "i", "o", "u", "y", "uh" };

            if (firstSyl) {
                return consonants[rand.Next(consonants.Length)] + "" + vowels[rand.Next(vowels.Length)];
            } else {
                return consonants[(rand.Next(consonants.Length - 1)) + 1] + "" + vowels[rand.Next(vowels.Length)];
            }
        }

        //[Command("building")]
        //public async Task BuildingAsync() {
        //    var userInfo = Context.User;
        //    string output = "<@" + userInfo.Id + ">:\n";


        //    await ReplyAsync(output);
        //}
    }
}