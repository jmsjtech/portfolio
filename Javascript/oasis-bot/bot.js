const Discord = require("discord.js");
const client = new Discord.Client();


const Enmap = require('enmap');
const EnmapLevel = require('enmap-level');

var fs = require('fs'), xml2js = require('xml2js');
var parser = new xml2js.Parser();
var dice = require('./parser.js');


//client.creditsTable = new Enmap({provider: new EnmapLevel({name:"creditsTable"})});
//client.grindClock = new Enmap({provider: new EnmapLevel({name:"grindClock"})});
//client.autoSessions = new Enmap({provider: new EnmapLevel({name:"autoSessions"})});
client.inventory = new Enmap({provider: new EnmapLevel({name:"inventory"})});
client.shopTable = new Enmap({provider: new EnmapLevel({name:"shopTable"})});
client.settlements = new Enmap({provider: new EnmapLevel({name:"settlements"})});
client.fullTable = new Enmap({provider: new EnmapLevel({name:"fullTable"})});

//client.creditsTable = new Enmap({name:"creditsTable"});

client.welcome = new Enmap({provider: new EnmapLevel({name:"welcome"})});

process.on('unhandledRejection', error => {
	console.log(error.message);
});

process.on('error', error => {
	console.log(error.message);
	return process.exit(1);
});


client.on("guildMemberRemove", member => {
	const channel = member.guild.channels.find('name', 'general');
  if (!channel) return;
  channel.send(`I've seen things you people wouldn't believe. Attack ships on fire off the shoulder of Orion. I watched C-beams glitter in the dark near the Tannhäuser Gate. I saw ${member} leave the OASIS. All those moments will be lost in time, like tears in rain. Time to die. `);
});

client.on("ready", () => {
	var startupTime = new Date();
	
	console.log("Ready Player One! (Started at " + startupTime.getHours() + ":" + startupTime.getMinutes() + ")");
	
	const datechannel = client.channels.find("name", "date-ticks");
	
	function dateTick() {
		if (!datechannel) return;
		if (!client.fullTable.has("autoSessionValue")) {
			client.fullTable.set("autoSessionValue", 0);
		}
		
		var autosessionVal = (Math.ceil(Math.random()*500) + 500);
			
		client.fullTable.set("autoSessionValue", autosessionVal);
		
		var shopBooks = [ "BasicSet", "HighTech", "UltraTech", "DFRPG", "LowTech" ];
		var theBook = shopBooks[Math.ceil(Math.random()*(shopBooks.length-1))];
		var announceBook = client.shopTable.get(theBook);
		
		var itemIndex = Math.ceil(Math.random()*(announceBook.items.length-1));
		var tempItem = announceBook.items[itemIndex];
		
		
		console.log("Date Ticked!");
		datechannel.send("@here It's a new day in the OASIS! Everything with limited Daily Uses, including Session Credits, are refreshed!\nAutosessions are worth " + autosessionVal + " today!\nToday's Item of the Day is '" + tempItem.name + "' from the `" + theBook + "` store! It only costs " + tempItem.itemPrice + " Credits, and you can buy yourself one by typing `!store buy " + theBook + " " + itemIndex + "`!");
		
		var ArrKeys = client.fullTable.keyArray();
		for(var i = 0; i < ArrKeys.length; i++) {
			if (client.fullTable.has(ArrKeys[i]) && ArrKeys[i] != "autoSessionValue") {
				var newData = client.fullTable.get(ArrKeys[i]);
				newData.autosessions = 4;
				client.fullTable.set(ArrKeys[i], newData);
			}
		}
		
	}
	
	
	var now = new Date();
	var millisTillMid = new Date(now.getFullYear(), now.getMonth(), now.getDate(), 0, 0, 0, 0) - now;
	if (millisTillMid < 0) {
		millisTillMid += 86400000; // it's after midnight, try midnight tomorrow.
	}
	
	function setDateTick() {
		interval = setInterval(dateTick, 24*60*60*1000);
		console.log("Date Tick Interval Started");
		dateTick();
	}
	
	setTimeout(setDateTick, millisTillMid);	
});

client.on('guildMemberAdd', member => {
  const channel = member.guild.channels.find('name', 'general');
  if (!channel) return;
  channel.send(`Welcome to The OASIS, ${member}! You can get a lot of the commonly requested information by going to ` + member.guild.channels.find('name', 'bot-interactions') + ` , and using the !help command. If that doesn't answer all the questions you have, either check out the Information category of channels or feel free to ask!`);
  });

function getColor() {

	var colors = [
		"Amber", "Avocado Green", "Black", "Blue", "Bronze", "Brown", "Charcoal", "Coppery", "Crimson", "Dark Brown", "Forest Green", "Golden",
		"Gray", "Green", "Indigo", "Lapis", "Lavender", "Light Gray", "Light Green", "Lime", "Maroon", "Olive", "Orange", "Pale Yellow",
		"Peach", "Pink", "Purple", "Red", "Sea Green", "Sky Blue", "Tan", "Turquoise", "Vermillion", "Violet", "White", "Yellow"
	];	
	var colorRand1 = Math.ceil(Math.random()*(colors.length-1));
	var color = colors[colorRand1];
	
	return color;
}

function getGarmentMaterial() {
	var materials = [
		"Plain Weave", "Plain Weave", "Plain Weave", "Basket Weave", "Basket Weave", "Basket Weave", "Twill", "Twill", "Twill", "Cambric", "Cambric", "Cambric", 
		"Canvas", "Canvas", "Canvas", "Gauze", "Gauze", "Gauze", "Grogram", "Grogram", "Crinoline", "Taffeta", "Chiffon", "Double Weave", "Organdy", "Organza",
		"Satin", "Sateen", "Brocade", "Damask", "Lamé", "Jamdani", "Velvet", "Corduroy", "Double-Velvet", "Samite"
	];
	
	var matRand1 = Math.ceil(Math.random()*(materials.length-1));
	var material = materials[matRand1];
	
	return material;
}

function getImplausibleMat() {
	var matRand1 = Math.ceil(Math.random()*43);
	var material = { "name":"", "valueMod":0, "weightMod":0, "desc":"" };
	
	if (matRand1 == 1) { material.name = "Basalt"; material.valueMod = 2; material.weightMod = 5; material.desc = "Made of a rough-faced gray stone, often in a pattern of hexagonal cells. Basalt items are very hard, and get +1 to rolls to resist breakage.";}
	if (matRand1 == 2) { material.name = "Blood"; material.valueMod = 3; material.weightMod = 0.9; material.desc = "Deep red, verging on black in places, with a faint-but-distinct coppery smell. Gives +1 to scent-based Tracking rolls to follow the owner, and -2 to reactions from just about everybody. Conductive.";}
	if (matRand1 == 3 || matRand1 == 4) { material.name = "Bone"; material.valueMod = 1; material.weightMod = 2; material.desc = "White, or possibly slightly off-white, and somewhat porous. People using bone items suffer -1 to reactions from everyone but necromancers.";}
	if (matRand1 == 5) { material.name = "Cloud"; material.valueMod = 5; material.weightMod = 0.1; material.desc = "The item is a soft, swirling, mottled gray, and marginally translucent; very strong light sources are just barely visible through it, at least around the edges. It always feels slightly damp. Garments and armor made out of this material count as “wet clothes” for exposure to cold (p. B430), but give +1 to heat-based HT rolls. Conductive.";}
	if (matRand1 == 6) { material.name = "Mist"; material.valueMod = 5; material.weightMod = 0.1; material.desc = "The item is a soft, swirling, mottled gray, and marginally translucent; very strong light sources are just barely visible through it, at least around the edges. It always feels slightly damp. Garments and armor made out of this material count as “wet clothes” for exposure to cold (p. B430), but give +1 to heat-based HT rolls. Conductive.";}
	
	if (matRand1 == 7) { material.name = "Darkness"; material.valueMod = 2; material.weightMod = 0.9; material.desc = "The item is pure black, reflecting no light whatsoever. Fine surface features like engraving are almost impossible to see if the object is kept clean.";}
	if (matRand1 == 8) { material.name = "Ebony"; material.valueMod = 2; material.weightMod = 2; material.desc = "A fine-grained, dark brown or black wood. Flammable.";}
	if (matRand1 == 9) { material.name = "Flame"; material.valueMod = 4; material.weightMod = 0.75; material.desc = "Hot to the touch (though not so much so as to be damaging), the item sheds light as a torch. A garment or armor made from flame gives +2 to cold-based HT rolls, -2 to heatbased ones.";}
	if (matRand1 == 10) { material.name = "Flower Petals"; material.valueMod = 2; material.weightMod = 0.75; material.desc = "In addition to being colorful and sweet-smelling, the object is slightly soft and velvety to the touch. The distinct aroma gives +1 to attempts to track the owner by scent, but someone with prominently displayed flower-petal items gets +1 to reactions in addition to any bonuses granted by ornate equipment. Flammable.";}
	if (matRand1 == 11) { material.name = "Horn"; material.valueMod = 1; material.weightMod = 1; material.desc = "Smooth, if slightly porous, with colors ranging from ivory to a medium brown.";}
	if (matRand1 == 12) { material.name = "Everfrost"; material.valueMod = 4; material.weightMod = 2; material.desc = "White or faintly blue, and cold to the touch (don’t lick the item!). A garment or armor made from ice gives -2 to coldbased HT rolls, +2 to heat-based ones. Conductive.";}
	
	if (matRand1 == 13 || matRand1 == 14) { material.name = "Insects"; material.valueMod = 1; material.weightMod = 0.75; material.desc = "Made from clusters of interlocked insect exoskeletons, chitin items range from a dull gray-green to a rainbow sheen like an oil slick on water, which is attractive but very creepy (-2 to reactions from most people). Some items are made from stinging insects. Weapons made from such creatures cause an additional -1 in shock penalties when they inflict injury but do no extra damage.";}
	if (matRand1 == 15 || matRand1 == 16) { material.name = "Leaves"; material.valueMod = 1; material.weightMod = 0.75; material.desc = "The object is made from overlapping layers of greenery. Some items may turn colors with the seasons. Items made from leaves are Flammable (p. B433) when brown but Highly Resistant when green.";}
	if (matRand1 == 17) { material.name = "Lightning"; material.valueMod = 4; material.weightMod = 0.75; material.desc = "Silvery, but with shuddering edges, a faint crackling noise, and a slight scent of ozone. Lightning items cast a flickering glow equivalent to candlelight. However, the jarring flashes make them uncomfortable to use: -1 to long-term tasks such as reading or working with machinery while using such an object as a light source. Conductive.";}
	if (matRand1 == 18) { material.name = "Marble"; material.valueMod = 3; material.weightMod = 5; material.desc = "Mottled stone – often white or in earth tones, but sometimes with a pink or blue tint – with a smooth polish. Marble items are particularly vulnerable to acids; where applicable, halve DR against such attacks.";}
	
	if (matRand1 == 19) { material.name = "Moonbeams"; material.valueMod = 3; material.weightMod = 0.75; material.desc = "Through the course of a month, the item goes from being a pale, glowing gray (about the light of a candle) to near-black and back again.";}
	if (matRand1 == 20) { material.name = "Night"; material.valueMod = 2; material.weightMod = 0.75; material.desc = "As with darkness, but a slowly shifting array of stars makes surface features a bit easier to make out.";}
	if (matRand1 == 21) { material.name = "Quicksilver"; material.valueMod = 16; material.weightMod = 0.5; material.desc = "The item is mirror-smooth and silvery, but ripples faintly when disturbed. Conductive";}
	if (matRand1 == 22) { material.name = "Sandstone"; material.valueMod = 2; material.weightMod = 3; material.desc = "Rough-surfaced, with earth-tone colors ranging from white to a deep brick red. Larger items have multiple strata of colors.";}
	if (matRand1 == 23) { material.name = "Screams"; material.valueMod = 1; material.weightMod = 0.75; material.desc = "Translucent, if slightly cloudy. The item always seems to be vibrating slightly, and can be heard if one puts an ear very close to it.";}
	if (matRand1 == 24) { material.name = "Sea Foam"; material.valueMod = 3; material.weightMod = 0.75; material.desc = "Translucent, milky pale green or turquoise, with roiling whitish areas. The item smells faintly of the sea and has a salty taste. Like clouds and mist, garments and armor made out of this material count as “wet clothes” for exposure to cold (p. B430), but give +1 to heat-based HT rolls. Conductive.";}
	
	if (matRand1 == 25) { material.name = "Coarse Shell"; material.valueMod = -0.1; material.weightMod = 1; material.desc = "Very rough, dull-colored seashell resembling the outside of a clam or an oyster. Such items have a faint odor of the sea, making them slightly easier to track (+1 to scent-based Tracking rolls) away from coastal areas.";}
	if (matRand1 == 26 || matRand1 == 27) { material.name = "Fine Shell"; material.valueMod = 4; material.weightMod = 0.9; material.desc = "A smooth, mother-of-pearl surface, with rainbow colors on a white or slightly silvery ground.";}
	if (matRand1 == 28) { material.name = "Sky"; material.valueMod = 1; material.weightMod = 0.9; material.desc = "Usually a medium blue, but growing lighter or darker from time to time, sometimes becoming an overcast gray. A few examples change to match their owner’s mood: clear and blue in happy times, dark and cloudy with unhappier moments. While impressive, this makes the owner’s moods easy enough to spot that others get +1 to any social skill used against him.";}
	if (matRand1 == 29) { material.name = "Smoke"; material.valueMod = 1.5; material.weightMod = 0.1; material.desc = "Resembles clouds, but a bit darker. The item also has a distinct scent of burning and leaves dark smudges on things it touches.";}
	if (matRand1 == 30) { material.name = "Sunlight"; material.valueMod = 5; material.weightMod = 0.9; material.desc = "The item shines with a bright, golden light equivalent to a torch. It is pleasantly warm to the touch.";}
	
	if (matRand1 == 31) { material.name = "Tears"; material.valueMod = 1; material.weightMod = 0.9; material.desc = "Like water, the item is fairly transparent. It is also warm to the touch – and, if one tastes it, salty. Conductive.";}
	if (matRand1 == 32) { material.name = "Teeth"; material.valueMod = 1; material.weightMod = 1; material.desc = "Appears similar to bone and horn, though usually with a glossier surface and a pointed end.";}
	if (matRand1 == 33 || matRand1 == 34) { material.name = "Thorns"; material.valueMod = -0.1; material.weightMod = 0.75; material.desc = "The item is made from a thick tangle of thorny branches and vines. It feels prickly, though this causes no special damage. Flammable.";}
	if (matRand1 == 35 || matRand1 == 36) { material.name = "Water"; material.valueMod = 1; material.weightMod = 0.5; material.desc = "Transparent and somewhat reflective, a close observer can see ripples through the item if it is struck. Garments and armor made out of this material count as “wet clothes” for exposure to cold (p. B430), but give +1 to heat-based HT rolls.";}
	
	if (matRand1 == 37) { material.name = "Joy"; material.valueMod = 2; material.weightMod = 0.75; material.desc = "Mostly translucent, but with occasional ripples of colors that grow more frequent as nearby people get happier. Anyone who gets close enough will find that it smells faintly of their favorite scent.";}
	if (matRand1 == 38) { material.name = "Flesh"; material.valueMod = 2; material.weightMod = 1; material.desc = "Made of a bloody mass of flesh. Gives +1 to scent-based Tracking rolls to follow the owner, and -2 to reactions from just about everybody. ";}
	if (matRand1 == 39) { material.name = "Cardboard"; material.valueMod = 2; material.weightMod = 0.25; material.desc = "Looks as though it were made of discarded cardboard, while still somehow retaining all the physical properties of the items original material.";}
	if (matRand1 == 40) { material.name = "Lava"; material.valueMod = 4; material.weightMod = 5; material.desc = "Deep red and orange liquid, with occasional bubbles coming to the surface and bursting. Can be held without causing damage, but is almost painfully hot. People sensitive to temperature take 1 damage per round that they are actively in contact with this object.";}
	if (matRand1 == 41) { material.name = "Bubbles"; material.valueMod = 1; material.weightMod = 0.1; material.desc = "This object is made of soapy bubbles that are constantly popping and reforming randomly.";}
	if (matRand1 == 42) { material.name = "Bravery"; material.valueMod = 1; material.weightMod = 0.9; material.desc = "Often confused at first glance for being made of stupidity, this object confers a feeling of near-foolhardy invulnerability to the holder. No actual stat bonuses or toughness is included.";}
	
	if (matRand1 == 43) { material.name = "Love"; material.valueMod = 1; material.weightMod = 0.9; material.desc = "Translucent with a soft pink glow. +1 on any reaction rolls made by someone you freely give this object to.";}
	
	return material;
}

function getMaterialValue(matName) {
	var matValue = 0;
	
	// Standard Materials
	if (matName == "Plain Weave") { matValue = 0; }
	if (matName == "Basket Weave") { matValue = 0.1; }
	if (matName == "Twill") { matValue = 0.1; }
	if (matName == "Cambric") { matValue = 0.1; }
	if (matName == "Canvas") { matValue = 0.1; }
	if (matName == "Gauze") { matValue = 0.3; }
	if (matName == "Grogram") { matValue = 0.5; }
	if (matName == "Crinoline") { matValue = 0.1; }
	if (matName == "Taffeta") { matValue = 4; }
	if (matName == "Chiffon") { matValue = 0.5; }
	if (matName == "Double Weave") { matValue = 15; }
	if (matName == "Organdy") { matValue = 0; }
	if (matName == "Organza") { matValue = 2.5; }
	if (matName == "Satin") { matValue = 14; }
	if (matName == "Sateen") { matValue = 10; }
	if (matName == "Brocade") { matValue = 16; }
	if (matName == "Damask") { matValue = 20; }
	if (matName == "Lamé") { matValue = 22; }
	if (matName == "Jamdani") { matValue = 4; }
	if (matName == "Velvet") { matValue = 0.5; }
	if (matName == "Corduroy") { matValue = 0.75; }
	if (matName == "Double-Velvet") { matValue = 8; }
	if (matName == "Samite") { matValue = 5; }
	
	return matValue;
}

function getGem() {
	var gem = { "name":"", "valueMod":0, "carats":0, "value":0 };
	var nextLoot1 = Math.ceil(Math.random()*6);
	var nextLoot2 = Math.ceil(Math.random()*6);
	
	var carat1 = Math.ceil(Math.random()*6);
	var carat2 = Math.ceil(Math.random()*6);
	var carats = (carat1 + carat2)/4;
	
	var loopIt = true;
	
	while (loopIt) {
		if (nextLoot1 == 1 && (nextLoot2 == 1 || nextLoot2 == 2)) {
			carats = carats + Math.ceil(Math.random()*6);
			
			var nextLoot1 = Math.ceil(Math.random()*6);
			var nextLoot2 = Math.ceil(Math.random()*6);
		}
		
		else {
			loopIt = false;
			
			if (nextLoot1 == 1 && nextLoot2 == 3) { gem.name = "Agate"; gem.valueMod = 5; }
			if (nextLoot1 == 1 && nextLoot2 == 4) { gem.name = "Azurite"; gem.valueMod = 10; }
			if (nextLoot1 == 1 && nextLoot2 == 5) { gem.name = "Chalcedony"; gem.valueMod = 10; }
			if (nextLoot1 == 1 && nextLoot2 == 6) { gem.name = "Hematite"; gem.valueMod = 5; }
			
			if (nextLoot1 == 2 && nextLoot2 == 1) { gem.name = "Jade"; gem.valueMod = 20; }
			if (nextLoot1 == 2 && nextLoot2 == 2) { gem.name = "Jet"; gem.valueMod = 10; }
			if (nextLoot1 == 2 && nextLoot2 == 3) { gem.name = "Magnetite"; gem.valueMod = 5; }
			if (nextLoot1 == 2 && nextLoot2 == 4) { gem.name = "Malachite"; gem.valueMod = 15; }
			if (nextLoot1 == 2 && nextLoot2 == 5) { gem.name = "Obsidian"; gem.valueMod = 2; }
			if (nextLoot1 == 2 && nextLoot2 == 6) { gem.name = "Quartz"; gem.valueMod = 15; }
			
			if (nextLoot1 == 3 && nextLoot2 == 1) { gem.name = "Amber"; gem.valueMod = 25; }
			if (nextLoot1 == 3 && nextLoot2 == 2) { gem.name = "Amethyst"; gem.valueMod = 30; }
			if (nextLoot1 == 3 && nextLoot2 == 3) { gem.name = "Calcite"; gem.valueMod = 20; }
			if (nextLoot1 == 3 && nextLoot2 == 4) { gem.name = "Sard"; gem.valueMod = 25; }
			if (nextLoot1 == 3 && nextLoot2 == 5) { gem.name = "Coral"; gem.valueMod = 20; }
			if (nextLoot1 == 3 && nextLoot2 == 6) { gem.name = "Lapis Lazuli"; gem.valueMod = 25; }
			
			if (nextLoot1 == 4 && nextLoot2 == 1) { gem.name = "Onyx"; gem.valueMod = 20; }
			if (nextLoot1 == 4 && nextLoot2 == 2) { gem.name = "Tourmaline"; gem.valueMod = 25; }
			if (nextLoot1 == 4 && nextLoot2 == 3) { gem.name = "Turquoise"; gem.valueMod = 20; }
			if (nextLoot1 == 4 && nextLoot2 == 4) { gem.name = "Aquamarine"; gem.valueMod = 30; }
			if (nextLoot1 == 4 && nextLoot2 == 5) { gem.name = "Beryl"; gem.valueMod = 30; }
			if (nextLoot1 == 4 && nextLoot2 == 6) { gem.name = "Bloodstone"; gem.valueMod = 30; }
			
			if (nextLoot1 == 5 && nextLoot2 == 1) { gem.name = "Cat's Eye"; gem.valueMod = 30; }
			if (nextLoot1 == 5 && nextLoot2 == 2) { gem.name = "Emerald"; gem.valueMod = 35; }
			if (nextLoot1 == 5 && nextLoot2 == 3) { gem.name = "Garnet"; gem.valueMod = 35; }
			if (nextLoot1 == 5 && nextLoot2 == 4) { gem.name = "Iolite"; gem.valueMod = 30; }
			if (nextLoot1 == 5 && nextLoot2 == 5) { gem.name = "Moonstone"; gem.valueMod = 30; }
			if (nextLoot1 == 5 && nextLoot2 == 6) { gem.name = "Opal"; gem.valueMod = 35; }
			
			if (nextLoot1 == 6 && nextLoot2 == 1) { gem.name = "Pearl"; gem.valueMod = 35; }
			if (nextLoot1 == 6 && nextLoot2 == 2) { gem.name = "Peridot"; gem.valueMod = 30; }
			if (nextLoot1 == 6 && nextLoot2 == 3) { gem.name = "Ruby"; gem.valueMod = 35; }
			if (nextLoot1 == 6 && nextLoot2 == 4) { gem.name = "Sapphire"; gem.valueMod = 35; }
			if (nextLoot1 == 6 && nextLoot2 == 5) { gem.name = "Topaz"; gem.valueMod = 35; }
			if (nextLoot1 == 6 && nextLoot2 == 6) { gem.name = "Diamond"; gem.valueMod = 40; }
			
		}
		
		gem.carats = carats;
		
		gem.value = parseFloat(((1*(gem.carats * gem.carats) + (4*gem.carats))*gem.valueMod).toFixed(2));
	}
	
	return gem;
}

function getEnchantment() {
	var typeRand = Math.ceil(Math.random()*6);
	var enchType = "";
	
	if (typeRand == 1) { enchType = "Attack"; }
	if (typeRand == 2) { enchType = "Buffs"; }
	if (typeRand == 3) { enchType = "Defense"; }
	if (typeRand == 4) { enchType = "Environmental"; }
	if (typeRand == 5) { enchType = "Environmental"; }
	if (typeRand == 6) { enchType = "Influence"; }
	
	var enchant = { "name":"", "type":enchType, "cost":0, "reserve":0 };
	
	var enchRand1 = Math.ceil(Math.random()*6);
	var enchRand2 = Math.ceil(Math.random()*6);
	var enchRand3 = Math.ceil(Math.random()*6);
	
	if (enchType == "Attack") {
		if (enchRand1 < 4) {
			if (enchRand2 == 1 && enchRand3 == 1) { enchant.name = "Acid Ball"; enchant.cost = 6000; enchant.reserve = 6; }
			if (enchRand2 == 1 && enchRand3 == 2) { enchant.name = "Acid Jet"; enchant.cost = 8000; enchant.reserve = 6; }
			if (enchRand2 == 1 && enchRand3 == 3) { enchant.name = "Air Jet"; enchant.cost = 4000; enchant.reserve = 6; }
			if (enchRand2 == 1 && enchRand3 == 4) { enchant.name = "Arboreal Immurement"; enchant.cost = 20000; enchant.reserve = 48; }
			if (enchRand2 == 1 && enchRand3 == 5) { enchant.name = "Ball of Lightning"; enchant.cost = 12000; enchant.reserve = 12; }
			if (enchRand2 == 1 && enchRand3 == 6) { enchant.name = "Breathe Fire"; enchant.cost = 20000; enchant.reserve = 6; }
			
			if (enchRand2 == 2 && enchRand3 == 1) { enchant.name = "Breathe Steam"; enchant.cost = 20000; enchant.reserve = 6; }
			if (enchRand2 == 2 && enchRand3 == 2) { enchant.name = "Burning Death"; enchant.cost = 14000; enchant.reserve = 18; }
			if (enchRand2 == 2 && enchRand3 == 3) { enchant.name = "Burning Touch"; enchant.cost = 6000; enchant.reserve = 6; }
			if (enchRand2 == 2 && enchRand3 == 4) { enchant.name = "Clumsiness"; enchant.cost = 16000; enchant.reserve = 6; }
			if (enchRand2 == 2 && enchRand3 == 5) { enchant.name = "Concussion"; enchant.cost = 20000; enchant.reserve = 12; }
			if (enchRand2 == 2 && enchRand3 == 6) { enchant.name = "Debility"; enchant.cost = 18000; enchant.reserve = 6; }
			
			if (enchRand2 == 3 && enchRand3 == 1) { enchant.name = "Dehydrate"; enchant.cost = 14000; enchant.reserve = 6; }
			if (enchRand2 == 3 && enchRand3 == 2) { enchant.name = "Explosive Fireball"; enchant.cost = 24000; enchant.reserve = 12; }
			if (enchRand2 == 3 && enchRand3 == 3) { enchant.name = "Explosive Fireball"; enchant.cost = 24000; enchant.reserve = 12; }
			if (enchRand2 == 3 && enchRand3 == 4) { enchant.name = "Explosive Lightning"; enchant.cost = 24000; enchant.reserve = 12; }
			if (enchRand2 == 3 && enchRand3 == 5) { enchant.name = "Explosive Lightning"; enchant.cost = 24000; enchant.reserve = 12; }
			if (enchRand2 == 3 && enchRand3 == 6) { enchant.name = "Fire Cloud"; enchant.cost = 3500; enchant.reserve = 6; }
			
			if (enchRand2 == 4 && enchRand3 == 1) { enchant.name = "Fireball"; enchant.cost = 16000; enchant.reserve = 6; }
			if (enchRand2 == 4 && enchRand3 == 2) { enchant.name = "Fireball"; enchant.cost = 16000; enchant.reserve = 6; }
			if (enchRand2 == 4 && enchRand3 == 3) { enchant.name = "Flame Jet"; enchant.cost = 16000; enchant.reserve = 6; }
			if (enchRand2 == 4 && enchRand3 == 4) { enchant.name = "Flame Jet"; enchant.cost = 16000; enchant.reserve = 6; }
			if (enchRand2 == 4 && enchRand3 == 5) { enchant.name = "Flash"; enchant.cost = 4000; enchant.reserve = 24; }
			if (enchRand2 == 4 && enchRand3 == 6) { enchant.name = "Flash"; enchant.cost = 4000; enchant.reserve = 24; }
			
			if (enchRand2 == 5 && enchRand3 == 1) { enchant.name = "Flesh to Ice"; enchant.cost = 24000; enchant.reserve = 72; }
			if (enchRand2 == 5 && enchRand3 == 2) { enchant.name = "Frailty"; enchant.cost = 20000; enchant.reserve = 12; }
			if (enchRand2 == 5 && enchRand3 == 3) { enchant.name = "Frostbite"; enchant.cost = 14000; enchant.reserve = 6; }
			if (enchRand2 == 5 && enchRand3 == 4) { enchant.name = "Hinder"; enchant.cost = 12000; enchant.reserve = 6; }
			if (enchRand2 == 5 && enchRand3 == 5) { enchant.name = "Ice Dagger"; enchant.cost = 8000; enchant.reserve = 6; }
			if (enchRand2 == 5 && enchRand3 == 6) { enchant.name = "Ice Dagger"; enchant.cost = 8000; enchant.reserve = 6; }
			
			if (enchRand2 == 6 && enchRand3 == 1) { enchant.name = "Ice Sphere"; enchant.cost = 8000; enchant.reserve = 6; }
			if (enchRand2 == 6 && enchRand3 == 2) { enchant.name = "Icy Breath"; enchant.cost = 8000; enchant.reserve = 6; }
			if (enchRand2 == 6 && enchRand3 == 3) { enchant.name = "Icy Missiles"; enchant.cost = 20000; enchant.reserve = 24; }
			if (enchRand2 == 6 && enchRand3 == 4) { enchant.name = "Icy Touch"; enchant.cost = 16000; enchant.reserve = 12; }
			if (enchRand2 == 6 && enchRand3 == 5) { enchant.name = "Lightning"; enchant.cost = 16000; enchant.reserve = 6; }
			if (enchRand2 == 6 && enchRand3 == 6) { enchant.name = "Lightning"; enchant.cost = 16000; enchant.reserve = 6; }
		}
	
		if (enchRand1 > 3) {
			if (enchRand2 == 1 && enchRand3 == 1) { enchant.name = "Lightning Stare"; enchant.cost = 20000; enchant.reserve = 6; }
			if (enchRand2 == 1 && enchRand3 == 2) { enchant.name = "Lightning Whip"; enchant.cost = 7000; enchant.reserve = 6; }
			if (enchRand2 == 1 && enchRand3 == 3) { enchant.name = "Mud Jet"; enchant.cost = 12000; enchant.reserve = 6; }
			if (enchRand2 == 1 && enchRand3 == 4) { enchant.name = "Pain"; enchant.cost = 8000; enchant.reserve = 12; }
			if (enchRand2 == 1 && enchRand3 == 5) { enchant.name = "Pain"; enchant.cost = 8000; enchant.reserve = 12; }
			if (enchRand2 == 1 && enchRand3 == 6) { enchant.name = "Pestilence"; enchant.cost = 20000; enchant.reserve = 36; }
			
			if (enchRand2 == 2 && enchRand3 == 1) { enchant.name = "Rain of Acid"; enchant.cost = 24000; enchant.reserve = 18; }
			if (enchRand2 == 2 && enchRand3 == 2) { enchant.name = "Rain of Fire"; enchant.cost = 12000; enchant.reserve = 6; }
			if (enchRand2 == 2 && enchRand3 == 3) { enchant.name = "Rain of Ice Daggers"; enchant.cost = 16000; enchant.reserve = 12; }
			if (enchRand2 == 2 && enchRand3 == 4) { enchant.name = "Rain of Nuts"; enchant.cost = 10000; enchant.reserve = 3; }
			if (enchRand2 == 2 && enchRand3 == 5) { enchant.name = "Rain of Stones"; enchant.cost = 10000; enchant.reserve = 6; }
			if (enchRand2 == 2 && enchRand3 == 6) { enchant.name = "Rooted Feet"; enchant.cost = 8000; enchant.reserve = 18; }
			
			if (enchRand2 == 3 && enchRand3 == 1) { enchant.name = "Rotting Death"; enchant.cost = 14000; enchant.reserve = 18; }
			if (enchRand2 == 3 && enchRand3 == 2) { enchant.name = "Sand Jet"; enchant.cost = 12000; enchant.reserve = 6; }
			if (enchRand2 == 3 && enchRand3 == 3) { enchant.name = "Shocking Touch"; enchant.cost = 30000; enchant.reserve = 6; }
			if (enchRand2 == 3 && enchRand3 == 4) { enchant.name = "Shocking Touch"; enchant.cost = 30000; enchant.reserve = 6; }
			if (enchRand2 == 3 && enchRand3 == 5) { enchant.name = "Sickness (cast)"; enchant.cost = 16000; enchant.reserve = 18; }
			if (enchRand2 == 3 && enchRand3 == 6) { enchant.name = "Snow Jet"; enchant.cost = 12000; enchant.reserve = 6; }
			
			if (enchRand2 == 4 && enchRand3 == 1) { enchant.name = "Sound Jet"; enchant.cost = 8000; enchant.reserve = 6; }
			if (enchRand2 == 4 && enchRand3 == 2) { enchant.name = "Spider Silk"; enchant.cost = 8000; enchant.reserve = 6; }
			if (enchRand2 == 4 && enchRand3 == 3) { enchant.name = "Spit Acid"; enchant.cost = 8000; enchant.reserve = 6; }
			if (enchRand2 == 4 && enchRand3 == 4) { enchant.name = "Steal Energy"; enchant.cost = 16000; enchant.reserve = 3; }
			if (enchRand2 == 4 && enchRand3 == 5) { enchant.name = "Steal Vitality (cast)"; enchant.cost = 30000; enchant.reserve = 3; }
			if (enchRand2 == 4 && enchRand3 == 6) { enchant.name = "Steam Jet"; enchant.cost = 14000; enchant.reserve = 6; }
			
			if (enchRand2 == 5 && enchRand3 == 1) { enchant.name = "Resist Steelwraith"; enchant.cost = 5000; enchant.reserve = 42; }
			if (enchRand2 == 5 && enchRand3 == 2) { enchant.name = "Stone Missile"; enchant.cost = 8000; enchant.reserve = 6; }
			if (enchRand2 == 5 && enchRand3 == 3) { enchant.name = "Stun"; enchant.cost = 10000; enchant.reserve = 12; }
			if (enchRand2 == 5 && enchRand3 == 4) { enchant.name = "Sunbolt"; enchant.cost = 16000; enchant.reserve = 6; }
			if (enchRand2 == 5 && enchRand3 == 5) { enchant.name = "Sound Jet"; enchant.cost = 8000; enchant.reserve = 6; }
			if (enchRand2 == 5 && enchRand3 == 6) { enchant.name = "Spider Silk"; enchant.cost = 8000; enchant.reserve = 6; }
			
			if (enchRand2 == 6 && enchRand3 == 1) { enchant.name = "Spit Acid"; enchant.cost = 8000; enchant.reserve = 6; }
			if (enchRand2 == 6 && enchRand3 == 2) { enchant.name = "Steal Energy"; enchant.cost = 16000; enchant.reserve = 3; }
			if (enchRand2 == 6 && enchRand3 == 3) { enchant.name = "Steal Vitality (cast)"; enchant.cost = 30000; enchant.reserve = 3; }
			if (enchRand2 == 6 && enchRand3 == 4) { enchant.name = "Steam Jet"; enchant.cost = 14000; enchant.reserve = 6; }
			if (enchRand2 == 6 && enchRand3 == 5) { enchant.name = "Stone Missile"; enchant.cost = 8000; enchant.reserve = 6; }
			if (enchRand2 == 6 && enchRand3 == 6) { enchant.name = "Stun"; enchant.cost = 10000; enchant.reserve = 12; }
		}
	}
	
	if (enchType == "Buffs") {
		if (enchRand1 < 4) {
			if (enchRand2 == 1 && enchRand3 == 1) { enchant.name = "Alertness 1"; enchant.cost = 6000; enchant.reserve = 12; }
			if (enchRand2 == 1 && enchRand3 == 2) { enchant.name = "Alertness 1"; enchant.cost = 6000; enchant.reserve = 12; }
			if (enchRand2 == 1 && enchRand3 == 3) { enchant.name = "Alertness 2"; enchant.cost = 12000; enchant.reserve = 24; }
			if (enchRand2 == 1 && enchRand3 == 4) { enchant.name = "Alertness 3"; enchant.cost = 18000; enchant.reserve = 36; }
			if (enchRand2 == 1 && enchRand3 == 5) { enchant.name = "Alertness 4"; enchant.cost = 24000; enchant.reserve = 48; }
			if (enchRand2 == 1 && enchRand3 == 6) { enchant.name = "Alertness 5"; enchant.cost = 30000; enchant.reserve = 60; }
			
			if (enchRand2 == 2 && enchRand3 == 1) { enchant.name = "Balance (cast)"; enchant.cost = 15000; enchant.reserve = 30; }
			if (enchRand2 == 2 && enchRand3 == 2) { enchant.name = "Bravery (cast)"; enchant.cost = 10000; enchant.reserve = 12; }
			if (enchRand2 == 2 && enchRand3 == 3) { enchant.name = "Bravery (cast)"; enchant.cost = 10000; enchant.reserve = 12; }
			if (enchRand2 == 2 && enchRand3 == 4) { enchant.name = "Breathe Water"; enchant.cost = 8000; enchant.reserve = 24; }
			if (enchRand2 == 2 && enchRand3 == 5) { enchant.name = "Climbing +1"; enchant.cost = 5000; enchant.reserve = 3; }
			if (enchRand2 == 2 && enchRand3 == 6) { enchant.name = "Climbing +2"; enchant.cost = 10000; enchant.reserve = 6; }
			
			if (enchRand2 == 3 && enchRand3 == 1) { enchant.name = "Climbing +3"; enchant.cost = 15000; enchant.reserve = 9; }
			if (enchRand2 == 3 && enchRand3 == 2) { enchant.name = "Climbing +4"; enchant.cost = 20000; enchant.reserve = 12; }
			if (enchRand2 == 3 && enchRand3 == 3) { enchant.name = "Freedom 1"; enchant.cost = 10000; enchant.reserve = 12; }
			if (enchRand2 == 3 && enchRand3 == 4) { enchant.name = "Freedom 2"; enchant.cost = 20000; enchant.reserve = 24; }
			if (enchRand2 == 3 && enchRand3 == 5) { enchant.name = "Freedom 3"; enchant.cost = 30000; enchant.reserve = 36; }
			if (enchRand2 == 3 && enchRand3 == 6) { enchant.name = "Freedom 4"; enchant.cost = 40000; enchant.reserve = 48; }
			
			if (enchRand2 == 4 && enchRand3 == 1) { enchant.name = "Freedom 5"; enchant.cost = 50000; enchant.reserve = 60; }
			if (enchRand2 == 4 && enchRand3 == 2) { enchant.name = "Grace (cast)"; enchant.cost = 40000; enchant.reserve = 24; }
			if (enchRand2 == 4 && enchRand3 == 3) { enchant.name = "Grace 1"; enchant.cost = 40000; enchant.reserve = 24; }
			if (enchRand2 == 4 && enchRand3 == 4) { enchant.name = "Grace 1"; enchant.cost = 40000; enchant.reserve = 24; }
			if (enchRand2 == 4 && enchRand3 == 5) { enchant.name = "Grace 2"; enchant.cost = 80000; enchant.reserve = 48; }
			if (enchRand2 == 4 && enchRand3 == 6) { enchant.name = "Grace 3"; enchant.cost = 120000; enchant.reserve = 72; }
			
			if (enchRand2 == 5 && enchRand3 == 1) { enchant.name = "Grace 4"; enchant.cost = 160000; enchant.reserve = 96; }
			if (enchRand2 == 5 && enchRand3 == 2) { enchant.name = "Grace 5"; enchant.cost = 200000; enchant.reserve = 120; }
			if (enchRand2 == 5 && enchRand3 == 3) { enchant.name = "Haste 1"; enchant.cost = 5000; enchant.reserve = 12; }
			if (enchRand2 == 5 && enchRand3 == 4) { enchant.name = "Haste 2"; enchant.cost = 10000; enchant.reserve = 24; }
			if (enchRand2 == 5 && enchRand3 == 5) { enchant.name = "Haste 3"; enchant.cost = 15000; enchant.reserve = 36; }
			if (enchRand2 == 5 && enchRand3 == 6) { enchant.name = "Hold Breath"; enchant.cost = 18000; enchant.reserve = 24; }
			
			if (enchRand2 == 6 && enchRand3 == 1) { enchant.name = "Jump 1"; enchant.cost = 5000; enchant.reserve = 3; }
			if (enchRand2 == 6 && enchRand3 == 2) { enchant.name = "Jump 2"; enchant.cost = 10000; enchant.reserve = 6; }
			if (enchRand2 == 6 && enchRand3 == 3) { enchant.name = "Jump 3"; enchant.cost = 15000; enchant.reserve = 9; }
			if (enchRand2 == 6 && enchRand3 == 4) { enchant.name = "Jump 4"; enchant.cost = 20000; enchant.reserve = 12; }
			if (enchRand2 == 6 && enchRand3 == 5) { enchant.name = "Jump 5"; enchant.cost = 25000; enchant.reserve = 15; }
			if (enchRand2 == 6 && enchRand3 == 6) { enchant.name = "Jump 6"; enchant.cost = 30000; enchant.reserve = 18; }
		}
	
		if (enchRand1 > 3) {
			if (enchRand2 == 1 && enchRand3 == 1) { enchant.name = "Keen Sense 1"; enchant.cost = 3000; enchant.reserve = 6; }
			if (enchRand2 == 1 && enchRand3 == 2) { enchant.name = "Keen Sense 1"; enchant.cost = 3000; enchant.reserve = 6; }
			if (enchRand2 == 1 && enchRand3 == 3) { enchant.name = "Keen Sense 2"; enchant.cost = 6000; enchant.reserve = 12; }
			if (enchRand2 == 1 && enchRand3 == 4) { enchant.name = "Keen Sense 3"; enchant.cost = 9000; enchant.reserve = 18; }
			if (enchRand2 == 1 && enchRand3 == 5) { enchant.name = "Keen Sense 4"; enchant.cost = 12000; enchant.reserve = 24; }
			if (enchRand2 == 1 && enchRand3 == 6) { enchant.name = "Keen Sense 5"; enchant.cost = 15000; enchant.reserve = 30; }
			
			if (enchRand2 == 2 && enchRand3 == 1) { enchant.name = "Might (cast)"; enchant.cost = 20000; enchant.reserve = 12; }
			if (enchRand2 == 2 && enchRand3 == 2) { enchant.name = "Might 1"; enchant.cost = 30000; enchant.reserve = 12; }
			if (enchRand2 == 2 && enchRand3 == 3) { enchant.name = "Might 1"; enchant.cost = 30000; enchant.reserve = 12; }
			if (enchRand2 == 2 && enchRand3 == 4) { enchant.name = "Might 2"; enchant.cost = 60000; enchant.reserve = 24; }
			if (enchRand2 == 2 && enchRand3 == 5) { enchant.name = "Might 3"; enchant.cost = 90000; enchant.reserve = 36; }
			if (enchRand2 == 2 && enchRand3 == 6) { enchant.name = "Might 4"; enchant.cost = 120000; enchant.reserve = 48; }
			
			if (enchRand2 == 3 && enchRand3 == 1) { enchant.name = "Might 5"; enchant.cost = 150000; enchant.reserve = 60; }
			if (enchRand2 == 3 && enchRand3 == 2) { enchant.name = "Reflexes (cast)"; enchant.cost = 24000; enchant.reserve = 30; }
			if (enchRand2 == 3 && enchRand3 == 3) { enchant.name = "Resist Pain"; enchant.cost = 16000; enchant.reserve = 24; }
			if (enchRand2 == 3 && enchRand3 == 4) { enchant.name = "Strengthen Will (cast)"; enchant.cost = 14000; enchant.reserve = 6; }
			if (enchRand2 == 3 && enchRand3 == 5) { enchant.name = "Strengthen Will (cast)"; enchant.cost = 14000; enchant.reserve = 6; }
			if (enchRand2 == 3 && enchRand3 == 6) { enchant.name = "Strengthen Will 1"; enchant.cost = 20000; enchant.reserve = 6; }
			
			if (enchRand2 == 4 && enchRand3 == 1) { enchant.name = "Strengthen Will 2"; enchant.cost = 40000; enchant.reserve = 12; }
			if (enchRand2 == 4 && enchRand3 == 2) { enchant.name = "Strengthen Will 3"; enchant.cost = 60000; enchant.reserve = 18; }
			if (enchRand2 == 4 && enchRand3 == 3) { enchant.name = "Strengthen Will 4"; enchant.cost = 80000; enchant.reserve = 24; }
			if (enchRand2 == 4 && enchRand3 == 4) { enchant.name = "Strengthen Will 5"; enchant.cost = 100000; enchant.reserve = 30; }
			if (enchRand2 == 4 && enchRand3 == 5) { enchant.name = "Vigor (cast)"; enchant.cost = 20000; enchant.reserve = 12; }
			if (enchRand2 == 4 && enchRand3 == 6) { enchant.name = "Vigor 1"; enchant.cost = 30000; enchant.reserve = 12; }
			
			if (enchRand2 == 5 && enchRand3 == 1) { enchant.name = "Vigor 2"; enchant.cost = 60000; enchant.reserve = 24; }
			if (enchRand2 == 5 && enchRand3 == 2) { enchant.name = "Vigor 3"; enchant.cost = 90000; enchant.reserve = 36; }
			if (enchRand2 == 5 && enchRand3 == 3) { enchant.name = "Vigor 4"; enchant.cost = 120000; enchant.reserve = 48; }
			if (enchRand2 == 5 && enchRand3 == 4) { enchant.name = "Vigor 5"; enchant.cost = 150000; enchant.reserve = 60; }
			
			else { enchant = getEnchantment(); }
		}
	}
	
	if (enchType == "Defense") {
		if (enchRand1 < 4) {
			if (enchRand2 == 1 && enchRand3 == 1) { enchant.name = "Bladeturning"; enchant.cost = 6000; enchant.reserve = 12; }
			if (enchRand2 == 1 && enchRand3 == 2) { enchant.name = "Blink"; enchant.cost = 16000; enchant.reserve = 12; }
			if (enchRand2 == 1 && enchRand3 == 3) { enchant.name = "Blur 1"; enchant.cost = 2000; enchant.reserve = 6; }
			if (enchRand2 == 1 && enchRand3 == 4) { enchant.name = "Blur 1"; enchant.cost = 2000; enchant.reserve = 6; }
			if (enchRand2 == 1 && enchRand3 == 5) { enchant.name = "Blur 2"; enchant.cost = 4000; enchant.reserve = 12; }
			if (enchRand2 == 1 && enchRand3 == 6) { enchant.name = "Blur 2"; enchant.cost = 4000; enchant.reserve = 12; }
			
			if (enchRand2 == 2 && enchRand3 == 1) { enchant.name = "Blur 3"; enchant.cost = 6000; enchant.reserve = 18; }
			if (enchRand2 == 2 && enchRand3 == 2) { enchant.name = "Blur 3"; enchant.cost = 6000; enchant.reserve = 18; }
			if (enchRand2 == 2 && enchRand3 == 3) { enchant.name = "Blur 4"; enchant.cost = 8000; enchant.reserve = 24; }
			if (enchRand2 == 2 && enchRand3 == 4) { enchant.name = "Blur 5"; enchant.cost = 10000; enchant.reserve = 30; }
			if (enchRand2 == 2 && enchRand3 == 5) { enchant.name = "Catch Missile"; enchant.cost = 6000; enchant.reserve = 12; }
			if (enchRand2 == 2 && enchRand3 == 6) { enchant.name = "Catch Spell"; enchant.cost = 24000; enchant.reserve = 18; }
			
			if (enchRand2 == 3 && enchRand3 == 1) { enchant.name = "Force Wall"; enchant.cost = 6000; enchant.reserve = 12; }
			if (enchRand2 == 3 && enchRand3 == 2) { enchant.name = "Hide (cast)"; enchant.cost = 20000; enchant.reserve = 6; }
			if (enchRand2 == 3 && enchRand3 == 3) { enchant.name = "Hide (cast)"; enchant.cost = 20000; enchant.reserve = 6; }
			if (enchRand2 == 3 && enchRand3 == 4) { enchant.name = "Invisibility (cast)"; enchant.cost = 24000; enchant.reserve = 30; }
			if (enchRand2 == 3 && enchRand3 == 5) { enchant.name = "Iron Arm"; enchant.cost = 12000; enchant.reserve = 6; }
			if (enchRand2 == 3 && enchRand3 == 6) { enchant.name = "Mage-Stealth"; enchant.cost = 10000; enchant.reserve = 18; }
			
			if (enchRand2 == 4 && enchRand3 == 1) { enchant.name = "Magic Resistance 1"; enchant.cost = 4000; enchant.reserve = 6; }
			if (enchRand2 == 4 && enchRand3 == 2) { enchant.name = "Magic Resistance 1"; enchant.cost = 4000; enchant.reserve = 6; }
			if (enchRand2 == 4 && enchRand3 == 3) { enchant.name = "Magic Resistance 2"; enchant.cost = 8000; enchant.reserve = 12; }
			if (enchRand2 == 4 && enchRand3 == 4) { enchant.name = "Magic Resistance 2"; enchant.cost = 8000; enchant.reserve = 12; }
			if (enchRand2 == 4 && enchRand3 == 5) { enchant.name = "Magic Resistance 3"; enchant.cost = 12000; enchant.reserve = 18; }
			if (enchRand2 == 4 && enchRand3 == 6) { enchant.name = "Magic Resistance 3"; enchant.cost = 12000; enchant.reserve = 18; }
			
			if (enchRand2 == 5 && enchRand3 == 1) { enchant.name = "Magic Resistance 4"; enchant.cost = 16000; enchant.reserve = 24; }
			if (enchRand2 == 5 && enchRand3 == 2) { enchant.name = "Magic Resistance 5"; enchant.cost = 20000; enchant.reserve = 30; }
			if (enchRand2 == 5 && enchRand3 == 3) { enchant.name = "Mirror"; enchant.cost = 7000; enchant.reserve = 12; }
			if (enchRand2 == 5 && enchRand3 == 4) { enchant.name = "Mirror"; enchant.cost = 7000; enchant.reserve = 12; }
			if (enchRand2 == 5 && enchRand3 == 5) { enchant.name = "Missile Shield"; enchant.cost = 8000; enchant.reserve = 30; }
			if (enchRand2 == 5 && enchRand3 == 6) { enchant.name = "Reflect Gaze"; enchant.cost = 12000; enchant.reserve = 12; }
			
			if (enchRand2 == 6 && enchRand3 == 1) { enchant.name = "Resist Acid (cast)"; enchant.cost = 24000; enchant.reserve = 12; }
			if (enchRand2 == 6 && enchRand3 == 2) { enchant.name = "Resist Acid (cast)"; enchant.cost = 24000; enchant.reserve = 12; }
			if (enchRand2 == 6 && enchRand3 == 3) { enchant.name = "Resist Acid (always on)"; enchant.cost = 16000; enchant.reserve = 12; }
			if (enchRand2 == 6 && enchRand3 == 4) { enchant.name = "Resist Acid (always on)"; enchant.cost = 16000; enchant.reserve = 12; }
			if (enchRand2 == 6 && enchRand3 == 5) { enchant.name = "Resist Cold"; enchant.cost = 16000; enchant.reserve = 12; }
			if (enchRand2 == 6 && enchRand3 == 6) { enchant.name = "Resist Cold"; enchant.cost = 16000; enchant.reserve = 12; }
		}
	
		if (enchRand1 > 3) {
			if (enchRand2 == 1 && enchRand3 == 1) { enchant.name = "Resist Fire"; enchant.cost = 16000; enchant.reserve = 12; }
			if (enchRand2 == 1 && enchRand3 == 2) { enchant.name = "Resist Fire"; enchant.cost = 16000; enchant.reserve = 12; }
			if (enchRand2 == 1 && enchRand3 == 3) { enchant.name = "Resist Lightning (cast)"; enchant.cost = 30000; enchant.reserve = 12; }
			if (enchRand2 == 1 && enchRand3 == 4) { enchant.name = "Resist Lightning (cast)"; enchant.cost = 30000; enchant.reserve = 12; }
			if (enchRand2 == 1 && enchRand3 == 5) { enchant.name = "Resist Lightning (always on)"; enchant.cost = 20000; enchant.reserve = 12; }
			if (enchRand2 == 1 && enchRand3 == 6) { enchant.name = "Resist Lightning (always on)"; enchant.cost = 20000; enchant.reserve = 12; }
			
			if (enchRand2 == 2 && enchRand3 == 1) { enchant.name = "Resist Sound (cast)"; enchant.cost = 24000; enchant.reserve = 12; }
			if (enchRand2 == 2 && enchRand3 == 2) { enchant.name = "Resist Sound (cast)"; enchant.cost = 24000; enchant.reserve = 12; }
			if (enchRand2 == 2 && enchRand3 == 3) { enchant.name = "Resist Sound (always on)"; enchant.cost = 16000; enchant.reserve = 12; }
			if (enchRand2 == 2 && enchRand3 == 4) { enchant.name = "Resist Sound (always on)"; enchant.cost = 16000; enchant.reserve = 12; }
			if (enchRand2 == 2 && enchRand3 == 5) { enchant.name = "Resist Water (cast)"; enchant.cost = 8000; enchant.reserve = 12; }
			if (enchRand2 == 2 && enchRand3 == 6) { enchant.name = "Resist Water (always on)"; enchant.cost = 4000; enchant.reserve = 12; }
			
			if (enchRand2 == 3 && enchRand3 == 1) { enchant.name = "Return Missiles"; enchant.cost = 8000; enchant.reserve = 12; }
			if (enchRand2 == 3 && enchRand3 == 2) { enchant.name = "Reverse Missiles"; enchant.cost = 12000; enchant.reserve = 42; }
			if (enchRand2 == 3 && enchRand3 == 3) { enchant.name = "Scryguard"; enchant.cost = 10000; enchant.reserve = 24; }
			if (enchRand2 == 3 && enchRand3 == 4) { enchant.name = "Steelwraith"; enchant.cost = 24000; enchant.reserve = 6; }
			if (enchRand2 == 3 && enchRand3 == 5) { enchant.name = "Talisman 1"; enchant.cost = 15; enchant.reserve = 6; }
			if (enchRand2 == 3 && enchRand3 == 6) { enchant.name = "Talisman 1"; enchant.cost = 15; enchant.reserve = 6; }
			
			if (enchRand2 == 4 && enchRand3 == 1) { enchant.name = "Talisman 2"; enchant.cost = 45; enchant.reserve = 12; }
			if (enchRand2 == 4 && enchRand3 == 2) { enchant.name = "Talisman 2"; enchant.cost = 45; enchant.reserve = 12; }
			if (enchRand2 == 4 && enchRand3 == 3) { enchant.name = "Talisman 3"; enchant.cost = 90; enchant.reserve = 18; }
			if (enchRand2 == 4 && enchRand3 == 4) { enchant.name = "Talisman 3"; enchant.cost = 90; enchant.reserve = 18; }
			if (enchRand2 == 4 && enchRand3 == 5) { enchant.name = "Talisman 4"; enchant.cost = 3000; enchant.reserve = 24; }
			if (enchRand2 == 4 && enchRand3 == 6) { enchant.name = "Turn Blade"; enchant.cost = 6000; enchant.reserve = 6; }
			
			if (enchRand2 == 5 && enchRand3 == 1) { enchant.name = "Utter Wall"; enchant.cost = 20000; enchant.reserve = 24; }
			if (enchRand2 == 5 && enchRand3 == 2) { enchant.name = "Wall of Light"; enchant.cost = 4000; enchant.reserve = 6; }
			if (enchRand2 == 5 && enchRand3 == 3) { enchant.name = "Wall of Lightning"; enchant.cost = 10000; enchant.reserve = 12; }
			if (enchRand2 == 5 && enchRand3 == 4) { enchant.name = "Wall of Wind"; enchant.cost = 8000; enchant.reserve = 12; }
			
			else { enchant = getEnchantment(); }
		}
	}
	
	if (enchType == "Environmental") {
		if (enchRand1 < 4) {
			if (enchRand2 == 1 && enchRand3 == 1) { enchant.name = "Air Vortex"; enchant.cost = 24000; enchant.reserve = 48; }
			if (enchRand2 == 1 && enchRand3 == 2) { enchant.name = "Boil Water"; enchant.cost = 4000; enchant.reserve = 12; }
			if (enchRand2 == 1 && enchRand3 == 3) { enchant.name = "Clouds"; enchant.cost = 6000; enchant.reserve = 3; }
			if (enchRand2 == 1 && enchRand3 == 4) { enchant.name = "Clouds"; enchant.cost = 6000; enchant.reserve = 3; }
			if (enchRand2 == 1 && enchRand3 == 5) { enchant.name = "Cold"; enchant.cost = 8000; enchant.reserve = 6; }
			if (enchRand2 == 1 && enchRand3 == 6) { enchant.name = "Cool"; enchant.cost = 3000; enchant.reserve = 3; }
			
			if (enchRand2 == 2 && enchRand3 == 1) { enchant.name = "Coolness"; enchant.cost = 4000; enchant.reserve = 12; }
			if (enchRand2 == 2 && enchRand3 == 2) { enchant.name = "Create Acid"; enchant.cost = 6000; enchant.reserve = 24; }
			if (enchRand2 == 2 && enchRand3 == 3) { enchant.name = "Create Air"; enchant.cost = 4000; enchant.reserve = 6; }
			if (enchRand2 == 2 && enchRand3 == 4) { enchant.name = "Create Fire"; enchant.cost = 6000; enchant.reserve = 12; }
			if (enchRand2 == 2 && enchRand3 == 5) { enchant.name = "Create Fire"; enchant.cost = 6000; enchant.reserve = 12; }
			if (enchRand2 == 2 && enchRand3 == 6) { enchant.name = "Create Ice"; enchant.cost = 4000; enchant.reserve = 12; }
			
			if (enchRand2 == 3 && enchRand3 == 1) { enchant.name = "Create Steam"; enchant.cost = 8000; enchant.reserve = 12; }
			if (enchRand2 == 3 && enchRand3 == 2) { enchant.name = "Create Water"; enchant.cost = 4000; enchant.reserve = 12; }
			if (enchRand2 == 3 && enchRand3 == 3) { enchant.name = "Create Water"; enchant.cost = 4000; enchant.reserve = 12; }
			if (enchRand2 == 3 && enchRand3 == 4) { enchant.name = "Destroy Water"; enchant.cost = 6000; enchant.reserve = 18; }
			if (enchRand2 == 3 && enchRand3 == 5) { enchant.name = "Extinguish Fire"; enchant.cost = 8000; enchant.reserve = 18; }
			if (enchRand2 == 3 && enchRand3 == 6) { enchant.name = "False Tracks"; enchant.cost = 6000; enchant.reserve = 12; }
			
			if (enchRand2 == 4 && enchRand3 == 1) { enchant.name = "Fast Fire"; enchant.cost = 7500; enchant.reserve = 12; }
			if (enchRand2 == 4 && enchRand3 == 2) { enchant.name = "Fog"; enchant.cost = 6000; enchant.reserve = 12; }
			if (enchRand2 == 4 && enchRand3 == 3) { enchant.name = "Foul Water"; enchant.cost = 2000; enchant.reserve = 18; }
			if (enchRand2 == 4 && enchRand3 == 4) { enchant.name = "Freeze"; enchant.cost = 4000; enchant.reserve = 12; }
			if (enchRand2 == 4 && enchRand3 == 5) { enchant.name = "Frost"; enchant.cost = 4000; enchant.reserve = 6; }
			if (enchRand2 == 4 && enchRand3 == 6) { enchant.name = "Garble"; enchant.cost = 20000; enchant.reserve = 24; }
			
			if (enchRand2 == 5 && enchRand3 == 1) { enchant.name = "Gloom"; enchant.cost = 5000; enchant.reserve = 3; }
			if (enchRand2 == 5 && enchRand3 == 2) { enchant.name = "Glue"; enchant.cost = 18000; enchant.reserve = 18; }
			if (enchRand2 == 5 && enchRand3 == 3) { enchant.name = "Grease"; enchant.cost = 14000; enchant.reserve = 18; }
			if (enchRand2 == 5 && enchRand3 == 4) { enchant.name = "Hail"; enchant.cost = 10000; enchant.reserve = 3; }
			if (enchRand2 == 5 && enchRand3 == 5) { enchant.name = "Heal Plant"; enchant.cost = 8000; enchant.reserve = 18; }
			if (enchRand2 == 5 && enchRand3 == 6) { enchant.name = "Heat"; enchant.cost = 8000; enchant.reserve = 6; }
			
			if (enchRand2 == 6 && enchRand3 == 1) { enchant.name = "Hide Path"; enchant.cost = 6000; enchant.reserve = 12; }
			if (enchRand2 == 6 && enchRand3 == 2) { enchant.name = "Hold Fast"; enchant.cost = 6000; enchant.reserve = 6; }
			if (enchRand2 == 6 && enchRand3 == 3) { enchant.name = "Hush"; enchant.cost = 4000; enchant.reserve = 12; }
			if (enchRand2 == 6 && enchRand3 == 4) { enchant.name = "Ice Slick"; enchant.cost = 5000; enchant.reserve = 18; }
			if (enchRand2 == 6 && enchRand3 == 5) { enchant.name = "Ignite Fire"; enchant.cost = 2000; enchant.reserve = 6; }
			if (enchRand2 == 6 && enchRand3 == 6) { enchant.name = "Melt Ice"; enchant.cost = 6000; enchant.reserve = 12; }
		}
	
		if (enchRand1 > 3) {
			if (enchRand2 == 1 && enchRand3 == 1) { enchant.name = "Mystic Mist"; enchant.cost = 8000; enchant.reserve = 6; }
			if (enchRand2 == 1 && enchRand3 == 2) { enchant.name = "Noise"; enchant.cost = 2000; enchant.reserve = 24; }
			if (enchRand2 == 1 && enchRand3 == 3) { enchant.name = "Pollen Cloud"; enchant.cost = 2000; enchant.reserve = 6; }
			if (enchRand2 == 1 && enchRand3 == 4) { enchant.name = "Pull"; enchant.cost = 5000; enchant.reserve = 6; }
			if (enchRand2 == 1 && enchRand3 == 5) { enchant.name = "Purify Air"; enchant.cost = 50; enchant.reserve = 6; }
			if (enchRand2 == 1 && enchRand3 == 6) { enchant.name = "Purify Water"; enchant.cost = 50; enchant.reserve = 6; }
			
			if (enchRand2 == 2 && enchRand3 == 1) { enchant.name = "Rain"; enchant.cost = 12000; enchant.reserve = 3; }
			if (enchRand2 == 2 && enchRand3 == 2) { enchant.name = "Repel"; enchant.cost = 5000; enchant.reserve = 6; }
			if (enchRand2 == 2 && enchRand3 == 3) { enchant.name = "Sandstorm"; enchant.cost = 20000; enchant.reserve = 18; }
			if (enchRand2 == 2 && enchRand3 == 4) { enchant.name = "Seek Water"; enchant.cost = 340; enchant.reserve = 12; }
			if (enchRand2 == 2 && enchRand3 == 5) { enchant.name = "Shade"; enchant.cost = 2000; enchant.reserve = 6; }
			if (enchRand2 == 2 && enchRand3 == 6) { enchant.name = "Shape Fire"; enchant.cost = 8000; enchant.reserve = 12; }
			
			if (enchRand2 == 3 && enchRand3 == 1) { enchant.name = "Shape Plant"; enchant.cost = 10000; enchant.reserve = 18; }
			if (enchRand2 == 3 && enchRand3 == 2) { enchant.name = "Shape Water"; enchant.cost = 8000; enchant.reserve = 6; }
			if (enchRand2 == 3 && enchRand3 == 3) { enchant.name = "Silence"; enchant.cost = 80; enchant.reserve = 12; }
			if (enchRand2 == 3 && enchRand3 == 4) { enchant.name = "Slow Fire"; enchant.cost = 7000; enchant.reserve = 12; }
			if (enchRand2 == 3 && enchRand3 == 5) { enchant.name = "Smoke"; enchant.cost = 10; enchant.reserve = 6; }
			if (enchRand2 == 3 && enchRand3 == 6) { enchant.name = "Snow"; enchant.cost = 9000; enchant.reserve = 3; }
			
			if (enchRand2 == 4 && enchRand3 == 1) { enchant.name = "Spark Cloud"; enchant.cost = 3000; enchant.reserve = 6; }
			if (enchRand2 == 4 && enchRand3 == 2) { enchant.name = "Spark Storm"; enchant.cost = 6000; enchant.reserve = 12; }
			if (enchRand2 == 4 && enchRand3 == 3) { enchant.name = "Stench"; enchant.cost = 60; enchant.reserve = 6; }
			if (enchRand2 == 4 && enchRand3 == 4) { enchant.name = "Storm"; enchant.cost = 20000; enchant.reserve = 3; }
			if (enchRand2 == 4 && enchRand3 == 5) { enchant.name = "Thunderclap"; enchant.cost = 6000; enchant.reserve = 12; }
			if (enchRand2 == 4 && enchRand3 == 6) { enchant.name = "Umbrella"; enchant.cost = 2000; enchant.reserve = 6; }
			
			if (enchRand2 == 5 && enchRand3 == 1) { enchant.name = "Warm"; enchant.cost = 3000; enchant.reserve = 3; }
			if (enchRand2 == 5 && enchRand3 == 2) { enchant.name = "Waves"; enchant.cost = 6000; enchant.reserve = 3; }
			if (enchRand2 == 5 && enchRand3 == 3) { enchant.name = "Whirlpool"; enchant.cost = 4000; enchant.reserve = 12; }
			if (enchRand2 == 5 && enchRand3 == 4) { enchant.name = "Wind"; enchant.cost = 6000; enchant.reserve = 3; }
			if (enchRand2 == 5 && enchRand3 == 5) { enchant.name = "Wither Plant"; enchant.cost = 4000; enchant.reserve = 12; }
			
			else { enchant = getEnchantment(); }
		}
	}
	
	if (enchType == "Influence") {
		if (enchRand2 == 1 && enchRand3 == 1) { enchant.name = "Animal Control"; enchant.cost = 4000; enchant.reserve = 6; }
		if (enchRand2 == 1 && enchRand3 == 2) { enchant.name = "Beast Summoning"; enchant.cost = 8000; enchant.reserve = 18; }
		if (enchRand2 == 1 && enchRand3 == 3) { enchant.name = "Charm"; enchant.cost = 60000; enchant.reserve = 36; }
		if (enchRand2 == 1 && enchRand3 == 4) { enchant.name = "Command"; enchant.cost = 10000; enchant.reserve = 12; }
		if (enchRand2 == 1 && enchRand3 == 5) { enchant.name = "Control Elemental"; enchant.cost = 20000; enchant.reserve = 6; }
		if (enchRand2 == 1 && enchRand3 == 6) { enchant.name = "Drunkenness (cast)"; enchant.cost = 16000; enchant.reserve = 6; }
		
		if (enchRand2 == 2 && enchRand3 == 1) { enchant.name = "Emotion Control (cast, any emotion)"; enchant.cost = 44000; enchant.reserve = 12; }
		if (enchRand2 == 2 && enchRand3 == 2) { enchant.name = "Emotion Control (cast, single emotion)"; enchant.cost = 20000; enchant.reserve = 12; }
		if (enchRand2 == 2 && enchRand3 == 3) { enchant.name = "Fear (cast)"; enchant.cost = 6000; enchant.reserve = 6; }
		if (enchRand2 == 2 && enchRand3 == 4) { enchant.name = "Glib Tongue"; enchant.cost = 13000; enchant.reserve = 12; }
		if (enchRand2 == 2 && enchRand3 == 5) { enchant.name = "Great Voice"; enchant.cost = 4000; enchant.reserve = 18; }
		if (enchRand2 == 2 && enchRand3 == 6) { enchant.name = "Loyalty (cast)"; enchant.cost = 40000; enchant.reserve = 12; }
		
		if (enchRand2 == 3 && enchRand3 == 1) { enchant.name = "Madness (cast)"; enchant.cost = 20000; enchant.reserve = 24; }
		if (enchRand2 == 3 && enchRand3 == 2) { enchant.name = "Mass Suggestion"; enchant.cost = 30000; enchant.reserve = 24; }
		if (enchRand2 == 3 && enchRand3 == 3) { enchant.name = "Nightmare (cast)"; enchant.cost = 16000; enchant.reserve = 36; }
		if (enchRand2 == 3 && enchRand3 == 4) { enchant.name = "Panic"; enchant.cost = 10000; enchant.reserve = 24; }
		if (enchRand2 == 3 && enchRand3 == 5) { enchant.name = "Plant Control"; enchant.cost = 12000; enchant.reserve = 18; }
		if (enchRand2 == 3 && enchRand3 == 6) { enchant.name = "Plant Speech (all plants)"; enchant.cost = 40000; enchant.reserve = 18; }
		
		if (enchRand2 == 4 && enchRand3 == 1) { enchant.name = "Plant Speech (one type)"; enchant.cost = 15000; enchant.reserve = 18; }
		if (enchRand2 == 4 && enchRand3 == 2) { enchant.name = "Repel Animal"; enchant.cost = 2000; enchant.reserve = 6; }
		if (enchRand2 == 4 && enchRand3 == 3) { enchant.name = "Repel Spirits"; enchant.cost = 2000; enchant.reserve = 24; }
		if (enchRand2 == 4 && enchRand3 == 4) { enchant.name = "Silver Tongue (always on)"; enchant.cost = 12000; enchant.reserve = 18; }
		if (enchRand2 == 4 && enchRand3 == 5) { enchant.name = "Silver Tongue (cast)"; enchant.cost = 4000; enchant.reserve = 18; }
		if (enchRand2 == 4 && enchRand3 == 6) { enchant.name = "Suggestion"; enchant.cost = 10000; enchant.reserve = 24; }
		
		if (enchRand2 == 5 && enchRand3 == 1) { enchant.name = "Summon Elemental (Air)"; enchant.cost = 10000; enchant.reserve = 24; }
		if (enchRand2 == 5 && enchRand3 == 2) { enchant.name = "Summon Elemental (Earth)"; enchant.cost = 16000; enchant.reserve = 24; }
		if (enchRand2 == 5 && enchRand3 == 3) { enchant.name = "Summon Elemental (Fire)"; enchant.cost = 16000; enchant.reserve = 24; }
		if (enchRand2 == 5 && enchRand3 == 4) { enchant.name = "Summon Elemental (Water)"; enchant.cost = 16000; enchant.reserve = 24; }
		if (enchRand2 == 5 && enchRand3 == 5) { enchant.name = "Summon Spirit"; enchant.cost = 20000; enchant.reserve = 120; }
		if (enchRand2 == 5 && enchRand3 == 6) { enchant.name = "Terror"; enchant.cost = 12000; enchant.reserve = 24; }
		
		if (enchRand2 == 6 && enchRand3 == 1) { enchant.name = "Turn Spirit"; enchant.cost = 7000; enchant.reserve = 24; }
		if (enchRand2 == 6 && enchRand3 == 2) { enchant.name = "Weaken Will (cast)"; enchant.cost = 14000; enchant.reserve = 12; }
		
		else { enchant = getEnchantment(); }
	}
	
	return enchant;
}

function getWeaponEnchant(weaponType) {
	var rand1 = Math.ceil(Math.random()*6);
	var rand2 = Math.ceil(Math.random()*6);
	
	var enchant = { "name":"", "cost":0, "costPerPound":false, "desc":"" }
	
	if (rand1 == 1) { 
		if (rand2 == 1 || rand2 == 2 || rand2 == 3) {
			enchant.name = "Accuracy +1";
			if (weaponType == "Melee") { enchant.cost = 5000; }
			if (weaponType == "Missile") { enchant.cost = 5000; }
			if (weaponType == "Projectile") { enchant.cost = 100; }
		}
		if (rand2 == 4) {
			enchant.name = "Accuracy +2";
			if (weaponType == "Melee") { enchant.cost = 20000; }
			if (weaponType == "Missile") { enchant.cost = 20000; }
			if (weaponType == "Projectile") { enchant.cost = 100; }
		}
		if (rand2 == 5) {
			enchant.name = "Accuracy +3";
			if (weaponType == "Melee") { enchant.cost = 100000; }
			if (weaponType == "Missile") { enchant.cost = 20000; }
			if (weaponType == "Projectile") { enchant.cost = 100; }
		}
		if (rand2 == 6) {
			enchant.name = "Blood Drinker";
			if (weaponType == "Melee") { enchant.cost = 50000; }
			if (weaponType == "Missile") { enchant.cost = 0; }
			if (weaponType == "Projectile") { enchant.cost = 0; }
			enchant.desc = "The weapon draws life energy from the damage it does. For every attack that penetrates DR and does damage, the weapon gains a point in an energy reserve. The maximum capacity is calculated from the weapon’s value as a magician’s power item. However, the energy can only be used for mundane physical exertion such as extra effort, long-distance running, and making up for lost sleep – not for spellcasting, chirelated efforts, or other more-or-less supernatural efforts.";
		}
	}
	
	if (rand1 == 2) { 
		if (rand2 == 1) {
			enchant.name = "Defending Weapon +1";
			if (weaponType == "Melee") { enchant.cost = 10000; }
			if (weaponType == "Missile") { enchant.cost = 0; }
			if (weaponType == "Projectile") { enchant.cost = 0; }
		}
		if (rand2 == 2) {
			enchant.name = "Defending Weapon +2";
			if (weaponType == "Melee") { enchant.cost = 20000; }
			if (weaponType == "Missile") { enchant.cost = 0; }
			if (weaponType == "Projectile") { enchant.cost = 0; }
		}
		if (rand2 == 3) {
			enchant.name = "Defending Weapon +3";
			if (weaponType == "Melee") { enchant.cost = 40000; }
			if (weaponType == "Missile") { enchant.cost = 0; }
			if (weaponType == "Projectile") { enchant.cost = 0; }
		}
		if (rand2 == 4) {
			enchant.name = "Dismemberment";
			if (weaponType == "Melee") { enchant.cost = 25000; }
			if (weaponType == "Missile") { enchant.cost = 0; }
			if (weaponType == "Projectile") { enchant.cost = 0; }
			enchant.desc = "The weapon cuts through targets with remarkable power. Any limb that takes crippling damage from a cutting attack from a weapon enchanted with Dismemberment is immediately and automatically cut off. This bypasses any HT rolls for duration of crippling injuries. It may only be cast on weapons that do cutting damage; reroll result for weapons that only do impaling or crushing damage.";
		}
		if (rand2 == 5) {
			enchant.name = "Flaming Weapon";
			if (weaponType == "Melee") { enchant.cost = 15400; }
			if (weaponType == "Missile") { enchant.cost = 15400; }
			if (weaponType == "Projectile") { enchant.cost = 75; }
		}
		if (rand2 == 6) {
			enchant.name = "Graceful Weapon";
			if (weaponType == "Melee") { enchant.cost = 3000; }
			if (weaponType == "Missile") { enchant.cost = 3000; }
			if (weaponType == "Projectile") { enchant.cost = 0; }
			enchant.costPerPound = true;
		}
	}
	
	if (rand1 == 3) { 
		if (rand2 == 1) {
			enchant.name = "Icy Weapon";
			if (weaponType == "Melee") { enchant.cost = 17000; }
			if (weaponType == "Missile") { enchant.cost = 17000; }
			if (weaponType == "Projectile") { enchant.cost = 75; }
		}
		if (rand2 == 2) {
			enchant.name = "Lightning Weapon";
			if (weaponType == "Melee") { enchant.cost = 15300; }
			if (weaponType == "Missile") { enchant.cost = 15300; }
			if (weaponType == "Projectile") { enchant.cost = 75; }
		}
		if (rand2 == 3) {
			enchant.name = "Loyal Sword";
			if (weaponType == "Melee") { enchant.cost = 15000; }
			if (weaponType == "Missile") { enchant.cost = 15000; }
			if (weaponType == "Projectile") { enchant.cost = 15000; }
			enchant.costPerPound = true;
		}
		if (rand2 == 4) {
			enchant.name = "Ghost Weapon";
			if (weaponType == "Melee") { enchant.cost = 5000; }
			if (weaponType == "Missile") { enchant.cost = 5000; }
			if (weaponType == "Projectile") { enchant.cost = 5000; }
			enchant.costPerPound = true;
		}
		if (rand2 == 5) {
			enchant.name = "Penetrating Weapon (2)";
			if (weaponType == "Melee") { enchant.cost = 5000; }
			if (weaponType == "Missile") { enchant.cost = 10000; }
			if (weaponType == "Projectile") { enchant.cost = 25; }
		}
		if (rand2 == 6) {
			enchant.name = "Penetrating Weapon (2)";
			if (weaponType == "Melee") { enchant.cost = 5000; }
			if (weaponType == "Missile") { enchant.cost = 10000; }
			if (weaponType == "Projectile") { enchant.cost = 25; }
		}
	}
	
	if (rand1 == 4) { 
		if (rand2 == 1) {
			enchant.name = "Penetrating Weapon (3)";
			if (weaponType == "Melee") { enchant.cost = 15000; }
			if (weaponType == "Missile") { enchant.cost = 30000; }
			if (weaponType == "Projectile") { enchant.cost = 75; }
		}
		if (rand2 == 2) {
			enchant.name = "Penetrating Weapon (5)";
			if (weaponType == "Melee") { enchant.cost = 50000; }
			if (weaponType == "Missile") { enchant.cost = 100000; }
			if (weaponType == "Projectile") { enchant.cost = 5000; }
		}
		if (rand2 == 3) {
			enchant.name = "Penetrating Weapon (10)";
			if (weaponType == "Melee") { enchant.cost = 150000; }
			if (weaponType == "Missile") { enchant.cost = 300000; }
			if (weaponType == "Projectile") { enchant.cost = 15000; }
		}
		if (rand2 == 4) {
			enchant.name = "Penetrating Weapon (Ignores Armor)";
			if (weaponType == "Melee") { enchant.cost = 500000; }
			if (weaponType == "Missile") { enchant.cost = 1000000; }
			if (weaponType == "Projectile") { enchant.cost = 50000; }
		}
		if (rand2 == 5) {
			enchant.name = "Puissance +1";
			if (weaponType == "Melee") { enchant.cost = 5000; }
			if (weaponType == "Missile") { enchant.cost = 10000; }
			if (weaponType == "Projectile") { enchant.cost = 25; }
		}
		if (rand2 == 6) {
			enchant.name = "Puissance +1";
			if (weaponType == "Melee") { enchant.cost = 5000; }
			if (weaponType == "Missile") { enchant.cost = 10000; }
			if (weaponType == "Projectile") { enchant.cost = 25; }
		}
	}
	
	if (rand1 == 5) { 
		if (rand2 == 1) {
			enchant.name = "Puissance +2";
			if (weaponType == "Melee") { enchant.cost = 20000; }
			if (weaponType == "Missile") { enchant.cost = 40000; }
			if (weaponType == "Projectile") { enchant.cost = 100; }
		}
		if (rand2 == 2) {
			enchant.name = "Puissance +3";
			if (weaponType == "Melee") { enchant.cost = 100000; }
			if (weaponType == "Missile") { enchant.cost = 200000; }
			if (weaponType == "Projectile") { enchant.cost = 10000; }
		}
		if (rand2 == 3 || rand2 == 4) {
			enchant.name = "Quick-Aim (1 turn)";
			if (weaponType == "Melee") { enchant.cost = 50; }
			if (weaponType == "Missile") { enchant.cost = 100; }
			if (weaponType == "Projectile") { enchant.cost = 0; }
			enchant.desc = "Treat 'Melee Weapon' as 'Thrown Weapon' applicable only to throwable weapons such as knives and axes.";
		}
		if (rand2 == 5) {
			enchant.name = "Quick-Aim (2 turns)";
			if (weaponType == "Melee") { enchant.cost = 100; }
			if (weaponType == "Missile") { enchant.cost = 4000; }
			if (weaponType == "Projectile") { enchant.cost = 0; }
		}
		if (rand2 == 6) {
			enchant.name = "Quick-Draw";
			if (weaponType == "Melee") { enchant.cost = 6000; }
			if (weaponType == "Missile") { enchant.cost = 6000; }
			if (weaponType == "Projectile") { enchant.cost = 4000; }
			enchant.costPerPound = true;
			enchant.desc = "If the weapon is a projectile, the enchantment is on the quiver holding appropriate ammunition; cost is not per pound in this case.";
		}
	}
	
	if (rand1 == 6) { 
		if (rand2 == 1) {
			enchant.name = "Shatterproof";
			if (weaponType == "Melee") { enchant.cost = 8000; }
			if (weaponType == "Missile") { enchant.cost = 8000; }
			if (weaponType == "Projectile") { enchant.cost = 8000; }
		}
		if (rand2 == 2) {
			enchant.name = "Shatterproof";
			if (weaponType == "Melee") { enchant.cost = 8000; }
			if (weaponType == "Missile") { enchant.cost = 8000; }
			if (weaponType == "Projectile") { enchant.cost = 8000; }
		}
		if (rand2 == 3) {
			enchant.name = "Vital Seeker +1";
			if (weaponType == "Melee") { enchant.cost = 100; }
			if (weaponType == "Missile") { enchant.cost = 100; }
			if (weaponType == "Projectile") { enchant.cost = 10; }
		}
		if (rand2 == 4) {
			enchant.name = "Vital Seeker +1";
			if (weaponType == "Melee") { enchant.cost = 100; }
			if (weaponType == "Missile") { enchant.cost = 100; }
			if (weaponType == "Projectile") { enchant.cost = 10; }
			enchant.desc = "The weapon is enchanted to seek out important targets. The bonus offsets hit location penalties for targeting vitals (including through chinks in armor). For example, a sword with Vital Seeker +2 would only suffer a -1 penalty to hit the target’s vitals.";
		}
		if (rand2 == 5) {
			enchant.name = "Vital Seeker +2";
			if (weaponType == "Melee") { enchant.cost = 8000; }
			if (weaponType == "Missile") { enchant.cost = 8000; }
			if (weaponType == "Projectile") { enchant.cost = 40; }
			enchant.desc = "The weapon is enchanted to seek out important targets. The bonus offsets hit location penalties for targeting vitals (including through chinks in armor). For example, a sword with Vital Seeker +2 would only suffer a -1 penalty to hit the target’s vitals.";
		}
		if (rand2 == 6) {
			enchant.name = "Vital Seeker +3";
			if (weaponType == "Melee") { enchant.cost = 40000; }
			if (weaponType == "Missile") { enchant.cost = 40000; }
			if (weaponType == "Projectile") { enchant.cost = 4000; }
			enchant.desc = "The weapon is enchanted to seek out important targets. The bonus offsets hit location penalties for targeting vitals (including through chinks in armor). For example, a sword with Vital Seeker +2 would only suffer a -1 penalty to hit the target’s vitals.";
		}
	}

	return enchant;
}

function getArmorEnchant() {
	var rand1 = Math.ceil(Math.random()*36);
	
	
	var enchant = { "name":"", "cost":0, "costPerPound":false, "desc":"" }
	
	if (rand1 == 1) { enchant.name = "Fortify +1"; enchant.cost = 50; }
	if (rand1 == 2) { enchant.name = "Fortify +1"; enchant.cost = 50; }
	if (rand1 == 3) { enchant.name = "Fortify +1"; enchant.cost = 50; }
	if (rand1 == 4) { enchant.name = "Fortify +1"; enchant.cost = 50; }
	if (rand1 == 5) { enchant.name = "Fortify +1"; enchant.cost = 50; }
	if (rand1 == 6) { enchant.name = "Fortify +1"; enchant.cost = 50; }
	if (rand1 == 7) { enchant.name = "Lighten (25%)"; enchant.cost = 100; }
	if (rand1 == 8) { enchant.name = "Lighten (25%)"; enchant.cost = 100; }
	if (rand1 == 9) { enchant.name = "Lighten (25%)"; enchant.cost = 100; }
	if (rand1 == 10) { enchant.name = "Lighten (25%)"; enchant.cost = 100; }
	if (rand1 == 11) { enchant.name = "Lighten (25%)"; enchant.cost = 100; }
	if (rand1 == 12) { enchant.name = "Lighten (25%)"; enchant.cost = 100; }
	if (rand1 == 13) { enchant.name = "Deflect +1"; enchant.cost = 2000; }
	if (rand1 == 14) { enchant.name = "Deflect +1"; enchant.cost = 2000; }
	if (rand1 == 15) { enchant.name = "Deflect +1"; enchant.cost = 2000; }
	if (rand1 == 16) { enchant.name = "Deflect +1"; enchant.cost = 2000; }
	if (rand1 == 17) { enchant.name = "Deflect +1"; enchant.cost = 2000; }
	if (rand1 == 18) { enchant.name = "Deflect +1"; enchant.cost = 2000; }
	if (rand1 == 19) { enchant.name = "Deflect +1"; enchant.cost = 2000; }
	if (rand1 == 20) { enchant.name = "Deflect +1"; enchant.cost = 2000; }
	if (rand1 == 21) { enchant.name = "Lighten (50%)"; enchant.cost = 10000; }
	if (rand1 == 22) { enchant.name = "Lighten (50%)"; enchant.cost = 10000; }
	if (rand1 == 23) { enchant.name = "Lighten (50%)"; enchant.cost = 10000; }
	if (rand1 == 24) { enchant.name = "Lighten (50%)"; enchant.cost = 10000; }
	if (rand1 == 25) { enchant.name = "Fortify +2"; enchant.cost = 4000; }
	if (rand1 == 26) { enchant.name = "Fortify +2"; enchant.cost = 4000; }
	if (rand1 == 27) { enchant.name = "Fortify +2"; enchant.cost = 4000; }
	if (rand1 == 28) { enchant.name = "Deflect +2"; enchant.cost = 10000; }
	if (rand1 == 29) { enchant.name = "Deflect +2"; enchant.cost = 10000; }
	if (rand1 == 30) { enchant.name = "Deflect +2"; enchant.cost = 10000; }
	if (rand1 == 31) { enchant.name = "Fortify +3"; enchant.cost = 16000; }
	if (rand1 == 32) { enchant.name = "Deflect +3"; enchant.cost = 40000; }
	if (rand1 == 33) { enchant.name = "Fortify +4"; enchant.cost = 75000; }
	if (rand1 == 34) { enchant.name = "Deflect +4"; enchant.cost = 160000; }
	if (rand1 == 35) { enchant.name = "Fortify +5"; enchant.cost = 160000; }
	if (rand1 == 36) { enchant.name = "Deflect +5"; enchant.cost = 400000; }

	return enchant;
}

function generateFantasyItem(itemType, message, dontPrint, isGachapon) {
	if (itemType == "spice") {
		var nextLoot1 = Math.ceil(Math.random()*6);
		var nextLoot2 = Math.ceil(Math.random()*6);
		var spiceAmount = (Math.ceil(Math.random()*6))/2;
		var spiceName = "";
		var spicePrice = 0;
		var extraNotes = "";
		
		if (nextLoot1 == 1 && nextLoot2 == 1) { spiceName = "Allspice"; spicePrice = 150; }
		if (nextLoot1 == 1 && nextLoot2 == 2) { spiceName = "Anise"; spicePrice = 150; }					
		if (nextLoot1 == 1 && nextLoot2 == 3) { spiceName = "Annatto"; spicePrice = 113; }				
		if (nextLoot1 == 1 && nextLoot2 == 4) { spiceName = "Asafetida"; spicePrice = 75; }					
		if (nextLoot1 == 1 && nextLoot2 == 5) { spiceName = "Cardamom"; spicePrice = 150; }					
		if (nextLoot1 == 1 && nextLoot2 == 6) { spiceName = "Cassia"; spicePrice = 75; }

		if (nextLoot1 == 2 && nextLoot2 == 1) { spiceName = "Chiles"; spicePrice = 38; extraNotes = "An ounce of this, ground to powder and scattered in the user's path, will make anyone tracking him by scent have a fit of sneezing (p B428). Afterward, the tracker must wait an hour or make an HT-3 roll to recover."; }
		if (nextLoot1 == 2 && nextLoot2 == 2) { spiceName = "Cinnamon"; spicePrice = 150; extraNotes = "Well-known aphrodisiac. Consuming an ounce imposes -1 on any rolls to resist Lecherousness and seduction attempts for the next hour."; }
		if (nextLoot1 == 2 && nextLoot2 == 3) { spiceName = "Clove"; spicePrice = 150; }
		if (nextLoot1 == 2 && nextLoot2 == 4) { spiceName = "Coriander"; spicePrice = 150; extraNotes = "If an ounce of this is consumed within an hour before ingesting a poison, the user is at +1 to HT rolls to resist."; }
		if (nextLoot1 == 2 && nextLoot2 == 5) { spiceName = "Cumin"; spicePrice = 150; }
		if (nextLoot1 == 2 && nextLoot2 == 6) { spiceName = "Dwarven Savory Fungus"; spicePrice = 75; extraNotes = "Useful for strengthening the blood and speeding healing. Consuming an ounce a day gives +1 to daily HT rolls to recover lost HP."; }

		if (nextLoot1 == 3 && nextLoot2 == 1) { spiceName = "Elven Pepperbark"; spicePrice = 38; }
		if (nextLoot1 == 3 && nextLoot2 == 2) { spiceName = "Faerie Glimmerseed"; spicePrice = 270; extraNotes = "Highly prized, but the consumer of an ounce or more is at -1 to resist any mind-reading or mind-control attempts made in the next hour."; }					
		if (nextLoot1 == 3 && nextLoot2 == 3) { spiceName = "Fennel"; spicePrice = 75; }				
		if (nextLoot1 == 3 && nextLoot2 == 4) { spiceName = "Fenugreek"; spicePrice = 150; }					
		if (nextLoot1 == 3 && nextLoot2 == 5) { spiceName = "Ginger"; spicePrice = 38; extraNotes = "Aids digestion; an ounce acts as treatment to resist nausea (p B428) for an hour."; }					
		if (nextLoot1 == 3 && nextLoot2 == 6) { spiceName = "Halfling Savory"; spicePrice = 150; extraNotes = "Useful for strengthening the blood and speeding healing. Consuming an ounce a day gives +1 to daily HT rolls to recover lost HP."; }

		if (nextLoot1 == 4 && nextLoot2 == 1) { spiceName = "Huajiao (Szechuan Pepper)"; spicePrice = 150; extraNotes = "An ounce of this, ground to powder and scattered in the user's path, will make anyone tracking him by scent have a fit of sneezing (p B428). Afterward, the tracker must wait an hour or make an HT-3 roll to recover.";}
		if (nextLoot1 == 4 && nextLoot2 == 2) { spiceName = "Mace"; spicePrice = 225; }					
		if (nextLoot1 == 4 && nextLoot2 == 3) { spiceName = "Mustard"; spicePrice = 38; }				
		if (nextLoot1 == 4 && nextLoot2 == 4) { spiceName = "Nigella"; spicePrice = 75; extraNotes = "Balances humors and helps stabilize mood. Consuming an ounce gives +1 to resist sudden bursts of anger and rage, including the Berserk and Bloodlust disadvantages, for an hour."; }					
		if (nextLoot1 == 4 && nextLoot2 == 5) { spiceName = "Nutmeg"; spicePrice = 150; }					
		if (nextLoot1 == 4 && nextLoot2 == 6) { spiceName = "Onion Seed"; spicePrice = 38; }

		if (nextLoot1 == 5 && nextLoot2 == 1) { spiceName = "Orcish Firegrain"; spicePrice = 150; extraNotes = "Very mild stimulant. Anyone who ingests an ounce is at +1 HT to resist poisons that cause unconsciousness or fatigue damage for the next hour."; }
		if (nextLoot1 == 5 && nextLoot2 == 2) { spiceName = "Pepper, Black"; spicePrice = 150; extraNotes = "An ounce of this, ground to powder and scattered in the user's path, will make anyone tracking him by scent have a fit of sneezing (p B428). Afterward, the tracker must wait an hour or make an HT-3 roll to recover."; }					
		if (nextLoot1 == 5 && nextLoot2 == 3) { spiceName = "Pepper, White"; spicePrice = 188; extraNotes = "An ounce of this, ground to powder and scattered in the user's path, will make anyone tracking him by scent have a fit of sneezing (p B428). Afterward, the tracker must wait an hour or make an HT-3 roll to recover."; }				
		if (nextLoot1 == 5 && nextLoot2 == 4) { spiceName = "Poppy Seed"; spicePrice = 38; }					
		if (nextLoot1 == 5 && nextLoot2 == 5) { spiceName = "Saffron"; spicePrice = 300; }					
		if (nextLoot1 == 5 && nextLoot2 == 6) { spiceName = "Salt"; spicePrice = 15; extraNotes = "Tossing an ounce of salt gives clerics +1 to cast Turn Zombie and to Will rolls for True Faith to turn zombies."; }

		if (nextLoot1 == 6 && nextLoot2 == 1) { spiceName = "Salt, Black"; spicePrice = 38; extraNotes = "Tossing an ounce of salt gives clerics +1 to cast Turn Zombie and to Will rolls for True Faith to turn zombies."; }
		if (nextLoot1 == 6 && nextLoot2 == 2) { spiceName = "Salt, Red"; spicePrice = 38; extraNotes = "Tossing an ounce of salt gives clerics +1 to cast Turn Zombie and to Will rolls for True Faith to turn zombies."; }					
		if (nextLoot1 == 6 && nextLoot2 == 3) { spiceName = "Sumac"; spicePrice = 38; }				
		if (nextLoot1 == 6 && nextLoot2 == 4) { spiceName = "Tamarind"; spicePrice = 15; }					
		if (nextLoot1 == 6 && nextLoot2 == 5) { spiceName = "Turmeric"; spicePrice = 38; extraNotes = "Has antiseptic properties. Using an ounce of it while dressing wounds gives +1 to First Aid."; }					
		if (nextLoot1 == 6 && nextLoot2 == 6) { spiceName = "Zeodary"; spicePrice = 150; }
		
		var newMessage = "";
		
		iBasePrice = spicePrice;
		
		if (isGachapon) {
			newMessage = "\nYou played the Fantasy Gachapon for 1000 credits and got:";
		}
		
		if (!dontPrint) {
			if (extraNotes == "") {
				message.channel.send(message.author + newMessage + "\nSpice: " + spiceName + " (" + spiceAmount + "oz)\n*$" + spicePrice + "/oz*");
			}
			
			else if (extraNotes != "") {
				message.channel.send(message.author + newMessage + "\nSpice: " + spiceName + " (" + spiceAmount + "oz)\n*$" + spicePrice + "/oz*\n" + extraNotes);
			}
		}
		
		tempName = "Ounce of " + spiceName;
		
		newArgs = [ tempName, spicePrice, 0, 0, spiceAmount, "DF8, p11", extraNotes, iBasePrice];
		return newArgs;
		
	}
	
	if (itemType == "fiber") {
		var nextLoot1 = Math.ceil(Math.random()*6);
		var nextLoot2 = Math.ceil(Math.random()*6);
		
		var fiberBundle1 = Math.ceil(Math.random()*6);
		var fiberBundle2 = Math.ceil(Math.random()*6);
		var fiberPounds = (fiberBundle1 + fiberBundle2)*3;
		
		var itemName = "";
		var itemPrice = 0;
		var itemWeight = 0;
		var itemDesc;
		var implausibleMat = getImplausibleMat();
		
		
		if (nextLoot1 == 1 && nextLoot2 == 1) { itemName = "Otherworldly Cloth (" + implausibleMat.name + ")"; itemWeight = 7.5; itemPrice = 200 * (1.0 + implausibleMat.valueMod); itemDesc = "100-square-foot bolt. Made of " + implausibleMat.name + ". No immediate magical effect, but can be very valuable in complex magical work; it gives +2 to Merchant skill if sold or traded to an enchanter or other dealer in magical items.";}
		if (nextLoot1 == 1 && nextLoot2 == 2) { itemName = "Giant-Spider Silk Cloth"; itemWeight = 1; itemPrice = 565; itemDesc = "100-square-foot bolt. Very durable fabric, suitable for armor use."; }
		if (nextLoot1 == 1 && (nextLoot2 == 3 || nextLoot2 == 4)) { itemName = "Gauze Cloth"; itemWeight = 1.5; itemPrice = 5; itemDesc = "100-square-foot bolt. Extremely delicate cloth, much beloved of pixies (+1 Merchant skill if selling them gauze) but generally suitable only for decorative use."; }
		if (nextLoot1 == 1 && (nextLoot2 == 5 || nextLoot2 == 6)) { itemName = "Linen Cloth"; itemWeight = 2.5; itemPrice = 14; itemDesc = "100-square-foot bolt. Pure, fine, white cloth, often used in priestly garments."; }
		
		if (nextLoot1 == 2 && (nextLoot2 == 1 || nextLoot2 == 2)) { itemName = "Pashmina Wool Cloth"; itemWeight = 4; itemPrice = 45; itemDesc = "100-square-foot bolt. This kind of wool is rather expensive but makes particularly pleasant, warm garments with a light weight."; }
		if (nextLoot1 == 2 && (nextLoot2 == 3 || nextLoot2 == 4)) { itemName = "Plain Silk Cloth"; itemWeight = 2; itemPrice = 17; itemDesc = "100-square-foot bolt. Has a fairly coarse weave, but using a fine fiber."; }
		if (nextLoot1 == 2 && (nextLoot2 == 5 || nextLoot2 == 6)) { itemName = "Samite Cloth"; itemWeight = 3; itemPrice = 42.5; itemDesc = "100-square-foot bolt. A finer, slightly shiny cloth made from silk fibers."; }
		
		if (nextLoot1 == 3 && (nextLoot2 == 1 || nextLoot2 == 2)) { itemName = "Satin Cloth"; itemWeight = 2; itemPrice = 75; itemDesc = "100-square-foot bolt. An extremely fine, smooth silk fabric."; }
		if (nextLoot1 == 3 && (nextLoot2 == 3 || nextLoot2 == 4)) { itemName = "Velvet Cloth"; itemWeight = 5; itemPrice = 18; itemDesc = "100-square-foot bolt."; }
		if (nextLoot1 == 3 && nextLoot2 == 5) { itemName = "Wool Cloth"; itemWeight = 6; itemPrice = 15; itemDesc = "100-square-foot bolt. Utility-grade woolen fabric, warm and sturdy but heavy."; }
		if (nextLoot1 == 3 && nextLoot2 == 6) { 
			var furRand1 = Math.ceil(Math.random()*6);
			var furRand2 = Math.ceil(Math.random()*6);
			
			if ((furRand1 == 1 || furRand1 == 2 || furRand1 == 3) && furRand2 == 1) { itemName = "Common Fur (Seal)"; }
			if ((furRand1 == 1 || furRand1 == 2 || furRand1 == 3) && furRand2 == 2) { itemName = "Common Fur (Monkey)"; }
			if ((furRand1 == 1 || furRand1 == 2 || furRand1 == 3) && furRand2 == 3) { itemName = "Common Fur (Rabbit)"; }
			if ((furRand1 == 1 || furRand1 == 2 || furRand1 == 3) && furRand2 == 4) { itemName = "Common Fur (Fox)"; }
			if ((furRand1 == 1 || furRand1 == 2 || furRand1 == 3) && furRand2 == 5) { itemName = "Common Fur (Goat)"; }
			if ((furRand1 == 1 || furRand1 == 2 || furRand1 == 3) && furRand2 == 6) { itemName = "Common Fur (Horse)"; }
			if ((furRand1 == 4 || furRand1 == 5 || furRand1 == 6) && furRand2 == 1) { itemName = "Common Fur (Ox)"; }
			if ((furRand1 == 4 || furRand1 == 5 || furRand1 == 6) && furRand2 == 2) { itemName = "Common Fur (Deer)"; }
			if ((furRand1 == 4 || furRand1 == 5 || furRand1 == 6) && furRand2 == 3) { itemName = "Common Fur (Elk)"; }
			if ((furRand1 == 4 || furRand1 == 5 || furRand1 == 6) && furRand2 == 4) { itemName = "Common Fur (Reindeer)"; }
			if ((furRand1 == 4 || furRand1 == 5 || furRand1 == 6) && furRand2 == 5) { itemName = "Common Fur (Antelope)"; }
			if ((furRand1 == 4 || furRand1 == 5 || furRand1 == 6) && furRand2 == 6) { itemName = "Common Fur (Ibex)"; }
			
			itemWeight = 75; itemPrice = 200; itemDesc = "100-square-foot bundle of common animal pelt."; 			
		}
		
		if (nextLoot1 == 4 && (nextLoot2 == 1 || nextLoot2 == 2)) { 
			var furRand1 = Math.ceil(Math.random()*6);
			var furRand2 = Math.ceil(Math.random()*6);
			
			if ((furRand1 == 1 || furRand1 == 2 || furRand1 == 3) && furRand2 == 1) { itemName = "Exotic Fur (Sable)"; }
			if ((furRand1 == 1 || furRand1 == 2 || furRand1 == 3) && furRand2 == 2) { itemName = "Exotic Fur (Ermine)"; }
			if ((furRand1 == 1 || furRand1 == 2 || furRand1 == 3) && furRand2 == 3) { itemName = "Exotic Fur (Jaguar)"; }
			if ((furRand1 == 1 || furRand1 == 2 || furRand1 == 3) && furRand2 == 4) { itemName = "Exotic Fur (Lion)"; }
			if ((furRand1 == 1 || furRand1 == 2 || furRand1 == 3) && furRand2 == 5) { itemName = "Exotic Fur (Tiger)"; }
			if ((furRand1 == 1 || furRand1 == 2 || furRand1 == 3) && furRand2 == 6) { itemName = "Exotic Fur (Wolf)"; }
			if ((furRand1 == 4 || furRand1 == 5 || furRand1 == 6) && furRand2 == 1) { itemName = "Exotic Fur (Bear)"; }
			if ((furRand1 == 4 || furRand1 == 5 || furRand1 == 6) && furRand2 == 2) { itemName = "Exotic Fur (Wyvern)"; }
			if ((furRand1 == 4 || furRand1 == 5 || furRand1 == 6) && furRand2 == 3) { itemName = "Exotic Fur (Dire Wolf)"; }
			if ((furRand1 == 4 || furRand1 == 5 || furRand1 == 6) && furRand2 == 4) { itemName = "Exotic Fur (Cave Bear)"; }
			if ((furRand1 == 4 || furRand1 == 5 || furRand1 == 6) && furRand2 == 5) { itemName = "Exotic Fur (Giant Ape)"; }
			if ((furRand1 == 4 || furRand1 == 5 || furRand1 == 6) && furRand2 == 6) { itemName = "Exotic Fur (Frost Snake)"; }
			
			itemWeight = 75; itemPrice = 500; itemDesc = "100-square-foot bundle of exotic animal pelt."; 			
		}
		if (nextLoot1 == 4 && (nextLoot2 == 3 || nextLoot2 == 4)) { 
			var furRand1 = Math.ceil(Math.random()*6);
			var furRand2 = Math.ceil(Math.random()*6);
			
			if ((furRand1 == 1 || furRand1 == 2 || furRand1 == 3) && furRand2 == 1) { itemName = "Common Leather (Seal)"; }
			if ((furRand1 == 1 || furRand1 == 2 || furRand1 == 3) && furRand2 == 2) { itemName = "Common Leather (Monkey)"; }
			if ((furRand1 == 1 || furRand1 == 2 || furRand1 == 3) && furRand2 == 3) { itemName = "Common Leather (Rabbit)"; }
			if ((furRand1 == 1 || furRand1 == 2 || furRand1 == 3) && furRand2 == 4) { itemName = "Common Leather (Fox)"; }
			if ((furRand1 == 1 || furRand1 == 2 || furRand1 == 3) && furRand2 == 5) { itemName = "Common Leather (Goat)"; }
			if ((furRand1 == 1 || furRand1 == 2 || furRand1 == 3) && furRand2 == 6) { itemName = "Common Leather (Horse)"; }
			if ((furRand1 == 4 || furRand1 == 5 || furRand1 == 6) && furRand2 == 1) { itemName = "Common Leather (Ox)"; }
			if ((furRand1 == 4 || furRand1 == 5 || furRand1 == 6) && furRand2 == 2) { itemName = "Common Leather (Deer)"; }
			if ((furRand1 == 4 || furRand1 == 5 || furRand1 == 6) && furRand2 == 3) { itemName = "Common Leather (Elk)"; }
			if ((furRand1 == 4 || furRand1 == 5 || furRand1 == 6) && furRand2 == 4) { itemName = "Common Leather (Reindeer)"; }
			if ((furRand1 == 4 || furRand1 == 5 || furRand1 == 6) && furRand2 == 5) { itemName = "Common Leather (Antelope)"; }
			if ((furRand1 == 4 || furRand1 == 5 || furRand1 == 6) && furRand2 == 6) { itemName = "Common Leather (Ibex)"; }
			
			itemWeight = 50; itemPrice = 150; itemDesc = "100-square-foot bundle of common animal leather."; 			
		}
		if (nextLoot1 == 4 && (nextLoot2 == 5 || nextLoot2 == 6)) { 
			var furRand1 = Math.ceil(Math.random()*6);
			var furRand2 = Math.ceil(Math.random()*6);
			
			if ((furRand1 == 1 || furRand1 == 2 || furRand1 == 3) && furRand2 == 1) { itemName = "Exotic Leather (Sable)"; }
			if ((furRand1 == 1 || furRand1 == 2 || furRand1 == 3) && furRand2 == 2) { itemName = "Exotic Leather (Ermine)"; }
			if ((furRand1 == 1 || furRand1 == 2 || furRand1 == 3) && furRand2 == 3) { itemName = "Exotic Leather (Jaguar)"; }
			if ((furRand1 == 1 || furRand1 == 2 || furRand1 == 3) && furRand2 == 4) { itemName = "Exotic Leather (Lion)"; }
			if ((furRand1 == 1 || furRand1 == 2 || furRand1 == 3) && furRand2 == 5) { itemName = "Exotic Leather (Tiger)"; }
			if ((furRand1 == 1 || furRand1 == 2 || furRand1 == 3) && furRand2 == 6) { itemName = "Exotic Leather (Wolf)"; }
			if ((furRand1 == 4 || furRand1 == 5 || furRand1 == 6) && furRand2 == 1) { itemName = "Exotic Leather (Bear)"; }
			if ((furRand1 == 4 || furRand1 == 5 || furRand1 == 6) && furRand2 == 2) { itemName = "Exotic Leather (Wyvern)"; }
			if ((furRand1 == 4 || furRand1 == 5 || furRand1 == 6) && furRand2 == 3) { itemName = "Exotic Leather (Dire Wolf)"; }
			if ((furRand1 == 4 || furRand1 == 5 || furRand1 == 6) && furRand2 == 4) { itemName = "Exotic Leather (Cave Bear)"; }
			if ((furRand1 == 4 || furRand1 == 5 || furRand1 == 6) && furRand2 == 5) { itemName = "Exotic Leather (Giant Ape)"; }
			if ((furRand1 == 4 || furRand1 == 5 || furRand1 == 6) && furRand2 == 6) { itemName = "Exotic Leather (Frost Snake)"; }
			
			itemWeight = 50; itemPrice = 250; itemDesc = "100-square-foot bundle of exotic animal pelt."; 			
		}
		
		if (nextLoot1 == 5 && (nextLoot2 == 1 || nextLoot2 == 2)) { itemName = "Scale-Hide"; itemWeight = 50; itemPrice = 275; itemDesc = "100-square-foot bundle of scale leather."; }
		if (nextLoot1 == 5 && (nextLoot2 == 3 || nextLoot2 == 4)) { itemName = "Contraband Leather"; itemWeight = 25; itemPrice = 500; itemDesc = "100-square-foot bundle of leather made from the skin of a sapient being. Necromancers aside, any member of the offended race reacts to the owner at -4."; }
		if (nextLoot1 == 5 && nextLoot2 == 5) { itemName = "Otherworldly Leather"; itemWeight = 50; itemPrice = 1000; itemDesc = "100-square-foot-bundle of leather made from a supernatural entity of some kind. No immediate effect but is extremely valuable to the right people and gives +2 to Merchant skill if sold or traded to an enchanter or other dealer in magical items."; }
		
		
		if (nextLoot1 == 5 && nextLoot2 == 6) { itemName = "Linen Fiber"; itemWeight = fiberPounds; itemPrice = 0.25; itemDesc = "Price per pound."; }
		if (nextLoot1 == 6 && nextLoot2 == 1) { itemName = "Silk Fiber"; itemWeight = fiberPounds; itemPrice = 0.75; itemDesc = "Price per pound."; }
		if (nextLoot1 == 6 && nextLoot2 == 2) { itemName = "Wild Silk Fiber"; itemWeight = fiberPounds; itemPrice = 0.65; itemDesc = "Price per pound."; }
		if (nextLoot1 == 6 && nextLoot2 == 3) { itemName = "Wool Fiber"; itemWeight = fiberPounds; itemPrice = 0.20; itemDesc = "Price per pound."; }
		if (nextLoot1 == 6 && nextLoot2 == 4) { itemName = "Pashmina Wool Fiber"; itemWeight = fiberPounds; itemPrice = 0.60; itemDesc = "Price per pound."; }
		if (nextLoot1 == 6 && nextLoot2 == 5) { itemName = "Giant-Spider Silk Fiber"; itemWeight = fiberPounds; itemPrice = 10; itemDesc = "Price per pound."; }
		if (nextLoot1 == 6 && nextLoot2 == 6) { itemName = "Otherworldly Fiber (" + implausibleMat + ")"; itemWeight = fiberPounds; itemPrice = 7 * parseInt(getMaterialValue(implausibleMat)); itemDesc = "Price per pound."; }
		
		var newMessage = "";
		
		iBasePrice = itemPrice;
		
		if (isGachapon) {
			newMessage = "\nYou played the Fantasy Gachapon for 1000 credits and got:";
		}
		
		if (!dontPrint) {
			message.channel.send(message.author + newMessage + "\nFiber/Fabric: " + itemName + "\n" + itemWeight + " lbs. *$" + itemPrice + "*\n" + itemDesc);
		}
		
		if (itemDesc != "Price per pound.") {
			newArgs = [ itemName, itemPrice, itemWeight, 0, 1, "DF8, p12-13", itemDesc, iBasePrice];
		}
		else {
			newArgs = [ itemName, itemPrice, itemWeight, 0, fiberPounds, "DF8, p12-13", itemDesc, iBasePrice];
		}
		return newArgs;
	}
	
	if (itemType == "otherMats") {
		var nextLoot1 = Math.ceil(Math.random()*6);
		var nextLoot2 = Math.ceil(Math.random()*6);
		var quantity = (Math.ceil(Math.random()*6))+1
		
		var itemName = "";
		var itemUnit = "";
		var itemPrice = 0;
		var itemDesc = "";
		
		if (nextLoot1 == 1 && nextLoot2 == 1) { itemName = "Ale"; itemUnit = "gallon"; itemPrice = 5;}
		if (nextLoot1 == 1 && nextLoot2 == 2) { itemName = "Distilled Liquor"; itemUnit = "pint"; itemPrice = 16;}
		if (nextLoot1 == 1 && nextLoot2 == 3) { itemName = "Flavored Ale"; itemUnit = "gallon"; itemPrice = 7.5;}
		if (nextLoot1 == 1 && nextLoot2 == 4) { itemName = "Flavored Brandy"; itemUnit = "pint"; itemPrice = 20;}
		if (nextLoot1 == 1 && nextLoot2 == 5) { itemName = "Kumiz"; itemUnit = "gallon"; itemPrice = 15; itemDesc = "Fermented alcoholic milk.";}
		if (nextLoot1 == 1 && nextLoot2 == 6) { itemName = "Mead"; itemUnit = "gallon"; itemPrice = 11;}
		
		if (nextLoot1 == 2 && nextLoot2 == 1) { itemName = "Opium"; itemUnit = "ounce"; itemPrice = 20;}
		if (nextLoot1 == 2 && nextLoot2 == 2) { itemName = "Black Tea"; itemUnit = "ounce"; itemPrice = 2.25;}
		if (nextLoot1 == 2 && nextLoot2 == 3) { itemName = "Green Tea"; itemUnit = "ounce"; itemPrice = 2.25;}
		if (nextLoot1 == 2 && nextLoot2 == 4) { itemName = "Date Wine"; itemUnit = "gallon"; itemPrice = 9;}
		if (nextLoot1 == 2 && nextLoot2 == 5) { itemName = "Grape Wine"; itemUnit = "gallon"; itemPrice = 9;}
		if (nextLoot1 == 2 && nextLoot2 == 6) { itemName = "Rice Wine"; itemUnit = "gallon"; itemPrice = 8;}
		
		if (nextLoot1 == 3 && nextLoot2 == 1) { itemName = "Otherworldly Wine"; itemUnit = "gallon"; itemPrice = 20; itemDesc = "Produced from an unusual substance. No immediate supernatural properties like a potion, but might be a valuable alchemical ingredient or a treatment for a specific magical affliction.";}
		if (nextLoot1 == 3 && nextLoot2 == 2) { itemName = "Sealing Wax"; itemUnit = "ounce"; itemPrice = 1.25;}
		if (nextLoot1 == 3 && nextLoot2 == 3) { itemName = "Ambergris"; itemUnit = "ounce"; itemPrice = 35;}
		if (nextLoot1 == 3 && nextLoot2 == 4) { itemName = "Cedar Resin"; itemUnit = "ounce"; itemPrice = 10;}
		if (nextLoot1 == 3 && nextLoot2 == 5) { itemName = "Copal"; itemUnit = "ounce"; itemPrice = 11;}
		if (nextLoot1 == 3 && nextLoot2 == 6) { itemName = "Frankincense"; itemUnit = "ounce"; itemPrice = 16;}
		
		if (nextLoot1 == 4 && nextLoot2 == 1) { itemName = "Musk"; itemUnit = "ounce"; itemPrice = 28;}
		if (nextLoot1 == 4 && nextLoot2 == 2) { itemName = "Myrrh"; itemUnit = "ounce"; itemPrice = 15;}
		if (nextLoot1 == 4 && nextLoot2 == 3) { itemName = "Onycha"; itemUnit = "ounce"; itemPrice = 20;}
		if (nextLoot1 == 4 && nextLoot2 == 4) { itemName = "Patchouli"; itemUnit = "ounce"; itemPrice = 9;}
		if (nextLoot1 == 4 && nextLoot2 == 5) { itemName = "Sandalwood Gum"; itemUnit = "ounce"; itemPrice = 8.5;}
		if (nextLoot1 == 4 && nextLoot2 == 6) { itemName = "Flower Water"; itemUnit = "ounce"; itemPrice = 5; itemDesc = "Water lightly scented with flowers.";}
		
		if (nextLoot1 == 5 && nextLoot2 == 1) { itemName = "Perfumed Essence"; itemUnit = "ounce"; itemPrice = 12; itemDesc = "An alcohol solution scented with flowers, spices, and/or resins.";}
		if (nextLoot1 == 5 && nextLoot2 == 2) { itemName = "Perfumed Oil"; itemUnit = "ounce"; itemPrice = 8; itemDesc = "A perfumed vegetable oil or animal fat.";}
		if (nextLoot1 == 5 && nextLoot2 == 3) { itemName = "Pomander"; itemUnit = "ounce"; itemPrice = 9; itemDesc = "An object coated with or containing perfume elements.";}
		if (nextLoot1 == 5 && nextLoot2 == 4) { itemName = "Carmine"; itemUnit = "ounce"; itemPrice = 40; itemDesc = "A very expensive insect-derived red.";}
		if (nextLoot1 == 5 && nextLoot2 == 5) { itemName = "Cinnabar"; itemUnit = "ounce"; itemPrice = 18; itemDesc = "A vivid-red mineral pigment.";}
		if (nextLoot1 == 5 && nextLoot2 == 6) { itemName = "Ochre"; itemUnit = "ounce"; itemPrice = 0.75;}
		
		if (nextLoot1 == 6 && nextLoot2 == 1) { itemName = "Henna"; itemUnit = "ounce"; itemPrice = 1;}
		if (nextLoot1 == 6 && nextLoot2 == 2) { itemName = "Indigo"; itemUnit = "ounce"; itemPrice = 32; itemDesc = "A deep-blue vegetable dye.";}
		if (nextLoot1 == 6 && nextLoot2 == 3) { itemName = "Madder"; itemUnit = "ounce"; itemPrice = 2; itemDesc = "A relatively inexpensive red vegetable dye.";}
		if (nextLoot1 == 6 && nextLoot2 == 4) { itemName = "Murex"; itemUnit = "ounce"; itemPrice = 29; itemDesc = "A rare purple-red derived from mollusks.";}
		if (nextLoot1 == 6 && nextLoot2 == 5) { itemName = "Orpiment"; itemUnit = "ounce"; itemPrice = 22; itemDesc = "A yellow mineral pigment.";}
		if (nextLoot1 == 6 && nextLoot2 == 6) { itemName = "Woad"; itemUnit = "ounce"; itemPrice = 2.75; itemDesc = "A pale-blue vegetable dye related to indigo.";}
		
		var newMessage = "";
		
		if (isGachapon) {
			newMessage = "\nYou played the Fantasy Gachapon for 1000 credits and got:";
		}
		
		iBasePrice = itemPrice;
		
		if (!dontPrint) {
			if (itemDesc != "") {
				if (quantity != 1) {
					message.channel.send(message.author + newMessage + "\nOther Materials: " + itemName + "\n" + quantity + " " + itemUnit + "s, *$" + itemPrice + "/" + itemUnit + "*\n" + itemDesc);
				}
				
				else if (quantity == 1) {
					message.channel.send(message.author + newMessage + "\nOther Materials: " + itemName + "\n" + quantity + " " + itemUnit + ", *$" + itemPrice + "/" + itemUnit + "*\n" + itemDesc);
				}
			}
			
			else if (itemDesc == "") {
				if (quantity != 1) {
					message.channel.send(message.author + newMessage + "\nOther Materials: " + itemName + "\n" + quantity + " " + itemUnit + "s, *$" + itemPrice + "/" + itemUnit + "*");
				}
				
				else if (quantity == 1) {
					message.channel.send(message.author + newMessage + "\nOther Materials: " + itemName + "\n" + quantity + " " + itemUnit + ", *$" + itemPrice + "/" + itemUnit + "*");
				}
			}
			
		}
		newItemDesc = "Each unit of quantity is: " + itemUnit + ". " + itemDesc;
		
		newArgs = [ itemName, itemPrice, 0.0625, 0, quantity, "DF8, p13", newItemDesc, iBasePrice];
		return newArgs;
		
		
	}
	
	if (itemType == "household") {

		var nextLoot1 = Math.ceil(Math.random()*6);
		var nextLoot2 = Math.ceil(Math.random()*6);
		var nextLoot3 = Math.ceil(Math.random()*6);
		
		var rugArea1 = Math.ceil(Math.random()*6);
		var rugArea2 = Math.ceil(Math.random()*6);
		var rugArea3 = Math.ceil(Math.random()*6);
		var rugArea = (rugArea1 + rugArea2)+2;
		var tapestryArea = (rugArea1 + rugArea2 + rugArea3)+10;
		
		var itemName = "";
		var itemWeight = 0;
		var itemPrice = 0;
		var itemDesc = "";
		
		var householdItems = [
			{ "n":"Basin", "w":4, "p":3, "d":"A wide, open bowl (two gallons), appropriate for large quantities of soup, washing up, or draining the blood of sacrificial victims.", "e":"hard"},
			{ "n":"Bowl", "w":0.3, "p":1, "d":"A small ceramic bowl suitable for individual meals.", "e":"hard"},
			{ "n":"Bucket", "w":4, "p":15, "d":"With rope handle. Holds 1 gallon of liquid (8 lbs. if water).", "e":"hard"},
			{ "n":"Cauldron", "w":20, "p":180, "d":"A blackened iron cooking pot with a capacity of about four gallons.", "e":"hard"},
			{ "n":"Chopsticks", "w":0, "p":1, "e":"hard"},
			{ "n":"Cup", "w":0.15, "p":0.5, "e":"hard"},
		
			{ "n":"Dinner Plate", "w":0.5, "p":2, "e":"hard"},
			{ "n":"Drinking Set", "w":3, "p":7, "d":"A set of drinking paraphernalia for four.", "e":"hard"},
			{ "n":"Cooking Fork", "w":2, "p":10, "d":"A heavy fork, about a foot long, good for holding roasts during carving, or piercing and lifting large vegetables. If used as a weapon, does thr-1 imp; use Knife skill at -2.", "e":"hard"},
			{ "n":"Table Fork", "w":0.4, "p":3, "d":"If used as a weapon, does thr-3 imp; use Knife skill at -2", "e":"hard"},
			{ "n":"Goblet", "w":0.5, "p":5, "d":"A large (at least one-pint capacity), footed cup.", "e":"hard"},
			{ "n":"Table Knife", "w":0.4, "p":2.5, "d":"A dull knife, not sharp enough for cutting damage or pointed enough for impaling.", "e":"hard"},
			
			{ "n":"Ladle", "w":2, "p":9, "e":"hard"},
			{ "n":"Mortar and Pestle", "w":6, "p":20, "d":"About a pint capacity.", "e":"hard"},
			{ "n":"Pitcher", "w":3, "p":2.5, "d":"Half-gallon capacity.", "e":"hard"},
			{ "n":"Place Setting", "w":2, "p":5, "d":"A set of dishes and eating utensils.", "e":"hard"},
			{ "n":"Platter", "w":1, "p":0.75, "e":"hard"},
			{ "n":"Pot", "w":2, "p":30, "d":"A lightweight cooking pot, holding about two quarts.", "e":"hard"},
			
			{ "n":"Skillet", "w":8, "p":50, "d":"A 12-inch pan for cooking.", "e":"hard"},
			{ "n":"Cooking Spit", "w":15, "p":100, "d":"A pointed metal bar large enough to cook a whole goat or sheep. Does not include posts to set it up on.", "e":"hard"},
			{ "n":"Tea Set", "w":4.5, "p":6, "d":"A pot for brewing and four small cups.", "e":"hard"},
			{ "n":"Teapot", "w":7, "p":45, "e":"hard"},
			{ "n":"Wine Glass", "w":0.5, "p":10, "e":"hard"},
			{ "n":"Banner", "w":0.5, "p":5, "d":"A flag large enough to cover a person or table.", "e":"soft"},
			
			{ "n":"Brazier", "w":3, "p":65, "d":"An open metal dish on a pedestal or short legs, for heating or cooking.", "e":"hard"},
			{ "n":"Candlesticks", "w":1, "p":4, "e":"hard"},
			{ "n":"Censer", "w":2, "p":60, "d":"A small, enclosed brazier used for incense, often on a chain so that it can be hung or carried around.", "e":"hard"},
			{ "n":"Complex Clock", "w":25, "p":1000, "d":"A timepiece driven by water or a pendulum. Keeps time for eight hours before it requires resetting the pendulum or refilling the tank. Has some kind of elaborate display that may involve multiple hands, moving displays of celestial bodies, ringing bells, etc. The clock becomes very inaccurate quickly if moved while it is functioning.", "e":"hard"},
			{ "n":"Simple Clock", "w":15, "p":400, "d":"An unremarkable timepiece driven by water or a pendulum. Keeps time for eight hours before it requires resetting the pendulum or refilling the tank. This clock doesn't keep time well when it is moved.", "e":"hard"},
			{ "n":"Music Box", "w":2, "p":120, "d":"Plays a tune with small chimes when the crank is turned.", "e":"hard"},
			
			{ "n":"Encaustic Painting", "w":0.75, "p":700, "d":"Pigments are mixed with hot wax for vibrant color and interesting texture.", "e":"soft"},
			{ "n":"Oil Painting", "w":0.75, "p":600, "e":"soft"},
			{ "n":"Watercolor Painting", "w":0.75, "p":400, "e":"soft"},
			{ "n":"Pennant", "w":0.1, "p":1, "d":"A colored strip of cloth, appropriate for tying to the end of a lance or spear.", "e":"soft"},
			
			{ "n":"Clay Sculpture", "w":95, "p":200, "e":"hard"},
			{ "n":"Clay Sculpture", "w":95, "p":200, "e":"hard"},
			{ "n":"Bone Sculpture", "w":38, "p":240, "e":"hard"},
			{ "n":"Porcelain Sculpture", "w":100, "p":380, "e":"hard"},
			{ "n":"Stone Sculpture", "w":165, "p":430, "e":"hard"},
			{ "n":"Stone Sculpture", "w":165, "p":430, "e":"hard"},
			{ "n":"Wood Sculpture", "w":32, "p":175, "e":"hard"},
			{ "n":"Wood Sculpture", "w":32, "p":175, "e":"hard"},
			{ "n":"Ivory Sculpture", "w":45, "p":460, "e":"hard"},
		
			{ "n":"Bench", "w":25, "p":160, "d":"Simple seating for two or three people, with no arms, back, or padding.", "e":"hard"},
			{ "n":"Chair", "w":12, "p":90, "d":"A solid chair with a back and possible arms.", "e":"hard"},
			{ "n":"Folding Chair", "w":8, "p":75, "d":"A chair that collapses for transport, much like a modern director's chair.", "e":"hard"},
			{ "n":"Couch", "w":140, "p":600, "d":"Well-padded seating for two or three, with arms and a back.", "e":"hard"},
			{ "n":"Footstool", "w":3, "p":30, "d":"Better versions are cushioned.", "e":"hard"},
			
			{ "n":"Headrest", "w":3, "p":8, "d":"Carved wooden headrest used by some societies instead of a pillow.", "e":"hard"},
			{ "n":"Mattress", "w":30, "p":850, "d":"A thick sack filled with feathers. Can be rolled up for transport, cost and weight are for a single-person bed.", "e":"soft"},
			{ "n":"Pillow", "w":2, "p":70, "e":"soft"},
			{ "n":"Rug", "w":1, "p":45, "d":"A piece of fabric heavy enough to stand up to foot traffic. " + rugArea + " square feet, price and weight are by square foot.", "e":"soft"},
			{ "n":"Carpet", "w":1, "p":45, "d":"A piece of fabric heavy enough to stand up to foot traffic. " + rugArea + " square feet, price and weight are by square foot.", "e":"soft"},
			{ "n":"Sheets", "w":2, "p":35, "e":"soft"},
			
			{ "n":"Table", "w":18, "p":120, "e":"hard"},
			{ "n":"Folding Table", "w":15, "p":90, "e":"hard"},
			{ "n":"Tapestry", "w":0.75, "p":40, "d":"A decorate wall hanging. Cost and weight are per square foot. " + tapestryArea + " square feet, price and weight are by square foot.", "e":"soft"},
			{ "n":"Tapestry", "w":0.75, "p":40, "d":"A decorate wall hanging. Cost and weight are per square foot. " + tapestryArea + " square feet, price and weight are by square foot.", "e":"soft"},
			{ "n":"Tub", "w":100, "p":300, "e":"hard"},
			{ "n":"Brush", "w":0.5, "p":6, "e":"hard"},
			
			{ "n":"Comb", "w":0.2, "p":3, "e":"hard"},
			{ "n":"Cosmetics", "w":0, "p":40, "d":"Pigments, often in a fatty base. " + rugArea1/2 + " ounces, price is per ounce. Contained in a small box.", "e":"hard"},
			{ "n":"Grooming Kit", "w":3, "p":80, "d":"Brush, comb, razor, or small scissors, and a few small pots of soaps, cosmetics, and/or perfumes.", "e":"hard"},
			{ "n":"Razor", "w":0.1, "p":30, "d":"Can be used in close combat with Knife skill, but cannot parry; does thr-2 cut", "e":"hard"},
			{ "n":"Scissors", "w":0.3, "p":35, "d":"Small (2-3 inch blades) scissors for grooming or light cloth work.", "e":"hard"},
			{ "n":"Strigil", "w":0.5, "p":6, "d":"A dull, hooked blade for scraping dirt and oil off the skin.", "e":"hard"},
		
			{ "n":"Fan", "w":0.25, "p":10, "d":"Thin wooden slats or cloth or paper glued to a wooden skeleton.", "e":"soft"},
			{ "n":"Mask", "w":0.25, "p":25, "d":"A cloth domino or similarly shaped papier-mâché mask covering the area around the eyes.", "e":"hard"},
			{ "n":"Parasol", "w":3, "p":12, "d":"Protection from the sun and rain for one person.", "e":"soft"},
			{ "n":"Canopy Parasol", "w":27, "p":108, "d":"A large (three-yard diameter) parasol, typically carried by one person to shelter another.", "e":"soft"},
			{ "n":"Pipe", "w":0.25, "p":1, "d":"For smoking tobacco or medicinal herbs.", "e":"hard"},
			{ "n":"Seal", "w":1.5, "p":55, "d":"An elaborate pattern cavern into a solid object to press on sealing wax.", "e":"hard"},
		
			{ "n":"Staff", "w":5, "p":7, "d":"A long stick for walking. Not balanced for combat but could be used as a quarterstaff at -2.", "e":"hard"},
			{ "n":"Tinderbox", "w":0, "p":2, "d":"Fire-starting equipment, such as a flint and steel, and finely divided tinder to get small flames going.", "e":"hard"},
			{ "n":"Walking Stick", "w":2, "p":4, "d":"A short walking stick. If used in combat, treat as a cheap baton.", "e":"hard"},
			{ "n":"Wand", "w":1, "p":3, "d":"Used for pointing or as a status symbol. Not inherently magical.", "e":"hard"},
			{ "n":"Water Pipe", "w":4, "p":35, "d":"A large, freestanding pipe with two mouthpieces on the end of flexible tubes.", "e":"hard"},
			{ "n":"Large Wig", "w":6, "p":500, "d":"An enormous hairpiece, extending well down the back or piled up several inches over the wearer's head.", "e":"supernatural"},
			
			{ "n":"Small Wig", "w":1, "p":75, "d":"A hairpiece that simply covers the head.", "e":"supernatural"},
			{ "n":"Abacus", "w":2, "p":50, "e":"hard"},
			{ "n":"Armillary Sphere", "w":4, "p":200, "d":"A model of a planet and its surrounding heavenly bodies, constructed as a series of concentric openwork spheres.", "e":"hard"},
			{ "n":"Astrolabe", "w":5, "p":250, "d":"A complex navigational instrument made from stacks of thin, inscribed plates indicating angles, paths of stars, and mathematical calculations. Gives +2 to Cartography and Navigation skills.", "e":"hard"},
			{ "n":"Globe", "w":2, "p":25, "d":"A world map on a sphere. Probably but not necessarily a map of the planet this was found on.", "e":"hard"},
			{ "n":"Orerry", "w":12, "p":750, "d":"A model of a solar system with planets rotating around a central body. It is geared like a clock so that the planets retain the proper spatial relationships to one another.", "e":"hard"},
			
			{ "n":"Quadrant", "w":3, "p":35, "d":"A quarter-circle-shaped navigational device. By sighting through a low-power spyglass along one side and consulting an attached plumb bob, the user can determine visual angles. Gives +1 to Cartography and Navigation skills (may not be combined with an astrolabe).", "e":"hard"},
			{ "n":"Knitting Needles", "w":0, "p":5, "e":"hard"},
			{ "n":"Loom", "w":5, "p":36, "d":"A portable backstrap loom rather than a rigid frame loom.", "e":"hard"},
			{ "n":"Sewing Needles", "w":0, "p":15, "d":"Four needles in a card or pincushion.", "e":"hard"},
			{ "n":"Spindle", "w":0.5, "p":4, "e":"hard"},
			{ "n":"Spinning Wheel", "w":40, "p":100, "e":"hard"},
			
			{ "n":"Ball", "w":0.25, "p":3, "e":"hard"},
			{ "n":"Dice", "w":0, "p":6, "d":"Common low-tech randomizer.", "e":"hard"},
			{ "n":"Game Board and Pieces", "w":3, "p":40, "d":"Chess, checkers, backgammon, go, or another game with a playing board and pieces.", "e":"hard"},
			{ "n":"Game Tiles", "w":1.5, "p":22, "d":"Complex pieces for games that don't require boards, such as dominoes and mahjong.", "e":"hard"},
			{ "n":"Kite", "w":2, "p":33, "e":"hard"},
			{ "n":"Playing Cards", "w":0.25, "p":50, "d":"Nonmagical (unless enhancements are added) but used for divination as well as gambling.", "e":"hard"},
			
			{ "n":"Toy Top", "w":0.1, "p":3, "e":"hard"},
			{ "n":"Toy Boat", "w":0.5, "p":4, "e":"hard"},
			{ "n":"Toy Castle", "w":0.5, "p":4, "e":"hard"},
			{ "n":"Toy Wagon", "w":0.5, "p":4, "e":"hard"},
			{ "n":"Toy House", "w":0.5, "p":4, "e":"hard"},
			{ "n":"Toy Monster", "w":0.5, "p":4, "d":"A toy dragon, demon, Elder Thing, or other extremely dangerous creature.", "e":"hard"},
			
			{ "n":"Toy Animal", "w":0.5, "p":4, "d":"A dog, bird, fish, barnyard animal, or other beast one might see in everyday life.", "e":"hard"},
			{ "n":"Toy Person", "w":0.5, "p":4, "d":"A baby, identifiable craftsman, priest, or warrior, or simply a generic humanoid doll.", "e":"hard"},
			{ "n":"Toy Soldier", "w":0.5, "p":4, "e":"hard"},
			{ "n":"Toy Exotic Animal", "w":0.5, "p":4, "d":"A lion, giraffe, or other rare but mundane animal.", "e":"hard"},
			{ "n":"Toy Weapon", "w":0.5, "p":4, "e":"hard"},
			{ "n":"Toy Nesting Dolls", "w":0.5, "p":4, "e":"hard"}
		];
		
		var rand1 = Math.ceil(Math.random()*(householdItems.length-1));
		
		itemName = householdItems[rand1].n;
		itemWeight = householdItems[rand1].w;
		itemPrice = householdItems[rand1].p;
		if (householdItems[rand1].d != null) { itemDesc = householdItems[rand1].d; }
		else { itemDesc = ""; }
		
		iBasePrice = itemPrice;
		
	
		var newMessage = "";
		
		if (isGachapon) {
			newMessage = "\nYou played the Fantasy Gachapon for 1000 credits and got:";
		}
		
		var randType = Math.ceil(Math.random()*12);
		
		if (randType == 7 || randType == 8 || randType == 9) {
			var embellishment = getDF8Embellishment(householdItems[rand1].e);
			var motif = getMotif();
			
			randEmb = Math.ceil(Math.random()*3);
			if (randEmb == 1) { var embellishment = getDF8Embellishment(householdItems[rand1].e); } else { var embellishment = getDF8Embellishment("supernatural"); }
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment.n + ". " + embellishment.d;
			if (embellishment.n != "Fine Material" && embellishment.n != "Exceptional Material" && randEmb == 1 && embellishment.n != "Silver Plating" && embellishment.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif + "."; }
			itemPrice = itemPrice * (1 + embellishment.v);
		}
		
		if (randType == 10 || randType == 11) {
			randEmb = Math.ceil(Math.random()*3);
			if (randEmb == 1) { var embellishment = getDF8Embellishment(householdItems[rand1].e); } else { var embellishment = getDF8Embellishment("supernatural"); }
			var motif = getMotif();
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment.n + ". " + embellishment.d;
			if (embellishment.n != "Fine Material" && embellishment.n != "Exceptional Material" && randEmb == 1 && embellishment.n != "Silver Plating" && embellishment.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif + "."; }
			
			randEmb2 = Math.ceil(Math.random()*3);
			if (randEmb2 == 1) { var embellishment2 = getDF8Embellishment(householdItems[rand1].e); } else { var embellishment2 = getDF8Embellishment("supernatural"); }
			var motif2 = getMotif();
			
			while (embellishment2 == embellishment || (embellishment.n.includes("Made of") && embellishment2.n.includes("Made of"))) {
				randEmb2 = Math.ceil(Math.random()*3);
				if (randEmb2 == 1) { var embellishment2 = getDF8Embellishment(householdItems[rand1].e); } else { var embellishment2 = getDF8Embellishment("supernatural"); }
				var motif2 = getMotif();
			}
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment2.n + ". " + embellishment2.d;
			if (embellishment2.n != "Fine Material" && embellishment2.n != "Exceptional Material" && randEmb2 == 1 && embellishment2.n != "Silver Plating" && embellishment2.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif2 + "."; }
			itemPrice = itemPrice * (1 + embellishment.v + embellishment2.v);
			
		}
		
		if (randType == 12) {
			randEmb = Math.ceil(Math.random()*3);
			if (randEmb == 1) { var embellishment = getDF8Embellishment(householdItems[rand1].e); } else { var embellishment = getDF8Embellishment("supernatural"); }
			var motif = getMotif();
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment.n + ". " + embellishment.d;
			if (embellishment.n != "Fine Material" && embellishment.n != "Exceptional Material" && randEmb == 1 && embellishment.n != "Silver Plating" && embellishment.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif + "."; }
			
			randEmb2 = Math.ceil(Math.random()*3);
			if (randEmb2 == 1) { var embellishment2 = getDF8Embellishment(householdItems[rand1].e); } else { var embellishment2 = getDF8Embellishment("supernatural"); }
			var motif2 = getMotif();
			
			while (embellishment2 == embellishment || (embellishment.n.includes("Made of") && embellishment2.n.includes("Made of"))) {
				randEmb2 = Math.ceil(Math.random()*3);
				if (randEmb2 == 1) { var embellishment2 = getDF8Embellishment(householdItems[rand1].e); } else { var embellishment2 = getDF8Embellishment("supernatural"); }
				var motif2 = getMotif();
			}
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment2.n + ". " + embellishment2.d;
			if (embellishment2.n != "Fine Material" && embellishment2.n != "Exceptional Material" && randEmb2 == 1 && embellishment2.n != "Silver Plating" && embellishment2.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif2 + "."; }
			
			
			randEmb3 = Math.ceil(Math.random()*3);
			if (randEmb3 == 1) { var embellishment3 = getDF8Embellishment(householdItems[rand1].e); } else { var embellishment3 = getDF8Embellishment("supernatural"); }
			var motif3 = getMotif();
			
			while ((embellishment3 == embellishment2 || embellishment3 == embellishment) || (embellishment.n.includes("Made of") && embellishment3.n.includes("Made of")) || (embellishment2.n.includes("Made of") && embellishment3.n.includes("Made of"))) {
				randEmb3 = Math.ceil(Math.random()*3);
				if (randEmb3 == 1) { var embellishment3 = getDF8Embellishment(householdItems[rand1].e); } else { var embellishment3 = getDF8Embellishment("supernatural"); }
				var motif2 = getMotif();
			}
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment3.n + ". " + embellishment3.d;
			if (embellishment3.n != "Fine Material" && embellishment3.n != "Exceptional Material" && randEmb3 == 1 && embellishment3.n != "Silver Plating" && embellishment3.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif3 + "."; }
			
			
			
			itemPrice = itemPrice * (1 + embellishment.v + embellishment2.v + embellishment3.v);
		}
		
		itemPrice = Math.ceil(itemPrice);
		
		if (!dontPrint) {
			message.channel.send(message.author + newMessage + "\nHousehold Item: " + itemName + "\n" + itemWeight + " lbs. *$" + itemPrice + "*\n" + itemDesc);
		}	
		newArgs = [ itemName, itemPrice, itemWeight, 4, 1, "DF8, p14-16", itemDesc, iBasePrice];


		return newArgs;
	}

	if (itemType == "gem") {
		gem = getGem();
		
		var newMessage = "";
		
		if (isGachapon) {
			newMessage = "\nYou played the Fantasy Gachapon for 1000 credits and got:";
		}
		
		if (!dontPrint) {
			message.channel.send(message.author + newMessage + "\nGem: " + gem.name + "\n" + gem.carats + " carats. *$" + gem.value + "*");
		}
		newItemDesc = gem.carats + " carats.";
		
		iBasePrice = gem.value
		
		newArgs = [ gem.name, gem.value, 0, 0, 1, "DF8, p19-20", newItemDesc, iBasePrice];
		return newArgs;
	}
	
	if (itemType == "jewelry") {
		var materialRand1 = Math.ceil(Math.random()*6);
		var materialRand2 = Math.ceil(Math.random()*6);
		var materialRand = materialRand1 + materialRand2;
		var material = "";
		var gems = "";
		
		var itemShape = "";
		var itemPrice = 0;
		var gemAdd = 0;
		var itemWeight = 0;
		var itemDesc = "";
		var biggerMultiplier = 1;
		var loopIt = true;
		
		if (materialRand == 2 || materialRand == 3 || materialRand == 4) { material = "Bronze"; }
		if (materialRand == 5) { material = "Billon"; }
		if (materialRand == 6 || materialRand == 7 || materialRand == 8) { material = "Silver"; }
		if (materialRand == 9) { material = "Electrum"; }
		if (materialRand == 10 || materialRand == 11) { material = "Gold"; }
		if (materialRand == 12) { material = "Platinum"; }
		
		var nextLoot1 = Math.ceil(Math.random()*6);
		var nextLoot2 = Math.ceil(Math.random()*6);
		
		while (loopIt) {
			if (nextLoot1 == 6 && (nextLoot2 == 1 || nextLoot2 == 2 || nextLoot2 == 3)) {
				biggerMultiplier = biggerMultiplier + 1;
				var nextLoot1 = Math.ceil(Math.random()*6);
				var nextLoot2 = Math.ceil(Math.random()*6);
			}
			
			else if (nextLoot1 == 6 && (nextLoot2 == 4 || nextLoot2 == 5 || nextLoot2 == 6)) {
				var newGem = getGem();
				gemAdd += newGem.value;
				if (gems == "") {
					gems = "Inlaid with " + newGem.name + " (" + newGem.carats + " carats)";
				}
				
				else if (gems != "") {
					gems = gems + ", " + newGem.name + " (" + newGem.carats + " carats)";
				}
				
				var nextLoot1 = Math.ceil(Math.random()*6);
				var nextLoot2 = Math.ceil(Math.random()*6);
			}
			
			else {
				loopIt = false;
				if (nextLoot1 == 1) {
					itemShape = "Piercing";
					if (material == "Bronze") { itemPrice = 0.65; itemWeight = 0.01; }
					if (material == "Billon") { itemPrice = 5.75; itemWeight = 0.01; }
					if (material == "Silver") { itemPrice = 12; itemWeight = 0.01; }
					if (material == "Electrum") { itemPrice = 170; itemWeight = 0.05; }
					if (material == "Gold") { itemPrice = 420; itemWeight = 0.02; }
					if (material == "Platinum") { itemPrice = 960; itemWeight = 0.02; }
				}
				
				if (nextLoot1 == 2) {
					itemShape = "Chain";
					if (material == "Bronze") { itemPrice = 2.5; itemWeight = 0.04; }
					if (material == "Billon") { itemPrice = 22.75; itemWeight = 0.05; }
					if (material == "Silver") { itemPrice = 45; itemWeight = 0.05; }
					if (material == "Electrum") { itemPrice = 685; itemWeight = 0.1; }
					if (material == "Gold") { itemPrice = 1680; itemWeight = 0.1; }
					if (material == "Platinum") { itemPrice = 3840; itemWeight = 0.1; }
				}
				
				if (nextLoot1 == 3 && nextLoot2 == 1) {
					itemShape = "Plug";
					if (material == "Bronze") { itemPrice = 3.13; itemWeight = 0.05; }
					if (material == "Billon") { itemPrice = 29; itemWeight = 0.05; }
					if (material == "Silver") { itemPrice = 60; itemWeight = 0.05; }
					if (material == "Electrum") { itemPrice = 855; itemWeight = 0.05; }
					if (material == "Gold") { itemPrice = 2100; itemWeight = 0.1; }
					if (material == "Platinum") { itemPrice = 4800; itemWeight = 0.15; }
				}
				
				if (nextLoot1 == 3 && nextLoot2 == 2) {
					itemShape = "Nasal";
					if (material == "Bronze") { itemPrice = 3.75; itemWeight = 0.06; }
					if (material == "Billon") { itemPrice = 34.25; itemWeight = 0.05; }
					if (material == "Silver") { itemPrice = 60; itemWeight = 0.05; }
					if (material == "Electrum") { itemPrice = 1025; itemWeight = 0.1; }
					if (material == "Gold") { itemPrice = 2520; itemWeight = 0.15; }
					if (material == "Platinum") { itemPrice = 5760; itemWeight = 0.15; }
				}
				
				if (nextLoot1 == 3 && (nextLoot2 == 3 || nextLoot2 == 4 || nextLoot2 == 5 || nextLoot2 == 6)) {
					itemShape = "Button";
					if (material == "Bronze") { itemPrice = 5; itemWeight = 0.08; }
					if (material == "Billon") { itemPrice = 45.75; itemWeight = 0.1; }
					if (material == "Silver") { itemPrice = 95; itemWeight = 0.1; }
					if (material == "Electrum") { itemPrice = 1365; itemWeight = 0.15; }
					if (material == "Gold") { itemPrice = 3360; itemWeight = 0.15; }
					if (material == "Platinum") { itemPrice = 7680; itemWeight = 0.25; }
				}
				
				if (nextLoot1 == 4 && (nextLoot2 == 1 || nextLoot2 == 2 || nextLoot2 == 3)) {
					itemShape = "Ring";
					if (material == "Bronze") { itemPrice = 6.25; itemWeight = 0.1; }
					if (material == "Billon") { itemPrice = 57; itemWeight = 0.15; }
					if (material == "Silver") { itemPrice = 115; itemWeight = 0.15; }
					if (material == "Electrum") { itemPrice = 1705; itemWeight = 0.15; }
					if (material == "Gold") { itemPrice = 4200; itemWeight = 0.25; }
					if (material == "Platinum") { itemPrice = 9600; itemWeight = 0.25; }
				}
				
				if (nextLoot1 == 4 && nextLoot2 == 4) {
					itemShape = "Comb";
					if (material == "Bronze") { itemPrice = 6.25; itemWeight = 0.1; }
					if (material == "Billon") { itemPrice = 57; itemWeight = 0.15; }
					if (material == "Silver") { itemPrice = 115; itemWeight = 0.15; }
					if (material == "Electrum") { itemPrice = 1705; itemWeight = 0.15; }
					if (material == "Gold") { itemPrice = 4200; itemWeight = 0.25; }
					if (material == "Platinum") { itemPrice = 9600; itemWeight = 0.25; }
				}
				
				if (nextLoot1 == 4 && (nextLoot2 == 5 || nextLoot2 == 6)) {
					itemShape = "Buckle";
					if (material == "Bronze") { itemPrice = 12.5; itemWeight = 0.2; }
					if (material == "Billon") { itemPrice = 114; itemWeight = 0.25; }
					if (material == "Silver") { itemPrice = 230; itemWeight = 0.25; }
					if (material == "Electrum") { itemPrice = 3415; itemWeight = 0.25; }
					if (material == "Gold") { itemPrice = 8400; itemWeight = 0.5; }
					if (material == "Platinum") { itemPrice = 19200; itemWeight = 0.5; }
				}
				
				if (nextLoot1 == 5 && (nextLoot2 == 1 || nextLoot2 == 2 || nextLoot2 == 3)) {
					itemShape = "Bracelet";
					if (material == "Bronze") { itemPrice = 18.75; itemWeight = 0.3; }
					if (material == "Billon") { itemPrice = 171; itemWeight = 0.25; }
					if (material == "Silver") { itemPrice = 345; itemWeight = 0.25; }
					if (material == "Electrum") { itemPrice = 5120; itemWeight = 0.5; }
					if (material == "Gold") { itemPrice = 12600; itemWeight = 0.75; }
					if (material == "Platinum") { itemPrice = 28800; itemWeight = 0.75; }
				}
				
				if (nextLoot1 == 5 && (nextLoot2 == 4 || nextLoot2 == 5)) {
					itemShape = "Crown";
					if (material == "Bronze") { itemPrice = 31.25; itemWeight = 0.5; }
					if (material == "Billon") { itemPrice = 286; itemWeight = 0.5; }
					if (material == "Silver") { itemPrice = 575; itemWeight = 0.5; }
					if (material == "Electrum") { itemPrice = 8530; itemWeight = 0.75; }
					if (material == "Gold") { itemPrice = 21000; itemWeight = 1; }
					if (material == "Platinum") { itemPrice = 48000; itemWeight = 1.25; }
				}
				
				if (nextLoot1 == 5 && nextLoot2 == 6) {
					itemShape = "Torc";
					if (material == "Bronze") { itemPrice = 46.75; itemWeight = 0.5; }
					if (material == "Billon") { itemPrice = 428; itemWeight = 0.5; }
					if (material == "Silver") { itemPrice = 865; itemWeight = 0.5; }
					if (material == "Electrum") { itemPrice = 12800; itemWeight = 0.75; }
					if (material == "Gold") { itemPrice = 31500; itemWeight = 1; }
					if (material == "Platinum") { itemPrice = 72000; itemWeight = 1.25; }
				}
			}
		}
		
		if (gems != "") { itemDesc = gems; } else { itemDesc = ""; }
		
		var newMessage = "";
		
		if (isGachapon) {
			newMessage = "\nYou played the Fantasy Gachapon for 1000 credits and got:";
		}
		
		iBasePrice = itemPrice;
		
		var randType = Math.ceil(Math.random()*12);
		
		if (randType == 7 || randType == 8 || randType == 9) {
			var embellishment = getDF8Embellishment("hard");
			var motif = getMotif();
			
			randEmb = Math.ceil(Math.random()*3);
			if (randEmb == 1) { var embellishment = getDF8Embellishment("hard"); } else { var embellishment = getDF8Embellishment("supernatural"); }
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment.n + ". " + embellishment.d;
			if (embellishment.n != "Fine Material" && embellishment.n != "Exceptional Material" && randEmb == 1 && embellishment.n != "Silver Plating" && embellishment.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif + "."; }
			itemPrice = itemPrice * (1 + embellishment.v);
		}
		
		if (randType == 10 || randType == 11) {
			randEmb = Math.ceil(Math.random()*3);
			if (randEmb == 1) { var embellishment = getDF8Embellishment("hard"); } else { var embellishment = getDF8Embellishment("supernatural"); }
			var motif = getMotif();
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment.n + ". " + embellishment.d;
			if (embellishment.n != "Fine Material" && embellishment.n != "Exceptional Material" && randEmb == 1 && embellishment.n != "Silver Plating" && embellishment.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif + "."; }
			
			randEmb2 = Math.ceil(Math.random()*3);
			if (randEmb2 == 1) { var embellishment2 = getDF8Embellishment("hard"); } else { var embellishment2 = getDF8Embellishment("supernatural"); }
			var motif2 = getMotif();
			
			while (embellishment2 == embellishment || (embellishment.n.includes("Made of") && embellishment2.n.includes("Made of"))) {
				randEmb2 = Math.ceil(Math.random()*3);
				if (randEmb2 == 1) { var embellishment2 = getDF8Embellishment("hard"); } else { var embellishment2 = getDF8Embellishment("supernatural"); }
				var motif2 = getMotif();
			}
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment2.n + ". " + embellishment2.d;
			if (embellishment2.n != "Fine Material" && embellishment2.n != "Exceptional Material" && randEmb2 == 1 && embellishment2.n != "Silver Plating" && embellishment2.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif2 + "."; }
			itemPrice = itemPrice * (1 + embellishment.v + embellishment2.v);
			
		}
		
		if (randType == 12) {
			randEmb = Math.ceil(Math.random()*3);
			if (randEmb == 1) { var embellishment = getDF8Embellishment("hard"); } else { var embellishment = getDF8Embellishment("supernatural"); }
			var motif = getMotif();
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment.n + ". " + embellishment.d;
			if (embellishment.n != "Fine Material" && embellishment.n != "Exceptional Material" && randEmb == 1 && embellishment.n != "Silver Plating" && embellishment.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif + "."; }
			
			randEmb2 = Math.ceil(Math.random()*3);
			if (randEmb2 == 1) { var embellishment2 = getDF8Embellishment("hard"); } else { var embellishment2 = getDF8Embellishment("supernatural"); }
			var motif2 = getMotif();
			
			while (embellishment2 == embellishment || (embellishment.n.includes("Made of") && embellishment2.n.includes("Made of"))) {
				randEmb2 = Math.ceil(Math.random()*3);
				if (randEmb2 == 1) { var embellishment2 = getDF8Embellishment("hard"); } else { var embellishment2 = getDF8Embellishment("supernatural"); }
				var motif2 = getMotif();
			}
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment2.n + ". " + embellishment2.d;
			if (embellishment2.n != "Fine Material" && embellishment2.n != "Exceptional Material" && randEmb2 == 1 && embellishment2.n != "Silver Plating" && embellishment2.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif2 + "."; }
			
			
			randEmb3 = Math.ceil(Math.random()*3);
			if (randEmb3 == 1) { var embellishment3 = getDF8Embellishment("hard"); } else { var embellishment3 = getDF8Embellishment("supernatural"); }
			var motif3 = getMotif();
			
			while ((embellishment3 == embellishment2 || embellishment3 == embellishment) || (embellishment.n.includes("Made of") && embellishment3.n.includes("Made of")) || (embellishment2.n.includes("Made of") && embellishment3.n.includes("Made of"))) {
				randEmb3 = Math.ceil(Math.random()*3);
				if (randEmb3 == 1) { var embellishment3 = getDF8Embellishment("hard"); } else { var embellishment3 = getDF8Embellishment("supernatural"); }
				var motif3 = getMotif();
			}
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment3.n + ". " + embellishment3.d;
			if (embellishment3.n != "Fine Material" && embellishment3.n != "Exceptional Material" && randEmb3 == 1 && embellishment3.n != "Silver Plating" && embellishment3.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif3 + "."; }
			
			
			
			itemPrice = itemPrice * (1 + embellishment.v + embellishment2.v + embellishment3.v);
		}
	
		itemName = material + " " + itemShape;
		newPrice = Math.ceil((itemPrice*biggerMultiplier)+gemAdd);
		newWeight = itemWeight*biggerMultiplier;
		
		var randEnchantChance = Math.ceil(Math.random()*24);
		
		if (randEnchantChance == 24) {
			var enchantment = getEnchantment();
			itemName = "Enchanted " + itemName + " of " + enchantment.name;
			itemDesc = itemDesc + "\n\nEnchanted to be able to cast `" + enchantment.name + "` 6 times before having to be recharged.";
			newPrice = newPrice + enchantment.cost;
		}
		
		if (!dontPrint) {
			message.channel.send(message.author + newMessage + "\nJewelry: " + itemName + "\n" + newWeight + " lbs. *$" + newPrice + "*\n" + itemDesc);
		}
		
		newArgs = [ itemName, newPrice, newWeight, 4, 1, "DF8, p18", itemDesc, iBasePrice];
		return newArgs;
	}
	
	if (itemType == "container") {
		var nextLoot1 = Math.ceil(Math.random()*6);
		var nextLoot2 = Math.ceil(Math.random()*6);
		
		var itemName = "";
		var itemWeight = 0;
		var itemPrice = 0;
		var itemDesc = "";
		
		var containers = [
			{ "n":"Amphora", "p":60, "w":40, "d":"A large (six-gallon or 2.75-cubic-foot) earthenware container with handles and a pointed bottom, excellent for planting in the ground. DR 1, 12 HP."},
			{ "n":"Amphora", "p":60, "w":40, "d":"A large (six-gallon or 2.75-cubic-foot) earthenware container with handles and a pointed bottom, excellent for planting in the ground. DR 1, 12 HP."},
			{ "n":"Barrel", "p":65, "w":20, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Barrel", "p":65, "w":20, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Basket", "p":1.75, "w":1.5, "d":"A wicker basket (one-cubic-foot capacity) with a lid and carrying handle. DR 0, 4 HP."},
			{ "n":"Basket", "p":1.75, "w":1.5, "d":"A wicker basket (one-cubic-foot capacity) with a lid and carrying handle. DR 0, 4 HP."},
			
			{ "n":"Bottle/Jar", "p":3, "w":1, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Bottle/Jar", "p":3, "w":1, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Small Bottle/Jar", "p":2, "w":0.5, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Small Bottle/Jar", "p":2, "w":0.5, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Large Stone Box", "p":1000, "w":220, "d":"A box the size of a wooden chest but carved from soapstone. Attractive and durable, but extremely heavy. DR 4, 24 HP."},
			{ "n":"Small Stone Box", "p":50, "w":6, "d":"A soapstone box the size of a large book. DR 4, 7 HP."},
			
			{ "n":"Large Wooden Box", "p":15, "w":2, "d":"A wooden box the size of a large book. DR 1, 5 HP."},
			{ "n":"Small Wooden Box", "p":2, "w":0.25, "d":"A box small enough to fit in the palm. DR 1, 2 HP."},
			{ "n":"Cabinet", "p":150, "w":30, "d":"An upright chest with a capacity of eight cubic feet (700 lbs.) and doors opening in front. DR 1, 12 HP."},
			{ "n":"Compartmentalized Cabinet", "p":200, "w":32, "d":"An upright chest with a capacity of eight cubic feet (700 lbs.) and doors opening in front. Has many small drawers/shelves. The one who packed the chest can find things in it in half the usual time. DR 1, 12 HP."},
			{ "n":"Small Cabinet", "p":75, "w":15, "d":"A smaller version of the cabinet, with a capacity of about five cubic feet (400 lbs.). DR 1, 9 HP."},
			{ "n":"Small Compartmentalized Cabinet", "p":100, "w":16, "d":"A smaller version of the cabinet, with a capacity of about five cubic feet (400 lbs.). Has drawers and shelves. DR 1, 9 HP."},
			
			{ "n":"Large Cage", "p":120, "w":7, "d":"A cage made from thin iron bars (DR 5, 6 HP each) spaced at one-inch intervals. The cage is about two cubic feet, big enough for a mid-sized dog. The cage has a latch, but no lock, so an intelligent creature can release itself."},
			{ "n":"Small Cage", "p":40, "w":3, "d":"A smaller cage, about 200 cubic inches, suitable for birds, rats, and other small animals."},
			{ "n":"Small Wooden Cage", "p":20, "w":2, "d":"A smaller cage, about 200 cubic inches, made of wood (bars have DR 1, 4 HP). Suitable for small birds and other animals that won't try hard to escape."},
			{ "n":"Wooden Canteen", "p":10, "w":3, "d":"1 quart capacity."},
			{ "n":"Chest", "p":200, "w":40, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Compartmentalized Chest", "p":300, "w":45, "d":" See *Dungeon Fantasy 1*, page 24. Divided into compartments and cubbyholes."},
			
			{ "n":"Compartmentalized Chest", "p":300, "w":45, "d":" See *Dungeon Fantasy 1*, page 24. Divided into compartments and cubbyholes."},
			{ "n":"Small Chest", "p":100, "w":18, "d":"A smaller chest, about two cubic feet or 100 lbs. capacity. DR 1, 10 HP."},
			{ "n":"Small Chest", "p":100, "w":18, "d":"A smaller chest, about two cubic feet or 100 lbs. capacity. DR 1, 10 HP."},
			{ "n":"Small Compartmentalized Chest", "p":120, "w":20, "d":"A smaller chest, about two cubic feet or 100 lbs. capacity. Divided into small compartments. DR 1, 10 HP."},
			{ "n":"Small Compartmentalized Chest", "p":120, "w":20, "d":"A smaller chest, about two cubic feet or 100 lbs. capacity. Divided into small compartments. DR 1, 10 HP."},
			{ "n":"Iron Strongbox", "p":250, "w":15, "d":"See *Dungeon Fantasy 1*, page 24."},
			
			{ "n":"Compartmentalized Iron Strongbox", "p":300, "w":18, "d":"See *Dungeon Fantasy 1*, page 24. Divided into small compartments."},
			{ "n":"Crystal Vial", "p":5, "w":0.25, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Iron Vial", "p":15, "w":0.5, "d":"See *Dungeon Fantasy 1*, page 24. DR 3, 3 HP."},
			{ "n":"Wineskin", "p":10, "w":0.25, "d":"1 gallon capacity."},
			{ "n":"Wineskin", "p":10, "w":0.25, "d":"1 gallon capacity."},
			{ "n":"Wineskin", "p":10, "w":0.25, "d":"1 gallon capacity."}
		];
		
		var rand1 = Math.ceil(Math.random()*(containers.length-1));
		
		itemName = containers[rand1].n;
		itemPrice = containers[rand1].p;
		itemWeight = containers[rand1].w;
		itemDesc = containers[rand1].d;
		
		var newMessage = "";
		
		if (isGachapon) {
			newMessage = "\nYou played the Fantasy Gachapon for 1000 credits and got:";
		}
		
		iBasePrice = itemPrice;
		
		var randType = Math.ceil(Math.random()*12);
		
		if (randType == 7 || randType == 8 || randType == 9) {
			var embellishment = getDF8Embellishment("hard");
			var motif = getMotif();
			
			randEmb = Math.ceil(Math.random()*3);
			if (randEmb == 1) { var embellishment = getDF8Embellishment("hard"); } else { var embellishment = getDF8Embellishment("supernatural"); }
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment.n + ". " + embellishment.d;
			if (embellishment.n != "Fine Material" && embellishment.n != "Exceptional Material" && randEmb == 1 && embellishment.n != "Silver Plating" && embellishment.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif + "."; }
			itemPrice = itemPrice * (1 + embellishment.v);
		}
		
		if (randType == 10 || randType == 11) {
			randEmb = Math.ceil(Math.random()*3);
			if (randEmb == 1) { var embellishment = getDF8Embellishment("hard"); } else { var embellishment = getDF8Embellishment("supernatural"); }
			var motif = getMotif();
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment.n + ". " + embellishment.d;
			if (embellishment.n != "Fine Material" && embellishment.n != "Exceptional Material" && randEmb == 1 && embellishment.n != "Silver Plating" && embellishment.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif + "."; }
			
			randEmb2 = Math.ceil(Math.random()*3);
			if (randEmb2 == 1) { var embellishment2 = getDF8Embellishment("hard"); } else { var embellishment2 = getDF8Embellishment("supernatural"); }
			var motif2 = getMotif();
			
			while (embellishment2 == embellishment || (embellishment.n.includes("Made of") && embellishment2.n.includes("Made of"))) {
				randEmb2 = Math.ceil(Math.random()*3);
				if (randEmb2 == 1) { var embellishment2 = getDF8Embellishment("hard"); } else { var embellishment2 = getDF8Embellishment("supernatural"); }
				var motif2 = getMotif();
			}
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment2.n + ". " + embellishment2.d;
			if (embellishment2.n != "Fine Material" && embellishment2.n != "Exceptional Material" && randEmb2 == 1 && embellishment2.n != "Silver Plating" && embellishment2.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif2 + "."; }
			itemPrice = itemPrice * (1 + embellishment.v + embellishment2.v);
			
		}
		
		if (randType == 12) {
			randEmb = Math.ceil(Math.random()*3);
			if (randEmb == 1) { var embellishment = getDF8Embellishment("hard"); } else { var embellishment = getDF8Embellishment("supernatural"); }
			var motif = getMotif();
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment.n + ". " + embellishment.d;
			if (embellishment.n != "Fine Material" && embellishment.n != "Exceptional Material" && randEmb == 1 && embellishment.n != "Silver Plating" && embellishment.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif + "."; }
			
			randEmb2 = Math.ceil(Math.random()*3);
			if (randEmb2 == 1) { var embellishment2 = getDF8Embellishment("hard"); } else { var embellishment2 = getDF8Embellishment("supernatural"); }
			var motif2 = getMotif();
			
			while (embellishment2 == embellishment || (embellishment.n.includes("Made of") && embellishment2.n.includes("Made of"))) {
				randEmb2 = Math.ceil(Math.random()*3);
				if (randEmb2 == 1) { var embellishment2 = getDF8Embellishment("hard"); } else { var embellishment2 = getDF8Embellishment("supernatural"); }
				var motif2 = getMotif();
			}
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment2.n + ". " + embellishment2.d;
			if (embellishment2.n != "Fine Material" && embellishment2.n != "Exceptional Material" && randEmb2 == 1 && embellishment2.n != "Silver Plating" && embellishment2.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif2 + "."; }
			
			
			randEmb3 = Math.ceil(Math.random()*3);
			if (randEmb3 == 1) { var embellishment3 = getDF8Embellishment("hard"); } else { var embellishment3 = getDF8Embellishment("supernatural"); }
			var motif3 = getMotif();
			
			while ((embellishment3 == embellishment2 || embellishment3 == embellishment) || (embellishment.n.includes("Made of") && embellishment3.n.includes("Made of")) || (embellishment2.n.includes("Made of") && embellishment3.n.includes("Made of"))) {
				randEmb3 = Math.ceil(Math.random()*3);
				if (randEmb3 == 1) { var embellishment3 = getDF8Embellishment("hard"); } else { var embellishment3 = getDF8Embellishment("supernatural"); }
				var motif3 = getMotif();
			}
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment3.n + ". " + embellishment3.d;
			if (embellishment3.n != "Fine Material" && embellishment3.n != "Exceptional Material" && randEmb3 == 1 && embellishment3.n != "Silver Plating" && embellishment3.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif3 + "."; }
			
			
			
			itemPrice = itemPrice * (1 + embellishment.v + embellishment2.v + embellishment3.v);
		}
	
		itemPrice = Math.ceil(itemPrice);
		
		if (!dontPrint) {
			message.channel.send(message.author + newMessage + "\nContainer: " + itemName + "\n" + itemWeight + " lbs. *$" + itemPrice + "*\n" + itemDesc);
		}
		
		newArgs = [ itemName, itemPrice, itemWeight, 4, 1, "DF8, p20", itemDesc, iBasePrice];
		return newArgs;
	}
	
	if (itemType == "garment") {
		var nextLoot1 = Math.ceil(Math.random()*6);
		var nextLoot2 = Math.ceil(Math.random()*6);
		var nextLoot3 = Math.ceil(Math.random()*6);
		
		
		var itemName = "";
		var itemWeight = 0;
		var itemPrice = 0;
		var itemDesc = "";
		var garmentMat = getGarmentMaterial();
		var garmentColor = getColor();
		
		
		var garments = [
			{ "n":"Belt", "w":0.25, "p":15}, { "n":"Cap", "w":0.1, "p":6}, { "n":"Cape", "w":2, "p":20}, { "n":"Cloak", "w":5, "p":50}, { "n":"Gloves", "w":0.5, "p":15},
			{ "n":"Light Gown", "w":0.6, "p":40}, { "n":"Gown", "w":1.2, "p":75}, { "n":"Winter Gown", "w":3, "p":112}, { "n":"Small Hat", "w":0.2, "p":12},
			{ "n":"Small Winter Hat", "w":0.4, "p":24}, { "n":"Tall Hat", "w":0.3, "p":18}, { "n":"Tall Winter Hat", "w":0.6, "p":36},
			{ "n":"Wide Hat", "w":0.3, "p":18}, { "n":"Wide Winter Hat", "w":0.6, "p":36}, { "n":"Wide and Tall Hat", "w":0.35, "p":28},
			{ "n":"Wide and Tall Winter Hat", "w":0.7, "p":56}, { "n":"Hooded Cape", "w":2.2, "p":22}, { "n":"Hooded Cloak", "w":5.5, "p":55},
			{ "n":"Light Hooded Shirt", "w":0.44, "p":26.4}, { "n":"Hooded Shirt", "w":0.88, "p":52.8}, { "n":"Winter Hooded Shirt", "w":2.2, "p":79.2},
			{ "n":"Light Hooded Tunic", "w":0.33, "p":20}, { "n":"Hooded Tunic", "w":0.66, "p":39.6}, { "n":"Winter Hooded Tunic", "w":1.65, "p":59.4},
			{ "n":"Light Hose", "w":0.3, "p":18}, { "n":"Hose", "w":0.6, "p":36}, { "n":"Winter Hose", "w":1.5, "p":54}, { "n":"Keffiyeh", "w":0.5, "p":16},
			{ "n":"Long Coat", "w":5, "p":50}, { "n":"Poncho", "w":3, "p":25}, { "n":"Light Robe", "w":0.6, "p":36}, { "n":"Robe", "w":1.2, "p":72},
			{ "n":"Winter Robe", "w":3, "p":108}, { "n":"Ruff", "w":0.1, "p":8}, { "n":"Sandals", "w":0.5, "p":24}, { "n":"Sash", "w":0.2, "p":12},
			{ "n":"Scarf", "w":0.1, "p":6}, { "n":"Shawl", "w":0.4, "p":24}, { "n":"Light Shirt", "w":0.4, "p":24}, { "n":"Shirt", "w":0.8, "p":48},
			{ "n":"Winter Shirt", "w":2, "p":72}, { "n":"Shoes", "w":2, "p":40}, { "n":"Short Boots", "w":3, "p":80}, { "n":"Light Skirt", "w":0.3, "p":15},
			{ "n":"Skirt", "w":0.6, "p":30}, { "n":"Winter Skirt", "w":1.5, "p":50}, { "n":"Light Sleeves", "w":0.1, "p":6}, { "n":"Heavy Sleeves", "w":0.5, "p":18},
			{ "n":"Sleeves", "w":0.2, "p":12}, { "n":"Slippers", "w":1, "p":35}, { "n":"Surcoat", "w":0.5, "p":35}, { "n":"Light Trousers", "w":0.6, "p":36},
			{ "n":"Trousers", "w":1.2, "p":72}, { "n":"Winter Trousers", "w":3, "p":108}, { "n":"Light Tunic", "w":0.3, "p":18}, { "n":"Regular Tunic", "w":0.6, "p":36},
			{ "n":"Winter Tunic", "w":1.5, "p":56}, { "n":"Turban", "w":0.4, "p":24}, { "n":"Light Wrap", "w":0.8, "p":48}, { "n":"Regular Wrap", "w":1.6, "p":96},
			{ "n":"Winter Wrap", "w":4, "p":144}
		];
		
		var rand1 = Math.ceil(Math.random()*(garments.length-1));
		
		itemName = garments[rand1].n;
		itemPrice = garments[rand1].p;
		itemWeight = garments[rand1].w;
		itemDesc = "It's a " + garmentColor + " colored " + itemName + ", made of " + garmentMat + ".";
		
		var newMessage = "";
		
		if (isGachapon) {
			newMessage = "\nYou played the Fantasy Gachapon for 1000 credits and got:";
		}
		
		iBasePrice = itemPrice;
		
		var randType = Math.ceil(Math.random()*12);
		
		if (randType == 7 || randType == 8 || randType == 9) {
			var embellishment = getDF8Embellishment("soft");
			var motif = getMotif();
			
			randEmb = Math.ceil(Math.random()*3);
			if (randEmb == 1) { var embellishment = getDF8Embellishment("soft"); } else { var embellishment = getDF8Embellishment("supernatural"); }
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment.n + ". " + embellishment.d;
			if (embellishment.n != "Fine Material" && embellishment.n != "Exceptional Material" && randEmb == 1 && embellishment.n != "Silver Plating" && embellishment.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif + "."; }
			itemPrice = itemPrice * (1 + embellishment.v);
		}
		
		if (randType == 10 || randType == 11) {
			randEmb = Math.ceil(Math.random()*3);
			if (randEmb == 1) { var embellishment = getDF8Embellishment("soft"); } else { var embellishment = getDF8Embellishment("supernatural"); }
			var motif = getMotif();
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment.n + ". " + embellishment.d;
			if (embellishment.n != "Fine Material" && embellishment.n != "Exceptional Material" && randEmb == 1 && embellishment.n != "Silver Plating" && embellishment.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif + "."; }
			
			randEmb2 = Math.ceil(Math.random()*3);
			if (randEmb2 == 1) { var embellishment2 = getDF8Embellishment("soft"); } else { var embellishment2 = getDF8Embellishment("supernatural"); }
			var motif2 = getMotif();
			
			while (embellishment2 == embellishment || (embellishment.n.includes("Made of") && embellishment2.n.includes("Made of"))) {
				randEmb2 = Math.ceil(Math.random()*3);
				if (randEmb2 == 1) { var embellishment2 = getDF8Embellishment("soft"); } else { var embellishment2 = getDF8Embellishment("supernatural"); }
				var motif2 = getMotif();
			}
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment2.n + ". " + embellishment2.d;
			if (embellishment2.n != "Fine Material" && embellishment2.n != "Exceptional Material" && randEmb2 == 1 && embellishment2.n != "Silver Plating" && embellishment2.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif2 + "."; }
			itemPrice = itemPrice * (1 + embellishment.v + embellishment2.v);
			
		}
		
		if (randType == 12) {
			randEmb = Math.ceil(Math.random()*3);
			if (randEmb == 1) { var embellishment = getDF8Embellishment("soft"); } else { var embellishment = getDF8Embellishment("supernatural"); }
			var motif = getMotif();
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment.n + ". " + embellishment.d;
			if (embellishment.n != "Fine Material" && embellishment.n != "Exceptional Material" && randEmb == 1 && embellishment.n != "Silver Plating" && embellishment.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif + "."; }
			
			randEmb2 = Math.ceil(Math.random()*3);
			if (randEmb2 == 1) { var embellishment2 = getDF8Embellishment("soft"); } else { var embellishment2 = getDF8Embellishment("supernatural"); }
			var motif2 = getMotif();
			
			while (embellishment2 == embellishment || (embellishment.n.includes("Made of") && embellishment2.n.includes("Made of"))) {
				randEmb2 = Math.ceil(Math.random()*3);
				if (randEmb2 == 1) { var embellishment2 = getDF8Embellishment("soft"); } else { var embellishment2 = getDF8Embellishment("supernatural"); }
				var motif2 = getMotif();
			}
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment2.n + ". " + embellishment2.d;
			if (embellishment2.n != "Fine Material" && embellishment2.n != "Exceptional Material" && randEmb2 == 1 && embellishment2.n != "Silver Plating" && embellishment2.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif2 + "."; }
			
			
			randEmb3 = Math.ceil(Math.random()*3);
			if (randEmb3 == 1) { var embellishment3 = getDF8Embellishment("soft"); } else { var embellishment3 = getDF8Embellishment("supernatural"); }
			var motif3 = getMotif();
			
			while ((embellishment3 == embellishment2 || embellishment3 == embellishment) || (embellishment.n.includes("Made of") && embellishment3.n.includes("Made of")) || (embellishment2.n.includes("Made of") && embellishment3.n.includes("Made of"))) {
				randEmb3 = Math.ceil(Math.random()*3);
				if (randEmb3 == 1) { var embellishment3 = getDF8Embellishment("soft"); } else { var embellishment3 = getDF8Embellishment("supernatural"); }
				var motif3 = getMotif();
			}
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment3.n + ". " + embellishment3.d;
			if (embellishment3.n != "Fine Material" && embellishment3.n != "Exceptional Material" && randEmb3 == 1 && embellishment3.n != "Silver Plating" && embellishment3.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif3 + "."; }
			
			
			
			itemPrice = itemPrice * (1 + embellishment.v + embellishment2.v + embellishment3.v);
		}
	
		itemPrice = Math.ceil(itemPrice);
		
		var randEnchantChance = Math.ceil(Math.random()*50);
		
		if (randEnchantChance == 50) {
			var enchantment = getArmorEnchant();
			itemName = "Enchanted " + itemName + " of " + enchantment.name;
			itemDesc = itemDesc + "\n\nEnchanted to have `" + enchantment.name + "` as if it were a piece of armor.";
			itemPrice = itemPrice + enchantment.cost;
		}
		
		
		newName = garmentColor + " " + garmentMat + " " + itemName;
		if (!dontPrint) {
			message.channel.send(message.author + newMessage + "\nGarment: " + newName + "\n" + itemWeight + " lbs. *$" + itemPrice + "*\n" + itemDesc);
		}
		newArgs = [ newName, itemPrice, itemWeight, 4, 1, "DF8, p17", itemDesc, iBasePrice];
		
		return newArgs;
	}

	if (itemType == "accoutrement") {
		var nextLoot1 = Math.ceil(Math.random()*6);
		var nextLoot2 = Math.ceil(Math.random()*6);
		var nextLoot3 = Math.ceil(Math.random()*6);
		
		var rand1 = Math.ceil(Math.random()*6);
		var rand2 = Math.ceil(Math.random()*6);
		var rand = rand1 + rand2 + 4;
		var otherRand = rand1 + rand2;
		var prosthetic = "";
		var elvenLimb = "";
		
		
		if (rand1 == 1) { elvenLimb = "Foot"; }
		if (rand1 == 2) { elvenLimb = "Hand"; }
		if (rand1 == 3 || rand1 == 4) { elvenLimb = "Leg"; }
		if (rand1 == 5 || rand1 == 6) { elvenLimb = "Arm"; }
		
		if (otherRand == 2) { prosthetic = "Tail"; }
		if (otherRand == 3) { prosthetic = "Nose"; }
		if (otherRand == 4) { prosthetic = "Foot"; }
		if (otherRand == 5 || otherRand == 6) { prosthetic = "Leg"; }
		if (otherRand == 7 || otherRand == 8) { prosthetic = "Arm"; }
		if (otherRand == 9 || otherRand == 10) { prosthetic = "Hand"; }
		if (otherRand == 11) { prosthetic = "Eye"; }
		if (otherRand == 12) { prosthetic = "Ear"; }
		
		
		var itemName = "";
		var itemWeight = 0;
		var itemPrice = 0;
		var itemDesc = "";
		
		var accoutrements = [
			{ "n":"Block and Tackle", "p":200, "w":10, "d":"An arrangement of pulleys allowing users to lift very heavy loads. Using a block and tackle permits someone to lift up to BLx32 one foot per second. Multiple adventures and even draft animals can pull on the rope; total the BL of everyone involved."},
			{ "n":"Climbing Spikes", "p":400, "w":4, "d":"See *Dungeon Fantasy 1*, see page 25."},
			{ "n":"Climbing Spikes, Improved", "p":4000, "w":8, "d":"Instead of just having claws at the hands and feet, the improved spikes put additional claws around the knees and elbows for greater traction. In addition to granting +3 to Climbing skill, the claws allow the user to stay attached to a vertical climbing surface by the legs but freely use both hands (or, if necessary, hang on by the arms but use the legs freely). They also give +1 to damage with unarmed strikes."},
			{ "n":"Crossbow Grapnel", "p":300, "w":9, "d":"A crossbow with a special harpoon-shaped bolt and a spool of strong cord secure to a pulley at the end of the bolt. The extra weight of the line-bolt gives it range and damage as a crossbow with 2/3 the user's ST (round up). However, it can fire the grapnel much farther than someone could throw it. The double thickness of cord supports 180 lbs., or a sturdier rope can be pulled through at a rate of ST feet per second. Additional spools are $50, 2.5 lbs."},
			{ "n":"Giant-Spider Silk Cord", "p":100, "w":0.5, "d":"10 yards. See *Dungeon Fantasy 1*, page 25."},
			{ "n":"Grapnel", "p":80, "w":2, "d":"See *Dungeon Fantasy 1*, page 25."},
			{ "n":"10-foot Ladder", "p":25, "w":15, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Orichalcum Climbing Chain", "p":150, "w":0.5, "d":"A strong but lightweight chain with larger rings set every foot for grabbing. The chain cannot be broken, cut, or burned (though repeated heat-based attacks can make it too hot to hold on to). Cost and weight are per foot, " + rand + " feet."},
			{ "n":"30-foot Portable Ladder", "p":200, "w":10, "d":"See *Dungeon Fantasy 1*, page 26."},
			{ "n":"3/4-inch Rope", "p":25, "w":5, "d":"10 yards. See *Dungeon Fantasy 1*, page 24."},
			{ "n":"3/8-inch Rope", "p":5, "w":1.5, "d":"10 yards. See *Dungeon Fantasy 1*, page 24."},
			{ "n":"3/4-inch Dragonhide Rope", "p":250, "w":5, "d":"10 yards. Rope made from braided strips of treated dragon hide. Resistant to fire (DR 10 against heat and flame attacks). Supports 1,100 lbs."},
			{ "n":"3/8-inch Dragonhide Rope", "p":50, "w":1.5, "d":"10 yards. Rope made from braided strips of treated dragon hide. Resistant to fire (DR 10 against heat and flame attacks). Supports 300 lbs."},
			{ "n":"3/4-inch Elven Rope", "p":75, "w":5, "d":"10 yards. Supports 2,200 lbs."},
			{ "n":"3/8-inch Elven Rope", "p":15, "w":1.5, "d":"10 yards. Supports 600 lbs."},
			{ "n":"Lifting Sling", "p":20, "w":2.5, "d":"A comfortable leather seat capable of lifting or lowering an occupant up to SM+1. Includes ties so the user doesn't fall out if killed or rendered unconscious."},
			{ "n":"Bandages", "p":10, "w":1, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Healer's Kit", "p":200, "w":10, "d":"See *Dungeon Fantasy 1*, page 26. Designed for one specific Esoteric Medicine specialty."},
			{ "n":"First Aid Kit", "p":50, "w":2, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"First Aid Kit", "p":50, "w":2, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Prosthetic " + prosthetic, "p":400, "w":1, "d":"A mundane replacement, not a cybernetic or mystical replacement. Can only partially replace the function of a lost limb."},
			{ "n":"Elven " + elvenLimb + " Brace", "p":2500, "w":2, "d":"If applied quickly after combat, this lightweight but sturdy exoskeleton gives +3 to the HT roll to determine the duration of crippling injuries."},
			{ "n":"Spider-Silk Bandages", "p":80, "w":0, "d":"Six patches of giant-spider silk, excellent for patching wounds. Gives +1 to First Aid skill; can be combined with bonuses from other medical gear."},
			{ "n":"Surgical Instruments", "p":300, "w":15, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Backboard", "p":60, "w":7, "d":"A broad board with shoulder straps and cords with which to tie equipment. It takes half as much time to locate and retrieve gear as from a backpack. Holds 70 lbs. of gear."},
			{ "n":"Frame Backpack", "p":100, "w":10, "d":"See *Dungeon Fantasy 1*, page 23."},
			{ "n":"Small Backpack", "p":60, "w":3, "d":"See *Dungeon Fantasy 1*, page 23."},
			{ "n":"Bandoleer", "p":60, "w":1, "d":"See *Dungeon Fantasy 1*, page 25."},
			{ "n":"Carrying Yoke", "p":70, "w":3, "d":"A W-shaped frame that rests on the shoulder. A hand on the front balances the load in the back. It can be dropped as a free action, but it requires a hand to keep it steady while it is being used. Holds up to 100 lbs. of gear."},
			{ "n":"Crossbow Sling", "p":200, "w":2, "d":"See *Dungeon Fantasy 1*, page 25."},
			{ "n":"Delver's Webbing", "p":160, "w":3, "d":"See *Dungeon Fantasy 1*, page 25."},
			{ "n":"Hip Quiver", "p":15, "w":1, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Large Hip Quiver", "p":30, "w":2, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Potion Belt", "p":60, "w":1, "d":"See *Dungeon Fantasy 1*, page 25."},
			{ "n":"Pouch/Purse", "p":10, "w":0.2, "d":"See *Dungeon Fantasy 1*, page 23."},
			{ "n":"Quick-Release Backpack", "p":300, "w":3, "d":"See *Dungeon Fantasy 1*, page 25."},
			{ "n":"Sack", "p":30, "w":3, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Scroll Belt", "p":60, "w":1, "d":"See *Dungeon Fantasy 4*, page 12."},
			{ "n":"Scroll Case", "p":75, "w":1, "d":"See *Dungeon Fantasy 4*, page 12."},
			{ "n":"Shoulder Quiver", "p":10, "w":0.5, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Wheelbarrow", "p":60, "w":18, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Bagpipe", "p":270, "w":3, "d":"An exceptionally loud instrument - so loud, in fact, that other bards within 10 yards have trouble hearing themselves and are at -2 to their own Bard-Song rolls."},
			{ "n":"Drum", "p":40, "w":2, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Glass Harmonica", "p":500, "w":10, "d":"A series of glass disks on an axle inside a box, spun with a crank, and played with a wet fingertip touched to the rims; the principle is the same as running a finger around the rim of a wineglass to produce a tone. The instrument’s supernatural tone quality gives +3 to any Bard-Song rolls, but it has a number of drawbacks. First, it is not a particularly mobile instrument. It takes three seconds to set up on its stand or put away, and it must be stationary while it is played. Second, it is very fragile. The casing has DR 2, but any attack that does damage to it destroys the instrument. Third, because of its weird resonances, a critical failure while using it attracts the attention of Elder Things, who can be expected to appear before the end of the current adventure."},
			{ "n":"Harp", "p":250, "w":3, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Horn", "p":100, "w":2, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"One-Handed Concertina", "p":200, "w":0.5, "d":"A tiny accordion that can be squeezed with the thumb and played with fingers on a small set of keys. Gives -1 to Bard- Song rolls, but it can be played with one hand. The other can be holding a weapon, hanging on to a rope, etc."},
			{ "n":"Stringed (instrument)", "p":150, "w":5, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Whistle (instrument)", "p":5, "w":0.1, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Woodwind (instrument)", "p":40, "w":1, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Alchemist's Garb", "p":225, "w":5, "d":"See *Dungeon Fantasy 4*, page 12."},
			{ "n":"Anti-Garrote Collar", "p":35, "w":1, "d":"See *Dungeon Fantasy 1*, page 25."},
			{ "n":"Float Coat", "p":100, "w":6, "d":"See *Dungeon Fantasy 4*, page 12."},
			{ "n":"Tinted Goggles", "p":150, "w":0.5, "d":"See *Dungeon Fantasy 1*, page 26."},
			{ "n":"Wet Cloak", "p":200, "w":12, "d":"A long, hooded cloak with thin tubes of water running through it. Provides DR 3 against fire and burning damage for five seconds."},
			{ "n":"Pixie Stained-Glass Spectacles", "p":600, "w":0.5, "d":"These colored glass spectacles are superior eye protection for dungeon delvers. Like tinted goggles, they provide Protected Vision. However, they give only a -1 to Vision rolls. They provide no more physical protection than an ordinary pair of spectacles."},
			{ "n":"Slippery Oil", "p":80, "w":1, "d":"A pint of this oil, applied to the body or noncloth armor, provides +5 to any attempts to resist grappling or to escape sticky webs and surfaces. However, it also gives -5 to any grappling attempts the wearer makes, and the inevitable bit of spillage and running get the oil on hands and feet, giving -1 to any rolls to retain footing or hold on to items. Slippery oil wears off after about an hour."},
			{ "n":"Holy Symbol", "p":50, "w":1, "d":"See *Dungeon Fantasy 1*, p. 26."},
			{ "n":"Blessed Holy Symbol", "p":250, "w":1, "d":"See *Dungeon Fantasy 1*, p. 26."},
			{ "n":"High Holy Symbol", "p":1000, "w":1, "d":"See *Dungeon Fantasy 1*, p. 26."},
			{ "n":"Portable Shrine", "p":400, "w":8, "d":"A small set of religious images and paraphernalia in a folding case. Once set up by a cleric (takes a minute of ritual), it temporarily raises the sanctity of the surrounding area by a level for a radius equal to the cleric's Power Investiture. The bonus lasts until the shrine is removed or destroyed."},
			{ "n":"Blessed Portable Shrine", "p":2000, "w":8, "d":"A small set of religious images and paraphernalia in a folding case. Once set up by a cleric (takes a minute of ritual), it temporarily raises the sanctity of the surrounding area by a level for a radius equal to the cleric's Power Investiture. The bonus lasts until the shrine is removed or destroyed. Increased radius of sanctity by 50%"},
			{ "n":"High Portable Shrine", "p":8000, "w":8, "d":"A small set of religious images and paraphernalia in a folding case. Once set up by a cleric (takes a minute of ritual), it temporarily raises the sanctity of the surrounding area by a level for a radius equal to the cleric's Power Investiture. The bonus lasts until the shrine is removed or destroyed. Increased radius of sanctity by double."},
			{ "n":"Holy Water", "p":15, "w":1, "d":"See *Dungeon Fantasy 1*, p. 26."},
			{ "n":"Flying Animal Barding", "p":500, "w":30, "d":"A full set of leather and cloth barding (DR 2) for a griffin, giant eagle, or other winged mount. Wings are not covered, but they aren’t encumbered, either."},
			{ "n":"Giant Carnivore Barding", "p":400, "w":30, "d":"A full set of leather and cloth barding (DR 2) for a giant cat, riding wolf, or other large, predatory animal used as a mount."},
			{ "n":"Horse Barding", "p":345, "w":30, "d":"A full set of leather and cloth barding (DR 2) for a horse or horse-like animal."},
			{ "n":"Bit and Bridle", "p":35, "w":3, "d":"See *Basic Set*, page 289."},
			{ "n":"Saddle and Tack", "p":150, "w":15, "d":"See *Basic Set*, page 289."},
			{ "n":"Saddle with Stirrups", "p":125, "w":20, "d":"See *Basic Set*, page 289."},
			{ "n":"Flying Saddle", "p":300, "w":20, "d":"A saddle with additional straps to keep a rider secure at extreme angles. It takes two minutes to secure, but the user will not fall out, nor can he be pulled out without undoing the straps. If the rider doesn’t take the time to strap in, treat it as a regular saddle."},
			{ "n":"War Saddle", "p":250, "w":35, "d":"See *Basic Set*, page 289."},
			{ "n":"Saddlebags", "p":100, "w":3, "d":"See *Basic Set*, page 289."},
			{ "n":"Alchemist's Matches", "p":15, "w":0.25, "d":"See *Dungeon Fantasy 1*, page 25."},
			{ "n":"Alchemist's Matches", "p":15, "w":0.25, "d":"See *Dungeon Fantasy 1*, page 25."},
			{ "n":"Bull's-Eye Lantern", "p":100, "w":2, "d":"See *Dungeon Fantasy 1*, page 26."},
			{ "n":"Burning Glass", "p":40, "w":0.25, "d":"See *Dungeon Fantasy 1*, page 26."},
			{ "n":"Burning Stone", "p":250, "w":1, "d":"A burning stone is about the size and color of a charcoal briquette, but when lit, it becomes something quite different. It burns with tremendous heat, glowing with a bright, white light for an hour. If used as a light source, it sheds a light equal to daylight at its source, with Vision penalties for darkness increasing by one for every two yards of distance. It can also burn through just about anything short of stone. If held against a target, it does 4d burning damage (cyclic, per second). That kind of prolonged contact is essentially impossible in combat; treat being struck with a burning stone as equivalent to being struck with a torch. The main drawback of the burning stone is that it makes a loud hissing, sizzling sound when lit, about the same volume as a normal conversation, making stealth nearly impossible even if the light is covered. The heat with which it burns can also be an issue, since it will eventually melt metal and can even damage ceramics. It is usually carried in a mug-like stone holder ($6, 4 lbs.) and cannot be contained in a helmet lamp or shield lamp."},
			{ "n":"Beeswax Candle", "p":5, "w":1, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Tallow Candle", "p":0.5, "w":1, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Corrective Spectacles", "p":150, "w":0.5, "d":"See *Dungeon Fantasy 1*, page 26."},
			{ "n":"Delver's Periscope", "p":50, "w":4, "d":"A simple arrangement of angle mirrors in a collapsible tube (three feet fully extended), allowing the user to look over walls and around corners without being seen. The end of the periscope is SM -6 and commensurately hard to spot."},
			{ "n":"Dwarven Theodolite", "p":4000, "w":15, "d":"A set of sights and tiny spirit levels attached to a stable platform, allowing the user to quickly take rough readings of distances and visual angles. Accuracy isn’t great (distances can be measured in, at best, 10-foot increments and angles in fivedegree increments), but it’s good enough for rough dungeon mapping or precisely placing the effects of area spells. Furthermore, it can be done quickly (10 seconds to set up plus 10 seconds each to measure any one distance or angle)."},
			{ "n":"Faerie Mirror Cloth", "p":200, "w":0, "d":"A costly reflective fabric produced by secretive leprechauns, equivalent to a hand mirror, but lightweight and won’t break if dropped."},
			{ "n":"Helmet Lamp", "p":100, "w":2, "d":"See *Dungeon Fantasy 1*, page 25."},
			{ "n":"Lantern", "p":20, "w":2, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Hand Mirror", "p":15, "w":1, "d":"See *Dungeon Fantasy 1*, page 26."},
			{ "n":"Tall Mirror", "p":125, "w":10, "d":"See *Dungeon Fantasy 1*, page 26."},
			{ "n":"Lantern Oil", "p":2, "w":1, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Lantern Oil", "p":2, "w":1, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Reflector", "p":75, "w":1, "d":"See *Dungeon Fantasy 4*, page 12."},
			{ "n":"Safety Matches", "p":30, "w":0.25, "d":"As alchemist’s matches (Dungeon Fantasy 1, p. 25), but using a two-part formula. The matches must be struck on a treated pad kept in a separate part of a small box. They will not ignite accidentally."},
			{ "n":"Shield Lamp", "p":200, "w":4, "d":"See *Dungeon Fantasy 4*, page 12."},
			{ "n":"Telescope", "p":500, "w":6, "d":"See *Dungeon Fantasy 1*, page 26."},
			{ "n":"Torch", "p":3, "w":1, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Torch", "p":3, "w":1, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Blanket", "p":20, "w":4, "d":"See *Dungeon Fantasy 1*, page 23."},
			{ "n":"Blanket", "p":20, "w":4, "d":"See *Dungeon Fantasy 1*, page 23."},
			{ "n":"Steel Chain", "p":5, "w":2, "d":"Supports up to 2,000 lbs. Cost and weight are per foot. " + rand + " feet."},
			{ "n":"Steel Chain", "p":5, "w":2, "d":"Supports up to 2,000 lbs. Cost and weight are per foot. " + rand + " feet."},
			{ "n":"Cobweb Kite", "p":16000, "w":8, "d":"See *Dungeon Fantasy 4*, page 12."},
			{ "n":"Fishhooks and Line", "p":50, "w":0.1, "d":"See *Dungeon Fantasy 1*, page 23. Requires Pole"},
			{ "n":"Group Basics", "p":50, "w":20, "d":"See *Dungeon Fantasy 1*, page 23."},
			{ "n":"Halfling Disinfectant", "p":20, "w":0.1, "d":"A small vial of this powerful mixture of herbal extracts and special vinegars destroys microorganisms and neutralizes small quantities of toxins on rotten food, essentially granting Cast-Iron Stomach (p. B80) for a single meal. It doesn’t improve flavor, so it’s probably just as well that it has a slight numbing effect on the tongue."},
			{ "n":"Necromantic Preservative", "p":110, "w":1, "d":"A single bottle of this foul-smelling liquid can completely preserve up to 20 lbs. of dead body parts for a week. Repeated treatments can extend preservation indefinitely."},
			{ "n":"10-foot Pole", "p":8, "w":5, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"6-foot Pole", "p":5, "w":3, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Collapsible Pole", "p":2, "w":1, "d":"A two-foot section of pole with threaded ends. Multiple sections may be connected (five seconds per section) to form longer poles for any use, although very long constructions are difficult to handle. Any action with a longer pole is at -1 to DX for every additional section beyond 10 feet."},
			{ "n":"Spring-Loaded Pole", "p":75, "w":7, "d":"A two-foot metal tube that springs into a six-foot pole for use with tents, fishing gear, and poking at dubious spots on the floor. Requires a Ready maneuver."},
			{ "n":"Dwarven Rations", "p":5, "w":1, "d":"See *Dungeon Fantasy 1*, page 25."},
			{ "n":"Elven Rations", "p":15, "w":0.5, "d":"See *Dungeon Fantasy 1*, page 25."},
			{ "n":"Sleeping Fur", "p":50, "w":8, "d":"See *Dungeon Fantasy 1*, page 23."},
			{ "n":"6-foot Snorkel", "p":30, "w":1, "d":"See *Dungeon Fantasy 1*, page 26."},
			{ "n":"Soap", "p":5, "w":0.5, "d":"Used by exceptionally careful thieves, a minute of work with a pint of soap and a cloth clears 10 square feet of just about any surface of contact agents. However, it can damage texts and decorated fabrics. Daily washing with soap also imposes a -2 on anyone trying to track the user by scent."},
			{ "n":"1-man Tent", "p":50, "w":5, "d":"See *Dungeon Fantasy 1*, page 23."},
			{ "n":"2-man Tent", "p":80, "w":12, "d":"See *Dungeon Fantasy 1*, page 23. Requires a pole."},
			{ "n":"4-man Tent", "p":150, "w":30, "d":"See *Dungeon Fantasy 1*, page 23. Requires two poles."},
			{ "n":"20-man Tent", "p":300, "w":100, "d":"See *Dungeon Fantasy 1*, page 25. Requires 16 poles."},
			{ "n":"Insulated 20-man Tent", "p":600, "w":200, "d":"It is made from double-thick fabric. Provides +1 to Survival rolls in cold conditions."},
			{ "n":"Waterproof 20-man Tent", "p":600, "w":150, "d":"It is made from oiled cloth or leather. Provides +1 to Survival rolls in damp conditions."},
			{ "n":"Adamant Drill", "p":800, "w":5, "d":"Like the brace and bit (*Dungeon Fantasy 1*, p. 25), but a tip of exceptionally hard material for the bit increases the armor divisor to (5)."},
			{ "n":"Brace and Bit", "p":120, "w":5, "d":"See *Dungeon Fantasy 1*, page 25."},
			{ "n":"Bow Drill", "p":8, "w":1, "d":"A cheaper, more compact drill than the brace and bit (*Dungeon Fantasy 1*, p. 25). Does th-1(2) pi+ damage per second."},
			{ "n":"Burglar Bar", "p":20, "w":2, "d":"A thin piece of metal, about 18 inches long. Gives +1 to Traps skill for springing small triggers. Can be used as a crude (-2 to skill) lockpick."},
			{ "n":"Caltrops", "p":5, "w":0.5, "d":"See *Dungeon Fantasy 1*, page 25."},
			{ "n":"Disguise Kit", "p":800, "w":10, "d":"See *Dungeon Fantasy 1*, page 25."},
			{ "n":"Door Needle", "p":4, "w":0, "d":"A tiny spike on an adhesive base, resembling a thumbtack with a sticky head. The tip is treated with strong poison (purchased separately), and the base is stuck in a concealed location on a doorknob or handle. Unwary users opening the door are stuck by the poisoned pin."},
			{ "n":"Felonious Extensors", "p":2000, "w":2, "d":"A set of lockpicks set on the ends of long, articulated rods, allowing the user to attempt to pick a lock from three feet away. Though somewhat clumsy (-3 to Lockpicking skill), it can be much safer (+5 to Traps skill for the purpose of avoiding traps)."},
			{ "n":"Basic Lockpicks", "p":50, "w":0.1, "d":"See *Dungeon Fantasy 1*, page 25."},
			{ "n":"Good Lockpicks", "p":250, "w":0.5, "d":"See *Dungeon Fantasy 1*, page 25."},
			{ "n":"Fine Lockpicks", "p":1000, "w":2, "d":"See *Dungeon Fantasy 1*, page 25."},
			{ "n":"Dwarven Lockpicks", "p":3000, "w":3, "d":"A complex set of lockpicks that adapt themselves to the lock they are being used on. If an attempt to pick a lock fails, subsequent attempts at the same lock are at a cumulative +1 to a maximum bonus of +5."},
			{ "n":"Nightingale Carpet", "p":35, "w":5, "d":"When staked down and properly set (roll against Traps at +2), taut cords embedded in these long strips of cloth will ring small bells at the end when the cloth is stepped on. This provides a portable and easy-to-set-up alarm system. Most are a dull-colored canvas that blends in well with many kinds of terrain, and they may be covered with dust and leaves for improved camouflage. Cost and weight are for a nine-foot strip."},
			{ "n":"Portable Ram", "p":150, "w":35, "d":"See *Dungeon Fantasy 1*, page 26."},
			{ "n":"Shackles", "p":200, "w":2, "d":"See *Dungeon Fantasy 1*, page 26."},
			{ "n":"Shackles", "p":200, "w":2, "d":"See *Dungeon Fantasy 1*, page 26."},
			{ "n":"Spy's Horn", "p":100, "w":2, "d":"See *Dungeon Fantasy 1*, page 26."},
			{ "n":"Man Trap", "p":180, "w":6, "d":"See *Dungeon Fantasy 1*, page 26."},
			{ "n":"Fine Man Trap", "p":360, "w":6, "d":"See *Dungeon Fantasy 1*, page 26. Like a standard man trap, but does +1 damage and has +1 ST."},
			{ "n":"Mini-Trap", "p":80, "w":2.5, "d":"See *Dungeon Fantasy 1*, page 26."},
			{ "n":"Fine Mini-Trap", "p":160, "w":2.5, "d":"See *Dungeon Fantasy 1*, page 26. A mini-trap with +1 damage and +1 ST."},
			{ "n":"Monster Trap", "p":320, "w":11, "d":"See *Dungeon Fantasy 1*, page 26."},
			{ "n":"Fine Monster Trap", "p":640, "w":11, "d":"See *Dungeon Fantasy 1*, page 26. A monster trap with +2 damage and +2 ST."},
			{ "n":"Fine Trap-Finder's Kit", "p":1250, "w":10, "d":"See *Dungeon Fantasy 4*, page 12."},
			{ "n":"Trap-Finder's Kit", "p":250, "w":2, "d":"See *Dungeon Fantasy 4*, page 12."},
			{ "n":"Trap-Finder's Kit", "p":250, "w":2, "d":"See *Dungeon Fantasy 4*, page 12."},
			{ "n":"Self-Righting Hourglass", "p":160, "w":4, "d":"See *Dungeon Fantasy 4*, page 12."},
			{ "n":"Self-Righting Hourglass", "p":160, "w":4, "d":"See *Dungeon Fantasy 4*, page 12."},
			{ "n":"Miniature Sundial", "p":40, "w":1, "d":"See *Dungeon Fantasy 1*, page 23."},
			{ "n":"Miniature Sundial", "p":40, "w":1, "d":"See *Dungeon Fantasy 1*, page 23."},
			{ "n":"Timed Candle", "p":5, "w":1, "d":"See *Dungeon Fantasy 1*, page 25."},
			{ "n":"Timed Candle", "p":5, "w":1, "d":"See *Dungeon Fantasy 1*, page 25."},
			{ "n":"Balance and Weights", "p":35, "w":3, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Balance and Weights", "p":35, "w":3, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Compass", "p":50, "w":5, "d":"See *Dungeon Fantasy 1*, page 25."},
			{ "n":"3-foot Crowbar", "p":20, "w":3, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"5-foot Crowbar", "p":60, "w":6, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Endless Chain", "p":350, "w":140, "d":"A loop of chain (20 yards total, effectively 10 yards of hauling) going around two pulleys on posts. When anchored, the chain can be used to continuously haul loads up and down or across gaps rather than letting a rope down and pulling it up again."},
			{ "n":"File", "p":40, "w":1, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Hatchet", "p":15, "w":2, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Iron Spike", "p":1, "w":0.5, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Iron Spike", "p":1, "w":0.5, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Mallet", "p":15, "w":3, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Mallet", "p":15, "w":3, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Cheap Padlock", "p":20, "w":1, "d":"See *Dungeon Fantasy 1*, page 26."},
			{ "n":"Cheap Padlock", "p":20, "w":1, "d":"See *Dungeon Fantasy 1*, page 26."},
			{ "n":"Fine Padlock", "p":8000, "w":2, "d":"See *Dungeon Fantasy 1*, page 26."},
			{ "n":"Fine Padlock", "p":8000, "w":2, "d":"See *Dungeon Fantasy 1*, page 26."},
			{ "n":"Good Padlock", "p":400, "w":2, "d":"See *Dungeon Fantasy 1*, page 26."},
			{ "n":"Good Padlock", "p":400, "w":2, "d":"See *Dungeon Fantasy 1*, page 26."},
			{ "n":"Pickaxe", "p":15, "w":8, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Pickaxe", "p":15, "w":8, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Saw", "p":150, "w":3, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Saw", "p":150, "w":3, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Shovel", "p":12, "w":6, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Steelyard", "p":100, "w":20, "d":"A heavy-duty scale, which uses a weight slid along a beam like a doctor’s office scale. It isn’t as fine-tuned as the small balance scale (*Dungeon Fantasy 1*, p. 24), but it can weigh objects up to 300 lbs."},
			{ "n":"Tongs", "p":40, "w":3, "d":"Heavy blacksmith’s tongs, good for picking up hot coals and other items a delver doesn’t want to touch."},
			{ "n":"Tongs", "p":40, "w":3, "d":"Heavy blacksmith’s tongs, good for picking up hot coals and other items a delver doesn’t want to touch."},
			{ "n":"Universal Tool Kit", "p":12000, "w":20, "d":"See *Dungeon Fantasy 4*, page 12."},
			{ "n":"Backpack Alchemy Lab", "p":1000, "w":10, "d":"See *Dungeon Fantasy 1*, page 26."},
			{ "n":"Artificer Backpack Tool Kit", "p":600, "w":20, "d":"See *Dungeon Fantasy 4*, page 12."},
			{ "n":"Blacksmith Backpack Tool Kit", "p":600, "w":20, "d":"Hammers, bellows, tongs, and chisels."},
			{ "n":"Carpenter Backpack Tool Kit", "p":600, "w":20, "d":"Saws, level, planes, and hammers."},
			{ "n":"Cobbler/Leatherworker Backpack Tool Kit", "p":200, "w":7, "d":"Large needles, shears, punches, and small hammers."},
			{ "n":"Gardener Backpack Tool Kit", "p":200, "w":7, "d":"A small saw and a variety of trowels, shears, and small rakes."},
			{ "n":"Glassblower Backpack Tool Kit", "p":600, "w":20, "d":"A crucible, pipes, blades, and smoothing tools."},
			{ "n":"Jeweler Backpack Tool Kit", "p":700, "w":5, "d":"Essentially a blacksmith’s kit, but in miniature and of high quality."},
			{ "n":"Mason Backpack Tool Kit", "p":600, "w":20, "d":"Hammers, chisels, levels, and grinding and polishing equipment."},
			{ "n":"Surveyor Backpack Tool Kit", "p":600, "w":20, "d":"Levels, chains, poles, and sighting equipment. Basic equipment for mapping dungeons."},
			{ "n":"Dwarven Accurizer", "p":1200, "w":14, "d":"See *Dungeon Fantasy 4*, page 12."},
			{ "n":"Crossbow Rest", "p":40, "w":2, "d":"See *Dungeon Fantasy 1*, page 25."},
			{ "n":"Crossbow Rest", "p":40, "w":2, "d":"See *Dungeon Fantasy 1*, page 25."},
			{ "n":"Crossbow Sight", "p":100, "w":1, "d":"See *Dungeon Fantasy 1*, page 25."},
			{ "n":"Crossbow Sight", "p":100, "w":1, "d":"See *Dungeon Fantasy 1*, page 25."},
			{ "n":"Goat's Foot", "p":50, "w":2, "d":"See *Basic Set*, page 276."},
			{ "n":"Chain Lanyard", "p":15, "w":0.5, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Leather Lanyard", "p":1, "w":0.1, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Complex Nageteppo", "p":100, "w":0.3, "d":"Acts as both a flash and smoke nageteppo (See *Dungeon Fantasy 1*, page 25)."},
			{ "n":"Flash Nageteppo", "p":40, "w":0.2, "d":"See *Dungeon Fantasy 1*, page 25."},
			{ "n":"Smoke Nageteppo", "p":40, "w":0.2, "d":"See *Dungeon Fantasy 1*, page 25."},
			{ "n":"Sprayer", "p":80, "w":4, "d":"A bellows and short tube, allowing the user to spray a liquid, such as holy water or flammable oil (unlit!) a short distance. Treat as a jet with Range 3. A sprayer holds up to three shots and requires a Ready maneuver before firing each shot."},
			{ "n":"Whetstone", "p":5, "w":1, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Whetstone", "p":5, "w":1, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Dwarven Whestone", "p":500, "w":1, "d":"See *Dungeon Fantasy 1*, page 25."},
			{ "n":"Chalk", "p":1, "w":0.25, "d":"For marking caves, walls, and other hard objects. A quarter pound is enough to mark a single typical dungeon level or to silently sketch out a single complex plan."},
			{ "n":"Coleopteran Glowing Fluid", "p":65, "w":0.5, "d":"When exposed to light, this rare ink glows faintly for an extended period: one hour if exposed to a torch, a whole day if exposed to daylight or equivalent. This allows scribes to write texts or for adventurers to make markings that can be read in the dark. One bottle is enough to mark a single typical dungeon level."},
			{ "n":"Etching Kit", "p":2600, "w":18, "d":"For making durable notes in the field. A stylus, acids, and portable treatment tanks to produced etched sheets of metal, as in dwarven books."},
			{ "n":"Faerie Ink", "p":75, "w":0.5, "d":"See *Dungeon Fantasy 4*, page 12."},
			{ "n":"Faerie Ink Developer", "p":75, "w":0.5, "d":"See *Dungeon Fantasy 4*, page 12."},
			{ "n":"20 Sheets of Paper", "p":20, "w":1, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"20 Sheets of Rice Paper", "p":40, "w":0.25, "d":"See *Dungeon Fantasy 1*, page 24. A thinner, lighter-weight paper."},
			{ "n":"Portable Scribe", "p":400, "w":15, "d":"See *Dungeon Fantasy 4*, page 12."},
			{ "n":"Rubbing Wax", "p":12, "w":1, "d":"A block of dark-colored wax, like a huge crayon. A piece of thin paper is placed over an inscription or relief painting and the wax rubbed over it to make a copy in negative. Can copy just about any inscription (including dwarven engraved books) in five seconds per square foot, but, unlike a portable scribe, will not transfer magic from scrolls. A pound of wax is good for 100 square feet of rubbings."},
			{ "n":"Scribe's Kit", "p":50, "w":2, "d":"See *Dungeon Fantasy 1*, page 24."},
			{ "n":"Shield Lectern", "p":50, "w":2, "d":"See *Dungeon Fantasy 4*, page 12."},
			{ "n":"Wax Tablet", "p":10, "w":2, "d":"See *Dungeon Fantasy 4*, page 12."}			
		];
		
		var rand1 = Math.ceil(Math.random()*(accoutrements.length-1));
		
		itemName = accoutrements[rand1].n;
		itemPrice = accoutrements[rand1].p;
		itemWeight = accoutrements[rand1].w;
		itemDesc = accoutrements[rand1].d;
		
		var newMessage = "";
		
		if (isGachapon) {
			newMessage = "\nYou played the Fantasy Gachapon for 1000 credits and got:";
		}
		
		iBasePrice = itemPrice;
		
		var randType = Math.ceil(Math.random()*12);
		
		if (randType == 7 || randType == 8 || randType == 9) {
			var embellishment = getDF8Embellishment("hard");
			var motif = getMotif();
			
			randEmb = Math.ceil(Math.random()*3);
			if (randEmb == 1) { var embellishment = getDF8Embellishment("hard"); } else { var embellishment = getDF8Embellishment("supernatural"); }
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment.n + ". " + embellishment.d;
			if (embellishment.n != "Fine Material" && embellishment.n != "Exceptional Material" && randEmb == 1 && embellishment.n != "Silver Plating" && embellishment.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif + "."; }
			itemPrice = itemPrice * (1 + embellishment.v);
		}
		
		if (randType == 10 || randType == 11) {
			randEmb = Math.ceil(Math.random()*3);
			if (randEmb == 1) { var embellishment = getDF8Embellishment("hard"); } else { var embellishment = getDF8Embellishment("supernatural"); }
			var motif = getMotif();
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment.n + ". " + embellishment.d;
			if (embellishment.n != "Fine Material" && embellishment.n != "Exceptional Material" && randEmb == 1 && embellishment.n != "Silver Plating" && embellishment.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif + "."; }
			
			randEmb2 = Math.ceil(Math.random()*3);
			if (randEmb2 == 1) { var embellishment2 = getDF8Embellishment("hard"); } else { var embellishment2 = getDF8Embellishment("supernatural"); }
			var motif2 = getMotif();
			
			while (embellishment2 == embellishment || (embellishment.n.includes("Made of") && embellishment2.n.includes("Made of"))) {
				randEmb2 = Math.ceil(Math.random()*3);
				if (randEmb2 == 1) { var embellishment2 = getDF8Embellishment("hard"); } else { var embellishment2 = getDF8Embellishment("supernatural"); }
				var motif2 = getMotif();
			}
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment2.n + ". " + embellishment2.d;
			if (embellishment2.n != "Fine Material" && embellishment2.n != "Exceptional Material" && randEmb2 == 1 && embellishment2.n != "Silver Plating" && embellishment2.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif2 + "."; }
			itemPrice = itemPrice * (1 + embellishment.v + embellishment2.v);
			
		}
		
		if (randType == 12) {
			randEmb = Math.ceil(Math.random()*3);
			if (randEmb == 1) { var embellishment = getDF8Embellishment("hard"); } else { var embellishment = getDF8Embellishment("supernatural"); }
			var motif = getMotif();
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment.n + ". " + embellishment.d;
			if (embellishment.n != "Fine Material" && embellishment.n != "Exceptional Material" && randEmb == 1 && embellishment.n != "Silver Plating" && embellishment.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif + "."; }
			
			randEmb2 = Math.ceil(Math.random()*3);
			if (randEmb2 == 1) { var embellishment2 = getDF8Embellishment("hard"); } else { var embellishment2 = getDF8Embellishment("supernatural"); }
			var motif2 = getMotif();
			
			while (embellishment2 == embellishment || (embellishment.n.includes("Made of") && embellishment2.n.includes("Made of"))) {
				randEmb2 = Math.ceil(Math.random()*3);
				if (randEmb2 == 1) { var embellishment2 = getDF8Embellishment("hard"); } else { var embellishment2 = getDF8Embellishment("supernatural"); }
				var motif2 = getMotif();
			}
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment2.n + ". " + embellishment2.d;
			if (embellishment2.n != "Fine Material" && embellishment2.n != "Exceptional Material" && randEmb2 == 1 && embellishment2.n != "Silver Plating" && embellishment2.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif2 + "."; }
			
			
			randEmb3 = Math.ceil(Math.random()*3);
			if (randEmb3 == 1) { var embellishment3 = getDF8Embellishment("hard"); } else { var embellishment3 = getDF8Embellishment("supernatural"); }
			var motif3 = getMotif();
			
			while ((embellishment3 == embellishment2 || embellishment3 == embellishment) || (embellishment.n.includes("Made of") && embellishment3.n.includes("Made of")) || (embellishment2.n.includes("Made of") && embellishment3.n.includes("Made of"))) {
				randEmb3 = Math.ceil(Math.random()*3);
				if (randEmb3 == 1) { var embellishment3 = getDF8Embellishment("hard"); } else { var embellishment3 = getDF8Embellishment("supernatural"); }
				var motif3 = getMotif();
			}
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment3.n + ". " + embellishment3.d;
			if (embellishment3.n != "Fine Material" && embellishment3.n != "Exceptional Material" && randEmb3 == 1 && embellishment3.n != "Silver Plating" && embellishment3.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif3 + "."; }
			
			
			
			itemPrice = itemPrice * (1 + embellishment.v + embellishment2.v + embellishment3.v);
		}
	
		itemPrice = Math.ceil(itemPrice);
		
		if (!dontPrint) {
			message.channel.send(message.author + newMessage + "\nAccoutrement: " + itemName + "\n" + itemWeight+ " lbs. *$" + itemPrice + "*\n" + itemDesc);
		}
		
		newArgs = [ itemName, itemPrice, itemWeight, 4, 1, "DF8, p21 - 27", itemDesc, iBasePrice];
		return newArgs;
		
	}

	if (itemType == "scroll") {
		var skillLevelRand = Math.ceil(Math.random()*6);
		var costPerPoint = (skillLevelRand+16)+4*skillLevelRand;
		
		var scrProp1 = Math.ceil(Math.random()*6);
		var scrProp2 = Math.ceil(Math.random()*6);
		var property = "";
		var property2 = "";
		var propDesc = "";
		var propDesc2 = "";
		var propCostMultiplier = 1;
		var propCostAdd = 0;
		var scrWeight = 0.05;
		
		if ((scrProp1 == 1 || scrProp1 == 2) && (scrProp2 == 1 && scrProp2 == 2)) { property = "No Remarkable Properties"; propDesc = "Just a scroll.";}
		if ((scrProp1 == 1 || scrProp1 == 2) && (scrProp2 == 3 && scrProp2 == 4)) { property = "Charged"; propDesc = "A caster scroll that works at no energy cost to the reader! If it casts a maintainable spell, duration is fixed – the user cannot terminate the spell early or extend it (but it’s still subject to Dispel Magic, etc.)."; propCostMultiplier = 2.5;}
		if ((scrProp1 == 1 || scrProp1 == 2) && (scrProp2 == 5 && scrProp2 == 6)) { property = "Universal"; propDesc = "A scroll that anybody can activate – paying energy as usual, unless the scroll is also charged. The underlying spell type doesn’t change. To learn from such a scroll you must still be a caster who meets all the prerequisites."; propCostMultiplier = 2;}
		
		if ((scrProp1 == 3 || scrProp1 == 4) && (scrProp2 == 1 && scrProp2 == 2)) { property = "Embroidered Cloth"; propDesc = "Takes 3 HP of flame to ignite. Not ruined by water. When read, embroidery explodes in a cloud of threads, leaving a convenient handkerchief."; propCostAdd = 5;}
		if ((scrProp1 == 3 || scrProp1 == 4) && (scrProp2 == 3 && scrProp2 == 4)) { property = "Engraved Metal"; propDesc = "Won’t be destroyed by accident! Engraving vanishes when the spell is cast, leaving a thin metal sheet (usually copper). Heavier than a normal scroll."; scrWeight = 0.25; propCostAdd = 30;}
		if ((scrProp1 == 3 || scrProp1 == 4) && (scrProp2 == 5 && scrProp2 == 6)) { property = "Stone Tablet"; propDesc = "Definitely won’t be destroyed by accident – and could even be used as a light shield, at -2 to skill – but crumbles to sand once read. Weighs a lot more than a normal scroll would!"; scrWeight = 2; propCostAdd = 20;}
		
		if ((scrProp1 == 5 || scrProp1 == 6) && (scrProp2 == 1 && scrProp2 == 2)) { property = "Tattooed Leather"; propDesc = "Surprisingly tough, and only likely to be ruined if deliberately cut or hit by 10 HP of flame. Evil cleric spells, Zombie spells, etc., might be on the skin of sapient beings (-4 reactions!). Ink drips off like blood when read. Weighs more than a normal scroll."; scrWeight = 0.1; propCostAdd = 0.1;}
		if ((scrProp1 == 5 || scrProp1 == 6) && (scrProp2 == 3 && scrProp2 == 4)) { property = "Clay Slab"; propDesc = "Clay slabs are heavy but slightly more durable than paper. They’ve been baked for better preservation, so water and fire don’t damage them. However, they shatter if they take 2 HP of damage. They crumble into dust when read."; scrWeight = 1; propCostAdd = -2;}
		if ((scrProp1 == 5 || scrProp1 == 6) && (scrProp2 == 5 && scrProp2 == 6)) {
			scrProp1 = Math.ceil(Math.random()*6);
			if (scrProp1 < 4) { property = "Charged"; propDesc = "A caster scroll that works at no energy cost to the reader! If it casts a maintainable spell, duration is fixed – the user cannot terminate the spell early or extend it (but it’s still subject to Dispel Magic, etc.)."; }
			if (scrProp1 > 3) { property = "Universal"; propDesc = "A scroll that anybody can activate – paying energy as usual, unless the scroll is also charged. The underlying spell type doesn’t change. To learn from such a scroll you must still be a caster who meets all the prerequisites.";}
			
			scrProp1 = math.ceil(Math.random()*6);
			
			
			if (scrProp1 == 1) {
				if (property == "Charged") { property2 = "Universal"; propDesc2 = "A scroll that anybody can activate – paying energy as usual, unless the scroll is also charged. The underlying spell type doesn’t change. To learn from such a scroll you must still be a caster who meets all the prerequisites."; propCostMultiplier = 5;}
				else if (property == "Universal") { property2 = "Charged"; propDesc2 = "A caster scroll that works at no energy cost to the reader! If it casts a maintainable spell, duration is fixed – the user cannot terminate the spell early or extend it (but it’s still subject to Dispel Magic, etc.)."; propCostMultiplier = 5;}
			}
			
			if (scrProp1 == 2) { property2 = "Embroidered Cloth"; propDesc2 = "Takes 3 HP of flame to ignite. Not ruined by water. When read, embroidery explodes in a cloud of threads, leaving a convenient handkerchief."; propCostAdd = 5; }
			if (scrProp1 == 3) { property2 = "Engraved Metal"; propDesc2 = "Won’t be destroyed by accident! Engraving vanishes when the spell is cast, leaving a thin metal sheet (usually copper). Heavier than a normal scroll."; scrWeight = 0.25; propCostAdd = 30; }
			if (scrProp1 == 4) { property2 = "Stone Tablet"; propDesc2 = "Definitely won’t be destroyed by accident – and could even be used as a light shield, at -2 to skill – but crumbles to sand once read. Weighs a lot more than a normal scroll would!"; scrWeight = 2; propCostAdd = 20; }
			if (scrProp1 == 5) { property2 = "Tattooed Leather"; propDesc2 = "Surprisingly tough, and only likely to be ruined if deliberately cut or hit by 10 HP of flame. Evil cleric spells, Zombie spells, etc., might be on the skin of sapient beings (-4 reactions!). Ink drips off like blood when read. Weighs more than a normal scroll."; scrWeight = 0.1; propCostAdd = 0.1; }
			if (scrProp1 == 6) { property2 = "Clay Slab"; propDesc2 = "Clay slabs are heavy but slightly more durable than paper. They’ve been baked for better preservation, so water and fire don’t damage them. However, they shatter if they take 2 HP of damage. They crumble into dust when read."; scrWeight = 1; propCostAdd = -2; }
		}
		
		var scrollInfo = getEnchantment();
		
		var newMessage = "";
		
		if (isGachapon) {
			newMessage = "\nYou played the Fantasy Gachapon for 1000 credits and got:";
		}
		
		itemDesc = "Skill Level " + (skillLevelRand+14);
		itemPrice = (((costPerPoint*scrollInfo.reserve)*propCostMultiplier)+propCostAdd);
		
		iBasePrice = itemPrice;
		
		var randType = Math.ceil(Math.random()*12);
		
		if (randType == 7 || randType == 8 || randType == 9) {
			var embellishment = getDF8Embellishment("soft");
			var motif = getMotif();
			
			randEmb = Math.ceil(Math.random()*3);
			if (randEmb == 1) { var embellishment = getDF8Embellishment("soft"); } else { var embellishment = getDF8Embellishment("supernatural"); }
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment.n + ". " + embellishment.d;
			if (embellishment.n != "Fine Material" && embellishment.n != "Exceptional Material" && randEmb == 1 && embellishment.n != "Silver Plating" && embellishment.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif + "."; }
			itemPrice = itemPrice * (1 + embellishment.v);
		}
		
		if (randType == 10 || randType == 11) {
			randEmb = Math.ceil(Math.random()*3);
			if (randEmb == 1) { var embellishment = getDF8Embellishment("soft"); } else { var embellishment = getDF8Embellishment("supernatural"); }
			var motif = getMotif();
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment.n + ". " + embellishment.d;
			if (embellishment.n != "Fine Material" && embellishment.n != "Exceptional Material" && randEmb == 1 && embellishment.n != "Silver Plating" && embellishment.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif + "."; }
			
			randEmb2 = Math.ceil(Math.random()*3);
			if (randEmb2 == 1) { var embellishment2 = getDF8Embellishment("soft"); } else { var embellishment2 = getDF8Embellishment("supernatural"); }
			var motif2 = getMotif();
			
			while (embellishment2 == embellishment || (embellishment.n.includes("Made of") && embellishment2.n.includes("Made of"))) {
				randEmb2 = Math.ceil(Math.random()*3);
				if (randEmb2 == 1) { var embellishment2 = getDF8Embellishment("soft"); } else { var embellishment2 = getDF8Embellishment("supernatural"); }
				var motif2 = getMotif();
			}
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment2.n + ". " + embellishment2.d;
			if (embellishment2.n != "Fine Material" && embellishment2.n != "Exceptional Material" && randEmb2 == 1 && embellishment2.n != "Silver Plating" && embellishment2.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif2 + "."; }
			itemPrice = itemPrice * (1 + embellishment.v + embellishment2.v);
			
		}
		
		if (randType == 12) {
			randEmb = Math.ceil(Math.random()*3);
			if (randEmb == 1) { var embellishment = getDF8Embellishment("soft"); } else { var embellishment = getDF8Embellishment("supernatural"); }
			var motif = getMotif();
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment.n + ". " + embellishment.d;
			if (embellishment.n != "Fine Material" && embellishment.n != "Exceptional Material" && randEmb == 1 && embellishment.n != "Silver Plating" && embellishment.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif + "."; }
			
			randEmb2 = Math.ceil(Math.random()*3);
			if (randEmb2 == 1) { var embellishment2 = getDF8Embellishment("soft"); } else { var embellishment2 = getDF8Embellishment("supernatural"); }
			var motif2 = getMotif();
			
			while (embellishment2 == embellishment || (embellishment.n.includes("Made of") && embellishment2.n.includes("Made of"))) {
				randEmb2 = Math.ceil(Math.random()*3);
				if (randEmb2 == 1) { var embellishment2 = getDF8Embellishment("soft"); } else { var embellishment2 = getDF8Embellishment("supernatural"); }
				var motif2 = getMotif();
			}
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment2.n + ". " + embellishment2.d;
			if (embellishment2.n != "Fine Material" && embellishment2.n != "Exceptional Material" && randEmb2 == 1 && embellishment2.n != "Silver Plating" && embellishment2.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif2 + "."; }
			
			
			randEmb3 = Math.ceil(Math.random()*3);
			if (randEmb3 == 1) { var embellishment3 = getDF8Embellishment("soft"); } else { var embellishment3 = getDF8Embellishment("supernatural"); }
			var motif3 = getMotif();
			
			while ((embellishment3 == embellishment2 || embellishment3 == embellishment) || (embellishment.n.includes("Made of") && embellishment3.n.includes("Made of")) || (embellishment2.n.includes("Made of") && embellishment3.n.includes("Made of"))) {
				randEmb3 = Math.ceil(Math.random()*3);
				if (randEmb3 == 1) { var embellishment3 = getDF8Embellishment("soft"); } else { var embellishment3 = getDF8Embellishment("supernatural"); }
				var motif3 = getMotif();
			}
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment3.n + ". " + embellishment3.d;
			if (embellishment3.n != "Fine Material" && embellishment3.n != "Exceptional Material" && randEmb3 == 1 && embellishment3.n != "Silver Plating" && embellishment3.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif3 + "."; }
			
			
			
			itemPrice = itemPrice * (1 + embellishment.v + embellishment2.v + embellishment3.v);
		}
	
		itemPrice = Math.ceil(itemPrice);
		
		
		newName = "";
		
		if (property2 != "") {
			newName = property + " " + property2 + " Scroll of " + scrollInfo.name + " (" + scrollInfo.type + ")";
			if (!dontPrint) {
				message.channel.send(message.author + newMessage + "\nScroll: " + property + " " + property2 + " Scroll of " + scrollInfo.name + " (" + scrollInfo.type + ")\n" + scrWeight + " lbs. *$" + (((costPerPoint*scrollInfo.reserve)*propCostMultiplier)+propCostAdd) + "*\n" + itemDesc);
			}
		}
		
		else if (property2 == "") {
			newName = property + " Scroll of " + scrollInfo.name + " (" + scrollInfo.type + ")";
			if (!dontPrint) {
				message.channel.send(message.author + newMessage + "\nScroll: " + property + " Scroll of " + scrollInfo.name + " (" + scrollInfo.type + ")\n" + scrWeight + " lbs. *$" + (((costPerPoint*scrollInfo.reserve)*propCostMultiplier)+propCostAdd) + "*\n" + itemDesc);
			}
		}
		
		newPrice = (((costPerPoint*scrollInfo.reserve)*propCostMultiplier)+propCostAdd);
		
		newArgs = [ newName, itemPrice, scrWeight, 4, 1, "GURPS Magic", itemDesc, iBasePrice];
		return newArgs;
		
	}

	if (itemType == "basicMelee") {
		var rand1 = Math.ceil(Math.random()*69);
		
		var itemPrice = 0;
		var itemWeight = 0;
		var itemName = "";
		var pageRef = "";
		var itemTL = 4;
		var itemDesc = "";
		
		var basicMeleeWeps = [
			{ "n":"Axe", "p":50, "w":4, "pg":"B271", "tl":0}, { "n":"Bastard Sword", "p":650, "w":5, "pg":"B271", "tl":3},
			{ "n":"Baton", "p":20, "w":1, "pg":"B273", "tl":0}, { "n":"Blackjack", "p":20, "w":1, "pg":"B271", "tl":1},
			{ "n":"Brass Knuckles", "p":10, "w":0.25, "pg":"B271", "tl":1},	{ "n":"Broadsword", "p":500, "w":3, "pg":"B271", "tl":2},
			{ "n":"Cavalry Saber", "p":500, "w":3, "pg":"B271", "tl":4}, { "n":"Cutlass", "p":300, "w":2, "pg":"B273", "tl":4},
			{ "n":"Dagger", "p":20, "w":0.25, "pg":"B272", "tl":1},	{ "n":"Flail", "p":100, "w":8, "pg":"B274", "tl":2},
			{ "n":"Garrote", "p":2, "w":0, "pg":"B272", "tl":0}, { "n":"Glaive", "p":8, "w":8, "pg":"B272", "tl":1},
			{ "n":"Great Axe", "p":100, "w":8, "pg":"B274", "tl":1}, { "n":"Greatsword", "p":800, "w":7, "pg":"B274", "tl":3},
			{ "n":"Halberd", "p":150, "w":12, "pg":"B272", "tl":3},	{ "n":"Harpoon", "p":60, "w":6, "pg":"B276", "tl":2},
			{ "n":"Hatchet", "p":40, "w":2, "pg":"B271", "tl":0}, { "n":"Javelin", "p":30, "w":2, "pg":"B273", "tl":1},
			{ "n":"Katana", "p":650, "w":5, "pg":"B271", "tl":3}, { "n":"Kusari", "p":70, "w":5, "pg":"B272", "tl":3},
			{ "n":"Lance", "p":60, "w":6, "pg":"B272", "tl":2},	{ "n":"Large Knife", "p":40, "w":1, "pg":"B272", "tl":0},
			{ "n":"Large Net", "p":40, "w":20, "pg":"B276", "tl":0}, { "n":"Lariat", "p":40, "w":3, "pg":"B276", "tl":1},
			{ "n":"Light Club", "p":5, "w":3, "pg":"B271", "tl":0}, { "n":"Long Spear", "p":60, "w":5, "pg":"B273", "tl":2},
			{ "n":"Mace", "p":50, "w":5, "pg":"B271", "tl":2}, { "n":"Maul", "p":20, "w":12, "pg":"B274", "tl":0},
			{ "n":"Melee Net", "p":20, "w":5, "pg":"B276", "tl":2}, { "n":"Morningstar", "p":80, "w":6, "pg":"B272", "tl":3},
			{ "n":"Naginata", "p":100, "w":6, "pg":"B272", "tl":2}, { "n":"Nunchaku", "p":20, "w":2, "pg":"B272", "tl":3},
			{ "n":"Pick", "p":70, "w":3, "pg":"B271", "tl":3}, { "n":"Poleaxe", "p":120, "w":10, "pg":"B272", "tl":3},
			{ "n":"Quarterstaff", "p":10, "w":4, "pg":"B273", "tl":0}, { "n":"Rapier", "p":500, "w":2.75, "pg":"B273", "tl":4},
			{ "n":"Saber", "p":700, "w":2, "pg":"B273", "tl":4}, { "n":"Scythe", "p":15, "w":5, "pg":"B274", "tl":1},
			{ "n":"Short Staff", "p":20, "w":1, "pg":"B273", "tl":0}, { "n":"Shortsword", "p":400, "w":2, "pg":"B273", "tl":2},
			{ "n":"Small Knife", "p":30, "w":0.5, "pg":"B272", "tl":0}, { "n":"Small Mace", "p":35, "w":3, "pg":"B271", "tl":2},
			{ "n":"Smallsword", "p":400, "w":1.5, "pg":"B273", "tl":4}, { "n":"Spear", "p":40, "w":4, "pg":"B273", "tl":0},
			{ "n":"Spear", "p":40, "w":4, "pg":"B273", "tl":0}, { "n":"Throwing Axe", "p":60, "w":4, "pg":"B271", "tl":0},
			{ "n":"Thrusting Bastard Sword", "p":750, "w":5, "pg":"B274", "tl":3}, { "n":"Thrusting Broadsword", "p":600, "w":3, "pg":"B271", "tl":2},
			{ "n":"Thrusting Greatsword", "p":900, "w":7, "pg":"B274", "tl":3}, { "n":"Warhammer", "p":100, "w":7, "pg":"B274", "tl":3},
			{ "n":"Whip (2 Yards)", "p":40, "w":4, "pg":"B274", "tl":1}, { "n":"Whip (3 Yards)", "p":60, "w":6, "pg":"B274", "tl":1},
			{ "n":"Wooden Stake", "p":4, "w":0.5, "pg":"B272", "tl":0}
		];
		
		var randomWep = basicMeleeWeps[Math.ceil(Math.random()*(basicMeleeWeps.length - 1))];
		
		itemName = randomWep.n;
		itemPrice = randomWep.p;
		pageRef = randomWep.pg;
		itemTL = randomWep.tl;
				
		var newMessage = "";
		
		if (isGachapon) {
			newMessage = "\nYou played the Fantasy Gachapon for 1000 credits and got:";
		}
		
		iBasePrice = itemPrice;
		
		var randType = Math.ceil(Math.random()*12);
		
		if (randType == 7 || randType == 8 || randType == 9) {
			var embellishment = getDF8Embellishment("hard");
			var motif = getMotif();
			
			randEmb = Math.ceil(Math.random()*3);
			if (randEmb == 1) { var embellishment = getDF8Embellishment("hard"); } else { var embellishment = getDF8Embellishment("supernatural"); }
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment.n + ". " + embellishment.d;
			if (embellishment.n != "Fine Material" && embellishment.n != "Exceptional Material" && randEmb == 1 && embellishment.n != "Silver Plating" && embellishment.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif + "."; }
			itemPrice = itemPrice * (1 + embellishment.v);
		}
		
		if (randType == 10 || randType == 11) {
			randEmb = Math.ceil(Math.random()*3);
			if (randEmb == 1) { var embellishment = getDF8Embellishment("hard"); } else { var embellishment = getDF8Embellishment("supernatural"); }
			var motif = getMotif();
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment.n + ". " + embellishment.d;
			if (embellishment.n != "Fine Material" && embellishment.n != "Exceptional Material" && randEmb == 1 && embellishment.n != "Silver Plating" && embellishment.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif + "."; }
			
			randEmb2 = Math.ceil(Math.random()*3);
			if (randEmb2 == 1) { var embellishment2 = getDF8Embellishment("hard"); } else { var embellishment2 = getDF8Embellishment("supernatural"); }
			var motif2 = getMotif();
			
			while (embellishment2 == embellishment || (embellishment.n.includes("Made of") && embellishment2.n.includes("Made of"))) {
				randEmb2 = Math.ceil(Math.random()*3);
				if (randEmb2 == 1) { var embellishment2 = getDF8Embellishment("hard"); } else { var embellishment2 = getDF8Embellishment("supernatural"); }
				var motif2 = getMotif();
			}
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment2.n + ". " + embellishment2.d;
			if (embellishment2.n != "Fine Material" && embellishment2.n != "Exceptional Material" && randEmb2 == 1 && embellishment2.n != "Silver Plating" && embellishment2.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif2 + "."; }
			itemPrice = itemPrice * (1 + embellishment.v + embellishment2.v);
			
		}
		
		if (randType == 12) {
			randEmb = Math.ceil(Math.random()*3);
			if (randEmb == 1) { var embellishment = getDF8Embellishment("hard"); } else { var embellishment = getDF8Embellishment("supernatural"); }
			var motif = getMotif();
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment.n + ". " + embellishment.d;
			if (embellishment.n != "Fine Material" && embellishment.n != "Exceptional Material" && randEmb == 1 && embellishment.n != "Silver Plating" && embellishment.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif + "."; }
			
			randEmb2 = Math.ceil(Math.random()*3);
			if (randEmb2 == 1) { var embellishment2 = getDF8Embellishment("hard"); } else { var embellishment2 = getDF8Embellishment("supernatural"); }
			var motif2 = getMotif();
			
			while (embellishment2 == embellishment || (embellishment.n.includes("Made of") && embellishment2.n.includes("Made of"))) {
				randEmb2 = Math.ceil(Math.random()*3);
				if (randEmb2 == 1) { var embellishment2 = getDF8Embellishment("hard"); } else { var embellishment2 = getDF8Embellishment("supernatural"); }
				var motif2 = getMotif();
			}
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment2.n + ". " + embellishment2.d;
			if (embellishment2.n != "Fine Material" && embellishment2.n != "Exceptional Material" && randEmb2 == 1 && embellishment2.n != "Silver Plating" && embellishment2.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif2 + "."; }
			
			
			randEmb3 = Math.ceil(Math.random()*3);
			if (randEmb3 == 1) { var embellishment3 = getDF8Embellishment("hard"); } else { var embellishment3 = getDF8Embellishment("supernatural"); }
			var motif3 = getMotif();
			
			while ((embellishment3 == embellishment2 || embellishment3 == embellishment) || (embellishment.n.includes("Made of") && embellishment3.n.includes("Made of")) || (embellishment2.n.includes("Made of") && embellishment3.n.includes("Made of"))) {
				randEmb3 = Math.ceil(Math.random()*3);
				if (randEmb3 == 1) { var embellishment3 = getDF8Embellishment("hard"); } else { var embellishment3 = getDF8Embellishment("supernatural"); }
				var motif3 = getMotif();
			}
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment3.n + ". " + embellishment3.d;
			if (embellishment3.n != "Fine Material" && embellishment3.n != "Exceptional Material" && randEmb3 == 1 && embellishment3.n != "Silver Plating" && embellishment3.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif3 + "."; }
			
			
			
			itemPrice = itemPrice * (1 + embellishment.v + embellishment2.v + embellishment3.v);
		}
	
		
		var randEnchantChance = Math.ceil(Math.random()*24);
		
		if (randEnchantChance == 24) {
			var enchantment = getWeaponEnchant("Melee");
			itemName = itemName + " of " + enchantment.name;
			itemDesc = itemDesc + "\n\nEnchanted with `" + enchantment.name + "`.";
			if (enchantment.desc != "") {
				itemDesc = itemDesc + "\n" + enchantment.desc;
			}
			if (enchantment.costPerPound) {
				itemPrice = itemPrice + (enchantment.cost*Math.ceil(itemWeight));
			} else {
				itemPrice = itemPrice + enchantment.cost;
			}
		}
		
		
		
		var finalMessage = newMessage + "\nMelee Weapon (Basic Set): ";
		finalMessage = finalMessage + itemName + " ";
		finalMessage = finalMessage + "\n";
		finalMessage = finalMessage + itemWeight + " lbs. *$" + itemPrice + "*\n";
		
		if (itemDesc != "") {
			finalMessage = finalMessage + itemDesc;
		}
		
		if (!dontPrint) {
			message.channel.send(message.author + ":" + finalMessage);
		}
		
		newArgs = [ itemName, itemPrice, itemWeight, itemTL, 1, pageRef, itemDesc, iBasePrice];
		return newArgs;
		
	}

	if (itemType == "martialMelee") {
		var rand1 = Math.ceil(Math.random()*70);
		
		var itemPrice = 0;
		var itemWeight = 0;
		var itemName = "";
		var itemTL = 4;
		var itemDesc = "";
		var pageRef = "";
		
		var martialMeleeWeps = [
			{ "n":"Backsword", "p":450, "w":3, "pg":"MA227", "tl":4}, { "n":"Balisong", "p":50, "w":0.5, "pg":"MA228", "tl":3},
			{ "n":"Bill", "p":125, "w":8, "pg":"MA229", "tl":3}, { "n":"Bladed Hand", "p":100, "w":1, "pg":"MA226", "tl":3},
			{ "n":"Bokken", "p":40, "w":3, "pg":"MA227", "tl":3}, { "n":"Bola Perdida", "p":10, "w":1, "pg":"MA227", "tl":0},
			{ "n":"Cestus", "p":50, "w":1, "pg":"MA226", "tl":2}, { "n":"Chain Whip (2 Yards)", "p":100, "w":6, "pg":"MA228", "tl":3},
			{ "n":"Chain Whip (3 Yards)", "p":150, "w":9, "pg":"MA228", "tl":3}, { "n":"Combat Fan", "p":40, "w":1, "pg":"MA226", "tl":3},
			{ "n":"Dao", "p":700, "w":5, "pg":"MA227", "tl":3}, { "n":"Deer Antlers", "p":75, "w":1.5, "pg":"MA228", "tl":3},
			{ "n":"Dress Smallsword", "p":300, "w":1, "pg":"MA229", "tl":4}, { "n":"Dueling Bill", "p":100, "w":6, "pg":"MA229", "tl":3},
			{ "n":"Dueling Glaive", "p":80, "w":6, "pg":"MA229", "tl":3}, { "n":"Dueling Halberd", "p":120, "w":10, "pg":"MA229", "tl":3},
			{ "n":"Dueling Pollaxe", "p":100, "w":8, "pg":"MA229", "tl":3 }, { "n":"Dusack", "p":30, "w":1.5, "pg":"MA229", "tl":2},
			{ "n":"Edged Rapier", "p":1000, "w":3, "pg":"MA227", "tl":4}, { "n":"Eku", "p":40, "w":8, "pg":"MA229", "tl":0},
			{ "n":"Estoc", "p":500, "w":3, "pg":"MA227", "tl":3}, { "n":"Falchion", "p":400, "w":3, "pg":"MA229", "tl":2},
			{ "n":"Gada", "p":100, "w":15, "pg":"MA230", "tl":1}, { "n":"Heavy Horse-Cutter", "p":150, "w":12, "pg":"MA229", "tl":3},
			{ "n":"Heavy Spear", "p":90, "w":6, "pg":"MA229", "tl":1}, { "n":"Hook Sword", "p":200, "w":3, "pg":"MA226", "tl":3},
			{ "n":"Jian", "p":700, "w":3, "pg":"MA227", "tl":3, "tl":3}, { "n":"Jo", "p":10, "w":2, "pg":"MA227", "tl":0},
			{ "n":"Jutte", "p":40, "w":1, "pg":"MA227", "tl":3}, { "n":"Kakute", "p":10, "w":0.1, "pg":"MA227", "tl":3},
			{ "n":"Katar", "p":50, "w":1, "pg":"MA228", "tl":2}, { "n":"Knife-Wheel", "p":75, "w":1.5, "pg":"MA228", "tl":3},
			{ "n":"Knobbed Club", "p":20, "w":2, "pg":"MA226", "tl":0}, { "n":"Kukri", "p":50, "w":1.5, "pg":"MA228", "tl":2},
			{ "n":"Kusarigama", "p":80, "w":4.5, "pg":"MA228", "tl":3}, { "n":"Kusarijutte", "p":80, "w":3.5, "pg":"MA228", "tl":3},
			{ "n":"Lajatang", "p":100, "w":7, "pg":"MA229", "tl":3}, { "n":"Large Falchion", "p":625, "w":4.5, "pg":"MA227", "tl":3},
			{ "n":"Large Katar", "p":400, "w":2, "pg":"MA229", "tl":2}, { "n":"Late Katana", "p":550, "w":3, "pg":"MA227", "tl":4},
			{ "n":"Light Edged Rapier", "p":700, "w":2.25, "pg":"MA229", "tl":4}, { "n":"Light Horse-Cutter", "p":120, "w":8, "pg":"MA229", "tl":3},
			{ "n":"Light Rapier", "p":400, "w":2, "pg":"MA229", "tl":4}, { "n":"Long Knife", "p":120, "w":1.5, "pg":"MA228", "tl":2},
			{ "n":"Long Staff", "p":15, "w":5, "pg":"MA230", "tl":0}, { "n":"Longsword", "p":700, "w":4, "pg":"MA227", "tl":3},
			{ "n":"Main-Gauche", "p":50, "w":1.25, "pg":"MA228", "tl":4}, { "n":"Monk's Spade", "p":100, "w":6, "pg":"MA229", "tl":3},
			{ "n":"Myrmex", "p":20, "w":0.25, "pg":"MA226", "tl":1}, { "n":"Qian Kun Ri Yue Dao", "p":250, "w":3, "pg":"MA226", "tl":3},
			{ "n":"Rondel Dagger", "p":40, "w":1, "pg":"MA228", "tl":3}, { "n":"Rope Dart", "p":30, "w":0.5, "pg":"MA228", "tl":2},
			{ "n":"Round Mace", "p":35, "w":5, "pg":"MA226", "tl":0}, { "n":"Sai", "p":60, "w":1.5, "pg":"MA227", "tl":3},
			{ "n":"Short Baton", "p":10, "w":0.5, "pg":"MA228", "tl":0}, { "n":"Short Spear", "p":30, "w":2, "pg":"MA229", "tl":1},
			{ "n":"Sickle", "p":40, "w":2, "pg":"MA226", "tl":1}, { "n":"Slashing Wheel", "p":60, "w":1, "pg":"MA228", "tl":3},
			{ "n":"Small Axe", "p":45, "w":3, "pg":"MA226", "tl":0}, { "n":"Small Falchion", "p":200, "w":2, "pg":"MA229", "tl":2},
			{ "n":"Small Round Mace", "p":25, "w":3, "pg":"MA226", "tl":0}, { "n":"Small Throwing Axe", "p":50, "w":3, "pg":"MA226", "tl":0},
			{ "n":"Sodegarami", "p":100, "w":4, "pg":"MA230", "tl":3}, { "n":"Stiletto", "p":20, "w":0.25, "pg":"MA228", "tl":3},
			{ "n":"Tetsubo", "p":100, "w":10, "pg":"MA230", "tl":2}, { "n":"Three-Part Staff", "p":60, "w":5, "pg":"MA230", "tl":2},
			{ "n":"Tonfa", "p":40, "w":1.5, "pg":"MA226", "tl":3}, { "n":"Trident", "p":80, "w":5, "pg":"MA229", "tl":2},
			{ "n":"Urumi", "p":400, "w":4, "pg":"MA230", "tl":3}, { "n":"Weighted Scarf", "p":10, "w":1, "pg":"MA227", "tl":0}
		];
		
		var randomWep = martialMeleeWeps[Math.ceil(Math.random() * (martialMeleeWeps.length - 1))];
		
		itemName = randomWep.n;
		itemPrice = randomWep.p;
		itemWeight = randomWep.w;
		pageRef = randomWep.pg;
		itemTL = randomWep.tl;
		
		iBasePrice = itemPrice;
		
		var newMessage = "";
		
		if (isGachapon) {
			newMessage = "\nYou played the Fantasy Gachapon for 1000 credits and got:";
		}
		
		var randType = Math.ceil(Math.random()*12);
		
		if (randType == 7 || randType == 8 || randType == 9) {
			var embellishment = getDF8Embellishment("hard");
			var motif = getMotif();
			
			randEmb = Math.ceil(Math.random()*3);
			if (randEmb == 1) { var embellishment = getDF8Embellishment("hard"); } else { var embellishment = getDF8Embellishment("supernatural"); }
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment.n + ". " + embellishment.d;
			if (embellishment.n != "Fine Material" && embellishment.n != "Exceptional Material" && randEmb == 1 && embellishment.n != "Silver Plating" && embellishment.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif + "."; }
			itemPrice = itemPrice * (1 + embellishment.v);
		}
		
		if (randType == 10 || randType == 11) {
			randEmb = Math.ceil(Math.random()*3);
			if (randEmb == 1) { var embellishment = getDF8Embellishment("hard"); } else { var embellishment = getDF8Embellishment("supernatural"); }
			var motif = getMotif();
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment.n + ". " + embellishment.d;
			if (embellishment.n != "Fine Material" && embellishment.n != "Exceptional Material" && randEmb == 1 && embellishment.n != "Silver Plating" && embellishment.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif + "."; }
			
			randEmb2 = Math.ceil(Math.random()*3);
			if (randEmb2 == 1) { var embellishment2 = getDF8Embellishment("hard"); } else { var embellishment2 = getDF8Embellishment("supernatural"); }
			var motif2 = getMotif();
			
			while (embellishment2 == embellishment || (embellishment.n.includes("Made of") && embellishment2.n.includes("Made of"))) {
				randEmb2 = Math.ceil(Math.random()*3);
				if (randEmb2 == 1) { var embellishment2 = getDF8Embellishment("hard"); } else { var embellishment2 = getDF8Embellishment("supernatural"); }
				var motif2 = getMotif();
			}
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment2.n + ". " + embellishment2.d;
			if (embellishment2.n != "Fine Material" && embellishment2.n != "Exceptional Material" && randEmb2 == 1 && embellishment2.n != "Silver Plating" && embellishment2.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif2 + "."; }
			itemPrice = itemPrice * (1 + embellishment.v + embellishment2.v);
			
		}
		
		if (randType == 12) {
			randEmb = Math.ceil(Math.random()*3);
			if (randEmb == 1) { var embellishment = getDF8Embellishment("hard"); } else { var embellishment = getDF8Embellishment("supernatural"); }
			var motif = getMotif();
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment.n + ". " + embellishment.d;
			if (embellishment.n != "Fine Material" && embellishment.n != "Exceptional Material" && randEmb == 1 && embellishment.n != "Silver Plating" && embellishment.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif + "."; }
			
			randEmb2 = Math.ceil(Math.random()*3);
			if (randEmb2 == 1) { var embellishment2 = getDF8Embellishment("hard"); } else { var embellishment2 = getDF8Embellishment("supernatural"); }
			var motif2 = getMotif();
			
			while (embellishment2 == embellishment || (embellishment.n.includes("Made of") && embellishment2.n.includes("Made of"))) {
				randEmb2 = Math.ceil(Math.random()*3);
				if (randEmb2 == 1) { var embellishment2 = getDF8Embellishment("hard"); } else { var embellishment2 = getDF8Embellishment("supernatural"); }
				var motif2 = getMotif();
			}
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment2.n + ". " + embellishment2.d;
			if (embellishment2.n != "Fine Material" && embellishment2.n != "Exceptional Material" && randEmb2 == 1 && embellishment2.n != "Silver Plating" && embellishment2.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif2 + "."; }
			
			
			randEmb3 = Math.ceil(Math.random()*3);
			if (randEmb3 == 1) { var embellishment3 = getDF8Embellishment("hard"); } else { var embellishment3 = getDF8Embellishment("supernatural"); }
			var motif3 = getMotif();
			
			while ((embellishment3 == embellishment2 || embellishment3 == embellishment) || (embellishment.n.includes("Made of") && embellishment3.n.includes("Made of")) || (embellishment2.n.includes("Made of") && embellishment3.n.includes("Made of"))) {
				randEmb3 = Math.ceil(Math.random()*3);
				if (randEmb3 == 1) { var embellishment3 = getDF8Embellishment("hard"); } else { var embellishment3 = getDF8Embellishment("supernatural"); }
				var motif3 = getMotif();
			}
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment3.n + ". " + embellishment3.d;
			if (embellishment3.n != "Fine Material" && embellishment3.n != "Exceptional Material" && randEmb3 == 1 && embellishment3.n != "Silver Plating" && embellishment3.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif3 + "."; }
			
			
			
			itemPrice = itemPrice * (1 + embellishment.v + embellishment2.v + embellishment3.v);
		}
	
		
		var randEnchantChance = Math.ceil(Math.random()*24);
		
		if (randEnchantChance == 24) {
			var enchantment = getWeaponEnchant("Melee");
			itemName = itemName + " of " + enchantment.name;
			itemDesc = itemDesc + "\n\nEnchanted with `" + enchantment.name + "`.";
			if (enchantment.desc != "") {
				itemDesc = itemDesc + "\n" + enchantment.desc;
			}
			if (enchantment.costPerPound) {
				itemPrice = itemPrice + (enchantment.cost*Math.ceil(itemWeight));
			} else {
				itemPrice = itemPrice + enchantment.cost;
			}
		}
		
		

		
		var finalMessage = newMessage + "\nMelee Weapon (Martial Arts): ";
		finalMessage = finalMessage + itemName + " ";
		finalMessage = finalMessage + "\n";
		finalMessage = finalMessage + itemWeight + " lbs. *$" + itemPrice + "*\n";
		
		if (itemDesc != "") {
			finalMessage = finalMessage + itemDesc;
		}
		
		if (!dontPrint) {
			message.channel.send(message.author + ":" + finalMessage);
		}
		
		newArgs = [ itemName, itemPrice, itemWeight, itemTL, 1, pageRef, itemDesc, iBasePrice];
		return newArgs;
	}
	
	if (itemType == "basicMissile") {
		var rand1 = Math.ceil(Math.random()*18);
		
		var randAmmoAmount = (Math.ceil(Math.random()*12))+5;
		
		var itemPrice = 0;
		var itemWeight = 0;
		var itemName = "";
		var itemDesc = "";
		var pageRef = "";
		var itemTL = 4;
		var projectile = false;
		
		var basicMissiles = [
			{ "n":"Atlatl", "p":20, "w":1, "pg":"B276", "tl":0}, { "n":"Blowpipe", "p":30, "w":1, "pg":"B275", "tl":0},
			{ "n":"Bolas", "p":20, "w":2, "pg":"B275", "tl":0}, { "n":"Composite Bow", "p":900, "w":4, "pg":"B275", "tl":1},
			{ "n":"Crossbow", "p":150, "w":6, "pg":"B276", "tl":2}, { "n":"Longbow", "p":200, "w":3, "pg":"B275", "tl":0},
			{ "n":"Pistol Crossbow", "p":150, "w":4, "pg":"B276", "tl":3}, { "n":"Prodd", "p":150, "w":6, "pg":"B276", "tl":3},
			{ "n":"Regular Bow", "p":100, "w":2, "pg":"B275", "tl":0}, { "n":"Staff Sling", "p":20, "w":2, "pg":"B276", "tl":1},
			{ "n":"Short Bow", "p":50, "w":2, "pg":"B275", "tl":0}, { "n":"Shuriken", "p":3, "w":0.1, "pg":"B276", "tl":3},
			{ "n":"Sling", "p":20, "w":0.5, "pg":"B276", "tl":0}, { "n":"Arrows", "p":2, "w":0.1, "pr":true, "pg":"None", "tl":0},
			{ "n":"Atlatl Darts", "p":20, "w":1, "pr":true, "pg":"None", "tl":0},
			{ "n":"Generitek Any-Bullet", "p":0.50, "w":0.05, "pr":true, "pg":"None", "tl":0, "d":"Can be used with any gun that uses bullets for ammunition."},
			{ "n":"Crossbow Bolts", "p":2, "w":0.05, "pr":true, "pg":"None", "tl":0}, { "n":"Blowpipe Darts", "p":0.10, "w":0.05, "pr":true, "pg":"None", "tl":0}
		];
		
		var randWep = basicMissiles[Math.ceil(Math.random() * (basicMissiles.length - 1))];
		
		itemName = randWep.n;
		itemPrice = randWep.p;
		itemWeight = randWep.w;
		itemTL = randWep.tl;
		pageRef = randWep.pg;
		if (randWep.pr != null) { projectile = true; }
		if (randWep.d != null) { itemDesc = randWep.d; }
		
		var newMessage = "";
		
		if (isGachapon) {
			newMessage = "\nYou played the Fantasy Gachapon for 1000 credits and got:";
		}
		
		iBasePrice = itemPrice;
		
		var randType = Math.ceil(Math.random()*12);
		
		if (randType == 7 || randType == 8 || randType == 9) {
			var embellishment = getDF8Embellishment("hard");
			var motif = getMotif();
			
			randEmb = Math.ceil(Math.random()*3);
			if (randEmb == 1) { var embellishment = getDF8Embellishment("hard"); } else { var embellishment = getDF8Embellishment("supernatural"); }
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment.n + ". " + embellishment.d;
			if (embellishment.n != "Fine Material" && embellishment.n != "Exceptional Material" && randEmb == 1 && embellishment.n != "Silver Plating" && embellishment.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif + "."; }
			itemPrice = itemPrice * (1 + embellishment.v);
		}
		
		if (randType == 10 || randType == 11) {
			randEmb = Math.ceil(Math.random()*3);
			if (randEmb == 1) { var embellishment = getDF8Embellishment("hard"); } else { var embellishment = getDF8Embellishment("supernatural"); }
			var motif = getMotif();
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment.n + ". " + embellishment.d;
			if (embellishment.n != "Fine Material" && embellishment.n != "Exceptional Material" && randEmb == 1 && embellishment.n != "Silver Plating" && embellishment.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif + "."; }
			
			randEmb2 = Math.ceil(Math.random()*3);
			if (randEmb2 == 1) { var embellishment2 = getDF8Embellishment("hard"); } else { var embellishment2 = getDF8Embellishment("supernatural"); }
			var motif2 = getMotif();
			
			while (embellishment2 == embellishment || (embellishment.n.includes("Made of") && embellishment2.n.includes("Made of"))) {
				randEmb2 = Math.ceil(Math.random()*3);
				if (randEmb2 == 1) { var embellishment2 = getDF8Embellishment("hard"); } else { var embellishment2 = getDF8Embellishment("supernatural"); }
				var motif2 = getMotif();
			}
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment2.n + ". " + embellishment2.d;
			if (embellishment2.n != "Fine Material" && embellishment2.n != "Exceptional Material" && randEmb2 == 1 && embellishment2.n != "Silver Plating" && embellishment2.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif2 + "."; }
			itemPrice = itemPrice * (1 + embellishment.v + embellishment2.v);
			
		}
		
		if (randType == 12) {
			randEmb = Math.ceil(Math.random()*3);
			if (randEmb == 1) { var embellishment = getDF8Embellishment("hard"); } else { var embellishment = getDF8Embellishment("supernatural"); }
			var motif = getMotif();
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment.n + ". " + embellishment.d;
			if (embellishment.n != "Fine Material" && embellishment.n != "Exceptional Material" && randEmb == 1 && embellishment.n != "Silver Plating" && embellishment.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif + "."; }
			
			randEmb2 = Math.ceil(Math.random()*3);
			if (randEmb2 == 1) { var embellishment2 = getDF8Embellishment("hard"); } else { var embellishment2 = getDF8Embellishment("supernatural"); }
			var motif2 = getMotif();
			
			while (embellishment2 == embellishment || (embellishment.n.includes("Made of") && embellishment2.n.includes("Made of"))) {
				randEmb2 = Math.ceil(Math.random()*3);
				if (randEmb2 == 1) { var embellishment2 = getDF8Embellishment("hard"); } else { var embellishment2 = getDF8Embellishment("supernatural"); }
				var motif2 = getMotif();
			}
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment2.n + ". " + embellishment2.d;
			if (embellishment2.n != "Fine Material" && embellishment2.n != "Exceptional Material" && randEmb2 == 1 && embellishment2.n != "Silver Plating" && embellishment2.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif2 + "."; }
			
			
			randEmb3 = Math.ceil(Math.random()*3);
			if (randEmb3 == 1) { var embellishment3 = getDF8Embellishment("hard"); } else { var embellishment3 = getDF8Embellishment("supernatural"); }
			var motif3 = getMotif();
			
			while ((embellishment3 == embellishment2 || embellishment3 == embellishment) || (embellishment.n.includes("Made of") && embellishment3.n.includes("Made of")) || (embellishment2.n.includes("Made of") && embellishment3.n.includes("Made of"))) {
				randEmb3 = Math.ceil(Math.random()*3);
				if (randEmb3 == 1) { var embellishment3 = getDF8Embellishment("hard"); } else { var embellishment3 = getDF8Embellishment("supernatural"); }
				var motif3 = getMotif();
			}
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment3.n + ". " + embellishment3.d;
			if (embellishment3.n != "Fine Material" && embellishment3.n != "Exceptional Material" && randEmb3 == 1 && embellishment3.n != "Silver Plating" && embellishment3.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif3 + "."; }
			
			
			
			itemPrice = itemPrice * (1 + embellishment.v + embellishment2.v + embellishment3.v);
		}
	
		
		var randEnchantChance = Math.ceil(Math.random()*24);
		
		if (randEnchantChance == 24) {
			if (projectile) { var enchantment = getWeaponEnchant("Projectile"); } else { var enchantment = getWeaponEnchant("Missile"); }
			itemName = itemName + " of " + enchantment.name;
			itemDesc = itemDesc + "\n\nEnchanted with `" + enchantment.name + "`.";
			if (enchantment.desc != "") {
				itemDesc = itemDesc + "\n" + enchantment.desc;
			}
			if (enchantment.costPerPound) {
				itemPrice = itemPrice + (enchantment.cost*Math.ceil(itemWeight));
			} else {
				itemPrice = itemPrice + enchantment.cost;
			}
		}
		
		
		var finalMessage = newMessage + "\nMissile Weapon (Basic Set): ";
		if (projectile) { finalMessage = finalMessage + randAmmoAmount + "x "; }
		finalMessage = finalMessage + itemName + " ";
		finalMessage = finalMessage + "\n";
		finalMessage = finalMessage + itemWeight + " lbs. *$" + itemPrice + "*";
		if (projectile) { finalMessage = finalMessage + " each"; }
		finalMessage = finalMessage + "\n";
		
		if (itemDesc != "") {
			finalMessage = finalMessage + itemDesc;
		}
		
		if (!dontPrint) {
			message.channel.send(message.author + ":" + finalMessage);
		}
		newArgs = [ itemName, itemPrice, itemWeight, itemTL, 1, pageRef, itemDesc, iBasePrice];
		return newArgs;
	}
	
	if (itemType == "martialMissile") {
		var rand1 = Math.ceil(Math.random()*12);

		var itemPrice = 0;
		var itemWeight = 0;
		var itemName = "";
		var itemTL = 0;
		var pageRef = "";
		var itemDesc = "";
		
		var martialMissileWeps = [
			{ "n":"Repeating Crossbow", "p":500, "w":10, "tl":2, "pg":"MA231"}, { "n":"Composite Crossbow", "p":950, "w":7, "tl":3, "pg":"MA231"},
			{ "n":"Large Hungamunga", "p":60, "w":4, "tl":2, "pg":"MA231"}, { "n":"Plumbata", "p":20, "w":1, "tl":2, "pg":"MA231"},
			{ "n":"Discus", "p":40, "w":2, "tl":1, "pg":"MA231"}, { "n":"Chakram", "p":50, "w":1.5, "tl":2, "pg":"MA231"},
			{ "n":"Hungamunga", "p":40, "w":1, "tl":2, "pg":"MA231"}, { "n":"Boomerang", "p":20, "w":1, "tl":0, "pg":"MA231"},
			{ "n":"Throwing Stick", "p":10, "w":1, "tl":0, "pg":"MA231"}
		];
		
		var randomWep = martialMissileWeps[Math.ceil(Math.random() * (martialMissileWeps.length - 1))];
		
		itemName = randomWep.n;
		itemPrice = randomWep.p;
		itemWeight = randomWep.w;
		itemTL = randomWep.tl;
		pageRef = randomWep.pg;

		var newMessage = "";
		
		if (isGachapon) {
			newMessage = "\nYou played the Fantasy Gachapon for 1000 credits and got:";
		}
		
		iBasePrice = itemPrice;
		
		var randType = Math.ceil(Math.random()*12);
		
		if (randType == 7 || randType == 8 || randType == 9) {
			var embellishment = getDF8Embellishment("hard");
			var motif = getMotif();
			
			randEmb = Math.ceil(Math.random()*3);
			if (randEmb == 1) { var embellishment = getDF8Embellishment("hard"); } else { var embellishment = getDF8Embellishment("supernatural"); }
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment.n + ". " + embellishment.d;
			if (embellishment.n != "Fine Material" && embellishment.n != "Exceptional Material" && randEmb == 1 && embellishment.n != "Silver Plating" && embellishment.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif + "."; }
			itemPrice = itemPrice * (1 + embellishment.v);
		}
		
		if (randType == 10 || randType == 11) {
			randEmb = Math.ceil(Math.random()*3);
			if (randEmb == 1) { var embellishment = getDF8Embellishment("hard"); } else { var embellishment = getDF8Embellishment("supernatural"); }
			var motif = getMotif();
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment.n + ". " + embellishment.d;
			if (embellishment.n != "Fine Material" && embellishment.n != "Exceptional Material" && randEmb == 1 && embellishment.n != "Silver Plating" && embellishment.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif + "."; }
			
			randEmb2 = Math.ceil(Math.random()*3);
			if (randEmb2 == 1) { var embellishment2 = getDF8Embellishment("hard"); } else { var embellishment2 = getDF8Embellishment("supernatural"); }
			var motif2 = getMotif();
			
			while (embellishment2 == embellishment || (embellishment.n.includes("Made of") && embellishment2.n.includes("Made of"))) {
				randEmb2 = Math.ceil(Math.random()*3);
				if (randEmb2 == 1) { var embellishment2 = getDF8Embellishment("hard"); } else { var embellishment2 = getDF8Embellishment("supernatural"); }
				var motif2 = getMotif();
			}
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment2.n + ". " + embellishment2.d;
			if (embellishment2.n != "Fine Material" && embellishment2.n != "Exceptional Material" && randEmb2 == 1 && embellishment2.n != "Silver Plating" && embellishment2.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif2 + "."; }
			itemPrice = itemPrice * (1 + embellishment.v + embellishment2.v);
			
		}
		
		if (randType == 12) {
			randEmb = Math.ceil(Math.random()*3);
			if (randEmb == 1) { var embellishment = getDF8Embellishment("hard"); } else { var embellishment = getDF8Embellishment("supernatural"); }
			var motif = getMotif();
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment.n + ". " + embellishment.d;
			if (embellishment.n != "Fine Material" && embellishment.n != "Exceptional Material" && randEmb == 1 && embellishment.n != "Silver Plating" && embellishment.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif + "."; }
			
			randEmb2 = Math.ceil(Math.random()*3);
			if (randEmb2 == 1) { var embellishment2 = getDF8Embellishment("hard"); } else { var embellishment2 = getDF8Embellishment("supernatural"); }
			var motif2 = getMotif();
			
			while (embellishment2 == embellishment || (embellishment.n.includes("Made of") && embellishment2.n.includes("Made of"))) {
				randEmb2 = Math.ceil(Math.random()*3);
				if (randEmb2 == 1) { var embellishment2 = getDF8Embellishment("hard"); } else { var embellishment2 = getDF8Embellishment("supernatural"); }
				var motif2 = getMotif();
			}
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment2.n + ". " + embellishment2.d;
			if (embellishment2.n != "Fine Material" && embellishment2.n != "Exceptional Material" && randEmb2 == 1 && embellishment2.n != "Silver Plating" && embellishment2.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif2 + "."; }
			
			
			randEmb3 = Math.ceil(Math.random()*3);
			if (randEmb3 == 1) { var embellishment3 = getDF8Embellishment("hard"); } else { var embellishment3 = getDF8Embellishment("supernatural"); }
			var motif3 = getMotif();
			
			while ((embellishment3 == embellishment2 || embellishment3 == embellishment) || (embellishment.n.includes("Made of") && embellishment3.n.includes("Made of")) || (embellishment2.n.includes("Made of") && embellishment3.n.includes("Made of"))) {
				randEmb3 = Math.ceil(Math.random()*3);
				if (randEmb3 == 1) { var embellishment3 = getDF8Embellishment("hard"); } else { var embellishment3 = getDF8Embellishment("supernatural"); }
				var motif3 = getMotif();
			}
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment3.n + ". " + embellishment3.d;
			if (embellishment3.n != "Fine Material" && embellishment3.n != "Exceptional Material" && randEmb3 == 1 && embellishment3.n != "Silver Plating" && embellishment3.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif3 + "."; }
			
			
			
			itemPrice = itemPrice * (1 + embellishment.v + embellishment2.v + embellishment3.v);
		}
	
		
		var randEnchantChance = Math.ceil(Math.random()*24);
		
		if (randEnchantChance == 24) {
			var enchantment = getWeaponEnchant("Missile");
			itemName = itemName + " of " + enchantment.name;
			itemDesc = itemDesc + "\n\nEnchanted with `" + enchantment.name + "`.";
			if (enchantment.desc != "") {
				itemDesc = itemDesc + "\n" + enchantment.desc;
			}
			if (enchantment.costPerPound) {
				itemPrice = itemPrice + (enchantment.cost*Math.ceil(itemWeight));
			} else {
				itemPrice = itemPrice + enchantment.cost;
			}
		}
		
		
		var finalMessage = newMessage + "\nMissile Weapon (Martial Arts): ";
		finalMessage = finalMessage + itemName + " ";
		finalMessage = finalMessage + "\n";
		finalMessage = finalMessage + itemWeight + " lbs. *$" + itemPrice + "*";
		finalMessage = finalMessage + "\n";
		
		if (itemDesc != "") {
			finalMessage = finalMessage + itemDesc;
		}
		
		if (!dontPrint) {
			message.channel.send(message.author + ":" + finalMessage);
		}
		newArgs = [ itemName, itemPrice, itemWeight, itemTL, 1, pageRef, itemDesc, iBasePrice];
		return newArgs;
	}
	
	if (itemType == "armor") {
		var rand1 = Math.ceil(Math.random()*71);
		
		var itemPrice = 0;
		var itemWeight = 0;
		var itemName = "";
		var itemDesc = "";
		var projectile = false;
		
		var implausibleChance = Math.ceil(Math.random()*50);
		
		if (forceImplausible == true) { implausibleChance = 50; }
		
		var enchantedChance = Math.ceil(Math.random()*50);
		
		if (implausibleChance == 50) { var implausibleMat = getImplausibleMat(); }
		if (enchantedChance == 50) { var enchantment = getArmorEnchant(); }
		
		if (rand1 == 1) { itemName = "Barrel Helm"; itemPrice = 240; itemWeight = 10; }
		if (rand1 == 2) { itemName = "Boots"; itemPrice = 80; itemWeight = 3; }
		if (rand1 == 3) { itemName = "Bronze Armbands"; itemPrice = 180; itemWeight = 9; }
		if (rand1 == 4) { itemName = "Bronze Breastplate"; itemPrice = 400; itemWeight = 20; }
		if (rand1 == 5) { itemName = "Bronze Corselet"; itemPrice = 1300; itemWeight = 40; }
		if (rand1 == 6) { itemName = "Bronze Greaves"; itemPrice = 270; itemWeight = 17; }
		if (rand1 == 7) { itemName = "Bronze Helmet"; itemPrice = 160; itemWeight = 7.5; }
		if (rand1 == 8) { itemName = "Bronze Pot-Helm"; itemPrice = 60; itemWeight = 5; }
		if (rand1 == 9) { itemName = "Bronze Suit"; itemPrice = 2020; itemWeight = 76.5; itemDesc = "Bronze armbands, corselet, greaves, and helmet, plus boots and leather gloves."; }
		if (rand1 == 10) { itemName = "Bronze Suit"; itemPrice = 2020; itemWeight = 76.5; itemDesc = "Bronze armbands, corselet, greaves, and helmet, plus boots and leather gloves."; }
		if (rand1 == 11) { itemName = "Buff Coat"; itemPrice = 210; itemWeight = 16; }
		if (rand1 == 12) { itemName = "Cloth Armor"; itemPrice = 30; itemWeight = 6; }
		if (rand1 == 13) { itemName = "Cloth Cap"; itemPrice = 5; itemWeight = 0; }
		if (rand1 == 14) { itemName = "Cloth Gloves"; itemPrice = 15; itemWeight = 0; }
		if (rand1 == 15) { itemName = "Cloth Sleeves"; itemPrice = 20; itemWeight = 2; }
		if (rand1 == 16) { itemName = "Cloth Suit"; itemPrice = 150; itemWeight = 13; itemDesc = "Cloth armor, cap, gloves, and sleeves, plus shoes and leather pants.";}
		if (rand1 == 17) { itemName = "Cloth Suit"; itemPrice = 150; itemWeight = 13; itemDesc = "Cloth armor, cap, gloves, and sleeves, plus shoes and leather pants.";}
		if (rand1 == 18) { itemName = "Double Mail Hauberk"; itemPrice = 520; itemWeight = 44; }
		if (rand1 == 19) { itemName = "Face Mask"; itemPrice = 100; itemWeight = 2; }
		if (rand1 == 20) { itemName = "Fur Loincloth"; itemPrice = 10; itemWeight = 0; }
		if (rand1 == 21) { itemName = "Gauntlets"; itemPrice = 100; itemWeight = 2; }
		if (rand1 == 22) { itemName = "Greathelm"; itemPrice = 340; itemWeight = 10; }
		if (rand1 == 23) { itemName = "Heavy Gauntlets"; itemPrice = 250; itemWeight = 2.5; }
		if (rand1 == 24) { itemName = "Heavy Leather Leggings"; itemPrice = 60; itemWeight = 4; }
		if (rand1 == 25) { itemName = "Heavy Leather Sleeves"; itemPrice = 50; itemWeight = 2; }
		if (rand1 == 26) { itemName = "Heavy Plate Arms"; itemPrice = 1500; itemWeight = 20; }
		if (rand1 == 27) { itemName = "Heavy Plate Legs"; itemPrice = 1600; itemWeight = 25; }
		if (rand1 == 28) { itemName = "Heavy Plate Suit"; itemPrice = 6140; itemWeight = 109.5; itemDesc = "Heavy plate corselet, arms, legs, and gauntlets, plus greathelm and sollerets.";}
		if (rand1 == 29) { itemName = "Heavy Plate Suit"; itemPrice = 6140; itemWeight = 109.5; itemDesc = "Heavy plate corselet, arms, legs, and gauntlets, plus greathelm and sollerets.";}
		if (rand1 == 30) { itemName = "Heavy Steel Corselet"; itemPrice = 2300; itemWeight = 45; }
		if (rand1 == 31) { itemName = "Leather Armor"; itemPrice = 100; itemWeight = 10; }
		if (rand1 == 32) { itemName = "Leather Cap"; itemPrice = 10; itemWeight = 0; }
		if (rand1 == 33) { itemName = "Leather Gloves"; itemPrice = 30; itemWeight = 0; }
		if (rand1 == 34) { itemName = "Leather Helm"; itemPrice = 20; itemWeight = 0.5; }
		if (rand1 == 35) { itemName = "Leather Jacket"; itemPrice = 50; itemWeight = 4; }
		if (rand1 == 36) { itemName = "Leather Leggings"; itemPrice = 40; itemWeight = 2; }
		if (rand1 == 37) { itemName = "Leather Pants"; itemPrice = 40; itemWeight = 3; }
		if (rand1 == 38) { itemName = "Leather Suit"; itemPrice = 340; itemWeight = 19.5; itemDesc = "Leather armor, gloves, helm, heavy leather leggings and sleeves, plus boots.";}
		if (rand1 == 39) { itemName = "Leather Suit"; itemPrice = 340; itemWeight = 19.5; itemDesc = "Leather armor, gloves, helm, heavy leather leggings and sleeves, plus boots.";}
		if (rand1 == 40) { itemName = "Leather Suit"; itemPrice = 340; itemWeight = 19.5; itemDesc = "Leather armor, gloves, helm, heavy leather leggings and sleeves, plus boots.";}
		if (rand1 == 41) { itemName = "Legionary Helmet"; itemPrice = 150; itemWeight = 6; }
		if (rand1 == 42) { itemName = "Light Scale Armor"; itemPrice = 150; itemWeight = 15; }
		if (rand1 == 43) { itemName = "Lorica Segmentata"; itemPrice = 680; itemWeight = 26; }
		if (rand1 == 44) { itemName = "Mail Coif"; itemPrice = 55; itemWeight = 4; }
		if (rand1 == 45) { itemName = "Mail Hauberk"; itemPrice = 230; itemWeight = 25; }
		if (rand1 == 46) { itemName = "Mail Leggings"; itemPrice = 110; itemWeight = 15; }
		if (rand1 == 47) { itemName = "Mail Shirt"; itemPrice = 150; itemWeight = 16; }
		if (rand1 == 48) { itemName = "Mail Sleeves"; itemPrice = 70; itemWeight = 9; }
		if (rand1 == 49) { itemName = "Mail Suit"; itemPrice = 645; itemWeight = 58; itemDesc = "Mail coif, hauberk, leggings, and sleeves, plus gauntlets and boots."; }
		if (rand1 == 50) { itemName = "Mail Suit"; itemPrice = 645; itemWeight = 58; itemDesc = "Mail coif, hauberk, leggings, and sleeves, plus gauntlets and boots."; }
		if (rand1 == 51) { itemName = "Mail Suit"; itemPrice = 645; itemWeight = 58; itemDesc = "Mail coif, hauberk, leggings, and sleeves, plus gauntlets and boots."; }
		if (rand1 == 52) { itemName = "Plate Arms"; itemPrice = 1000; itemWeight = 15; }
		if (rand1 == 53) { itemName = "Plate Legs"; itemPrice = 1100; itemWeight = 20; }
		if (rand1 == 54) { itemName = "Plate Suit"; itemPrice = 3890; itemWeight = 89; itemDesc = "Steel corselet, plate arms and legs, sollerets, gauntlets, and barrel helm.";}
		if (rand1 == 55) { itemName = "Plate Suit"; itemPrice = 3890; itemWeight = 89; itemDesc = "Steel corselet, plate arms and legs, sollerets, gauntlets, and barrel helm.";}
		if (rand1 == 56) { itemName = "Pot-Helm"; itemPrice = 100; itemWeight = 5; }
		if (rand1 == 57) { itemName = "Sandals"; itemPrice = 25; itemWeight = 0.5; }
		if (rand1 == 58) { itemName = "Scale Armor"; itemPrice = 420; itemWeight = 35; }
		if (rand1 == 59) { itemName = "Scale Leggings"; itemPrice = 250; itemWeight = 21; }
		if (rand1 == 60) { itemName = "Scale Sleeves"; itemPrice = 210; itemWeight = 14; }
		if (rand1 == 61) { itemName = "Scale Suit"; itemPrice = 1160; itemWeight = 80; itemDesc = "Scale armor, leggings, and sleeves, plus pot-helm, gauntlets, and boots."; }
		if (rand1 == 62) { itemName = "Scale Suit"; itemPrice = 1160; itemWeight = 80; itemDesc = "Scale armor, leggings, and sleeves, plus pot-helm, gauntlets, and boots."; }
		if (rand1 == 63) { itemName = "Scale Suit"; itemPrice = 1160; itemWeight = 80; itemDesc = "Scale armor, leggings, and sleeves, plus pot-helm, gauntlets, and boots."; }
		if (rand1 == 64) { itemName = "Legionary Suit"; itemPrice = 1070; itemWeight = 41; itemDesc = "Lorica segmentata, legionary helmet, and studded leather skirt, plus boots and gauntlets.";}
		if (rand1 == 65) { itemName = "Legionary Suit"; itemPrice = 1070; itemWeight = 41; itemDesc = "Lorica segmentata, legionary helmet, and studded leather skirt, plus boots and gauntlets.";}
		if (rand1 == 66) { itemName = "Shoes"; itemPrice = 40; itemWeight = 2; }
		if (rand1 == 67) { itemName = "Sollerets"; itemPrice = 150; itemWeight = 7; }
		if (rand1 == 68) { itemName = "Steel Breastplate"; itemPrice = 500; itemWeight = 18; }
		if (rand1 == 69) { itemName = "Steel Corselet"; itemPrice = 1300; itemWeight = 35; }
		if (rand1 == 70) { itemName = "Steel Laminate Plate"; itemPrice = 900; itemWeight = 30; }
		if (rand1 == 71) { itemName = "Studded Leather Skirt"; itemPrice = 60; itemWeight = 4; }

		var totalPrice = itemPrice;
		if (implausibleChance == 50) { itemWeight = itemWeight * implausibleMat.weightMod; }

		var newMessage = "";
		
		if (isGachapon) {
			newMessage = "\nYou played the Fantasy Gachapon for 1000 credits and got:";
		}
		
		var finalMessage = newMessage + "\nArmor (Basic Set): ";
		if (implausibleChance == 50) { finalMessage = finalMessage + implausibleMat.name + " "; }
		finalMessage = finalMessage + itemName + " ";
		if (enchantedChance == 50) { finalMessage = finalMessage + "[" + enchantment.name + "] "; }
		finalMessage = finalMessage + "\n";
		finalMessage = finalMessage + itemWeight + " lbs. *$";
		if (implausibleChance == 50) { totalPrice = totalPrice * (1+implausibleMat.valueMod); }
		if (enchantedChance == 50) { 
			if (enchantment.costPerPound) { totalPrice = totalPrice + (itemWeight*enchantment.cost); }
			else if (!enchantment.costPerPound) { totalPrice = totalPrice + enchantment.cost; }
		}
		finalMessage = finalMessage + totalPrice + "*\n";
		
		finalMessage = finalMessage + itemDesc + " ";
		
		if (enchantedChance == 50) {
			finalMessage = finalMessage + "Has a magical enchantement!\n"
			if (enchantment.desc != "") { finalMessage = finalMessage + enchantment.desc + "\n"; }
		}
		
		if (implausibleChance == 50) {
			finalMessage = finalMessage + implausibleMat.desc;
		}
	
		message.channel.send(message.author + ":" + finalMessage);
	}
	
	if (itemType == "concoction") {
		var rand1 = Math.ceil(Math.random()*53);
		var itemPrice = 0;
		var itemWeight = 0;
		var itemName = "";
		var itemDesc = "";

		if (rand1 == 1) { itemName = "Acid"; itemPrice = 10; itemWeight = 1; itemDesc = "See *Dungeon Fantasy 1*, page 28.";}
		if (rand1 == 2) { itemName = "Acid"; itemPrice = 10; itemWeight = 1; itemDesc = "See *Dungeon Fantasy 1*, page 28.";}
		if (rand1 == 3) { itemName = "Alchemist's Fire"; itemPrice = 100; itemWeight = 1; itemDesc = "See *Dungeon Fantasy 1*, page 28.";}
		if (rand1 == 4) { itemName = "Alchemist's Fire"; itemPrice = 100; itemWeight = 1; itemDesc = "See *Dungeon Fantasy 1*, page 28.";}
		if (rand1 == 5) { itemName = "Glow Vial"; itemPrice = 30; itemWeight = 0.5; itemDesc = "See *Dungeon Fantasy 1*, page 28.";}
		if (rand1 == 6) { itemName = "Glow Vial"; itemPrice = 30; itemWeight = 0.5; itemDesc = "See *Dungeon Fantasy 1*, page 28.";}
		if (rand1 == 7) { itemName = "Goblin Nerve Tonic"; itemPrice = 50; itemWeight = 1; itemDesc = "Drinkable. This syrupy liquid, carefully derived from rare saps and berries, allows the user to move a bit faster, but it causes small twitches that interfere with fine motor skills. It provides +0.5 to Basic Speed (improving Move, combat sequence, etc.), gives +1 to initiative rolls, and reduces the penalty for Rapid Strike by one, but makes the user effectively Ham-Fisted. It lasts for one hour and costs 1 FP when it wears off.";}
		if (rand1 == 8) { itemName = "Orcish Energy Bane"; itemPrice = 45; itemWeight = 1; itemDesc = "Drinkable. This vile mix of unsavory ingredients is foul, but it does work. It restores 1d/2 (round up) FP lost to exertion (but not spellcasting or other mystical uses) immediately. However, consumers without Cast-Iron Stomach (p. B80) become nauseated for 1d minutes if they fail a HT roll. Additional doses taken within eight hours cause a loss of 1 FP.";}
		if (rand1 == 9) { itemName = "Jamming Glue"; itemPrice = 60; itemWeight = 1; itemDesc = "Grenade. Adhesives from a variety of creeping creatures are combined with sand, sawdust, and other thickeners to produce a sticky substance that gets into tight crevices and stiffens in place. Anyone hit with a glue grenade must roll against ST to get out any new piece of equipment from a belt or pouch. Adventurers wearing mail or plate armor are at -2 to DX as joints stiffen and seize up. It takes about a half-hour to chip off enough glue to negate its effects.";}
		if (rand1 == 10) { itemName = "Anti-Toxin"; itemPrice = 20; itemWeight = 0.5; itemDesc = "See *Dungeon Fantasy 1*, page 28.";}
		if (rand1 == 11) { itemName = "Black Dust"; itemPrice = 50; itemWeight = 1; itemDesc = "Grenade or utility. Made from fine soot derived from the ashes of certain undead, this dust has a mild and specific resistance to magic. Sprinkled over an invisible object, the dust remains visible, describing the outline of the object. Invisible entities caught in a cloud of black dust (one “grenade” fills a two-yard radius) become faintly visible, making them only -3 to hit. Black Dust washes off easily, so full invisibility can be regained by jumping into water.";}
		if (rand1 == 12) { itemName = "Druidic Fertilizer"; itemPrice = 60; itemWeight = 1; itemDesc = "Utility. A smelly but powerful mixture, this concoction vastly speeds the growth of plants, giving a one-yard area the equivalent of a full year’s growth in a single hour. This can be useful for anything from covering tracks (treated plants almost instantly recover from trampling) to actively blocking a path (vines and shrubs form a thick barrier).";}
		if (rand1 == 13) { itemName = "Foul Pepper"; itemPrice = 50; itemWeight = 1; itemDesc = "Grenade. Not suitable for culinary use, ground foul pepper produces a noxious cloud of dust. A jarful creates a cloud with a two-yard diameter. Any breathing creature in the area of effect must roll against HT twice: once to avoid a sneezing fit lasting 30 seconds, and another to avoid retching for 10 seconds (see p. B428-429).";}
		if (rand1 == 14) { itemName = "Garlic"; itemPrice = 5; itemWeight = 0.25; itemDesc = "See *Dungeon Fantasy 1*, page 28.";}
		if (rand1 == 15) { itemName = "Halfling Joy Powder"; itemPrice = 55; itemWeight = 1; itemDesc = "Grenade. This combination of euphoric substances produces a two-yard-radius cloud that puts its subjects in a remarkably good mood. Targets roll vs. HT; they become tipsy if they succeed or drunk if they fail (see p. B428 for both afflictions). In addition to the usual “grenade” form, it can be purchased in a version where the powder is wrapped in fragile gauze. It must be removed from its carrying jar before it is thrown (takes two Ready maneuvers) but can be thrown silently, rather than shattering noisily like most grenades.";}
		if (rand1 == 16) { itemName = "Luminous Dust"; itemPrice = 20; itemWeight = 0.5; itemDesc = "Utility. Luminous dust is made of a very finely ground mineral that glitters in any light and stays airborne for up to a minute when sprinkled from its packet. It is extremely visible and follows air currents, providing +1 to attempts to find hidden doors. One packet is good for up to a 20’ by 20’ room.";}
		if (rand1 == 17) { itemName = "Wolfsbane"; itemPrice = 5; itemWeight = 0.25; itemDesc = "See *Dungeon Fantasy 1*, page 28.";}
		if (rand1 == 18) { itemName = "Bane"; itemPrice = 50; itemWeight = 0.5; itemDesc = "Utility. Rather than a specific poison, this is actually a series of poisons, each of which targets a specific race or type of monster. Others suffer no ill effects (for example, elves are harmed by elfbane but take no damage from dwarfbane, while the opposite is true of dwarves). If desired, roll on the Race Table (p. 59) or Class Table (p. 59). If the target is of the appropriate type, roll vs. HT. Take 3d additional damage on a failed roll or 1d+1 on a successful roll.";}
		if (rand1 == 19) { itemName = "Bladeblack"; itemPrice = 1000; itemWeight = 0.5; itemDesc = "See *Dungeon Fantasy 1*, page 28.";}
		if (rand1 == 20) { itemName = "Demon's Brew"; itemPrice = 500; itemWeight = 1; itemDesc = "See *Dungeon Fantasy 1*, page 28.";}
		if (rand1 == 21) { itemName = "Fire Toxin"; itemPrice = 250; itemWeight = 0.5; itemDesc = "Utility. This poison does little actual damage but causes lasting pain. Roll vs. HT or suffer from severe pain (see p. B428) for a half-hour. On a critical failure, the poison does a point of damage and pain increases to terrible.";}
		if (rand1 == 22) { itemName = "Knockout Gas"; itemPrice = 300; itemWeight = 1; itemDesc = "Grenade. A jar full of giant-lotus pollen, producing a cloud with a two-yard radius. Anyone inside it must roll against HT to avoid becoming drowsy (see p. B428) for a half-hour (roll vs. Will to remain conscious at the end of the period). On a critical failure, the target falls asleep. Additional doses require rolls at a cumulative -1 each.";}
		if (rand1 == 23) { itemName = "Monster Drool"; itemPrice = 20; itemWeight = 0.5; itemDesc = "See *Dungeon Fantasy 1*, page 29.";}
		if (rand1 == 24) { itemName = "Oozing Doom"; itemPrice = 100; itemWeight = 1; itemDesc = "See *Dungeon Fantasy 1*, page 29.";}
		if (rand1 == 25) { itemName = "Agility Potion"; itemPrice = 700; itemWeight = 0.5; itemDesc = "See *Dungeon Fantasy 1*, page 29.";}
		if (rand1 == 26) { itemName = "Alchemical Antidote"; itemPrice = 400; itemWeight = 0.5; itemDesc = "See *Dungeon Fantasy 1*, page 29.";}
		if (rand1 == 27) { itemName = "Alchemical Glue"; itemPrice = 1000; itemWeight = 1; itemDesc = "Grenade. This potion covers a one-yard area in a very sticky glue. Anyone stepping in it must win a Quick Contest against ST 20 to exit the area. The glue loses its strength after about an hour.";}
		if (rand1 == 28) { itemName = "Alkahest"; itemPrice = 1650; itemWeight = 1; itemDesc = "See *Dungeon Fantasy 1*, page 29.";}
		if (rand1 == 29) { itemName = "Balm of Regeneration"; itemPrice = 900; itemWeight = 0.5; itemDesc = "See *Dungeon Fantasy 1*, page 29.";}
		if (rand1 == 30) { itemName = "Death Potion"; itemPrice = 1000; itemWeight = 1; itemDesc = "See *Dungeon Fantasy 1*, page 29.";}
		if (rand1 == 31) { itemName = "Euphoria Potion"; itemPrice = 750; itemWeight = 1; itemDesc = "Grenade. This potion produces a small cloud of euphoric gas that, if inhaled, can cause the target to “bliss out” under the most trying circumstances. Roll vs. HT or suffer from ecstasy (see p. B428) for five minutes.";}
		if (rand1 == 32) { itemName = "Fire Resistance Potion"; itemPrice = 500; itemWeight = 0.5; itemDesc = "See *Dungeon Fantasy 1*, page 29.";}
		if (rand1 == 33) { itemName = "Flight Potion"; itemPrice = 3000; itemWeight = 0.5; itemDesc = "See *Dungeon Fantasy 1*, page 29.";}
		if (rand1 == 34) { itemName = "Great Healing Potion"; itemPrice = 1000; itemWeight = 0.5; itemDesc = "See *Dungeon Fantasy 1*, page 29.";}
		if (rand1 == 35) { itemName = "Invisibility Potion"; itemPrice = 2250; itemWeight = 0.5; itemDesc = "See *Dungeon Fantasy 1*, page 29.";}
		if (rand1 == 36) { itemName = "Invulnerability Potion"; itemPrice = 2100; itemWeight = 0.5; itemDesc = "See *Dungeon Fantasy 1*, page 29.";}
		if (rand1 == 37) { itemName = "Liquid Ice"; itemPrice = 250; itemWeight = 1; itemDesc = "See *Dungeon Fantasy 1*, page 29.";}
		if (rand1 == 38) { itemName = "Magebane"; itemPrice = 1400; itemWeight = 1; itemDesc = "See *Dungeon Fantasy 1*, page 29."; }
		if (rand1 == 39) { itemName = "Magic Resistance Potion"; itemPrice = 1600; itemWeight = 0.5; itemDesc = "See *Dungeon Fantasy 1*, page 29.";}
		if (rand1 == 40) { itemName = "Major Healing Potion"; itemPrice = 350; itemWeight = 0.5; itemDesc = "See *Dungeon Fantasy 1*, page 29.";}
		if (rand1 == 41) { itemName = "Minor Healing Potion"; itemPrice = 120; itemWeight = 0.5; itemDesc = "See *Dungeon Fantasy 1*, page 29.";}
		if (rand1 == 42) { itemName = "Paut"; itemPrice = 135; itemWeight = 0.5; itemDesc = "See *Dungeon Fantasy 1*, page 29.";}
		if (rand1 == 43) { itemName = "Perception Potion"; itemPrice = 700; itemWeight = 0.5; itemDesc = "See *Dungeon Fantasy 1*, page 29.";}
		if (rand1 == 44) { itemName = "Sleep Potion"; itemPrice = 500; itemWeight = 1; itemDesc = "See *Dungeon Fantasy 1*, page 29.";}
		if (rand1 == 45) { itemName = "Speed Potion"; itemPrice = 550; itemWeight = 0.5; itemDesc = "See *Dungeon Fantasy 1*, page 29.";}
		if (rand1 == 46) { itemName = "Strength Potion"; itemPrice = 250; itemWeight = 0.5; itemDesc = "See *Dungeon Fantasy 1*, page 29.";}
		if (rand1 == 47) { itemName = "Super-Speed Potion"; itemPrice = 2500; itemWeight = 0.5; itemDesc = "Drinkable. Provides the benefits of the Speed potion (Dungeon Fantasy 1, p. 29), as well as a level of Altered Time Rate (p. B38). Two additional doses grant one additional level of Altered Time Rate.";}
		if (rand1 == 48) { itemName = "Thieves' Oil"; itemPrice = 700; itemWeight = 0.5; itemDesc = "See *Dungeon Fantasy 1*, page 29.";}
		if (rand1 == 49) { itemName = "Transparency Potion"; itemPrice = 450; itemWeight = 0.5; itemDesc = "Utility. This potion is readily absorbed into any opaque, nonliving substance, making it temporarily light pervious. One vial of potion renders a cubic foot of matter transparent for an hour. The user may spread it with a brush over a large area to create a broad window through a thin wall or work it into a smaller area to make a small viewing aperture in a thick one.";}
		if (rand1 == 50) { itemName = "True Water"; itemPrice = 750; itemWeight = 0.5; itemDesc = "See *Dungeon Fantasy 1*, page 29.";}
		if (rand1 == 51) { itemName = "Universal Antidote"; itemPrice = 750; itemWeight = 0.5; itemDesc = "See *Dungeon Fantasy 1*, page 29.";}
		if (rand1 == 52) { itemName = "Weakness Potion"; itemPrice = 800; itemWeight = 1; itemDesc = "See *Dungeon Fantasy 1*, page 29.";}
		if (rand1 == 53) { itemName = "Wisdom Potion"; itemPrice = 1500; itemWeight = 0.5; itemDesc = "See *Dungeon Fantasy 1*, page 29.";}
		
		var newMessage = "";
		
		if (isGachapon) {
			newMessage = "\nYou played the Fantasy Gachapon for 1000 credits and got:";
		}
		
		iBasePrice = itemPrice;
		
		var finalMessage = newMessage + "\nConcoctions: ";
		finalMessage = finalMessage + itemName + " ";
		finalMessage = finalMessage + "\n";
		finalMessage = finalMessage + itemWeight + " lbs. *$";
		finalMessage = finalMessage + itemPrice + "*\n";
		finalMessage = finalMessage + itemDesc + " ";
		
		if (!dontPrint) {
			message.channel.send(message.author + ":" + finalMessage);
		}
		
		newArgs = [ itemName, itemPrice, itemWeight, 4, 1, "See Desc", itemDesc, iBasePrice];
		return newArgs;
	}

	if (itemType == "wand" || itemType == "staff") {
		var enchant = getEnchantment();
		var itemBase = Math.ceil(enchant.reserve/6);
		var itemWeight = 0;
		var itemName = enchant.name;
		var itemDesc = "";
		var rand1 = Math.ceil(Math.random()*100);
		var itemPrice = (10*(itemBase*itemBase) +  60*(itemBase));
		
		
		var newMessage = "";
		
		if (isGachapon) {
			newMessage = "\nYou played the Fantasy Gachapon for 1000 credits and got:";
		}
		
		var finalMessage = "";
		
		if (itemType == "wand") {
			var finalMessage = newMessage + " Magic Device:";
			itemDesc = "Casts " + itemName + " using the wielders own FP pool. Costs " + enchant.reserve/6 + " FP.";
			itemPrice = Math.ceil(enchant.cost/6);
			itemWeight = 1;
			
			itemName = "Wand of " + itemName;
		}
		
		if (itemType == "staff") {
			var finalMessage = newMessage + " Magic Device:";
			itemDesc = "Casts " + itemName + " using a built-in energy pool. Has enough energy for " + rand1 + " casts before requiring recharging. ";
			itemPrice = itemPrice * rand1;
			itemWeight = 5;
			
			itemName = "Staff of " + itemName;
		}
		
		finalMessage = finalMessage + "\n";
		
		iBasePrice = itemPrice;
		
		var randType = Math.ceil(Math.random()*12);
		
		if (randType == 7 || randType == 8 || randType == 9) {
			var embellishment = getDF8Embellishment("hard");
			var motif = getMotif();
			
			randEmb = Math.ceil(Math.random()*3);
			if (randEmb == 1) { var embellishment = getDF8Embellishment("hard"); } else { var embellishment = getDF8Embellishment("supernatural"); }
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment.n + ". " + embellishment.d;
			if (embellishment.n != "Fine Material" && embellishment.n != "Exceptional Material" && randEmb == 1 && embellishment.n != "Silver Plating" && embellishment.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif + "."; }
			itemPrice = itemPrice * (1 + embellishment.v);
		}
		
		if (randType == 10 || randType == 11) {
			randEmb = Math.ceil(Math.random()*3);
			if (randEmb == 1) { var embellishment = getDF8Embellishment("hard"); } else { var embellishment = getDF8Embellishment("supernatural"); }
			var motif = getMotif();
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment.n + ". " + embellishment.d;
			if (embellishment.n != "Fine Material" && embellishment.n != "Exceptional Material" && randEmb == 1 && embellishment.n != "Silver Plating" && embellishment.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif + "."; }
			
			randEmb2 = Math.ceil(Math.random()*3);
			if (randEmb2 == 1) { var embellishment2 = getDF8Embellishment("hard"); } else { var embellishment2 = getDF8Embellishment("supernatural"); }
			var motif2 = getMotif();
			
			while (embellishment2 == embellishment || (embellishment.n.includes("Made of") && embellishment2.n.includes("Made of"))) {
				randEmb2 = Math.ceil(Math.random()*3);
				if (randEmb2 == 1) { var embellishment2 = getDF8Embellishment("hard"); } else { var embellishment2 = getDF8Embellishment("supernatural"); }
				var motif2 = getMotif();
			}
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment2.n + ". " + embellishment2.d;
			if (embellishment2.n != "Fine Material" && embellishment2.n != "Exceptional Material" && randEmb2 == 1 && embellishment2.n != "Silver Plating" && embellishment2.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif2 + "."; }
			itemPrice = itemPrice * (1 + embellishment.v + embellishment2.v);
			
		}
		
		if (randType == 12) {
			randEmb = Math.ceil(Math.random()*3);
			if (randEmb == 1) { var embellishment = getDF8Embellishment("hard"); } else { var embellishment = getDF8Embellishment("supernatural"); }
			var motif = getMotif();
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment.n + ". " + embellishment.d;
			if (embellishment.n != "Fine Material" && embellishment.n != "Exceptional Material" && randEmb == 1 && embellishment.n != "Silver Plating" && embellishment.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif + "."; }
			
			randEmb2 = Math.ceil(Math.random()*3);
			if (randEmb2 == 1) { var embellishment2 = getDF8Embellishment("hard"); } else { var embellishment2 = getDF8Embellishment("supernatural"); }
			var motif2 = getMotif();
			
			while (embellishment2 == embellishment || (embellishment.n.includes("Made of") && embellishment2.n.includes("Made of"))) {
				randEmb2 = Math.ceil(Math.random()*3);
				if (randEmb2 == 1) { var embellishment2 = getDF8Embellishment("hard"); } else { var embellishment2 = getDF8Embellishment("supernatural"); }
				var motif2 = getMotif();
			}
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment2.n + ". " + embellishment2.d;
			if (embellishment2.n != "Fine Material" && embellishment2.n != "Exceptional Material" && randEmb2 == 1 && embellishment2.n != "Silver Plating" && embellishment2.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif2 + "."; }
			
			
			randEmb3 = Math.ceil(Math.random()*3);
			if (randEmb3 == 1) { var embellishment3 = getDF8Embellishment("hard"); } else { var embellishment3 = getDF8Embellishment("supernatural"); }
			var motif3 = getMotif();
			
			while ((embellishment3 == embellishment2 || embellishment3 == embellishment) || (embellishment.n.includes("Made of") && embellishment3.n.includes("Made of")) || (embellishment2.n.includes("Made of") && embellishment3.n.includes("Made of"))) {
				randEmb3 = Math.ceil(Math.random()*3);
				if (randEmb3 == 1) { var embellishment3 = getDF8Embellishment("hard"); } else { var embellishment3 = getDF8Embellishment("supernatural"); }
				var motif3 = getMotif();
			}
			
			itemDesc = itemDesc + "\n\nHas an embellishment: " + embellishment3.n + ". " + embellishment3.d;
			if (embellishment3.n != "Fine Material" && embellishment3.n != "Exceptional Material" && randEmb3 == 1 && embellishment3.n != "Silver Plating" && embellishment3.n != "Gilding") { itemDesc = itemDesc + " Embellishment is in the form of a " + motif3 + "."; }
			
			
			
			itemPrice = itemPrice * (1 + embellishment.v + embellishment2.v + embellishment3.v);
		}
	
		finalMessage = finalMessage + itemName + " ";
		finalMessage = finalMessage + "\n";
		finalMessage = finalMessage + itemWeight + " lbs. *$";
		finalMessage = finalMessage + itemPrice + "*\n";
		finalMessage = finalMessage + itemDesc + " ";
		
		if (!dontPrint) {
			message.channel.send(message.author + ":" + finalMessage);
		}
		
		
		newArgs = [ itemName, itemPrice, itemWeight, 4, 1, "GURPS Magic", itemDesc, iBasePrice];
		return newArgs;
	}
	
}
  
function newMTGcard(cardName, message) {
	var loadTest = client.deckTest.get(message.mentions.members.first().id);

	loadTest.owned.push(cardName);
	
	client.deckTest.set(message.mentions.members.first().id, loadTest);
}

function newInventoryItem(newArgs, message, isGacha) {
	var invenArr = [ ];
	var hammerArr = [ ];
	var buildingArr = [ ];
	var tradeArr = [ ];
	var emptyInventory = { "hammerspace":hammerArr, "inventory":invenArr, "externalStorage":buildingArr, "pendingTradeWith":null, "tradeConfirmed":false, "listOfTradeItems":tradeArr, "creditsBeingSent":0 };	
	
	if (!isGacha) {
		if (!client.inventory.has(message.mentions.members.first().id)) {
			client.inventory.set(message.mentions.members.first().id, emptyInventory);
		}
		
		var inventoryTemp = client.inventory.get(message.mentions.members.first().id);
	}
	if (isGacha) {
		if (!client.inventory.has(message.author.id)) {
			client.inventory.set(message.author.id, emptyInventory);
		}
		
		var inventoryTemp = client.inventory.get(message.author.id);
	}
	
	var name = newArgs[0];
	var price = newArgs[1];
	var weight = newArgs[2];
	var itemTL = newArgs[3];
	var quantity = newArgs[4];
	var pageRef = newArgs[5];
	var description = newArgs[6];
	var foundIdentical = false;
	
	var newItem = { "name":name, "itemPrice":price, "itemWeight":weight, "TL":itemTL, "quantity":quantity, "itemDesc":description, "note":"", "pageRef":pageRef, "basePrice":price };
	
	for (i = 0; i < inventoryTemp.hammerspace.length; i++) {
		if (inventoryTemp.hammerspace[i].name == newItem.name && inventoryTemp.hammerspace[i].itemPrice == newItem.itemPrice && inventoryTemp.hammerspace[i].TL == newItem.TL) {
			inventoryTemp.hammerspace[i].quantity = parseInt(inventoryTemp.hammerspace[i].quantity) + parseInt(newItem.quantity);
			foundIdentical = true;
			break;
		}
	}
	
	if (foundIdentical == false) {
		inventoryTemp.hammerspace.push(newItem);
	}
	
	inventoryTemp.hammerspace.sort(compareByName);
	if (!isGacha) {
		client.inventory.set(message.mentions.members.first().id, inventoryTemp);
	}
	
	if (isGacha) {
		client.inventory.set(message.author.id, inventoryTemp);
	}
}	

function getDF8Embellishment(type) {
	var softEmbellishments = [
		{ "n":"Fine Material", "v":2, "d":"The item is made from a high-quality material, such as silk or velvet instead of common cloth, or common fur instead of leather." },
		{ "n":"Fine Material", "v":2, "d":"The item is made from a high-quality material, such as silk or velvet instead of common cloth, or common fur instead of leather." },
		{ "n":"Exceptional Material", "v":19, "d":"As above, but with considerably more valuable material: pashmina-spider silk blend instead of common cloth, cavebear fur instead of leather, etc." },
		{ "n":"Exceptional Material", "v":19, "d":"As above, but with considerably more valuable material: pashmina-spider silk blend instead of common cloth, cavebear fur instead of leather, etc." },
		{ "n":"Dyed, Cheap", "v":1.5, "d":"The item is dyed with a lasting black or brown or a dull red or yellow." },
		{ "n":"Dyed, Average", "v":4, "d":"The item is dyed with a more vivid color (such as red, green, blue, or purple), though the dye may fade with extended wear." },
		{ "n":"Dyed, Expensive", "v":8, "d":"The item is dyed with colors that are both vivid and longlasting." },
		{ "n":"Block Printing", "v":0.5, "d":"A carved block was used to make patterns of ink or paint on the item." },
		{ "n":"Resist Dyed", "v":2.5, "d":"The item is dyed with a technique such a batik or shabon that results in dramatic, complex patterns of contrasting dyed and undyed surfaces." },
		{ "n":"Branding", "v":0.5, "d":"A pattern is burned into the item with a hot iron." },
		{ "n":"Patchwork, Cheap", "v":2, "d":"The item is composed of large patches of different colors or patterns of material sewn together" },
		{ "n":"Patchwork, Expensive", "v":5, "d":"As with cheap patchwork, but the item is composed of many small patches, painstakingly sewn in elaborate patterns." },
		{ "n":"Fringe, Cheap", "v":1, "d":"The item has edging of dull-colored string or strips of hide." },
		{ "n":"Fringe, Cheap", "v":1, "d":"The item has edging of dull-colored string or strips of hide." },
		{ "n":"Fringe, Expensive", "v":6, "d":"The item has edging of elaborately woven and colored tassels." },
		{ "n":"Lace, Minimal", "v":3.5, "d":"The edges of the item are trimmed with a small amount of lace, or the item has one large, prominent lace decoration." },
		{ "n":"Lace, Minimal", "v":3.5, "d":"The edges of the item are trimmed with a small amount of lace, or the item has one large, prominent lace decoration." },
		{ "n":"Lace, Extensive", "v":9, "d":"The item is extensively decorated with lace, possibly entirely covered in it." },
		{ "n":"Feathers, Simple", "v":0.5, "d":"The item is decorated with a few small and probably dullcolored feathers." },
		{ "n":"Feathers, Elaborate", "v":4, "d":"The item is extensively decorated with feathers or has a few large, colorful feathers as a particularly visible accent." },
		{ "n":"Fur Trim, Cheap", "v":3, "d":"The item is strategically trimmed with a comfortable but inexpensive and dull-colored fur, such as rabbit or deer." },
		{ "n":"Fur Trim, Expensive", "v":8, "d":"The item is trimmed with an expensive fur, such as ermine or sable." },
		{ "n":"Beading, Cheap, Minimal", "v":1.5, "d":"Decoration with a few colored beads made from clay, common shells, or colored glass, perhaps along the edges or corners of the item." },
		{ "n":"Beading, Cheap, Minimal", "v":1.5, "d":"Decoration with a few colored beads made from clay, common shells, or colored glass, perhaps along the edges or corners of the item." },
		{ "n":"Beading, Cheap, Extensive", "v":4, "d":"Decorated heavily with colored beads made from clay, common shells, or colored glass, perhaps along the edges or corners of the item." },
		{ "n":"Beading, Expensive, Minimal", "v":3, "d":"Decoration with a few beads made from semi-precious stones such as jet, coral, or turquoise, or attractive shells." },
		{ "n":"Beading, Expensive, Extensive", "v":7, "d":"Decoration heavily with beads made from semi-precious stones such as jet, coral, or turquoise, or attractive shells." },
		{ "n":"Bells, Cheap", "v":3, "d":"The item is decorated with small, tinkling brass bells that jingle as the item is moved. Gives -1 to attempts at Stealth if worn or carried openly." },
		{ "n":"Bells, Expensive", "v":10, "d":"The item is decorated with small, tinkling silver or gold bells that jingle as the item is moved. Gives -1 to attempts at Stealth if worn or carried openly." },
		{ "n":"Embroidery, Minimal", "v":2, "d":"A small pattern of colored threads is sewn into the item." },
		{ "n":"Embroidery, Extensive", "v":5, "d":"A large pattern of colored threads is sewn into the item." },
		{ "n":"Tattooed, Minimal", "v":2, "d":"A small design is inked directly into the material (leather only; for other soft items, treat as minimal embroidery)." },
		{ "n":"Tattooed, Extensive", "v":6, "d":"A design is inked directly into the material, the design covers the entire item (leather only; for other soft items, treat as extensive embroidery)." },
		{ "n":"Tapestry Weaving", "v":6, "d":"Colored threads are woven into the body of a fabric in a figurative or elaborate geometric pattern such as a tartan (cloth only; for leather, treat as minimal tattooing)." },
		{ "n":"Quilting", "v":4, "d":"The item is decorated with a pattern of stitches holding together thin layers of material." },
		{ "n":"Patchwork Quilt", "v":8, "d":"Combines patchwork materials with quilt stitching." }
	];
	
	var hardEmbellishments = [
		{ "n":"Fine Material", "v":2, "d":"The item is made from a high-quality material, such as teak, ebony, or ivory instead of common wood, or deeply colored glass instead of common ceramic. For most metal items, treat as silver plating (DF8 p. 55)." },
		{ "n":"Fine Material", "v":2, "d":"The item is made from a high-quality material, such as teak, ebony, or ivory instead of common wood, or deeply colored glass instead of common ceramic. For most metal items, treat as silver plating (DF8 p. 55)." },
		{ "n":"Exceptional Material", "v":19, "d":"The item is made from a very high-quality material, such as dragonbone instead of wood, or porcelain instead of common ceramic. For most metal items, treat as gilding (DF8 p. 55)." },
		{ "n":"Exceptional Material", "v":19, "d":"The item is made from a very high-quality material, such as dragonbone instead of wood, or porcelain instead of common ceramic. For most metal items, treat as gilding (DF8 p. 55)." },
		{ "n":"Fringe, Cheap", "v":0.25, "d":"The item has an edging of feathers or fur from common animals or dull-colored string, either tied on or sewn on through holes drilled in the item." },
		{ "n":"Fringe, Expensive", "v":0.5, "d":"The item has edging of costly feathers or fur or brightly colored tassels, either tied on or sewn on through holes drilled in the item." },
		{ "n":"Beads/Nails, Minimal", "v":0.75, "d":"A pattern of metal beads driven into or soldered onto a small part of the item." },
		{ "n":"Beads/Nails, Extensive", "v":2, "d":"A pattern of metal beads driven into or soldered onto most of the item." },
		{ "n":"Branding", "v":1, "d":"A pattern was burned into the item (nonmetal items only; for metal items, treat as minimal beads/nails, above)." },
		{ "n":"Painting/Enamel, Minimal", "v":2, "d":"The item has a small design painted on it; for metal items, the paint is a baked-on enamel." },
		{ "n":"Painting/Enamel, Extensive", "v":5, "d":"The item has an extensive design painted on it; for metal items, the paint is a baked-on enamel." },
		{ "n":"Relief, Minimal", "v":1.5, "d":"A small design is carved, etched, or stamped into the item’s surface." },
		{ "n":"Relief, Extensive", "v":4, "d":"A large design is carved, etched, or stamped into most of the item’s surface." },
		{ "n":"Inlay, Cheap, Minimal", "v":2.5, "d":"Like minimal relief, but the carved portions are filled in with differently colored materials, such as dark wood in a pale wooden panel or dark granite in pale marble." },
		{ "n":"Inlay, Cheap, Extensive", "v":7, "d":"Like extensive relief, but the carved portions are filled in with differently colored materials, such as dark wood in a pale wooden panel or dark granite in pale marble." },
		{ "n":"Inlay, Expensive, Minimal", "v":6, "d":"Like minimal relief, but the carved portions are filled in with differently valuable colored materials, such as ebony or smilar, mother of pearl, and tiny semiprecious stones." },
		{ "n":"Inlay, Expensive, Extensive", "v":14, "d":"Like extensive relief, but the carved portions are filled in with differently valuable colored materials, such as ebony or smilar, mother of pearl, and tiny semiprecious stones." },
		{ "n":"Silver Plating", "v":2, "d":"The item is decorated with a thin layer of silver." },
		{ "n":"Gilding", "v":19, "d":"The item is decorated with a thin layer of gold." }
	];
	
	
	var supernaturalEmbellishments = [
		{ "n":"Everyone within 3-yards feels a vague sensation of Coldness", "v":-0.1, "d":"" },
		{ "n":"Everyone within 3-yards feels a vague sensation of Sorrow", "v":-0.1, "d":"" },
		{ "n":"Everyone within 3-yards feels a vague sensation of Joy", "v":0.1, "d":"" },
		{ "n":"Everyone within 3-yards feels a vague sensation of Fear", "v":-0.1, "d":"" },
		{ "n":"Everyone within 3-yards feels a vague sensation of Confusion", "v":-0.1, "d":"" },
		{ "n":"The surface of the item is animated with the image of Rolling Clouds", "v":1, "d":"" },
		{ "n":"The surface of the item is animated with the image of Running Water", "v":1, "d":"" },
		{ "n":"The surface of the item is animated with the image of Moving Stars", "v":1, "d":"" },
		{ "n":"The surface of the item is animated with the image of Shifting Colors/Textures", "v":1, "d":"" },
		{ "n":"The surface of the item is animated with the image of Moving Figures", "v":1, "d":"" },
		{ "n":"Makes the sound of Musical Chiming whenever its magical properties (if any) are used", "v":1, "d":"" },
		{ "n":"Makes the sound of Wordless Singing whenever its magical properties (if any) are used", "v":1, "d":"" },
		{ "n":"Makes the sound of Pained Screaming whenever its magical properties (if any) are used", "v":1, "d":"" },
		{ "n":"Makes the sound of Thunderclap whenever its magical properties (if any) are used", "v":1, "d":"" },
		{ "n":"This object always small faintly of Flowers", "v":1, "d":"" },
		{ "n":"This object always small faintly of Spices", "v":1, "d":"" },
		{ "n":"This object always small faintly of Blood", "v":-0.1, "d":"" },
		{ "n":"This object always small faintly of Decay", "v":-0.25, "d":"" },
		{ "n":"This object always small faintly of Brimstone", "v":-0.1, "d":"" },
		{ "n":"This object always small faintly of Smoke", "v":0.1, "d":"" },
		{ "n":"Sparkling Motes", "v":1, "d":"When moved, the object leaves a faint trail following its path over the past second." },
		{ "n":"Faint Glow", "v":1, "d":"When moved, the object leaves a faint trail following its path over the past second." },
		{ "n":"Pale Mist", "v":1, "d":"When moved, the object leaves a faint trail following its path over the past second." },
		{ "n":"Black Mist", "v":1, "d":"When moved, the object leaves a faint trail following its path over the past second." },
		{ "n":"Faint Glow", "v":2, "d":"Not enough light to see by, but darkness penalties against seeing the item (and its user) are reduced by 1." },
		{ "n":"Glowing Parts/Design", "v":4, "d":"Not enough light to see by, but darkness penalties against seeing the item (and its user) are reduced by 1. Glow is limited to a small part of the item; Illuminated capitals in a book, a gemstone in a ring, or inlaid parts of a sculpture." },
		{ "n":"Spectral", "v":1, "d":"The item has a ghostly, semi-translucent appearance." },
		{ "n":"Spirit Aura", "v":1, "d":"The item has an aura that has a shape suggesting its function or origin. For example, a cloak protected by Fortify might have an outline faintly resembling a suit of armor." },
		{ "n":"Blurry", "v":0.1, "d":"The object always appears slightly out of focus." },
		{ "n":"Altered Reflection", "v":0.25, "d":"The object is shiny enough to reflect images, but they are slightly skewed, discolored, or a fraction of a second behind the action. The item may even “reflect” items that aren’t there." },
		{ "n":"Casts no shadow", "v":0.5, "d":"" },
		{ "n":"Casts a shadow, but always at the wrong angle.", "v":0.1, "d":"" },
		{ "n":"Flashes brightly when its magical properties are used.", "v":1.5, "d":"" },
		{ "n":"Always make a faint humming or ringing noise.", "v":0.5, "d":"" },
		{ "n":"Vibrates faintly when its magical abilities are used.", "v":0.1, "d":"If the item has “always on” enchantments, it vibrates constantly." },
		{ "n":"Always warm to the touch.", "v":0.1, "d":"The item is always at about body temperature." },
		{ "n":"Always cool to the touch.", "v":0.1, "d":"" },
		{ "n":"Always orients itself to a point of the compass.", "v":1, "d":"If dropped, dangled from a rope or string, or left alone for a while, the object shifts itself to point in a specific direction. For example, a sword or arrow points the appropriate direction, or the front of a cabinet or statue turns to face that way." },
		{ "n":"When left alone, the object is always level.", "v":0.5, "d":"If dropped or placed on a sloping surface, the object may tumble to the bottom, but it will always stop in a position where it is level, even if its final position leaves part of the item hovering in mid-air. The slightest pressure will disturb its balance, however." },
		{ "n":"Clean", "v":2, "d":"Dirt, dust, even sticky substances like blood and tar don’t cling to the item, and it can be cleaned off with a quick shake or dusting." },
		{ "n":"Dirty", "v":-0.25, "d":"The item always has a patina of dust or mud no matter how much it is cleaned." },
		{ "n":"Damp", "v":-0.1, "d":"Though it does not provide enough moisture to drink, the item is always slightly wet." },
		{ "n":"Dry", "v":0.25, "d":"The object immediately becomes bone-dry if not immersed in liquid." },
		{ "n":"Bloody", "v":-0.5, "d":"The item slowly drips blood. Tracking rolls to follow the user are at +2." },
		{ "n":"Empathic", "v":1, "d":"The item subtly changes to reflect the user’s emotions. For example, it may growl when the user is angry, or figures on the item may smile when the user is happy. An observer who notices this property is at +1 to any influence skill rolls against the user." },
		
		{ "n":"Made of Basalt", "v":2, "d":"Made of a rough-faced gray stone, often in a pattern of hexagonal cells. Basalt items are very hard, and get +1 to rolls to resist breakage."},
		{ "n":"Made of Blood", "v":3, "d":"Deep red, verging on black in places, with a faint-but-distinct coppery smell. Gives +1 to scent-based Tracking rolls to follow the owner, and -2 to reactions from just about everybody. Conductive."},
		{ "n":"Made of Bone", "v":1, "d":"White, or possibly slightly off-white, and somewhat porous. People using bone items suffer -1 to reactions from everyone but necromancers."},
		{ "n":"Made of Cloud", "v":5, "d":"The item is a soft, swirling, mottled gray, and marginally translucent, very strong light sources are just barely visible through it, at least around the edges. It always feels slightly damp. Garments and armor made out of this material count as “wet clothes” for exposure to cold (p. B430), but give +1 to heat-based HT rolls. Conductive."},
		{ "n":"Made of Mist", "v":5, "d":"The item is a soft, swirling, mottled gray, and marginally translucent, very strong light sources are just barely visible through it, at least around the edges. It always feels slightly damp. Garments and armor made out of this material count as “wet clothes” for exposure to cold (p. B430), but give +1 to heat-based HT rolls. Conductive."},
		{ "n":"Made of Darkness", "v":2, "d":"The item is pure black, reflecting no light whatsoever. Fine surface features like engraving are almost impossible to see if the object is kept clean."},
		{ "n":"Made of Ebony", "v":2, "d":"A fine-grained, dark brown or black wood. Flammable."},
		{ "n":"Made of Flame", "v":4, "d":"Hot to the touch (though not so much so as to be damaging), the item sheds light as a torch. A garment or armor made from flame gives +2 to cold-based HT rolls, -2 to heatbased ones."},
		{ "n":"Made of Flower Petals", "v":2, "d":"In addition to being colorful and sweet-smelling, the object is slightly soft and velvety to the touch. The distinct aroma gives +1 to attempts to track the owner by scent, but someone with prominently displayed flower-petal items gets +1 to reactions in addition to any bonuses granted by ornate equipment. Flammable."},
		{ "n":"Made of Horn", "v":1, "d":"Smooth, if slightly porous, with colors ranging from ivory to a medium brown."},
		{ "n":"Made of Everfrost", "v":4, "d":"White or faintly blue, and cold to the touch (don’t lick the item!). A garment or armor made from ice gives -2 to coldbased HT rolls, +2 to heat-based ones. Conductive."},
		{ "n":"Made of Insects", "v":1, "d":"Made from clusters of interlocked insect exoskeletons, chitin items range from a dull gray-green to a rainbow sheen like an oil slick on water, which is attractive but very creepy (-2 to reactions from most people). Some items are made from stinging insects. Weapons made from such creatures cause an additional -1 in shock penalties when they inflict injury but do no extra damage."},
		{ "n":"Made of Leaves", "v":1, "d":"The object is made from overlapping layers of greenery. Some items may turn colors with the seasons. Items made from leaves are Flammable (p. B433) when brown but Highly Resistant when green."},
		{ "n":"Made of Lightning", "v":4, "d":"Silvery, but with shuddering edges, a faint crackling noise, and a slight scent of ozone. Lightning items cast a flickering glow equivalent to candlelight. However, the jarring flashes make them uncomfortable to use: -1 to long-term tasks such as reading or working with machinery while using such an object as a light source. Conductive."},
		{ "n":"Made of Marble", "v":3, "d":"Mottled stone – often white or in earth tones, but sometimes with a pink or blue tint – with a smooth polish. Marble items are particularly vulnerable to acids, where applicable, halve DR against such attacks."},
		{ "n":"Made of Moonbeams", "v":3, "d":"Through the course of a month, the item goes from being a pale, glowing gray (about the light of a candle) to near-black and back again."},
		{ "n":"Made of Night", "v":2, "d":"As with darkness, but a slowly shifting array of stars makes surface features a bit easier to make out."},
		{ "n":"Made of Quicksilver", "v":16, "d":"The item is mirror-smooth and silvery, but ripples faintly when disturbed. Conductive"},
		{ "n":"Made of Sandstone", "v":2, "d":"Rough-surfaced, with earth-tone colors ranging from white to a deep brick red. Larger items have multiple strata of colors."},
		{ "n":"Made of Screams", "v":1, "d":"Translucent, if slightly cloudy. The item always seems to be vibrating slightly, and can be heard if one puts an ear very close to it."},
		{ "n":"Made of Sea Foam", "v":3, "d":"Translucent, milky pale green or turquoise, with roiling whitish areas. The item smells faintly of the sea and has a salty taste. Like clouds and mist, garments and armor made out of this material count as “wet clothes” for exposure to cold (p. B430), but give +1 to heat-based HT rolls. Conductive."},
		{ "n":"Made of Coarse Shell", "v":-0.1, "d":"Very rough, dull-colored seashell resembling the outside of a clam or an oyster. Such items have a faint odor of the sea, making them slightly easier to track (+1 to scent-based Tracking rolls) away from coastal areas."},
		{ "n":"Made of Fine Shell", "v":4, "d":"A smooth, mother-of-pearl surface, with rainbow colors on a white or slightly silvery ground."},
		{ "n":"Made of Sky", "v":1, "d":"Usually a medium blue, but growing lighter or darker from time to time, sometimes becoming an overcast gray. A few examples change to match their owner’s mood: clear and blue in happy times, dark and cloudy with unhappier moments. While impressive, this makes the owner’s moods easy enough to spot that others get +1 to any social skill used against him."},
		{ "n":"Made of Smoke", "v":1.5, "d":"Resembles clouds, but a bit darker. The item also has a distinct scent of burning and leaves dark smudges on things it touches."},
		{ "n":"Made of Sunlight", "v":5, "d":"The item shines with a bright, golden light equivalent to a torch. It is pleasantly warm to the touch."},
		{ "n":"Made of Tears", "v":1, "d":"Like water, the item is fairly transparent. It is also warm to the touch – and, if one tastes it, salty. Conductive."},
		{ "n":"Made of Teeth", "v":1, "d":"Appears similar to bone and horn, though usually with a glossier surface and a pointed end."},
		{ "n":"Made of Thorns", "v":-0.1, "d":"The item is made from a thick tangle of thorny branches and vines. It feels prickly, though this causes no special damage. Flammable."},
		{ "n":"Made of Water", "v":1, "d":"Transparent and somewhat reflective, a close observer can see ripples through the item if it is struck. Garments and armor made out of this material count as “wet clothes” for exposure to cold (p. B430), but give +1 to heat-based HT rolls."},
		{ "n":"Made of Joy", "v":2, "d":"Mostly translucent, but with occasional ripples of colors that grow more frequent as nearby people get happier. Anyone who gets close enough will find that it smells faintly of their favorite scent."},
		{ "n":"Made of Flesh", "v":2, "d":"Made of a bloody mass of flesh. Gives +1 to scent-based Tracking rolls to follow the owner, and -2 to reactions from just about everybody. "},
		{ "n":"Made of Cardboard", "v":2, "d":"Looks as though it were made of discarded cardboard, while still somehow retaining all the physical properties of the items original material."},
		{ "n":"Made of Lava", "v":4, "d":"Deep red and orange liquid, with occasional bubbles coming to the surface and bursting. Can be held without causing damage, but is almost painfully hot. People sensitive to temperature take 1 damage per round that they are actively in contact with this object."},
		{ "n":"Made of Bubbles", "v":1, "d":"This object is made of soapy bubbles that are constantly popping and reforming randomly."},
		{ "n":"Made of Bravery", "v":1, "d":"Often confused at first glance for being made of stupidity, this object confers a feeling of near-foolhardy invulnerability to the holder. No actual stat bonuses or toughness is included."},
		{ "n":"Made of Love", "v":1, "d":"Translucent with a soft pink glow. +1 on any reaction rolls made by someone you freely give this object to."}
	];
	
	if (type == "soft") {
		var rand1 = Math.ceil(Math.random()*(softEmbellishments.length-1));
		return softEmbellishments[rand1];
	} else if (type == "hard") {
		var rand1 = Math.ceil(Math.random()*(hardEmbellishments.length-1));
		return hardEmbellishments[rand1];
	} else if (type == "supernatural") {
		var rand1 = Math.ceil(Math.random()*(supernaturalEmbellishments.length-1));
		return supernaturalEmbellishments[rand1];
	}
	
	
}

function getMotif() {
	var motifs = [
		"Alligator/Crocodile", "Ant", "Ape", "Bat", "Bear", "Bee", "Bull", "Butterfly", "Cat", "Cobra", "Cow", "Crab", "Crane", "Deer", "Dog", "Dolphin", "Domestic Fowl", "Dove",
		"Dragonfly", "Eagle", "Elephant", "Elk", "Fish", "Fly/Gnat", "Goat", "Hawk", "Horse", "Leopard", "Lion", "Lizard", "Locust", "Mammoth", "Mongoose/Weasel", "Mouse/Rat", "Owl", "Ox",
		"Peacock", "Pig", "Rabbit", "Scarab Beetle", "Scorpion", "Shark", "Sheep", "Snail", "Snake", "Sparrow", "Spider", "Squid/Octopus", "Swan", "Tiger", "Turtle", "Wasp/Hornet", "Whale", "Wolf",
		"Animal-Headed Person", "Demon", "Dragon", "Gargoyle", "Giant", "Goblin-Kin", "Griffin", "Horned/Clawed Human", "Multi-Animal Hybrid", "Multi-Headed Animal", "Phoenix", "Sea Serpent", "Undead", "Unicorn",
		"Baby", "Barbarian", "Butcher", "Carpenter", "Child", "Clown", "Cook", "Dancer", "Druid", "Herdsman", "Hunter", "Knight", "Laborer", "Magician", "Mason", "Monk/Holy Hermit",
		"Mounted Archer", "Musician", "Peasant", "Potter", "Priest", "Ruler", "Sage", "Seamstress/Tailor", "Smith", "Warrior", "Bedchamber", "Castle/Palace", "City", "Cliffs", "Court",
		"Desert", "Fields", "Forest/Jungle", "Kitchen", "Lake/Pond", "Meadow", "Mountains", "Night Sky", "Ocean", "Office/Study/Library", "River", "Ruins", "Tavern/Inn", "Temple",
		"Village", "Waterfall", "Anvil", "Arm/Hand", "Bamboo", "Boat", "Carnivorous Plant", "Carriage", "Chariot", "Chrysanthemum", "Constellation", "Eye", "Farming Tool", "Fir/Pine Tree",
		"Fire", "Fruit Tree", "Garment", "Grain Plants", "Grape Vines", "Hammer", "Hand", "Head", "Helmet", "Lotus", "Mortar and Pestle", "Oak Tree", "Rose", "Rowed Ship", "Sailing Ship",
		"Shell", "Shield", "Spinning Wheel", "Star", "Sun", "Tentacle", "Torture Device", "Wagon", "War Wagon", "Weapon", "Wheel", "Willow Tree", "Alchemical Symbol", "Arabesques",
		"Calligraphy", "Checkerboard", "Concentric Circles", "Concentric Squares/Rectangles", "Concentric Triangles", "Crosshatching", "Floral Pattern", "Girih Pattern (Moroccan Tile)",
		"Greek Key", "Hexagonal Grid", "Square Grid", "Triangular Grid", "Herringbone Pattern", "Interlocked Circles", "Interlocked Spirals", "Interlocked Squares/Rectangles",
		"Interlocked Triangles", "Knotwork", "Pinstripes", "Scale Pattern", "Thick Stripes", "Vines and Leaves", "Zodiacal Signs", "Scene depicting a Ball Game", "Scene depicting a Battle", 
		"Scene depicting a Birth", "Scene depicting Construction", "Scene depicting a Communal Meal", "Scene depicting a Coronation", "Scene depicting Dancing", "Scene depicting an Earthquake",
		"Scene depicting Erotica", "Scene depicting an Exorcism", "Scene depicting a footrace", "Scene depicting a Funeral", "Scene depicting Game-Playing", "Scene depicting Gift-Giving",
		"Scene depicting a Harvest", "Scene depicting Hunting", "Scene depicting a Magical Battle", "Scene depicting a Market", "Scene depicting a Murder", "Scene depicting a Musical Performance",
		"Scene depicting a Nursing Mother", "Scene depicting a Party", "Scene depicting a Peaceful Death", "Scene depicting Plowing/Planting", "Scene depicting a Procession",
		"Scene depicting a Sacrifice", "Scene depicting Sailing", "Scene depicting a Shipwreck", "Scene depicting a Speech/Sermon", "Speech depicting Teaching",
		"Scene depicting Animals being Tended", "Scene depicting Travel", "Scene depicting a Wedding", "Scene depicting Worship", "Scene depicting Wrestling/Boxing"
	];
	
	var rand1 = Math.ceil(Math.random()*(motifs.length-1));
	
	return motifs[rand1];
}

function compareByName(a,b) {
  if (a.name < b.name)
    return -1;
  if (a.name > b.name)
    return 1;
  return 0;
}

function compareByIndex(a,b) {
  if (a.index < b.index)
    return -1;
  if (a.index > b.index)
    return 1;
  return 0;
}

function leaderCompare(a,b) {
  if (parseInt(a.index) < parseInt(b.index))
    return 1;
  if (parseInt(a.index) > parseInt(b.index))
    return -1;
  return 0;
}

function basicCompare(a,b) {
  if (parseInt(a) > parseInt(b))
    return 1;
  if (parseInt(a) < parseInt(b))
    return -1;
  return 0;
}

function arckitPersonTitle() {	
	var personTitles = [
		"Street Gang Member", "Corporate Aristocrat", "Pimp", "Thug", "Prostitute", "Fixer", "Priest", "Business Owner", "Solo or Mercenary", "Hacker",
		"Scientist", "Cop", "Mobster", "Smuggler", "Bounty Hunter", "Syndicate Boss", "Concubine", "Tech Specialist", "Soldier", "Scavenger", "Agent",
		"Doctor", "Drug Dealer", "Celebrity", "Artifical Intelligence", "Artist", "Thief", "Media", "Nomad", "Synthetic", "Driver", "Child", "Broker",
		"Unemployed Person", "Clone", "Programmer", "Designer", "Homeless Person", "People Trafficker", "Revolutionary", "Psychiatrist", "Cyborg",
		"Intelligent Animal", "Courier", "Image Consultant", "Forger", "Ex-Con", "Fanatic/Extremist", "Performer", "Junkie"
	];
	
	var rand1 = Math.ceil(Math.random()*(personTitles.length-1));
	var output = personTitles[rand1];
	
	return output;
}

function arckitJobDesire() {	
	var jobDesire = [
		"wants to", "needs to", "must", "plans to", "forced to"
	];
	
	var rand1 = Math.ceil(Math.random()*(jobDesire.length-1));
	var output = jobDesire[rand1];
	
	return output;
}

function arckitJobActionPerson() {
	var jobAction = [
		"kill", "maim", "deliver to", "protect", "intimidate", "escape", "monitor", "smuggle", "find", "blackmail",
		"steal from", "collect from", "pay", "assist", "modify", "record", "threaten", "kidnap", "own", "defeat",
		"ruin", "control", "save", "submit to", "entrap", "con", "flee with", "employ", "marry", "sell out",
		"extract", "kill", "deliver to", "blackmail", "modify", "escape", "ruin", "steal from", "kidnap",
		"find", "escort", "deliver to", "save", "collect from", "flee", "kill", "sell out", "investigate", "submit to"
	];

	var rand1 = Math.ceil(Math.random()*(jobAction.length-1));
	var output = jobAction[rand1];
	
	if (output == "collect from") { output = "collect " + arckitJobTargetItem() + " from"; }
	if (output == "deliver to") { output = "deliver " + arckitJobTargetItem() + " to"; }
	if (output == "steal from") { output = "steal " + arckitJobTargetItem() + " from"; }
	
	return output;
}
  
function arckitJobActionItem() {
	var jobAction = [
		"destroy", "copy", "deliver", "protect", "sell", "steal", "destroy", "smuggle", "locate", "hide",
		"steal", "collect", "receive", "control", "modify", "locate", "destroy", "ransom", "own", "flee with",
		"spoil", "control", "save", "upload", "protect", "use", "flee with", "sell", "steal", "flee with",
		"locate", "destroy", "copy", "steal", "hack into", "escape with", "destroy", "protect", "locate", "design",
		"buy", "protect", "own", "steal", "hide", "sell", "deliver", "save", "copy", "steal"
	];

	var rand1 = Math.ceil(Math.random()*(jobAction.length-1));
	var output = jobAction[rand1];
	
	return output;
}

function arckitJobTargetItem() {
	var targetItems = [
		"neural processor", "vintage wine", "photograph(s)", "IFF tags", "narcotic", "weapon", "ID card", "jewelry", "software", "security passcard",
		"target's DNA", "cybermodem", "offline digital files", "hard drive", "designer virus", "attache case", "data/vid chip", "holdall of drugs", "vehicle", "keys/key card",
		"computer virus", "cybernetic limb", "synthetic brain", "personality module", "cell phone/agent", "exo-womb", "hardcopy schematic", "military ICE breaker", "nano fabricator", "antique katana",
		"cloned coca leaf", "antidote/medicine", "human eye/thumb", "artificial intelligence", "cybernetic optics", "SimStim recording", "robot", "operating system", "tablet device", "memory chip",
		"server", "holdall of cash", "bioware", "augmented pet", "chemical", "human organ(s)", "patient in cryo vat", "work of art", "drone/remote", "cybernetic implant"
	];
	
	var rand1 = Math.ceil(Math.random()*(targetItems.length-1));
	var output = targetItems[rand1];
	
	return output;
}

function arckitMusicGenres() {
	var output = "";
	var rand1 = Math.ceil(Math.random()*10);
	
	if (rand1 == 1) { output = "Black Ambient"; }
	if (rand1 == 2) { output = "Photonic Wave"; }
	if (rand1 == 3) { output = "Xhosa Trance"; }
	if (rand1 == 4) { output = "Industrial Grind House"; }
	if (rand1 == 5) { output = "Eurozeit Groove"; }
	if (rand1 == 6) { output = "Chip Hop"; }
	if (rand1 == 7) { output = "Glitchcore"; }
	if (rand1 == 8) { output = "Machine-Soul Dub"; }
	if (rand1 == 9) { output = "Toxic Ska"; }
	if (rand1 == 10) { output = "Anthemic NeoPunk"; }
	
	return output;
}	

function arckitBuildingFeatures() {
	var features = [
		"Extreme Security Protocols", "Decrepit and Rundown", "Graffitpocalypse/Street Art Heavy", "Obvious Gang Turf", "Back Room Brothel", "Newly Renovated", "Unusually Busy", "Empty/Quiet", "Inadequate Security", "High Tech Equipment",
		"Abandoned Edifice", "Repurposed as", "Front for Corporate Activity", "Front For Criminal Activity", "Hidden Squatters", "Obscured by Ad Screens", "'Grown' by Nanites", "Incomplete", "Self Sufficient", "Crumbling Cheapcrete",
		"War Zone", "Fire Damage", "Not a Building, but Mobile", "Elaborate Balconies", "Years of Clumsy Modification", "For Sale/To-Let", "Bright Emoji Glyphs and Graphics", "Reinforced for Repair Work", "Extremely Leaky", "Gothic Style",
		"Chic and Minimalist", "Brutalist", "Encased in Plastic Siding", "Labyrinthine", "Skywalks to Other Buildings", "Industrial Style", "Anti-Drone System", "Nano Immune System", "People Queue to Enter", "Well Guarded",
		"AI Guardian", "AR Heavy", "Selective Access", "Buggy Security Net", "Taken Over by Junkies", "Foreign Ghetto", "Sweatshop Conditions", "Utilised Solely for Storage", "Veiling Behind Polymer Sheeting", "All Windows Damaged",
		"External Utilities", "Extensive Solar Grid and Fog Catchers", "Bad Epoxy Repairs", "Unpleasant Micro-Climate", "Infested with Rogue Remotes", "Self Aware", "Cellular Black Hole", "Mainly Populated by Hoarders", "Accumulated Trash Heaps", "Unusual Smell",
		"Very Few Working Lights", "A Riot of Satellite Dishes", "Very Noisy Utilities", "Catastrophically Polluted", "Unfinished Extension or Empty Levels", "Permadamp", "Legacy Infrastructure", "Bad Wiring; High EMF; Black Outs", "Patrolled by Hired Rent-A-Cops", "Aggressively Enforced No-Parking Zone",
		"Windows Boarded with Opaque Acrylic", "Main Entrance Out of Order", "Several Trashed Cars Out Front", "Being Audited by Fanatical City Inspectors", "Wind Tunnel", "Tinted Glass and Carbon Nano-Tubes", "Homeless Magnet", "Shakes when Trucks Pass", "Completely Automated", "Exotic/Foreign Architecture",
		"Obvious Corporate Sponsor", "Endorsed by Celebutant", "Promotes Religion", "Independent Nation State", "Family Owned", "Scheduled for Demolition", "Rat or Roach Problem", "Security Camera Overkill", "A Crime Scene", "Target of Net Terrorism",
		"Malfunctioning Incessant Alarm", "Slowly Collapsing or Subsiding", "Under Surveillance", "Enforced No Fly Zone", "Prone to Flooding", "Ecologically Sound; No Carbon Footprint", "Popular with Particular Subculture", "Totally Sterile", "Extensive Drone Docks", "Causes Sickness"
	];
	
	var rand1 = Math.ceil(Math.random()*(features.length-1));
	
	var feature = features[rand1];
	
	if (feature == "Repurposed as") { feature = feature + " " + arckitDowntownBuilding(); }
	if (feature == "AR Heavy") { feature = feature + " (" + arckitAR() + ")"; }
	if (feature == "Buggy Security Net") { }
	if (feature == "Unusual Smell") { feature = feature + " (" + arckitSmells() + ")"; }
	if (feature == "Legacy Infrastructure") { }
	if (feature == "Patrolled By Hired Rent-A-Cops") { }
	if (feature == "Causes Sickness") { }
	if (feature == "Obscured by Ad Screens") { feature = feature + " (" + arckitAd() + ")"; }
	if (feature == "Front for Corporate Activity") { }
	
	return feature;
}

function arckitBuildingInterior() {
	var styles = [ "Minimalist", "Industrial", "Shabby-Chic", "(Bio)Organic", "Brushed Steel", "Polymer Baroque", "Gothic", "Rustic", "Office Beige", "Hexagonal Tiles" ];
	var states = [ "Untidy", "Pristine", "Sterile", "Cramped", "Spacious", "Cavernous", "Biohazard", "Organized", "Cluttered", "Feng Shui" ];
	var unusuals = [ "Hydroponics", "Scavenged Furniture", "Holograms", "Monochromatic", "Artificial Plants", "Strange Acoustics", "Weird Smell", "Remote Assistants", "Legacy Tech", "Exotic Pet" ];
	var secrets = [ "Cameras", "Microphone", "Privacy Screen", "Separate LAN", "Hidden Room", "Escape Route", "Custodian AI", "Weapon Sensor", "Weapons Cache", "Hidden Stash" ];
	
	var rand1 = Math.ceil(Math.random()*(styles.length-1));
	var rand2 = Math.ceil(Math.random()*(states.length-1));
	var rand3 = Math.ceil(Math.random()*(unusuals.length-1));
	var rand4 = Math.ceil(Math.random()*(secrets.length-1));
	
	var interior = { "style":styles[rand1], "state":states[rand2], "unusual":unusuals[rand3], "secret":secrets[rand4] };
	
	return interior;
}
  
function arckitVidShow() {
	
}

function arckitAR() {
	var ARs = [ 
		{ "t":"Direction", "a":"Typographic Overlays", "s":"Crisp/Bright" }, { "t":"Logo Storm", "a":"Bright and Garish Glyphs", "s":"Animated 3D" },
		{ "t":"Assistant", "a":"Animated Persona", "s":"High Resolution" }, { "t":"Blipvert", "a":arckitAd(), "s":"-" },
		{ "t":"Political", "a":"Emotive Imagery", "s":"Sophisticated" }, { "t":"Promotional", "a":"Rapid Motion/Distracting", "s":"Cheap/Low Resolution" },
		{ "t":"Advice", "a":"Reassuring Infomemes", "s":"Clear, Yet Subtle" }, { "t":"Menu System", "a":"Unobtrusive and Integrated", "s":"Technical/Sci-fi" },
		{ "t":"Filter", "a":"Fantastical and Otherworldly", "s":"Surreal/Immersive/Arty" }, { "t":"Prohibition", "a":"Bold/Dramatic/Authorative", "s":"Flat Vector Graphics" }	
	];
	
	var rand1 = Math.ceil(Math.random()*(ARs.length-1));
	AR = ARs[rand1];
	
	output = AR.t + " AR, " + AR.a + " with a " + AR.t + " style.";
	
	return output;
}

function arckitSmells() {
	var smells = [
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
	];
	
	var rand1 = Math.ceil(Math.random()*(smells.length-1));
	
	return smells[rand1];
}

function arckitSounds() {
	var sounds = [
		"Building Alarm", "Barking Dogs", "Incoherent Shouting", "A Single Gunshot", "Ventilation System", "Car Doors Slamming", "Loud Television", "Children", "Aerocar Overhead", "Catchy Corporate Jingle",
		"The Screech of Tires", "Pedestrian Crosswalk", "Rattle Cans in Use", "Heavy Gunfire", "Sobbing", "Channeled Winds", "Hum of a Cleaning Bot", "Buzzing of Flies", "Rattle of Chains", "Arcade Machines",
		"Breaking Glass", "Text Alert", "An RPG Launching", "Sirens Inbound", "Running", "Subway Rumble", "A Loud Argument", "Drill or Power Saw", "Road Traffic Accident", "Hellfire Street Preacher",
		"Loud Energetic Music", "Laughter", "Hydraulics", "A Reversing Vehicle", "An Annoying Ringtone", "Sexual Activity", "Police Radio Chatter", "Loose Door or Gate", "Group Chanting", "Popular Theme Tune",
		"Chesty Coughing", "Vehicle Alarm", "Steady Footsteps", "Automated Warning", "Beating of Heavy Rain", "Garbled Loudspeaker", "Exchange of Gunfire", "Background 'Muzak'", "Gutteral Screaming", "A Call to Prayer",
		"Loose Piping", "Helicopter Overhead", "The Buzz of Neon", "A Revving Engine", "An Impromptu Rave", "Water Pump", "Road Works", "Right Wing Talk Radio", "Foreign Busker", "Ice Cream Truck",
		"Distant Explosion", "Car Horn", "UAV/Drone Motor", "Tumbling Trash Cans", "Evangelical Broadcast", "Very Heavy Transport", "Running Water", "Drunk Singing", "Skateboards", "Tattoo Gun",
		"Rolling Cans", "Obvious Porno", "Riot or Demo", "Overhead Train", "Cats Fighting", "Shrill Whistling", "Bug Zapper", "Street Hawker", "Car Chase", "Chilling War Cry",
		"Motorcycles", "Crackle of Flames", "Door Buzzer", "An Anguished Cry", "Sneaker Squeak", "Scraping Metal", "Splashing", "Noisy Printer", "Laser Fire", "Jingle of Keys",
		"Quiet Conversation", "Sirens Outbound", "Aerocar Landing", "Frying Food", "A Brawl", "Doors Slamming", "Ads", "Thunder", "Hissing White Noise"
	];
	
	var rand1 = Math.ceil(Math.random()*(sounds.length-1));
	
	sound = sounds[rand1];
	
	if (sound == "Ads") { sound = "Ads (" + arckitAd() + ")"; }
	
	return sound;
}

function arckitAd() {
	var brands = [ "NoLogo", "Matsushira", "Al Emaar", "Horizon", "De Santo", "Ellis-Itami", "Osprey", "Numan-Lloyd", "Modus", "Cortex" ];
	var ranges = [ "Phoenix", "Omni", "AEX", "Luxuria", "Solaris", "Rapide", "Hydra", "Eco+", "Nexus", "Platina" ];
	var productLines = [ "Cyber/Bio/Nanoware", "Car/Motorcycle/AV", "Foodstuff/Drink", "Clothing/Lifestyle", "Personal Service", "Computing/Comms", "Personal Defense", "Pharmaceuticals", "Cosmetics/Scent", "Travel/Vacation" ];
	var marketingStyles = ["Loud Hard Sell", "Cartoony/Manga", "Sexualized", "Sophisticated", "Abstract", "Humorous", "Gender Specific", "Cheap", "Exotic/Arty", "Aspirational" ];
	
	var rand1 = Math.ceil(Math.random()*(brands.length-1));
	var rand2 = Math.ceil(Math.random()*(ranges.length-1));
	var rand3 = Math.ceil(Math.random()*(productLines.length-1));
	var rand4 = Math.ceil(Math.random()*(marketingStyles.length-1));
	
	var outputAd = brands[rand1] + " " + ranges[rand2] + "-Class " + productLines[rand3] + ". Advertising style is: " + marketingStyles[rand4];
	
	return outputAd;
}

function arckitDowntownBuilding() {
	var output = "";
	
	var buildings = [
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
	];
	
	var rand = Math.ceil(Math.random()*(buildings.length-1));
	
	output = buildings[rand];
	
	return output;
}

function arckitInstaCitizen() {
	var output = "";
	
	var citizens = [
		"Lounge or Bar Manager", "Lounge or Bar Manager", "Bouncer or Private Security", "Bouncer or Private Security", "Aero Pilot", "Researcher or Data Analyst", "Street Gang Thug", "Street Gang Thug", "Lab Worker",
		"Street Kid(s)", "Street Kid(s)", "Gambler or Con Artist", "Gambler or Con Artist", "Uber or Limo Driver", "Fixer", "Fixer", "Fixer", "Designer", "Designer",
		"Pimp", "Corporate Worker", "Corporate Worker", "Corporate Worker", "Sex Worker or Puppet", "Sex Worker or Puppet", "Celeb", "City Beat Cop", "City Beat Cop", "Company Exec",
		"Drone Wrangler or Remote Operator", "Drone Wrangler or Remote Operator", "Drone Wrangler or Remote Operator", "Drone Wrangler or Remote Operator", "Service Staff or Store Worker", "Service Staff or Store Worker", "Freelance Media", "Freelance Media", "Thief or Fence", "Thief or Fence",
		"Hacker", "Hacker", "Hacker", "Military Veteran", "Military Veteran", "Syndicate Footsoldier", "Syndicate Footsoldier", "Smuggler", "Smuggler", "Bounty Hunter",
		"Techie", "Techie", "Unemployed (Increased Leisure Citizen)", "Unemployed (Increased Leisure Citizen)", "Unemployed (Increased Leisure Citizen)", "Unemployed (Increased Leisure Citizen)", "Unemployed (Increased Leisure Citizen)", "Corporate Soldier", "Corporate Soldier", "Corporate Soldier", 
		"Bioware Tech", "Factory Worker", "Factory Worker", "Factory Worker", "Scavenger", "Scavenger", "Corporate Agent", "Corporate Agent", "Doctor or Street Medic", "Doctor or Street Medic",
		"Detective", "Detective", "Detective", "Online Business Person", "Online Business Person", "Store Manager", "Store Manager", "Solo or Mercenary", "Solo or Mercenary", "Killer or Psycho",
		"Transport Worker", "Transport Worker", "Aristocrat or Independently Wealthy", "Aristocrat or Independently Wealthy", "Refuse or Recyc Worker", "Psych Evaluator", "Emergency Worker (EMT, Nurse, Paramedic, Fire Fighter)", "Emergency Worker (EMT, Nurse, Paramedic, Fire Fighter)", "Government Agent", "Scientist"
	];
	
	var rand = Math.ceil(Math.random()*(citizens.length-1));
	
	output = citizens[rand];
	
	return output;
}

function arckitVendomat() {
	var output = { "category":"", "inventory":"" }
	var rand = Math.ceil(Math.random()*10);
	
	if (rand == 1) { output.category = "Hot/Cold Beverages"; output.inventory = "Coffee, tea, cocoa, sodas, water, juice, shakes, energy drinks."; }
	if (rand == 2) { output.category = "Hot/Cold Food"; output.inventory = "Burritos, noodles, burgers, fries, sandwiches, 'sushi'."; }
	if (rand == 3) { output.category = "Stimulants"; output.inventory = "Cigarettes, cigars, e-cig fluid, patches, caffeine tabs, alcohol."; }
	if (rand == 4) { output.category = "Travel Packs"; output.inventory = "Toiletries, travel guides, cosmetics, sanitaries, sunglasses."; }
	if (rand == 5) { output.category = "Electronics"; output.inventory = "Headphones, batteries, phones, flashlights, cameras, radios."; }
	if (rand == 6) { output.category = "Personal Defense"; output.inventory = "Tasers, pepper spray, telescoping batons, polymer oneshots."; }
	if (rand == 7) { output.category = "Clothing"; output.inventory = "Rainwear, underclothes, thermals, hats, t-shirts, socks."; }
	if (rand == 8) { output.category = "Media"; output.inventory = "Newsprint, digital music, games, films, simstim, e-books."; }
	if (rand == 9) { output.category = "Pharmaceuticals"; output.inventory = "Medicines, contraceptives, first aid, sensory aids, vitamins."; }
	if (rand == 10) { output.category = "Snacks"; output.inventory = "Gum, candy, chips/crisps, popcorn, nuts, cryo-dried fruits."; }
	
	return output;
}

function arckitStreetGang() {
	var output = { "firstname":"", "secondname":"", "desc1":"", "desc2":"", "desc3":"", "longdesc":"", "connection":"" };
	
	for (i = 0; i < 7; i++) {
		var rand = Math.ceil(Math.random()*10);
		var tO = { "firstname":"", "secondname":"", "desc1":"", "desc2":"", "desc3":"", "longdesc":"", "connection":"" };
	
		if (rand == 1) { tO.firstname = "Cortical"; tO.secondname = "Dogs"; tO.desc1 = "Neo-Primitive"; tO.desc2 = "Wirehead"; tO.desc3 = "Combat Gang"; tO.longdesc = "Deal in prohibited neural boosts, intellect stims, sakawa charms"; tO.connection = "in a turf war with"; }
		if (rand == 2) { tO.firstname = "Subway"; tO.secondname = "Assassins"; tO.desc1 = "Soft-armored"; tO.desc2 = "Nerve Boosted"; tO.desc3 = "Skaters"; tO.longdesc = "Operate a stash house in the old Metro system and fence goods"; tO.connection = "stealing from"; }
		if (rand == 3) { tO.firstname = "Radical"; tO.secondname = "Dragons"; tO.desc1 = "VR Game Playing"; tO.desc2 = "Frame Assisted"; tO.desc3 = "Martial Artists"; tO.longdesc = "Import, crack and distribute Vr games and Hong Kong action sims"; tO.connection = "merging with"; }
		if (rand == 4) { tO.firstname = "Binary"; tO.secondname = "Prophets"; tO.desc1 = "Neon-Punk"; tO.desc2 = "Scavenger"; tO.desc3 = "Tech-Jackers"; tO.longdesc = "Hack and tap electronic infrastructure, for a price"; tO.connection = "in a rivalry with"; }
		if (rand == 5) { tO.firstname = "Fragile"; tO.secondname = "Machine"; tO.desc1 = "Drone Utilising"; tO.desc2 = "Info-Socialist"; tO.desc3 = "Party Artists"; tO.longdesc = "Will put on one hell of a party in exchange for hot information"; tO.connection = "being employed by"; }
		if (rand == 6) { tO.firstname = "Hate"; tO.secondname = "Society"; tO.desc1 = "Patriarchal"; tO.desc2 = "Sports Fan"; tO.desc3 = "Street Fighters"; tO.longdesc = "Trade in counterfeit sportswear, tickets and fan merchandise"; tO.connection = "preying on"; }
		if (rand == 7) { tO.firstname = "Electric"; tO.secondname = "Freaks"; tO.desc1 = "LAN-Linked"; tO.desc2 = "Sex Predator"; tO.desc3 = "Wireheads"; tO.longdesc = "Supply banned pornographic SimStims and vid-chips"; tO.connection = "despising"; }
		if (rand == 8) { tO.firstname = "Mobile"; tO.secondname = "Girls"; tO.desc1 = "Augment Heavy"; tO.desc2 = "All Female"; tO.desc3 = "Go-Gang"; tO.longdesc = "Sell Chiba-grade imported black market implants and upgrades"; tO.connection = "trading with"; }
		if (rand == 9) { tO.firstname = "Endgame"; tO.secondname = "Militia"; tO.desc1 = "Spike Covered"; tO.desc2 = "Doom Cult"; tO.desc3 = "Boosters"; tO.longdesc = "Prolific dumpster-divers and parts-hunters; trade and exchange"; tO.connection = "actively targeting"; }
		if (rand == 10) { tO.firstname = "Bubblegum"; tO.secondname = "Atrocity"; tO.desc1 = "Ex-Convict"; tO.desc2 = "Transgender"; tO.desc3 = "Combat Gang"; tO.longdesc = "Premier suppliers of street level gender reassignment technology"; tO.connection = "being protected by"; }
		if (rand == 11) { tO.firstname = "Biological"; tO.secondname = "Impulse"; tO.desc1 = "Biomodified"; tO.desc2 = "Brain Damaged"; tO.desc3 = "Boosters"; tO.longdesc = "Leader hires out her most neurologically damaged boys as muscle"; tO.connection = "jealous of"; }
		if (rand == 12) { tO.firstname = "Shaolin"; tO.secondname = "Rippers"; tO.desc1 = "Blade Wielding"; tO.desc2 = "Skill Chipping"; tO.desc3 = "Martial Artists"; tO.longdesc = "Currently making a killing in the underground fighting scene"; tO.connection = "in a turf war with"; }
		if (rand == 13) { tO.firstname = "Chrome"; tO.secondname = "Savages"; tO.desc1 = "Body Modified"; tO.desc2 = "Scrap Tech"; tO.desc3 = "Chromers"; tO.longdesc = "Bargain basement prices for spectacular acts of arson"; tO.connection = "betrayed by"; }
		if (rand == 14) { tO.firstname = "Polymer"; tO.secondname = "Riot"; tO.desc1 = "Gun Fetishist"; tO.desc2 = "Dog-Faced"; tO.desc3 = "Boosters"; tO.longdesc = "Specialists in smash and grab jobs, for money and firearms"; tO.connection = "wary of"; }
		if (rand == 15) { tO.firstname = "Rudeboy"; tO.secondname = "Apocalypse"; tO.desc1 = "Rastafarian"; tO.desc2 = "Racist"; tO.desc3 = "Combat Gang"; tO.longdesc = "Own and operate a warehouse nightclub in the factory district"; tO.connection = "being monitored by"; }
		if (rand == 16) { tO.firstname = "Terminal"; tO.secondname = "Storm"; tO.desc1 = "SimStim Rigged"; tO.desc2 = "Media Savvy"; tO.desc3 = "Chromers"; tO.longdesc = "Digital surveillance and stalking, seamless manipulation of video"; tO.connection = "employing"; }
		if (rand == 17) { tO.firstname = "Shadow"; tO.secondname = "Soldiers"; tO.desc1 = "Tech-Ninja"; tO.desc2 = "Parkour"; tO.desc3 = "Combat Gang"; tO.longdesc = "Happy to be hired as rooftop street guides and expert sneaks"; tO.connection = "trading with"; }
		if (rand == 18) { tO.firstname = "Subsonic"; tO.secondname = "Cult"; tO.desc1 = "Mood Chipping"; tO.desc2 = "Alcoholic"; tO.desc3 = "Party Artists"; tO.longdesc = "Doctor common drugs, altering their composition and legality"; tO.connection = "borrowing money from"; }
		if (rand == 19) { tO.firstname = "Chemical"; tO.secondname = "Loas"; tO.desc1 = "Voodoo Practicing"; tO.desc2 = "Drug Cooking"; tO.desc3 = "Cultists"; tO.longdesc = "Take protection money, cook drugs and provide bespoke curses"; tO.connection = "stealing from"; }
		if (rand == 20) { tO.firstname = "Chosen"; tO.secondname = "Kidz"; tO.desc1 = "Chip-Hop Fan"; tO.desc2 = "Political"; tO.desc3 = "Combat Gang"; tO.longdesc = "Work protection on a number of chip-hop bars and clubs"; tO.connection = "being supplied by"; }
		if (rand == 21) { tO.firstname = "Toxic"; tO.secondname = "Ghosts"; tO.desc1 = "Grime-Punk"; tO.desc2 = "Tech-Junkie"; tO.desc3 = "Boosters"; tO.longdesc = "3D-print their own implanted weapons (fragile)"; tO.connection = "in a turf war with"; }
		if (rand == 22) { tO.firstname = "Steel"; tO.secondname = "Church"; tO.desc1 = "Death Metal Fan"; tO.desc2 = "Evangelical"; tO.desc3 = "Chromers"; tO.longdesc = "Several members are skilled AR coders"; tO.connection = "allied to"; }
		if (rand == 23) { tO.firstname = "Waffen"; tO.secondname = "Technicals"; tO.desc1 = "Skinhead"; tO.desc2 = "Übermensch"; tO.desc3 = "Moto-Cultists"; tO.longdesc = "Make their money dealing in heavy weapons and stolen cars"; tO.connection = "despising"; }
		if (rand == 24) { tO.firstname = "Cannibal"; tO.secondname = "Clowns"; tO.desc1 = "Blood Stained"; tO.desc2 = "Juggalo"; tO.desc3 = "Pranksters"; tO.longdesc = "Grow synthetic opium in hydroponic blisters and sell it on"; tO.connection = "being targeted by"; }
		if (rand == 25) { tO.firstname = "Panzer"; tO.secondname = "Fists"; tO.desc1 = "Armor Plated"; tO.desc2 = "Cyborg"; tO.desc3 = "Street Fighters"; tO.longdesc = "Target small-fry fixers and gangers and steal their shit to sell on"; tO.connection = "protecting"; }
		if (rand == 26) { tO.firstname = "Gun"; tO.secondname = "Sharks"; tO.desc1 = "Couture Stealing"; tO.desc2 = "Psychotic"; tO.desc3 = "Street Kids"; tO.longdesc = "Sell stolen fashions, counterfeits and cloned credit cards"; tO.connection = "being terrified of"; }
		if (rand == 27) { tO.firstname = "Disposable"; tO.secondname = "Boys"; tO.desc1 = "Military Surplus"; tO.desc2 = "War Veteran"; tO.desc3 = "Nihilists"; tO.longdesc = "Professional-level hits and heists, unfettered by rules of warfare"; tO.connection = "being amused by"; }
		if (rand == 28) { tO.firstname = "Iron"; tO.secondname = "Maniacs"; tO.desc1 = "WoW Inspired"; tO.desc2 = "Armor Clad"; tO.desc3 = "Chromers"; tO.longdesc = "Promise them in-game bitcoins and they’ll perform any ‘quest’"; tO.connection = "being humiliated by"; }
		if (rand == 29) { tO.firstname = "Speedball"; tO.secondname = "Ultimates"; tO.desc1 = "Risk Taking"; tO.desc2 = "Rich Kid"; tO.desc3 = "Skaters"; tO.longdesc = "Fat allowances and legal back-up allow extreme road race stunts"; tO.connection = "being threatened by"; }
		if (rand == 30) { tO.firstname = "AK"; tO.secondname = "Legends"; tO.desc1 = "African-Tribal"; tO.desc2 = "Drug Enhanced"; tO.desc3 = "Guardians"; tO.longdesc = "Smuggle russian military hardware and protect ‘their’ hab stacks"; tO.connection = "in a turf war with"; }
		if (rand == 31) { tO.firstname = "Liquid"; tO.secondname = "Killaz"; tO.desc1 = "Ultraviolent"; tO.desc2 = "Home Invading"; tO.desc3 = "Boosters"; tO.longdesc = "Slave traffickers and suppliers of stolen consumer electronics"; tO.connection = "working with"; }
		if (rand == 32) { tO.firstname = "Junky"; tO.secondname = "Bullets"; tO.desc1 = "Drug Dependent"; tO.desc2 = "Cannibal"; tO.desc3 = "Street Kids"; tO.longdesc = "Suspected organ-leggers and kidnappers"; tO.connection = "despising"; }
		if (rand == 33) { tO.firstname = "Instant"; tO.secondname = "Revolvers"; tO.desc1 = "War Painted"; tO.desc2 = "Suicidal"; tO.desc3 = "Combat Gang"; tO.longdesc = "Experts at depopulating hab stacks quickly and brutally"; tO.connection = "trading with"; }
		if (rand == 34) { tO.firstname = "Zone"; tO.secondname = "Bosses"; tO.desc1 = "Rag Enshrouded"; tO.desc2 = "Homeless"; tO.desc3 = "Guardians"; tO.longdesc = "Brew their own alcohol, trade it for SCOP; do gig work for SCOP"; tO.connection = "tending to avoid"; }
		if (rand == 35) { tO.firstname = "Sushi"; tO.secondname = "Thugs"; tO.desc1 = "Buddha-Faced"; tO.desc2 = "Steroid Using"; tO.desc3 = "Street Fighters"; tO.longdesc = "Skin-traders and flesh peddlars, buy and sell pretty young things"; tO.connection = "trading with"; }
		if (rand == 36) { tO.firstname = "Lucifer's"; tO.secondname = "Babies"; tO.desc1 = "Emo-Goth"; tO.desc2 = "Pseudo-Satanic"; tO.desc3 = "Nihilists"; tO.longdesc = "Find and sell corpses or parts to the body banks"; tO.connection = "making use of"; }
		if (rand == 37) { tO.firstname = "Remodelled"; tO.secondname = "Army"; tO.desc1 = "Goggle Wearing"; tO.desc2 = "Militant"; tO.desc3 = "Boosters"; tO.longdesc = "Self-styled free-company, market themselves as street condottieri"; tO.connection = "wary of"; }
		if (rand == 38) { tO.firstname = "Faceless"; tO.secondname = "Fanatics"; tO.desc1 = "Androgynous"; tO.desc2 = "Mask Wearing"; tO.desc3 = "Nihilists"; tO.longdesc = "Fence and sell black market cybernetic limbs of dubious origin"; tO.connection = "in a turf war with"; }
		if (rand == 39) { tO.firstname = "Vampire"; tO.secondname = "Daddies"; tO.desc1 = "Blood Drinking"; tO.desc2 = "Asexual"; tO.desc3 = "Skaters"; tO.longdesc = "Selling synthesized copies of virulent, manufactured HIVPlus strain"; tO.connection = "making use of"; }
		if (rand == 40) { tO.firstname = "Crosstown"; tO.secondname = "Chaos"; tO.desc1 = "Road Armored"; tO.desc2 = "Anarchist"; tO.desc3 = "Go-Gang"; tO.longdesc = "Will consider any job for new bikes or fuel; flaky, unpredictable"; tO.connection = "in a rivalry with"; }
		if (rand == 41) { tO.firstname = "Aryan"; tO.secondname = "Hammer"; tO.desc1 = "Heavily Tattooed"; tO.desc2 = "Alt-Right"; tO.desc3 = "Policlub"; tO.longdesc = "Frighteningly good propagandists and excellent rent-a-thugs"; tO.connection = "tending to avoid"; }
		if (rand == 42) { tO.firstname = "Spirit"; tO.secondname = "Method"; tO.desc1 = "Neo-Luddite"; tO.desc2 = "Religious"; tO.desc3 = "Guardians"; tO.longdesc = "Paid to protect a number of local Christian agri-businesses"; tO.connection = "in talks with"; }
		if (rand == 43) { tO.firstname = "Nomad"; tO.secondname = "Clan"; tO.desc1 = "Goth-Punk"; tO.desc2 = "Peddle Biking"; tO.desc3 = "Go-Gang"; tO.longdesc = "No-questions-asked bike courier service, as well as ride-by hits"; tO.connection = "making use of"; }
		if (rand == 44) { tO.firstname = "Hydraulic"; tO.secondname = "Terror"; tO.desc1 = "Industrial-Tech"; tO.desc2 = "Psychotic"; tO.desc3 = "Chromers"; tO.longdesc = "Taken over a wrecking yard so they can just, ya know, wreck stuff"; tO.connection = "being threatened by"; }
		if (rand == 45) { tO.firstname = "Fractal"; tO.secondname = "State"; tO.desc1 = "Tech Savvy"; tO.desc2 = "Eco-Socialist"; tO.desc3 = "Policlub"; tO.longdesc = "Minifacture their own flechette weapons and needleguns"; tO.connection = "borrowing money from"; }
		if (rand == 46) { tO.firstname = "Brain"; tO.secondname = "Wasters"; tO.desc1 = "Net Dependent"; tO.desc2 = "Philosophical"; tO.desc3 = "Nihilists"; tO.longdesc = "run an angst-soaked cloud forum, behind a secure pay wall"; tO.connection = "in a turf war with"; }
		if (rand == 47) { tO.firstname = "Screaming"; tO.secondname = "Shards"; tO.desc1 = "Radical"; tO.desc2 = "Feminist"; tO.desc3 = "Boosters"; tO.longdesc = "Hormone-farm captive young males to sell online"; tO.connection = "wanting to destroy"; }
		if (rand == 48) { tO.firstname = "Lucid"; tO.secondname = "Moderns"; tO.desc1 = "AR Manipulating"; tO.desc2 = "Political"; tO.desc3 = "Pranksters"; tO.longdesc = "Blackmail and harass prominent civil servants and create fake news"; tO.connection = "wary of"; }
		if (rand == 49) { tO.firstname = "Napalm"; tO.secondname = "Harvest"; tO.desc1 = "Afro-Haired"; tO.desc2 = "Multi-optic"; tO.desc3 = "Chromers"; tO.longdesc = "Carjacking and auto theft, as well as elaborate respraying"; tO.connection = "buying from"; }
		if (rand == 50) { tO.firstname = "Happy"; tO.secondname = "Losers"; tO.desc1 = "Neon-Raver"; tO.desc2 = "Anti-Corporate"; tO.desc3 = "Party Artists"; tO.longdesc = "Do a roaring trade in illegally downloaded, DrM-free music"; tO.connection = "allied to"; }

		
		
			
			
		if (i == 0) { output.firstname = tO.firstname; }
		if (i == 1) { output.secondname = tO.secondname; }
		if (i == 2) { output.desc1 = tO.desc1; }
		if (i == 3) { output.desc2 = tO.desc2; }
		if (i == 4) { output.desc3 = tO.desc3; }
		if (i == 5) { output.longdesc = tO.longdesc; }
		if (i == 6) { output.connection = tO.connection; }
		
	}
	return output;
}

function arckitPersonDetails() {
	var details = [
	{"a":"Armored up", "n":"Russian", "ct":"The state of the city", "d":"Friendly/Helpful" }, {"a":"Unwashed", "n":"Jamaican", "ct":"Big combat sports fan", "d":"Aggressive" },
	{"a":"Chain Smoker", "n":"French", "ct":"Driverless vehicles", "d":"Detached/Bored" }, {"a":"Goggles & tech", "n":"Sikh Indian", "ct":"Gangs in the area", "d":"Robotic" },
	{"a":"Fat, eating junk", "n":"Libyan", "ct":"Their huge family", "d":"Overly Cautious" }, {"a":"Looks exhausted", "n":"Nigerian", "ct":"Extreme alt-politics", "d":"Humorous" },
	{"a":"Sick & shaking", "n":"Uzbek", "ct":"Their tricked out vehicle", "d":"Inquisitive/Nosy" }, {"a":"Fresh implant(s)", "n":"American", "ct":"The ecopocalypse", "d":"Drunk/Stoned" },
	{"a":"Well groomed", "n":"Chinese", "ct":"Conspiracy theories", "d":"Uncommunicative" }, {"a":"Way too young", "n":"German", "ct":"Cod philosophy", "d":"Extremely Positive" },
	{"a":"Pock-marked", "n":"Dutch", "ct":"Classic rock music", "d":"Surly" }, {"a":"Half-starved", "n":"Mexican", "ct":"How connected they are", "d":"Lecherous" },
	{"a":"Heavily tattooed", "n":"Scottish", "ct":"The erratic weather", "d":"Servile" }, {"a":"Charms & symbols", "n":"Irish", "ct":"Favorite TV shows", "d":"Superstitious" },
	{"a":"Army fatigues", "n":"Syrian", "ct":"Cyberware brands", "d":"Abrupt" }, {"a":"Fully cyborged", "n":"Ukrainian", "ct":"The recent/ongoing war", "d":"Devout/Religious" },
	{"a":"Hardwired in", "n":"Thai", "ct":"They used to be rich", "d":"Boastful" }, {"a":"Transvestite", "n":"Mongolian", "ct":"Racist tirades", "d":"Flamboyant" },
	{"a":"Tribal Scarring", "n":"Somali", "ct":"Corporate gossip (fake)", "d":"Careless/Clumsy" }, {"a":"Oversize optics", "n":"Local", "ct":"Kung-fu movies", "d":"Suicidal" },
	{"a":"Chromed teeth", "n":"Saudi", "ct":"Was once a surgeon", "d":"Distracted" }, {"a":"Slick with sweat", "n":"Portuguese", "ct":"Their sexual conquests", "d":"Cynical" },
	{"a":"Atrophied limb", "n":"Local", "ct":"Reality TV shows suck", "d":"Has Tourettes" }, {"a":"Twitches", "n":"Innuit", "ct":"Their deep dislike of AR", "d":"Argumentative" },
	{"a":"Polythene coverall", "n":"Spanish", "ct":"Their pet synthetic goat", "d":"Unpredictable" }, {"a":"Layered T-shirts", "n":"Turkish", "ct":"Global politics", "d":"Nervous" },
	{"a":"Four Handguns", "n":"Danish", "ct":"The spectre of terrorism", "d":"Ashamed" }, {"a":"Excessive vaping", "n":"Latvian", "ct":"Hi-tech weaponry", "d":"Charming" },
	{"a":"Filthy baseball cap", "n":"Romany", "ct":"Their many ex-partners", "d":"Incoherent" }, {"a":"Scruffy looking", "n":"Greek", "ct":"Their work on their PHD", "d":"Noisy/Interrupts" },
	{"a":"Facial burn scars", "n":"Local", "ct":"Their missing daughter", "d":"1000 Yard Stare" }, {"a":"Constant texting", "n":"Romanian", "ct":"Rampant crime", "d":"Ambitious" },
	{"a":"Designer coat", "n":"Finn", "ct":"The latest VR games", "d":"Calm" }, {"a":"HUD Visor", "n":"Flemish", "ct":"Their favorite eateries", "d":"Arrogant" },
	{"a":"Respirator", "n":"Pakistani", "ct":"Info-Socialist revolution", "d":"Grumpy" }, {"a":"Mirrorshades", "n":"Italian", "ct":"Their aggressive cancer", "d":"Law Abiding" },
	{"a":"Slab-headed", "n":"Siberian", "ct":"Disdain for the police", "d":"Depressive" }, {"a":"Surplus flight suit", "n":"Local", "ct":"Was once a pilot", "d":"Sensible" },
	{"a":"Geriatric", "n":"Bulgarian", "ct":"Today's youth/drugs", "d":"Greedy" }, {"a":"Creaking leathers", "n":"South African", "ct":"The rich/poor divide", "d":"Respectful" },
	{"a":"Jury-rigged IV drip", "n":"Malay", "ct":"Desire to be an actor", "d":"Volatile/Touchy" }, {"a":"Bright blue hair", "n":"Kenyan", "ct":"The ethics of cloning", "d":"Judgemental" },
	{"a":"Bullet earrings", "n":"Tibetan", "ct":"Endorses brands (paid)", "d":"Vulgar/Rude" }, {"a":"Wearable cameras", "n":"Polish", "ct":"Making own net show", "d":"Furtive" },
	{"a":"Autoshotgun", "n":"Swedish", "ct":"Pushing PCs for a job", "d":"Patronising" }, {"a":"Has no legs", "n":"Hungarian", "ct":"The welfare system", "d":"Foolish/Stupid" },
	{"a":"Alopecia", "n":"Congolese", "ct":"Getting out the business", "d":"Grateful" }, {"a":"Thin cyberarms", "n":"Moldovan", "ct":"Extreme sex SimStims", "d":"Lonely" },
	{"a":"Aviator glasses", "n":"Afghan", "ct":"State of their slum-cube", "d":"Tuneful/Musical" }, {"a":"Pale green optics", "n":"Kashmiri", "ct":"How they live in the cab", "d":"Naive" }
	];
	
	var rand1 = Math.ceil(Math.random()*(details.length-1));
	var rand2 = Math.ceil(Math.random()*(details.length-1));
	var rand3 = Math.ceil(Math.random()*(details.length-1));
	var rand4 = Math.ceil(Math.random()*(details.length-1));
	
	var rand5 = Math.ceil(Math.random()*2);
	if (rand5 == 1) { var gender = "Man"; }
	if (rand5 == 2) { var gender = "Woman"; }
	
	var personDetails = { "appearance":details[rand1].a, "nationality":details[rand2].n, "conversation":details[rand3].ct, "demeanour":details[rand4].d, "gender":gender };
	
	return personDetails;
}

function arckitPersonName(gender) {
	var gender1 = gender;
	
	var firstNamesMaleEnglish = [
		"Jackson", "Aiden", "Lucas", "Liam", "Noah", "Ethan", "Mason", "Caden", "Oliver", "Elijah", "Grayson", "Jacob", "Michael", "Bejamin", "Carter", "James", "Jayden", "Logan", "Alexander", "Caleb",
		"Ryan", "Luke", "Daniel", "Jack", "William", "Owen", "Gabriel", "Matthew", "Connor", "Jayce", "Isaac", "Sebastian", "Henry", "Ben", "Cameron", "Wyatt", "Dylan", "Nathan", "Nicholas", "Julian",
		"Eli", "Levi", "Isaiah", "Landon", "David", "Christian", "Andrew", "Brayden", "John", "Lincoln", "Samuel", "Joseph", "Hunter", "Joshua", "Mateo", "Dominic", "Adam", "Leo", "Ian", "Josiah",
		"Anthony", "Colton", "Max", "Thomas", "Evan", "Nolan", "Aaron", "Carson", "Christopher", "Hudson", "Cooper", "Adrian", "Jonathan", "Jason", "Charlie", "Miles", "Jeremiah", "Gavin", "Asher", "Austin",
		"Ezra", "Chase", "Alex", "Xavier", "Jordan", "Tristan", "Easton", "Zachary", "Parker", "Bryson", "Tyler", "Camden", "Damian", "Declan", "Elliot", "Elias", "Cole", "Harrison", "Zane", "Kai"
	];
	
	var firstNamesFemaleEnglish = [
		"Sophia", "Emma", "Olivia", "Ava", "Mia", "Isabella", "Riley", "Aria", "Zoe", "Charlotte", "Lily", "Layla", "Amelia", "Emily", "Madelyn", "Aubrey", "Adalyn", "Madison", "Chloe", "Harper",
		"Abigail", "Aaliyah", "Avery", "Evelyn", "Kaylee", "Ella", "Ellie", "Scarlett", "Arianna", "Hailey", "Nora", "Addison", "Brooklyn", "Hannah", "Mila", "Leah", "Elizabeth", "Sarah", "Eliana", "Mackenzie",
		"Peyton", "Maria", "Grace", "Adeline", "Elena", "Anna", "Victoria", "Camilla", "Lillian", "Natalie", "Isabelle", "Skyler", "Maya", "Lucy", "Lila", "Audrey", "Makayla", "Penelope", "Claire", "Kennedy",
		"Paisley", "Savannah", "Alaina", "Gabriella", "Violet", "Kylie", "Charlie", "Stella", "Allison", "Liliana", "Eva", "Callie", "Kinsley", "Reagan", "Sophie", "Alyssa", "Alice", "Caroline", "Aurora", "Eleanor",
		"Juliana", "Annabelle", "Emilia", "Sadie", "Bella", "Julia", "Keira", "Bailey", "Hazel", "Jocelyn", "London", "Samantha", "Vivian", "Gianna", "Alexandra", "Cora", "Melanie", "Everly", "Jordyn", "Luna"
	];
	
	var surnamesEnglish = [
		"Smith", "Johnson", "Williams", "Jones", "Brown", "Davis", "Miller", "Wilson", "Moore", "Taylor", "Anderson", "Thomas", "Jackson", "White", "Harris", "Martin", "Thompson", "Garcia", "Martinez", "Robinson",
		"Clark", "Rodriguez", "Lewis", "Lee", "Walker", "Hall", "Allen", "Young", "Hernandez", "King", "Wright", "Lopez", "Hill", "Scott", "Green", "Adams", "Baker", "Gonzalez", "Nelson", "Carter",
		"Mitchell", "Perez", "Roberts", "Turner", "Phillips", "Campbell", "Parker", "Evans", "Edwards", "Collins", "Stewart", "Sanchez", "Morris", "Rogers", "Reed", "Cook", "Morgan", "Bell", "Murphy", "Bailey",
		"Rivera", "Cooper", "Richardson", "Cox", "Howard", "Ward", "Torres", "Peterson", "Gray", "Ramirez", "James", "Watson", "Brooks", "Kelly", "Sanders", "Price", "Bennett", "Wood", "Barnes", "Ross",
		"Henderson", "Coleman", "Jenkins", "Perry", "Powell", "Long", "Patterson", "Hughes", "Flores", "Washington", "Butler", "Simmons", "Foster", "Gonzales", "Bryant", "Alexander", "Russell", "Griffin", "Diaz", "Hayes"
	];
	
	var surnamesJapanese = [
		"Sato", "Suzuki", "Takahashi", "Watanabe", "Ito", "Yamamoto", "Nakamura", "Kobayashi", "Kato", "Yoshida", "Yamada", "Sasaki", "Yamaguchi", "Saito", "Matsumoto", "Inoue", "Kimura", "Hayashi", "Shimizu",
		"Yamazaki", "Mori", "Abe", "Ikeda", "Hashimoto", "Yamashita", "Ishikawa", "Nakajima", "Maeda", "Fujita", "Ogawa", "Goto", "Okada", "Hasegawa", "Murakami", "Kondo", "Ishii", "Saito", "Sakamoto", "Endo",
		"Aoki", "Fujii", "Nishimura", "Fukuda", "Ota", "Miura", "Fujiwara", "Okamoto", "Matsuda", "Nakagawa", "Nakano", "Harada", "Ono", "Tamura", "Takeuchi", "Kaneko", "Wada", "Nakayama", "Ishida", "Ueda",
		"Morita", "Hara", "Shibata", "Sakai", "Kudo", "Yokoyama", "Miyazaki", "Miyamoto", "Uchida", "Takagi", "Ando", "Taniguchi", "Ohno", "Maruyama", "Imai", "Takada", "Fujimoto", "Takeda", "Murata", "Ueno",
		"Sugiyama", "Masuda", "Sugawara", "Hirano", "Kojima", "Otsuka", "Chiba", "Kubo", "Matsui", "Iwasaki", "Sakurai", "Kinoshita", "Noguchi", "Matsuo", "Nomura", "Kikuchi", "Sano", "Onishi", "Sugimoto", "Arai"
	];
	
	var name = { "fn":"", "ln":"" };
	
	if (gender1 != "male" && gender1 != "female") {
		var randomGen = Math.ceil(Math.random()*2);
		
		if (randomGen == 1) { gender1 = "male"; }
		if (randomGen == 2) { gender1 = "female"; }
	}
	
	if (gender1 == "male") { name.fn = firstNamesMaleEnglish[Math.ceil(Math.random()*(firstNamesMaleEnglish.length-1))]; }
	if (gender1 == "female") { name.fn = firstNamesFemaleEnglish[Math.ceil(Math.random()*(firstNamesFemaleEnglish.length-1))]; }
	
	
	var randomLast = Math.ceil(Math.random()*2);
	if (randomLast == 1) { name.ln = surnamesEnglish[Math.ceil(Math.random()*(surnamesEnglish.length-1))]; }
	if (randomLast == 2) { name.ln = surnamesJapanese[Math.ceil(Math.random()*(surnamesJapanese.length-1))]; }
		
	return name;
}

function fantasyEncounterType() {
	var eTypes = [
		"Settlement", "Stronghold/Castle", "Ruins/Remains", "Monster", "Obstacle", "Person", "Object/Vehicle/Mount", "Adventurers", "Special"
	];
	
	var randType = Math.ceil(Math.random()*(eTypes.length-1));
	
	if (eTypes[randType].n == "Special") {
		var randType = Math.ceil(Math.random()*(eTypes.length-2));
		var randType1 = Math.ceil(Math.random()*(eTypes.length-2));
		if (randType == -1) { randType = 0; }
		if (randType1 == -1) { randType1 = 0; }
		
		output = "Special (" + eTypes[randType] + ", " + eTypes[randType1] + ")";
	} else {
		output = eTypes[randType];
	}
	
	return output;
}

function fantasyEncounterReaction() {
	var eReactions = [
		{ "n":"Helpful", "d":"The encountered entity wants to help the PCs in some way." }, { "n":"Wary", "d":"The encountered entity may be willing to help the PCs. Make a reaction roll (Exploits, p12) based on what the delvers offer." },
		{ "n":"Neutral", "d":"The encountered entity is ambivalent towards the PCs." }, { "n":"Neutral", "d":"The encountered entity is ambivalent towards the PCs." },
		{ "n":"Unfriendly", "d":"The encountered entity wants to be life alone, or dislikes the PCs." }, { "n":"Hostile!", "d":"The encountered entity initiates combat!" }
	];
	
	var randReact = Math.ceil(Math.random() * (eReactions.length - 1));
	
	var output = eReactions[randReact];
	
	return output;
}

function fantasyEncounterMotivation() {
	var eMotivations = [
		{ "n":"Ill Intent/Malevolent", "d":"Bears ill will toward the PCs. This could stem from coercion, blackmail, or being misinformed as much as malice, greed, or just plain bloodthirsty. If the result from Initial Reaction (above) is anything except “hostile,” then the ill will is not insurmountable. (Thus, the party may be able to talk their way out of violence, or run away without being pursued.)" }, { "n":"Curiosity", "d":"Is curious about the PCs and may talk, bargain, or negotiate with them. Combined with a negative result from Initial Reaction (above), this could indicate an NPC who wants to learn something from, spy on, or otherwise gather information on the PCs for dubious purposes." },
		{ "n":"Impressed/Envious", "d":"Is impressed by or envious of the PCs, or something they’ve done in the past" }, { "n":"Stalking/Hunting", "d":"Is stalking or hunting the PCs for some reason. A helpful stalker might be an overeager fan who wants to share in the glory even though it might kill him; a hostile stalker wants the PCs dead or hurt for some reason" },
		{ "n":"Being Attacked", "d":"Is being attacked, hassled, or otherwise harassed by a third party. Roll the other group separately; getting this result again might mean a comedy of errors!" }, { "n":"Deranged", "d":"Is crazy. Maybe the encountered likes how the PCs smell or perhaps the sky showered invisible skulls on them until they attacked the PCs" },
		{ "n":"Teaching", "d":"Wants to teach the PCs something. Hostile teachers are possible. Pai Mei from Kill Bill is a prime example, and malicious faeries “educating” mortals are a classic fantasy trope." }, { "n":"On a Mission/Task/Errand", "d":"Has been tasked with a specifc mission or job of vital importance (at least to that person). An adventurer might be on a rescue mission, or a villager may be traveling to sell their wares. This can even apply to settlements (a village in charge of keeping a bridge open and defended) or objects (a sacred rock that prevents undead from entering the area, but can be despoiled by an evil cleric or similar)" },
		{ "n":"Bored/Jaded", "d":"Is bored and following the PCs because they live exciting lives." }, { "n":"Weird", "d":"Has Weirdness Magnet (Adventurers, p. 67) and has attracted the PCs into its orbit (a mutual effect, if one of the PCs also has Weirdness Magnet). Maybe they’re a pointy-hat-wearing wizard with a funny name and the inability to properly cast a freball without bat guano, or a delver who has been cursed to “live in interesting times” – and what’s more interesting than adventurers? Regardless of intent, their very presence is strange. Play it up!" },
		{ "n":"Lost", "d":"Is lost and trying to make their way back to familiar surroundings. Helpful ones are trying to assist others with their own problems along the way! A classic trope for a knight-errant" }, { "n":"Criminal", "d":"Is a criminal or has criminal intentions. This roll can indicate bandits, those on the run from the law (innocent or not), etc." },
		{ "n":"Merchant", "d":"Is trying to sell, buy, or trade goods, services, or other commodities. This can also represent a merchant caravan hauling goods from one place to another." }, { "n":"Looking for a Fight", "d":"Is looking for a fght, a duel, or some other form of combat between themselves and the PCs. This does not have to be to the death! Little John challenged Robin Hood to a fight, but never wanted to kill the rogue." },
		{ "n":"Revenge/Vendetta", "d":"Is looking for revenge or otherwise has a vendetta. This may be directed toward the PCs or (more likely) someone or something else." },
		{ "n":"Special", "d":"" }
	];
	
	var randMotiv = Math.ceil(Math.random()*(eMotivations.length-1));
	
	if (eMotivations[randMotiv].n == "Special") {
		var randMotiv = Math.ceil(Math.random()*(eMotivations.length-2));
		var randMotiv1 = Math.ceil(Math.random()*(eMotivations.length-2));
		if (randMotiv == -1) { randMotiv = 0; }
		if (randMotiv1 == -1) {randMotiv1 = 0; }
		
		output = "Dual Motivation (" + eMotivations[randMotiv].n + ": " + eMotivations[randMotiv].d + "\n" + eMotivations[randMotiv1].n + ": " + eMotivations[randMotiv1].d + ")";
	} else {
		output = eMotivations[randMotiv].n + " (" + eMotivations[randMotiv].d + ")";
	}
	
	return output;
}

function fantasyEncounterDistance() {
	var eDistances = [
		{ "n":"Close", "r":"0 - 5 yards", "p":"0 to -2", "d":"Can touch encountered, at least some of the time." }, { "n":"Short", "r":"6 - 20 yards", "p":"-3 to -6", "d":"Can talk to encountered, or toss things at them." }, 
		{ "n":"Medium", "r":"21 - 100 yards", "p":"-7 to -10", "d":"Can only shout at encountered (ranged weapons needed to attack)." }, { "n":"Medium", "r":"21 - 100 yards", "p":"-7 to -10", "d":"Can only shout at encountered (ranged weapons needed to attack)." },
		{ "n":"Long", "r":"101 - 500 yards", "p":"-11 to -14", "d":"Encountered is out of earshot (ranged weapons needed to attack)." }, { "n":"Extreme", "r":"501+ yards", "p":"-15 or worse", "d":"Encountered is difficult or impossible to see or attack (ranged weapons needed to attack)." }
	];
	
	var randDist = Math.ceil(Math.random() * (eDistances.length - 1));
	distance = eDistances[randDist];
	
	output = distance.n + " Range: " + distance.r + " starting distance, with a Vision penalty of " + distance.p + ". " + distance.d;
	
	return output;
}

function fantasyEncounterManaSanctity() {
	var randSum = (Math.ceil(Math.random() * 6) + Math.ceil(Math.random() * 6) + Math.ceil(Math.random() * 6));
	
	if (randSum == 3 || randSum == 4 || randSum == 5) {
		msObj = { "n":"Very High", "d":"Every second, automatically recover all personal FP spent. Failures are treated as critical failures; critical failures nullify abilities until penance is paid (for sanctity) or cause spectacular disasters (for mana)." };
	}
	
	if (randSum == 6 || randSum == 7) {
		msObj = { "n":"High", "d":"Those with Recover Energy have enhanced recovery rates (Spells, pp 5, 7)." };
	}
	
	if (randSum == 8 || randSum == 9 || randSum == 10 || randSum == 11 || randSum == 12 || randSum == 13) {
		msObj = { "n":"Normal", "d":"No additional effect." };
	}

	if (randSum == 14 || randSum == 15) {
		msObj = { "n":"Low", "d":"Spells are cast at -5 for all purposes, and Recover Energy does not function." };
	}
	
	if (randSum == 16 || randSum == 17 || randSum == 18) {
		msObj = { "n":"No", "d":"Spells cannot be used, and ongoing spells end immediately." };
	}
	
	return msObj;
}

function fantasyEncounterTerrain() {
	var eTerrains = [
		"Arctic/Tundra", "Desert", "Island/Beach", "Plains", "Mountains", "Woodlands", "Swampland", "Underground", "Jungle", "Watery", "Urban", "Special"
	];
	
	var eSpecTerrains = [
		"Ruins", "Wizard Tower/Keep/Academy", "Temple/Sacred Grove", "Stronghold/Castle", "Fantastic Natural Location (e.g., volcano)", "Location of Supernatural Significance", "Graveyard/Tomb/Burial Place"
	];
	
	var randET = Math.ceil(Math.random() * (eTerrains.length - 1));
	
	if (eTerrains[randET] == "Special") {
		var randET = Math.ceil(Math.random()*(eTerrains.length-2));
		if (randET == -1) { randET = 0; }
		
		var randEST = Math.ceil(Math.random()*(eSpecTerrains.length-1));
		
		output = { "n":eTerrains[randET], "s":eSpecTerrains[randEST] };
	} else {
		output = { "n":eTerrains[randET], "s":"None" };
	}
	
	return output;
}

function fantasyEncounterWeather() {
	var randSum = (Math.ceil(Math.random() * 6) + Math.ceil(Math.random() * 6) + Math.ceil(Math.random() * 6));
	
	if (randSum == 3 || randSum == 4 || randSum == 5) {
		weatherObj = { "n":"Perfect", "d":"The wind is at the travelers’ back or in their sails, and the terrain’s usual misery abates – say, a dry day in jungle terrain, or a warm, snowstorm-free one in arctic. Add 10% to travel speed. All Survival and Tracking rolls that day are at +1. Skill rolls for aimed ranged attacks get +1." };
	}
	
	if (randSum == 6 || randSum == 7) {
		weatherObj = { "n":"Near Perfect", "d":"As for Perfect, but a little less so. Add 5% to travel speed and give +1 to either Survival or Tracking rolls. No effect on ranged attacks." };
	}
	
	if (randSum == 8 || randSum == 9 || randSum == 10 || randSum == 11) {
		weatherObj = { "n":"Passable", "d":"As bad or as fair as usual for the terrain. No effect on attacks or travel speed or skills." };
	}
	
	if (randSum == 12 || randSum == 13) {
		weatherObj = { "n":"Bad", "d":"Rain or snow in most terrain, extra rain in jungle, light snowstorm in arctic, sandstorms in desert, etc. Subtract 50% from travel speed. Survival and Tracking rolls that day are at -1." };
	}
	
	if (randSum == 14 || randSum == 15) {
		weatherObj = { "n":"Very Bad", "d":"As for Bad, but much worse. Subtract 75% from travel speed. Ranged attacks are at -1, while aimed ranged attacks are at -2." };
	}
	
	if (randSum == 16 || randSum == 17 || randSum == 18) {
		weatherObj = { "n":"Dire", "d":"As for Very Bad, but something or someone hates the party. Subtract 75% from travel speed. Survival and Tracking rolls that day are at -2. Ranged attacks are at -2, while aimed ranged attacks are at -3." };
	}
	
	return weatherObj;
}

function fantasyEncounterTime() {
	var randSum = (Math.ceil(Math.random() * 6) + Math.ceil(Math.random() * 6) + Math.ceil(Math.random() * 6));
	
	if (randSum == 3 || randSum == 4 || randSum == 5) {
		timeObj = { "n":"Dawn", "d":"The hours right before, during, or right after sunrise. This gives a -1 on all Vision, melee, and ranged attack rolls." };
	}
	
	if (randSum == 6 || randSum == 7) {
		timeObj = { "n":"Morning", "d":"Any time after the sun has fully risen, but before it has reached its zenith. This has no effect on rolls." };
	}
	
	if (randSum == 8 || randSum == 9 || randSum == 10 || randSum == 11) {
		timeObj = { "n":"Afternoon", "d":"Any time after the sun has reached its zenith, but before it has set. This has no effect on rolls." };
	}
	
	if (randSum == 12 || randSum == 13) {
		timeObj = { "n":"Evening/Dusk", "d":"The hours right before, during, or right after sunset. This gives a -1 on all Vision, melee, and ranged attack rolls." };
	}
	
	if (randSum == 14 || randSum == 15) {
		timeObj = { "n":"Night", "d":"Any time after the sun has fully set, but before midnight. This gives at least -3 on all Vision, melee, and ranged attack rolls." };
	}
	
	if (randSum == 16 || randSum == 17 || randSum == 18) {
		timeObj = { "n":"Early Hours", "d":"Any time after midnight, but before dawn. This gives at least -5 on all Vision, melee, and ranged attack rolls." };
	}
	
	return timeObj;
}

function fantasyPersonName(gender) {
	var gender1 = gender;
	
	var firstNamesMaleEnglish = [
		"Jackson", "Aiden", "Lucas", "Liam", "Noah", "Ethan", "Mason", "Caden", "Oliver", "Elijah", "Grayson", "Jacob", "Michael", "Bejamin", "Carter", "James", "Jayden", "Logan", "Alexander", "Caleb",
		"Ryan", "Luke", "Daniel", "Jack", "William", "Owen", "Gabriel", "Matthew", "Connor", "Jayce", "Isaac", "Sebastian", "Henry", "Ben", "Cameron", "Wyatt", "Dylan", "Nathan", "Nicholas", "Julian",
		"Eli", "Levi", "Isaiah", "Landon", "David", "Christian", "Andrew", "Brayden", "John", "Lincoln", "Samuel", "Joseph", "Hunter", "Joshua", "Mateo", "Dominic", "Adam", "Leo", "Ian", "Josiah",
		"Anthony", "Colton", "Max", "Thomas", "Evan", "Nolan", "Aaron", "Carson", "Christopher", "Hudson", "Cooper", "Adrian", "Jonathan", "Jason", "Charlie", "Miles", "Jeremiah", "Gavin", "Asher", "Austin",
		"Ezra", "Chase", "Alex", "Xavier", "Jordan", "Tristan", "Easton", "Zachary", "Parker", "Bryson", "Tyler", "Camden", "Damian", "Declan", "Elliot", "Elias", "Cole", "Harrison", "Zane", "Kai"
	];
	
	var firstNamesFemaleEnglish = [
		"Sophia", "Emma", "Olivia", "Ava", "Mia", "Isabella", "Riley", "Aria", "Zoe", "Charlotte", "Lily", "Layla", "Amelia", "Emily", "Madelyn", "Aubrey", "Adalyn", "Madison", "Chloe", "Harper",
		"Abigail", "Aaliyah", "Avery", "Evelyn", "Kaylee", "Ella", "Ellie", "Scarlett", "Arianna", "Hailey", "Nora", "Addison", "Brooklyn", "Hannah", "Mila", "Leah", "Elizabeth", "Sarah", "Eliana", "Mackenzie",
		"Peyton", "Maria", "Grace", "Adeline", "Elena", "Anna", "Victoria", "Camilla", "Lillian", "Natalie", "Isabelle", "Skyler", "Maya", "Lucy", "Lila", "Audrey", "Makayla", "Penelope", "Claire", "Kennedy",
		"Paisley", "Savannah", "Alaina", "Gabriella", "Violet", "Kylie", "Charlie", "Stella", "Allison", "Liliana", "Eva", "Callie", "Kinsley", "Reagan", "Sophie", "Alyssa", "Alice", "Caroline", "Aurora", "Eleanor",
		"Juliana", "Annabelle", "Emilia", "Sadie", "Bella", "Julia", "Keira", "Bailey", "Hazel", "Jocelyn", "London", "Samantha", "Vivian", "Gianna", "Alexandra", "Cora", "Melanie", "Everly", "Jordyn", "Luna"
	];
	
	var firstNamesMaleGerman = [
		"Adam", "Adrian", "Albert", "Alex", "Alexis", "Alfred", "Andre", "Andreas", "Anton", "Arnie", "Arnold", "Axel", "Benedict", "Benjamin", "Bjorn",
		"Bruno", "Carl", "Christian", "Daniel", "David", "Dominik", "Elias", "Emil", "Eric", "Ernest", "Aveline", "Fabian", "Felix", "Florian", "Frank",
		"Frederick", "Gabriel", "Gerhard", "Harald", "Heinz", "Henrik", "Hugo", "Jakob", "Johann", "Jonas", "Josef", "Julian", "Karl", "Kevin", "Kurt",
		"Lars", "Lawrence", "Leon", "Manuel", "Marcel", "Mark", "Marco", "Marius", "Markus", "Martin", "Mathias", "Maximilian", "Michael", "Nico", "Oliver",
		"Pascal", "Paul", "Peter", "Raphael", "Richard", "Robert", "Roger", "Roland", "Roman", "Rudolf", "Samuel", "Sebastian", "Simon", "Stephan", "Thomas",
		"Timothy", "Tobias", "Tom", "Viktor", "Walter", "Werner"
	];
	
	var firstNamesFemaleGerman = [
		"Abigail", "Agnes", "Alena", "Alex", "Alexandra", "Alexis", "Alina", "Amalie", "Andrea", "Angela", "Anita", "Anya", "Anna", "Astrid",
		"Aurora", "Ava", "Barbara", "Bertha", "Brigitte", "Camilla", "Carla", "Caroline", "Charlotte", "Christina", "Clara", "Claudia", "Dana",
		"Daniela", "Doris", "Edith", "Elena", "Elisa", "Elizabeth", "Elsa", "Emma", "Erika", "Gertrude", "Greta", "Hannah", "Heidi", "Helen", "Helena",
		"Hilda", "Ida", "Ingrid", "Irene", "Janna", "Jasmine", "Jennifer", "Jessica", "Josephine", "Judith", "Julie", "Justine", "Karen", "Karolina",
		"Katya", "Kristin", "Lara", "Laura", "Leah", "Lena", "Lily", "Linda", "Maya", "Lucia", "Louise", "Lisa", "Margaret", "Maria", "Marta", "Martha",
		"Martina", "Matilda", "Melanie", "Mia", "Mikayla", "Monica", "Nadia", "Natalie", "Nicole", "Nina", "Olivia", "Patricia", "Paula", "Petra", "Phyllis",
		"Rachel", "Regina", "Rita", "Rosa", "Ruth", "Sabrina", "Sandra", "Sara", "Silvia", "Sofia", "Stephanie", "Susanna", "Dorothy", "Ursula", "Valerie",
		"Veronica", "Victoria", "Wilma"
	];
	
	var surnamesEnglish = [
		"Smith", "Johnson", "Williams", "Jones", "Brown", "Davis", "Miller", "Wilson", "Moore", "Taylor", "Anderson", "Thomas", "Jackson", "White", "Harris", "Martin", "Thompson", "Garcia", "Martinez", "Robinson",
		"Clark", "Rodriguez", "Lewis", "Lee", "Walker", "Hall", "Allen", "Young", "Hernandez", "King", "Wright", "Lopez", "Hill", "Scott", "Green", "Adams", "Baker", "Gonzalez", "Nelson", "Carter",
		"Mitchell", "Perez", "Roberts", "Turner", "Phillips", "Campbell", "Parker", "Evans", "Edwards", "Collins", "Stewart", "Sanchez", "Morris", "Rogers", "Reed", "Cook", "Morgan", "Bell", "Murphy", "Bailey",
		"Rivera", "Cooper", "Richardson", "Cox", "Howard", "Ward", "Torres", "Peterson", "Gray", "Ramirez", "James", "Watson", "Brooks", "Kelly", "Sanders", "Price", "Bennett", "Wood", "Barnes", "Ross",
		"Henderson", "Coleman", "Jenkins", "Perry", "Powell", "Long", "Patterson", "Hughes", "Flores", "Washington", "Butler", "Simmons", "Foster", "Gonzales", "Bryant", "Alexander", "Russell", "Griffin", "Diaz", "Hayes"
	];
	
	var surnamesScottish = [
		"Smith", "Brown", "Wilson", "Thomson", "Robertson", "Campbell", "Stewart", "Anderson", "MacDonald", "Scott", "Reid", "Murray", "Taylor", "Clark", "Mitchel",
		"Ross", "Walker", "Paterson", "Young", "Watson", "Morrison", "Miller", "Fraser", "Davidson", "Gray", "McDonald", "Henderson", "Johnston", "Hamilton", "Graham",
		"Kerr", "Simpson", "Martin", "Ferguson", "Cameron", "Duncan", "Hunter", "Kelly", "Bell", "Grant", "Mackenzie", "MacKay", "Allan", "Black", "MacLeod",
		"McLean", "Russell", "Gibson", "Wallace", "Gordon", "Marshall", "Stevenson", "Wood", "Sutherland", "Craig", "Wright", "McKenzie", "Kennedy", "Jones", "Burns",
		"White", "Muir", "Murphy", "Johnstone", "Hughes", "Watt", "McMillan", "McIntosih", "Milne", "Munro", "Ritchie", "Dickson", "Bruce", "King", "Crawford",
		"Docherty", "Millar", "Cunningham", "Sinclair", "Williamson", "Hill", "McGregor", "McKay", "Boyle", "Shaw", "Fleming", "Moore", "Christie", "Douglas", "Donaldson",
		"Alexander", "MacLean", "Forbes", "Williams", "McIntyre", "Findlay", "Jamieson", "Aitken", "Reilly", "Thompson"
	];
	
	/*
	var total = (firstNamesFemaleEnglish.length * surnamesEnglish.length) + (firstNamesMaleEnglish.length * surnamesEnglish.length);
	total = total + (firstNamesFemaleEnglish.length * surnamesScottish.length) + (firstNamesMaleEnglish.length * surnamesScottish.length);
	total = total + (firstNamesFemaleGerman.length * surnamesEnglish.length) + (firstNamesMaleGerman.length * surnamesEnglish.length);
	total = total + (firstNamesFemaleGerman.length * surnamesScottish.length) + (firstNamesMaleGerman.length * surnamesScottish.length);
	
	console.log(total); */
	
	
	if (gender1 != "male" && gender1 != "female") {
		var randomGen = Math.ceil(Math.random()*2);
		
		if (randomGen == 1) { gender1 = "male"; }
		if (randomGen == 2) { gender1 = "female"; }
	}
	var name = { "fn":"", "ln":"", "gen":gender1.charAt(0).toUpperCase() + gender1.slice(1) };
	
	var randomFirstNationality = Math.ceil(Math.random()*2);
	if (randomFirstNationality == 1) {
		if (gender1 == "male") { name.fn = firstNamesMaleGerman[Math.ceil(Math.random()*(firstNamesMaleGerman.length-1))]; }
		if (gender1 == "female") { name.fn = firstNamesFemaleGerman[Math.ceil(Math.random()*(firstNamesFemaleGerman.length-1))]; }
	} else if (randomFirstNationality == 2) {
		if (gender1 == "male") { name.fn = firstNamesMaleEnglish[Math.ceil(Math.random()*(firstNamesMaleEnglish.length-1))]; }
		if (gender1 == "female") { name.fn = firstNamesFemaleEnglish[Math.ceil(Math.random()*(firstNamesFemaleEnglish.length-1))]; }
	}
	
	
	var randomLast = Math.ceil(Math.random()*2);
	if (randomLast == 1) { name.ln = surnamesEnglish[Math.ceil(Math.random()*(surnamesEnglish.length-1))]; }
	if (randomLast == 2) { name.ln = surnamesScottish[Math.ceil(Math.random()*(surnamesScottish.length-1))]; }
		
	return name;
}

function fantasyPersonCharacterTraits() {
	var speechTypes = [
		"Accented", "Breathless", "Crisp", "Fast", "Guttural", "Halting", "Husky", "Lisps", "Low-pitched", "Loud",
		"Nasal", "Nervous", "Raspy", "Slow", "Slurs", "Squeaky", "Stutters", "Wheezy", "Whiny", "Whispery"
	];
	
	var hairTypes = [
		"Bald", "Braided", "Curly", "Dreadlocks", "Frazzled", "Greasy", "Limp", "Long", "Messy", "Strange Hairstyle",
		"Pony-tail", "Short", "Straight", "Streaked", "Thick", "Thinning", "Very Long", "Wavy", "Well Groomed", "Wiry"
	];
	
	var facialFeatures = [
		"Acne", "Beard", "Buck-toothed", "Chiseled", "Doe-eyed", "Distinctive nose", "Gap-toothed", "Goatee", "Grizzled",
		"Missing ear", "Missing eye", "Missing teeth", "Moustache", "Pierced", "Pock-marked", "Sideburns", "Squinty", "Stained teeth", "Weather-beaten"
	];
	
	var traitsTable1 = [
		"Birthmark*", "Birthmark*", "Body piercings*", "Body piercings*", "Body piercings*", "Chews tobacco", "Scarred*", "Scarred*", "Scarred*", "Scarred*", "Scarred*",
		"Scarred*", "Scarred*", "Smokes", "Smokes", "Smokes", "Smokes", "Tattooed*", "Tattooed*", "Tattooed*"
	];
	
	var traitsTable2 = [
		"Allergic to food/dust/pollen/animals", "Always arrives late", "Always gives vaguest possible answer", "Always has something in hands", "Always wears as little as possible",
		"Always wears expensive clothes", "Always wears same color", "Always wears tattered clothes", "Answers questions with questions", "Aversion to certain kind of food",
		"Bad breath or strong body odor", "Bad/loud/annoying/shrill laugh", "Bad with money", "Believes all animals can talk to each other", "Bites fingernails", "Bites lips",
		"Black eye", "Bleeding nose", "Blinks constantly", "Bruises easily"
	];
	
	var traitsTable3 = [
		"Burps", "Burn scar*", "Chews with mouth open", "Chortles", "Clicks tongue", "Collects teeth/hair/claws of slain opponents", "Constantly asks for divine advice",
		"Covered in sores, boils, or a rash", "Cracks knuckles", "Dandruff", "Dirty", "Distinctive jewelry", "Distracted easily during conversation",
		"Double-checks everything", "Drones on and on while talking", "Easily confused", "Enjoys own body odor", "Exaggerates", "Excessive body hair", "Fidgets"
	];
	
	var traitsTable4 = [
		"Finishes others' sentences", "Flatulent", "Flips a coin", "Foams at mouth when excited/angry", "Freckled", "Gesticulates wildly", "Giggles", "Grins evilly",
		"Hands shake", "Hacking cough", "Has nightmares", "Hates animals", "Hates children", "Hates quiet pauses in conversations", "Hiccoughs", "Hook for a hand",
		"Hums", "If unable to recall word, stops conversation and will not give up until can finally remember it", "Imaginary friend", "Interrupts others"
	];
	
	var traitsTable5 = [
		"Jumps conversation topics", "Laughs at own jokes", "Lazy eyed", "Leers", "Likes to arm wrestle", "Limps", "Loves animals", "Loves children", "Loves the sea and ships",
		"Makes up words", "Mispronounces names", "Missing finger", "Mutters", "Needs story before sleeping", "Nervous cough", "Nervous eye twitch", "Nervous muscle twitch",
		"Paces", "Peg-legged", "Perfumed"
	];
	
	var traitsTable6 = [
		"Picks fights", "Picks at fingernails", "Picks nose", "Picks scabs", "Picks at teeth", "Plays practical jokes", "Plays with hair", "Plays with own jewelry",
		"Pokes/taps others with finger", "Predilection for certain kind of food", "Prefers to be called by last name", "Puts garlic on all food",
		"Reads constantly, especially when inappropriate", "Refuses to let anyone walk behind them", "Refuses to sit in chairs", "Repeats same phrase over and over",
		"Rolls eyes when bored/annoyed", "Scratches", "Sharpens weapon", "Shivers"
	];
	
	var traitsTable7 = [
		"Sings", "Sleeps late", "Sleeps nude", "Smiles when angry/annoyed", "Sneers", "Sneezes", "Sniffles", "Spits", "Squeamish", "Stands very close", "Stares",
		"Sucks teeth", "Sun-burned", "Swears profusely", "Sweaty", "Talks about self in third-person", "Talks to inanimate objects", "Talks to self", "Talks with food in mouth",
		"Taps feet"
	];
	
	var traitsTable8 = [
		"Taps fingers", "Taunts foes", "Thinks they are very lucky", "Thinks they can speak a language they can't", "Tone-deaf", "Touches people while talking to them",
		"Turns every conversation into a story about themselves", "Unable to figure out which color clothes match", "Unable to let a joke die", "Unable to remember names",
		"Unexplained dislike for certain organization", "Urinates frequently", "Uses wrong word and refuses to acknowledge correct word", "Warts",
		"Wears flamboyant or outlandish clothes", "Wears hat or hood", "Wears only jewelry of one type of metal", "Wets bed", "Whistles"
	];
	
	var traitsTable9 = [
		"Achluophobic (afraid of darkness)", "Agoraphobic (afraid of crowds)", "Altophobic (afraid of heights)", "Claustrophobic (afraid of small spaces)", "Drools",
		"Entomophobic (afraid of insects)", "Excessively clean", "Facial tic", "Haphephobic (afraid of being touched)", "Hallucinates", "Hemaphobic (afraid of blood)",
		"Hydrophobic (afraid of water)", "Insomniac", "Narcoleptic", "Pathological liar", "Picks at lint or dirt on others' clothes", "Obsessive gambler",
		"Ophidiophobic (afraid of snakes)", "Ornithophobic (afraid of birds)", "Short attention span"
	];
	
	var locations = [
		"Head/Face", "Neck", "Chest", "Back", "Stomach", "Waist", "Right arm", "Left arm", "Right leg", "Left leg"
	];
	
	var personalityTable = [
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
	];
	
	var speech = speechTypes[Math.ceil(Math.random() * (speechTypes.length - 1))];
	var hair = hairTypes[Math.ceil(Math.random() * (hairTypes.length - 1))];
	var facialFeature = facialFeatures[Math.ceil(Math.random() * (facialFeatures.length - 1))];
	
	var characteristicRand = Math.ceil(Math.random()*20);
	var characteristic = "";
	
	if (characteristicRand >= 1 && characteristicRand <= 5) { var characteristic = traitsTable1[Math.ceil(Math.random()*(traitsTable1.length - 1))]; }
	if (characteristicRand >= 6 && characteristicRand <= 7) { var characteristic = traitsTable2[Math.ceil(Math.random()*(traitsTable2.length - 1))]; }
	if (characteristicRand >= 8 && characteristicRand <= 9) { var characteristic = traitsTable3[Math.ceil(Math.random()*(traitsTable3.length - 1))]; }
	if (characteristicRand >= 10 && characteristicRand <= 11) { var characteristic = traitsTable4[Math.ceil(Math.random()*(traitsTable4.length - 1))]; }
	if (characteristicRand >= 12 && characteristicRand <= 13) { var characteristic = traitsTable5[Math.ceil(Math.random()*(traitsTable5.length - 1))]; }
	if (characteristicRand >= 14 && characteristicRand <= 15) { var characteristic = traitsTable6[Math.ceil(Math.random()*(traitsTable6.length - 1))]; }
	if (characteristicRand >= 16 && characteristicRand <= 17) { var characteristic = traitsTable7[Math.ceil(Math.random()*(traitsTable7.length - 1))]; }
	if (characteristicRand >= 18 && characteristicRand <= 19) { var characteristic = traitsTable8[Math.ceil(Math.random()*(traitsTable8.length - 1))]; }
	if (characteristicRand == 20) { var characteristic = traitsTable9[Math.ceil(Math.random()*(traitsTable9.length - 1))]; }
	
	
	var hairColorRand = Math.ceil(Math.random() * 100);
	var hairColor = "";
	if (hairColorRand >= 1 && hairColorRand <= 12) { hairColor = "Black"; }
	if (hairColorRand >= 13 && hairColorRand <= 20) { hairColor = "Gray"; }
	if (hairColorRand >= 21 && hairColorRand <= 28) { hairColor = "Platinum"; }
	if (hairColorRand >= 29 && hairColorRand <= 36) { hairColor = "White"; }
	if (hairColorRand >= 37 && hairColorRand <= 44) { hairColor = "Dark Blonde"; }
	if (hairColorRand >= 45 && hairColorRand <= 52) { hairColor = "Blonde"; }
	if (hairColorRand >= 53 && hairColorRand <= 60) { hairColor = "Bleach Blonde"; }
	if (hairColorRand >= 61 && hairColorRand <= 68) { hairColor = "Dark Redhead"; }
	if (hairColorRand >= 69 && hairColorRand <= 76) { hairColor = "Redhead"; }
	if (hairColorRand >= 77 && hairColorRand <= 84) { hairColor = "Light Redhead"; }
	if (hairColorRand >= 85 && hairColorRand <= 92) { hairColor = "Brunette"; }
	if (hairColorRand >= 93 && hairColorRand <= 100) { hairColor = "Auburn"; }
	
	if (hair != "Bald") { hair = hair + ", " + hairColor; }
	
	
	
	var eyeColorRand = Math.ceil(Math.random() * 100);
	var eyeColor = "";
	if (eyeColorRand >= 1 && eyeColorRand <= 16) { eyeColor = "Amber"; }
	if (eyeColorRand >= 17 && eyeColorRand <= 36) { eyeColor = "Brown"; }
	if (eyeColorRand >= 37 && eyeColorRand <= 52) { eyeColor = "Hazel"; }
	if (eyeColorRand >= 53 && eyeColorRand <= 68) { eyeColor = "Green"; }
	if (eyeColorRand >= 69 && eyeColorRand <= 84) { eyeColor = "Blue"; }
	if (eyeColorRand >= 85 && eyeColorRand <= 100) { eyeColor = "Gray"; }
	
	
	var skinColorRand = Math.ceil(Math.random() * 100);
	var skinColor = "";
	if (skinColorRand >= 1 && skinColorRand <= 12) { skinColor = "Pale"; }
	if (skinColorRand >= 13 && skinColorRand <= 24) { skinColor = "Fair"; }
	if (skinColorRand >= 25 && skinColorRand <= 37) { skinColor = "Light"; }
	if (skinColorRand >= 38 && skinColorRand <= 50) { skinColor = "Light Tan"; }
	if (skinColorRand >= 51 && skinColorRand <= 63) { skinColor = "Tan"; }
	if (skinColorRand >= 64 && skinColorRand <= 76) { skinColor = "Dark Tan"; }
	if (skinColorRand >= 77 && skinColorRand <= 88) { skinColor = "Brown"; }
	if (skinColorRand >= 89 && skinColorRand <= 100) { skinColor = "Dark Brown"; }
	
	
	
	if (characteristic[characteristic.length-1] == "*") {
		var locationChar = locations[Math.ceil(Math.random() * (locations.length - 1))];
		characteristic = characteristic + " (" + locationChar + ")";
	}
	
	var majorPer1 = personalityTable[Math.ceil(Math.random()*(personalityTable.length - 1))];
	var majorPer2 = personalityTable[Math.ceil(Math.random()*(personalityTable.length - 1))];
	var minorPer1 = personalityTable[Math.ceil(Math.random()*(personalityTable.length - 1))];
	var minorPer2 = personalityTable[Math.ceil(Math.random()*(personalityTable.length - 1))];
	
	return { "hair":hair, "eyes":eyeColor, "skin":skinColor, "speech":speech, "facial":facialFeature, "characteristic":characteristic, "major1":majorPer1, "major2":majorPer2, "minor1":minorPer1, "minor2":minorPer2 };
}

function fantasyLifepath(citizen, settlement) {
	var theirName = citizen.name.split(/ +/g);
	
	var caretakerOrigins = [
		{ "n":"Original Parents", "d":"Raised by the ones that gave birth to them." }, { "n":"Original Parents", "d":"Raised by the ones that gave birth to them." },
		{ "n":"Original Parents", "d":"Raised by the ones that gave birth to them." }, { "n":"Original Parents", "d":"Raised by the ones that gave birth to them." },
		{ "n":"Close Family", "d":"Raised by family, but not their parents." }, { "n":"Close Family", "d":"Raised by family, but not their parents." },
		{ "n":"Close Family", "d":"Raised by family, but not their parents." },
		{ "n":"Adopted", "d":"Never knew original parents, raised by a couple not related to them." },
		{ "n":"Institution", "d":"Raised at an institution." },
		{ "n":"Master", "d":"Sold or given or kidnapped at an early age and raised as property." },
		{ "n":"On Their Own", "d":"Have had to rely on themselves for as long as they can remember." },
		{ "n":"On Their Own", "d":"Have had to rely on themselves for as long as they can remember." }
	];
	
	var caretakerBackgrounds = [
		"Homeless",
		"Entertainer",
		"Servant",
		"Free Laborer",
		"Monk",
		"Scholar",
		"Military",
		"Tradesman",
		"Nomad Merchant",
		"Nobility"
	];
	
	
	var caretakerStatuses = [
		{ "n":"Alive and well", "d":"Their parents or guardians are both doing well." }, { "n":"Alive and well", "d":"Their parents or guardians are both doing well." },
		{ "n":"Alive and well", "d":"Their parents or guardians are both doing well." }, { "n":"Alive and well", "d":"Their parents or guardians are both doing well." },
		{ "n":"Alive and well", "d":"Their parents or guardians are both doing well." }, { "n":"Alive and well", "d":"Their parents or guardians are both doing well." },
		{ "n":"Misfortune", "d":"One of their parents is affected by misfortune." }, { "n":"Misfortune", "d":"Both of their parents are affected by misfortune." },
		{ "n":"Misfortune", "d":"One of their parents is affected by misfortune." }, { "n":"Misfortune", "d":"Both of their parents are affected by misfortune." },
		{ "n":"Death", "d":"One of their parents is dead." }, { "n":"Death", "d":"Both of their parents are dead." }
	];
	
	var misfortunes = [
		"Cult", "Cult", "Addiction", "Addiction", "Crippled", "Crippled",
		"Cursed", "Taken", "Indentured Servant", "Bankruptcy", "Crazy", "Prison"
	];
	
	var deathTable = [
		{ "n":"Warfare", "d":"An event ranging from a raid to a siege caused death." }, { "n":"Warfare", "d":"An event ranging from a raid to a siege caused death." },
		{ "n":"Disease", "d":"Anything from a simple cold to a Pox caused death." }, { "n":"Disease", "d":"Anything from a simple cold to a Pox caused death." },
		{ "n":"Disease", "d":"Anything from a simple cold to a Pox caused death." },
		{ "n":"Accident", "d":"Any number of random events, from a mule kicking at the wrong time to a fire." },
		{ "n":"Accident", "d":"Any number of random events, from a mule kicking at the wrong time to a fire." },
		{ "n":"Murdered", "d":"From a random pick pocket to a planned assassination." },
		{ "n":"Murdered", "d":"From a random pick pocket to a planned assassination." },
		{ "n":"Unknown", "d":"Found dead under mysterious circumstances." },
		{ "n":"Unknown", "d":"Found dead under mysterious circumstances." },
		{ "n":"Murdered by " + theirName[0], "d":"Murdered by the person they were taking care of." }
	];
	
	var siblingFates = [
		{ "n":"Lost Touch", "d":"It is unknown what happened to this sibling." }, { "n":"Lost Touch", "d":"It is unknown what happened to this sibling." },
		{ "n":"Lives with Parents", "d":"This sibling is home with their parents." }, { "n":"Lives with Parents", "d":"This sibling is home with their parents." },
		{ "n":"Misfortune", "d":"This sibling has had bad luck in life." }, { "n":"Misfortune", "d":"This sibling has had bad luck in life." },
		{ "n":"Keeps in Touch", "d":"This sibling is enjoying his or her own life, apart, but keeps in touch." },
		{ "n":"Keeps in Touch", "d":"This sibling is enjoying his or her own life, apart, but keeps in touch." },
		{ "n":"They Hate " + theirName[0], "d":"This sibling despises " + theirName[0] + " for some past transgression." },
		{ "n":"They Hate " + theirName[0], "d":"This sibling despises " + theirName[0] + " for some past transgression." },
		{ "n":"Dead", "d":"This sibling has died." }, { "n":"Dead", "d":"This sibling has died." }
	];
	
	var siblings = [ ];
	var charges = [ ];
	var events = [ ];
	var maleParent = { "n":"", "bg":"", "stat":"" };
	var femaleParent = { "n":"", "bg":"", "stat":"" };

	var family = { "maleParent":maleParent, "femaleParent":femaleParent, "siblings":siblings, "charges":charges, "origin":"", "careStat":"", "events":events, "relationship":""};
	if (citizen.lifepath == true) {
		family = citizen.family;
	} else if (citizen.lifepath == false) {
		citizen.family = family;
	}
	
	var randOrigin = caretakerOrigins[Math.ceil(Math.random() * (caretakerOrigins.length - 1))];
	
	if (citizen.family.origin == "") {
		citizen.family.origin = randOrigin.n + " (" + randOrigin.d + ")";
	}
	
	
	if (randOrigin.n != "Institution" && randOrigin.n != "Master" && randOrigin.n != "On Their Own") {
		var newStatus = caretakerStatuses[Math.ceil(Math.random() * (caretakerStatuses.length - 1))];
		var randParentStatus = -1;
		if (newStatus.n == "Misfortune" && newStatus.d.includes("One")) { randParentStatus = Math.ceil(Math.random() * 2); }
		if (newStatus.n == "Death" && newStatus.d.includes("One")) { randParentStatus = Math.ceil(Math.random() * 2); }
		
		var bothStatus = "";
		if (newStatus.n == "Misfortune" && newStatus.d.includes("Both")) { 
			var randMisfortune = misfortunes[Math.ceil(Math.random() * (misfortunes.length - 1))];
			bothStatus = newStatus.n + ": " + randMisfortune;
		}
		
		if (newStatus.n == "Death" && newStatus.d.includes("Both")) { 
			var randDeath = deathTable[Math.ceil(Math.random() * (deathTable.length - 1))];
			bothStatus = newStatus.n + ": " + randDeath.n + " (" + randDeath.d + ")";
		}
		
		else if (newStatus.n != "Death" && newStatus.n != "Misfortune") {
			bothStatus = "Alive and well.";
		}
		
		citizen.family.careStat = newStatus.n + " (" + newStatus.d + ")";
		
		if (citizen.family.maleParent.n == "") {
			var randBack = Math.ceil(Math.random() * 6) + Math.ceil(Math.random() * 6) + Math.ceil(Math.random() * 6);
			var caretakerBackground = "";
			if ([3,4].includes(randBack)) { caretakerBackground = "Homeless"; }
			if ([5].includes(randBack)) { caretakerBackground = "Servant"; }
			if ([6].includes(randBack)) { caretakerBackground = "Entertainer"; }
			if ([7].includes(randBack)) { caretakerBackground = "Monk"; }
			if ([8].includes(randBack)) { caretakerBackground = "Scholar"; }
			if ([9,10,11].includes(randBack)) { caretakerBackground = "Free Laborer"; }
			if ([13,14].includes(randBack)) { caretakerBackground = "Military"; }
			if ([15].includes(randBack)) { caretakerBackground = "Tradesman"; }
			if ([16].includes(randBack)) { caretakerBackground = "Nomad Merchant"; }
			if ([17,18].includes(randBack)) { caretakerBackground = "Nobility"; }
			
			if (["Servant","Nobility"].includes(caretakerBackground)) {
				caretakerBackground = caretakerBackground + " ( House " + settlement.nob[Math.ceil(Math.random() * (settlement.nob.length-1))].name + " )";
			}
			
			citizen.family.maleParent.bg = caretakerBackground;
			
			
			if (bothStatus == "") {
				if (newStatus.n == "Misfortune" && randParentStatus == 1) {
					var randMisfortune = misfortunes[Math.ceil(Math.random() * (misfortunes.length - 1))];
					citizen.family.maleParent.stat = "Misfortune: " + randMisfortune;
				} else if (newStatus.n == "Misfortune" && randParentStatus == 2) {
					citizen.family.maleParent.stat = "Alive and well.";
				}
				
				if (newStatus.n == "Death" && randParentStatus == 1) {
					var randDeath = deathTable[Math.ceil(Math.random() * (deathTable.length - 1))];
					citizen.family.maleParent.stat = "Dead: " + deathTable.n + " (" + deathTable.d + ")";
				} else if (newStatus.n == "Death" && randParentStatus == 2) {
					citizen.family.maleParent.stat = "Alive and well.";
				}
			} else if (bothStatus != "") {
				citizen.family.maleParent.stat = bothStatus;
			}
			
			citizen.family.maleParent.n = fantasyPersonName("male").fn + " " + theirName[1];
		}
		
		if (citizen.family.femaleParent.n == "") {
			var randBack = Math.ceil(Math.random() * 6) + Math.ceil(Math.random() * 6) + Math.ceil(Math.random() * 6);
			var caretakerBackground = "";
			if ([3,4].includes(randBack)) { caretakerBackground = "Homeless"; }
			if ([5].includes(randBack)) { caretakerBackground = "Servant"; }
			if ([6].includes(randBack)) { caretakerBackground = "Entertainer"; }
			if ([7].includes(randBack)) { caretakerBackground = "Monk"; }
			if ([8].includes(randBack)) { caretakerBackground = "Scholar"; }
			if ([9,10,11].includes(randBack)) { caretakerBackground = "Free Laborer"; }
			if ([13,14].includes(randBack)) { caretakerBackground = "Military"; }
			if ([15].includes(randBack)) { caretakerBackground = "Tradesman"; }
			if ([16].includes(randBack)) { caretakerBackground = "Nomad Merchant"; }
			if ([17,18].includes(randBack)) { caretakerBackground = "Nobility"; }
			
			if (["Servant","Nobility"].includes(caretakerBackground)) {
				caretakerBackground = caretakerBackground + " ( House " + settlement.nob[Math.ceil(Math.random() * (settlement.nob.length-1))].name + " )";
			}
			
			citizen.family.femaleParent.bg = caretakerBackground;
			
			if (bothStatus == "") {
				if (newStatus.n == "Misfortune" && randParentStatus == 2) {
					var randMisfortune = misfortunes[Math.ceil(Math.random() * (misfortunes.length - 1))];
					citizen.family.femaleParent.stat = "Misfortune: " + randMisfortune.n;
				} else if (newStatus.n == "Misfortune" && randParentStatus == 1) {
					citizen.family.femaleParent.stat = "Alive and well.";
				}
				
				if (newStatus.n == "Death" && randParentStatus == 2) {
					var randDeath = deathTable[Math.ceil(Math.random() * (deathTable.length - 1))];
					citizen.family.femaleParent.stat = "Dead: " + deathTable.n + " (" + deathTable.d + ")";
				} else if (newStatus.n == "Death" && randParentStatus == 1) {
					citizen.family.femaleParent.stat = "Alive and well.";
				}
			} else if (bothStatus != "") {
				citizen.family.femaleParent.stat = bothStatus;
			}
			
			citizen.family.femaleParent.n = fantasyPersonName("female").fn + " " + theirName[1];
		}
	}
	
	
	
	var tragedies = [
		"Financial blow", "Debt", "Imprisoned", "Accident", "Addiction", "Lost a pet"
	];
	
	var windfalls = [
		"Financial boon", "Someone Owes Them", "Fame", "Long Lost Sibling", "New Pet", "Travelled to distant lands"
	];
	
	var newFriends = [
		{ "n":"Like a Big Brother or Sister to Them", "d":"Someone that is older and looks after them, fussing over them at all times." },
		{ "n":"Like a Little Brother or Sister to Them", "d":"Someone they look after as well as tease." },
		{ "n":"Teacher or Mentor", "d":"A sage becomes a friend that instructs them in matters." },
		{ "n":"Partner or Co-Worker", "d":"Someone they work with became a close friend." },
		{ "n":"An Old Lover", "d":"They said 'I just want to be friends' and meant it." },
		{ "n":"An Old Enemy", "d":"Bygones became bygones and old rivalries became funny stories." },
		{ "n":"Like a Foster Parent to Them", "d":"This friend regails them with advice as well as cares for them." },
		{ "n":"Old Childhood Friend", "d":"They bumped into someone they hadn't seen in years." },
		{ "n":"Relative", "d":"A relative became a friend in addition to a relation." },
		{ "n":"Gang or Tribe", "d":"Someone they earned the friendship of a gang or tribe of people." },
		{ "n":"Creature with Animal Intelligence", "d":"They befriended a badger, horse, or other common animal." },
		{ "n":"Intelligent Creature", "d":"They befriended some kind of animal with human-like intelligence." }
	];
	
	var newEnemies = [
		"Ex-Friend", "Ex-Lover", "Relative", "Childhood Enemy", "Employer", "Employee", "Ex-Coworker",
		"Noble House", "Guild", "Law Enforcement", "The Church", "Creature with Animal Intelligence",
		"Intelligent Creature"
	];
	
	var animosityCauses = [
		{ "n":"Humiliation", "d":"Caused a loss of face or status publicly." },
		{ "n":"Rift", "d":"Caused the loss of a friend or lover." },
		{ "n":"Busted", "d":"Truly or falsely brought criminal charges against the person." },
		{ "n":"Betrayed", "d":"Left the other out to dry or outright backstabbing." },
		{ "n":"Cold Shoulder", "d":"Turned down for a job or turned down romantic advances." },
		{ "n":"Rival", "d":"Had been competing for a job or romantically and won over the other." },
		{ "n":"Foiled", "d":"Caused the failure of some plot, quest, or undertaking." },
		{ "n":"Sore loser", "d":"Defeated this person in combat or game/gamble." },
		{ "n":"Bigotry", "d":"The hatred stems from a stereotype." },
		{ "n":"Murdered", "d":"Convinced this person killed a friend/relative/lover." },
		{ "n":"Jealousy", "d":"This person's looks/life/luck/wealth bother them." },
		{ "n":"Took Advantage", "d":"Took economic advantage by scam, or physical advantage through force." },
	];
	
	var animosityIntensities = [
		{ "n":"Annoyed", "d":"It rubs them the wrong way to be around this person, but they can control it." },
		{ "n":"Annoyed", "d":"It rubs them the wrong way to be around this person, but they can control it." },
		{ "n":"Bothered", "d":"They can't restrain quips and cut downs when they are around this person." },
		{ "n":"Bothered", "d":"They can't restrain quips and cut downs when they are around this person." },
		{ "n":"Angry", "d":"Proximity to this individual leads to arguments, shouting, yelling." },
		{ "n":"Angry", "d":"Proximity to this individual leads to arguments, shouting, yelling." },
		{ "n":"Ignore", "d":"This person doesn't exist to them." },
		{ "n":"Ignore", "d":"This person doesn't exist to them." },
		{ "n":"Violent", "d":"A fight will erupt when they are around this person, if they have to start it themselves." },
		{ "n":"Hot Murder", "d":"All bets are off and so are the gloves - if the two of them are in the same room, Thunderdome rules apply." },
		{ "n":"Cold Murder", "d":"If they catch sight of this person, their head starts drawing up schemes for death." },
		{ "n":"Ruination", "d":"Whatever they have to do to bring this person down would be worth it." }
	];
	
	var whoHatesWhomTable = [
		"They hate the other person.", "They hate the other person.", "They hate the other person.", "They hate the other person.",
		"The other person hates them", "The other person hates them.", "The other person hates them.", "The other person hates them.",
		"The feelings are mutual.", "The feelings are mutual.", "The feelings are mutual.", "The feelings are mutual."
	];
	
	var notInRelationship = [
		"Got drunk at a tavern and ended up having a one night stand.",
		"Had a few casual relationships throughout the year, but none lasted more than a month.",
		"Asked out someone they'd been interested in for a while but got rejected.",
		"Asked out someone they'd been interested in for a while - and they said yes!*",
		"Went on a blind date and it was awful - for the other person.",
		"Went on a blind date and it was awful - for " + theirName[0] + ".",
		"Went on a blind date and it was awful for everyone involved.",
		"Went on a blind date and really hit it off!*",
		"Spent a long while 'soul searching' (single)",
		"Met someone new and ended up in a serious relationship with them.*"
	];
	
	var inRelationship = [
		"Cheated on their partner, who found out and broke up with them because of it.*",
		"Cheated on their partner, who never found out about the adultery.",
		"Continued seeing the person they were with into the new year.",
		"Continued seeing the person they were with into the new year.",
		"Continued seeing the person they were with into the new year.",
		"Continued seeing the person they were with into the new year.",
		"Continued seeing the person they were with into the new year.",
		"Cheated on by their partner and broke up with them because of it.*",
		"Got in a huge fight with their partner, but talked it out and stayed together.",
		"Got in a huge fight with their partner and ended up breaking up because of it.*",
		"Their partner broke up with them out of nowhere and didn't give a reason.*",
		"Proposed to their partner - who accepted! They got married later that year.",
		"Proposed to their partner - who said no. They worked it out and stayed together anyways.",
		"Proposed to their partner - who said no. " + theirName[0] + " ended up breaking things off with them shortly afterwards.*",
		"Ended things with their partner because the feelings were gone.*",
		"Was going to end things with their partner because the feelings were gone, but they worked it out and stayed together."
	];
	
	if (citizen.family.events.length == undefined || citizen.family.events.length == 0) {
		var numofEvents = Math.ceil(Math.random() * 12);
	
		for (i = 0; i < numofEvents; i++) {
			var eventLength = Math.ceil(Math.random() * 12);
			
			var typeOfEvent = Math.ceil(Math.random() * 5);
			
			if (typeOfEvent == 1 ) {
					var actualEvent = tragedies[Math.ceil(Math.random() * (tragedies.length - 1))];
					var tempEvent = "Tragedy: " + actualEvent;
					
					if (actualEvent == "Imprisoned" || actualEvent == "Accident" || actualEvent == "Addiction") {
						tempEvent = tempEvent + " for " + eventLength + " months";
					}
					
					if (actualEvent == "Debt") {
						
						var personOrHouse = Math.ceil(Math.random() * 4);
						
						if (personOrHouse != 4) {
							var newOrOld = 1;
							if (settlement.cit.length < settlement.pop) {
								newOrOld = Math.ceil(Math.random() * 4);
							}
							
							if (newOrOld != 4) {
								var newCitizen = new Citizen("Commoner");
								settlement.cit.push(newCitizen);
								
								tempEvent = tempEvent + " (Owes "+ nameInput + ")";
							} else if (newOrOld == 4) {
								var citIndex = Math.ceil(Math.random() * (settlement.cit.length - 1));
								var nameInput = settlement.cit[citIndex].name;
								if (nameInput == citizen.name && (citIndex >= 0 && citIndex < settlement.cit.length-1)) {
									nameInput = settlement.cit[citIndex+1].name;
								} else if (nameInput == citizen.name && citIndex == settlement.cit.length-1) {
									nameInput = settlement.cit[citIndex-1].name;
								}
								
								tempEvent = tempEvent + " (Owes " + nameInput + ")";
							}
						}
						
						else if (personOrHouse == 4) {
							var houseIndex = Math.ceil(Math.random() * (settlement.nob.length - 1));
							var nameInput = settlement.nob[houseIndex].name;
							
							tempEvent = tempEvent + " (Owes House " + nameInput + ", " + settlement.nob[houseIndex].nobleLevel + ")";
						}
					}
					
					tempEvent = tempEvent + "\n\n";
					
					
					citizen.family.events.push(tempEvent);
			}
			
			if (typeOfEvent == 2 ) {
					var actualEvent = windfalls[Math.ceil(Math.random() * (windfalls.length - 1))];
					var tempEvent = "Windfall: " + actualEvent;
					
					if (actualEvent == "Travelled to distant lands") {
						tempEvent = tempEvent + " for " + eventLength + " months";
					}
					
					if (actualEvent == "Someone Owes Them") {
						
						var personOrHouse = Math.ceil(Math.random() * 4);
						
						if (personOrHouse != 4) {
							var newOrOld = 1;
							if (settlement.cit.length < settlement.pop) {
								newOrOld = Math.ceil(Math.random() * 4);
							}
							
							if (newOrOld != 4) {
								var newCitizen = new Citizen("Commoner");
								settlement.cit.push(newCitizen);
								
								tempEvent = tempEvent + " (" + nameInput + ")";
							} else if (newOrOld == 4) {
								var citIndex = Math.ceil(Math.random() * (settlement.cit.length - 1));
								var nameInput = settlement.cit[citIndex].name;
								
								if (nameInput == citizen.name && (citIndex >= 0 && citIndex < settlement.cit.length-1)) {
									nameInput = settlement.cit[citIndex+1].name;
								} else if (nameInput == citizen.name && citIndex == settlement.cit.length-1) {
									nameInput = settlement.cit[citIndex-1].name;
								}
								
								tempEvent = tempEvent + " (" + nameInput + ")";
							}
						}
						else if (personOrHouse == 4) {
							var houseIndex = Math.ceil(Math.random() * (settlement.nob.length - 1));
							var nameInput = settlement.nob[houseIndex].name;
							
							tempEvent = tempEvent + " (House " + nameInput + ", " + settlement.nob[houseIndex].nobleLevel + ")";
						}
					}
					
					tempEvent = tempEvent + "\n\n";
					
					
					citizen.family.events.push(tempEvent);
			}
			
			if (typeOfEvent == 3 ) {
					var actualEvent = newFriends[Math.ceil(Math.random() * (newFriends.length - 1))];
					
					if (actualEvent != "Guild" && actualEvent != "Noble House") {
						var newName = fantasyPersonName();
						var traits = fantasyPersonCharacterTraits();
						var traitsWGender = "`Gender: " + newName.gen + "` " + traits;
						
						var nameInput = "";
						
						if (actualEvent.n == "Relative") { nameInput = newName.fn + " " + theirName[1]; }
						else if (actualEvent.n != "Relative") { nameInput = newName.fn + " " + newName.ln; }
						
						var newCitizen = { "name":nameInput, "job":"Commoner", "notes":"", "traits":traitsWGender, "lifepath":false };
						settlement.cit.push(newCitizen);
						
						var tempEvent = "New Friend: " + actualEvent.n + " (" + settlement.cit[settlement.cit.length-1].name + ")\n\n";
					}
					
					else if (actualEvent == "Noble House") {
						var randomHouse = settlement.nob[Math.ceil(Math.random() * (settlement.nob.length - 1))];
						
						var tempEvent = "New Friend: " + actualEvent.n + " (House " + randomHouse.name + ", " + randomHouse.nobleLevel + ").\nCause of Animosity: " + causeOfAnimosity.n + " (" + causeOfAnimosity.d + ")\nAnimosity Intensity: " + intensity.n + " (" + intensity.d + ")\nWho Hates Whom: " + whohates + "\n\n";
					
					}
					
					citizen.family.events.push(tempEvent);
			}
			
			if (typeOfEvent == 4 ) {
					var actualEvent = newEnemies[Math.ceil(Math.random() * (newEnemies.length - 1))];
					var causeOfAnimosity = animosityCauses[Math.ceil(Math.random() * (animosityCauses.length - 1))];
					var intensity = animosityIntensities[Math.ceil(Math.random() * (animosityIntensities.length - 1))];
					var whohates = whoHatesWhomTable[Math.ceil(Math.random() * (whoHatesWhomTable.length - 1))];
					
					if (actualEvent != "Guild" && actualEvent != "Noble House") {
						var newName = fantasyPersonName();
						var traits = fantasyPersonCharacterTraits();
						var traitsWGender = "`Gender: " + newName.gen + "` " + traits;
						
						var nameInput = "";
						
						if (actualEvent == "Relative") { nameInput = newName.fn + " " + theirName[1]; }
						else if (actualEvent != "Relative" ) { nameInput = newName.fn + " " + newName.ln; }
						
						var newCitizen = { "name":nameInput, "job":"Commoner", "notes":"", "traits":traitsWGender, "lifepath":false };
						settlement.cit.push(newCitizen);
						
						
						var tempEvent = "New Enemy: " + actualEvent + " (" + settlement.cit[settlement.cit.length-1].name + ").\nCause of Animosity: " + causeOfAnimosity.n + " (" + causeOfAnimosity.d + ")\nAnimosity Intensity: " + intensity.n + " (" + intensity.d + ")\nWho Hates Whom: " + whohates + "\n\n";
					}
					
					else if (actualEvent == "Noble House") {
						var randomHouse = settlement.nob[Math.ceil(Math.random() * (settlement.nob.length - 1))];
						
						var tempEvent = "New Enemy: " + actualEvent + " (House " + randomHouse.name + ", " + randomHouse.nobleLevel + ").\nCause of Animosity: " + causeOfAnimosity.n + " (" + causeOfAnimosity.d + ")\nAnimosity Intensity: " + intensity.n + " (" + intensity.d + ")\nWho Hates Whom: " + whohates + "\n\n";
					
					}
					
					citizen.family.events.push(tempEvent);
			}
			
			if (typeOfEvent == 5 ) {
				if (citizen.family.relationship == "") {
					var actualEvent = notInRelationship[Math.ceil(Math.random() * (notInRelationship.length - 1))];
					var tempEvent = "Relationship: " + actualEvent;
					
					if (actualEvent[actualEvent.length-1] == "*") {
				
						var newOrOld = 1;
						if (settlement.cit.length < settlement.pop) {
							newOrOld = Math.ceil(Math.random() * 4);
						}
						
						if (newOrOld != 4) {
							var newName = "";
							if (citizen.gender == "Male") {
								newName = fantasyPersonName("female");
							} else if (citizen.gender == "Female") {
								newName = fantasyPersonName("male");
							}
							
							var newCitizen = new Citizen("Commoner");
							settlement.cit.push(newCitizen);
							
							tempEvent = tempEvent + " (" + nameInput + ")";
						} else if (newOrOld == 4) {
							var citIndex = Math.ceil(Math.random() * (settlement.cit.length - 1));
							var nameInput = settlement.cit[citIndex].name;
							
							if (nameInput == citizen.name && (citIndex >= 0 && citIndex < settlement.cit.length-1)) {
								nameInput = settlement.cit[citIndex+1].name;
							} else if (nameInput == citizen.name && citIndex == settlement.cit.length-1) {
								nameInput = settlement.cit[citIndex-1].name;
							}
							
							tempEvent = tempEvent + " (" + nameInput + ")";
						}
						
						
						citizen.family.relationship = nameInput;
					}
					
					tempEvent = tempEvent + "\n\n";
					
					
					citizen.family.events.push(tempEvent);
				}
				
				if (citizen.family.relationship != "") {
					var actualEvent = inRelationship[Math.ceil(Math.random() * (inRelationship.length - 1))];
					var tempEvent = "Relationship: " + actualEvent;
					
					if (actualEvent[actualEvent.length-1] == "*") {
						tempEvent = tempEvent + " (" + citizen.family.relationship + ")";
						citizen.family.relationship = "";
					}
					
					tempEvent = tempEvent + "\n\n";
					
					
					citizen.family.events.push(tempEvent);
				}
			}
			
		}
	
	
		
	}
	
	
	
	return citizen;
}

function getGeneralLifeEvent(citizen, settlement) {
	var theirName = citizen.name.split(/ +/g);
	
	var caretakerOrigins = [
		{ "n":"Original Parents", "d":"Raised by the ones that gave birth to them." }, { "n":"Original Parents", "d":"Raised by the ones that gave birth to them." },
		{ "n":"Original Parents", "d":"Raised by the ones that gave birth to them." }, { "n":"Original Parents", "d":"Raised by the ones that gave birth to them." },
		{ "n":"Close Family", "d":"Raised by family, but not their parents." }, { "n":"Close Family", "d":"Raised by family, but not their parents." },
		{ "n":"Close Family", "d":"Raised by family, but not their parents." },
		{ "n":"Adopted", "d":"Never knew original parents, raised by a couple not related to them." },
		{ "n":"Institution", "d":"Raised at an institution." },
		{ "n":"Master", "d":"Sold or given or kidnapped at an early age and raised as property." },
		{ "n":"On Their Own", "d":"Have had to rely on themselves for as long as they can remember." },
		{ "n":"On Their Own", "d":"Have had to rely on themselves for as long as they can remember." }
	];
	
	var caretakerBackgrounds = [ "Homeless",	"Entertainer", "Servant", "Free Laborer", "Monk",	"Scholar", "Military",	"Tradesman", "Nomad Merchant", "Nobility" ];
	
	
	var caretakerStatuses = [
		{ "n":"Alive and well", "d":"Their parents or guardians are both doing well." }, { "n":"Alive and well", "d":"Their parents or guardians are both doing well." },
		{ "n":"Alive and well", "d":"Their parents or guardians are both doing well." }, { "n":"Alive and well", "d":"Their parents or guardians are both doing well." },
		{ "n":"Alive and well", "d":"Their parents or guardians are both doing well." }, { "n":"Alive and well", "d":"Their parents or guardians are both doing well." },
		{ "n":"Misfortune", "d":"One of their parents is affected by misfortune." }, { "n":"Misfortune", "d":"Both of their parents are affected by misfortune." },
		{ "n":"Misfortune", "d":"One of their parents is affected by misfortune." }, { "n":"Misfortune", "d":"Both of their parents are affected by misfortune." },
		{ "n":"Death", "d":"One of their parents is dead." }, { "n":"Death", "d":"Both of their parents are dead." }
	];
	
	var misfortunes = [ "Cult", "Cult", "Addiction", "Addiction", "Crippled", "Crippled", "Cursed", "Taken", "Indentured Servant", "Bankruptcy", "Crazy", "Prison" ];
	
	var deathTable = [
		{ "n":"Warfare", "d":"An event ranging from a raid to a siege caused death." }, { "n":"Warfare", "d":"An event ranging from a raid to a siege caused death." },
		{ "n":"Disease", "d":"Anything from a simple cold to a Pox caused death." }, { "n":"Disease", "d":"Anything from a simple cold to a Pox caused death." },
		{ "n":"Disease", "d":"Anything from a simple cold to a Pox caused death." },
		{ "n":"Accident", "d":"Any number of random events, from a mule kicking at the wrong time to a fire." },
		{ "n":"Accident", "d":"Any number of random events, from a mule kicking at the wrong time to a fire." },
		{ "n":"Murdered", "d":"From a random pick pocket to a planned assassination." },
		{ "n":"Murdered", "d":"From a random pick pocket to a planned assassination." },
		{ "n":"Unknown", "d":"Found dead under mysterious circumstances." },
		{ "n":"Unknown", "d":"Found dead under mysterious circumstances." },
		{ "n":"Murdered by " + theirName[0], "d":"Murdered by the person they were taking care of." }
	];
	
	var siblingFates = [
		{ "n":"Lost Touch", "d":"It is unknown what happened to this sibling." }, { "n":"Lost Touch", "d":"It is unknown what happened to this sibling." },
		{ "n":"Lives with Parents", "d":"This sibling is home with their parents." }, { "n":"Lives with Parents", "d":"This sibling is home with their parents." },
		{ "n":"Misfortune", "d":"This sibling has had bad luck in life." }, { "n":"Misfortune", "d":"This sibling has had bad luck in life." },
		{ "n":"Keeps in Touch", "d":"This sibling is enjoying his or her own life, apart, but keeps in touch." },
		{ "n":"Keeps in Touch", "d":"This sibling is enjoying his or her own life, apart, but keeps in touch." },
		{ "n":"They Hate " + theirName[0], "d":"This sibling despises " + theirName[0] + " for some past transgression." },
		{ "n":"They Hate " + theirName[0], "d":"This sibling despises " + theirName[0] + " for some past transgression." },
		{ "n":"Dead", "d":"This sibling has died." }, { "n":"Dead", "d":"This sibling has died." }
	];
	
	var siblings = [ ];
	var charges = [ ];
	var events = [ ];
	var maleParent = { "n":"", "bg":"", "stat":"" };
	var femaleParent = { "n":"", "bg":"", "stat":"" };

	var family = { "maleParent":maleParent, "femaleParent":femaleParent, "siblings":siblings, "charges":charges, "origin":"", "careStat":"", "events":events, "relationship":""};
	if (citizen.lifepath == true) {
		family = citizen.family;
	} else if (citizen.lifepath == false) {
		citizen.family = family;
	}
	
	var randOrigin = caretakerOrigins[Math.ceil(Math.random() * (caretakerOrigins.length - 1))];
	
	if (citizen.family.origin == "") {
		citizen.family.origin = randOrigin.n + " (" + randOrigin.d + ")";
	}
	
	
	if (randOrigin.n != "Institution" && randOrigin.n != "Master" && randOrigin.n != "On Their Own") {
		var newStatus = caretakerStatuses[Math.ceil(Math.random() * (caretakerStatuses.length - 1))];
		var randParentStatus = -1;
		if (newStatus.n == "Misfortune" && newStatus.d.includes("One")) { randParentStatus = Math.ceil(Math.random() * 2); }
		if (newStatus.n == "Death" && newStatus.d.includes("One")) { randParentStatus = Math.ceil(Math.random() * 2); }
		
		var bothStatus = "";
		if (newStatus.n == "Misfortune" && newStatus.d.includes("Both")) { 
			var randMisfortune = misfortunes[Math.ceil(Math.random() * (misfortunes.length - 1))];
			bothStatus = newStatus.n + ": " + randMisfortune;
		}
		
		if (newStatus.n == "Death" && newStatus.d.includes("Both")) { 
			var randDeath = deathTable[Math.ceil(Math.random() * (deathTable.length - 1))];
			bothStatus = newStatus.n + ": " + randDeath.n + " (" + randDeath.d + ")";
		}
		
		else if (newStatus.n != "Death" && newStatus.n != "Misfortune") {
			bothStatus = "Alive and well.";
		}
		
		citizen.family.careStat = newStatus.n + " (" + newStatus.d + ")";
		
		if (citizen.family.maleParent.n == "") {
			var randBack = Math.ceil(Math.random() * 6) + Math.ceil(Math.random() * 6) + Math.ceil(Math.random() * 6);
			var caretakerBackground = "";
			if ([3,4].includes(randBack)) { caretakerBackground = "Homeless"; }
			if ([5].includes(randBack)) { caretakerBackground = "Servant"; }
			if ([6].includes(randBack)) { caretakerBackground = "Entertainer"; }
			if ([7].includes(randBack)) { caretakerBackground = "Monk"; }
			if ([8].includes(randBack)) { caretakerBackground = "Scholar"; }
			if ([9,10,11].includes(randBack)) { caretakerBackground = "Free Laborer"; }
			if ([13,14].includes(randBack)) { caretakerBackground = "Military"; }
			if ([15].includes(randBack)) { caretakerBackground = "Tradesman"; }
			if ([16].includes(randBack)) { caretakerBackground = "Nomad Merchant"; }
			if ([17,18].includes(randBack)) { caretakerBackground = "Nobility"; }
			
			if (["Servant","Nobility"].includes(caretakerBackground)) {
				caretakerBackground = caretakerBackground + " ( House " + settlement.nob[Math.ceil(Math.random() * (settlement.nob.length-1))].name + " )";
			}
			
			citizen.family.maleParent.bg = caretakerBackground;
			
			
			if (bothStatus == "") {
				if (newStatus.n == "Misfortune" && randParentStatus == 1) {
					var randMisfortune = misfortunes[Math.ceil(Math.random() * (misfortunes.length - 1))];
					citizen.family.maleParent.stat = "Misfortune: " + randMisfortune;
				} else if (newStatus.n == "Misfortune" && randParentStatus == 2) {
					citizen.family.maleParent.stat = "Alive and well.";
				}
				
				if (newStatus.n == "Death" && randParentStatus == 1) {
					var randDeath = deathTable[Math.ceil(Math.random() * (deathTable.length - 1))];
					citizen.family.maleParent.stat = "Dead: " + deathTable.n + " (" + deathTable.d + ")";
				} else if (newStatus.n == "Death" && randParentStatus == 2) {
					citizen.family.maleParent.stat = "Alive and well.";
				}
			} else if (bothStatus != "") {
				citizen.family.maleParent.stat = bothStatus;
			}
			
			citizen.family.maleParent.n = fantasyPersonName("male").fn + " " + theirName[1];
		}
		
		if (citizen.family.femaleParent.n == "") {
			var randBack = Math.ceil(Math.random() * 6) + Math.ceil(Math.random() * 6) + Math.ceil(Math.random() * 6);
			var caretakerBackground = "";
			if ([3,4].includes(randBack)) { caretakerBackground = "Homeless"; }
			if ([5].includes(randBack)) { caretakerBackground = "Servant"; }
			if ([6].includes(randBack)) { caretakerBackground = "Entertainer"; }
			if ([7].includes(randBack)) { caretakerBackground = "Monk"; }
			if ([8].includes(randBack)) { caretakerBackground = "Scholar"; }
			if ([9,10,11].includes(randBack)) { caretakerBackground = "Free Laborer"; }
			if ([13,14].includes(randBack)) { caretakerBackground = "Military"; }
			if ([15].includes(randBack)) { caretakerBackground = "Tradesman"; }
			if ([16].includes(randBack)) { caretakerBackground = "Nomad Merchant"; }
			if ([17,18].includes(randBack)) { caretakerBackground = "Nobility"; }
			
			if (["Servant","Nobility"].includes(caretakerBackground)) {
				caretakerBackground = caretakerBackground + " ( House " + settlement.nob[Math.ceil(Math.random() * (settlement.nob.length-1))].name + " )";
			}
			
			citizen.family.femaleParent.bg = caretakerBackground;
			
			if (bothStatus == "") {
				if (newStatus.n == "Misfortune" && randParentStatus == 2) {
					var randMisfortune = misfortunes[Math.ceil(Math.random() * (misfortunes.length - 1))];
					citizen.family.femaleParent.stat = "Misfortune: " + randMisfortune.n;
				} else if (newStatus.n == "Misfortune" && randParentStatus == 1) {
					citizen.family.femaleParent.stat = "Alive and well.";
				}
				
				if (newStatus.n == "Death" && randParentStatus == 2) {
					var randDeath = deathTable[Math.ceil(Math.random() * (deathTable.length - 1))];
					citizen.family.femaleParent.stat = "Dead: " + deathTable.n + " (" + deathTable.d + ")";
				} else if (newStatus.n == "Death" && randParentStatus == 1) {
					citizen.family.femaleParent.stat = "Alive and well.";
				}
			} else if (bothStatus != "") {
				citizen.family.femaleParent.stat = bothStatus;
			}
			
			citizen.family.femaleParent.n = fantasyPersonName("female").fn + " " + theirName[1];
		}
	}
	
	
	
	var tragedies = [
		"Financial blow", "Debt", "Imprisoned", "Accident", "Addiction", "Lost a pet"
	];
	
	var windfalls = [
		"Financial boon", "Someone Owes Them", "Fame", "Long Lost Sibling", "New Pet", "Travelled to distant lands"
	];
	
	var newFriends = [
		{ "n":"Like a Big Brother or Sister to Them", "d":"Someone that is older and looks after them, fussing over them at all times." },
		{ "n":"Like a Little Brother or Sister to Them", "d":"Someone they look after as well as tease." },
		{ "n":"Teacher or Mentor", "d":"A sage becomes a friend that instructs them in matters." },
		{ "n":"Partner or Co-Worker", "d":"Someone they work with became a close friend." },
		{ "n":"An Old Lover", "d":"They said 'I just want to be friends' and meant it." },
		{ "n":"An Old Enemy", "d":"Bygones became bygones and old rivalries became funny stories." },
		{ "n":"Like a Foster Parent to Them", "d":"This friend regails them with advice as well as cares for them." },
		{ "n":"Old Childhood Friend", "d":"They bumped into someone they hadn't seen in years." },
		{ "n":"Relative", "d":"A relative became a friend in addition to a relation." },
		{ "n":"Gang or Tribe", "d":"Someone they earned the friendship of a gang or tribe of people." },
		{ "n":"Creature with Animal Intelligence", "d":"They befriended a badger, horse, or other common animal." },
		{ "n":"Intelligent Creature", "d":"They befriended some kind of animal with human-like intelligence." }
	];
	
	var newEnemies = [
		"Ex-Friend", "Ex-Lover", "Relative", "Childhood Enemy", "Employer", "Employee", "Ex-Coworker",
		"Noble House", "Guild", "Law Enforcement", "The Church", "Creature with Animal Intelligence",
		"Intelligent Creature"
	];
	
	var animosityCauses = [
		{ "n":"Humiliation", "d":"Caused a loss of face or status publicly." },
		{ "n":"Rift", "d":"Caused the loss of a friend or lover." },
		{ "n":"Busted", "d":"Truly or falsely brought criminal charges against the person." },
		{ "n":"Betrayed", "d":"Left the other out to dry or outright backstabbing." },
		{ "n":"Cold Shoulder", "d":"Turned down for a job or turned down romantic advances." },
		{ "n":"Rival", "d":"Had been competing for a job or romantically and won over the other." },
		{ "n":"Foiled", "d":"Caused the failure of some plot, quest, or undertaking." },
		{ "n":"Sore loser", "d":"Defeated this person in combat or game/gamble." },
		{ "n":"Bigotry", "d":"The hatred stems from a stereotype." },
		{ "n":"Murdered", "d":"Convinced this person killed a friend/relative/lover." },
		{ "n":"Jealousy", "d":"This person's looks/life/luck/wealth bother them." },
		{ "n":"Took Advantage", "d":"Took economic advantage by scam, or physical advantage through force." },
	];
	
	var animosityIntensities = [
		{ "n":"Annoyed", "d":"It rubs them the wrong way to be around this person, but they can control it." },
		{ "n":"Annoyed", "d":"It rubs them the wrong way to be around this person, but they can control it." },
		{ "n":"Bothered", "d":"They can't restrain quips and cut downs when they are around this person." },
		{ "n":"Bothered", "d":"They can't restrain quips and cut downs when they are around this person." },
		{ "n":"Angry", "d":"Proximity to this individual leads to arguments, shouting, yelling." },
		{ "n":"Angry", "d":"Proximity to this individual leads to arguments, shouting, yelling." },
		{ "n":"Ignore", "d":"This person doesn't exist to them." },
		{ "n":"Ignore", "d":"This person doesn't exist to them." },
		{ "n":"Violent", "d":"A fight will erupt when they are around this person, if they have to start it themselves." },
		{ "n":"Hot Murder", "d":"All bets are off and so are the gloves - if the two of them are in the same room, Thunderdome rules apply." },
		{ "n":"Cold Murder", "d":"If they catch sight of this person, their head starts drawing up schemes for death." },
		{ "n":"Ruination", "d":"Whatever they have to do to bring this person down would be worth it." }
	];
	
	var whoHatesWhomTable = [
		"They hate the other person.", "They hate the other person.", "They hate the other person.", "They hate the other person.",
		"The other person hates them", "The other person hates them.", "The other person hates them.", "The other person hates them.",
		"The feelings are mutual.", "The feelings are mutual.", "The feelings are mutual.", "The feelings are mutual."
	];
	
	
	
}

function Business (type, owner) {
	this.name = "(Unnamed)";
	this.owner = owner;
	this.type = type;
	this.desc = "";
	
	this.setName = function (newName) {
		this.name = newName;
	}
	
	this.setOwner = function (newOwner) {
		this.owner = newOwner;
	}
	
	this.setDesc = function (newDesc) {
		this.desc = newDesc;
	}
	
}

function Citizen (job) {
	this.tempName = fantasyPersonName();
	this.name = this.tempName.fn + " " + this.tempName.ln;
	
	this.gender = this.tempName.gen;
	this.tempTraits = fantasyPersonCharacterTraits();
	this.hair = this.tempTraits.hair;
	this.eyes = this.tempTraits.eyes;
	this.skin = this.tempTraits.skin;
	this.speech = this.tempTraits.speech;
	this.facial = this.tempTraits.facial;
	this.characteristic = this.tempTraits.characteristic;
	this.major1 = this.tempTraits.major1;
	this.major2 = this.tempTraits.major2;
	this.minor1 = this.tempTraits.minor1;
	this.minor2 = this.tempTraits.minor2;
	
	this.job = job;
	this.lifepath = false;
	
	this.notes = "";
	
	this.getAllTraits = function () {
		return { "gender":this.gender, "hair":this.hair, "eyes":this.eyes, "skin":this.skin, "speech":this.speech, "facial":this.facial, "characteristic":this.characteristic, "major1":this.major1, "major2":this.major2, "minor1":this.minor1, "minor2":this.minor2 };
	}
	
	this.setLastName = function (newLast) {
		this.name = this.tempName.fn + " " + newLast;
	}
	
	this.getName = function () {
		return this.name;
	}
}

function fantasyCreateSettlement(placeName) {
	if (placeName == "" || placeName == null) {
		//code for random place name
	}
	
	var settlementType = (Math.ceil(Math.random()*6) + Math.ceil(Math.random()*6) + Math.ceil(Math.random()*6));
	var settlement = "";
	var popMin = 0;
	var popMax = 0;
	
	if ([3].includes(settlementType)) { settlement = "Thorp"; popMin = 1; popMax = 20; }
	if ([4, 5].includes(settlementType)) { settlement = "Hamlet"; popMin = 21; popMax = 60; }
	if ([6, 7].includes(settlementType)) { settlement = "Village"; popMin = 61; popMax = 200; }
	if ([8, 9].includes(settlementType)) { settlement = "Small Town"; popMin = 201; popMax = 2000; }
	if ([10, 11, 12].includes(settlementType)) { settlement = "Large Town"; popMin = 2001; popMax = 5000; }
	if ([13, 14].includes(settlementType)) { settlement = "Small City"; popMin = 5001; popMax = 10000; }
	if ([15, 16].includes(settlementType)) { settlement = "Large City"; popMin = 10001; popMax = 25000; }
	if ([17, 18].includes(settlementType)) { settlement = "Metropolis"; popMin = 25001; popMax = 100000; }

	
	var population = Math.ceil(Math.random()*(popMax - popMin)) + popMin;
	var nobleHouses = [ ];
	var citizens = [ ];
	var settlementBusinesses = [ ];
	
	
	var settlementObj = { "n":placeName, "t":settlement, "pop":population, "bus":settlementBusinesses, "nob":nobleHouses, "cit":citizens};
	
	var businesses = [
		{ "t":"Shoemaker", "v":150}, { "t":"Furrier", "v":250}, { "t":"Maidservant", "v":250}, { "t":"Tailor", "v":250}, { "t":"Barber", "v":350}, { "t":"Jeweler", "v":400}, 
		{ "t":"Tavern", "v":400}, { "t":"Clothier", "v":400}, { "t":"Pastrycook", "v":500}, { "t":"Mason", "v":500}, { "t":"Carpenter", "v":550}, { "t":"Weaver", "v":600}, 
		{ "t":"Chandler", "v":700}, { "t":"Mercer", "v":700}, { "t":"Cooper", "v":700}, { "t":"Baker", "v":800}, { "t":"Watercarrier", "v":850}, { "t":"Scabbardmaker", "v":850}, 
		{ "t":"Wine-Seller", "v":900}, { "t":"Hatmaker", "v":950}, { "t":"Saddler", "v":1000}, { "t":"Chicken Butcher", "v":1000}, { "t":"Pursemaker", "v":1100}, { "t":"Woodseller", "v":2400}, 
		{ "t":"Magic-Shop", "v":2800}, { "t":"Bookbinder", "v":3000}, { "t":"Butcher", "v":1200}, { "t":"Fishmonger", "v":1200}, { "t":"Beer-Seller", "v":1400}, { "t":"Buckle Maker", "v":1400}, 
		{ "t":"Plasterer", "v":1400}, { "t":"Spice Merchant", "v":1400}, { "t":"Blacksmith", "v":1500}, { "t":"Painter", "v":1500}, { "t":"Doctor", "v":1700}, { "t":"Roofer", "v":1800}, 
		{ "t":"Locksmith", "v":1900}, { "t":"Bather", "v":1900}, { "t":"Ropemaker", "v":1900}, { "t":"Inn", "v":2000}, { "t":"Tanner", "v":2000}, { "t":"Copyist", "v":2000}, 
		{ "t":"Sculptor", "v":2000}, { "t":"Rugmaker", "v":2000}, { "t":"Harness-Maker", "v":2000}, { "t":"Bleacher", "v":2100}, { "t":"Hay Merchant", "v":2300}, { "t":"Cutler", "v":2300},
		{ "t":"Glovemaker", "v":2400}, { "t":"Woodcarver", "v":2400}, { "t":"Bookseller", "v":6300}, { "t":"Illuminator", "v":3900}		
	];
	
	for (i = 0; i < businesses.length - 1; i++) {
		var numberOfBus = Math.floor(parseFloat(population / businesses[i].v));
		
		for (j = 0; j < numberOfBus; j++) {
			tempHolder = businesses[i];
			
			var jobTitle = tempHolder.t;
			if (jobTitle == "Tavern") { jobTitle = "Tavernkeeper"; }
			if (jobTitle == "Inn") { jobTitle = "Innkeeper"; }
			if (jobTitle == "Magic-Shop") { jobTitle = "Magic-Shopkeep"; }
			
			var newCitizen = new Citizen(jobTitle);
			settlementObj.cit.push(newCitizen);
			
			var independentShop = new Business(businesses[i].t, newCitizen);
			settlementObj.bus.push(independentShop);
		}
		
		if (population % businesses[i].v != 0) {
			var remainder = population % businesses[i].v;
			var fraction = remainder / businesses[i].v;
			
			if (Math.random() <= fraction) {
				tempHolder = businesses[i];
				
				var jobTitle = tempHolder.t;
				if (jobTitle == "Tavern") { jobTitle = "Tavernkeeper"; }
				if (jobTitle == "Inn") { jobTitle = "Innkeeper"; }
				if (jobTitle == "Magic-Shop") { jobTitle = "Magic-Shopkeep"; }
				
				var newCitizen = new Citizen(jobTitle);
				settlementObj.cit.push(newCitizen);
				
				var independentShop = new Business(businesses[i].t, newCitizen);
				settlementObj.bus.push(independentShop);
			}
		}
	}

	
	var numOfNobleHouses = Math.floor(population / 200);
	for (i = 0; i < numOfNobleHouses; i++) {
		var newName = fantasyPersonName();
		var nobleLevel = "";
		if (i % 251 == 0 && i != 0) { nobleLevel = "Royalty"; } // Max of 1 Royal Family per city | Requires 50200 people before a city can have a Royal Family
		else if (i % 50 == 0 && i != 0) { nobleLevel = "Archduke"; } // Max of 10 Archdukes | Requires 10000 people before a city can have an Archduke
		else if (i % 26 == 0 && i != 0) { nobleLevel = "Duke"; } // Max of 19 Dukes | Requires 5000 people before a city can have a Duke
		else if (i % 11 == 0 && i != 0) { nobleLevel = "Count"; } // Max of 45 Counts | Requires 2000 people before a city can have a Count
		else if (i % 7 == 0 && i != 0) { nobleLevel = "Baron"; } // Max of 71 Barons | Requires 1000 people before a city can have a Baron
		else if (i % 4 == 0 && i != 0) { nobleLevel = "Baronet"; } // Max of 125 Barons | Requires 1000 people before a city can have a Baron
		else if (i % 2 == 0 && i != 0) { nobleLevel = "Knight Family"; } // Max of 100 Barons | Requires 1000 people before a city can have a Baron
		else { nobleLevel = "Merchant Family"; } // Everything else
		
		
		
		var newHouse = { "name":newName.ln, "nobleLevel":nobleLevel, "crest":"", "notes":"" };
		
		settlementObj.nob.push(newHouse);
	}
	
	
	var numOfLawyers = Math.floor(population / 650);
	for (i = 0; i < numOfLawyers; i++) {
		var newCitizen = new Citizen("Solicitor");
		settlementObj.cit.push(newCitizen);
	}
	
	
	var numOfClergy = Math.floor(population / 40);
	for (i = 0; i < numOfClergy; i++) {
		var newCitizen = new Citizen("Clergy");
		settlementObj.cit.push(newCitizen);
	}
	
	var numOfPriests = Math.floor(numOfClergy / 30);
	for (i = 0; i < numOfPriests; i++) {
		var newCitizen = new Citizen("Priest");
		settlementObj.cit.push(newCitizen);
	}
	
	
	var numOfGuards = Math.floor(population / 150);
	for (i = 0; i < numOfGuards; i++) {
		var newCitizen = new Citizen("Guard");
		settlementObj.cit.push(newCitizen);
	}
	
	
	return settlementObj;
}

  
function fantasyLoot(message, isGacha, justName) {
	var lootCategory = [
		"spice", "spice", "spice", "spice", "spice", "spice", "spice", "spice", "spice", "spice", "spice", "spice",
		"fiber", "fiber", "fiber", "fiber", "fiber", "fiber", "fiber", "fiber", "fiber", "fiber", "fiber", "fiber", 
		"otherMats", "otherMats", "otherMats", "otherMats", "otherMats", "otherMats", "otherMats", "otherMats", "otherMats", "otherMats", "otherMats", "otherMats", 
		"household", "household", "household", "household", "household", "household", "household", "household", "household", "household", "household", "household", 
		"garment", "garment", "garment", "garment", "garment", "garment", "garment", "garment", "garment", "garment", "garment", "garment", 
		"jewelry", "jewelry", "jewelry", "jewelry", "jewelry", "jewelry", "jewelry", "jewelry", "jewelry", 
		"gem", "gem", "gem", "gem", "gem", "gem", "gem", "gem", "gem", "gem",
		"container", "container", "container", "container", "container", "container", "container", "container", "container", "container", "container",
		"accoutrement", "accoutrement", "accoutrement", "accoutrement", "accoutrement", "accoutrement", "accoutrement", "accoutrement", "accoutrement", "accoutrement", "accoutrement", "accoutrement", 
		"scroll", "scroll", "scroll", "scroll", "scroll", "scroll", "scroll", "scroll", "scroll", 
		"concoction", "concoction", "concoction", "concoction", "concoction", "concoction", "concoction", "concoction", "concoction", "concoction", "concoction", "concoction", 
		"wand", "wand", "wand", "wand", "wand", "wand", "wand", "wand", 
		"staff", "staff", "staff", "staff",
		"basicMelee", "basicMelee", "basicMelee", "basicMelee", "basicMelee", "basicMelee", "basicMelee", "basicMelee", "basicMelee", "basicMelee", "basicMelee", "basicMelee", 
		"martialMelee", "martialMelee", "martialMelee", "martialMelee", "martialMelee", "martialMelee", "martialMelee", "martialMelee", "martialMelee", "martialMelee", "martialMelee", "martialMelee", 
		"basicMissile", "basicMissile", "basicMissile", "basicMissile", "basicMissile", "basicMissile", "basicMissile", "basicMissile", "basicMissile", "basicMissile", "basicMissile", "basicMissile", 
		"martialMissile", "martialMissile", "martialMissile", "martialMissile", "martialMissile", "martialMissile", "martialMissile", "martialMissile", "martialMissile", "martialMissile", "martialMissile", "martialMissile"
	];
	
	var loot1 = Math.ceil(Math.random()*(lootCategory.length-1));
	var itemTypeString = lootCategory[loot1];
	
	if (justName == true) {
		return itemTypeString;
	}
	
	if (isGacha == false) {
		generatedItem = generateFantasyItem(itemTypeString, message, false, isGacha);
	}
	
	if (isGacha == true) {
		generatedItem = generateFantasyItem(itemTypeString, message, false, isGacha);
		return generatedItem;
	}
}
  

function rollDice(amount, size) {
	var rolledAmounts = [ ];
	
	for (i = 0; i < amount; i++) {
		rolledAmounts.push(Math.ceil(Math.random() * size));
	}
	
	return rolledAmounts;
}
  
client.on("message", (message) => {
	if (message.content[0] == "!") {
		const args = message.content.slice(1).trim().split(/ +/g);
		const command = args.shift().toLowerCase();
		
		if (message.guild != null) {
			const fullID = message.guild.id + "+" + message.author.id;
			
			if (!client.fullTable.has(fullID)) {
				var newFull = { "credits":1000, "grindClock":0, "autosessions":4 };
				client.fullTable.set(fullID, newFull);
			}
			
			var newData = client.fullTable.get(fullID);
			
			/* if (command == "updateall") {						
				function updateCredits(value, key, map) {
					if (!client.fullTable.has(message.guild.id + "+" + key)) {
						var newFull = { "credits":1000, "grindClock":0, "autosessions":4 };
						client.fullTable.set(message.guild.id + "+" + key, newFull);
					}
						
					var oldData = client.creditsTable.get(key);
					var newData = client.fullTable.get(message.guild.id + "+" + key);
					newData.credits = parseInt(oldData);
					client.fullTable.set(message.guild.id + "+" + key, newData);
				}
				
				function updateGrind(value, key, map) {
					if (!client.fullTable.has(message.guild.id + "+" + key)) {
						var newFull = { "credits":1000, "grindClock":0, "autosessions":4 };
						client.fullTable.set(message.guild.id + "+" + key, newFull);
					}
						
					var oldData = client.grindClock.get(key);
					var newData = client.fullTable.get(message.guild.id + "+" + key);
					newData.grindClock = parseInt(oldData);
					client.fullTable.set(message.guild.id + "+" + key, newData);
				}
				
				function updateAutosessions(value, key, map) {
					if (!client.fullTable.has(message.guild.id + "+" + key)) {
						var newFull = { "credits":1000, "grindClock":0, "autosessions":4 };
						client.fullTable.set(message.guild.id + "+" + key, newFull);
					}
						
					var oldData = client.autoSessions.get(key);
					var newData = client.fullTable.get(message.guild.id + "+" + key);
					newData.autosessions = parseInt(oldData);
					client.fullTable.set(message.guild.id + "+" + key, newData);
				}
				
				client.creditsTable.forEach(updateCredits);
				client.grindClock.forEach(updateGrind);
				client.autoSessions.forEach(updateAutosessions);
				
				message.channel.send("Updated all data entries.");
			}
			*/
			
			
			if (command == "loot") {
				if (args[0] == "fantasy") {			
					if (args[1] == "help") {
						message.channel.send(message.author + ": spice, fiber, otherMats, household, garment, jewelry, container, accoutrement, scroll, enchantment, basicMelee, martialMelee, basicMissile, martialMissile, armor, concoction");
					}
					
					else if (args[1] == "average") { 
						if (message.guild != null) {
							if (message.member.roles.find("name", "Halliday")) {
								var totalValue = 0;
								var argArray = [ ];
								for (i = 0; i < parseInt(args[2]); i++) {
									typeString = fantasyLoot(message, false, true);
									argArray = generateFantasyItem(typeString, message, true);
									totalValue = parseInt(totalValue) + parseInt(argArray[1]);
								}
								
								message.channel.send(message.author + ", average value over " + args[2] + " attempts was " + (parseInt(totalValue)/parseInt(args[2])) + " Credits.");
							}
						}
					}
					
					else if (args[1] == "spice") { generateFantasyItem("spice", message, false); }
					
					else if (args[1] == "fiber") { generateFantasyItem("fiber", message, false); }
					
					else if (args[1] == "otherMats") { generateFantasyItem("otherMats", message, false); }
					
					else if (args[1] == "gem") { generateFantasyItem("gem", message, false); }
					
					else if (args[1] == "scroll") { generateFantasyItem("scroll", message, false); }
					
					else if (args[1] == "concoction") { generateFantasyItem("concoction", message, false); }
					
					else if (args[1] == "wand") { generateFantasyItem("wand", message, false); }
					
					else if (args[1] == "staff") { generateFantasyItem("staff", message, false); }
					
					else if (args[1] == "enchantment") { 
						var newEnchant = getEnchantment();
						
						message.channel.send(message.author + "\nEnchantment: " + newEnchant.name + " (" + newEnchant.type + ")\n" + "*$" + newEnchant.cost + "*, Reserve: " + newEnchant.reserve);
					}
					
					else if (args[1] == "household") {
						if (args[2] == "implausible") { generateFantasyItem("household", message, true); }
						else { generateFantasyItem("household", message, false); }
					}
					
					else if (args[1] == "garment") {
						if (args[2] == "implausible") { generateFantasyItem("garment", message, true); }
						else { generateFantasyItem("garment", message, false); }
					}
					
					else if (args[1] == "jewelry") {
						if (args[2] == "implausible") { generateFantasyItem("jewelry", message, true); }
						else { generateFantasyItem("jewelry", message, false); }
					}
					
					else if (args[1] == "container") {
						if (args[2] == "implausible") { generateFantasyItem("container", message, true); }
						else { generateFantasyItem("container", message, false); }
					}
					
					else if (args[1] == "accoutrement") {
						if (args[2] == "implausible") { generateFantasyItem("accoutrement", message, true); }
						else { generateFantasyItem("accoutrement", message, false); }
					}
					
					else if (args[1] == "basicMelee") {
						if (args[2] == "implausible") { generateFantasyItem("basicMelee", message, true); }
						else { generateFantasyItem("basicMelee", message, false); }
					}
					
					else if (args[1] == "martialMelee") {
						if (args[2] == "implausible") { generateFantasyItem("martialMelee", message, true); }
						else { generateFantasyItem("martialMelee", message, false); }
					}
					
					else if (args[1] == "basicMissile") {
						if (args[2] == "implausible") { generateFantasyItem("basicMissile", message, true); }
						else { generateFantasyItem("basicMissile", message, false); }
					}
					
					else if (args[1] == "martialMissile") {
						if (args[2] == "implausible") { generateFantasyItem("martialMissile", message, true); }
						else { generateFantasyItem("martialMissile", message, false); }
					}
					
					else if (args[1] == "armor") {
						if (args[2] == "implausible") { generateFantasyItem("armor", message, true); }
						else { generateFantasyItem("armor", message, false); }
					}
					
					else {
						fantasyLoot(message, false);
					}
				}
			}
			
			if (command == "gachapon" || command == "lootbox") {
				var invenArr = [ ];
				var hammerArr = [ ];
				var buildingArr = [ ];
				var tradeArr = [ ];
				var emptyInventory = { "hammerspace":hammerArr, "inventory":invenArr, "externalStorage":buildingArr, "pendingTradeWith":null, "inTradeWith":null, "tradeConfirmed":false, "listOfTradeItems":tradeArr, "creditsBeingSent":0 };
				
				if (!client.inventory.has(message.author.id)) {
					client.inventory.set(message.author.id, emptyInventory);
				}
				
				var inventoryTemp = client.inventory.get(message.author.id);
				
				
				if (inventoryTemp.inTradeWith == null) {
					if (args[0] === args[0]) {
						var loops = 1;
						if (args[0] != null) {
							loops = parseInt(args[0]);
						}
						
						for (var i = 0; i < loops; i++) {
							if (!client.fullTable.has(fullID)) {
								var newFull = { "credits":1000, "grindClock":0, "autosessions":4 };
								client.fullTable.set(fullID, newFull);
							}
							var newData = client.fullTable.get(fullID);
	
							if (parseInt(newData.credits) >= 1000) {
								newData.credits = newData.credits - 1000;
								client.fullTable.set(fullID, newData);
								
								newItem = fantasyLoot(message, true);
								newInventoryItem(newItem, message, true);
							} else {
								message.channel.send(message.author + " , playing the Fantasy Gachapon costs 1000 credits! Get some more cash and try again.");
								break;
							}
							
						}
					}
					
					else {
						message.channel.send(message.author + ", " + args[0] + " isn't a valid argument for this command.");
					}
				} else {
					message.channel.send("Finish or cancel your trade before playing the Fantasy Gachapon!");
				}
				
			}
			
			if (command == "credits") {
				if (args[0] == "bal") {
					if (message.guild != null && message.member.roles.find("name", "Credit Handler")) {
						if (message.mentions.members.first() != null) {
							if (client.fullTable.has(message.guild.id + "+" + message.mentions.members.first().id)) {
								var newData = client.fullTable.get(message.guild.id + "+" + message.mentions.members.first().id);
								message.channel.send(message.author + ", " + message.mentions.members.first() + " has " + newData.credits + " Credits!");
							}
						}
						
						
					} else {
							message.channel.send(message.author + " , you have to be in a server and have a role called 'Credit Handler' to use that command.");
					}
				}
			
				else if (args[0] == "add") {
					if (message.guild != null && message.member.roles.find("name", "Credit Handler")) {
						if (message.mentions.members.first() != null) {			
							var mentionFullID = message.guild.id + "+" + message.mentions.members.first().id;
							
							if (!client.fullTable.has(mentionFullID)) {
								var newFull = { "credits":1000, "grindClock":0, "autosessions":4 };
								client.fullTable.set(mentionFullID, newFull);
							}
							
							var newData = client.fullTable.get(mentionFullID);
							
							if (args[2] != null && parseInt(args[2]) >= 0) {
								newData.credits = newData.credits + parseInt(args[2]);
								client.fullTable.set(mentionFullID, newData);
								message.channel.send(message.author + " , " + message.mentions.members.first() + " has been given " + args[2] + " Credits.");
							}
							
							else if (args[2] != null && parseInt(args[2]) < 0) {
								message.channel.send(message.author + " , use 'remove' to remove Credits from people instead of a negative 'add'.");
							}
							
							else {
								message.channel.send(message.author + " , double check to make sure you entered the arguments correctly.");
							}
						}
					
						
					} else {
						message.channel.send(message.author + ", you have to have a role called 'Credit Handler' to use that command.");
					}
				}
			
				else if (args [0] == "remove") {
					if (message.guild != null && message.member.roles.find("name", "Credit Handler")) {
						if (message.mentions.members.first() != null) {
							var mentionFullID = message.guild.id + "+" + message.mentions.members.first().id;
							
							if (!client.fullTable.has(mentionFullID)) {
								var newFull = { "credits":1000, "grindClock":0, "autosessions":4 };
								client.fullTable.set(mentionFullID, newFull);
							}
							
							var newData = client.fullTable.get(mentionFullID);
								
							if (args[2] != null && parseInt(args[2]) >= 0) {
								if (newData.credits - parseInt(args[2]) >= 0) {
									newData.credits -= parseInt(args[2]);
									client.fullTable.set(mentionFullID, newData);
									message.channel.send(message.author + " , " + args[2] + " Credits removed from " + message.mentions.members.first());
								} else {
									message.channel.send(message.author + " , they don't have enough Credits to do that transaction.");
								}
							}
							
							else if (args[2] != null && parseInt(args[2]) < 0) {
								message.channel.send(message.author + " , use 'add' to add Credits to people instead of a negative 'remove'.");
							}
							
							else {
								message.channel.send(message.author + " , double check to make sure you entered the arguments correctly.");
							}
						}
					} else {
						message.channel.send(message.author + " , the 'remove' command is only for Credit Handlers.");
					}
				}
				
				else if (args [0] == "empty") {
					if (message.guild != null && message.member.roles.find("name", "Credit Handler")) {
						if (message.mentions.members.first() != null) {
							var mentionFullID = message.guild.id + "+" + message.mentions.members.first().id;
							if (!client.fullTable.has(mentionFullID)) {
								var newFull = { "credits":1000, "grindClock":0, "autosessions":4 };
								client.fullTable.set(mentionFullID, newFull);
							}
							
							var newData = client.fullTable.get(mentionFullID);
							newData.credits = 1000;
							client.fullTable.set(mentionFullID, newData);
							message.channel.send(message.author + " , all Credits removed from " + message.mentions.members.first());
						}
					} else {
						message.channel.send(message.author + " , the 'empty' command is only for Credit Handlers.");
					}
				}
				
				else if (args[0] == "public") {
					if (!client.fullTable.has(fullID)) {
						var newFull = { "credits":1000, "grindClock":0, "autosessions":4 };
						client.fullTable.set(fullID, newFull);
					}
					
					var newData = client.fullTable.get(fullID);
					
					message.channel.send(message.author + " currently has: " + newData.credits + " Credits!");
				}
				
				
				else {
					if (!client.fullTable.has(fullID)) {
						var newFull = { "credits":1000, "grindClock":0, "autosessions":4 };
						client.fullTable.set(fullID, newFull);
					}
					
					var newData = client.fullTable.get(fullID);
					message.author.send("You currently have: " + newData.credits + " Credits! (" + message.guild.name + ")");
				}
			}
			
			/*if (command == "resetauto") {
				var ArrKeys = client.creditsTable.keyArray();
				for(var i = 0; i < ArrKeys.length; i++) {
					if (client.autoSessions.has(ArrKeys[i]) && ArrKeys[i] != "autoSessionValue") {
						client.autoSessions.set(ArrKeys[i], 4);
						console.log("Reset: " + ArrKeys[i]);
					}
				}
			}*/
			
			if (command == "shop" || command == "store") {
				var finalMessage = "";
				if (args[0] == "prep") {
					if (!client.shopTable.has(args[1])) {
						if (message.guild != null) {
							if (message.member.roles.find("name", "Halliday")) {
								shopItems = [ ];
								emptyShop = { "name":args[1], "items":shopItems };
								client.shopTable.set(args[1], emptyShop);
							}
						}
					}
					
				}
				
				if (args[0] == "add") {
					if (client.shopTable.has(args[1])) {
						if (message.guild != null) {
							if (message.member.roles.find("name", "Halliday")) {
								var shopTemp = client.shopTable.get(args[1]);
								var joinedArgs = args.slice(2).join(" ");
								var newArgs = joinedArgs.trim().split(";");
								
								var name = newArgs[0];
								var price = newArgs[1];
								var weight = newArgs[2];
								var itemTL = newArgs[3];
								var quantity = newArgs[4];
								var pageRef = newArgs[5];
								var description = newArgs[6];
								var tags = newArgs[7];
								var foundIdentical = false;
								
								var newItem = { "name":name, "itemPrice":price, "itemWeight":weight, "TL":itemTL, "quantity":quantity, "itemDesc":description, "note":"", "pageRef":pageRef, "tags":tags, "basePrice":price };
								
								for (i = 0; i < shopTemp.items.length; i++) {
									if (shopTemp.items[i].name == newItem.name && shopTemp.items[i].TL == newItem.TL) {
										foundIdentical = true;
										message.channel.send("That item already exists in that store!");
										break;
									}
								}
								
								if (foundIdentical == false) {
									shopTemp.items.push(newItem);
								}
								
								client.shopTable.set(args[1], shopTemp);
							}
						}
					}
				}
				
				if (args[0] == "rem") {
					if (client.shopTable.has(args[1])) {
						if (message.guild != null) {
							if (message.member.roles.find("name", "Halliday")) {
								var shopTemp = client.shopTable.get(args[1]);
								
								if (shopTemp.items[parseInt(args[2])] != null) {
									console.log("Target Exterminated");
									shopTemp.items.splice(parseInt(args[2]), 1);
								}
								
								client.shopTable.set(args[1], shopTemp);
							}
						}
					}
				}
				
				if (args[0] == "fromfile") {
					if (client.shopTable.has(args[1])) {
						if (message.guild != null) {
							if (message.member.roles.find("name", "Halliday")) {
								var shopTemp = client.shopTable.get(args[1]);
								
								shopTemp.items = [ ];
								
								fs.readFile(__dirname + '/'+args[1]+'.eqp', function(err, data) {
									parser.parseString(data, function (err, result) {
									//	console.dir(result.equipment_list.equipment);
										var fullEquipmentList = result.equipment_list.equipment;
										var stringversion = fullEquipmentList[0].categories;
									//	console.log(stringversion);
										
										
										for (i = 0; i < fullEquipmentList.length; i++) {
											var itemName = fullEquipmentList[i].description[0];
											if (fullEquipmentList[i].value != null) {
												var itemPrice = fullEquipmentList[i].value[0];
											} else {
												var itemPrice = 0;
											}
											
											if (fullEquipmentList[i].weight != null) {
												var itemWeight = fullEquipmentList[i].weight[0];
											} else {
												var itemWeight = 0;
											}
											
											if (fullEquipmentList[i].tech_level != null) {
												var itemTL = fullEquipmentList[i].tech_level[0];
											} else {
												var itemTL = 0;
											}
											
											if (fullEquipmentList[i].reference != null) {
												var pageRef = fullEquipmentList[i].reference[0];
											} else {
												var pageRef = "";
											}
											
											if (fullEquipmentList[i].categories != null) {
												if (fullEquipmentList[i].categories[0].category != null) {
													var tags = "";
													for (j = 0; j < fullEquipmentList[i].categories[0].category.length; j++) {
														if (j == 0) {
															tags = fullEquipmentList[i].categories[0].category[j];
														} else {
															tags = tags + ", " + fullEquipmentList[i].categories[0].category[j];
														}
													}
												} else {
													var tags = "";
												}
											}
											
											
											var newItem = { "name":itemName, "itemPrice":itemPrice, "itemWeight":itemWeight, "TL":itemTL, "quantity":1, "itemDesc":"", "note":"", "pageRef":pageRef, "tags":tags, "basePrice":itemPrice };
											
											shopTemp.items.push(newItem);
										}
										
										var fullEquipmentList = result.equipment_list.equipment_container;
										var stringversion = fullEquipmentList[0].categories;
									//	console.log(stringversion);
										
										
										for (i = 0; i < fullEquipmentList.length; i++) {
											var itemName = fullEquipmentList[i].description[0];
											if (fullEquipmentList[i].value != null) {
												var itemPrice = fullEquipmentList[i].value[0];
											} else {
												var itemPrice = 0;
											}
											
											if (fullEquipmentList[i].weight != null) {
												var itemWeight = fullEquipmentList[i].weight[0];
											} else {
												var itemWeight = 0;
											}
											
											if (fullEquipmentList[i].tech_level != null) {
												var itemTL = fullEquipmentList[i].tech_level[0];
											} else {
												var itemTL = 0;
											}
											
											if (fullEquipmentList[i].reference != null) {
												var pageRef = fullEquipmentList[i].reference[0];
											} else {
												var pageRef = "";
											}
											
											if (fullEquipmentList[i].categories != null) {
												if (fullEquipmentList[i].categories[0].category != null) {
													var tags = "";
													for (j = 0; j < fullEquipmentList[i].categories[0].category.length; j++) {
														if (j == 0) {
															tags = fullEquipmentList[i].categories[0].category[j];
														} else {
															tags = tags + ", " + fullEquipmentList[i].categories[0].category[j];
														}
													}
												} else {
													var tags = "";
												}
											}
											
											
											var newItem = { "name":itemName, "itemPrice":itemPrice, "itemWeight":itemWeight, "TL":itemTL, "quantity":1, "itemDesc":"", "note":"", "pageRef":pageRef, "tags":tags, "basePrice":itemPrice};
											
											shopTemp.items.push(newItem);
										}
									});
									client.shopTable.set(args[1], shopTemp);
								});
								
								console.log("Built shop from file. Saved!");
								client.shopTable.set(args[1], shopTemp);
							}
						}
					}
				}
				
				if (args[0] == "info") {
					if (args[1] == null) {
						message.channel.send("Current Shops: `BasicSet`, `HighTech`, `UltraTech`\nIf you add a word or part of a word after the shop argument, you can search for any items with the entered letters in their name or tags. Example: `!shop info BasicSet tent` will show all Tents.\nIf you enter a number after the shop argument, it will instead give you the full details for the item at the listed index in that store. Example: `!shop info BasicSet 27` will display a TL9 Ballistic Helmet.");
					}
					
					if (client.shopTable.has(args[1])) {
						var shopTemp = client.shopTable.get(args[1]);
						
						finalMessage = "Shop Inventory (" + shopTemp.name + "):\n";
						
						if (args[2] == null) {
							for (i = 0; i < shopTemp.items.length; i++) {
								tempItem = shopTemp.items[i];
								finalMessage = finalMessage + "`[" + i + "]`";
								finalMessage = finalMessage + "[TL: " + tempItem.TL + "] ";
								if (tempItem.quantity != 1) {
									finalMessage = finalMessage + " " + tempItem.quantity + "x ";
								}
								finalMessage = finalMessage + tempItem.name + " (" + tempItem.itemWeight + " lbs., *$" + tempItem.itemPrice + "*)\n";
								totalvalue = totalvalue + parseInt(tempItem.itemPrice);
								totalweight = totalweight + parseInt(tempItem.itemWeight);
								
								if (finalMessage.length >= 1900) {
									message.author.send(finalMessage);
									finalMessage = "Shop Inventory (Continued):\n";
								}
							}
						} else if (+args[2] === +args[2]) {					
							if (shopTemp.items[parseInt(args[2])] != null) {
								var theItem = shopTemp.items[parseInt(args[2])];
								var finalMessage = "Full Item Details (In Inventory):\n";
								finalMessage = finalMessage + "`[" + args[2] + "]` ";
								if (parseInt(theItem.quantity) > 1) {
									finalMessage = finalMessage + parseInt(theItem.quantity) + "x ";
								}
								
								finalMessage = finalMessage + theItem.name + " [TL: " + theItem.TL + "] (" + theItem.pageRef + ")\n";
								
								if (theItem.quantity > 1) {
									finalMessage = finalMessage + theItem.itemWeight + " lbs., *$" + theItem.itemPrice + "* each,\n";
									finalMessage = finalMessage + theItem.itemWeight*theItem.quantity + " lbs., *$" + theItem.itemPrice*theItem.quantity + "* Total.\n";
								}
								
								else {
									finalMessage = finalMessage + theItem.itemWeight + " lbs., *$" + theItem.itemPrice + "*\n";
								}
								
								finalMessage = finalMessage + theItem.itemDesc + "\n";
							}
						} else {
							for (i = 0; i < shopTemp.items.length; i++) {
								tempItem = shopTemp.items[i];
								
								if (tempItem.name.toLowerCase().includes(args[2].toLowerCase()) || tempItem.tags.toLowerCase().includes(args[2].toLowerCase())) {
									finalMessage = finalMessage + "`[" + i + "]`";
									finalMessage = finalMessage + "[TL: " + tempItem.TL + "] ";
									if (tempItem.quantity != 1) {
										finalMessage = finalMessage + " " + tempItem.quantity + "x ";
									}
									finalMessage = finalMessage + tempItem.name + " (" + tempItem.itemWeight + " lbs., *$" + tempItem.itemPrice + "*)\n";
									totalvalue = totalvalue + parseInt(tempItem.itemPrice);
									totalweight = totalweight + parseInt(tempItem.itemWeight);
									
									if (finalMessage.length >= 1900) {
										message.author.send(finalMessage);
										finalMessage = "Shop Inventory (Continued):\n";
									}
								}
							}
						}
						
						message.author.send(finalMessage);
				
					}
				}
				
				
				if (args[0] == "display") {
					if (message.guild != null) {
						if (message.member.roles.find("name", "Halliday")) {
							if (args[1] == null) {
								message.channel.send("Current Shops: `BasicSet`\nIf you add a word or part of a word after the shop argument, you can search for any items with the entered letters in their name or tags. Example: `!shop info BasicSet tent` will show all Tents.\nIf you enter a number after the shop argument, it will instead give you the full details for the item at the listed index in that store. Example: `!shop info BasicSet 27` will display a TL9 Ballistic Helmet.");
							}
							
							if (client.shopTable.has(args[1])) {
								var shopTemp = client.shopTable.get(args[1]);
								
								finalMessage = "Shop Inventory (" + shopTemp.name + "):\n";
								
								if (args[2] == null) {
									for (i = 0; i < shopTemp.items.length; i++) {
										tempItem = shopTemp.items[i];
										finalMessage = finalMessage + "`[" + i + "]`";
										finalMessage = finalMessage + "[TL: " + tempItem.TL + "] ";
										if (tempItem.quantity != 1) {
											finalMessage = finalMessage + " " + tempItem.quantity + "x ";
										}
										finalMessage = finalMessage + tempItem.name + " (" + tempItem.itemWeight + " lbs., *$" + tempItem.itemPrice + "*)\n";
										totalvalue = totalvalue + parseInt(tempItem.itemPrice);
										totalweight = totalweight + parseInt(tempItem.itemWeight);
										
										if (finalMessage.length >= 1900) {
											message.channel.send(finalMessage);
											finalMessage = "Shop Inventory (Continued):\n";
										}
									}
								} else if (+args[2] === +args[2]) {					
									if (shopTemp.items[parseInt(args[2])] != null) {
										var theItem = shopTemp.items[parseInt(args[2])];
										var finalMessage = "Full Item Details (In Inventory):\n";
										finalMessage = finalMessage + "`[" + args[2] + "]` ";
										if (parseInt(theItem.quantity) > 1) {
											finalMessage = finalMessage + parseInt(theItem.quantity) + "x ";
										}
										
										finalMessage = finalMessage + theItem.name + " [TL: " + theItem.TL + "] (" + theItem.pageRef + ")\n";
										
										if (theItem.quantity > 1) {
											finalMessage = finalMessage + theItem.itemWeight + " lbs., *$" + theItem.itemPrice + "* each,\n";
											finalMessage = finalMessage + theItem.itemWeight*theItem.quantity + " lbs., *$" + theItem.itemPrice*theItem.quantity + "* Total.\n";
										}
										
										else {
											finalMessage = finalMessage + theItem.itemWeight + " lbs., *$" + theItem.itemPrice + "*\n";
										}
										
										finalMessage = finalMessage + theItem.itemDesc + "\n";
									}
								} else {
									for (i = 0; i < shopTemp.items.length; i++) {
										tempItem = shopTemp.items[i];
										
										if (tempItem.name.toLowerCase().includes(args[2].toLowerCase()) || tempItem.tags.toLowerCase().includes(args[2].toLowerCase())) {
											finalMessage = finalMessage + "`[" + i + "]`";
											finalMessage = finalMessage + "[TL: " + tempItem.TL + "] ";
											if (tempItem.quantity != 1) {
												finalMessage = finalMessage + " " + tempItem.quantity + "x ";
											}
											finalMessage = finalMessage + tempItem.name + " (" + tempItem.itemWeight + " lbs., *$" + tempItem.itemPrice + "*)\n";
											totalvalue = totalvalue + parseInt(tempItem.itemPrice);
											totalweight = totalweight + parseInt(tempItem.itemWeight);
											
											if (finalMessage.length >= 1900) {
												message.channel.send(finalMessage);
												finalMessage = "Shop Inventory (Continued):\n";
											}
										}
									}
								}
								
								message.channel.send(finalMessage);
						
							}
						}
					}
				}
			
				
				
				if (args[0] == "buy") {
					if (args[1] == null) {
						message.channel.send(message.author + ", this command is used to buy items from stores - note that there is NO confirmation when you enter the command.\nFormat: `!shop buy {storename} {item index} {quantity}`");
					}
					
					if (client.shopTable.has(args[1])) {
						var shopTemp = client.shopTable.get(args[1]);
						
						

						if (args[2] != null) {
							if (shopTemp.items[parseInt(args[2])] != null) {
								var quantityBuying = 1;
								
								if (+args[3] === +args[3]) {
									quantityBuying = parseInt(args[3]);
								}
								
								var cost = parseFloat(shopTemp.items[parseFloat(args[2])].itemPrice) * quantityBuying;
								
								if (!client.fullTable.has(fullID)) {
									var newFull = { "credits":1000, "grindClock":0, "autosessions":4 };
									client.fullTable.set(fullID, newFull);
								}
								
								var newData = client.fullTable.get(fullID);
								
								if (newData.credits >= cost) {
									newData.credits -= cost;
									client.fullTable.set(fullID, newData);
					
									var newArgs = [ shopTemp.items[parseInt(args[2])].name, shopTemp.items[parseInt(args[2])].itemPrice, shopTemp.items[parseInt(args[2])].itemWeight, shopTemp.items[parseInt(args[2])].TL, quantityBuying, shopTemp.items[parseInt(args[2])].pageRef, shopTemp.items[parseInt(args[2])].itemDesc ];
									newInventoryItem(newArgs, message, true);
									
									message.channel.send(message.author + ", Bought " + quantityBuying + "x " + shopTemp.items[parseInt(args[2])].name + " for " + cost + " Credits!");
								} else {
									message.channel.send(message.author + ", You don't have enough Credits to buy " + quantityBuying + "x " + shopTemp.items[parseInt(args[2])].name + ". That costs " + cost + "!");
								}
							} else {
								message.channel.send(message.author + ", No item at that store index!");
							}
						} else if (args[2] == null) {
							message.channel.send(message.author + ", you must add an index argument, and optionally a quantity argument, to buy an item!");
						}
					} else if (!client.shopTable.has(args[1]) && args[1] != null) {
						message.channel.send(message.author + ", you must add the name of a store to use this command! If you'd like to view the store inventory, use `!store info` instead.");
					}
				}
			}
			
			if (command == "randomitem") {
				var shopBooks = [ "BasicSet", "HighTech", "UltraTech", "DFRPG", "LowTech" ];
				var theBook = shopBooks[Math.ceil(Math.random()*(shopBooks.length-1))];
				var announceBook = client.shopTable.get(theBook);
				
				var itemIndex = Math.ceil(Math.random()*(announceBook.items.length-1));
				var tempItem = announceBook.items[itemIndex];
				
				message.channel.send(message.author + ", Random Store Item: '" + tempItem.name + "' from the `" + theBook + "` store! It only costs " + tempItem.itemPrice + " Credits, and you can buy yourself one by typing `!store buy " + theBook + " " + itemIndex + "`!");
			}
			
			if (command == "autosession") {
				if (args[0] == "set") {
					if (message.guild != null) {
						if (message.member.roles.find("name", "Halliday")) {
							if (!client.fullTable.has("autoSessionValue")) {
								client.fullTable.set("autoSessionValue", 0);
							}
							
							client.fullTable.set("autoSessionValue", args[1]);
							message.channel.send(message.author + " , autosession value set to " + args[1]);
							console.log(message.author.username + " set autosession value to " + args[1]);
						}
					}
				}
				
				else if (args[0] == "check") {
					if (!client.fullTable.has("autoSessionValue")) {
						client.fullTable.set("autoSessionValue", 0);
					}

					message.channel.send(message.author + " , you currently have " + newData.autosessions + " Session Credits. Autosessions are worth " + client.fullTable.get("autoSessionValue") + " Credits each right now.");
				}
				
				else {
					if (!client.fullTable.has("autoSessionValue")) {
						client.fullTable.set("autoSessionValue", 0);
					}
					var autoValue = parseInt(client.fullTable.get("autoSessionValue"));
					
					if (!client.fullTable.has(fullID)) {
						var newFull = { "credits":1000, "grindClock":0, "autosessions":4 };
						client.fullTable.set(fullID, newFull);
					}
					var newData = client.fullTable.get(fullID);
					
					if (parseInt(args[0]) >= 2) {
						if (parseInt(newData.autosessions)-parseInt(args[0]) >= 0) {
							newData.credits += autoValue*parseInt(args[0]);
							newData.autosessions -= parseInt(args[0]);
							client.fullTable.set(fullID, newData);
							
							message.channel.send(message.author + " , you got " + (autoValue*parseInt(args[0])) + " Credits for doing " + parseInt(args[0]) + " Autosessions!");
						}
						
						else {
							message.channel.send(message.author + " , you don't have " + args[0] + " Session Credits left!");
						}
					}
					
					else {
						if (parseInt(newData.autosessions) > 0) {
							newData.credits += autoValue;
							newData.autosessions -= 1;
							client.fullTable.set(fullID, newData);
							
							message.channel.send(message.author + " , you got " + autoValue + " Credits for doing an Autosession!");
						}
						
						else {
							message.channel.send(message.author + " , you don't have any Session Credits left!");
						}
					}
				}
			}
			
			/*
			if (command == "movejackpot") {
				console.log(client.creditsTable.get("jackpot"));
				client.fullTable.set(message.guild.id + "+jackpot", client.creditsTable.get("jackpot"));
				console.log(client.fullTable.get(message.guild.id + "+" + "jackpot"));
			} */
			
			if (command == "grind") {
					if (!client.fullTable.has(message.guild.id + "+" + "jackpot")) {
						client.fullTable.set(message.guild.id + "+" + "jackpot", 0);
					}
					
					var currentJackpot = parseInt(client.fullTable.get(message.guild.id + "+" + "jackpot"));
					
					if (args[0] == "jackpot") {
						message.channel.send(message.author + " , the current jackpot is worth: " + currentJackpot + " Credits! Every time someone grinds and doesn't get the jackpot, this goes up!");
					}
					
					else {				
						var creditsGained = (Math.ceil(Math.random()*9)*10)+Math.ceil(Math.random()*10);
						
						if (!client.fullTable.has(fullID)) {
							var newFull = { "credits":1000, "grindClock":0, "autosessions":4 };
							client.fullTable.set(fullID, newFull);
						}
						var newData = client.fullTable.get(fullID);
						
						timeSince = Date.now() - parseInt(newData.grindClock);
						timeUntil = 1800000 - timeSince;
						
						if (timeSince < 1800000) {
							if (Math.ceil((timeUntil/1000)/60) >= 1) {
								message.channel.send(message.author + " , wait " + Math.ceil((timeUntil/1000)/60) + " minutes before trying again.");
							}
							
							else {
								message.channel.send(message.author + " , wait " + Math.ceil(timeUntil/1000) + " seconds before trying again.");
							}
							
							console.log(message.author.username + " tried to grind again too soon.");
						} 
						
						else {	
							if (Math.ceil(Math.random()*100000) >= 99999) {
								message.channel.send(message.author + " hit the jackpot and found " + currentJackpot + " Credits!");
								console.log(message.author.username + " hit the jackpot for " + currentJackpot + " Credits!");
								newData.credits += currentJackpot;
								client.fullTable.set(fullID, newData);
								
								client.fullTable.set(message.guild.id + "+" + "jackpot", 0);
							}
							else {
								client.fullTable.set(message.guild.id + "+" + "jackpot", currentJackpot+creditsGained);
								
								switch(Math.floor(Math.random()*10)) {
									case 0:
										message.channel.send(message.author + " ran some errands for " + creditsGained + " Credits!");
										break;
									case 1:
										message.channel.send(message.author + " broke some pots and found " + creditsGained + " Credits!");
										break;
									case 2:
										message.channel.send(message.author + " stole candy from a baby and sold the candy for " + creditsGained + " Credits!");
										break;
									case 3:
										message.channel.send(message.author + " found " + creditsGained + " Credits on the sidewalk!");
										break;
									case 4:
										message.channel.send(message.author + " wrote a fanfic and got " + creditsGained + " Credits for it!");
										break;
									case 5:
										message.channel.send(message.author + " accepted a sponsorship for " + creditsGained + " Credits! Live Mas!");
										break;
									case 6:
										message.channel.send(message.author + " accepted a sponsorship for " + creditsGained + " Credits! They're lovin' it!");
										break;
									case 7:
										message.channel.send(message.author + " accepted a sponsorship for " + creditsGained + " Credits! Eat Fresh!");
										break;
									case 8:
										message.channel.send(message.author + " robbed a bank (but only a little bit) and got " + creditsGained + " Credits!");
										break;
									case 9:
										message.channel.send(message.author + " sold some deathsticks and got " + creditsGained + " Credits!");
										break;
									default:
										message.channel.send(message.author + " grinded and found " + creditsGained + " Credits!");
										break;
										
								}
								
								newData.credits += creditsGained;
								client.fullTable.set(fullID, newData);
							}
							
							newData.grindClock = Date.now();
							client.fullTable.set(fullID, newData);
						}
					}
				}
			
			if (command == "building") {
				if (args[0] == "workshop") {
					MaxCraftedItemValue = parseInt(args[1]);
					NumOfPeopleMoreThan1 = parseInt(args[2]);
					WorkshopTL = parseInt(args[3]);
					
					if (WorkshopTL == 0) { WorkshopTL = 0.5; }
					
					var actualCost = (MaxCraftedItemValue*10)*WorkshopTL;
					
					if (NumOfPeopleMoreThan1 > 0) {
						actualCost = actualCost*(1.5*(NumOfPeopleMoreThan1));
						message.channel.send("For a workshop with a item value cap of " + MaxCraftedItemValue + " at TL " + WorkshopTL + ", cost is " + actualCost);
					}
					
					else {
						message.channel.send("For a workshop with a item value cap of " + MaxCraftedItemValue + " at TL " + WorkshopTL + ", cost is " + actualCost);
					}
				}
				
				else if (args[0] == "wall") {
					if (args[1] == "help") {
						var finalMessage = message.author + " , how to use this command:\n";
						finalMessage = finalMessage + "Example of command usage: !building wall rubble 10 8 6\n";
						finalMessage = finalMessage + "The first argument (example 'rubble') is the material the wall ise made of. Options are 'hardEarth', 'rubble', 'thatch', 'wood', 'ashlar', 'brick' and 'concrete'.\n";
						finalMessage = finalMessage + "The follow arguments are much simpler. In order: Length of wall in feet, Height of wall in feet, thickness of wall in inches.";
					
						message.channel.send(finalMessage);
					}
					
					else {
						var wallCost = 0;
						var wallWeight = 0;
						var wallDR = 0;
						
						LengthInFeet = parseInt(args[2]);
						HeightInFeet = parseInt(args[3]);
						ThicknessInInches = parseInt(args[4]);
						
						if (args[1] == "hardEarth") { materialName = "Hard Earth"; wallCost = 0.85; wallWeight = 3.75; wallDR = 1; }
						if (args[1] == "rubble") { materialName = "Rubble"; wallCost = 3.82; wallWeight = 14; wallDR = 12; }
						if (args[1] == "thatch") { materialName = "Thatch"; wallCost = 0.74; wallWeight = 1.3; wallDR = 0.5; }
						if (args[1] == "wood") { 
							materialName = "Wood";
							wallWeight = 2.67; 
							wallDR = 1;
							if (ThicknessInInches == 1) { wallCost = 7.75; }
							if (ThicknessInInches == 2) { wallCost = 4.14; }
							if (ThicknessInInches == 3) { wallCost = 3.10; }
							if (ThicknessInInches == 4) { wallCost = 2.47; }
							if (ThicknessInInches == 5) { wallCost = 1.99; }
							if (ThicknessInInches == 6) { wallCost = 1.66; }
							if (ThicknessInInches == 7) { wallCost = 1.43; }
							if (ThicknessInInches >= 8) { wallCost = 1.26; }
						}
						if (args[1] == "ashlar") { materialName = "Ashlar"; wallCost = 11.72; wallWeight = 15.5; wallDR = 13; }
						if (args[1] == "brick") { materialName = "Brick"; wallCost = 3.34; wallWeight = 7.7; wallDR = 8; }
						if (args[1] == "concrete") { materialName = "Concrete"; wallCost = 9.98; wallWeight = 15.5; wallDR = 9; }
						
						var totalCost = Math.floor(((LengthInFeet*HeightInFeet)*wallCost)*ThicknessInInches);
						var totalWeight = ((LengthInFeet*HeightInFeet)*wallWeight)*ThicknessInInches;
						
						var finalMessage = message.author + " " + materialName + " Wall:\n";
						finalMessage = finalMessage + LengthInFeet + " ft. long, ";
						finalMessage = finalMessage + HeightInFeet + " ft. tall, ";
						finalMessage = finalMessage + ThicknessInInches + " inches thick.\n";
						finalMessage = finalMessage + "Total Cost: " + totalCost + " Credits.\n";
						finalMessage = finalMessage + "Total Weight: " + totalWeight + " pounds.\n";
						finalMessage = finalMessage + "DR: " + ThicknessInInches*wallDR + ", ";
						finalMessage = finalMessage + "HP of Wall: " + Math.floor(100*Math.cbrt(totalWeight/2000));
						
						message.channel.send(finalMessage);
					}
				}
				
				else if (args[0] == "building") {
					if (args[1] == "help") {
						var finalMessage = message.author + " , how to use this command:\n";
						finalMessage = finalMessage + "Example of command usage: !building building brick 10 500 6 0.25\n";
						finalMessage = finalMessage + "The first argument (example 'brick') is the material the building is made of. Options are 'hardEarth', 'rubble', 'thatch', 'wood', 'ashlar', 'brick' and 'concrete'.\n";
						finalMessage = finalMessage + "The follow arguments are much simpler. In order: Height of building in feet, square feet of interior, thickness of wall in inches, and finally the partition factor.\n";
						finalMessage = finalMessage + "Partition Factor starts at a minimum of 0.25, representing a box with walls and a ceiling but no interior rooms. Maximimum of 0.25+(1/average wall thickness in inches).";
					
						message.channel.send(finalMessage);
					}
					
					else {
						var wallCost = 0;
						var wallWeight = 0;
						var wallDR = 0;
						
						HeightInFeet = parseInt(args[2]);
						SquareFeet = parseInt(args[3]);
						ThicknessInInches = parseInt(args[4]);
						PartitionFactor = parseFloat(args[5]);
						
						if (args[1] == "hardEarth") { materialName = "Hard Earth"; wallCost = 0.85; wallWeight = 3.75; wallDR = 1; }
						if (args[1] == "rubble") { materialName = "Rubble"; wallCost = 3.82; wallWeight = 14; wallDR = 12; }
						if (args[1] == "thatch") { materialName = "Thatch"; wallCost = 0.74; wallWeight = 1.3; wallDR = 0.5; }
						if (args[1] == "wood") { 
							materialName = "Wood";
							wallWeight = 2.67; 
							wallDR = 1;
							if (ThicknessInInches == 1) { wallCost = 7.75; }
							if (ThicknessInInches == 2) { wallCost = 4.14; }
							if (ThicknessInInches == 3) { wallCost = 3.10; }
							if (ThicknessInInches == 4) { wallCost = 2.47; }
							if (ThicknessInInches == 5) { wallCost = 1.99; }
							if (ThicknessInInches == 6) { wallCost = 1.66; }
							if (ThicknessInInches == 7) { wallCost = 1.43; }
							if (ThicknessInInches >= 8) { wallCost = 1.26; }
						}
						if (args[1] == "ashlar") { materialName = "Ashlar"; wallCost = 11.72; wallWeight = 15.5; wallDR = 13; }
						if (args[1] == "brick") { materialName = "Brick"; wallCost = 3.34; wallWeight = 7.7; wallDR = 8; }
						if (args[1] == "concrete") { materialName = "Concrete"; wallCost = 9.98; wallWeight = 15.5; wallDR = 9; }
						
						var totalCost = Math.floor((((SquareFeet*HeightInFeet)*wallCost)*ThicknessInInches)*PartitionFactor);
						var totalWeight = (((SquareFeet*HeightInFeet)*wallWeight)*ThicknessInInches)*PartitionFactor;
						
						var finalMessage = message.author + " " + materialName + " Building:\n";
						finalMessage = finalMessage + HeightInFeet + " ft. tall, " + SquareFeet + " sq. ft. interior, " + ThicknessInInches + " inch thick walls.\n";
						finalMessage = finalMessage + "Total Cost: " + totalCost + " Credits, Total Weight: " + totalWeight + " pounds.\n";
						finalMessage = finalMessage + "DR: " + ThicknessInInches*wallDR + ", ";
						finalMessage = finalMessage + "HP of Wall: " + Math.floor(100*Math.cbrt(totalWeight/2000));
						
						message.channel.send(finalMessage);
					}
				}
			}
			
			if (command == "trade" || command == "t") {
				var invenArr = [ ];
				var hammerArr = [ ];
				var buildingArr = [ ];
				var tradeArr = [ ];
				var emptyInventory = { "hammerspace":hammerArr, "inventory":invenArr, "externalStorage":buildingArr, "pendingTradeWith":null, "inTradeWith":null, "tradeConfirmed":false, "listOfTradeItems":tradeArr, "creditsBeingSent":0 };
				
				if (!client.inventory.has(message.author.id)) {
					client.inventory.set(message.author.id, emptyInventory);
				}
				
				if (message.mentions.members.first() != null) {
					if (!client.inventory.has(message.mentions.members.first().id)) {
						client.inventory.set(message.mentions.members.first().id, emptyInventory);
					}
					
					var inventoryOtherTemp = client.inventory.get(message.mentions.members.first().id);
				}
				
				var inventoryTemp = client.inventory.get(message.author.id);
				
				if (args[0] == null) {
					if (inventoryTemp.pendingTradeWith == null && inventoryTemp.inTradeWith == null) {
						var finalMessage = message.author + " isn't currently trading with anyone.\n";
						finalMessage = finalMessage + "If you'd like to trade with someone, do `!trade @name`. Once they respond correctly, a trade will be started.\n\n";
						finalMessage = finalMessage + "Trades can be cancelled by one of the participating players at any time by doing `!trade cancel`";
						message.channel.send(finalMessage);
					}
					
					else  if (inventoryTemp.pendingTradeWith != null) {
						theBoy = client.users.get(inventoryTemp.pendingTradeWith);
						message.channel.send(message.author + " is currently trying to trade with " + theBoy.username + ".");
					}
					
					else if (inventoryTemp.inTradeWith != null) {
						theBoy = client.users.get(inventoryTemp.inTradeWith);
						var inventoryOtherTemp = client.inventory.get(theBoy.id);
						
						var finalMessage = "Trade Between " + message.author.username + " and " + theBoy.username + "\nWhat " + message.author.username + " currently has on the table:\n";
						var totalyourvalue = parseInt(inventoryTemp.creditsBeingSent);
						var totaltheyvalue = parseInt(inventoryOtherTemp.creditsBeingSent);
						
						finalMessage = finalMessage + "`Credits: " + inventoryTemp.creditsBeingSent + "`\n";
						
						for (i = 0; i < inventoryTemp.listOfTradeItems.length; i++) {
							finalMessage = finalMessage + "`[" + i + "]` ";
							theItem = inventoryTemp.listOfTradeItems[i];
							
							if (parseInt(theItem.quantity) > 1) {
								finalMessage = finalMessage + theItem.quantity + "x ";
							}
							
							finalMessage = finalMessage + theItem.name + " (" + theItem.itemWeight + " lbs., *$" + theItem.itemPrice + "*)\n";
							totalyourvalue = totalyourvalue + parseInt(theItem.itemPrice);
						}
						
						finalMessage = finalMessage + "Total value from you (including Credits): " + totalyourvalue + "\n";
						finalMessage = finalMessage + "\nWhat " + theBoy.username + " currently has on the table:\n";
						
						finalMessage = finalMessage + "`Credits: " + inventoryOtherTemp.creditsBeingSent + "`\n";
						for (i = 0; i < inventoryOtherTemp.listOfTradeItems.length; i++) {
							finalMessage = finalMessage + "`[" + i + "]` ";
							theItem = inventoryOtherTemp.listOfTradeItems[i];
							
							if (parseInt(theItem.quantity) > 1) {
								finalMessage = finalMessage + theItem.quantity + "x ";
							}
							
							finalMessage = finalMessage + theItem.name + " (" + theItem.itemWeight + " lbs., *$" + theItem.itemPrice + "*)\n";
							totaltheyvalue = totaltheyvalue + parseInt(theItem.itemPrice);
						}
						
						finalMessage = finalMessage + "Total value from you (including Credits): " + totaltheyvalue + "\n";
						
						message.channel.send(finalMessage);
						
					}
					
					else {
						message.channel.send("Unspecified Error related to Trading - this should never appear.");
					}
				}
				
				if (args[0] == "add") {
					if (inventoryTemp.inTradeWith != null) {	
						indexNum = parseInt(args[1]);
						theBoy = client.users.get(inventoryTemp.inTradeWith);
						var inventoryOtherTemp = client.inventory.get(theBoy.id);
						
						if (inventoryTemp.inventory[indexNum] != null) {
							var alreadyHasEntry = false;
							var existsIndex = 0;
							
							for (i = 0; i < inventoryTemp.listOfTradeItems.length; i++) {
								if (inventoryTemp.listOfTradeItems[i].index == indexNum) {
										alreadyHasEntry = true;
										existsIndex = i;
										break;
								}
							}
						
							
							var itemRef = inventoryTemp.inventory[indexNum];
							
							if (!alreadyHasEntry) {
								if (args[2] != null) {
									quantityNum = parseInt(args[2]);
									
									if (quantityNum < 1) { quantityNum = 1; }
								}	
								
								else if (args[2] == null) {
									quantityNum = 1;
								}
								
								var addedItem = { "name":itemRef.name, "itemWeight":itemRef.itemWeight, "itemPrice":itemRef.itemPrice, "index":indexNum, "quantity":quantityNum }
								
								if (quantityNum > 1) {
									if (quantityNum <= inventoryTemp.inventory[indexNum].quantity) {
										message.channel.send("Added " + quantityNum + "x " + addedItem.name + " to the trade.");
										inventoryTemp.listOfTradeItems.push(addedItem);
									}
									
									else {
										message.channel.send("You don't have enough " + addedItem.name + "s to add that many!");
									}
								}
								
								else {
									message.channel.send("Added " + addedItem.name + " to the trade.");
									inventoryTemp.listOfTradeItems.push(addedItem);
								}
							}
							
							else if (alreadyHasEntry) {
								if (args[2] != null) {
									quantityNum = parseInt(args[2]);
									
									if (quantityNum <= 1) { quantityNum = 1; }
								}	
								
								else if (args[2] == null) {
									quantityNum = 1;
								}
								
								if ((parseInt(inventoryTemp.listOfTradeItems[existsIndex].quantity) + parseInt(quantityNum)) <= parseInt(inventoryTemp.inventory[indexNum].quantity)) {
									if (quantityNum >= 1) {
										if (quantityNum > 1) {
											message.channel.send("Added " + quantityNum + "x more " + itemRef.name + "s to the trade.");
										}
										
										else if (quantityNum == 1) {
											message.channel.send("Added another " + itemRef.name + " to the trade.");
										}
										
										inventoryTemp.listOfTradeItems[existsIndex].quantity = parseInt(inventoryTemp.listOfTradeItems[existsIndex].quantity) + parseInt(quantityNum);
									}
									
									else if (quantityNum <= 0) {
										message.channel.send("Can't add a negative amount items!");
									}
								}
								
								else {
									message.channel.send("You don't have enough " + itemRef.name + "s to add that many more!");
								}
							}
							
							inventoryTemp.tradeConfirmed = false;
							inventoryOtherTemp.tradeConfirmed = false;
							
							
						}
						
						
						else if (inventoryTemp.inventory[indexNum] == null) {
							message.channel.send("No item at that index number in your main Inventory.");
						}
						
						client.inventory.set(message.author.id, inventoryTemp);
					}
					
					else {
						message.channel.send("Can't add items to a nonexistent trade.");
					}
				}		
				
				if (args[0] == "remove") {
					if (inventoryTemp.inTradeWith != null) {
						indexNum = parseInt(args[1]);
						theBoy = client.users.get(inventoryTemp.inTradeWith);
						var inventoryOtherTemp = client.inventory.get(theBoy.id);
						
						
						if (inventoryTemp.listOfTradeItems[indexNum] != null) {
							var itemRef = inventoryTemp.listOfTradeItems[indexNum];
							
							if (args[2] != null) {
								if (quantityNum < 1) {
									quantityNum = 1;
								}
								else {
									quantityNum = parseInt(args[2]);
								}
							}	
							
							else {
								quantityNum = 1;
							}
							
							itemRef.quantity = itemRef.quantity - quantityNum;
							
							if (itemRef.quantity <= 0) {
								inventoryTemp.listOfTradeItems.splice(indexNum, 1);
							}
							
							else {
								inventoryTemp.listOfTradeItems[indexNum] = itemRef;
							}
							
							if (quantityNum > 1) {
								message.channel.send("Removed " + quantityNum + "x " + itemRef.name + "s from the trade.");
							}
							
							else {
								message.channel.send("Removed " + itemRef.name + " from the trade.");
							}
							
							
						}
						
						inventoryTemp.tradeConfirmed = false;
						inventoryOtherTemp.tradeConfirmed = false;
						
						client.inventory.set(message.author.id, inventoryTemp);
						
					}
					
					else {
						message.channel.send("Can't remove items from a nonexistent trade.");
					}
				}
				
				if (args[0] == "confirm") {
					if (inventoryTemp.inTradeWith != null) {
						var inventoryTemp = client.inventory.get(message.author.id);
						theBoy = client.users.get(inventoryTemp.inTradeWith);
						var inventoryOtherTemp = client.inventory.get(theBoy.id);	
						var foundIdentical = false;	
						var refItem;
						var subtractNum = 0;
						
						
						if (inventoryOtherTemp.tradeConfirmed == false) {
							message.channel.send("Trade confirmed, waiting on other party to confirm.");
							inventoryTemp.tradeConfirmed = true;
							client.inventory.set(message.author.id, inventoryTemp);
						}
						
						else if (inventoryOtherTemp.tradeConfirmed == true) {
							var inventoryTemp = client.inventory.get(message.author.id);
							theBoy = client.users.get(inventoryTemp.inTradeWith);
							var inventoryOtherTemp = client.inventory.get(theBoy.id);
							
							inventoryTemp.listOfTradeItems.sort(compareByIndex);
							inventoryOtherTemp.listOfTradeItems.sort(compareByIndex);
							
							for (i = inventoryTemp.listOfTradeItems.length - 1; i >= 0; i--) {
								otherrefItem = inventoryTemp.listOfTradeItems[i];
								newQuan = parseInt(inventoryTemp.inventory[otherrefItem.index].quantity) - parseInt(otherrefItem.quantity);
								
								var prepareItem = inventoryTemp.inventory[otherrefItem.index];
								
								
								
								if (inventoryTemp.inventory[parseInt(otherrefItem.index)].name == otherrefItem.name) {
									inventoryTemp.inventory[parseInt(otherrefItem.index)].quantity = newQuan;
									
									if (parseInt(inventoryTemp.inventory[parseInt(otherrefItem.index)].quantity) <= 0) {
										inventoryTemp.inventory.splice(parseInt(otherrefItem.index), 1);
									}
								}
								
								prepareItem.quantity = parseInt(otherrefItem.quantity);
								
								for (k = 0; k < inventoryOtherTemp.inventory.length; k++) {
									if (inventoryOtherTemp.inventory[k].name == otherrefItem.name) {		
										inventoryOtherTemp.inventory[k].quantity = parseInt(inventoryOtherTemp.inventory[k].quantity) + parseInt(otherrefItem.quantity);
										foundIdentical = true;
										
										break;
									}
								}
								
								if (foundIdentical == false) {
									inventoryOtherTemp.inventory.push(prepareItem);
								}
							}
							
							foundIdentical = false;
							
							
							for (i = inventoryOtherTemp.listOfTradeItems.length - 1; i >= 0; i--) {
								otherrefItem = inventoryOtherTemp.listOfTradeItems[i];
								newQuan = parseInt(inventoryOtherTemp.inventory[otherrefItem.index].quantity) - parseInt(otherrefItem.quantity);
								
								var prepareItem = inventoryOtherTemp.inventory[otherrefItem.index];
								
								if (inventoryOtherTemp.inventory[parseInt(otherrefItem.index)].name == otherrefItem.name) {
									inventoryOtherTemp.inventory[parseInt(otherrefItem.index)].quantity = newQuan;
									
									if (parseInt(inventoryOtherTemp.inventory[parseInt(otherrefItem.index)].quantity) <= 0) {
										inventoryOtherTemp.inventory.splice(parseInt(otherrefItem.index), 1);
									}
								}
								
								prepareItem.quantity = parseInt(otherrefItem.quantity);
								
								for (k = 0; k < inventoryTemp.inventory.length; k++) {
									if (inventoryTemp.inventory[k].name == otherrefItem.name) {		
										inventoryTemp.inventory[k].quantity = parseInt(inventoryTemp.inventory[k].quantity) + parseInt(otherrefItem.quantity);
										foundIdentical = true;
										
										break;
									}
								}
								
								if (foundIdentical == false) {
									inventoryTemp.inventory.push(prepareItem);
								}
							}
							
							if (!client.fullTable.has(fullID)) {
								var newFull = { "credits":1000, "grindClock":0, "autosessions":4 };
								client.fullTable.set(fullID, newFull);
							}
							var newData = client.fullTable.get(fullID);
							
							newData.credits -= parseInt(inventoryTemp.creditsBeingSent);
							newData.credits += parseInt(inventoryOtherTemp.creditsBeingSent)
							client.fullTable.set(fullID, newData);
							
							var otherFullID = message.guild.id + "+" + theBoy.id;
							
							if (!client.fullTable.has(otherFullID)) {
								var newFull = { "credits":1000, "grindClock":0, "autosessions":4 };
								client.fullTable.set(otherFullID, newFull);
							}
							var newData2 = client.fullTable.get(otherFullID);
							
							newData2.credits -= parseInt(inventoryTemp.creditsBeingSent);
							newData2.credits += parseInt(inventoryOtherTemp.creditsBeingSent)
							client.fullTable.set(otherFullID, newData2);
							
							
							inventoryTemp.inventory.sort(compareByName);
							inventoryOtherTemp.inventory.sort(compareByName);
							
							inventoryOtherTemp.pendingTradeWith = null;
							inventoryOtherTemp.inTradeWith = null;
							inventoryOtherTemp.tradeConfirmed = false;
							inventoryOtherTemp.listOfTradeItems = [ ];
							inventoryOtherTemp.creditsBeingSent = 0;
							
							inventoryTemp.pendingTradeWith = null;
							inventoryTemp.inTradeWith = null;
							inventoryTemp.tradeConfirmed = false;
							inventoryTemp.listOfTradeItems = [ ];
							inventoryTemp.creditsBeingSent = 0;
							
							message.channel.send("Trade completed!");
							client.inventory.set(theBoy.id, inventoryOtherTemp);
							client.inventory.set(message.author.id, inventoryTemp);
						}
					}
					
					else {
						message.channel.send("Can't confirm a trade when you aren't trading!");
					}
				}
				
				if (args[0] == "credits") {
					if (inventoryTemp.inTradeWith != null) {
						theBoy = client.users.get(inventoryTemp.inTradeWith);
						var inventoryOtherTemp = client.inventory.get(theBoy.id);				
						
						if (!client.fullTable.has(fullID)) {
							var newFull = { "credits":1000, "grindClock":0, "autosessions":4 };
							client.fullTable.set(fullID, newFull);
						}
						var newData = client.fullTable.get(fullID);
						
						if (parseInt(args[1]) <= parseInt(newData.credits) && parseInt(args[1]) >= 0) {
							inventoryTemp.creditsBeingSent = parseInt(args[1]);
							message.channel.send("Offered Credits changed to " + args[1]);
							
							inventoryTemp.tradeConfirmed = false;
							inventoryOtherTemp.tradeConfirmed = false;
							
							client.inventory.set(message.author.id, inventoryTemp);
						}
						
						else if (parseInt(args[1]) > parseInt(client.creditsTable.get(message.author.id))){
							message.channel.send(message.author + " , you don't have that many Credits!");
						}
						
						else {
							message.channel.send(message.author + " , enter a positive number of Credits less than your total Credits.");
						}
					}
					
					else {
						message.channel.send("Can't add Credits to a nonexistent trade.");
					}
				}
				
				if (args[0] == "cancel") {
					if (inventoryTemp.inTradeWith != null) {
						var inventoryOtherTemp = client.inventory.get(inventoryTemp.inTradeWith);
						inventoryOtherTemp.pendingTradeWith = null;
						inventoryOtherTemp.inTradeWith = null;
						inventoryOtherTemp.tradeConfirmed = false;
						inventoryOtherTemp.listOfTradeItems = [ ];
						inventoryOtherTemp.creditsBeingSent = 0;
						client.inventory.set(inventoryTemp.inTradeWith, inventoryOtherTemp);
					}
					
					inventoryTemp.pendingTradeWith = null;
					inventoryTemp.inTradeWith = null;
					inventoryTemp.tradeConfirmed = false;
					inventoryTemp.listOfTradeItems = [ ];
					inventoryTemp.creditsBeingSent = 0;
					
					message.channel.send("Trade cancelled!");
					client.inventory.set(message.author.id, inventoryTemp);
				}
				
				else {
					if (message.mentions.members.first() != null) {
						if (!client.inventory.has(message.mentions.members.first().id)) {
							client.inventory.set(message.mentions.members.first().id, emptyInventory);
						}
						
						var inventoryOtherTemp = client.inventory.get(message.mentions.members.first().id);
					
						if (inventoryTemp.pendingTradeWith == null && inventoryTemp.inTradeWith == null) { 
							inventoryTemp.pendingTradeWith = message.mentions.members.first().id; 
							theBoy = client.users.get(inventoryTemp.pendingTradeWith);
							
							if (inventoryOtherTemp.pendingTradeWith == message.author.id) {
								message.channel.send("Trade Started between " + message.author.username + " and " + theBoy.username + "! \nAdd items with `!trade add {index in inventory}` \nRemove items with `!trade remove {index in trade}` \nSet (not add) Credits to be sent with `!trade credits {number}`");
								
								inventoryTemp.inTradeWith = inventoryTemp.pendingTradeWith;
								inventoryTemp.pendingTradeWith = null;
								
								inventoryOtherTemp.inTradeWith = inventoryOtherTemp.pendingTradeWith;
								inventoryOtherTemp.pendingTradeWith = null;
							}
							
							else {
								message.channel.send("Trade Invitation extended to " + theBoy.username + ", they must respond with `!trade {mention you}` to start the trade.");
							}
						}
						
						client.inventory.set(message.author.id, inventoryTemp);
						client.inventory.set(message.mentions.members.first().id, inventoryOtherTemp);
					}
					
				}
			}
			
			if (command == "sheet" || command == "s") {
				if (args[0] == "new") {
					fantasyPersonName();
				}
			}
			
			if (command == "leaderboard") {
				if (message.guild != null) {
				var leaderArray = [ ];
				function logElements(value, key, map) {
					var tempKey = key;
					var guildID = message.guild.id;
					var parsedName = tempKey.substr(guildID.length+1, tempKey.length);
					name = message.guild.members.get(parsedName);
					if (name != null && key.includes(message.guild.id)) {
					//	console.log(name.user.username + " = " + value);
						var newEntry = { "name":name.user.username, "index":value.credits };
						leaderArray.push(newEntry);
					}
				}
				
				client.fullTable.forEach(logElements);
				
				leaderArray.sort(leaderCompare);
				
				var finalMessage = "Credit Leaderboard for " + message.guild.name + ":\n"
				for (i = 0; i < 5; i++) {
					if (leaderArray[i] != null) {
						finalMessage = finalMessage + "`[" + (i+1) + "] " + leaderArray[i].name + ": " + leaderArray[i].index + " Credits.`\n";
					}
				}
				
				message.channel.send(finalMessage);
				} else {
					message.channel.send("That command only works in a Server!");
				}
			}
			
			if (command == "help") {
				var finalMessage = "";
				if (args[0] == null) {
					finalMessage = "Everything OASIS Bot can do. Add the name of the command below to this help command for extra info.\n";
					finalMessage = finalMessage + "-) `LW` : Explanation for what a Living World TTRPG is and how one works.\n";
					finalMessage = finalMessage + "-) `create` : Explanation of the Character Creation rules.\n";
					finalMessage = finalMessage + "-) `death` : Explanation of what happens when you die.\n";
					finalMessage = finalMessage + "-) `!credits` : View your current in-game Credits balance.\n";
					finalMessage = finalMessage + "-) `!grind` : Get 10-100 credits randomly, added to your Credit balance.\n";
					finalMessage = finalMessage + "-) `!gachapon` or `!lootbox` : Pay 500 Credits, get a random item.\n";
					finalMessage = finalMessage + "-) `!3d6` : Roll 3d6, but doesn't allow modifiers currently.\n";
					finalMessage = finalMessage + "-) `!inventory` : Does so much stuff related to inventory management.\n";
					finalMessage = finalMessage + "-) `!trade` : Trade items or Credits to another player.\n";
					finalMessage = finalMessage + "-) `!autosession` : Use a Session Credit (4/day) to adventure without a GM automatically.\n\n";
					finalMessage = finalMessage + "If you have any suggestions for additional commands, put them in #oasis-bot-suggestions!";
				}
				
				else if (args[0] == "inventory") {
					finalMessage = "Help - `Inventory`\n";
					finalMessage = finalMessage + "-) `!inventory` or `!i` : View Inventory, add `h` or `hammerspace` to view that. Add the following things for even more functionality.\n\n";
					finalMessage = finalMessage + "-) `movetohammerspace` or `mth`, then inventory item index, then optional quantity. Example `!i mth 0 10`. Move all (or specific amount) of item at index to Hammerspace.\n\n";
					finalMessage = finalMessage + "-) `movefromhammerspace` or `mfh`, then hammerspace item index, then optional quantity. Example `!i mfh 0 5`. Move all (or specific amount) of item at index from Hammerspace.\n\n";
					finalMessage = finalMessage + "-) `desc` or `desc h` followed by item index number. Gives full details for item at index.\n\n";
					finalMessage = finalMessage + "-) `sell` followed by the item index number, then the amount to sell (defaults to 1). Has no confirmation if you use it, so be careful.\n\n";
					finalMessage = finalMessage + "-) `delitem` can be used to completely delete the inventory entry at the specified index. No confirmation, gives you nothing back.\n\n";
					finalMessage = finalMessage + "-) `info` gives you the total value and weight for all items in your Inventory and in your Hammerspace.";
				}
				
				else if (args[0] == "lw" || args[0] == "LW") {
					finalMessage = "Help - `Living World`\n";
					finalMessage = finalMessage + "The basic concept behind a Living World (or West Marches) game is that there is a larger pool of players and multiple GMs, and your character can be used to play in any adventure (or 'session') run by any of the GMs!\n"
					finalMessage = finalMessage + "Beyond any progress you make carrying over to other sessions, this also means that all of the characters exist in one cohesive world and can roleplay together for fun outside of sessions.\n";
				}
				
				else if (args[0] == "create") {
					finalMessage = "Help - `Character Creation`\n";
					finalMessage = finalMessage + "Players start with a base of 25 character points and can take up to 50 points of disadvantages, plus they can lower any of their starting attributes to as low as 8 to gain points.\n\n"
					finalMessage = finalMessage + "There's no limit on individual costs when buying things with your character points, but we recommend not focusing on one area too much at the start. A healthy dose of skills that will be widely useful is an excellent choice over a highly situational advantage.\n\n";
					finalMessage = finalMessage + "Ordinarily you would buy some skills at a specific TL and only use them at that level. In this game, you buy all such skills at TL12 and can use them at any TL with no penalty. This doesn't allow players to break TL restrictions on planets by 'inventing' higher TL items, as creating items with a TL or effective TL above the current planets TL instantly moves the item into your hammerspace (virtual storage)\n\n";
					finalMessage = finalMessage + "On the subject of Hammerspace: This is a virtual storage system accessible by your character. It takes two Ready maneuvers to retrieve an item from Hammerspace, but items stored there will not have their weight counted against your Encumbrance level.";
				}
				
				else if (args[0] == "death") {
					finalMessage = "Help - `Death`\n";
					finalMessage = finalMessage + "When you die, you essentially roll a new character. There are some benefits however:\n";
					finalMessage = finalMessage + "Your character remains the same 'person' overall, they just lose their Credits, Advantages, Disadavantages, Attribute changes, Held Equipment, and Hammerspace Items.\n\n";
					finalMessage = finalMessage + "At the start of your next life, you gain additional character points equal to 10% of the total points you earned in your previous life (not counting starting points). Note down how many points this gives you - this starting boost doesn't count towards points earned for the next time you die!\n\n";
					finalMessage = finalMessage + "Now you might be wondering: What happens to your items and Credits when you die? You essentially explode into all of the loot you had in your main inventory and your hammerspace, plus all of your credits drop into the pile as well. These items remain on the ground where you died until they are disturbed by another player or the environment. Some items are kept after death - the most common will be items kept in external storage, such as a building you purchased. You also keep the building when you die, making buildings a very good investment. The exception to this rule is that all Memorabilia you possess, regardless of where it is stored, will be dropped in your loot pile at death.";
				}
				
				else if (args[0] == "credits") {
					finalMessage = "Help - `!credits`\n";
					finalMessage = finalMessage + "When used alone, sends your Credit balance in a private message to you.\n"
					finalMessage = finalMessage + "If you add public to the end (`!credits public`), your Credit balance will be displayed in the channel you used the command in.\n";
				}
				
				else if (args[0] == "grind") {
					finalMessage = "Help - `!grind`\n";
					finalMessage = finalMessage + "Generates a random amount of credits from 10 to 100 and adds them to your Credit balance.\n";
					finalMessage = finalMessage + "Because the OASIS is meant to be a VRMMO, this command represents a players ability to go and kill some monsters that are too weak to give 'experience', but still give a small amount of loot.";
				}
				
				else if (args[0] == "gachapon" || args[0] == "lootbox") {
					finalMessage = "Help - `!gachapon` or `!lootbox`\n";
					finalMessage = finalMessage + "Generates a random loot item from the tables given in Dungeon Fantasy 8 with a small few tweaks.\n";
					finalMessage = finalMessage + "The main difference is Unusual Items and Rare Artifacts can't generate. They are replaced with 'wands' which use the users FP to cast the listed spell, and 'staffs' which have a built in amount of charges that can be used to cast the spell before having to be recharged.\n";
					finalMessage = finalMessage + "In the case of both Wands and Staves, they can be activated by using the Use Magic Device skill (" + message.member.guild.channels.find('name', 'homebrew-content') + ").\n";
				}
				
				else if (args[0] == "3d6") {
					finalMessage = "Help - `!3d6`\n";
					finalMessage = finalMessage + "Generates three random numbers from 1 to 6, then displays them and their sum.\n";
					finalMessage = finalMessage + "Displays critical successes on a sum of 17 or 18, and critical failures on a total of 3-4.";
				}
				
				else if (args[0] == "autosession") {
					finalMessage = "Help - `!autosession`\n";
					finalMessage = finalMessage + "Represents a players ability to go on adventures even if a GM isn't available.\n";
					finalMessage = finalMessage + "Using the command with no arguments defaults to 1 use, each use consumes a Session Credit. You can add a number after the command to do multiple at once.\n";
					finalMessage = finalMessage + "You can also add check to the end of the command (`!autosession check`) to see how many Session Credits you have left, and how much an autosession is currently worth.\n";
				}
				
				else {
					finalMessage = "Invalid help command.";
				}
				
				message.channel.send(finalMessage);
			}
			
			if (command == "arckit") {
				if (args[0] == "job") {
					if (args[1] == "person") {				
						var personName = arckitPersonTitle();
						var want = arckitJobDesire();
						var action = arckitJobActionPerson();
						var target = arckitPersonTitle();
					
						message.channel.send(message.author + ": " + personName + " " + want + " " + action + " " + target + ".");
					}
					
					if (args[1] == "thing") {				
						var personName = arckitPersonTitle();
						var want = arckitJobDesire();
						var action = arckitJobActionItem();
						var target = arckitJobTargetItem();
						
						message.channel.send(message.author + ": " + personName + " " + want + " " + action + " " + target + ".");
					}
				}
				
				if (args[0] == "music") {
					var music = arckitMusicGenres();
					
					message.channel.send(message.author + ": " + music);
				}
				
				if (args[0] == "vendomat") {
					var vendomat = arckitVendomat();
					
					message.channel.send(message.author + ": " + vendomat.category + " VendoMat, containing: " + vendomat.inventory);
				}
				
				if (args[0] == "gang") {
					var gang = arckitStreetGang();
					
					message.channel.send(message.author + ": The " + gang.firstname + " " + gang.secondname + ". " + gang.desc1 + ", " + gang.desc2 + " " + gang.desc3 + ".\n" + gang.longdesc + ". They're currently " + gang.connection + "...");
				}
				
				if (args[0] == "building") {
					if (args[1] == "full") {
						var building = arckitDowntownBuilding();
						var interior = arckitBuildingInterior();
						
						var amountFeatures = Math.ceil(Math.random()*100);
						var amountFloors = Math.ceil(Math.random()*10);
						
						if (building == "Office Block" || building == "Hotel" || building == "Apartment Block or Hab Stack") { amountFloors = amountFloors * 2; }
						if (building == "Luxury Apartments") { amountFloors = amountFloors * 3; }
						
						var feature = arckitBuildingFeatures();
						var feature2;
						var feature3;
						
						
						if (amountFeatures >= 33) { feature2 = arckitBuildingFeatures(); }
						if (amountFeatures >= 66) { feature3 = arckitBuildingFeatures(); }
						
						var finalMessage = message.author + ": " + amountFloors + "-Story " + building + "\n"; 
						finalMessage = finalMessage + "Interior Style is primarily " + interior.state + " " + interior.style + ". Unusual Interior feature is: " + interior.unusual + ". Secret feature is: " + interior.secret + ".\n";
						finalMessage = finalMessage + feature + ".\n";
						if (feature2 != null) { finalMessage = finalMessage + feature2 + ".\n"; }
						if (feature3 != null) { finalMessage = finalMessage + feature3 + "."; }
						
						message.channel.send(finalMessage);
					}
					
					else {
						var building = arckitDowntownBuilding();
					
						message.channel.send(message.author + ": " + building);
					}
				}
				
				if (args[0] == "citizen") {
					var citizen = arckitInstaCitizen();
					
					message.channel.send(message.author + ": " + citizen);
				}
				
				if (args[0] == "personDetails") {
					var personDetails = arckitPersonDetails();
					
					message.channel.send(message.author + ": The most noticeable thing about the " + personDetails.nationality + " " + personDetails.gender + " is they're/their: (a?) " + personDetails.appearance + ". If they strike up a conversation, it's probably about: " + personDetails.conversation + ". Their general demeanour is: " + personDetails.demeanour + ".");
				}
				
				if (args[0] == "sound") {
					var sound = arckitSounds();
					
					message.channel.send(message.author + ": " + sound);
				}
				
				if (args[0] == "smell") {
					var smell = arckitSmells();
					
					message.channel.send(message.author + ": " + smell);
				}
				
				if (args[0] == "name") {
					if (args[1] == "male" || args[1] == "man") {
						var name = arckitPersonName("male");
					}
					
					if (args[1] == "female" || args[1] == "woman") {
						var name = arckitPersonName("female");
					}
					
					else {
						var name = arckitPersonName("neither");
					}
					
					message.channel.send(message.author + ": " + name.fn + " " + name.ln);
				}
			}
			
			
			
			if (command == "place") {
				if (message.guild != null) {
					if (args[0] == null || args[0] == "") {
						message.channel.send("Add a place name like `!place {name}` to use this command.");
					}
					
					else if (args[0] == "list") {
						var ind = 0;
						var finalMessage = "List of all settlements:\n";
						client.settlements.forEach(placeList);
						
						function placeList(value, key, map) {
							if (value != "blank") {
								finalMessage = finalMessage + "`[" + ind + "]` " + key + "\n";
								ind = ind + 1;
							}
						}
						
						message.channel.send(finalMessage);
						
					}
					
					else {				
						if (message.member.roles.find("name", "Mods")) {
							if ((!client.settlements.has(args[0]) || client.settlements.get(args[0]) == "blank") && args[1] == "create") {
								var newSettlement = fantasyCreateSettlement(args[0]);
								
								client.settlements.set(args[0], newSettlement);
								message.channel.send("New settlement '" + args[0] + "` created!");
							}
							
							else if ((client.settlements.has(args[0]) && client.settlements.get(args[0]) != "blank") && args[1] == "create") {
								message.channel.send("A settlement already exists with that name. Either pick a different name or delete the previous entry first.");
							}
							
							else if (client.settlements.has(args[0]) && args[1] == "destroy") {
								client.settlements.set(args[0], "blank");
								message.channel.send(args[0] + " destroyed.");
							}
						} else {
							message.channel.send("You don't have the proper permissions to create new settlements.");
						}
						
						if (!client.settlements.has(args[0]) && args[1] != "create") {
							message.channel.send("No settlement found under that name. If you're trying to create a new settlement with that name, try `!place {name} create`. Otherwise, check the list of settlements to verify the name `!place list`");
						}
						
						else if (client.settlements.has(args[0])) {
							var theSettlement = client.settlements.get(args[0]);
							
							if (args[1] == null) {
								var finalMessage = "The " + theSettlement.t + " of " + theSettlement.n + ":\n";
								finalMessage = finalMessage + theSettlement.pop + " citizens (" + theSettlement.cit.length + " specified)\n";
								finalMessage = finalMessage + theSettlement.bus.length + " businesses\n";
								finalMessage = finalMessage + theSettlement.nob.length + " Noble Houses.\n";
								
								message.channel.send(finalMessage);
							}
							
							if (args[1] == "businesses" || args[1] == "b") {
								
								if (args[2] == null) {
									var finalMessage = theSettlement.n + " Businesses:\n";
									
									for (i = 0; i < theSettlement.bus.length; i++) {
										if (i != 0 && theSettlement.bus[i].type != theSettlement.bus[i-1].type) { finalMessage = finalMessage + "\n"; }
										
										finalMessage = finalMessage + "`[" + i + "]` " + theSettlement.bus[i].name + " (" + theSettlement.bus[i].type + ")\n";
																		
										if (finalMessage.length > 1900) {
											message.channel.send(finalMessage);
											finalMessage = theSettlement.n + " Businesses (continued):\n";
										}
									}
									
									message.channel.send(finalMessage);
								}
								
								if (args[2] == "search") {
									var finalMessage = theSettlement.n + " Businesses:\n";
									var joinedArgs = args.slice(3).join(" ").toLowerCase();
									
									for (i = 0; i < theSettlement.bus.length; i++) {
										if (theSettlement.bus[i].name.toLowerCase().includes(joinedArgs) || theSettlement.bus[i].desc.toLowerCase().includes(joinedArgs) || theSettlement.bus[i].type.toLowerCase().includes(joinedArgs)) {
											if (i != 0 && theSettlement.bus[i].type != theSettlement.bus[i-1].type) { finalMessage = finalMessage + "\n"; }
											
											finalMessage = finalMessage + "`[" + i + "]` " + theSettlement.bus[i].name + " (" + theSettlement.bus[i].type + ")\n";
																			
											if (finalMessage.length > 1900) {
												message.channel.send(finalMessage);
												finalMessage = theSettlement.n + " Businesses (continued):\n";
											}
										}
									}
									
									message.channel.send(finalMessage);
								}
								
								if (parseInt(args[2]) === parseInt(args[2]) && args[2] != null) {
									var indexS = parseInt(args[2]);
									if (args[3] == null) {
										if (theSettlement.bus[indexS] != null) {
											var finalMessage = theSettlement.n + " Business:\n";
											finalMessage = finalMessage + theSettlement.bus[indexS].name + " (" + theSettlement.bus[indexS].type + ")\n";
											var ownerName = theSettlement.bus[indexS].owner.getName();
											finalMessage = finalMessage + "Owned By: " + ownerName + "\n";
											
											finalMessage = finalMessage + theSettlement.bus[indexS].desc;
											
											message.channel.send(finalMessage);
										} else {
											message.channel.send("No business at that index, double check number.");
										}
									}
									
									if (args[3] == "name") {
										if (message.member.roles.find("name", "Mods")) {
											if (theSettlement.bus[indexS] != null) {
												var joinedArgs = args.slice(4).join(" ");
												theSettlement.bus[indexS].setName(joinedArgs);
												
												message.channel.send("Set the name of the " + theSettlement.bus[indexS].type + " to " + joinedArgs);
											} else {
												message.channel.send("No business at that index, double check number.");
											}
										} else {
											message.channel.send("You don't have the proper permissions to change the names of businesses.");
										}
									}
									
									if (args[3] == "descset") {
										if (message.member.roles.find("name", "Mods")) {
											if (theSettlement.bus[indexS] != null) {
												var joinedArgs = args.slice(4).join(" ");
												theSettlement.bus[indexS].setDesc(joinedArgs);
												
												message.channel.send("Set the description of " + theSettlement.bus[indexS].name + " to `" + joinedArgs + "`");
											} else {
												message.channel.send("No business at that index, double check number.");
											}
										} else {
											message.channel.send("You don't have the proper permissions to change the description of businesses.");
										}
									}
									
									if (args[3] == "descadd") {
										if (message.member.roles.find("name", "Mods")) {
											if (theSettlement.bus[indexS] != null) {
												var joinedArgs = args.slice(4).join(" ");
												theSettlement.bus[indexS].setDesc(theSettlement.bus[indexS].desc + "\n" + joinedArgs);
												
												message.channel.send("Added `" + joinedArgs + "` to the description of " + theSettlement.bus[indexS].name);
											} else {
												message.channel.send("No business at that index, double check number.");
											}
										} else {
											message.channel.send("You don't have the proper permissions to add to the descriptions of businesses.");
										}
									}
								}
							}
							
							if (args[1] == "citizens" || args[1] == "c") {
								
								if (args[2] == null) {
									var finalMessage = theSettlement.n + " Citizens:\n";
									
									for (i = 0; i < theSettlement.cit.length; i++) {
										if (i != 0 && theSettlement.cit[i].job != theSettlement.cit[i-1].job) { finalMessage = finalMessage + "\n"; }
										
										finalMessage = finalMessage + "`[" + i + "]` " + theSettlement.cit[i].name + " (" + theSettlement.cit[i].job + ")\n";
										
										
										if (finalMessage.length > 1900) {
											message.channel.send(finalMessage);
											finalMessage = theSettlement.n + " Citizens (continued):\n";
										}
									}
									
									message.channel.send(finalMessage);
								}
								
								if (args[2] == "search") {
									var finalMessage = theSettlement.n + " Citizens:\n";
									var joinedArgs = args.slice(3).join(" ").toLowerCase();
									
									for (i = 0; i < theSettlement.cit.length; i++) {
										if (theSettlement.cit[i].name.toLowerCase().includes(joinedArgs) || theSettlement.cit[i].notes.toLowerCase().includes(joinedArgs) || theSettlement.cit[i].job.toLowerCase().includes(joinedArgs)) {
											if (i != 0 && theSettlement.cit[i].job != theSettlement.cit[i-1].job) { finalMessage = finalMessage + "\n"; }
										
											finalMessage = finalMessage + "`[" + i + "]` " + theSettlement.cit[i].name + " (" + theSettlement.cit[i].job + ")\n";
										
											
											if (finalMessage.length > 1900) {
												message.channel.send(finalMessage);
												finalMessage = theSettlement.n + " Citizens (continued):\n";
											}
										}
									}
									
									message.channel.send(finalMessage);
								}
								
								if (parseInt(args[2]) === parseInt(args[2]) && args[2] != null) {
									var indexS = parseInt(args[2]);
									if (args[3] == null) {
										if (theSettlement.cit[indexS] != null) {
											var finalMessage = theSettlement.n + " Citizen:\n";
											finalMessage = finalMessage + theSettlement.cit[indexS].name + " (" + theSettlement.cit[indexS].job + ")\n";
											
											tempTraitObj = theSettlement.cit[indexS].getAllTraits;
											
											finalMessage = finalMessage + "`Gender: " + theSettlement.cit[indexS].gender + "` || ";
											finalMessage = finalMessage + "`Hair: " + theSettlement.cit[indexS].hair + "` || ";
											finalMessage = finalMessage + "`Eyes: " + theSettlement.cit[indexS].eyes + "` || ";
											finalMessage = finalMessage + "`Skin: " + theSettlement.cit[indexS].skin + "`\n";
											finalMessage = finalMessage + "`Speech: " + theSettlement.cit[indexS].speech + "` || ";
											finalMessage = finalMessage + "`Facial Feature: " + theSettlement.cit[indexS].facial + "` || ";
											finalMessage = finalMessage + "`Characteristic: " + theSettlement.cit[indexS].characteristic + "`\n";
											finalMessage = finalMessage + "`Major Personality Traits: " + theSettlement.cit[indexS].major1 + " and " + theSettlement.cit[indexS].major2 + "`\n";
											finalMessage = finalMessage + "`Minor Personality Traits: " + theSettlement.cit[indexS].minor1 + " and " + theSettlement.cit[indexS].minor2 + "`\n\n";

											finalMessage = finalMessage + theSettlement.cit[indexS].notes;
											
											message.channel.send(finalMessage);
										} else {
											message.channel.send("No citizen at that index, double check number.");
										}
									}
									
									if (args[3] == "lifepath") {
										if (theSettlement.cit[indexS] != null) {
											if (args[4] != "reset") {
												if (theSettlement.cit[indexS].lifepath == false) {
													theSettlement.cit[indexS] = fantasyLifepath(theSettlement.cit[indexS], theSettlement);
													theSettlement.cit[indexS].lifepath = true;
													message.channel.send("Lifepath created for " + theSettlement.cit[indexS].name);
												} else {
													finalMessage = "Lifepath for " + theSettlement.cit[indexS].name + ", Citizen of " + theSettlement.n + "\n";
													finalMessage = finalMessage + "`Caretaker/s: " + theSettlement.cit[indexS].family.origin + "`\n";
													if (theSettlement.cit[indexS].family.careStat != "") {
														finalMessage = finalMessage + "`Caretaker Status: " + theSettlement.cit[indexS].family.careStat + "`\n";
														finalMessage = finalMessage + "\n`Male 'Parent': " + theSettlement.cit[indexS].family.maleParent.n + "` || `" + theSettlement.cit[indexS].family.maleParent.bg + "` || `" + theSettlement.cit[indexS].family.maleParent.stat + "`\n";
														finalMessage = finalMessage + "\n`Female 'Parent': " + theSettlement.cit[indexS].family.femaleParent.n + "` || `" + theSettlement.cit[indexS].family.femaleParent.bg + "` || `" + theSettlement.cit[indexS].family.femaleParent.stat + "`\n";
													}
													
													
													finalMessage = finalMessage + "\n`Significant Life Events:\n====================\n";
													
													for (i = 0; i < theSettlement.cit[indexS].family.events.length; i++) {
														finalMessage = finalMessage + theSettlement.cit[indexS].family.events[i];
														
														if (finalMessage.length > 1750) {
																finalMessage = finalMessage + "`";
																message.channel.send(finalMessage);
																finalMessage = "\n`Significant Life Events (Continued):\n====================\n";
														}
													}
													
													finalMessage = finalMessage + "`";
													if (finalMessage != "\n`Significant Life Events (Continued):\n====================\n`") {
														message.channel.send(finalMessage);
													}
												}
											} else {
												theSettlement.cit[indexS].lifepath = false;
												message.channel.send("Reset Lifepath for " + theSettlement.cit[indexS].name);
											}
										}
									}
									
									if (args[3] == "name") {
										if (message.member.roles.find("name", "Mods")) {
											if (theSettlement.cit[indexS] != null) {
												var joinedArgs = args.slice(4).join(" ");
												theSettlement.cit[indexS].name = joinedArgs;
												
												message.channel.send("Set the name of the " + theSettlement.cit[indexS].job + " to " + joinedArgs);
											} else {
												message.channel.send("No citizen at that index, double check number.");
											}
										} else {
											message.channel.send("You don't have the proper permissions to change the names of citizens.");
										}
									}
									
									if (args[3] == "descset") {
										if (message.member.roles.find("name", "Mods")) {
											if (theSettlement.cit[indexS] != null) {
												var joinedArgs = args.slice(4).join(" ");
												theSettlement.cit[indexS].notes = joinedArgs;
												
												message.channel.send("Set the description of " + theSettlement.cit[indexS].name + " to `" + joinedArgs + "`");
											} else {
												message.channel.send("No citizen at that index, double check number.");
											}
										} else {
											message.channel.send("You don't have the proper permissions to change the description of citizens.");
										}
									}
									
									if (args[3] == "descadd") {
										if (message.member.roles.find("name", "Mods")) {
											if (theSettlement.cit[indexS] != null) {
												var joinedArgs = args.slice(4).join(" ");
												theSettlement.cit[indexS].notes = theSettlement.cit[indexS].notes + "\n" + joinedArgs;
												
												message.channel.send("Added `" + joinedArgs + "` to the description of " + theSettlement.cit[indexS].name);
											} else {
												message.channel.send("No citizen at that index, double check number.");
											}
										} else {
											message.channel.send("You don't have the proper permissions to add to the descriptions of citizens.");
										}
									}
								}
							}
							
							if (args[1] == "noble" || args[1] == "n") {
								
								if (args[2] == null) {
									var finalMessage = theSettlement.n + " Noble Houses:\n";
									finalMessage = finalMessage + "`Merchant / Knight / Baronet / Baron / Count / Duke / Archduke / Royalty`\n";
									
									for (i = 0; i < theSettlement.nob.length; i++) {
										if (i != 0 && theSettlement.nob[i].job != theSettlement.nob[i-1].job) { finalMessage = finalMessage + "\n"; }
										
										finalMessage = finalMessage + "`[" + i + "]` House " + theSettlement.nob[i].name + " (" + theSettlement.nob[i].nobleLevel + ")\n";
										
										
										if (finalMessage.length > 1900) {
											message.channel.send(finalMessage);
											finalMessage = theSettlement.n + " Noble Houses (continued):\n";
										}
									}
									
									message.channel.send(finalMessage);
								}
								
								if (args[2] == "search") {
									var finalMessage = theSettlement.n + " Noble Houses:\n";
									var joinedArgs = args.slice(3).join(" ");
									var joinedArgs = joinedArgs.toLowerCase();
									
									for (i = 0; i < theSettlement.nob.length; i++) {
										if (theSettlement.nob[i].name.toLowerCase().includes(joinedArgs) || theSettlement.nob[i].notes.toLowerCase().includes(joinedArgs) || theSettlement.nob[i].nobleLevel.toLowerCase().includes(joinedArgs)) {
											if (i != 0 && theSettlement.nob[i].nobleLevel != theSettlement.nob[i-1].nobleLevel) { finalMessage = finalMessage + "\n"; }
										
											finalMessage = finalMessage + "`[" + i + "]` House " + theSettlement.nob[i].name + " (" + theSettlement.nob[i].nobleLevel + ")\n";
										
											
											if (finalMessage.length > 1900) {
												message.channel.send(finalMessage);
												finalMessage = theSettlement.n + " Noble Houses (continued):\n";
											}
										}
									}
									
									message.channel.send(finalMessage);
								}
								
								if (parseInt(args[2]) === parseInt(args[2]) && args[2] != null) {
									var indexS = parseInt(args[2]);
									if (args[3] == null) {
										if (theSettlement.nob[indexS] != null) {
											var finalMessage = theSettlement.n + " Noble House:\n";
											finalMessage = finalMessage + "House " + theSettlement.nob[indexS].name + " (" + theSettlement.nob[indexS].nobleLevel + ")\n";
											finalMessage = finalMessage + "Crest: " + theSettlement[indexS].crest + "\n";
											finalMessage = finalMessage + theSettlement.nob[indexS].notes;
											
											message.channel.send(finalMessage);
										} else {
											message.channel.send("No Noble House at that index, double check number.");
										}
									}
									
									if (args[3] == "name") {
										if (message.member.roles.find("name", "Mods")) {
											if (theSettlement.nob[indexS] != null) {
												var joinedArgs = args.slice(4).join(" ");
												theSettlement.nob[indexS].name = joinedArgs;
												
												message.channel.send("Set the name of the House to " + joinedArgs);
											} else {
												message.channel.send("No Noble House at that index, double check number.");
											}
										} else {
											message.channel.send("You don't have the proper permissions to change the names of Noble Houses.");
										}
									}
									
									if (args[3] == "nobility") {
										if (message.member.roles.find("name", "Mods")) {
											if (theSettlement.nob[indexS] != null) {
												var joinedArgs = args.slice(4).join(" ");
												theSettlement.nob[indexS].nobleLevel = joinedArgs;
												
												message.channel.send("Set the nobility level of the House to " + joinedArgs);
											} else {
												message.channel.send("No Noble House at that index, double check number.");
											}
										} else {
											message.channel.send("You don't have the proper permissions to change the nobility level of Noble Houses.");
										}
									}
									
									if (args[3] == "descset") {
										if (message.member.roles.find("name", "Mods")) {
											if (theSettlement.nob[indexS] != null) {
												var joinedArgs = args.slice(4).join(" ");
												theSettlement.nob[indexS].notes = joinedArgs;
												
												message.channel.send("Set the description of House " + theSettlement.nob[indexS].name + " to `" + joinedArgs + "`");
											} else {
												message.channel.send("No Noble House at that index, double check number.");
											}
										} else {
											message.channel.send("You don't have the proper permissions to change the description of Noble Houses.");
										}
									}
									
									if (args[3] == "descadd") {
										if (message.member.roles.find("name", "Mods")) {
											if (theSettlement.nob[indexS] != null) {
												var joinedArgs = args.slice(4).join(" ");
												theSettlement.nob[indexS].notes = theSettlement.nob[indexS].notes + "\n" + joinedArgs;
												
												message.channel.send("Added `" + joinedArgs + "` to the description of " + theSettlement.nob[indexS].name);
											} else {
												message.channel.send("No Noble House at that index, double check number.");
											}
										} else {
											message.channel.send("You don't have the proper permissions to add to the descriptions of Noble Houses.");
										}
									}
								}
							}
							
							
							client.settlements.set(args[0], theSettlement);
						}
					}
				}
			}
			
			if (command == "inventory" || command == "i") {
				var invenArr = [ ];
				var hammerArr = [ ];
				var buildingArr = [ ];
				var tradeArr = [ ];
				var emptyInventory = { "hammerspace":hammerArr, "inventory":invenArr, "externalStorage":buildingArr, "pendingTradeWith":null, "tradeConfirmed":false, "listOfTradeItems":tradeArr, "creditsBeingSent":0 };
				
				var interacted = 'Inventory';
				var otherInt = 'Hammerspace';
				
				if (!client.inventory.has(message.author.id)) {
					client.inventory.set(message.author.id, emptyInventory);
				}
				
				var inventoryTemp = client.inventory.get(message.author.id);
				
				if (args[0] == "h" || args[0] == "hammerspace") {
					interacted = 'Hammerspace';
					otherInt = 'Inventory';
					args.splice(0, 1);
				}
				
				if (args[0] == null) {
					var inventoryTemp = client.inventory.get(message.author.id);
					var finalMessage = "Your " + interacted + ":\n";
					var totalvalue = 0;
					var totalweight = 0;
					var tempItem;
					
					inventoryTemp[interacted.toLowerCase()].sort(compareByName);
					
					for (i = 0; i < inventoryTemp[interacted.toLowerCase()].length; i++) {
						tempItem = inventoryTemp[interacted.toLowerCase()][i];
						finalMessage = finalMessage + "`[" + i + "]`";
						finalMessage = finalMessage + "[TL: " + tempItem.TL + "] ";
						if (tempItem.quantity != 1) {
								finalMessage = finalMessage + " " + tempItem.quantity + "x ";
								finalMessage = finalMessage + tempItem.name + " (" + tempItem.itemWeight + " lbs., *$" + tempItem.itemPrice + "* each // " + (tempItem.itemWeight*tempItem.quantity) + " lbs., *$" + (tempItem.itemPrice*tempItem.quantity) + "* total)\n";
						} else {
							finalMessage = finalMessage + tempItem.name + " (" + tempItem.itemWeight + " lbs., *$" + tempItem.itemPrice + "*)\n";
						}
						totalvalue = totalvalue + (parseInt(tempItem.itemPrice)*tempItem.quantity);
						totalweight = totalweight + (parseInt(tempItem.itemWeight)*tempItem.quantity);
						
						if (finalMessage.length >= 1900) {
							message.author.send(finalMessage);
							finalMessage = "Your " + interacted + " (Continued):\n";
						}
					}
					
					finalMessage = finalMessage + "Total Weight: " + totalweight + " lbs, Total Value: " + totalvalue;
					
					message.author.send(finalMessage);
				}
				
				if (+args[0] === +args[0] && inventoryTemp[interacted.toLowerCase()][args[0]] != null) {
					var theItem = inventoryTemp[interacted.toLowerCase()][args[0]];
					var finalMessage = "Full Item Details (In " + interacted + "):\n";
					finalMessage = finalMessage + "`[" + args[0] + "]` ";
					if (parseInt(theItem.quantity) > 1) {
						finalMessage = finalMessage + parseInt(theItem.quantity) + "x ";
					}
					
					finalMessage = finalMessage + theItem.name + " [TL: " + theItem.TL + "] (" + theItem.pageRef + ")\n";
					
					if (theItem.quantity > 1) {
						finalMessage = finalMessage + theItem.itemWeight + " lbs., *$" + theItem.itemPrice + "* each,\n";
						finalMessage = finalMessage + theItem.itemWeight*theItem.quantity + " lbs., *$" + theItem.itemPrice*theItem.quantity + "* Total.\n";
					}
					
					else {
						finalMessage = finalMessage + theItem.itemWeight + " lbs., *$" + theItem.itemPrice + "*\n";
					}
					
					finalMessage = finalMessage + theItem.itemDesc + "\n";
					finalMessage = finalMessage + theItem.note;
					
					message.author.send(finalMessage);
				} else if (+args[0] === +args[0] && inventoryTemp[interacted.toLowerCase()][args[0]] == null) {
					message.channel.send(message.author + ", No item at that index!");
				}
				
				if (args[0] == "search") {
					var inventoryTemp = client.inventory.get(message.author.id);
					var finalMessage = "Your " + interacted + ":\n";
					var totalvalue = 0;
					var totalweight = 0;
					var tempItem;
					
					inventoryTemp.inventory.sort(compareByName);
					
					for (i = 0; i < inventoryTemp[interacted.toLowerCase()].length; i++) {
						tempItem = inventoryTemp[interacted.toLowerCase()][i];
						
						var searchable = tempItem.name + " " + tempItem.itemDesc + " " + tempItem.notes;
						if (tempItem.tags != null) { searchable = searchable + tempItem.tags; }
						
						if (searchable.toLowerCase().includes(args[1].toLowerCase())) {
							finalMessage = finalMessage + "`[" + i + "]`";
							finalMessage = finalMessage + "[TL: " + tempItem.TL + "] ";
							if (tempItem.quantity != 1) {
									finalMessage = finalMessage + " " + tempItem.quantity + "x ";
									finalMessage = finalMessage + tempItem.name + " (" + tempItem.itemWeight + " lbs., *$" + tempItem.itemPrice + "* each // " + (tempItem.itemWeight*tempItem.quantity) + " lbs., *$" + (tempItem.itemPrice*tempItem.quantity) + "* total)\n";
							} else {
								finalMessage = finalMessage + tempItem.name + " (" + tempItem.itemWeight + " lbs., *$" + tempItem.itemPrice + "*)\n";
							}
							totalvalue = totalvalue + (parseInt(tempItem.itemPrice)*tempItem.quantity);
							totalweight = totalweight + (parseInt(tempItem.itemWeight)*tempItem.quantity);
							
							if (finalMessage.length >= 1900) {
								message.author.send(finalMessage);
								finalMessage = "Your " + interacted + " (Continued):\n";
							}
						}
					}
					
					finalMessage = finalMessage + "Total Weight: " + totalweight + " lbs, Total Value: " + totalvalue;
					
					message.author.send(finalMessage);
				}
				
				if (args[0] == "m" || args[0] == "move" && inventoryTemp.inTradeWith == null) {
					if (args[1] != null && inventoryTemp[interacted.toLowerCase()][args[1]] != null) {
						var foundIdentical = false;
						var itemRef = 0;
						
						
						tempItem = inventoryTemp[interacted.toLowerCase()][args[1]];
						var quantityMoved = 1;
						if (args[2] != null) { quantityMoved = parseInt(args[2]); }
						if (quantityMoved > tempItem.quantity) { quantityMoved = tempItem.quantity; }
						
						for (k = 0; k < inventoryTemp[otherInt.toLowerCase()].length; k++) {
							newTempItem = inventoryTemp[otherInt.toLowerCase()][k];
							if (newTempItem.name == tempItem.name && newTempItem.TL == tempItem.TL && newTempItem.itemPrice == tempItem.itemPrice) {		
								itemRef = k;
								foundIdentical = true;
								
								break;
							}
						}
						
						if (foundIdentical == true) {
							inventoryTemp[otherInt.toLowerCase()][parseInt(itemRef)].quantity += quantityMoved;
						}
						
						if (foundIdentical == false) {
							var pushedObj = { "name":tempItem.name, "itemPrice":tempItem.itemPrice, "itemWeight":tempItem.itemWeight, "TL":tempItem.TL, "quantity":quantityMoved, "itemDesc":tempItem.itemDesc, "note":tempItem.note, "pageRef":tempItem.pageRef };
							inventoryTemp[otherInt.toLowerCase()].push(pushedObj);
						}
						
						
						
						inventoryTemp[interacted.toLowerCase()][args[1]].quantity -= quantityMoved;
						if (inventoryTemp[interacted.toLowerCase()][args[1]].quantity <= 0) { inventoryTemp[interacted.toLowerCase()].splice(args[1], 1); }
						
						message.channel.send(message.author + ", " + quantityMoved + "x " + tempItem.name + " moved to " + otherInt + "!");
					} else if (args[1] != null && inventoryTemp[interacted.toLowerCase()][args[1]] == null) {
						message.channel.send(message.author + ", No item at that index!");
					}
				}
				
				if (args[0] == "note") {
					if (args[1] == "clear" && args[2] != null) { inventoryTemp[interacted.toLowerCase()][args[2]].note = ""; }
					else {
						var newNote = args.slice(2).join(" ");
						
						if (inventoryTemp[interacted.toLowerCase()][args[2]] != null) {
							inventoryTemp[interacted.toLowerCase()][args[2]].note += newNote;
						}
					}
				}
				
				
				if (args[0] == "sell") {
					if (args[1] == null) {
						message.author.send("`!inventory sell (index)` sells the item (for 50% of listed value, to an NPC) in the specified index slot (index number visible in inventory view). This has ***NO CONFIRMATION*** if you do this command with an index number, so double check before using it! Use with caution!");
					} else if (args[1] != null && +args[1] === +args[1]) {
						if (inventoryTemp[interacted.toLowerCase()][args[1]] != null) {
							var theItem = inventoryTemp[interacted.toLowerCase()][args[1]];
							var amountTryingSell = 1;
							if (args[2] != null) { amountTryingSell = parseInt(args[2]); }
							
							if (!client.fullTable.has(fullID)) {
								var newFull = { "credits":1000, "grindClock":0, "autosessions":4 };
								client.fullTable.set(fullID, newFull);
							}
							var newData = client.fullTable.get(fullID);
							
							if (theItem.quantity - amountTryingSell < 0) {
								amountTryingSell = theItem.quantity;
							}
							
							newData.credits += Math.floor(parseInt(theItem.itemPrice)/2)*amountTryingSell;
							client.fullTable.set(fullID, newData);
							
							message.author.send("Sold " + amountTryingSell + "x " + theItem.name + " for " + (Math.floor(parseInt(theItem.itemPrice)/2)*amountTryingSell) + " Credits.");
							inventoryTemp[interacted.toLowerCase()][args[1]].quantity -= parseInt(amountTryingSell);
							
							
							
							if (inventoryTemp[interacted.toLowerCase()][args[1]].quantity <= 0) {
								inventoryTemp[interacted.toLowerCase()].splice(parseInt(args[1]), 1);
							}
						}
					}
				}
								
				
				if (args[0] == "additem") {
					if (message.guild != null) {
						if (message.member.roles.find("name", "Halliday")) {
							if (!client.inventory.has(message.mentions.members.first().id)) {
								client.inventory.set(message.mentions.members.first().id, emptyInventory);
							}
							
							var joinedArgs = args.slice(2).join(" ");
							var newArgs = joinedArgs.trim().split(";");
							
							newInventoryItem(newArgs, message, false);
						}
					}
				}
				
				if (args[0] == "delitem") {
					if (inventoryTemp[interacted.toLowerCase()][parseInt(args[1])] != null) {
						inventoryTemp[interacted.toLowerCase()].splice(parseInt(args[1]), 1);
					}
				}
				
				client.inventory.set(message.author.id, inventoryTemp);
			}
			
			
			if (command == "itest") {
				var invenArr = [ ];
				var hammerArr = [ ];
				var buildingArr = [ ];
				var tradeArr = [ ];
				var emptyInventory = { "hammerspace":hammerArr, "inventory":invenArr, "externalStorage":buildingArr, "pendingTradeWith":null, "tradeConfirmed":false, "listOfTradeItems":tradeArr, "creditsBeingSent":0 };
				
				if (!client.inventory.has(message.author.id)) {
					client.inventory.set(message.author.id, emptyInventory);
				}
				
				var inventoryTemp = client.inventory.get(message.author.id);
				
				
				console.log(inventoryTemp[args[0]]);
			}
			
			if (command == "upgrade") {
					var inventoryTemp = client.inventory.get(message.author.id);
					
					if (!client.fullTable.has(fullID)) {
						var newFull = { "credits":1000, "grindClock":0, "autosessions":4 };
						client.fullTable.set(fullID, newFull);
					}
					var newData = client.fullTable.get(fullID);
					
					if (args[0] == "weapon" && args[1] != null) {
						theIndex = parseInt(args[1]);
						if (inventoryTemp.inventory[theIndex] != null) {
							if (inventoryTemp.inventory[theIndex].basePrice == null) { inventoryTemp.inventory[theIndex].basePrice = inventoryTemp.inventory[theIndex].itemPrice; }
							
							var basePrice = inventoryTemp.inventory[theIndex].basePrice;
							var costModifier = 0;
							var upDesc = "";
							var inputStr = args[2].toLowerCase();
							
							if (inputStr == "balanced") { costModifier = 4; actualName = "Balanced"; upDesc = "Balanced: +1 to skill with any melee weapon or projectile (arrow, bolt etc.), or +1 Acc with a blowpipe, bow, or crossbow."; }
							if (inputStr == "dwarven") { costModifier = 4; actualName = "Dwarven"; upDesc = "Dwarven: Changes a Parry of 0U to 0, letting a weapon that can’t normally parry and attack on the same turn do just that. Doesn’t prevent the weapon from becoming unready after attacking (‡ on ST).";}
							if (inputStr == "elven") { costModifier = 16; actualName = "Elven"; upDesc = "Elven: Lets a bow shoot at +2 to ST for range and damage purposes; e.g., a ST 11 elf could draw a ST 13 bow."; }
							if (inputStr == "fine2") { costModifier = 2; actualName = "Fine"; upDesc = "Fine: -1 to odds of breakage, and +1 to damage for any cutting or impaling weapon, or +20% to range for a blowpipe, bow, or crossbow. (Projectiles, Crushing/Implaying melee, or thrown)"; }
							if (inputStr == "fine3") { costModifier = 3; actualName = "Fine"; upDesc = "Fine: -1 to odds of breakage, and +1 to damage for any cutting or impaling weapon, or +20% to range for a blowpipe, bow, or crossbow. (Fencing Weapons, Swords, Blowpipes, Bows, and Crossbows)"; }
							if (inputStr == "fine9") { costModifier = 9; actualName = "Fine"; upDesc = "Fine: -1 to odds of breakage, and +1 to damage for any cutting or impaling weapon, or +20% to range for a blowpipe, bow, or crossbow. (All other cutting melee/thrown weapons)"; }
							if (inputStr == "meteoric") { costModifier = 19; actualName = "Meteoric"; upDesc = "Meteoric: Meteoric iron is immune to magic – Reverse Missiles, Steelwraith, Turn Blade, and so on won’t stop it, and Shape Metal, Shatter, and the like can’t destroy it. Of course, it can’t benefit from enchantments, weapon-enhancing spells, or magical repairs."; }
							if (inputStr == "orichalcum") { costModifier = 29; actualName = "Orichalcum"; upDesc = "Orichalcum: Orichalcum weapons won’t break. Nonorichalcum weapons have +2 to odds of breakage when parrying heavy orichalcum ones. ";}
							if (inputStr == "ornate1") { costModifier = 1; actualName = "Ornate (+1)"; upDesc = "Ornate (+1): Jewels, gold, etc. Modifies reactions from buyers, gullible hirelings, etc. +1 to reactions."; }
							if (inputStr == "ornate2") { costModifier = 4; actualName = "Ornate (+2)"; upDesc = "Ornate (+2): Jewels, gold, etc. Modifies reactions from buyers, gullible hirelings, etc. +2 to reactions."; }
							if (inputStr == "ornate3") { costModifier = 9; actualName = "Ornate (+3)"; upDesc = "Ornate (+3): Jewels, gold, etc. Modifies reactions from buyers, gullible hirelings, etc. +3 to reactions."; }
							if (inputStr == "silversolid") { costModifier = 19; actualName = "Solid Silver"; upDesc = "Solid Silver: Metal arrows, bolts, melee weapons, and thrown weapons can be made of solid silver to exploit monster Vulnerability, but have +2 to odds of breakage"; }
							if (inputStr == "silvercoat") { costModifier = 4; actualName = "Silver Coating"; upDesc = "Silver Coating: Silver coating for these weapons doesn’t worsen breakage but isn’t as effective (see p. B275)"; }
							if (inputStr == "veryfine1") { costModifier = 19; actualName = "Very Fine"; upDesc = "Very Fine: -2 to odds of breakage and +2 to damage. +2 Accuracy and +1 Malf for firearms. (Fencing Weapons, Swords)"; }
							if (inputStr == "veryfine2") { costModifier = 49; actualName = "Very Fine"; upDesc = "Very Fine: -2 to odds of breakage and +2 to damage. +2 Accuracy and +1 Malf for firearms. (All Other Weapons)"; }

							var actualCost = (basePrice * (1 + costModifier)) - basePrice;
							
							if (!inventoryTemp.inventory[theIndex].itemDesc.includes(upDesc)) {
								var canGo = true;
								if ((inputStr == "fine" || inputStr == "veryfine" || inputStr == "silversolid") && (inventoryTemp.inventory[theIndex].itemDesc.includes("Fine") || inventoryTemp.inventory[theIndex].itemDesc.includes("Very Fine") || inventoryTemp.inventory[theIndex].itemDesc.includes("Solid Silver"))) {
									message.channel.send(message.author + ", " + args[2] + " is mutually exclusive with another upgrade that item already has (Fine, Very Fine, or Solid Silver).");
									canGo = false;
								}

								else if ((inputStr == "meteoric" || inputStr == "orichalcum" || inputStr == "silversolid") && (inventoryTemp.inventory[theIndex].itemDesc.includes("Orichalcum") || inventoryTemp.inventory[theIndex].itemDesc.includes("Meteoric") || inventoryTemp.inventory[theIndex].itemDesc.includes("Solid Silver"))) {
									message.channel.send(message.author + ", " + args[2] + " is mutually exclusive with another upgrade that item already has (Orichalcum, Meteoric, or Solid Silver).");
									canGo = false;
								}
								
								else if (canGo == true) {
									if (parseInt(args[3]) === parseInt(args[3]) && args[3] != null) { var quantityDone = parseInt(args[3]); }
									else { var quantityDone = 1; }
									if (quantityDone <= inventoryTemp.inventory[theIndex].quantity) {
										var upgradeCostEach = actualCost;
										actualCost = actualCost * quantityDone;
										var foundIdentical = false;
										
										if (parseInt(newData.credits) >= actualCost) {
											newData.credits -= actualCost;
											client.fullTable.set(fullID, newData);
											
											tempItem = inventoryTemp.inventory[theIndex];
											newDesc = inventoryTemp.inventory[theIndex].itemDesc + "\n" + upDesc;
											newName = actualName + " " + inventoryTemp.inventory[theIndex].name;
											newPrice = parseInt(inventoryTemp.inventory[theIndex].itemPrice) + upgradeCostEach;
											var newItem = { "name":newName, "itemPrice":newPrice, "itemWeight":tempItem.itemWeight, "TL":tempItem.TL, "quantity":quantityDone, "itemDesc":newDesc, "note":tempItem.note, "pageRef":pageRef, "basePrice":tempItem.basePrice };
											
											inventoryTemp.inventory[theIndex].quantity = parseInt(inventoryTemp.inventory[theIndex].quantity) - parseInt(quantityDone);
											if (inventoryTemp.inventory[theIndex].quantity <= 0) { inventoryTemp.inventory.splice(theIndex, 1); }
											
											for (i = 0; i < inventoryTemp.inventory.length; i++) {
												if (inventoryTemp.inventory[i].name == newItem.name && inventoryTemp.inventory[i].itemPrice == newItem.itemPrice && inventoryTemp.inventory[i].TL == newItem.TL) {
													inventoryTemp.inventory[i].quantity = parseInt(inventoryTemp.inventory[i].quantity) + parseInt(newItem.quantity);
													foundIdentical = true;
													break;
												}
											}
											
											if (foundIdentical == false) {
												inventoryTemp.inventory.push(newItem);
											}
											
											client.inventory.set(message.author.id, inventoryTemp);
											
											message.channel.send("Upgraded " + quantityDone + "x " + inventoryTemp.inventory[theIndex].name + " with " + actualName + " for " + actualCost + " Credits.");
										} else {
											message.channel.send("Not enough Credits to upgrade " + quantityDone + "x " + inventoryTemp.inventory[theIndex].name + " with " + actualName + ". Cost is " + actualCost + " Credits.");
										}
									} else {
										message.channel.send("You don't have " + quantityDone + "x " + inventoryTemp.inventory[theIndex].name + "!");
									}
								}
							} else {
								message.channel.send(message.author + ", that item is already upgraded in that way!");
							}
							
						} else {
							message.channel.send(message.author + ", No item at that index in your Inventory.");
						}
					} else if (args[0] == "weapon" && args[1] == null) {
						message.channel.send(message.author + ", correct command syntax: `!upgrade weapon {item index} {upgrade}`\nWeapon Upgrades: `balanced`, `dwarven`, `elven`, `fine2`, `fine3`, `fine9`, `meteoric`, `orichalcum`, `ornate`, `ornate2`, `ornate3`, `silversolid`, `silvercoat`, `veryfine1`, `veryfine2`.\n\nNote that there is NO CONFIRMATION if you use this command - to verify upgrades and cost use `!upcheck weapon {item index} {upgrade}` instead!");
					}
					
					if (args[0] == "armor" && args[1] != null) {
						theIndex = parseInt(args[1]);
						if (inventoryTemp.inventory[theIndex] != null) {
							if (inventoryTemp.inventory[theIndex].basePrice == null) { inventoryTemp.inventory[theIndex].basePrice = inventoryTemp.inventory[theIndex].itemPrice; }
							
							var basePrice = inventoryTemp.inventory[theIndex].basePrice;
							var costModifier = 0;
							var weightMod = 1;
							var upDesc = "";
							var inputStr = args[2].toLowerCase();
							
							if (inputStr == "dragonhide1") { costModifier = 37; actualName = "Dragonhide (+1)"; weightMod = 1.25; upDesc = "Dragonhide: Any hard leather armor (DR 2) can be dragonhide. This provides +1 DR. It also gives -3 reactions from dragons!"; }
							if (inputStr == "dragonhide2") { costModifier = 44; actualName = "Dragonhide (+2)"; weightMod = 1.5; upDesc = "Dragonhide: Any hard leather armor (DR 2) can be dragonhide. This provides +2 DR. It also gives -3 reactions from dragons!";}
							if (inputStr == "dragonhide3") { costModifier = 52; actualName = "Dragonhide (+3)"; weightMod = 1.75; upDesc = "Dragonhide: Any hard leather armor (DR 2) can be dragonhide. This provides +3 DR. It also gives -3 reactions from dragons!"; }
							if (inputStr == "dragonhide4") { costModifier = 59; actualName = "Dragonhide (+4)"; weightMod = 2; upDesc = "Dragonhide: Any hard leather armor (DR 2) can be dragonhide. This provides +4 DR. It also gives -3 reactions from dragons!"; }
							if (inputStr == "dwarven1") { costModifier = 1; actualName = "Dwarven (+1)"; weightMod = 1.2; upDesc = "Dwarven: Dwarves can forge any DR 6-7 plate armor to be extra-thick. This raises weight: +1 DR, 1.2x weight";}
							if (inputStr == "dwarven2") { costModifier = 2; actualName = "Dwarven (+2)"; weightMod = 1.4; upDesc = "Dwarven: Dwarves can forge any DR 6-7 plate armor to be extra-thick. This raises weight: +2 DR, 1.4x weight";}
							if (inputStr == "dwarven3") { costModifier = 3; actualName = "Dwarven (+3)"; weightMod = 1.6; upDesc = "Dwarven: Dwarves can forge any DR 6-7 plate armor to be extra-thick. This raises weight: +3 DR, 1.6x weight";}
							if (inputStr == "elven") { costModifier = 3; actualName = "Elven"; upDesc = "Elven: Elven mail uses the higher DR listed for the base armor against all damage – don’t reduce its DR vs. crushing blows.";}
							if (inputStr == "fine") { costModifier = 9; actualName = "Fine"; weightMod = 0.75; upDesc = "Fine: Expertly fitted, with no waste material. Offers full DR at 3/4 the usual weight. Only fits wearers whose height and weight match the original owner’s!"; }
							if (inputStr == "silk") { costModifier = 99; actualName = "Giant Spider Silk"; upDesc = "Giant Spider Silk: Improves cloth armor from DR 1 to DR 2, and allows it to be worn under other armor, for +2 DR, without the DX penalty for layering."; }
							if (inputStr == "meteoric") { costModifier = 19; actualName = "Meteoric"; upDesc = "Meteoric: Meteoric iron is immune to magic – Reverse Missiles, Steelwraith, Turn Blade, and so on won’t stop it, and Shape Metal, Shatter, and the like can’t destroy it. Of course, it can’t benefit from enchantments, weapon-enhancing spells, or magical repairs."; }
							if (inputStr == "orichalcum") { costModifier = 29; actualName = "Orichalcum"; weightMod = 0.66; upDesc = "Orichalcum: Provides full DR at just 1/3 the usual weight! Works on any bronze plate armor (assume that gauntlets and sollerets can be bronze)"; }
							if (inputStr == "spiked") { costModifier = 2; actualName = "Spiked"; upDesc = "Spiked: Lets the wearer roll DX-4 to stab each foe in close combat with him for 1d-2 imp, once per turn, as a free action. Anyone who strikes him with an unarmed attack is hit immediately and automatically – and a bite, slam, or Constriction Attack means that attacker suffers maximum damage (4 points). Works on any plate armor."; }
							if (inputStr == "thieves") { costModifier = 3; actualName = "Thieves'"; upDesc = "Thieves': Blackened mail woven for maximum flexibility and minimum noise. Ignore its weight for encumbrance purposes when making Climbing and Stealth rolls. Works on any mail."; }
							if (inputStr == "ornate1") { costModifier = 1; actualName = "Ornate (+1)"; upDesc = "Ornate (+1): Jewels, gold, etc. Modifies reactions from buyers, gullible hirelings, etc. +1 to reactions."; }
							if (inputStr == "ornate2") { costModifier = 4; actualName = "Ornate (+2)"; upDesc = "Ornate (+2): Jewels, gold, etc. Modifies reactions from buyers, gullible hirelings, etc. +2 to reactions."; }
							if (inputStr == "ornate3") { costModifier = 9; actualName = "Ornate (+3)"; upDesc = "Ornate (+3): Jewels, gold, etc. Modifies reactions from buyers, gullible hirelings, etc. +3 to reactions."; }
							if (inputStr == "experttailor") { costModifier = 5; weightMod = 0.85; actualName = "Tailored (Ex)"; upDesc = "Expert Tailoring: Low-Tech assumes that all but cheap-quality armor is custom-fitted to the owner. An experienced armorer can improve the fit even further. This adds -1 to penalties to target chinks in armor."; }
							if (inputStr == "mastertailor") { costModifier = 29; weightMod = 0.7; actualName = "Tailored (Ma)"; upDesc = "Master Tailoring: This is very fine armor, made by one of the world’s best armorers. This adds -1 to penalties to target chinks in armor "; }
							if (inputStr == "fluting") { costModifier = 4; weightMod = 0.9; actualName = "Fluted"; upDesc = "All plate armor (LT108-109) is specifically shaped to cause blows to glance off harmlessly, but adding flutes, ribs, and bosses in key areas allows a weight reduction with no loss of strength. Scale and lamellar armor (LT106-107) can likewise be strengthened with a boss or vertical medial rib raised on each scale. Not available for other armor."; }
							if (inputStr == "duplex") { costModifier = 8; weightMod = 0.9; actualName = "Duplex Plated"; upDesc = "Duplex Plate: This is an advanced form of hardened steel; see Armor of Proof (LT110). It’s only an option for plate armor"; }
							var actualCost = (basePrice * (1 + costModifier)) - basePrice;
							
							if (!inventoryTemp.inventory[theIndex].itemDesc.includes(upDesc)) {
								var canGo = true;
								if ((inputStr == "thieves" || inputStr == "ornate1" || inputStr == "ornate2" || inputStr == "ornate3") && (inventoryTemp.inventory[theIndex].itemDesc.includes("Thieves") || inventoryTemp.inventory[theIndex].itemDesc.includes("Ornate"))) {
									message.channel.send(message.author + ", " + args[2] + " is mutually exclusive with another upgrade that item already has (Ornate or Thieves').");
									canGo = false;
								}

								else if ((inputStr.includes("dragonhide") || inputStr == "orichalcum" || inputStr == "meteoric" || inputStr.includes("dwarven") || inputStr.includes("spiked")) && inventoryTemp.inventory[theIndex].itemDesc.includes("Dragonhide")) {
									message.channel.send(message.author + ", " + args[2] + " is mutually exclusive with another upgrade that item already has (Dragonhide).");
									canGo = false;
								}
								
								else if ((inputStr == "meteoric" || inputStr == "orichalcum") && (inventoryTemp.inventory[theIndex].itemDesc.includes("Orichalcum") || inventoryTemp.inventory[theIndex].itemDesc.includes("Meteoric"))) {
									message.channel.send(message.author + ", " + args[2] + " is mutually exclusive with another upgrade that item already has (Orichalcum or Meteoric).");
									canGo = false;
								}
								
								else if (canGo == true) {
									if (parseInt(args[3]) === parseInt(args[3]) && args[3] != null) { var quantityDone = parseInt(args[3]); }
									else { var quantityDone = 1; }
									if (quantityDone <= inventoryTemp.inventory[theIndex].quantity) {
										var upgradeCostEach = actualCost;
										actualCost = actualCost * quantityDone;
										var foundIdentical = false;
										if (newData.credits >= actualCost) {
											newData.credits -= actualCost;
											client.fullTable.set(fullID, newData);
											tempItem = inventoryTemp.inventory[theIndex];
											newDesc = inventoryTemp.inventory[theIndex].itemDesc + "\n" + upDesc;
											newName = actualName + " " + inventoryTemp.inventory[theIndex].name;
											newPrice = parseInt(inventoryTemp.inventory[theIndex].itemPrice) + upgradeCostEach;
											newWeight = parseInt(inventoryTemp.inventory[theIndex].itemWeight) * weightMod;
											var newItem = { "name":newName, "itemPrice":newPrice, "itemWeight":newWeight, "TL":tempItem.TL, "quantity":quantityDone, "itemDesc":newDesc, "note":tempItem.note, "pageRef":pageRef, "basePrice":tempItem.basePrice };
											
											inventoryTemp.inventory[theIndex].quantity = parseInt(inventoryTemp.inventory[theIndex].quantity) - parseInt(quantityDone);
											if (inventoryTemp.inventory[theIndex].quantity <= 0) { inventoryTemp.inventory.splice(theIndex, 1); }
											
											for (i = 0; i < inventoryTemp.inventory.length; i++) {
												if (inventoryTemp.inventory[i].name == newItem.name && inventoryTemp.inventory[i].itemPrice == newItem.itemPrice && inventoryTemp.inventory[i].TL == newItem.TL) {
													inventoryTemp.inventory[i].quantity = parseInt(inventoryTemp.inventory[i].quantity) + parseInt(newItem.quantity);
													foundIdentical = true;
													break;
												}
											}
											
											if (foundIdentical == false) {
												inventoryTemp.inventory.push(newItem);
											}
											
											client.inventory.set(message.author.id, inventoryTemp);
											
											message.channel.send("Upgraded " + quantityDone + "x " + inventoryTemp.inventory[theIndex].name + " with " + actualName + " for " + actualCost + " Credits.");
										} else {
											message.channel.send("Not enough Credits to upgrade " + quantityDone + "x " + inventoryTemp.inventory[theIndex].name + " with " + actualName + ". Cost is " + actualCost + " Credits.");
										}
									} else {
										message.channel.send("You don't have " + quantityDone + "x " + inventoryTemp.inventory[theIndex].name + "!");
									}
								}
							} else {
								message.channel.send(message.author + ", that item is already upgraded in that way!");
							}
							
						} else {
							message.channel.send(message.author + ", No item at that index in your Inventory.");
						}
					} else if (args[0] == "armor" && args[1] == null) {
						message.channel.send(message.author + ", correct command syntax: `!upgrade armor {item index} {upgrade}`\nArmor Upgrades: `dragonhide1`, `dragonhide2`, `dragonhide3`, `dragonhide4`, `dwarven1`, `dwarven2`, `dwarven3`, `elven`, `fine`, `silk`, `meteoric`, `ornate`, `ornate2`, `ornate3`, `orichalcum`, `thieves`, `spiked`, 'experttailor', 'mastertailor', 'fluting', 'duplex'.\n\nNote that there is NO CONFIRMATION if you use this command - to verify upgrades and cost use `!upcheck armor {item index} {upgrade}` instead!");
					}
					
				}
			
			if (command == "upcheck") {
				var inventoryTemp = client.inventory.get(message.author.id);
				
				if (args[0] == "weapon" && args[1] != null) {
					theIndex = parseInt(args[1]);
					if (inventoryTemp.inventory[theIndex] != null) {
						if (inventoryTemp.inventory[theIndex].basePrice == null) { inventoryTemp.inventory[theIndex].basePrice = inventoryTemp.inventory[theIndex].itemPrice; }
						
						if (parseInt(args[3]) === parseInt(args[3]) && args[3] != null) { var quantityDone = parseInt(args[3]); }
						else { var quantityDone = 1; }
						
						var basePrice = inventoryTemp.inventory[theIndex].basePrice;
						var costModifier = 0;
						var upDesc = "";
						if (args[2] != null) {
							var inputStr = args[2].toLowerCase();
							
							if (inputStr == "balanced") { costModifier = 4; upDesc = "Balanced: +1 to skill with any melee weapon or projectile (arrow, bolt etc.), or +1 Acc with a blowpipe, bow, or crossbow."; }
							if (inputStr == "dwarven") { costModifier = 4; upDesc = "Dwarven: Changes a Parry of 0U to 0, letting a weapon that can’t normally parry and attack on the same turn do just that. Doesn’t prevent the weapon from becoming unready after attacking (‡ on ST).";}
							if (inputStr == "elven") { costModifier = 16; upDesc = "Elven: Lets a bow shoot at +2 to ST for range and damage purposes; e.g., a ST 11 elf could draw a ST 13 bow."; }
							if (inputStr == "fine2") { costModifier = 2; upDesc = "Fine: -1 to odds of breakage, and +1 to damage for any cutting or impaling weapon, or +20% to range for a blowpipe, bow, or crossbow. (Projectiles, Crushing/Implaying melee, or thrown)"; }
							if (inputStr == "fine3") { costModifier = 3; upDesc = "Fine: -1 to odds of breakage, and +1 to damage for any cutting or impaling weapon, or +20% to range for a blowpipe, bow, or crossbow. (Fencing Weapons, Swords, Blowpipes, Bows, and Crossbows)"; }
							if (inputStr == "fine9") { costModifier = 9; upDesc = "Fine: -1 to odds of breakage, and +1 to damage for any cutting or impaling weapon, or +20% to range for a blowpipe, bow, or crossbow. (All other cutting melee/thrown weapons)"; }
							if (inputStr == "meteoric") { costModifier = 19; upDesc = "Meteoric: Meteoric iron is immune to magic – Reverse Missiles, Steelwraith, Turn Blade, and so on won’t stop it, and Shape Metal, Shatter, and the like can’t destroy it. Of course, it can’t benefit from enchantments, weapon-enhancing spells, or magical repairs."; }
							if (inputStr == "orichalcum") { costModifier = 29; upDesc = "Orichalcum: Orichalcum weapons won’t break. Nonorichalcum weapons have +2 to odds of breakage when parrying heavy orichalcum ones. ";}
							if (inputStr == "ornate1") { costModifier = 1; upDesc = "Ornate (+1): Jewels, gold, etc. Modifies reactions from buyers, gullible hirelings, etc. +1 to reactions."; }
							if (inputStr == "ornate2") { costModifier = 4; upDesc = "Ornate (+2): Jewels, gold, etc. Modifies reactions from buyers, gullible hirelings, etc. +2 to reactions."; }
							if (inputStr == "ornate3") { costModifier = 9; upDesc = "Ornate (+3): Jewels, gold, etc. Modifies reactions from buyers, gullible hirelings, etc. +3 to reactions."; }
							if (inputStr == "silversolid") { costModifier = 19; upDesc = "Solid Silver: Metal arrows, bolts, melee weapons, and thrown weapons can be made of solid silver to exploit monster Vulnerability, but have +2 to odds of breakage"; }
							if (inputStr == "silvercoat") { costModifier = 4; upDesc = "Silver Coating: Silver coating for these weapons doesn’t worsen breakage but isn’t as effective (see p. B275)"; }
							if (inputStr == "veryfine1") { costModifier = 19; upDesc = "Very Fine: -2 to odds of breakage and +2 to damage. +2 Accuracy and +1 Malf for firearms. (Fencing Weapons, Swords)"; }
							if (inputStr == "veryfine2") { costModifier = 49; upDesc = "Very Fine: -2 to odds of breakage and +2 to damage. +2 Accuracy and +1 Malf for firearms. (All Other Weapons)"; }
							
							else { message.channel.send("That's not a correct upgrade name!"); return; }
							
							actualCost = ((basePrice * (1 + costModifier)) - basePrice);
							
							if (!inventoryTemp.inventory[theIndex].itemDesc.includes(upDesc)) {
								var canGo = true;
								if ((inputStr == "fine" || inputStr == "veryfine" || inputStr == "silversolid") && (inventoryTemp.inventory[theIndex].itemDesc.includes("Fine:") || inventoryTemp.inventory[theIndex].itemDesc.includes("Very Fine:") || inventoryTemp.inventory[theIndex].itemDesc.includes("Solid Silver:"))) {
									message.channel.send(message.author + ", " + args[2] + " is mutually exclusive with another upgrade that item already has (Fine, Very Fine, or Solid Silver).");
									canGo = false;
								}

								if ((inputStr == "meteoric" || inputStr == "orichalcum" || inputStr == "silversolid") && (inventoryTemp.inventory[theIndex].itemDesc.includes("Orichalcum::") || inventoryTemp.inventory[theIndex].itemDesc.includes("Meteoric::") || inventoryTemp.inventory[theIndex].itemDesc.includes("Solid Silver:"))) {
									message.channel.send(message.author + ", " + args[2] + " is mutually exclusive with another upgrade that item already has (Orichalcum, Meteoric, or Solid Silver).");
									canGo = false;
								}
								
								message.channel.send("Cost to upgrade " + quantityDone + "x " + inventoryTemp.inventory[theIndex].name + " with " + inputStr + " would be " + (actualCost*quantityDone) + " Credits.\n\n" + upDesc);
							} else {
								message.channel.send(message.author + ", that item is already upgraded in that way!");
							}
						} else {
							message.channel.send(message.author + ", you have to add an upgrade name to do that!");
						}
						
					} else {
						message.channel.send(message.author + ", No item at that index in your Inventory.");
					}
				} else if (args[0] == "weapon" && args[1] == null) {
					message.channel.send(message.author + ", correct command syntax: `!upcheck weapon {item index} {upgrade}`\nWeapon Upgrades: `balanced`, `dwarven`, `elven`, `fine2`, `fine3`,`fine9`, `meteoric`, `orichalcum`, `ornate`, `ornate2`, `ornate3`, `silversolid`, `silvercoat`, `veryfine1`, `veryfine2`.");
				}
				
				if (args[0] == "armor" && args[1] != null) {
					theIndex = parseInt(args[1]);
					if (inventoryTemp.inventory[theIndex] != null) {
						if (inventoryTemp.inventory[theIndex].basePrice == null) { inventoryTemp.inventory[theIndex].basePrice = inventoryTemp.inventory[theIndex].itemPrice; }
						
						var basePrice = inventoryTemp.inventory[theIndex].basePrice;
						var costModifier = 0;
						var weightMod = 0;
						var upDesc = "";
						
						if (args[2] != null) {
							var inputStr = args[2].toLowerCase();
								
							if (inputStr == "dragonhide1") { costModifier = 37; actualName = "Dragonhide (+1)"; weightMod = 1.25; upDesc = "Dragonhide: Any hard leather armor (DR 2) can be dragonhide. This provides +1 DR. It also gives -3 reactions from dragons!"; }
							if (inputStr == "dragonhide2") { costModifier = 44; actualName = "Dragonhide (+2)"; weightMod = 1.5; upDesc = "Dragonhide: Any hard leather armor (DR 2) can be dragonhide. This provides +2 DR. It also gives -3 reactions from dragons!";}
							if (inputStr == "dragonhide3") { costModifier = 52; actualName = "Dragonhide (+3)"; weightMod = 1.75; upDesc = "Dragonhide: Any hard leather armor (DR 2) can be dragonhide. This provides +3 DR. It also gives -3 reactions from dragons!"; }
							if (inputStr == "dragonhide4") { costModifier = 59; actualName = "Dragonhide (+4)"; weightMod = 2; upDesc = "Dragonhide: Any hard leather armor (DR 2) can be dragonhide. This provides +4 DR. It also gives -3 reactions from dragons!"; }
							if (inputStr == "dwarven1") { costModifier = 1; actualName = "Dwarven (+1)"; weightMod = 1.2; upDesc = "Dwarven: Dwarves can forge any DR 6-7 plate armor to be extra-thick. This raises weight: +1 DR, 1.2x weight";}
							if (inputStr == "dwarven2") { costModifier = 2; actualName = "Dwarven (+2)"; weightMod = 1.4; upDesc = "Dwarven: Dwarves can forge any DR 6-7 plate armor to be extra-thick. This raises weight: +2 DR, 1.4x weight";}
							if (inputStr == "dwarven3") { costModifier = 3; actualName = "Dwarven (+3)"; weightMod = 1.6; upDesc = "Dwarven: Dwarves can forge any DR 6-7 plate armor to be extra-thick. This raises weight: +3 DR, 1.6x weight";}
							if (inputStr == "elven") { costModifier = 3; actualName = "Elven"; upDesc = "Elven: Elven mail uses the higher DR listed for the base armor against all damage – don’t reduce its DR vs. crushing blows.";}
							if (inputStr == "fine") { costModifier = 9; actualName = "Fine"; weightMod = 0.75; upDesc = "Fine: Expertly fitted, with no waste material. Offers full DR at 3/4 the usual weight. Only fits wearers whose height and weight match the original owner’s!"; }
							if (inputStr == "silk") { costModifier = 99; actualName = "Giant Spider Silk"; upDesc = "Giant Spider Silk: Improves cloth armor from DR 1 to DR 2, and allows it to be worn under other armor, for +2 DR, without the DX penalty for layering."; }
							if (inputStr == "meteoric") { costModifier = 19; actualName = "Meteoric"; upDesc = "Meteoric: Meteoric iron is immune to magic – Reverse Missiles, Steelwraith, Turn Blade, and so on won’t stop it, and Shape Metal, Shatter, and the like can’t destroy it. Of course, it can’t benefit from enchantments, weapon-enhancing spells, or magical repairs."; }
							if (inputStr == "orichalcum") { costModifier = 29; actualName = "Orichalcum"; weightMod = 0.66; upDesc = "Orichalcum: Provides full DR at just 1/3 the usual weight! Works on any bronze plate armor (assume that gauntlets and sollerets can be bronze)"; }
							if (inputStr == "spiked") { costModifier = 2; actualName = "Spiked"; upDesc = "Spiked: Lets the wearer roll DX-4 to stab each foe in close combat with him for 1d-2 imp, once per turn, as a free action. Anyone who strikes him with an unarmed attack is hit immediately and automatically – and a bite, slam, or Constriction Attack means that attacker suffers maximum damage (4 points). Works on any plate armor."; }
							if (inputStr == "thieves") { costModifier = 3; actualName = "Thieves'"; upDesc = "Thieves': Blackened mail woven for maximum flexibility and minimum noise. Ignore its weight for encumbrance purposes when making Climbing and Stealth rolls. Works on any mail."; }
							if (inputStr == "ornate1") { costModifier = 1; actualName = "Ornate (+1)"; upDesc = "Ornate (+1): Jewels, gold, etc. Modifies reactions from buyers, gullible hirelings, etc. +1 to reactions."; }
							if (inputStr == "ornate2") { costModifier = 4; actualName = "Ornate (+2)"; upDesc = "Ornate (+2): Jewels, gold, etc. Modifies reactions from buyers, gullible hirelings, etc. +2 to reactions."; }
							if (inputStr == "ornate3") { costModifier = 9; actualName = "Ornate (+3)"; upDesc = "Ornate (+3): Jewels, gold, etc. Modifies reactions from buyers, gullible hirelings, etc. +3 to reactions."; }
							if (inputStr == "experttailor") { costModifier = 5; weightMod = 0.85; actualName = "Tailored (Ex)"; upDesc = "Expert Tailoring: Low-Tech assumes that all but cheap-quality armor is custom-fitted to the owner. An experienced armorer can improve the fit even further. This adds -1 to penalties to target chinks in armor."; }
							if (inputStr == "mastertailor") { costModifier = 29; weightMod = 0.7; actualName = "Tailored (Ma)"; upDesc = "Master Tailoring: This is very fine armor, made by one of the world’s best armorers. This adds -1 to penalties to target chinks in armor "; }
							if (inputStr == "fluting") { costModifier = 4; weightMod = 0.9; actualName = "Fluted"; upDesc = "All plate armor (LT108-109) is specifically shaped to cause blows to glance off harmlessly, but adding flutes, ribs, and bosses in key areas allows a weight reduction with no loss of strength. Scale and lamellar armor (LT106-107) can likewise be strengthened with a boss or vertical medial rib raised on each scale. Not available for other armor."; }
							if (inputStr == "duplex") { costModifier = 8; weightMod = 0.9; actualName = "Duplex Plated"; upDesc = "Duplex Plate: This is an advanced form of hardened steel; see Armor of Proof (LT110). It’s only an option for plate armor"; }
							else { message.channel.send("That's not a correct upgrade name!"); return; }
							
							var actualCost = (basePrice * (1 + costModifier)) - basePrice;
							
							if (!inventoryTemp.inventory[theIndex].itemDesc.includes(upDesc)) {
								var canGo = true;
								if ((inputStr == "thieves" || inputStr == "ornate1" || inputStr == "ornate2" || inputStr == "ornate3") && (inventoryTemp.inventory[theIndex].itemDesc.includes("Thieves") || inventoryTemp.inventory[theIndex].itemDesc.includes("Ornate"))) {
									message.channel.send(message.author + ", " + args[2] + " is mutually exclusive with another upgrade that item already has (Ornate or Thieves').");
									canGo = false;
								}

								else if ((inputStr.includes("dragonhide") || inputStr == "orichalcum" || inputStr == "meteoric" || inputStr.includes("dwarven") || inputStr.includes("spiked")) && inventoryTemp.inventory[theIndex].itemDesc.includes("Dragonhide")) {
									message.channel.send(message.author + ", " + args[2] + " is mutually exclusive with another upgrade that item already has (Dragonhide).");
									canGo = false;
								}
								
								else if ((inputStr == "meteoric" || inputStr == "orichalcum") && (inventoryTemp.inventory[theIndex].itemDesc.includes("Orichalcum") || inventoryTemp.inventory[theIndex].itemDesc.includes("Meteoric"))) {
									message.channel.send(message.author + ", " + args[2] + " is mutually exclusive with another upgrade that item already has (Orichalcum or Meteoric).");
									canGo = false;
								}
								
								message.channel.send("Cost to upgrade " + inventoryTemp.inventory[theIndex].name + " with " + inputStr + " would be " + ((basePrice * (1 + costModifier)) - basePrice) + " Credits.\n\n" + upDesc);
							} else {
								message.channel.send(message.author + ", that item is already upgraded in that way!");
							}
						} else {
							message.channel.send(message.author + ", you have to add an upgrade name to do that!");
						}
						
					} else {
						message.channel.send(message.author + ", No item at that index in your Inventory.");
					}
					
							
				} else if (args[0] == "armor" && args[1] == null) {
					message.channel.send(message.author + ", correct command syntax: `!upgrade armor {item index} {upgrade}`\nArmor Upgrades: `dragonhide1`, `dragonhide2`, `dragonhide3`, `dragonhide4`, `dwarven1`, `dwarven2`, `dwarven3`, `elven`, `fine`, `silk`, `meteoric`, `ornate`, `ornate2`, `ornate3`, `orichalcum`, `thieves`, `spiked`, 'experttailor', 'mastertailor', 'fluting', 'duplex'.");
				}
					
			}
			
			if (command == "baseprice") {
				if (message.guild != null) {
					if (message.member.roles.find("name", "Halliday")) {
						var inventoryTemp = client.inventory.get(message.author.id);
						
						if (inventoryTemp.inventory[parseInt(args[0])] != null) {
							inventoryTemp.inventory[parseInt(args[0])].basePrice = parseInt(args[1]);
							client.inventory.set(message.author.id, inventoryTemp);
							message.channel.send(message.author + ", set base price of " + inventoryTemp.inventory[parseInt(args[0])].name + " to " + parseInt(args[1]) + " Credits.");
						}
						
						
					}
				}
			}
			
			

		}
		if (command == "fantasy") {
			if (args[0] == "encounter") {
				var encType = fantasyEncounterType();
				var encReaction = fantasyEncounterReaction();
				var encMotivation = fantasyEncounterMotivation();
				var encDistance = fantasyEncounterDistance();
				var encTerrain = fantasyEncounterTerrain();
				var encMana = fantasyEncounterManaSanctity();
				var encSanctity = fantasyEncounterManaSanctity();
				var encWeather = fantasyEncounterWeather();
				var encTime = fantasyEncounterTime();
				
				var finalMessage = message.author + ", Encounter:\n";
				finalMessage = finalMessage + "Type: " + encType + "\n\n";
				finalMessage = finalMessage + "Reaction (if applicable): " + encReaction.n + " (" + encReaction.d + ")" + "\n\n";
				finalMessage = finalMessage + "Motivation (if applicable): " + encMotivation + "\n\n";
				finalMessage = finalMessage + "Starting Distance: " + encDistance + "\n\n";
				finalMessage = finalMessage + "Terrain: " + encTerrain.n;
				if (encTerrain.s != "None") { 
					finalMessage = finalMessage + ", " + encTerrain.s; 
					if (encTerrain.s == "Location of Supernatural Significance") {
						finalMessage = finalMessage + "\n" + encMana.n + " Mana: " + encMana.d;
						finalMessage = finalMessage + "\n" + encSanctity.n + " Sanctity: " + encSanctity.d;
					}
				}
				finalMessage = finalMessage + "\n\nWeather: " + encWeather.n + " (" + encWeather.d + ")\n\n";
				finalMessage = finalMessage + "Time: " + encTime.n + " (" + encTime.d + ")\n\n";
				
				message.channel.send(finalMessage);
			}
		}
		
		if (command == "r+" || command == "roll+") {
			var joinedArgs = args.join(" ");
			var output = diceParse.parse(joinedArgs);
			
			message.channel.send(message.author + ": `" + joinedArgs + "` = " + output);
		}
		
		if (command == "r" || command == "roll") {
			var joinedArgs = args.join(" ");
			var newArgs = joinedArgs.split("");
			var numberArgs = [ ];
			var counter = 0;
			var total = 0;
			var subtotal = 0;
			var finalMessage = message.author + ": `" + joinedArgs + "` = ";
			var operator = "add";
			
			for (i = 0; i < newArgs.length; i++) {
				if ((i != 0 && +newArgs[i] === +newArgs[i] && +numberArgs[counter-1] === +numberArgs[counter-1]) || (newArgs[i] == "h" || newArgs[i] == "l") && numberArgs[counter-1] == "k") {
					numberArgs[counter-1] = numberArgs[counter-1] + newArgs[i];
				} else { numberArgs.push(newArgs[i]); counter++; }
			}
			
			for (i = 0; i < numberArgs.length; i++) {
				if (i != 0 && numberArgs[i+1] != "d" && numberArgs[i-1] != "d") { finalMessage = finalMessage + " "; }
					
				if (numberArgs[i] == "d") {
					var amount = 1;
					if (numberArgs[i-1] !== numberArgs[i-1] || numberArgs[i-1] == null || numberArgs[i-1] == " ") { amount = 1;
					} else if (+numberArgs[i-1] === +numberArgs[i-1] && numberArgs[i-1] != null) { amount = parseInt(numberArgs[i-1]); }
					
					var size = 6;
					if (numberArgs[i+1] !== numberArgs[i+1] || numberArgs[i+1] == null || numberArgs[i+1] == " ") { size = 6;
					} else if (+numberArgs[i-1] === +numberArgs[i-1] && numberArgs[i-1] != null) { size = parseInt(numberArgs[i+1]); }
					
					finalMessage = finalMessage + "(";
					var rolledDice = rollDice(amount, size);
					rolledDice.sort(basicCompare);
					
					if (numberArgs[i+1] == "kh" || numberArgs[i+2] == "kh") {
						var keepHighAmt = 1;
						if (numberArgs[i+1] == "kh") {
							if (numberArgs[i+2] !== numberArgs[i+2] && numberArgs[i+2] != null) { keepHighAmt = parseInt(numberArgs[i+2]); 	}
							rolledDice.splice(0, rolledDice.length-keepHighAmt); 
						}
						
						if (numberArgs[i+2] == "kh") {
							if (numberArgs[i+3] !== numberArgs[i+2] && numberArgs[i+2] != null) { keepHighAmt = parseInt(numberArgs[i+3]); }
							rolledDice.splice(0, rolledDice.length-keepHighAmt); 
						}
					}
					
					if (numberArgs[i+1] == "kl" || numberArgs[i+2] == "kl") {
						var keepLowAmt = 1;
						if (numberArgs[i+1] == "kl") {
							if (numberArgs[i+2] !== numberArgs[i+2] && numberArgs[i+2] != null) { keepLowAmt = parseInt(numberArgs[i+2]); }
							rolledDice.splice(keepLowAmt, (rolledDice.length-1)-keepLowAmt); 
						}
						
						if (numberArgs[i+2] == "kl") {
							if (numberArgs[i+3] !== numberArgs[i+2] && numberArgs[i+2] != null) { keepLowAmt = parseInt(numberArgs[i+3]); }
							rolledDice.splice(keepLowAmt, (rolledDice.length-1)-keepLowAmt);  
						}
					}
				
					for (j = 0; j < rolledDice.length; j++) {
						subtotal = subtotal + parseInt(rolledDice[j]);
						if (j != 0) { finalMessage = finalMessage + ", "; }
						finalMessage = finalMessage + rolledDice[j];
					} 
					finalMessage = finalMessage + ")";
				}
				
				if (numberArgs[i] == "+") { operator = "add"; finalMessage = finalMessage + "+"; } 
				if (numberArgs[i] == "-") { operator = "sub"; finalMessage = finalMessage + "-"; } 
				if (numberArgs[i] == "*" || numberArgs[i] == "x") { operator = "mul"; finalMessage = finalMessage + "x"; }
				if (numberArgs[i] == "/" ) { operator = "div"; finalMessage = finalMessage + "/"; }
				
				if (+numberArgs[i] === +numberArgs[i] && numberArgs[i+1] != "d" && numberArgs[i-1] != "d" && numberArgs[i-1] != "kh" && numberArgs[i-1] != "kl") {
					finalMessage = finalMessage + numberArgs[i];
					subtotal = parseInt(numberArgs[i]);
				}
				
				if (subtotal != 0) {
					if (operator == "add") { total += subtotal; } if (operator == "sub") { total -= subtotal; }
					if (operator == "mul") { total *= subtotal; } if (operator == "div") { total = total / subtotal; }
					subtotal = 0;
				} 
			}
			
			finalMessage = finalMessage + " = " + total;
			if (finalMessage.length < 2000) {
				message.channel.send(finalMessage);
			} else {
				message.channel.send(message.author + ": `" + joinedArgs + "` = (Full result too long to display) = " + total);
			}
		}
	}
});


try {
	client.login("NDY5Njk0NzI5MjExMjE1ODcy.DjLj2A.sEtH36hvkyEfkykm86pbIrYqxQ0");
} catch (error) {
	console.log("Client Login Failed - Exiting Program");
	return process.exit(1);
}