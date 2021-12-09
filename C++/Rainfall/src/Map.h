// This is for the actual tiles in a map 
struct Tile {
    int ch; // Symbol
    TCODColor fg; // Foreground color
    TCODColor bg; // Background color

    TileMeta metadata; // The metadata for the tile

    Tile() : ch(','), fg(TCODColor::darkerGreen), bg(TCODColor::black) {}

    Tile (const Tile* t1) { 
        ch = t1->ch; 
        fg = TCODColor(t1->fg.r, t1->fg.g, t1->fg.b);
        bg = TCODColor(t1->bg.r, t1->bg.g, t1->bg.b);
        metadata.name = t1->metadata.name;
        metadata.blocksMove = t1->metadata.blocksMove;
    }
};

// Minimap tiles specifically
struct MapTile {
    std::string name; // Tile name
    int ch; // Symbol

    TCODColor fg; // Foreground color
    TCODColor bg; // Background color

    Position worldPos;

    MapTile() : name("Void"), ch(' '), fg(TCODColor::black), bg(TCODColor::black) {}
};

class Map {
public:
    Map() { tiles = new Tile[MAPTILE_WIDTH * MAPTILE_HEIGHT]; }
    ~Map() { delete[] tiles; }

    static const int MAPTILE_WIDTH = 78;
    static const int MAPTILE_HEIGHT = 42;

    void SetTile(Tile& tile, int x, int y) const;
    void SetTileIndex(Tile& tile, int index) const;

    // Two tile returning and setting functions based on what numbers you've got
    Tile& tileAt(int x, int y) const;
    Tile& tileAtIndex(int index) const;

    void render() const;

    int getWidth() { return MAPTILE_WIDTH; } // Return the Width constant for the map
    int getHeight() { return MAPTILE_HEIGHT; } // Return the Height constant for the map

    Tile* tiles;
    MapTile* minimapTile;
};