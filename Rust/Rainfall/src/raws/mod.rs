mod item_structs;
use item_structs::*;
mod mob_structs;
use mob_structs::*;
mod prop_structs;
use prop_structs::*;
mod spawn_table_structs;
use spawn_table_structs::*;
mod loot_structs;
use loot_structs::*;
mod faction_structs;
pub use faction_structs::*;
mod spell_structs;
pub use spell_structs::Spell;
mod weapon_traits;
pub use weapon_traits::*;

mod rawmaster;
pub use rawmaster::*;
use serde::Deserialize;
use std::sync::Mutex;
use std::fs;


lazy_static! {
    pub static ref RAWS : Mutex<RawMaster> = Mutex::new(RawMaster::empty());
}

#[derive(Deserialize, Debug)]
pub struct Raws {
    pub items : Vec<Item>,
    pub mobs : Vec<Mob>,
    pub props : Vec<Prop>,
    pub spawn_table : Vec<SpawnTableEntry>,
    pub loot_tables : Vec<LootTable>,
    pub faction_table : Vec<FactionInfo>,
    pub spells : Vec<Spell>,
    pub weapon_traits : Vec<WeaponTrait>
}

pub fn load_raws() {    
    let faction_paths = fs::read_dir("./raws/factions/").unwrap();
    let item_paths = fs::read_dir("./raws/items/").unwrap();
    let loot_table_paths = fs::read_dir("./raws/loot_tables/").unwrap();
    let mob_paths = fs::read_dir("./raws/mobs/").unwrap();
    let prop_paths = fs::read_dir("./raws/props/").unwrap();
    let spawn_table_paths = fs::read_dir("./raws/spawn_tables/").unwrap();
    let spell_paths = fs::read_dir("./raws/spells/").unwrap();
    let weapon_paths = fs::read_dir("./raws/weapon_traits/").unwrap();
    
    let mut all_raws = "{".to_owned();
    
    
    // Load all the files in /raws/items/
    let mut first_item_path = true;
    for path in item_paths { 
        let raw_data = fs::read_to_string(path.unwrap().path().display().to_string()).expect("Something went wrong reading the file"); 
        if first_item_path { 
            first_item_path = false;
            all_raws.push_str(" \"items\": [");
            all_raws.push_str(raw_data.as_str());
        } else {
            all_raws.push_str(",");
            all_raws.push_str(raw_data.as_str());
        }
    }
    all_raws.push_str("],");
    
    
    // Load all the files in /raws/factions/
    let mut first_faction_path = true;
    for path in faction_paths { 
        let raw_data = fs::read_to_string(path.unwrap().path().display().to_string()).expect("Something went wrong reading the file"); 
        if first_faction_path { 
            first_faction_path = false;
            all_raws.push_str(" \"faction_table\": [");
            all_raws.push_str(raw_data.as_str());
        } else {
            all_raws.push_str(",");
            all_raws.push_str(raw_data.as_str());
        }
    }
    all_raws.push_str("],");
    
    // Load all the files in /raws/loot_tables/
    let mut first_loot_path = true;
    for path in loot_table_paths { 
        let raw_data = fs::read_to_string(path.unwrap().path().display().to_string()).expect("Something went wrong reading the file"); 
        if first_loot_path { 
            first_loot_path = false;
            all_raws.push_str(" \"loot_tables\": [");
            all_raws.push_str(raw_data.as_str());
        } else {
            all_raws.push_str(",");
            all_raws.push_str(raw_data.as_str());
        }
    }
    all_raws.push_str("],");
    
    // Load all the files in /raws/spawn_tables/
    let mut first_spawn_path = true;
    for path in spawn_table_paths { 
        let raw_data = fs::read_to_string(path.unwrap().path().display().to_string()).expect("Something went wrong reading the file"); 
        if first_spawn_path { 
            first_spawn_path = false;
            all_raws.push_str(" \"spawn_table\": [");
            all_raws.push_str(raw_data.as_str());
        } else {
            all_raws.push_str(",");
            all_raws.push_str(raw_data.as_str());
        }
    }
    all_raws.push_str("],");
    
    
    // Load all the files in /raws/weapon_traits/
    let mut first_weapon_path = true;
    for path in weapon_paths { 
        let raw_data = fs::read_to_string(path.unwrap().path().display().to_string()).expect("Something went wrong reading the file"); 
        if first_weapon_path { 
            first_weapon_path = false;
            all_raws.push_str(" \"weapon_traits\": [");
            all_raws.push_str(raw_data.as_str());
        } else {
            all_raws.push_str(",");
            all_raws.push_str(raw_data.as_str());
        }
    }
    all_raws.push_str("],");
    
    
    // Load all the files in /raws/spells/
    let mut first_spell_path = true;
    for path in spell_paths { 
        let raw_data = fs::read_to_string(path.unwrap().path().display().to_string()).expect("Something went wrong reading the file"); 
        if first_spell_path { 
            first_spell_path = false;
            all_raws.push_str(" \"spells\": [");
            all_raws.push_str(raw_data.as_str());
        } else {
            all_raws.push_str(",");
            all_raws.push_str(raw_data.as_str());
        }
    }
    all_raws.push_str("],");
    
    
    // Load all the files in /raws/mobs/
    let mut first_mob_path = true;
    for path in mob_paths { 
        let raw_data = fs::read_to_string(path.unwrap().path().display().to_string()).expect("Something went wrong reading the file"); 
        if first_mob_path { 
            first_mob_path = false;
            all_raws.push_str(" \"mobs\": [");
            all_raws.push_str(raw_data.as_str());
        } else {
            all_raws.push_str(",");
            all_raws.push_str(raw_data.as_str());
        }
    }
    all_raws.push_str("],");
    
    
    // Load all the files in /raws/props/
    let mut first_prop_path = true;
    for path in prop_paths { 
        let raw_data = fs::read_to_string(path.unwrap().path().display().to_string()).expect("Something went wrong reading the file"); 
        if first_prop_path { 
            first_prop_path = false;
            all_raws.push_str(" \"props\": [");
            all_raws.push_str(raw_data.as_str());
        } else {
            all_raws.push_str(",");
            all_raws.push_str(raw_data.as_str());
        }
    }
    all_raws.push_str("]}");
    
    
    
    let decoder : Raws = serde_json::from_str(&all_raws).expect("Unable to parse JSON");

    RAWS.lock().unwrap().load(decoder);
}
