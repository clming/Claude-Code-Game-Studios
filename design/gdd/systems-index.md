# Systems Index: Jelly Candy Workshop

> **Status**: Draft
> **Created**: 2026-03-28
> **Last Updated**: 2026-03-28
> **Source Concept**: design/gdd/game-concept.md

---

## Overview

Jelly Candy Workshop is a single-player mobile match-3 puzzle game, so its
mechanical scope is centered on one highly readable level loop rather than a
wide feature set. The project needs a small number of foundational systems that
make board play deterministic and satisfying, a compact progression structure
that supports 50 handcrafted levels, and a thin presentation layer that makes
the game feel commercially polished without adding live-service scope. The
systems below are prioritized against the concept's three pillars: juicy
match-3 satisfaction, clean commercial puzzle structure, and scoped learnable
production.

---

## Systems Enumeration

| # | System Name | Category | Priority | Status | Design Doc | Depends On |
|---|-------------|----------|----------|--------|------------|------------|
| 1 | Board Grid State System | Core | MVP | In Design | design/gdd/board-grid-state-system.md | - |
| 2 | Tile Swap and Match Resolution System | Gameplay | MVP | In Design | design/gdd/tile-swap-and-match-resolution-system.md | Board Grid State System |
| 3 | Cascade, Refill, and Determinism System | Gameplay | MVP | In Design | design/gdd/cascade-refill-and-determinism-system.md | Board Grid State System, Tile Swap and Match Resolution System |
| 4 | Level Goal and Move Limit System | Gameplay | MVP | In Design | design/gdd/level-goal-and-move-limit-system.md | Board Grid State System, Tile Swap and Match Resolution System, Cascade, Refill, and Determinism System |
| 5 | Power-Up Creation and Activation System | Gameplay | MVP | In Design | design/gdd/power-up-creation-and-activation-system.md | Board Grid State System, Tile Swap and Match Resolution System, Cascade, Refill, and Determinism System |
| 6 | MatchJoy Authoring System (inferred) | Core | MVP | In Design / Reviewed Once | design/gdd/matchjoy-authoring-system.md | Board Grid State System |
| 7 | Game Flow State System (inferred) | Core | MVP | In Design | design/gdd/game-flow-state-system.md | - |
| 8 | Input and Tile Selection System (inferred) | Core | MVP | In Design | design/gdd/input-and-tile-selection-system.md | Game Flow State System, Board Grid State System |
| 9 | Core HUD and Goal Feedback System (inferred) | UI | MVP | In Design | design/gdd/core-hud-and-goal-feedback-system.md | Level Goal and Move Limit System, Power-Up Creation and Activation System, Game Flow State System |
| 10 | Juice Feedback System (animation, VFX, SFX timing) (inferred) | Audio | MVP | Not Started | design/gdd/juice-feedback-system.md | Tile Swap and Match Resolution System, Cascade, Refill, and Determinism System, Power-Up Creation and Activation System |
| 11 | Chapter Map Progression System | Progression | Vertical Slice | In Design | design/gdd/chapter-map-progression-system.md | Game Flow State System, Level Goal and Move Limit System |
| 12 | Obstacle and Special Tile System (inferred) | Gameplay | Vertical Slice | In Design | design/gdd/obstacle-and-special-tile-system.md | Board Grid State System, Cascade, Refill, and Determinism System, Level Goal and Move Limit System |
| 13 | Level Difficulty Curve and Content Pacing System (inferred) | Progression | Vertical Slice | Not Started | design/gdd/level-difficulty-curve-and-content-pacing-system.md | MatchJoy Authoring System, Level Goal and Move Limit System, Obstacle and Special Tile System, Power-Up Creation and Activation System |
| 14 | Results, Retry, and Star Rating System (inferred) | UI | Vertical Slice | In Design | design/gdd/results-retry-and-star-rating-system.md | Level Goal and Move Limit System, Game Flow State System, Chapter Map Progression System |
| 15 | Menu and Session Shell UI System (inferred) | UI | Vertical Slice | Not Started | design/gdd/menu-and-session-shell-ui-system.md | Game Flow State System, Chapter Map Progression System |
| 16 | Save/Load and Settings Persistence System (inferred) | Persistence | Vertical Slice | In Design | design/gdd/save-load-and-settings-persistence-system.md | Game Flow State System, Chapter Map Progression System, Results, Retry, and Star Rating System |
| 17 | Frozen Ingredient Objective System | Gameplay | Vertical Slice | In Design | design/gdd/frozen-ingredient-objective-system.md | Board Grid State System, Tile Swap and Match Resolution System, Cascade, Refill, and Determinism System, Level Goal and Move Limit System |
| 18 | Economy-Free Booster Scope Decision System (inferred placeholder) | Meta | Alpha | Not Started | design/gdd/booster-scope-decision-system.md | Level Goal and Move Limit System, Chapter Map Progression System, Results, Retry, and Star Rating System |
| 19 | Onboarding and Tutorial Messaging System (inferred) | Meta | Alpha | Not Started | design/gdd/onboarding-and-tutorial-messaging-system.md | Game Flow State System, Input and Tile Selection System, Core HUD and Goal Feedback System, Chapter Map Progression System |
| 20 | Audio Presentation System (music, mixing, routing) (inferred) | Audio | Alpha | Not Started | design/gdd/audio-presentation-system.md | Game Flow State System, Juice Feedback System |
| 21 | Accessibility and Assist Options System (inferred) | Meta | Alpha | Not Started | design/gdd/accessibility-and-assist-options-system.md | Input and Tile Selection System, Core HUD and Goal Feedback System, Save/Load and Settings Persistence System |
| 22 | Analytics and Telemetry Hooks System (inferred) | Meta | Full Vision | Not Started | design/gdd/analytics-and-telemetry-hooks-system.md | Game Flow State System, Chapter Map Progression System, Results, Retry, and Star Rating System |
| 23 | Live Ops / Monetization System | Meta | Full Vision | Cut / Not Planned | - | - |
| 24 | Social / Networking System | Meta | Full Vision | Cut / Not Planned | - | - |

