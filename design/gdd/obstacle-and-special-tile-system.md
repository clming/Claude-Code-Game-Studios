# Obstacle and Special Tile System

> **Status**: In Design
> **Author**: User + Codex
> **Last Updated**: 2026-03-28
> **Implements Pillar**: Clean Commercial Puzzle Structure; Juicy Match-3 Satisfaction

## Overview

The Obstacle and Special Tile System defines the non-standard board content that
adds structure, friction, and level variety to MatchJoy. It is responsible for
how blockers, overlays, and special objective-linked board elements exist on the
board, how they respond to valid gameplay events, and how they constrain or
modify the normal tile loop.

This system is not the same as the power-up system. Power-ups are rewards the
player earns through strong matching patterns. Obstacles and special tiles are
board-authored constraints or targets that shape what a level asks of the
player. For the current project scope, this system is the main source of
handcrafted level identity across 50 levels.

## Player Fantasy

This system supports the feeling that each level has a unique puzzle character.
The player should feel that obstacles change the meaning of an otherwise familiar
board, forcing them to think differently about positioning, timing, and target
priority.

A strong version of this system makes obstacles readable rather than annoying.
The player should feel challenged by them, not confused by hidden behavior. The
emotional target is purposeful friction: obstacles should slow or redirect the
player in interesting ways, while still feeling fair and understandable.

## Detailed Design

### Core Rules

1. The Obstacle and Special Tile System is responsible for authored board content
   that changes how cells behave, how progress is made, or how the board must be
   cleared beyond ordinary same-type tile matching.

2. For MVP, this system must support a limited, curated obstacle set rather than
   an open-ended catalog. The initial supported families are:
   - `Jelly Overlay`: an overlay goal layer that is removed when affected by valid clears according to its rules
   - `Blocker Tile`: a board occupant or obstacle that prevents normal tile use until removed
   - `Frozen / Locked Objective Content`: authored special content tied to goal progression, including frozen ingredient-linked states where applicable

3. Obstacles and special tiles must use explicit board-layer contracts from the
   Board Grid State System. They may exist as:
   - overlay content on an otherwise playable cell
   - occupying content that blocks normal tile presence
   - objective-linked special content with explicit coexistence rules
   They must not rely on undocumented scene-only behavior.

4. Every obstacle type must define, at minimum:
   - where it can exist
   - whether it coexists with a primary tile
   - what kinds of events affect it
   - whether it blocks swap, fall, refill, or clear propagation
   - what state change or destruction condition removes or advances it

5. Jelly Overlay rules for MVP are:
   - jelly may coexist with a primary tile on a playable cell
   - jelly is removed when the covered cell is directly cleared by a valid match, cascade clear, line clear, area blast, or rainbow-clear effect
   - adjacency alone does not clear jelly unless the affecting power-up or clear explicitly targets the covered cell
   - clearing the tile on a jelly-covered cell counts as clearing jelly only when that cell itself is included in the approved clear payload

6. Blocker Tile rules for MVP are:
   - a blocker occupies a cell in a way that prevents ordinary primary-tile usage unless explicitly defined otherwise
   - blocker removal must be driven by approved gameplay events, not by arbitrary overwrite
   - a blocker must explicitly define whether power-up effects can damage or remove it

7. Frozen / Locked Objective Content rules for MVP are:
   - this content must explicitly define whether it coexists with a primary tile or replaces it
   - it must define what event counts as release, unlock, damage, or completion
   - if it participates in a level goal, the goal system must be able to observe its progress deterministically

8. Obstacles must never mutate the board outside approved system contracts. If an
   obstacle changes state, that change must be emitted as a legal board mutation
   or explicit obstacle-state transition.

9. Obstacle interactions must be deterministic. For a given board state and event
   payload, the same obstacle response must occur every time.

10. Obstacle state changes may be triggered by:
   - direct match clears
   - cascade clears
   - power-up activations
   - explicit objective events
   Each obstacle type must declare which of these sources affect it.

11. The system must keep obstacle rules readable. If two obstacle types interact,
   the resolution order must be explicit rather than hidden in implementation side
   effects.

12. This system must remain scope-conscious. The MVP goal is a small set of
   readable obstacle behaviors that support strong handcrafted levels, not a large
   taxonomy of one-off gimmicks.

### States and Transitions

The Obstacle and Special Tile System models obstacle lifecycle per obstacle
instance on the board.

