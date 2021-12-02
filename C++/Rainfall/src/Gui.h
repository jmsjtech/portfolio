class Gui {
public:
	const char* selected;
	int selectScroll;
	Gui();
	~Gui();
	void render();
	void message(const TCODColor& col, const char* text, ...);
protected:
	TCODConsole* con;
	TCODConsole* inv;
	TCODConsole* sidebar;
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