# Juice Feedback System

> **Status**: In Design
> **Author**: User + Codex
> **Last Updated**: 2026-03-28
> **Implements Pillar**: Juicy Match-3 Satisfaction; Clean Commercial Puzzle Structure

## Overview

The Juice Feedback System defines the timing, priority, and coordination rules
for moment-to-moment feel feedback in MatchJoy. It is responsible for turning
committed gameplay events into readable visual and audio responses such as tile
selection confirmation, invalid-swap rejection, match pop timing, cascade reward
escalation, goal-progress emphasis, power-up activation punch, and result
handoff energy.

This system does not own gameplay truth, result truth, or board mutation rules.
Instead, it consumes committed events from gameplay and flow systems and applies
a controlled feedback layer that makes the puzzle loop feel responsive,
satisfying, and commercially polished without obscuring board readability.

For MVP, the juice layer must support:
- selection and invalid-swap feedback
- accepted swap and match confirmation
- match clear and cascade escalation feedback
- power-up creation and activation feedback
- goal progress and goal completion emphasis
- level victory/failure handoff energy

## Player Fantasy

This system supports the feeling that the board is alive, responsive, and
rewarding. Every meaningful action should feel acknowledged: a valid move should
feel committed, a strong cascade should feel exciting, and an important goal
step should feel celebrated without becoming noisy or confusing.

The emotional target is delight with clarity. MatchJoy should feel energetic and
juicy, but never mushy or unreadable. The player should feel that the game is
reacting to what truly happened, not just firing random effects.

## Detailed Design

### Core Rules

1. The Juice Feedback System is a presentation consumer of committed gameplay and
   flow events. It must not infer or invent gameplay truth.

2. Feedback must be event-driven. For MVP, the system should respond only to
   authoritative events such as:
   - tile selected
   - invalid swap rejected
   - valid swap accepted
   - match groups committed
   - cascade step committed
   - power-up created
   - power-up activated
   - goal progress changed
   - goal completed
   - level victory committed
   - level failure committed

3. Feedback timing must preserve board readability. Juice may delay the visual
   pace slightly for clarity, but it must not hide or contradict the underlying
   board truth.

4. Invalid swap feedback must be fast, clear, and lightweight. It should confirm
   that the attempted action was understood and rejected without creating the
   illusion that the board truly changed.

5. Accepted swap feedback must feel more committed than invalid swap feedback. At
   minimum, the player should be able to distinguish instantly between:
   - swap rejected
   - swap accepted with normal match
   - swap accepted with stronger outcome such as multi-match or power-up creation

6. Match clear feedback must follow the committed clear payload. Only cells that
   are truly part of the approved clear should receive primary clear emphasis.

7. Cascade feedback must escalate modestly with each additional committed cascade
   step. Escalation may include stronger animation timing, brighter emphasis,
   layered audio, or increased reward framing, but it must remain bounded and
   readable.

8. Goal-progress feedback must be synchronized to committed progress changes from
   goal, obstacle, and frozen-ingredient systems. Feedback must not trigger from
   speculative or preview state.

9. Goal completion feedback must be clearly stronger than ordinary progress
   feedback, while still remaining smaller in scope than full victory feedback.

10. Power-up creation and activation feedback must clearly communicate both
    origin and effect. The player should understand where a special piece was
    created and what cells an activation actually affected.

11. If multiple feedback-worthy events occur in close sequence, the system must
    apply a deterministic priority order so important signals are not buried.
    For MVP, recommended priority is:
    - terminal victory/failure handoff
    - goal completion
    - power-up activation
    - power-up creation
    - cascade escalation
    - normal match clear
    - accepted swap confirmation
    - selection/invalid-swap feedback

12. Juice must be interruptible or compressible when a higher-priority state
    arrives. For example, victory handoff may cut short low-priority lingering
    feedback once terminal outcome is committed.

13. Juice must be tightly coordinated with flow state. During pause, result-only
    screens, or non-gameplay flow states, gameplay-loop juice should be
    suppressed or transitioned into the appropriate presentation layer.

14. MVP juice should avoid excessive layering that obscures the board. Stronger
    feedback is not always better; the board must remain readable at all times.

### States and Transitions

The juice system uses a lightweight presentation lifecycle around event playback.

| State | Entry Condition | Exit Condition | Behavior |
|-------|----------------|----------------|----------|
| Idle | No active high-priority feedback event is playing | A feedback-worthy committed event arrives | Waits for next feedback trigger |
| Playing Event Feedback | A committed event requests associated feedback | Feedback finishes, is superseded, or chains into another event | Plays the mapped visual/audio response for the current event |
| Escalated Chain Feedback | Consecutive committed cascade/power-up chain events justify stepped-up feedback | Chain ends, priority is superseded, or state returns to idle | Applies bounded escalation rules to chain-aware feedback |
| Suspended | Game flow or overlay state temporarily disallows normal gameplay feedback | Legal feedback playback resumes or system hands off to another presentation context | Suppresses or safely pauses ordinary gameplay juice |
| Terminal Handoff | Victory or failure result has been committed | Result presentation or shell transition fully takes over | Compresses lower-priority feedback and emphasizes terminal state handoff |

