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

pub mod camera;
pub mod raws;

mod saveload_system;
mod random_table;
mod trigger_system;

mod particle_system;
use particle_system::*;

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
    ShowRemoveItem,
    PlayerDied,
    MagicMapReveal { row: i32 },
    MapGeneration
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
        let mut vis = VisibilitySystem{};
        vis.run_now(&self.ecs);
        let mut mob = MonsterAI{};
        mob.run_now(&self.ecs);
        let mut triggers = trigger_system::TriggerSystem{};
        triggers.run_now(&self.ecs);
        let mut mapindex = MapIndexingSystem{};
        mapindex.run_now(&self.ecs);
        let mut melee = MeleeCombatSystem{};
        melee.run_now(&self.ecs);
        let mut damage = DamageSystem{};
        damage.run_now(&self.ecs);
        let mut pickup = ItemCollectionSystem{};
        pickup.run_now(&self.ecs);
        let mut items = ItemUseSystem{};
        items.run_now(&self.ecs);
        let mut drop_items = ItemDropSystem{};
        drop_items.run_now(&self.ecs);
        let mut item_remove = ItemRemoveSystem{};
        item_remove.run_now(&self.ecs);
        let mut particles = particle_system::ParticleSpawnSystem{};
        particles.run_now(&self.ecs);

        self.ecs.maintain();
    }
    
    fn generate_world_map(&mut self, new_depth : i32) {
        self.mapgen_index = 0;
        self.mapgen_timer = 0.0;
        self.mapgen_history.clear();
        let mut rng = self.ecs.write_resource::<rltk::RandomNumberGenerator>();
        let mut builder = map_builders::random_builder(new_depth, &mut rng, 128, 128);
        builder.build_map(&mut rng);
        std::mem::drop(rng);
        self.mapgen_history = builder.build_data.history.clone();
        let player_start;
        {
            let mut worldmap_resource = self.ecs.write_resource::<Map>();
            *worldmap_resource = builder.build_data.map.clone();
            player_start = builder.build_data.starting_position.as_mut().unwrap().clone();
        }
    
        // Spawn bad guys
        builder.spawn_entities(&mut self.ecs);

       // Place the player and update resources
       let (player_x, player_y) = (player_start.x, player_start.y);
       let mut player_position = self.ecs.write_resource::<Point>();
       *player_position = Point::new(player_x, player_y);
       let mut position_components = self.ecs.write_storage::<Position>();
       let player_entity = self.ecs.fetch::<Entity>();
       let player_pos_comp = position_components.get_mut(*player_entity);
       if let Some(player_pos_comp) = player_pos_comp {
           player_pos_comp.x = player_x;
           player_pos_comp.y = player_y;
       }

       // Mark the player's visibility as dirty
       let mut viewshed_components = self.ecs.write_storage::<Viewshed>();
       let vs = viewshed_components.get_mut(*player_entity);
       if let Some(vs) = vs {
           vs.dirty = true;
       }
   }

    fn entities_to_remove_on_level_change(&mut self) -> Vec<Entity> {
        let entities = self.ecs.entities();
        let player = self.ecs.read_storage::<Player>();
        let backpack = self.ecs.read_storage::<InBackpack>();
        let player_entity = self.ecs.fetch::<Entity>();
        let equipped = self.ecs.read_storage::<Equipped>();

        let mut to_delete : Vec<Entity> = Vec::new();
        for entity in entities.join() {
            let mut should_delete = true;

            // Don't delete the player
            let p = player.get(entity);
            if let Some(_p) = p {
                should_delete = false;
            }

            // Don't delete the player's equipment
            let bp = backpack.get(entity);
            if let Some(bp) = bp {
                if bp.owner == *player_entity {
                    should_delete = false;
                }
            }

            let eq = equipped.get(entity);
            if let Some(eq) = eq {
               if eq.owner == *player_entity {
                   should_delete = false;
               }
            }

            if should_delete {
                to_delete.push(entity);
            }
        }

        to_delete
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

        // Build a new map and place the player
        self.generate_world_map(1);
    }

    fn goto_next_level(&mut self) {
        // Delete entities that aren't the player or his/her equipment
        let to_delete = self.entities_to_remove_on_level_change();
        for target in to_delete {
            self.ecs.delete_entity(target).expect("Unable to delete entity");
        }

        // Build a new map and place the player
        let current_depth;
        {
            let worldmap_resource = self.ecs.fetch::<Map>();
            current_depth = worldmap_resource.depth;
        }
        self.generate_world_map(current_depth + 1);

        // Notify the player and give them some health
        let player_entity = self.ecs.fetch::<Entity>();
        let mut gamelog = self.ecs.fetch_mut::<gamelog::GameLog>();
        gamelog.entries.push("You descend to the next level, and take a moment to heal.".to_string());
        let mut player_health_store = self.ecs.write_storage::<CombatStats>();
        let player_health = player_health_store.get_mut(*player_entity);
        if let Some(player_health) = player_health {
            player_health.hp = i32::max(player_health.hp, player_health.max_hp / 2);
        }
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
                }
                ctx.cls();
                if self.mapgen_index < self.mapgen_history.len() as usize {
                    if self.mapgen_index < self.mapgen_history.len() { camera::render_debug_map(&self.mapgen_history[self.mapgen_index], ctx); }
                }

                self.mapgen_timer += ctx.frame_time_ms;
                if self.mapgen_timer > 100.0 {
                    self.mapgen_timer = 0.0;
                    self.mapgen_index += 1;
                    if self.mapgen_index >= self.mapgen_history.len() {
                        newrunstate = RunState::MainMenu{ menu_selection: gui::MainMenuSelection::NewGame };
                    }
                }
            }
            RunState::PreRun => {
                newrunstate = RunState::Running;
            }
            RunState::Running => {
                self.run_systems();
                self.ecs.maintain();
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
                self.goto_next_level();
                newrunstate = RunState::PreRun;
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

        //let mut clock = self.ecs.fetch_mut::<TimeKeeper>();
        let time_now = SystemTime::now().duration_since(UNIX_EPOCH).expect("Clock may have gone backwards?").as_millis();
        let player_entity = self.ecs.fetch::<Entity>();
        let mut combat_stats = self.ecs.write_storage::<CombatStats>();
        let mut lastactions = self.ecs.write_storage::<LastActed>();
        let mut clocks = self.ecs.write_storage::<TimeKeeper>();

        let mut gamelog = self.ecs.fetch_mut::<GameLog>();

        let stats = combat_stats.get_mut(*player_entity);
        let last_acted = lastactions.get_mut(*player_entity);
        let clock = clocks.get_mut(*player_entity);

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
                                SufferDamage::new_damage(&mut inflict_damage, *player_entity, 1);
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

                        if last_acted.lastacted + 1000 < time_now && stats.hp > 0 && can_heal && !hungry_or_starving {
                            stats.hp = i32::min(stats.max_hp, stats.hp + 1);
                        }
                    }
                }
            }
        }
    }
}


