#include <stdio.h>
#include <stdarg.h>
#include "main.h"

// A bunch of constants we need for various things
static const int PANEL_HEIGHT = 27;
static const int BAR_WIDTH = 19;
static const int MSG_X = BAR_WIDTH + 3;
static const int MSG_HEIGHT = PANEL_HEIGHT - 4;
static const int INVENTORY_WIDTH = 40;
static const int INVENTORY_HEIGHT = 28;
static const int SIDEBAR_WIDTH = 40;
static const int SIDEBAR_HEIGHT = 42;


Gui::Gui() {
	// Initialize our display areas and the scroll position
	con = new TCODConsole(engine.screenWidth, PANEL_HEIGHT);
	inv = new TCODConsole(INVENTORY_WIDTH, INVENTORY_HEIGHT);
	sidebar = new TCODConsole(SIDEBAR_WIDTH, SIDEBAR_HEIGHT);
	selected = "Skills";
	selectScroll = 0;

	// Fill out some basic skill info for debug purposes
	engine.skills.push("Attack");
	engine.skills.push("Strength");
	engine.skills.push("Defense");
	engine.skills.push("Health");
	engine.skills.push("Magic");
	engine.skills.push("Mining");
	engine.skills.push("Smithing");

	engine.quests.push("No quests currently");
}

Gui::~Gui() {
	delete con;
	delete inv;
	delete sidebar;
	log.clearAndDelete();
}

