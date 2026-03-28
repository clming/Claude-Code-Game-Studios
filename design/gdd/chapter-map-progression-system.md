# Chapter Map Progression System

> **Status**: In Design
> **Author**: User + Codex
> **Last Updated**: 2026-03-28
> **Implements Pillar**: Clean Commercial Puzzle Structure; Scoped Learnable Production

## Overview

The Chapter Map Progression System defines how MatchJoy organizes levels into a
chapter-based campaign, how players move between levels, and how victory results
unlock forward progress across the 50-level sample. It is responsible for the
structure of the chapter map, level-node progression rules, unlock behavior, and
the relationship between in-level completion and out-of-level campaign advance.

This system does not determine whether a player truly won a level; that truth is
provided by gameplay and results systems. Instead, it translates confirmed level
completion into durable campaign state: which nodes are available, which level
is current, what the player's best-known chapter progress is, and what the next
recommended action should be after a result screen.

For MVP, the chapter map must support:
- a fixed linear chapter sequence
- explicit level nodes within each chapter
- unlock-on-victory forward progression
- return from results into a sensible next map state

## Player Fantasy

This system supports the feeling that the player is traveling through a crafted,
rewarding candy-puzzle journey rather than replaying disconnected stages. The
map should make progress feel tangible: finishing a level should move the player
forward in a visible campaign, and each new chapter should feel like an earned
step deeper into the game.

The emotional target is forward momentum and structured anticipation. The player
should always know where they are, what they have completed, and what the next
level is. The campaign should feel inviting and clean, not like a menu maze.

## Detailed Design

### Core Rules

1. The campaign is organized into authored chapters containing authored level
   nodes. Each playable level in the sample must belong to exactly one chapter
   and one position within that chapter.

2. For MVP, chapter progression is linear. The player unlocks the next level by
   achieving `Victory` on the current level.

3. Levels may be replayed after completion, but replay access must not break the
   linear forward progression rule.

4. Unlock state is progression truth, not UI-only decoration. A level node is
   considered unlocked only when campaign progression data says it is unlocked.

5. For MVP, the default unlock rule is:
   - the first level in the campaign is unlocked from the start
   - each subsequent level unlocks when the immediately previous level is won
   - a later level may not be entered if it has not yet been unlocked

6. Chapters are primarily organizational and presentation groupings in MVP. They
   may shape pacing, visual theme, and node grouping, but they must not require
   separate meta resources or world-map mechanics unless later systems add them.

7. A completed level node must be able to display at least these progression
   states:
   - locked
   - unlocked but not yet won
   - completed
   - completed with recorded best stars

8. Returning from a victory result should route the player to a sensible next map
   context. For MVP, the preferred default is to focus or emphasize the newly
   unlocked next playable level if one exists.

9. Returning from a failure result must not alter unlock state. Failure may
   return the player to the current level node or map context, but it must not
   remove previously earned progress.

10. Campaign progression must be stable across sessions once persistence is added.
    Progression should key from stable level IDs and chapter ordering metadata,
    not scene names or transient runtime object references.

11. The chapter map must remain scope-conscious. It is not a social meta layer,
    live-ops route network, or renovation builder in MVP. Its job is to make the
    50-level sample feel structured and complete.

### States and Transitions

The map progression system tracks node availability and map-session focus state.

| State | Entry Condition | Exit Condition | Behavior |
|-------|----------------|----------------|----------|
| Map Uninitialized | Campaign/map state has not yet been loaded for the current player/session | Progression data and authored map data are available | No node should be treated as interactable yet |
| Map Ready | Chapter and node progression truth are available | Player enters a level, progression changes, or higher-priority shell state interrupts | Map can display node states and accept legal navigation/input |
| Level Launch Transition | Player selects an unlocked node and flow accepts level entry | Gameplay session takes over | Map hands off control to level session |
| Post-Victory Update | A level victory result is confirmed and campaign progression must advance | Node state and next focus are updated | Unlocks next level if applicable and records map-visible completion state |
| Post-Failure Return | A failed or abandoned session returns to map context | Player selects an action or re-enters a level | Map restores current progression state without losing unlocks |
| Chapter Complete Focus | The final level in a chapter is won | Next chapter focus is established or campaign end state is reached | Highlights chapter completion and advances the player's visible campaign position |

