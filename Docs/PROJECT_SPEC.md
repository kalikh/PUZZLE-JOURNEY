# Caravan Secrets: Project Specification
# Project Specification Authority

This document is the permanent master specification for the project.

It defines:

- Game vision.
- Core gameplay.
- Technical architecture.
- Localization.
- Visual direction.
- Development stages.
- Acceptance criteria.
- Restrictions.

Codex must consult this document before every implementation task.

However, this document does not authorize implementing every listed feature at once.

The active scope is always defined by `CURRENT_TASK.md`.

## Commercial Vertical Slice Extension — Adopted 2026-08-18

The user-authorized requirements supplied in the repository root as `update.md` are adopted as a mandatory extension of this master specification. Where the extension adds detail, the stricter requirement applies. It does not erase the established directional/path-puzzle identity, accepted working systems, localization requirements, or data-driven architecture.

The extension changes the next product milestone to a coherent commercial vertical slice centered on the first ten Desert Road stages and the connected loop of preparation, long-road travel, survival resources, consequential events, three functional characters, rewards, persistent camp upgrades, and progression. The full acceptance conditions include bilingual mobile UI, resilient versioned saving, provider-neutral service boundaries, audio/visual feedback, deterministic tests, offline operation, Android verification, and an evidence-based accounting of reused/improved/replaced/unused resources.

Mandatory execution constraints from the extension:

- Inspect and classify existing resources before creating replacements.
- Preserve functional gameplay and serialized compatibility unless evidence justifies the smallest safe change.
- Do not duplicate managers or introduce disconnected screens or nonfunctional buttons.
- The first ten Desert Road stages must form a 15–25 minute progressive vertical slice with the specified stage roles, real survival/character/event consequences, valid solutions, undo/reset/current-state hints, localization, rewards, and persistence.
- The road must visibly and mechanically feel like a journey beyond one viewport; length must serve gameplay.
- Use water, food, caravan health, and morale as explicit data-driven journey state.
- Implement Guide, Guard, and Doctor with real validated data-driven effects.
- Implement three configurable decision-event types and exactly five initial camp facilities with meaningful persistent upgrades.
- Keep economy simple, rewards idempotent, monetization/analytics provider-neutral and offline-safe, and do not add live SDKs without approval.
- Do not claim commercial completion without the concrete verification and final-report evidence listed in the extension.
- Do not download or add packages or external assets until existing resources have been reviewed and the user has approved the specific need.

The original root `update.md` remains the adoption record. This section makes its complete requirements part of this authoritative specification; `CURRENT_TASK.md` continues to limit implementation to one safe milestone at a time.
## Project Title

**Caravan Secrets: Puzzle Journey**

Arabic title:

**أسرار القافلة: رحلة الألغاز**

---

# 1. Role and Responsibility

Act as a senior Unity game developer, C# software architect, mobile game designer, puzzle systems designer, UI/UX designer, level designer, technical artist, localization specialist, performance engineer, and quality-assurance engineer.

Design and build a polished, scalable, commercially viable mobile puzzle game called:

**Caravan Secrets: Puzzle Journey**

The game must be developed using:

* Unity.
* C#.
* A stable Unity LTS version.
* Android as the first target platform.
* A project architecture that supports future export to iOS and other platforms.
* Arabic and English interface support from the beginning.
* Offline gameplay for all core levels.
* Data-driven level creation.
* A custom visual level editor inside Unity.

The final result must be a maintainable game foundation, not a disposable prototype.

Do not directly copy the artwork, interface, level layouts, characters, names, audio, story, progression systems, or proprietary visual identity of any existing commercial game.

The game may use proven puzzle design principles, but it must have its own mechanics, progression, story, visual identity, and user experience.

---

# 2. Game Vision

Caravan Secrets is a family-friendly puzzle adventure inspired by caravans, desert routes, ancient markets, oases, forgotten cities, mysterious maps, mechanical ruins, and fantasy architecture influenced by the wider Arab world.

The game should appeal to both Arab and international players.

The visual identity should feel culturally inspired without becoming limited to one country, political period, tribe, religion, or historical conflict.

The game must feel:

* Mysterious.
* Relaxing.
* Satisfying.
* Visually rich.
* Easy to understand.
* Difficult to master.
* Suitable for short mobile sessions.
* Suitable for long-term progression.

The game must not rely on violence, firearms, graphic content, or sensitive political themes.

---

# 3. Core Game Concept

The player controls a caravan traveling through dangerous and mysterious regions.

Each region contains puzzle boards where the player must:

* Clear blocked caravan routes.
* Move carts in the correct order.
* Open gates.
* Activate switches.
* Sort cargo.
* Repair damaged roads.
* Recover map fragments.
* Discover hidden symbols.
* Unlock lost cities.
* Upgrade the caravan camp.

The game combines several systems, but one primary gameplay mechanic must remain dominant.

The game must not feel like a random collection of unrelated mini-games.

---

# 4. Gameplay Structure Rule

Use the following design balance:

* 70% core directional and path-based puzzles.
* 20% closely related puzzle variations.
* 10% special story puzzles and mini-games.

The primary puzzle mechanic must remain consistent across the game.

Secondary systems must support one or more of the following:

* The caravan journey.
* Story progression.
* Resource collection.
* Player retention.
* World-building.
* Visual progression.

Do not add unrelated gameplay types only to increase feature count.

---

# 5. Primary Puzzle Mechanic

The main puzzle mechanic is a directional caravan path puzzle.

Each puzzle board contains a grid or structured board with objects such as:

* Caravan carts.
* Cargo carts.
* Directional arrows.
* Gates.
* Locks.
* Rocks.
* Sand barriers.
* Animals.
* Bridges.
* One-way roads.
* Switches.
* Temporary storage spaces.
* Cargo destinations.
* Exit points.
* Rotating tiles.
* Mechanical platforms.

Each movable object has specific movement rules.

A cart may:

* Move only forward.
* Move in one assigned direction.
* Move until blocked.
* Require a clear exit.
* Require a matching destination.
* Require a gate to be opened.
* Change direction after crossing a special tile.
* Trigger another object.

The player must move objects in the correct sequence to clear the board and safely complete the objective.

The player should understand the basic controls within the first minute.

---

# 6. Main Gameplay Loop

The main gameplay loop must be:

1. Open the journey map.
2. Select an unlocked level.
3. Enter a caravan puzzle.
4. Review the objective.
5. Solve the puzzle.
6. Earn coins, stars, materials, or map fragments.
7. Return to the caravan camp.
8. Spend resources on visual caravan upgrades.
9. Unlock story scenes, regions, decorations, or mechanics.
10. Continue to a more challenging level.

Each completed level must contribute to visible progression.

The player must feel that short puzzle sessions produce meaningful long-term progress.

---

# 7. Initial MVP Scope

Build an initial MVP with:

* 30 complete playable levels.
* 3 visual regions.
* 10 levels per region.
* One journey map.
* One caravan camp.
* Five caravan upgrade stages.
* One primary in-game currency.
* One secondary collectible: map fragments.
* A three-star rating system.
* Three booster types.
* Arabic and English localization.
* Local progress saving.
* Offline gameplay.
* Sound and music controls.
* Basic accessibility settings.
* A development-only level editor.
* A development-only level validation tool.
* A rewarded-ad abstraction layer.
* No mandatory registration.
* No multiplayer.
* No backend requirement.
* No live chat.
* No real-money economy in the first development stage.

All systems must be designed so new regions, levels, objects, languages, events, and progression layers can be added later without rewriting the core game.

---

# 8. Initial Regions

## Region 1: The Desert Road

Arabic name:

**طريق الصحراء**

Visual identity:

* Sand dunes.
* Open desert roads.
* Wooden carts.
* Small camps.
* Rocks.
* Palm trees.
* Cloth tents.
* Warm daylight.
* Dust particles.
* Simple wooden signs.

Gameplay purpose:

* Teach tapping.
* Teach directional movement.
* Teach exits.
* Teach blocked paths.
* Teach restart.
* Teach undo.
* Introduce the first booster.

Difficulty:

* Beginner.
* Small boards.
* Few objects.
* Clear solutions.
* No complex chains.

---

## Region 2: The Oasis Market

Arabic name:

**سوق الواحة**

Visual identity:

* Market stalls.
* Colorful fabrics.
* Cargo boxes.
* Lanterns.
* Spice baskets.
* Water channels.
* Stone pathways.
* Decorative doors.
* Merchants in the background.
* Soft evening light.

Gameplay purpose:

* Introduce cargo sorting.
* Introduce matching destinations.
* Introduce temporary storage.
* Introduce multiple exits.
* Introduce restricted routes.
* Increase board density.

Difficulty:

* Intermediate.
* More object dependencies.
* Limited storage.
* Multiple objectives.

---

## Region 3: The Forgotten City

Arabic name:

**المدينة المنسية**

Visual identity:

* Ancient stone ruins.
* Hidden chambers.
* Mechanical doors.
* Symbols.
* Broken bridges.
* Torch lighting.
* Dark blue night tones.
* Gold and turquoise details.
* Mysterious machines.
* Rotating stone paths.

Gameplay purpose:

* Introduce switches.
* Introduce locks.
* Introduce chained actions.
* Introduce direction-changing tiles.
* Introduce multi-stage objectives.
* Introduce story-specific puzzles.

Difficulty:

* Intermediate to advanced.
* Multi-step logic.
* Larger boards.
* More dependencies.
* More puzzle object interaction.

---

# 9. Level Progression

Suggested progression:

## Levels 1–3

* One movable cart.
* One direction.
* One exit.
* No fail pressure.

## Levels 4–6

* Multiple carts.
* Simple blocking order.
* Basic undo.

## Levels 7–10

* Rocks.
* Fixed obstacles.
* Restart conditions.
* First optional move target.

## Levels 11–15

* Cargo types.
* Matching destinations.
* Temporary storage spaces.

## Levels 16–20

* Gates.
* Switches.
* Multiple exits.
* Restricted movement.

## Levels 21–25

* Direction-changing tiles.
* Linked mechanisms.
* Multi-object objectives.

## Levels 26–30

* Combined mechanics.
* Multi-stage solutions.
* Story puzzles.
* Limited storage.
* Advanced dependency ordering.

Do not increase difficulty only by adding more objects.

Difficulty should come from:

* Dependency order.
* Route interaction.
* Limited temporary space.
* Multiple objectives.
* Predictable consequences.
* Optional move efficiency.
* Strategic booster usage.

Every level must remain logically understandable and solvable.

---

# 10. Secondary Gameplay Systems

Secondary systems must not replace the main puzzle mechanic.

## Cargo Sorting

Use cargo sorting in selected levels.

Possible cargo types:

* Spices.
* Fabrics.
* Water containers.
* Metal parts.
* Ancient artifacts.
* Tools.
* Food supplies.
* Scrolls.

Cargo must be delivered to matching carts, gates, warehouses, or exit points.

Do not rely only on color.

Use:

* Symbols.
* Shapes.
* Patterns.
* Labels.
* Outlines.

---

## Caravan Upgrading

The player upgrades the caravan camp using earned resources.

Possible upgrades:

* Main caravan cart.
* Travel tent.
* Lantern station.
* Storage area.
* Map table.
* Animal equipment.
* Merchant stall.
* Water storage.
* Workshop.

For the MVP, implement five upgrade stages.

Each upgrade must:

* Change the environment visually.
* Play a short animation.
* Show the upgrade cost.
* Unlock a small reward or story element.
* Persist after closing the game.

Do not turn the camp into a complex city-building economy in the MVP.

The camp is primarily:

* A progression system.
* A retention system.
* A visual reward system.
* A story hub.

---

## Map Fragments

Map fragments are a secondary collectible.

The player earns fragments by:

* Completing key levels.
* Finishing regions.
* Completing optional objectives.
* Opening story chests.
* Completing selected special puzzles.

Map fragments unlock:

* Region previews.
* Story scenes.
* Hidden routes.
* Bonus stages.
* Lore entries.

Do not use map fragments as a confusing premium currency.

---

## Mini-Games

Mini-games should represent no more than 10% of the experience.

Examples:

* Rotate symbols to open a lock.
* Connect water channels.
* Align mirrors.
* Rebuild a broken map.
* Match ancient patterns.
* Find a hidden item.
* Repair a mechanical gate.

Mini-games must:

* Be short.
* Use simple controls.
* Fit the story.
* Reuse the game’s art direction.
* Avoid introducing unrelated genres.

---

# 11. Booster System

Implement three boosters.

## Compass

Arabic:

