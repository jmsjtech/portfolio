#include <stdio.h>
#include <math.h>
#include "main.h"

static const int TRACKING_TURNS = 3;


// The player update function
void PlayerAi::update(Actor* owner) {
	if (owner->destructible && owner->destructible->isDead()) {
		return;
	}

	int dx = 0, dy = 0;
	 
	if (!engine.gui->chatSelected) { // Handle key presses when the chat isn't selected
		switch (engine.lastKey.vk) {
		case TCODK_UP:
		case TCODK_KP8: // UP
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
		case TCODK_KP2: // DOWN
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
		case TCODK_KP4: // LEFT
			dx = -1;
			break;
		case TCODK_RIGHT:
		case TCODK_KP6: // RIGHT
			dx = 1;
			break;
		case TCODK_CHAR: // Pretty much any a-z or some symbols/non-keypad numbers
			handleActionKey(owner, engine.lastKey.c);
			break;
		default: break;
		}
	} else { // If the chat is selected keys should be sent there instead of doing their normal hotkeys
		std::string concat;
		switch (engine.lastKey.vk) {
			case TCODK_ESCAPE: // Deselect the chat
				engine.gui->chatSelected = false;
				break;
			case TCODK_SPACE: // Add a space to the chat
				engine.gui->chatBuffer->append(" ");
				break;
			case TCODK_BACKSPACE: // Remove the last character in the chat bar as long as there's characters to remove
				if (engine.gui->chatBuffer->length() > 0)
					engine.gui->chatBuffer->pop_back();
				break;
			case TCODK_ENTER: // Send the current message
				engine.gui->chatSelected = false;
				if (!IsCommand()) { // If the chat isn't a command, send it to other players and the message log
					engine.client.ChatMessage(*engine.gui->chatBuffer, engine.ownName.c_str());
					concat = std::string(engine.ownName.c_str()) + ": " + std::string(*engine.gui->chatBuffer);
					engine.gui->message(TCODColor::lightBlue, concat.c_str());
				}

				engine.gui->chatBuffer->clear(); // Either way clear the chat
				break;
			case TCODK_0:
			case TCODK_1:
			case TCODK_2:
			case TCODK_3:
			case TCODK_4:
			case TCODK_5:
			case TCODK_6:
			case TCODK_7:
			case TCODK_8:
			case TCODK_9:
			case TCODK_CHAR: // If any number 0-9 or a letter is pressed, add it to the chat message
				if (!engine.lastKey.shift)
					engine.gui->chatBuffer->push_back(engine.lastKey.c);
				else
					engine.gui->chatBuffer->push_back(engine.lastKey.c - 32);
				break;
			default: break;
		}
	}

	if (dx != 0 || dy != 0) { // If the player pressed a movement key, send it along
		engine.pathing = false;
		moveOrAttack(owner, dx, dy);
	}

	// Left-click handling
	if (engine.mouse.lbutton_pressed) {
		// Clicked in the sidebar
		if (engine.mouse.cx > 80) {
			// Clicked in the inventory
			if (engine.mouse.cy < 27) {
				int invIndex = engine.mouse.cy - 1;
				// If you clicked an item, try to use it
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
				} else if (engine.mouse.cx >= 97 && engine.mouse.cx <= 99) {
					engine.gui->selected = "Map Editor";
					engine.gui->selectScroll = 0;
				}
			}
			// Clicked somewhere in the sidebar
			else if (engine.mouse.cy > 28) {
				if (engine.gui->selected == "Skills") { // Clicks in the skills menu

				} else if (engine.gui->selected == "Magic") { // Clicks in the magic menu

				} else if (engine.gui->selected == "Quests") { // Clicks in the quest menu
					int questIndex = ((engine.mouse.cy - 32) / 2) + engine.gui->selectScroll;
					if (questIndex < engine.quests.size()) {
						engine.gui->message(TCODColor::lightBlue, "Clicked on quest: %s", engine.quests.get(questIndex));
					}
				} else if (engine.gui->selected == "Equipment") { // Clicks in the equipment menu

				} else if (engine.gui->selected == "Map Editor") { // Clicks in the equipment menu
					if (engine.mouse.cy == 68) {}
				}
			}
		}

		// Clicked to start chatting
		else if (engine.mouse.cx >= 27 && engine.mouse.cx <= 79 && engine.mouse.cy >= 67 && engine.mouse.cy <= 69) {
			engine.gui->chatSelected = true;
			engine.gui->chatBuffer = new std::string;
		}

		// Clicked somewhere on the map
		else if (engine.mouse.cx >= 0 && engine.mouse.cx <= 79 && engine.mouse.cy >= 0 && engine.mouse.cy <= 42) {
			if (!(engine.gui->selected == "Map Editor")) { // If you aren't editing the map, start pathing to the clicked location
				engine.pathing = true;
				engine.pathToX = engine.mouse.cx;
				engine.pathToY = engine.mouse.cy;
			} 
		}

		else { // Clicked somewhere else
		engine.gui->message(TCODColor::lightBlue, "Clicked at %d, %d", engine.mouse.cx, engine.mouse.cy);
		}
	}


	// If you're hovering over the sidebar area and scroll, either change the scroll position or the map editor character value
	if (engine.mouse.cx > 80 && engine.mouse.cy > 28) {
		if (engine.gui->selected == "Map Editor") {
			if (engine.mouse.wheel_up) {
					engine.mapEdit_ch++;
			} else if (engine.mouse.wheel_down) {
				if (engine.mapEdit_ch > 0)
					engine.mapEdit_ch--;
			}
		} else {
			if (engine.mouse.wheel_up && engine.gui->selectScroll > 0) {
				engine.gui->selectScroll--;
			} else if (engine.mouse.wheel_down) {
				engine.gui->selectScroll++;
			}
		}
	}

	// Get the position hovered on the map if you're hovering over the map
	if (engine.mouse.cx >= 2 && engine.mouse.cx <= 18 && engine.mouse.cy >= 47 && engine.mouse.cy <= 64) {
		engine.gui->miniHover.x = engine.mouse.cx - 2;
		engine.gui->miniHover.y = engine.mouse.cy - 47;
	} else {
		engine.gui->miniHover.x = -1;
		engine.gui->miniHover.y = -1;
	}

	// If you clicked to path, move towards the clicked point a step at a time
	if (engine.pathing) {
		int dx = engine.pathToX - engine.player->pos.x;
		int dy = engine.pathToY - engine.player->pos.y;
		int stepdx = (dx > 0 ? 1 : -1);
		int stepdy = (dy > 0 ? 1 : -1);
		float distance = sqrtf(dx * dx + dy * dy);
		
		dx = (int)(round(dx / distance));
		dy = (int)(round(dy / distance));
		moveOrAttack(engine.player, dx, dy);
	}
}


