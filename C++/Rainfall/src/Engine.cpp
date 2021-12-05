#include "main.h"

Engine::Engine(int screenWidth, int screenHeight) : screenWidth(screenWidth), screenHeight(screenHeight), gameTimeMS(0) {
	TCODConsole::initRoot(screenWidth, screenHeight, "Rainfall Online", false);
	map = new Map();

	player = new Actor(40, 25, 3, 3, '@', "player", 0.9, TCODColor::white);
	player->destructible = new PlayerDestructible(30, 2, "your cadaver");
	player->attacker = new Attacker(5);
	player->ai = new PlayerAi();
	player->container = new Container(26);

	engine.actors.push(player);

	ownDesc.x = player->x;
	ownDesc.y = player->y;
	ownDesc.mX = player->mX;
	ownDesc.mY = player->mY;
	ownDesc.ch = '@';
	ownDesc.r = player->col.r;
	ownDesc.g = player->col.g;
	ownDesc.b = player->col.b;
	ownName = "player";


	gui = new Gui();

	gui->message(TCODColor::red, "Welcome to Rainfall Online.");


	client.Connect("127.0.0.1", 60000);
	//client.Connect("18.219.35.213", 60000);
}

Engine::~Engine() {
	skills.clear();
	quests.clear();
	spells.clear();
	delete map;
	delete gui;
	actors.clearAndDelete();
	otherPlayers.clear();
}

void Engine::update() {
	TCODSystem::checkForEvent(TCOD_EVENT_KEY_PRESS|TCOD_EVENT_MOUSE, &lastKey, &mouse);
	for (Actor** iterator = engine.actors.begin(); iterator != engine.actors.end(); iterator++) {
		Actor* actor = *iterator;
		actor->update();
	}

	if (client.IsConnected()) {
		if (!client.Incoming().empty()) {
			auto msg = client.Incoming().pop_front().msg;

			switch (msg.header.id) {
				case GameMsg::Client_Accepted: {
					engine.gui->message(TCODColor::green, "Connected to server!");
					nanz::net::message<GameMsg> msg;
					msg.header.id = GameMsg::Client_RegisterWithServer;
					
					break;
				}

				case GameMsg::Client_AssignID: {
					msg >> nPlayerID;
					std::cout << "Assigned Client ID = " << nPlayerID << "\n";
					break;
				}

				case GameMsg::Game_AddPlayer: {
					sPlayerDescription desc;
					msg >> desc;
					otherPlayers.insert_or_assign(desc.nUniqueID, desc);

					if (desc.nUniqueID == nPlayerID) {
						bWaitingForConnection = false;
					}

					break;
				}

				case GameMsg::Game_RemovePlayer: {
					uint32_t nRemovalID = 0;
					msg >> nRemovalID;
					otherPlayers.erase(nRemovalID);
					break;
				}
				
				case GameMsg::Game_UpdatePlayer: {
					sPlayerDescription desc;
					msg >> desc;
					otherPlayers.insert_or_assign(desc.nUniqueID, desc);
					break;
				}

				case GameMsg::Chat_Message: {
					int length;
					msg >> length;

					std::string message;

					for (int i = 0; i < length; i++) {
						char letter;
						msg >> letter;
						message.push_back(letter);
					}
					std::reverse(message.begin(), message.end());

					engine.gui->message(TCODColor::yellow, &message[0]);
					break;
				}
			}
		}
	} else {
		engine.gui->message(TCODColor::red, "Server Down");
	}
}

void Engine::render() {
	TCODConsole::root->clear();

	map->render();

	for (Actor** i = engine.actors.begin(); i != engine.actors.end(); i++) {
		Actor* actor = *i;
		if (actor->mX == engine.player->mX && actor->mY == engine.player->mY)
			actor->render();
	}

	std::map<uint32_t, Actor*>::iterator it;

	for (auto& object: otherPlayers) {
		sPlayerDescription p = object.second;

		if (p.mX == ownDesc.mX && p.mY == ownDesc.mY) {
			TCODConsole::root->setChar(p.x, p.y, p.ch);
			TCODConsole::root->setCharForeground(p.x, p.y, TCODColor(p.r, p.g, p.b));
		}
	}
	
//	player->render();
	gui->render();
}

void Engine::sendToBack(Actor* actor) {
	engine.actors.remove(actor);
	engine.actors.insertBefore(actor, 0);
}