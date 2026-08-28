# GAMEPLAY_EVOLUTION_SPEC.md

## Caravan Secrets: Puzzle Journey

**Arabic:** أسرار القافلة: رحلة الألغاز

## Gameplay Evolution & Level Design Specification

---

# 1. Purpose

This document defines the required gameplay evolution for **Caravan Secrets: Puzzle Journey**.

The current project already has:

* A functioning Unity project.
* Working Android builds.
* Existing movement and touch interaction.
* Working puzzle completion.
* Long-road journey foundation.
* Checkpoint progression.
* Caravan travel between checkpoints.
* Arabic and English localization foundations.
* Save infrastructure.
* 30 validated level definitions.
* Edit Mode and Play Mode automated tests.
* Existing imported resources and project assets documented through the project resource inventory and reuse matrix.

The purpose of this document is **not to restart the project**.

The purpose is to transform the current technically functional game into a deeper puzzle-adventure with:

* Genuine logical puzzles.
* Meaningful variation.
* Gradually introduced mechanics.
* Long-term progression.
* Strong connection between puzzles and the caravan journey.
* Reuse of existing project resources wherever reasonable.

The game must stop behaving like a sequence of simple movement demonstrations.

---

# 2. Mandatory Resource Reuse Rule

Before creating any new visual object, prefab, mechanic presentation, UI component, texture, particle effect, or environmental prop:

1. Read the resource inventory in `PROJECT_STATUS.md`.
2. Read the existing reuse matrix.
3. Inspect the actual project Assets folders.
4. Search existing Prefabs, Sprites, Materials, ScriptableObjects, UI assets, shaders, effects, and environment assets.
5. Reuse compatible resources before creating replacements.

Use the existing classifications:

* REUSE AS-IS
* REUSE WITH IMPROVEMENT
* REPLACE
* ARCHIVE / IGNORE

Do not download a new asset because the existing asset is merely imperfect.

Prefer:

* Recoloring.
* Rescaling.
* Reframing.
* Sprite variation.
* Material variation.
* Recomposition.
* Prefab composition.
* Animation improvement.

before replacement.

Do not break existing GUID references unnecessarily.

Do not replace a working resource without recording the reason in `PROJECT_STATUS.md`.

---

# 3. External Dependency Rule

Do not download or install:

* New Unity packages.
* Asset Store packages.
* Third-party code libraries.
* External art packs.
* Audio packs.
* Shader packs.

unless the required gameplay cannot reasonably be built with the current project resources and Unity packages.

If a missing dependency is discovered:

1. Document the exact gap.
2. Explain why existing resources cannot satisfy it.
3. Propose the smallest possible addition.
4. Stop and request approval before downloading anything.

The current installed Unity systems should be used first, including existing:

* URP capabilities.
* Unity Localization.
* Input System.
* Addressables.
* Unity testing infrastructure.
* Android build tooling.

---

# 4. Core Product Identity

The game is not:

> A generic traffic puzzle with Arabian graphics.

The game must become:

> A puzzle adventure about moving a caravan through a dangerous, mysterious, evolving journey.

The player must feel that every puzzle exists because something is preventing the caravan from continuing.

Examples:

* A blocked desert crossing.
* Carts trapped at an intersection.
* A damaged bridge.
* A locked city gate.
* Incorrect cargo distribution.
* A dry oasis mechanism.
* An ancient machine blocking the route.
* A collapsed passage.
* A market congestion problem.
* A map route that must be reconstructed.

The puzzle and the journey must feel like one product.

---

# 5. Core Design Principle

Maintain one understandable core interaction:

> Inspect the situation, determine dependencies, move or manipulate objects in the correct order, clear the route, and continue the journey.

Do not transform the game into a random mini-game collection.

New mechanics must be related to:

* Position.
* Direction.
* Order.
* Path.
* Space.
* Dependency.
* Delivery.
* Activation.
* Routing.

---

# 6. Gameplay Composition

Target long-term gameplay distribution:

* 55% core caravan routing and dependency puzzles.
* 15% cargo and destination puzzles.
* 10% switch, gate, and mechanism puzzles.
* 10% environmental route puzzles.
* 5% map and exploration puzzles.
* 5% special story puzzle encounters.

This distribution may vary by region.

Do not place every mechanic into every level.

---

# 7. Fundamental Puzzle Requirement

