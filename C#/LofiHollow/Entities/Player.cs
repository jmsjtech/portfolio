using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SadConsole;
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
        public bool AM = true;

        [JsonProperty]
        public Dictionary<string, int> MetNPCs = new Dictionary<string, int>();

        public double TimeLastTicked = 0;


        


        public Player(Color foreground) : base(foreground, '@', true) {
            ActorGlyph = '@';
        }


        public string GetSeason() {
            if (Month == 1 || Month == 2) { return "Spring"; } 
            else if (Month == 3 || Month == 4 || Month == 5) { return "Summer"; } 
            else if (Month == 6 || Month == 7) { return "Fall"; } 
            else if (Month == 8 || Month == 9 || Month == 10) { return "Winter"; } 
            else { return "Holiday"; }
        }

        public bool IsItThisDay(int month, int day) { return (Month == month && Day == day); }

        public int GetCurrentTime() {
            int total = Minutes + (Hours * 60);

            if (!AM)
                total += (12 * 60);

            return total;
        }

        public void ApplyLevel(int slot) {
            if (ClassLevels.Count > slot && slot >= 0) {
                ClassLevels[slot].ClassLevels++;

                for (int i = 0; i < ClassLevels[slot].ClassFeatures[ClassLevels[slot].ClassLevels].Count; i++) {
                    ClassFeature temp = ClassLevels[slot].ClassFeatures[ClassLevels[slot].ClassLevels][i];
                    ClassFeatures.Add(temp);
                }

                int rolledHP = GoRogue.DiceNotation.Dice.Roll(ClassLevels[slot].HitDie);

                rolledHP += GetMod("CON");

                CurrentHP += rolledHP;
                MaxHP += rolledHP;

                UpdateSaves();
            }
        }

        public void PlayerDied() {
            Experience = ExpToLevel(Level - 1);
            MoveTo(new Point(35, 6), new Point3D(0, 0, 0));
            CurrentHP = MaxHP;
            GameLoop.UIManager.MessageLog.Add(new ColoredString("A warm yellow light fills your vision.", Color.Yellow, Color.Black));
            GameLoop.UIManager.MessageLog.Add(new ColoredString("As it fades, you find yourself in the Cemetary.", Color.Yellow, Color.Black));
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
                        DailyUpdates();
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


        public void DailyUpdates() {
            foreach(KeyValuePair<int, NPC.NPC> kv in GameLoop.World.npcLibrary) {
                kv.Value.ReceivedGiftToday = false; 
            }
        }
    }
}
