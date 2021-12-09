class Gui {
public: 
	const char* selected; // Which sidebar menu is selected
	int selectScroll; // Where the scroll is for the current menu
	bool chatSelected = false; // Whether or not the player has chat selected
	Position miniHover; // Position hovered over on the minimap
	std::string* chatBuffer; // What the player currently has typed in the chat bar
	Gui();
	~Gui();
	void render();
	void message(const TCODColor& col, const char* text, ...);
protected:
	TCODConsole* con; // The message log and minimap area
	TCODConsole* inv; // The inventory window
	TCODConsole* sidebar; // The sidebar menu
	struct Message {
		char* text;
		TCODColor col;
		Message(const char* text, const TCODColor& col);
		~Message();
	};
	TCODList<Message*> log;

	void renderBar(int x, int y, int width, const char* name, float value, float maxValue, const TCODColor& barColor, const TCODColor& backColor);
	void renderMouseLook();
};