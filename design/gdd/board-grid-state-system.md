# Board Grid State System

> **Status**: In Design
> **Author**: User + Codex
> **Last Updated**: 2026-03-28
> **Implements Pillar**: Juicy Match-3 Satisfaction; Clean Commercial Puzzle Structure

## Overview

The Board Grid State System defines the authoritative runtime representation of a
MatchJoy puzzle board. It is responsible for how board dimensions, cells,
occupants, and board-shape constraints are modeled in memory during play. Every
other core puzzle system depends on this system because swap validation, match
resolution, cascades, goals, obstacles, and authoring all need one shared source
of truth for what exists on the board at a given moment.

This system does not decide whether a swap is legal, whether a match clears, or
how refill randomness is produced. Instead, it provides the stable board state
model those systems read and mutate through explicit rules. Its primary design
goal is clarity: at any frame of play, the game should be able to answer which
cells exist, which cells are playable, what occupies each cell, and what board
metadata is currently active.

## Player Fantasy

This system supports the feeling that the board is trustworthy, readable, and
responsive. The player should never feel that pieces are clipping into illegal
spaces, occupying ambiguous cells, or reacting according to hidden rules. A
strong version of this system makes the board feel crisp and deterministic even
when the game becomes visually busy with cascades, power-ups, and obstacles.

Although the player never sees the grid state model directly, they feel its
quality through the consistency of every move. The emotional target is trust:
when a player inspects a tile, swaps two neighbors, or watches a cascade unfold,
the board should behave as if it has one coherent internal truth.

## Detailed Design

### Core Rules

1. The Board Grid State System is the authoritative runtime source of truth for
   board topology and board occupancy during active play.

2. A board must define explicit width and height values and expose a coordinate
   convention used consistently by all dependent systems. The current default is
   an integer 2D grid addressed as `(x, y)`, where `x` increases left-to-right
   and `y` increases bottom-to-top unless a later implementation decision requires
   a different orientation. If the orientation changes, it must change everywhere.

3. A board is composed of cells. Each cell must be in exactly one board-state
   availability category:
   - Disabled: not part of the playable board shape
   - Playable Empty: part of the board shape but currently containing no primary tile
   - Playable Occupied: part of the board shape and currently containing at least one valid occupant

4. The system must distinguish board topology from board occupancy.
   - Topology answers whether a cell exists and is playable
   - Occupancy answers what currently lives in that cell
   This separation is required so that runtime systems can clear or refill cells
   without accidentally changing the board shape.

5. Each playable cell must support a controlled occupancy model rather than an
   undefined stack. The current design assumption is a layered occupancy model:
   - Base cell state: disabled or playable
   - Primary tile layer: the matchable tile currently occupying the cell, if any
   - Overlay content layer: board-affecting content such as jelly, blockers, ice,
     ingredient containers, or other obstacle/state markers that may coexist with
     a primary tile only when explicitly allowed by schema
   This system must not allow arbitrary unlimited stacking.

6. Occupancy compatibility must be explicit. If two content types can coexist in
   one cell, that compatibility must be defined by rule rather than inferred by
   implementation shortcuts.

7. The board state model must support non-rectangular playable shapes inside a
   rectangular bounds area. Disabled cells may exist within board bounds and must
   be treated as absent for gameplay purposes.

8. The board state model must support runtime queries for:
   - whether a coordinate is in bounds
   - whether a cell is playable
   - what occupies a cell
   - whether a cell is empty
   - whether two cells are orthogonally adjacent
   - whether a cell can accept a falling or spawned tile

9. The system must support deterministic mutation by dependent systems. Any
   change to the board state must be representable as explicit cell-level updates
   rather than implicit scene behavior.

10. The board state model must support initialization from authored level data
    without mutating the authored source asset. Authoring data is an input to
    board creation; runtime board state is a separate live instance.

11. The system must support temporary empty cells during normal gameplay. A cell
    becoming empty after a clear is a legal intermediate runtime state, not an error.

12. The system must not contain swap resolution logic, scoring logic, or refill
    randomness policy. It exists to represent state, not to own higher-level game
    rules that act on that state.

13. The system must be optimized for readability and correctness first. For the
    current 50-level sample, an unambiguous and debuggable board model is more
    important than pursuing an overly clever or compressed representation.

### States and Transitions

The Board Grid State System models the runtime board instance lifecycle rather
than the authored content lifecycle. These states describe what kind of board
truth currently exists in memory and what mutations are permitted.

