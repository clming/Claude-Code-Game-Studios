# Level Goal and Move Limit System

> **Status**: In Design
> **Author**: User + Codex
> **Last Updated**: 2026-03-28
> **Implements Pillar**: Clean Commercial Puzzle Structure; Juicy Match-3 Satisfaction

## Overview

The Level Goal and Move Limit System defines how a level tracks player progress
against its win conditions and how move consumption constrains the puzzle loop.
It is responsible for interpreting board events as goal progress, decrementing
moves according to the approved move policy, and determining whether the level
has reached success or failure conditions.

This system sits above the board, swap, and cascade layers. It does not decide
how tiles move or how matches are found. Instead, it receives confirmed gameplay
results from those systems and converts them into level-state progress. For
MatchJoy, this system is where the player's tactical board actions become
chapter progress, star opportunities, and final win/loss outcomes.

## Player Fantasy

This system supports the feeling that every move matters and every level has a
clear purpose. The player should understand what they are trying to accomplish,
how close they are to success, and whether they still have enough moves to pull
it off.

A strong version of this system makes goals feel fair, readable, and motivating.
The player should feel tension when moves are running low, satisfaction when a
goal counter drops, and confidence that the game is judging success according to
clear rules rather than hidden exceptions.

## Detailed Design

### Core Rules

1. The Level Goal and Move Limit System is responsible for tracking level
   progress against authored goals and enforcing the authored move limit.

2. Every playable level must define one or more explicit goals. For MVP, the
   system must support at least these goal families:
   - clear jelly
   - collect specified tile colors or tile categories
   - release frozen ingredients to their explicit completion milestone

3. A level is won only when all required active goals are complete according to
   their authored completion conditions.

4. A level is lost when the player has no remaining moves and the level has not
   already satisfied all required goals.

5. For MVP frozen ingredient goals, completion occurs when the ingredient is released from its frozen or locked state during legal resolution. It does not need to exit the board afterward unless a later system explicitly changes that contract.

6. The system must distinguish between:
   - authored goal definition
   - runtime goal progress
   - final level result state
   Authored goals come from level data, runtime progress changes during play,
   and final result state determines win/loss completion.

7. The default MVP move policy is:
   - a move is consumed only when a swap is accepted as a valid match-producing move
   - rejected swaps do not consume moves
   This policy must remain explicit so downstream systems do not infer different rules.

8. Goal progress must be updated only from validated gameplay events. The system
   must not infer progress from speculative input or partially resolved board states.

9. Goal progress updates may be triggered by:
   - first-pass accepted matches
   - later cascade clears
   - obstacle or objective state transitions caused by valid board resolution
   The system must treat all of these as legitimate sources of progress when they
   are emitted by approved upstream systems.

10. Goal tracking must support multiple simultaneous goals in one level. Progress
   for one goal must not accidentally overwrite or hide the state of another.

11. The system must support level-completion timing that is consistent with the
    puzzle loop. If the final required goal is completed during a cascade after
    the last accepted move, the level still counts as a success.

12. The system must expose readable runtime progress data for UI and results
    systems, including:
   - remaining moves
   - per-goal current progress
   - per-goal completion state
   - overall level result state

13. This system must not own board mutation, match detection, gravity, or refill.
    It consumes verified events from those systems and translates them into level
    progression outcomes.

14. The system must preserve enough metadata for later star-rating and results
    systems to evaluate performance, even if the full star policy is finalized
    elsewhere.

15. The system must remain scope-conscious. For the current phase, it does not
    need to support timers, score attack modes, or alternative fail conditions
    unless explicitly introduced later.

### States and Transitions

The Level Goal and Move Limit System models the lifecycle of a single level's
progress state.

| State | Entry Condition | Exit Condition | Behavior |
|-------|----------------|----------------|----------|
| Not Started | The level has been loaded but gameplay has not yet begun | The level enters active play | Goal progress is initialized from authored data and remaining moves equal the authored move limit |
| In Progress | The player is actively making moves and the level has unresolved goals | All goals complete or moves reach zero without success | Goal counters and remaining moves update in response to validated gameplay events |
| Won Pending Finalization | All required goals are complete, but downstream result presentation has not yet finalized | The session commits a win outcome | No further failure evaluation should override the success state |
| Lost Pending Finalization | Remaining moves are zero and required goals are not all complete | The session commits a loss outcome | No additional normal move progression occurs |
| Won | The level has been finalized as a success | Session ends or transitions onward | Progress, rewards, and completion metadata may now be consumed by other systems |
| Lost | The level has been finalized as a failure | Session ends or retries | Retry and fail presentation may consume the result |

