using LofiHollow.Entities;
using LofiHollow.Entities.NPC;
using SadConsole;
using SadConsole.Input;
using SadConsole.UI;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;


namespace LofiHollow.UI {
    public class UI_DialogueWindow {
        public bool BuyingFromShop = true;
        public List<Item> BuyingItems = new List<Item>();
        public List<Item> SellingItems = new List<Item>();
        public int BuyCopper = 0;
        public int BuySilver = 0;
        public int BuyGold = 0;
        public int BuyJade = 0;

        public string chitChat1 = "";
        public string chitChat2 = "";
        public string chitChat3 = "";
        public string chitChat4 = "";
        public string chitChat5 = "";

        public string dialogueOption = "None";
        public string dialogueLatest = "";

        public NPC DialogueNPC = null;

        public SadConsole.Console DialogueConsole;
        public Window DialogueWindow;

        public UI_DialogueWindow(int width, int height, string title) {
            DialogueWindow = new Window(width, height);
            DialogueWindow.CanDrag = false;
            DialogueWindow.Position = new Point(11, 6);

            int diaConWidth = width - 2;
            int diaConHeight = height - 2;

            DialogueConsole = new SadConsole.Console(diaConWidth, diaConHeight);
            DialogueConsole.Position = new Point(1, 1);
            DialogueWindow.Title = title.Align(HorizontalAlignment.Center, diaConWidth, (char)196);


            DialogueWindow.Children.Add(DialogueConsole);
            GameLoop.UIManager.Children.Add(DialogueWindow);

            DialogueWindow.Show();
            DialogueWindow.IsVisible = false;
        }

