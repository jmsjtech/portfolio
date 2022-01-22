using LofiHollow.Entities;
using LofiHollow.Managers;
using SadConsole;
using SadConsole.Input;
using SadConsole.UI;
using SadRogue.Primitives;
using Key = SadConsole.Input.Keys;

namespace LofiHollow.UI {
    public class UI_Inventory {
        public SadConsole.Console InventoryConsole;
        public Window InventoryWindow;
        public int invMoveIndex = -1;

        public UI_Inventory(int width, int height, string title) {
            InventoryWindow = new(width, height);
            InventoryWindow.CanDrag = false;
            InventoryWindow.Position = new(11, 6);

            int invConWidth = width - 2;
            int invConHeight = height - 2;

            InventoryConsole = new(invConWidth, invConHeight);
            InventoryConsole.Position = new(1, 1);
            InventoryWindow.Title = title.Align(HorizontalAlignment.Center, invConWidth, (char)196);


            InventoryWindow.Children.Add(InventoryConsole);
            GameLoop.UIManager.Children.Add(InventoryWindow);

            InventoryWindow.Show();
            InventoryWindow.IsVisible = false;
        }


        public void RenderInventory() {
            Point mousePos = new MouseScreenObjectState(InventoryConsole, GameHost.Instance.Mouse).CellPosition;

            InventoryConsole.Clear();
            InventoryConsole.Print((InventoryConsole.Width / 2) - 4, 0, "BACKPACK");

            for (int i = 0; i < 27; i++) {
                if (i < GameLoop.World.Player.Inventory.Length) {
                    Item item = GameLoop.World.Player.Inventory[i];

                    string nameWithDurability = item.Name;

                    if (item.Durability >= 0)
                        nameWithDurability = "[" + item.Durability + "] " + item.Name;

                    InventoryConsole.Print(0, i + 1, item.AsColoredGlyph());
                    if (item.Dec != null) {
                        InventoryConsole.SetDecorator(0, i+1, 1, new CellDecorator(new Color(item.Dec.R, item.Dec.G, item.Dec.B), item.Dec.Glyph, Mirror.None));
                    }
                    if (!item.IsStackable || (item.IsStackable && item.ItemQuantity == 1))
                        InventoryConsole.Print(2, i + 1, new ColoredString(nameWithDurability, invMoveIndex == i ? Color.Yellow : item.Name == "(EMPTY)" ? Color.DarkSlateGray : Color.White, Color.Black));
                    else
                        InventoryConsole.Print(2, i + 1, new ColoredString(("(" + item.ItemQuantity + ") " + item.Name), invMoveIndex == i ? Color.Yellow : item.Name == "(EMPTY)" ? Color.DarkSlateGray : Color.White, Color.Black));

                    ColoredString Options = new("MOVE", (mousePos.Y == i + 1 && mousePos.X < 33) ? Color.Yellow : item.Name == "(EMPTY)" ? Color.DarkSlateGray : Color.White, Color.Black);

                    Options += new ColoredString(" | ", Color.White, Color.Black);

                    if (item.EquipSlot != -1) {
                        Options += new ColoredString("EQUIP", (mousePos.Y == i + 1 && mousePos.X > 33 && mousePos.X < 41) ? Color.Yellow : item.Name == "(EMPTY)" ? Color.DarkSlateGray : Color.White, Color.Black);
                    } else if (item.ItemCategory == 11) {
                        Options += new ColoredString(" USE ", (mousePos.Y == i + 1 && mousePos.X > 33 && mousePos.X < 41) ? Color.Yellow : item.Name == "(EMPTY)" ? Color.DarkSlateGray : Color.White, Color.Black);
                    } else {
                        Options += new ColoredString("     ", item.Name == "(EMPTY)" ? Color.DarkSlateGray : Color.White, Color.Black);
                    }

                    Options += new ColoredString(" | ", Color.White, Color.Black);
                    Options += new ColoredString("DROP", (mousePos.Y == i + 1 && mousePos.X > 41) ? Color.Yellow : item.Name == "(EMPTY)" ? Color.DarkSlateGray : Color.White, Color.Black);

                    InventoryConsole.Print(28, i + 1, Options);
                } else {
                    InventoryConsole.Print(2, i + 1, new ColoredString("[LOCKED]", Color.DarkSlateGray, Color.Black));
                }
            }

        }

        public void InventoryInput() {
            Point mousePos = GameHost.Instance.Mouse.ScreenPosition.PixelLocationToSurface(12, 12);
            if (GameHost.Instance.Keyboard.IsKeyReleased(Key.I)) {
                ToggleInventory();
            }

            if (mousePos.X >= 10 && mousePos.X <= 59 && mousePos.Y >= 5 && mousePos.Y <= 34) {
                int slot = mousePos.Y - 8;
                if (slot >= 0 && slot < GameLoop.World.Player.Inventory.Length) {
                    int x = mousePos.X - 10;
                    if (GameHost.Instance.Mouse.LeftClicked) {
                        if (x < 35) {
                            if (invMoveIndex == -1)
                                invMoveIndex = slot;
                            else {
                                Item tempID = GameLoop.World.Player.Inventory[invMoveIndex];
                                GameLoop.World.Player.Inventory[invMoveIndex] = GameLoop.World.Player.Inventory[slot];
                                GameLoop.World.Player.Inventory[slot] = tempID;
                                invMoveIndex = -1;
                            }
                        } else if (x > 35 && x < 43) {
                            Item item = GameLoop.World.Player.Inventory[slot];
                            if (GameLoop.World.Player.Inventory[slot].EquipSlot != -1) {
                                CommandManager.EquipItem(GameLoop.World.Player, slot, GameLoop.World.Player.Inventory[slot]);
                            } else if (item.ItemCategory == 11) {
                                string[] itemResult = CommandManager.UseItem(GameLoop.World.Player, item).Split("|"); ;

                                if (itemResult[0] != "f") {
                                    if (item.IsStackable && item.ItemQuantity > 1) {
                                        item.ItemQuantity -= 1;
                                    } else {
                                        GameLoop.World.Player.Inventory[slot] = new Item(0);
                                    }
                                    GameLoop.UIManager.AddMsg(new ColoredString("Used the " + item.Name + ".", Color.AliceBlue, Color.Black));
                                    GameLoop.UIManager.AddMsg(new ColoredString(itemResult[1], Color.AliceBlue, Color.Black));
                                } else {
                                    GameLoop.UIManager.AddMsg(new ColoredString("Tried to use the " + item.Name + ".", Color.AliceBlue, Color.Black));
                                    GameLoop.UIManager.AddMsg(new ColoredString(itemResult[1], Color.AliceBlue, Color.Black));
                                }
                            }
                        } else if (x > 43) {
                            if (slot < GameLoop.World.Player.Inventory.Length && slot >= 0) {
                                CommandManager.DropItem(GameLoop.World.Player, slot);
                            }
                        }
                    }
                }
            }
        }


        public void ToggleInventory() {
            if (InventoryWindow.IsVisible) {
                GameLoop.UIManager.selectedMenu = "None";
                InventoryWindow.IsVisible = false;
                GameLoop.UIManager.Map.MapConsole.IsFocused = true;
            } else {
                GameLoop.UIManager.selectedMenu = "Inventory";
                InventoryWindow.IsVisible = true;
                InventoryWindow.IsFocused = true;
            }
        }
    }
}
