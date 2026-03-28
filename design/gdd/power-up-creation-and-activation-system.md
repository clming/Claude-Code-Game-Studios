# Power-Up Creation and Activation System

> **Status**: In Design
> **Author**: User + Codex
> **Last Updated**: 2026-03-28
> **Implements Pillar**: Juicy Match-3 Satisfaction; Clean Commercial Puzzle Structure

## Overview

The Power-Up Creation and Activation System defines how special pieces are earned,
placed, preserved, and triggered during play. In MatchJoy, this system is
responsible for converting qualifying match patterns into power-up outcomes and
for translating later activations into board-affecting clear events that other
systems can resolve.

For the current project scope, the MVP power-up families are the classic match-3
set implied by the concept:
- line clear
- area blast
- rainbow clear

This system does not own low-level board topology, swap legality, or cascade
continuation. Instead, it sits on top of the swap/match and board-state layers,
and emits clear activation payloads back into the board-resolution pipeline.
Its main job is to make powerful outcomes feel deterministic, readable, and
worth planning around.

## Player Fantasy

This system supports the feeling that clever matches create exciting tools and
that those tools can turn a difficult board into a dramatic comeback. The player
should feel that stronger pattern recognition leads to stronger board control.

A strong version of this system makes power-ups feel intentional rather than
random. When a player earns or triggers one, they should understand why it was
created, what it will do, and why the outcome is powerful. The emotional target
is anticipation followed by payoff.

## Detailed Design

### Core Rules

1. The Power-Up Creation and Activation System is responsible for:
   - deciding when a qualifying match pattern creates a power-up
   - deciding where that created power-up is placed
   - defining the activation behavior of each supported power-up family
   - emitting activation payloads that downstream systems resolve

2. MVP power-up families are:
   - `Line Clear`: clears a full row or column depending on its orientation
   - `Area Blast`: clears a local area centered on its activation position
   - `Rainbow Clear`: clears all tiles of a chosen or linked type according to activation rules

3. Power-up creation must be driven by deterministic pattern signals from the
   Tile Swap and Match Resolution System, not by ad hoc visual heuristics.

4. The default MVP creation mapping is:
   - line of 4 -> Line Clear
   - T-shape or L-shape -> Area Blast
   - line of 5 or more -> Rainbow Clear
   If future tuning changes this mapping, the new mapping must remain explicit.

5. Power-up placement must be deterministic. The system must define one approved
   placement priority rule for created special pieces rather than relying on
   animation timing or renderer order.

6. The current default placement policy is:
   - if one of the matched cells is the player-driven swap destination, use it
   - otherwise choose the matched cell with the lowest y, then the lowest `x`
   - if a later system needs a different rule, it must replace this globally rather than per case
   This rule may be refined later, but it must remain one global contract.

7. A created power-up must replace or occupy a legal board position that remains
   valid after the originating match resolution. It must not be placed into a
   disabled, illegal, or structurally incompatible cell.

8. A power-up may be activated by:
   - being directly included in a valid player swap
   - being affected by another clear or activation according to its activation rules
   - being combined with another power-up if that combination is later enabled

9. For MVP, single-power-up activation behavior is prioritized over full combo
   explosion design. However, the system must preserve enough metadata to add
   pairwise power-up combinations later without redesigning the activation contract.

10. Activation payloads must be explicit. At minimum, activation output should
    describe:
   - activation origin cell
   - power-up type
   - orientation if applicable
   - target cells or target selection rule
   - chain source metadata if the activation came from a cascade or another power-up

11. The system must distinguish between:
   - creation event
   - dormant board presence
   - activation event
   A created power-up should persist as a board occupant until triggered or otherwise removed by approved rules.

12. Power-up activation must not bypass board contracts. It may request clears or
    transformations, but those results must still enter the board-resolution and
    cascade pipeline through explicit payloads.

13. The system must remain readable. A player should be able to infer from the
    board why a power-up exists and what broad effect it will have.

14. The system must remain scope-conscious for MVP. It does not yet need to fully
    specify every pairwise combo interaction if that would stall the core sample.
    Single activations are the required baseline; combo expansion is secondary.

### States and Transitions

The Power-Up Creation and Activation System models the lifecycle of a power-up as
board content.

