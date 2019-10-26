use crate::grid::Grid;
use crate::coords::Coords;
use crate::ship::{ self, Ship };

#[derive(Default, Serialize, Deserialize, Debug)]
pub struct Player {
    ships: Vec<Ship>,
    bombs: Vec<Coords>,
    grid: Grid,
}

impl Player {
    pub fn get_block_index_at_pos(&self, pos: &Coords) -> i32 {
        let mut idx = 0;

        for ship in self.ships.iter() {
            for block in ship.blocks.iter() {
                if block.pos == *pos {
                    return idx;
                }
                idx += 1;
            }
        }

        -1
    }

    pub fn add_ship(&mut self, ship: Ship) {
        for block in ship.blocks.iter() {
            self.grid.at_pos_mut(&block.pos).ship = true;
        }
        self.ships.push(ship);
    }

    pub fn add_bomb(&mut self, bomb: Coords) {
        self.bombs.push(bomb);
    }

    pub fn ships(&self) -> &Vec<Ship> {
        &self.ships
    }

    pub fn bombs(&self) -> &Vec<Coords> {
        &self.bombs
    }

    pub fn grid(&self) -> &Grid {
        &self.grid
    }

    fn get_block(&mut self, idx: i32) -> Option<&mut ship::Block> {
        let mut i = 0;
        for ship in self.ships.iter_mut() {
            for block in ship.blocks.iter_mut() {
                if i == idx {
                    return Some(block);
                }
                i += 1;
            }
        }

        None
    }

    pub fn get_ship_at_pos(&self, pos: &Coords) -> Option<&Ship> {
        for ship in self.ships.iter() {
            for block in ship.blocks.iter() {
                if block.pos == *pos {
                    return Some(ship);
                }
            }
        }

        None
    }

    pub fn reset_damage(&mut self) {
        for ship in self.ships.iter_mut() {
            for block in ship.blocks.iter_mut() {
                block.damage = 0;
            }
        }
    }

    pub fn add_damage(&mut self, idx: i32, dmg: i32) {
        let (pos, total_damage) = match self.get_block(idx) {
            Some(block) => {
                block.damage += dmg;
                (block.pos, block.damage)
            },
            None => return,
        };
        self.grid.at_mut(pos.x, pos.y).damage = total_damage;
    }
}
