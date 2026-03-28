# Input and Tile Selection System

> **Status**: In Design
> **Author**: User + Codex
> **Last Updated**: 2026-03-28
> **Implements Pillar**: Clean Commercial Puzzle Structure; Scoped Learnable Production

## Overview

The Input and Tile Selection System defines how player input is interpreted into
board-targeted selection and swap intent during MatchJoy gameplay. It is
responsible for validating whether gameplay input is currently legal, mapping
pointer/touch interaction onto board cells, managing transient selection state,
and emitting well-formed swap requests to the gameplay resolution layer.

This system does not decide whether a swap is ultimately valid according to
match rules; that responsibility belongs to Tile Swap and Match Resolution
System. Instead, it owns the player-facing interaction contract that turns touch
or pointer gestures into consistent, readable tile-selection behavior.

For MVP, the system must support:
- selecting a swappable tile on the active board
- choosing an adjacent swap target by gesture or second tap
- clearing selection when interaction is cancelled or illegal
- refusing gameplay input outside legal flow states

## Player Fantasy

This system supports the feeling that moving tiles is immediate, trustworthy, and
pleasant. The player should feel that the board understands their intent, reacts
quickly, and never seems to ignore or misread obvious actions.

The emotional target is responsiveness and confidence. A good selection system
makes the player feel connected to the board: taps feel crisp, drag direction is
interpreted fairly, and failed interactions are clearly rejected without making
the player feel blamed.

## Detailed Design

### Core Rules

1. This system is the authoritative owner of transient board-targeted input
   interpretation during active puzzle play.

2. Ordinary tile-selection input is legal only while Game Flow State System is in
   `Level Active`.

3. For MVP, the system must support two interaction paths for swap intent:
   - tap a tile, then tap an orthogonally adjacent target tile
   - select a tile and drag/swipe toward an orthogonally adjacent target cell

4. A tile may become selected only if all of the following are true:
   - the input occurs during legal gameplay flow
   - the targeted cell is within board bounds
   - the targeted cell is currently playable according to Board Grid State System
   - the targeted occupant is currently swappable according to board/runtime rules

5. Selecting a tile creates transient selection state only. It must not mutate
   authoritative board truth by itself.

6. When a valid adjacent target intent is formed, this system emits a swap
   request containing:
   - source cell
   - target cell
   - input origin metadata if needed for later presentation/debugging

7. This system does not decide final move acceptance. It forwards a swap request
   to Tile Swap and Match Resolution System, which accepts or rejects it.

8. If a swap request is rejected by downstream gameplay truth, selection state
   must resolve into a clear post-rejection outcome. For MVP, the preferred
   default is:
   - clear the active selection
   - allow immediate new input
   - optionally play a lightweight invalid-swap feedback response

9. If the player taps the currently selected tile again and no swap target has
   been committed, the system may clear selection.

10. If the player taps a different legal tile while one is already selected, the
    system may retarget selection to the new tile unless a valid swap request is
    already being committed.

11. Diagonal targets are not legal swap targets in MVP.

12. Non-board UI interaction must not leak into board-selection behavior. When UI
    overlays or buttons own the input focus, board input must be ignored.

13. During pauses, result screens, setup, or transitions, this system must ignore
    ordinary tile-selection input.

14. The system must remain scope-conscious for mobile readability. MVP does not
    require gesture-combo input, multi-touch puzzle interaction, or long-press
    ability casting.

### States and Transitions

The selection system uses a lightweight interaction state machine layered on top
of legal gameplay flow.

| State | Entry Condition | Exit Condition | Behavior |
|-------|----------------|----------------|----------|
| Idle | No active tile is selected and gameplay input is legal | A legal tile is selected or gameplay input becomes illegal | Waits for a valid board-targeted input |
| Tile Selected | A legal swappable tile has been selected | Selection is cleared, retargeted, or a swap request is emitted | Highlights the selected source tile and waits for adjacent target intent |
| Swap Intent Pending Commit | A valid adjacent target has been chosen and a swap request has been emitted | Downstream swap acceptance/rejection resolves | Temporarily suppresses conflicting new swap intents until the request is resolved |
| Input Suspended | Top-level flow or overlay state makes normal board input illegal | Legal gameplay input resumes | Ignores or blocks ordinary board-selection interaction |

