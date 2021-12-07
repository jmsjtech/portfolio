#include "main.h"

static const int MAPTILE_WIDTH = 78;
static const int MAPTILE_HEIGHT = 42;
static const int OVERWORLD_WIDTH = 5;
static const int OVERWORLD_HEIGHT = 5;


bool Map::canWalk(int x, int y) const {
	// If the tile blocks move you definitely can't go there
	if (tiles[x + y * MAPTILE_WIDTH].blocksMove) {
		return false;
	}

	// Otherwise see if there's an actor on it that's blocking movement
	for (Actor** i = engine.actors.begin(); i != engine.actors.end(); i++) {
		Actor* actor = *i;
		if (actor->blocks && actor->x == x && actor->y == y) {
			// there's an actor so you can't walk here
			return false;
		}
	}

	// If the tile doesn't block and there's no blocking actor, it must be walkable
	return true;
}

// Get a tile at position or index
Tile& Map::tileAt(int x, int y) const { return tiles[x + y * MAPTILE_WIDTH]; }
Tile& Map::tileAtIndex(int index) const { return tiles[index]; }

// Set the tile at a position
void Map::SetTile(Tile& tile, int x, int y) const {
	tiles[x + y * MAPTILE_WIDTH] = tile;
	return;
}

// Set the tile at an index
void Map::SetTileIndex(Tile& tile, int index) const {
	tiles[index] = tile;
	return;
}

// Return the dimension constants
int Map::getWidth() const { return MAPTILE_WIDTH; }
int Map::getHeight() const { return MAPTILE_HEIGHT; }

// Render the whole tile array to the root console at the right position
void Map::render() const {
	for (int x = 0; x < MAPTILE_WIDTH; x++) {
		for (int y = 0; y < MAPTILE_HEIGHT; y++) {
			TCODConsole::root->setChar(x+1, y+1, tileAt(x, y).ch);
			TCODConsole::root->setCharBackground(x+1, y+1, tileAt(x, y).bg);
			TCODConsole::root->setCharForeground(x+1, y+1, tileAt(x, y).fg);
		}
	}
}


// Make the new map by initializing the tile array
Map::Map()  {
	tiles = new Tile[MAPTILE_WIDTH * MAPTILE_HEIGHT];
}

Map::~Map() {
	delete[] tiles;
}


// Create a new monster at the specified position and map coordinates, though it should pretty much never be creating monsters on other maps locally
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

// Create an item at the specified position and map coordinates, but it shouldn't be doing this unless you're on the map in the first place
void Map::addItem(int x, int y, int mX, int mY) {
	Actor* healthPotion = new Actor(x, y, mX, mY, '!', "health potion", 0, TCODColor::violet);
	healthPotion->blocks = false;
	healthPotion->pickable = new Healer(4);
	engine.actors.push(healthPotion);
}