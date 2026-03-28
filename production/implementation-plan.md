# MatchJoy First Implementation Plan

> **Status**: Draft
> **Author**: User + Codex
> **Last Updated**: 2026-03-28
> **Purpose**: Turn the current GDD set into a realistic first playable implementation plan for Unity.

---

## Goal

Build the first playable MatchJoy slice in Unity as quickly as possible without
creating heavy rework later. The target of this first implementation plan is not
"full product completeness". It is a clean, extensible vertical path through the
core loop:

- load one authored level
- render a playable board
- select and swap tiles
- detect valid/invalid swaps
- resolve clears
- perform cascades and refill
- update goals and moves
- show win/loss
- retry or return to map
- persist basic progression

This plan deliberately separates:
- **Must build now** to prove the game loop
- **Build thinly now** to avoid blocking later systems
- **Defer for later** to protect schedule and reduce rework

---

## Slice Definition

### First Playable Slice

The first playable slice should support exactly this experience:

1. Player launches the game into a simple shell/map entry.
2. Player enters one test chapter map with a small set of connected nodes.
3. Player starts one or more authored test levels.
4. Inside the level:
   - board appears correctly
   - player can select and swap tiles
   - invalid swaps reject cleanly
   - valid swaps create matches
   - board clears, cascades, and refills deterministically
   - moves decrement only on accepted swaps
   - at least 2 goal types work (`clear jelly`, `collect target tiles`)
   - optional third goal type (`release frozen ingredients`) can be included if stable
5. Win/loss results display correctly.
6. Retry rebuilds the level from authored state.
7. Returning to map updates progression and preserves best stars.

### Out of Scope for First Playable Slice

Do **not** treat these as required in the first implementation pass:

- full 50-level content production
- final art polish
- advanced menu shell
- onboarding/tutorial system
- analytics
- accessibility feature set
- cloud save or multi-profile support
- complex power-up combo matrix beyond core MVP families
- large obstacle family count

---

## Implementation Strategy

### Principle 1: Build the Spine First

Implement the systems that create the smallest complete playable loop before
building comfort systems or polish systems.

### Principle 2: Keep Authoring Real

Even in the first slice, use real authored level data rather than hardcoded test
values scattered in scene scripts. The authoring layer can be thin, but it
should already resemble the intended production path.

### Principle 3: Prefer Thin Vertical Integrations

A thin real path through map -> level -> result -> retry -> save is more useful
than deep local sophistication in one subsystem.

### Principle 4: Fake Presentation Before Faking Rules

If something must be stubbed, stub visual polish before gameplay contracts.
Board truth and result truth should stabilize earlier than final UX polish.

---

## Phase Plan

## Phase 0: Unity Project Skeleton

### Objective

Create the minimum Unity-side structure needed for implementation to begin
without architecture churn.

### Deliverables

- Unity project created with agreed stack (`Unity 6.3 LTS`, `URP`, `C#`)
- initial folder structure
- assembly definition strategy if desired
- base scene structure:
  - boot scene or boot entry
  - map scene or shell scene
  - level scene
- baseline coding conventions reflected in project structure

### Suggested Folder Skeleton

```text
Assets/
  MatchJoy/
    Scripts/
      Core/
      Flow/
      Board/
      Input/
      Goals/
      PowerUps/
      Obstacles/
      UI/
      Persistence/
      Authoring/
    ScriptableObjects/
      Levels/
      Config/
    Prefabs/
      Board/
      Tiles/
      UI/
    Scenes/
      Boot.unity
      Map.unity
      Level.unity
    Art/
    Audio/
```

### Notes

Do not overdesign package/module boundaries yet. Keep the folder structure clean,
but avoid premature abstraction until the first playable loop exists.

---

## Phase 1: Core Board Spine

### Objective

Make one board playable from authored data.

### Systems Included

- MatchJoy Authoring System
- Board Grid State System
- Input and Tile Selection System
- Tile Swap and Match Resolution System
- Cascade, Refill, and Determinism System

### Must-Have Deliverables