State transition rules:

1. The system begins in `Map Uninitialized` until authored map data and player
   progression state are available.
2. It enters `Map Ready` once node unlock state and completion state are known.
3. It enters `Level Launch Transition` only when the player selects a legal,
   unlocked level node.
4. It enters `Post-Victory Update` only after a confirmed victory result package
   is received.
5. It enters `Post-Failure Return` when a non-victory session exits back to map
   without progression gain.
6. It enters `Chapter Complete Focus` when the won level is the final level node
   in its chapter.
7. Any map focus state must eventually return to `Map Ready` for continued use.

### Interactions with Other Systems

1. **Results, Retry, and Star Rating System**
   - Provides confirmed level outcome packages, including victory state and best
     star data.
   - Responsibility boundary: results system confirms what happened; chapter map
     progression decides how the campaign advances visibly.

2. **MatchJoy Authoring System**
   - Provides stable level IDs, chapter ordering, and level sequence metadata.
   - Responsibility boundary: authoring defines campaign structure inputs; map
     progression applies runtime/player progression truth to them.

3. **Save/Load and Settings Persistence System**
   - Stores unlocked node state, completion state, and best-known progression.
   - Responsibility boundary: chapter map progression defines what campaign data
     must be saved; persistence stores it durably.

4. **Game Flow State System**
   - Routes between map, gameplay session, result screen, and shell states.
   - Responsibility boundary: flow owns navigation transitions; chapter map owns
     campaign-state interpretation and legal node entry.

5. **Core HUD and Goal Feedback System**
   - No direct runtime dependency during active play, but map progression uses the
     same level identity and completion truth that the HUD consumes in-session.

6. **Results, Retry, and Star Rating System**
   - Also provides the best-star package shown on completed nodes.

## Formulas

### Node Unlock Rule

```text
node_unlocked(i) =
  true, if i == first_campaign_node
  true, if previous_node(i) is completed with Victory
  false, otherwise
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| i | node index | valid campaign node range | authored map structure | Target node being evaluated |
| first_campaign_node | node index | one valid node | authored campaign structure | The starting node of the campaign |
| previous_node(i) | node index | valid predecessor node | authored campaign structure | Immediately preceding node in linear progression |
| node_unlocked(i) | bool | true/false | calculated | Whether the node is currently legal to enter |

### Chapter Completion Predicate

```text
chapter_complete = all(nodes_in_chapter are completed with Victory)
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| nodes_in_chapter | set | chapter node set | authored campaign structure | All nodes belonging to a chapter |
| chapter_complete | bool | true/false | calculated | Whether the current chapter is fully completed |

## Edge Cases

| Scenario | Expected Behavior | Rationale |
|----------|------------------|-----------|
| Player wins a level for the first time | Current node becomes completed and the next node unlocks if one exists | Victory should create visible forward momentum |
| Player replays an already completed level and loses | Existing completion and unlock state remain unchanged | Failure should not erase campaign progress |
| Player replays an already completed level and earns a better star result | Completion state remains completed and best star display may improve | Replay should support mastery without breaking progression |
| Player finishes the last level of a chapter | Current chapter is marked complete and the next chapter's first node becomes the next focus if another chapter exists | Chapter boundaries should feel meaningful and readable |
| Player finishes the final level of the whole sample | Campaign reaches a terminal completed state without trying to unlock a nonexistent next node | End-of-sample flow should remain stable |
| Player returns from failure to the map | Map focus returns to the current playable node or last-entered node without altering unlocks | Failure return should preserve orientation |
| Authoring data contains duplicate chapter order or node order conflicts | Validation fails before structured playtest | Campaign structure must be deterministic |
| A saved progression record references a level ID no longer present in authored campaign data | Development builds should surface migration error/warning rather than silently corrupting progression | Stable IDs are critical for campaign continuity |

## Dependencies

