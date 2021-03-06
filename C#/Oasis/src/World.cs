using System;
using SadConsole.Components;
using Microsoft.Xna.Framework;
using DefaultEcs;
using System.Collections.Generic;

namespace Oasis {
    // All game state data is stored in World
    // also creates and processes generators
    // for map creation
    public class World {
        // map creation and storage data
        private int _mapWidth = 100;
        private int _mapHeight = 100;
        private Entity[] _mapTiles;
        private int _maxRooms = 100;
        private int _minRoomSize = 4;
        private int _maxRoomSize = 15;
        public Map CurrentMap { get; set; }

        public Dictionary<long, Entity> players { get; set; }

        public World() {
            players = new Dictionary<long, Entity>();


            CreateMap();

            CreatePlayer(GameLoop.NetworkManager.myUID);

            CreateMonsters();

            CreateLoot();
        }

        private void CreateMap() {
            _mapTiles = new Entity[_mapWidth * _mapHeight];
            CurrentMap = new Map(_mapWidth, _mapHeight);
            MapGenerator mapGen = new MapGenerator();
            CurrentMap = mapGen.GenerateMap(_mapWidth, _mapHeight, _maxRooms, _minRoomSize, _maxRoomSize);
            
        }

        public void CreatePlayer(long playerUID) {
            if (!players.ContainsKey(playerUID)) {

                Entity player = GameLoop.gs.ecs.CreateEntity();

                player.Set(new Render { sce = new SadConsole.Entities.Entity(1, 1) });

                for (int i = 0; i < CurrentMap.Tiles.Length; i++) {
                    if (!CurrentMap._tiles[i].Has<BlocksMovement>()) {
                        Point loc = SadConsole.Helpers.GetPointFromIndex(i, CurrentMap.Width);
                        player.Get<Render>().Init(loc, '@', Color.Yellow);
                    }
                }
                player.Set(new Player { });
                player.Set(new Name { name = "Player" });
                player.Set(new BlocksMovement { });
                player.Set(new LastActed { last_action = 0, speed_in_ms = 100 });
                player.Set(new Viewshed { radius = 10 });

                player.Set(new Stats { Defense = 10, Attack = 12, Damage = "1d6", Health = 10, MaxHealth = 10 });
                players.Add(playerUID, player);
            }
        }

        private void CreateLoot() {
            int numLoot = 20;

            for (int i = 0; i < numLoot; i++) {
                int lootPosition = 0;

                Entity newItem = GameLoop.gs.ecs.CreateEntity();
                newItem.Set(new Render { sce = new SadConsole.Entities.Entity(1, 1) });
                newItem.Set(new Item { condition = 100, weight = 2, glyph = 'L', fg = Color.Green, value = 2 });
                newItem.Set(new Name { name = "Fancy Shirt" });

                while (CurrentMap._tiles[lootPosition].Has<BlocksMovement>()) {
                    lootPosition = GameLoop.GlobalRand.Next(0, CurrentMap.Width * CurrentMap.Height);
                }

                newItem.Get<Render>().Init(CurrentMap.idx_xy(lootPosition), 'L', Color.Green);
            }
        }

        private void CreateMonsters() {
            int numMonsters = 100;

            for (int i = 0; i < numMonsters; i++) {
                int monsterPosition = 0;
                Entity newMonster = GameLoop.gs.ecs.CreateEntity();
                newMonster.Set(new Render { sce = new SadConsole.Entities.Entity(1, 1) });
                newMonster.Set(new Monster { });

                while (CurrentMap._tiles[monsterPosition].Has<BlocksMovement>()) {
                    monsterPosition = GameLoop.GlobalRand.Next(0, CurrentMap.Width * CurrentMap.Height);
                }

                newMonster.Set(new Stats { Defense = GameLoop.GlobalRand.Next(3, 10), Attack = GameLoop.GlobalRand.Next(10, 14), Damage = "1d6", Health = 10, MaxHealth = 10 });
                newMonster.Set(new BlocksMovement { });
                newMonster.Set(new Name { name = "orc " + i.ToString() });
                newMonster.Set(new LastActed { last_action = 0, speed_in_ms = 250 });
                newMonster.Set(new Viewshed { radius = 10 });

                Entity newItem = GameLoop.gs.ecs.CreateEntity();
                newItem.Set(new Item { condition = 100, weight = 2, glyph = 'L', fg = Color.HotPink, value = 1 });
                newItem.Set(new Name { name = "Spork" });
                newItem.Set(new InBackpack { owner = newMonster });

                newMonster.Get<Render>().Init(CurrentMap.idx_xy(monsterPosition), 'o', Color.Red);
            }
        }
    }
}
