using DefaultEcs;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Components;
using System.Collections.Generic;

namespace Oasis {
    public struct Render {
        public SadConsole.Entities.Entity sce;

        public bool Move(int x_move, int y_move) {
            if (GameLoop.World.CurrentMap.IsTileWalkable(sce.Position + new Point(x_move, y_move))) {
                sce.Position += new Point(x_move, y_move);
                return true;
            } else return false;
        }

        public bool Move(Point loc) {
            if (GameLoop.World.CurrentMap.IsTileWalkable(sce.Position + loc)) {
                sce.Position += loc;
                return true;
            } else return false;
        }

        public bool MoveTo(Point loc) {
            if (GameLoop.World.CurrentMap.IsTileWalkable(loc)) {
                sce.Position = loc;
                return true;
            } else return false;
        }

        public void SimpleSet(int glyph, Color? fg = null, Color? bg = null) {
            sce.Animation.CurrentFrame[0].Glyph = glyph;
            if (fg != null) {
                sce.Animation.CurrentFrame[0].Foreground = fg.Value;
            }

            if (bg != null) {
                sce.Animation.CurrentFrame[0].Background = bg.Value;
            }


            sce.Animation.IsDirty = true;
        }

        public void SetPosition(int x, int y) {
            sce.Position = new Point(x, y);
        }

        public void SetPosition(Point loc) {
            sce.Position = loc;
        }

        public Point GetPosition() {
            return sce.Position;
        }

        public void Init(Point loc, int glyph, Color? fg = null, Color? bg = null) {
            SetPosition(loc);
            SimpleSet(glyph, fg, bg);
            sce.Components.Add(new EntityViewSyncComponent());
        }
    }

    public struct Name { public string name; }

    public struct Player { }

    public struct Monster { }

   
    public struct Door {
        public bool is_open;
        public bool is_locked;

        public void ToggleOpen(Entity parent) {
            if (!is_open) {
                if (!is_locked) {
                    is_open = true;
                    if (parent.Has<BlocksVisibility>()) { parent.Remove<BlocksVisibility>(); }
                    if (parent.Has<BlocksMovement>()) { parent.Remove<BlocksMovement>(); }
                    if (parent.Has<Render>()) { 
                        parent.Get<Render>().SimpleSet('-');
                    }
                }
            } else {
                is_open = false;
                parent.Set(new BlocksVisibility { });
                parent.Set(new BlocksMovement { });
                if (parent.Has<Render>()) { parent.Get<Render>().SimpleSet('+'); }
            }
        }
    }

    public struct InBackpack {
        public Entity owner;
    }

    public struct Item {
        public int condition;
        public int weight;
        public int glyph;
        public Color fg;
        public Color bg;
        public float value;

        public void SetCondition (Entity parent, int mod) {
            condition += mod;

            if (condition <= 0) {
                parent.Dispose();
            } 
        }
    }




    public struct Stats {
        public int Health;
        public int MaxHealth;
        public int Attack;
        public int Defense;
        public int Gold;
        public string Damage;
    }

    public struct LastActed {
        public long last_action;
        public long speed_in_ms;
    }

    public struct BlocksMovement { }
    public struct BlocksVisibility { }


    public struct Viewshed {
        public GoRogue.FOV view;
        public int radius;
    }


    public struct AI {
        public int ApparentStrength; // This is how strong this monster *looks* to other entities.
        public int SelfStrength; // This is how strong the monster *thinks* it is.
        public float Greed; // This is their likelihood to want to pick up valuable objects. 
        public float Bravery; // This is how much stronger the monster needs to be compared to an enemies' apparent strength before it'll attack. Positive = attacks stronger monsters, Negative = attacks weaker monsters.

        public List<AI_GOAL> Goals; // List of what they want to do and how much they want to do it
        public GoRogue.Pathing.Path CurrentPath;

        public void SortGoals () {
            Goals.Sort((p, q) => p.Desire.CompareTo(q.Desire));
            Goals.Reverse();
        }

        public void ClearGoals() {
            Goals.Clear();
        }

        public void NewGoal(string name, float base_desire, float adds, float mult, Point pos, Point goal_loc, Entity? target = null) {
            // base_desire is unidentified value for items, or (perceived self-strength - perceived enemy strength) for NPCs
            // adds: Any general modifiers. Greed for items, Bravery for monsters
            // mult: Multiplier for the goal desire.

            if (target.HasValue) {
                foreach (AI_GOAL existing in Goals) {
                    if (existing.Target.HasValue && existing.Target.Value == target.Value) {
                        RecalculateDesire(existing, pos, adds, mult, existing.Target.Value.Get<Render>().GetPosition());
                        return;
                    }
                }
            }

            double distance = GoRogue.Distance.EUCLIDEAN.Calculate(pos.X, pos.Y, goal_loc.X, goal_loc.Y); 
            double dist_mod = 20 - distance; 

            if (distance == 0) {
                dist_mod = 50;
            }

            double desire = ((base_desire + adds) * mult) + dist_mod;

            AI_GOAL goal = new AI_GOAL(name, goal_loc, desire, base_desire, target);

            Goals.Add(goal);
        }

        public void RecalculateDesire(AI_GOAL goal, Point pos, float adds, float mult, Point new_loc) {
            if (goal.Location != new_loc) {
                goal.Location = new_loc;
            }

            double distance = GoRogue.Distance.EUCLIDEAN.Calculate(pos.X, pos.Y, goal.Location.X, goal.Location.Y);
            double dist_mod = 20 - distance;
            double desire = ((goal.BaseDesire + adds) * mult) + dist_mod;

            goal.Desire = desire;
           
        }
    }

    public struct Faction {
        public string Name;
        public Dictionary<string, int> Relationships;
        
        public void ChangeRelationship(string target, int change) {
            if (Relationships.TryGetValue(target, out int rel)) {
                rel += change;
            }
        }

        public void AddRelationship(string target, int initial) {
            Relationships.Add(target, initial);
        }

        public string GetReaction(string target) {
            if (Relationships.TryGetValue(target, out int relationship)) {
                if (relationship < -50) { return "Hostile"; }
                if (relationship < 0) { return "Unfriendly"; }
                if (relationship < 25) { return "Neutral"; }
                if (relationship < 50) { return "Friendly"; }
                if (relationship >= 50) { return "Ally"; }
            }

            return "Neutral";
        }
    }


    public struct AI_GOAL {
        public string Action; // The action they want to take
        public double Desire; // How strongly they want to do that thing
        public double BaseDesire; // The base desire for the object for easier recalculation later.
        public Point Location; // The location where the goal is
        public Entity? Target; // The target of the goal, if it's an entity.

        public AI_GOAL(string name, Point goal_loc, double desire, double base_desire, Entity? target = null) {
            Action = name;
            Desire = desire;
            BaseDesire = base_desire;
            Location = goal_loc;
            Target = target;
        }
    }
}