| State | Entry Condition | Exit Condition | Behavior |
|-------|----------------|----------------|----------|
| Present | The obstacle or special content exists on the board in its current authored or runtime state | It is damaged, transformed, released, or removed by a valid event | The obstacle affects board play according to its rules |
| Progressing | A valid event has partially advanced the obstacle but not yet removed/completed it | Another valid event continues progress, or it reaches completion/removal | Multi-step obstacle states can visibly track intermediate progress |
| Removed / Completed | The obstacle has been fully cleared, released, or satisfied | A new instance is authored in another level or session | The board no longer treats this instance as active obstacle content |

State transition rules:

1. Every obstacle instance begins in `Present` when initialized from authored level data unless its type supports a more specific starting substate.
2. A valid affecting event may move the obstacle from `Present` to `Progressing` if it supports partial damage or staged release.
3. A valid affecting event may move the obstacle directly from `Present` to `Removed / Completed` if it is a one-step obstacle.
4. Once in `Removed / Completed`, the obstacle no longer blocks, overlays, or contributes active board behavior.
5. Any state transition must be driven by an approved board or objective event.

### Interactions with Other Systems

1. **MatchJoy Authoring System**
   - This system consumes authored obstacle placements and configuration.
   - Data flowing in: obstacle type, coordinate, state configuration, coexistence flags, and level-specific rules.
   - Responsibility boundary: authoring defines what obstacle exists and where; obstacle logic defines how it behaves at runtime.

2. **Board Grid State System**
   - This system depends on the board's topology and occupancy-layer contracts.
   - Data flowing in: legal cell types, layer compatibility, occupancy truth, and mutation rules.
   - Data flowing out: obstacle occupancy and overlay state changes.
   - Responsibility boundary: board state defines how obstacle content can exist; obstacle logic defines what that content means.

3. **Tile Swap and Match Resolution System**
   - This system may consume obstacle-defined swappability or matchability constraints.
   - Data flowing out: whether a cell remains swappable, occupiable, or match-relevant.
   - Responsibility boundary: swap/match evaluates legal actions using obstacle-aware board truth; obstacles do not override swap resolution by hidden rules.

4. **Cascade, Refill, and Determinism System**
   - This system may consume obstacle-defined blocking and refill interaction rules.
   - Data flowing out: whether a cell blocks falling, blocks refill entry, or changes state during cascades.
   - Responsibility boundary: cascade/refill owns movement; obstacle logic defines how obstacle-bearing cells respond to that movement.

5. **Power-Up Creation and Activation System**
   - This system must know whether and how power-up effects interact with obstacles.
   - Data flowing in: activation categories and target payloads.
   - Data flowing out: obstacle damage, removal, or immunity outcomes.
   - Responsibility boundary: power-up logic defines intended effect pattern; obstacle logic defines the obstacle's response to that effect.

6. **Level Goal and Move Limit System**
   - This system provides obstacle-linked progress events for goal tracking.
   - Data flowing out: jelly cleared, blocker removed if goal-relevant, ingredient released, or equivalent completion signals.
   - Responsibility boundary: obstacle logic tracks its own state; goal logic interprets which state changes count toward victory.

7. **Frozen Ingredient Objective System**
   - This system may own detailed ingredient-specific objective rules while still relying on the obstacle framework for board presence.
   - Responsibility boundary: obstacle/special-tile logic represents the board-side state contract; frozen ingredient logic can extend the objective semantics where needed.

## Formulas

### Obstacle Affect Predicate

```text
obstacle_is_affected = incoming_event_type is in obstacle_supported_event_set
```

### Goal-Relevant Completion Predicate

```text
goal_relevant_obstacle_complete = obstacle_state == RemovedOrCompleted
```

### Coexistence Predicate

```text
can_coexist = cell_layer_contract_allows(primary_tile, obstacle_type)
```

## Edge Cases

| Scenario | Expected Behavior | Rationale |
|----------|------------------|-----------|
| Jelly exists on a cell whose tile is cleared | The jelly follows its explicit rule set rather than disappearing automatically unless that is the defined behavior | Overlay behavior must be explicit |
| A blocker occupies a cell targeted by a normal swap | The swap is rejected if blocker rules make the cell non-swappable | Obstacles must affect interaction through declared contracts |
| A power-up affects a blocker that is immune to that effect | The blocker remains unchanged and the interaction resolves deterministically | Immunity rules must be readable and stable |
| A falling tile would enter a cell whose obstacle forbids occupancy | The move is blocked or redirected according to explicit obstacle rules | Board legality must be preserved |
| An obstacle uses partial-damage states | Progress is tracked deterministically and not skipped by duplicate events unless rules explicitly allow it | Multi-step obstacles need stable progression |
| Frozen objective content is both goal-relevant and board-blocking | The system exposes both facts without conflating them | Goal semantics and board semantics must stay separable |
| Two obstacle rules would appear to conflict on one event | One explicit resolution order applies | Hidden precedence creates bugs and confusion |