From Level 6 onward, a level is not considered a genuine puzzle unless it contains at least one meaningful dependency.

Examples:

* A must move before B.
* B must remain in place until C activates a switch.
* Moving D too early creates a dead end.
* A storage position must be used temporarily.
* An exit must first be unlocked.
* Cargo must be sent through matching destinations.
* A bridge state changes the available route.

A level must require thought, not merely repeated tapping.

---

# 8. Puzzle Quality Rules

Every production puzzle must satisfy most of the following:

* The player can understand the objective.
* Multiple actions appear possible.
* Not every possible action is equally useful.
* At least one action affects another object.
* Order matters.
* A meaningful wrong decision can occur.
* Undo has a practical purpose.
* The correct solution is logically predictable.
* Failure must not depend on randomness.
* The puzzle remains readable on a mobile screen.

Avoid fake difficulty created by:

* Tiny objects.
* Hidden rules.
* Poor contrast.
* Random behavior.
* Unclear destinations.
* Excessive move limits.
* Artificial timers.

---

# 9. Puzzle Families

The game will evolve through the following puzzle families.

---

## FAMILY A — Caravan Routing

This remains the foundational mechanic.

Objects:

* Caravan carts.
* Direction restrictions.
* Rocks.
* Gates.
* Exits.
* Road intersections.

Challenge:

Move carts in the correct order to clear the route.

Evolution:

A1:
Single cart.

A2:
Two carts block one another.

A3:
Three or more carts form dependency chains.

A4:
Multiple exits.

A5:
Special destination requirements.

A6:
Long multi-section route.

Reuse existing cart, road, rock, gate, and exit resources whenever possible.

---

## FAMILY B — Temporary Space / Caravan Parking

Introduce limited temporary positions.

The player may temporarily move:

* A cart.
* Cargo.
* A supply wagon.

into a waiting zone.

The waiting zone is limited.

Example:

Three carts block an intersection.

Only one temporary desert pull-off area exists.

The player must decide which cart temporarily leaves the main road.

This creates genuine planning without requiring a new genre.

---

## FAMILY C — Gates and Switches

Use existing gate and switch concepts but deepen their interaction.

Possible switch behavior:

* Open one gate.
* Close another gate.
* Rotate a bridge.
* Unlock a road.
* Activate a mechanical path.
* Temporarily hold a gate open while occupied.

Later levels may include:

* Two switches controlling one mechanism.
* One switch controlling two opposing gates.
* Timed visual state without real-time pressure.

The logic must remain deterministic.

---

## FAMILY D — Cargo Routing

Introduce cargo as a puzzle object.

Cargo categories may include:

* Fabric.
* Spices.
* Water.
* Tools.
* Food supplies.
* Metal.
* Scrolls.
* Artifacts.

Do not require new art for every category initially.

Reuse existing objects with:

* Icons.
* Symbols.
* Color accents.
* Material variants.

Puzzle concepts:

* Match cargo to correct caravan carts.
* Clear cargo before moving a cart.
* Deliver cargo through a specific exit.
* Avoid blocking a narrow road with the wrong cargo.
* Use limited storage.

---

## FAMILY E — Bridge and Crossing Puzzles

The journey must feel geographically meaningful.

Introduce:

* Broken bridges.
* Rotating bridges.
* Narrow crossings.
* Temporary wooden crossings.
* Canyon passages.
* River or dry-valley crossings.

Puzzle principles:

* Change route state.
* Move carts in correct weight/order sequence.
* Prevent carts from trapping each other.
* Activate crossing mechanisms.

Do not create realistic vehicle physics.

Use logical board states.

---

## FAMILY F — Oasis Water Routing

This is not a survival system yet.

Use water initially as an environmental puzzle mechanic.

Possible interactions:

* Rotate channels.
* Open gates.
* Redirect water.
* Activate a dry mechanism.
* Fill a basin to reveal a route.
* Provide water to open the next checkpoint.

This gives thematic variety without creating a full water-resource management system yet.

---

## FAMILY G — Ancient Mechanisms

Use these primarily in the Forgotten City.

Mechanics may include:

* Rotating stone paths.
* Mechanical locks.
* Pressure plates.
* Linked doors.
* Mirrors.
* Symbol alignment.
* Moving platforms.

These mechanics must still interact with the caravan journey.

