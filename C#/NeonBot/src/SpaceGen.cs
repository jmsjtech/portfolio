using System;
using System.Collections.Generic;
using NeonBot.obj;

namespace NeonBot {
    public class SpaceGen {

        public static int r3d6() {
            OnePlat.DiceNotation.IDice dice = new OnePlat.DiceNotation.Dice(); 
            return dice.Roll("3d6", new OnePlat.DiceNotation.DieRoller.RandomDieRoller()).Value;
        }


        public static int StarsNum() {
            int result = r3d6();

            if (result <= 10) {
                return 1;
            } else if (result > 10 && result <= 15) {
                return 2;
            } else {
                return 3;
            }
        }

        public static float StarMass(bool isGardenWorldPresent = false) {
            int firstRoll = r3d6();
            int secondRoll = r3d6();

            if (isGardenWorldPresent) {
                OnePlat.DiceNotation.IDice dice = new OnePlat.DiceNotation.Dice();
                int gardenWorld = dice.Roll("1d6", new OnePlat.DiceNotation.DieRoller.RandomDieRoller()).Value;

                if (gardenWorld == 1) { firstRoll = 5; }
                if (gardenWorld == 2) { firstRoll = 6; }
                if (gardenWorld == 3 || gardenWorld == 4) { firstRoll = 7; }
                if (gardenWorld == 5 || gardenWorld == 6) { firstRoll = 8; }
            }


            switch (firstRoll) {
                case 3:
                    if (secondRoll <= 10) { return 2.00f; } else return 1.90f;
                case 4:
                    if (secondRoll <= 8) { return 1.80f; } else if (secondRoll <= 11) { return 1.70f; } else return 1.60f;
                case 5:
                    if (secondRoll <= 7) { return 1.50f; } else if (secondRoll <= 10) { return 1.45f; } else if (secondRoll <= 12) { return 1.40f; } else return 1.35f;
                case 6:
                    if (secondRoll <= 7) { return 1.30f; } else if (secondRoll <= 9) { return 1.25f; } else if (secondRoll == 10) { return 1.20f; } else if (secondRoll <= 12) { return 1.15f; } else return 1.10f;
                case 7:
                    if (secondRoll <= 7) { return 1.05f; } else if (secondRoll <= 9) { return 1.00f; } else if (secondRoll == 10) { return 0.95f; } else if (secondRoll <= 12) { return 0.90f; } else return 0.85f;
                case 8:
                    if (secondRoll <= 7) { return 0.80f; } else if (secondRoll <= 9) { return 0.75f; } else if (secondRoll == 10) { return 0.70f; } else if (secondRoll <= 12) { return 0.65f; } else return 0.60f;
                case 9:
                    if (secondRoll <= 8) { return 0.55f; } else if (secondRoll <= 11) { return 0.50f; } else return 0.45f;
                case 10:
                    if (secondRoll <= 8) { return 0.40f; } else if (secondRoll <= 11) { return 0.35f; } else return 0.30f;
                case 11:
                    return 0.25f;
                case 12:
                    return 0.20f;
                case 13:
                    return 0.15f;
                default:
                    return 0.10f;
            }
        }

        public static string WorldType() {
            string[] overallTypes = {
                "Hostile", "Hostile", "Hostile", "Hostile", "Hostile",
                "Barren", "Barren", "Barren", "Barren", "Barren", "Barren",
                "Garden", "Garden", "Garden", "Garden", "Garden"
            };

            string[] hostileTypes = {
                "Standard (Cthonian)", "Standard (Cthonian)", "Standard (Greenhouse)", "Standard (Greenhouse)",
                "Tiny (Sulfur)", "Tiny (Sulfur)", "Tiny (Sulfur)", "Standard (Ammonia)", "Standard (Ammonia)", "Standard (Ammonia)",
                "Large (Ammonia)", "Large (Ammonia)", "Large (Ammonia)", "Large (Greenhouse)", "Large (Greenhouse)", "Large (Cthonian)", "Large (Cthonian)"
            };

            string[] barrenTypes = {
                "Small (Hadean)", "Small (Ice)", "Small (Rock)", "Small (Rock)", "Tiny (Rock)", "Tiny (Rock)",
                "Tiny (Ice)", "Tiny (Ice)", "Asteroid Belt", "Asteroid Belt", "Standard (Ocean)", "Standard (Ocean)",
                "Standard (Ice)", "Standard (Hadean)", "Large (Ocean)", "Large (Ice)"
            };

            string[] gardenTypes = {
                "Standard (Garden)", "Standard (Garden)", "Standard (Garden)", "Standard (Garden)", "Standard (Garden)", "Standard (Garden)",
                "Standard (Garden)", "Standard (Garden)", "Standard (Garden)", "Standard (Garden)", "Standard (Garden)", "Standard (Garden)",
                "Standard (Garden)", "Standard (Garden)", "Large (Garden)", "Large (Garden)"
            };

            string overallType = overallTypes[r3d6()];

            if (overallType == "Hostile") { return hostileTypes[r3d6()]; }
            if (overallType == "Barren") { return barrenTypes[r3d6()]; }
            if (overallType == "Garden") { return gardenTypes[r3d6()]; }





            return "error";
        }

        public static string AtmosphericComposition(string worldType) {
            if (new List<string> { "Asteroid Belt", "Tiny (Ice)", "Tiny (Rock)", "Tiny (Sulfur)", "Small (Hadean)", "Small (Rock)", 
                "Standard (Hadean)", "Standard (Chthonian)", "Large (Chthonian)" }.Contains(worldType)) {
                return "Effectively Vacuum";
            }

            if (worldType == "Small (Ice)") {
                
            }

            return "Error - World Type (" + worldType + ") not found";
        }
    
    
    }
}
