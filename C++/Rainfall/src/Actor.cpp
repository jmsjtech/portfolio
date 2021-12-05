#include "main.h"

Actor::Actor(int x, int y, int mX, int mY, int ch, const char *name, float speed, const TCODColor& col) :
	x(x), y(y), mX(mX), mY(mY), ch(ch), col(col), name(name), speed(speed), blocks(true), attacker(NULL), destructible(NULL), ai(NULL), pickable(NULL), container(NULL) {
}

void Actor::render() const {
	TCODConsole::root->setChar(x, y, ch);
	TCODConsole::root->setCharForeground(x, y, col);
}

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