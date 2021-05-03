using System;
using NeonBot.obj;

namespace NeonBot {
    public class LootGenFunctions {
        public static string GetColor() {
            Random rand = new Random();

            string[] colorsBlack = {
                "bistre", "black", "black bean", "black olive", "café noir", "charcoal", "ebony", "eigengrau", "jet", "licorice", "onyx", "outer space", "raising black", "smokey black"
            };

            string[] colorsBlue = {
                "azure", "baby blue", "bice blue", "blue", "blue-gray", "barandeis blue", "byzantine blue", "capri", "carolina blue", "cerulean",
                "cobalt blue", "cornflower blue", "cyan", "dark blue", "dodger blue", "egyptian blue", "electric blue", "federal blue",
                "electric indigo", "ice blue", "indigo", "iris", "light blue", "midnight blue", "navy blue", "neon blue", "periwinkle",
                "powder blue", "robin egg blue", "royal blue", "sapphire", "sky blue", "slate blue", "steel blue", "teal blue", "turquoise", "ultramarine",
                "vivid sky blue", "zaffre"
            };

            string[] colorsBrown = {
                "auburn", "almond", "beaver", "bistre", "bronze", "brown", "brown sugar", "burgundy", "burnt sienna", "burnt umber", "camel", "caramel",
                "chamoisee", "chestnut", "chocolate", "citron", "cocoa brown", "coffee", "copper", "cordovan", "coyote", "desert sand", "drab dark brown",
                "earth yellow", "ecru", "fallow", "fawn", "field drab", "fulvous", "golden brown", "goldenrod", "harvest gold", "khaki", "lion", "liver",
                "mahogany", "maroon", "ochre", "olive", "raw umber", "redwood", "rufous", "russet", "rust", "sand", "sandy brown", "seal brown", "sepia",
                "sienna", "sinopia", "tan", "taupe", "tawny", "umber", "walnut brown", "wenge", "wheat"
            };

            string[] colorsGrey = {
                "grey", "ash grey", "battleship grey", "blue-grey", "cadet grey", "cinerous", "cool grey", "dim gray", "marengo", "nickel", "gunmetal",
                "platinum", "silver", "slate grey", "taupe", "purple taupe", "medium taupe", "rose quartz", "timberwolf", "white smoke"
            };

            string[] colorsGreen = {
                "apple green", "aquamarine", "asparagus", "avocado", "bright green", "celadon", "chartreuse", "dark green", "dark moss green", "dark pastel green",
                "dark spring green", "emerald", "erin", "feldgrau", "fern green", "green", "green-yellow", "harlequin", "honeydew", "hunter green", "jade green",
                "jungle green", "lawn green", "light green", "lime", "lime green", "mantis", "malachite", "mint", "neon green", "olive", "olivine", "pear",
                "pistachio", "sea green", "shamrock green", "spring green", "tea green", "teal", "viridian", "yellow-green"
            };

            string[] colorsOrange = {
                "amber", "apricot", "atomic tangerine", "bittersweet", "buff", "burnt orange", "butterscotch", "caramel", "carrot orange", "champagne",
                "citron", "coral", "dark salmon", "deep carrot orange", "flame", "gamboge", "gold", "metallic gold", "golden poppy", "mango", "marigold",
                "old gold", "orange", "orange-red", "orange peel", "peach", "peach-orange", "peach-yellow", "persimmon", "pumpkin", "rust", "salmon",
                "scarlet", "seashell", "sunglow", "sunset", "tangelo", "tangerine", "tea rose", "tomato", "vermillion"
            };

            string[] colorsPink = {
                "amaranth pink", "blush", "brilliant rose", "carnation pink", "cerise", "cherry blossom pink", "coral pink", "deep pink", "french rose",
                "fuchsia", "heliotrope", "hot magenta", "hot pink", "lavender pink", "magenta", "orchid pink", "pink", "raspberry", "red-violet", "rose",
                "rose pink", "salmon pink", "shocking pink", "ultra pink"
            };

            string[] colorsPurple = {
                "amethyst", "blue-violet", "byzantium", "dark purple", "dark violet", "electric purple", "electric violet", "eminence purple", "grape",
                "lavender", "lilac", "mauve", "mulberry", "orchid", "purple", "royal purple", "violet", "wisteria"
            };

            string[] colorsWhite = {
                "alabaster", "beige", "blond", "bone", "cream", "eggshell", "flax", "ivory", "magnolia", "mint cream", "misty rose", "peach", "pearl", "snow", "vanilla", "white"
            };

            string[] colorsYellow = {
                "amber", "citrine", "dark goldenrod", "ecru", "jasmine", "maize", "mustard", "saffron", "straw", "sunglow", "sunset", "yellow"
            };

            string[] colorCats = {"Black", "Blue", "Green", "Brown", "Grey", "Orange", "Pink", "Purple", "White", "Yellow"};
            string colorCat = colorCats[rand.Next(0, colorCats.Length)];

            if (colorCat == "Black") { return colorsBlack[rand.Next(0, colorsBlack.Length)]; }
            if (colorCat == "Blue") { return colorsBlue[rand.Next(0, colorsBlue.Length)]; }
            if (colorCat == "Brown") { return colorsBrown[rand.Next(0, colorsBrown.Length)]; }
            if (colorCat == "Grey") { return colorsGrey[rand.Next(0, colorsGrey.Length)]; }
            if (colorCat == "Green") { return colorsGreen[rand.Next(0, colorsGreen.Length)]; }
            if (colorCat == "Orange") { return colorsOrange[rand.Next(0, colorsOrange.Length)]; }
            if (colorCat == "Pink") { return colorsPink[rand.Next(0, colorsPink.Length)]; }
            if (colorCat == "Purple") { return colorsPurple[rand.Next(0, colorsPurple.Length)]; }
            if (colorCat == "White") { return colorsWhite[rand.Next(0, colorsWhite.Length)]; }
            if (colorCat == "Yellow") { return colorsYellow[rand.Next(0, colorsYellow.Length)]; }

            return "None";
        }


