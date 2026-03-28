# Results, Retry, and Star Rating System

> **Status**: In Design
> **Author**: User + Codex
> **Last Updated**: 2026-03-28
> **Implements Pillar**: Clean Commercial Puzzle Structure; Scoped Learnable Production

## Overview

The Results, Retry, and Star Rating System defines how MatchJoy ends a level
session, evaluates outcome quality, presents win/loss results, and supports
clean retry flow. It is responsible for translating authoritative gameplay state
into a readable end-of-level conclusion that confirms success or failure,
communicates performance, and routes the player into the next sensible action.

This system does not determine whether gameplay goals are truly complete; that
truth comes from Level Goal and Move Limit System and related runtime systems.
Instead, it owns the post-session interpretation layer: when a session ends,
what result category is shown, how star rating is assigned, what retry means,
and how the player moves onward to the map or back into the same level.

For MVP, the system must support three core result outcomes:
- victory
- failure by move exhaustion
- retry / replay from fresh authored state

It must also support a simple star-rating model that remains readable,
deterministic, and compatible with chapter progression.

## Player Fantasy

This system supports the feeling that every level has a clear payoff. A win
should feel earned, legible, and rewarding; a loss should feel fair and quickly
recoverable. The player should always understand why the session ended and what
they can do next.

The emotional target is closure and momentum. On victory, the player should feel
that the game recognizes their success and gives them a satisfying step forward.
On failure, the game should encourage immediate re-engagement instead of shame or
confusion. A good retry flow keeps the player in motion.

## Detailed Design

### Core Rules

1. This system consumes authoritative session outcome state from gameplay systems.
   It must not infer victory or failure from presentation timing alone.

2. A level session may enter result presentation only after win/loss state has
   been committed by gameplay truth.

3. MVP session outcomes must include at minimum:
   - `Victory`
   - `Failure`
   - `Retry Requested`

4. `Victory` occurs when all required level goals are satisfied according to the
   Level Goal and Move Limit System, including goals completed during the final
   accepted move's cascade chain.

5. `Failure` occurs when the player has no remaining accepted moves and required
   goals are still incomplete after all legal post-move resolution has finished.

6. `Retry Requested` is not a gameplay outcome by itself; it is a player action
   from a failed or abandoned session that starts a fresh session rebuild using
   authored level truth.

7. Retry must never resume transient runtime board state in MVP. It always
   reconstructs the level from authored data and resets runtime progress.

8. The result layer must distinguish between:
   - authoritative outcome truth
   - result presentation state
   - progression consequences

9. The result screen must present at least these pieces of information on
   victory:
   - level completion state
   - star rating earned
   - immediate next action options

10. The result screen must present at least these pieces of information on
    failure:
    - failure state
    - encouragement to retry
    - immediate next action options

11. For MVP, star rating must be deterministic and data-driven. A level must not
    compute stars from hidden heuristics or presentation-only rules.

12. The system must support per-level star-threshold policy through level data.
    The current MVP recommendation is to use move-efficiency bands, but the
    results system must treat the threshold definition as authored data rather
    than a hardcoded global assumption.

13. A completed level may update progression-facing best-result records only
    after the authoritative session outcome is `Victory`.

14. Losing a level must not overwrite a previously better victory/star result for
    that level.

15. Retry and post-result navigation must be fast enough to preserve mobile
    session momentum.

### States and Transitions

This system uses a result presentation lifecycle layered on top of committed
session outcome truth.

| State | Entry Condition | Exit Condition | Behavior |
|-------|----------------|----------------|----------|
| Waiting for Outcome | Level session is active and no terminal outcome has been committed | Gameplay commits win or loss state | Results layer is dormant and does not yet present terminal UI |
| Victory Pending Presentation | Gameplay commits a win state | Victory presentation is opened | Captures final session truth and prepares victory result content |
| Failure Pending Presentation | Gameplay commits a loss state | Failure presentation is opened | Captures final session truth and prepares failure result content |
| Result Presentation Active | Result UI is visible and awaiting player action | Player chooses retry, next, or return navigation | Shows outcome, performance, and next actions |
| Retry Transition | Player selects retry | Fresh level session is initialized | Clears old result presentation and requests new session build |
| Exit Transition | Player selects next step away from current result screen | Map/session shell or next level takes over | Hands session control back to higher-level flow systems |

State transition rules:

1. The system begins in `Waiting for Outcome` whenever a level session starts.
2. It enters `Victory Pending Presentation` only after committed win truth exists.
3. It enters `Failure Pending Presentation` only after committed loss truth exists.
4. It enters `Result Presentation Active` when the appropriate result UI becomes
   the active player-facing state.
