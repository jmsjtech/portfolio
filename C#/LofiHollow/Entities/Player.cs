using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SadConsole;
using SadRogue.Primitives;

namespace LofiHollow.Entities {
    [JsonObject(MemberSerialization.OptIn)]
    public class Player : Actor {
        [JsonProperty]
        public TimeKeeper Clock = new TimeKeeper();

        [JsonProperty]
        public Dictionary<string, int> MetNPCs = new Dictionary<string, int>();

        [JsonProperty]
        public bool OwnsFarm = false;

        public List<Point3D> VisitedMaps = new List<Point3D>();

        public double TimeLastTicked = 0;

        public Player(Color foreground) : base(foreground, '@', true) {
            ActorGlyph = '@';
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
            if (MapPos == GameLoop.World.Player.MapPos && this != GameLoop.World.Player) {
                GameLoop.UIManager.AddMsg(new ColoredString(Name + " died!", Color.Red, Color.Black));
            }

            if (Level > 1)
                Experience = ExpToLevel(Level - 1);
            else
                Experience = 0;
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

        
    }
}