        public static GarmentMat GetGarmentMat() {
            Random rand = new Random();
            string col = GetColor();

            switch (rand.Next(0, 36)) {
                case 0:
                case 1:
                case 2:
                    return new GarmentMat("plain weave", 0, "", col);
                case 3:
                case 4:
                case 5:
                    return new GarmentMat("basket weave", 0.1f, "", col);
                case 6:
                case 7:
                case 8:
                    return new GarmentMat("twill", 0.1f, "", col);
                case 9:
                case 10:
                case 11:
                    return new GarmentMat("cambric", 0.1f, "", col);
                case 12:
                case 13:
                case 14:
                    return new GarmentMat("canvas", 0.1f, "", col);
                case 15:
                case 16:
                case 17:
                    return new GarmentMat("gauze", 0.3f, "", col);
                case 18:
                case 19:
                    return new GarmentMat("grogram", 0.5f, "", col);
                case 20:
                    return new GarmentMat("crinoline", 0.1f, "", col);
                case 21:
                    return new GarmentMat("taffeta", 4, "", col);
                case 22:
                    return new GarmentMat("chiffon", 0.5f, "", col);
                case 23:
                    return new GarmentMat("double weave", 15, "", col);
                case 24:
                    return new GarmentMat("organdy", 0, "", col);
                case 25:
                    return new GarmentMat("organza", 2.5f, "", col);
                case 26:
                    return new GarmentMat("satin", 14, "", col);
                case 27:
                    return new GarmentMat("sateen", 10, "", col);
                case 28:
                    return new GarmentMat("brocade", 16, "", col);
                case 29:
                    return new GarmentMat("damask", 20, "", col);
                case 30:
                    return new GarmentMat("lamé", 22, "", col);
                case 31:
                    return new GarmentMat("jamdani", 4, "", col);
                case 32:
                    return new GarmentMat("velvet", 0.5f, "", col);
                case 33:
                    return new GarmentMat("corduroy", 0.75f, "", col);
                case 34:
                    return new GarmentMat("double-velvet", 8, "", col);
                case 35:
                    return new GarmentMat("samite", 5, "", col);
                default:
                    return new GarmentMat("plain weave", 0, "", col);

            }
        }

