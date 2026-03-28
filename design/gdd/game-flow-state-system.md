# Game Flow State System

> **Status**: In Design
> **Author**: User + Codex
> **Last Updated**: 2026-03-28
> **Implements Pillar**: Clean Commercial Puzzle Structure; Scoped Learnable Production

## Overview

The Game Flow State System defines the top-level runtime state machine for
MatchJoy. It controls how the game moves between boot, shell/menu context,
chapter map, level session setup, active play, pause, result presentation,
retry, and return transitions. Its job is to provide one authoritative flow
contract so all other systems know when they are allowed to initialize, update,
suspend, or hand off control.

This system does not own low-level gameplay truth such as board state, goal
completion, or star evaluation. Instead, it owns session routing and phase
coordination. It determines which major mode the player is in, which systems are
active in that mode, and what legal transitions exist between modes.

For MVP, the flow system must support:
- boot into a usable shell state
- entering the chapter map
- launching a level session from an unlocked node
- pausing and resuming a level session
- finishing a level into results
- retrying a level from fresh authored state
- returning from results to the map

## Player Fantasy

This system supports the feeling that MatchJoy is smooth, responsive, and easy
to navigate. The player should never wonder whether they are still in gameplay,
returning to the map, waiting for a retry, or trapped between screens. State
changes should feel intentional and polished rather than accidental.

The emotional target is stability and momentum. A good flow system makes the
whole product feel trustworthy: level start is clean, pause is safe, retry is
fast, and return to the map feels natural. The player should always understand
what context they are in and what action comes next.

## Detailed Design

### Core Rules

1. The Game Flow State System is the authoritative owner of top-level runtime
   mode transitions. Other systems may request transitions, but they must not
   independently change global session mode without going through flow logic.

2. At any moment, the game must be in exactly one top-level flow state.

3. For MVP, the top-level flow states must include at minimum:
   - Boot
   - Shell Ready
   - Chapter Map Active
   - Level Session Setup
   - Level Active
   - Level Paused
   - Result Presentation
   - Retry Transition
   - Return-to-Map Transition

4. Boot is responsible for establishing the minimum state needed to enter the
   playable shell safely. It must not leave the game in a partially initialized,
   interactable state.

5. Shell Ready is the non-level top-level context from which the player may
   enter chapter map flow and other future shell surfaces.

6. Chapter Map Active is the state in which chapter map interaction is legal and
   the player may select an unlocked level node.

7. Level Session Setup is responsible for constructing a fresh playable session
   from authored level data and runtime initialization rules. Gameplay systems
   must not treat the level as actively playable until setup has completed.

8. Level Active is the only top-level state in which normal tile input, live
   board updates, move consumption, and goal progression may occur.

9. Level Paused suspends ordinary gameplay interaction without destroying the
   current runtime session state.

10. Result Presentation may begin only after an authoritative terminal outcome
    has been committed by gameplay truth.

11. Retry Transition always rebuilds a level from authored state in MVP. It does
    not resume partially completed transient board state.

12. Return-to-Map Transition exits the current session/result context and hands
    control back to chapter map flow.

13. Systems that depend on active gameplay truth must be gated by flow state. If
    the top-level state is not compatible with their role, they must remain
    inactive, suspended, or read-only as appropriate.

14. Top-level transitions must be explicit and deterministic. The same trigger in
    the same valid source state should always route to the same target state.

15. Invalid transitions must fail safely in development builds and must not leave
    the game in a half-switched global state.

16. This system must remain scope-conscious for the sample. MVP flow does not
    need live-ops inbox states, social invite states, store flows, or background
    match restoration.

### States and Transitions

The flow system owns the major runtime states below.

| State | Entry Condition | Exit Condition | Behavior |
|-------|----------------|----------------|----------|
| Boot | Game executable/session starts | Minimum shell-safe initialization is complete | Initializes required runtime services and blocks normal player interaction |
| Shell Ready | Boot completes successfully or a higher-level shell return occurs | Player enters chapter map or future shell route | Shell context is active and ready to route into map flow |
| Chapter Map Active | Shell/map routing enters chapter map context | Player launches a level or exits map context | Campaign node interaction is legal and map progression state is visible |
| Level Session Setup | An unlocked level launch is accepted | Session data is built successfully or setup fails safely | Builds a fresh runtime session from authored data and initializes gameplay-facing systems |
| Level Active | Level session setup completes successfully or pause is resumed | Pause, terminal outcome, or forced exit occurs | Normal gameplay interaction and runtime puzzle simulation are active |
| Level Paused | Pause is requested during Level Active | Resume, retry request, or exit request occurs | Gameplay interaction is suspended and session state is preserved |
| Result Presentation | Terminal win/loss outcome is committed and result UI is opened | Retry, next action, or return-to-map is chosen | Displays authoritative session outcome and available next actions |
| Retry Transition | Retry is requested from pause or results flow | Fresh session setup begins | Tears down transient session state and requests a clean rebuild of the same level |
| Return-to-Map Transition | Player exits a session/result context back to campaign flow | Chapter map is ready again | Cleans up session-local context and restores map focus |

