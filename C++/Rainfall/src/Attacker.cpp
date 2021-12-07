#include <stdio.h>
#include "main.h"

Attacker::Attacker(float power) : power(power) {
}

void Attacker::attack(Actor* owner, Actor* target) {
	if (target->destructible && !target->destructible->isDead()) { // If the target isn't dead already
		if (power - target->destructible->defense > 0) { // And the attackers power is higher than the defenders defense
			// Deal damage
			engine.gui->message(owner==engine.player ? TCODColor::red : TCODColor::lightGrey, "%s attacks %s for %g hit points.\n", owner->name, target->name, power - target->destructible->defense);
		} else {
			// Otherwise no damage can be dealt
			engine.gui->message(TCODColor::lightGrey, "%s attacks %s but it has no effect!\n", owner->name, target->name);
		}
		target->destructible->takeDamage(target, power);
	} else { // If the target is dead you can stop hitting them
		engine.gui->message(TCODColor::lightGrey, "%s attacks %s in vain.\n", owner->name, target->name);
	}
}