## Dependencies

| System | Direction | Nature of Dependency |
|--------|-----------|---------------------|
| MatchJoy Authoring System | This system depends on MatchJoy Authoring System | Obstacle placements and configuration originate there |
| Board Grid State System | This system depends on Board Grid State System | Obstacle existence depends on board-layer and occupancy contracts |
| Tile Swap and Match Resolution System | Tile Swap and Match Resolution System depends on this system indirectly | Obstacles affect swappability and matchability through board truth |
| Cascade, Refill, and Determinism System | Cascade, Refill, and Determinism System depends on this system indirectly | Obstacles may block movement, refill, or react during cascades |
| Power-Up Creation and Activation System | Power-Up Creation and Activation System depends on this system | Activation results need obstacle interaction rules |
| Level Goal and Move Limit System | Level Goal and Move Limit System depends on this system | Goal progress may depend on obstacle completion or removal |
| Frozen Ingredient Objective System | Frozen Ingredient Objective System overlaps with this system | Ingredient-related content may extend obstacle semantics |

Dependency notes:

1. This system is the main source of handcrafted level variation in the current scope.
2. Its biggest risk is overgrowing into too many bespoke one-off mechanics.
3. The most important contract to preserve is explicit coexistence and interaction behavior per obstacle type.

## Tuning Knobs

| Parameter | Current Value | Safe Range | Effect of Increase | Effect of Decrease |
|-----------|--------------|------------|-------------------|-------------------|
| Jelly Density | Per level authored | Low to moderate in MVP | Increases board cleanup pressure and layered goals | Makes levels more direct and less layered |
| Blocker Density | Per level authored | Low to moderate in MVP | Increases friction and routing challenge | Makes boards more open and forgiving |
| Multi-Step Obstacle Depth | Minimal in MVP | 1-2 states | More stages increase depth but add cognitive load | Fewer stages improve readability |
| Obstacle Variety Per Chapter | Curated | 1-3 main types | More variety increases novelty but risks overload | Lower variety improves learnability |

## Visual/Audio Requirements

| Event | Visual Feedback | Audio Feedback | Priority |
|-------|----------------|---------------|----------|
| Jelly progress/removal | Clear overlay reduction/removal feedback | Positive cleanup cue | High |
| Blocker hit or removal | Distinct damage/removal feedback | Impact cue | High |
| Frozen/locked content progress | Readable state-change feedback | Progress cue | High |
| Obstacle immunity or no effect | Clear resisted-feedback when needed | Subtle reject cue | Medium |

## UI Requirements

| Information | Display Location | Update Frequency | Condition |
|-------------|-----------------|-----------------|-----------|
| Obstacle-linked goal status | In-level HUD | On relevant progress events | Goal-relevant obstacles |
| Obstacle debug state | Developer overlay/log | On demand | Debug builds |
| Obstacle coexistence info | Inspector/debug tooling | On demand | Development workflows |

## Acceptance Criteria

- [ ] MVP obstacle families are explicitly defined and limited in scope.
- [ ] Every obstacle type defines coexistence, trigger sources, and removal/completion rules.
- [ ] Obstacle behavior is expressed through board contracts rather than hidden scene behavior.
- [ ] Goal-relevant obstacle progress can be consumed deterministically by the goal system.
- [ ] Obstacle interactions with swaps, cascades, and power-ups are explicit.
- [ ] The system supports readable handcrafted level variation without requiring dozens of bespoke mechanics.

## Open Questions

| Question | Owner | Deadline | Resolution |
|----------|-------|----------|-----------|
| What exact jelly rule should MVP use: clear when matched on-cell, clear when affected by any adjacent blast/line, or another explicit rule? | User + tuning pass | Before implementation | Locked for MVP: jelly clears only when the covered cell itself is included in a valid clear payload; broader variants are future tuning options |
| Which blocker family should be the first true non-overlay obstacle in MVP? | User | Before obstacle implementation | Not yet decided |
| Should frozen ingredient-linked content live entirely here or split more deeply with Frozen Ingredient Objective System later? | User + Frozen Ingredient Objective System | Before implementation | Locked for MVP: keep board-state presence here and let Frozen Ingredient Objective System define objective semantics |
| Do any obstacle types in MVP block refill entry, or should refill blocking wait until later complexity tiers? | User | Before implementation | Default assumption is most MVP obstacles do not create exotic refill rules |



