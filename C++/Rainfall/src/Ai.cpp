#include <stdio.h>
#include <math.h>
#include "main.h"

static const int TRACKING_TURNS = 3;


void PlayerAi::update(Actor* owner) {
	if (owner->destructible && owner->destructible->isDead()) {
		return;
	}

	int dx = 0, dy = 0;
	switch (engine.lastKey.vk) {
	case TCODK_UP:
	case TCODK_KP8:
		dy = -1;
		break;
	case TCODK_KP7: // UP-LEFT
		dx = -1;
		dy = -1;
		break;
	case TCODK_KP9: // UP-RIGHT
		dx = 1;
		dy = -1;
		break;
	case TCODK_DOWN:
	case TCODK_KP2:
		dy = 1;
		break;
	case TCODK_KP1: // DOWN-LEFT
		dx = -1;
		dy = 1;
		break;
	case TCODK_KP3: // DOWN-RIGHT
		dx = 1;
		dy = 1;
		break;
	case TCODK_LEFT:
	case TCODK_KP4:
		dx = -1;
		break;
	case TCODK_RIGHT:
	case TCODK_KP6:
		dx = 1;
		break;
	case TCODK_CHAR:
		handleActionKey(owner, engine.lastKey.c);
		break;
	default: break;
	}

	if (dx != 0 || dy != 0) {
		moveOrAttack(owner, owner->x + dx, owner->y + dy);
	}


	if (engine.mouse.lbutton_pressed) {
		// Clicked in the sidebar
		if (engine.mouse.cx > 80) {
			// Clicked in the inventory
			if (engine.mouse.cy < 27) {
				int invIndex = engine.mouse.cy - 1;
				if (invIndex >= 0 && invIndex < owner->container->inventory.size()) {
					Actor* actor = owner->container->inventory.get(invIndex);
					const char* itemName = actor->name;
					if (actor->pickable->use(actor, owner)) {
						engine.gui->message(TCODColor::lightYellow, "You used the %s.", itemName);
					}
				}
			}

			// Clicked to switch selected sidebar menu
			else if (engine.mouse.cy == 28) {
				if (engine.mouse.cx >= 81 && engine.mouse.cx <= 83) {
					engine.gui->selected = "Skills";
					engine.gui->selectScroll = 0;
				} else if (engine.mouse.cx >= 85 && engine.mouse.cx <= 87) {
					engine.gui->selected = "Magic";
					engine.gui->selectScroll = 0;
				} else if (engine.mouse.cx >= 89 && engine.mouse.cx <= 91) {
					engine.gui->selected = "Quests";
					engine.gui->selectScroll = 0;
				} else if (engine.mouse.cx >= 93 && engine.mouse.cx <= 95) {
					engine.gui->selected = "Equipment";
					engine.gui->selectScroll = 0;
				}
			}

			// Clicked somewhere in the sidebar
			else if (engine.mouse.cy > 28) {
				engine.gui->message(TCODColor::lightBlue, "Clicked at %d, %d", engine.mouse.cx, engine.mouse.cy);
				if (engine.gui->selected == "Skills") { // Clicks in the skills menu

				} else if (engine.gui->selected == "Magic") { // Clicks in the magic menu

				} else if (engine.gui->selected == "Quests") { // Clicks in the quest menu
					int questIndex = ((engine.mouse.cy - 32) / 2) + engine.gui->selectScroll;
					if (questIndex < engine.quests.size()) {
						engine.gui->message(TCODColor::lightBlue, "Clicked on quest: %s", engine.quests.get(questIndex));
					}
				} else if (engine.gui->selected == "Equipment") { // Clicks in the equipment menu

				}
			}
		} 
		
		
		
		
		else { // Clicked somewhere else
			engine.gui->message(TCODColor::lightBlue, "Clicked at %d, %d", engine.mouse.cx, engine.mouse.cy);
		}
	}

	if (engine.mouse.cx > 80 && engine.mouse.cy > 28) {
		if (engine.mouse.wheel_up && engine.gui->selectScroll > 0) {
			engine.gui->selectScroll--;
		} else if (engine.mouse.wheel_down) {
			engine.gui->selectScroll++;
		}
	}
}


