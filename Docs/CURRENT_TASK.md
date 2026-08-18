# Current Task: Phase 2 — Long Journey Road and Camera Foundation

## Status

**Complete (2026-08-18).** Stable pre-change baseline commit: `a3587b94510936fe6e1740992223fb348d01f727`.

## Objective

Implement one representative, reviewable Desert Road journey segment without changing puzzle rules or applying the system to all 30 levels.

Required sequence:

`Start checkpoint → long desert road → puzzle location → puzzle completion → caravan travel animation → next checkpoint`

## Authorized Scope

- Add a Journey domain/session model independent of `BoardGame` and board rules.
- Track the current checkpoint and journey phase.
- Add one reusable long-road presentation using the existing Desert Road visual resources.
- Add controlled camera transitions between start checkpoint, puzzle location, and next checkpoint.
- Keep the puzzle board at its existing usable mobile scale.
- Trigger the representative post-puzzle travel sequence from completion/Next, then advance normally.
- Add focused Edit Mode and Play Mode tests.
- Capture before-puzzle, during-puzzle, and after-travel screenshots.
- Run the full tests, 30-level validation, frozen-level hash check, and Android development build.

## Frozen / Prohibited

- Do not modify puzzle movement, collision, undo, restart, solver, validator, level loading contracts, save architecture, localization architecture, or existing level data.
- Do not implement survival, camp, ads, purchases, analytics, audio production, characters, events, economy, or new puzzle mechanics.
- Do not apply journey presentation to all 30 levels.
- Do not shrink puzzle objects or change board scale to simulate distance.
- Camera movement must never change puzzle state.
- Do not add or download packages or external assets.

## Acceptance Criteria

- `JourneySession` can report the current checkpoint and valid phase transitions without referencing puzzle types.
- One representative segment visibly spans more than one viewport and uses start/puzzle/next landmarks.
- Camera transitions are controlled and input is blocked only during travel.
- Puzzle objects retain the established `CellSize` and touch bounds.
- Completing representative level 1 and pressing Next plays travel to the next checkpoint before level 2 loads.
- Existing non-representative level navigation remains unchanged.
- Full Edit/Play tests pass, 30 levels validate, levels 1–5 hashes remain unchanged, and Android development build succeeds.
- Required screenshots and limitations are recorded.

## Completion Evidence

- Journey domain and presentation remain separate from `BoardGame`; camera movement does not mutate puzzle state.
- Representative sequence verified on Android: start checkpoint, approach road, level 1 puzzle, departure travel, next checkpoint, then level 2.
- Edit Mode: **67/67 passed** (`Logs/phase2-editmode.xml`).
- Play Mode: **7/7 passed** (`Logs/phase2-playmode.xml`).
- Level validation: **30/30 passed** (`Logs/phase2-level-validation.log`).
- Android development build succeeded (`Logs/phase2-android-build.log`).
- Levels 1–5 remain byte-identical to the accepted baseline.
- No package, external asset, survival, camp, monetization, analytics, audio, or new puzzle mechanic was added.
- The installed emulator is x86_64 while the current APK contains ARM64 native libraries only; device validation was used and no emulator image/package was downloaded.
