#include "main.h"
#include <stdio.h>

Engine::Engine(int screenWidth, int screenHeight) : screenWidth(screenWidth), screenHeight(screenHeight), gameTimeMS(0) {
	TCODConsole::initRoot(screenWidth, screenHeight, "Rainfall Online", false);
	map = new Map();

	player = new Actor(40, 25, 3, 3, '@', "player", 0.9, TCODColor::white);
	player->destructible = new PlayerDestructible(30, 2, "your cadaver");
	player->attacker = new Attacker(5);
	player->ai = new PlayerAi();
	player->container = new Container(26);

	engine.actors.push(player);


	gui = new Gui();

	gui->message(TCODColor::red, "Welcome to Rainfall Online.");
}

Engine::~Engine() {
	skills.clear();
	quests.clear();
	spells.clear();
	delete map;
	delete gui;
}

void Engine::update() {
	TCODSystem::checkForEvent(TCOD_EVENT_KEY_PRESS|TCOD_EVENT_MOUSE, &lastKey, &mouse);
	for (Actor** iterator = engine.actors.begin(); iterator != engine.actors.end(); iterator++) {
		Actor* actor = *iterator;
		actor->update();
	}
}

void Engine::render() {
	TCODConsole::root->clear();

	map->render();

	for (Actor** i = engine.actors.begin(); i != engine.actors.end(); i++) {
		Actor* actor = *i;
		actor->render();
	}
	
	player->render();
	gui->render();
}

void Engine::sendToBack(Actor* actor) {
	engine.actors.remove(actor);
	engine.actors.insertBefore(actor, 0);
}