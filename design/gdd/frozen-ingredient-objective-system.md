# Frozen Ingredient Objective System

> **Status**: In Design
> **Author**: User + Codex
> **Last Updated**: 2026-03-28
> **Implements Pillar**: Clean Commercial Puzzle Structure; Scoped Learnable Production

## Overview

The Frozen Ingredient Objective System defines how frozen ingredient-linked
content behaves as a player-facing level objective in MatchJoy. It specifies the
board-state contract, release conditions, runtime tracking rules, and completion
criteria for ingredient goals that begin in a frozen or locked state and become
countable toward victory once they are released.

This system exists to turn frozen ingredients from a generic obstacle-like board
element into a clean, teachable, and measurable objective family. It does not
replace the board representation owned by Board Grid State System or the generic
obstacle state owned by Obstacle and Special Tile System. Instead, it defines
how ingredient-linked content participates in goal progression and how the game
interprets state changes that matter to the player.

For MVP scope, the defining rule is simple: a frozen ingredient goal is completed
when the authored ingredient is released from its frozen or locked state during
legal gameplay resolution. It does not need to exit the board afterward unless a
future system explicitly extends the contract.

## Player Fantasy

This system supports the feeling that the player is rescuing or freeing special
objective pieces from board constraints rather than merely clearing anonymous
tiles. A good implementation makes frozen ingredients feel legible, intentional,
and satisfying to target: the player should understand what is trapped, what is
blocking it, and what action will set it free.

The desired emotional outcome is focused relief and puzzle payoff. When a frozen
ingredient is finally released, the player should feel that a specific board
problem has been solved and that visible progress toward victory has been made.
That clarity matters because frozen-ingredient goals are meant to create variety
without becoming opaque or overly simulation-heavy.

## Detailed Design

### Core Rules

1. A frozen ingredient objective is a level goal whose progress is tied to one or
   more authored ingredient-linked board objects that begin in a constrained
   state.

2. This system does not own generic board occupancy. It depends on Board Grid
   State System for coordinates, cell legality, and layer rules, and on Obstacle
   and Special Tile System for the board-side representation of frozen or locked
   ingredient-linked content.

3. Every frozen ingredient objective instance must have a stable authored
   identity inside the level data. At minimum, each instance must define:
   - ingredient objective ID unique within the level
   - board coordinate or authored spawn position
   - frozen or locked start state
   - release condition type
   - optional visual/type metadata used for display and debugging

4. For MVP, the default completion rule is:
   - an ingredient objective counts as completed when its constrained state is
     removed during valid gameplay resolution
   - it does not need to fall off the board, reach an exit, or be collected into
     inventory after release

5. A frozen ingredient objective must never be counted twice. Once marked
   released/completed, subsequent clears, moves, cascades, or board updates may
   reference its history for feedback purposes, but they must not increment goal
   progress again.

6. A frozen ingredient objective may coexist with a playable cell only if the
   underlying board-layer contract explicitly allows it. This system must obey
   the occupancy and overlay rules already defined by Board Grid State System and
   Obstacle and Special Tile System.

7. MVP release conditions must be explicit and designer-authored. At minimum,
   the system must support a simple release condition family where the ingredient
   becomes released when the covering frozen/locked state on its cell is removed
   by an approved clear payload.

8. If future content introduces more complex ingredient logic, such as movement,
   chained locks, or exit collection, those behaviors must extend this system
   deliberately rather than redefining the meaning of completion on a per-level
   basis.

9. Goal tracking must reference authored ingredient instances, not only aggregate
   counts. Runtime may display aggregate progress, but correctness must be based
   on the completion state of each authored ingredient objective.

10. Frozen ingredient objective progress must update only after the underlying
    gameplay resolution step is accepted as legal board state change. Preview,
    attempted swaps, rejected swaps, or speculative UI hints must not mark
    progress.

11. The system must remain scope-conscious for the 50-level sample. MVP frozen
    ingredient goals are intended to add board variety and objective clarity,
    not to introduce a second full puzzle layer with autonomous pathfinding or
    transport simulation.

