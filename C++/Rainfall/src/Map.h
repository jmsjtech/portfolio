// This is for the actual tiles in a map
struct Tile {
	bool blocksMove; // Blocks movement?

	std::string name; // Tile name
	int ch; // Symbol

	TCODColor fg; // Foreground color
	TCODColor bg; // Background color

	Tile() : name("Grass"), ch(','), blocksMove(false), fg(TCODColor::darkerGreen), bg(TCODColor::black) {}

};

// Minimap tiles specifically
struct MapTile {
    std::string name; // Tile name
    int ch; // Symbol

    TCODColor fg; // Foreground color
    TCODColor bg; // Background color

    int mX = -99; // Position in the overworld
    int mY = -99; // Position in the overworld

    MapTile() : name("Void"), ch(' '), fg(TCODColor::black), bg(TCODColor::black) {}
};

class Map {
public:
    Map();
    ~Map();
    // Two tile returning and setting functions based on what numbers you've got
    Tile& tileAt(int x, int y) const;
    Tile& tileAtIndex(int index) const;

    void SetTile(Tile& tile, int x, int y) const;
    void SetTileIndex(Tile& tile, int index) const;

    bool canWalk(int x, int y) const; // Is a tile solid?
    void render() const; //  Render the tile to the console
    int getWidth() const; // Return the Width constant for the map
    int getHeight() const; // Return the Height constant for the map

    Tile* tiles;
protected:

    void addMonster(int x, int y, int mX, int mY);
    void addItem(int x, int y, int mX, int mY);
};