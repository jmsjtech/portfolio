﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using SadConsole;
using SadRogue.Primitives; 

namespace LofiHollow.Entities {
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class Actor : Entity {
        [JsonProperty]
        public int CurrentHP;
        [JsonProperty]
        public int MaxHP;

        [JsonProperty]
        public int STR = 10;
        [JsonProperty]
        public int DEX = 10;
        [JsonProperty]
        public int CON = 10;
        [JsonProperty]
        public int INT = 10;
        [JsonProperty]
        public int WIS = 10;
        [JsonProperty]
        public int CHA = 10;

        [JsonProperty]
        public int SizeMod = 0;
        [JsonProperty]
        public int NaturalArmor = 0;
        [JsonProperty]
        public int DeflectionMod = 0;
        [JsonProperty]
        public int LandSpeed = 0;
        [JsonProperty]
        public int BaseFort = 0;
        [JsonProperty]
        public int FortMod = 0;
        [JsonProperty]
        public int BaseReflex = 0;
        [JsonProperty]
        public int ReflexMod = 0;
        [JsonProperty]
        public int BaseWill = 0;
        [JsonProperty]
        public int WillMod = 0;

        [JsonProperty]
        public int BaseAttackBonus = 0;
        [JsonProperty]
        public int InitiativeMod = 0;

        [JsonProperty]
        public int CopperCoins = 0;
        [JsonProperty]
        public int SilverCoins = 0;
        [JsonProperty]
        public int GoldCoins = 0;
        [JsonProperty]
        public int PlatinumCoins = 0;

        [JsonProperty]
        public int Level = 1;
        [JsonProperty]
        public int Experience = 0;
        [JsonProperty]
        public string ExpTrack = "Medium";
        [JsonProperty]
        public Race Race = new Race();
        [JsonProperty]
        public List<string> KnownLanguages = new List<string>();


        // Stuff for monsters
        [JsonProperty]
        public int ExpGranted = 0;
        [JsonProperty]
        public int CR = 1; // Negative number means fractional CR
        [JsonProperty]
        public string HitDice = "1d1";
        [JsonProperty]
        public string UnarmedDice = "1d3";

        public double TimeLastActed = 0;

        [JsonProperty]
        public Item[] Inventory;
        [JsonProperty]
        public Item[] Equipment;

        [JsonProperty]
        public List<ClassDef> ClassLevels = new List<ClassDef>();
        [JsonProperty]
        public List<ClassFeature> ClassFeatures = new List<ClassFeature>();
        [JsonProperty]
        public List<string> WeaponProficiencies = new List<string>();



        public List<int> KnownMoves = new List<int>();

        [JsonProperty]
        public List<ItemDrop> DropTable = new List<ItemDrop>();

        [JsonProperty]
        public Point3D MapPos = new Point3D(0, 0, 0);

        [JsonProperty]
        public Dictionary<string, Skill> Skills = new Dictionary<string, Skill>();


        [JsonProperty]
        public int ForegroundR = 0;
        [JsonProperty]
        public int ForegroundG = 0;
        [JsonProperty]
        public int ForegroundB = 0;
        [JsonProperty]
        public int ActorGlyph = 0;

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context) {
            Appearance.Foreground = new Color(ForegroundR, ForegroundG, ForegroundB);
            Appearance.Glyph = ActorGlyph;
        }

        protected Actor(Color foreground, int glyph, bool initInventory = false) : base(foreground, Color.Transparent, glyph) {
            Appearance.Foreground = foreground; 
            Appearance.Glyph = glyph;

            ForegroundR = foreground.R;
            ForegroundG = foreground.G;
            ForegroundB = foreground.B;

            KnownMoves.Add(0);

            if (initInventory) {
                Inventory = new Item[9];
                Equipment = new Item[16];

                for (int i = 0; i < Inventory.Length; i++) {
                    Inventory[i] = new Item(0);
                }

                for (int i = 0; i < Equipment.Length; i++) {
                    Equipment[i] = new Item(0);
                }
            }
        }


