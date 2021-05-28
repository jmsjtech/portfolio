use specs::prelude::*;
use crate::{LastActed, MoveMode, Movement, Position, Map, Viewshed, EntityMoved};
use std::time::{SystemTime, UNIX_EPOCH};

pub struct DefaultMoveAI {}

impl<'a> System<'a> for DefaultMoveAI {
    #[allow(clippy::type_complexity)]
    type SystemData = ( 
        WriteStorage<'a, LastActed>,
        ReadStorage<'a, MoveMode>,
        WriteStorage<'a, Position>,
        WriteExpect<'a, Map>,
        WriteStorage<'a, Viewshed>,
        WriteStorage<'a, EntityMoved>,
        WriteExpect<'a, rltk::RandomNumberGenerator>,
        Entities<'a>
    );

    fn run(&mut self, data : Self::SystemData) {
        let (mut lastactions, move_mode, mut positions, mut map, mut viewsheds, mut entity_moved, mut rng, entities) = data;
            
        for (entity, mut pos, mode, mut viewshed, lastacted) in (&entities, &mut positions, &move_mode, &mut viewsheds, &mut lastactions).join() {
            let time_now = SystemTime::now().duration_since(UNIX_EPOCH).expect("Clock Error").as_millis();
            if lastacted.lastacted + lastacted.speed_in_ms < time_now { 
                lastacted.lastacted = time_now;
                match mode.mode {
                    Movement::Static => {},
                    Movement::Random => {
                        let mut x = pos.x;
                        let mut y = pos.y;
                        let move_roll = rng.roll_dice(1, 5);
                        match move_roll {
                            1 => x -= 1,
                            2 => x += 1,
                            3 => y -= 1,
                            4 => y += 1,
                            _ => {}
                        }

                        if x > 0 && x < map.width-1 && y > 0 && y < map.height-1 {
                            let dest_idx = map.xy_idx(x, y);
                            if !map.blocked[dest_idx] {
                                let idx = map.xy_idx(pos.x, pos.y);
                                map.blocked[idx] = false;
                                pos.x = x;
                                pos.y = y;
                                entity_moved.insert(entity, EntityMoved{}).expect("Unable to insert marker");
                                map.blocked[dest_idx] = true;
                                viewshed.dirty = true;
                            }
                        }
                    },
                    Movement::RandomWaypoint{path} => {
                        if let Some(path) = path {
                            // We have a target - go there
                            let mut idx = map.xy_idx(pos.x, pos.y);
                            if path.len()>1 {
                                if !map.blocked[path[1] as usize] {
                                    map.blocked[idx] = false;
                                    pos.x = (path[1] % map.width as usize) as i32;
                                    pos.y = (path[1] / map.width as usize) as i32;
                                    entity_moved.insert(entity, EntityMoved{}).expect("Unable to insert marker");
                                    idx = map.xy_idx(pos.x, pos.y);
                                    map.blocked[idx] = true;
                                    viewshed.dirty = true;
                                    path.remove(0); // Remove the first step in the path
                                }
                                // Otherwise we wait a turn to see if the path clears up
                            } else {
                                mode.mode = Movement::RandomWaypoint{ path : None };
                            }
                        } else {
                            let target_x = rng.roll_dice(1, map.width-2);
                            let target_y = rng.roll_dice(1, map.height-2);
                            let idx = map.xy_idx(target_x, target_y);
                            if tile_walkable(map.tiles[idx]) {
                                let path = rltk::a_star_search(
                                    map.xy_idx(pos.x, pos.y) as i32, 
                                    map.xy_idx(target_x, target_y) as i32, 
                                    &mut *map
                                );
                                if path.success && path.steps.len()>1 {
                                    mode.mode = Movement::RandomWaypoint{ 
                                        path: Some(path.steps)
                                    };
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
