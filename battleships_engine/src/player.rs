use crate::ship::Ship;
use crate::coords::Coords;

#[derive(Default, Serialize, Deserialize, Debug)]
pub struct Player {
    pub ships: Vec<Ship>,
    pub bombs: Vec<Coords>,
}

impl Player {
    pub fn add_ship(&mut self, ship: Ship) {
        self.ships.push(ship);
    }

    pub fn add_bomb(&mut self, bomb: Coords) {
        self.bombs.push(bomb);
    }

    pub fn reset_damage(&mut self) {
        for ship in self.ships.iter_mut() {
            for block in ship.blocks.iter_mut() {
                block.damage = 0;
            }
        }
    }

    pub fn add_damage(&mut self, ship_idx: usize, block_idx: usize, dmg: i32) {
        let block = &mut self.ships[ship_idx].blocks[block_idx];
        block.damage += dmg;
    }
}
