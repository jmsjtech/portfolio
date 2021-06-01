﻿using DefaultEcs;
using Microsoft.Xna.Framework;

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

        public void SimpleSet(int glyph, Color? fg = null, Color? bg = null) {
            sce.Animation.CurrentFrame[0].Glyph = glyph;
            if (fg != null) {
                sce.Animation.CurrentFrame[0].Foreground = fg.Value;
            }

            if (bg != null) {
                sce.Animation.CurrentFrame[0].Background = bg.Value;
            }
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
    }

    public struct Name { public string name; }

    public struct Player { }

    public struct Monster { }

    public struct Tile { }

    public struct InBackpack {
        public Entity owner;
    }

    public struct Item {
        public int condition;
        public int weight;
        public int glyph;
        public Color fg;
        public Color bg;

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

    public struct BlocksMovement { }
    public struct BlocksVisibility { }
}