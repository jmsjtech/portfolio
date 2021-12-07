class Attacker {
public:
	float power; // Attack power

	Attacker(float power);
	void attack(Actor* owner, Actor* target);
};