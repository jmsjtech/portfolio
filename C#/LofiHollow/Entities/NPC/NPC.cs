using Newtonsoft.Json;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LofiHollow.Entities.NPC {
    [JsonObject(MemberSerialization.OptIn)]
    public class NPC : Actor {
        [JsonProperty]
        public int npcID;
        [JsonProperty]
        public NPCAi AI;
        [JsonProperty]
        public string Occupation = "";

        [JsonProperty]
        public int BirthMonth = 1;
        [JsonProperty]
        public int BirthDay = 2;
         
        [JsonProperty]
        public string Introduction = "";

        [JsonProperty]
        public Dictionary<string, string> Greetings = new Dictionary<string, string>();

        [JsonProperty]
        public Dictionary<string, string> Farewells = new Dictionary<string, string>();

        [JsonProperty]
        public Dictionary<string, string> ChitChats = new Dictionary<string, string>();

        [JsonProperty]
        public Dictionary<string, string> GiftResponses = new Dictionary<string, string>();

        [JsonProperty]
        public List<int> HatedGiftIDs = new List<int>();
        [JsonProperty]
        public List<int> DislikedGiftIDs = new List<int>(); 
        [JsonProperty]
        public List<int> LikedGiftIDs = new List<int>();
        [JsonProperty]
        public List<int> LovedGiftIDs = new List<int>();

        public bool ReceivedGiftToday = false; 

        [JsonProperty]
        public ShopData Shop = new ShopData();

        [JsonConstructor]
        public NPC() : base(Color.White, '@') {
            Appearance.Foreground = new Color(ForegroundR, ForegroundG, ForegroundB);
            Appearance.Glyph = ActorGlyph;
        }


        public NPC(NPC other) : base(Color.White, '@') {
            AI = other.AI;
            ForegroundR = other.ForegroundR;
            ForegroundG = other.ForegroundG;
            ForegroundB = other.ForegroundB;
            ActorGlyph = other.ActorGlyph;

            Appearance.Foreground = new Color(ForegroundR, ForegroundG, ForegroundB);
            Appearance.Glyph = ActorGlyph;
        }

        public void Update(bool newSchedule) {
            if (newSchedule || AI.Current == null) {
                string season = GameLoop.World.Player.GetSeason();

                if (GameLoop.World.Player.IsItThisDay(BirthMonth, BirthDay))
                    season = "Birthday";


                AI.SetSchedule(season, "Sunny");
            }

            AI.MoveTowardsNode(GameLoop.World.Player.GetCurrentTime(), this);
        }

        public string RelationshipDescriptor() {
            int PlayerOpinion = 0;
            
            if (GameLoop.World.Player.MetNPCs.ContainsKey(Name)) {
                PlayerOpinion = GameLoop.World.Player.MetNPCs[Name];
            }

            if (PlayerOpinion == -100) 
                return "Nemesis"; 

            if (PlayerOpinion <= -50)
                return "Hate";

            if (PlayerOpinion <= -25)
                return "Unfriendly"; 

            if (PlayerOpinion <= -10)
                return "Dislike"; 

            if (PlayerOpinion < 10)
                return "Neutral"; 

            if (PlayerOpinion <= 25)
                return "Like";

            if (PlayerOpinion <= 50)
                return "Friendly";

            if (PlayerOpinion <= 100)
                return "Close Friend";

            if (PlayerOpinion == 100)
                return "Best Friend";

            return "ERROR";
        }

        public bool IsBirthday() {
            return GameLoop.World.Player.IsItThisDay(BirthMonth, BirthDay);
        }


        public void UpdateChitChats() {
            int first = GameLoop.rand.Next(0, ChitChats.Count);
            int second = GameLoop.rand.Next(0, ChitChats.Count);
            int third = GameLoop.rand.Next(0, ChitChats.Count);
            int fourth = GameLoop.rand.Next(0, ChitChats.Count);
            int fifth = GameLoop.rand.Next(0, ChitChats.Count);

            while (second == first)
                second = GameLoop.rand.Next(0, ChitChats.Count);

            while (third == second || third == first)
                third = GameLoop.rand.Next(0, ChitChats.Count);

            while (fourth == third || fourth == second || fourth == first)
                fourth = GameLoop.rand.Next(0, ChitChats.Count);

            while (fifth == fourth || fifth == third || fifth == second || fifth == first)
                fifth = GameLoop.rand.Next(0, ChitChats.Count);


            GameLoop.UIManager.chitChat1 = ChitChats.ElementAt(first).Key;
            GameLoop.UIManager.chitChat2 = ChitChats.ElementAt(second).Key;
            GameLoop.UIManager.chitChat3 = ChitChats.ElementAt(third).Key;
            GameLoop.UIManager.chitChat4 = ChitChats.ElementAt(fourth).Key;
            GameLoop.UIManager.chitChat5 = ChitChats.ElementAt(fifth).Key;
        }

        public string ReactGift(int ID) {
            string react = "Neutral";
            int relModifier = 0;
            if (HatedGiftIDs.Contains(ID)) {
                relModifier = -10;
                react = "Hated";
            } else if (DislikedGiftIDs.Contains(ID)) {
                relModifier = -5;
                react = "Disliked";
            } else if (LikedGiftIDs.Contains(ID) || ID == -3) { // 1 silver or more = liked gift
                relModifier = 5;
                react = "Liked";
            } else if (LovedGiftIDs.Contains(ID) || ID == -2) { // More than 10 silver = loved gift
                relModifier = 10;
                react = "Loved";
            }

            if (IsBirthday())
                relModifier *= 2;

            if (!ReceivedGiftToday && GameLoop.World.Player.MetNPCs.ContainsKey(Name)) {
                GameLoop.World.Player.MetNPCs[Name] += relModifier;
                ReceivedGiftToday = true;
            }

            return react;
        }
    }
}