- level data asset can define board size and tile set
- board grid can be instantiated from level data
- player can select/swipe/tap tiles
- swap request path works
- invalid swap rejection works
- valid 3-match resolution works
- refill works deterministically enough for prototype use
- dead-board handling follows current MVP contract

### Thin / Temporary Choices Allowed

- use simple placeholder tile visuals
- use minimal animation timings
- use deterministic/random refill implementation without full tuning sophistication
- use debug overlays/logging to inspect board state

### Explicit Deferrals

- advanced special tile interactions
- rich juice layer
- multiple obstacle families
- level editor tooling beyond usable ScriptableObject authoring

### Phase 1 Exit Criteria

A tester can play a board, make matches, watch refill happen, and never see an
ambiguous board state during ordinary play.

---

## Phase 2: Goal and Session Closure

### Objective

Turn the board toy into a real level session.

### Systems Included

- Level Goal and Move Limit System
- Core HUD and Goal Feedback System
- Results, Retry, and Star Rating System
- Game Flow State System

### Must-Have Deliverables

- accepted swaps consume moves
- goals progress from committed board events
- HUD shows moves and goals clearly
- win/loss states resolve correctly
- final-cascade win rule works
- result screen shows success/failure
- retry fully rebuilds the level

### Thin / Temporary Choices Allowed

- HUD can be plain and functional rather than polished
- result screen can be simple text/buttons first
- stars can use the current move-efficiency model without advanced UX flourish

### Phase 2 Exit Criteria

A complete single-level session can begin, play, end, retry, and re-enter cleanly
without designer or programmer intervention.

---

## Phase 3: Vertical Slice Wrapper

### Objective

Wrap the single-level loop in enough product structure to feel like a real slice.

### Systems Included

- Chapter Map Progression System
- Save/Load and Settings Persistence System
- minimal shell/menu routing from Game Flow

### Must-Have Deliverables

- player can enter a level from a chapter map
- victory unlocks the next level
- best stars persist
- completed/unlocked nodes display correctly
- app relaunch restores durable progression
- map return after results feels correct

### Thin / Temporary Choices Allowed

- map can be simple node buttons rather than final path art
- settings screen can be minimal
- shell UI can remain thin if flow is clear

### Phase 3 Exit Criteria

A player can play multiple connected levels across sessions and see progression
persist.

---

## Phase 4: Signature Content Layer

### Objective

Add the minimum content features needed for MatchJoy to feel like *this* project,
not just a generic board prototype.

### Systems Included

- Power-Up Creation and Activation System
- Obstacle and Special Tile System
- Frozen Ingredient Objective System
- Juice Feedback System

### Must-Have Deliverables

- core power-up families work
- jelly goal works cleanly
- one frozen ingredient goal path works cleanly
- juice layer gives readable invalid swap, match, cascade, goal, and result feedback

### Thin / Temporary Choices Allowed

- cap power-up combo set aggressively
- cap obstacle family count aggressively
- keep frozen ingredient rollout to one clean MVP behavior

### Phase 4 Exit Criteria

The slice feels recognizably like a commercial match-3 prototype rather than a
purely mechanical exercise.

---

## Recommended Build Order

1. Project skeleton and scenes
2. Level data asset + board instantiation
3. Input selection and swap requests
4. Match detection and invalid swap rejection
5. Clear -> cascade -> refill loop
6. Move consumption and goal progress
7. Win/loss and retry
8. Minimal HUD
9. Minimal map progression
10. Save/load for stars and unlocks
11. Power-ups
12. Jelly
13. Frozen ingredient
14. Juice/polish pass

---

## Unity Module Breakdown

## Core Runtime Modules

### `Flow`

Owns:
- top-level runtime mode
- map/level/result routing
- retry and return transitions

Suggested first classes:
- `GameFlowController`
- `GameFlowState`
- `FlowTransitionRequest`

### `Authoring`

Owns:
- level asset schema
- mapping from ScriptableObject data to runtime session config

Suggested first classes:
- `LevelDefinition`
- `GoalDefinition`
- `BoardLayoutDefinition`
- `StarThresholdPolicy`

### `Board`

Owns:
- board cells
- occupancy state
- runtime board snapshot and mutation application