5. It enters `Retry Transition` only from an active result or pause/abandon flow
   that explicitly requests retry.
6. Retry always rebuilds from authored level state and returns session control to
   gameplay flow.
7. Any exit action leaves result presentation and hands control to map or other
   shell flow systems.

### Interactions with Other Systems

1. **Level Goal and Move Limit System**
   - Provides authoritative win/loss truth, final move state, and goal-completion
     status.
   - Responsibility boundary: goal/move system owns outcome truth; results system
     owns result interpretation and presentation.

2. **Core HUD and Goal Feedback System**
   - Hands off final in-session visible state once terminal outcome is committed.
   - Responsibility boundary: HUD owns in-session display; results system owns
     post-session display.

3. **Game Flow State System**
   - Controls transitions between active level, result UI, retry, and return to
     map or shell.
   - Responsibility boundary: game flow owns navigation/state routing; results
     system defines what result states exist and what actions are exposed.

4. **MatchJoy Authoring System**
   - Provides authored star-threshold metadata and level identity used by result
     evaluation and persistence.
   - Responsibility boundary: authoring system stores result policy inputs;
     results system evaluates and presents the outcome.

5. **Chapter Map Progression System**
   - Consumes confirmed victory results to unlock next progression steps or update
     node completion status.
   - Responsibility boundary: results system confirms the outcome package;
     chapter map system applies broader campaign consequences.

6. **Save/Load and Settings Persistence System**
   - Persists best-completion state, earned stars, and relevant progression-safe
     result data.
   - Responsibility boundary: results system emits what should be saved;
     persistence system stores it durably.

7. **Juice Feedback System**
   - Provides polish for victory/failure transitions, star reveals, and retry
     responsiveness.
   - Responsibility boundary: juice layer polishes the presentation; results
     system defines what must be communicated.

## Formulas

### Move-Efficiency Star Evaluation

For MVP, the preferred default policy is authored move-efficiency thresholds.
The system evaluates remaining moves against authored threshold bands.

```text
stars_awarded =
  0, if outcome != Victory
  1, if victory and remaining_moves >= star_1_threshold
  2, if victory and remaining_moves >= star_2_threshold
  3, if victory and remaining_moves >= star_3_threshold
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| outcome | enum | Victory/Failure | runtime | Final authoritative session outcome |
| remaining_moves | int | 0 to authored move limit | runtime | Moves left when victory is committed |
| star_1_threshold | int | 0 to move limit | level data | Minimum remaining moves for 1-star result |
| star_2_threshold | int | star_1 to move limit | level data | Minimum remaining moves for 2-star result |
| star_3_threshold | int | star_2 to move limit | level data | Minimum remaining moves for 3-star result |
| stars_awarded | int | 0 to 3 | calculated | Final star count shown in results |

**Expected output range**: 0 to 3  
**Edge case**: If authored thresholds are missing or invalid, validation must
fail before structured playtest.

### Best Result Merge Rule

```text
best_stars = max(previous_best_stars, current_victory_stars)
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| previous_best_stars | int | 0 to 3 | persistence | Best previously saved result |
| current_victory_stars | int | 0 to 3 | runtime | Current victory star result |
| best_stars | int | 0 to 3 | calculated | Updated persistent best star value |

**Expected output range**: 0 to 3  
**Edge case**: Failure outcomes must not reduce stored best result.

## Edge Cases

| Scenario | Expected Behavior | Rationale |
|----------|------------------|-----------|
| Final goal completes during the last accepted move's cascade | Session is a victory and results present win, not loss | Final cascades are part of legal move resolution |
| Player reaches zero moves before cascade finishes | Result waits until all legal resolution is complete before determining final outcome | Outcome must follow committed gameplay truth, not intermediate counters |
| Player retries immediately after failure | Retry starts a fresh rebuilt session with no stale board state | Retry must be fast and trustworthy |
| Player already has 3 stars and wins again with 2 stars | Best saved result remains 3 stars | Worse results must not overwrite best progress |
| Player loses a previously completed level | Existing victory record remains intact | Failure should not erase progress |
| Authored star thresholds are invalid | Validation fails before structured playtest | Star model must stay deterministic and designer-readable |
| A level is completed for the first time with minimum qualifying victory | Results still show victory with the authored star count earned | Completion and performance are related but not identical |
| The player exits result UI without retrying | Session truth is preserved and control returns cleanly to the next flow owner | Result UI should be a conclusion point, not a state trap |

## Dependencies

