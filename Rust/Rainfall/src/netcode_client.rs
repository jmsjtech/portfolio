use super::netcode_common::{FromServerMsg, FromClientMsg};

use message_io::network::{NetEvent, Transport, RemoteAddr};
use message_io::node::{self, NodeEvent};
use serde::{Deserialize, Serialize};

use std::time::{Duration};

enum Signal {
    Greet,
}

pub fn run(transport: Transport, remote_addr: RemoteAddr) {
    let (handler, listener) = node::split();

    let (server_id, local_addr) =
        handler.network().connect(transport, remote_addr.clone()).unwrap();

    listener.for_each(move |event| match event {
        NodeEvent::Network(net_event) => match net_event {
            NetEvent::Connected(_, established) => {
                if established {
                    crate::gamelog::Logger::new()
                        .color(rltk::SPRINGGREEN)
                        .append("Connected to server!")
                        .log()
                } else {
                    crate::gamelog::Logger::new()
                        .color(rltk::SPRINGGREEN)
                        .append("Couldn't connect to server.")
                        .log()
                }
            }
            NetEvent::Accepted(_, _) => unreachable!(),
            NetEvent::Message(_, input_data) => {
                let message: FromServerMsg = bincode::deserialize(&input_data).unwrap();
                match message {
                    FromServerMsg::Pong(count) => {
                        crate::gamelog::Logger::new()
                            .color(rltk::SPRINGGREEN)
                            .append("Received message from connected server!")
                            .log()
                    }
                    FromServerMsg::UnknownPong => crate::gamelog::Logger::new()
                        .color(rltk::SPRINGGREEN)
                        .append("Received message from unknown server!")
                        .log(),
                }
            }
            NetEvent::Disconnected(_) => {
                crate::gamelog::Logger::new()
                    .color(rltk::SPRINGGREEN)
                    .append("Disconnected from server.")
                    .log();
                handler.stop();
            }
        },
        NodeEvent::Signal(signal) => match signal {
            Signal::Greet => {
                let message = FromClientMsg::Ping;
                let output_data = bincode::serialize(&message).unwrap();
                handler.network().send(server_id, &output_data);
                handler.signals().send_with_timer(Signal::Greet, Duration::from_secs(1));
            }
        },
    });
}