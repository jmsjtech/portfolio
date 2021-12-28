using System;
using LofiHollow.Entities;
using GoRogue.DiceNotation;
using SadRogue.Primitives;
using SadConsole;

namespace LofiHollow {
    public class BattleManager {
        public string BattleState = "None";
        public Monster Enemy;
        public float fleePercent;

        public int GetEncounter(string mapName, string tileName) {
            if (mapName == "Field" || mapName == "Road") {
                if (tileName == "Tall Grass") {
                    int[] possible = { 0, 1, 2, 3 };

                    return possible[GameLoop.rand.Next(possible.Length)];
                }
            }

            return -1;
        }

        public void StartBattle(Monster monster, int level) {
            Enemy = monster;

            Enemy.SetExpGranted();
            Enemy.UpdateHP(0);

            if (GameLoop.World.Player.RollInitiative() > Enemy.RollInitiative()) {
                GameLoop.UIManager.PlayerFirst = true;
            } else {
                GameLoop.UIManager.PlayerFirst = false;
            }
            

            GameLoop.UIManager.BattleWindow.Title = monster.Name.ToUpper();
            GameLoop.UIManager.BattleWindow.IsVisible = true;
            GameLoop.UIManager.BattleWindow.IsFocused = true;
            GameLoop.UIManager.BattleLog.IsVisible = true;
            
            GameLoop.UIManager.selectedMenu = "Battle";
            BattleState = "Battle";

            GameLoop.UIManager.BattleLog.Clear();
            GameLoop.UIManager.battleDone = false;

            CalculateFlee(); 
        }

        public void EndBattle(bool fled) {

            if (!fled) {
                GameLoop.UIManager.battleResult = "Victory";
                GameLoop.World.Player.Experience += Enemy.ExpGranted;
                if (GameLoop.World.Player.Experience >= GameLoop.World.Player.ExpToLevel()) {
                    GameLoop.World.Player.Level += 1;
                    GameLoop.UIManager.battleResult = "Level";
                } 
            } else {
                GameLoop.UIManager.battleResult = "Fled";
            }

            GameLoop.UIManager.battleDone = true;
        }

        public void ResolveTurn(string action, bool melee, bool playerFirst) { 
            GameLoop.UIManager.BattleLog.Clear();
            
            if (playerFirst) {
                // Player goes first
                if (action == "Attack") {
                    CalculateDamage(GameLoop.World.Player, Enemy, melee, 0);

                    if (Enemy.CurrentHP <= 0) {
                        GameLoop.UIManager.BattleLog.Print(0, 1, Enemy.Name + " died!");
                        EndBattle(false);
                    }
                }
                if (action == "Flee")
                    if (TryEscape()) {
                        GameLoop.UIManager.BattleLog.Print(0, 0, "Successfully ran away!");
                        EndBattle(true);
                    } else
                        GameLoop.UIManager.BattleLog.Print(0, 0, "Couldn't get away!");

                if (!GameLoop.UIManager.battleDone) {
                    // Then monster
                    if (Enemy.CurrentHP > 0)
                        CalculateDamage(Enemy, GameLoop.World.Player, true, 1);
                }
            } else {
                // Monster goes first
                CalculateDamage(Enemy, GameLoop.World.Player, true, 0);

                // Then player
                if (action == "Attack") {
                    CalculateDamage(GameLoop.World.Player, Enemy, melee, 1);

                    if (Enemy.CurrentHP <= 0) {
                        GameLoop.UIManager.BattleLog.Print(0, 2, Enemy.Name + " died!");
                        EndBattle(false);
                    }
                }
                if (action == "Flee")
                    if (TryEscape())
                        EndBattle(true);
                    else
                        GameLoop.UIManager.BattleLog.Print(0, 1, "Couldn't get away!");
            }

            GameLoop.UIManager.selectedMenu = "TurnWait";
            GameLoop.UIManager.AlreadyUsedItem = false;
        }

        public bool TryEscape() { 
            return GameLoop.rand.Next(100) < fleePercent; 
        }

