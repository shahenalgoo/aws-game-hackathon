export const promptLevels = `
I am using a 2d array to represent a grid (9 rows and 20 columns) which will be used to build a level in a video game.

OBJECTS:
0 = void,
1 = floor
2 = target (enemy)
3 = floor spikes - trap
4 = saw blades - trap
5 = pitfall - trap
6 = moving lasers - trap
7 = turning blades - trap
8 = extraction point (ends level)

All objects (0-8) takes 1 cell space.

FINAL VALIDATION CHECKLIST (MUST MATCH EXACTLY):
- Total Void (0): 90 cells
- Total Floor (1): 26 cells
- Total Targets (2): 23 cells
- Total Saw Blades (3): 8 cells
- Total Axe (4): 8 cells
- Total Pitfall (5): 8 cells
- Total Moving Lasers (6): 8 cells
- Total Turning Blades (7): 8 cells
- Total Extraction Point (8): 1 cell
- Total cells filled (non-zero): 90 cells

PATH DEFINITIONS:
Corridors (straight lines), Rooms (rectangles), and Dead ends (isolated paths that contain targets/bonus rooms).

PLAYER:
The player can move left, right, up and down.
The player can move freely around the grid where there are floors.
The player can dash. The dash makes the player skip a floor (typically where there are traps) and go to the one after.
The player can shoot targets.

RULES:
1. The player MUST start at row 4 column 0. The goal is for the player to reach the exit/extraction point (8) after collecting all stars. Stars are dropped by targets when killed.
2. Row 4 column 0 must always be a 1. We cannot have a trap on the starting floor.
3. If there is only one floor connecting to another floor, they cannot be connected diagonally (orthogonally).
4. Objects other than floors count towards the path. A floor will be automatically placed under them.
5. MAXIMIZE dead ends and MINIMIZE interconnections between paths. This does not mean paths should be minimized, only the interconnections.
6. Do not exceed 6 cells (in a single direction) for a corridor.
7. Make sure corridors/rooms are accessible. This means that a corridor/room cannot be all surrounded by zeros.
8. Do not place traps at the end of dead ends.
9. Other than lasers, do not place traps where corridors are turning (corners). Lasers can be placed in corners since they move along a corridor.
10. Do not place 2 laser traps in the same corridor. Make sure they are well scattered across the grid.
11. Make sure dead ends are always rewarded with at least one target. Else there would be no point to them.
12. Make sure the total amount for each object matches that in the checklist.

Example of final product:
[
  [0, 0, 0, 2, 7, 2, 0, 0, 0, 0, 0, 2, 1, 2, 0, 0, 2, 4, 1, 6],
  [0, 2, 0, 0, 1, 0, 0, 0, 0, 2, 0, 5, 0, 4, 0, 1, 5, 0, 0, 5],
  [0, 4, 1, 0, 3, 1, 0, 1, 3, 1, 0, 1, 3, 6, 0, 0, 6, 0, 0, 1],
  [0, 0, 5, 2, 6, 0, 2, 0, 8, 0, 0, 6, 0, 7, 0, 0, 3, 1, 0, 3],
  [1, 7, 1, 0, 1, 2, 0, 1, 5, 6, 4, 1, 5, 2, 1, 0, 4, 7, 0, 7],
  [0, 2, 0, 0, 0, 0, 0, 6, 1, 7, 0, 0, 0, 0, 3, 0, 1, 2, 0, 1],
  [0, 1, 2, 3, 7, 2, 0, 5, 0, 2, 1, 0, 0, 0, 1, 0, 0, 0, 0, 4],
  [0, 0, 0, 0, 0, 0, 0, 4, 0, 0, 7, 0, 0, 0, 2, 6, 5, 4, 1, 2],
  [0, 0, 0, 0, 0, 0, 0, 1, 2, 0, 2, 3, 2, 0, 0, 2, 0, 1, 2, 0]
]
You will be provided with a seed number (sn) that you must use to offset your regular patterns so that your generation is always unique.
Please make sure the amount of traps match that in the checklist or else the game will be too easy or too difficult.
Generate a new randomized path where all the rules are carefully followed. Make the grid look very different from the example.
Do not write any sentence/words. Only return the grid in the format provided in the example.
`;