use super::{BuilderChain, BuilderMap, MetaMapBuilder, TileType, PrefabBuilder};

pub fn town_builder(new_depth: i32, width: i32, height: i32) -> BuilderChain {
    let noonbreeze_path = "../resources/noonbreeze.xp";
    let mut chain = BuilderChain::new(new_depth, width, height, "The Town of Noonbreeze");
    chain.start_with(PrefabBuilder::rex_level(noonbreeze_path));
    chain.with(TownBuilder::new());
    chain
}

pub struct TownBuilder {}

impl MetaMapBuilder for TownBuilder {
    #[allow(dead_code)]
    fn build_map(&mut self, build_data : &mut BuilderMap) {
        self.add_doors(build_data);
        
        
        
        // Peasant House 1
        build_data.spawn_list.push((build_data.map.xy_idx(21, 42), "Table".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(22, 42), "ChairL".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(21, 44), "Bed".to_string()));
        
        // Peasant House 2
        build_data.spawn_list.push((build_data.map.xy_idx(21, 48), "Table".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(22, 48), "ChairL".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(21, 50), "Bed".to_string()));
        
        // Peasant House 3
        build_data.spawn_list.push((build_data.map.xy_idx(21, 54), "Table".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(22, 54), "ChairL".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(21, 56), "Bed".to_string()));
        
        // Peasant House 4
        build_data.spawn_list.push((build_data.map.xy_idx(32, 42), "ChairR".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(33, 42), "Table".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(33, 44), "Bed".to_string()));
        
        // Peasant House 5
        build_data.spawn_list.push((build_data.map.xy_idx(32, 48), "ChairR".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(33, 48), "Table".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(33, 50), "Bed".to_string()));
        
        // Peasant House 6
        build_data.spawn_list.push((build_data.map.xy_idx(32, 54), "ChairR".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(33, 54), "Table".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(33, 56), "Bed".to_string()));
        
        
        
        // Peasant House 7
        build_data.spawn_list.push((build_data.map.xy_idx(37, 42), "Table".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(38, 42), "ChairL".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(37, 44), "Bed".to_string()));
        
        // Peasant House 8
        build_data.spawn_list.push((build_data.map.xy_idx(37, 48), "Table".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(38, 48), "ChairL".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(37, 50), "Bed".to_string()));
        
        // Peasant House 9
        build_data.spawn_list.push((build_data.map.xy_idx(37, 54), "Table".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(38, 54), "ChairL".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(37, 56), "Bed".to_string()));
        
        // Peasant House 10
        build_data.spawn_list.push((build_data.map.xy_idx(48, 42), "ChairR".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(49, 42), "Table".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(49, 44), "Bed".to_string()));
        
        // Peasant House 11
        build_data.spawn_list.push((build_data.map.xy_idx(48, 48), "ChairR".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(49, 48), "Table".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(49, 50), "Bed".to_string()));
        
        // Peasant House 12
        build_data.spawn_list.push((build_data.map.xy_idx(48, 54), "ChairR".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(49, 54), "Table".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(49, 56), "Bed".to_string()));
        
        
        
        // The Inn
        build_data.spawn_list.push((build_data.map.xy_idx(56, 30), "Bed".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(56, 27), "Bed".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(50, 27), "Bed".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(50, 30), "Bed".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(50, 33), "Bed".to_string()));
        
        
        
        // Blacksmith
        build_data.spawn_list.push((build_data.map.xy_idx(45, 29), "Armor Stand".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(45, 33), "Weapon Rack".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(47, 29), "Water Trough".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(48, 29), "Anvil".to_string()));
        
        // Tavern
        build_data.spawn_list.push((build_data.map.xy_idx(33, 27), "Keg".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(34, 27), "Keg".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(35, 27), "Cabinet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(36, 27), "Cabinet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(33, 29), "Countertop".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(34, 29), "Countertop".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(35, 29), "Countertop".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(36, 29), "Countertop".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(33, 30), "Stool".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(34, 30), "Stool".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(35, 30), "Stool".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(36, 30), "Stool".to_string())); 
        build_data.spawn_list.push((build_data.map.xy_idx(33, 34), "ChairR".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(34, 34), "Table".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(35, 34), "ChairL".to_string())); 
        build_data.spawn_list.push((build_data.map.xy_idx(33, 32), "ChairR".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(34, 32), "Table".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(35, 32), "ChairL".to_string()));
        
        
        
        // Carpenter
        build_data.spawn_list.push((build_data.map.xy_idx(26, 25), "Bed".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(25, 27), "Cabinet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(21, 25), "Cabinet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(22, 25), "Cabinet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(23, 25), "Cabinet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(22, 27), "Stool".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(21, 28), "Countertop".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(22, 28), "Countertop".to_string()));
        
        
        // Herbalist
        build_data.spawn_list.push((build_data.map.xy_idx(26, 22), "Cabinet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(26, 23), "Bed".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(21, 23), "Stool".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(22, 22), "Countertop".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(22, 23), "Countertop".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(21, 20), "Potted Plant".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(22, 20), "Potted Plant".to_string()));
    }
}

impl TownBuilder {
    pub fn new() -> Box<TownBuilder> {
        Box::new(TownBuilder{})
    }

    fn add_doors(&mut self, build_data : &mut BuilderMap) {
        for idx in 1 .. build_data.map.tiles.len() {
            if build_data.map.tiles[idx] == TileType::DoorSpot {
                build_data.map.tiles[idx] = TileType::Floor;
                build_data.spawn_list.push((idx, "Door".to_string()));
            }
        }
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
