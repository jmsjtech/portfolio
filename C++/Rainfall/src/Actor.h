class Actor {
public:
	Position pos;
	Position worldPos;
	int ch; // ascii code
	TCODColor col; // color
	const char* name; // the actor's name
	float speed; // speed multiplier
	int lastActed; // last gametime an action was taken
	bool blocks; // does this actor block movement?
	Attacker* attacker; // something that deals damage
	Destructible* destructible; // something that can take damage
	Ai* ai; // something self-updating
	Pickable* pickable; // something that can be picked up
	Container* container; // something that can contain actors

	Actor(int x, int y, int mX, int mY, int ch, const char *name, float speed, const TCODColor& col);
	~Actor();
	void update();
	void render() const;
};