using MonoGame.Extended.Entities;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Components;
using System.Collections.Generic;

namespace Oasis {
    public class Render {
        public SadConsole.Entities.Entity sce;
    }

    public class Name { public string name; }
    public class ID { public int id; }


    public class Player { }
    public class Monster { }

    public class Door {
        public bool is_open;
        public bool is_locked;
    }

    public class InBackpack {
        public int ownerID;
    }

    public class Item {
        public int condition;
        public int weight;
        public int glyph;
        public Color fg;
        public Color bg;
        public float value;
    }

    public class Stats {
        public int Health;
        public int MaxHealth;
        public int Attack;
        public int Defense;
        public int Gold;
        public string Damage;
    }

    public class LastActed {
        public long last_action;
        public long speed_in_ms;
    }

    public class BlocksMovement { }
    public class BlocksVisibility { }


    public class Viewshed {
        public GoRogue.FOV view;
        public GoRogue.MapViews.IMapView<bool> old_bool;
        public int radius;

    }

    public class Tile {
        public Cell cell;
        public Point pos;
    }
}