**البوصلة**

Function:

* Highlights one valid next move.
* Does not fully solve the level.
* Must select a move that progresses toward a solution where possible.

## Rope

Arabic:

**الحبل**

Function:

* Temporarily removes or relocates one eligible obstacle.
* Must not break level logic.
* Must have clear eligibility rules.

## Extra Space

Arabic:

**مساحة إضافية**

Function:

* Adds one temporary storage slot for the current level.
* Must be removed after the level ends.

Use a shared booster architecture.

Example:

```csharp
public interface IBooster
{
    string BoosterId { get; }
    bool CanUse(GameState state);
    BoosterResult Use(GameState state);
}
```

Boosters must be extendable without modifying the core puzzle engine.

In the MVP, boosters are earned through gameplay.

Do not implement aggressive real-money booster sales initially.

---

# 12. Star Rating System

Award up to three stars per level.

Suggested conditions:

* One star for completing the level.
* One star for completing under the recommended move count.
* One star for completing without using a booster.

Store:

* Completion state.
* Best move count.
* Best star result.
* Booster usage.
* First-completion reward status.
* Number of attempts.
* Best completion time, if later required.

Do not repeatedly grant first-completion rewards.

---

# 13. Story System

Use a lightweight story system.

Story premise:

The main traveler inherits an old map from a missing caravan guide. The map points toward a forgotten city containing a secret connected to the history of the caravan route.

The player travels between regions, recovers map fragments, meets travelers, repairs the caravan, and gradually reveals the mystery.

Story delivery methods:

* Short dialogue scenes.
* Region introductions.
* Map fragments.
* Environmental clues.
* Chapter endings.
* Camp conversations.
* Short character reactions.

Avoid long paragraphs.

Dialogue should be readable on mobile.

Each dialogue entry should support:

* Speaker ID.
* Speaker display name.
* Arabic text.
* English text.
* Portrait.
* Emotion.
* Order.
* Optional animation.
* Optional sound.
* Optional action.
* Skip state.

The player must still understand the main objective if dialogue is skipped.

---

# 14. Characters

Create a small original cast.

Suggested roles:

* The main caravan traveler.
* An experienced map keeper.
* A merchant.
* A mechanic.
* A young scout.
* A mysterious traveler.

Characters must:

* Have original designs.
* Avoid offensive stereotypes.
* Avoid politically sensitive identities.
* Use short, clear dialogue.
* Support the puzzle journey rather than dominate gameplay.

Do not require full voice acting in the MVP.

---

# 15. Visual Style

Use a polished stylized 2D art direction.

The game should not attempt photorealism.

Preferred style:

* Soft 2D illustration.
* Clean shapes.
* Layered environments.
* Subtle depth.
* Warm lighting.
* Strong object readability.
* Family-friendly characters.
* Satisfying movement.
* Light fantasy elements.

Suggested palette:

* Sand.
* Gold.
* Turquoise.
* Deep blue.
* Warm brown.
* Fabric red.
* Oasis green.
* Lantern orange.

Gameplay readability is more important than decoration.

Do not place complex patterns behind interactive pieces when they reduce clarity.

---

# 16. Animation Style

Use lightweight and responsive animations.

Required animations:

* Cart movement.
* Cargo movement.
* Gate opening.
* Switch activation.
* Invalid-action shake.
* Object selection.
* Dust particles.
* Coin collection.
* Star reveal.
* Map node unlocking.
* Region unlocking.
* Camp upgrades.
* Reward chest opening.
* Dialogue transitions.
* Booster usage.
* Level completion.

Animations must not block input longer than necessary.

Gameplay state must remain synchronized with animation state.

Prevent:

* Double movement.
* Multiple simultaneous taps.
* Input during unresolved movement.
* Permanent animation locks.
* Progress being saved before movement completion.

Use animation event callbacks, coroutines, async patterns, or a controlled command queue.

Do not make core logic depend on visual animation timing.

---

# 17. Unity Technical Stack

Use:

* Unity LTS.
* C#.
* Universal Render Pipeline where appropriate.
* Unity 2D toolset.
* Unity Input System.
* TextMeshPro.
* Unity Localization package.
* Addressables.
* Unity Test Framework.
* ScriptableObjects.
* Assembly Definitions.
* Unity UI Toolkit or uGUI based on project suitability.
* DOTween only if approved and available, otherwise create a lightweight animation abstraction.
* JSON or ScriptableObject level data.
* Android App Bundle output.
* IL2CPP for release builds.
* ARM64 support.

Avoid unnecessary third-party packages.

Before adding any package:

* Confirm its purpose.
* Confirm its license.
* Confirm Android compatibility.
* Confirm Unity version compatibility.
* Confirm long-term maintenance risk.

---

# 18. Project Architecture

Use a modular, scalable architecture.

Suggested structure:

```text
Assets/
    Art/
        Characters/
        Environment/
        UI/
        PuzzleObjects/
        Effects/
    Audio/
        Music/
        SFX/
        Ambient/
    Addressables/
    Localization/
    Prefabs/
        Gameplay/
        UI/
        Effects/
        Camp/
    Scenes/
        Bootstrap/
        MainMenu/
        JourneyMap/
        Gameplay/
        CaravanCamp/
    ScriptableObjects/
        Levels/
        Regions/
        PuzzleObjects/
        Boosters/
        Economy/
        Story/
    Scripts/
        Core/
            Bootstrap/
            Events/
            StateMachine/
            Services/
            Utilities/
        Game/
            Board/
            Movement/
            Rules/
            Objectives/
            Validation/
            Solver/
            Commands/
            Boosters/
        Features/
            MainMenu/
            JourneyMap/
            Gameplay/
            Camp/
            Story/
            Settings/
            Completion/
        Data/
            Save/
            Levels/
            Localization/
            Repositories/
        UI/
            Views/
            Presenters/
            Components/
        Audio/
        Analytics/
        Ads/
        Editor/
            LevelEditor/
            Validation/
            DebugTools/
    Tests/
        EditMode/
        PlayMode/
```

Create Assembly Definition files for major systems.

Separate:

* Domain logic.
* Unity rendering.
* Input.
* Animation.
* Save data.
* UI.
* Level definitions.
* Localization.
* Analytics.
* Advertising.
* Audio.

The puzzle rules should be testable without loading a Unity scene.

---

# 19. Scene Structure

Use a controlled scene architecture.