State transition rules:

1. The system begins in `Idle`.
2. It enters `Playing Event Feedback` whenever a committed event with mapped
   feedback arrives.
3. It enters `Escalated Chain Feedback` when consecutive committed chain events
   exceed the baseline single-event case.
4. It enters `Suspended` whenever flow or overlay state makes ordinary gameplay
   juice inappropriate.
5. It enters `Terminal Handoff` when committed victory/failure outcome arrives.
6. It returns to `Idle` when no higher-priority feedback remains active.

### Interactions with Other Systems

1. **Input and Tile Selection System**
   - Provides selection, retarget, and invalid-swap rejection timing hooks.
   - Responsibility boundary: input owns interaction truth; juice adds feel.

2. **Tile Swap and Match Resolution System**
   - Provides accepted/rejected swap results and first-pass match metadata.
   - Responsibility boundary: swap system defines outcome; juice expresses it.

3. **Cascade, Refill, and Determinism System**
   - Provides committed cascade-step and refill-resolution timing.
   - Responsibility boundary: cascade system owns resolution sequence; juice
     decides how escalation feels.

4. **Power-Up Creation and Activation System**
   - Provides creation and activation events plus affected-cell context.
   - Responsibility boundary: power-up system defines what happened; juice
     communicates intensity and clarity.

5. **Level Goal and Move Limit System**
   - Provides progress-change and goal-complete events.
   - Responsibility boundary: goal system owns correctness; juice owns emphasis.

6. **Obstacle and Special Tile System**
   - May provide specific board-state changes such as jelly clear milestones.
   - Responsibility boundary: obstacle logic decides state change; juice decides
     whether and how to accent it.

7. **Frozen Ingredient Objective System**
   - Provides ingredient release events that deserve stronger-than-normal goal
     feedback.
   - Responsibility boundary: ingredient system owns release truth; juice
     presents the emotional payoff.

8. **Game Flow State System**
   - Gates when gameplay-loop feedback is active, suspended, or transitioning.
   - Responsibility boundary: flow owns context legality; juice owns playback
     within that context.

9. **Results, Retry, and Star Rating System**
   - Receives terminal handoff once victory/failure is committed.
   - Responsibility boundary: juice punctuates the transition; results owns the
     full post-session presentation.

## Formulas

### Cascade Escalation Step

```text
cascade_feedback_level = clamp(cascade_step_index, 1, max_cascade_feedback_level)
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| cascade_step_index | int | 1+ | runtime | Which committed cascade step is currently resolving |
| max_cascade_feedback_level | int | 1+ | tuning/config | Upper bound on feedback escalation |
| cascade_feedback_level | int | 1 to max level | calculated | Effective escalation tier used for polish playback |

### Feedback Priority Comparison

```text
event_a_preempts_event_b = (priority_a > priority_b)
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| priority_a | int | low to high | event mapping | Priority of incoming event |
| priority_b | int | low to high | active playback | Priority of currently active event |
| event_a_preempts_event_b | bool | true/false | calculated | Whether the new event should interrupt/compress the current one |

## Edge Cases

| Scenario | Expected Behavior | Rationale |
|----------|------------------|-----------|
| Invalid swap is rejected while a low-priority selection glow is active | Invalid-swap feedback replaces or compresses the lower-priority selection effect | Rejection must remain readable |
| A power-up activation also completes the final goal | Goal/victory-relevant feedback takes precedence over low-priority lingering effects | Important state changes must not be buried |
| A long cascade chain produces many quick commits | Escalation remains bounded and readable rather than stacking infinitely | Juiciness must not become noise |
| Goal progress changes from a cascade rather than the original swap | Goal feedback still triggers normally | Committed cascade progress is real progress |
| Player pauses during active feedback playback | Gameplay-loop juice is suspended or safely frozen according to flow rules | Pause should preserve state clarity |
| Victory is committed while ordinary clear feedback is still playing | System enters terminal handoff and compresses lower-priority effects | Terminal outcome should dominate |
| A cell visually shakes for invalid swap but the board never legally changed | No downstream system interprets this as a real board mutation | Juice must never masquerade as gameplay truth |
| Several low-priority events arrive during result transition | They are dropped or compressed according to priority rules | Result handoff should stay clean |

## Dependencies

| System | Direction | Nature of Dependency |
|--------|-----------|---------------------|
| Input and Tile Selection System | This system depends on Input and Tile Selection System | Needs selection and invalid-swap timing events |
| Tile Swap and Match Resolution System | This system depends on Tile Swap and Match Resolution System | Needs accepted/rejected swap and match metadata |
| Cascade, Refill, and Determinism System | This system depends on Cascade, Refill, and Determinism System | Needs chain-step timing and committed resolution events |
| Power-Up Creation and Activation System | This system depends on Power-Up Creation and Activation System | Needs creation/activation event payloads |
| Level Goal and Move Limit System | This system depends on Level Goal and Move Limit System | Needs progress-change and completion events |
| Obstacle and Special Tile System | Soft dependency in MVP | Uses obstacle-relevant state changes for emphasis when needed |
| Frozen Ingredient Objective System | Soft dependency in MVP | Uses release events for objective payoff emphasis |
| Game Flow State System | This system depends on Game Flow State System | Needs legality gating for playback and suspension |
| Results, Retry, and Star Rating System | Results system coordinates with this system | Needs clean terminal handoff timing |

