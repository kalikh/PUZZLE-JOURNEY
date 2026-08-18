# Codex Repository Instructions

## Authoritative Documents

The only authoritative project documents are:

1. `Docs/PROJECT_SPEC.md`
2. `Docs/PROJECT_STATUS.md`
3. `Docs/CURRENT_TASK.md`

Ignore all files inside `Docs/Archive/`.

`Docs/PROJECT_SPEC.md` contains the complete original project requirements and must not be treated as optional background reading.

## Mandatory Reading Protocol

Before modifying any source file, scene, prefab, ScriptableObject, package, or project setting:

1. Read `Docs/PROJECT_SPEC.md` completely.
2. Read `Docs/PROJECT_STATUS.md` completely.
3. Read `Docs/CURRENT_TASK.md` completely.
4. Inspect the existing project architecture and current implementation.
5. Create or update `Docs/TASK_ANALYSIS.md`.

`Docs/TASK_ANALYSIS.md` must contain:

- A concise summary of the game vision.
- The current development stage.
- The exact current task.
- Relevant requirements copied as references by section title, not as long quotations.
- Existing systems that must be preserved.
- Files expected to change.
- Files that must not change.
- Acceptance criteria.
- Risks and possible conflicts.
- A statement confirming whether the requested work is consistent with `PROJECT_SPEC.md`.

Do not modify implementation files until `TASK_ANALYSIS.md` is complete.

## Instruction Priority

Use this priority order:

1. `AGENTS.md`
2. `Docs/CURRENT_TASK.md`
3. `Docs/PROJECT_STATUS.md`
4. `Docs/PROJECT_SPEC.md`
5. Existing implementation and comments

If instructions conflict:

- Stop implementation.
- Document the conflict in `Docs/TASK_ANALYSIS.md`.
- Do not guess.
- Do not silently choose one instruction.

## Scope Control

Implement only the task described in `Docs/CURRENT_TASK.md`.

Do not:

- Add unrelated features.
- Create additional levels unless requested.
- Refactor stable systems without necessity.
- Replace working architecture merely because another design is preferred.
- rewrite movement, collision, undo, save, localization, or level loading unless a reproducible defect requires it.
- modify archived instructions.
- claim future requirements are already implemented.

## Frozen Systems

Any system listed under “Frozen Systems” in `Docs/PROJECT_STATUS.md` must not be modified unless:

1. A reproducible bug is documented.
2. The affected files are listed.
3. The smallest possible fix is selected.
4. Existing tests are preserved or expanded.

## Pre-Implementation Report

Before coding, report:

- Documents read.
- Relevant specification sections.
- Current architecture understood.
- Exact files planned for modification.
- Tests planned.
- Features explicitly excluded from this task.

## Implementation Rules

- Preserve working systems.
- Prefer minimal, isolated changes.
- Keep puzzle logic independent from visual presentation.
- Do not hard-code visible text.
- Maintain Arabic and English localization.
- Maintain RTL correctness.
- Keep Android performance constraints.
- Do not create primitive placeholder presentation in player-facing builds unless the current task explicitly authorizes greyboxing.

## Validation

After implementation:

1. Compile the project.
2. Run relevant Edit Mode tests.
3. Run relevant Play Mode tests.
4. Validate affected levels.
5. Confirm Arabic and English behavior where relevant.
6. Confirm Android portrait layouts where relevant.
7. Update `Docs/PROJECT_STATUS.md`.

## Completion Report

The final report must include:

- Files created.
- Files modified.
- Files intentionally not modified.
- Requirements completed.
- Tests run and results.
- Build status.
- Known limitations.
- Remaining placeholders.
- Exact next recommended task.

Do not mark the task complete if any acceptance criterion in `Docs/CURRENT_TASK.md` is unmet.