Suggested scenes:

## Bootstrap Scene

Responsibilities:

* Initialize services.
* Load player data.
* Initialize localization.
* Initialize audio.
* Initialize analytics abstraction.
* Initialize ad abstraction.
* Route to the next scene.

## Main Menu Scene

Responsibilities:

* Continue.
* New game.
* Journey map.
* Camp.
* Settings.
* Currency display.

## Journey Map Scene

Responsibilities:

* Region navigation.
* Level node states.
* Stars.
* Locked and unlocked levels.
* Current progression.

## Gameplay Scene

Responsibilities:

* Load level data.
* Render board.
* Process input.
* Play movement.
* Evaluate objectives.
* Handle undo.
* Handle restart.
* Handle boosters.
* Handle completion.

## Caravan Camp Scene

Responsibilities:

* Show camp state.
* Show available upgrades.
* Play upgrade animations.
* Show map fragments.
* Trigger story scenes.

Avoid placing all game systems in one scene.

Use additive scenes only when justified.

---

# 20. Game Bootstrap and Services

Create a bootstrap system.

Suggested services:

* SaveService.
* LocalizationService.
* AudioService.
* SceneService.
* LevelService.
* ProgressionService.
* EconomyService.
* AnalyticsService.
* AdsService.
* AddressablesService.
* SettingsService.

Use interfaces.

Example:

```csharp
public interface ISaveService
{
    PlayerSaveData Load();
    void Save(PlayerSaveData data);
    void DeleteSave();
}
```

Avoid global static classes for complex mutable state.

A small service container or dependency-injection pattern may be used.

Do not over-engineer the project with unnecessary frameworks.

---

# 21. Core Data Models

Create strongly typed models.

Example:

```csharp
public readonly struct GridPosition
{
    public int Row { get; }
    public int Column { get; }

    public GridPosition(int row, int column)
    {
        Row = row;
        Column = column;
    }
}
```

```csharp
public enum MoveDirection
{
    Up,
    Down,
    Left,
    Right
}
```

```csharp
public enum PuzzleObjectType
{
    Cart,
    Cargo,
    Rock,
    Gate,
    Switch,
    Exit,
    DirectionTile,
    StorageSlot,
    Bridge,
    Lock
}
```

```csharp
[Serializable]
public class PuzzleObjectDefinition
{
    public string Id;
    public PuzzleObjectType Type;
    public int Row;
    public int Column;
    public int Width = 1;
    public int Height = 1;
    public MoveDirection Direction;
    public string VariantId;
    public string DestinationId;
    public bool IsLocked;
}
```

```csharp
[CreateAssetMenu(menuName = "Caravan Secrets/Level Definition")]
public class LevelDefinition : ScriptableObject
{
    public string LevelId;
    public string RegionId;
    public int LevelNumber;
    public int Rows;
    public int Columns;
    public int RecommendedMoves;
    public int RewardCoins;
    public List<PuzzleObjectDefinition> Objects;
    public List<ObjectiveDefinition> Objectives;
}
```

Improve the models where needed.

Avoid storing critical logic in generic dictionaries or arbitrary strings.

Use dedicated types for:

* Cart definitions.
* Cargo definitions.
* Gates.
* Switches.
* Objectives.
* Rewards.
* Star conditions.
* Booster effects.
* Region progression.

---

# 22. ScriptableObject Usage

Use ScriptableObjects for design-time configuration.

Suitable uses:

* Level definitions.
* Region definitions.
* Puzzle object catalogs.
* Booster definitions.
* Audio catalogs.
* Reward tables.
* Camp upgrades.
* Dialogue chapters.
* Visual themes.
* Difficulty configuration.

Do not use mutable ScriptableObjects as the player’s permanent save file.

Runtime state must be separate from design data.

Do not modify original ScriptableObject assets during gameplay.

Clone or convert them into runtime models.

---

# 23. Board System

Create a reusable board system.

The board must support:

* Variable row counts.
* Variable column counts.
* Different cell sizes.
* Board centering.
* Responsive scaling.
* Portrait mobile screens.
* Object footprints larger than one cell.
* Layered visuals.
* Board backgrounds by region.
* Object selection.
* Movement previews.
* Exit visualization.
* Storage visualization.
* Highlight states.

The board renderer must be separate from board rules.

Possible separation:

```text
BoardModel
BoardController
BoardView
BoardInputHandler
BoardAnimator
BoardValidator
```

The board logic must not depend directly on sprites or GameObjects.

---

# 24. Movement System

Implement movement through commands.

Example:

```csharp
public interface IGameCommand
{
    bool CanExecute(GameState state);
    CommandResult Execute(GameState state);
    CommandResult Undo(GameState state);
}
```

Possible commands:

* MoveCartCommand.
* MoveCargoCommand.
* ActivateSwitchCommand.
* OpenGateCommand.
* UseStorageCommand.
* UseBoosterCommand.

Movement must support:

* Collision detection.
* Grid boundaries.
* Object size.
* Direction restrictions.
* Exit logic.
* Trigger activation.
* Chained effects.
* Undo.
* Animation synchronization.

Store the logical result before or independently from the visual animation.

Do not directly change game rules from animation components.

---

# 25. Game State Machine

Implement explicit game states.

Suggested states:

* Booting.
* LoadingLevel.
* Ready.
* ProcessingInput.
* Animating.
* Paused.
* Completed.
* Failed.
* ShowingDialogue.
* ShowingReward.
* Error.

The player must not interact during:

* Level loading.
* Movement resolution.
* Critical animation sequences.
* Completion processing.
* Reward processing.

Use a controlled state machine.

Avoid scattered boolean flags such as:

* `isBusy`.
* `isMoving`.
* `canTap`.
* `isFinished`.

Use explicit state transitions.

---

# 26. Objectives System

Create an extensible objective system.

Possible objective types:

* Move all carts to exits.
* Deliver cargo to matching destinations.
* Open a final gate.
* Activate all switches.
* Recover a map fragment.
* Clear all blocked cells.
* Move a specific cart to a specific exit.
* Complete within a target move count.

Example:

```csharp
public interface ILevelObjective
{
    string ObjectiveId { get; }
    bool IsCompleted(GameState state);
    float GetProgress(GameState state);
}
```

Support multiple objectives in the same level.

The completion screen must clearly show achieved objectives.

---

# 27. Undo System