        public void RenderDialogue() {
            DialogueConsole.Clear();
            Point mousePos = new MouseScreenObjectState(DialogueConsole, GameHost.Instance.Mouse).CellPosition;

            if (DialogueNPC != null) {
                DialogueWindow.IsFocused = true;
                int opinion = 0;
                if (GameLoop.World.Player.MetNPCs.ContainsKey(DialogueNPC.Name))
                    opinion = GameLoop.World.Player.MetNPCs[DialogueNPC.Name];

                DialogueWindow.Title = (DialogueNPC.Name + ", " + DialogueNPC.Occupation + " - " + DialogueNPC.RelationshipDescriptor() + " (" + opinion + ")").Align(HorizontalAlignment.Center, DialogueWindow.Width - 2, (char)196);

                if (dialogueOption == "None" || dialogueOption == "Goodbye") {
                    if (dialogueLatest.Contains('@') && !GameLoop.World.Player.Name.Contains('@')) {
                        int index = dialogueLatest.IndexOf('@');
                        dialogueLatest = dialogueLatest.Remove(index, 1);
                        dialogueLatest = dialogueLatest.Insert(index, GameLoop.World.Player.Name);
                    }
                    string[] allLines = dialogueLatest.Split("|");

                    for (int i = 0; i < allLines.Length; i++)
                        DialogueConsole.Print(1, 1 + i, new ColoredString(allLines[i], Color.White, Color.Black));
                }

                if (dialogueOption == "None") {
                    //  DialogueConsole.Print(1, DialogueConsole.Height - 19, new ColoredString("Mission: [A book for Cobalt]", mousePos.Y == DialogueConsole.Height - 19 ? Color.Yellow : Color.White, Color.Black));

                    DialogueConsole.DrawLine(new Point(0, DialogueConsole.Height - 20), new Point(DialogueConsole.Width - 1, DialogueConsole.Height - 20), 196, Color.White, Color.Black);

                    if (DialogueNPC.Shop != null && DialogueNPC.Shop.ShopOpen(DialogueNPC))
                        DialogueConsole.Print(1, DialogueConsole.Height - 17, new ColoredString("[Open Store]", mousePos.Y == DialogueConsole.Height - 17 ? Color.Yellow : Color.White, Color.Black));

                    DialogueConsole.Print(1, DialogueConsole.Height - 15, new ColoredString("[Give Item]", mousePos.Y == DialogueConsole.Height - 15 ? Color.Yellow : Color.White, Color.Black));

                    DialogueConsole.Print(1, DialogueConsole.Height - 12, new ColoredString("Chit-chat: " + chitChat1, mousePos.Y == DialogueConsole.Height - 12 ? Color.Yellow : Color.White, Color.Black));
                    DialogueConsole.Print(1, DialogueConsole.Height - 10, new ColoredString("Chit-chat: " + chitChat2, mousePos.Y == DialogueConsole.Height - 10 ? Color.Yellow : Color.White, Color.Black));
                    DialogueConsole.Print(1, DialogueConsole.Height - 8, new ColoredString("Chit-chat: " + chitChat3, mousePos.Y == DialogueConsole.Height - 8 ? Color.Yellow : Color.White, Color.Black));
                    DialogueConsole.Print(1, DialogueConsole.Height - 6, new ColoredString("Chit-chat: " + chitChat4, mousePos.Y == DialogueConsole.Height - 6 ? Color.Yellow : Color.White, Color.Black));
                    DialogueConsole.Print(1, DialogueConsole.Height - 4, new ColoredString("Chit-chat: " + chitChat5, mousePos.Y == DialogueConsole.Height - 4 ? Color.Yellow : Color.White, Color.Black));

                    DialogueConsole.Print(1, DialogueConsole.Height - 1, new ColoredString("Nevermind.", mousePos.Y == DialogueConsole.Height - 1 ? Color.Yellow : Color.White, Color.Black));
                } else if (dialogueOption == "Goodbye") {
                    DialogueConsole.Print(1, DialogueConsole.Height - 1, new ColoredString("[Click anywhere to close]", mousePos.Y == DialogueConsole.Height - 1 ? Color.Yellow : Color.White, Color.Black));
                } else if (dialogueOption == "Gift") {
                    int y = 1;
                    DialogueConsole.Print(1, y++, "Give what?");

                    for (int i = 0; i < GameLoop.World.Player.Inventory.Length; i++) {
                        Item item = GameLoop.World.Player.Inventory[i];

                        string nameWithDurability = item.Name;

                        if (item.Durability >= 0)
                            nameWithDurability = "[" + item.Durability + "] " + item.Name;

                        DialogueConsole.Print(1, y, item.AsColoredGlyph());
                        if (item.Dec != null) {
                            DialogueConsole.SetDecorator(1, y, 1, new CellDecorator(new Color(item.Dec.R, item.Dec.G, item.Dec.B), item.Dec.Glyph, Mirror.None));
                        }
                        if (!item.IsStackable || (item.IsStackable && item.ItemQuantity == 1))
                            DialogueConsole.Print(3, y, new ColoredString(nameWithDurability, mousePos.Y == y ? Color.Yellow : item.Name == "(EMPTY)" ? Color.DarkSlateGray : Color.White, Color.Black));
                        else
                            DialogueConsole.Print(3, y, new ColoredString(("(" + item.ItemQuantity + ") " + item.Name), mousePos.Y == y ? Color.Yellow : item.Name == "(EMPTY)" ? Color.DarkSlateGray : Color.White, Color.Black));

                        y++;
                    }

                    DialogueConsole.Print(1, y + 2, new ColoredString("Nevermind.", mousePos.Y == y + 2 ? Color.Yellow : Color.White, Color.Black));
                } else if (dialogueOption == "Shop") {

                    ColoredString shopHeader = new ColoredString(DialogueNPC.Shop.ShopName, Color.White, Color.Black);


                    ColoredString playerCopperString = new ColoredString("CP:" + GameLoop.World.Player.CopperCoins, new Color(184, 115, 51), Color.Black);
                    ColoredString playerSilverString = new ColoredString("SP:" + GameLoop.World.Player.SilverCoins, Color.Silver, Color.Black);
                    ColoredString playerGoldString = new ColoredString("GP:" + GameLoop.World.Player.GoldCoins, Color.Yellow, Color.Black);
                    ColoredString playerJadeString = new ColoredString("JP:" + GameLoop.World.Player.JadeCoins, new Color(0, 168, 107), Color.Black);

                    ColoredString playerMoney = new ColoredString("", Color.White, Color.Black);
                    playerMoney += playerCopperString + new ColoredString(" / ", Color.White, Color.Black);
                    playerMoney += playerSilverString + new ColoredString(" / ", Color.White, Color.Black);
                    playerMoney += playerGoldString + new ColoredString(" / ", Color.White, Color.Black);
                    playerMoney += playerJadeString;



                    DialogueConsole.Print(1, 0, GameLoop.World.Player.Name);
                    DialogueConsole.Print(1, 1, playerMoney);
                    DialogueConsole.Print(DialogueConsole.Width - (shopHeader.Length + 1), 0, shopHeader);
                    DialogueConsole.DrawLine(new Point(0, 2), new Point(DialogueConsole.Width - 1, 2), 196);
                    if (BuyingFromShop) {
                        DialogueConsole.Print(0, 3, " Buy  |" + "Item Name".Align(HorizontalAlignment.Center, 23) + "|" + "Short Description".Align(HorizontalAlignment.Center, 27) + "|" + "Price".Align(HorizontalAlignment.Center, 11));
                        DialogueConsole.DrawLine(new Point(0, 4), new Point(DialogueConsole.Width - 1, 4), 196);


                        for (int i = 0; i < DialogueNPC.Shop.SoldItems.Count; i++) {
                            Item item = new Item(DialogueNPC.Shop.SoldItems[i]);
                            int price = DialogueNPC.Shop.GetPrice(GameLoop.World.Player.MetNPCs[DialogueNPC.Name], item, false);

                            int buyQuantity = 0;

                            for (int j = 0; j < BuyingItems.Count; j++) {
                                if (BuyingItems[j].ItemID == item.ItemID && BuyingItems[j].SubID == item.SubID) {
                                    buyQuantity += BuyingItems[j].ItemQuantity;
                                }
                            }

                            DialogueConsole.Print(0, 5 + i, new ColoredString("-", mousePos.Y == 5 + i && mousePos.X == 0 ? Color.Red : Color.White, Color.Black));
                            DialogueConsole.Print(1, 5 + i, new ColoredString(buyQuantity.ToString().Align(HorizontalAlignment.Center, 3), Color.White, Color.Black));
                            DialogueConsole.Print(5, 5 + i, new ColoredString("+", mousePos.Y == 5 + i && mousePos.X == 5 ? Color.Lime : Color.White, Color.Black));
                            DialogueConsole.Print(6, 5 + i, new ColoredString("|" + item.Name.Align(HorizontalAlignment.Center, 23) + "|" + item.ShortDesc.Align(HorizontalAlignment.Center, 27) + "|", mousePos.Y == 5 + i ? Color.Yellow : Color.White, Color.Black));
                            DialogueConsole.Print(DialogueConsole.Width - 10, 5 + i, ConvertCoppers(price));
                        }

                        DialogueConsole.DrawLine(new Point(0, DialogueConsole.Height - 7), new Point(DialogueConsole.Width - 1, DialogueConsole.Height - 7), 196);
                        DialogueConsole.Print(1, DialogueConsole.Height - 6, new ColoredString("[View Inventory]", (mousePos.Y == DialogueConsole.Height - 6 && mousePos.X >= 1 && mousePos.X <= 16) ? Color.Yellow : Color.White, Color.Black));
                    } else {
                        DialogueConsole.Print(0, 3, " Sell |" + "Item Name".Align(HorizontalAlignment.Center, 23) + "|" + "Short Description".Align(HorizontalAlignment.Center, 27) + "|" + "Price".Align(HorizontalAlignment.Center, 11));
                        DialogueConsole.DrawLine(new Point(0, 4), new Point(DialogueConsole.Width - 1, 4), 196);


                        for (int i = 0; i < GameLoop.World.Player.Inventory.Length; i++) {
                            Item item = new Item(GameLoop.World.Player.Inventory[i]);
                            item.ItemQuantity = GameLoop.World.Player.Inventory[i].ItemQuantity;
                            int price = DialogueNPC.Shop.GetPrice(GameLoop.World.Player.MetNPCs[DialogueNPC.Name], item, true);

                            int sellQuantity = 0;

                            for (int j = 0; j < SellingItems.Count; j++) {
                                if (SellingItems[j].ItemID == item.ItemID && SellingItems[j].SubID == item.SubID) {
                                    sellQuantity = SellingItems[j].ItemQuantity;
                                }
                            }

                            string name = item.Name;

                            if (item.ItemQuantity > 1) {
                                name = "[" + item.ItemQuantity + "] " + name;
                            }

                            DialogueConsole.Print(0, 5 + i, new ColoredString("-", mousePos.Y == 5 + i && mousePos.X == 0 ? Color.Red : Color.White, Color.Black));
                            DialogueConsole.Print(1, 5 + i, new ColoredString(sellQuantity.ToString().Align(HorizontalAlignment.Center, 3), Color.White, Color.Black));
                            DialogueConsole.Print(5, 5 + i, new ColoredString("+", mousePos.Y == 5 + i && mousePos.X == 5 ? Color.Lime : Color.White, Color.Black));
                            DialogueConsole.Print(6, 5 + i, new ColoredString("|" + name.Align(HorizontalAlignment.Center, 23) + "|" + item.ShortDesc.Align(HorizontalAlignment.Center, 27) + "|", mousePos.Y == 5 + i ? Color.Yellow : Color.White, Color.Black));
                            DialogueConsole.Print(DialogueConsole.Width - 10, 5 + i, ConvertCoppers(price));
                        }

                        DialogueConsole.DrawLine(new Point(0, DialogueConsole.Height - 7), new Point(DialogueConsole.Width - 1, DialogueConsole.Height - 7), 196);
                        DialogueConsole.Print(1, DialogueConsole.Height - 6, new ColoredString("[View Shop]", (mousePos.Y == DialogueConsole.Height - 6 && mousePos.X >= 1 && mousePos.X <= 11) ? Color.Yellow : Color.White, Color.Black));
                    }

                    int buyValue = 0;

                    for (int j = 0; j < BuyingItems.Count; j++) {
                        buyValue += BuyingItems[j].ItemQuantity * DialogueNPC.Shop.GetPrice(GameLoop.World.Player.MetNPCs[DialogueNPC.Name], BuyingItems[j], false);
                    }

                    int sellValue = 0;

                    for (int j = 0; j < SellingItems.Count; j++) {
                        sellValue += SellingItems[j].ItemQuantity * DialogueNPC.Shop.GetPrice(GameLoop.World.Player.MetNPCs[DialogueNPC.Name], SellingItems[j], true);
                    }

                    DialogueConsole.Print(28, DialogueConsole.Height - 6, "Coins");

                    ColoredString buyCopperString = new ColoredString(BuyCopper.ToString(), new Color(184, 115, 51), Color.Black);
                    ColoredString buySilverString = new ColoredString(BuySilver.ToString(), Color.Silver, Color.Black);
                    ColoredString buyGoldString = new ColoredString(BuyGold.ToString(), Color.Yellow, Color.Black);
                    ColoredString buyJadeString = new ColoredString(BuyJade.ToString(), new Color(0, 168, 107), Color.Black);

                    DialogueConsole.Print(30, DialogueConsole.Height - 5, buyCopperString);
                    DialogueConsole.Print(30, DialogueConsole.Height - 4, buySilverString);
                    DialogueConsole.Print(30, DialogueConsole.Height - 3, buyGoldString);
                    DialogueConsole.Print(30, DialogueConsole.Height - 2, buyJadeString);

                    DialogueConsole.Print(28, DialogueConsole.Height - 5, new ColoredString("-", mousePos == new Point(28, DialogueConsole.Height - 5) ? Color.Red : Color.White, Color.Black));
                    DialogueConsole.Print(28, DialogueConsole.Height - 4, new ColoredString("-", mousePos == new Point(28, DialogueConsole.Height - 4) ? Color.Red : Color.White, Color.Black));
                    DialogueConsole.Print(28, DialogueConsole.Height - 3, new ColoredString("-", mousePos == new Point(28, DialogueConsole.Height - 3) ? Color.Red : Color.White, Color.Black));
                    DialogueConsole.Print(28, DialogueConsole.Height - 2, new ColoredString("-", mousePos == new Point(28, DialogueConsole.Height - 2) ? Color.Red : Color.White, Color.Black));

                    DialogueConsole.Print(32, DialogueConsole.Height - 5, new ColoredString("+", mousePos == new Point(32, DialogueConsole.Height - 5) ? Color.Lime : Color.White, Color.Black));
                    DialogueConsole.Print(32, DialogueConsole.Height - 4, new ColoredString("+", mousePos == new Point(32, DialogueConsole.Height - 4) ? Color.Lime : Color.White, Color.Black));
                    DialogueConsole.Print(32, DialogueConsole.Height - 3, new ColoredString("+", mousePos == new Point(32, DialogueConsole.Height - 3) ? Color.Lime : Color.White, Color.Black));
                    DialogueConsole.Print(32, DialogueConsole.Height - 2, new ColoredString("+", mousePos == new Point(32, DialogueConsole.Height - 2) ? Color.Lime : Color.White, Color.Black));

                    sellValue += BuyCopper;
                    sellValue += BuySilver * 10;
                    sellValue += BuyGold * 100;
                    sellValue += BuyJade * 1000;

                    DialogueConsole.Print(27, DialogueConsole.Height - 1, "[EXACT]");

                    DialogueConsole.Print(1, DialogueConsole.Height - 4, new ColoredString("Buy Value: ", Color.White, Color.Black) + ConvertCoppers(buyValue));
                    DialogueConsole.Print(1, DialogueConsole.Height - 3, new ColoredString("Sell Value: ", Color.White, Color.Black) + ConvertCoppers(sellValue));

                    int diff = buyValue - sellValue;

                    string total = diff > 0 ? "You owe " : diff == 0 ? "Trade is equal" : "You are owed ";

                    if (diff < 0)
                        DialogueConsole.Print(1, DialogueConsole.Height - 2, new ColoredString(total, Color.White, Color.Black) + ConvertCoppers(-diff));
                    else if (diff > 0)
                        DialogueConsole.Print(1, DialogueConsole.Height - 2, new ColoredString(total, Color.White, Color.Black) + ConvertCoppers(diff));
                    else
                        DialogueConsole.Print(1, DialogueConsole.Height - 2, new ColoredString(total, Color.White, Color.Black));

                    if (diff <= 0)
                        DialogueConsole.Print(DialogueConsole.Width - 15, DialogueConsole.Height - 5, new ColoredString("[Accept - Gift]", Color.Green, Color.Black));
                    else
                        DialogueConsole.Print(DialogueConsole.Width - 15, DialogueConsole.Height - 5, new ColoredString("[Accept - Gift]", Color.Red, Color.Black));


                    if (diff <= 0)
                        DialogueConsole.Print(DialogueConsole.Width - 8, DialogueConsole.Height - 3, new ColoredString("[Accept]", Color.Green, Color.Black));
                    else
                        DialogueConsole.Print(DialogueConsole.Width - 8, DialogueConsole.Height - 3, new ColoredString("[Accept]", Color.Red, Color.Black));

                    DialogueConsole.Print(DialogueConsole.Width - 12, DialogueConsole.Height - 1, new ColoredString("[Close Shop]", (mousePos.Y == DialogueConsole.Height - 1 && mousePos.X >= DialogueConsole.Width - 12 && mousePos.X <= DialogueConsole.Width - 1) ? Color.Yellow : Color.White, Color.Black));

                }
            }
        } 

