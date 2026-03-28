# Cascade, Refill, and Determinism System

> **Status**: In Design
> **Author**: User + Codex
> **Last Updated**: 2026-03-28
> **Implements Pillar**: Juicy Match-3 Satisfaction; Clean Commercial Puzzle Structure

## Overview

The Cascade, Refill, and Determinism System defines what happens after an
accepted match has been converted into board mutations. It is responsible for
resolving cleared spaces, applying gravity, generating refill tiles where
allowed, and repeating this process until the board returns to a stable resting
state. It also defines the deterministic rules that make these outcomes
trustworthy, debuggable, and suitable for handcrafted level design.

This system is one of the most important feel systems in MatchJoy because it
turns a single successful swap into the ongoing chain reaction the player sees
and enjoys. Its job is not to decide whether the original swap was valid. Its
job is to take a valid first-pass clear and carry the board forward through all
subsequent empty-space resolution steps until no further automatic board changes
remain.

## Player Fantasy

This system supports the fantasy that the board is alive, juicy, and fair.
When a player makes a good move, pieces should fall naturally, refill cleanly,
and sometimes chain into satisfying cascades. Those cascades should feel lucky
in a fun way, but never arbitrary or broken.

The player should feel that the board follows understandable physical and puzzle
rules. Empty spaces should resolve in a way that feels consistent, and the game
should not appear to cheat by spawning impossible outcomes. The emotional target
is delight built on trust: cascades should feel surprising, but not suspicious.

## Detailed Design

### Core Rules

1. The Cascade, Refill, and Determinism System is responsible for resolving the
   board after an accepted match or other approved clear event has produced empty
   playable cells.

2. This system begins only after the Tile Swap and Match Resolution System has
   emitted an accepted first-pass resolution payload and the Board Grid State
   System has entered a legal mutating phase.

3. The system must resolve board progression in ordered stages:
   - apply clear results already approved by upstream systems
   - identify empty playable cells
   - apply gravity or movement rules to bring existing supported occupants into
     valid lower positions
   - spawn refill tiles into legal entry cells where the board rules allow
   - evaluate whether the resulting board now contains new automatic matches
   - repeat until the board reaches a stable state with no pending automatic resolution

4. The default MVP gravity rule is downward resolution only. A tile may move into
   the nearest reachable playable empty cell below it within the same gravity
   column unless a later topology rule explicitly changes this behavior.

5. Refill must occur only in legal spawn-entry positions determined by board
   topology and current occupancy state. Disabled cells and blocked cells must
   never be treated as valid refill targets.

6. Refill tile generation must respect the current level's allowed tile set and
   any approved spawn weighting rules supplied by authored data.

7. The system must treat one full post-clear progression as a deterministic
   cascade cycle. Each cycle must produce a well-defined sequence of:
   - pre-fall state
   - post-fall state
   - post-refill state
   - post-auto-match evaluation state

8. If refill or falling produces one or more new valid matches, those matches are
   treated as automatic cascade matches and resolved in the next cycle without
   requiring additional player input.

9. The system must stop only when the board is stable, meaning:
   - no pending legal falls remain
   - no pending legal refill spawns remain
   - no automatic post-resolution matches remain

10. Determinism requirements must be explicit. For any given board snapshot,
    clear payload, and refill input policy, the system must produce the same
    ordered resolution result. Hidden non-deterministic shortcuts are not allowed.

11. If the project uses weighted random refill, that randomness must still be
    reproducible in debug/test workflows through an approved deterministic seed or
    logged generation context.

12. This system must not silently repair impossible board states by inventing
    out-of-contract movement. If a board cannot legally resolve under the current
    rules, the failure must surface as a debug or validation problem.

13. The system must support cascade chain counting as metadata for downstream
    systems, even though it does not own scoring. At minimum, downstream systems
    should be able to know whether a resolution came from the initial swap or a
    later automatic chain step.

14. The system must remain scope-conscious for MVP. It does not yet need to own
    exotic topology such as portals, conveyors, or directional gravity shifts
    unless those features are explicitly introduced later.

### States and Transitions

The Cascade, Refill, and Determinism System models the lifecycle of one automatic
resolution sequence following an accepted clear event.

| State | Entry Condition | Exit Condition | Behavior |
|-------|----------------|----------------|----------|
| Idle | No automatic board resolution is pending | A valid clear payload or follow-up cascade match enters the system | The system is waiting for a board state that requires automatic continuation |
| Clearing Applied | A clear payload has been committed to the board | Empty-cell analysis begins | The board reflects approved removals and may contain playable empty cells |
| Resolving Fall | The board has empty cells that existing occupants can legally move into | No more legal falls remain for the current cycle | Existing occupants move according to gravity and topology rules |
| Resolving Refill | The board has legal refill entry points after falls finish | No more legal refills remain for the current cycle | New tiles are spawned into valid entry positions |
| Evaluating Auto-Matches | Falls/refills have completed for the current cycle | New cascade matches are found or the board is stable | The system checks whether automatic new matches now exist |
| Continuing Cascade | One or more auto-matches were found | The next cycle begins at Clearing Applied | The system increments cascade depth and repeats the resolution loop |
| Stable Complete | No further automatic board changes remain | The system returns to Idle | The board is once again stable and ready for the next player action |
| Invalid | A deterministic or structural resolution contract is broken | The sequence is aborted, repaired in debug flow, or the session fails | The system must surface the failure clearly rather than continue on corrupt state |

