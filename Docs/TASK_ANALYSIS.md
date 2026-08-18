# Task Analysis: Phase 2 — Long Journey Road and Camera Foundation

## Game Vision and Current Stage

Caravan Secrets remains a bilingual portrait directional caravan puzzle. Stage 4 puzzle systems are accepted. Commercial Vertical Slice Phase 1 established a green baseline and Git protection; Phase 2 now adds only the journey/road/camera foundation around one representative puzzle.

## Exact Task

Build an independent Journey state model and one integrated Desert Road segment demonstrating start checkpoint, continuous road, puzzle stop, completion-triggered caravan travel, and arrival at the next checkpoint.

## Relevant Specification Sections

- Core Game Concept; Gameplay Structure Rule; Primary Puzzle Mechanic.
- Commercial Vertical Slice Extension.
- Long Desert Road Requirement.
- Architecture; Camera and Resolution; Mobile Input; Performance and Android; Testing.

## Existing Systems to Preserve

- `BoardGame`, `BoardState`, movement/collision, snapshots, undo/restart, solver/validator.
- `LevelAsset` loading and all 30 validated level assets.
- `GameplayController` board scale (`CellSize = 1.25f`) and enlarged touch bounds.
- Bootstrap/save, localization/Arabic shaping, HUD, debug browser, packages, Android settings.
- Existing Desert Road background, road strip, cart, gate, rock, and switch assets.

## Planned Architecture

- `Game/Journey/JourneySession.cs`: pure checkpoint/phase state and transition validation; no Unity or puzzle dependency.
- `Features/Journey/RepresentativeJourneyPresenter.cs`: creates the continuous road and landmarks from existing prefabs and animates camera/caravan presentation.
- `GameplayController`: minimal orchestration hooks only—start representative arrival, block puzzle input during camera travel, and route completed level 1 through departure before loading level 2.
- Tests: pure transition tests plus Play Mode integration proving camera/puzzle separation and post-completion progression.

## Files Expected to Change

- New Journey domain and presentation source files with Unity `.meta` files.
- `GameplayController.cs` for isolated orchestration hooks.
- Game and Play Mode tests/assembly references as required.
- `Docs/CURRENT_TASK.md`, `Docs/TASK_ANALYSIS.md`, and `Docs/PROJECT_STATUS.md`.
- Generated ignored logs, APK, and screenshots.

## Files That Must Not Change

- All 30 `Assets/Resources/Levels/*.asset` files.
- `BoardGame.cs`, `BoardState.cs`, `LevelDefinition.cs`, movement/collision, solver, validator, boosters/results.
- Bootstrap/save/localization implementation and tables.
- Existing scenes, prefabs, raster art, package manifest, and project settings unless a reproducible blocker is documented.
- `Docs/Archive/`.

## Tests Planned

- JourneySession valid/invalid transition and checkpoint tests.
- Representative journey presenter structure and camera phase integration.
- Existing Edit Mode and Play Mode suites.
- 30-level validation and frozen hashes.
- Android development build and three device screenshots.

## Risks and Mitigations

- Camera coroutine could interfere with scene tests: expose deterministic phase state and keep non-representative navigation unchanged.
- Journey visuals could be destroyed by board rebuild: use a separate presentation root/component.
- Background coverage could reveal empty world during travel: tile existing background across checkpoint sections.
- Travel could accidentally mutate puzzle state: presenter receives no `BoardGame`; only controller observes completion and starts presentation.
- Touch usability could regress: retain CellSize, camera puzzle zoom, and existing hit expansion unchanged.

## Consistency Statement

The task exactly matches the user-authorized Phase 2 scope and the adopted specification. It explicitly excludes every prohibited system. No instruction conflict exists.

## Pre-Implementation Report

- Documents read completely: `AGENTS.md`, `PROJECT_SPEC.md`, `PROJECT_STATUS.md`, `CURRENT_TASK.md`.
- Stable baseline commit created before Phase 2: `a3587b94510936fe6e1740992223fb348d01f727`.
- Planned implementation files and tests are listed above.
- Explicit exclusions: survival, camp, ads, purchases, analytics, audio, characters, events, economy, new mechanics, all-level rollout, packages, and external assets.

## Completion Verification

- Created the planned pure journey state model and isolated presentation assembly.
- Integrated only the representative level 1 journey; board rules, accepted level assets, save, localization, scenes, prefabs, art, packages, and project settings were not modified.
- Full regression result: Edit Mode 67/67, Play Mode 7/7, level validation 30/30, Android build successful.
- Physical-device test confirmed rendering, touch movement, level completion, departure travel, next checkpoint, and subsequent level loading.
- Puzzle objects retain `CellSize = 1.25f`; presentation camera receives no `BoardGame` reference.
- The installed x86_64 emulator cannot load the ARM64-only development APK. This is an environment compatibility limitation, not a gameplay failure; no additional system image or package was downloaded.
- Visual review is required before any rollout to additional levels.