The undo system must restore all affected logical state.

Undo must restore:

* Object positions.
* Exit states.
* Gate states.
* Switch states.
* Storage slots.
* Objective progress.
* Move count.
* Triggered mechanisms.
* Temporary effects.
* Booster-related changes where allowed.

Use:

* Command history.
* State snapshots.
* Reversible commands.

Choose the approach that keeps the game reliable.

Do not implement undo as only moving a GameObject backward.

---

# 28. Restart System

Restart must:

* Restore the original level state.
* Reset move count.
* Reset temporary booster effects.
* Reset gates.
* Reset switches.
* Clear undo history.
* Reset objective progress.
* Preserve booster inventory correctly.
* Avoid granting duplicate rewards.

Ask for confirmation only when appropriate.

For early tutorial levels, immediate restart may be acceptable.

---

# 29. Level Validation

Create an editor validation system.

Validate:

* Unique object IDs.
* Grid boundaries.
* Object overlaps.
* Valid exits.
* Required destinations.
* Valid object size.
* Valid directions.
* Reachable objectives.
* Missing prefabs.
* Missing visual variants.
* Broken references.
* Duplicate level numbers.
* Invalid reward values.
* Unsupported mechanic combinations.

Display clear editor errors.

Example:

```text
Level 12: Cargo object C03 references missing destination D07.
Level 18: Cart K04 overlaps Rock R02.
Level 21: Exit is outside board bounds.
```

Do not allow invalid levels into production builds.

---

# 30. Puzzle Solver

Create a puzzle solver or validation simulator for supported level types.

Use an approach such as:

* Breadth-first search.
* A* search.
* State-space exploration.
* Heuristic search.

The solver must:

* Detect solvable and unsolvable levels.
* Avoid duplicate states.
* Support configurable search limits.
* Return a possible move sequence.
* Estimate minimum moves for simple levels.
* Run in editor or development tools.
* Avoid running during normal gameplay unless explicitly required.

Document solver limitations.

If advanced mechanics cannot be solved automatically, mark those level types as requiring manual validation.

Never falsely report that every level is solvable without actual validation.

---

# 31. Custom Level Editor

Create a custom Unity Editor Window called:

**Caravan Level Editor**

The editor must allow designers to:

* Create a new level.
* Select region.
* Set board dimensions.
* Paint cells.
* Place carts.
* Set movement direction.
* Place exits.
* Place rocks.
* Place gates.
* Place switches.
* Place cargo.
* Assign destinations.
* Place storage slots.
* Set objectives.
* Set rewards.
* Set recommended moves.
* Set tutorial steps.
* Preview the board.
* Validate the level.
* Play-test the level.
* Duplicate a level.
* Save the level asset.
* Export level data where needed.

The editor must visually display:

* Object type.
* Direction.
* Object ID.
* Destination links.
* Grid coordinates.
* Validation warnings.

Do not require editing raw JSON for normal level creation.

---

# 32. Localization

The development prompt, source code, comments, documentation, variable names, class names, and file names must be written in English.

The game interface must support:

* Arabic.
* English.

Arabic must be treated as a full first-class language.

Use Unity Localization.

Required support:

* String Tables.
* Localized assets where necessary.
* Runtime language switching.
* System language detection.
* Fallback language.
* Right-to-left layout.
* Correct Arabic shaping.
* Correct alignment.
* Proper UI mirroring.
* Arabic-compatible fonts.
* TextMeshPro Arabic support.
* Long text testing.
* Mixed Arabic and numbers.
* Proper punctuation.

Do not hard-code visible text in C# scripts or prefabs.

Use localization keys.

Suggested initial interface translations:

| English                         | Arabic                      |
| ------------------------------- | --------------------------- |
| Caravan Secrets: Puzzle Journey | أسرار القافلة: رحلة الألغاز |
| Play                            | العب                        |
| Continue                        | متابعة                      |
| New Game                        | لعبة جديدة                  |
| Journey Map                     | خريطة الرحلة                |
| Caravan Camp                    | مخيم القافلة                |
| Settings                        | الإعدادات                   |
| Restart                         | إعادة المرحلة               |
| Undo                            | تراجع                       |
| Hint                            | تلميح                       |
| Pause                           | إيقاف مؤقت                  |
| Resume                          | متابعة                      |
| Level Complete                  | اكتملت المرحلة              |
| Try Again                       | حاول مجددًا                 |
| Next Level                      | المرحلة التالية             |
| New Region Unlocked             | تم فتح منطقة جديدة          |
| Coins                           | العملات                     |
| Stars                           | النجوم                      |
| Moves                           | الحركات                     |
| Best Score                      | أفضل نتيجة                  |
| Sound                           | المؤثرات الصوتية            |
| Music                           | الموسيقى                    |
| Language                        | اللغة                       |
| Vibration                       | الاهتزاز                    |
| Privacy Policy                  | سياسة الخصوصية              |
| Map Fragment                    | جزء من الخريطة              |
| Upgrade                         | تطوير                       |
| Locked                          | مقفل                        |
| Reward                          | المكافأة                    |
| Objective                       | الهدف                       |

Language options:

* العربية
* English
* System Default

Changing language must not reset progress.

---

# 33. RTL Support

Arabic UI must be properly mirrored.

Check:

* Back buttons.
* Navigation arrows.
* Progress direction.
* Dialogue layouts.
* Currency placement.
* Icon and text ordering.
* Journey map labels.
* Settings rows.
* Reward displays.
* Confirmation dialogs.
* Level objective layouts.

Do not mirror:

* Gameplay directions.
* Puzzle board coordinates.
* Directional arrows whose meaning is part of the puzzle.
* Art that becomes logically incorrect when mirrored.

Separate UI mirroring from gameplay direction logic.

---

# 34. UI Screens

Implement the following screens.

## Splash Screen

* Game logo.
* Short caravan movement.
* Loading indicator.
* Fast startup.
* No artificial long delay.

## Main Menu

* Continue.
* Play or new game.
* Journey map.
* Caravan camp.
* Settings.
* Current coin balance.
* Current stars.
* Clean hierarchy.

## Journey Map

* Scrollable route.
* Region sections.
* Level nodes.
* Locked states.
* Unlocked states.
* Star counts.
* Current level indicator.
* Region names.
* Region unlock animation.

## Gameplay Screen

