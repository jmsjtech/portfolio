#include "main.h"

bool Pickable::pick(Actor* owner, Actor* wearer) {
	if (wearer->container && wearer->container->add(owner)) {
		engine.actors.remove(owner);
		return true;
	}

	return false;
}

bool Pickable::use(Actor* owner, Actor* wearer) {
	if (wearer->container) {
		wearer->container->remove(owner);
		delete owner;
		return true;
	}

	return false;
}