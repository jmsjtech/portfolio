#include <stdio.h>
#include "main.h"

Destructible::Destructible(float maxHp, float defense, const char* corpseName) :
	maxHp(maxHp), hp(maxHp), defense(defense), corpseName(corpseName) {
}

MonsterDestructible::MonsterDestructible(float maxHp, float defense, const char* corpseName) :
	Destructible(maxHp, defense, corpseName) {
}

PlayerDestructible::PlayerDestructible(float maxHp, float defense, const char* corpseName) :
	Destructible(maxHp, defense, corpseName) {
}

// Take damage and return how much damage was actually taken
float Destructible::takeDamage(Actor* owner, float damage) {
	damage -= defense;
	if (damage > 0) {
		hp -= damage;
		if (hp <= 0) {
			die(owner);
		}
	} else {
		damage = 0;
	}
	return damage;
}

void Destructible::die(Actor* owner) {
	// transform the actor into a corpse!
	owner->ch = '%';
	owner->col = TCODColor::darkRed;
	owner->name = corpseName;
	owner->blocks = false;

	// make sure corpses are drawn before living actors
	engine.sendToBack(owner);
}

void MonsterDestructible::die(Actor* owner) {
	// transform it into a corpse
	engine.gui->message(TCODColor::lightGrey, "%s is dead\n", owner->name);
	Destructible::die(owner);
}

void PlayerDestructible::die(Actor* owner) { // If the player died, run some logic on it
	engine.gui->message(TCODColor::red, "You died!\n");
	Destructible::die(owner);
}

float Destructible::heal(float amount) { // Heal, then return how much was healed
	hp += amount;
	if (hp > maxHp) {
		amount -= hp - maxHp;
		hp = maxHp;
	}
	return amount;
}