State transition rules:

1. The system begins in `Idle`.
2. A new accepted clear event moves the system to `Clearing Applied`.
3. If legal falls exist, the system moves to `Resolving Fall`; otherwise it may proceed directly to `Resolving Refill` or `Evaluating Auto-Matches` depending on board state.
4. After falls finish, the system evaluates whether refill is required and moves accordingly.
5. After refill finishes, the system enters `Evaluating Auto-Matches`.
6. If new matches are found, the system moves to `Continuing Cascade` and then re-enters `Clearing Applied` for the next chain step.
7. If no new matches remain and no further movement or refill is legal, the system moves to `Stable Complete` and then returns to `Idle`.
8. Any violation of deterministic or structural board contracts may move the system to `Invalid`.

### Interactions with Other Systems

1. **Board Grid State System**
   - This system consumes and mutates the authoritative board state during automatic resolution.
   - Data flowing in: occupancy truth, topology truth, empty-cell positions, and legal spawn targets.
   - Data flowing out: ordered mutation batches for falls, spawns, and post-cascade stable states.
   - Responsibility boundary: Board Grid State System owns board truth; Cascade, Refill, and Determinism owns how that truth evolves after clears.

2. **Tile Swap and Match Resolution System**
   - This system consumes the first accepted clear payload produced by swap/match resolution.
   - Data flowing in: matched cells, accepted swap metadata, and first-pass resolution context.
   - Data flowing out: later cascade-chain match contexts if new automatic matches occur.
   - Responsibility boundary: swap/match owns initial move validation; cascade/refill owns automatic continuation after clears.

3. **Power-Up Creation and Activation System**
   - This system may need to know whether a clear came from the initial move or a later cascade step.
   - Data flowing out: cascade depth, resolution order, and post-clear board progression context.
   - Responsibility boundary: cascade/refill does not define power-up rules, but it must preserve timing and chain metadata those rules may consume.

4. **Level Goal and Move Limit System**
   - This system provides the ongoing matched/cleared progression facts that goal tracking consumes.
   - Data flowing out: cascade-chain clear events, spawned content outcomes, and stable completion timing.
   - Responsibility boundary: cascade/refill progresses the board; goal logic interprets what that progression means for objectives.

5. **Juice Feedback System**
   - This system provides the timing backbone for fall, refill, and cascade presentation.
   - Data flowing out: fall batches, refill batches, cascade depth, and stable completion timing.
   - Responsibility boundary: cascade/refill determines the ordered logical steps; Juice Feedback determines how those steps are animated and sounded.

6. **MatchJoy Authoring System**
   - This system consumes authored tile-set and refill-weight constraints.
   - Data flowing in: allowed tile types, optional spawn weighting, and board-mode configuration.
   - Responsibility boundary: authoring declares refill constraints; cascade/refill executes them at runtime.

## Formulas

### Linear Gravity Candidate

A downward gravity step considers cells in descending order toward lower `y` values.

```text
fall_target_y < source_y
```

### Stable Board Predicate

A board is stable only when no legal falls, no legal refills, and no auto-matches remain.

```text
is_stable = no_legal_falls && no_legal_refills && auto_match_count == 0
```

### Cascade Depth

The initial accepted player match begins at depth 0. Each automatic follow-up
resolution increments cascade depth by 1.

```text
next_cascade_depth = current_cascade_depth + 1
```

## Edge Cases

| Scenario | Expected Behavior | Rationale |
|----------|------------------|-----------|
| A clear creates multiple empty cells in one column | All legal falls resolve in deterministic order before refill begins | Fall ordering must be stable and reproducible |
| A board has empty playable cells but no legal refill entries | The board remains invalid or unresolved by contract and surfaces a debug failure | The system must not invent impossible spawn behavior |
| A refill produces an immediate new match | The system records an automatic cascade step and continues resolution | Cascades are an intended outcome of refill |
| Two tiles could conceptually fall into the same target if processed ambiguously | The system uses one deterministic ordering rule and never allows double-occupancy | Board truth must stay coherent during resolution |
| A disabled cell sits inside a gravity column | Falling logic treats it as a hole in topology, not a valid occupancy destination | Non-rectangular shapes must remain consistent during gravity |
| Weighted refill randomness is enabled | The chosen result is still reproducible through seed/logged context in debug workflows | Randomness must remain debuggable |
| A cascade chain becomes unexpectedly long | The system continues until stable, while exposing cascade depth metadata for downstream consumers | Long chains are allowed and should not break state tracking |
| A post-clear board cannot reach stability because of a contract error | The system enters invalid/debug failure flow rather than looping forever | Deterministic failure is safer than silent infinite resolution |

