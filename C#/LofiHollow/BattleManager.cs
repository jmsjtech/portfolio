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
            if (mapName == "Field") {
                if (tileName == "Tall Grass") {
                    int[] possible = { 0, 1 };

                    return possible[GameLoop.rand.Next(possible.Length)];
                }
            }

            return -1;
        }

        public void StartBattle(Monster monster) {
            Enemy = monster;

            ApplyLevels(Enemy, 1);

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
                if (GameLoop.World.Player.Experience >= GameLoop.World.Player.ExpToNext) {
                    GameLoop.World.Player.Experience -= GameLoop.World.Player.ExpToNext;
                    GameLoop.World.Player.Level += 1;
                    GameLoop.World.Player.RecalculateEXP();
                    GameLoop.UIManager.battleResult = "Level";
                }

            } else {
                GameLoop.UIManager.battleResult = "Fled";
            }

            GameLoop.UIManager.battleDone = true;
        }

        public void ResolveTurn(string action, int usedID) {
            Move move = GameLoop.World.moveLibrary[usedID];
            GameLoop.UIManager.BattleLog.Clear();

            if (move.Cost != 0) {
                if (move.IsPhysical) {
                    if (move.Cost > GameLoop.World.Player.Energy) {  
                        GameLoop.UIManager.selectedMenu = "TurnWait";
                        GameLoop.UIManager.BattleLog.Print(0, 1, "You don't have enough energy for that attack!");
                        return;
                    } else {
                        GameLoop.World.Player.Energy -= move.Cost;
                    }
                } else {
                    if (move.Cost > GameLoop.World.Player.Mana) {
                        GameLoop.UIManager.selectedMenu = "TurnWait";
                        GameLoop.UIManager.BattleLog.Print(0, 1, "You don't have enough mana for that attack!");
                        return;
                    } else {
                        GameLoop.World.Player.Mana -= move.Cost;
                    }
                }
            }



            if (GameLoop.World.Player.Speed >= Enemy.Speed) {
                // Player goes first
                if (action == "Attack") {
                    CalculateDamage(GameLoop.World.Player, Enemy, move, 0);

                    if (Enemy.HitPoints <= 0) {
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
                    if (Enemy.HitPoints > 0)
                        CalculateDamage(Enemy, GameLoop.World.Player, Enemy.PickKnownMove(), 1);
                    else
                        GameLoop.UIManager.BattleLog.Print(0, 1, Enemy.Name + " died!");
                }
            } else {
                // Monster goes first
                CalculateDamage(Enemy, GameLoop.World.Player, GameLoop.World.moveLibrary[0], 0);

                // Then player
                if (action == "Attack") {
                    CalculateDamage(GameLoop.World.Player, Enemy, Enemy.PickKnownMove(), 1);

                    if (Enemy.HitPoints <= 0) {
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
        }

        public bool TryEscape() { 
            return GameLoop.rand.Next(100) < fleePercent; 
        }

        public void CalculateFlee() {
            float playerSpeed = GameLoop.World.Player.Speed;
            float monsterSpeed = Enemy.Speed;

            fleePercent = ((playerSpeed / monsterSpeed) * 100) / 2;

            if (fleePercent > 100)
                fleePercent = 100;
            if (fleePercent < 0)
                fleePercent = 0;

            fleePercent = (float) Math.Ceiling(fleePercent); 
        }

        public void CalculateDamage(Actor attacker, Actor defender, Move move, int y) {
            int level = attacker.Level;
            int attackVsDamage;

            if (move.IsPhysical)
                attackVsDamage = attacker.Attack / defender.Defense;
            else
                attackVsDamage = attacker.MagicAttack / defender.MagicDefense;

            int damage = (((((2 * level) / 5) + 2) * move.Power * attackVsDamage) / 50) + 2;

            if (move.Type == "None") {
                GameLoop.UIManager.BattleLog.Print(0, y, attacker.Name + " used " + move.Name + ". Nothing happens.");
                damage = 0;
            } else { 
                if (GameLoop.rand.Next(100) + 1 < move.Accuracy) {
                    if (attacker.Equipment[0].ItemID != 0) {
                        damage += attacker.Equipment[0].NumericalBonus;
                    }

                    if (defender.Equipment[1].ItemID != 0) {
                        damage -= defender.Equipment[1].NumericalBonus;
                    }

                    if (defender.Equipment[2].ItemID != 0) {
                        damage -= defender.Equipment[2].NumericalBonus;
                    }

                    if (defender.Equipment[3].ItemID != 0) {
                        damage -= defender.Equipment[3].NumericalBonus;
                    }

                    if (defender.Equipment[4].ItemID != 0) {
                        damage -= defender.Equipment[4].NumericalBonus;
                    }

                    if (damage > 0) {
                        if (GameLoop.rand.Next(20) + 1 <= attacker.CritChance) {
                            damage = damage * 2;
                            GameLoop.UIManager.BattleLog.Print(0, y, new ColoredString(attacker.Name + " used " + move.Name + ". Critical hit!", Color.Yellow, Color.Black));
                        } else {
                            GameLoop.UIManager.BattleLog.Print(0, y, attacker.Name + " used " + move.Name + ".");
                        }
                    } else {
                        GameLoop.UIManager.BattleLog.Print(0, y, attacker.Name + " used " + move.Name + " but did no damage!");
                    }
                } else {
                    GameLoop.UIManager.BattleLog.Print(0, y, attacker.Name + " used " + move.Name + " but missed!");
                    damage = 0;
                }
            }


           
            defender.HitPoints -= damage;
        }

        public void ApplyLevels(Actor actor, int levels) {
            actor.Vitality = 0;
            actor.Speed = 0;
            actor.Attack = 0;
            actor.Defense = 0;
            actor.MagicAttack = 0;
            actor.MagicDefense = 0;
            actor.Level = levels;

            for (int i = 0; i < levels; i++) {
                if (actor.BaseVitality != 0)
                    actor.Vitality += GameLoop.rand.Next(actor.BaseVitality) + 1;
                if (actor.BaseSpeed != 0)
                    actor.Speed += GameLoop.rand.Next(actor.BaseSpeed) + 1;
                if (actor.BaseAttack != 0)
                    actor.Attack += GameLoop.rand.Next(actor.BaseAttack) + 1;
                if (actor.BaseDefense != 0)
                    actor.Defense += GameLoop.rand.Next(actor.BaseDefense) + 1;
                if (actor.BaseMagicAttack != 0)
                    actor.MagicAttack += GameLoop.rand.Next(actor.BaseMagicAttack) + 1;
                if (actor.BaseMagicDefense != 0)
                    actor.MagicDefense += GameLoop.rand.Next(actor.BaseMagicDefense) + 1;
            }

            int statsInAverage = 0;

            if (actor.BaseVitality != 0)
                statsInAverage++;
            if (actor.BaseSpeed != 0)
                statsInAverage++;
            if (actor.BaseAttack != 0)
                statsInAverage++;
            if (actor.BaseDefense != 0)
                statsInAverage++;
            if (actor.BaseMagicAttack != 0)
                statsInAverage++;
            if (actor.BaseMagicDefense != 0)
                statsInAverage++;



            actor.MaxStrength = (actor.BaseVitality + actor.BaseSpeed + actor.BaseAttack + actor.BaseDefense + actor.BaseMagicAttack + actor.BaseMagicDefense) * levels;
            actor.MinStrength = levels * statsInAverage;
            actor.AverageStrength = (actor.MaxStrength + actor.MinStrength) / 2;
            actor.ExpGranted = actor.StatTotal() * levels;
            actor.RecalculateHP();
            actor.HitPoints = actor.MaxHP;

            int diff = actor.MaxStrength - actor.MinStrength;
            int powerVsMin = actor.StatTotal() - actor.MinStrength;

            if (powerVsMin > (float) diff * 0.8) {
                actor.Descriptor = "Elite";
            } else if (powerVsMin > (float) diff * 0.6) {
                actor.Descriptor = "Strong";
            } else if (powerVsMin > (float)diff * 0.4) {
                // Average
            } else if (powerVsMin > (float)diff * 0.2) {
                actor.Descriptor = "Weak";
            } else {
                actor.Descriptor = "Fragile";
            }
        }
    }
}