---

## Categories

| Category | Description | Typical Systems |
|----------|-------------|-----------------|
| **Core** | Foundation systems everything else plugs into | Board state, game flow, input handling, level authoring |
| **Gameplay** | The systems that define puzzle play and win/loss conditions | Swap resolution, cascades, goals, power-ups, obstacles |
| **Progression** | How the player advances across sessions and content | Chapter map, level sequencing, difficulty pacing |
| **Persistence** | Save state and continuity between sessions | Save/load, settings, progression persistence |
| **UI** | Player-facing displays and session flow surfaces | HUD, results, menus, shell screens |
| **Audio** | Sound and feel-supporting presentation systems | Juice timing, music routing, SFX layers |
| **Meta** | Systems outside the immediate board loop | Tutorials, accessibility, analytics, scope placeholders |

---

## Priority Tiers

| Tier | Definition | Target Milestone | Design Urgency |
|------|------------|------------------|----------------|
| **MVP** | Required to test whether the core match-3 loop is playable, readable, and satisfying | First playable prototype | Design FIRST |
| **Vertical Slice** | Required to show one complete commercial-style slice with map flow, representative goals, and polished session loop | Vertical slice / demo | Design SECOND |
| **Alpha** | Required to support the full planned 50-level sample in rough form | Alpha milestone | Design THIRD |
| **Full Vision** | Nice-to-have polish, instrumentation, or future-facing systems beyond the current sample scope | Beta / Release | Design as needed |

---

## Dependency Map

Systems are sorted by dependency order. Work from top to bottom when writing
GDDs and when planning implementation.

### Foundation Layer (no dependencies)

1. Board Grid State System - defines the authoritative board model every puzzle
   rule depends on.
2. Game Flow State System (inferred) - defines transitions between menu, map,
   level start, active play, fail, and win states.

### Core Layer (depends on foundation)

1. MatchJoy Authoring System (inferred) - depends on: Board Grid
   State System
2. Input and Tile Selection System (inferred) - depends on: Game Flow State
   System, Board Grid State System
3. Tile Swap and Match Resolution System - depends on: Board Grid State System

### Feature Layer (depends on core)

1. Cascade, Refill, and Determinism System - depends on: Board Grid State
   System, Tile Swap and Match Resolution System
2. Level Goal and Move Limit System - depends on: Board Grid State System, Tile
   Swap and Match Resolution System, Cascade, Refill, and Determinism System
3. Power-Up Creation and Activation System - depends on: Board Grid State
   System, Tile Swap and Match Resolution System, Cascade, Refill, and
   Determinism System
4. Obstacle and Special Tile System (inferred) - depends on: Board Grid State
   System, Cascade, Refill, and Determinism System, Level Goal and Move Limit
   System
5. Frozen Ingredient Objective System - depends on: Board Grid State System,
   Tile Swap and Match Resolution System, Cascade, Refill, and Determinism
   System, Level Goal and Move Limit System
6. Chapter Map Progression System - depends on: Game Flow State System, Level
   Goal and Move Limit System
