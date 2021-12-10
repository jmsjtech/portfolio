#pragma once
#include <cstdint>

#define NANZ_PGEX_NETWORK
#include "Nanz_Net.h"


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
	std::string name = "Grass";
	bool blocksMove = false;

	TileMeta() {}
	TileMeta(int idIn, const char* nameIn, bool blockIn) : id(idIn), blocksMove(blockIn) {
		name = nameIn;
	}
};