# Turn / Floor

A three-room Unity 6 prototype that combines top-down crate pushing with a
gravity-based side-view platformer. The room rotates in 90-degree steps around
the same 2D physics layout, turning vertical walls into platforms.

## Controls

- `WASD` / arrow keys: top-down movement
- `A` / `D`: side-view running
- `Space` / `W` / up arrow: side-view jump
- `B`: rotate the room 90 degrees clockwise
- `N`: rotate the room 90 degrees counterclockwise
- `R`: restart the current room
- `Esc`: pause or resume
- `Enter`: start, advance, or replay after a clear

## Game Loop

1. Push every glowing core onto its socket in Top mode.
2. The socket latches and opens the signal gate.
3. Press `B` or `N`; the room rotates 90 degrees and gravity switches on.
4. Use the transformed walls as platforms to reach the exit signal.
5. Falling costs one of three sparks.

## Project Structure

- `Assets/Scripts/TurnGame.cs`: room state, level loading, rotation, and progression
- `Assets/Scripts/TurnPlayer.cs`: top-down and side-view movement
- `Assets/Scripts/LevelCatalog.cs`: data for the three rooms
- `Assets/Scripts/SpriteFactory.cs`: original procedural 2D art
- `Assets/Scripts/GameHud.cs`: title, HUD, pause, room-clear, and final screens
- `Assets/Tests/EditMode`: deterministic rules and level-data tests

All gameplay art and audio are generated at runtime; no third-party art or
audio assets are included.

## Open In Unity

Open `Assets/Scenes/SampleScene.unity` in Unity `6000.3.10f1` and press Play.
The scene contains one `GameBootstrap` object that builds the complete demo.
