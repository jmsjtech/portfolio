#include "main.h" 
#include <fstream>

Engine::Engine(int screenWidth, int screenHeight) : screenWidth(screenWidth), screenHeight(screenHeight), gameTimeMS(0) {
	TCODConsole::initRoot(screenWidth, screenHeight, "Rainfall Online", false);
	map = new Map();

	for (auto & tile : map->tiles) {
		tile = *new Tile();
	}

	// Create the client player and put it in the map
	player = new Actor(40, 25, 3, 3, '@', "player", 0.9, TCODColor::white);
	player->destructible = new PlayerDestructible(30, 2, "your cadaver");
	player->attacker = new Attacker(5);
	player->ai = new PlayerAi();
	player->container = new Container(26);
	engine.actors.push(player);

	// Fill out the ownDesc variable to send it to the server later
	ownDesc.x = player->x;
	ownDesc.y = player->y;
	ownDesc.mX = player->mX;
	ownDesc.mY = player->mY;
	ownDesc.ch = '@';
	ownDesc.r = player->col.r;
	ownDesc.g = player->col.g;
	ownDesc.b = player->col.b;
	ownName = "player";

	// Initialize the GUI
	gui = new Gui();
	gui->message(TCODColor::red, "Welcome to Rainfall Online.");

	// Connect to the client
	client.Connect("127.0.0.1", 60000);
	//client.Connect("18.219.35.213", 60000);

	LoadTiles();
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

	// Update all the actors
	for (Actor** iterator = engine.actors.begin(); iterator != engine.actors.end(); iterator++) {
		Actor* actor = *iterator;
		actor->update();
	}

	// If the client is connected to the server, start processing messages
	if (client.IsConnected()) {
		if (!client.Incoming().empty()) {
			auto msg = client.Incoming().pop_front().msg;

			switch (msg.header.id) {
				case GameMsg::Client_Accepted: { // Verify that the server accepted the connection
					engine.gui->message(TCODColor::green, "Connected to server!");
					nanz::net::message<GameMsg> msg;
					msg.header.id = GameMsg::Client_RegisterWithServer;
					msg << engine.ownDesc;
					engine.client.Send(msg);
					
					break;
				}

				case GameMsg::Client_AssignID: { // Receive and save the unique ID sent by the server
					msg >> nPlayerID;
					ownDesc.nUniqueID = nPlayerID;
					std::cout << "Assigned Client ID = " << nPlayerID << "\n";

					nanz::net::message<GameMsg> msg;
					msg.header.id = GameMsg::PlayersOnMapTile;
					msg << engine.ownDesc.mX << engine.ownDesc.mY;
					engine.client.Send(msg);
					
					break;
				}

				case GameMsg::Game_AddPlayer: { // If a new player connects, add their info to the internal list
					sPlayerDescription desc;
					msg >> desc;
					otherPlayers.insert_or_assign(desc.nUniqueID, desc);

					if (desc.nUniqueID == nPlayerID) {
						bWaitingForConnection = false;
					}

					break;
				}

				case GameMsg::Game_RemovePlayer: { // Remove a disconnected player from the list
					uint32_t nRemovalID = 0;
					msg >> nRemovalID;
					otherPlayers.erase(nRemovalID);
					break;
				}
				
				case GameMsg::Game_UpdatePlayer: { // When a player needs updating, update their entry
					sPlayerDescription desc;
					msg >> desc;
					otherPlayers.insert_or_assign(desc.nUniqueID, desc);

					if (desc.nUniqueID == nPlayerID) {
						player->x = desc.x;
						player->y = desc.y;
						player->mX = desc.mX;
						player->mY = desc.mY;

						ownDesc.x = desc.x;
						ownDesc.y = desc.y;
						ownDesc.mX = desc.mX;
						ownDesc.mY = desc.mY;
					}

					break;
				}

				case GameMsg::Send_Map:
				{ // When a player needs updating, update their entry
					std::array<int, MAP_WIDTH* MAP_HEIGHT> map;
					TileMeta minimap;
					int mX, mY;
					msg >> map;
					msg >> minimap;
					msg >> mX >> mY;
					//msg >> minimap.bgB >> minimap.bgG >> minimap.bgR >> minimap.fgB >> minimap.fgG >> minimap.fgR >> minimap.ch >> minimap.name;


					for (int i = 0; i < map.size(); i++) {
						auto it = tileLibrary.find(map[i]);
						if (it == tileLibrary.end()) {
							std::cout << "Failed to find tile in library.\n";
						} else {
							engine.map->SetTileIndex(it->second, i);
						}
					}

					break;
				}

				case GameMsg::Send_Minimap:
				{ // When a player needs updating, update their entry
					msg >> engine.topleft_x >> engine.topleft_y;
					int amountMaps;
					msg >> amountMaps;

					for (auto m : mini) {
						m.name = "Void";
					}

					for (int i = 0; i < amountMaps; i++) {
						TileMeta minimap;
						int mX, mY;
						msg >> minimap;
						msg >> mX >> mY;

						MapTile mapTile = *new MapTile();
						mapTile.name = minimap.name;
						mapTile.ch = minimap.ch;
						mapTile.mX = mX;
						mapTile.mY = mY;
						mapTile.fg = TCODColor(minimap.fgR, minimap.fgG, minimap.fgB);
						mapTile.bg = TCODColor(minimap.bgR, minimap.bgG, minimap.bgB);
						 
						mini[(mX - engine.topleft_x) + ((mY - engine.topleft_y) * 17)] = mapTile;
					}

					break;
				}

				case GameMsg::Chat_Message: { // Receive a chat message sent by another player
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

				case GameMsg::PlayersOnMapTile: // Receive the list of all players on a map tile
				{
					int count;

					msg >> count;
					otherPlayers.clear(); // Also we can assume that this means you've moved map tiles, so clear out the list

					for (int i = 0; i < count; i++) {
						sPlayerDescription desc;
						msg >> desc;
						if (desc.nUniqueID != engine.ownDesc.nUniqueID)
							otherPlayers.insert_or_assign(desc.nUniqueID, desc);
					}

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

	map->render(); // Render the map
	
	// Then all actors
	for (Actor** i = engine.actors.begin(); i != engine.actors.end(); i++) {
		Actor* actor = *i;
		if (actor->mX == engine.player->mX && actor->mY == engine.player->mY)
			actor->render();
	}

	std::map<uint32_t, Actor*>::iterator it;
	// Then all players
	for (auto& object: otherPlayers) {
		sPlayerDescription p = object.second;

		if (p.mX == ownDesc.mX && p.mY == ownDesc.mY && p.nUniqueID != engine.ownDesc.nUniqueID) {
			TCODConsole::root->setChar(p.x, p.y, p.ch);
			TCODConsole::root->setCharForeground(p.x, p.y, TCODColor(p.r, p.g, p.b));
		}
	}
	
	// And finally the GUI itself
	gui->render();
}

void Engine::sendToBack(Actor* actor) {
	engine.actors.remove(actor);
	engine.actors.insertBefore(actor, 0);
}


// Set all the map editor tile variables back to default grass
void Engine::ResetMapEdit() const {
	engine.mapEdit_ch = ',';
	engine.mapEdit_name = "Grass";
	engine.mapEdit_fg_r = TCODColor::darkerGreen.r;
	engine.mapEdit_fg_g = TCODColor::darkerGreen.g;
	engine.mapEdit_fg_b = TCODColor::darkerGreen.b;
	engine.mapEdit_bg_r, engine.mapEdit_bg_g, engine.mapEdit_bg_b = 0;
	engine.mapEdit_blocksMove = false;
}

void Engine::LoadTiles() { 
	std::ifstream file("./data/tiles.dat");

	if (!file.is_open()) { // If the file doesn't exist we can't load it, so just back out immediately
		engine.gui->message(TCODColor::cyan, "Failed to load tiles from file");
		return;
	}

	std::string line;
	int index = -1;
	int x = 0;
	int y = 0;
	while (std::getline(file, line)) { // Read all the lines at a time
		std::string fromLine[10];

		for (int i = 0; i < 10; i++) {
			fromLine[i] = line.substr(0, line.find('|'));
			line.erase(0, line.find('|') + 1);
		}

		Tile* newTile = new Tile();

		newTile->metadata.id = atoi(fromLine[0].c_str());
		strcpy_s(newTile->metadata.name, fromLine[1].c_str());
		newTile->metadata.blocksMove = fromLine[2] == "true" ? true : false;
		newTile->metadata.ch = atoi(fromLine[3].c_str());

		int fr = atoi(fromLine[4].c_str());
		int fg = atoi(fromLine[5].c_str());
		int fb = atoi(fromLine[6].c_str());

		newTile->fg = TCODColor(fr, fg, fb);

		int br = atoi(fromLine[7].c_str());
		int bg = atoi(fromLine[8].c_str());
		int bb = atoi(fromLine[9].c_str());

		newTile->bg = TCODColor(br, bg, bb);

		tileLibrary.insert_or_assign(newTile->metadata.id, Tile(*newTile));
	}
}