#include "main.h"


// Default Constructor
Actor::Actor(int x, int y, int mX, int mY, int ch, const char *name, float speed, const TCODColor& col) :
	pos(x, y), worldPos(mX, mY), ch(ch), col(col), name(name), speed(speed), blocks(true), attacker(NULL), destructible(NULL), ai(NULL), pickable(NULL), container(NULL) {
}

// Render the actor onto the root console
void Actor::render() const {
	TCODConsole::root->setChar(pos.x, pos.y, ch);
	TCODConsole::root->setCharForeground(pos.x, pos.y, col);
}

// Run the update code for things that can act
void Actor::update() {
	if (ai) ai->update(this);
}

Actor::~Actor() {
	if (attacker) delete attacker;
	if (destructible) delete destructible;
	if (ai) delete ai;
	if (pickable) delete pickable;
	if (container) delete container;
}