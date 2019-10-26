use crate::coords::Coords;

#[derive(Default, Serialize, Deserialize, Debug)]
pub struct Cell {
    pub pos: Coords,
    pub ship: bool,
    pub damage: i32,
}

#[derive(Serialize, Deserialize, Debug)]
pub struct Grid {
    pub cells: Vec<Cell>,
}

impl Default for Grid {
    fn default() -> Self {
        let mut grid = Self {
            cells: Vec::new(),
        };

        for y in 0..10 {
            for x in 0..10 {
                grid.cells.push(Cell {
                    pos: Coords::new(x, y),
                    ship: false,
                    damage: 0,
                });
            }
        }

        grid
    }
}

impl Grid {
    pub fn at(&self, x: i32, y: i32) -> &Cell {
        &self.cells[(y * 10 + x) as usize]
    }

    pub fn at_pos(&self, pos: &Coords) -> &Cell {
        self.at(pos.x, pos.y)
    }

    pub fn at_mut(&mut self, x: i32, y: i32) -> &mut Cell {
        &mut self.cells[(y * 10 + x) as usize]
    }

    pub fn at_pos_mut(&mut self, pos: &Coords) -> &mut Cell {
        self.at_mut(pos.x, pos.y)
    }
}
