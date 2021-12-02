class Destructible {
public:
	float maxHp; // max health
	float hp; // current health
	float defense; // hit points deflected
	const char* corpseName; // actors name once dead/destroyed

	Destructible(float maxHp, float defense, const char* corpseName);
	inline bool isDead() { return hp <= 0; }
	float takeDamage(Actor* owner, float damage);
	virtual void die(Actor* owner);
	float heal(float amount);
	virtual ~Destructible() {};
};

class MonsterDestructible : public Destructible {
public:
	MonsterDestructible(float maxHp, float defense, const char* corpseName);
	void die(Actor* owner);
};

class PlayerDestructible : public Destructible {
public:
	PlayerDestructible(float maxHp, float defense, const char* corpseName);
	void die(Actor* owner);
};