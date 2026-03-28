# Tile Swap and Match Resolution System

> **Status**: In Design
> **Author**: User + Codex
> **Last Updated**: 2026-03-28
> **Implements Pillar**: Juicy Match-3 Satisfaction; Clean Commercial Puzzle Structure

## Overview

The Tile Swap and Match Resolution System defines how the player attempts a swap,
how the game determines whether that swap is structurally and mechanically valid,
and how newly created matches are detected and converted into board-state
mutation requests. It sits directly on top of the Board Grid State System and is
one of the most trust-sensitive systems in MatchJoy because it determines whether
a player action feels fair, readable, and responsive.

This system is responsible for match-making rules, but it is not responsible for
gravity, refill, long cascade continuation, or final scoring. Its output is a
resolved set of accepted or rejected swaps and a clear description of which board
positions should enter the next mutation phase. In other words, this system
owns the moment where player intent becomes puzzle truth.

## Player Fantasy

This system supports the core fantasy that every swipe feels intentional and
rewarding. When the player drags two neighboring tiles, the game should feel
like it immediately understands their intent, checks it against clear rules, and
either rewards them with a satisfying match or cleanly rejects the action.

The player should never feel that a legal match was ignored, that an illegal
swap succeeded mysteriously, or that a resolution happened for hidden reasons.
The emotional target is confidence and delight: valid swaps should feel crisp,
invalid swaps should feel readable, and successful matches should feel earned.

## Detailed Design

### Core Rules

1. The Tile Swap and Match Resolution System is responsible for evaluating a
   proposed tile swap between two player-selected cells and determining whether
   that swap produces at least one valid match under the current puzzle rules.

2. A swap attempt is only structurally eligible for evaluation if:
   - both coordinates are in bounds
   - both cells are playable
   - both cells currently contain swappable primary tiles
   - the two cells are orthogonally adjacent

3. Structural swap eligibility is not the same as swap success. A structurally
   eligible swap only succeeds if it creates at least one valid post-swap match.

4. The default MVP match rule is a line match of 3 or more same-type primary
   tiles in a contiguous horizontal or vertical sequence. Diagonal matches are
   not valid unless a future system explicitly extends the rule set.

5. Match detection must operate on the post-swap board arrangement, not the
   pre-swap arrangement.

6. If a structurally eligible swap does not produce any valid match, the swap is
   rejected and the board must return to its original stable state.

7. If a swap produces one or more valid matches, the swap is accepted and the
   system must emit a match resolution payload describing:
   - the accepted swap coordinates
   - all matched cells in the first resolution pass
   - the tile types involved
   - any shape metadata needed by downstream systems
   - any candidate power-up creation signals triggered by match pattern rules

8. This system must detect all valid matches produced by the accepted swap in the
   same initial post-swap board snapshot. It must not stop after finding only the
   first qualifying line if multiple simultaneous matches exist.

9. Match groups must be normalized so that overlapping or connected matched cells
   are represented deterministically. If one cell participates in more than one
   valid line, downstream systems must receive one coherent interpretation of that
   result rather than contradictory duplicate claims.

10. The system must not directly apply gravity or refill. Its job ends after the
    swap outcome and first-pass match result are determined and converted into a
    clean mutation request for downstream systems.

11. The system must not directly award score or complete goals. It may provide
    the matched-cell payload those systems consume, but scoring and goal progress
    belong to their own systems.

12. The system must support special match pattern signaling for later power-up
    logic, even if the exact power-up catalog is finalized elsewhere. At minimum,
    the payload should preserve whether a match pattern corresponds to:
    - a line of 4
    - a line of 5+
    - a T-shape or L-shape
    - a multi-line overlap

13. Invalid swaps must be readable and deterministic. If a player attempts a swap
    that is structurally illegal or mechanically unsuccessful, the outcome must be
    consistent and must not partially mutate the board.

14. The system must work against a stable board snapshot. It must not begin swap
    resolution while the board is still in a mutating state from a previous action.

15. The system must remain scope-conscious for MVP. It is not responsible for
    hint generation, AI move suggestion, autoplayer logic, or full cascade-chain
    ownership in the current phase.

### States and Transitions

The Tile Swap and Match Resolution System models the lifecycle of one player swap
attempt against a stable board snapshot.

