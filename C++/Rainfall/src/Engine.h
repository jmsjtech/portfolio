class Actor;
class Map;

#include <map>
#include "NetCommon.h"

class CustomClient : public nanz::net::client_interface<GameMsg> {
public:
	void PingServer() {
		nanz::net::message<GameMsg> msg;

		Send(msg);
	}

	void MovePlayer(sPlayerDescription player) {
		nanz::net::message<GameMsg> msg;
		msg.header.id = GameMsg::Game_UpdatePlayer;

		msg << player;

		Send(msg);
	}

	void ChatMessage(std::string message, const char* name) {
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
	TCODList<Actor*> actors;
	Actor* player;
	
	Map* map;
	int screenWidth;
	int screenHeight;
	Gui* gui;

	TCODList<const char*> skills;
	TCODList<const char*> quests;
	TCODList<const char*> spells;

	TCOD_key_t lastKey;
	TCOD_mouse_t mouse;

	int gameTimeMS;

	Engine(int screenWidth, int screenHeight);
	~Engine();
	void update();
	void render();
	void sendToBack(Actor* actor);

	CustomClient client;
	sPlayerDescription ownDesc;
	bool bWaitingForConnection = true;
	uint32_t nPlayerID = 0;
	std::string ownName;

	std::unordered_map<uint32_t, sPlayerDescription> otherPlayers;
};

extern Engine engine;