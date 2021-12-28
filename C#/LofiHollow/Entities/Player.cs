using System;
using Newtonsoft.Json;
using SadRogue.Primitives;

namespace LofiHollow.Entities {
    public class Player : Actor {
        [JsonProperty]
        public int Hours = 9;
        [JsonProperty]
        public int Minutes = 0;
        [JsonProperty]
        public int Month = 1;
        [JsonProperty]
        public int Day = 1;
        [JsonProperty]
        public int Year = 1;
        [JsonProperty]
        public bool AM = false;

        public double TimeLastTicked = 0;

        public Player(Color foreground) : base(foreground, '@', true) {
            ActorGlyph = '@';
        }

        public void TickTime() {
            Minutes++;
            if (Minutes >= 60) {
                Hours++;
                Minutes = 0;
                if (Hours == 12) {
                    AM = !AM;
                    if (AM) {
                        Day++;
                        if (Day > 28) {
                            Day = 1;
                            Month++;

                            if (Month > 12) {
                                Month = 1;
                                Year++;
                            }
                        }
                    }
                }
                if (Hours > 12) {
                    Hours = 1;
                }
            }
        }
    }
}
