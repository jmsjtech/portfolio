use specs::prelude::*;
use specs_derive::*;
use serde::{Serialize, Deserialize};
use specs::saveload::{Marker, ConvertSaveload};
use specs::error::NoError;

#[derive(Component, Debug, ConvertSaveload)]
pub struct TimeKeeper {
    pub last_second: u128,
    pub last_10sec: u128,
    pub last_minute: u128,
    pub last_10min: u128,
    pub last_hour: u128,

    pub min: i32,
    pub hour: i32,
    pub day: i32,
    pub season: i32,
    pub year: i32
}

pub fn new_time() -> TimeKeeper {
    let clock = TimeKeeper {
        last_second: 0,
        last_10sec: 0,
        last_minute: 0,
        last_10min: 0,
        last_hour: 0,

        //Friendly Time Text
        min: 0,
        hour: 12,
        day: 1,
        season: 1,
        year: 1
    };

    clock
}