State transition rules:

1. `Boot -> Shell Ready` when required startup initialization succeeds.
2. `Shell Ready -> Chapter Map Active` when chapter map context is entered.
3. `Chapter Map Active -> Level Session Setup` when the player selects a legal,
   unlocked level node.
4. `Level Session Setup -> Level Active` when gameplay session construction is
   complete and input-safe state is ready.
5. `Level Active -> Level Paused` when pause is requested.
6. `Level Paused -> Level Active` when resume is accepted.
7. `Level Active -> Result Presentation` when committed win/loss truth exists and
   result flow is opened.
8. `Level Paused -> Retry Transition` when retry is requested from pause flow.
9. `Result Presentation -> Retry Transition` when retry/replay is requested.
10. `Retry Transition -> Level Session Setup` when fresh session rebuild begins.
11. `Result Presentation -> Return-to-Map Transition` when the player chooses to
    leave the current result context.
12. `Level Paused -> Return-to-Map Transition` when the player abandons the
    current session back to map.
13. `Return-to-Map Transition -> Chapter Map Active` when chapter map context is
    ready to resume.

### Interactions with Other Systems

1. **Chapter Map Progression System**
   - Uses flow state to know when map interaction is active and when to focus a
     newly unlocked or current level node.
   - Responsibility boundary: flow owns mode routing; map progression owns node
     state and campaign interpretation.

2. **MatchJoy Authoring System**
   - Provides stable level identity referenced during level launch and retry.
   - Responsibility boundary: authoring provides content references; flow decides
     when they are loaded into session context.

3. **Board Grid State System**
   - Is activated during level session setup and live during Level Active.
   - Responsibility boundary: board owns runtime puzzle truth; flow decides when
     board runtime may exist and update.

4. **Input and Tile Selection System**
   - Must only accept normal gameplay input during Level Active.
   - Responsibility boundary: input interprets player actions; flow determines
     when those actions are legal.

5. **Core HUD and Goal Feedback System**
   - Reacts differently depending on whether gameplay is active, paused, or in
     result hand-off.
   - Responsibility boundary: HUD presents state; flow gates when HUD is active,
     suspended, or handing off.

6. **Level Goal and Move Limit System**
   - Emits committed terminal outcomes that permit transition into results.
   - Responsibility boundary: goal system owns win/loss truth; flow owns routing
     after that truth exists.

7. **Results, Retry, and Star Rating System**
   - Runs inside Result Presentation and Retry Transition contexts.
   - Responsibility boundary: results owns result interpretation and UI; flow
     owns which phase the player is in.

8. **Save/Load and Settings Persistence System**
   - May load player progression before map flow and save durable changes after
     meaningful result progression updates.
   - Responsibility boundary: persistence stores durable state; flow determines
     when load/save-sensitive transitions occur.

## Formulas

### Legal Gameplay Input Predicate

```text
gameplay_input_enabled = (flow_state == Level Active)
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| flow_state | enum | top-level flow states | runtime | Current top-level flow state |
| gameplay_input_enabled | bool | true/false | calculated | Whether ordinary puzzle input is currently legal |

### Retry Rebuild Predicate

```text
retry_uses_fresh_session = true
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| retry_uses_fresh_session | bool | true | system rule | Retry always rebuilds from authored truth in MVP |

## Edge Cases

| Scenario | Expected Behavior | Rationale |
|----------|------------------|-----------|
| Player tries to interact with tiles during Level Session Setup | Input is ignored or blocked until Level Active begins | Gameplay must not start from partial initialization |
| Player pauses during a cascade-heavy moment | Flow enters Level Paused only at a safe pause boundary defined by runtime systems | Pause should preserve consistency, not interrupt commits mid-transition |
| Win state is committed during final cascade | Flow routes to Result Presentation only after committed outcome exists | Flow must follow gameplay truth, not visual guesswork |
| Player requests retry from pause | Flow enters Retry Transition and rebuilds the same level from authored state | Retry from pause should match retry from results semantically |
| Player returns to map from a failed session | Flow uses Return-to-Map Transition and preserves durable progression truth | Leaving a failed session should not corrupt campaign state |
| Invalid level launch is requested from a locked node | Transition is rejected and flow remains in Chapter Map Active | Illegal launches must fail safely |
| Boot initialization fails in development | Flow remains in safe non-playable state and surfaces error information | The game must not enter partially valid flow |
| A system tries to update gameplay during Result Presentation | That system should be suspended or ignored according to flow gating rules | Session conclusion must be stable once result flow is active |

