class Healer : public Pickable {
public:
	float amount; // how much hp to heal

	Healer(float amount);
	bool use(Actor* owner, Actor* wearer);
};