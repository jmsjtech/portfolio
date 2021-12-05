#include "main.h"

Engine engine(120,70);

int main() {

    while (!TCODConsole::isWindowClosed()) {
        engine.update();
        engine.render();
        TCODConsole::flush();
    }
    return 0;
}