bool PlayerAi::moveOrAttack(Actor* owner, int targetx, int targety) {
	if (owner->lastActed + (100 * owner->speed) > TCODSystem::getElapsedMilli()) {
		return false;
	} else {
		owner->lastActed = TCODSystem::getElapsedMilli();

		for (Actor** iterator = engine.actors.begin(); iterator != engine.actors.end(); iterator++) {
			Actor* actor = *iterator;
			if (actor->destructible && !actor->destructible->isDead() && actor->x == targetx && actor->y == targety) {
				owner->attacker->attack(owner, actor);
				return false;
			}
		}

		for (Actor** iterator = engine.actors.begin(); iterator != engine.actors.end(); iterator++) {
			Actor* actor = *iterator;
			bool corpseOrItem = (actor->destructible && actor->destructible->isDead()) || actor->pickable;
			if (corpseOrItem && actor->x == targetx && actor->y == targety) {
				engine.gui->message(TCODColor::lightGrey, "There's a %s here\n", actor->name);
			}
		}

		owner->x = targetx;
		owner->y = targety;
		return true;
	}
}

void PlayerAi::handleActionKey(Actor* owner, int ascii) {
	switch (ascii) {
		case 'g': // pick up something
		{
			bool found = false;
			for (Actor** iterator = engine.actors.begin(); iterator != engine.actors.end(); iterator++) {
				Actor* actor = *iterator;
				if (actor->pickable && actor->x == owner->x && actor->y == owner->y) {
					if (actor->pickable->pick(actor, owner)) {
						found = true;
						engine.gui->message(TCODColor::lightGrey, "You pick up the %s.", actor->name);
						break;
					} else if (!found) {
						found = true;
						engine.gui->message(TCODColor::red, "Your inventory is full.");
					}
				}
			}

			if (!found) {
				engine.gui->message(TCODColor::lightGrey, "There's nothing here that you can pick up.");
			}
			break;
		}

		case 'w': // move up
		{
			moveOrAttack(owner, owner->x, owner->y - 1);
			break;
		}

		case 's': // move up
		{
			moveOrAttack(owner, owner->x, owner->y + 1);
			break;
		}

		case 'a': // move up
		{
			moveOrAttack(owner, owner->x - 1, owner->y);
			break;
		}

		case 'd': // move up
		{
			moveOrAttack(owner, owner->x + 1, owner->y);
			break;
		}
	}
}

void MonsterAi::update(Actor* owner) {
	if (owner->destructible && owner->destructible->isDead()) {
		return;
	}

	moveOrAttack(owner, engine.player->x, engine.player->y);
}

void MonsterAi::moveOrAttack(Actor* owner, int targetx, int targety) {
	if (owner->lastActed + (100 * owner->speed) > TCODSystem::getElapsedMilli()) return;

	owner->lastActed = TCODSystem::getElapsedMilli();

	int dx = targetx - owner->x;
	int dy = targety - owner->y;
	int stepdx = (dx > 0 ? 1 : -1);
	int stepdy = (dy > 0 ? 1 : -1);
	float distance = sqrtf(dx * dx + dy * dy);

	if (distance >= 2) {
		dx = (int)(round(dx / distance));
		dy = (int)(round(dy / distance));
		
		if (engine.map->canWalk(owner->x + dx, owner->y + dy)) {
			owner->x += dx;
			owner->y += dy;
		} else if (engine.map->canWalk(owner->x + stepdx, owner->y)) {
			owner->x += stepdx;
		} else if (engine.map->canWalk(owner->x, owner->y + stepdy)) {
			owner->y += stepdy;
		}
	} else if (owner->attacker) {
		owner->attacker->attack(owner, engine.player);
	}
}