bool PlayerAi::IsCommand() { // Command handling
	if (engine.gui->chatBuffer->front() == '/') { // It can only be a command if it starts with a /
		int pos = 0;
		if (engine.gui->chatBuffer->find(" "))
			pos = engine.gui->chatBuffer->find(" ");
		std::string commandToken = engine.gui->chatBuffer->substr(1, pos-1);
		std::string restOfMessage = engine.gui->chatBuffer->substr(pos + 1, engine.gui->chatBuffer->length() - (pos + 1));

		MapTile* mapTile = &engine.minimap[engine.topleft.x + 8][engine.topleft.y + 8];

		if (commandToken == "nick") { // Change your nickname
			std::string newName = restOfMessage;
			engine.ownName = newName;

			std::string concat = std::string("Changed your nickname to: ") + engine.ownName;

			engine.gui->message(TCODColor::lightBlue, concat.c_str());
			return true;
		} else if (commandToken == "m-ch") { // Change the minimap tile character
			std::string newTileCh;
			if (restOfMessage[0] == '[') { // If the character starts with [ and is then a number, assume the player wants to set the character to that specific value
				newTileCh = (char) atoi(restOfMessage.substr(1, restOfMessage.find(']')).c_str());
			} else {
				newTileCh = restOfMessage;
			}

			mapTile->ch = newTileCh.c_str()[0];
			return true;
		} else if (commandToken == std::string("m-fg")) {  // Change the minimap tile foreground
			int fgR = atoi(restOfMessage.substr(0, restOfMessage.find(',')).c_str());
			restOfMessage.erase(0, restOfMessage.find(',') + 1);
			int fgG = atoi(restOfMessage.substr(0, restOfMessage.find(',')).c_str());
			restOfMessage.erase(0, restOfMessage.find(',') + 1);
			int fgB = atoi(restOfMessage.substr(0, restOfMessage.find(',')).c_str());

			mapTile->fg = TCODColor(fgR, fgG, fgB);
			return true;
		} else if (commandToken == "m-bg") {  // Change the minimap tile background
			int bgR = atoi(restOfMessage.substr(0, restOfMessage.find(',')).c_str());
			restOfMessage.erase(0, restOfMessage.find(',') + 1);
			int bgG = atoi(restOfMessage.substr(0, restOfMessage.find(',')).c_str());
			restOfMessage.erase(0, restOfMessage.find(',') + 1);
			int bgB = atoi(restOfMessage.substr(0, restOfMessage.find(',')).c_str());

			mapTile->bg = TCODColor(bgR, bgG, bgB);
			return true;
		} else if (commandToken == "debug") { // A debug command, currently displays the name and mx/my of the entered coordinates
			int x = atoi(restOfMessage.substr(0, restOfMessage.find(',')).c_str());
			restOfMessage.erase(0, restOfMessage.find(',') + 1);
			int y = atoi(restOfMessage.c_str());

			MapTile* debugTile = &engine.minimap[x][y];
			engine.gui->message(TCODColor::celadon, "Name: %s, mX: %d, mY: %d", debugTile->name, debugTile->worldPos.x, debugTile->worldPos.y);
			return true;

		}
		return true;
	}

	return false;
}