## Dependencies

| System | Direction | Nature of Dependency |
|--------|-----------|---------------------|
| Board Grid State System | This system depends on Board Grid State System | Automatic resolution requires authoritative topology, occupancy, and legal-target queries |
| Tile Swap and Match Resolution System | This system depends on Tile Swap and Match Resolution System | The initial accepted clear payload originates there |
| MatchJoy Authoring System | This system depends on MatchJoy Authoring System | Allowed tile sets and refill constraints come from authored data |
| Level Goal and Move Limit System | Level Goal and Move Limit System depends on this system | Cascade outcomes affect goal progress and move-result interpretation |
| Power-Up Creation and Activation System | Power-Up Creation and Activation System depends on this system | Chain depth and resolution ordering matter for downstream special behavior |
| Juice Feedback System | Juice Feedback System depends on this system | Animation and sound timing follow logical cascade order |
| Obstacle and Special Tile System | Obstacle and Special Tile System depends on this system | Some obstacle reactions occur during fall/refill/clear progression |

Dependency notes:

1. This system is the bridge between one accepted match and the full juicy board reaction.
2. Its main architectural risk is mixing state mutation, randomness, and presentation timing too tightly.
3. The most important contract to preserve is deterministic logical ordering before any presentation layer interprets it.

## Tuning Knobs

| Parameter | Current Value | Safe Range | Effect of Increase | Effect of Decrease |
|-----------|--------------|------------|-------------------|-------------------|
| Allowed Tile Type Count | 5 | 4-6 | More tile types reduce accidental matches and lower cascade frequency | Fewer tile types increase cascade frequency and ease |
| Refill Weight Bias | Balanced default | All weights >= 0 and normalized | Stronger bias can support level-specific targets but may feel suspicious if overused | Lower bias feels more neutral but less intentionally shaped |
| Cascade Depth Visibility | Exposed as metadata | 0 to full chain | More tracking helps feedback/scoring systems | Less tracking simplifies implementation but weakens downstream reward hooks |
| Mutation Batch Strictness | Strict | Advisory to strict | Stricter sequencing improves debuggability | Looser sequencing risks incoherent chain behavior |

## Visual/Audio Requirements

| Event | Visual Feedback | Audio Feedback | Priority |
|-------|----------------|---------------|----------|
| Tiles falling into empty spaces | Clear downward movement readable by column | Fall or settle cue | High |
| Refill spawn | New tiles enter visibly from legal spawn direction | Refill cue | High |
| Cascade continuation | Distinct escalation from previous chain step | Cascade escalation cue | High |
| Stable board reached | Subtle completion/rest state cue | Optional settle cue | Medium |

## UI Requirements

| Information | Display Location | Update Frequency | Condition |
|-------------|-----------------|-----------------|-----------|
| Cascade depth debug value | Debug overlay or log | Per cascade step | Debug builds |
| Refill spawn events | Developer overlay/log | Per refill batch | Debug builds |
| Stable-complete event | Internal event/log | Per resolution sequence | Runtime and debug use |

## Acceptance Criteria

- [ ] The system resolves clears through ordered fall, refill, and auto-match evaluation steps.
- [ ] The board returns to a stable state only when no legal falls, refills, or auto-matches remain.
- [ ] Refill respects authored tile-set and spawn constraints.
- [ ] Cascade depth is preserved as metadata for downstream systems.
- [ ] Resolution ordering is deterministic for a given board snapshot and refill input context.
- [ ] The system does not silently invent illegal movement or spawn behavior.
- [ ] Gravity, refill, and chain continuation are kept logically separate from presentation timing.

## Open Questions

| Question | Owner | Deadline | Resolution |
|----------|-------|----------|-----------|
| Should refill randomness always use a debug-visible seed even in non-test development builds? | User + future implementation pass | Before implementation | Not yet decided |
| Should the game detect and resolve dead boards immediately after a stable state is reached, or leave that to a later dedicated system? | User + future scope decision | Before polish implementation | MVP uses immediate deterministic reshuffle after stable dead-board detection |
| Which obstacle types are allowed to interrupt, block, or redirect falling tiles in MVP scope? | User + future Obstacle and Special Tile System design | Before obstacle implementation | Not yet decided |
| Should refill spawn from a single top entry model only, or must authored board modes support multiple legal entry columns/points in MVP? | User + future authoring alignment | Before implementation | Not yet decided |