        public static GarmentMat GetImplausibleMat() {
            Random rand = new Random();


            switch (rand.Next(0, 33)) {
                case 0:
                    return new GarmentMat("Blood", 3, "Deep red, verging on black in places, with a faint-but-distinct coppery smell. Gives +1 to scent-based Tracking rolls to follow the owner, and -2 to reactions from just about everybody. Conductive.");
                case 1:
                    return new GarmentMat("Bone", 1, "White, or possibly slightly off-white, and somewhat porous. People using bone items suffer -1 to reactions from everyone but necromancers.");
                case 2:
                    return new GarmentMat("Cloud", 5, "The item is a soft, swirling, mottled gray, and marginally translucent; very strong light sources are just barely visible through it, at least around the edges. It always feels slightly damp. Garments and armor made out of this material count as “wet clothes” for exposure to cold (p. B430), but give +1 to heat-based HT rolls. Conductive.");
                case 3:
                    return new GarmentMat("Mist", 5, "The item is a soft, swirling, mottled gray, and marginally translucent; very strong light sources are just barely visible through it, at least around the edges. It always feels slightly damp. Garments and armor made out of this material count as “wet clothes” for exposure to cold (p. B430), but give +1 to heat-based HT rolls. Conductive.");
                case 4:
                    return new GarmentMat("Darkness", 2, "The item is pure black, reflecting no light whatsoever. Fine surface features like engraving are almost impossible to see if the object is kept clean.");
                case 5:
                    return new GarmentMat("Flame", 4, "Hot to the touch (though not so much so as to be damaging), the item sheds light as a torch. A garment or armor made from flame gives +2 to cold-based HT rolls, -2 to heatbased ones.");
                case 6:
                    return new GarmentMat("Flower Petals", 2, "In addition to being colorful and sweet-smelling, the object is slightly soft and velvety to the touch. The distinct aroma gives +1 to attempts to track the owner by scent, but someone with prominently displayed flower-petal items gets +1 to reactions in addition to any bonuses granted by ornate equipment. Flammable.");
                case 7:
                    return new GarmentMat("Horn", 1, "Smooth, if slightly porous, with colors ranging from ivory to a medium brown.");
                case 8:
                    return new GarmentMat("Everfrost", 4, "White or faintly blue, and cold to the touch (don’t lick the item!). A garment or armor made from ice gives -2 to coldbased HT rolls, +2 to heat-based ones. Conductive.");
                case 9:
                    return new GarmentMat("Insects", 1, "Made from clusters of interlocked insect exoskeletons, chitin items range from a dull gray-green to a rainbow sheen like an oil slick on water, which is attractive but very creepy (-2 to reactions from most people). Some items are made from stinging insects. Weapons made from such creatures cause an additional -1 in shock penalties when they inflict injury but do no extra damage.");
                case 10:
                    return new GarmentMat("Leaves", 1, "The object is made from overlapping layers of greenery. Some items may turn colors with the seasons. Items made from leaves are Flammable (p. B433) when brown but Highly Resistant when green.");
                case 11:
                    return new GarmentMat("Lightning", 4, "Silvery, but with shuddering edges, a faint crackling noise, and a slight scent of ozone. Lightning items cast a flickering glow equivalent to candlelight. However, the jarring flashes make them uncomfortable to use: -1 to long-term tasks such as reading or working with machinery while using such an object as a light source. Conductive.");
                case 12:
                    return new GarmentMat("Moonbeams", 3, "Through the course of a month, the item goes from being a pale, glowing gray (about the light of a candle) to near-black and back again.");
                case 13:
                    return new GarmentMat("Night", 2, "As with darkness, but a slowly shifting array of stars makes surface features a bit easier to make out.");
                case 14:
                    return new GarmentMat("Quicksilver", 16, "The item is mirror-smooth and silvery, but ripples faintly when disturbed. Conductive");
                case 15:
                    return new GarmentMat("Screams", 1, "Translucent, if slightly cloudy. The item always seems to be vibrating slightly, and can be heard if one puts an ear very close to it.");
                case 16:
                    return new GarmentMat("Sea Foam", 3, "Translucent, milky pale green or turquoise, with roiling whitish areas. The item smells faintly of the sea and has a salty taste. Like clouds and mist, garments and armor made out of this material count as “wet clothes” for exposure to cold (p. B430), but give +1 to heat-based HT rolls. Conductive.");
                case 17:
                    return new GarmentMat("Coarse Shell", -0.1f, "Very rough, dull-colored seashell resembling the outside of a clam or an oyster. Such items have a faint odor of the sea, making them slightly easier to track (+1 to scent-based Tracking rolls) away from coastal areas.");
                case 18:
                    return new GarmentMat("Fine Shell", 4, "A smooth, mother-of-pearl surface, with rainbow colors on a white or slightly silvery ground.");
                case 19:
                    return new GarmentMat("Sky", 1, "Usually a medium blue, but growing lighter or darker from time to time, sometimes becoming an overcast gray. A few examples change to match their owner’s mood: clear and blue in happy times, dark and cloudy with unhappier moments. While impressive, this makes the owner’s moods easy enough to spot that others get +1 to any social skill used against him.");
                case 20:
                    return new GarmentMat("Smoke", 1.5f, "Resembles clouds, but a bit darker. The item also has a distinct scent of burning and leaves dark smudges on things it touches.");
                case 21:
                    return new GarmentMat("Sunlight", 5, "The item shines with a bright, golden light equivalent to a torch. It is pleasantly warm to the touch.");
                case 22:
                    return new GarmentMat("Tears", 1, "Like water, the item is fairly transparent. It is also warm to the touch – and, if one tastes it, salty. Conductive.");
                case 23:
                    return new GarmentMat("Teeth", 1, "Appears similar to bone and horn, though usually with a glossier surface and a pointed end.");
                case 24:
                    return new GarmentMat("Thorns", -0.1f, "The item is made from a thick tangle of thorny branches and vines. It feels prickly, though this causes no special damage. Flammable.");
                case 25:
                    return new GarmentMat("Water", 1, "Transparent and somewhat reflective, a close observer can see ripples through the item if it is struck. Garments and armor made out of this material count as “wet clothes” for exposure to cold (p. B430), but give +1 to heat-based HT rolls.");
                case 26:
                    return new GarmentMat("Joy", 2, "Mostly translucent, but with occasional ripples of colors that grow more frequent as nearby people get happier. Anyone who gets close enough will find that it smells faintly of their favorite scent.");
                case 27:
                    return new GarmentMat("Flesh", 2, "Made of a bloody mass of flesh. Gives +1 to scent-based Tracking rolls to follow the owner, and -2 to reactions from just about everybody. ");
                case 28:
                    return new GarmentMat("Cardboard", 2, "Looks as though it were made of discarded cardboard, while still somehow retaining all the physical properties of the items original material.");
                case 29:
                    return new GarmentMat("Lava", 4, "Deep red and orange liquid, with occasional bubbles coming to the surface and bursting. Can be held without causing damage, but is almost painfully hot. People sensitive to temperature take 1 damage per round that they are actively in contact with this object.");
                case 30:
                    return new GarmentMat("Bubbles", 1, "This object is made of soapy bubbles that are constantly popping and reforming randomly.");
                case 31:
                    return new GarmentMat("Bravery", 1, "This object confers a feeling of near-foolhardy invulnerability to the holder. No actual stat bonuses or toughness is included.");
                case 32:
                default:
                    return new GarmentMat("Love", 1, "Translucent with a soft pink glow. +1 on any reaction rolls made by someone you freely give this object to.");

            }
        }


