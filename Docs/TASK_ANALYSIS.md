# Task Analysis: Phase 3 — Journey Rollout and Commercial Presentation Polish

## Game Vision and Current Stage

Caravan Secrets is a bilingual portrait directional caravan puzzle for Android. The puzzle engine is verified (73 Edit / 9 Play tests, 30/30 solver validation) and Phase 2/2.1 proved a single representative long-road journey (level 1→2) with checkpoint persistence. The game still *presents* as a prototype: one hard-coded journey segment, sparse board readability, minimal feedback. This task is the first milestone of the adopted Commercial Vertical Slice Program (root `update.md`).

## Exact Current Task

See `Docs/CURRENT_TASK.md`: data-drive the journey chain for Desert Road levels 1–10, improve board route readability and interactable/decoration separation, extend visual feedback for every state change, and polish the existing HUD — using only existing project resources, with frozen puzzle domain and level assets.

## Relevant Specification Sections (references, not quotations)

- `PROJECT_SPEC.md`: Commercial Vertical Slice Extension; §7 Long Desert Road Requirement (update.md); §15 Visual Style; §16 Animation Style; §23 Board System (renderer separate from rules); §32–33 Localization/RTL; §44 Camera and Resolution; §45 Performance.
- `update.md`: §6 Target Vertical Slice; §7 Long Desert Road Requirement; §16 Visual Direction; §22 Implementation Order items 5 and 13.

## Existing Systems That Must Be Preserved

- `Game/Board/*` — movement, state, undo/restart, solver, validator (frozen).
- 30 `Resources/Levels/desert_*.asset` — byte-frozen 1–5; all 30 unchanged.
- `Game/Journey/JourneySession.cs` — pure phase state machine with `RestoreStable`; extend via data, not rewrite.
- `JsonFileSaveService` / `PlayerSaveData` — version-1 compatible; additive fields only.
- `RuntimeLocalizationService` + `Gameplay` table; `ArabicText` shaping.
- `GameplayController` puzzle scale (`CellSize = 1.25f`), camera framing formula, touch bounds.
- Boosters, results/stars, development level browser.
- Scenes (Bootstrap, Gameplay), prefab wiring approach, packages, project settings.

## Current Architecture (verified by inspection, 2026-08-25)

- `GameplayController.cs` (917 lines) owns all presentation: runtime camera, `Resources.Load` of `VerticalSlice/*` prefabs, board visual build, road `RoadStrip` tracks per movable object, tap input (dual-path), HUD instantiation, journey sequencing, save.
- `RepresentativeJourneyPresenter.cs` (226) builds one segment (StartY −14 / PuzzleY 0 / NextY 14) from existing art prefabs with smoothstep camera travel; hard-coded ids.
- `GameplayFeedback.cs` (58): one dust ParticleSystem (7 particles/move), Pulse/Shake coroutines, 4 empty AudioClip slots.
- `GameplayHUD.prefab`: Canvas 1080×2160 scaler, top panel (level/moves/objective), bottom bar (Pause/Undo/Drive/Restart/Hint), embedded EventSystem; TMP font `ArialUnicode SDF`.
- Art kit: `Assets/Art/Region1/VerticalSlice/` — cart, gate, rock, road-strip, road-tile (loaded, unused), switch, desert-background; 3 unused atlas/chroma textures (dead weight, leave untouched this phase).
- Localization: single `Gameplay` table, 36 keys, EN/AR.
- No audio clips, no materials/shaders/anims; URP volume profile exists unused.

## Files Expected to Change

