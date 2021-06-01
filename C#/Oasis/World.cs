using System;
using SadConsole.Components;
using Microsoft.Xna.Framework;
using DefaultEcs;

namespace Oasis {
    // All game state data is stored in World
    // also creates and processes generators
    // for map creation
    public class World {
        // map creation and storage data
        private int _mapWidth = 100;
        private int _mapHeight = 100;
        private TileBase[] _mapTiles;
        private int _maxRooms = 100;
        private int _minRoomSize = 4;
        private int _maxRoomSize = 15;
        public Map CurrentMap { get; set; }

        public Entity player { get; set; }

        public World() {
            CreateMap();

            CreatePlayer();

            CreateMonsters();

            CreateLoot();
        }

        private void CreateMap() {
            _mapTiles = new TileBase[_mapWidth * _mapHeight];
            CurrentMap = new Map(_mapWidth, _mapHeight);
            MapGenerator mapGen = new MapGenerator();
            CurrentMap = mapGen.GenerateMap(_mapWidth, _mapHeight, _maxRooms, _minRoomSize, _maxRoomSize);
        }

        private void CreatePlayer() {
            player = GameLoop.gs.ecs.CreateEntity();

            player.Set(new Render { sce = new SadConsole.Entities.Entity(1, 1) });
            player.Get<Render>().SimpleSet('@', Color.Yellow);

            for (int i = 0; i < CurrentMap.Tiles.Length; i++) {
                if (!CurrentMap.Tiles[i].IsBlockingMove) {
                    Point loc = SadConsole.Helpers.GetPointFromIndex(i, CurrentMap.Width);
                    player.Get<Render>().SetPosition(loc.X, loc.Y);
                }
            }
            
            player.Get<Render>().sce.Components.Add(new EntityViewSyncComponent());

            player.Set(new Player { });
            player.Set(new Name { name = "Player" });
            player.Set(new Stats { Defense = 10, Attack = 12, Damage = "1d6", Health = 10, MaxHealth = 10 });
        }

        private void CreateLoot() {
            int numLoot = 20;

            for (int i = 0; i < numLoot; i++) {
                int lootPosition = 0;

                Entity newItem = GameLoop.gs.ecs.CreateEntity();
                newItem.Set(new Render { sce = new SadConsole.Entities.Entity(1, 1) });
                newItem.Get<Render>().SimpleSet('L', Color.Green);
                newItem.Get<Render>().sce.Components.Add(new EntityViewSyncComponent());
                newItem.Set(new Item { condition = 100, weight = 2, glyph = 'L', fg = Color.Green });
                newItem.Set(new Name { name = "Fancy Shirt" });
                
                while (CurrentMap.Tiles[lootPosition].IsBlockingMove) {
                    lootPosition = GameLoop.GlobalRand.Next(0, CurrentMap.Width * CurrentMap.Height);
                }

                newItem.Get<Render>().SetPosition(CurrentMap.idx_xy(lootPosition));
            }
        }

        private void CreateMonsters() { 
            int numMonsters = 10; 
             
            for (int i = 0; i < numMonsters; i++) {
                int monsterPosition = 0;
                Entity newMonster = GameLoop.gs.ecs.CreateEntity();
                newMonster.Set(new Render { sce = new SadConsole.Entities.Entity(1, 1) });
                newMonster.Get<Render>().SimpleSet('o', Color.Blue);
                newMonster.Get<Render>().sce.Components.Add(new EntityViewSyncComponent());
                newMonster.Set(new Monster { });

                while (CurrentMap.Tiles[monsterPosition].IsBlockingMove) {
                    monsterPosition = GameLoop.GlobalRand.Next(0, CurrentMap.Width * CurrentMap.Height);
                }

                newMonster.Set(new Stats { Defense = GameLoop.GlobalRand.Next(3, 10), Attack = GameLoop.GlobalRand.Next(3, 10), Damage = "1d6", Health = 10, MaxHealth = 10 });

                newMonster.Set(new Name { name = "orc" });

                Entity newItem = GameLoop.gs.ecs.CreateEntity();
                newItem.Set(new Item { condition = 100, weight = 2, glyph = 'L', fg = Color.HotPink });
                newItem.Set(new Name { name = "Spork" });
                newItem.Set(new InBackpack { owner = newMonster });

                newMonster.Get<Render>().SetPosition(CurrentMap.idx_xy(monsterPosition));
            }
        }
    }
}