        public static GeneralItem GetSpice() {
            Random rand = new Random();

            switch (rand.Next(0, 36)) {
                case 0: return new GeneralItem("allspice", 150);
                case 1: return new GeneralItem("anise", 150);
                case 2: return new GeneralItem("annatto", 113);
                case 3: return new GeneralItem("asafetida", 75);
                case 4: return new GeneralItem("cardamom", 150);
                case 5: return new GeneralItem("cassia", 75);
                case 6: return new GeneralItem("chiles", 38, "An ounce of this, ground to powder and scattered in the user's path, will make anyone tracking him by scent have a fit of sneezing (p B428). Afterward, the tracker must wait an hour or make an HT-3 roll to recover.");
                case 7: return new GeneralItem("cinnamon", 150, "Well-known aphrodisiac. Consuming an ounce imposes -1 on any rolls to resist Lecherousness and seduction attempts for the next hour.");
                case 8: return new GeneralItem("cloves", 150);
                case 9: return new GeneralItem("coriander", 150, "If an ounce of this is consumed within an hour before ingesting a poison, the user is at +1 to HT rolls to resist.");
                case 10: return new GeneralItem("cumin", 150);
                case 11: return new GeneralItem("dwarven savory fungus", 75, "Useful for strengthening the blood and speeding healing. Consuming an ounce a day gives +1 to daily HT rolls to recover lost HP.");
                case 12: return new GeneralItem("elven pepperbark", 38);
                case 13: return new GeneralItem("faerie glimmerseed", 270, "Highly prized, but the consumer of an ounce or more is at -1 to resist any mind-reading or mind-control attempts made in the next hour.");
                case 14: return new GeneralItem("fennel", 75);
                case 15: return new GeneralItem("fenugreek", 150);
                case 16: return new GeneralItem("ginger", 38, "Aids digestion; an ounce acts as treatment to resist nausea (p B428) for an hour.");
                case 17: return new GeneralItem("halfling savory", 150, "Useful for strengthening the blood and speeding healing. Consuming an ounce a day gives +1 to daily HT rolls to recover lost HP.");
                case 18: return new GeneralItem("huajiao (szechuan pepper)", 150, "An ounce of this, ground to powder and scattered in the user's path, will make anyone tracking him by scent have a fit of sneezing (p B428). Afterward, the tracker must wait an hour or make an HT-3 roll to recover.");
                case 19: return new GeneralItem("mace", 225);
                case 20: return new GeneralItem("mustard", 38);
                case 21: return new GeneralItem("nigella", 75, "Balances humors and helps stabilize mood. Consuming an ounce gives +1 to resist sudden bursts of anger and rage, including the Berserk and Bloodlust disadvantages, for an hour.");
                case 22: return new GeneralItem("nutmeg", 150);
                case 23: return new GeneralItem("onion seed", 38);
                case 24: return new GeneralItem("orcish firegrain", 150, "Very mild stimulant. Anyone who ingests an ounce is at +1 HT to resist poisons that cause unconsciousness or fatigue damage for the next hour.");
                case 25: return new GeneralItem("black pepper", 150, "An ounce of this, ground to powder and scattered in the user's path, will make anyone tracking him by scent have a fit of sneezing (p B428). Afterward, the tracker must wait an hour or make an HT-3 roll to recover.");
                case 26: return new GeneralItem("white pepper", 188, "An ounce of this, ground to powder and scattered in the user's path, will make anyone tracking him by scent have a fit of sneezing (p B428). Afterward, the tracker must wait an hour or make an HT-3 roll to recover.");
                case 27: return new GeneralItem("poppy seed", 38);
                case 28: return new GeneralItem("saffron", 300);
                case 29: return new GeneralItem("salt", 15, "Tossing an ounce of salt gives clerics +1 to cast Turn Zombie and to Will rolls for True Faith to turn zombies.");
                case 30: return new GeneralItem("black salt", 38, "Tossing an ounce of salt gives clerics +1 to cast Turn Zombie and to Will rolls for True Faith to turn zombies.");
                case 31: return new GeneralItem("red salt", 38, "Tossing an ounce of salt gives clerics +1 to cast Turn Zombie and to Will rolls for True Faith to turn zombies.");
                case 32: return new GeneralItem("sumac", 38);
                case 33: return new GeneralItem("tamarind", 15);
                case 34: return new GeneralItem("turmeric", 38, "Has antiseptic properties. Using an ounce of it while dressing wounds gives +1 to First Aid.");
                default: return new GeneralItem("zeodary", 150);
            }
        }