* Puzzle board.
* Level number.
* Objective display.
* Move counter.
* Undo.
* Restart.
* Pause.
* Booster buttons.
* Clear board focus.
* Responsive layout.

## Pause Screen

* Resume.
* Restart.
* Settings.
* Return to map.

## Level Completion Screen

* Stars earned.
* Rewards.
* Best move result.
* Next level.
* Return to map.
* Reward multiplier placeholder.
* Camp progress indicator.

## Caravan Camp

* Upgrade locations.
* Upgrade prices.
* Resource balance.
* Current map fragments.
* Visual camp state.
* Story triggers.

## Settings

* Language.
* Music.
* Sound effects.
* Vibration.
* Reduced animation preparation.
* Privacy policy.
* Credits.
* Reset progress.
* Confirmation dialog.

## Dialogue Screen

* Character portrait.
* Character name.
* Localized dialogue.
* Continue.
* Skip.
* Optional emotion animation.

---

# 35. UI Architecture

Separate screen logic from presentation.

Suggested pattern:

* View.
* Presenter or Controller.
* ViewModel-style state.
* Services.
* Navigation controller.

Avoid:

* Large MonoBehaviours controlling entire scenes.
* UI buttons directly changing save data.
* UI components directly controlling puzzle rules.
* Repeated navigation logic.

Create reusable UI components:

* Currency display.
* Star display.
* Localized button.
* Confirmation dialog.
* Reward panel.
* Level node.
* Booster button.
* Objective panel.
* Loading overlay.
* Error dialog.

---

# 36. Save System

Save locally:

* Current level.
* Completed levels.
* Star results.
* Best move counts.
* Coin balance.
* Map fragments.
* Booster inventory.
* Unlocked regions.
* Camp upgrade states.
* Dialogue progress.
* Tutorial completion.
* Language.
* Audio settings.
* Vibration setting.
* First-completion rewards.
* Claimed rewards.

Use versioned save data.

Example:

```csharp
[Serializable]
public class PlayerSaveData
{
    public int SaveVersion;
    public string CurrentLevelId;
    public int Coins;
    public int MapFragments;
    public List<LevelProgressData> Levels;
    public List<string> UnlockedRegions;
    public List<CampUpgradeData> CampUpgrades;
    public PlayerSettingsData Settings;
}
```

Requirements:

* Atomic save writes where possible.
* Backup save file.
* Corruption recovery.
* Migration support.
* Safe default data.
* No progress loss after updates.
* Save after meaningful progression.
* Avoid saving every frame.

Do not store sensitive information.

Prepare interfaces for future cloud save without implementing a backend now.

---

# 37. Economy

Use one main currency in the MVP:

* Coins.

Coins are earned through:

* Level completion.
* Star achievements.
* First-completion rewards.
* Region completion.
* Optional rewarded ads.
* Selected story rewards.

Coins are spent on:

* Caravan camp upgrades.
* Optional booster acquisition later.
* Cosmetic decorations later.

Map fragments remain a progression collectible, not a general-purpose currency.

Avoid:

* Excessive currencies.
* Confusing exchange systems.
* Artificial energy limits.
* Aggressive purchase pressure.
* Pay-to-win level design.

---

# 38. Ads Architecture

Do not tightly couple the game to one advertising SDK.

Create interfaces.

Example:

```csharp
public interface IRewardedAdService
{
    bool IsReady { get; }
    void Load();
    Task<RewardedAdResult> ShowAsync(string placementId);
}
```

Use a fake implementation during development.

Possible rewarded-ad placements:

* Receive one hint.
* Add temporary storage.
* Double a level reward.
* Recover after a failed attempt.
* Receive a small coin reward.

Rules:

* No ad during the first session.
* No forced ad after every level.
* No ad while the player is solving a puzzle.
* No reward before successful ad completion.
* No duplicate reward after callbacks repeat.
* Handle failed ad loading gracefully.
* Gameplay must still work without ads.

---

# 39. In-App Purchase Preparation

Do not implement purchases in the first phase unless explicitly requested.

Prepare future architecture for:

* Remove ads.
* Starter bundle.
* Booster packs.
* Cosmetic caravan decorations.
* Event pass.

Use interfaces.

Do not expose purchase logic directly inside UI buttons.

Future purchases must use official store billing systems.

---

# 40. Analytics

Create an analytics abstraction.

Track events such as:

* Game launched.
* Session started.
* Language selected.
* Tutorial started.
* Tutorial completed.
* Level started.
* Level completed.
* Level failed.
* Level restarted.
* Move count.
* Booster used.
* Hint used.
* Rewarded ad requested.
* Rewarded ad completed.
* Reward granted.
* Region unlocked.
* Camp upgraded.
* Dialogue skipped.
* Error encountered.

Do not collect personally identifiable information.

Analytics must be replaceable and disableable.

Do not place provider-specific code throughout gameplay classes.

---

# 41. Audio

Create an audio management system.

Audio categories:

* Music.
* Sound effects.
* Ambient sound.
* UI sounds.

Required sounds:

* Button tap.
* Cart movement.
* Cargo placement.
* Invalid move.
* Gate opening.
* Switch activation.
* Booster use.
* Coin collection.
* Star reveal.
* Level completion.
* Region unlock.
* Camp upgrade.
* Dialogue transition.
* Desert ambience.
* Market ambience.
* Forgotten city ambience.

Requirements:

* Independent volume controls.
* Mute options.
* Audio persistence.
* Proper app background handling.
* Audio pooling.
* No repeated overlapping sounds.
* No audio resource leaks.

---

# 42. Accessibility

Support:

* Large readable text.
* Good contrast.
* Color-blind-friendly symbols.
* Shapes in addition to colors.
* Clear selection states.
* Vibration toggle.
* Music toggle.
* Sound toggle.
* Reduced animation preparation.
* Large touch targets.
* No essential information through sound only.
* No essential information through color only.
* Clear Arabic fonts.
* Comfortable mobile text sizes.

Prepare the project for a future high-contrast mode.

---

# 43. Mobile Input

Use Unity Input System.

Support:

* Tap.
* Drag only if necessary.
* Long press only if justified.
* Back button.
* Pause.
* Touch cancellation.
* Multi-touch rejection during gameplay.

Prevent:

* Accidental repeated taps.
* Input through overlays.
* Input while paused.
* Input during movement.
* Input while reward screens are open.

The core puzzle should primarily use tapping for simplicity.

