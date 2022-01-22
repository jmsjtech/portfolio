using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SadConsole;
using SadRogue.Primitives;
using LofiHollow.Managers;

namespace LofiHollow.Entities {
    [JsonObject(MemberSerialization.OptIn)]
    public class Player : Actor {
        [JsonProperty]
        public TimeManager Clock = new();

        [JsonProperty]
        public Dictionary<string, int> MetNPCs = new();

        [JsonProperty]
        public bool OwnsFarm = false;

        public List<Point3D> VisitedMaps = new();

        public double TimeLastTicked = 0;
        public int MapsClearedToday = 0;
        public Stack<ColoredString> killList = new(52);

        public string MineLocation = "None";
        public int MineDepth = 0;
        public bool MineVisible = false;
        public Point MineEnteredAt = new Point(0, 0);

        public Player(Color foreground) : base(foreground, '@', true) {
            ActorGlyph = '@';
        }
         

        public void PlayerDied() {
            if (MapPos == GameLoop.World.Player.MapPos && this != GameLoop.World.Player) {
                GameLoop.UIManager.AddMsg(new ColoredString(Name + " died!", Color.Red, Color.Black));
            }

            for (int i = 0; i < Inventory.Length; i++) {
                if (Inventory[i].ItemID != 0) {
                    CommandManager.DropItem(this, i);
                }
            }

            for (int i = 0; i < Equipment.Length; i++) {
                if (Equipment[i].ItemID != 0) {
                    CommandManager.UnequipItem(this, i); 
                    CommandManager.DropItem(this, 0);
                }
            }


            MoveTo(new Point(35, 6), new Point3D(0, 0, 0));
            CurrentHP = MaxHP;

            if (this == GameLoop.World.Player) {
                GameLoop.UIManager.AddMsg(new ColoredString("Oh no, you died!", Color.Red, Color.Black));
                GameLoop.UIManager.AddMsg(new ColoredString("A warm yellow light fills your vision.", Color.Yellow, Color.Black));
                GameLoop.UIManager.AddMsg(new ColoredString("As it fades, you find yourself in the Cemetary.", Color.Yellow, Color.Black));
            }

            if (MapPos == GameLoop.World.Player.MapPos && this != GameLoop.World.Player) {
                GameLoop.UIManager.AddMsg(new ColoredString(Name + " appears in a flash of yellow light!", Color.Yellow, Color.Black));
            }
        }


        public int GetToolTier(int Category) {
            if (Inventory[GameLoop.UIManager.Sidebar.hotbarSelect].ItemCategory == Category) {
                return Inventory[GameLoop.UIManager.Sidebar.hotbarSelect].ItemTier;
            }

            return 0;
        }
        
    }
}