7. Level Difficulty Curve and Content Pacing System (inferred) - depends on:
   MatchJoy Authoring System, Level Goal and Move Limit System,
   Obstacle and Special Tile System, Power-Up Creation and Activation System

### Presentation Layer (depends on features)

1. Core HUD and Goal Feedback System (inferred) - depends on: Level Goal and
   Move Limit System, Power-Up Creation and Activation System, Game Flow State
   System
2. Juice Feedback System (animation, VFX, SFX timing) (inferred) - depends on:
   Tile Swap and Match Resolution System, Cascade, Refill, and Determinism
   System, Power-Up Creation and Activation System
3. Results, Retry, and Star Rating System (inferred) - depends on: Level Goal
   and Move Limit System, Game Flow State System, Chapter Map Progression
   System
4. Menu and Session Shell UI System (inferred) - depends on: Game Flow State
   System, Chapter Map Progression System
5. Save/Load and Settings Persistence System (inferred) - depends on: Game Flow
   State System, Chapter Map Progression System, Results, Retry, and Star
   Rating System
6. Audio Presentation System (music, mixing, routing) (inferred) - depends on:
   Game Flow State System, Juice Feedback System

### Polish Layer (depends on everything)

1. Onboarding and Tutorial Messaging System (inferred) - depends on: Game Flow
   State System, Input and Tile Selection System, Core HUD and Goal Feedback
   System, Chapter Map Progression System
2. Accessibility and Assist Options System (inferred) - depends on: Input and
   Tile Selection System, Core HUD and Goal Feedback System, Save/Load and
   Settings Persistence System
3. Economy-Free Booster Scope Decision System (inferred placeholder) - depends
   on: Level Goal and Move Limit System, Chapter Map Progression System,
   Results, Retry, and Star Rating System
4. Analytics and Telemetry Hooks System (inferred) - depends on: Game Flow
   State System, Chapter Map Progression System, Results, Retry, and Star
   Rating System

---

## Recommended Design Order

The order below combines dependency order with milestone priority. Systems that
define board truth and resolution rules come first because almost every other
decision depends on them being unambiguous.

| Order | System | Priority | Layer | Agent(s) | Est. Effort |
|-------|--------|----------|-------|----------|-------------|
| 1 | Board Grid State System | MVP | Foundation | game-designer, unity-specialist | M |
| 2 | Game Flow State System (inferred) | MVP | Foundation | game-designer | S |
| 3 | MatchJoy Authoring System (inferred) | MVP | Core | game-designer, tools-programmer | M |
| 4 | Input and Tile Selection System (inferred) | MVP | Core | game-designer, unity-specialist | S |
| 5 | Tile Swap and Match Resolution System | MVP | Core | game-designer, systems-designer | L |
| 6 | Cascade, Refill, and Determinism System | MVP | Feature | game-designer, systems-designer | L |
| 7 | Level Goal and Move Limit System | MVP | Feature | game-designer | M |
| 8 | Power-Up Creation and Activation System | MVP | Feature | game-designer, systems-designer | L |
| 9 | Core HUD and Goal Feedback System (inferred) | MVP | Presentation | ux-designer, game-designer | M |
| 10 | Juice Feedback System (animation, VFX, SFX timing) (inferred) | MVP | Presentation | technical-artist, sound-designer | M |
| 11 | Chapter Map Progression System | Vertical Slice | Feature | game-designer, ux-designer | M |
| 12 | Obstacle and Special Tile System (inferred) | Vertical Slice | Feature | game-designer | M |
| 13 | Frozen Ingredient Objective System | Vertical Slice | Feature | game-designer | S |
| 14 | Results, Retry, and Star Rating System (inferred) | Vertical Slice | Presentation | ux-designer, game-designer | S |
| 15 | Menu and Session Shell UI System (inferred) | Vertical Slice | Presentation | ux-designer | S |
| 16 | Save/Load and Settings Persistence System (inferred) | Vertical Slice | Presentation | game-designer, unity-specialist | M |
| 17 | Level Difficulty Curve and Content Pacing System (inferred) | Vertical Slice | Feature | level-designer, game-designer | L |
| 18 | Onboarding and Tutorial Messaging System (inferred) | Alpha | Polish | ux-designer, game-designer | M |
| 19 | Audio Presentation System (music, mixing, routing) (inferred) | Alpha | Presentation | audio-director, sound-designer | S |
| 20 | Accessibility and Assist Options System (inferred) | Alpha | Polish | accessibility-specialist, ux-designer | M |
| 21 | Economy-Free Booster Scope Decision System (inferred placeholder) | Alpha | Polish | producer, game-designer | S |
| 22 | Analytics and Telemetry Hooks System (inferred) | Full Vision | Polish | analytics-engineer, producer | S |