- New: `Assets/Scripts/Data/Journey/JourneySegmentDefinition.cs` (ScriptableObject segment/checkpoint contract) + desert road chain asset(s) under `Assets/Resources/Journey/`.
- New: `Assets/Scripts/Features/Journey/JourneyPresenter.cs` (generalized from `RepresentativeJourneyPresenter`, data-driven; representative class retained or thin-wrapped to avoid breaking existing tests).
- `Assets/Scripts/Features/Gameplay/GameplayController.cs`: consume journey definitions; board route-cell rendering from existing road art; traveled/untraveled visual state; wire extended feedback.
- `Assets/Scripts/Features/Gameplay/GameplayFeedback.cs`: additional bounded effects (gate open, switch pulse exists, completion celebration, dust along path).
- `Assets/Scripts/Features/Gameplay/GameplayHudView.cs` + `Assets/Resources/VerticalSlice/GameplayHUD.prefab`: styling/spacing/safe-area polish only.
- `Assets/Scripts/Data/Save/PlayerSaveData.cs`: additive journey chain fields only if required, version-1 compatible.
- Localization tables: new keys only (journey/progress labels) via existing setup.
- Tests: new Edit Mode (journey definition validation, chain progression, save compat) and Play Mode (multi-level journey traversal, feedback hooks) files.
- Docs: `CURRENT_TASK.md`, `TASK_ANALYSIS.md`, `PROJECT_STATUS.md`.

## Files That Must Not Change

- `Assets/Scripts/Game/Board/**` (all), `Game/Boosters/**`, `Game/Results/**`.
- All `Assets/Resources/Levels/*.asset`.
- `JsonFileSaveService.cs`, `RuntimeLocalizationService.cs`, `GameBootstrap.cs`, `ServiceRegistry.cs`.
- Scenes, packages/manifest, project settings, Android configuration.
- `Docs/Archive/**` and historical logs.

## Acceptance Criteria

As listed in `Docs/CURRENT_TASK.md`: continuous data-driven 1–10 journey, readable routes, distinct feedback per state change, all tests green + new coverage, 30/30 validation, frozen hashes unchanged, EN/AR RTL correct, Android build + device/emulator evidence, docs updated.

## Risks and Possible Conflicts

- Existing Play Mode tests reference the representative journey behavior for level 1→2 — the generalized presenter must keep that flow passing (checkpoint ids `desert_start`, `desert_checkpoint_02` remain valid as data).
- Board readability changes must not alter `CellSize`, camera framing, or touch mapping; visual layers only.
- More visuals per board increases draw calls/overdraw — reuse existing sprites, cap particles, no new textures.
- Save extension must normalize missing fields without erasing existing saves.
- New localization keys must resolve in both locales; missing keys must fail loudly in tests, not silently in builds.

## Plan Divergences (documented per AGENTS.md, 2026-08-28)

- The ScriptableObject contract was implemented as `Assets/Scripts/Data/Journey/JourneyChainAsset.cs` (with `SegmentData`/`LandmarkData`) instead of a `JourneySegmentDefinition.cs` SO; `JourneySegmentDefinition` exists as a pure C# class inside `Assets/Scripts/Game/Journey/JourneySession.cs`. The data-driven contract requirement is satisfied.
- `RepresentativeJourneyPresenter.cs` was generalized in place to be fully data-driven instead of adding a new `JourneyPresenter.cs`; the name is retained for compatibility with accepted Phase 2/2.1 tests.
- `GameplayHUD.prefab` was intentionally not modified; safe-area and button-style polish is applied at runtime in `GameplayHudView.cs` (`ApplySafeArea`, `NormalizeButtonStyles`), keeping prefab wiring stable.

## Consistency Statement

This work is consistent with `PROJECT_SPEC.md` including the adopted `update.md` extension; it implements the next items of the mandated implementation order without touching frozen systems or prohibited scope. No authoritative-instruction conflict exists.

## Pre-Implementation Report

- Documents read completely: `AGENTS.md`, `update.md`, `Docs/PROJECT_SPEC.md` (all 2608 lines), `Docs/PROJECT_STATUS.md`, `Docs/CURRENT_TASK.md` (previous), secondary docs (`SPEC_COMPLIANCE_REPORT`, `GAMEPLAY_EVOLUTION_SPEC`, `LEVEL_QUALITY_AUDIT`, `LEVEL_SOLUTIONS`) summarized.
- Architecture inspected via full inventory (26 runtime scripts, art, prefabs, scenes, tests, localization).
- Files planned, frozen files, tests, risks, and exclusions are explicit above.
- Explicitly excluded from this task: survival/characters/events/camp/economy, audio clips, new packages/assets, level content changes, new scenes.
