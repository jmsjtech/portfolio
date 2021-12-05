use super::netcode_common::{FromClientMsg, FromServerMsg};

use message_io::network::{Endpoint, NetEvent, Transport};
use message_io::node::{self};
use serde::{Deserialize, Serialize};

use std::collections::HashMap;
use std::net::SocketAddr;

struct ClientInfo {
    count: usize,
}

pub fn run(transport: Transport, addr: SocketAddr) {
    let (handler, listener) = node::split::<()>();
    
    let mut clients: HashMap<Endpoint, ClientInfo> = HashMap::new();
    
    match handler.network().listen(transport, addr) {
        Ok((_id, real_addr)) => println!("Server hosted"),
        Err(_) => crate::gamelog::Logger::new()
            .color(rltk::SPRINGGREEN)
            .append("Server couldn't be hosted.")
            .log(),
    }

    listener.for_each(move |event| match event.network() {
        NetEvent::Connected(_, _) => (),
        NetEvent::Accepted(endpoint, _listener_id) => {
            clients.insert(endpoint, ClientInfo { count: 0 });
            crate::gamelog::Logger::new()
                .color(rltk::SPRINGGREEN)
                .append("Client connected!")
                .log()
        }
        NetEvent::Message(endpoint, input_data) => {
            let message: FromClientMsg = bincode::deserialize(&input_data).unwrap();
            match message {
                FromClientMsg::Ping => {
                    let message = match clients.get_mut(&endpoint) {
                        Some(client) => {
                            client.count += 1;
                            FromServerMsg::Pong(client.count)
                        }
                        None => {
                            FromServerMsg::UnknownPong
                        }
                    };
                    let output_data = bincode::serialize(&message).unwrap();
                    handler.network().send(endpoint, &output_data);
                }
            }
        }
        NetEvent::Disconnected(endpoint) => {
            clients.remove(&endpoint).unwrap();
            crate::gamelog::Logger::new()
                .color(rltk::SPRINGGREEN)
                .append("Client disconnected!")
                .log();
        }
    });
}