Effort legend: S = 1 focused design session, M = 2-3 sessions, L = 4+ sessions.

---

## Circular Dependencies

- Board Grid State System <-> MatchJoy Authoring System: authoring
  must understand the board schema, while the board schema is partly shaped by
  what level data needs to express. Resolve by defining board runtime data first
  and treating authoring as a serializer/editor layer over that schema.
- Chapter Map Progression System <-> Results, Retry, and Star Rating System:
  the map unlock flow needs results output, while results screens need map
  context to display unlocks and progression. Resolve by making results emit a
  clean completion payload that the map system consumes.
- Core HUD and Goal Feedback System <-> Level Goal and Move Limit System: goal
  logic defines what must be shown, but HUD readability may constrain goal
  complexity. Resolve by finishing goal-state data contracts before final HUD
  layout decisions.

---

## High-Risk Systems

| System | Risk Type | Risk Description | Mitigation |
|--------|-----------|-----------------|------------|
| Tile Swap and Match Resolution System | Design | If swap validation or match detection rules feel inconsistent, the entire puzzle loop loses trust | Prototype early with debug visualization and deterministic test boards |
| Cascade, Refill, and Determinism System | Technical | Chain reactions, refill randomness, and board solvability can create hidden bugs or unfair-feeling outcomes | Build automated board-state tests and log every resolution step during prototype |
| Power-Up Creation and Activation System | Design | Combo rules can quickly become unreadable or overpowered, hurting Pillar 1 clarity | Limit initial power-up families, test pairwise interactions first, defer exotic combos |
| MatchJoy Authoring System (inferred) | Scope | If authoring is clumsy, producing 50 handcrafted levels becomes the real bottleneck | Standardize a lightweight data schema and minimal editor workflow before content production |
| Obstacle and Special Tile System (inferred) | Scope | Too many obstacles can bloat scope and reduce board readability | Cap obstacle families for the sample and tie each one to a distinct learning beat |
| Level Difficulty Curve and Content Pacing System (inferred) | Design | Poor pacing can make the sample feel flat or unfair even if core mechanics are solid | Define chapter-by-chapter teaching goals and playtest every 5-10 levels as a set |
| Core HUD and Goal Feedback System (inferred) | UX | Mixed goals may become visually noisy on mobile if the HUD competes with the board | Keep HUD lean, reserve the largest visual emphasis for board readability, run device-size reviews |
| Save/Load and Settings Persistence System (inferred) | Technical | Progress loss or retry-state bugs damage trust quickly in mobile puzzle games | Save only durable progression state first, avoid serializing transient board state until needed |

---

## Progress Tracker

| Metric | Count |
|--------|-------|
| Total systems identified | 24 |
| Actively planned systems | 22 |
| Explicitly cut / out-of-scope systems | 2 |
| Design docs started | 14 |
| Design docs reviewed | 1 |
| Design docs approved | 0 |
| MVP systems designed | 9/10 |
| Vertical Slice systems designed | 5/7 |
| Alpha systems designed | 0/4 |

Notes:
- Existing system design docs currently drafted: `matchjoy-authoring-system`, `board-grid-state-system`, `tile-swap-and-match-resolution-system`, `cascade-refill-and-determinism-system`, `level-goal-and-move-limit-system`, `power-up-creation-and-activation-system`, `game-flow-state-system`, `input-and-tile-selection-system`, `core-hud-and-goal-feedback-system`, `obstacle-and-special-tile-system`, `frozen-ingredient-objective-system`, `results-retry-and-star-rating-system`, `chapter-map-progression-system`, `save-load-and-settings-persistence-system`.
- `design/gdd/matchjoy-authoring-system.md` has completed one design-review pass.
- Remaining highest-leverage gaps are a focused cross-document review pass, `Menu and Session Shell UI System`, `Juice Feedback System`, and `Level Difficulty Curve and Content Pacing System`.

---

## Next Steps

- [ ] Refresh dependency and naming consistency across the drafted core/vertical-slice GDDs
- [ ] Run focused `/design-review` passes on the drafted gameplay chain before implementation
- [ ] Review cross-document consistency for flow, input, result, progression, and persistence write timing
- [ ] Design `Menu and Session Shell UI System` if you want a clearer non-level wrapper before implementation
- [ ] Design `Juice Feedback System` before polish/prototype feel work
- [ ] Prototype the full core loop early: swap -> match -> clear -> cascade -> refill -> goal -> result -> map