| State | Entry Condition | Exit Condition | Behavior |
|-------|----------------|----------------|----------|
| Idle | No active swap attempt is being processed | A new player swap request is received while the board is stable | The system is ready to accept a new swap evaluation |
| Evaluating Eligibility | A swap request enters the system | The swap is classified as structurally ineligible or moves to post-swap match evaluation | The system checks in-bounds, playability, adjacency, and swappability requirements |
| Evaluating Match Result | The swap is structurally eligible | The swap is rejected or accepted | The system evaluates the post-swap board snapshot for valid matches |
| Rejected | The swap is structurally invalid or fails to create a valid match | The rejection outcome is emitted and the system returns to Idle | No board mutation is committed |
| Accepted | The swap creates one or more valid matches | A resolution payload is emitted to downstream systems and the system returns to Idle | The accepted swap and first-pass match groups are finalized |

State transition rules:

1. The system begins in `Idle`.
2. A swap may enter `Evaluating Eligibility` only if the board is currently stable.
3. If eligibility checks fail, the system moves to `Rejected` without mutating the board.
4. If eligibility checks pass, the system moves to `Evaluating Match Result` using the post-swap snapshot.
5. If no valid match is found, the system moves to `Rejected` and the board remains unchanged.
6. If one or more valid matches are found, the system moves to `Accepted` and emits a deterministic resolution payload.
7. After emitting either rejection or acceptance, the system returns to `Idle`.

### Interactions with Other Systems

1. **Board Grid State System**
   - This system consumes stable board topology and occupancy truth from the Board Grid State System.
   - Data flowing in: adjacency, playability, tile identity, and occupancy state.
   - Data flowing out: accepted swap commands and first-pass match mutation requests.
   - Responsibility boundary: Board Grid State System owns board truth; Tile Swap and Match Resolution interprets that truth to validate swaps and detect matches.

2. **Input and Tile Selection System**
   - This system receives player-selected swap candidates from the input layer.
   - Data flowing in: first selected cell, second selected cell, or equivalent swap request.
   - Data flowing out: swap accepted/rejected result suitable for UX feedback.
   - Responsibility boundary: input decides what the player tried to do; Tile Swap and Match Resolution decides whether that attempt becomes puzzle truth.

3. **Cascade, Refill, and Determinism System**
   - This system provides the initial matched-cell payload that starts the cascade pipeline.
   - Data flowing out: matched cells, accepted swap origin, and first-pass resolution payload.
   - Responsibility boundary: Tile Swap and Match Resolution ends at first-pass match resolution; cascade/refill owns post-clear continuation.

4. **Power-Up Creation and Activation System**
   - This system provides power-up candidate pattern data for downstream conversion.
   - Data flowing out: line-of-4, line-of-5+, overlap, and T/L shape signals.
   - Responsibility boundary: Tile Swap and Match Resolution detects qualifying patterns; Power-Up Creation and Activation decides what special piece is created and how it behaves.

5. **Level Goal and Move Limit System**
   - This system provides the matched-cell facts that goal and move tracking later consume.
   - Data flowing out: accepted move event and matched cell groups.
   - Responsibility boundary: Tile Swap and Match Resolution confirms whether a move succeeded; goal/move logic decides what that means for progress.

6. **Juice Feedback System**
   - This system provides outcome timing hooks for valid and invalid swaps.
   - Data flowing out: rejected swap event, accepted swap event, and match-group metadata.
   - Responsibility boundary: Tile Swap and Match Resolution determines the result; Juice Feedback decides how it feels visually and audibly.

## Formulas

### Orthogonal Swap Eligibility

A swap is structurally eligible only when the two selected cells are orthogonally adjacent.

```text
is_swap_adjacent = abs(ax - bx) + abs(ay - by) == 1
```

### Minimum Match Length

A contiguous same-type line is a valid MVP match only when its length is at least 3.

```text
is_valid_line_match = contiguous_same_type_count >= 3
```

### Swap Success Predicate

A structurally eligible swap succeeds only if at least one valid post-swap match exists.

```text
swap_success = is_structurally_eligible && matched_group_count >= 1
```

## Edge Cases

| Scenario | Expected Behavior | Rationale |
|----------|------------------|-----------|
| The player selects two non-adjacent tiles | The swap is rejected immediately | Orthogonal adjacency is a hard precondition |
| One selected cell is disabled or empty | The swap is rejected immediately | Only playable occupied cells may participate in MVP swap evaluation |
| A swap would be structurally legal but creates no match | The swap is rejected and the board remains unchanged | Standard match-3 expectation requires a productive swap |
| One accepted swap creates two simultaneous line matches | Both matches are included in the same accepted resolution payload | The system must evaluate the full post-swap snapshot, not stop at the first result |
| A tile participates in overlapping horizontal and vertical matches | The payload uses one deterministic interpretation of the overlap and preserves shape metadata | Overlaps are important for later power-up logic |
| The board begins mutating from another system while a swap request arrives | The new swap request is rejected or deferred by contract | Swap resolution requires a stable board snapshot |
| A special or blocked tile is marked non-swappable by board compatibility rules | The swap is rejected at structural eligibility stage | Swappability must be explicit and not inferred from visuals alone |
| A swap creates a line longer than 5 | The line still resolves as a valid match and emits pattern metadata for downstream systems | Long matches should not be lost just because downstream power-up behavior is defined elsewhere |

