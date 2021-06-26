using System;
using SadConsole.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System.Collections.Generic;

namespace Oasis {
    // All game state data is stored in World
    // also creates and processes generators
    // for map creation
    public class GameWorld {
        // map creation and storage data
        private int _mapWidth = 100;
        private int _mapHeight = 100;
        private Entity[] _mapTiles;
        private int _maxRooms = 100;
        private int _minRoomSize = 4;
        private int _maxRoomSize = 15;
        public Map CurrentMap { get; set; }

        public Dictionary<long, Entity> players { get; set; }

        public GameWorld() {
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

                Entity player = GameLoop.ecs.CreateEntity();

                player.Attach(new Render { sce = new SadConsole.Entities.Entity(1, 1) });

                for (int i = 0; i < CurrentMap.Tiles.Length; i++) {
                    if (!CurrentMap._tiles[i].Has<BlocksMovement>()) {
                        Point loc = SadConsole.Helpers.GetPointFromIndex(i, CurrentMap.Width);
                        Helper.Render_Init(player.Get<Render>(), loc, '@', Color.Yellow);
                    }
                }
                player.Attach(new Player { });
                player.Attach(new Name { name = "Player" });
                player.Attach(new BlocksMovement { });
                player.Attach(new LastActed { last_action = 0, speed_in_ms = 100 });
                player.Attach(new Viewshed { radius = 10 });
                player.Attach(new ID { id = Helper.GenerateID() });

                player.Attach(new Stats { Defense = 10, Attack = 12, Damage = "1d6", Health = 10, MaxHealth = 10 });
                players.Add(playerUID, player);
            }
        }

        private void CreateLoot() {
            int numLoot = 20;

            for (int i = 0; i < numLoot; i++) {
                int lootPosition = 0;

                Entity newItem = GameLoop.ecs.CreateEntity();
                newItem.Attach(new Render { sce = new SadConsole.Entities.Entity(1, 1) });
                newItem.Attach(new Item { condition = 100, weight = 2, glyph = 'L', fg = Color.Green, value = 2 });
                newItem.Attach(new Name { name = "Fancy Shirt" });
                newItem.Attach(new ID { id = Helper.GenerateID() });

                while (CurrentMap._tiles[lootPosition].Has<BlocksMovement>()) {
                    lootPosition = GameLoop.GlobalRand.Next(0, CurrentMap.Width * CurrentMap.Height);
                }

                Helper.Render_Init(newItem.Get<Render>(), CurrentMap.idx_xy(lootPosition), 'L', Color.Green);
            }
        }

        private void CreateMonsters() {
            int numMonsters = 100;

            for (int i = 0; i < numMonsters; i++) {
                int monsterPosition = 0;
                Entity newMonster = GameLoop.ecs.CreateEntity();
                newMonster.Attach(new Render { sce = new SadConsole.Entities.Entity(1, 1) });
                newMonster.Attach(new Monster { });

                while (CurrentMap._tiles[monsterPosition].Has<BlocksMovement>()) {
                    monsterPosition = GameLoop.GlobalRand.Next(0, CurrentMap.Width * CurrentMap.Height);
                }

                newMonster.Attach(new Stats { Defense = GameLoop.GlobalRand.Next(3, 10), Attack = GameLoop.GlobalRand.Next(10, 14), Damage = "1d6", Health = 10, MaxHealth = 10 });
                newMonster.Attach(new BlocksMovement { });
                newMonster.Attach(new Name { name = "orc " + i.ToString() });
                newMonster.Attach(new ID { id = Helper.GenerateID() });
                newMonster.Attach(new LastActed { last_action = 0, speed_in_ms = 250 });
                newMonster.Attach(new Viewshed { radius = 10 });

                Entity newItem = GameLoop.ecs.CreateEntity();
                newItem.Attach(new Item { condition = 100, weight = 2, glyph = 'L', fg = Color.HotPink, value = 1 });
                newItem.Attach(new Name { name = "Spork" });
                newItem.Attach(new InBackpack { ownerID = newMonster.Get<ID>().id });
                newItem.Attach(new ID { id = Helper.GenerateID() });

                Helper.Render_Init(newMonster.Get<Render>(), CurrentMap.idx_xy(monsterPosition), 'o', Color.Red);
            }
        }
    }
}