State transition rules:

1. The system begins in `Not Started` when a level session is created.
2. The system moves to `In Progress` when active play begins.
3. After every accepted move and all resulting board events, the system reevaluates goals and remaining moves.
4. If all required goals are complete at any point during legal resolution, the system moves to `Won Pending Finalization`.
5. If remaining moves reach zero before all goals are complete, the system moves to `Lost Pending Finalization`.
6. A win state takes priority over loss if the final required goal is completed during the resolution of the last accepted move.
7. Pending finalization states move to `Won` or `Lost` once the session flow commits the final result.

### Interactions with Other Systems

1. **MatchJoy Authoring System**
   - This system consumes authored goal definitions, move limit values, and result-policy metadata.
   - Data flowing in: goal types, target values, move limit, and progression-related completion metadata.
   - Responsibility boundary: authoring defines the rules of the level; Level Goal and Move Limit System tracks runtime progress against them.

2. **Tile Swap and Match Resolution System**
   - This system consumes accepted-move events and first-pass match payloads.
   - Data flowing in: accepted move confirmation, matched cells, and match metadata.
   - Responsibility boundary: swap/match decides whether a move succeeded; goal/move tracking decides how that affects progress and moves remaining.

3. **Cascade, Refill, and Determinism System**
   - This system consumes later cascade clear events and stable-complete timing.
   - Data flowing in: cascade-chain clear events, cascade depth, and stable board completion.
   - Responsibility boundary: cascade/refill progresses the board automatically; goal/move tracking interprets resulting progress and final success timing.

4. **Frozen Ingredient Objective System**
   - This system may provide objective-specific state changes that count toward release goals.
   - Data flowing in: ingredient release, unlock, or completion events.
   - Responsibility boundary: frozen ingredient logic owns ingredient behavior; Level Goal and Move Limit System counts resulting progress.

5. **Core HUD and Goal Feedback System**
   - This system provides the live counters and completion state the HUD must display.
   - Data flowing out: remaining moves, per-goal progress, per-goal completion, and final result state.
   - Responsibility boundary: goal/move tracking owns progress truth; HUD owns presentation.

6. **Results, Retry, and Star Rating System**
   - This system provides final level outcome and performance-related metadata.
   - Data flowing out: win/loss result, remaining moves at finish, and goal completion summary.
   - Responsibility boundary: Level Goal and Move Limit System decides success/failure; results/star logic decides how to present and grade that outcome.

7. **Chapter Map Progression System**
   - This system provides the final completion signal needed to unlock next content.
   - Data flowing out: completed level result and progression-safe completion state.
   - Responsibility boundary: goal/move tracking defines whether the level was won; chapter progression uses that fact.

## Formulas

### Remaining Moves Update

Under the default MVP move policy, remaining moves decrease only on accepted moves.

```text
next_remaining_moves = current_remaining_moves - accepted_move_count_delta
```

Where:
- `accepted_move_count_delta` is `1` for a successful accepted move
- `accepted_move_count_delta` is `0` for a rejected swap or non-move event

### Goal Completion Predicate

A count-based goal is complete when current progress meets or exceeds its target.

```text
goal_complete = current_progress >= target_value
```

### Level Win Predicate

```text
level_won = all_required_goals_complete == true
```

### Level Loss Predicate

```text
level_lost = remaining_moves <= 0 && level_won == false
```

## Edge Cases

| Scenario | Expected Behavior | Rationale |
|----------|------------------|-----------|
| A rejected swap occurs | No move is consumed and no goal progress changes | Rejected swaps are not successful gameplay actions under the default MVP policy |
| The last remaining move triggers a cascade that finishes the final goal | The level is treated as a win | Final success should consider the full legal resolution of the accepted move |
| A cascade advances progress on multiple goals at once | All relevant goals update in the same resolution window | Simultaneous progress must not be arbitrarily serialized into lost information |
| A goal target is already satisfied at initialization because of malformed data | Structural validation should fail before gameplay, or the level should start in a clearly flagged invalid state in development | Goals should begin from an intentional authored setup |
| Remaining moves reaches zero during an unfinished resolution chain | Loss is not finalized until legal resolution for the current accepted move finishes | The system must not cut off a valid success that resolves at the end of the move |
| One goal completes while another remains incomplete | The level stays in progress | Multi-goal levels require all required goals to be satisfied |
| A level contains only one goal type | The system still tracks progress through the same general goal framework | Single-goal and multi-goal levels should use one consistent architecture |
| A later system introduces optional or bonus goals | Required-goal evaluation must remain distinct from optional-goal reporting | Core win/loss logic should not become ambiguous |

