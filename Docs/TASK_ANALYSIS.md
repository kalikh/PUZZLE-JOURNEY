# Task Analysis: Phase 2.1 — Journey Polish, Stability, and UX Validation

## Game Vision and Current Stage

Caravan Secrets is a bilingual portrait directional caravan puzzle whose dominant board logic remains independent from the journey presentation. Phase 2 supplied one functional long-road/checkpoint prototype around level 1. Phase 2.1 must polish and harden only that representative experience before any wider rollout or Phase 3 system.

## Exact Current Task

Make the existing checkpoint-to-puzzle-to-next-checkpoint sequence visually coherent, pause-safe, idempotent, persistently restorable, localized, and verified across supported portrait layouts and Android.

## Relevant Specification Sections

- Commercial Vertical Slice Extension; Long Desert Road Requirement.
- Core Game Concept; Gameplay Structure Rule; Primary Puzzle Mechanic.
- Architecture and Separation of Concerns; Save System; Localization and RTL.
- Camera and Resolution; Mobile Input; Performance and Android; Testing and Validation.

## Current Journey Architecture

- `Game/Journey/JourneySession.cs` is a pure state machine containing the segment identity, current checkpoint, and five journey phases; it has no Unity or board dependency.
- `Features/Journey/RepresentativeJourneyPresenter.cs` creates a runtime-only landscape from existing background, road, cart, gate, and rock prefabs and owns presentation coroutines.
- `Features/Gameplay/GameplayController.cs` owns board orchestration, starts the representative approach for level 1, disables input while the presenter travels, and loads level 2 after departure.
- `BoardGame` remains unaware of journey and camera state.

## Existing Camera Behavior

- Orthographic camera starts at journey Y=-14, follows the presentation caravan toward Y=0, then settles at the fixed puzzle position.
- After completion it follows from the puzzle area to Y=14, holds, hides the landscape, resets to Y=0, and loads level 2.
- Motion uses unscaled time and smoothstep interpolation, but currently lacks explicit pause suspension, safe-area framing validation, resize refresh, and transition cancellation/idempotence protection.

## Current Checkpoint Flow

- New in-memory session starts at `desert_start`.
- Approach transitions through `TravellingToPuzzle` to `AtPuzzle`.
- Completing level 1 and invoking Next transitions through `TravellingToNextCheckpoint` to `desert_checkpoint_02`, then loads level 2.
- The current implementation reconstructs this session on every gameplay launch and does not restore checkpoint state.

## Existing Save Interaction

- Bootstrap registers the existing `JsonFileSaveService` behind `ISaveService`.
- `PlayerSaveData` version 1 contains `CurrentLevelId`, currency, language, and level progress but no journey checkpoint fields.
- Gameplay currently does not resolve/use that service. The smallest compatible extension is to add optional journey fields to `PlayerSaveData`, preserve version-1 JSON defaults, and have gameplay load/save only stable phases. No second save system is permitted.

## Files Expected to Change

- `Assets/Scripts/Game/Journey/JourneySession.cs`: restoration/idempotence-safe domain behavior.
- `Assets/Scripts/Features/Journey/RepresentativeJourneyPresenter.cs`: visual continuity, camera/pause/safe-area polish, stable restore presentation, localized checkpoint feedback hook.
- `Assets/Scripts/Features/Gameplay/GameplayController.cs`: existing-save resolution, ordered checkpoint persistence, duplicate guard, stable restoration, localization/status orchestration.
- `Assets/Scripts/Data/Save/PlayerSaveData.cs`: backward-compatible journey fields only.
- Focused Edit Mode and Play Mode journey/save/layout tests and only necessary assembly references.
- Existing English/Arabic string tables only if required journey keys are absent.
- `Docs/CURRENT_TASK.md`, `Docs/TASK_ANALYSIS.md`, and `Docs/PROJECT_STATUS.md`.
- Ignored validation logs, APK, and screenshots.

## Files That Must Not Change

- All `Assets/Resources/Levels/*.asset`, especially byte-frozen levels 1–5.
- `BoardGame.cs`, `BoardState.cs`, `LevelDefinition.cs`, solver, validator, movement/collision, undo/restart, boosters, and results.
- Scenes, prefabs, raster art, package manifest/lock, project settings, Android toolchain, and archived documentation.
- `JsonFileSaveService` and localization infrastructure unless a reproducible defect makes a smaller change impossible.

## Test Plan

- Edit Mode: restoration from stable checkpoints; intermediate phases normalize safely; duplicate departure/arrival prevention; backward-compatible save defaults and checkpoint persistence data.
- Play Mode: full representative flow, input gating, single next-level load, pause/resume during travel, save before level 2, restored level/checkpoint, restart safety, EN/AR strings, and camera/layout bounds at 720×1600, 1080×1920, 1080×2400, and portrait tablet.
- Full Edit/Play regression, 30-level validation, frozen hash verification, Android development build, install/launch/manual device sequence, and clean screenshots.

## Risks and Possible Conflicts

- Existing save data may omit new fields: normalize missing/invalid values to level 1/start without destroying other data.
- Closing during travel could persist an impossible phase: save only stable arrival/checkpoint state and restore transitional values to the last stable position.
- Time-scale pause could conflict with unscaled coroutines: explicitly suspend journey progression while application/game pause is active.
- Camera/layout polishing could shrink puzzle objects: keep orthographic puzzle size and `CellSize = 1.25f` unchanged; adjust journey framing/content instead.
- Completion feedback and Next may race: one controller gate must own departure and load exactly once.
- Visual labels risk hard-coded text: resolve keys through the existing localization route and preserve world direction in Arabic.

## Acceptance Criteria

All criteria in `CURRENT_TASK.md` must be met, with objective test/build/device evidence, before marking Phase 2.1 complete.

## Consistency Statement

The requested Phase 2.1 work is consistent with `PROJECT_SPEC.md` and narrows the adopted long-road milestone. No authoritative-instruction conflict exists. Phase 3 and all prohibited systems remain excluded.

## Pre-Implementation Report

- Read completely: `AGENTS.md`, `Docs/PROJECT_SPEC.md`, `Docs/PROJECT_STATUS.md`, `Docs/CURRENT_TASK.md`, `Docs/TASK_ANALYSIS.md`, and the attached Phase 2.1 task.
- Architecture and current save gap inspected as documented above.
- Planned files, frozen files, tests, risks, and exclusions are explicit.
- Working tree was clean at commit `22488e8` before this documentation update.

## Validation So Far

- Edit Mode 73/73, Play Mode 9/9, and level validation 30/30 pass.
- Android development build succeeds and installs; levels 1–5 hashes remain unchanged.
- Save restoration to level 2 was confirmed from the physical-device JSON and a relaunch.
- Final clean five-shot visual evidence is still pending because an external phone overlay obscured capture. Phase 2.1 therefore remains in progress and is not marked complete.
