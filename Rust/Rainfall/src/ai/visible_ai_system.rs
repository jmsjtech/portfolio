use specs::prelude::*;
use crate::{LastActed, Faction, Position, Map, raws::Reaction, Viewshed, WantsToFlee, WantsToApproach, Chasing};
use std::time::{SystemTime, UNIX_EPOCH};

pub struct VisibleAI {}

impl<'a> System<'a> for VisibleAI {
    #[allow(clippy::type_complexity)]
    type SystemData = ( 
        WriteStorage<'a, LastActed>,
        ReadStorage<'a, Faction>,
        ReadStorage<'a, Position>,
        ReadExpect<'a, Map>,
        WriteStorage<'a, WantsToApproach>,
        WriteStorage<'a, WantsToFlee>,
        Entities<'a>,
        ReadExpect<'a, Entity>,
        ReadStorage<'a, Viewshed>,
        WriteStorage<'a, Chasing>
    );

    fn run(&mut self, data : Self::SystemData) {
        let (mut lastactions, factions, positions, map, mut want_approach, mut want_flee, entities, player, viewsheds, mut chasing) = data;

        for (entity, lastacted, my_faction, pos, viewshed) in (&entities, &mut lastactions, &factions, &positions, &viewsheds).join() {
            let time_now = SystemTime::now().duration_since(UNIX_EPOCH).expect("Clock Error").as_millis();
            if lastacted.lastacted + lastacted.speed_in_ms < time_now { 
                lastacted.lastacted = time_now;
                if entity != *player {
                    let my_idx = map.xy_idx(pos.x, pos.y);
                    let mut reactions : Vec<(usize, Reaction, Entity)> = Vec::new();
                    let mut flee : Vec<usize> = Vec::new();
                    for visible_tile in viewshed.visible_tiles.iter() {
                        let idx = map.xy_idx(visible_tile.x, visible_tile.y);
                        if my_idx != idx {
                            evaluate(idx, &map, &factions, &my_faction.name, &mut reactions);
                        }
                    }

                    for reaction in reactions.iter() {
                        match reaction.1 {
                            Reaction::Attack => {
                                want_approach.insert(entity, WantsToApproach{ idx: reaction.0 as i32 }).expect("Unable to insert");
                                chasing.insert(entity, Chasing{ target: reaction.2}).expect("Unable to insert");
                            }
                            Reaction::Flee => {
                                flee.push(reaction.0);
                            }
                            _ => {}
                        }
                    }
                    if !flee.is_empty() {
                        want_flee.insert(entity, WantsToFlee{ indices : flee }).expect("Unable to insert");
                    }
                }
            }
        }
    }
}

fn evaluate(idx : usize, map : &Map, factions : &ReadStorage<Faction>, my_faction : &str, reactions : &mut Vec<(usize, Reaction, Entity)>) {
    for other_entity in map.tile_content[idx].iter() {
        if let Some(faction) = factions.get(*other_entity) {
            reactions.push((
                idx, 
                crate::raws::faction_reaction(my_faction, &faction.name, &crate::raws::RAWS.lock().unwrap()),
                *other_entity
            ));
        }
    }
}
