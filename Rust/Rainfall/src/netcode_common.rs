use serde::{Deserialize, Serialize};

#[derive(Serialize, Deserialize)]
pub enum FromClientMsg {
    Ping,
}

#[derive(Serialize, Deserialize)]
pub enum FromServerMsg {
    Pong(usize),
    UnknownPong,
}
