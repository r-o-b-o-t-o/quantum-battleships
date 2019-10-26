# Quantum Battleships Expanded Edition

This project is an attempt at improving *Quantum Battleships with partial NOT gates* ([code](https://github.com/Qiskit/qiskit-community-tutorials/blob/master/games/battleships_with_partial_NOT_gates.ipynb) and [article](https://medium.com/qiskit/how-to-program-a-quantum-computer-982a9329ed02)) and integrating it into Unity.



## Building the engine

The engine is programmed in the Rust programming language using the [*q1tsim* quantum computer simulator crate](https://crates.io/crates/q1tsim).

To build the engine in order to use the project, install [Rust](https://www.rust-lang.org/tools/install), then run the following commands:
```bash
$ cd path/to/repository/battleships_engine
$ cargo build --release # building in release mode will yield better performances
$ cp target/release/battleships_engine ../Battleships # for Unix and macOS users
$ cp target/release/battleships_engine.exe ../Battleships # for Windows users
```

You can now open the `Battleships` directory in Unity and play the game from the editor.  
Don't forget to copy the engine binary next to the game's executable if you build the Unity project.  



## Assets used in this project

### Models

* **Ships** : *Kenney Pirate Kit* (published under the CC0 1.0 Universal license) : https://www.kenney.nl/assets/pirate-kit
* **Ships** : *Ships by Quaternius* (published under the CC0 1.0 Universal license) : http://quaternius.com/assets.html / https://drive.google.com/drive/folders/1Qf31QTnGfxRzYxx8dHmlVGDT4KmIa0Vy
* **Low-poly Bomb model** by *Google* (published under the CC-BY license) : https://poly.google.com/view/3_eYb_Ooax_

### Unity assets
* **Low-poly water** : https://assetstore.unity.com/packages/tools/particles-effects/lowpoly-water-107563

### Audio
* **Bomb sound** (published under the CC BY 3.0 license) : https://freesound.org/s/155235/
* **Error sound** (published under the CC BY 3.0 license) : https://freesound.org/s/417794/
* **Splash sounds** (published under the CC BY 3.0 license) : https://freesound.org/s/98335/
