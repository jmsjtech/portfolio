using DefaultEcs;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Components;
using System.Collections.Generic;

namespace NeonCity {
    public struct Render {
        public SadConsole.Entities.Entity sce;

        public void Move(int x_move, int y_move) {
            sce.Position += new Point(x_move, y_move);
        }

        public void Move(Point loc) {
            sce.Position += loc;
        }

        public void MoveTo(Point loc) {
            sce.Position = loc;
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

        public void Resize(int width, int height, string tiles, Color foreground, Color background) {
            sce = new SadConsole.Entities.Entity(width, height);

            string temp = tiles;

            if (tiles == "player") {
                temp = "  ";
                temp += (char) 179;
                temp += "  ";

                temp += " ";
                temp += (char) 218;
                temp += (char) 193;
                temp += (char) 191;
                temp += " ";

                temp += (char) 196;
                temp += (char) 180;
                temp += " ";
                temp += (char) 195;
                temp += (char) 196;

                temp += " ";
                temp += (char) 192;
                temp += (char) 194;
                temp += (char) 217;
                temp += " ";

                temp += "  ";
                temp += (char) 179;
                temp += "  ";
            }

            for (int i = 0; i < sce.Animation.CurrentFrame.Cells.Length; i++) {
                sce.Animation.CurrentFrame.Cells[i].Glyph = temp[i];
                sce.Animation.CurrentFrame.Cells[i].Foreground = foreground;
                sce.Animation.CurrentFrame.Cells[i].Background = background;
                sce.Animation.CurrentFrame.SetEffect(i, new SadConsole.Effects.Blink());
            }

            sce.Animation.IsDirty = true;
            sce.Components.Add(new EntityViewSyncComponent());
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

            if (GameLoop.UIManager.MapConsole != null) {
                GameLoop.UIManager.MapConsole.IsDirty = true;
            }
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

    public struct Name {
        public string name;
    }

    public struct Resources {
        public Dictionary<string, int> resources;

        public Resources(string name, int amount) {
            resources = new Dictionary<string, int>();
            resources.Add(name, amount);
        }

        public void Add(string name, int amount) {
            if (resources.ContainsKey(name)) {
                resources[name] += amount;
            } else {
                resources.Add(name, amount);
            }
        }
    }

    public struct Player {
        public int gold;
        public int wood;
        public int stone;



        public Player(int g, int w, int s) {
            gold = g;
            wood = w;
            stone = s;
        }
    }
}
