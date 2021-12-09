#include "main.h" 
#include <fstream>

Engine::Engine(int screenWidth, int screenHeight) : screenWidth(screenWidth), screenHeight(screenHeight), gameTimeMS(0) {
	TCODConsole::initRoot(screenWidth, screenHeight, "Rainfall Online", false);
	map = new Map();

	// Create the client player and put it in the map
	player = new Actor(40, 25, 3, 3, '@', "player", 0.9, TCODColor::white);
	player->destructible = new PlayerDestructible(30, 2, "your cadaver");
	player->attacker = new Attacker(5);
	player->ai = new PlayerAi();
	player->container = new Container(26);
	engine.actors.push(player);

	// Fill out the ownDesc variable to send it to the server later
	ownDesc.pos.x = player->pos.x;
	ownDesc.pos.y = player->pos.y;
	ownDesc.worldPos.x = player->worldPos.x;
	ownDesc.worldPos.y = player->worldPos.y;
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


	//InitTileDatabase();


	// Load the current map tile from file, then update the minimap
	//LoadMap(ownDesc.worldPos.x, ownDesc.worldPos.y, false);
	UpdateMinimap();
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

				case GameMsg::Client_AssignID: { // Recieve and save the unique ID sent by the server
					msg >> nPlayerID;
					ownDesc.nUniqueID = nPlayerID;
					std::cout << "Assigned Client ID = " << nPlayerID << "\n";

					nanz::net::message<GameMsg> msg;
					msg.header.id = GameMsg::PlayersOnMapTile;
					msg << engine.ownDesc.worldPos.x << engine.ownDesc.worldPos.y;
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
					break;
				}

				case GameMsg::Player_Move:
				{ 
					sPlayerDescription desc;
					msg >> desc;
					otherPlayers.insert_or_assign(desc.nUniqueID, desc);

					if (desc.nUniqueID == engine.nPlayerID) {
						UpdateOwnPosition(desc);
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

void Engine::UpdateOwnPosition(sPlayerDescription desc) {
	engine.ownDesc.pos.x = desc.pos.x;
	engine.ownDesc.pos.y = desc.pos.y;
	engine.ownDesc.worldPos.x = desc.worldPos.x;
	engine.ownDesc.worldPos.y = desc.worldPos.y;

	engine.player->pos.x = desc.pos.x;
	engine.player->pos.y = desc.pos.y;
	engine.player->worldPos.x = desc.worldPos.x;
	engine.player->worldPos.y = desc.worldPos.y;
}

void Engine::render() {
	TCODConsole::root->clear();

	map->render(); // Render the map
	
	// Then all actors
	for (Actor** i = engine.actors.begin(); i != engine.actors.end(); i++) {
		Actor* actor = *i;
		if (actor->worldPos.x == engine.player->worldPos.x && actor->worldPos.y == engine.player->worldPos.y)
			actor->render();
	}

	std::map<uint32_t, Actor*>::iterator it;
	// Then all players
	for (auto& object: otherPlayers) {
		sPlayerDescription p = object.second;

		if (p.worldPos.x == ownDesc.worldPos.x && p.worldPos.y == ownDesc.worldPos.y && p.nUniqueID != engine.ownDesc.nUniqueID) {
			TCODConsole::root->setChar(p.pos.x, p.pos.y, p.ch);
			TCODConsole::root->setCharForeground(p.pos.x, p.pos.y, TCODColor(p.r, p.g, p.b));
		}
	}
	
	// And finally the GUI itself
	gui->render();
}

void Engine::sendToBack(Actor* actor) {
	engine.actors.remove(actor);
	engine.actors.insertBefore(actor, 0);
}


void Engine::NewMapAt(int mX, int mY) const {
	Tile* tiles = new Tile[engine.map->getWidth() * engine.map->getHeight()]; // Make a new tile array

	// Set all the minimap defaults - if we're making a new overworld tile, grass is a decent guess
	MapTile* mapTile = &engine.minimap[mX][mY];
	mapTile->worldPos.x = mX;
	mapTile->worldPos.y = mY;
	mapTile->name = "Grass";
	mapTile->ch = ',';
	mapTile->fg = TCODColor::darkerGreen;

	// Print that the map was made successfully
	engine.gui->message(TCODColor::cyan, "New map at %d, %d", engine.minimap[mX][mY].worldPos.x, engine.minimap[mX][mY].worldPos.y);

	// Just for posterity so we aren't making maps twice or having to manually save every time, save the map and then reload it
	SaveMap(mX, mY, tiles);
	LoadMap(mX, mY, true);
}


// Save the specified map to file
void Engine::SaveMap(int mX, int mY, Tile* tiles) const {
	std::ofstream file;
	std::string fileName = "./maps/" + std::to_string(mX) + "," + std::to_string(mY) + ".dat"; // Use the coordinate in the name
	file.open(fileName, std::fstream::out);
	
	// Write the header line to save the minimap tile details
	MapTile tile = engine.minimap[engine.topleft.x + 8][engine.topleft.y + 8];
	file << tile.name << "|" << 
		tile.ch << "|" << 
		(int)tile.fg.r << "|" <<
		(int)tile.fg.g << "|" <<
		(int)tile.fg.b << "|" <<
		(int)tile.bg.r << "|" <<
		(int)tile.bg.g << "|" <<
		(int)tile.bg.b << "|" <<
		(int)mX << "|" <<
		(int)mY << "\n";

	// Write every tile to the file, one line at a time
	for (int i = 0; i < engine.map->getWidth() * engine.map->getHeight(); i++) {
		Tile tile = tiles[i];
		file << tile.metadata.name << "|\n";
	}

	// Close out the file
	file.close();
}

// Load the specified map from file, allow the user to specify whether or not they need just the minimap info
bool Engine::LoadMap(int mX, int mY, bool justHeader) const {
	std::string fileName = "./maps/" + std::to_string(mX) + "," + std::to_string(mY) + ".dat"; // Construct the file name from the coordinates
	std::ifstream file(fileName);

	if (!file.is_open()) { // If the file doesn't exist we can't load it, so just back out immediately
		return false;
	}

	std::string line;
	int index = -1;
	int x = 0; 
	int y = 0;
	while (std::getline(file, line)) { // Read all the lines at a time
		if (index < 0) { // Very first line should always be the header, so treat that differently
			std::string fromLine[10];

			for (int i = 0; i < 10; i++) {
				fromLine[i] = line.substr(0, line.find('|'));
				line.erase(0, line.find('|') + 1);
			}
			engine.mini_name = fromLine[0];
			engine.mini_ch = atoi(fromLine[1].c_str());

			int fr = atoi(fromLine[2].c_str());
			int fg = atoi(fromLine[3].c_str());
			int fb = atoi(fromLine[4].c_str());

			engine.mini_fg = TCODColor(fr, fg, fb);

			int br = atoi(fromLine[5].c_str());
			int bg = atoi(fromLine[6].c_str());
			int bb = atoi(fromLine[7].c_str());

			engine.mini_bg = TCODColor(br, bg, bb);

			int mX = atoi(fromLine[8].c_str());
			int mY = atoi(fromLine[9].c_str());

			engine.mini_pos.x = mX;
			engine.mini_pos.y = mY;

			if (justHeader) {
				return true;
			}
			index++;
		} else { // Then switch to regular tile reading for the rest of the file
			std::string fromLine[1];

			for (int i = 0; i < 1; i++) {
				fromLine[i] = line.substr(0, line.find('|'));
				line.erase(0, line.find('|') + 1);
			}

			//engine.map->tiles[index] = Tile(tileLibrary.at(fromLine[0].c_str()));

			index++;
			//x = index % engine.map->getWidth();
			//y = index / engine.map->getWidth();
		}
	}

	// Close the file
	file.close();

	// A file was read, so return true just in case we need it
	return true;
}

// Reset the topleft-most tile to the right spot, then for every position load the map header and set the info
void Engine::UpdateMinimap() {
	topleft.x = ownDesc.worldPos.x - 8;
	topleft.y = ownDesc.worldPos.y - 8;

	for (int y = engine.topleft.y; y < engine.topleft.y + 17; y++) {
		for (int x = engine.topleft.x; x < engine.topleft.x + 17; x++) {
			if (LoadMap(x, y, true)) {
				engine.minimap[x][y].name = mini_name;
				engine.minimap[x][y].ch = mini_ch;
				engine.minimap[x][y].fg = mini_fg;
				engine.minimap[x][y].bg = mini_bg;
				engine.minimap[x][y].worldPos.x = mini_pos.x;
				engine.minimap[x][y].worldPos.y = mini_pos.y;
			}
		}
	}
}



void Engine::InitTileDatabase() {
	 
	std::ifstream file("./tiles.dat");

	if (!file.is_open()) { // If the file doesn't exist we can't load it, so just back out immediately
		engine.gui->message(TCODColor::cyan, "Failed to load tile file. %s", std::string("./tiles.dat"));
		return;
	}

	std::string line;
	int index = -1;
	int x = 0;
	int y = 0;
	while (std::getline(file, line)) {
		Tile* tile = new Tile();
		std::string fromLine[5];

		for (int i = 0; i < 5; i++) {
			fromLine[i] = line.substr(0, line.find('|'));
			line.erase(0, line.find('|') + 1);
		}

		tile->metadata.name = fromLine[0];
		tile->metadata.blocksMove = fromLine[1] == "true" ? true : false;
		tile->ch = atoi(fromLine[2].c_str());

		int fgR = atoi(fromLine[3].substr(0, fromLine[3].find(',')).c_str());
		fromLine[3].erase(0, fromLine[3].find(',') + 1);
		int fgG = atoi(fromLine[3].substr(0, fromLine[3].find(',')).c_str());
		fromLine[3].erase(0, fromLine[3].find(',') + 1);
		int fgB = atoi(fromLine[3].substr(0, fromLine[3].find(',')).c_str()); 
		tile->fg = TCODColor(fgR, fgG, fgB);

		int bgR = atoi(fromLine[4].substr(0, fromLine[4].find(',')).c_str());
		fromLine[4].erase(0, fromLine[4].find(',') + 1);
		int bgG = atoi(fromLine[4].substr(0, fromLine[4].find(',')).c_str());
		fromLine[4].erase(0, fromLine[4].find(',') + 1);
		int bgB = atoi(fromLine[4].substr(0, fromLine[4].find(',')).c_str());
		tile->bg = TCODColor(bgR, bgG, bgB);
		
	//	tileLibrary.insert_or_assign(tile->metadata.name, new Tile(tile));
		
		engine.gui->message(TCODColor::cyan, "Added %s to the tile library.", tile->metadata.name);
	}
}