void Gui::render() {
	// Clear all three display consoles
	con->setDefaultBackground(TCODColor::black);
	con->clear();
	inv->setDefaultBackground(TCODColor::black);
	inv->clear();
	sidebar->setDefaultBackground(TCODColor::black);
	sidebar->clear();
	
	con->setDefaultForeground(TCODColor::white);

	// draw the health bar
	if (engine.player->destructible) {
		renderBar(1, 1, BAR_WIDTH, "HP", engine.player->destructible->hp, engine.player->destructible->maxHp, TCODColor::lightRed, TCODColor::darkerRed);
	}

	// Draw the message log messages
	int y = 1;
	float colorCoef = 0.4f;
	for (Message** it = log.begin(); it != log.end(); it++) {
		Message* message = *it;
		con->setDefaultForeground(message->col * colorCoef);
		con->print(MSG_X, y, message->text);
		y++;
		if (colorCoef < 1.0f) {
			colorCoef += 0.3f;
		}
	}

	// Frame the message log
	con->setDefaultForeground(TCODColor(100, 100, 100));
	con->printFrame(MSG_X - 1, 0, 59, 27, false, TCOD_BKGND_DEFAULT);
	con->printFrame(MSG_X - 1, 0, 59, 25, false, TCOD_BKGND_DEFAULT, "message log");
	con->print(MSG_X, PANEL_HEIGHT - 2, "CHAT: ");
	if (chatBuffer == NULL) {
		chatBuffer = new std::string();
	}
	
	// Also print the current chat buffer in the chat area
	con->print(MSG_X + 6, PANEL_HEIGHT - 2, chatBuffer->c_str());

	// Frame the minimap area??
	con->printFrame(0, 0, 21, 27, false, TCOD_BKGND_DEFAULT);
	con->printFrame(1, 3, 19, 19, false, TCOD_BKGND_DEFAULT, "[%d, %d]", engine.ownDesc.mX, engine.ownDesc.mY);

	// Iterate through all the minimap tiles to display them
	for (int y = 0; y < minimap::h; y++) {
		for (int x = 0; x < minimap::w; x++) {
			MapTile mapTile = engine.minimap[x][y];
			// If a tile is called "Void" it hasn't been initialized yet, so don't bother displaying it
			if (mapTile.name == "Void")
				continue;

			// Minimap Tile
			con->setDefaultForeground(mapTile.fg);
			con->setDefaultBackground(mapTile.bg);
			con->print(2 + (mapTile.mX - engine.topleft_x), 4 + (mapTile.mY - engine.topleft_y), std::string(1, mapTile.ch).c_str());

			// Set the middle of the minimap to the player icon, since that's where the player always is.
			con->setDefaultForeground(TCODColor::white);
			con->print(2 + 8, 4 + 8, std::string("@").c_str());

			// If the player is hovering over the minimap with their mouse, display the name and location of the hovered tile
			if (mapTile.mX - engine.topleft_x == miniX && mapTile.mY - engine.topleft_y == miniY) {
				con->print(2, 23, "%s [%d, %d]", mapTile.name.c_str(), engine.topleft_x + miniX, engine.topleft_y + miniY);
			}
		}
	}

	con->setDefaultForeground(TCODColor::white);


	// display the inventory frame
	inv->setDefaultForeground(TCODColor(100, 100, 100));
	inv->printFrame(0, 0, INVENTORY_WIDTH, INVENTORY_HEIGHT, true, TCOD_BKGND_DEFAULT, "backpack");


	// display the inventory
	inv->setDefaultForeground(TCODColor::white);
	int shortcut = 'a';
	int invy = 1;
	for (Actor** it = engine.player->container->inventory.begin(); it != engine.player->container->inventory.end(); it++) {
		Actor* actor = *it;
		inv->print(1, invy, "(%c) %s", shortcut, actor->name);
		invy++;
		shortcut++;
	}


	// display the sidebar
	sidebar->setDefaultForeground(TCODColor(100, 100, 100));
	sidebar->printFrame(0, 0, SIDEBAR_WIDTH, SIDEBAR_HEIGHT, true, TCOD_BKGND_DEFAULT);
	sidebar->print(1, 0, "<S>|<M>|<Q>|<E>|<Z>");
	sidebar->setDefaultForeground(TCODColor::white);

	if (selected == "Skills") {
		// lock scrolling properly
		if (selectScroll > engine.skills.size() - 19) {
			selectScroll = engine.skills.size() - 19;
		}
		if (selectScroll < 0) { selectScroll = 0; }

		// skill header
		sidebar->print(1, 0, "<S>");
		sidebar->print(1, 1, "--------------------------------------");
		sidebar->print(1, 2, "    SKILL    |   LV   |   XP TO NEXT  ");
		sidebar->print(1, 3, "--------------------------------------");

		// print all the skills
		int skillY = 2;
		int skillCount = 0;
		for (const char** it = engine.skills.begin(); it != engine.skills.end(); it++) {
			skillCount++;
			if (skillCount > selectScroll) {
				skillY += 2;
				const char* skill = *it;
				sidebar->print(1, skillY, skill);
			}
		}
	} else if (selected == "Magic") {
		// lock scrolling properly
		if (selectScroll > engine.skills.size() - 19) {
			selectScroll = engine.skills.size() - 19;
		}
		if (selectScroll < 0) { selectScroll = 0; }

		// magic header
		sidebar->print(5, 0, "<M>");
		sidebar->print(1, 1, "--------------------------------------");
		sidebar->print(1, 2, "      SPELL      | LV |     RUNES     ");
		sidebar->print(1, 3, "--------------------------------------");

		// print all the spells
		int magicY = 3;
		int magicCount = 0;
		for (const char** it = engine.spells.begin(); it != engine.spells.end(); it++) {
			magicCount++;
			if (magicCount > selectScroll) {
				magicY += 2;
				const char* spell = *it;
				sidebar->print(1, magicY, spell);
			}
		}
	} else if (selected == "Quests") {
		// lock scrolling properly
		if (selectScroll > engine.quests.size() - 19) {
			selectScroll = engine.quests.size() - 19;
		}
		if (selectScroll < 0) { selectScroll = 0; }

		// quest header
		sidebar->print(9, 0, "<Q>");
		sidebar->print(1, 1, "--------------------------------------");
		sidebar->print(1, 2, "          QUEST          |    DIFF    ");
		sidebar->print(1, 3, "--------------------------------------");

		int questY = 2;
		int questCount = 0;
		if (selectScroll > 0) {
			sidebar->print(1, 4, "^");
		}

		if (selectScroll < engine.quests.size() - 19) {
			sidebar->print(1, SIDEBAR_HEIGHT - 2, "v");
		}

		for (const char** it = engine.quests.begin(); it != engine.quests.end(); it++) {
			questCount++;
			if (questCount > selectScroll && questY < SIDEBAR_HEIGHT - 2) {
				questY += 2;
				const char* quest = *it;
				sidebar->print(3, questY, quest);
			}
		}
	} else if (selected == "Equipment") {
		sidebar->print(13, 0, "<E>");
		sidebar->print(1, 1, "Equipment");
	} else if (selected == "Map Editor") {
		// lock scrolling properly
		if (selectScroll > engine.quests.size() - 19) {
			selectScroll = engine.quests.size() - 19;
		}
		if (selectScroll < 0) { selectScroll = 0; }

		// map editor header
		sidebar->print(17, 0, "<Z>");
		sidebar->print(1, 1, "--------------------------------------");
		sidebar->print(1, 2, "               Map Editor             ");
		sidebar->print(1, 3, "--------------------------------------");


		// Tile to place
		sidebar->print(1, 5, "Name: %s", engine.mapEdit_name.c_str());
		TCODColor tileFG = TCODColor(engine.mapEdit_fg_r, engine.mapEdit_fg_g, engine.mapEdit_fg_b);
		TCODColor tileBG = TCODColor(engine.mapEdit_bg_r, engine.mapEdit_bg_g, engine.mapEdit_bg_b);
		sidebar->print(1, 7, "Appearance:");
		sidebar->setDefaultForeground(tileFG);
		sidebar->setDefaultBackground(tileBG);
		sidebar->print(13, 7, std::string(1, engine.mapEdit_ch).c_str());
		sidebar->setDefaultForeground(TCODColor::white);
		sidebar->setDefaultBackground(TCODColor::black);
		sidebar->print(1, 9, "Foreground: %dr, %dg, %db", engine.mapEdit_fg_r, engine.mapEdit_fg_g, engine.mapEdit_fg_b);
		sidebar->print(1, 11, "Background: %dr, %dg, %db", engine.mapEdit_bg_r, engine.mapEdit_bg_g, engine.mapEdit_bg_b);
		sidebar->print(1, 13, "Blocks Move: %s", engine.mapEdit_blocksMove ? "true" : "false");

		// Minimap Tile
		MapTile* tile = &engine.minimap[engine.topleft_x + 8][engine.topleft_y + 8];

		sidebar->print(1, 17, "Minimap Tile:");
		sidebar->print(1, 19, "Name: %s", tile->name.c_str());
		sidebar->print(1, 21, "Appearance:");
		sidebar->setDefaultForeground(tile->fg);
		sidebar->setDefaultBackground(tile->bg);
		sidebar->print(13, 21, std::string(1, tile->ch).c_str());
		sidebar->setDefaultForeground(TCODColor::white);
		sidebar->setDefaultBackground(TCODColor::black);
		sidebar->print(1, 23, "Foreground: %dr, %dg, %db", tile->fg.r, tile->fg.g, tile->fg.b);
		sidebar->print(1, 25, "Background: %dr, %dg, %db", tile->bg.r, tile->bg.g, tile->bg.b);


		// The reset "button" to change the editor tile back to default grass
		sidebar->print(1, SIDEBAR_HEIGHT - 3, "--------------------------------------");
		sidebar->print(1, SIDEBAR_HEIGHT - 2, "                RESET                 ");
	}




	// mouse look
	renderMouseLook();

	// Frame the map
	TCODConsole::root->setDefaultForeground(TCODColor(100, 100, 100));
	TCODConsole::root->printFrame(0, 0, engine.map->getWidth() + 2, engine.map->getHeight() + 1, false, TCOD_BKGND_DEFAULT, "map");

	// Ship all three consoles along to the main console for display
	TCODConsole::blit(con, 0, 0, engine.screenWidth, PANEL_HEIGHT, TCODConsole::root, 0, engine.screenHeight - PANEL_HEIGHT);
	TCODConsole::blit(inv, 0, 0, INVENTORY_WIDTH, INVENTORY_HEIGHT, TCODConsole::root, engine.screenWidth - INVENTORY_WIDTH, 0);
	TCODConsole::blit(sidebar, 0, 0, SIDEBAR_WIDTH, SIDEBAR_HEIGHT, TCODConsole::root, engine.screenWidth - SIDEBAR_WIDTH, engine.screenHeight - SIDEBAR_HEIGHT);
}

