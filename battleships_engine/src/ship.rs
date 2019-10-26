use crate::coords::Coords;

#[derive(Serialize, Deserialize, Debug)]
pub struct Ship {
    pub blocks: Vec<Block>,
    pub health: i32,
}

impl Ship {
    pub fn new(health: i32) -> Self {
        Self {
            blocks: Vec::new(),
            health,
        }
    }

    pub fn add_block(&mut self, x: i32, y: i32) {
        self.blocks.push(Block::new(x, y));
    }

    pub fn is_sinked(&self) -> bool {
        self.blocks.iter().all(|block| block.damage > 95)
    }
}

#[derive(Default, Serialize, Deserialize, Debug)]
pub struct Block {
    pub pos: Coords,
    pub damage: i32,
}

impl Block {
    pub fn new(x: i32, y: i32) -> Self {
        Self {
            pos: Coords::new(x, y),
            damage: 0,
        }
    }
}

impl From<Coords> for Block {
    fn from(pos: Coords) -> Self {
        Self::new(pos.x, pos.y)
    }
}
