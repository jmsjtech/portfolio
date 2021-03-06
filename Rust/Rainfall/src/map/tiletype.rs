use serde::{Serialize, Deserialize};

#[derive(PartialEq, Eq, Hash, Copy, Clone, Serialize, Deserialize)]
pub enum TileType {
    Wall,
    Stalactite,
    Stalagmite,
    Floor,
    DownStairs,
    Road,
    Grass,
    ShallowWater,
    DeepWater,
    WoodFloor,
    Bridge,
    Gravel,
    UpStairs,
    Sand,
    BuildingWall,
    DoorSpot,
    Glass
}

pub fn tile_walkable(tt : TileType) -> bool {
    match tt {
        TileType::Floor | TileType::DownStairs | TileType::Road | TileType::Grass |
        TileType::ShallowWater | TileType::WoodFloor | TileType::Bridge | TileType::Gravel |
        TileType::UpStairs | TileType::Sand
            => true,
        _ => false
    }
}

pub fn tile_opaque(tt : TileType) -> bool {
    match tt {
        TileType::Wall | TileType::Stalactite | TileType::Stalagmite | TileType::BuildingWall => true,
        _ => false
    }
}

pub fn tile_cost(tt : TileType) -> f32 {
    match tt {
        TileType::Road => 0.8,
        TileType::Grass => 1.1,
        TileType::ShallowWater => 1.2,
        _ => 1.0
    }
}
