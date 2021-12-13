static const int MAPTILE_WIDTH = 78;
static const int MAPTILE_HEIGHT = 42;

// This is for the actual tiles in a map
struct Tile {
    TileMeta metadata;

	TCODColor fg = TCODColor::darkerGreen; // Foreground color
	TCODColor bg = TCODColor::black; // Background color

	Tile() {}

    Tile(TileMeta meta) {
        metadata = meta;
        fg = TCODColor(meta.fgR, meta.fgG, meta.fgB);
        bg = TCODColor(meta.bgR, meta.bgG, meta.bgB);
    }

    void operator = (const Tile& copy) { 
        strcpy_s(metadata.name, copy.metadata.name);
        metadata.id = copy.metadata.id;
        metadata.blocksMove = copy.metadata.blocksMove;
        metadata.ch = copy.metadata.ch;
        fg = TCODColor(copy.fg.r, copy.fg.g, copy.fg.b);
        bg = TCODColor(copy.bg.r, copy.bg.g, copy.bg.b);
    }
};

// Minimap tiles specifically
struct MapTile {
    MapMeta metadata;
    TCODColor fg; // Foreground color
    TCODColor bg; // Background color

    std::map<std::pair<int, int>, ItemMeta> itemsOnMap;


    MapTile() : fg(TCODColor::black), bg(TCODColor::black) {}

    MapTile(MapMeta meta) {
        metadata = meta;
        fg = TCODColor(meta.fgR, meta.fgG, meta.fgB);
        bg = TCODColor(meta.bgR, meta.bgG, meta.bgB);
    }
};

class Map {
public:
    Map();
    ~Map();
    // Two tile returning and setting functions based on what numbers you've got
    Tile tileAt(int x, int y);
    Tile tileAtIndex(int index);

    void SetTile(Tile tile, int x, int y);
    void SetTileIndex(Tile tile, int index);

    bool canWalk(int x, int y) const; // Is a tile solid?
    void render(); //  Render the tile to the console
    int getWidth() const; // Return the Width constant for the map
    int getHeight() const; // Return the Height constant for the map

    std::array<Tile, MAPTILE_WIDTH * MAPTILE_HEIGHT> tiles;
protected:

    void addMonster(int x, int y, int mX, int mY);
    void addItem(int x, int y, int id);
};