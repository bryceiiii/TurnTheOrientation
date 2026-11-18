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

- `Assets/Scenes/SampleScene.unity`: persistent bootstrap scene and `LevelManager`
- `Assets/Scenes/Level_01.unity`: First Turn
- `Assets/Scenes/Level_02.unity`: Two Signals
- `Assets/Scenes/Level_03.unity`: Cross Current
- `Assets/Prefabs/Common`: shared player, wall, floor, crate, socket, gate, exit, and prop prefabs
- `Assets/Prefabs/Systems`: editable HUD, audio, game-state, and runtime-effect prefabs
- `Assets/Scripts/LevelManager.cs`: scene order, next-level loading, and current-level reload
- `Assets/Scripts/LevelSceneController.cs`: per-scene prefab references and starting layout
- `Assets/Scripts/TurnGame.cs`: room state, rotation, gravity, plates, and progression
- `Assets/Scripts/TurnPlayer.cs`: top-down and side-view movement
- `Assets/Scripts/GameHud.cs`: title, HUD, pause, room-clear, and final screens
- `Assets/Tests/EditMode`: rules, Build Settings, and prefab migration tests

Level layouts and system objects are authored as prefab instances and can be
adjusted directly in the Unity editor. The only runtime instances created from
prefabs are short-lived effect particles. Gameplay audio clips are synthesized
at runtime without creating scene objects.

## Open In Unity

Open `Assets/Scenes/SampleScene.unity` in Unity `6000.3.27f2` and press Play.
`LevelManager` on `Turn The Orientation` loads `Level_01` and keeps the camera,
HUD, audio, and game state alive across level scene changes.