| State | Entry Condition | Exit Condition | Behavior |
|-------|----------------|----------------|----------|
| Uninitialized | No runtime board instance exists for the active level session | Board creation is requested from authored level data | The system exposes no live board cells and rejects gameplay queries except initialization checks |
| Initializing | A new runtime board instance is being built from authored data and runtime rules | The board has completed topology setup, occupancy setup, and structural validation | Cells, layers, and board metadata are allocated and populated, but player input is not yet allowed |
| Stable | The board exists in a valid resting state with no pending mutation batch | A dependent system begins a legal mutation sequence, or the board is torn down | Queries are fully available, adjacency is stable, and dependent systems may read the board for swap, goal, and UI logic |
| Mutating | A dependent system is applying a bounded set of cell-level changes such as swap commit, clear, fall, spawn, or obstacle reaction | All changes in the current mutation batch are applied and the board returns to a valid resting state | The board remains authoritative, but mutation ordering must be explicit so that no dependent system reads half-applied truth as final truth |
| Invalid | A structural invariant has been violated in development or validation flow | The board is rebuilt, repaired in debug flow, or the session is aborted | Runtime should surface the failure clearly; gameplay must not continue on a board known to be internally inconsistent |
| Disposed | The board instance has been intentionally torn down because the level session ended or reloaded | A new initialization request creates a fresh board instance | No prior occupancy or topology state should be reused implicitly |

State transition rules:

1. Every gameplay session begins with the board in `Uninitialized`.
2. The board may move from `Uninitialized` to `Initializing` only when a valid level payload is supplied.
3. The board may move from `Initializing` to `Stable` only after topology and occupancy contracts pass structural checks.
4. The board may move from `Stable` to `Mutating` whenever a dependent system begins an approved board update sequence.
5. The board may return from `Mutating` to `Stable` only after all changes in the current batch are applied and the board is again internally coherent.
6. Any detected invariant break may move the board into `Invalid` in development or test builds.
7. A board may move from any live state to `Disposed` when the level session ends, reloads, or aborts.
8. No dependent system may treat `Mutating` as equivalent to `Stable`; if a stable snapshot is required, it must wait until mutation completes.

### Interactions with Other Systems

1. **MatchJoy Authoring System**
   - This system consumes authored board shape, placement, and board-mode data to create a runtime board instance.
   - Data flowing in: board width/height, disabled cells, initial placements, obstacle placements, and board initialization mode.
   - Data flowing out: the final runtime board contract the authoring schema must target.
   - Responsibility boundary: authoring defines the intended start state; Board Grid State System owns the live in-memory representation after initialization.

2. **Tile Swap and Match Resolution System**
   - This system provides the cell adjacency and occupancy truth used to determine whether a proposed swap is structurally legal.
   - Data flowing out: cell occupancy, adjacency, tile identity, and mutation-safe update hooks.
   - Responsibility boundary: Board Grid State System does not decide whether a legal swap should resolve into a match; it only exposes state and accepts approved mutations.

3. **Cascade, Refill, and Determinism System**
   - This system exposes empty cells, occupancy gaps, and supported spawn targets used by gravity and refill logic.
   - Data flowing out: emptiness, occupancy changes, legal drop targets, and spawnable coordinates.
   - Responsibility boundary: Board Grid State System represents the changing board; cascade/refill decides how the next valid board mutation batch is produced.

4. **Level Goal and Move Limit System**
   - This system provides the board-facing facts goal tracking needs, such as whether jelly exists on a cell or whether an ingredient-linked occupant remains present.
   - Data flowing out: occupancy and overlay state readable by goal evaluators.
   - Responsibility boundary: Board Grid State System exposes state; goal logic interprets whether that state advances a win condition.

5. **Power-Up Creation and Activation System**
   - This system exposes the tile placement and adjacency truth required for power-up creation and activation.
   - Data flowing out: tile identity, tile position, occupancy compatibility, and mutation entry points.
   - Responsibility boundary: Board Grid State System does not define power-up rules; it only represents where power-ups and affected content currently exist.

6. **Obstacle and Special Tile System**
   - This system relies on the occupancy-layer model to represent blockers, overlays, and special board content.
   - Data flowing out: supported overlay and occupant slots, compatibility rules, and mutation-safe updates.
   - Responsibility boundary: Board Grid State System defines how obstacle content can exist on the board; the obstacle system defines what that content does.

