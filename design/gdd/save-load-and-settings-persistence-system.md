# Save/Load and Settings Persistence System

> **Status**: In Design
> **Author**: User + Codex
> **Last Updated**: 2026-03-28
> **Implements Pillar**: Clean Commercial Puzzle Structure; Scoped Learnable Production

## Overview

The Save/Load and Settings Persistence System defines what durable player state
MatchJoy stores across sessions, when that state is written or read, and how
persistent data is kept separate from transient gameplay runtime state. It is
responsible for saving campaign progression, best-known level performance,
relevant result records, and lightweight player settings needed for a stable
mobile sample experience.

This system does not serialize live board simulation for mid-session resume in
MVP. Instead, it focuses on durable state that should survive app restarts or
new sessions: unlocked progression, completed levels, best stars, and a small
set of player preferences. Its purpose is to make the sample feel like a real
product without expanding into complex cloud sync or full session restoration.

For MVP, the persistence system must support:
- loading durable progression before map usage
- saving progression after meaningful victory-result updates
- storing best stars per completed level
- storing lightweight user settings such as audio and vibration preferences
- keeping transient board/session state out of durable save data

## Player Fantasy

This system supports the feeling that progress matters and the game respects the
player's time. When the player returns to MatchJoy, their unlocked levels,
completed stars, and basic preferences should still be there. Losing a session
or closing the game should not make the game feel flimsy or unreliable.

The emotional target is trust. Players should feel that victories stick,
settings stay the way they left them, and retrying a level is a fresh choice,
not a gamble with saved progress.

## Detailed Design

### Core Rules

1. This system is the authoritative owner of durable player-facing state across
   sessions in MVP.

2. Persistent data must be separated from transient runtime state. For MVP, the
   system must not serialize live board occupancy, unresolved cascades, active
   selection state, or half-finished level sessions.

3. At minimum, persistent progression data must support:
   - unlocked level IDs
   - completed level IDs
   - best stars earned per completed level
   - last meaningful chapter-map focus context if desired by shell flow

4. At minimum, persistent settings data must support:
   - master audio enabled/disabled or volume setting
   - music enabled/disabled or volume setting
   - SFX enabled/disabled or volume setting
   - vibration enabled/disabled

5. Persistent data keys must derive from stable authored identities such as level
   IDs and chapter metadata, not from scene names, runtime instance IDs, or UI
   object names.

6. Progression-saving writes must occur only after authoritative outcome truth is
   available. In MVP, the most important write point is after a confirmed victory
   result has been evaluated and merged with existing best-result data.

7. Failure outcomes must not remove previously saved progression or overwrite a
   better historical result.

8. Retry does not create a save checkpoint in MVP. Retry always rebuilds from
   authored level data and current durable progression, not from session-local
   state snapshots.

9. Save/load must be robust against repeated app launches. If no save data exists,
   the system must create or expose a valid default profile state without
   corrupting campaign structure.

10. The system must support version-safe evolution at a simple level. If save data
    schema changes during development, the system should fail clearly in
    development or use a controlled migration/defaulting strategy rather than
    silently producing broken progression.

11. The persistence system must remain scope-conscious for the sample. MVP does
    not require cloud sync, multi-slot accounts, remote conflict resolution, or
    background session resume of active board state.

### States and Transitions

The persistence system uses a lightweight lifecycle around app/session startup
and meaningful save events.

| State | Entry Condition | Exit Condition | Behavior |
|-------|----------------|----------------|----------|
| Uninitialized | App/session has not yet loaded or created durable player data | Default or existing save profile is available | Persistence-dependent systems should not assume progression truth yet |
| Profile Ready | Durable data is loaded or defaulted successfully | A save-worthy mutation occurs or app/session shuts down | Exposes stable progression/settings state for map, results, and shell systems |
| Save Pending | A meaningful durable-state change has been requested | Save write succeeds or fails safely | Coalesces and prepares the current durable state for write |
| Saving | A write is actively being performed | Write completes successfully or fails safely | Prevents contradictory overlapping write assumptions |
| Save Failed | A durable write failed in development or runtime | Recovery strategy runs or a later save succeeds | Surfaces error state without corrupting in-memory durable truth |