bool PlayerAi::moveOrAttack(Actor* owner, int targetx, int targety) { // Player Movement
	if (owner->lastActed + (100 * owner->speed) > TCODSystem::getElapsedMilli()) { // Only move if it's been long enough since they last moved
		return false;
	} else {
		owner->lastActed = TCODSystem::getElapsedMilli(); // Update the last time they moved

		engine.client.MovePlayerBy(engine.ownDesc, targetx, targety);

		return true;
		/*
		for (Actor** iterator = engine.actors.begin(); iterator != engine.actors.end(); iterator++) { // If there's a live attackable actor at the target pos, attack it
			Actor* actor = *iterator;
			if (actor->destructible && !actor->destructible->isDead() && actor->x == targetx && actor->y == targety) {
				owner->attacker->attack(owner, actor);
				return false;
			}
		}

		if (!engine.map->canWalk(targetx - 1, targety - 1)) // If you can't walk there, return false
			return false;

		for (Actor** iterator = engine.actors.begin(); iterator != engine.actors.end(); iterator++) { // If there's items on the tile, print all of them to the log
			Actor* actor = *iterator;
			bool corpseOrItem = (actor->destructible && actor->destructible->isDead()) || actor->pickable;
			if (corpseOrItem && actor->x == targetx && actor->y == targety) {
				engine.gui->message(TCODColor::lightGrey, "There's a %s here\n", actor->name);
			}
		}

		owner->x = targetx;
		owner->y = targety;

		bool movedMaps = false;
		
		if (owner->x < 1) { // If the player moves off the left side of the map
			engine.pathing = false;
			if (engine.LoadMap(owner->mX - 1, owner->mY, false)) { // Load the map there if it exists
				owner->x = engine.map->getWidth();
				owner->mX -= 1;
				movedMaps = true;
			} else if (engine.gui->selected == "Map Editor") { // Create a map there if the player is in map editor mode and it doesn't already exist
				engine.NewMapAt(owner->mX - 1, owner->mY);
				owner->x = engine.map->getWidth();
				owner->mX -= 1;
				movedMaps = true;
			} else { // Otherwise print out a generic message indicating that map isn't available
				owner->x = 1;
				engine.gui->message(TCODColor::brass, "You feel like you can't go there yet...");
			}
		}

		if (owner->x > engine.map->getWidth()) { // If the player moves off the right side of the map
			engine.pathing = false;
			if (engine.LoadMap(owner->mX + 1, owner->mY, false)) { // Load the map there if it exists
				owner->x = 1;
				owner->mX += 1;
				movedMaps = true;
			} else if (engine.gui->selected == "Map Editor") { // Create a map there if the player is in map editor mode and it doesn't already exist
				engine.NewMapAt(owner->mX + 1, owner->mY);
				owner->x = 1;
				owner->mX += 1;
				movedMaps = true;
			} else { // Otherwise print out a generic message indicating that map isn't available
				owner->x = engine.map->getWidth();
				engine.gui->message(TCODColor::brass, "You feel like you can't go there yet...");
			}
		}

		if (owner->y < 1) {
			engine.pathing = false;
			if (engine.LoadMap(owner->mX, owner->mY - 1, false)) { // Load the map there if it exists
				owner->y = engine.map->getHeight() - 1;
				owner->mY -= 1;
				movedMaps = true;
			} else if (engine.gui->selected == "Map Editor") { // Create a map there if the player is in map editor mode and it doesn't already exist
				engine.NewMapAt(owner->mX, owner->mY - 1);
				owner->y = engine.map->getHeight() - 1;
				owner->mY -= 1;
				movedMaps = true;
			} else { // Otherwise print out a generic message indicating that map isn't available
				owner->y = 1;
				engine.gui->message(TCODColor::brass, "You feel like you can't go there yet...");
			}
		}

		if (owner->y > engine.map->getHeight() - 1) {
			engine.pathing = false;
			if (engine.LoadMap(owner->mX, owner->mY + 1, false)) { // Load the map there if it exists
				owner->y = 1;
				owner->mY += 1;
				movedMaps = true;
			} else if (engine.gui->selected == "Map Editor") { // Create a map there if the player is in map editor mode and it doesn't already exist
				engine.NewMapAt(owner->mX, owner->mY + 1);
				owner->y = 1;
				owner->mY += 1;
				movedMaps = true;
			} else { // Otherwise print out a generic message indicating that map isn't available
				owner->y = engine.map->getHeight() - 1;
				engine.gui->message(TCODColor::brass, "You feel like you can't go there yet...");
			}
		}

		// Update the position in the ownDesc variable for messages to other players
		engine.ownDesc.x = owner->x;
		engine.ownDesc.y = owner->y;
		engine.ownDesc.worldPos.x = owner->mX;
		engine.ownDesc.worldPos.y = owner->mY;

		if (movedMaps) { // If the player switched maps, update the minimap to reflect the new center
			engine.UpdateMinimap();
			engine.gui->message(TCODColor::blue, "Updated minimap. New start %d, %d", engine.topleft_x, engine.topleft_y);
		}

		// Send a message to the server saying you moved
		engine.client.MovePlayer(engine.ownDesc);

		return true;
		*/
	}
}

