#include "main.h"

static const int MAPTILE_WIDTH = 78;
static const int MAPTILE_HEIGHT = 42;

// Two tile returning and setting functions based on what numbers you've got
Tile& Map::tileAt(int x, int y) const { return tiles[x + (y * MAPTILE_WIDTH)]; }
Tile& Map::tileAtIndex(int index) const { return tiles[index]; }

void Map::SetTile(Tile& tile, int x, int y) const { tiles[x + (y * MAPTILE_WIDTH)] = tile; }
void Map::SetTileIndex(Tile & tile, int index) const { tiles[index] = tile; }

void Map::render() const {
    for (int x = 0; x < MAPTILE_WIDTH; x++) {
        for (int y = 0; y < MAPTILE_HEIGHT; y++) {
            TCODConsole::root->setChar(x + 1, y + 1, engine.map->tileAt(x, y).ch);
            TCODConsole::root->setCharBackground(x + 1, y + 1, engine.map->tileAt(x, y).bg);
            TCODConsole::root->setCharForeground(x + 1, y + 1, engine.map->tileAt(x, y).fg);
        }
    }
} 