## Dependencies

| System | Direction | Nature of Dependency |
|--------|-----------|---------------------|
| MatchJoy Authoring System | This system depends on MatchJoy Authoring System | Goals and move limits are authored there |
| Tile Swap and Match Resolution System | This system depends on Tile Swap and Match Resolution System | Accepted move and first-pass match data originate there |
| Cascade, Refill, and Determinism System | This system depends on Cascade, Refill, and Determinism System | Later cascade events affect progress timing and goal completion |
| Frozen Ingredient Objective System | This system depends on Frozen Ingredient Objective System for ingredient-specific progress events | Ingredient goals need objective-specific events |
| Core HUD and Goal Feedback System | Core HUD and Goal Feedback System depends on this system | HUD displays goal counters and moves remaining |
| Results, Retry, and Star Rating System | Results, Retry, and Star Rating System depends on this system | Final level outcome and progress summary originate here |
| Chapter Map Progression System | Chapter Map Progression System depends on this system | Winning a level is a prerequisite for progression |

Dependency notes:

1. This system closes the loop between board actions and level outcomes.
2. Its biggest trust risk is ambiguous success timing around the final move.
3. Its most important contract is that win/loss evaluation must happen after the full legal resolution of an accepted move.

## Tuning Knobs

| Parameter | Current Value | Safe Range | Effect of Increase | Effect of Decrease |
|-----------|--------------|------------|-------------------|-------------------|
| Move Limit | Per level authored | 10-40 | Makes levels more forgiving and reduces tension | Increases tension and difficulty |
| Goal Target Count | Per goal authored | Goal-dependent | Extends required effort and level duration | Makes levels easier and shorter |
| Goal Mix Complexity | Mixed goals allowed | Single to mixed | Increases variety and challenge but raises cognitive load | Simplifies player understanding but reduces variety |
| Accepted Move Policy Strictness | Accepted moves only consume moves | Locked in MVP unless intentionally changed | Stricter accepted-only policy feels fairer | Broader move consumption would increase punishment and frustration |

## Visual/Audio Requirements

| Event | Visual Feedback | Audio Feedback | Priority |
|-------|----------------|---------------|----------|
| Goal progress increases | Goal counter update or target hit emphasis | Positive progress cue | High |
| Remaining moves decrease | Move counter update with clear readability | Subtle move-consumed cue | High |
| Final goal completed | Clear success emphasis | Strong completion cue | High |
| Moves exhausted without success | Fail-state emphasis | Fail cue | High |

## UI Requirements

| Information | Display Location | Update Frequency | Condition |
|-------------|-----------------|-----------------|-----------|
| Remaining moves | In-level HUD | After each accepted move and at level start | Active play |
| Per-goal progress | In-level HUD | On relevant progress events | Active play |
| Goal completion state | In-level HUD and results | On change and at result finalization | Active play and result screens |
| Final level outcome | Results flow | Once per completed session | Win/loss finalization |

## Acceptance Criteria

- [ ] The system tracks multiple simultaneous goals without ambiguity.
- [ ] Moves are consumed only according to the explicit approved move policy.
- [ ] Goal progress updates only from validated gameplay events.
- [ ] The level is won only when all required goals are complete.
- [ ] The level is lost only when moves are exhausted and success has not already been achieved.
- [ ] Final success timing correctly handles last-move cascades.
- [ ] HUD and results systems can consume readable progress and outcome state.

## Open Questions

| Question | Owner | Deadline | Resolution |
|----------|-------|----------|-----------|
| Should some goal types count only direct player-created matches, or should all legal cascade clears count equally in MVP? | User + future balancing pass | Before advanced goal tuning | Default assumption is all legal cascade clears count |
| Do frozen ingredient goals complete on release, on exit, or on another explicit milestone in MVP? | User + future Frozen Ingredient Objective System design | Before ingredient implementation | MVP completion occurs on release from the frozen/locked state, not later board exit |
| Should bonus moves or extra-move power-ups exist in current scope, or should move count only decrease? | User + future scope decision | Before power-up scope expands | Default assumption is no bonus moves in MVP |
| Should optional secondary goals exist in the first sample, or only required goals? | User | Before content production expands | Default assumption is required goals only in MVP |