## Dependencies

| System | Direction | Nature of Dependency |
|--------|-----------|---------------------|
| Board Grid State System | This system depends on Board Grid State System | Swap and match logic require stable topology and occupancy truth |
| Input and Tile Selection System | This system depends on Input and Tile Selection System | Swap requests originate from player interaction |
| Cascade, Refill, and Determinism System | Cascade, Refill, and Determinism System depends on this system | First-pass match output starts the cascade pipeline |
| Power-Up Creation and Activation System | Power-Up Creation and Activation System depends on this system | Pattern metadata for special-piece creation originates here |
| Level Goal and Move Limit System | Level Goal and Move Limit System depends on this system | Successful move and matched-cell facts are produced here |
| Juice Feedback System | Juice Feedback System depends on this system | Presentation timing depends on valid/invalid swap results |

Dependency notes:

1. This system is the first place where player intent meets board truth.
2. Any ambiguity here will immediately damage feel, fairness, and player trust.
3. Its most important output contract is a deterministic accepted/rejected swap result plus a first-pass match payload.

## Tuning Knobs

| Parameter | Current Value | Safe Range | Effect of Increase | Effect of Decrease |
|-----------|--------------|------------|-------------------|-------------------|
| Minimum Match Length | 3 | Locked to 3 in MVP | Raising it would make the game harsher and less standard | Lowering it would break standard match-3 readability |
| Invalid Swap Strictness | Strict reject | Advisory to strict | Stricter rules increase clarity and predictability | Looser rules risk ambiguous behavior |
| Pattern Signal Granularity | 4-match, 5+-match, T/L, overlap | MVP to extended | More granularity helps downstream power-up logic but adds complexity | Less granularity simplifies implementation but weakens special-piece fidelity |

## Visual/Audio Requirements

| Event | Visual Feedback | Audio Feedback | Priority |
|-------|----------------|---------------|----------|
| Invalid swap | Clear rejection response such as swap bounce-back | Short reject cue | High |
| Valid swap with match | Immediate visual commitment into match state | Positive swap/match cue | High |
| Multi-match or overlap detection | Distinct emphasis for stronger resolution outcomes | Enhanced reward cue | Medium |

## UI Requirements

| Information | Display Location | Update Frequency | Condition |
|-------------|-----------------|-----------------|-----------|
| Selected swap pair | Board selection highlight | Per input update | During tile selection |
| Invalid swap feedback | On-board feedback | On failed swap | When a swap is rejected |
| Match group debug info | Developer overlay or log | On demand | Debug builds |

## Acceptance Criteria

- [ ] The system rejects non-adjacent, non-playable, or non-swappable swap attempts.
- [ ] A structurally eligible swap succeeds only if it creates at least one valid match.
- [ ] The system evaluates the full post-swap snapshot and captures all first-pass matches.
- [ ] Overlapping or multi-line matches are represented deterministically.
- [ ] The accepted resolution payload is sufficient for cascade, goal, and power-up systems to continue.
- [ ] Invalid swaps do not partially mutate the board.
- [ ] The system does not own gravity, refill, scoring, or final goal completion logic.

## Open Questions

| Question | Owner | Deadline | Resolution |
|----------|-------|----------|-----------|
| Should move consumption happen on accepted swaps only, or on every structurally attempted swap? | User + future Level Goal and Move Limit System design | Before move system implementation | Not yet decided |
| Which tile/content categories are swappable in MVP besides standard matchable tiles? | User + future Obstacle and Special Tile System design | Before obstacle implementation | Not yet decided |
| Should special tiles created from match patterns spawn at the origin tile, destination tile, or a deterministic priority cell? | User + Power-Up Creation and Activation System | Before implementation | Use swap destination when eligible; otherwise use the deterministic lowest-y then lowest-x priority cell |
| Should invalid swaps be rejected instantly or play a short reversible animation before returning to stable state? | User + future Juice Feedback System design | Before polish implementation | Not yet decided |

