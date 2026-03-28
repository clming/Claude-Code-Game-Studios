# Core HUD and Goal Feedback System

> **Status**: In Design
> **Author**: User + Codex
> **Last Updated**: 2026-03-28
> **Implements Pillar**: Clean Commercial Puzzle Structure; Scoped Learnable Production

## Overview

The Core HUD and Goal Feedback System defines the player-facing in-level
information layer for MatchJoy. It is responsible for showing the current move
budget, active goals, meaningful progress updates, and resolution feedback in a
way that remains readable on mobile while supporting the commercial-style match-3
session loop.

This system does not own gameplay truth. It consumes board, goal, power-up,
obstacle, and ingredient-objective state from runtime gameplay systems and
translates them into stable, understandable player-facing UI. Its purpose is to
reduce cognitive load, make objective progress unmistakable, and help the player
understand what matters now without pulling attention away from the board.

For MVP, the HUD must prioritize clarity over density. At minimum, it must make
these things obvious at all times:
- remaining moves
- active goal types
- current goal progress
- major resolution feedback when progress changes

## Player Fantasy

This system supports the feeling that the player is in control of a clean,
readable, polished puzzle session. The player should feel they always know what
the current objective is, how close they are to success, and whether a recent
move meaningfully helped or hurt their situation.

A strong HUD does not compete with the board. Instead, it acts like a calm and
reliable guide layered around the board, confirming progress at the right
moments and keeping the player oriented. The emotional target is confidence,
clarity, and momentum: the player should feel that the game communicates fairly
and rewards attention rather than hiding the state of the puzzle.

## Detailed Design

### Core Rules

1. The HUD is a presentation consumer of gameplay truth. It must never be the
   source of authority for moves, goals, progress, power-up state, or board
   outcome.

2. The in-level HUD must display the player's remaining move count at all times
   during active play.

3. The in-level HUD must display every currently active level goal in a stable,
   easy-to-scan goal panel. For MVP, the HUD must support at least these goal
   families:
   - clear jelly
   - collect target tiles
   - release frozen ingredients

4. Goal progress must update only from committed gameplay state changes. Rejected
   swaps, speculative hints, preview effects, or pre-commit animations must not
   alter displayed authoritative progress values.

5. The HUD must emphasize what remains to be done, not merely what has already
   been achieved. Goal presentation should favor remaining count or clear visual
   completion state over verbose descriptions.

6. When a move produces meaningful progress, the HUD must acknowledge it with a
   clear but lightweight feedback event. This may include:
   - decrementing remaining goal counts
   - pulsing the affected goal icon or counter
   - briefly highlighting the move counter when it changes state critically

7. When a goal is fully completed, the HUD must make that completion unmistakable
   without obscuring the board. A completed goal should remain visible long
   enough for the player to register success.

8. The move counter must visually communicate critical pressure states. At
   minimum, the HUD should distinguish between:
   - healthy remaining move budget
   - low remaining move budget
   - final move / last-chance state

9. If a level contains multiple goal types, the HUD must present them in a
   deterministic order. Goal order must not reshuffle during play.

10. Goal display must be icon-first and count-supported in MVP. Long-form text
    explanations may exist in pre-level overlays or tutorial messaging, but the
    active HUD should remain compact.

11. The HUD must not overload the player with low-priority information during
    normal play. Score, advanced analytics, hidden probabilities, or debug-only
    state must remain outside the MVP runtime HUD unless later systems require
    them.

12. When gameplay enters terminal result conditions, the HUD must freeze or hand
    off cleanly to result presentation rather than continuing to animate outdated
    counters.

13. The HUD must remain legible across the expected mobile play area and must be
    designed so the board remains the visual focus of the session.

### States and Transitions

The HUD uses a lightweight presentation lifecycle tied to the gameplay session.

| State | Entry Condition | Exit Condition | Behavior |
|-------|----------------|----------------|----------|
| Hidden / Pre-Session | Level is not yet in active play, or the board is not yet ready for player input | Level enters active play and initial state is available | HUD is not interactively presenting authoritative runtime values |
| Active Tracking | Level is in active play with valid gameplay state | Gameplay pauses, enters terminal result, or board state becomes unavailable | HUD shows moves, goals, and progress and reacts to committed updates |
| Attention Event | A goal, move, or critical session state has triggered a brief highlight-worthy update | Timed feedback ends or a higher-priority state interrupts it | HUD runs a lightweight emphasis event such as pulse, flash, or count animation |
| Suspended | Gameplay is paused, overlay-blocked, or temporarily not accepting player action | Gameplay resumes or transitions to results | HUD remains visible but suppresses misleading live feedback and time-sensitive emphasis |
| Result Hand-Off | Win or loss condition has been committed | Results screen or retry flow takes over | HUD locks its final displayed state and stops normal active-tracking reactions |

