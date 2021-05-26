use rltk::{VirtualKeyCode, Rltk, Point};
use specs::prelude::*;
use std::cmp::{max, min};
use super::{Position, Player, Viewshed, State, Map, LastActed, TileType, TimeKeeper, Name, Renderable, BlocksTile, BlocksVisibility, Door};
use super::{CombatStats, WantsToMelee, WantsToPickupItem, Item, GameLog, RunState, EntityMoved};
use std::time::{SystemTime, UNIX_EPOCH};


pub fn try_move_player(delta_x: i32, delta_y: i32, ecs: &mut World) {
    let mut entity_moved = ecs.write_storage::<EntityMoved>();

    let mut positions = ecs.write_storage::<Position>();
    let players = ecs.read_storage::<Player>();
    let mut viewsheds = ecs.write_storage::<Viewshed>();
    let entities = ecs.entities();
    let combat_stats = ecs.read_storage::<CombatStats>();
    let map = ecs.fetch::<Map>();
    let mut wants_to_melee = ecs.write_storage::<WantsToMelee>();
    let mut lastactions = ecs.write_storage::<LastActed>();
    let mut doors = ecs.write_storage::<Door>();
    let mut blocks_visibility = ecs.write_storage::<BlocksVisibility>();
    let mut blocks_movement = ecs.write_storage::<BlocksTile>();
    let mut renderables = ecs.write_storage::<Renderable>();
    
    for (entity, _player, pos, viewshed, lastaction) in (&entities, &players, &mut positions, &mut viewsheds, &mut lastactions).join() {
        if pos.x + delta_x < 1 || pos.x + delta_x > map.width-1 || pos.y + delta_y < 1 || pos.y + delta_y > map.height-1 { return; }
        let destination_idx = map.xy_idx(pos.x + delta_x, pos.y + delta_y);
    
        for potential_target in map.tile_content[destination_idx].iter() {
            let target = combat_stats.get(*potential_target);
            if let Some(_target) = target {
                wants_to_melee.insert(entity, WantsToMelee{ target: *potential_target }).expect("Add target failed");
                return;
            }
            let door = doors.get_mut(*potential_target);
            if let Some(door) = door {
                door.open = true;
                blocks_visibility.remove(*potential_target);
                blocks_movement.remove(*potential_target);
                let glyph = renderables.get_mut(*potential_target).unwrap();
                glyph.glyph = rltk::to_cp437('/');
                viewshed.dirty = true;
            }
        }
        if !map.blocked[destination_idx] && lastaction.lastacted + lastaction.speed_in_ms < SystemTime::now().duration_since(UNIX_EPOCH).expect("Clock may have gone backwards?").as_millis() {
            lastaction.lastacted = SystemTime::now().duration_since(UNIX_EPOCH).expect("Clock may have gone backwards?").as_millis();
            pos.x = min(map.width-1 , max(0, pos.x + delta_x));
            pos.y = min(map.height-1, max(0, pos.y + delta_y));

            viewshed.dirty = true;
            let mut ppos = ecs.write_resource::<Point>();
            ppos.x = pos.x;
            ppos.y = pos.y;

            entity_moved.insert(entity, EntityMoved{}).expect("Unable to insert marker");
        }
    }

    let items = ecs.read_storage::<Item>();
    let mut gamelog = ecs.fetch_mut::<GameLog>();
    let ppos = ecs.read_resource::<Point>();
    let names = ecs.read_storage::<Name>();


    for (item_entity, _item, position) in (&entities, &items, &mut positions).join() {
        let name = names.get(item_entity);
        if let Some(name) = name {
            if position.x == ppos.x && position.y == ppos.y {
                gamelog.entries.push(format!("There is a {} here.", name.name));
            }
        }
    }
}

fn get_item(ecs: &mut World) {
    let player_pos = ecs.fetch::<Point>();
    let player_entity = ecs.fetch::<Entity>();
    let entities = ecs.entities();
    let items = ecs.read_storage::<Item>();
    let positions = ecs.read_storage::<Position>();
    let mut gamelog = ecs.fetch_mut::<GameLog>();

    let mut target_item : Option<Entity> = None;
    for (item_entity, _item, position) in (&entities, &items, &positions).join() {
        if position.x == player_pos.x && position.y == player_pos.y {
            target_item = Some(item_entity);
        }
    }

    match target_item {
        None => gamelog.entries.push("There is nothing here to pick up.".to_string()),
        Some(item) => {
            let mut pickup = ecs.write_storage::<WantsToPickupItem>();
            pickup.insert(*player_entity, WantsToPickupItem{ collected_by: *player_entity, item }).expect("Unable to insert want to pickup");
        }
    }
}

//fn log_entry(ecs: &mut World, log: String) {
//    let mut gamelog = ecs.fetch_mut::<GameLog>();
//    gamelog.entries.push(log);
//}

pub fn try_next_level(ecs: &mut World) -> bool {
    let player_pos = ecs.fetch::<Point>();
    let map = ecs.fetch::<Map>();
    let player_idx = map.xy_idx(player_pos.x, player_pos.y);

    if map.tiles[player_idx] == TileType::DownStairs {
        true
    } else {
        let mut gamelog = ecs.fetch_mut::<GameLog>();
        gamelog.entries.push("There is no way down from here.".to_string());
        false
    }
}


pub fn player_input(gs: &mut State, ctx: &mut Rltk, state: RunState) -> RunState {
    match ctx.key {
        None => {}
        Some(key) => match key {
            VirtualKeyCode::G => get_item(&mut gs.ecs),

            VirtualKeyCode::Grave => {
                let player_entity = gs.ecs.fetch::<Entity>();
                let mut clocks = gs.ecs.write_storage::<TimeKeeper>();
                let clock = clocks.get_mut(*player_entity);

                if let Some(clock) = clock {
                    clock.day += 1;
                }
            },


            VirtualKeyCode::I => return RunState::ShowInventory,
            VirtualKeyCode::X => return RunState::ShowDropItem,
            VirtualKeyCode::Escape => return RunState::SaveGame,
            VirtualKeyCode::T => return RunState::ShowRemoveItem,

            VirtualKeyCode::Period => {
                    if try_next_level(&mut gs.ecs) {
                        return RunState::NextLevel;
                    }
            },

            VirtualKeyCode::A |
            VirtualKeyCode::Numpad4 => try_move_player(-1, 0, &mut gs.ecs),

            VirtualKeyCode::D |
            VirtualKeyCode::Numpad6 => try_move_player(1, 0, &mut gs.ecs),

            VirtualKeyCode::W |
            VirtualKeyCode::Numpad8 => try_move_player(0, -1, &mut gs.ecs),

            VirtualKeyCode::S |
            VirtualKeyCode::Numpad2 => try_move_player(0, 1, &mut gs.ecs),

            VirtualKeyCode::Numpad7 => try_move_player(-1, -1, &mut gs.ecs),
            VirtualKeyCode::Numpad9 => try_move_player(1, -1, &mut gs.ecs),
            VirtualKeyCode::Numpad1 => try_move_player(-1, 1, &mut gs.ecs),
            VirtualKeyCode::Numpad3 => try_move_player(1, 1, &mut gs.ecs),
            _ => { return state }
        },
    }
    state
}
