// This is for the actual tiles in a map
struct Tile {
    TileMeta metadata = TileMeta(0, "Grass\0", false);
    int ch = ',';

	TCODColor fg = TCODColor::darkerGreen; // Foreground color
	TCODColor bg = TCODColor::black; // Background color

	Tile() {}

    void operator = (const Tile& copy) { 
        metadata.name = copy.metadata.name;
        metadata.id = copy.metadata.id;
        metadata.blocksMove = copy.metadata.blocksMove;

        ch = copy.ch;
        fg = TCODColor(copy.fg.r, copy.fg.g, copy.fg.b);
        bg = TCODColor(copy.bg.r, copy.bg.g, copy.bg.b);
    }
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