State transition rules:

1. The system begins in `Input Suspended` until gameplay becomes legally active.
2. It enters `Idle` when Game Flow enters `Level Active` and board-targeted input
   is allowed.
3. It enters `Tile Selected` when the player selects a legal swappable tile.
4. It enters `Swap Intent Pending Commit` when the player specifies a legal
   orthogonally adjacent target and a swap request is emitted.
5. It returns to `Idle` after swap acceptance/rejection resolves, unless a later
   UX pass deliberately preserves selection.
6. It enters `Input Suspended` whenever gameplay flow leaves `Level Active` or a
   higher-priority overlay claims input focus.

### Interactions with Other Systems

1. **Game Flow State System**
   - Determines whether ordinary puzzle input is currently legal.
   - Responsibility boundary: flow owns mode legality; input owns board-targeted
     interpretation while gameplay is active.

2. **Board Grid State System**
   - Provides board bounds, playable-cell truth, occupant presence, and any
     swappability-related structural constraints.
   - Responsibility boundary: board owns what exists where; input decides what
     the player is trying to act on.

3. **Tile Swap and Match Resolution System**
   - Receives emitted swap requests and decides whether they are accepted.
   - Responsibility boundary: input forms swap intent; swap/match logic judges
     legality under gameplay rules.

4. **Core HUD and Goal Feedback System**
   - May reflect or suppress certain board-adjacent input feedback states.
   - Responsibility boundary: HUD may present helpful feedback, but input owns
     selection-state transitions.

5. **Juice Feedback System**
   - May provide polish such as selection highlight, invalid-swap shake, or drag
     response cues.
   - Responsibility boundary: juice polishes the interaction; input defines the
     interaction state machine and event timing.

## Formulas

### Orthogonal Adjacency Predicate

```text
is_orthogonally_adjacent = (abs(dx) + abs(dy) == 1)
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| dx | int | board delta | calculated | Difference in x between source and target |
| dy | int | board delta | calculated | Difference in y between source and target |
| is_orthogonally_adjacent | bool | true/false | calculated | Whether target is a legal orthogonal neighbor |

### Input Legality Predicate

```text
board_input_legal = (flow_state == Level Active) and (ui_input_focus == false)
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| flow_state | enum | top-level flow states | runtime | Current game flow state |
| ui_input_focus | bool | true/false | runtime | Whether a higher-priority UI layer currently owns input |
| board_input_legal | bool | true/false | calculated | Whether ordinary board-targeted input is currently legal |

## Edge Cases

| Scenario | Expected Behavior | Rationale |
|----------|------------------|-----------|
| Player taps a non-playable or disabled cell | No tile becomes selected | Illegal board cells should not create ambiguous state |
| Player taps a legal tile, then taps a non-adjacent tile | Selection retargets or remains on the original tile according to UX rule, but no swap request is emitted | Non-adjacent taps should not produce illegal swaps |
| Player drags slightly but not enough to form a directional adjacent target | Selection remains or clears according to current UX policy without emitting swap | Minor touch noise should not trigger accidental swaps |
| Player tries to swap diagonally | Swap request is not emitted | Diagonal swaps are not legal in MVP |
| Player input arrives during Level Session Setup | Input is ignored | Setup must not behave like active play |
| Player pauses while a tile is selected | Selection is cleared or safely suspended and no pending swap is emitted | Pause should not leave confusing stale intent state |
| A swap request is rejected | Selection returns to stable post-rejection state and player regains immediate control | Invalid swaps should feel readable, not sticky or broken |
| UI overlay appears while board input is active | Board input is suspended until overlay focus ends | UI focus must win over board selection |
| Player rapidly taps multiple tiles during an unresolved swap request | Additional swap intents are ignored or queued as disallowed in MVP | Prevents state races and contradictory input commits |

## Dependencies

