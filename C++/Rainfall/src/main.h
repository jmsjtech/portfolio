#include "libtcod.hpp"
#include <iostream>
#include "Nanz_Net.h"
#include "NetCommon.h"

enum minimap {
	w = 17,
	h = 17,
};

class Actor;
#include "Item.h"
#include "Destructible.h"
#include "Attacker.h"
#include "Ai.h"
#include "Pickable.h"
#include "Container.h"
#include "Actor.h"
#include "Map.h"
#include "Healer.h"
#include "Gui.h"
#include "Engine.h"