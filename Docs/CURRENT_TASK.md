# Current Task: Phase 3 — Journey Rollout and Commercial Presentation Polish

## Status

**Implementation complete and verified (2026-08-28).** Edit 81/81, Play 11/11, validation 30/30, levels 1–5 hashes unchanged, Android build succeeded, device evidence captured. Awaiting user review/commit approval before closing. Evidence recorded in `Docs/PROJECT_STATUS.md` › Phase 3 Evidence; plan divergences documented in `Docs/TASK_ANALYSIS.md`.

This task is the first implementation milestone of the Commercial Vertical Slice Program adopted from root `update.md` (see `PROJECT_SPEC.md` › Commercial Vertical Slice Extension). It executes items 5 and 13 (partial) of the update.md implementation order: long-road camera/presentation rollout and visual feedback polish. Items 1–4 (baseline, build repair, puzzle state model, movement/undo/reset/hints/validation) are already verified complete.

## Objective

Make the game *look and feel* like a commercial caravan journey instead of a prototype board, using only existing project resources:

1. **Generalize the journey**: replace the hard-coded representative level-1→2 journey with a data-driven journey segment contract (ScriptableObject) describing the Desert Road chain (levels 1–10: checkpoints, travel segments, landmarks), and drive the existing journey presenter from it.
2. **Board readability**: render visible route cells/tracks and traveled/untraveled distinction from the existing road art; strengthen separation between interactable objects and decoration; keep puzzle scale, `CellSize = 1.25f`, and touch bounds unchanged.
3. **Feedback polish**: extend the existing dust/movement/gate/switch/completion feedback so every state change has a visual response; all effects bounded for mid-range Android (capped particles, no per-frame allocations in hot paths).
4. **HUD polish**: consistent button styling/spacing/typography within the existing `GameplayHUD` prefab, safe-area adherence, EN/AR unchanged behavior.

## Authorized Scope

- New data assets/scripts for journey segment definitions (data-driven, no hard-coded level ids in presenters/controllers).
- Changes to `Features/Journey/*`, `Features/Gameplay/GameplayController.cs`, `GameplayFeedback.cs`, `GameplayHudView.cs`, and the `GameplayHUD.prefab` where required by the above.
- New localized EN/AR string keys for journey/presentation needs, added through the existing `Gameplay` table only.
- New Edit/Play Mode tests for journey rollout, presentation helpers, and feedback state effects.
- Additive-only `PlayerSaveData` extension if journey chain progress requires it (preserve version-1 compatibility).

## Frozen / Prohibited

- Do not modify puzzle movement, collision, undo, restart, solver, validator, boosters, result rules, or the 30 level assets (levels 1–5 remain byte-frozen).
- Do not add survival, camp, characters, events, economy, ads, purchases, analytics, or audio production (later phases).
- Do not add or download packages, external assets, fonts, or SDKs. No internet downloads without explicit user approval.
- Do not add scenes; `Bootstrap.unity` and `Gameplay.unity` remain the only build scenes.
- Do not refactor the save service, localization service, or bootstrap beyond the minimum required.
- Do not use crude raw primitives as player-facing presentation; runtime-generated shapes are allowed only as non-player-facing debug aids.
- Do not change the selected Unity version or installed packages.

## Acceptance Criteria

- Desert Road levels 1–10 play as one continuous journey: checkpoint → travel → puzzle → departure → next checkpoint, data-driven, with landmarks and traveled/untraveled distinction.
- Every board shows readable routes for all movable objects; interactable vs decoration separation is visually obvious in screenshots at 1080×2400.
- Move, invalid move, switch activation, gate open, and level completion each produce distinct visual feedback; no effect blocks input beyond existing animation gates.
- All existing tests pass; new tests cover journey definition validation, chain progression, and presentation state helpers; level validation remains 30/30; levels 1–5 SHA-256 hashes unchanged.
- Arabic and English resolve through localization with correct RTL; no new hard-coded player-facing text.
- Android development build succeeds and the journey chain is verified on device or documented emulator limitation.
- `Docs/PROJECT_STATUS.md` updated with evidence; `Docs/TASK_ANALYSIS.md` completed before implementation.

## Program Roadmap (subsequent phases, not this task)

- Phase 4: Survival resources (water/food/health/morale), data-driven consumption, HUD resource display.
- Phase 5: Guide/Guard/Doctor characters with validated abilities.
- Phase 6: Decision-event framework (rescue, route choice, trade).
- Phase 7: Five-facility persistent camp + upgrades.
- Phase 8: Rewards/progression/economy connection, monetization/analytics interfaces (mock only).
- Phase 9: Ten-stage content polish pass, audio architecture, final verification per update.md §24.
