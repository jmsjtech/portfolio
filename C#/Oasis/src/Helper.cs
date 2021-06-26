using MonoGame.Extended.Entities;
using Microsoft.Xna.Framework;
using SadConsole.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oasis {
    class Helper {
        public static bool Render_Move(Render render, int x_move, int y_move) {
            if (GameLoop.World.CurrentMap.IsTileWalkable(render.sce.Position + new Point(x_move, y_move))) {
                render.sce.Position += new Point(x_move, y_move);
                return true;
            } else return false;
        }

        public static bool Render_Move(Render render, Point loc) {
            if (GameLoop.World.CurrentMap.IsTileWalkable(render.sce.Position + loc)) {
                render.sce.Position += loc;
                return true;
            } else return false;
        }

        public static bool Render_MoveTo(Render render, Point loc) {
            if (GameLoop.World.CurrentMap.IsTileWalkable(loc)) {
                render.sce.Position = loc;
                return true;
            } else return false;
        }

        public static void Render_SimpleSet(Render render, int glyph, Color? fg = null, Color? bg = null) {
            render.sce.Animation.CurrentFrame[0].Glyph = glyph;
            if (fg != null) {
                render.sce.Animation.CurrentFrame[0].Foreground = fg.Value;
            }

            if (bg != null) {
                render.sce.Animation.CurrentFrame[0].Background = bg.Value;
            }


            render.sce.Animation.IsDirty = true;
        }

        public static void Render_SetPosition(Render render, int x, int y) {
            render.sce.Position = new Point(x, y);
        }

        public static void Render_SetPosition(Render render, Point loc) {
            render.sce.Position = loc;
        }

        public static void Render_Init(Render render, Point loc, int glyph, Color? fg = null, Color? bg = null) {
            Render_SetPosition(render, loc);
            Render_SimpleSet(render, glyph, fg, bg);
            render.sce.Components.Add(new EntityViewSyncComponent());
        }

        public static void Door_ToggleOpen(Entity parent) {
            if (!parent.Get<Door>().is_open) {
                if (!parent.Get<Door>().is_locked) {
                    parent.Get<Door>().is_open = true;
                    if (parent.Has<BlocksVisibility>()) { parent.Detach<BlocksVisibility>(); }
                    if (parent.Has<BlocksMovement>()) { parent.Detach<BlocksMovement>(); }
                    if (parent.Has<Tile>()) {
                        Tile_SimpleSet(parent.Get<Tile>(), '-');
                    }
                }
            } else {
                parent.Get<Door>().is_open = false;
                parent.Attach(new BlocksVisibility { });
                parent.Attach(new BlocksMovement { });
                if (parent.Has<Tile>()) { Tile_SimpleSet(parent.Get<Tile>(), '+'); }
            }
        }

        public static void UpdateFOV(Viewshed viewshed, Point center) {
           viewshed.old_bool = viewshed.view.BooleanFOV;
           viewshed.view.Calculate(center, viewshed.radius);
        }


        public static void Tile_SimpleSet(Tile tile, int glyph, Color? fg = null, Color? bg = null) {
            tile.cell.Glyph = glyph;
            if (fg != null) {
                tile.cell.Foreground = fg.Value;
            }

            if (bg != null) {
                tile.cell.Background = bg.Value;
            }

            GameLoop.UIManager.MapConsole.IsDirty = true;
        }


        public static int GenerateID() {
            int newID = GameLoop.GlobalRand.Next(int.MinValue, int.MaxValue);

            while (GameLoop.entityIDs.Contains(newID)) {
                newID = GameLoop.GlobalRand.Next(int.MinValue, int.MaxValue);
            }

            GameLoop.entityIDs.Add(newID);
            return newID;
        }
    }
}