## Dependencies

| System | Direction | Nature of Dependency |
|--------|-----------|---------------------|
| MatchJoy Authoring System | This system depends on MatchJoy Authoring System | Needs stable level identity and content references for launch/retry |
| Chapter Map Progression System | This system coordinates with Chapter Map Progression System | Needs map-active flow phase and legal node-entry routing |
| Level Goal and Move Limit System | This system depends on Level Goal and Move Limit System for terminal truth | Needs authoritative win/loss outcome before entering results |
| Results, Retry, and Star Rating System | Results system depends on this system for phase routing | Needs a stable result phase and retry/return transitions |
| Input and Tile Selection System | Input system depends on this system | Needs a clear legality gate for player interaction |
| Save/Load and Settings Persistence System | Soft dependency in current draft, hard dependency once implemented | Needs load/save-safe transition boundaries |

Dependency notes:

1. This is one of the highest-leverage systems because it connects nearly every
   player-visible phase of the product.
2. The most critical contract is that gameplay truth systems emit outcomes, while
   flow decides where the player goes next.
3. If flow becomes ambiguous, the whole product feels unstable even if local
   systems are correct.

## Tuning Knobs

| Parameter | Current Value | Safe Range | Effect of Increase | Effect of Decrease |
|-----------|--------------|------------|-------------------|-------------------|
| Transition Friction | Low | Very low to moderate | Makes state changes feel more ceremonial but slower | Makes the game feel snappier and more mobile-friendly |
| Retry Path Speed | High | Medium to very high | Makes failure recovery faster and more momentum-friendly | Gives more breathing room but slows loop cadence |
| Return-to-Map Focus Strength | High | Medium to high | Makes next action clearer after result/map return | Gives more player freedom but less guidance |
| Pause Depth | Minimal in MVP | Minimal to moderate | Adds more pause features but increases flow complexity | Keeps the state machine simple and robust |

## Visual/Audio Requirements

| Event | Visual Feedback | Audio Feedback | Priority |
|-------|----------------|---------------|----------|
| Level launch transition | Clear entry into gameplay session | Optional launch cue | High |
| Pause entered/resumed | Obvious pause state and safe resume confirmation | Optional soft pause/resume cue | Medium |
| Result hand-off | Clear transition from active play to result context | Result entry cue owned with results/juice systems | High |
| Return to map | Clear restoration of chapter map context | Optional transition cue | Medium |

## UI Requirements

| Information | Display Location | Update Frequency | Condition |
|-------------|-----------------|-----------------|-----------|
| Current top-level context affordance | Context-specific shell/map/session UI | On every major flow transition | Always |
| Pause availability | In-level session UI | During Level Active | When pause is supported |
| Retry/return actions | Pause and result UI | When those states are active | In pause/result contexts |
| Legal map-node entry affordance | Chapter map node UI | While map is active | In Chapter Map Active |

## Acceptance Criteria

- [ ] The system can route from boot to map to level to result and back without ambiguous top-level state.
- [ ] Gameplay input is legal only during Level Active.
- [ ] Retry always rebuilds from authored session truth in MVP.
- [ ] Pause/resume does not corrupt active session state.
- [ ] Result presentation begins only after committed terminal outcome truth exists.
- [ ] Returning to the map from pause/result restores a valid campaign context.
- [ ] Illegal level-launch requests are rejected safely.
- [ ] Flow state remains singular and deterministic throughout runtime.
- [ ] Performance: normal top-level transitions feel responsive enough to preserve a mobile-friendly session loop.
- [ ] No major system bypasses flow ownership to change top-level runtime mode directly.

## Open Questions

| Question | Owner | Deadline | Resolution |
|----------|-------|----------|-----------|
| Should MVP include a distinct title/home shell screen before the chapter map, or treat map as the first meaningful shell state after boot? | User | Before UI implementation | Not yet decided |
| What is the exact safe pause boundary rule during long cascade/power-up chains: immediate visual pause request with deferred commit, or only accept pause at board-stable moments? | User + future runtime implementation pass | Before implementation | Not yet decided |
| Should retry from victory be exposed in MVP result flow, or only next/return actions? | User + future UX pass | Before implementation | Not yet decided |
| Does the sample need a dedicated end-of-campaign flow state beyond normal result -> map return? | User | Before final sample polish | Not yet decided |

Open question notes:

1. None of these questions block a coherent MVP state machine.
2. The default principle is determinism: state transitions should be easy to
   reason about both for the player and for implementation.
