use specs::prelude::*;
use crate::{LastActed, WantsToFlee, Position, Map, Viewshed, EntityMoved};
use std::time::{SystemTime, UNIX_EPOCH};

pub struct FleeAI {}

impl<'a> System<'a> for FleeAI {
    #[allow(clippy::type_complexity)]
    type SystemData = ( 
        WriteStorage<'a, LastActed>,
        WriteStorage<'a, WantsToFlee>,
        WriteStorage<'a, Position>,
        WriteExpect<'a, Map>,
        WriteStorage<'a, Viewshed>,
        WriteStorage<'a, EntityMoved>,
        Entities<'a>
    );

    fn run(&mut self, data : Self::SystemData) {
        let (mut lastactions, mut want_flee, mut positions, mut map, mut viewsheds, mut entity_moved, entities) = data;
            
        for (entity, mut pos, flee, mut viewshed, lastacted) in (&entities, &mut positions, &want_flee, &mut viewsheds, &mut lastactions).join() {
            let time_now = SystemTime::now().duration_since(UNIX_EPOCH).expect("Clock Error").as_millis();
            if lastacted.lastacted + lastacted.speed_in_ms < time_now { 
                lastacted.lastacted = time_now;
                let my_idx = map.xy_idx(pos.x, pos.y);
                map.populate_blocked();
                let flee_map = rltk::DijkstraMap::new(map.width as usize, map.height as usize, &flee.indices, &*map, 100.0);
                let flee_target = rltk::DijkstraMap::find_highest_exit(&flee_map, my_idx, &*map);
                if let Some(flee_target) = flee_target {
                    if !map.blocked[flee_target as usize] {
                        map.blocked[my_idx] = false;
                        map.blocked[flee_target as usize] = true;
                        viewshed.dirty = true;
                        pos.x = flee_target as i32 % map.width;
                        pos.y = flee_target as i32 / map.width;
                        entity_moved.insert(entity, EntityMoved{}).expect("Unable to insert marker");                        
                    }
                }
            }
        }

        want_flee.clear();
    }
}
