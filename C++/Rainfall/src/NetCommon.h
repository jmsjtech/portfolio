#pragma once
#include <cstdint>

#define NANZ_PGEX_NETWORK
#include "Nanz_Net.h"

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
};

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