| System | Direction | Nature of Dependency |
|--------|-----------|---------------------|
| Game Flow State System | This system depends on Game Flow State System | Needs top-level legality for gameplay input |
| Board Grid State System | This system depends on Board Grid State System | Needs board bounds, playable-cell truth, and occupant legality |
| Tile Swap and Match Resolution System | Tile Swap and Match Resolution System depends on this system for player-originated swap intents | Emits normalized swap requests for gameplay judgment |
| Core HUD and Goal Feedback System | Soft dependency in MVP | May display or coordinate lightweight selection-related UI cues |
| Juice Feedback System | Soft dependency in MVP | May polish selection and rejection feedback |

Dependency notes:

1. The most important legality gate comes from `Game Flow State System`.
2. The most important board contract comes from `Board Grid State System`.
3. This system should stay narrow: it interprets player intent, but it must not
   absorb swap-resolution logic.

## Tuning Knobs

| Parameter | Current Value | Safe Range | Effect of Increase | Effect of Decrease |
|-----------|--------------|------------|-------------------|-------------------|
| Drag Threshold | Moderate | Low to moderate | Makes accidental drags less likely but can make swaps feel less responsive | Makes swaps feel faster but may increase accidental input |
| Invalid Swap Feedback Strength | Light | Subtle to moderate | Makes rejected swaps clearer but risks feeling punitive | Keeps interaction calm but may reduce clarity |
| Selection Persistence After Non-Adjacent Tap | Retarget or keep original by UX policy | Clear / keep / retarget | Stronger persistence can support careful play | Lower persistence can make interaction feel simpler and lighter |
| Selection Highlight Intensity | Moderate | Subtle to strong | Improves selected-tile clarity | Reduces visual noise but may weaken target awareness |

## Visual/Audio Requirements

| Event | Visual Feedback | Audio Feedback | Priority |
|-------|----------------|---------------|----------|
| Tile selected | Clear selected-tile highlight | Optional light selection tick | High |
| Valid swap intent formed | Directionally clear response before/at commit | Optional soft confirm cue | Medium |
| Invalid swap rejected | Lightweight rejection feedback | Optional soft error cue | High |
| Selection cleared | Clean removal of highlight | None required | Medium |

## UI Requirements

| Information | Display Location | Update Frequency | Condition |
|-------------|-----------------|-----------------|-----------|
| Selected tile state | On board cell | While a tile is selected | During active selection |
| Invalid swap feedback | On affected board tiles | On rejected swap | When a swap request is rejected |
| Input disabled state | Optional via overlays / flow state UI | When gameplay input is illegal | During setup, pause, results, or blocked overlay |

## Acceptance Criteria

- [ ] The system accepts board-targeted gameplay input only during legal active-play flow.
- [ ] A legal swappable tile can be selected and highlighted.
- [ ] An orthogonally adjacent target can produce a normalized swap request.
- [ ] Diagonal or otherwise illegal targets do not emit swap requests.
- [ ] Rejected swaps return selection/input state to a stable readable state.
- [ ] Pause, setup, result, and overlay states suppress ordinary board input correctly.
- [ ] The system cleanly supports both tap-tap and drag/swipe swap intent for MVP.
- [ ] Performance: normal selection and swap-intent handling feel immediate enough for a polished mobile puzzle interaction.
- [ ] No gameplay truth is mutated by selection state alone.
- [ ] No system outside flow/input/swap contracts bypasses input legality rules.

## Open Questions

| Question | Owner | Deadline | Resolution |
|----------|-------|----------|-----------|
| After tapping a non-adjacent second legal tile, should MVP retarget selection or keep the original selection until explicit cancel? | User + future UX pass | Before implementation | Not yet decided |
| Should invalid swap feedback be purely visual in MVP, or visual plus dedicated audio cue? | User + future juice/audio pass | Before polish implementation | Not yet decided |
| What exact drag threshold feels best on target devices for mobile input? | User + future implementation tuning | During prototype tuning | Not yet decided |
| Should selection always clear after any rejected swap, or is there value in preserving source selection for rapid retry? | User + future UX pass | Before implementation | Clear-after-reject recommended for MVP, not yet fully locked |

Open question notes:

1. None of these questions block a clean MVP input contract.
2. The guiding principle is player trust: input should feel strict enough to be
   predictable and generous enough to feel responsive.