### States and Transitions

Frozen ingredient objectives use a lightweight runtime lifecycle. These states
apply to each authored ingredient objective instance during play.

| State | Entry Condition | Exit Condition | Behavior |
|-------|----------------|----------------|----------|
| Authored-Frozen | Level initializes and the ingredient objective is spawned in its constrained state | A valid gameplay resolution removes its frozen/locked constraint | Counts as incomplete objective content and is visible as still trapped |
| Releasing | A legal board resolution step has satisfied the authored release condition | Resolution commit finalizes the release event | Used as a transient event/reporting state so goal logic, feedback, and animation can react consistently |
| Released | Release event has been committed successfully | Level ends or board is reset/retried | Counts as completed objective content and must not be counted again |
| Invalid | Initialization or runtime detects schema/state mismatch for this objective instance | Development-only recovery or level reset | Signals a content/runtime contract problem; should never be reachable in shipped content |

State transition rules:

1. Every authored frozen ingredient objective begins in `Authored-Frozen`.
2. An objective moves to `Releasing` only when the runtime receives a legal
   resolution payload proving that its authored release condition has been met.
3. An objective moves from `Releasing` to `Released` only after the board update
   is committed successfully.
4. Once `Released`, the objective is terminal for scoring/progress purposes in
   the current run.
5. Retry or restart rebuilds the objective from authored level data and returns
   it to `Authored-Frozen`.
6. Any mismatch between authored objective metadata and runtime board presence is
   a development error and may place the instance in `Invalid`.

### Interactions with Other Systems

1. **Board Grid State System**
   - Provides the legal coordinate system, playable-cell topology, and layer
     compatibility rules used by ingredient-linked content.
   - Responsibility boundary: board state owns where content exists; this system
     owns whether that content counts as objective progress.

2. **Obstacle and Special Tile System**
   - Owns the board-side representation of frozen/locked ingredient-linked
     content and the low-level state transition that removes the constraint.
   - Responsibility boundary: obstacle logic says the board state changed;
     frozen ingredient objective logic interprets whether that state change means
     an ingredient has been released.

3. **Level Goal and Move Limit System**
   - Consumes objective progress from this system and includes it in level
     completion evaluation.
   - Responsibility boundary: this system tracks per-ingredient completion;
     goal system aggregates that progress into win/loss rules.

4. **Tile Swap and Match Resolution System**
   - Supplies the accepted clear/match payloads that may contribute to release
     events.
   - Responsibility boundary: swap/match resolution defines what cells were
     legally cleared; this system uses that information to test release rules.

5. **Cascade, Refill, and Determinism System**
   - Supplies follow-up resolution steps after accepted clears, allowing release
     events to happen during cascades as well as direct swaps.
   - Responsibility boundary: cascade system advances the board; this system
     updates objective state whenever a committed step satisfies release rules.

6. **Power-Up Creation and Activation System**
   - May produce clear payloads that directly affect frozen ingredient cells.
   - Responsibility boundary: power-up logic decides what cells are affected;
     this system decides whether those affected cells release ingredient goals.

7. **Core HUD and Goal Feedback System**
   - Displays progress for ingredient objectives and surfaces release feedback.
   - Responsibility boundary: this system exposes state/progress; HUD decides how
     to present it.

## Formulas

### Frozen Ingredient Progress

Frozen ingredient goal progress is computed from authored instances rather than
from inferred board counts.

```text
released_count = count(objective_instance.state == Released)
total_count = count(all_authored_objective_instances)
progress_ratio = released_count / total_count
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| released_count | int | 0 to total_count | runtime | Number of authored ingredient objectives currently marked Released |
| total_count | int | > 0 | level data | Total authored frozen ingredient objective instances in the level |
| progress_ratio | float | 0.0 to 1.0 | calculated | UI/debug-safe normalized progress toward the ingredient goal |

**Expected output range**: 0.0 to 1.0  
**Edge case**: If `total_count` is zero for a level that declares a frozen
ingredient goal, validation must fail.

### Goal Completion Predicate

```text
goal_complete = (released_count >= required_release_count)
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| released_count | int | 0 to total_count | runtime | Current number of released ingredient objective instances |
| required_release_count | int | 1 to total_count | level data | Number of released instances required to satisfy the goal |
| goal_complete | bool | true/false | calculated | Whether the frozen ingredient goal is satisfied |

