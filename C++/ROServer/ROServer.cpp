#include <iostream>
#include <unordered_map>
#include <fstream>
#include <map>
#include <filesystem>

#include "Nanz_Net.h"
#include <NetCommon.h>

typedef std::pair<int, int> pairKey;
typedef std::pair < TileMeta, std::array<int, MAP_WIDTH* MAP_HEIGHT>> pairMap;

struct comp {
	template<typename T>
	bool operator()(const T& l, const T& r) const {
		if (l.first == r.first) {
			return l.second > r.second;
		}

		return l.first < r.first;
	}
};

bool IsTileWalkable(int mX, int mY, int x, int y);
bool EnsureMapExists(int mX, int mY);

class GameServer : public nanz::net::server_interface<GameMsg> {
public:
	GameServer(uint16_t nPort) : nanz::net::server_interface<GameMsg>(nPort) {
	}

	std::unordered_map<uint32_t, sPlayerDescription> m_mapPlayerRoster;
	std::unordered_map<int, TileMeta> tileLibrary;
	std::vector<uint32_t> m_vGarbageIDs;

	std::map<pairKey, pairMap, comp> maps;

protected:
	void SendMap(int mX, int mY, std::shared_ptr<nanz::net::connection<GameMsg>> client) {
		nanz::net::message<GameMsg> msg;
		msg.header.id = GameMsg::Send_Map;

		TileMeta mapIcon = maps.at(std::make_pair(mX, mY)).first;

		//msg << mapIcon.name << mapIcon.ch << mapIcon.fgR << mapIcon.fgG << mapIcon.fgB << mapIcon.bgR << mapIcon.bgG << mapIcon.bgB;
		msg << mY << mX;
		msg << mapIcon;
		msg << maps.at(std::make_pair(mX, mY)).second;

		MessageClient(client, msg);
		 
	}

	void SendMinimap(int mX, int mY, std::shared_ptr<nanz::net::connection<GameMsg>> client) {
		nanz::net::message<GameMsg> msg;
		msg.header.id = GameMsg::Send_Minimap;

		int num = 0;

		for (int y = mY - 8; y < mY + 8; y++) {
			for (int x = mX - 8; x < mX + 8; x++) {
				auto it = maps.find(std::make_pair(x, y));
				if (it == maps.end()) {
					// There's no map there already, just skip it
				} else {
					// There's a map there, load up all the data
					TileMeta mapIcon = maps.at(std::make_pair(x, y)).first;
					msg << y << x;
					msg << mapIcon;
					num++;
				}
			}
		}

		msg << num;
		msg << mY - 8 << mX - 8;

		MessageClient(client, msg);
	}

	bool OnClientConnect(std::shared_ptr<nanz::net::connection<GameMsg>> client) override {
		// For now we will allow all 
		return true;
	}

	void OnClientValidated(std::shared_ptr<nanz::net::connection<GameMsg>> client) override {
		// Client passed validation check, so send them a message informing
		// them they can continue to communicate
		nanz::net::message<GameMsg> msg;
		msg.header.id = GameMsg::Client_Accepted;
		client->Send(msg);
	}

	void OnClientDisconnect(std::shared_ptr<nanz::net::connection<GameMsg>> client) override {
		if (client) {
			if (m_mapPlayerRoster.find(client->GetID()) == m_mapPlayerRoster.end()) {
				// client never added to roster, so just let it disappear
			} else {
				auto& pd = m_mapPlayerRoster[client->GetID()];
				std::cout << "[UNGRACEFUL REMOVAL]:" + std::to_string(pd.nUniqueID) + "\n";
				m_mapPlayerRoster.erase(client->GetID());
				m_vGarbageIDs.push_back(client->GetID());
			}
		}

	}

