extern crate serde;

use rltk::{GameState, Rltk, Point};
use specs::prelude::*;
use specs::saveload::{SimpleMarker, SimpleMarkerAllocator};
use std::time::{SystemTime, UNIX_EPOCH};

#[macro_use]
extern crate lazy_static;

mod components;
pub use components::*;
mod map;
pub use map::*;
mod player;
use player::*;
mod rect;
pub use rect::Rect;
mod visibility_system;
use visibility_system::VisibilitySystem;
mod monster_ai_system;
use monster_ai_system::MonsterAI;
mod map_indexing_system;
use map_indexing_system::MapIndexingSystem;
mod melee_combat_system;
use melee_combat_system::MeleeCombatSystem;
mod damage_system;
use damage_system::DamageSystem;
mod gui;
mod gamelog;
use gamelog::*;
mod spawner;
mod inventory_system;
use inventory_system::*;

mod gamesystem;
pub use gamesystem::*;

pub mod camera;
pub mod raws;
pub mod bystander_ai_system;

mod saveload_system;
mod random_table;
mod trigger_system;

mod particle_system;
use particle_system::*;

mod lighting_system;

mod animal_ai_system;

mod rex_assets;

mod map_builders;

const SHOW_MAPGEN_VISUALIZER : bool = false;

#[derive(PartialEq, Copy, Clone)]
pub enum RunState { Running,
    PreRun,
    ShowInventory,
    ShowDropItem,
    ShowTargeting { range: i32, item: Entity },
    MainMenu { menu_selection: gui::MainMenuSelection },
    SaveGame,
    NextLevel,
    PreviousLevel,
    ShowRemoveItem,
    PlayerDied,
    MagicMapReveal { row: i32 },
    MapGeneration,
    ShowCheatMenu
}

pub struct State {
    pub ecs: World,
    mapgen_next_state : Option<RunState>,
    mapgen_history : Vec<Map>,
    mapgen_index : usize,
    mapgen_timer : f32
}

impl State {
    fn run_systems(&mut self) {
        let is_paused;
        {
            let paused = self.ecs.fetch::<bool>();
            is_paused = *paused;
        }
        
        let mut vis = VisibilitySystem{};
        vis.run_now(&self.ecs);
        let mut mapindex = MapIndexingSystem{};
        mapindex.run_now(&self.ecs);
        
        if !is_paused { 
            let mut mob = MonsterAI{};
            mob.run_now(&self.ecs);
            let mut triggers = trigger_system::TriggerSystem{};
            triggers.run_now(&self.ecs);
            let mut bystander = bystander_ai_system::BystanderAI{};
            bystander.run_now(&self.ecs);
            let mut melee = MeleeCombatSystem{};
            melee.run_now(&self.ecs);
            let mut animals = animal_ai_system::AnimalAI{};
            animals.run_now(&self.ecs);
            
        }
        
        let mut damage = DamageSystem{};
        damage.run_now(&self.ecs);
        let mut particles = particle_system::ParticleSpawnSystem{};
        particles.run_now(&self.ecs);
        let mut pickup = ItemCollectionSystem{};
        pickup.run_now(&self.ecs);
        let mut items = ItemUseSystem{};
        items.run_now(&self.ecs);
        let mut drop_items = ItemDropSystem{};
        drop_items.run_now(&self.ecs);
        let mut item_remove = ItemRemoveSystem{};
        item_remove.run_now(&self.ecs);
        
        let mut lighting = lighting_system::LightingSystem{};
        lighting.run_now(&self.ecs);

        self.ecs.maintain();
    }
    
    fn generate_world_map(&mut self, new_depth : i32, offset: i32) {
        self.mapgen_index = 0;
        self.mapgen_timer = 0.0;
        self.mapgen_history.clear();
        let map_building_info = map::level_transition(&mut self.ecs, new_depth, offset);
        if let Some(history) = map_building_info {
            self.mapgen_history = history;
        } else {
            map::thaw_level_entities(&mut self.ecs);
        }
    }