---

# 44. Camera and Resolution

Design primarily for portrait orientation.

Support:

* Different Android aspect ratios.
* Safe areas.
* Camera notches.
* Tablets.
* Low-resolution devices.
* High-density screens.

The board must scale without becoming unreadable.

Use:

* Canvas Scaler.
* Safe area handling.
* Adaptive board sizing.
* Proper anchors.
* Dynamic camera sizing where needed.

Do not rely on a single reference device.

---

# 45. Performance

Target low- and mid-range Android devices.

Requirements:

* Stable frame rate.
* Low memory usage.
* Minimal garbage collection during gameplay.
* Object pooling.
* Efficient sprites.
* Sprite atlases.
* Compressed textures.
* Asynchronous asset loading.
* Avoid unnecessary Update methods.
* Avoid LINQ in hot gameplay loops.
* Avoid runtime Resources scanning.
* Avoid expensive reflection.
* Avoid excessive physics.
* Avoid unnecessary rigid bodies.
* Cache component references.
* Profile CPU, GPU, memory, rendering, and loading.

Use 2D colliders only where necessary.

The grid puzzle logic may use mathematical collision checks instead of physics.

---

# 46. Addressables

Use Addressables for expandable content.

Suitable assets:

* Region art.
* Level assets.
* Character portraits.
* Story content.
* Audio.
* Optional event content.
* Future language assets.

Requirements:

* Clear address naming.
* Asset groups by feature or region.
* Safe fallback behavior.
* Loading indicators.
* Failure handling.
* Release build configuration.
* No missing-reference crashes.

The MVP may package all Addressables locally.

Remote content delivery can be added later.

---

# 47. Security and Permissions

Request no unnecessary Android permissions.

Do not request:

* Contacts.
* Camera.
* Microphone.
* Location.
* SMS.
* Call history.
* Broad storage access.

Use application-private storage.

Do not hard-code:

* Secret API keys.
* Private service credentials.
* Store credentials.

Separate development and production configuration.

Disable editor and cheat tools in release builds.

---

# 48. Android Build Requirements

Configure:

* Android build target.
* Android App Bundle.
* ARM64.
* IL2CPP.
* Appropriate minimum Android API level.
* Latest stable target API supported by the environment.
* Portrait orientation.
* Keystore preparation instructions.
* Development and release configurations.
* Managed stripping settings.
* Internet permission only if ads or analytics require it.
* Proper application identifier.
* Proper version code and version name.

Suggested package identifier:

```text
com.ysoft.caravansecrets
```

Allow the final identifier to be configurable.

Do not commit production keystore files or passwords.

---

# 49. Testing

Use Unity Test Framework.

## Edit Mode Tests

Test:

* Movement rules.
* Collision checks.
* Board boundaries.
* Gate logic.
* Switch logic.
* Objective completion.
* Star calculation.
* Reward calculation.
* Undo.
* Save migration.
* Level validation.
* Solver behavior.
* Booster rules.
* Region unlocking.

## Play Mode Tests

Test:

* Level loading.
* Scene transitions.
* UI buttons.
* Arabic switching.
* English switching.
* RTL layout.
* Pause and resume.
* Restart.
* Level completion.
* Save and reload.
* Camp upgrade.
* Reward screen.
* Android back button.

Create test levels specifically for edge cases.

---

# 50. Debug Tools

Create a development-only debug panel.

Functions:

* Open any level.
* Unlock all levels.
* Lock all levels.
* Add coins.
* Add map fragments.
* Add boosters.
* Reset save data.
* Complete current level.
* Toggle invulnerability from fail states.
* Switch language.
* Force RTL.
* Test all dialogs.
* Test reward callbacks.
* Display grid coordinates.
* Display object IDs.
* Display current game state.
* Display command history.
* Run level validation.
* Run solver.
* Simulate corrupted save data.

The debug panel must not be included in release builds.

Use compiler symbols or build configuration.

---

# 51. Error Handling

Handle errors explicitly.

Examples:

* Missing level asset.
* Invalid level data.
* Missing localized string.
* Missing sprite.
* Failed save.
* Corrupted save.
* Addressables load failure.
* Advertisement load failure.
* Scene load failure.
* Invalid reward callback.
* Missing audio clip.

Show user-friendly fallback messages where appropriate.

Log detailed technical information in development builds.

Do not silently fail.

Do not crash because one optional asset is missing.

---

# 52. Code Quality

Follow professional C# standards.

Requirements:

* Clear naming.
* Small focused classes.
* SOLID principles where appropriate.
* Composition over inheritance where practical.
* Interfaces for external services.
* No giant manager classes.
* No unrelated logic in MonoBehaviours.
* Avoid public mutable fields.
* Use `[SerializeField] private`.
* Use properties for controlled access.
* Use namespaces.
* Use Assembly Definitions.
* Use XML documentation for important public systems.
* Use comments for non-obvious reasoning.
* Avoid comments that only repeat code.
* Remove dead code.
* Remove unused assets.
* Avoid deprecated Unity APIs.

Use nullable reference type awareness where supported.

---

# 53. Git and Version Control

Prepare the project for Git.

Include:

* Unity `.gitignore`.
* Text serialization.
* Visible meta files.
* Clear branch guidance.
* No Library folder.
* No Temp folder.
* No Logs folder.
* No generated build folders.
* No keystore credentials.
* No local machine paths.

Commit level assets and ScriptableObjects in a merge-friendly format where possible.

---

# 54. Documentation

Create:

## README.md

Include:

* Project overview.
* Unity version.
* Required packages.
* How to open the project.
* How to run the game.
* How to build Android.
* How to create a level.
* How to validate a level.
* How to add a region.
* How to add a puzzle object.
* How to add a booster.
* How to add a language.
* How to test Arabic RTL.
* How to use debug tools.
* Known limitations.

## Architecture Documentation

Explain:

* Service initialization.
* Scene flow.
* Board model.
* Command system.
* Undo system.
* Save system.
* Level format.
* Localization.
* Analytics abstraction.
* Ads abstraction.

## Level Design Guide

Explain:

* Difficulty progression.
* Valid mechanic combinations.
* Tutorial rules.
* Star conditions.
* Reward balance.
* Validation process.
* Solver usage.
* Common design mistakes.

---

# 55. Development Stages

Build the project incrementally.

## Stage 1: Foundation