7. **Frozen Ingredient Objective System**
   - This system uses board occupancy truth to determine where objective-linked content exists and how it travels or clears.
   - Data flowing out: ingredient-related occupancy and overlay presence.
   - Responsibility boundary: Board Grid State System stores objective-linked state; frozen ingredient logic interprets objective progress from that state.

8. **Input and Tile Selection System**
   - This system consumes playability and adjacency queries to know whether a player-selected coordinate maps to a real playable cell.
   - Data flowing out: in-bounds checks, playability checks, occupancy visibility, and adjacency relations.
   - Responsibility boundary: Board Grid State System does not decide interaction UX; it supplies the structural truth interaction logic depends on.

9. **Core HUD and Goal Feedback System**
   - This system may consume readonly snapshots of board state for debug overlays, tutorial cues, or goal highlighting.
   - Data flowing out: stable board-query results and optional debug metadata.
   - Responsibility boundary: Board Grid State System is not a presentation system, but it must support trustworthy read access for presentation layers.

## Formulas

### Linear Cell Index

If a flat storage layout is used internally, a cell's linear index should be derived
consistently from grid coordinates.

```text
linear_index = y * board_width + x
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| x | int | 0 to board_width - 1 | runtime query | Horizontal cell coordinate |
| y | int | 0 to board_height - 1 | runtime query | Vertical cell coordinate |
| board_width | int | > 0 | board config | Width of the board bounds |
| linear_index | int | 0 to board_width * board_height - 1 | calculated | Flat array position for a cell |

### Orthogonal Adjacency Distance

Orthogonal adjacency must use Manhattan distance exactly equal to 1.

```text
is_orthogonally_adjacent = abs(ax - bx) + abs(ay - by) == 1
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| ax, ay | int | in board bounds | runtime query | First cell coordinate |
| bx, by | int | in board bounds | runtime query | Second cell coordinate |

### Playable Cell Predicate

A cell is playable only if it is inside bounds and not marked disabled by topology.

```text
is_playable = is_in_bounds && cell_topology != Disabled
```

## Edge Cases

| Scenario | Expected Behavior | Rationale |
|----------|------------------|-----------|
| A coordinate is inside board bounds but maps to a disabled cell | The board reports the coordinate as in bounds but not playable | Bounds and playability are different questions and must not be conflated |
| A dependent system queries a coordinate outside board bounds | The query returns a safe invalid result or explicit failure depending on call contract | Out-of-bounds access must never silently alias to another cell |
| A playable cell has no primary tile after a clear | The cell remains valid as a playable empty cell | Empty cells are normal intermediate states during cascades and refills |
| A disabled cell is accidentally assigned runtime occupancy during initialization | Structural validation fails and the board instance is rejected | Disabled cells are not legal occupancy targets |
| Two incompatible occupants are assigned to one cell | Structural validation fails or the mutation batch is rejected | Occupancy compatibility must be explicit and enforced |
| A mutation batch removes a primary tile but leaves an allowed overlay behind | The cell becomes a valid playable empty cell with retained overlay content if schema permits it | Some board effects, such as jelly, may survive tile removal |
| A mutation batch leaves a cell in an impossible half-updated state | The board must not return to Stable; the batch is rejected or the board enters Invalid in development flow | Dependent systems must not observe incoherent board truth as final state |
| A non-rectangular board contains holes inside the visible bounds | Disabled interior cells remain absent from adjacency and occupancy logic | Non-playable holes are expected in authored shapes and must behave consistently |
| A falling or spawned tile targets a disabled or incompatible cell | The mutation is rejected by board-state compatibility checks | Board truth should prevent illegal placement regardless of who requested it |
| A board is disposed and a stale reference tries to read or mutate it | The operation must fail safely rather than touching a dead board instance | Session teardown must not leave zombie state in use |
| Two systems try to mutate the board simultaneously | Mutation ordering must be serialized or rejected by contract | The board must remain authoritative and deterministic |
| A level uses overlay content on a cell type that does not allow overlays | Structural validation fails at initialization time | Layer rules belong to board truth, not to late-stage presentation fixes |

## Dependencies

