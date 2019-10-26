#[macro_use]
extern crate serde_derive;

use q1tsim::circuit::Circuit;
use std::io::{ Error, ErrorKind };

pub mod grid;
pub mod ship;
pub mod player;
pub mod coords;

use player::Player;

macro_rules! arr {
    ( $( $n:expr )? ) => {
        {
            $(
                let mut array = [ 0; $n ];
                for i in 0..$n {
                    array[i] = i;
                }
                array
            )*
        }
    };
}

#[derive(Default, Serialize, Deserialize, Debug)]
struct GameState {
    pub players: [ Player; 2 ],
    pub winner: i32,
}

#[derive(Deserialize, Debug)]
struct Query {
    pub shots: usize,
    #[serde(alias = "gameState")]
    pub game_state: GameState,
}

fn read_line() -> std::io::Result<String> {
    let mut s = String::new();
    std::io::stdin().read_line(&mut s)?;
    Ok(s.trim().into())
}

fn game_loop(query: &mut Query) -> Result<(), q1tsim::error::Error> {
    let game_state = &mut query.game_state;

    const QBITS: usize = 2 + 3 + 3 + 4 + 5;
    const CBITS: usize = QBITS;
    let mut circuits = [ Circuit::new(QBITS, CBITS), Circuit::new(QBITS, CBITS) ];

    for player in 0..2 {
        let circuit = &mut circuits[player];
        circuit.reset_all();

        for bomb in game_state.players[(player + 1) % 2].bombs().iter() {
            // Add a gate in the current player's circuit for each bomb the other player placed
            match game_state.players[player].get_ship_at_pos(bomb) {
                Some(ship) => {
                    let bit = game_state.players[player].get_block_index_at_pos(bomb);
                    let frac = 1.0_f64 / (ship.health as f64);
                    circuit.u3(frac * std::f64::consts::PI, 0.0, 0.0, bit as usize)?;
                },
                None => {},
            }
        }

        circuit.measure_all(&arr![QBITS])?;
        circuit.execute(query.shots)?;
        let result = circuit.histogram()?;

        game_state.players[player].reset_damage();
        for (key, val) in result {
            for power in find_powers_of_two(key as i32) {
                let idx = which_power_of_two(power);
                let dmg = ((val as f32 / query.shots as f32) * 100.0).round() as i32;
                game_state.players[player].add_damage(idx, dmg);
            }
        }
    }

    for (player_idx, player) in game_state.players.iter().enumerate() {
        if player.ships().iter().all(|ship| ship.is_sinked()) {
            game_state.winner = ((player_idx as i32 + 1) % 2) + 1
        }
    }

    Ok(())
}

fn main() -> Result<(), Box<dyn std::error::Error>> {
    loop {
        let command = read_line()?;
        if command == "blankGameState" {
            let game_state = GameState::default();
            println!("{}", serde_json::to_string(&game_state)?);
        } else if command == "updateGameState" {
            let query_string = read_line()?;
            let mut q = serde_json::from_str(&query_string)?;
            game_loop(&mut q).map_err(|e| Error::new(ErrorKind::Other, e.to_string()))?;
            println!("{}", serde_json::to_string(&q.game_state)?);
        } else if command == "quit" {
            break;
        }
    }

    Ok(())
}

fn find_powers_of_two(x: i32) -> Vec<i32> {
    let mut powers = Vec::new();
    let mut i = 1;

    while i <= x {
        if i & x > 0 {
            powers.push(i);
        }
        i <<= 1;
    }

    powers
}

fn which_power_of_two(n: i32) -> i32 {
    (n as f32).log2() as i32
}