    fn game_over_cleanup(&mut self) {
        // Delete everything
        let mut to_delete = Vec::new();
        for e in self.ecs.entities().join() {
            to_delete.push(e);
        }
        for del in to_delete.iter() {
            self.ecs.delete_entity(*del).expect("Deletion failed");
        }

        // Spawn a new player
        {
            let player_entity = spawner::player(&mut self.ecs, 0, 0);
            let mut player_entity_writer = self.ecs.write_resource::<Entity>();
            *player_entity_writer = player_entity;
        }
        
        // Replace the world maps
        self.ecs.insert(map::MasterDungeonMap::new());

        // Build a new map and place the player
        self.generate_world_map(1, 0);
    }

    
    fn goto_level(&mut self, offset: i32) {
        freeze_level_entities(&mut self.ecs);

        // Build a new map and place the player
        let current_depth = self.ecs.fetch::<Map>().depth;
        self.generate_world_map(current_depth + offset, offset);

        // Notify the player
        let mut gamelog = self.ecs.fetch_mut::<gamelog::GameLog>();
        gamelog.entries.push("You change level.".to_string());
    }
}

impl GameState for State {
    fn tick(&mut self, ctx : &mut Rltk) {
        let mut newrunstate;
        {
            let runstate = self.ecs.fetch::<RunState>();
            newrunstate = *runstate;
        }

        ctx.cls();
        particle_system::cull_dead_particles(&mut self.ecs, ctx);
        

        match newrunstate {
            RunState::MainMenu{..} => {}
            _ => {
                camera::render_camera(&self.ecs, ctx);
                gui::draw_ui(&self.ecs, ctx); 
            }
        }

        match newrunstate {
            RunState::MapGeneration => {
                if !SHOW_MAPGEN_VISUALIZER {
                    newrunstate = self.mapgen_next_state.unwrap();
                } else {
                    ctx.cls();
                    if self.mapgen_index < self.mapgen_history.len() { camera::render_debug_map(&self.mapgen_history[self.mapgen_index], ctx); }

                    self.mapgen_timer += ctx.frame_time_ms;
                    if self.mapgen_timer > 500.0 {
                        self.mapgen_timer = 0.0;
                        self.mapgen_index += 1;
                        if self.mapgen_index >= self.mapgen_history.len() {
                            //self.mapgen_index -= 1;
                            newrunstate = self.mapgen_next_state.unwrap();
                        }
                    }
                }
            }
            RunState::PreRun => {
                newrunstate = RunState::Running;
            }
            RunState::Running => {
                
                //self.run_systems();
                //self.ecs.maintain();
                match *self.ecs.fetch::<RunState>() {
                    #[allow(unused_assignments)]
                    RunState::MagicMapReveal{ .. } => newrunstate = RunState::MagicMapReveal{ row: 0 },
                    #[allow(unused_assignments)]
                    _ => newrunstate = RunState::Running

                }
                newrunstate = player_input(self, ctx, RunState::Running);

            }
            RunState::ShowInventory => {
                let result = gui::show_inventory(self, ctx);
                match result.0 {
                    gui::ItemMenuResult::Cancel => newrunstate = RunState::Running,
                    gui::ItemMenuResult::NoResponse => {}
                    gui::ItemMenuResult::Selected => {
                        let item_entity = result.1.unwrap();
                        let is_ranged = self.ecs.read_storage::<Ranged>();
                        let is_item_ranged = is_ranged.get(item_entity);
                        if let Some(is_item_ranged) = is_item_ranged {
                            newrunstate = RunState::ShowTargeting{ range: is_item_ranged.range, item: item_entity };
                        } else {
                            let mut intent = self.ecs.write_storage::<WantsToUseItem>();
                            intent.insert(*self.ecs.fetch::<Entity>(), WantsToUseItem{ item: item_entity, target: None }).expect("Unable to insert intent");
                            newrunstate = RunState::Running;
                        }
                    }
                }
            }
            RunState::ShowDropItem => {
                let result = gui::drop_item_menu(self, ctx);
                match result.0 {
                    gui::ItemMenuResult::Cancel => newrunstate = RunState::Running,
                    gui::ItemMenuResult::NoResponse => {}
                    gui::ItemMenuResult::Selected => {
                        let item_entity = result.1.unwrap();
                        let mut intent = self.ecs.write_storage::<WantsToDropItem>();
                        intent.insert(*self.ecs.fetch::<Entity>(), WantsToDropItem{ item: item_entity }).expect("Unable to insert intent");
                        newrunstate = RunState::Running;
                    }
                }
            }
            RunState::ShowTargeting{range, item} => {
                newrunstate = player_input(self, ctx, newrunstate);

                let result = gui::ranged_target(self, ctx, range);
                match result.0 {
                    gui::ItemMenuResult::Cancel => newrunstate = RunState::Running,
                    gui::ItemMenuResult::NoResponse => {}
                    gui::ItemMenuResult::Selected => {
                        let mut intent = self.ecs.write_storage::<WantsToUseItem>();
                        intent.insert(*self.ecs.fetch::<Entity>(), WantsToUseItem{ item, target: result.1 }).expect("Unable to insert intent");
                        newrunstate = RunState::Running;
                    }
                }
            }
            RunState::MainMenu{ .. } => {
                let result = gui::main_menu(self, ctx);
                match result {
                    gui::MainMenuResult::NoSelection{ selected } => newrunstate = RunState::MainMenu{ menu_selection: selected },
                    gui::MainMenuResult::Selected{ selected } => {
                        match selected {
                            gui::MainMenuSelection::NewGame => newrunstate = RunState::PreRun,
                            gui::MainMenuSelection::LoadGame => {
                                saveload_system::load_game(&mut self.ecs);
                                newrunstate = RunState::Running;
                            }
                            gui::MainMenuSelection::Quit => { ::std::process::exit(0); }
                        }
                    }
                }
            }
            RunState::SaveGame => {
                saveload_system::save_game(&mut self.ecs);
                newrunstate = RunState::MainMenu{ menu_selection : gui::MainMenuSelection::LoadGame };
            }
            RunState::NextLevel => {
                self.goto_level(1);
                self.mapgen_next_state = Some(RunState::PreRun);
                newrunstate = RunState::MapGeneration;
            }
            RunState::PreviousLevel => {
                self.goto_level(-1);
                self.mapgen_next_state = Some(RunState::PreRun);
                newrunstate = RunState::MapGeneration;
            }
            RunState::ShowRemoveItem => {
                let result = gui::remove_item_menu(self, ctx);
                match result.0 {
                    gui::ItemMenuResult::Cancel => newrunstate = RunState::Running,
                    gui::ItemMenuResult::NoResponse => {}
                    gui::ItemMenuResult::Selected => {
                        let item_entity = result.1.unwrap();
                        let mut intent = self.ecs.write_storage::<WantsToRemoveItem>();
                        intent.insert(*self.ecs.fetch::<Entity>(), WantsToRemoveItem{ item: item_entity }).expect("Unable to insert intent");
                        newrunstate = RunState::Running;
                    }
                }
            }
            RunState::PlayerDied => {
                let result = gui::game_over(ctx);
                match result {
                    gui::GameOverResult::NoSelection => {}
                    gui::GameOverResult::QuitToMenu => {
                        self.game_over_cleanup();
                        newrunstate = RunState::MainMenu{ menu_selection: gui::MainMenuSelection::NewGame };
                    }
                }
            }
            RunState::MagicMapReveal{row} => {
                let mut map = self.ecs.fetch_mut::<Map>();
                for x in 0..map.width {
                    let idx = map.xy_idx(x as i32,row);
                    map.revealed_tiles[idx] = true;
                }
                if row == map.height-1 {
                    newrunstate = RunState::Running;
                } else {
                    newrunstate = RunState::MagicMapReveal{ row: row+1 };
                }
            }
            RunState::ShowCheatMenu => {
                let result = gui::show_cheat_mode(self, ctx);
                match result {
                    gui::CheatMenuResult::Cancel => newrunstate = RunState::Running,
                    gui::CheatMenuResult::NoResponse => {}
                    gui::CheatMenuResult::TeleportToExit => {
                        self.goto_level(1);
                        self.mapgen_next_state = Some(RunState::PreRun);
                        newrunstate = RunState::MapGeneration;
                    }
                }
            }
        }

        {
            let mut runwriter = self.ecs.write_resource::<RunState>();
            *runwriter = newrunstate;
        }

        match newrunstate {
            RunState::MapGeneration => {
                return;
            }
            RunState::MainMenu { .. } => {
                return;
            }

            _ => { }
        }
        
        self.run_systems();
        self.ecs.maintain();
        damage_system::delete_the_dead(&mut self.ecs);
        
        let is_paused;
        {
            let paused = self.ecs.fetch::<bool>();
            is_paused = *paused;
        }
        
        if is_paused { return; }
        


        //let mut clock = self.ecs.fetch_mut::<TimeKeeper>();
        let time_now = SystemTime::now().duration_since(UNIX_EPOCH).expect("Clock may have gone backwards?").as_millis();
        let player_entity = self.ecs.fetch::<Entity>();
        let mut lastactions = self.ecs.write_storage::<LastActed>();
        let mut clocks = self.ecs.write_storage::<TimeKeeper>();

        let mut gamelog = self.ecs.fetch_mut::<GameLog>();
        let mut all_pools = self.ecs.write_storage::<Pools>();

        let last_acted = lastactions.get_mut(*player_entity);
        let clock = clocks.get_mut(*player_entity);
        let stats = all_pools.get_mut(*player_entity);

        if let Some(clock) = clock {
            if clock.last_second + 1000 <= time_now {
                clock.last_second = time_now;

                let mut hunger_clocks = self.ecs.write_storage::<HungerClock>();
                let hc = hunger_clocks.get_mut(*player_entity);
                let mut inflict_damage = self.ecs.write_storage::<SufferDamage>();

                let mut hungry_or_starving = false;


                if let Some(hc) = hc {
                    if hc.state == HungerState::Hungry || hc.state == HungerState::Starving { hungry_or_starving = true; }

                    hc.duration -= 1;
                    if hc.duration < 1 {
                        match hc.state {
                            HungerState::WellFed => {
                                hc.state = HungerState::Normal;
                                hc.duration = 400;
                                gamelog.entries.push("You are no longer well fed.".to_string());
                            }
                            HungerState::Normal => {
                                hc.state = HungerState::Hungry;
                                hc.duration = 400;
                                gamelog.entries.push("You are hungry.".to_string());
                            }
                            HungerState::Hungry => {
                                hc.state = HungerState::Starving;
                                hc.duration = 400;
                                gamelog.entries.push("You are starving!".to_string());
                                hungry_or_starving = true;
                            }
                            HungerState::Starving => {
                                gamelog.entries.push("You're so hungry it hurts!".to_string());
                                SufferDamage::new_damage(&mut inflict_damage, *player_entity, 1, false);
                            }
                        }
                    }
                }


                clock.min += 1;
                if clock.min >= 60 {
                    clock.hour += 1;
                    clock.min = 0;
                }

                if clock.hour >= 24 {
                    clock.day += 1;
                    clock.hour = 0;
                }

                if clock.day >= 29 {
                    clock.season += 1;
                    clock.day = 1;

                    if clock.season == 1 { gamelog.entries.push("Spring has sprung!".to_string()); }
                    if clock.season == 2 { gamelog.entries.push("Summer is here!".to_string()); }
                    if clock.season == 3 { gamelog.entries.push("Autumn has arrived!".to_string()); }
                    if clock.season == 4 { gamelog.entries.push("Winter has come.".to_string()); }
                }

                if clock.season >= 5 {
                    clock.year += 1;
                    clock.season = 1;

                    gamelog.entries.push("Spring has sprung, it's a new year!".to_string());
                }



                if let Some(stats) = stats {
                    if let Some(last_acted) = last_acted {
                        let viewshed_components = self.ecs.read_storage::<Viewshed>();
                        let monsters = self.ecs.read_storage::<Monster>();

                        let worldmap_resource = self.ecs.fetch::<Map>();

                        let mut can_heal = true;
                        let viewshed = viewshed_components.get(*player_entity).unwrap();
                        for tile in viewshed.visible_tiles.iter() {
                            let idx = worldmap_resource.xy_idx(tile.x, tile.y);
                            for entity_id in worldmap_resource.tile_content[idx].iter() {
                                let mob = monsters.get(*entity_id);
                                match mob {
                                    None => {}
                                    Some(_) => { can_heal = false; }
                                }
                            }
                        }

                        if last_acted.lastacted + 1000 < time_now && stats.hit_points.current > 0 && can_heal && !hungry_or_starving {
                            let pools = all_pools.get_mut(*player_entity).unwrap();
                            pools.hit_points.current = i32::min(pools.hit_points.current + 1, pools.hit_points.max);
                        }
                    }
                }
            }
        }
    }
}


