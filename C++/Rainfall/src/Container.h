class Container {
public:
	int size; // How many things it can hold
	TCODList<Actor*> inventory; // Things in the container

	Container(int size);
	~Container();
	bool add(Actor* actor);
	void remove(Actor* actor);
};