using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SadRogue.Primitives;

namespace LofiHollow.Entities {
    [JsonObject(MemberSerialization.OptIn)]
    public class Monster : Actor {
        [JsonProperty]
        public int MonsterID = -1;

        [JsonProperty]
        public string UniqueID;

        [JsonConstructor]
        public Monster() : base(Color.White, 'e') { 
        }

        public GoRogue.FOV FOV;
        public GoRogue.Pathing.Path CurrentPath;
        public bool UpdatePath = true;
        public int pathPos = 0;
        public int trackingLength = 5;


        public Monster(Color foreground, int glyph, int ID, string name) : base(foreground, glyph) {
            MonsterID = ID;
            Name = name;
        }

        public Monster(int ID) : base(Color.Black, 32) {
            if (GameLoop.World.monsterLibrary != null && GameLoop.World.monsterLibrary.ContainsKey(ID)) {
                Monster temp = GameLoop.World.monsterLibrary[ID];

                Name = temp.Name;


                ForegroundR = temp.ForegroundR;
                ForegroundG = temp.ForegroundG;
                ForegroundB = temp.ForegroundB;
                ActorGlyph = temp.ActorGlyph;

                Appearance.Foreground = new Color(ForegroundR, ForegroundG, ForegroundB);
                Appearance.Glyph = ActorGlyph;

                MonsterID = temp.MonsterID;

                UniqueID = Guid.NewGuid().ToString("N");

                SetAttribs(temp.STR, temp.DEX, temp.CON, temp.INT, temp.WIS, temp.CHA);

                LandSpeed = temp.LandSpeed;
                BaseAttackBonus = temp.BaseAttackBonus;
                SizeMod = temp.SizeMod;
                HitDice = temp.HitDice;
                CR = temp.CR;
                InitiativeMod = temp.InitiativeMod;
                Templates = temp.Templates;
            }
        }



        public void Update() {
            Point oldPos = new Point(Position.X, Position.Y);

            int distanceToNearest = 99;
            Point targetPoint = new Point(-1, -1);
            Actor defender = null;

            if (GameLoop.World.Player.MapPos == MapPos) {
                defender = GameLoop.World.Player;
                targetPoint = new Point(GameLoop.World.Player.Position.X, GameLoop.World.Player.Position.Y);
                distanceToNearest = GameLoop.World.maps[MapPos].MapPath.ShortestPath(new GoRogue.Coord(Position.X, Position.Y), new GoRogue.Coord(targetPoint.X, targetPoint.Y)).Length;
            }

            foreach (KeyValuePair<long, Player> kv in GameLoop.World.otherPlayers) {
                if (kv.Value.MapPos == MapPos) {
                    GoRogue.Coord otherPos = new GoRogue.Coord(kv.Value.Position.X, kv.Value.Position.Y);
                    int newDist = GameLoop.World.maps[MapPos].MapPath.ShortestPath(new GoRogue.Coord(Position.X, Position.Y), otherPos).Length;
                    if (newDist < distanceToNearest) {
                        defender = kv.Value;
                        targetPoint = new Point(kv.Value.Position.X, kv.Value.Position.Y);
                        distanceToNearest = newDist;
                    } 
                }
            }

            if (distanceToNearest < 25 && distanceToNearest > 1) { 
                if (CurrentPath == null) {
                    CurrentPath = GameLoop.World.maps[MapPos].MapPath.ShortestPath(new GoRogue.Coord(Position.X, Position.Y), new GoRogue.Coord(targetPoint.X, targetPoint.Y));
                    pathPos = 0;
                }

                if (CurrentPath != null && targetPoint != new Point(-1, -1)) {
                    if (TimeLastActed + (120) > SadConsole.GameHost.Instance.GameRunningTotalTime.TotalMilliseconds) {
                        return;
                    }
                    TimeLastActed = SadConsole.GameHost.Instance.GameRunningTotalTime.TotalMilliseconds;


                    if (CurrentPath.Length > pathPos) {
                        GoRogue.Coord nextStep = CurrentPath.GetStep(pathPos);

                        if (MoveTo(new Point(nextStep.X, nextStep.Y), MapPos))
                            pathPos++;
                    }

                    Point currEnd = new Point(CurrentPath.End.X, CurrentPath.End.Y);

                    if (targetPoint != currEnd) {
                        CurrentPath = GameLoop.World.maps[MapPos].MapPath.ShortestPath(new GoRogue.Coord(Position.X, Position.Y), new GoRogue.Coord(targetPoint.X, targetPoint.Y));
                        pathPos = 0;
                    }
                }

                if (oldPos != Position) {
                    if (GameLoop.NetworkManager != null && GameLoop.NetworkManager.lobbyManager != null && GameLoop.NetworkManager.isHost) {
                        string msg = "moveMonster;" + UniqueID + ";" + Position.X + ";" + Position.Y + ";" + MapPos.X + ";" + MapPos.Y + ";" + MapPos.Z;
                        GameLoop.NetworkManager.BroadcastMsg(msg);
                    }
                }
            } else if (distanceToNearest == 1) { // Already adjacent, make an attack
                if (defender != null) {
                    if (TimeLastActed + (120) > SadConsole.GameHost.Instance.GameRunningTotalTime.TotalMilliseconds) {
                        return;
                    }
                    TimeLastActed = SadConsole.GameHost.Instance.GameRunningTotalTime.TotalMilliseconds;

                    GameLoop.CommandManager.Attack(this, defender, true);
                }
            }

            UpdatePosition();
        } 
    }
}
