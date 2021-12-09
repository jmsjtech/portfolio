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
	Player_Move,
};

struct Position {
	int x;
	int y;

	Position(int x, int y) : x(x), y(y) {}
	Position() : x(0), y(0) {}

	friend bool operator== (Position& first, Position& second) {
		if (first.x == second.x && first.y == second.y)
			return true;
		return false;
	}

	Position operator+(const Position& pos) {
		Position newPos;
		newPos.x = this->x + pos.x;
		newPos.y = this->y + pos.y;
		return newPos;
	}

	bool operator <(const Position& rhs) const {
		if (x != rhs.y) {
			return y < rhs.y;
		} 
		return x < rhs.y;
	}
};

// Minimalist player information to exchange between clients
struct sPlayerDescription {
	uint32_t nUniqueID = 0;
	uint32_t nAvatarID = 0;

	int ch; // the character representing the player, almost always @
	int r, g, b; // the color of the player's avatar

	Position pos;
	Position worldPos;

	sPlayerDescription() {}
};

struct TileMeta {
	std::string name;
	bool blocksMove;

	TileMeta() : name("Grass"), blocksMove(false) {}
};