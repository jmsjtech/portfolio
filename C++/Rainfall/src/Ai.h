class Ai {
public:
	virtual void update(Actor* owner) = 0;
	virtual ~Ai() {};
};

class PlayerAi : public Ai {
public:
	void update(Actor* owner);
	bool IsCommand();
	void handleActionKey(Actor* owner, int ascii);
protected:
	bool moveOrAttack(Actor* owner, int targetx, int targety);
};

class MonsterAi : public Ai {
public:
	void update(Actor* owner);

protected:
	void moveOrAttack(Actor* owner, int targetx, int targety);
	int moveCount;
};