State transition rules:

1. The HUD enters `Hidden / Pre-Session` before gameplay truth is ready.
2. The HUD enters `Active Tracking` only after initial move count, goals, and
   board-related goal state are available.
3. The HUD may temporarily enter `Attention Event` whenever a committed gameplay
   change warrants emphasis.
4. The HUD enters `Suspended` when gameplay is paused or blocked by a higher
   priority overlay that could make live HUD updates misleading.
5. The HUD enters `Result Hand-Off` only after win/loss state is committed.
6. Retry or restart returns the HUD to `Hidden / Pre-Session` and then back into
   `Active Tracking` with rebuilt state.

### Interactions with Other Systems

1. **Level Goal and Move Limit System**
   - Provides the authoritative move count, goal list, remaining targets, and
     terminal goal-completion state.
   - Responsibility boundary: goal/move system owns truth; HUD owns display.

2. **Frozen Ingredient Objective System**
   - Provides release progress and release-event timing for ingredient goals.
   - Responsibility boundary: ingredient system defines completion; HUD presents
     release progress and celebratory confirmation.

3. **Obstacle and Special Tile System**
   - May provide state transitions relevant to player-facing goal progress, such
     as jelly removal milestones.
   - Responsibility boundary: obstacle state changes become HUD events only when
     they affect player-visible objective progress.

4. **Power-Up Creation and Activation System**
   - Supplies event context when major power-up outcomes cause visible progress.
   - Responsibility boundary: power-up logic defines effect; HUD decides whether
     to highlight the resulting progress change.

5. **Board Grid State System**
   - Provides contextual board/session truth indirectly through other systems and
     may expose high-level state needed to determine whether HUD should be active
     or suspended.

6. **Game Flow State System**
   - Determines when a level session begins, pauses, resumes, retries, wins, or
     loses.
   - Responsibility boundary: flow logic controls session phase; HUD reacts to it.

7. **Results, Retry, and Star Rating System**
   - Receives the final HUD-relevant session outcome state after terminal play.
   - Responsibility boundary: HUD owns in-session presentation; results system
     owns post-session presentation.

## Formulas

### Goal Remaining Count

```text
goal_remaining = max(0, goal_target - goal_progress)
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| goal_target | int | > 0 | level data/runtime goal state | Total target amount for a goal |
| goal_progress | int | 0 to goal_target+ | runtime | Current committed progress toward the goal |
| goal_remaining | int | 0 to goal_target | calculated | Player-facing remaining amount to show in HUD |

### Low-Move Warning Predicate

```text
low_move_warning = (remaining_moves <= low_move_threshold)
final_move_state = (remaining_moves == 1)
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| remaining_moves | int | 0+ | runtime | Current committed move count |
| low_move_threshold | int | 1 to authored move limit | tuning/config | Threshold for warning-state presentation |
| low_move_warning | bool | true/false | calculated | Whether HUD should show low-move pressure state |
| final_move_state | bool | true/false | calculated | Whether HUD should present final-move urgency |

## Edge Cases

| Scenario | Expected Behavior | Rationale |
|----------|------------------|-----------|
| A swap is rejected and no move is consumed | HUD does not decrement moves or animate fake progress | HUD must reflect committed truth only |
| A cascade completes a goal after the player's final accepted move | HUD shows the goal as completed and hands off to win state if level logic confirms victory | Final cascades are real resolution and must count visually |
| A level has multiple active goals with different update timings | HUD updates each goal deterministically as its committed progress changes | Mixed-goal levels must remain readable |
| A goal is completed and then later receives more irrelevant clear events | HUD keeps the goal in completed state without double celebratory spam | Completed goals should remain stable |
| A pause overlay appears during an active attention event | HUD suppresses or safely pauses the emphasis event | Presentation should not desync from session state |
| A retry occurs immediately after loss | HUD resets all displayed counts from rebuilt session truth | Retry must not carry stale counts |
| A level has only one goal type | HUD uses simpler presentation but still follows the same authoritative data rules | Single-goal levels should feel cleaner, not differently authored |
| A level has zero score display in MVP | HUD remains valid because score is not a required active-tracking element | MVP prioritizes board clarity over score clutter |

## Dependencies

