class Actor;
class Map;

#include <map>
#include "NetCommon.h"

// Prepare messages to be sent to the server, 
class CustomClient : public nanz::net::client_interface<GameMsg> {
public:
	void MovePlayer(sPlayerDescription player, int xchange, int ychange) { // Player moved, send the new info to the server
		nanz::net::message<GameMsg> msg;
		msg.header.id = GameMsg::Player_Move;

		msg << player << xchange << ychange;

		Send(msg);
	}



	void ChatMessage(std::string message, const char* name) { // Player sent a chat message, pass it along to the server
		nanz::net::message<GameMsg> msg;
		msg.header.id = GameMsg::Chat_Message;

		std::string full = std::string(name) + ": " + message;
		
		const char* converted = full.c_str();

		int length = strlen(converted);

		for (int i = 0; i < length; i++) {
			msg << converted[i];
		}

		msg << length;

		Send(msg);
	}

};


class Engine {
public: 
	TCODList<Actor*> actors; // All the actors the client is currently aware of
	Actor* player; // The client's actor
	
	Map* map; // The visible array of tiles
	int screenWidth; // The window width
	int screenHeight; // The window height
	Gui* gui; // The GUI

	std::array<MapTile, 17*17> mini;
	int topleft_x; // Where the topleft-most corner of the minimap currently is (x)
	int topleft_y; // Where the topleft-most corner of the minimap currently is (y)

	// These are for setting each minimap tile as we iterate through the info
	std::string mini_name; // Minimap tile name
	int mini_ch; // Minimap tile symbol
	TCODColor mini_fg; // Minimap tile foreground color
	TCODColor mini_bg; // Minimap tile background color
	int mini_mX; // Minimap X
	int mini_mY; // Minimap Y



	TCODList<const char*> skills; // A list of all the skills - change this to a Skill object list in the future
	TCODList<const char*> quests; // A list of all the quests - change this to a Quest object list in the future
	TCODList<const char*> spells; // A list of all the spells - change this to a Spell object list in the future

	// Current key press and mouse information
	TCOD_key_t lastKey;
	TCOD_mouse_t mouse;

	// The game time in milliseconds
	int gameTimeMS;

	// If the player is pathing and where they're pathing to
	int pathToX = 0;
	int pathToY = 0;
	bool pathing = false;

	// The stored tile definitions for the map editor. The "clipboard" of sorts
	int mapEdit_ch = ',';
	std::string mapEdit_name = "Grass";
	int mapEdit_fg_r = TCODColor::darkerGreen.r;
	int mapEdit_fg_g = TCODColor::darkerGreen.g;
	int mapEdit_fg_b = TCODColor::darkerGreen.b;
	int mapEdit_bg_r, mapEdit_bg_g, mapEdit_bg_b = 0;
	bool mapEdit_blocksMove = false;



	void ResetMapEdit() const;
	Engine(int screenWidth, int screenHeight);
	~Engine();
	void update();
	void render();
	void sendToBack(Actor* actor);
	
	void LoadTiles();

	// Multiplayer logic components
	CustomClient client;
	sPlayerDescription ownDesc;
	bool bWaitingForConnection = true;
	uint32_t nPlayerID = 0;
	std::string ownName;

	std::unordered_map<uint32_t, sPlayerDescription> otherPlayers;
	std::map<int, Tile> tileLibrary;
};

extern Engine engine;