// Handle non-chat letter inputs
void PlayerAi::handleActionKey(Actor* owner, int ascii) {
	switch (ascii) {
		case 'g': // pick up something
		{
			bool found = false;
			for (Actor** iterator = engine.actors.begin(); iterator != engine.actors.end(); iterator++) {
				Actor* actor = *iterator;
				if (actor->pickable && actor->pos.x == owner->pos.x && actor->pos.y == owner->pos.y) {
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
			engine.pathing = false;
			moveOrAttack(owner, owner->pos.x, owner->pos.y - 1);
			break;
		}

		case 's': // move down
		{
			engine.pathing = false;
			moveOrAttack(owner, owner->pos.x, owner->pos.y + 1);
			break;
		}

		case 'a': // move left
		{
			engine.pathing = false;
			moveOrAttack(owner, owner->pos.x - 1, owner->pos.y);
			break;
		}

		case 'd': // move right
		{
			engine.pathing = false;
			moveOrAttack(owner, owner->pos.x + 1, owner->pos.y);
			break;
		}
		

		case 'p': // save the current map to file
		{
			engine.SaveMap(engine.ownDesc.worldPos.x, engine.ownDesc.worldPos.y, engine.map->tiles);
			engine.gui->message(TCODColor::purple, "Saved current map to file.");
			break;
		}

		case 'o' : // reload the current map from file
		{
			engine.LoadMap(engine.ownDesc.worldPos.x, engine.ownDesc.worldPos.y, false);
		}
	}
}

// Monster update logic
void MonsterAi::update(Actor* owner) {
	if (owner->destructible && owner->destructible->isDead()) {
		return;
	}

	moveOrAttack(owner, engine.player->pos.x, engine.player->pos.y);
}


// Monster movement
void MonsterAi::moveOrAttack(Actor* owner, int targetx, int targety) {
	if (owner->lastActed + (100 * owner->speed) > TCODSystem::getElapsedMilli()) return; // Only move if it's been long enough since their last move

	owner->lastActed = TCODSystem::getElapsedMilli(); // Update their last move time

	int dx = targetx - owner->pos.x;
	int dy = targety - owner->pos.y;
	int stepdx = (dx > 0 ? 1 : -1);
	int stepdy = (dy > 0 ? 1 : -1);
	float distance = sqrtf(dx * dx + dy * dy);

	if (distance >= 2) { // If the destination is farther than 2 tiles away, just move one step towards it
		dx = (int)(round(dx / distance));
		dy = (int)(round(dy / distance));
		
	} else if (owner->attacker) { // If the monster is next to the target player, try to attack
		owner->attacker->attack(owner, engine.player);
	}
}