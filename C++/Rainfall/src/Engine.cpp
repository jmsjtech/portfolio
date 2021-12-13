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

					nanz::net::message<GameMsg> itemsMsg;
					itemsMsg.header.id = GameMsg::ItemRequest;
					engine.client.Send(itemsMsg);
					
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
					std::array<int, MAP_WIDTH* MAP_HEIGHT> items;
					TileMeta minimap;
					int mX, mY;
					msg >> items;
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

					engine.mini[8 + (8 * 17)].itemsOnMap.clear();

					for (int i = 0; i < items.size(); i++) {
						if (items[i] != -1) {
							auto it = itemLibrary.find(items[i]);
							if (it == itemLibrary.end()) {
								std::cout << "Failed to find item in library.\n";
							} else {
								int x, y;
								x = i % MAP_WIDTH;
								y = i / MAP_WIDTH;
								engine.mini[8 + (8 * 17)].itemsOnMap.insert_or_assign(std::make_pair(x, y), itemLibrary.at(items[i]));
							}
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
						strcpy_s(m.metadata.name, "Void");
					}

					for (int i = 0; i < amountMaps; i++) {
						TileMeta minimap;
						int mX, mY;
						msg >> minimap;
						msg >> mX >> mY;

						MapTile mapTile = *new MapTile();
						strcpy_s(mapTile.metadata.name, minimap.name);
						mapTile.metadata.ch = minimap.ch;
						mapTile.metadata.mX = mX;
						mapTile.metadata.mY = mY;
						mapTile.fg = TCODColor(minimap.fgR, minimap.fgG, minimap.fgB);
						mapTile.bg = TCODColor(minimap.bgR, minimap.bgG, minimap.bgB);
						
						int index = (mX - engine.topleft_x) + ((mY - engine.topleft_y) * 17);

						if (index < 289 && index >= 0) {
							mini[(mX - engine.topleft_x) + ((mY - engine.topleft_y) * 17)] = mapTile;
						}
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

				case GameMsg::Add_Item: // Receive the list of all players on a map tile
				{
					int id, x, y, mX, mY;

					msg >> id >> mY >> mX >> y >> x;

					if (itemLibrary.find(id) != itemLibrary.end()) {
						engine.mini[8 + (8 * 17)].itemsOnMap.insert_or_assign(std::make_pair(x, y), itemLibrary.at(id));
					} 

					break;
				}

				case GameMsg::ItemRequest:
				{ // Remove a disconnected player from the list
					int amount;
					msg >> amount;

					itemLibrary.clear();

					for (int i = 0; i < amount; i++) {
						ItemMeta newItem;
						msg >> newItem;


						itemLibrary.insert_or_assign(newItem.id, newItem);
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
	

	// Then all items
	for (auto& item : mini[8 + (8 * 17)].itemsOnMap) {
		TCODColor fg = TCODColor(item.second.fgR, item.second.fgG, item.second.fgB);
		TCODColor bg = TCODColor(item.second.bgR, item.second.bgG, item.second.bgB);

		TCODConsole::root->setChar(item.first.first, item.first.second, item.second.ch);
		TCODConsole::root->setCharForeground(item.first.first, item.first.second, fg);
		TCODConsole::root->setCharBackground(item.first.first, item.first.second, bg);
	}



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

EquipSlot Engine::slotFromID(int id) {
	switch (id) {
		case 0:
			return MAIN_HAND;
			break;
		case 1:
			return OFF_HAND;
			break;
		case 2:
			return HEAD;
			break;
		case 3:
			return TORSO;
			break;
		case 4:
			return LEGS;
			break;
		case 5:
			return HANDS;
			break;
		case 6:
			return FEET;
			break;
		case 7:
			return RING;
			break;
		case 8:
			return AMULET;
			break;
		case 9:
			return CAPE;
			break;
		default:
			return NOT_EQUIPPABLE;
			break;
	}
}


ItemCategory Engine::catFromID(int id) {
	switch (id) {
		case 0:
			return ItemCategory::HATCHET;
			break;
		case 1:
			return ItemCategory::PICKAXE;
			break;
		case 2:
			return ItemCategory::KNIFE;
			break;
		case 3:
			return ItemCategory::HAMMER;
			break;
		case 4:
			return ItemCategory::FISHING;
			break;
		case 5:
			return ItemCategory::POTION;
			break;
		case 6:
			return ItemCategory::FOOD;
			break;
		case 7:
			return ItemCategory::RUNE;
			break;
		default:
			return ItemCategory::ERROR_ITEM;
			break;
	}
}