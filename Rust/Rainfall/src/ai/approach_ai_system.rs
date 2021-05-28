use specs::prelude::*;
use crate::{LastActed, WantsToApproach, Position, Map, Viewshed, EntityMoved};
use std::time::{SystemTime, UNIX_EPOCH};

pub struct ApproachAI {}

impl<'a> System<'a> for ApproachAI {
    #[allow(clippy::type_complexity)]
    type SystemData = ( 
        WriteStorage<'a, LastActed>,
        WriteStorage<'a, WantsToApproach>,
        WriteStorage<'a, Position>,
        WriteExpect<'a, Map>,
        WriteStorage<'a, Viewshed>,
        WriteStorage<'a, EntityMoved>,
        Entities<'a>
    );

    fn run(&mut self, data : Self::SystemData) {
        let (mut lastactions, mut want_approach, mut positions, mut map, 
            mut viewsheds, mut entity_moved, entities) = data;
            
        for (entity, mut pos, approach, mut viewshed, mut lastacted) in (&entities, &mut positions, &want_approach, &mut viewsheds, &mut lastactions).join() {
            
            let time_now = SystemTime::now().duration_since(UNIX_EPOCH).expect("Clock Error").as_millis();
            if lastacted.lastacted + lastacted.speed_in_ms < time_now { 
                lastacted.lastacted = time_now;
                let path = rltk::a_star_search(
                    map.xy_idx(pos.x, pos.y) as i32, 
                    map.xy_idx(approach.idx % map.width, approach.idx / map.width) as i32, 
                    &mut *map
                );
                if path.success && path.steps.len()>1 {
                    let mut idx = map.xy_idx(pos.x, pos.y);
                    map.blocked[idx] = false;
                    pos.x = path.steps[1] as i32 % map.width;
                    pos.y = path.steps[1] as i32 / map.width;
                    entity_moved.insert(entity, EntityMoved{}).expect("Unable to insert marker");
                    idx = map.xy_idx(pos.x, pos.y);
                    map.blocked[idx] = true;
                    viewshed.dirty = true;
                }
            }
        }

        want_approach.clear();
    }
}