void Gui::renderBar(int x, int y, int width, const char* name, float value, float maxValue, const TCODColor& barColor, const TCODColor& backColor) {
	con->setDefaultBackground(backColor);
	con->rect(x, y, width, 1, false, TCOD_BKGND_SET);

	int barWidth = (int)(value / maxValue * width);
	if (barWidth > 0) {
		// draw bar
		con->setDefaultBackground(barColor);
		con->rect(x, y, barWidth, 1, false, TCOD_BKGND_SET);
	}

	// print text on top of the bar
	con->setDefaultBackground(TCODColor::white);
	con->printEx(x + width / 2, y, TCOD_BKGND_NONE, TCOD_CENTER, "%s : %g/%g", name, value, maxValue);
}

Gui::Message::Message(const char* text, const TCODColor& col) : text(_strdup(text)), col(col) {
}

Gui::Message::~Message() {
	free(text);
}

void Gui::message(const TCODColor& col, const char* text, ...) {
	// build the text
	va_list ap;
	char buf[128];
	va_start(ap, text);
	vsprintf_s(buf, text, ap);
	va_end(ap);

	char* lineBegin = buf;
	char* lineEnd;
	do {
		//make room
		if (log.size() == MSG_HEIGHT) {
			Message* toRemove = log.get(0);
			log.remove(toRemove);
			delete toRemove;
		}

		lineEnd = strchr(lineBegin, '\n');

		if (lineEnd) {
			*lineEnd = '\0';
		}

		// add a new message
		Message* msg = new Message(lineBegin, col);
		log.push(msg);

		// go to next line
		lineBegin = lineEnd + 1;
	} while (lineEnd);
}

void Gui::renderMouseLook() {
	char buf[128] = "";
	bool first = true;
	for (Actor** it = engine.actors.begin(); it != engine.actors.end(); it++) {
		Actor* actor = *it;

		if (actor->x == engine.mouse.cx && actor->y == engine.mouse.cy) {
			if (!first) {
				strcat_s(buf, ", ");
			} else {
				first = false;
			}
			strcat_s(buf, actor->name);
		}
	}

	// display actors under mouse cursor
	con->setDefaultForeground(TCODColor::lightGrey);
	con->print(1, 0, buf);
}