use rltk::prelude::*;
use specs::prelude::*;
use crate::{State, InBackpack };
use super::{get_item_display_name, ItemMenuResult, item_result_menu};

pub fn drink_potion_menu(gs : &mut State, ctx : &mut Rltk) -> (ItemMenuResult, Option<Entity>) {
    let mut draw_batch = DrawBatch::new();

    let player_entity = gs.ecs.fetch::<Entity>();
    let backpack = gs.ecs.read_storage::<InBackpack>();
    let entities = gs.ecs.entities();

    let mut items : Vec<(Entity, String)> = Vec::new();
    (&entities, &backpack).join()
        .filter(|item| item.1.owner == *player_entity )
        .for_each(|item| {
            if get_item_display_name(&gs.ecs, item.0).to_lowercase().contains("potion") {
                items.push((item.0, get_item_display_name(&gs.ecs, item.0)))
            }
        });
        
    let result;
        
    if items.len() != 0 {
        result = item_result_menu(
            &mut draw_batch,
            "Quaff which potion?",
            items.len(),
            &items,
            ctx.key
        );
    } else {
        crate::gamelog::Logger::new().append("No potions to drink.").log();
        result = (ItemMenuResult::Cancel, None);
    }
    draw_batch.submit(6000).expect("Unhelpful Error Code");
    result
}
