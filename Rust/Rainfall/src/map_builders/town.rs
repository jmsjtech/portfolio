use super::{BuilderChain, BuilderMap, MetaMapBuilder, TileType, PrefabBuilder};

pub fn town_builder(new_depth: i32, width: i32, height: i32, town_level: i32) -> BuilderChain {
    if (town_level == 0) {
        let noonbreeze_path = "../resources/noonbreeze.xp";
        let mut chain = BuilderChain::new(new_depth, width, height, "The Town of Noonbreeze");
        chain.start_with(PrefabBuilder::rex_level(noonbreeze_path));
        chain.with(TownBuilder::new());
        chain
    } else {
        let noonbreeze_path = "../resources/noonbreeze_1.xp";
        let mut chain = BuilderChain::new(new_depth, width, height, "The Town of Noonbreeze");
        chain.start_with(PrefabBuilder::rex_level(noonbreeze_path));
        chain.with(TowerBuilder::new());
        chain
    }
}


pub struct TowerBuilder {}

impl MetaMapBuilder for TowerBuilder {
    #[allow(dead_code)]
    fn build_map(&mut self, build_data : &mut BuilderMap) {
        
    }
}

impl TowerBuilder {
    pub fn new() -> Box<TowerBuilder> {
        Box::new(TowerBuilder{})
    }

    fn add_doors(&mut self, build_data : &mut BuilderMap) {
        for idx in 1 .. build_data.map.tiles.len() {
            if build_data.map.tiles[idx] == TileType::DoorSpot {
                build_data.map.tiles[idx] = TileType::Floor;
                build_data.spawn_list.push((idx, "Door".to_string()));
            }
        }
    }
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
        
        
        // Tailor
        build_data.spawn_list.push((build_data.map.xy_idx(23, 15), "Hide Rack".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(22, 17), "Loom".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(22, 18), "Countertop".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(21, 18), "Stool".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(22, 15), "Cabinet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(21, 15), "Cabinet".to_string()));
        
        
        // Alchemist
        build_data.spawn_list.push((build_data.map.xy_idx(54, 15), "Bed".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(56, 15), "Cabinet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(56, 17), "Potted Plant".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(55, 20), "Chemistry Set".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(55, 20), "Stool".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(55, 21), "Countertop".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(56, 21), "Countertop".to_string()));
        
        // Waves
        build_data.spawn_list.push((build_data.map.xy_idx(1, 2), "Wave".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(3, 7), "Wave".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(2, 12), "Wave".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(1, 17), "Wave".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(3, 22), "Wave".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(2, 27), "Wave".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(1, 32), "Wave".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(3, 37), "Wave".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(2, 42), "Wave".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(3, 52), "Wave".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(2, 57), "Wave".to_string()));
        
        // Doctor
        build_data.spawn_list.push((build_data.map.xy_idx(51, 19), "Potted Plant".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(52, 19), "Bed".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(52, 22), "Cabinet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(47, 20), "Countertop".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(47, 21), "Countertop".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(48, 20), "Stool".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(48, 21), "Countertop".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(49, 21), "Countertop".to_string()));
    
        // Adventure Guild
        build_data.spawn_list.push((build_data.map.xy_idx(40, 12), "Quest Board".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(38, 12), "Stool".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(39, 12), "Countertop".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(38, 13), "Countertop".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(39, 13), "Countertop".to_string()));
        
        
        // Magic Guild
        build_data.spawn_list.push((build_data.map.xy_idx(23, 4), "Bookshelf".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(21, 6), "Bookshelf".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(23, 6), "Stack of BooksR".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(22, 5), "Stack of BooksB".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(23, 5), "Stool".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(24, 8), "Quest Board".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(21, 8), "Bookshelf".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(27, 8), "Teleporter CrystalU".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(23, 10), "Bookshelf".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(25, 10), "Bookshelf".to_string()));
        
        
        
        // Flowers about town
        build_data.spawn_list.push((build_data.map.xy_idx(40, 28), "Rose".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(43, 32), "Rose".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(45, 26), "Rose".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(52, 24), "Rose".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(58, 28), "Rose".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(57, 40), "Rose".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(51, 42), "Rose".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(35, 47), "Rose".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(34, 52), "Rose".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(35, 56), "Rose".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(41, 58), "Rose".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(44, 54), "Rose".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(42, 47), "Rose".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(31, 32), "Rose".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(27, 32), "Rose".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(29, 34), "Rose".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(25, 35), "Rose".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(26, 41), "Rose".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(39, 35), "Rose".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(19, 21), "Rose".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(19, 24), "Rose".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(20, 34), "Rose".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(26, 57), "Rose".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(28, 50), "Rose".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(25, 46), "Rose".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(20, 3), "Rose".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(26, 2), "Rose".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(29, 5), "Rose".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(32, 9), "Rose".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(27, 14), "Rose".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(22, 12), "Rose".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(58, 4), "Rose".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(54, 2), "Rose".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(49, 2), "Rose".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(44, 3), "Rose".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(42, 5), "Rose".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(39, 3), "Rose".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(43, 12), "Rose".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(34, 19), "Rose".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(42, 17), "Rose".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(40, 33), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(39, 29), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(42, 25), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(43, 20), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(45, 14), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(42, 12), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(51, 51), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(50, 46), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(53, 40), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(56, 36), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(49, 36), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(45, 35), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(42, 40), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(44, 48), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(42, 51), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(42, 56), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(46, 58), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(52, 58), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(26, 44), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(28, 53), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(30, 52), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(35, 50), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(34, 46), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(19, 31), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(21, 35), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(23, 34), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(25, 31), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(28, 32), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(31, 34), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(28, 11), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(28, 18), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(25, 15), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(21, 11), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(19, 22), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(19, 18), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(45, 5), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(43, 2), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(41, 3), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(38, 5), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(34, 3), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(28, 3), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(54, 13), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(58, 13), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(58, 8), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(58, 3), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(56, 2), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(51, 2), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(48, 26), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(56, 24), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(58, 22), "Violet".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(52, 36), "Daisy".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(51, 35), "Daisy".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(48, 35), "Daisy".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(43, 34), "Daisy".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(39, 34), "Daisy".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(41, 30), "Daisy".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(42, 58), "Daisy".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(44, 57), "Daisy".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(41, 52), "Daisy".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(42, 50), "Daisy".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(44, 46), "Daisy".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(44, 41), "Daisy".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(32, 40), "Daisy".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(28, 42), "Daisy".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(31, 46), "Daisy".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(35, 49), "Daisy".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(35, 54), "Daisy".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(32, 58), "Daisy".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(28, 33), "Daisy".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(25, 32), "Daisy".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(19, 33), "Daisy".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(20, 52), "Daisy".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(19, 43), "Daisy".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(21, 40), "Daisy".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(30, 10), "Daisy".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(29, 7), "Daisy".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(27, 3), "Daisy".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(21, 2), "Daisy".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(19, 7), "Daisy".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(20, 13), "Daisy".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(45, 4), "Daisy".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(43, 4), "Daisy".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(42, 2), "Daisy".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(39, 4), "Daisy".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(36, 3), "Daisy".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(35, 5), "Daisy".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(53, 24), "Daisy".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(58, 23), "Daisy".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(58, 16), "Daisy".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(58, 10), "Daisy".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(53, 2), "Daisy".to_string()));
        build_data.spawn_list.push((build_data.map.xy_idx(50, 2), "Daisy".to_string()));
        
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
