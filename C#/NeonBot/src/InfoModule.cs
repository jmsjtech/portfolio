using Discord;
using Discord.Commands;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace NeonBot {
    public class InfoModule : ModuleBase<SocketCommandContext> {
        Random rand = new Random();
        ulong catID = 0;

        [Command("say")]
        public Task SayAsync([Remainder] string echo) => ReplyAsync(echo);



        [Command("roll")]
        [Alias("r")]
        public async Task RollAsync([Remainder] string diceNotation) {
            OnePlat.DiceNotation.IDice dice = new OnePlat.DiceNotation.Dice();
            OnePlat.DiceNotation.DiceResult result = dice.Roll(diceNotation, new OnePlat.DiceNotation.DieRoller.RandomDieRoller());

            string allDice = "";
            for (int i = 0; i < result.Results.Count; i++) {
                if (allDice != "") { allDice += ", "; }

                allDice += result.Results[i].Value;
            }
            await ReplyAsync("<@" + Context.User.Id + ">, Dice Result: " + result.Value + " (" + allDice + ")");
        }
    }


    [Group("loot")]
    public class LootModule : ModuleBase<SocketCommandContext> {
        [Command("")]
        public async Task LootDefaultAsync() {
            string detailList = "<@" + Context.User.Id + ">, the possible commands for `!loot` are: `color`, 'garment-mat', and 'implausible-mat'.";
            await ReplyAsync(detailList);
        }

        [Command("color")]
        public async Task ColorAsync() {
            string detailList = "<@" + Context.User.Id + ">, random color: " + LootGenFunctions.GetColor() + ".";
            await ReplyAsync(detailList);
        }

        [Command("garment-mat")]
        public async Task GarmentMatAsync() {
            NeonBot.obj.GarmentMat garmentMat = LootGenFunctions.GetGarmentMat();
            string randomGarment = "<@" + Context.User.Id + ">, random material: " + garmentMat.name + " (x" + (1 + garmentMat.valueMod) + " value)";
            await ReplyAsync(randomGarment);
        }

        [Command("implausible-mat")]
        public async Task GarmentImplausibleMatAsync() {
            NeonBot.obj.GarmentMat garmentMat = LootGenFunctions.GetImplausibleMat();
            string randomGarment = "<@" + Context.User.Id + ">, implausible material: " + garmentMat.name + " (x" + (1 + garmentMat.valueMod) + " value). " + garmentMat.desc;
            await ReplyAsync(randomGarment);
        }

        [Command("spice")]
        public async Task SpiceAsync() {
            NeonBot.obj.GeneralItem spiceItem = LootGenFunctions.GetSpice();
            Random rand = new Random();
            float spiceOz = rand.Next(1, 7) / 2;

            string randomSpice = "<@" + Context.User.Id + ">, spice: " + spiceOz + " ounces of " + spiceItem.name + ", ($" + spiceItem.value + " per oz). " + spiceItem.desc;
            await ReplyAsync(randomSpice);
        }

        [Command("fiber")]
        public async Task FiberASync() {
            NeonBot.obj.GeneralItem fiberItem = LootGenFunctions.GetFiber();

            string randomFiber = "";

            if (fiberItem.desc != "Price per pound.") {
                randomFiber = "<@" + Context.User.Id + ">, fiber: " + fiberItem.name + ", *$" + fiberItem.value + "*. Weighs " + fiberItem.weight + " lbs. " + fiberItem.desc;
            }

            await ReplyAsync(randomFiber);
        }
    }


    [Group("quest")]
    public class QuestModule : ModuleBase<SocketCommandContext> {
        Random rand = new Random();

        string[] people = { "Shoemaker", "Furrier", "Maidservant", "Tailor", "Barber", "Jeweler", "Innkeeper", "Tavernkeep", "Clothier", "Pastrycook", 
            "Mason", "Carpenter", "Weaver", "Chandler", "Mercer", "Cooper", "Baker", "Watercarrier", "Scabbardmaker", "Wine-Seller", "Hatmaker",
            "Saddler", "Butcher", "Pursemaker", "Bookbinder", "Beer-Seller", "Buckle Maker", "Plasterer", "Spice Merchant", "Blacksmith", "Painter",
            "Doctor", "Roofer", "Locksmith", "Bather", "Ropemaker", "Tanner", "Copyist", "Sculptor", "Rugmaker", "Cutler", "Glovemaker", "Woodcarver",
            "Bookseller", "Illuminator", "Priest", "Clergy", "Lawyer", "Noble", "Guard", "Knight"};




        [Command("")]
        public async Task QuestAsync() {
            string questPost = "<@" + Context.User.Id + ">, the quest is:\n";

            questPost = questPost + "Goal: ";


            await ReplyAsync(questPost);
        }



        [Command("help")]
        public async Task QuestDefaultAsync() {
            string detailList = "<@" + Context.User.Id + ">, the possible commands for `!quest` are: .";
            await ReplyAsync(detailList);
        }
    }

    [Group("set")]
    public class SettlementModule : ModuleBase<SocketCommandContext> {
        Random rand = new Random();
        string[] districts = { "Water MarketAA", "BoathouseAB", "FisheryAC", "BathhouseAD", "Noble HouseAE", "Judicial CourtAF", "SewersAG", "TempleVBA",
            "CaravansaryBB", "Cattle RangeBC", "Ranger's HallBD", "BazaarBE", "Courier StationBF", "GraveyardBG", "Watercrop FieldCA", "Hanging GardensCB",
            "RestaurantVCC", "Destination DiningCD", "GreenhouseCE", "Luxury Crop FieldCF", "Terrace FarmCG", "Hot Spring SpaDA",
            "Bard's GuildDB", "VineyardDC", "BrothelDD", "CarnivalDE", "BarVDF", "AmphitheaterDG", "Scholar GuildVEA", "Merchant GuildEB",
            "Textiles GuildEC", "Jewelers GuildED", "Artisans GuildEE", "Blacksmith GuildEF", "Mason GuildEG", "PrisonFA", "Scout StationFB", "Supply OutpostFC",
            "Mercenary GuildFD", "TreasuryVFE", "Adventurer GuildFF", "Primary GarrisonFG", "FountainsGA", "TowerGB", "GardensGC", "MuseumGD",
            "Engineer GuildGE", "City WallsGF", "Town HallGG"
        };

        [Command("")]
        public async Task SettlementAsync([Remainder] int settleSize) {
            string setPost = "";

            double demoAq = 0;
            double demoAu = 0;
            double demoCi = 0;
            double demoOt = 0;
            double demoPa = 0;
            double demoSa = 0;
            double demoTe = 0;

            int districtCount = 0;
            bool newline = false;

            if (settleSize <= 0) {
                setPost = "Must enter a number greater than 0.";
            } else {
                setPost = "<@" + Context.User.Id + ">, the settlement (size " + settleSize + ") has:\n";
                setPost += "``` \n";
                
                for (int count = 0; count < settleSize; count += 0) {
                    int districtSize = rand.Next(1, 7);
                    bool isDistrict = false;

                    

                    //if (districtSize >= 4) {
                    //    districtSize -= 3;
                    //    isDistrict = true;
                   // }

                    if (count + districtSize > settleSize) {
                        districtSize = settleSize - count;
                    }

                    int sizeMod = districtSize;

                    string district = districts[rand.Next(0, districts.Length)];
                    string tempDist = "";


                    if (district[district.Length - 3].ToString() == "V") {
                        tempDist = district.Substring(0, district.Length - 3);

                        if (tempDist == "Vineyard") {
                            string[] types = { "Vineyard", "Winery", "Brewery", "Distillery" };
                            tempDist = types[rand.Next(0, types.Length)];

                        } else if (tempDist == "Treasury") {
                            string[] types = { "Treasury", "Bank", "Lender", "Currency Exchange" };
                            tempDist = types[rand.Next(0, types.Length)];
                        } else if (tempDist == "Restaurant") {
                            string[] types = { "Bakery", "Butcher", "Exotic Meats", "Restauraunt", "Farmer's Market" };
                            tempDist = types[rand.Next(0, types.Length)];
                        } else if (tempDist == "Bar") {
                            string[] types = { "Bar", "Tavern", "Inn" };
                            tempDist = types[rand.Next(0, types.Length)];
                        } else if (tempDist == "Temple") {
                            string[] types = { "Temple", "Church", "Monastery", "Nunnery", "Shrine" };
                            tempDist = types[rand.Next(0, types.Length)];
                        } else if (tempDist == "Scholar Guild") {
                            string[] types = { "Scholar Guild", "Library", "Magic Item Shop" };
                            tempDist = types[rand.Next(0, types.Length)];
                        }

                        tempDist += " (" + districtSize + ")";

                    } else {
                        tempDist = district.Substring(0, district.Length - 2) + " (" + districtSize + ")";
                    }

                 
                    if (!newline && count != settleSize - 1) {
                        tempDist = tempDist.PadRight(20);
                        setPost = setPost + tempDist;
                        setPost += "|    ";
                        newline = true;
                    } else {
                        setPost = setPost + tempDist;
                        setPost += "\n";
                        newline = false;
                    }


                    if (setPost.Length > 1900) {
                        setPost += "```";
                        await ReplyAsync(setPost);
                        setPost = "<@" + Context.User.Id + ">, settlement (size " + settleSize + ") continued:\n```\n";
                    }



                    if (district[district.Length-2].ToString() == "A") {
                        demoAq += sizeMod * 59;
                        demoAu += sizeMod * 1;
                        demoCi += sizeMod * 1;
                        demoOt += sizeMod * 1;
                        demoPa += sizeMod * 1;
                        demoSa += sizeMod * 1;
                        demoTe += sizeMod * 1;
                    } else if (district[district.Length - 2].ToString() == "B") {
                        demoAq += sizeMod * 1;
                        demoAu += sizeMod * 59;
                        demoCi += sizeMod * 1;
                        demoOt += sizeMod * 1;
                        demoPa += sizeMod * 1;
                        demoSa += sizeMod * 1;
                        demoTe += sizeMod * 1;
                    } else if (district[district.Length - 2].ToString() == "C") {
                        demoAq += sizeMod * 1;
                        demoAu += sizeMod * 1;
                        demoCi += sizeMod * 59;
                        demoOt += sizeMod * 1;
                        demoPa += sizeMod * 1;
                        demoSa += sizeMod * 1;
                        demoTe += sizeMod * 1;
                    } else if (district[district.Length - 2].ToString() == "D") {
                        demoAq += sizeMod * 1;
                        demoAu += sizeMod * 1;
                        demoCi += sizeMod * 1;
                        demoOt += sizeMod * 59;
                        demoPa += sizeMod * 1;
                        demoSa += sizeMod * 1;
                        demoTe += sizeMod * 1;
                    } else if (district[district.Length - 2].ToString() == "E") {
                        demoAq += sizeMod * 1;
                        demoAu += sizeMod * 1;
                        demoCi += sizeMod * 1;
                        demoOt += sizeMod * 1;
                        demoPa += sizeMod * 59;
                        demoSa += sizeMod * 1;
                        demoTe += sizeMod * 1;
                    } else if (district[district.Length - 2].ToString() == "F") {
                        demoAq += sizeMod * 1;
                        demoAu += sizeMod * 1;
                        demoCi += sizeMod * 1;
                        demoOt += sizeMod * 1;
                        demoPa += sizeMod * 1;
                        demoSa += sizeMod * 59;
                        demoTe += sizeMod * 1;
                    } else if (district[district.Length - 2].ToString() == "G") {
                        demoAq += sizeMod * 1;
                        demoAu += sizeMod * 1;
                        demoCi += sizeMod * 1;
                        demoOt += sizeMod * 1;
                        demoPa += sizeMod * 1;
                        demoSa += sizeMod * 1;
                        demoTe += sizeMod * 59;
                    }

                    if (district[district.Length - 1].ToString() == "A") {
                        demoAq += sizeMod * 29;
                        demoAu += sizeMod * 1;
                        demoCi += sizeMod * 1;
                        demoOt += sizeMod * 1;
                        demoPa += sizeMod * 1;
                        demoSa += sizeMod * 1;
                        demoTe += sizeMod * 1;
                    } else if (district[district.Length - 1].ToString() == "B") {
                        demoAq += sizeMod * 1;
                        demoAu += sizeMod * 29;
                        demoCi += sizeMod * 1;
                        demoOt += sizeMod * 1;
                        demoPa += sizeMod * 1;
                        demoSa += sizeMod * 1;
                        demoTe += sizeMod * 1;
                    } else if (district[district.Length - 1].ToString() == "C") {
                        demoAq += sizeMod * 1;
                        demoAu += sizeMod * 1;
                        demoCi += sizeMod * 29;
                        demoOt += sizeMod * 1;
                        demoPa += sizeMod * 1;
                        demoSa += sizeMod * 1;
                        demoTe += sizeMod * 1;
                    } else if (district[district.Length - 1].ToString() == "D") {
                        demoAq += sizeMod * 1;
                        demoAu += sizeMod * 1;
                        demoCi += sizeMod * 1;
                        demoOt += sizeMod * 29;
                        demoPa += sizeMod * 1;
                        demoSa += sizeMod * 1;
                        demoTe += sizeMod * 1;
                    } else if (district[district.Length - 1].ToString() == "E") {
                        demoAq += sizeMod * 1;
                        demoAu += sizeMod * 1;
                        demoCi += sizeMod * 1;
                        demoOt += sizeMod * 1;
                        demoPa += sizeMod * 29;
                        demoSa += sizeMod * 1;
                        demoTe += sizeMod * 1;
                    } else if (district[district.Length - 1].ToString() == "F") {
                        demoAq += sizeMod * 1;
                        demoAu += sizeMod * 1;
                        demoCi += sizeMod * 1;
                        demoOt += sizeMod * 1;
                        demoPa += sizeMod * 1;
                        demoSa += sizeMod * 29;
                        demoTe += sizeMod * 1;
                    } else if (district[district.Length - 1].ToString() == "G") {
                        demoAq += sizeMod * 1;
                        demoAu += sizeMod * 1;
                        demoCi += sizeMod * 1;
                        demoOt += sizeMod * 1;
                        demoPa += sizeMod * 1;
                        demoSa += sizeMod * 1;
                        demoTe += sizeMod * 29;
                    }



                    else {
                        setPost = district.Substring(district.Length - 2);
                    }


                    districtCount += sizeMod;
                    count += districtSize;
                }

                int disTotal = districtCount * 100;

                setPost += "\n\n";
                setPost += "Rough Demographics:\n";
                setPost += "Aquan: " + Math.Round((demoAq / disTotal)*100, 2) + "%\n";
                setPost += "Auran: " + Math.Round((demoAu / disTotal)*100, 2) + "%\n";
                setPost += "Cibum: " + Math.Round((demoCi / disTotal)*100, 2) + "%\n";
                setPost += "Otium: " + Math.Round((demoOt / disTotal)*100, 2) + "%\n";
                setPost += "Pannum: " + Math.Round((demoPa / disTotal)*100, 2) + "%\n";
                setPost += "Salutem: " + Math.Round((demoSa / disTotal)*100, 2) + "%\n";
                setPost += "Tectum: " + Math.Round((demoTe / disTotal)*100, 2) + "%\n";


            }

            setPost += "```";

            await ReplyAsync(setPost);
        }



        [Command("help")]
        public async Task SettlementDefaultAsync() {
            string detailList = "<@" + Context.User.Id + ">, the possible commands for `!settlement` are: .";
            await ReplyAsync(detailList);
        }
    }

    [Group("details")]
    public class ArckitModule : ModuleBase<SocketCommandContext> {
        Random rand = new Random();

        [Command("")]
        public async Task DetailDefaultAsync() {
            string detailList = "<@" + Context.User.Id + ">, the possible commands for `!details` are: `smell`, `building`, `job`, `vidfeed`, `gang`, and `person`.";
            await ReplyAsync(detailList);
        }


        [Command("smell")]
        public async Task SmellAsync() {
            string[] smells = {
            "Cigarette Smoke", "Cheap Perfume", "Expensive Cologne", "Exhaust Fumes", "Stale Refuse", "Urine", "Vomit", "Burning Plastic", "Ash", "Acrid Chemicals",
            "Cordite", "Blood", "Wet Hair", "Motor Oil", "Feces", "Soda Pop Sweetness", "Noodles", "Rubber", "Burnt Meat", "Dirty Sneakers",
            "Fried Food", "Beer", "Perfumed Bleach", "Body Odor", "Varnish", "Insecticide", "Soap", "Sulphur", "Hairspray", "Printed Polyamides",
            "Hydraulic Fluid", "Coffee", "Resin", "Antiseptic", "Candy", "Mint", "Salt", "Tea", "Fresh Sweat", "Infected Tissue",
            "Paint", "Mold & Mildew", "Baby Powder", "Acid", "Feet", "Cinnamon", "Leather", "Lemon Zest", "Damp", "Rot",
            "Overheated Circuit Board", "Cigars", "Floral Scent", "Pizza", "Spices", "Lavender", "Sewage", "Pine", "Crack Cocaine", "Cat Piss",
            "Gas", "Latex", "French Fries", "New Cyberlimb Smell", "Sex", "Menthol", "Cheese", "Wet Concrete", "Disinfectant", "Polythene",
            "Nail Varnish", "Whiskey", "Coconut Oil", "Vinyl", "Wine", "Acetone", "Cookies", "Ammonia", "Biodiesel", "Polish",
            "Printer Toner", "Dust", "Glass Cleaner", "Musty", "Opiates", "Raw Meat", "Laminate", "Weed/Skunk", "Drains", "Thinners",
            "Old Food", "Incense", "Fused Wiring", "Lube", "Sour Milk", "Garlic", "Alcohol Sanitizer", "Cheap Aftershave", "Gun Oil"
        };

            var userInfo = Context.User;

            string firstSmell = smells[rand.Next(0, smells.Length)].ToLower();
            string secondSmell = smells[rand.Next(0, smells.Length)].ToLower();
            string thirdSmell = smells[rand.Next(0, smells.Length)].ToLower();

            int complexity = rand.Next(0, 100);

            string[] intensities = { " strongly ", " overpoweringly ", " faintly ", " cloyingly ", " " };

            string intensity1 = intensities[rand.Next(0, intensities.Length)];
            string intensity2 = intensities[rand.Next(0, intensities.Length)];
            string intensity3 = intensities[rand.Next(0, intensities.Length)];

            string output = "<@" + userInfo.Id + ">:\nThis place smells";

            if (complexity < 33) {
                output += intensity1 + "of " + firstSmell + ".";
            } else if (complexity >= 33 && complexity < 66) {
                if (intensity1 == intensity2) {
                    output += intensity1 + "of " + firstSmell + " and " + secondSmell + ".";
                } else {
                    output += intensity1 + "of " + firstSmell + ", and" + intensity2 + "of " + secondSmell + ".";
                }
            } else if (complexity >= 66) {
                if (intensity1 == intensity2) {
                    output += intensity1 + "of " + firstSmell + " and " + secondSmell + ".";
                } else {
                    output += intensity1 + "of " + firstSmell + ", and" + intensity2 + "of " + secondSmell + ".";
                }

                output += " The occasional breeze brings the scent of " + thirdSmell + ".";
            }


            await ReplyAsync(output);
        }

        [Command("building")]
        public async Task BuildingAsync() {
            var userInfo = Context.User;


            string[] buildings = {
            "Pharmacy", "Pharmacy", "Consumer Electronics", "Consumer Electronics", "Art Dealer or Gallery", "Auto or Robotics Repair", "Storage Units", "Warehousing", "Legal Firm",
            "Religious Building", "Religious Building", "Capsule Hotel", "Capsule Hotel", "Data Storage", "Low Rent Housing Project", "Low Rent Housing Project", "Low Rent Housing Project", "Grocery Store", "Hypermarket",
            "Elevated Rail or Road Overpass", "Fast Food Franchise", "Fast Food Franchise", "Police Precinct", "Police Precinct", "School or College", "Government Building", "Government Building", "Garage or Parking Block",
            "Office Block", "Office Block", "Office Block", "Office Block", "Public Transport Hub", "Public Transport Hub", "Hospital or Clinic", "Hospital or Clinic", "Department Store", "Department Store",
            "Body Augmentation Clinic", "Body Augmentation Clinic", "Body Augmentation Clinic", "Luxury Apartments", "Luxury Apartments", "New Media Company", "New Media Company", "Industrial", "Industrial", "Security Tech",
            "Vehicle Showroom", "Vehicle Showroom", "Fashion Boutique", "Fashion Boutique", "Commercial Cybernetics", "Commercial Cybernetics", "Commercial Cybernetics", "Mall", "VRcade", "Gym",
            "Leisureplex", "Leisureplex", "Apartment Block or Hab Stack", "Apartment Block or Hab Stack", "Apartment Block or Hab Stack", "Apartment Block or Hab Stack", "Apartment Block or Hab Stack", "Nightclub", "Nightclub", "Nightclub",
            "Underpass", "Hotel", "Hotel", "Hotel", "Ripperdoc", "Ripperdoc", "3D Print Fabrication", "3D Print Fabrication", "Courier or Bulk Transport Company", "Courier or Bulk Transport Company",
            "Bar", "Bar", "Bar", "Restaurant", "Restaurant", "Pop-Up Market", "Pop-Up Market", "Coffee Shop", "Coffee Shop", "Taxi Firm",
            "Pocket Park", "Pocket Park", "Suburban Housing", "Suburban Housing", "Movie Theater", "Weapons Tech or Sales", "Multi-Level Car Park", "Multi-Level Car Park", "Bank", "Antiques"
        };

            string building = buildings[rand.Next(0, buildings.Length)];

            string output = "<@" + userInfo.Id + ">:\nIt's a " + building + ".";




            string[] styles = { "Minimalist", "Industrial", "Shabby-Chic", "(Bio)Organic", "Brushed Steel", "Polymer Baroque", "Gothic", "Rustic", "Office Beige", "Hexagonal Tiles" };
            string[] states = { "Untidy", "Pristine", "Sterile", "Cramped", "Spacious", "Cavernous", "Biohazard", "Organized", "Cluttered", "Feng Shui" };
            string[] unusuals = { "Hydroponics", "Scavenged Furniture", "Holograms", "Monochromatic", "Artificial Plants", "Strange Acoustics", "Weird Smell", "Remote Assistants", "Legacy Tech", "Exotic Pet" };
            string[] secrets = { "Cameras", "Audio Surveillance", "Privacy Screen", "Air-Gapped LAN", "Hidden Room", "Escape Route", "Custodian AI", "Weapon Sensor", "Weapons Cache", "Hidden Stash" };

            string style = styles[rand.Next(0, styles.Length)];
            string state = states[rand.Next(0, states.Length)];
            string unusual = unusuals[rand.Next(0, unusuals.Length)];
            string secret = secrets[rand.Next(0, secrets.Length)];

            output += " The interior style is " + state + " " + style + ". If it's got an unusual interior feature, it's the " + unusual + ". Someone with inside knowledge might know about the " + secret + ".";

            await ReplyAsync(output);
        }

        [Command("job")]
        public async Task JobAsync(string type = "") {
            var userInfo = Context.User;
            string output = "<@" + userInfo.Id + ">:\nPlease add either 'person' or 'thing' as an additional argument to use this generator.";

            string[] titles = {
            "Street Gang Member", "Corporate Aristocrat", "Pimp", "Thug", "Prostitute", "Fixer", "Priest", "Business Owner", "Solo or Mercenary", "Hacker",
            "Scientist", "Cop", "Mobster", "Smuggler", "Bounty Hunter", "Syndicate Boss", "Concubine", "Tech Specialist", "Soldier", "Scavenger", "Agent",
            "Doctor", "Drug Dealer", "Celebrity", "Artifical Intelligence", "Artist", "Thief", "Media", "Nomad", "Synthetic", "Driver", "Child", "Broker",
            "Unemployed Person", "Clone", "Programmer", "Designer", "Homeless Person", "People Trafficker", "Revolutionary", "Psychiatrist", "Cyborg",
            "Intelligent Animal", "Courier", "Image Consultant", "Forger", "Ex-Con", "Fanatic/Extremist", "Performer", "Junkie"
        };

            string[] desires = {
            "wants to", "needs to", "must", "plans to", "forced to"
        };

            string[] actionsPeople = {
            "kill", "maim", "deliver to", "protect", "intimidate", "escape", "monitor", "smuggle", "find", "blackmail",
            "steal from", "collect from", "pay", "assist", "modify", "record", "threaten", "kidnap", "own", "defeat",
            "ruin", "control", "save", "submit to", "entrap", "con", "flee with", "employ", "marry", "sell out",
            "extract", "kill", "deliver to", "blackmail", "modify", "escape", "ruin", "steal from", "kidnap",
            "find", "escort", "deliver to", "save", "collect from", "flee", "kill", "sell out", "investigate", "submit to"
        };

            string[] actionsThing = {
            "destroy", "copy", "deliver", "protect", "sell", "steal", "destroy", "smuggle", "locate", "hide",
            "steal", "collect", "receive", "control", "modify", "locate", "destroy", "ransom", "own", "flee with",
            "spoil", "control", "save", "upload", "protect", "use", "flee with", "sell", "steal", "flee with",
            "locate", "destroy", "copy", "steal", "hack into", "escape with", "destroy", "protect", "locate", "design",
            "buy", "protect", "own", "steal", "hide", "sell", "deliver", "save", "copy", "steal"
        };

            string[] targetItems = {
            "neural processor", "vintage wine", "photograph(s)", "IFF tags", "narcotic", "weapon", "ID card", "jewelry", "software", "security passcard",
            "target's DNA", "cybermodem", "offline digital files", "hard drive", "designer virus", "attache case", "data/vid chip", "holdall of drugs", "vehicle", "keys/key card",
            "computer virus", "cybernetic limb", "synthetic brain", "personality module", "cell phone/agent", "exo-womb", "hardcopy schematic", "military ICE breaker", "nano fabricator", "antique katana",
            "cloned coca leaf", "antidote/medicine", "human eye/thumb", "artificial intelligence", "cybernetic optics", "SimStim recording", "robot", "operating system", "tablet device", "memory chip",
            "server", "holdall of cash", "bioware", "augmented pet", "chemical", "human organ(s)", "patient in cryo vat", "work of art", "drone/remote", "cybernetic implant"
        };


            if (type == "person") {
                string title = titles[rand.Next(0, titles.Length)];
                string desire = desires[rand.Next(0, desires.Length)];
                string action = actionsPeople[rand.Next(0, actionsPeople.Length)];
                string target = titles[rand.Next(0, titles.Length)];

                if (action == "collect from" || action == "steal from" || action == "deliver to") {
                    string[] splitAction = action.Split(' ');

                    action = splitAction[0] + " " + targetItems[rand.Next(0, targetItems.Length)] + " " + splitAction[1];

                }

                output = "<@" + userInfo.Id + ">:\n" + title + " " + desire + " " + action + " " + target + ".";
            } else if (type == "thing") {
                string title = titles[rand.Next(0, titles.Length)];
                string desire = desires[rand.Next(0, desires.Length)];
                string action = actionsThing[rand.Next(0, actionsThing.Length)];
                string target = targetItems[rand.Next(0, targetItems.Length)];

                output = "<@" + userInfo.Id + ">:\n" + title + " " + desire + " " + action + " " + target + ".";
            }


            await ReplyAsync(output);
        }


        [Command("gang")]
        public async Task GangAsync() {
            var userInfo = Context.User;
            string output = "<@" + userInfo.Id + ">:\n";


            string[] adjectives = {
            "Dub", "Radical", "Binary", "Fragile", "Hateful", "Electric", "Mobile", "Bubblegum", "Biological", "Shaolin", "Chrome",
            "Polymer", "Rude", "Terminal", "Shadow", "Subsonic", "Chemical", "Chosen", "Toxic", "Steel", "Cannibal", "Panzer", "Gun",
            "Disposable", "Iron", "Speedball", "Liquid", "Junky", "Instant", "Zoner", "Sushi", "Lucifer", "Faceless", "Vampire", "Crosstown",
            "Aryan", "Spirit", "Nomad", "Hydraulic", "Fractal", "Brainy", "Screaming", "Lucid", "Burning", "Euphoric", "Melancholic", "Randy",
            "Nasty", "Spiteful", "Popular", "Basic", "Difficult", "United", "Historic", "Hot", "Mental", "Scared", "Old", "Classic", "Actual",
            "Impossible", "Serious", "Technical", "Typical", "Critical", "Global", "Relevant", "Accurate", "Dangerous", "Capable", "Dangerous",
            "Dramatic", "Foreign", "Severe", "Psycho", "Pure", "Aggressive", "Strict", "Desperate", "Angry", "Lucky", "Ugly", "Sexual", "Terrible",
            "Confident", "Guilty", "Crazy", "Insane", "Ultimate", "Machine", "Killer", "Chaotic", "Lawful", "Neutral", "Evil", "Good", "Skilled",
            "Unique"
        };

            string[] nouns = {
            "Dogs", "Assassins", "Dragons", "Prophets", "Machines", "Society", "Freaks", "Girls", "Boys", "Militia", "Atrocity", "Impulse", "Rippers",
            "Savages", "Riot", "Apocalypse", "Storm", "Soldiers", "Cult", "Kidz", "Ghosts", "Church", "Technicals", "Clowns", "Fists", "Sharks",
            "Maniacs", "Ultimates", "Legends", "Killaz", "Bullets", "Revolvers", "Bosses", "Thugs", "Babies", "Army", "Fanatics", "Daddies",
            "Chaos", "Hammer", "Method", "Clan", "Terror", "State", "Zoners", "Shards", "Wasters", "Moderns", "Harvest", "Losers", "Skaters",
            "Posse", "Techiez", "Gangers", "Sons", "Daughters", "Uncles", "Overlords", "Artists", "Runners", "Corpos", "Chummers", "Homeless"
        };

            string[] descriptors = {
            "Neo-Primitive", "VR Game Playing", "Neon-Punks", "Drone Utilising",
            "LAN-Linked", "Augment Heavy", "Spike Covered", "Ex-Convicts", "Biomodded", "Blade Wielding", "Body Modified",
            "Gun Fetishists", "Rastafarians", "SimStim Rigged", "Tech-Ninjas", "Mood Chippers",
            "Music Fanatics", "Grime-Punks", "Skinheads", "Blood Stained", "Military Surplus", "Risk Takers",
            "Ultraviolent", "Drug Dependent", "War Painted", "Buddha-Faced", "Emo-Goths", "Goggle Wearing", "Androgynous",
            "Blood Drinkers", "Heavily Tattooed", "Neo-Luddites", "Goth-Punks", "Tech Savvy",
            "Net Dependent", "Radicals", "Afro-Haired", "Neon-Ravers", "Wireheads", "Nerve Boosted", "Scavengers",
            "Info-Socialists", "Sports Fans", "Doom Cultists", "Transgender", "Brain Damaged", "Skill Chippers",
            "Dog-Faced", "Racist", "Media Savvy", "Traceurs", "Alcoholics", "Drug Cooks", "Political", "Tech-Junkies",
            "Evangelicals", "Juggalos", "Cyborgs", "Psychotic", "War Veterans", "Rich Kids", "Drug Enhanced", "Home Invaders",
            "Cannibals", "Suicidal", "Homeless", "Pseudo-Satanic", "Militant", "Mask-Wearing", "Asexual", "Anarchists",
            "Alt-Right", "Religious", "Eco-Socialists", "Philosphical", "Anti-Corporate", "Pro-Corporate"
        };

            output += "The " + adjectives[rand.Next(0, adjectives.Length)] + " " + nouns[rand.Next(0, nouns.Length)];

            await ReplyAsync(output);
        }

        [Command("person")]
        public async Task PersonAsync() {
            var userInfo = Context.User;
            string output = "<@" + userInfo.Id + ">:\n";

            string[] traitsTable = {
            "Birthmark*", "Birthmark*", "Body piercings*", "Body piercings*", "Body piercings*", "Chews tobacco", "Scarred*", "Scarred*", "Scarred*", "Scarred*", "Scarred*",
            "Scarred*", "Scarred*", "Smokes", "Smokes", "Smokes", "Smokes", "Tattooed*", "Tattooed*", "Tattooed*",
            "Allergic to food/dust/pollen/animals", "Always arrives late", "Always gives vaguest possible answer", "Always has something in hands", "Always wears as little as possible",
            "Always wears expensive clothes", "Always wears same color", "Always wears tattered clothes", "Answers questions with questions", "Aversion to certain kind of food",
            "Bad breath or strong body odor", "Bad/loud/annoying/shrill laugh", "Bad with money", "Believes all animals can talk to each other", "Bites fingernails", "Bites lips",
            "Black eye", "Bleeding nose", "Blinks constantly", "Bruises easily",
            "Burps", "Burn scar*", "Chews with mouth open", "Chortles", "Clicks tongue", "Collects teeth/hair/claws of slain opponents", "Constantly asks for divine advice",
            "Covered in sores, boils, or a rash", "Cracks knuckles", "Dandruff", "Dirty", "Distinctive jewelry", "Distracted easily during conversation",
            "Double-checks everything", "Drones on and on while talking", "Easily confused", "Enjoys own body odor", "Exaggerates", "Excessive body hair", "Fidgets",
            "Finishes others' sentences", "Flatulent", "Flips a coin", "Foams at mouth when excited/angry", "Freckled", "Gesticulates wildly", "Giggles", "Grins evilly",
            "Hands shake", "Hacking cough", "Has nightmares", "Hates animals", "Hates children", "Hates quiet pauses in conversations", "Hiccoughs", "Hook for a hand",
            "Hums", "If unable to recall word, stops conversation and will not give up until can finally remember it", "Imaginary friend", "Interrupts others",
            "Jumps conversation topics", "Laughs at own jokes", "Lazy eyed", "Leers", "Likes to arm wrestle", "Limps", "Loves animals", "Loves children", "Loves the sea and ships",
            "Makes up words", "Mispronounces names", "Missing finger", "Mutters", "Needs story before sleeping", "Nervous cough", "Nervous eye twitch", "Nervous muscle twitch",
            "Paces", "Peg-legged", "Perfumed",
            "Picks fights", "Picks at fingernails", "Picks nose", "Picks scabs", "Picks at teeth", "Plays practical jokes", "Plays with hair", "Plays with own jewelry",
            "Pokes/taps others with finger", "Predilection for certain kind of food", "Prefers to be called by last name", "Puts garlic on all food",
            "Reads constantly, especially when inappropriate", "Refuses to let anyone walk behind them", "Refuses to sit in chairs", "Repeats same phrase over and over",
            "Rolls eyes when bored/annoyed", "Scratches", "Sharpens weapon", "Shivers",
            "Sings", "Sleeps late", "Sleeps nude", "Smiles when angry/annoyed", "Sneers", "Sneezes", "Sniffles", "Spits", "Squeamish", "Stands very close", "Stares",
            "Sucks teeth", "Sun-burned", "Swears profusely", "Sweaty", "Talks about self in third-person", "Talks to inanimate objects", "Talks to self", "Talks with food in mouth",
            "Taps feet",
            "Taps fingers", "Taunts foes", "Thinks they are very lucky", "Thinks they can speak a language they can't", "Tone-deaf", "Touches people while talking to them",
            "Turns every conversation into a story about themselves", "Unable to figure out which color clothes match", "Unable to let a joke die", "Unable to remember names",
            "Unexplained dislike for certain organization", "Urinates frequently", "Uses wrong word and refuses to acknowledge correct word", "Warts",
            "Wears flamboyant or outlandish clothes", "Wears hat or hood", "Wears only jewelry of one type of metal", "Wets bed", "Whistles",
            "Achluophobic (afraid of darkness)", "Agoraphobic (afraid of crowds)", "Altophobic (afraid of heights)", "Claustrophobic (afraid of small spaces)", "Drools",
            "Entomophobic (afraid of insects)", "Excessively clean", "Facial tic", "Haphephobic (afraid of being touched)", "Hallucinates", "Hemaphobic (afraid of blood)",
            "Hydrophobic (afraid of water)", "Insomniac", "Narcoleptic", "Pathological liar", "Picks at lint or dirt on others' clothes", "Obsessive gambler",
            "Ophidiophobic (afraid of snakes)", "Ornithophobic (afraid of birds)", "Short attention span"
        };

            string[] personalityTable = {
            "Accusative", "Active", "Adventurous", "Affable", "Aggressive", "Agreeable", "Aimless", "Aloof", "Altruistic", "Analytical",
            "Angry", "Animated", "Annoying", "Anxious", "Apathetic", "Apologetic", "Apprehensive", "Argumentative", "Arrogant", "Articulate",
            "Attentive", "Bigoted", "Bitter", "Blustering", "Boastful", "Bookish", "Bossy", "Braggart", "Brash", "Brave",
            "Bullying", "Callous", "Calm", "Candid", "Cantankerous", "Capricious", "Careful", "Careless", "Caring", "Casual",
            "Catty", "Cautious", "Cavalier", "Charming", "Chaste", "Chauvinistic", "Cheeky", "Cheerful", "Childish", "Chivalrous",
            "Clueless", "Clumsy", "Cocky", "Comforting", "Communicative", "Complacent", "Condescending", "Confident", "Conformist", "Confused",
            "Conscientous", "Conservative", "Contentious", "Contrary", "Contumely", "Conventional", "Cooperative", "Courageous", "Courteous", "Cowardly",
            "Coy", "Crabby", "Cranky", "Critical", "Cruel", "Cultured", "Curious", "Cynical", "Daring", "Deceitful",
            "Deceptive", "Defensive", "Defiant", "Deliberate", "Deluded", "Depraved", "Discreet", "Discreet", "Dishonest", "Disingenuous",
            "Disloyal", "Disrespectful", "Distant", "Distracted", "Distraught", "Docile", "Doleful", "Dominating", "Dramatic", "Drunkard",
            "Dull", "Earthy", "Eccentric", "Elitist", "Emotional", "Energetic", "Enthusiastic", "Epicurean", "Excited",
            "Expressive", "Extroverted", "Faithful", "Fanatical", "Fastidious", "Fatalistic", "Fearful", "Fearless", "Feral", "Fierce",
            "Feisty", "Flamboyant", "Flippant", "Flirtatious", "Foolhardy", "Foppish", "Forgiving", "Friendly", "Frightened", "Frivolous",
            "Frustrated", "Funny", "Furtive", "Generous", "Genial", "Gentle", "Gloomy", "Goofy", "Gossip", "Graceful",
            "Gracious", "Grave", "Gregarious", "Grouchy", "Groveling", "Gruff", "Gullible", "Happy", "Harsh", "Hateful",
            "Helpful", "Honest", "Hopeful", "Hostile", "Humble", "Humorless", "Humorous", "Idealistic", "Idiosyncratic", "Imaginative",
            "Imitative", "Impatient", "Impetuous", "Implacable", "Impractical", "Impulsive", "Inattentive", "Incoherent", "Indifferent", "Indiscreet",
            "Individualist", "Indolent", "Indomitable", "Industrious", "Inexorable", "Inexpressive", "Insecure", "Insensitive", "Instructive", "Intolerant",
            "Intransigent", "Introverted", "Irreligious", "Irresponsible", "Irreverent", "Irritable", "Jealous", "Jocular", "Joking", "Jolly",
            "Joyous", "Judgemental", "Jumpy", "Kind", "Know-it-all", "Languid", "Lazy", "Lethargic", "Lewd", "Liar",
            "Likable", "Lippy", "Listless", "Loquacious", "Loving", "Loyal", "Lust", "Madcap", "Magnanimous", "Malicious",
            "Maudlin", "Mean", "Meddlesome", "Melancholy", "Melodramatic", "Merciless", "Merry", "Meticulous", "Mischievous", "Miscreant",
            "Miserly", "Modest", "Moody", "Moralistic", "Morbid", "Morose", "Mournful", "Mousy", "Mouthy", "Mysterious",
            "Naive", "Narrow-minded", "Needy", "Nefarious", "Nervous", "Nettlesome", "Neurotic", "Noble", "Nonchalant", "Nurturing",
            "Obdurate", "Obedient", "Oblivious", "Obnoxious", "Obsequious", "Obsessive", "Obstinate", "Obtuse", "Odd", "Ornery",
            "Optimistic", "Organized", "Ostentatious", "Outgoing", "Overbearing", "Paranoid", "Passionate", "Pathological", "Patient", "Peaceful",
            "Pensive", "Pertinacious", "Pessimistic", "Philanderer", "Philosophical", "Phony", "Pious", "Playful", "Pleasant", "Poised",
            "Polite", "Pompous", "Pondering", "Pontificating", "Practical", "Prejudiced", "Pretentious", "Preoccupied", "Promiscuous", "Proper",
            "Proselytizing", "Proud", "Prudent", "Prudish", "Prying", "Purile", "Pugnacious", "Quiet", "Quirky", "Racist", "Rascal", "Rash",
            "Realistic", "Rebellious", "Reckless", "Refined", "Repellant", "Reserved", "Respectful", "Responsible",
            "Restless", "Reticent", "Reverent", "Rigid", "Risk-taking", "Rude", "Sadistic", "Sarcastic", "Sardonic", "Sassy", "Savage", "Scared",
            "Scolding", "Secretive", "Self-effacing", "Selfish", "Selfless", "Senile", "Sensible", "Sensitive",
            "Sensual", "Sentimental", "Serene", "Serious", "Servile", "Sexist", "Sexual", "Shallow", "Shameful", "Shameless", "Shifty", "Shrewd",
            "Shy", "Sincere", "Slanderous", "Sly", "Smug", "Snobbish", "Sober", "Sociable",
            "Solemn", "Solicitous", "Solitary", "Solitary", "Sophisticated", "Spendthrift", "Spiteful", "Stern", "Stingy", "Stoic", "Stubborn",
            "Submissive", "Sultry", "Superstitious", "Surly", "Suspicious", "Sybarite", "Sycophantic", "Sympathetic", "Taciturn",
            "Tactful", "Tawdry", "Teetotaler", "Temperamental", "Tempestuous", "Thorough", "Thrifty", "Timid", "Tolerant", "Transparent", "Treacherous",
            "Troublemaker", "Trusting", "Truthful", "Uncommitted", "Understanding", "Unfriendly", "Unhinged", "Uninhibited", "Unpredictable",
            "Unruly", "Unsupportive", "Vague", "Vain", "Vapid", "Vengeful", "Vigilant", "Violent", "Vivacious", "Vulgar", "Wanton", "Wasteful", "Weary",
            "Whimsical", "Whiny", "Wicked", "Wisecracking", "Wistful", "Witty", "Zealous"
        };

            string[] speechTypes = {
            "Accented", "Breathless", "Crisp", "Fast", "Guttural", "Halting", "Husky", "Lisps", "Low-pitched", "Loud",
            "Nasal", "Nervous", "Raspy", "Slow", "Slurs", "Squeaky", "Stutters", "Wheezy", "Whiny", "Whispery"
        };

            string[] hairTypes = {
            "Bald", "Braided", "Curly", "Dreadlocks", "Frazzled", "Greasy", "Limp", "Long", "Messy", "Strange Hairstyle",
            "Pony-tail", "Short", "Straight", "Streaked", "Thick", "Thinning", "Very Long", "Wavy", "Well Groomed", "Wiry"
        };


            await ReplyAsync(output);
        }




        //[Command("building")]
        //public async Task BuildingAsync() {
        //    var userInfo = Context.User;
        //    string output = "<@" + userInfo.Id + ">:\n";


        //    await ReplyAsync(output);
        //}
    }
}