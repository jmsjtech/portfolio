class Actor;
class Map;

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
};

extern Engine engine;