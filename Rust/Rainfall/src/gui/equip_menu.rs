use rltk::prelude::*;
use specs::prelude::*;
use crate::{State, InBackpack, Equippable };
use super::{get_item_display_name, get_equip_slot, ItemMenuResult, item_result_menu};

pub fn equip_menu(gs : &mut State, ctx : &mut Rltk) -> (ItemMenuResult, Option<Entity>) {
    let mut draw_batch = DrawBatch::new();

    let player_entity = gs.ecs.fetch::<Entity>();
    let backpack = gs.ecs.read_storage::<InBackpack>();
    let entities = gs.ecs.entities();
    let equippables = gs.ecs.read_storage::<Equippable>();

    let mut items : Vec<(Entity, String)> = Vec::new();
    (&entities, &backpack).join()
        .filter(|item| item.1.owner == *player_entity )
        .for_each(|item| {
            if equippables.contains(item.0) {
                    if let Some(equip) = equippables.get(item.0) {
                        let name = format!("{} ({})", get_item_display_name(&gs.ecs, item.0), get_equip_slot(equip.slot));
                        items.push((item.0, name));
                    }
            }
        });
        
    let result;
        
    if items.len() != 0 {
        result = item_result_menu(
            &mut draw_batch,
            "Equip which item?",
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