        public void CaptureDialogueClicks() {
            Point mousePos = new MouseScreenObjectState(DialogueConsole, GameHost.Instance.Mouse).CellPosition;
            if (GameHost.Instance.Mouse.LeftClicked) {
                if (dialogueOption == "Goodbye") {
                    dialogueOption = "None";
                    GameLoop.UIManager.selectedMenu = "None";
                    DialogueWindow.IsVisible = false;
                    DialogueNPC = null;
                    dialogueLatest = "";
                    DialogueConsole.Clear();
                } else if (dialogueOption == "Shop") {
                    if (mousePos.Y == DialogueConsole.Height - 1 && mousePos.X >= DialogueConsole.Width - 12 && mousePos.X <= DialogueConsole.Width - 1) {
                        dialogueOption = "None";
                        BuyingItems.Clear();
                        SellingItems.Clear();
                        BuyingFromShop = true;
                    }

                    if (mousePos.X == 28) {
                        if (mousePos.Y == DialogueConsole.Height - 5 && BuyCopper > 0) { BuyCopper--; }
                        if (mousePos.Y == DialogueConsole.Height - 4 && BuySilver > 0) { BuySilver--; }
                        if (mousePos.Y == DialogueConsole.Height - 3 && BuyGold > 0) { BuyGold--; }
                        if (mousePos.Y == DialogueConsole.Height - 2 && BuyJade > 0) { BuyJade--; }
                    }

                    if (mousePos.X == 32) {
                        if (mousePos.Y == DialogueConsole.Height - 5 && BuyCopper < GameLoop.World.Player.CopperCoins) { BuyCopper++; }
                        if (mousePos.Y == DialogueConsole.Height - 4 && BuySilver < GameLoop.World.Player.SilverCoins) { BuySilver++; }
                        if (mousePos.Y == DialogueConsole.Height - 3 && BuyGold < GameLoop.World.Player.GoldCoins) { BuyGold++; }
                        if (mousePos.Y == DialogueConsole.Height - 2 && BuyJade < GameLoop.World.Player.JadeCoins) { BuyJade++; }
                    }

                    int buyValue = 0;
                    for (int j = 0; j < BuyingItems.Count; j++) {
                        buyValue += BuyingItems[j].ItemQuantity * DialogueNPC.Shop.GetPrice(GameLoop.World.Player.MetNPCs[DialogueNPC.Name], BuyingItems[j], false);
                    }

                    int sellValue = 0;
                    for (int j = 0; j < SellingItems.Count; j++) {
                        sellValue += SellingItems[j].ItemQuantity * DialogueNPC.Shop.GetPrice(GameLoop.World.Player.MetNPCs[DialogueNPC.Name], SellingItems[j], true);
                    }

                    sellValue += BuyCopper;
                    sellValue += BuySilver * 10;
                    sellValue += BuyGold * 100;
                    sellValue += BuyJade * 1000;

                    int diff = buyValue - sellValue;

                    if (mousePos.X >= 27 && mousePos.X <= 33 && mousePos.Y == DialogueConsole.Height - 1 && diff > 0) {
                        if (diff >= 1000)
                            BuyJade = diff / 1000;
                        diff -= BuyJade * 1000;

                        if (diff >= 100)
                            BuyGold = diff / 100;
                        diff -= BuyGold * 100;

                        if (diff >= 10)
                            BuySilver = diff / 10;
                        diff -= BuySilver * 10;

                        BuyCopper = diff;

                        if (BuyJade > GameLoop.World.Player.JadeCoins) {
                            int jadeOff = BuyJade - GameLoop.World.Player.JadeCoins;
                            BuyJade = GameLoop.World.Player.JadeCoins;
                            BuyGold += jadeOff * 10;
                        }

                        if (BuyGold > GameLoop.World.Player.GoldCoins) {
                            int goldOff = BuyJade - GameLoop.World.Player.GoldCoins;
                            BuyGold = GameLoop.World.Player.GoldCoins;
                            BuySilver += goldOff * 10;
                        }

                        if (BuySilver > GameLoop.World.Player.SilverCoins) {
                            int silverOff = BuyJade - GameLoop.World.Player.SilverCoins;
                            BuySilver = GameLoop.World.Player.SilverCoins;
                            BuyCopper += silverOff * 10;
                        }

                        if (BuyCopper > GameLoop.World.Player.CopperCoins) {
                            BuyCopper = GameLoop.World.Player.CopperCoins;
                        }

                    }

                    if (diff <= 0) {
                        if (mousePos.X >= DialogueConsole.Width - 8 && mousePos.X <= DialogueConsole.Width - 3 && mousePos.Y == DialogueConsole.Height - 3) {
                            GameLoop.World.Player.CopperCoins -= BuyCopper;
                            GameLoop.World.Player.SilverCoins -= BuySilver;
                            GameLoop.World.Player.GoldCoins -= BuyGold;
                            GameLoop.World.Player.JadeCoins -= BuyJade;

                            diff *= -1;
                            int plat = 0;
                            int gold = 0;
                            int silver = 0;

                            if (diff > 1000)
                                plat = diff / 1000;
                            diff -= plat * 1000;

                            if (diff > 100)
                                gold = diff / 100;
                            diff -= gold * 100;

                            if (diff > 10)
                                silver = diff / 10;
                            diff -= silver * 10;


                            GameLoop.World.Player.CopperCoins += diff;
                            GameLoop.World.Player.SilverCoins += silver;
                            GameLoop.World.Player.GoldCoins += gold;
                            GameLoop.World.Player.JadeCoins += plat;

                            for (int i = 0; i < SellingItems.Count; i++) {
                                for (int j = 0; j < GameLoop.World.Player.Inventory.Length; j++) {
                                    if (GameLoop.World.Player.Inventory[j].ItemID == SellingItems[i].ItemID && GameLoop.World.Player.Inventory[j].SubID == SellingItems[i].SubID) {
                                        GameLoop.World.Player.Inventory[j].ItemQuantity -= SellingItems[i].ItemQuantity;
                                        if (GameLoop.World.Player.Inventory[j].ItemQuantity <= 0) {
                                            GameLoop.World.Player.Inventory[j] = new Item(0);
                                        }
                                        break;
                                    }
                                }
                            }

                            for (int i = 0; i < BuyingItems.Count; i++) {
                                GameLoop.CommandManager.AddItemToInv(GameLoop.World.Player, BuyingItems[i]);
                            }

                            BuyCopper = 0;
                            BuySilver = 0;
                            BuyGold = 0;
                            BuyJade = 0;

                            BuyingItems.Clear();
                            SellingItems.Clear();
                        } else if (mousePos.X >= DialogueConsole.Width - 15 && mousePos.X <= DialogueConsole.Width - 3 && mousePos.Y == DialogueConsole.Height - 5) {
                            GameLoop.World.Player.CopperCoins -= BuyCopper;
                            GameLoop.World.Player.SilverCoins -= BuySilver;
                            GameLoop.World.Player.GoldCoins -= BuyGold;
                            GameLoop.World.Player.JadeCoins -= BuyJade;

                            diff *= -1;

                            if (diff >= 100) {
                                string reaction = DialogueNPC.ReactGift(-2);
                                if (DialogueNPC.GiftResponses.ContainsKey(reaction))
                                    dialogueLatest = DialogueNPC.GiftResponses[reaction];
                                else
                                    dialogueLatest = "Error - No response for " + reaction + " gift.";
                            } else if (diff >= 10) {
                                string reaction = DialogueNPC.ReactGift(-3);
                                if (DialogueNPC.GiftResponses.ContainsKey(reaction))
                                    dialogueLatest = DialogueNPC.GiftResponses[reaction];
                                else
                                    dialogueLatest = "Error - No response for " + reaction + " gift.";
                            }



                            for (int i = 0; i < SellingItems.Count; i++) {
                                for (int j = 0; j < GameLoop.World.Player.Inventory.Length; j++) {
                                    if (GameLoop.World.Player.Inventory[j].ItemID == SellingItems[i].ItemID && GameLoop.World.Player.Inventory[j].SubID == SellingItems[i].SubID) {
                                        GameLoop.World.Player.Inventory[j].ItemQuantity -= SellingItems[i].ItemQuantity;
                                        if (GameLoop.World.Player.Inventory[j].ItemQuantity <= 0) {
                                            GameLoop.World.Player.Inventory[j] = new Item(0);
                                        }
                                        break;
                                    }
                                }
                            }

                            for (int i = 0; i < BuyingItems.Count; i++) {
                                GameLoop.CommandManager.AddItemToInv(GameLoop.World.Player, BuyingItems[i]);
                            }

                            BuyCopper = 0;
                            BuySilver = 0;
                            BuyGold = 0;
                            BuyJade = 0;

                            BuyingItems.Clear();
                            SellingItems.Clear();
                            dialogueOption = "None";
                            BuyingFromShop = false;
                            return;
                        }
                    }

                    if (BuyingFromShop) {
                        if (mousePos.Y == DialogueConsole.Height - 6 && mousePos.X >= 1 && mousePos.X <= 16) {
                            BuyingFromShop = false;
                        } else {
                            int itemSlot = mousePos.Y - 5;
                            if (itemSlot >= 0 && itemSlot <= DialogueNPC.Shop.SoldItems.Count) {
                                if (mousePos.X == 0) {
                                    for (int i = 0; i < BuyingItems.Count; i++) {
                                        if (BuyingItems[i].ItemID == DialogueNPC.Shop.SoldItems[itemSlot]) {
                                            if (BuyingItems[i].IsStackable && BuyingItems[i].ItemQuantity > 1) {
                                                BuyingItems[i].ItemQuantity--;
                                                break;
                                            } else if (!BuyingItems[i].IsStackable || BuyingItems[i].ItemQuantity == 1) {
                                                BuyingItems.RemoveAt(i);
                                                break;
                                            }
                                        }
                                    }
                                } else if (mousePos.X == 5) {
                                    bool alreadyInList = false;
                                    for (int i = 0; i < BuyingItems.Count; i++) {
                                        if (BuyingItems[i].ItemID == DialogueNPC.Shop.SoldItems[itemSlot]) {
                                            if (BuyingItems[i].IsStackable) {
                                                alreadyInList = true;
                                                BuyingItems[i].ItemQuantity++;
                                                break;
                                            } else if (!BuyingItems[i].IsStackable) {
                                                alreadyInList = true;
                                                BuyingItems.Add(new Item(DialogueNPC.Shop.SoldItems[itemSlot]));
                                                break;
                                            }
                                        }
                                    }

                                    if (!alreadyInList) {
                                        BuyingItems.Add(new Item(DialogueNPC.Shop.SoldItems[itemSlot]));
                                    }
                                }
                            }
                        }
                    } else {
                        if (mousePos.Y == DialogueConsole.Height - 6 && mousePos.X >= 1 && mousePos.X <= 11) {
                            BuyingFromShop = true;
                        } else {
                            int itemSlot = mousePos.Y - 5;
                            if (itemSlot >= 0 && itemSlot <= GameLoop.World.Player.Inventory.Length) {
                                if (mousePos.X == 0) {
                                    for (int i = 0; i < SellingItems.Count; i++) {
                                        if (SellingItems[i].ItemID == GameLoop.World.Player.Inventory[itemSlot].ItemID && SellingItems[i].SubID == GameLoop.World.Player.Inventory[itemSlot].SubID) {
                                            if (SellingItems[i].IsStackable && SellingItems[i].ItemQuantity > 1) {
                                                SellingItems[i].ItemQuantity--;
                                                break;
                                            } else if (!SellingItems[i].IsStackable || SellingItems[i].ItemQuantity == 1) {
                                                SellingItems.RemoveAt(i);
                                                break;
                                            }
                                        }
                                    }
                                } else if (mousePos.X == 5) {
                                    bool alreadyInList = false;

                                    for (int i = 0; i < SellingItems.Count; i++) {
                                        int thisItemCount = 0;
                                        int alreadyInListCount = 0;
                                        if (SellingItems[i].ItemID == GameLoop.World.Player.Inventory[itemSlot].ItemID && SellingItems[i].SubID == GameLoop.World.Player.Inventory[itemSlot].SubID) {
                                            if (SellingItems[i].IsStackable) {
                                                alreadyInList = true;
                                                if (SellingItems[i].ItemQuantity < GameLoop.World.Player.Inventory[itemSlot].ItemQuantity) {
                                                    SellingItems[i].ItemQuantity++;
                                                    break;
                                                }
                                            } else if (!SellingItems[i].IsStackable) {
                                                for (int j = 0; j < GameLoop.World.Player.Inventory.Length; j++) {
                                                    if (GameLoop.World.Player.Inventory[j].ItemID == SellingItems[i].ItemID && GameLoop.World.Player.Inventory[j].SubID == SellingItems[i].SubID) {
                                                        thisItemCount++;
                                                    }
                                                }

                                                for (int j = 0; j < SellingItems.Count; j++) {
                                                    if (GameLoop.World.Player.Inventory[itemSlot].ItemID == SellingItems[j].ItemID && GameLoop.World.Player.Inventory[itemSlot].SubID == SellingItems[j].SubID) {
                                                        alreadyInListCount++;
                                                    }
                                                }

                                                if (alreadyInListCount < thisItemCount) {
                                                    alreadyInList = true;
                                                    SellingItems.Add(new Item(GameLoop.World.Player.Inventory[itemSlot]));
                                                    break;
                                                } else {
                                                    alreadyInList = true;
                                                }
                                            }
                                        }
                                    }

                                    if (!alreadyInList) {
                                        SellingItems.Add(new Item(GameLoop.World.Player.Inventory[itemSlot]));
                                        SellingItems[SellingItems.Count - 1].ItemQuantity = 1;
                                    }
                                }
                            }
                        }
                    }
                } else if (dialogueOption == "Gift") {
                    if (mousePos.Y == 4 + GameLoop.World.Player.Inventory.Length) {
                        dialogueOption = "None";
                    } else if (mousePos.Y >= 2 && mousePos.Y <= 2 + GameLoop.World.Player.Inventory.Length) {
                        int slot = mousePos.Y - 2;
                        int itemID = GameLoop.CommandManager.RemoveOneItem(GameLoop.World.Player, slot);

                        if (itemID != -1 && itemID != 0) {
                            string reaction = DialogueNPC.ReactGift(itemID);
                            dialogueOption = "None";
                            if (DialogueNPC.GiftResponses.ContainsKey(reaction))
                                dialogueLatest = DialogueNPC.GiftResponses[reaction];
                            else
                                dialogueLatest = "Error - No response for " + reaction + " gift.";
                        }
                    }
                } else if (dialogueOption == "None") {
                    if (mousePos.Y == DialogueConsole.Height - 1) {
                        dialogueOption = "Goodbye";
                        if (DialogueNPC.Farewells.ContainsKey(DialogueNPC.RelationshipDescriptor())) {
                            dialogueLatest = DialogueNPC.Farewells[DialogueNPC.RelationshipDescriptor()];
                        } else {
                            dialogueLatest = "Error: Greeting not found for relationship " + DialogueNPC.RelationshipDescriptor();
                        }
                    } else if (mousePos.Y <= DialogueConsole.Height - 4 && mousePos.Y >= DialogueConsole.Height - 12) {
                        string chat = "";

                        if (mousePos.Y == DialogueConsole.Height - 12)
                            chat = DialogueNPC.ChitChats[chitChat1];
                        else if (mousePos.Y == DialogueConsole.Height - 10)
                            chat = DialogueNPC.ChitChats[chitChat2];
                        else if (mousePos.Y == DialogueConsole.Height - 8)
                            chat = DialogueNPC.ChitChats[chitChat3];
                        else if (mousePos.Y == DialogueConsole.Height - 6)
                            chat = DialogueNPC.ChitChats[chitChat4];
                        else if (mousePos.Y == DialogueConsole.Height - 4)
                            chat = DialogueNPC.ChitChats[chitChat5];

                        if (chat != "") {
                            string[] chatParts = chat.Split("~");

                            if (chatParts.Length == 2) {
                                dialogueLatest = chatParts[1];
                                GameLoop.World.Player.MetNPCs[DialogueNPC.Name] += Int32.Parse(chatParts[0]);
                                DialogueNPC.UpdateChitChats();
                            }
                        }
                    } else if (mousePos.Y == DialogueConsole.Height - 15) { // Give item
                        dialogueOption = "Gift";
                    } else if (mousePos.Y == DialogueConsole.Height - 17) { // Open shop dialogue
                        if (DialogueNPC.Shop != null && DialogueNPC.Shop.ShopOpen(DialogueNPC))
                            dialogueOption = "Shop";
                    }
                }
            }
        }

        public ColoredString ConvertCoppers(int copperValue) {
            int coinsLeft = copperValue;
            int Jade = copperValue / 1000;

            coinsLeft = coinsLeft - (Jade * 1000);
            int gold = coinsLeft / 100;

            coinsLeft = coinsLeft - (gold * 100);
            int silver = coinsLeft / 10;

            coinsLeft = coinsLeft - (silver * 10);
            int copper = coinsLeft;

            ColoredString build = new ColoredString("", Color.White, Color.Black);

            ColoredString copperString = new ColoredString(copper + "c", new Color(184, 115, 51), Color.Black);
            ColoredString silverString = new ColoredString(silver + "s ", Color.Silver, Color.Black);
            ColoredString goldString = new ColoredString(gold + "g ", Color.Yellow, Color.Black);
            ColoredString JadeString = new ColoredString(Jade + "j ", new Color(0, 168, 107), Color.Black);

            if (Jade > 0)
                build += JadeString;
            if (gold > 0)
                build += goldString;
            if (silver > 0)
                build += silverString;
            if (copper > 0)
                build += copperString;

            return build;
        }
    }
}
