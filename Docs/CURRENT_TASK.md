# Current Task: Phase 2.1 — Journey Polish, Stability, and UX Validation

## Status

**In progress (started 2026-08-18).** Preserve commits `a3587b9` and `22488e8`.

## Objective

Polish and validate the single representative journey so it reads as one intentional flow:

`Checkpoint → Travel → Puzzle Location → Puzzle → Completion → Caravan Travel → Next Checkpoint → Next Level`

## Authorized Scope

- Improve continuity and visual direction of the existing representative desert road using current assets only.
- Smooth and stabilize camera framing across supported portrait resolutions and safe areas.
- Make post-level travel single-shot, pause-safe, and ordered: completion, travel, checkpoint persistence, next-level unlock/load.
- Persist stable journey/checkpoint progress through the existing `ISaveService` and `PlayerSaveData` architecture.
- Add concise localized checkpoint/progress communication in Arabic and English without mirroring world direction.
- Add missing journey, persistence, duplicate-prevention, pause/resume, resolution, localization, and integration tests.
- Validate fresh launch through level 2 restart/reopen on Android and capture the five requested clean screenshots where possible.

## Frozen / Prohibited

- Do not modify puzzle movement, collision, undo, restart, solver, validator, boosters, result rules, or the 30 level assets.
- Do not modify levels 1–5 under any circumstance unless a separately documented reproducible content defect is authorized.
- Do not add survival, camp, water, food, health, morale, companions, events, ads, purchases, analytics, audio production, economy, or new major gameplay systems.
- Do not broadly refactor stable save, localization, bootstrap, scene, prefab, package, or Android configuration.
- Do not add/download packages, external assets, or an emulator image.
- Do not begin Phase 3.

## Acceptance Criteria

- Road/checkpoint presentation is continuous and directional without obvious disconnected rectangles.
- Camera transitions are smooth, settle before input, preserve puzzle touch size, respect safe areas, and fit 720×1600, 1080×1920, 1080×2400 plus one portrait tablet resolution where supported.
- Completion/departure cannot trigger twice, load twice, or remain stuck after pause/resume.
- Arrival is saved before level 2 loads; reopening restores a stable checkpoint/level state rather than an intermediate camera phase.
- Restart remains puzzle-only and does not move journey progress backward.
- Arabic and English journey text resolves through localization with correct RTL and unchanged world direction.
- Existing tests pass, new tests cover the requested journey cases, level validation remains 30/30, levels 1–5 hashes remain unchanged, and Android development build succeeds.
- Required screenshots and manual-validation outcomes are recorded.