Dependency notes:

1. The most important timing sources are swap/match, cascade, and goal systems.
2. This system should remain feedback-only; it must never become a hidden owner
   of gameplay sequencing.
3. If the juice layer becomes ambiguous, player trust drops even if the underlying
   mechanics are correct.

## Tuning Knobs

| Parameter | Current Value | Safe Range | Effect of Increase | Effect of Decrease |
|-----------|--------------|------------|-------------------|-------------------|
| Invalid Swap Feedback Strength | Light | Subtle to moderate | Makes rejection clearer but risks feeling punitive | Keeps feel softer but may weaken readability |
| Match Pop Intensity | Moderate | Subtle to strong | Makes clears feel more rewarding | Keeps board cleaner but less lively |
| Cascade Escalation Ceiling | 3 | 2-5 | Makes long chains feel more rewarding | Keeps feedback flatter and more controlled |
| Goal Completion Emphasis | Stronger than normal progress | Moderate to strong | Makes milestone payoff more legible | Risks under-celebrating important moments if too low |
| Terminal Handoff Compression | Moderate | Low to high | Makes victory/failure transitions cleaner and faster | Preserves more local feedback but risks muddy handoff |

## Visual/Audio Requirements

| Event | Visual Feedback | Audio Feedback | Priority |
|-------|----------------|---------------|----------|
| Tile selected | Clean highlight and slight confirmation response | Light selection tick | Medium |
| Invalid swap rejected | Bounce-back or shake with no false board commit | Soft reject cue | High |
| Valid swap accepted | Clear commitment response into match timing | Positive confirm cue | High |
| Match clear | Pop/despawn feedback aligned to approved clear payload | Match-clear cue | High |
| Cascade escalation | Progressive but bounded escalation in timing/emphasis | Escalating combo layer | Medium |
| Power-up created | Distinct spawn/readability cue at creation cell | Special-piece creation cue | High |
| Power-up activated | Strong area/line/rainbow effect tied to actual affected cells | Power-up activation cue | High |
| Goal progress advanced | Goal panel pulse and/or board-linked confirmation | Progress cue | High |
| Goal completed | Strong milestone emphasis short of full victory | Goal-complete cue | High |
| Victory/failure handoff | Clean transition into terminal state | Victory/failure stinger | High |

## UI Requirements

| Information | Display Location | Update Frequency | Condition |
|-------------|-----------------|-----------------|-----------|
| Selected-tile feedback | On board | On selection | During active input |
| Invalid-swap feedback | On attempted swap cells | On rejected swap | When a swap is rejected |
| Goal progress emphasis | Goal panel and optional board-adjacent confirmation | On committed progress change | When progress changes |
| Terminal handoff emphasis | Whole playfield to result transition layer | On committed victory/failure | During result handoff |

## Acceptance Criteria

- [ ] Invalid swaps receive readable feedback without implying a committed board mutation.
- [ ] Accepted swaps, clears, cascades, and power-ups receive distinct levels of feedback.
- [ ] Goal progress and goal completion feedback trigger only from committed state changes.
- [ ] Terminal victory/failure handoff cleanly preempts low-priority lingering effects.
- [ ] Pause and non-gameplay states suppress or safely suspend gameplay-loop juice.
- [ ] Feedback priority ordering remains deterministic under event overlap.
- [ ] Cascade escalation feels stronger than single clears without becoming visually noisy.
- [ ] Performance: the juice layer remains lightweight enough not to create noticeable interaction or resolution hitching.
- [ ] The system never becomes a hidden source of gameplay truth or sequencing.
- [ ] Board readability remains intact under ordinary and high-activity puzzle states.

## Open Questions

| Question | Owner | Deadline | Resolution |
|----------|-------|----------|-----------|
| Should invalid swap feedback be purely board-local, or also briefly accent the selected source tile/HUD? | User + future UX pass | Before polish implementation | Not yet decided |
| How aggressive should cascade escalation audio be before it starts to feel noisy on long chains? | User + future audio tuning | During polish tuning | Not yet decided |
| Should goal-progress callouts remain HUD-only in MVP, or include lightweight floating board confirmations? | User + future UX pass | Before polish implementation | Not yet decided |
| Should retry and result-entry transitions share a common polish language, or feel distinctly different in MVP? | User + future presentation pass | Before polish implementation | Not yet decided |

Open question notes:

1. None of these questions block a coherent MVP juice layer.
2. The guiding principle is readable delight: feedback should reinforce the
   puzzle, not compete with it.
