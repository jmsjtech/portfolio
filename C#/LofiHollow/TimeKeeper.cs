using LofiHollow.Entities.NPC;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LofiHollow {
    [JsonObject(MemberSerialization.OptIn)]
    public class TimeKeeper {
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

        public string MinutesToTime(int minutes) {
            return (minutes / 60) + ":" + (minutes % 60);
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


            if (GameLoop.NetworkManager != null && GameLoop.NetworkManager.lobbyManager != null) {
                string time = JsonConvert.SerializeObject(this, Formatting.Indented);
                string msg = "time;" + time;
                GameLoop.NetworkManager.BroadcastMsg(msg);
            }
        }


        public void DailyUpdates() {
            foreach (KeyValuePair<int, NPC> kv in GameLoop.World.npcLibrary) {
                kv.Value.ReceivedGiftToday = false;
            }

            for (int i = 0; i < GameLoop.World.maps[new Point3D(-1, 0, 0)].Tiles.Length; i++) {
                TileBase tile = GameLoop.World.maps[new Point3D(-1, 0, 0)].Tiles[i];

                if (tile.Plant != null) {
                    tile.Plant.DayUpdate();
                    tile.UpdateAppearance();
                    int x = i % GameLoop.MapWidth;
                    int y = i / GameLoop.MapWidth;

                    if (GameLoop.NetworkManager != null && GameLoop.NetworkManager.lobbyManager != null) {
                        string msg = "updateTile;" + x + ";" + y + ";-1;0;0;" + JsonConvert.SerializeObject(tile, Formatting.Indented);
                        GameLoop.NetworkManager.BroadcastMsg(msg);
                    }
                }
            }

            GameLoop.UIManager.Map.LoadMap(GameLoop.World.Player.MapPos);
        }
    }
}