**Expected output range**: boolean  
**Edge case**: `required_release_count` must never exceed authored instance count.

## Edge Cases

| Scenario | Expected Behavior | Rationale |
|----------|------------------|-----------|
| A level declares a frozen ingredient goal but authors zero ingredient objective instances | Validation fails | Goal declarations must correspond to real authored objective content |
| Two ingredient objective instances share the same per-level ID | Validation fails | Runtime progress tracking requires stable unique authored identities |
| An ingredient objective is authored on a non-playable or disabled cell | Validation fails unless the board contract explicitly supports it | Objective content must live in a legal board location |
| An ingredient objective is present on the board but has no authored release rule | Validation fails | Completion semantics must be explicit |
| A clear payload affects cells adjacent to the ingredient but not the ingredient cell itself | The ingredient does not release unless its authored rule explicitly allows adjacency-based release | MVP release must stay readable and deterministic |
| A cascade step, not the original player swap, releases the ingredient | The ingredient counts as released normally | Cascades are valid gameplay resolution and should advance objectives |
| The ingredient's covering state is removed by a power-up effect | The ingredient counts as released if that power-up's approved clear payload includes the relevant cell | Power-up effects should integrate cleanly with objective progression |
| The ingredient is visually animated as freed before board commit succeeds | Progress does not finalize until commit succeeds | Goal state must follow committed gameplay truth |
| An already released ingredient is hit again by later clears | No additional progress is awarded | Objective instances must be counted once |
| Retry/restart occurs after some ingredients were released | All ingredient objective instances reset to authored frozen state | Level retries must rebuild from authored truth |
| A future design adds ingredient exit behavior to some levels but not others | MVP contract remains release-on-unfreeze unless the system is globally extended and documented | Avoid per-level semantic drift that confuses goal logic |

## Dependencies

| System | Direction | Nature of Dependency |
|--------|-----------|---------------------|
| Board Grid State System | This system depends on Board Grid State System | Needs legal coordinates, layer contracts, and board-state truth |
| Obstacle and Special Tile System | This system depends on Obstacle and Special Tile System | Needs the board-side frozen/locked ingredient representation and release-state change events |
| Tile Swap and Match Resolution System | This system depends on Tile Swap and Match Resolution System | Needs accepted clear payloads to evaluate release conditions |
| Cascade, Refill, and Determinism System | This system depends on Cascade, Refill, and Determinism System | Needs committed cascade-step resolution events because release may occur after the initial swap |
| Level Goal and Move Limit System | Level Goal and Move Limit System depends on this system | Goal evaluation needs released-count progress and completion state |
| Power-Up Creation and Activation System | Soft dependency in MVP | Power-up effects may contribute release payloads but do not own ingredient-goal semantics |
| Core HUD and Goal Feedback System | Core HUD and Goal Feedback System depends on this system | Needs goal-state data, counts, and event timing for presentation |

Dependency notes:

1. The most important contract is with `Obstacle and Special Tile System`,
   because that system owns the board-side constrained state that this system
   interprets.
2. The most important downstream consumer is `Level Goal and Move Limit System`,
   because frozen ingredient progress must feed directly into win/loss logic.
3. This system should remain narrow: it interprets objective semantics, not full
   obstacle simulation or board occupancy rules.

## Tuning Knobs

