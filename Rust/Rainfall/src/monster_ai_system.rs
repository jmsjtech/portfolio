use specs::prelude::*;
use super::{Viewshed, Monster, LastActed, Position, Map, WantsToMelee, Confusion, ParticleBuilder};
use rltk::Point;
use std::time::{SystemTime, UNIX_EPOCH};

pub struct MonsterAI {}

impl<'a> System<'a> for MonsterAI {
    #[allow(clippy::type_complexity)]
    type SystemData = ( WriteExpect<'a, Map>,
                        ReadExpect<'a, Point>,
                        ReadExpect<'a, Entity>,
                        Entities<'a>,
                        WriteStorage<'a, Viewshed>,
                        ReadStorage<'a, Monster>,
                        WriteStorage<'a, Position>,
                        WriteStorage<'a, WantsToMelee>,
                        WriteStorage<'a, Confusion>,
                        WriteStorage<'a, LastActed>,
                        WriteExpect<'a, ParticleBuilder>);

    fn run(&mut self, data : Self::SystemData) {
        let (mut map, player_pos, player_entity, entities, mut viewshed, monster, mut position, mut wants_to_melee, mut confused, mut lastacted, mut particle_builder) = data;

        for (entity, mut viewshed, _monster, lastacted, mut pos) in (&entities, &mut viewshed, &monster, &mut lastacted, &mut position).join() {
            if viewshed.visible_tiles.contains(&*player_pos) && lastacted.lastacted + lastacted.speed_in_ms < SystemTime::now().duration_since(UNIX_EPOCH).expect("Clock Error").as_millis() {
                lastacted.lastacted = SystemTime::now().duration_since(UNIX_EPOCH).expect("Clock Error").as_millis();

                let mut can_act = true;
                let is_confused = confused.get_mut(entity);
                if let Some(i_am_confused) = is_confused {
                    i_am_confused.turns -= 1;
                    if i_am_confused.turns < 1 {
                        confused.remove(entity);
                    }
                    can_act = false;
                    particle_builder.request(pos.x, pos.y, rltk::RGB::named(rltk::MAGENTA), rltk::RGB::named(rltk::BLACK), rltk::to_cp437('?'), 200.0);
                }

                if can_act {
                    let distance = rltk::DistanceAlg::Pythagoras.distance2d(Point::new(pos.x, pos.y), *player_pos);
                    if distance < 1.5 {
                        wants_to_melee.insert(entity, WantsToMelee{ target: *player_entity }).expect("Unable to insert attack");
                        return;
                    }

                    let path = rltk::a_star_search(
                        map.xy_idx(pos.x, pos.y),
                        map.xy_idx(player_pos.x, player_pos.y),
                        &mut *map
                    );

                    if path.success && path.steps.len()>1 {
                        pos.x = path.steps[1] as i32 % map.width;
                        pos.y = path.steps[1] as i32 / map.width;
                        viewshed.dirty = true;
                    }
                }
            }
        }
    }
}