	void OnMessage(std::shared_ptr<nanz::net::connection<GameMsg>> client, nanz::net::message<GameMsg>& msg) override {
		if (!m_vGarbageIDs.empty()) {
			for (auto pid : m_vGarbageIDs) {
				nanz::net::message<GameMsg> m;
				m.header.id = GameMsg::Game_RemovePlayer;
				m << pid;
				std::cout << "Removing " << pid << "\n";
				MessageAllClients(m);
			}
			m_vGarbageIDs.clear();
		}



		switch (msg.header.id) {
			case GameMsg::Client_RegisterWithServer:
			{
				sPlayerDescription desc;
				msg >> desc;
				desc.nUniqueID = client->GetID();
				m_mapPlayerRoster.insert_or_assign(desc.nUniqueID, desc);

				nanz::net::message<GameMsg> msgSendID;
				msgSendID.header.id = GameMsg::Client_AssignID;
				msgSendID << desc.nUniqueID;
				MessageClient(client, msgSendID);

				nanz::net::message<GameMsg> msgAddPlayer;
				msgAddPlayer.header.id = GameMsg::Game_AddPlayer;
				msgAddPlayer << desc;
				MessageAllClients(msgAddPlayer);

				nanz::net::message<GameMsg> msgAddOtherPlayers;
				msgAddOtherPlayers.header.id = GameMsg::PlayersOnMapTile;
				int count = 0;

				for (const auto& player : m_mapPlayerRoster) {
					if (player.second.mX == desc.mX && player.second.mY == desc.mY) {

						msgAddOtherPlayers.header.id = GameMsg::Game_AddPlayer;
						msgAddOtherPlayers << player.second;
					}
				}
				msgAddOtherPlayers << count;
				MessageClient(client, msgAddOtherPlayers);


				SendMap(desc.mX, desc.mY, client);
				SendMinimap(desc.mX, desc.mY, client);

				break;
			}

			case GameMsg::Client_UnregisterWithServer:
			{
				break;
			}

			case GameMsg::Game_UpdatePlayer:
			{
				// Simply bounce update to everyone except incoming client
				MessageAllClients(msg, client);

				sPlayerDescription desc;
				msg >> desc;
				m_mapPlayerRoster.insert_or_assign(desc.nUniqueID, desc);


				break;
			}

			case GameMsg::Chat_Message: {
				MessageAllClients(msg, client);
				break;
			}

			case GameMsg::Player_Move:
			{
				sPlayerDescription desc;
				int x_change;
				int y_change;
				msg >> y_change >> x_change >> desc;

				int mX_change = 0;
				int mY_change = 0;
				bool skipCheck = false;

				if (desc.x + x_change >= MAP_WIDTH) {
					desc.x = 0;
					desc.mX++;
					skipCheck = true;
				}

				if (desc.x + x_change < 0) {
					desc.x = MAP_WIDTH - 1;
					desc.mX--;
					skipCheck = true;
				}

				if (desc.y + y_change >= MAP_HEIGHT) {
					desc.y = 0;
					desc.mY++;
					skipCheck = true;
				}

				if (desc.y + y_change < 0) {
					desc.y = MAP_HEIGHT - 1;
					desc.mY--;
					skipCheck = true;
				}

				EnsureMapExists(desc.mX, desc.mY);

				if (!skipCheck) {
					if (IsTileWalkable(desc.mX, desc.mY, desc.x + x_change, desc.y + y_change)) {
						desc.x += x_change;
						desc.y += y_change;

						m_mapPlayerRoster.insert_or_assign(desc.nUniqueID, desc);

						nanz::net::message<GameMsg> msgMovePlayer;
						msgMovePlayer.header.id = GameMsg::Game_UpdatePlayer;
						msgMovePlayer << desc;
						MessageAllClients(msgMovePlayer);
					} else {
						// Throw out the message because the movement wasn't valid
					}
				} else {
					m_mapPlayerRoster.insert_or_assign(desc.nUniqueID, desc);

					nanz::net::message<GameMsg> msgMovePlayer;
					msgMovePlayer.header.id = GameMsg::Game_UpdatePlayer;
					msgMovePlayer << desc;
					MessageAllClients(msgMovePlayer);

					SendMap(desc.mX, desc.mY, client);
					SendMinimap(desc.mX, desc.mY, client);
				}

				break;
			}

			case GameMsg::PlayersOnMapTile:
			{
				int x, y;
				msg >> y >> x;

				nanz::net::message<GameMsg> msgSendList;
				msgSendList.header.id = GameMsg::PlayersOnMapTile;

				int count = 0;

				for (auto& object : m_mapPlayerRoster) {
					if (object.second.mX == x && object.second.mY == y) {
						msgSendList << object.second;
						count++;
					}
				}

				msgSendList << count;

				MessageClient(client, msgSendList);
				break;
			}
		}
	}
};

GameServer server(60000);

