#pragma once
#include <cstdint>

#define NANZ_PGEX_NETWORK
#include "Nanz_Net.h"
#include <map>

static const int MAP_WIDTH = 78;
static const int MAP_HEIGHT = 42;

// Possible message headers
enum class GameMsg : uint32_t {
	Server_GetStatus,
	Server_GetPing,

	Client_Accepted,
	Client_AssignID,
	Client_RegisterWithServer,
	Client_UnregisterWithServer,

	Game_AddPlayer,
	Game_RemovePlayer,
	Game_UpdatePlayer,

	Chat_Message,
	PlayersOnMapTile,
	Player_Move,
	Send_Map,
	Send_Minimap,
	Add_Item,

	TileRequest,
	ItemRequest,
};

// Minimalist player information to exchange between clients
struct sPlayerDescription {
	uint32_t nUniqueID = 0;
	uint32_t nAvatarID = 0;

	int ch; // the character representing the player, almost always @
	int r, g, b; // the color of the player's avatar

	int x = 0;
	int y = 0;
	int mX = 0;
	int mY = 0;

	std::array<std::pair<int, int>, 28> inventory; // 28 slots. Each slot is a pair, first int is item id, second int is quantity held
};


// All the data the server needs for calculations
struct TileMeta { 
	int id = 0;
	char name[20] = "Grass";
	bool blocksMove = false;

	int fgR = 0, fgG = 127, fgB = 0;
	int bgR = 0, bgG = 0, bgB = 0;
	int ch = 44;

	TileMeta() {}
};

enum ItemCategory {
	ERROR_ITEM = -1,
	HATCHET = 0, // An item used to chop trees
	PICKAXE = 1, // An item used to mine rocks
	KNIFE = 2, // An item used for crafting (woodworking)
	HAMMER = 3, // An item used for crafting (metalworking)
	FISHING = 4, // An item used for fishing
	POTION = 5, // An item that can be consumed for a temporary effect
	FOOD = 6, // An item that can be consumed to restore hit points
	RUNE = 7, // An item used for casting spells
};

enum EquipSlot {
	NOT_EQUIPPABLE = -1,
	MAIN_HAND = 0,
	OFF_HAND = 1,
	HEAD = 2,
	TORSO = 3,
	LEGS = 4,
	HANDS = 5,
	FEET = 6,
	RING = 7,
	AMULET = 8,
	CAPE = 9,
};

struct ItemMeta {
	int id = -1;
	char name[20] = "DEBUG"; 

	int fgR = 255, fgG = 0, fgB = 0;
	int bgR = 0, bgG = 0, bgB = 0;
	int ch = 'x';

	ItemCategory cat = ERROR_ITEM; // The broad category of the item
	EquipSlot equip = NOT_EQUIPPABLE; // If the item is equippable and where
	int catID = -1; // The ID within the category for specific needs, like what potion or rune it counts as
};

// All the data the server needs for calculations
struct MapMeta {
	int id = 0;
	char name[20] = "Void";

	int fgR = 0, fgG = 0, fgB = 0;
	int bgR = 0, bgG = 0, bgB = 0;
	int ch = 44;

	int mX = -99; // Position in the overworld
	int mY = -99; // Position in the overworld

	MapMeta() {}
};