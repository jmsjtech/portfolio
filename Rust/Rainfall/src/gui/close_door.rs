use rltk::prelude::*;
use specs::prelude::*;
use crate::{State, Viewshed, Door, Position, Point, BlocksVisibility, BlocksTile, Player, Renderable, RunState };
use super::ItemMenuResult;


fn close_door_here (ecs: &mut World, off_x: i32, off_y: i32 ) -> RunState {
    let player_pos = ecs.fetch::<Point>();
    let entities = ecs.entities();
    let mut all_doors = ecs.write_storage::<Door>();
    let positions = ecs.read_storage::<Position>();
    let mut blocks_visibility = ecs.write_storage::<BlocksVisibility>();
    let mut blocks_movement = ecs.write_storage::<BlocksTile>();
    let mut renderables = ecs.write_storage::<Renderable>();
    
    let players = ecs.read_storage::<Player>();
    let mut viewsheds = ecs.write_storage::<Viewshed>();
    
    let close_target = Point::new(player_pos.x + off_x, player_pos.y + off_y);
    
    let mut dirty = 0;
        
    for (entity, door, position) in (&entities, &mut all_doors, &positions).join() {
        if position.x == close_target.x && position.y == close_target.y {
            if door.open {
                crate::gamelog::Logger::new().append("You close the door.").log();
                door.open = false;
                blocks_visibility.insert(entity, BlocksVisibility{}).expect("Failed to attach vision blocker");
                blocks_movement.insert(entity, BlocksTile{}).expect("Failed to attach move blocker");
                let glyph = renderables.get_mut(entity).unwrap();
                glyph.glyph = rltk::to_cp437('+');
                dirty = 1;
            } else {
                dirty = 2;
                crate::gamelog::Logger::new().append("The door was already closed, but you close it again anyways.").log();
            }
        }
    }
    
    if dirty == 1 {
        for (_player, mut viewshed) in (&players, &mut viewsheds).join() {
            viewshed.dirty = true;
        }
        return RunState::Ticking;
    } else if dirty == 0 {
        crate::gamelog::Logger::new().append("Nothing to close there.").log();
    }
    return RunState::AwaitingInput;
}

pub fn close_door(gs : &mut State, ctx : &mut Rltk) -> (ItemMenuResult, Option<Point>) {
    let mut draw_batch = DrawBatch::new();
    
    draw_batch.print_color(
        Point::new(20, 18), 
        "Close where?",
        ColorPair::new(RGB::named(rltk::YELLOW), RGB::named(rltk::BLACK))
    );
    
    draw_batch.draw_hollow_box(Rect::with_size(19, 17, 13, 2), ColorPair::new(RGB::from_hex("#999999").expect("Oops"), RGB::named(rltk::BLACK))); // Overall box
    
    draw_batch.submit(10000).expect("Unhelpful Error Code");

    match ctx.key {
        None => return (ItemMenuResult::NoResponse, None),
        Some(key) => {
            match key {
                VirtualKeyCode::Numpad4 => {
                    close_door_here(&mut gs.ecs, -1, 0);
                    return (ItemMenuResult::Selected, None)
                },
                VirtualKeyCode::Numpad6 => {
                    close_door_here(&mut gs.ecs, 1, 0);
                    return (ItemMenuResult::Selected, None)
                },
                VirtualKeyCode::Numpad8 => {
                    close_door_here(&mut gs.ecs, 0, -1);
                    return (ItemMenuResult::Selected, None)
                },
                VirtualKeyCode::Numpad2 => {
                    close_door_here(&mut gs.ecs, 0, 1);
                    return (ItemMenuResult::Selected, None)
                },
    
                // Diagonals
                VirtualKeyCode::Numpad9 => {
                    close_door_here(&mut gs.ecs, 1, -1);
                    return (ItemMenuResult::Selected, None)
                },
                VirtualKeyCode::Numpad7 => {
                    close_door_here(&mut gs.ecs, -1, -1);
                    return (ItemMenuResult::Selected, None)
                },
                VirtualKeyCode::Numpad3 => {
                    close_door_here(&mut gs.ecs, 1, 1);
                    return (ItemMenuResult::Selected, None)
                },
                VirtualKeyCode::Numpad1 => {
                    close_door_here(&mut gs.ecs, -1, 1);
                    return (ItemMenuResult::Selected, None)
                },
                _ => {
                    return (ItemMenuResult::NoResponse, None)
                }
            }
        }
    }
}