State transition rules:

1. The system begins in `Uninitialized` on app/session start.
2. It enters `Profile Ready` after existing save data is loaded or a valid default
   profile is created.
3. It enters `Save Pending` when a durable mutation is committed, such as updated
   best stars, newly unlocked levels, or changed settings.
4. It enters `Saving` when the system begins writing the latest durable state.
5. It returns to `Profile Ready` after a successful write.
6. It enters `Save Failed` if a write cannot be completed safely.
7. Recovery from `Save Failed` must preserve the last known valid in-memory state
   where possible and avoid writing partial corruption.

### Interactions with Other Systems

1. **Results, Retry, and Star Rating System**
   - Provides confirmed victory outcome, star result, and best-result merge input.
   - Responsibility boundary: results decides what outcome package exists;
     persistence stores the durable consequence.

2. **Chapter Map Progression System**
   - Consumes saved unlock/completion state and may request progression updates
     after victory.
   - Responsibility boundary: map progression defines campaign truth semantics;
     persistence stores them durably.

3. **Game Flow State System**
   - Needs durable profile data ready before stable map/shell usage and may define
     safe load/save transition boundaries.
   - Responsibility boundary: flow owns top-level timing; persistence owns data
     readiness and durable writes.

4. **MatchJoy Authoring System**
   - Provides stable level IDs and chapter structure that persistence references.
   - Responsibility boundary: authoring defines identity; persistence stores
     player-specific state keyed by that identity.

5. **Audio Presentation System**
   - Consumes saved audio settings such as music/SFX preference.
   - Responsibility boundary: audio system applies settings; persistence stores
     them durably.

6. **Accessibility and Assist Options System**
   - May later consume saved assist settings once that system exists.
   - Responsibility boundary: assist systems define behavior; persistence stores
     player preference state.

## Formulas

### Best Result Merge

```text
saved_best_stars = max(previous_best_stars, current_victory_stars)
level_completed = previous_level_completed or current_outcome_is_victory
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| previous_best_stars | int | 0 to 3 | saved data | Previously stored best star result |
| current_victory_stars | int | 0 to 3 | runtime result | Current victory star result |
| saved_best_stars | int | 0 to 3 | calculated | Best stars to persist after merge |
| previous_level_completed | bool | true/false | saved data | Previously stored completion state |
| current_outcome_is_victory | bool | true/false | runtime result | Whether current session ended in victory |
| level_completed | bool | true/false | calculated | Final durable completion flag |

### Next Unlock Merge Rule

```text
next_level_unlocked = previous_next_unlock or current_level_victory_unlocks_next
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| previous_next_unlock | bool | true/false | saved data | Whether the next level was already unlocked |
| current_level_victory_unlocks_next | bool | true/false | runtime progression update | Whether current confirmed victory unlocks the next level |
| next_level_unlocked | bool | true/false | calculated | Final durable unlock state after merge |

## Edge Cases

| Scenario | Expected Behavior | Rationale |
|----------|------------------|-----------|
| No save file exists on first launch | System creates or exposes a valid default profile with first-level progression unlocked | First-time use must be stable and playable |
| Save data exists but references a level ID missing from current authored data | Development builds surface warning/error and use safe fallback rather than silently corrupting progression | Stable IDs are required for safe persistence |
| Player wins a level with fewer stars than previously earned | Completion remains saved and best stars do not downgrade | Historical best result must be preserved |
| Player loses a completed level | Saved unlock/completion state does not regress | Failure should not erase progress |
| App closes after a victory but before save commit completes | System should avoid partial corruption and on next run either retain previous valid save or the successfully committed new one | Durable state must remain trustworthy |
| Settings are changed without any progression change | Settings can still be saved independently | Preferences are durable state too |
| Retry is triggered repeatedly | No transient board state is written as durable progression data | Retry must stay session-local in MVP |
| Save schema changes during development | System surfaces migration/default behavior clearly rather than loading nonsense values silently | Development iteration should not create invisible corruption |

## Dependencies

