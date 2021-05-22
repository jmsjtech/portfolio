use rltk::{VirtualKeyCode, Rltk, Point};
use specs::prelude::*;
use std::cmp::{max, min};
use super::{Position, Player, Viewshed, State, Map, LastActed};
use super::{CombatStats, WantsToMelee, WantsToPickupItem, Item, GameLog, RunState};
use std::time::{SystemTime, UNIX_EPOCH};


pub fn try_move_player(delta_x: i32, delta_y: i32, ecs: &mut World) {
    let mut positions = ecs.write_storage::<Position>();
    let players = ecs.read_storage::<Player>();
    let mut viewsheds = ecs.write_storage::<Viewshed>();
    let entities = ecs.entities();
    let combat_stats = ecs.read_storage::<CombatStats>();
    let map = ecs.fetch::<Map>();
    let mut wants_to_melee = ecs.write_storage::<WantsToMelee>();
    let mut lastactions = ecs.write_storage::<LastActed>();

    for (entity, _player, pos, viewshed, lastaction) in (&entities, &players, &mut positions, &mut viewsheds, &mut lastactions).join() {
        if pos.x + delta_x < 1 || pos.x + delta_x > map.width-1 || pos.y + delta_y < 1 || pos.y + delta_y > map.height-1 { return; }
        let destination_idx = map.xy_idx(pos.x + delta_x, pos.y + delta_y);

        for potential_target in map.tile_content[destination_idx].iter() {
            let target = combat_stats.get(*potential_target);
            if let Some(_target) = target {
                wants_to_melee.insert(entity, WantsToMelee{ target: *potential_target }).expect("Add target failed");
                return;
            }
        }

        if !map.blocked[destination_idx] && lastaction.lastacted + lastaction.speed_in_ms < SystemTime::now().duration_since(UNIX_EPOCH).expect("Clock may have gone backwards?").as_millis() {
            pos.x = min(79 , max(0, pos.x + delta_x));
            pos.y = min(49, max(0, pos.y + delta_y));

            viewshed.dirty = true;
            let mut ppos = ecs.write_resource::<Point>();
            ppos.x = pos.x;
            ppos.y = pos.y;
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


pub fn player_input(gs: &mut State, ctx: &mut Rltk, state: RunState) -> RunState {
    match ctx.key {
        None => {}
        Some(key) => match key {
            VirtualKeyCode::G => get_item(&mut gs.ecs),

            VirtualKeyCode::I => return RunState::ShowInventory,
            VirtualKeyCode::X => return RunState::ShowDropItem,
            VirtualKeyCode::Escape => return RunState::SaveGame,

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