fn main() -> rltk::BError {
    use rltk::RltkBuilder;
    let mut context = RltkBuilder::simple(80, 60)
        .unwrap()
        .with_title("Rainfall")
        .build()?;
    context.with_post_scanlines(true);

    let mut gs = State {
        ecs: World::new(),
        mapgen_next_state : Some(RunState::MainMenu{ menu_selection: gui::MainMenuSelection::NewGame }),
        mapgen_index : 0,
        mapgen_history: Vec::new(),
        mapgen_timer: 0.0
    };


    gs.ecs.register::<Position>();
    gs.ecs.register::<Renderable>();
    gs.ecs.register::<LastActed>();
    gs.ecs.register::<Viewshed>();
    gs.ecs.register::<BlocksTile>();
    gs.ecs.register::<SimpleMarker<SerializeMe>>();
    gs.ecs.register::<SerializationHelper>();

    gs.ecs.register::<Name>();

    gs.ecs.register::<Player>();
    gs.ecs.register::<Monster>();

    gs.ecs.register::<SufferDamage>();
    gs.ecs.register::<InBackpack>();
    gs.ecs.register::<LastActed>();

    gs.ecs.register::<WantsToMelee>();
    gs.ecs.register::<WantsToUseItem>();
    gs.ecs.register::<WantsToDropItem>();
    gs.ecs.register::<WantsToPickupItem>();
    gs.ecs.register::<WantsToRemoveItem>();

    gs.ecs.register::<Item>();
    gs.ecs.register::<Ranged>();
    gs.ecs.register::<Consumable>();
    gs.ecs.register::<AreaOfEffect>();

    gs.ecs.register::<ProvidesHealing>();
    gs.ecs.register::<InflictsDamage>();
    gs.ecs.register::<Confusion>();
    gs.ecs.register::<ProvidesFood>();
    gs.ecs.register::<MagicMapper>();

    gs.ecs.register::<Equippable>();
    gs.ecs.register::<Equipped>();
    gs.ecs.register::<MeleeWeapon>();
    gs.ecs.register::<Wearable>();
    gs.ecs.register::<NaturalAttackDefense>();

    gs.ecs.register::<TimeKeeper>();
    gs.ecs.register::<ParticleLifetime>();
    gs.ecs.register::<HungerClock>();

    gs.ecs.register::<EntryTrigger>();
    gs.ecs.register::<EntityMoved>();
    gs.ecs.register::<Hidden>();
    gs.ecs.register::<SingleActivation>();
    gs.ecs.register::<Door>();
    gs.ecs.register::<BlocksVisibility>();
    
    gs.ecs.register::<Bystander>();
    gs.ecs.register::<Vendor>();
    gs.ecs.register::<Quips>();
    gs.ecs.register::<LootTable>();
    gs.ecs.register::<Carnivore>();
    gs.ecs.register::<Herbivore>();
    
    gs.ecs.register::<Attributes>();
    gs.ecs.register::<Skills>();
    gs.ecs.register::<Pools>();
    gs.ecs.register::<OtherLevelPosition>();
    gs.ecs.register::<DMSerializationHelper>();
    
    gs.ecs.register::<LightSource>();

    gs.ecs.insert(SimpleMarkerAllocator::<SerializeMe>::new());
    
    raws::load_raws();
    
    gs.ecs.insert(map::MasterDungeonMap::new());
    gs.ecs.insert(Map::new(1, 64, 64, "New Map"));
    gs.ecs.insert(Point::new(0, 0));
    gs.ecs.insert(rltk::RandomNumberGenerator::new());
    let player_entity = spawner::player(&mut gs.ecs, 0, 0);
    gs.ecs.insert(player_entity);
    gs.ecs.insert(RunState::MapGeneration{} );
    gs.ecs.insert(gamelog::GameLog{ entries : vec!["Welcome to Rainfall".to_string()] });
    gs.ecs.insert(particle_system::ParticleBuilder::new());
    gs.ecs.insert(rex_assets::RexAssets::new());

    gs.generate_world_map(1, 0);
    
    let is_paused = false;
    gs.ecs.insert(is_paused);

    rltk::main_loop(context, gs)
}