Do not create unrelated abstract puzzles detached from the environment.

---

## FAMILY H — Map Fragment Puzzles

Use existing map-fragment progression.

Possible puzzles:

* Reassemble a torn route.
* Rotate map pieces.
* Identify the only safe route.
* Match landmarks.
* Reveal a hidden checkpoint.

These should be short special levels.

They should not replace the main gameplay.

---

## FAMILY I — Multi-Section Journey Puzzle

This becomes an important signature feature.

Instead of showing the entire puzzle at once:

### Section 1

Clear an obstacle.

The caravan moves.

### Section 2

Reach an intersection.

Solve cart ordering.

The caravan moves again.

### Section 3

Open a gate.

The caravan reaches the checkpoint.

Use existing Phase 2 long-road and camera systems.

The objective is to make the long road part of gameplay instead of only decoration.

---

# 10. Signature Mechanic — The Journey Puzzle

Develop one mechanic that differentiates Caravan Secrets from generic board puzzles:

## Progressive Road Puzzle

A level can contain multiple connected puzzle zones on one journey route.

Example:

### Zone A

Three carts block the road.

Solve the cart-order puzzle.

### Travel

Caravan advances along the desert road.

Camera follows.

### Zone B

A bridge is closed.

Move a cart onto a switch.

Rotate the bridge.

### Travel

Caravan advances.

### Zone C

Cargo must be arranged before entering the market.

Solve the final sorting puzzle.

### Completion

The caravan reaches the destination.

This mechanic should become more common in later regions.

Do not apply it to every early level.

---

# 11. Level 1–5 — Tutorial Foundation

Do not remove Levels 1–5 if they are stable.

Treat them as tutorial levels.

Their role:

### Level 1

Teach:

* Selecting a cart.
* Moving toward an exit.

### Level 2

Teach:

* Obstacles.

### Level 3

Teach:

* Interaction between two carts.

### Level 4

Teach:

* Gate or switch.

### Level 5

Teach:

* Undo or temporary planning.

If any existing level does not currently teach its intended mechanic clearly, improve the level data rather than rebuilding the underlying system.

Do not introduce major difficulty here.

---

# 12. Levels 6–10 — First Real Puzzles

Goal:

Transition from tutorials to actual puzzle solving.

Introduce:

* 3–4 carts.
* Dependencies.
* Wrong ordering.
* Multiple exits.
* Temporary waiting position.

Target solution length:

6–10 meaningful actions.

### Level 6

First genuine dependency puzzle.

Three carts.

One blocks another.

### Level 7

Two destinations.

Player must identify the correct exit.

### Level 8

Temporary waiting position.

### Level 9

Three-cart dependency chain.

### Level 10

Mini challenge combining:

* Rock.
* Temporary space.
* Two exits.
* Multiple carts.

Level 10 should feel like the first milestone.

---

# 13. Levels 11–15 — Gate Logic

Introduce:

* Switch.
* Gate.
* Cart dependency.

### Level 11

Single switch opens one gate.

### Level 12

A cart must remain on the switch.

### Level 13

Switch opens one route while restricting another.

### Level 14

Multiple carts and gate dependency.

### Level 15

First multi-stage gate puzzle.

Target solution:

8–12 meaningful actions.

---

# 14. Levels 16–20 — Cargo

Introduce cargo gradually.

### Level 16

One cargo type and one destination.

### Level 17

Two cargo types.

### Level 18

Cargo blocks cart movement.

### Level 19

Temporary cargo storage.

### Level 20

Cargo + carts + gate.

Use visual markers to avoid relying only on color.

---

# 15. Levels 21–25 — Oasis Route

Change environment and puzzle rhythm.

Introduce:

* Oasis roads.
* Water channels.
* Bridges.
* Narrow routes.

### Level 21

Simple water activation.

### Level 22

Water opens a road.

### Level 23

Cart blocks water mechanism.

### Level 24

Bridge + cart order.

### Level 25

Multi-section oasis journey.

Level 25 should use:

Road Section A
→ puzzle
→ caravan movement
→ Section B
→ puzzle
→ checkpoint.

---

# 16. Levels 26–30 — Combined Puzzle Chapter

Use the existing 30-level foundation, but redesign Levels 26–30 where necessary to prove deeper gameplay.

Combine:

* Multiple carts.
* Cargo.
* Gate.
* Switch.
* Temporary space.
* Multiple exits.