* Create Unity project.
* Configure URP if needed.
* Install required packages.
* Create Assembly Definitions.
* Create folder structure.
* Create bootstrap scene.
* Create service architecture.
* Create localization foundation.
* Create save foundation.
* Configure Android.

## Stage 2: Core Puzzle Prototype

* Create board model.
* Create grid rendering.
* Create cart object.
* Create rock.
* Create exit.
* Implement tap movement.
* Implement collision.
* Implement completion.
* Implement restart.
* Implement undo.
* Create five prototype levels.
* Add Edit Mode tests.

Do not continue until the first five levels are fully playable.

## Stage 3: Level Data and Editor

* Create ScriptableObject level format.
* Create custom level editor.
* Add validation.
* Add object IDs.
* Add destination linking.
* Add play-test button.
* Add level duplication.
* Add solver prototype.

## Stage 4: Gameplay Expansion

* Add cargo.
* Add gates.
* Add switches.
* Add storage.
* Add direction tiles.
* Add objectives.
* Add boosters.
* Add star system.
* Create 30 levels.

## Stage 5: Progression

* Create journey map.
* Create three regions.
* Create camp.
* Add five camp upgrades.
* Add map fragments.
* Add region unlocking.
* Add basic story.

## Stage 6: UI and Localization

* Complete menus.
* Complete settings.
* Complete Arabic and English.
* Test RTL.
* Add dialogue UI.
* Add completion UI.
* Add responsive layouts.

## Stage 7: Audio and Polish

* Add audio manager.
* Add music.
* Add SFX.
* Add movement effects.
* Add dust.
* Add reward animations.
* Add camp animations.
* Optimize assets.

## Stage 8: Ads and Analytics Abstraction

* Add fake ad service.
* Add analytics interfaces.
* Track core events.
* Test reward safety.
* Keep gameplay functional without providers.

## Stage 9: QA and Optimization

* Run all tests.
* Test low-end Android devices.
* Profile memory.
* Profile CPU.
* Test process interruption.
* Test save recovery.
* Test Arabic.
* Test all 30 levels.
* Validate every level.
* Confirm every level is solvable.

## Stage 10: Release Preparation

* Configure application identifier.
* Create icon.
* Create splash screen.
* Configure signing.
* Create Android App Bundle.
* Remove debug systems.
* Verify permissions.
* Add privacy screen.
* Create store screenshots.
* Create Arabic and English store descriptions.
* Run final release tests.

At the end of every stage:

* Compile.
* Run tests.
* Fix errors.
* Document changes.
* List known limitations.
* Do not claim unfinished work is complete.

---

# 56. Initial Deliverables

Produce:

1. Complete Unity project.
2. C# source code.
3. Android build configuration.
4. English and Arabic localization.
5. Five prototype levels first.
6. Thirty playable MVP levels after validation.
7. Three regions.
8. Journey map.
9. Caravan camp.
10. Five upgrades.
11. Three boosters.
12. Save system.
13. Undo and restart.
14. Level editor.
15. Level validator.
16. Solver prototype.
17. Edit Mode tests.
18. Play Mode tests.
19. Debug panel.
20. Documentation.
21. Android App Bundle-ready configuration.
22. List of incomplete optional systems.

---

# 57. Acceptance Criteria

The MVP is accepted only when:

* The Unity project opens without critical errors.
* The project compiles.
* Android build succeeds.
* The game launches on Android.
* Arabic and English work.
* RTL works correctly.
* Gameplay directions are not incorrectly mirrored.
* All 30 levels load.
* Every level has been validated.
* Every level has a confirmed solution.
* Movement works consistently.
* Collision works.
* Undo restores the complete state.
* Restart restores the original level.
* Stars are calculated correctly.
* Rewards are not duplicated.
* Progress persists after closing the game.
* Region unlocking works.
* Camp upgrades persist.
* Language settings persist.
* Audio settings persist.
* Core gameplay works offline.
* No unnecessary permissions are requested.
* Debug tools are removed from release.
* No visible text is hard-coded.
* Tests pass.
* Performance is acceptable on low- and mid-range Android devices.
* No copyrighted third-party content is included without a valid license.

---

# 58. Restrictions

Do not:

* Build the project in Kotlin.
* Use Android Studio as the main game engine.
* Create a direct clone of an existing puzzle game.
* Copy Arrow Puzzle level designs.
* Copy Parking Jam interfaces.
* Copy Screw Puzzle visuals.
* Copy commercial characters.
* Use copyrighted music.
* Add multiplayer to the MVP.
* Add a complex backend.
* Add mandatory registration.
* Add chat.
* Add forced ads after every level.
* Add an energy system that blocks gameplay.
* Build hundreds of levels before validating the first 30.
* Mix unrelated game genres.
* Store player progress in ScriptableObjects.
* Put core game logic inside animations.
* Put all systems in one manager class.
* Hard-code UI strings.
* Ignore Arabic RTL behavior.
* rely on colors alone.
* require internet for normal puzzles.
* request unnecessary permissions.
* claim all levels are solvable without testing them.
* continue development while critical compilation errors remain unresolved.

---

# 59. Final Implementation Instruction

Begin implementation with only the following:

1. Create the Unity project.
2. Configure Android.
3. Create the folder structure.
4. Configure Assembly Definitions.
5. Create the bootstrap architecture.
6. Create Arabic and English localization foundations.
7. Create the board data model.
8. Create cart, rock, and exit objects.
9. Implement movement.
10. Implement collision.
11. Implement completion.
12. Implement restart.
13. Implement undo.
14. Create five prototype levels.
15. Create automated tests for the core rules.

Do not begin:

* Camp upgrading.
* Advertising.
* In-app purchases.
* Advanced story.
* Advanced effects.
* Thirty-level production.
* Live events.
* Remote content.

until the first five levels are:

* Playable.
* Solvable.
* Tested.
* Understandable.
* Visually readable.
* Stable on Android.

At every milestone, provide:

* Files created.
* Files modified.
* Systems completed.
* Tests created.
* Compilation status.
* Known limitations.
* Next implementation step.

Do not provide only explanations or isolated code snippets.

Create and maintain the actual Unity project structure and working files.

The final objective is to build a scalable commercial foundation for:

**Caravan Secrets: Puzzle Journey — أسرار القافلة: رحلة الألغاز**

with Android as the first release platform and future support for iOS and additional platforms.