| System | Direction | Nature of Dependency |
|--------|-----------|---------------------|
| MatchJoy Authoring System | MatchJoy Authoring System depends on this system | Authoring must target the same topology, coordinate, and occupancy contract used at runtime |
| Tile Swap and Match Resolution System | Tile Swap and Match Resolution System depends on this system | Swap validation and match scanning require authoritative adjacency and tile placement truth |
| Cascade, Refill, and Determinism System | Cascade, Refill, and Determinism System depends on this system | Gravity and refill require stable empty-cell and placement-target information |
| Level Goal and Move Limit System | Level Goal and Move Limit System depends on this system | Goal progress depends on authoritative board content state |
| Power-Up Creation and Activation System | Power-Up Creation and Activation System depends on this system | Power-up placement and activation operate on tile positions and compatibility rules |
| Obstacle and Special Tile System | Obstacle and Special Tile System depends on this system | Obstacle behaviors require a defined overlay and occupancy contract |
| Frozen Ingredient Objective System | Frozen Ingredient Objective System depends on this system | Objective content must live in a coherent board representation |
| Input and Tile Selection System | Input and Tile Selection System depends on this system | Player interaction needs in-bounds, playability, and adjacency queries |
| Core HUD and Goal Feedback System | Core HUD and Goal Feedback System depends on this system indirectly | Debug overlays, hints, and highlights may consume readonly board state |

Dependency notes:

1. This is one of the earliest and most central systems in the project.
2. Any ambiguity left here will propagate into swap, cascade, obstacle, and authoring work.
3. The main contract to protect is the separation between topology truth and occupancy truth.

## Tuning Knobs

| Parameter | Current Value | Safe Range | Effect of Increase | Effect of Decrease |
|-----------|--------------|------------|-------------------|-------------------|
| Board Width | 9 | 7-10 | Increases horizontal scan space and combo opportunities | Reduces complexity and increases readability |
| Board Height | 9 | 7-10 | Increases vertical cascade depth and refill complexity | Reduces cascade depth and speeds readability |
| Overlay Layer Capacity | 1 controlled overlay layer | 0-1 in MVP | Allows richer cell state interactions but increases complexity | Simplifies state logic but reduces obstacle expressiveness |
| Disabled Cell Density | Per level authored | Low to moderate in MVP | Increases board-shape variety and puzzle specificity | Makes boards more regular and easier to reason about |
| Mutation Batch Strictness | Strict | Advisory to strict | Stricter batching prevents half-state bugs but may slow experimentation | Looser batching speeds prototyping but risks incoherent state |

## Visual/Audio Requirements

| Event | Visual Feedback | Audio Feedback | Priority |
|-------|----------------|---------------|----------|
| Invalid cell selection | Optional debug highlight in development builds | None required | Low |
| Board initialization complete | Stable board presentation with no illegal overlaps | None required | Medium |
| Invalid board invariant detected | Debug overlay or log marker in development builds | Optional debug alert only in development | High |
| Cell becomes empty after clear | Tile removal is visually readable and leaves the cell state understandable | Handled by downstream juice systems | Medium |

## UI Requirements

| Information | Display Location | Update Frequency | Condition |
|-------------|-----------------|-----------------|-----------|
| Board debug coordinates | Developer-only overlay | On demand | Debug builds or editor tools |
| Playable vs disabled cells | Authoring/debug visualization | On demand | Debug tools and validation views |
| Occupancy layer breakdown | Inspector/debug panel | On demand | Development and test workflows |
| Invalid state diagnostics | Console, inspector, or debug HUD | On failure | Development builds only |

## Acceptance Criteria

- [ ] The board exposes one authoritative runtime representation of topology and occupancy.
- [ ] The board can represent disabled cells, empty playable cells, and occupied playable cells.
- [ ] The board supports non-rectangular playable layouts within rectangular bounds.
- [ ] The board can be initialized from authored level data without mutating authored assets.
- [ ] Dependent systems can query adjacency, occupancy, emptiness, and playability without guessing.
- [ ] The occupancy model makes layer compatibility explicit.
- [ ] No swap, scoring, or refill-policy logic is hidden inside this system.

## Open Questions

| Question | Owner | Deadline | Resolution |
|----------|-------|----------|-----------|
| Should runtime coordinates be standardized as bottom-left origin or top-left origin for all board logic and tools? | User + Codex | Before implementation | Locked to bottom-left origin for MVP |
| Which obstacle/content types are allowed to coexist with a primary tile as overlay content in MVP scope? | User + future Obstacle and Special Tile System design | Before obstacle implementation | Not yet decided |
| Should cell occupancy be implemented as fixed typed fields or a small tagged-layer container? | User + future implementation pass | Before implementation | Not yet decided |
| Does the sample need support for portals, conveyors, or other nonstandard topology links, or should adjacency remain purely orthogonal in current scope? | User | Before leaving MVP design | Default assumption is no exotic topology in current scope |





