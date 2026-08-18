The desert road should feel long and continuous because the game represents a caravan journey across a wide desert.

Preserve the current gameplay logic, but redesign the visual road presentation.

Requirements:

1. Replace disconnected square sand patches with a continuous caravan road.
2. The main road should visually extend from the lower part of the screen toward the distant gate, oasis, or horizon.
3. Use continuous wheel tracks, compressed sand, footprints, and subtle path edges.
4. Add branch paths only where the puzzle grid requires them.
5. Keep the logical grid invisible or subtle. The road must visually communicate the valid movement cells without looking like a technical square board.
6. Use perspective:
   - Wider road near the bottom.
   - Narrower road toward the top.
   - Smaller distant decorations.
7. Keep interactive carts, rocks, gates, switches, and exits large enough for mobile touch input.
8. Do not shrink gameplay objects merely to show a larger desert.
9. Separate the visual journey scale from the logical puzzle scale.
10. For larger future levels, support:
    - Camera panning along the road.
    - Multiple connected road sections.
    - Checkpoints.
    - Smooth camera movement after completing part of the route.
11. After level completion, animate the caravan traveling farther along the road to reinforce the journey concept.
12. The road must be clearly readable against the surrounding sand, using texture, shadows, wheel marks, and subtle color contrast.
13. Do not use disconnected rectangular road tiles in the final player-facing presentation.

Create one improved desert-road level and provide screenshots before applying the design to all levels.
The current level does not yet feel like a real puzzle. It behaves like a movement demonstration because the player can move carts without meaningful planning.

Do not focus only on visual polish. Redesign one level so it contains a genuine logical challenge.

Puzzle requirements:

1. Include at least three movable carts.
2. Each cart must have a clear movement direction and a specific destination.
3. At least two carts must block or depend on each other.
4. Include one limited temporary storage space.
5. Include at least one gate or switch dependency.
6. Include at least one move that appears useful but creates a dead-end if performed too early.
7. The level must require a specific sequence of actions.
8. The player must be able to fail through poor ordering, not only through random tapping.
9. Undo must be useful.
10. The solution should require between 6 and 10 meaningful moves.
11. Validate the level with the solver if supported.
12. Provide:
   - The initial board state.
   - The intended solution sequence.
   - The minimum move count.
   - The failure condition.
   - The reason each move is necessary.

Do not add more art until this level is confirmed to be a genuine puzzle.