| State | Entry Condition | Exit Condition | Behavior |
|-------|----------------|----------------|----------|
| Not Present | No power-up exists in the relevant board position | A qualifying creation event is emitted | The system has no special-piece state at that location |
| Created Pending Placement | A qualifying pattern has produced a power-up outcome | A legal placement position is chosen and committed | The system has decided a power-up should exist, but board placement is not yet finalized |
| Dormant On Board | A power-up exists as a board occupant and is not currently activating | It is triggered, transformed, or removed by approved board rules | The piece remains available for later tactical use |
| Activating | A power-up has been triggered and is producing its effect payload | Activation output is emitted into board resolution | The special piece is no longer a passive board occupant |
| Consumed | The activation has completed and the original dormant piece no longer exists as a separate board occupant | A new power-up is created later | The piece has been spent |

State transition rules:

1. The system begins at `Not Present` for any given board position.
2. A qualifying pattern signal moves the system into `Created Pending Placement`.
3. Once a deterministic legal position is chosen, the system moves to `Dormant On Board`.
4. A trigger event moves the system from `Dormant On Board` to `Activating`.
5. After emitting its effect payload, the power-up moves to `Consumed`.
6. Any later board event may create a new power-up instance through the same lifecycle.

### Interactions with Other Systems

1. **Tile Swap and Match Resolution System**
   - This system consumes pattern metadata produced by accepted matches.
   - Data flowing in: line-of-4, line-of-5+, T/L shape, overlap, and accepted swap context.
   - Data flowing out: creation decisions and placement requests.
   - Responsibility boundary: swap/match detects qualifying patterns; power-up logic decides the resulting special piece.

2. **Board Grid State System**
   - This system depends on legal occupancy and placement rules from the board model.
   - Data flowing in: legal cells, occupancy compatibility, and current board positions.
   - Data flowing out: dormant power-up occupancy requests and activation payload targets.
   - Responsibility boundary: board state owns placement legality; power-up logic owns what kind of special piece should exist and what it wants to affect.

3. **Cascade, Refill, and Determinism System**
   - This system passes activation results into the normal clear/cascade pipeline.
   - Data flowing out: clear targets, activation chain metadata, and triggered follow-up context.
   - Responsibility boundary: power-up logic defines what gets cleared; cascade/refill resolves what happens after that clear.

4. **Level Goal and Move Limit System**
   - This system provides extra board-clear events that goals may consume.
   - Data flowing out: activation-driven clear events and chain source context.
   - Responsibility boundary: power-up logic creates more board impact; goal/move logic interprets the resulting progress.

5. **Juice Feedback System**
   - This system provides timing hooks for creation, idle presence, and activation.
   - Data flowing out: creation event, activation event, effect strength category, and chain source metadata.
   - Responsibility boundary: power-up logic decides what happened; Juice Feedback decides how large and rewarding it feels.

6. **Obstacle and Special Tile System**
   - This system may need to know how power-up effects interact with blockers and overlays.
   - Data flowing out: activation target rules and effect categories.
   - Responsibility boundary: power-up logic defines the intended effect pattern; obstacle logic defines whether specific obstacles block, absorb, or react to that effect.

## Formulas

### Power-Up Creation Mapping

```text
if match_pattern == line_4:
    create LineClear
elif match_pattern == t_shape or match_pattern == l_shape or match_pattern == overlap_blast_equivalent:
    create AreaBlast
elif match_pattern == line_5_plus:
    create RainbowClear
```

### Deterministic Placement Priority

```text
placement_cell = preferred_swap_destination_else_pattern_priority_cell
```

### Activation Presence Predicate

```text
can_activate = power_up_exists && trigger_event_is_valid
```

## Edge Cases

| Scenario | Expected Behavior | Rationale |
|----------|------------------|-----------|
| A qualifying pattern overlaps multiple candidate placement cells | One deterministic placement rule chooses the final cell | Creation must be reproducible and readable |
| A candidate placement cell becomes illegal by board contract during resolution | The system selects the next legal deterministic candidate or fails clearly in debug flow | Power-up creation must respect board legality |
| A power-up is included in a clear before the player manually triggers it | The system follows its approved triggered-by-clear behavior rather than silently deleting it | Board reactions should stay consistent |
| Two qualifying patterns arise from the same accepted move | The system emits both outcomes deterministically if rules allow both | Strong player moves should not lose earned special outcomes arbitrarily |
| A line clear orientation is ambiguous | The system uses a deterministic orientation rule tied to creation pattern metadata | Players should not see orientation chosen randomly |
| A rainbow clear is triggered without a valid linked tile type context | The activation must fall back to an explicit approved rule or fail clearly in debug flow | Powerful effects need clear target-selection rules |
| A later feature adds power-up combinations | Existing single-activation contracts remain valid and combo rules extend them rather than replace them silently | MVP scope should scale cleanly |