        public static GeneralItem GetFiber() {
            Random rand = new Random();
            GarmentMat imMat = GetImplausibleMat();
            GarmentMat regMat = GetGarmentMat();

            switch (rand.Next(0, 9)) {
                case 0: return new GeneralItem("Otherworldly Cloth (" + imMat.name + ")", (float)(200 * (1.0 + imMat.valueMod)), "100-square-foot bolt. Made of " + imMat.name + ". " + imMat.desc + ". No immediate magical effect, but can be very valuable in complex magical work; it gives +2 to Merchant skill if sold or traded to an enchanter or other dealer in magical items.", 7.5f);
                case 1:
                case 2:
                case 3: return new GeneralItem("Giant-Spider Silk Cloth", 565, "100-square-foot bolt. Very durable fabric, suitable for armor use.", 1);
                case 4:
                case 5:
                case 6:
                case 7:
                default: return new GeneralItem((regMat.color[0].ToString().ToUpper() + regMat.color.Substring(1)) + " " + (regMat.name[0].ToString().ToUpper() + regMat.name.Substring(1)) + " Cloth", (float)(5 * (1.0f + regMat.valueMod)), "100-square-foot bolt. Made of " + regMat.name + " cloth. " + regMat.desc, (float) (5 * (1.0 + regMat.weightMod)));
                
            }
        }
    }
}
