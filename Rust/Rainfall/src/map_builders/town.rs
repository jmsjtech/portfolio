use super::{BuilderChain, BuilderMap, InitialMapBuilder, TileType, Position, PrefabBuilder};
use crate::rex_assets::RexAssets;
use std::collections::HashSet;

pub fn town_builder(new_depth: i32, width: i32, height: i32) -> BuilderChain {
    let noonbreeze_path = "../resources/noonbreeze.xp";
    let mut chain = BuilderChain::new(new_depth, width, height, "The Town of Noonbreeze");
    chain.start_with(PrefabBuilder::rex_level(noonbreeze_path));
    //chain.with(TownBuilder::new());
    chain
}

pub struct TownBuilder {}

impl InitialMapBuilder for TownBuilder {
    #[allow(dead_code)]
    fn build_map(&mut self, build_data : &mut BuilderMap) {
        self.build_rooms(build_data);
    }
}

enum BuildingTag {
    Pub, Temple, Blacksmith, Clothier, Alchemist, PlayerHouse, Hovel, Abandoned, Unassigned
}

impl TownBuilder {
    pub fn new() -> Box<TownBuilder> {
        Box::new(TownBuilder{})
    }

    pub fn build_rooms(&mut self, build_data : &mut BuilderMap) {
        
        // Make visible for screenshot
        for t in build_data.map.visible_tiles.iter_mut() {
            *t = true;
        }
        build_data.take_snapshot();
    }

    

    

    // fn build_pub(&mut self,
    //     building: &(i32, i32, i32, i32),
    //     build_data : &mut BuilderMap)
    // {
    //     // Place the player
    //     build_data.starting_position = Some(Position{
    //         x : building.0 + (building.2 / 2),
    //         y : building.1 + (building.3 / 2)
    //     });
    //     let player_idx = build_data.map.xy_idx(building.0 + (building.2 / 2),
    //         building.1 + (building.3 / 2));
    // 
    //     // Place other items
    //     let mut to_place : Vec<&str> = vec!["Barkeep", "Shady Salesman", "Patron", "Patron", "Keg",
    //         "Table", "Chair", "Table", "Chair"];
    //     self.random_building_spawn(building, build_data, &mut to_place, player_idx);
    // }
    // 
    // fn build_temple(&mut self,
    //     building: &(i32, i32, i32, i32),
    //     build_data : &mut BuilderMap)
    // {
    //     // Place items
    //     let mut to_place : Vec<&str> = vec!["Priest", "Altar", "Parishioner", "Parishioner", "Chair", "Chair", "Candle", "Candle"];
    //     self.random_building_spawn(building, build_data, &mut to_place, 0);
    // }
    // 
    // fn build_smith(&mut self,
    //     building: &(i32, i32, i32, i32),
    //     build_data : &mut BuilderMap)
    // {
    //     // Place items
    //     let mut to_place : Vec<&str> = vec!["Blacksmith", "Anvil", "Water Trough", "Weapon Rack", "Armor Stand"];
    //     self.random_building_spawn(building, build_data, &mut to_place, 0);
    // }
    // 
    // fn build_clothier(&mut self,
    //     building: &(i32, i32, i32, i32),
    //     build_data : &mut BuilderMap)
    // {
    //     // Place items
    //     let mut to_place : Vec<&str> = vec!["Clothier", "Cabinet", "Table", "Loom", "Hide Rack"];
    //     self.random_building_spawn(building, build_data, &mut to_place, 0);
    // }
    // 
    // fn build_alchemist(&mut self,
    //     building: &(i32, i32, i32, i32),
    //     build_data : &mut BuilderMap)
    // {
    //     // Place items
    //     let mut to_place : Vec<&str> = vec!["Alchemist", "Chemistry Set", "Dead Thing", "Chair", "Table"];
    //     self.random_building_spawn(building, build_data, &mut to_place, 0);
    // }
    // 
    // fn build_my_house(&mut self,
    //     building: &(i32, i32, i32, i32),
    //     build_data : &mut BuilderMap)
    // {
    //     // Place items
    //     let mut to_place : Vec<&str> = vec!["Mom", "Bed", "Cabinet", "Chair", "Table"];
    //     self.random_building_spawn(building, build_data, &mut to_place, 0);
    // }
    // 
    // fn build_hovel(&mut self,
    //     building: &(i32, i32, i32, i32),
    //     build_data : &mut BuilderMap)
    // {
    //     // Place items
    //     let mut to_place : Vec<&str> = vec!["Peasant", "Bed", "Chair", "Table"];
    //     self.random_building_spawn(building, build_data, &mut to_place, 0);
    // }
    // 
    // fn build_abandoned_house(&mut self,
    //     building: &(i32, i32, i32, i32),
    //     build_data : &mut BuilderMap)
    // {
    //     for y in building.1 .. building.1 + building.3 {
    //         for x in building.0 .. building.0 + building.2 {
    //             let idx = build_data.map.xy_idx(x, y);
    //             if build_data.map.tiles[idx] == TileType::WoodFloor && idx != 0 && crate::rng::roll_dice(1, 2)==1 {
    //                 build_data.spawn_list.push((idx, "Rat".to_string()));
    //             }
    //         }
    //     }
    //}

    
}