Do not use every mechanic simultaneously.

### Level 30

Must be a chapter finale.

Recommended structure:

#### Section A

Clear desert congestion.

#### Section B

Activate city mechanism.

#### Section C

Deliver caravan through final gate.

Target:

10–16 meaningful actions across sections.

Provide clear checkpoint feedback between sections.

---

# 17. Levels 31–40 — Market Chapter

Future expansion.

Environment:

* Narrow streets.
* Market stalls.
* Cargo traffic.
* Warehouses.
* Multiple caravan destinations.

New emphasis:

* Sorting.
* Limited space.
* Destination routing.

Puzzle combinations:

Cargo + parking.

Cargo + gate.

Multiple destinations + cart dependency.

Introduce no more than one major new rule every 2–3 levels.

---

# 18. Levels 41–50 — Mountain Passage

Future expansion.

Environment:

* Narrow canyon.
* Cliff roads.
* Bridges.
* Falling-rock barriers represented logically, not dynamically.
* Alternate paths.

New emphasis:

* Route switching.
* Bridge states.
* Limited passage capacity.

Level 50:

Large multi-section caravan crossing.

---

# 19. Levels 51–60 — Ancient Ruins

Introduce more advanced mechanisms.

Use:

* Pressure plates.
* Rotating paths.
* Linked gates.
* Stone mechanisms.

Avoid overly abstract symbol puzzles.

Keep carts and journey central.

---

# 20. Levels 61–70 — Expedition Planning

Begin light strategic choices.

Not full survival yet.

Examples:

Choose between:

* Short difficult path.
* Longer easier path.

or:

Choose which caravan cart enters a route first.

Choices affect:

* Puzzle configuration.
* Reward.
* Optional story.

Do not introduce permanent character death or harsh punishment.

---

# 21. Levels 71–80 — Multi-Stage Journey Challenges

Increase use of Progressive Road Puzzles.

Typical level:

Puzzle A
→ Travel
→ Puzzle B
→ Travel
→ Puzzle C
→ destination.

Each section must be shorter than a full independent level.

Avoid exhausting mobile sessions.

---

# 22. Levels 81–90 — Mastery

Combine previously learned mechanics.

No major new mechanic is required.

Instead test mastery through:

* Clever dependencies.
* Efficient storage.
* Multi-exit planning.
* Gate sequencing.
* Cargo routing.
* Journey segmentation.

---

# 23. Levels 91–99 — Forgotten City

High-level puzzles.

Visual emphasis:

* Ancient mechanisms.
* Mysterious architecture.
* Map fragments.
* Linked doors.
* Hidden road revelation.

Keep mechanics deterministic.

Difficulty should come from reasoning.

---

# 24. Level 100 — Major Journey Finale

Level 100 should not simply be a larger board.

Create a multi-stage journey level.

Recommended structure:

## Section 1 — Caravan Approach

Route carts through an obstruction.

## Section 2 — Ancient Gate

Use switches and carts.

## Section 3 — Internal Mechanism

Solve a route mechanism.

## Section 4 — Final Passage

Deliver the caravan to the destination.

Use:

* Camera progression.
* Road travel.
* Story moment.
* Map-fragment payoff.

The level should demonstrate everything the player learned without becoming excessively long.

---

# 25. Difficulty Curve

Use five major bands.

## Easy

Levels 1–10

Target thinking time:

5–30 seconds.

## Easy-Medium

11–25

30–60 seconds.

## Medium

26–50

45–120 seconds.

## Medium-Hard

51–75

1–3 minutes.

## Advanced

76–100

2–5 minutes.

Avoid designing levels that regularly require more than five minutes of uninterrupted solving.

Mobile sessions must remain manageable.

---

# 26. Move Counts

Do not artificially inflate move count.

Typical ranges:

Tutorial:

1–5.

Early puzzles:

5–10.

Intermediate:

7–14.

Advanced:

10–20 meaningful actions.

Multi-section levels may exceed this because they contain multiple puzzle zones.

A move is considered meaningful when it changes the logical puzzle state.

Do not count decorative travel animation as puzzle moves.

---

# 27. Dead Ends

Use logical dead ends carefully.

Good dead end:

The player moves Cart A too early, causing Cart B to lose access to its destination.

Bad dead end:

The game unexpectedly blocks an object because of an unexplained hidden rule.

Always ensure:

* Cause is understandable.
* Undo is available.
* Player can learn from the mistake.

---

# 28. Hint System Evolution

Hints should not simply show the entire solution.

Hint Level 1:

Highlight a relevant object.

Hint Level 2:

Show the recommended next object and direction.

Future advanced hint:

Explain dependency indirectly.

Example:

> “This cart may be needed before the gate can close.”

Arabic localization must be supported.

Use existing hint infrastructure if available.

Do not create a second independent hint system.

---

# 29. Visual Language

Interactive meaning must be recognizable immediately.

Use consistent visual conventions.

Examples:

### Movable cart

Visible direction indicator.

### Destination

Matching symbol.

### Switch

Clearly connected to controlled object.

### Locked gate

Visible lock state.

### Temporary space

Distinct resting-zone marker.

### Water mechanism

Clear channel direction.

### Bridge

Clear open/closed state.

Do not communicate gameplay logic only through color.

---

# 30. Long Road Integration

The long road must no longer be treated purely as visual decoration.

Use the existing journey and camera foundation.

The player should experience:

Checkpoint
→ Road
→ Puzzle
→ Road
→ Checkpoint.

Later:

Checkpoint
→ Puzzle Zone A
→ Travel
→ Puzzle Zone B
→ Travel
→ destination.

Do not require one enormous logical grid covering the entire road.

Use connected logical puzzle sections.

---

# 31. Camera Rules

The camera must support gameplay, not become gameplay difficulty.

Use camera movement to:

* Reveal the next route section.
* Follow caravan travel.
* Frame checkpoints.
* Introduce new environments.

Do not:

* Hide important puzzle information unfairly.
* Move constantly while solving.
* make objects too small.
* require precision camera control from the player.

The player should not manually control the camera during normal early-game puzzles.

---

# 32. Existing 30 Levels

Do not delete all current levels.

Audit every current level.

Classify each as:

* KEEP AS-IS
* KEEP WITH MINOR IMPROVEMENT
* REDESIGN PUZZLE DATA
* REPLACE LEVEL DESIGN

Do not rewrite stable puzzle-engine code merely because a level is weak.

Weak level design should normally be corrected by changing:

* Object positions.
* Dependencies.
* Objectives.
* Exit configuration.
* Switch relationships.
* Storage availability.

not by rebuilding the engine.

---

# 33. Level Audit Table

Create and maintain a table in project documentation:

| Level | Current Mechanic | Genuine Puzzle? | Difficulty | Action | Reason |
| ----- | ---------------- | --------------- | ---------- | ------ | ------ |

Do this for all 30 existing levels.

No level should be labelled “genuine puzzle” merely because it can fail.

It must require reasoning.

---

# 34. Level Design Metadata

Extend level definitions where required to support:

* Mechanic tags.
* Difficulty.
* Expected solution length.
* Minimum known moves.
* Tutorial status.
* Required mechanics.
* Optional mechanics.
* Solver compatibility.
* Multi-section identifier.
* Journey checkpoint association.

Avoid destructive changes to existing serialized level data.

Use safe migration or backward-compatible fields.

---

# 35. Solver Usage

Use the current validation/solver infrastructure where compatible.

For each production puzzle:

Record:

* Solvable: yes/no.
* Known solution.
* Minimum move count where calculable.
* Search state count.
* Solver limitations.

For unsupported advanced mechanics:

Use manual validation plus documented solution sequence.

Never assume that a level is solvable merely because it visually appears solvable.

---

# 36. Puzzle Review Standard

Before accepting a level, ask:

1. What must the player understand?
2. What decision must the player make?
3. What dependencies exist?
4. What incorrect decision can occur?
5. Why is Undo useful?
6. Is the objective visually clear?
7. Is the solution logical?
8. Does the level teach or test something new?
9. Is the challenge different from the previous level?
10. Does this puzzle belong in a caravan journey?

If these cannot be answered clearly, redesign the level.

---

# 37. Anti-Repetition Rule

Do not create ten consecutive levels that differ only by:

* More carts.
* More rocks.
* Different starting positions.

Use a rhythm such as:

Teach
→ Practice
→ Variation
→ Combination
→ Challenge
→ New mechanic.

Example:

Level 11: Teach switch.
Level 12: Practice switch.
Level 13: Switch + cart dependency.
Level 14: Switch variation.
Level 15: Gate challenge.

---

# 38. Content Reuse Strategy

Existing art should be multiplied through controlled variation.

Example:

One existing caravan-cart asset may support:

* Normal cargo cart.
* Water cart.
* Fabric cart.
* Tool cart.

through:

* Symbol plates.
* Cargo overlays.
* Small color accents.
* Material variations.

One gate asset may support:

* Open.
* Closed.
* Locked.
* Activated.
* Destination-specific.

One road resource may support:

* Desert.
* Oasis.
* Market approach.

through environmental composition.

Do not create unnecessary unique assets for every mechanic.

---

# 39. Audio Constraint

No audio assets currently exist.

Therefore:

* Do not block gameplay evolution on audio.
* Keep audio hooks and interfaces ready.
* Do not fabricate external audio dependencies.
* Add actual sound production as a later approved task.

Puzzle feedback must still work visually without sound.

---

# 40. Survival Systems — Deferred

Do not implement full:

* Water resource management.
* Food resource management.
* Health.
* Morale.
* Doctor.
* Guard.
* Guide.

during the gameplay evolution phase.

These may become later strategic systems.

First prove:

* Core puzzles.
* Long journey.
* Checkpoint progression.
* Puzzle variety.

Environmental water puzzles do not count as a survival-resource system.

---

# 41. Camp — Deferred Until Puzzle Foundation Is Proven

Do not build the full camp simply because it exists in the master specification.

Before camp implementation, the project should have:

* At least 15 strong puzzle levels.
* At least four working puzzle families.
* Stable journey progression.
* Stable save data.
* One successful multi-section journey puzzle.

The camp should then reinforce progression rather than compensate for weak puzzles.

---

# 42. Commercial Design Principle

The game should be marketable through visible puzzle situations.

A gameplay screenshot or short video should show an immediately understandable problem.

Examples:

* Four carts trapped at an intersection.
* One open slot.
* Two locked gates.
* A switch behind another cart.

The viewer should think:

> “I know what I would try.”

This is stronger advertising material than an empty desert road with one cart.

---

# 43. Required Immediate Development Target

Do not attempt Levels 1–100 now.

The immediate target is a **10-Level Gameplay Proof**.

Use Levels 1–5 as tutorials where appropriate.

Redesign Levels 6–10 into genuine puzzles.

The first approval gate is:

### LEVELS 1–10

They must demonstrate:

* Basic movement.
* Blocking.
* Dependency.
* Multiple destinations.
* Temporary space.
* Gate/switch interaction.
* Long-road progression.
* At least one multi-step meaningful puzzle.

Do not continue large-scale level production until Levels 6–10 are reviewed.

---

# 44. First Immediate Level — Level 6

Build Level 6 as the first true puzzle.

Required:

* Minimum 3 movable carts.
* Prefer 4 if existing board readability allows it.
* At least two destinations.
* At least one dependency chain.
* One useful temporary space.
* One meaningful incorrect move.
* Undo must matter.
* 6–10 meaningful moves.
* Clear solution.
* No randomness.

Reuse current:

* Cart assets.
* Rock assets.
* Gate assets.
* Road visuals.
* existing interaction systems.

Do not introduce a new package or art pack.

---

# 45. Level 10 Milestone

Level 10 must combine several learned mechanics.

Recommended:

* 4 carts.
* 2 destinations.
* 1 temporary space.
* 1 switch.
* 1 gate.
* 1 environmental obstacle.

Target:

8–12 meaningful moves.

It should be solvable through reasoning without hidden information.

---

# 46. Multi-Section Prototype

After Levels 6–10 are approved, create one representative multi-section journey puzzle.

Use existing:

* Long-road system.
* Checkpoints.
* Camera.
* Caravan travel.
* Puzzle loading.

Flow:

Puzzle Section A
→ caravan movement
→ Puzzle Section B
→ checkpoint completion.

Do not develop a new journey architecture unless the current one cannot support this.

Extend the existing Phase 2 system.

---

# 47. Git Discipline

Before gameplay redesign:

Create a checkpoint commit.

Do not rewrite existing stable commits.

Each mechanic family should be developed through isolated commits.

Examples:

`feat: redesign levels 6-10 for dependency puzzles`

`feat: add reusable temporary-space puzzle mechanic`

