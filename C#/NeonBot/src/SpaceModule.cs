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
    public class SpaceModule : ModuleBase<SocketCommandContext> {
        Random rand = new Random();

        [Command("")]
        public async Task DetailDefaultAsync() {
            string detailList = "<@" + Context.User.Id + ">, the possible commands for `!space` are: `worldtype`.";
            await ReplyAsync(detailList);
        }


        [Command("worldtype")]
        public async Task PathAsync() {
            string output = SpaceGen.AtmosphericComposition(SpaceGen.WorldType());

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