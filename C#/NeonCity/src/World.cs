using System;
using SadConsole.Components;
using Microsoft.Xna.Framework;
using DefaultEcs;
using System.Collections.Generic;

namespace NeonCity {
    // All game state data is stored in World
    // also creates and processes generators
    // for map creation
    public class World {
        // map creation and storage data
        private int _mapWidth = 200;
        private int _mapHeight = 200;
        private Entity[] _mapTiles;
        public Map CurrentMap { get; set; }

        public Entity playerCursor { get; set; }

        public World() {
            CreateMap();

            CreatePlayer();
        }

        private void CreateMap() {
            _mapTiles = new Entity[_mapWidth * _mapHeight];
            CurrentMap = new Map(_mapWidth, _mapHeight);
            MapGenerator mapGen = new MapGenerator();
            CurrentMap = mapGen.GenerateMap(_mapWidth, _mapHeight);
        }

        private void CreatePlayer() {
            playerCursor = GameLoop.gs.ecs.CreateEntity();

            playerCursor.Set(new Render { sce = new SadConsole.Entities.Entity(1, 1) });
            playerCursor.Get<Render>().Resize(5, 5, "player", Color.Yellow, Color.Transparent);
            playerCursor.Get<Render>().SetPosition(0, 0);

            playerCursor.Set(new Player(100, 0, 0));

            playerCursor.Set(new Name { name = "Player" });
        }
    }
}