| System | Direction | Nature of Dependency |
|--------|-----------|---------------------|
| Results, Retry, and Star Rating System | This system depends on Results, Retry, and Star Rating System | Needs confirmed victory/failure and best-star results |
| MatchJoy Authoring System | This system depends on MatchJoy Authoring System | Needs chapter and node structure, stable IDs, and ordering |
| Save/Load and Settings Persistence System | Soft dependency in current draft, hard dependency once persistence is implemented | Needs durable storage for unlock and completion state |
| Game Flow State System | This system depends on Game Flow State System | Needs routing between map and gameplay contexts |

Dependency notes:

1. The most important truth source is `Results, Retry, and Star Rating System`.
2. The most important structural source is `MatchJoy Authoring System`.
3. This system should stay progression-focused and must not absorb shell-menu or
   monetization responsibilities in MVP.

## Tuning Knobs

| Parameter | Current Value | Safe Range | Effect of Increase | Effect of Decrease |
|-----------|--------------|------------|-------------------|-------------------|
| Levels Per Chapter | 10 recommended for 50-level sample | 5-15 | Makes chapter identity stronger and chapter completion less frequent | Makes chapter cadence faster and more segmented |
| Victory Return Focus Strength | High | Medium to high | Makes next playable node more obvious | Gives player a freer but less guided map experience |
| Replay Availability Visibility | Medium | Low to high | Makes replay/mastery options more obvious | Keeps attention on forward progression |
| Chapter Completion Emphasis | Moderate | Subtle to strong | Makes chapter milestones feel more rewarding | Keeps map flow faster and lighter |

## Visual/Audio Requirements

| Event | Visual Feedback | Audio Feedback | Priority |
|-------|----------------|---------------|----------|
| Node unlock | Newly unlocked node highlight/emphasis | Short unlock cue | High |
| Return from victory | Map focus or camera emphasis on next recommended node | Positive confirm cue | High |
| Chapter completion | Clear chapter-complete emphasis | Chapter-complete cue | Medium |
| Completed node with stars | Stable node state showing completion and best stars | None required | Medium |

## UI Requirements

| Information | Display Location | Update Frequency | Condition |
|-------------|-----------------|-----------------|-----------|
| Locked/unlocked/completed node state | Chapter map node visuals | On map load and after progression updates | Always on map |
| Best star state per completed node | Node badge/overlay | On map load and after improved result | For completed nodes |
| Current recommended next level | Focus/highlight state on map | After victory return or map load | When an obvious next node exists |
| Chapter grouping/title | Map chapter area/header | On map load | When chapter context is visible |

## Acceptance Criteria

- [ ] The system can represent a fixed 50-level campaign split into authored chapters.
- [ ] The first level is unlocked by default and later levels unlock only through authored progression rules.
- [ ] A victory result unlocks the correct next level when one exists.
- [ ] Replay of completed levels does not regress unlock state.
- [ ] Improved replay results can update best-star display without corrupting progression.
- [ ] Returning from results restores a sensible map focus state.
- [ ] Final chapter/final campaign completion is handled without invalid next-node references.
- [ ] Campaign structure depends on stable authored metadata rather than scene-name conventions.
- [ ] Performance: map progression updates are lightweight enough to feel immediate when returning from results.
- [ ] No unlock state is inferred only from visuals; progression truth exists in data/runtime state.

## Open Questions

| Question | Owner | Deadline | Resolution |
|----------|-------|----------|-----------|
| Should the 50-level sample be split as 5 chapters x 10 levels, or use another chapter cadence? | User | Before broad content production | 5 x 10 recommended, not yet locked |
| Should victory return automatically focus the next level only, or also offer an immediate `Next` launch path from the map? | User + future UX pass | Before implementation | Not yet decided |
| Should replay of completed levels be accessible from the same node tap flow as first-time play, or via a secondary action? | User + future UX pass | Before implementation | Not yet decided |
| Does the sample need a lightweight end-of-campaign celebration state beyond normal chapter completion focus? | User | Before final sample polish | Not yet decided |

Open question notes:

1. None of these questions block a clean MVP chapter map.
2. The default principle is clarity: forward progression should always be easier
   to understand than optional replay behavior.