`feat: add cargo destination puzzle rules`

Avoid combining:

* Puzzle redesign.
* Save overhaul.
* UI redesign.
* Camera rewrite.

in one commit.

---

# 48. Regression Requirements

After every gameplay task:

* All existing required Edit Mode tests must pass.
* All Play Mode tests must pass.
* Level validation must pass for all retained production levels.
* Android build must succeed.
* Arabic and English must remain operational.
* Existing journey progression must remain operational.

Do not accept:

> “The new mechanic works but several old tests now fail.”

Fix regressions before continuing.

---

# 49. Codex / Agent Execution Rule

Do not instruct the coding agent:

> “Improve the puzzles.”

Instead provide a specific level-design task.

Example:

> Redesign Level 6 using the existing cart, rock, road, exit, undo, and checkpoint systems. Create a four-cart dependency puzzle requiring 6–10 meaningful moves. Do not modify the movement engine.

The game design decision must precede code modification.

---

# 50. Mandatory Planning Before Code

Before implementing any new puzzle mechanic:

The coding agent must report:

* Existing systems to reuse.
* Existing assets to reuse.
* Existing prefabs to reuse.
* Existing classes to reuse.
* Required new data fields.
* Required new classes, if any.
* Files to modify.
* Tests to add.
* What will explicitly remain unchanged.

If existing resources can satisfy the requirement, do not create duplicates.

---

# 51. Definition of Gameplay Evolution Success

The gameplay evolution phase succeeds when a new player can play Levels 1–10 and clearly feel:

Levels 1–5:

> “I am learning how the caravan world works.”

Levels 6–8:

> “Now I have to think before moving.”

Levels 9–10:

> “I need to plan several moves ahead.”

Later:

> “I understand the rules, but the combinations keep changing.”

This is the target progression.

---

# 52. Non-Negotiable Rule

Do not continue creating large quantities of levels until the game proves that its puzzles are genuinely enjoyable.

The number of levels is not a quality metric.

A strong 10-level prototype is more valuable than 100 repetitive levels.

---

# 53. Immediate Execution Order

Perform the following in order:

1. Read the complete project specification.
2. Read this gameplay evolution specification.
3. Inspect the current resource inventory.
4. Inspect the reuse matrix.
5. Audit all existing 30 levels.
6. Do not change code during the audit.
7. Classify Levels 1–30.
8. Preserve Levels 1–5 as tutorial foundations unless a verified defect exists.
9. Create detailed designs for Levels 6–10.
10. Present the designs and expected solution sequences.
11. Do not implement them yet until the design is internally consistent.
12. Then implement Level 6 only.
13. Build.
14. Test.
15. Validate Level 6.
16. Review.
17. Proceed sequentially through Levels 7–10.
18. Do not automatically continue beyond Level 10.

---

# 54. Required Audit Deliverable

Before implementation create:

`Docs/LEVEL_GAMEPLAY_AUDIT.md`

It must include all 30 existing levels.

For each level:

* Level number.
* Current objects.
* Current objective.
* Current solution concept.
* Current approximate move count.
* Mechanics used.
* Genuine reasoning required: YES/NO.
* Tutorial: YES/NO.
* Repetition risk.
* Resource reuse opportunities.
* Recommended action.
* Proposed future mechanic.

Then provide a summary:

## Keep

Levels suitable without major changes.

## Improve

Levels with useful structure but weak depth.

## Redesign

Levels requiring different logical layouts.

## Replace

Only levels that cannot be salvaged efficiently.

---

# 55. First Stop Condition

After producing:

* Resource reuse review.
* 30-level gameplay audit.
* Level 6–10 designs.

STOP.

Do not modify scenes, scripts, prefabs, ScriptableObjects, or level data yet.

The next implementation task must be approved separately.

This is mandatory.

---

# Final Objective

Transform **Caravan Secrets: Puzzle Journey** from a technically functioning caravan movement prototype into a distinctive puzzle-adventure where:

* The road matters.
* The caravan matters.
* Order matters.
* Decisions matter.
* Levels evolve.
* Existing resources are reused intelligently.
* New mechanics appear gradually.
* The player continuously discovers new combinations instead of repeating the same interaction.

The result must remain recognizable as one coherent game:

**Caravan Secrets: Puzzle Journey — أسرار القافلة: رحلة الألغاز**