| System | Direction | Nature of Dependency |
|--------|-----------|---------------------|
| Level Goal and Move Limit System | This system depends on Level Goal and Move Limit System | Needs authoritative move and goal progress data |
| Frozen Ingredient Objective System | This system depends on Frozen Ingredient Objective System | Needs progress and release-event data for ingredient goals |
| Obstacle and Special Tile System | Soft dependency in MVP | Needs jelly/obstacle-related progress events only when player-facing goals depend on them |
| Power-Up Creation and Activation System | Soft dependency in MVP | Uses effect outcomes to decide when to emphasize HUD feedback |
| Game Flow State System | This system depends on Game Flow State System | Needs session phase changes such as start, pause, win, loss, retry |
| Results, Retry, and Star Rating System | Results system depends on this system indirectly | In-session HUD must hand off final visible state cleanly |

Dependency notes:

1. The most important upstream dependency is `Level Goal and Move Limit System`.
2. This system should stay thin: it presents authoritative state but must not
   duplicate gameplay logic.
3. The board should remain visually dominant; HUD complexity must be constrained
   by mobile readability first.

## Tuning Knobs

| Parameter | Current Value | Safe Range | Effect of Increase | Effect of Decrease |
|-----------|--------------|------------|-------------------|-------------------|
| Low Move Warning Threshold | 5 | 2-8 depending on move budgets | Warns earlier and increases tension/anticipation | Warns later and keeps the HUD calmer longer |
| Goal Panel Emphasis Intensity | Moderate | Subtle to strong | Makes progress updates more noticeable but risks visual noise | Keeps HUD calm but may weaken feedback clarity |
| Goal Completion Hold Time | Short | Brief to moderate | Gives stronger completion readability | Makes completion easier to miss |
| Active Goal Slot Capacity | 3 for MVP readability | 1-4 | Supports more mixed goals but risks clutter | Keeps HUD clean but constrains level design presentation |

## Visual/Audio Requirements

| Event | Visual Feedback | Audio Feedback | Priority |
|-------|----------------|---------------|----------|
| Move count changes after accepted move | Numeric update with subtle motion emphasis | Optional light click/tick owned by juice/audio layer | High |
| Goal progress changes | Goal icon/count pulse or decrement animation | Short progress confirmation cue | High |
| Goal completes | Strong but compact completion flash/check state | Goal-complete cue | High |
| Low moves reached | Move counter enters warning presentation | Optional tension cue | Medium |
| Final move state | Move counter enters last-chance presentation | Optional last-move sting | Medium |

## UI Requirements

| Information | Display Location | Update Frequency | Condition |
|-------------|-----------------|-----------------|-----------|
| Remaining moves | Top HUD priority zone | After every accepted move and on session init | During active play |
| Goal icons and remaining counts | Goal panel near top HUD | On session init and whenever committed progress changes | During active play |
| Goal completion state | Goal panel | Immediately on completion | When a goal is satisfied |
| Major progress event confirmation | Near goal panel and/or subtle board-linked callout | On meaningful committed progress events | When progress changes materially |
| Paused/suspended session state | HUD overlay-compatible state | On pause/resume | When gameplay is suspended |

## Acceptance Criteria

- [ ] The HUD can display remaining moves and all active MVP goal types during play.
- [ ] Displayed move count changes only after accepted moves.
- [ ] Goal counts and completion state update only from committed gameplay progress.
- [ ] Multi-goal levels remain readable on mobile without obscuring the board.
- [ ] Goal completion is visually unmistakable without requiring a full-screen interruption.
- [ ] Retry/restart rebuilds HUD state cleanly from authoritative gameplay systems.
- [ ] Pause/resume does not leave HUD counters or emphasis events in misleading states.
- [ ] The HUD hands off cleanly to terminal results presentation.
- [ ] Performance: HUD updates are lightweight enough not to create visible hitching during normal gameplay resolution.
- [ ] No gameplay-critical truth exists only inside HUD code.

## Open Questions

| Question | Owner | Deadline | Resolution |
|----------|-------|----------|-----------|
| Should MVP show score at all during active play, or reserve score for results only? | User | Before HUD implementation | Not yet decided |
| Should low-move warnings use only visual state, or visual plus dedicated audio cue? | User + future juice/audio pass | Before polish implementation | Not yet decided |
| Should goal progress callouts appear only in the HUD, or also as lightweight board-adjacent floating confirmations? | User + future UX pass | Before implementation | Not yet decided |
| Is three concurrent goal slots the maximum intended MVP readability limit, or should some goal combinations collapse into grouped presentation? | User + future UX pass | Before broad mixed-goal content production | Not yet decided |

Open question notes:

1. None of the current open questions block a readable MVP HUD implementation.
2. If a choice increases visual density, the board should win. MatchJoy's HUD
   should feel supportive, not crowded.