        public int GetMod(string attribute) {
            if (attribute == "STR") { return (int) Math.Floor((double) STR / 2) - 5; }
            if (attribute == "DEX") { return (int) Math.Floor((double) DEX / 2) - 5; }
            if (attribute == "CON") { return (int) Math.Floor((double) CON / 2) - 5; }
            if (attribute == "INT") { return (int) Math.Floor((double) INT / 2) - 5; }
            if (attribute == "WIS") { return (int) Math.Floor((double) WIS / 2) - 5; }
            if (attribute == "CHA") { return (int) Math.Floor((double) CHA / 2) - 5; }

            return 0;
        }

        public void SetAttribs(int str, int dex, int con, int intelligence, int wis, int cha) {
            STR = str;
            DEX = dex;
            CON = con;
            INT = intelligence;
            WIS = wis;
            CHA = cha;
        }

        public void SetSaves(int fort, int reflex, int will) {
            BaseFort = fort;
            BaseReflex = reflex;
            BaseWill = will;
        }

        public void UpdateHP(int miscMod) {
            MaxHP = GoRogue.DiceNotation.Dice.Roll(HitDice);

            if (MaxHP < 1)
                MaxHP = 1;

            CurrentHP = MaxHP;
        }

        public int GetAC(int miscMod) {
            // TODO: Add armor bonus
            return 10 + GetMod("DEX") + (-1 * SizeMod) + NaturalArmor + DeflectionMod + miscMod;
        }

        public int FortSave(int miscMod) { return BaseFort + GetMod("CON") + FortMod + miscMod; }
        public int ReflexSave(int miscMod) { return BaseReflex + GetMod("DEX") + ReflexMod + miscMod; }
        public int WillSave(int miscMod) { return BaseWill + GetMod("WIS") + WillMod + miscMod; }

        public void UpdateSaves() {
            BaseFort = 0;
            BaseReflex = 0;
            BaseWill = 0;

            foreach (ClassDef classLevels in ClassLevels) {
                BaseFort += classLevels.GetSave(classLevels.FortSaveProg);
                BaseReflex += classLevels.GetSave(classLevels.RefSaveProg);
                BaseWill += classLevels.GetSave(classLevels.WillSaveProg);
            }
        }


        public int GetCMB(int miscMod) { return BaseAttackBonus + GetMod("STR") + SizeMod + miscMod; } // For Melee Attacks
        public int GetCMD(int miscMod) { return BaseAttackBonus + GetMod("STR") + GetMod("DEX") + SizeMod + 10 + miscMod; } // For Ranged Attacks

        public int RollInitiative() {
            int miscMod = 0;



            return GoRogue.DiceNotation.Dice.Roll("1d20") + InitiativeMod + miscMod + GetMod("DEX"); 
        }

        public int RollAttack(bool justBonus = false) {
            int miscMod = 0;

            if (Equipment != null && Equipment[0].Weapon != null) {
                if (!WeaponProficiencies.Contains(Equipment[0].Weapon.WeaponClass) && Equipment[0].Weapon.WeaponClass != "Simple") {
                    miscMod -= 4;
                }
            }

            if (justBonus)
                return GetMod("STR") + BaseAttackBonus + miscMod;

            return GoRogue.DiceNotation.Dice.Roll("1d20") + GetMod("STR") + BaseAttackBonus + miscMod;
        }

        public int GetDamageBonus(bool melee) {
            int miscMod = 0;

            return GetMod("STR") + miscMod;
        }



