using System;
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
            int overallType = r3d6();
            int worldType = r3d6();

            string category = "Hostile";

            if (overallType <= 7) { category = "Hostile"; }
            else if (overallType > 7 && overallType <= 13) { category = "Barren"; }
            else if (overallType > 13) { category = "Garden"; }


            if (category == "Hostile") {
                if (worldType == 3 || worldType == 4) { return "Standard (Chthonian)"; }
                if (worldType == 5 || worldType == 6) { return "Standard (Greenhouse)"; }
                if (worldType == 7 || worldType == 8 || worldType == 9) { return "Tiny (Sulfur)"; }
                if (worldType == 10 || worldType == 11 || worldType == 12) { return "Standard (Ammonia)";  }
                if (worldType == 13 || worldType == 14) { return "Large (Ammonia)"; }
                if (worldType == 15 || worldType == 16) { return "Large (Greenhouse)"; }
                if (worldType == 17 || worldType == 18) { return "Large (Chthonian)"; }
            }

            if (category == "Barren") {
                if (worldType == 3) { return "Small (Hadean)"; }
                if (worldType == 4) { return "Small (Ice)"; }
                if (worldType == 5 || worldType == 6) { return "Small (Rock)"; }
                if (worldType == 7 || worldType == 8) { return "Tiny (Rock)"; }
                if (worldType == 9 || worldType == 10) { return "Tiny (Ice)"; }
                if (worldType == 11 || worldType == 12) { return "Asteroid Belt"; }
                if (worldType == 13 || worldType == 14) { return "Standard (Ocean)"; }
                if (worldType == 15) { return "Standard (Ice)"; }
                if (worldType == 16) { return "Standard (Hadean)"; }
                if (worldType == 17) { return "Large (Ocean)"; }
                if (worldType == 18) { return "Large (Ice)"; }
            }

            if (category == "Garden") {
                if (worldType < 17) { return "Standard (Garden)"; }
                if (worldType == 17 || worldType == 18) { return "Large (Garden)"; }
            }


            return "error";
        }

        public static string AtmosphericComposition(string worldType) {
            if (worldType == "Asteroid Belt" || worldType == "Tiny (Ice)" || worldType == "Tiny (Rock)" || 
                worldType == "Tiny (Sulfur)" || worldType == "Small (Hadean)" || worldType == "Small (Rock)" ||
                worldType == "Standard (Hadean)" || worldType == "Standard (Chthonian)" || worldType == "Large (Chthonian)") {
                return "Effectively Vacuum";
            }

            if (worldType == "Small (Ice)") {

            }

            return "Error - World Type not found";
        }
    
    
    }
}