| System | Direction | Nature of Dependency |
|--------|-----------|---------------------|
| Level Goal and Move Limit System | This system depends on Level Goal and Move Limit System | Needs authoritative victory/failure truth and final move/goal state |
| Core HUD and Goal Feedback System | This system depends on Core HUD and Goal Feedback System indirectly | Needs clean hand-off from in-session display to result display |
| Game Flow State System | This system depends on Game Flow State System | Needs session routing for result display, retry, and exit |
| MatchJoy Authoring System | This system depends on MatchJoy Authoring System | Needs authored result metadata such as thresholds and stable level ID |
| Chapter Map Progression System | Chapter Map Progression System depends on this system | Needs confirmed result package to update progression |
| Save/Load and Settings Persistence System | Save/Load and Settings Persistence System depends on this system | Needs result package to persist best stars and completion state |
| Juice Feedback System | Soft dependency in MVP | Adds presentation polish but does not define result truth |

Dependency notes:

1. The most important upstream truth source is `Level Goal and Move Limit System`.
2. The most important downstream consumers are `Chapter Map Progression System`
   and `Save/Load and Settings Persistence System`.
3. This system should remain outcome-focused and must not reimplement gameplay
   evaluation rules already owned elsewhere.

## Tuning Knobs

| Parameter | Current Value | Safe Range | Effect of Increase | Effect of Decrease |
|-----------|--------------|------------|-------------------|-------------------|
| Victory Result Hold Time | Short | Brief to moderate | Makes victory feel more ceremonious | Speeds the player back into flow |
| Failure Result Friction | Low | Very low to moderate | Slows retry slightly but can strengthen acknowledgment | Makes retry faster and more mobile-friendly |
| Retry Button Prominence | High on failure | Medium to high | Encourages fast re-engagement | Makes return-to-map more equally weighted |
| Star Reveal Intensity | Moderate | Subtle to strong | Makes performance feel more rewarding | Keeps result screen snappier |

## Visual/Audio Requirements

| Event | Visual Feedback | Audio Feedback | Priority |
|-------|----------------|---------------|----------|
| Victory result entry | Clear success presentation, positive transition, star reveal space | Victory stinger | High |
| Failure result entry | Clean failure presentation with retry emphasis | Failure cue that is soft, not punitive | High |
| Star reveal/update | Distinct star presentation and best-result clarity | Star reveal cue | High |
| Retry selected | Immediate responsive transition back toward gameplay | Fast confirm cue | Medium |

## UI Requirements

| Information | Display Location | Update Frequency | Condition |
|-------------|-----------------|-----------------|-----------|
| Victory/Failure state | Result screen headline area | Once per result entry | On result presentation |
| Stars earned | Result screen reward/performance area | On victory result entry | On victory |
| Retry action | Result screen primary or near-primary action zone | Static while result UI is open | On failure and optionally on victory |
| Next/Return action | Result screen action area | Static while result UI is open | When navigation onward is allowed |
| Best result clarity | Result screen performance area | On result entry | When prior completion data exists |

## Acceptance Criteria

- [ ] Victory is shown only after authoritative goal completion truth is committed.
- [ ] Failure is shown only after authoritative move exhaustion and post-resolution checks are complete.
- [ ] Retry always rebuilds the level from authored state rather than resuming transient board state.
- [ ] The system can assign stars deterministically from authored threshold data.
- [ ] A worse subsequent attempt never overwrites a better stored result.
- [ ] Result presentation clearly offers the player an immediate next action.
- [ ] The system integrates cleanly with chapter progression and persistence consumers.
- [ ] Final-move cascades that complete goals resolve as wins, not losses.
- [ ] Performance: result entry and retry flow are responsive enough to preserve mobile session momentum.
- [ ] No session outcome is determined solely by UI timing or presentation state.

## Open Questions

| Question | Owner | Deadline | Resolution |
|----------|-------|----------|-----------|
| Should MVP show stars only on victory results, or also preview prior best stars on failure screens? | User | Before results UI implementation | Not yet decided |
| Should retry be available directly from pause/abandon flow as well as failure results, or only from the failure result screen in MVP? | User + future flow pass | Before implementation | Not yet decided |
| Should star thresholds remain move-efficiency based long-term, or later support score-based policy in the same system? | User + future product decision | Before broader progression tuning | Move-efficiency recommended for MVP; long-term undecided |
| Should victory results show one clear primary CTA (`Next`) or co-equal CTA options (`Next` and `Replay`) in MVP? | User + future UX pass | Before UI implementation | Not yet decided |

Open question notes:

1. None of these open questions block an MVP result loop as long as victory,
   failure, retry, and deterministic stars are in place.
2. The guiding principle is momentum: result presentation should confirm the
   outcome without becoming a chore between puzzle sessions.