        public void RollDrops() {
            for (int i = 0; i < GameLoop.BattleManager.Enemy.DropTable.Count; i++) {
                ItemDrop drop = GameLoop.BattleManager.Enemy.DropTable[i];

                int roll = GameLoop.rand.Next(drop.DropChance);

                if (roll == 0) {
                    Item item = new Item(drop.ItemID);

                    if (item.IsStackable) {
                        item.ItemQuantity = GameLoop.rand.Next(drop.DropQuantity) + 1;
                        GameLoop.UIManager.dropTable.Add(item);
                    } else {
                        int qty = GameLoop.rand.Next(drop.DropQuantity) + 1;

                        for (int j = 0; j < qty; j++) {
                            Item itemNonStack = new Item(drop.ItemID);
                            GameLoop.UIManager.dropTable.Add(itemNonStack);
                        }
                    }
                }
            }


            int coreChance = GameLoop.rand.Next(100) + 1;

            if (coreChance < 75) {
                Item core = new Item(5);
                core.SubID = GameLoop.BattleManager.Enemy.CR;
                core.Name = "Monster Core [";

                string CR = Enemy.CR.ToString();

                if (Enemy.CR < 0) {
                    if (Enemy.CR == -2) { CR = ((char)25).ToString(); }
                    if (Enemy.CR == -3) { CR = ((char)26).ToString(); }
                    if (Enemy.CR == -4) { CR = ((char)27).ToString(); }
                    if (Enemy.CR == -6) { CR = ((char)28).ToString(); }
                    if (Enemy.CR == -8) { CR = ((char)29).ToString(); }
                }

                core.Name += CR + "]";

                core.AverageValue = Enemy.ExpGranted / 4;

                core.Description = "The core of a CR " + CR + " monster.";

                GameLoop.UIManager.dropTable.Add(core);
            }
        }

        public void CalculateFlee() {
            float playerSpeed = GameLoop.World.Player.DEX;
            float monsterSpeed = Enemy.DEX;

            fleePercent = ((playerSpeed / monsterSpeed) * 100) / 2;

            if (fleePercent > 100)
                fleePercent = 100;
            if (fleePercent < 0)
                fleePercent = 0;

            fleePercent = (float) Math.Ceiling(fleePercent); 
        }

        public void CalculateDamage(Actor attacker, Actor defender, bool melee, int y) {
            int attackRoll = GoRogue.DiceNotation.Dice.Roll("1d20");

            int attackBonus = attacker.RollAttack(true);
            

            if (attackRoll + attackBonus >= defender.GetAC(0)) {
                string weaponDice = attacker.UnarmedDice;
                int minCrit = 20;
                int critMod = 2;
                if (attacker.Equipment != null && attacker.Equipment[0].Weapon != null) {
                    weaponDice = attacker.Equipment[0].Weapon.DamageDice;
                    minCrit = attacker.Equipment[0].Weapon.MinCrit;
                    critMod = attacker.Equipment[0].Weapon.CritMod;
                }

                string moddedDice = weaponDice;

                if (attacker.GetMod("STR") > 0)
                    moddedDice += "+" + attacker.GetMod("STR");
                else
                    moddedDice += "-" + Math.Abs(attacker.GetMod("STR"));

                int damage = GoRogue.DiceNotation.Dice.Roll(moddedDice);

                if (damage < 0)
                    damage = 0; 
                 

                if (attackRoll >= minCrit) {
                    int confirmCrit = attacker.RollAttack(false);

                    if (confirmCrit >= defender.GetAC(0)) {
                        if (critMod == 2)
                            damage += GoRogue.DiceNotation.Dice.Roll(weaponDice); 
                        else
                            damage += (critMod - 1) * GoRogue.DiceNotation.Dice.Roll(weaponDice);

                        GameLoop.UIManager.BattleLog.Print(0, y, new ColoredString(attacker.Name + " hit for " + damage + " damage. Critical Hit!", Color.Green, Color.Black));
                    } else { 
                        GameLoop.UIManager.BattleLog.Print(0, y, new ColoredString(attacker.Name + " hit for " + damage + " damage.", Color.White, Color.Black));
                    }
                } else { 
                    GameLoop.UIManager.BattleLog.Print(0, y, new ColoredString(attacker.Name + " hit for " + damage + " damage.", Color.White, Color.Black));
                }

                defender.CurrentHP -= damage;
            } else { 
                GameLoop.UIManager.BattleLog.Print(0, y, attacker.Name + " attacked, but missed!");
            } 
        } 
    }
}