| System | Direction | Nature of Dependency |
|--------|-----------|---------------------|
| Results, Retry, and Star Rating System | This system depends on Results, Retry, and Star Rating System | Needs confirmed result packages for progression-safe writes |
| Chapter Map Progression System | This system coordinates with Chapter Map Progression System | Needs unlock/completion semantics and map-facing progression state |
| Game Flow State System | This system depends on Game Flow State System | Needs safe load/save timing around shell, map, session, and result transitions |
| MatchJoy Authoring System | This system depends on MatchJoy Authoring System | Needs stable level IDs and chapter structure |
| Audio Presentation System | Audio system depends on this system | Needs durable audio preference state |
| Accessibility and Assist Options System | Future soft dependency | May later consume persisted assist settings |

Dependency notes:

1. The most important write trigger comes from `Results, Retry, and Star Rating System`.
2. The most important identity contract comes from `MatchJoy Authoring System`.
3. This system should remain durable-state focused and must not drift into
   session restore complexity in MVP.

## Tuning Knobs

| Parameter | Current Value | Safe Range | Effect of Increase | Effect of Decrease |
|-----------|--------------|------------|-------------------|-------------------|
| Save Write Frequency | Event-driven on meaningful durable changes | Low to moderate | Writes more often and reduces rollback window but may add unnecessary churn | Writes less often and may risk more progress loss between commits |
| Save Failure Strictness (Dev) | High visibility | Warning to hard-fail in development | Catches persistence problems earlier | Makes iteration smoother but can hide issues |
| Settings Save Granularity | Immediate or short-batched | Immediate to lightly batched | Makes preference changes feel durable instantly | Reduces writes but may delay visible persistence confidence |
| Map Focus Persistence | Optional lightweight focus memory | Off to lightweight on | Restores player orientation on relaunch | Keeps implementation simpler but less guided |

## Visual/Audio Requirements

| Event | Visual Feedback | Audio Feedback | Priority |
|-------|----------------|---------------|----------|
| First load/default profile creation | None required beyond stable usable shell/map entry | None required | Medium |
| Save success after meaningful result | Usually silent in MVP | None required | Low |
| Save failure in development | Clear developer-facing warning/error | None required | High (dev only) |
| Settings change persisted | Optional subtle confirmation in settings UI | None required | Medium |

## UI Requirements

| Information | Display Location | Update Frequency | Condition |
|-------------|-----------------|-----------------|-----------|
| Saved best stars / completion state | Chapter map nodes and result UI | On profile load and after save-worthy result updates | When progression UI is visible |
| Saved settings values | Settings UI / applied systems | On profile load and settings change | When settings are relevant |
| Save failure warning (dev) | Developer-visible log/debug surface | On failure | In development builds |

## Acceptance Criteria

- [ ] The system can load a valid default profile when no save data exists.
- [ ] The system can persist unlocked levels, completed levels, and best stars keyed by stable level IDs.
- [ ] Failure outcomes never regress a previously saved better result.
- [ ] Retry does not serialize transient board/session state in MVP.
- [ ] Settings such as audio/vibration preferences can be saved and reloaded.
- [ ] Progression data is available before stable map use.
- [ ] Durable writes happen only from safe, authoritative progression events.
- [ ] Save/load logic remains separate from gameplay simulation state.
- [ ] Performance: normal save/load operations are lightweight enough not to create noticeable friction in ordinary mobile session flow.
- [ ] No persistent keying depends on transient runtime instance identity.

## Open Questions

| Question | Owner | Deadline | Resolution |
|----------|-------|----------|-----------|
| Should MVP store last-selected/current map focus node on relaunch, or always reopen on highest unlocked/current recommended node? | User | Before implementation | MVP should reopen on the highest unlocked current-recommended node; persisting last map focus is optional and deferred |
| Should settings writes happen immediately on change, or be lightly batched until menu exit/app suspend? | User + future implementation pass | Before implementation | Not yet decided |
| Does the sample need more than one local profile/save slot? | User | Before implementation | Single local profile assumed for MVP |
| Should development builds include a simple save-reset/debug-clear path for rapid iteration? | User + future tooling pass | Before implementation | Recommended, not yet formally locked |

Open question notes:

1. None of these questions block a durable MVP persistence contract.
2. The guiding principle is safety: save only what should outlive the session,
   and never let persistence become a hidden source of gameplay-state bugs.