fn main() -> rltk::BError {
    use rltk::RltkBuilder;
    let mut context = RltkBuilder::simple80x50()
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
    gs.ecs.register::<CombatStats>();

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
    gs.ecs.register::<MeleePowerBonus>();
    gs.ecs.register::<DefenseBonus>();

    gs.ecs.register::<TimeKeeper>();
    gs.ecs.register::<ParticleLifetime>();
    gs.ecs.register::<HungerClock>();

    gs.ecs.register::<EntryTrigger>();
    gs.ecs.register::<EntityMoved>();
    gs.ecs.register::<Hidden>();
    gs.ecs.register::<SingleActivation>();
    gs.ecs.register::<Door>();
    gs.ecs.register::<BlocksVisibility>();

    gs.ecs.insert(SimpleMarkerAllocator::<SerializeMe>::new());
    
    raws::load_raws();

    gs.ecs.insert(Map::new(1, 64, 64));
    gs.ecs.insert(Point::new(0, 0));
    gs.ecs.insert(rltk::RandomNumberGenerator::new());
    let player_entity = spawner::player(&mut gs.ecs, 0, 0);
    gs.ecs.insert(player_entity);
    gs.ecs.insert(RunState::MapGeneration{} );
    gs.ecs.insert(gamelog::GameLog{ entries : vec!["Welcome to Rainfall".to_string()] });
    gs.ecs.insert(particle_system::ParticleBuilder::new());
    gs.ecs.insert(rex_assets::RexAssets::new());

    gs.generate_world_map(1);

    rltk::main_loop(context, gs)
}
