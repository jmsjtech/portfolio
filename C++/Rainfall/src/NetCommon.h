#pragma once
#include <cstdint>

#define NANZ_PGEX_NETWORK
#include "Nanz_Net.h"

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