## Dependencies

| System | Direction | Nature of Dependency |
|--------|-----------|---------------------|
| Tile Swap and Match Resolution System | This system depends on Tile Swap and Match Resolution System | Creation patterns originate from match results |
| Board Grid State System | This system depends on Board Grid State System | Placement legality and board occupancy truth originate there |
| Cascade, Refill, and Determinism System | Cascade, Refill, and Determinism System depends on this system | Activation payloads lead into later board resolution |
| Level Goal and Move Limit System | Level Goal and Move Limit System depends on this system | Activation clears contribute to goals and result context |
| Juice Feedback System | Juice Feedback System depends on this system | Creation and activation events drive feedback timing |
| Obstacle and Special Tile System | Obstacle and Special Tile System depends on this system | Effect interaction rules must be interpreted against obstacle behavior |

Dependency notes:

1. This system is where advanced board mastery starts to feel rewarding.
2. Its biggest risk is ambiguity in creation placement and activation targeting.
3. The most important contract to preserve is deterministic creation and activation output.

## Tuning Knobs

| Parameter | Current Value | Safe Range | Effect of Increase | Effect of Decrease |
|-----------|--------------|------------|-------------------|-------------------|
| Line Clear Frequency | Pattern-driven | Match-4 only in MVP | More frequent line clears increase spectacle and ease | Lower frequency reduces excitement and control |
| Area Blast Frequency | Pattern-driven | T/L only in MVP | More blasts increase bursty board swings | Fewer blasts reduce payoff variety |
| Rainbow Clear Frequency | Pattern-driven | Line-5+ only in MVP | More rainbow clears dramatically increase power and volatility | Fewer rainbow clears preserve difficulty |
| Placement Priority Strictness | Deterministic | Locked in MVP | Strong determinism improves learnability | Looser placement logic feels arbitrary |

## Visual/Audio Requirements

| Event | Visual Feedback | Audio Feedback | Priority |
|-------|----------------|---------------|----------|
| Power-up creation | Clear special-piece birth moment | Creation cue | High |
| Dormant special-piece presence | Readable special visual identity | Optional idle cue only if needed | High |
| Activation | Strong distinctive effect per power-up family | Strong effect cue | High |
| Rainbow clear | High-clarity board-wide emphasis | Premium reward cue | High |

## UI Requirements

| Information | Display Location | Update Frequency | Condition |
|-------------|-----------------|-----------------|-----------|
| Special-piece type readability | On board | Persistent while dormant | During gameplay |
| Activation result debug info | Developer overlay/log | On activation | Debug builds |
| Power-up creation source metadata | Developer log | On creation | Debug builds |

## Acceptance Criteria

- [ ] Qualifying match patterns create deterministic power-up outcomes.
- [ ] Power-up placement follows one explicit legal priority rule.
- [ ] Dormant power-ups persist on the board until triggered or otherwise legally removed.
- [ ] Activation emits explicit effect payloads rather than mutating the board through hidden side effects.
- [ ] MVP single-power-up behavior is fully defined for line clear, area blast, and rainbow clear.
- [ ] The system preserves enough metadata for future combo expansion.
- [ ] Creation and activation remain readable to the player.

## Open Questions

| Question | Owner | Deadline | Resolution |
|----------|-------|----------|-----------|
| Should line clear orientation always follow the match direction, or can later rules rotate it under specific conditions? | User + future tuning pass | Before implementation | Default assumption is match-direction orientation |
| If both swap origin and swap destination are legal placement candidates, should destination always win in MVP? | User | Before implementation | Yes; otherwise use lowest-y then lowest-x priority |
| Which power-up activations can be triggered by passive board clears in MVP, and which require direct swap involvement? | User + future tuning pass | Before implementation | Not yet decided |
| How much combo behavior should be defined before vertical slice versus deferred until after single-power-up behavior is stable? | User + future scope decision | Before vertical slice implementation | Not yet decided |



