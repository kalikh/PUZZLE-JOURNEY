# Task Analysis: Commercial Vertical Slice — Phase 1 Baseline and Resource Reuse Audit

## Game Vision

Caravan Secrets is an offline-first Arabic/English portrait mobile game whose dominant identity remains directional caravan path puzzles. The adopted extension connects those puzzles to a visually long desert journey, lightweight survival decisions, useful characters, events, rewards, persistent camp upgrades, and progression. The first commercial target is ten polished Desert Road stages lasting roughly 15–25 minutes.

## Current Development Stage

Stage 4 is accepted: 30 solver-confirmed puzzle levels and the existing cargo, gates/switches, storage, direction tiles, boosters, stars, localization, and Android slice exist. The commercial vertical-slice program starts now at Phase 1 inspection; long-road, survival, characters, events, camp, and connected progression are not implemented.

## Exact Current Task

Inspect and baseline the existing project, classify resources for reuse, run current verification, document gaps, and select the next single dependency-safe milestone. No new product feature is authorized in this phase.

## Relevant Specification References

- Project Specification Authority and Commercial Vertical Slice Extension.
- Core Game Concept; Gameplay Structure Rule; Primary Puzzle Mechanic.
- Mandatory Reuse of Existing Project Resources.
- Target Vertical Slice; Long Desert Road Requirement.
- Architecture; Save System; Performance and Android; Testing.
- Implementation Order; Definition of Done; Final Report Format.

## Existing Systems to Preserve

- `BoardGame`, `BoardState`, `LevelDefinition`, `LevelValidator`, and `LevelSolver` deterministic puzzle domain.
- `LevelAsset` data pipeline, 30 accepted assets, editor tooling, and recorded solver solutions.
- Gameplay movement/input, undo/reset, typed objectives, cargo/mechanisms, boosters/stars, localization/Arabic shaping, Bootstrap/service registration, and atomic backup save foundation.
- Existing Region 1 art/prefabs/HUD and Android configuration until evidence supports targeted improvement.

## Files Expected to Change

- `Docs/PROJECT_SPEC.md` (adoption record only).
- `Docs/CURRENT_TASK.md`.
- `Docs/TASK_ANALYSIS.md`.
- `Docs/PROJECT_STATUS.md` after inspection and verification.
- Generated logs/build evidence only. Implementation files only if a reproducible build-breaking defect blocks the baseline.

## Files That Must Not Change

- `Assets/Resources/Levels/desert_01.asset` through `desert_30.asset`.
- Scenes, prefabs, art, localization tables, packages, project settings, and runtime source during audit unless the smallest documented blocker fix is required.
- `Docs/Archive/` and unrelated user files.

## Tests Planned

- Unity Edit Mode suite.
- Unity Play Mode suite.
- `LevelTools.ValidateAll` for 30 assets and hash comparison for frozen levels.
- Android development build with the already installed Unity/SDK/NDK/JDK toolchain.
- Read-only inspection of generated build/device evidence; no package downloads.

## Acceptance Criteria

- Concrete architecture/resource inventory and reuse matrix.
- Package sufficiency assessment with no downloads.
- Baseline compile/tests/level validation/build results.
- Identified gaps, risks, placeholders, and duplicates supported by file evidence.
- One exact next milestone consistent with the adopted extension.

## Risks and Conflicts

- The root extension asks for one status file, while repository policy mandates `CURRENT_TASK.md` and `TASK_ANALYSIS.md`; `AGENTS.md` has priority, so required control documents remain and product status stays consolidated in `PROJECT_STATUS.md`.
- Existing Stage 4 content covers 30 logical boards, while the new vertical slice requires redesigning the first ten into a connected journey. No accepted asset will change during baseline; later content changes require an explicitly authorized task and regression plan.
- Existing art may be temporary rather than commercial final art; it must be inspected and classified before replacement or external acquisition.
- Large feature breadth creates integration risk; implementation will follow dependency gates rather than parallel disconnected systems.

## Consistency Statement

The requested adoption and Phase 1 work are consistent with the newly adopted extension and the original master specification. The user explicitly authorized the extension and prohibited unreviewed downloads. No unresolved instruction conflict blocks this audit.

## Verification Checkpoint — 2026-08-18

- Unity `6000.3.21f1` compiled without C# errors.
- Edit Mode passed 64/64; Play Mode passed 6/6.
- All 30 level assets validated and solved; frozen levels 1–5 retain their accepted SHA-256 hashes.
- Android development APK built successfully using only the installed SDK/NDK/JDK and cached packages.
- No source, scene, prefab, ScriptableObject, package manifest, project setting, localization table, or player-facing asset was modified.
- No package or external asset was downloaded or imported.
- Phase 1 acceptance criteria are met. The next dependency-safe milestone is Phase 2: journey foundation and long-road presentation architecture, after its own task analysis.