Suggested first classes:
- `BoardState`
- `BoardCell`
- `BoardCoordinate`
- `BoardBuilder`
- `BoardMutation` / `BoardMutationStep`

### `Input`

Owns:
- tile selection state
- swipe/tap interpretation
- swap request emission

Suggested first classes:
- `BoardInputController`
- `TileSelectionState`
- `SwapRequest`

### `Match`

Owns:
- swap acceptance/rejection
- match group detection
- first-pass resolution payload

Suggested first classes:
- `SwapResolutionService`
- `MatchGroup`
- `MatchResolutionResult`

### `Cascade`

Owns:
- clear application
- gravity/refill steps
- dead-board detection and reshuffle trigger

Suggested first classes:
- `CascadeResolver`
- `RefillResolver`
- `DeadBoardResolver`

### `Goals`

Owns:
- goal progress aggregation
- move counting
- win/loss decision input

Suggested first classes:
- `GoalTracker`
- `MoveCounter`
- `LevelOutcomeEvaluator`

### `UI`

Owns:
- in-level HUD
- results screen
- map node state display

Suggested first classes:
- `HudPresenter`
- `ResultsPresenter`
- `ChapterMapPresenter`

### `Persistence`

Owns:
- progression profile
- best stars
- settings

Suggested first classes:
- `PlayerProfile`
- `PersistenceService`
- `SettingsData`

---

## Recommended First Technical Contracts

These contracts should be stabilized early because many systems depend on them:

1. `BoardCoordinate`
2. `SwapRequest`
3. `MatchResolutionResult`
4. `LevelDefinition`
5. `GoalProgressSnapshot`
6. `LevelOutcomeSnapshot`
7. `PlayerProfile`

If these drift too much during implementation, rework cost will rise quickly.

---

## MVP-First Cuts

To protect schedule, keep these implementation cuts explicit:

- Only one local player profile
- No mid-session resume
- No cloud sync
- No advanced shell stack
- No analytics
- No live-ops hooks
- No booster economy
- No more than one frozen ingredient behavior in first slice
- No more than one or two true obstacle families in early vertical slice
- No full combo explosion matrix before single-power-up behavior is stable

---

## First Sprint Proposal

## Sprint Goal

Have one level fully playable in Unity with swap, match, clear, refill, moves,
and at least one working goal type.

## Sprint Scope

Included:
- Unity skeleton
- one playable level asset
- board rendering from authored data
- input selection
- swap resolution
- clear/cascade/refill
- move counter
- one goal type (`clear jelly` or `collect target tiles`)
- basic debug HUD

Deferred from Sprint 1:
- map progression
- persistence
- power-ups
- frozen ingredients
- full results polish
- juice polish

## Sprint 1 Deliverables

- one Unity scene runs a playable board session
- one level asset loads successfully
- at least one successful and one rejected swap path are testable
- move counter updates correctly
- a level can be won or lost in a basic way

## Sprint 1 Risks

- board mutation architecture may get tangled if state ownership is unclear
- refill/determinism may produce hidden bugs early
- if level data is too thin, later systems will start hardcoding exceptions

## Sprint 1 Mitigations

- log every swap result and board mutation in development
- keep board truth centralized
- validate authored level data before session start
- resist adding presentation complexity until the loop is stable

---

## Sprint 2 Proposal

## Sprint Goal

Turn the single-level prototype into a small connected product slice.

## Sprint Scope

Included:
- result screen
- retry flow
- chapter map with 2-5 connected test nodes
- save/load for stars and unlocks
- second goal type
- initial power-up path

Deferred:
- full 50-level content production
- advanced shell UI
- broad obstacle library
- tutorialization

---

## Recommended Immediate Next Action

Start implementation with **Sprint 1**, not full vertical slice production.
The fastest reliable path is:

1. create Unity project skeleton
2. implement authored single-level loading
3. implement board/input/swap/cascade core
4. add minimal move+goal closure
5. only then widen to map/persistence/power-ups

---

## Definition of Success for This Planning Round

This planning round is successful if:

- the implementation order is dependency-safe
- the first sprint is small enough to finish
- the first slice still resembles the real product architecture
- later vertical-slice systems can be added without rewriting the board spine
