using DefaultEcs;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Components;
using System.Collections.Generic;

namespace CritterQuest {
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
                    if (parent.Has<Tile>()) { 
                        parent.Get<Tile>().SimpleSet('-');
                    }
                }
            } else {
                is_open = false;
                parent.Set(new BlocksVisibility { });
                parent.Set(new BlocksMovement { });
                if (parent.Has<Tile>()) { parent.Get<Tile>().SimpleSet('+'); }
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
        public GoRogue.MapViews.IMapView<bool> old_bool;
        public int radius;

        public void UpdateFOV(Point center) {
            old_bool = view.BooleanFOV;
            view.Calculate(center, radius);
        }
    }

    public struct Tile {
        public Cell cell;
        public int x;
        public int y;

        public void SimpleSet(int glyph, Color? fg = null, Color? bg = null) {
            cell.Glyph = glyph;
            if (fg != null) {
                cell.Foreground = fg.Value;
            }

            if (bg != null) {
                cell.Background = bg.Value;
            }

            GameLoop.UIManager.MapConsole.IsDirty = true;
        }

        public Point GetPosition() {
            return new Point(x, y);
        }
        
        public void SetPosition(int index, int width) {
            Point pos = index.ToPoint(width);
            x = pos.X;
            y = pos.Y;
        }

        public void SetPosition(Point pos) {
            x = pos.X;
            y = pos.Y;
        }
    }
}