        public int ExpToLevel() {
            if (ExpTrack == "Slow") {
                switch(Level) {
                    case 1: return 3000;
                    case 2: return 7500;
                    case 3: return 14000;
                    case 4: return 23000;
                    case 5: return 35000;
                    case 6: return 53000;
                    case 7: return 77000;
                    case 8: return 115000;
                    case 9: return 160000;
                    case 10: return 235000;
                    case 11: return 330000;
                    case 12: return 475000;
                    case 13: return 665000;
                    case 14: return 955000;
                    case 15: return 1350000;
                    case 16: return 1900000;
                    case 17: return 2700000;
                    case 18: return 3850000;
                    case 19: return 5350000;
                }
            }

            if (ExpTrack == "Medium") {
                switch (Level) {
                    case 1: return 2000;
                    case 2: return 5000;
                    case 3: return 9000;
                    case 4: return 15000;
                    case 5: return 23000;
                    case 6: return 35000;
                    case 7: return 51000;
                    case 8: return 75000;
                    case 9: return 105000;
                    case 10: return 155000;
                    case 11: return 220000;
                    case 12: return 315000;
                    case 13: return 445000;
                    case 14: return 635000;
                    case 15: return 890000;
                    case 16: return 1300000;
                    case 17: return 1800000;
                    case 18: return 2550000;
                    case 19: return 3600000;
                }
            }

            if (ExpTrack == "Fast") {
                switch (Level) {
                    case 1: return 1300;
                    case 2: return 3300;
                    case 3: return 6000;
                    case 4: return 10000;
                    case 5: return 15000;
                    case 6: return 23000;
                    case 7: return 34000;
                    case 8: return 50000;
                    case 9: return 71000;
                    case 10: return 105000;
                    case 11: return 145000;
                    case 12: return 210000;
                    case 13: return 295000;
                    case 14: return 425000;
                    case 15: return 600000;
                    case 16: return 850000;
                    case 17: return 1200000;
                    case 18: return 1700000;
                    case 19: return 2400000;
                }
            }


            return Int32.MaxValue;
        }


        public void SetExpGranted() {
            switch (CR) {
                case -8: ExpGranted = 50; break;
                case -6: ExpGranted = 65; break;
                case -4: ExpGranted = 100; break;
                case -3: ExpGranted = 135; break;
                case -2: ExpGranted = 200; break;
                case 1: ExpGranted = 400; break;
                case 2: ExpGranted = 600; break;
                case 3: ExpGranted = 800; break;
                case 4: ExpGranted = 1200; break;
                case 5: ExpGranted = 1600; break;
                case 6: ExpGranted = 2400; break;
                case 7: ExpGranted = 3200; break;
                case 8: ExpGranted = 4800; break;
                case 9: ExpGranted = 6400; break;
                case 10: ExpGranted = 9600; break;
                case 11: ExpGranted = 12800; break;
                case 12: ExpGranted = 19200; break;
                case 13: ExpGranted = 25600; break;
                case 14: ExpGranted = 38400; break;
                case 15: ExpGranted = 51200; break;
                case 16: ExpGranted = 76800; break;
                case 17: ExpGranted = 102400; break;
                case 18: ExpGranted = 153600; break;
                case 19: ExpGranted = 204800; break;
                case 20: ExpGranted = 307200; break;
                case 21: ExpGranted = 409600; break;
                case 22: ExpGranted = 614400; break;
                case 23: ExpGranted = 819200; break;
                case 24: ExpGranted = 1228800; break;
                case 25: ExpGranted = 1638400; break;
                case 26: ExpGranted = 2457600; break;
                case 27: ExpGranted = 3276800; break;
                case 28: ExpGranted = 4915200; break;
                case 29: ExpGranted = 6553600; break;
                case 30: ExpGranted = 9830400; break;
            }
        }


        public bool HasInventorySlotOpen(int stackID = -1) {
            for (int i = 0; i < Inventory.Length; i++) {
                if (Inventory[i].ItemID == 0 || (Inventory[i].ItemID == stackID && stackID != -1)) {
                    return true;
                }
            }
            return false;
        }

