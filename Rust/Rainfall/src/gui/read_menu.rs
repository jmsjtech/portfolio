use rltk::prelude::*;
use specs::prelude::*;
use crate::{State, InBackpack, TeachesSpell };
use super::{get_item_display_name, ItemMenuResult, item_result_menu};

pub fn read_menu(gs : &mut State, ctx : &mut Rltk) -> (ItemMenuResult, Option<Entity>) {
    let mut draw_batch = DrawBatch::new();

    let player_entity = gs.ecs.fetch::<Entity>();
    let backpack = gs.ecs.read_storage::<InBackpack>();
    let entities = gs.ecs.entities();
    let teaches_spells = gs.ecs.read_storage::<TeachesSpell>();

    let mut items : Vec<(Entity, String)> = Vec::new();
    (&entities, &backpack).join()
        .filter(|item| item.1.owner == *player_entity )
        .for_each(|item| {
            if get_item_display_name(&gs.ecs, item.0).to_lowercase().contains("scroll") ||
                teaches_spells.contains(item.0) {
                    let mut name = get_item_display_name(&gs.ecs, item.0);
                    if teaches_spells.contains(item.0) {
                        if let Some(taught) = teaches_spells.get(item.0) {
                            name = format!("{} ({})", get_item_display_name(&gs.ecs, item.0), taught.spell)
                        }
                    }
                items.push((item.0, name));
            }
        });
        
    let result;
        
    if items.len() != 0 {
        result = item_result_menu(
            &mut draw_batch,
            "Read which item?",
            items.len(),
            &items,
            ctx.key
        );
    } else {
        crate::gamelog::Logger::new().append("Nothing to read.").log();
        result = (ItemMenuResult::Cancel, None);
    }
    draw_batch.submit(6000).expect("Unhelpful Error Code");
    result
}