int main() {
	server.Start();


	std::ifstream tileFile("./data/tiles.dat");

	if (!tileFile.is_open()) { // If the file doesn't exist we can't load it, so just back out immediately
		std::cout << "[SERVER] Failed to load tiles from file.\n";
	} else {
		std::string line;
		int index = -1;
		int x = 0;
		int y = 0;
		while (std::getline(tileFile, line)) { // Read all the lines at a time
			std::string fromLine[10];

			for (int i = 0; i < 10; i++) {
				fromLine[i] = line.substr(0, line.find('|'));
				line.erase(0, line.find('|') + 1);
			}

			TileMeta newTile = *new TileMeta();

			newTile.id = atoi(fromLine[0].c_str());
			strcpy_s(newTile.name, fromLine[1].c_str());
			newTile.blocksMove = fromLine[2] == "true" ? true : false;
			newTile.ch = atoi(fromLine[3].c_str());

			newTile.fgR = atoi(fromLine[4].c_str());
			newTile.fgG = atoi(fromLine[5].c_str());
			newTile.fgB = atoi(fromLine[6].c_str());

			newTile.bgR = atoi(fromLine[7].c_str());
			newTile.bgG = atoi(fromLine[8].c_str());
			newTile.bgB = atoi(fromLine[9].c_str());


			server.tileLibrary.insert_or_assign(newTile.id, newTile);
		}

		std::cout << "[SERVER] Loaded " << server.tileLibrary.size() << " tiles.\n";
	}



	int mX;
	int mY;
	const std::filesystem::path mapsDir{ "./maps/" };

	
	for (auto const& dir_entry : std::filesystem::directory_iterator{ mapsDir }) {
		std::ifstream file(dir_entry);

		TileMeta newMap = *new TileMeta;
		std::array<int, MAP_WIDTH* MAP_HEIGHT> tiles;

		if (!file.is_open()) // If the file doesn't exist we can't load it, so just back out immediately
			continue;

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
				strcpy_s(newMap.name, fromLine[0].c_str());
				newMap.ch = atoi(fromLine[1].c_str());

				newMap.fgR = atoi(fromLine[2].c_str());
				newMap.fgG = atoi(fromLine[3].c_str());
				newMap.fgB = atoi(fromLine[4].c_str()); 

				newMap.bgR = atoi(fromLine[5].c_str());
				newMap.bgG = atoi(fromLine[6].c_str());
				newMap.bgB = atoi(fromLine[7].c_str()); 

				mX = atoi(fromLine[8].c_str());
				mY = atoi(fromLine[9].c_str());

				index++;
			} else { // Then switch to regular tile reading for the rest of the file
				tiles[index] = atoi(line.c_str());
				index++;
			}
		}

		server.maps.insert_or_assign(std::make_pair(mX, mY), std::make_pair(newMap, tiles));

		// Close the file
		file.close();

	}

	std::cout << "[SERVER] Loaded " << server.maps.size() << " maps from file.\n";





	while (1) {
		server.Update(-1, true);
	}
	return 0;
}


bool IsTileWalkable(int mX, int mY, int x, int y) {
	auto it = server.tileLibrary.find(server.maps[std::make_pair(mX, mY)].second[x + (y * MAP_WIDTH)]);
	if (it == server.tileLibrary.end()) {
		std::cout << "[SERVER] Failed to find tile in library.\n";
	} else {
		return !it->second.blocksMove;
	}
}

bool EnsureMapExists(int mX, int mY) {
	if (server.maps.find(std::make_pair(mX, mY)) == server.maps.end()) {
		// Map doesn't exist, let's make it
		TileMeta newMap = *new TileMeta;
		std::array<int, MAP_WIDTH* MAP_HEIGHT> tiles;

		for (auto& v : tiles)
			v = 0;

		strcpy_s(newMap.name, "Grass\0");
		newMap.ch = ',';
		newMap.fgG = 127;
		 
		server.maps.insert_or_assign(std::make_pair(mX, mY), std::make_pair(newMap, tiles));


		std::ofstream file;
		std::string fileName = "./maps/" + std::to_string(mX) + "," + std::to_string(mY) + ".dat"; // Use the coordinate in the name
		file.open(fileName, std::fstream::out);

		// Write the header line to save the minimap tile details
		file << newMap.name << "|" <<
			newMap.ch << "|" <<
			(int)newMap.fgR << "|" <<
			(int)newMap.fgG << "|" <<
			(int)newMap.fgB << "|" <<
			(int)newMap.bgR << "|" <<
			(int)newMap.bgG << "|" <<
			(int)newMap.bgB << "|" <<
			(int)mX << "|" <<
			(int)mY << "\n";

		// Write every tile to the file, one line at a time
		for (auto& tile : tiles) {
			file << tile << "\n";
		}

		// Close out the file
		file.close();



	} else {
		return true;
	}

	return false; // We should never reach this point but just in case we somehow do, we'll return false
}