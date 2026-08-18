# Caravan Secrets: Puzzle Journey

Unity 6000.3.21f1 project for a portrait, offline-first Android directional puzzle game with Arabic and English support.

## Current Milestone

- Stage 2 Core Puzzle Prototype accepted on Android.
- Stage 3 Level Data and Editor accepted in the Unity Editor.
- Stage 4 Gates A–D accepted; Gate E Android/device acceptance is next.
- Pure C# board rules, validator, and breadth-first solver.
- ScriptableObject levels with region, number, recommended moves, rewards, cells, stable cart IDs, directions, and destination links.
- Visual `Caravan Level Editor` with create/edit/save/duplicate/validate/solve/play-test workflows.
- Arabic/English Unity Localization gameplay tables.
- Edit Mode and Play Mode automated coverage.

## Open and Run

1. Activate a Unity Personal license in Unity Hub.
2. Open this folder with Unity 6000.3.21f1.
3. Open `Assets/Scenes/Bootstrap/Bootstrap.unity` and enter Play Mode, or build Android through the existing project setup command.

## Create or Edit a Level

1. Open `Caravan Secrets > Levels > Caravan Level Editor`.
2. Select an existing `LevelAsset`, or choose **New**.
3. Set the level/region IDs, level number, dimensions, recommended moves, and reward coins.
4. Choose a grid tool:
   - **Paint Cell** for exits, rocks, switches, and storage supported by the current domain.
   - **Place Cart** for a stable cart ID and direction.
   - **Remove** to clear an item safely.
   - **Link Destination** to bind a cart ID to an exit.
5. Use **Validate**, then **Solve**. Save only when validation is clear.
6. Use **Play-test** to open the Gameplay scene with the draft in Editor Play Mode.

Coordinates shown by the editor are `(column, row)`. Arabic/RTL presentation must never mirror board coordinates or gameplay directions.

## Validate Levels

- Menu: `Caravan Secrets > Levels > Validate All`.
- Command line execute method: `CaravanSecrets.Editor.LevelTools.ValidateAll`.
- Android development builds call the same validation gate before building.

Validation checks metadata, IDs, bounds, overlaps, exits, destination links, duplicate regional level numbers, supported cell types, and solver results. Error messages identify the affected level and rule.

The production campaign contains 30 ordered assets: ten each for `desert`, `oasis`, and `city`. Confirmed minimum solutions are recorded in `../Docs/LEVEL_SOLUTIONS.md`. Use `Caravan Secrets > Levels > Generate Stage 4 Production Levels` only when intentionally regenerating levels 6–30; it does not modify frozen levels 1–5.

## Solver

The solver uses breadth-first state-space exploration. It:

- avoids duplicate logical states;
- returns a confirmed minimum move sequence for supported mechanics;
- supports visited-state and depth limits;
- reports `Solved`, `Unsolvable`, `LimitReached`, `Unsupported`, or `Invalid` explicitly.

The solver is also used by the Compass booster to suggest one valid next move from the current logical state. It never performs the move for the player.

## Add a Booster

Implement `IBooster` in `Assets/Scripts/Game/Boosters`. Keep eligibility and effects independent from presentation and inventory. Return a `BoosterResult`, record usage only after successful use, and add tests for undo/restart boundaries.

Current shared boosters:

- `CompassBooster`: returns the first move of a confirmed solution from the current state without changing the board.
- `RopeBooster`: temporarily removes one eligible static rock; gates and arbitrary objects are not eligible.
- `ExtraSpaceBooster`: adds one temporary capacity slot to one typed storage position.

Temporary board effects participate in undo and are cleared by restart. Successful booster usage remains counted through undo for the current attempt, then resets on restart. This prevents undo from restoring the no-booster star.

## Star Results

`LevelResultCalculator` produces reward-independent result data. A completed level receives one completion star, one efficiency star when moves are at or below a positive recommended threshold, and one no-booster star. Incomplete attempts return zero stars.

## Known Limitations

- Gate C provides scene-independent booster and star domain behavior. Player-facing booster controls and completion-star presentation remain for Gate E integration and acceptance work.
- Levels 1–30 are validated production content; levels 1–5 remain frozen byte-for-byte.
- The current solver favors correctness and clarity over large-board performance; advanced mechanics may require specialized state encoding/heuristics later.
- Journey map, camp, economy, story, production audio, ads, analytics, and release systems remain later stages.
