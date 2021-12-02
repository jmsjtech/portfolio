#include "main.h"

static const int MAPTILE_WIDTH = 80;
static const int MAPTILE_HEIGHT = 43;
static const int OVERWORLD_WIDTH = 5;
static const int OVERWORLD_HEIGHT = 5;


bool Map::canWalk(int x, int y) const {
	if (tiles[x + y * MAPTILE_WIDTH].blocksMove) {
		return false;
	}

	for (Actor** i = engine.actors.begin(); i != engine.actors.end(); i++) {
		Actor* actor = *i;
		if (actor->blocks && actor->x == x && actor->y == y) {
			// there's an actor so you can't walk here
			return false;
		}
	}

	return true;
}

Tile Map::tileAt(int x, int y) const {
	return tiles[x + y * MAPTILE_WIDTH];
}

void Map::render() const {
	for (int x = 0; x < MAPTILE_WIDTH; x++) {
		for (int y = 0; y < MAPTILE_HEIGHT; y++) {
			TCODConsole::root->setChar(x, y, tileAt(x, y).ch);
			TCODConsole::root->setCharBackground(x, y, tileAt(x, y).bg);
			TCODConsole::root->setCharForeground(x, y, tileAt(x, y).fg);
		}
	}
}



Map::Map()  {
	tiles = new Tile[MAPTILE_WIDTH * MAPTILE_HEIGHT];
	map = new TCODMap(MAPTILE_WIDTH, MAPTILE_HEIGHT);
}

Map::~Map() {
	delete[] tiles;
}



void Map::addMonster(int x, int y, int mX, int mY) {
	TCODRandom* rng = TCODRandom::getInstance();
	if (rng->getInt(0, 100) < 80) {
		// create an orc
		Actor* orc = new Actor(x, y, mX, mY, 'o', "orc", 3, TCODColor::desaturatedGreen);
		orc->destructible = new MonsterDestructible(10, 0, "dead orc");
		orc->attacker = new Attacker(3);
		orc->ai = new MonsterAi();
		engine.actors.push(orc);
	} else {
		// create a troll
		Actor* troll = new Actor(x, y, mX, mY, 'T', "troll", 3, TCODColor::darkerGreen);
		troll->destructible = new MonsterDestructible(16, 1, "troll carcass");
		troll->attacker = new Attacker(4);
		troll->ai = new MonsterAi();
		engine.actors.push(troll);
	}
}

void Map::addItem(int x, int y, int mX, int mY) {
	Actor* healthPotion = new Actor(x, y, mX, mY, '!', "health potion", 0, TCODColor::violet);
	healthPotion->blocks = false;
	healthPotion->pickable = new Healer(4);
	engine.actors.push(healthPotion);
}