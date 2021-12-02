struct Tile {
	bool blocksMove; // Blocks movement?

	const char* name; // Tile name
	int ch; // Symbol

	TCODColor fg; // Foreground color
	TCODColor bg; // Background color

	Tile() : name("Grass"), ch(','), blocksMove(false), fg(TCODColor::darkerGreen), bg(TCODColor::black) {}
};

class Map {
public:
    Map();
    ~Map();
    Tile tileAt(int x, int y) const;
    bool canWalk(int x, int y) const;
    void render() const;
protected:
    Tile* tiles;
    TCODMap* map;

    void addMonster(int x, int y, int mX, int mY);
    void addItem(int x, int y, int mX, int mY);
};