| Parameter | Current Value | Safe Range | Effect of Increase | Effect of Decrease |
|-----------|--------------|------------|-------------------|-------------------|
| Ingredient Objective Count Per Level | Per level authored | 1-6 for MVP | Increases tracking complexity and objective pressure | Makes objective simpler and easier to parse |
| Required Release Count | Usually equals authored count | 1 to authored instance count | Makes the goal stricter and increases level tension | Makes the goal more forgiving |
| Ingredient Visual Distinctness | Moderate-high | Readable on mobile board | Improves clarity and reduces objective confusion | Risks players missing what must be freed |
| Ingredient Constraint Density | Low-moderate | Sparse to moderate in MVP | Increases challenge and board focus on release actions | Makes the ingredient objective less central |
| Release Rule Strictness | Cell-direct only in MVP | Cell-direct only to broader future rules | Broader rules make release easier and sometimes less readable | Stricter rules make release clearer but can feel harsher |

Tuning notes:

1. Most tuning happens per level via authored instance count, placement, and
   supporting board pressure rather than through global runtime constants.
2. MVP should favor readability over novelty. Frozen ingredients should be easy
   to recognize and their release condition should be easy to explain.

## Visual/Audio Requirements

| Event | Visual Feedback | Audio Feedback | Priority |
|-------|----------------|---------------|----------|
| Frozen ingredient present at level start | Clear trapped-state presentation with readable icon/silhouette | None or subtle idle cue | High |
| Ingredient release event | Visible state break, release pulse, and clear confirmation that the objective advanced | Distinct success cue separate from generic tile clear | High |
| Ingredient goal progress update | HUD count decrement/update synchronized with board event | Short progress tick or UI confirm cue | Medium |
| Goal fully completed | Strong board-to-HUD confirmation that ingredient objective is complete | Goal completion stinger | High |

## UI Requirements

| Information | Display Location | Update Frequency | Condition |
|-------------|-----------------|-----------------|-----------|
| Frozen ingredient goal icon/type | In-level goal HUD | On level start and while goal active | When a level contains frozen ingredient objectives |
| Released count / required count | In-level goal HUD | After every committed release event | While ingredient goal is incomplete |
| Release event confirmation | Board + HUD | On each release event | When an ingredient transitions to Released |
| Goal completion state | In-level goal HUD and results summary | Immediately on goal completion and at end of level | When required release count is met |

## Acceptance Criteria

- [ ] A level can author one or more frozen ingredient objective instances with stable per-level identity.
- [ ] Runtime can distinguish between authored-but-unreleased and released ingredient objective instances.
- [ ] Frozen ingredient progress increments exactly once per authored objective instance.
- [ ] A legal release event triggered by direct clear, cascade, or power-up payload updates progress correctly.
- [ ] Rejected swaps and non-committed preview states never change ingredient progress.
- [ ] Retry/restart fully resets frozen ingredient objective progress to authored state.
- [ ] The system integrates cleanly with Level Goal and Move Limit System so ingredient goals can gate victory.
- [ ] The MVP completion rule is documented and consistent: release from frozen/locked state completes the objective.
- [ ] Performance: Per-step objective evaluation is lightweight enough that ingredient checks do not create noticeable gameplay hitches in ordinary board resolution.
- [ ] No level-specific hardcoded ingredient-goal logic is required for normal MVP content creation.

## Open Questions

| Question | Owner | Deadline | Resolution |
|----------|-------|----------|-----------|
| Should MVP support more than one frozen ingredient visual/type family, or treat all ingredient objectives as one mechanical class with cosmetic variants? | User | Before content production broadens | Not yet decided |
| Should ingredient objectives ever move independently of normal tile refill in later milestones, or remain board-stationary in MVP? | User + future scope pass | Before any post-MVP extension | MVP assumes no independent movement |
| Should any future level type require ingredient exit/collection after release, or is release-only the long-term product direction? | User + future content review | Before advanced ingredient content | Release-only for MVP; long-term undecided |
| Does any MVP obstacle type require multi-step release for ingredients, or is one explicit release event enough? | User + future obstacle tuning | Before obstacle-heavy content implementation | One explicit release event for MVP |

Open question notes:

1. None of the open questions block MVP implementation if the current release-on-
   unfreeze contract is accepted.
2. Future ingredient complexity should be added only if it directly improves the
   50-level sample rather than expanding systemic scope for its own sake.
