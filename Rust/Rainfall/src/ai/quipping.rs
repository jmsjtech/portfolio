use specs::prelude::*;
use crate::{gamelog::GameLog, Quips, Name, Viewshed};

pub struct QuipSystem {}

impl<'a> System<'a> for QuipSystem {
    #[allow(clippy::type_complexity)]
    type SystemData = ( WriteExpect<'a, GameLog>,
        WriteStorage<'a, Quips>,
        ReadStorage<'a, Name>,
        ReadExpect<'a, rltk::Point>,
        ReadStorage<'a, Viewshed>,
        WriteExpect<'a, rltk::RandomNumberGenerator>);

    fn run(&mut self, data : Self::SystemData) {
        let (mut gamelog, mut quips, names, player_pos, viewsheds, mut rng) = data;

        for (quip, name, viewshed) in (&mut quips, &names, &viewsheds).join() {
            if !quip.available.is_empty() && viewshed.visible_tiles.contains(&player_pos) && rng.roll_dice(1,20)==1 {
                let quip_index = 
                    if quip.available.len() == 1 { 0 } 
                    else { (rng.roll_dice(1, quip.available.len() as i32)-1) as usize };
                
                gamelog.entries.push(
                    format!("{} says \"{}\"", name.name, quip.available[quip_index])
                );
                quip.available.remove(quip_index);
            }                
        }
    }
}
