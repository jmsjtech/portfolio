use specs::prelude::*;
use crate::{Confusion, LastActed};
use std::time::{SystemTime, UNIX_EPOCH};

pub struct TurnStatusSystem {}

impl<'a> System<'a> for TurnStatusSystem {
    #[allow(clippy::type_complexity)]
    type SystemData = ( WriteStorage<'a, Confusion>,
                        Entities<'a>,
                        WriteStorage<'a, LastActed>);

    fn run(&mut self, data : Self::SystemData) {
        let (mut confusion, entities, mut lastactions) = data;

        let mut not_confused : Vec<Entity> = Vec::new();
        for (entity, lastacted, confused) in (&entities, &mut lastactions, &mut confusion).join() {
            let time_now = SystemTime::now().duration_since(UNIX_EPOCH).expect("Clock Error").as_millis();
            if lastacted.lastacted + lastacted.speed_in_ms < time_now {
                lastacted.lastacted = time_now;
                confused.turns -= 1;
                if confused.turns < 1 {
                    not_confused.push(entity);
                } 
            }
        }

        for e in not_confused {
            confusion.remove(e);
        }
    }
}