        public bool MoveBy(Point positionChange) {
            if (TimeLastActed + (120 - (LandSpeed)) > SadConsole.GameHost.Instance.GameRunningTotalTime.TotalMilliseconds) {
                return false;
            }

            TimeLastActed = SadConsole.GameHost.Instance.GameRunningTotalTime.TotalMilliseconds;

            if (GameLoop.World.maps.TryGetValue(MapPos, out Map map)) {
                Point newPosition = Position + positionChange;
                if (newPosition.Y < 0 && GameLoop.World.maps.ContainsKey(MapPos - new Point3D(0, -1, 0)) && GameLoop.World.maps[MapPos - new Point3D(0, -1, 0)].MinimapTile.name == "Desert") {
                    GameLoop.UIManager.MessageLog.Add("There's dangerous sandstorms that way, best not go there for now.");
                    return false;
                }

                if (map.GetTile(newPosition).SkillableTile != null) {
                    SkillableTile tile = map.GetTile(newPosition).SkillableTile;
                    if (map.GetTile(newPosition).Name == tile.HarvestableName) {
                        if (Skills.ContainsKey(tile.RequiredSkill)) {
                            if (Skills[tile.RequiredSkill].Level >= tile.RequiredLevel) {
                                if (Equipment[0].ItemCategory == tile.HarvestTool || Inventory[GameLoop.UIManager.hotbarSelect].ItemCategory == tile.HarvestTool) {
                                    if (HasInventorySlotOpen()) {
                                        GameLoop.CommandManager.AddItemToInv(this, new Item(tile.ItemGiven));
                                        GameLoop.UIManager.MessageLog.Add(tile.HarvestMessage);

                                        int choppedChance = GameLoop.rand.Next(100) + 1;

                                        if (choppedChance < 33) {
                                            GameLoop.CommandManager.AddItemToInv(this, new Item(tile.DepletedItem));
                                            map.GetTile(newPosition).Name = tile.DepletedName;
                                            GameLoop.UIManager.MessageLog.Add(tile.DepleteMessage);
                                            Skills[tile.RequiredSkill].Experience += tile.ExpOnDeplete;
                                        } else {
                                            Skills[tile.RequiredSkill].Experience += tile.ExpOnHarvest;
                                        }

                                        if (Skills[tile.RequiredSkill].Experience >= Skills[tile.RequiredSkill].ExpToLevel()) {
                                            Skills[tile.RequiredSkill].Experience -= Skills[tile.RequiredSkill].ExpToLevel();
                                            Skills[tile.RequiredSkill].Level++;
                                            GameLoop.UIManager.MessageLog.Add(new ColoredString("You leveled " + tile.RequiredSkill + " to " + Skills[tile.RequiredSkill].Level + "!", Color.Cyan, Color.Black));
                                        }
                                    } else {
                                        GameLoop.UIManager.MessageLog.Add(new ColoredString("Your inventory is full.", Color.Red, Color.Black));
                                    }
                                } else {
                                    GameLoop.UIManager.MessageLog.Add(new ColoredString("You don't have the right tool equipped.", Color.Red, Color.Black));
                                }
                            } else {
                                GameLoop.UIManager.MessageLog.Add(new ColoredString("That requires level " + tile.RequiredLevel + " " + tile.RequiredSkill + ".", Color.Red, Color.Black));
                            }
                        }
                    } else {
                        GameLoop.UIManager.MessageLog.Add(new ColoredString("That's not harvestable yet, come back in a " + tile.RestoreTime + ".", Color.Red, Color.Black));
                    }
                }


                if (map.GetTile(Position).Name == "Up Slope" && positionChange == new Point(0, -1)) {
                    if (!map.IsTileWalkable(Position + positionChange)) {
                        // This is an up slope, move up a map instead of up on the current map
                        if (!GameLoop.World.maps.ContainsKey(MapPos + new Point3D(0, 0, 1))) { GameLoop.World.CreateMap(MapPos + new Point3D(0, 0, 1)); }
                        MapPos += new Point3D(0, 0, 1);
                    } else {
                        if (!GameLoop.World.maps.ContainsKey(MapPos + new Point3D(0, 0, -1))) { GameLoop.World.CreateMap(MapPos + new Point3D(0, 0, -1)); }
                        MapPos += new Point3D(0, 0, -1);
                    }
                    GameLoop.UIManager.LoadMap(GameLoop.World.maps[MapPos], false);
                    return true;
                }

                if (map.GetTile(Position).Name == "Down Slope" && positionChange == new Point(0, 1)) {
                    if (!map.IsTileWalkable(Position + positionChange)) {
                        // This is an up slope, move up a map instead of up on the current map
                        if (!GameLoop.World.maps.ContainsKey(MapPos + new Point3D(0, 0, 1))) { GameLoop.World.CreateMap(MapPos + new Point3D(0, 0, 1)); }
                        MapPos += new Point3D(0, 0, 1);
                    } else {
                        if (!GameLoop.World.maps.ContainsKey(MapPos + new Point3D(0, 0, -1))) { GameLoop.World.CreateMap(MapPos + new Point3D(0, 0, -1)); }
                        MapPos += new Point3D(0, 0, -1);
                    }
                    GameLoop.UIManager.LoadMap(GameLoop.World.maps[MapPos], false);
                    return true;
                }

                if (map.GetTile(Position).Name == "Left Slope" && positionChange == new Point(-1, 0)) {
                    if (!map.IsTileWalkable(Position + positionChange)) {
                        // This is an up slope, move up a map instead of up on the current map
                        if (!GameLoop.World.maps.ContainsKey(MapPos + new Point3D(0, 0, 1))) { GameLoop.World.CreateMap(MapPos + new Point3D(0, 0, 1)); }
                        MapPos += new Point3D(0, 0, 1);
                    } else {
                        if (!GameLoop.World.maps.ContainsKey(MapPos + new Point3D(0, 0, -1))) { GameLoop.World.CreateMap(MapPos + new Point3D(0, 0, -1)); }
                        MapPos += new Point3D(0, 0, -1);
                    }
                    GameLoop.UIManager.LoadMap(GameLoop.World.maps[MapPos], false);
                    return true;
                }

                if (map.GetTile(Position).Name == "Right Slope" && positionChange == new Point(1, 0)) {
                    if (!map.IsTileWalkable(Position + positionChange)) {
                        // This is an up slope, move up a map instead of up on the current map
                        if (!GameLoop.World.maps.ContainsKey(MapPos + new Point3D(0, 0, 1))) { GameLoop.World.CreateMap(MapPos + new Point3D(0, 0, 1)); }
                        MapPos += new Point3D(0, 0, 1);
                    } else {
                        if (!GameLoop.World.maps.ContainsKey(MapPos + new Point3D(0, 0, -1))) { GameLoop.World.CreateMap(MapPos + new Point3D(0, 0, -1)); }
                        MapPos += new Point3D(0, 0, -1);
                    }
                    GameLoop.UIManager.LoadMap(GameLoop.World.maps[MapPos], false);
                    return true;
                }

                if (map.IsTileWalkable(Position +  positionChange)) {
                    // if there's a monster here,
                    // do a bump attack
                    Monster monster = map.GetEntityAt<Monster>(Position + positionChange);
                    if (monster != null) {
                       // GameLoop.CommandManager.Attack(this, monster);
                        return true;
                    }

                    TileBase tile = map.GetTile(Position + positionChange);
                    if (tile.Name == "Sign") {
                        GameLoop.UIManager.SignText(Position + positionChange, MapPos);
                        return true;
                    }

                    Position += positionChange;


                    if (ID == GameLoop.World.Player.ID) {
                        if (map.Tiles[Position.ToIndex(GameLoop.MapWidth)].SpawnsMonsters || map.AmbientMonsters) {
                            if (GameLoop.rand.Next(20) == 0) {
                                int monsterID = GameLoop.BattleManager.GetEncounter(map.MinimapTile.name, map.Tiles[Position.ToIndex(GameLoop.MapWidth)].Name);
                                if (monsterID != -1) {
                                    if (GameLoop.World.monsterLibrary.ContainsKey(monsterID)) {
                                        Monster temp = GameLoop.World.monsterLibrary[monsterID];
                                        Monster mon = new Monster(temp.MonsterID, temp.Appearance.Foreground, temp.Appearance.Glyph);
                                        
                                        for (int i = 0; i < temp.DropTable.Count; i++) {
                                            mon.DropTable.Add(temp.DropTable[i]);
                                        }

                                        for (int i = 0; i < temp.KnownMoves.Count; i++) {
                                            mon.KnownMoves.Add(temp.KnownMoves[i]);
                                        }

                                        int NormalizedMapPos = Math.Abs(MapPos.X) + Math.Abs(MapPos.Y) + Math.Abs(MapPos.Z);

                                        GameLoop.BattleManager.StartBattle(mon, NormalizedMapPos - 2);
                                    } else {
                                        GameLoop.UIManager.MessageLog.Add("Monster ID wasn't in library.");
                                    }
                                } else {
                                    GameLoop.UIManager.MessageLog.Add("Failed to get an encounter for (" + map.MinimapTile.name + ") [" +  map.Tiles[Position.ToIndex(GameLoop.MapWidth)].Name + "]");
                                }
                                
                            }
                        }
                    }


                    return true;
                }

                if (map.ToggleDoor(Position + positionChange, true)) { 
                    return true;
                }

                Point newPos = Position + positionChange;
                bool movedMaps = false;
                
                if (newPos.X < 0) {
                    if (!GameLoop.World.maps.ContainsKey(MapPos + new Point3D(-1, 0, 0))) { GameLoop.World.CreateMap(MapPos + new Point3D(-1, 0, 0)); }

                    MapPos += new Point3D(-1, 0, 0);
                    Position = new Point(GameLoop.MapWidth-1, newPos.Y);
                    movedMaps = true;
                }
                
                if (newPos.X >= GameLoop.MapWidth) {
                    if (!GameLoop.World.maps.ContainsKey(MapPos + new Point3D(1, 0, 0))) { GameLoop.World.CreateMap(MapPos + new Point3D(1, 0, 0)); }

                    MapPos += new Point3D(1, 0, 0);
                    Position = new Point(0, newPos.Y);
                    movedMaps = true;
                } 
                if (newPos.Y < 0) {
                    if (!GameLoop.World.maps.ContainsKey(MapPos + new Point3D(0, -1, 0))) { GameLoop.World.CreateMap(MapPos + new Point3D(0, -1, 0)); }

                    MapPos += new Point3D(0, -1, 0);
                    Position = new Point(newPos.X, GameLoop.MapHeight - 1);
                    movedMaps = true;
                } 
                
                if (newPos.Y >= GameLoop.MapHeight) {
                    if (!GameLoop.World.maps.ContainsKey(MapPos + new Point3D(0, 1, 0))) { GameLoop.World.CreateMap(MapPos + new Point3D(0, 1, 0)); }

                    MapPos += new Point3D(0, 1, 0);
                    Position = new Point(newPos.X, 0);
                    movedMaps = true;
                } 

                if (movedMaps && ID == GameLoop.World.Player.ID) {
                    GameLoop.UIManager.LoadMap(GameLoop.World.maps[MapPos], false);
                    return true;
                }
            }
            return false;
        }

        public bool MoveTo(Point newPosition, Point3D mapLoc) {
            bool movedMaps = false;
            Position = newPosition;

            if (MapPos != mapLoc) { movedMaps = true; }

            if (!GameLoop.World.maps.ContainsKey(mapLoc)) { GameLoop.World.CreateMap(mapLoc); }
            MapPos = mapLoc;

            if (movedMaps && ID == GameLoop.World.Player.ID) {
                GameLoop.UIManager.LoadMap(GameLoop.World.maps[MapPos], false);
            }

            return true;
        } 
    }
}
