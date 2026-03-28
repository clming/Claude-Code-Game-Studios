# MatchJoy Authoring System

> **Status**: In Design
> **Author**: User + Codex
> **Last Updated**: 2026-03-28
> **Implements Pillar**: Clean Commercial Puzzle Structure; Scoped Learnable Production

## Overview

The MatchJoy Authoring System defines how puzzle levels are described,
stored, validated, and authored for Jelly Candy Workshop. It is responsible for
the structure of level content data and for the editor-facing workflow used to
create and maintain the game's 50 handcrafted levels in Unity.

This system allows designers to configure each level's initial board layout,
goal set, move limit, obstacle placement, and other board-specific parameters in
a consistent, data-driven format. Its purpose is to make level creation reliable,
repeatable, and scalable, so the team can produce and tune handcrafted puzzle
content without hardcoding rules into gameplay logic. In implementation terms,
the system should treat level definitions as authored content assets, with
ScriptableObject as the default Unity-facing source of truth and optional export
or debug pathways kept separate from the core authoring workflow.

## Player Fantasy

This system supports the feeling that every level in Jelly Candy Workshop is
deliberately crafted, readable, and worth solving. The player should feel that
each board has a clear purpose: teaching a mechanic, testing a skill, creating a
specific combo opportunity, or delivering a satisfying payoff through level
goals and obstacle layout.

Although the player never interacts with the authoring system directly, they
experience its quality through pacing, clarity, and variety. A strong version of
this system makes the player feel that the game is guiding them through a
carefully curated candy-puzzle journey rather than throwing arbitrary boards at
them. The emotional target is confidence, momentum, and trust: the player should
believe that a level is fair, understandable, and intentionally built to create
a satisfying puzzle experience.

## Detailed Design

### Core Rules

1. The MatchJoy Authoring System is the source of truth for authored
   level content. Every playable puzzle level must be defined by a data asset
   rather than by hardcoded values in gameplay scripts or scene objects.

2. The default authoring format for a level is a Unity ScriptableObject asset.
   This asset represents one level and stores all designer-authored content
   required to initialize and validate that level. Optional export or debug
   representations such as JSON may exist, but they must not replace the
   ScriptableObject as the primary authored source during the scoped sample.

3. Every level data asset must contain, at minimum, the following required fields:
   - Level ID: a unique identifier used by progression, save data, and results
   - Chapter ID or Chapter Index: identifies which chapter the level belongs to
   - Level Number / Order Index: defines its order within chapter progression
   - Board Width and Board Height: defines the playable grid dimensions
   - Initial Board Layout Definition: defines the starting board composition
   - Goal Set: defines one or more win conditions for the level
   - Move Limit: defines the total moves available to complete the level
   - Allowed Tile Types: defines which standard matchable tiles may appear
   - Obstacle Definitions: defines any obstacle or blocked-cell placement used by the level
   - Power-Up Availability Rules: defines which power-up generation or placement rules are active
   - Reward / Completion Metadata: defines star thresholds or progression outputs needed after completion

4. The level asset schema must be explicit enough to implement without guessing.
   At minimum, the authoring schema must define:
   - a board coordinate convention used consistently across all authored placements
   - a cell state model that distinguishes disabled, empty-playable, and occupied-playable cells
   - whether a cell supports only one authored occupant or a controlled stack of occupant layers
   - whether obstacles and special objective content are stored inline or reference shared definitions
   - an authored board fill mode that distinguishes fully-authored, partially-authored, and generated cells
   These schema decisions may be revised when Board Grid State System is designed,
   but this document must treat them as required contract areas rather than hidden implementation details.

5. The Initial Board Layout Definition must support authored control over the
   starting state of the level. At minimum, it must allow designers to specify:
   - empty or disabled cells
   - playable cells
   - starting tile placements where required
   - obstacle placements
   - frozen ingredient placements or other objective-linked elements
   The system must support the following authored board modes:
   - Fully Authored Board: every relevant starting cell is explicitly defined
   - Partial Authored Board: key cells are explicitly defined and the remainder are filled by controlled generation
   - Generated Board: board shape and constraints are authored, but the starting tile population is produced by runtime generation rules
   The selected board mode must be explicit in level data rather than assumed implicitly.

6. The Goal Set must support multiple simultaneous goals in one level. At
   minimum, the system must support:
   - clearing jelly
   - collecting specified tile colors or tile categories
   - releasing or unlocking frozen ingredients
   Each goal entry must define its type, target count or completion condition,
   and any level-specific metadata required to evaluate progress.

7. The Move Limit must be authored per level as explicit data. No level may rely
   on a hidden default move count at runtime. If a level is intended to be
   unusually short or long, that decision must be visible in its data asset.

8. Obstacle data must be authored as level content, not embedded inside generic
   board logic. The level asset must define obstacle placement and any obstacle-
   specific configuration that affects that level's board state at start.

9. The system must support designer-readable authoring. A level designer should
   be able to inspect a level asset and understand the level's board structure,
   goals, move pressure, and special setup without reading gameplay code.

10. The authoring workflow for a new level must follow this sequence:
   - create a new level data asset
   - assign level identity and chapter ordering
   - define board size and playable cell layout
   - place or configure starting tiles and obstacles
   - define goals and move limit
   - define any special level rules or objective-linked configuration
   - run validation checks
   - mark the level ready for playtest
   This workflow may later be supported by custom editor tooling, but the data
   model must not depend on advanced tools existing on day one.

11. Validation must be split into two layers:
    - Structural Validation: automated checks that verify schema completeness, references, bounds, and legal initialization contracts
    - Playability Review: automated heuristics and/or manual review used to judge fairness, readability, solvability risk, and tuning quality
    Structural validation is required before a level is treated as playable content.
    Playability review is required before a level is treated as content-complete for milestone approval.

12. Every level asset must pass structural validation before it is considered playable.
    Structural validation rules must include, at minimum:
    - required fields are present
    - Level ID is unique
    - board dimensions are valid
    - all authored coordinates are inside the board bounds
    - goals reference valid data
    - move limit is greater than zero
    - obstacle placements do not overlap illegally
    - the level can initialize into a legal runtime board state
    Structural validation must not claim to prove full puzzle quality or long-horizon balance.

13. Playability review must explicitly classify its outputs as one of:
    - blocking failure
    - warning requiring designer acknowledgement
    - advisory note
    Checks related to first-move readability, excessive auto-resolve behavior,
    likely dead boards, and suspicious goal-obstacle tension belong in this layer
    unless a later system provides a proven deterministic validator for them.

14. Validation failures must be treated as content errors, not silently repaired
    at runtime. If authored data is invalid, the level should fail validation in
    the editor or in development builds so that the content issue is fixed at the
    source.

15. The system must separate authored level intent from runtime board state.
    Authored data defines how a level begins and what it is trying to achieve;
    runtime systems may transform that data into a live board instance, but they
    must not overwrite the authored source asset during play.

16. Reward / Completion Metadata must support milestone-safe extensibility.
    For the current scope, the schema must support:
    - progression-facing completion outputs required by chapter flow
    - a star rating policy field
    - an optional star threshold payload
    The exact star rating model may remain configurable as move-based, score-based,
    or another approved metric, but the schema must not hardcode one policy unless
    the Results, Retry, and Star Rating System has already locked that decision.

17. The system must be designed to scale cleanly to the planned 50 handcrafted
    levels. Reusing templates, duplicating existing level assets, and making
    small controlled variations must be supported as normal authoring practice.

18. The system must remain scope-conscious. It is not responsible for building a
    full live-ops content platform, procedural campaign generator, or remote
    level publishing pipeline within the current project scope.

### States and Transitions

The MatchJoy Authoring System uses a lightweight content lifecycle so
designers can tell whether a level is incomplete, internally valid, and ready for
playtest. These states apply to the authored level asset, not to the runtime
board instance.

| State | Entry Condition | Exit Condition | Behavior |
|-------|----------------|----------------|----------|
| Draft | A new level data asset is created, or an existing level is being actively edited without passing validation | The asset is saved with all required fields populated and passes baseline validation | The level is editable, incomplete fields are allowed, and the asset is not considered reliable for structured playtesting |
| Validated | The level asset passes all required structural and content validation checks | The asset is modified again, or it is promoted to Ready for Playtest | The level is structurally sound, references are valid, coordinates are in bounds, and the board can initialize into a legal runtime state |
| Ready for Playtest | A validated level has been marked by the designer as ready for internal testing | Playtest feedback requires changes, or the level is approved as content-complete for the current milestone | The level is considered stable enough for gameplay evaluation, balancing, and user experience review |
| Needs Revision | A playtest, review pass, or validation update identifies design or data problems that must be corrected | The level is edited and passes validation again | The level remains authored content, but it is flagged as unreliable until its issues are resolved |
| Approved for Milestone | The level has passed validation and internal review for the current milestone target (MVP, Vertical Slice, or Alpha) | Later balancing or scope changes reopen the level for editing | The level is treated as approved content for the current build target and should not change casually without reason |

State transition rules:

1. Every newly created level begins in `Draft`.
2. A level may move from `Draft` to `Validated` only after all required fields are
   populated and automated validation checks pass.
3. A level may move from `Validated` to `Ready for Playtest` when the designer
   decides the level is stable enough for gameplay evaluation.
4. A level may move from `Ready for Playtest` to `Needs Revision` if playtest
   feedback identifies issues in readability, difficulty, board fairness, pacing,
   or data correctness.
5. A level may move from `Needs Revision` back to `Validated` only after its data
   is corrected and validation passes again.
6. A level may move from `Ready for Playtest` to `Approved for Milestone` when the
   team agrees it satisfies the current milestone's content needs.
7. Any edit to a `Validated`, `Ready for Playtest`, or `Approved for Milestone`
   level automatically returns it to `Draft` or `Needs Revision`, depending on how
   the workflow is implemented, because the previous validation result can no
   longer be assumed valid.
8. Runtime play must never change the authored lifecycle state directly. Runtime
   systems consume level data, but only editor-side authoring and review actions
   may change content status.

### Interactions with Other Systems

1. **Board Grid State System**
   - This system provides the authored board definition consumed by the Board Grid
     State System at level start.
   - Data flowing out: board dimensions, playable cell map, initial placements,
     blocked cells, obstacle placements, and any board initialization flags.
   - Data flowing in: schema constraints that define what a legal board layout is.
   - Responsibility boundary: the authoring system defines the intended starting
     board; the Board Grid State System owns the runtime representation after the
     level is instantiated.

2. **Game Flow State System**
   - This system provides the level identity and content package selected when a
     level begins.
   - Data flowing out: level ID, chapter/order metadata, level content reference,
     and content readiness status.
   - Data flowing in: requests to load a specific level into active play.
   - Responsibility boundary: the authoring system stores and validates level
     content; the Game Flow State System decides when that content is entered,
     exited, retried, or returned to from map/menu states.

3. **Level Goal and Move Limit System**
   - This system defines the authored goals and move budget used by goal tracking
     during play.
   - Data flowing out: goal types, target counts, special objective metadata,
     move limit, and completion metadata.
   - Data flowing in: goal schema requirements that determine what goal data must
     be present and valid.
   - Responsibility boundary: this system authors and validates the goal setup;
     the Level Goal and Move Limit System evaluates runtime progress against that setup.

4. **Obstacle and Special Tile System**
   - This system defines authored placements and configuration for obstacle-linked
     content in each level.
   - Data flowing out: obstacle coordinates, obstacle types, obstacle parameters,
     and any special tile initialization data.
   - Data flowing in: supported obstacle definitions and configuration constraints.
   - Responsibility boundary: this system declares where and how obstacle content
     starts; the obstacle runtime system controls obstacle behavior during play.

5. **Power-Up Creation and Activation System**
   - This system may expose level-level switches or restrictions affecting which
     power-up rules are active in a specific level.
   - Data flowing out: power-up availability rules, restricted combinations, or
     level modifiers if such controls are supported.
   - Data flowing in: the set of supported power-up identifiers and rule options.
   - Responsibility boundary: this system configures level-specific allowances;
     the power-up system handles runtime creation, activation, and chaining.
6. **Chapter Map Progression System**
   - This system provides the metadata that places a level inside the campaign
     structure.
   - Data flowing out: chapter ID, level order, unlock sequencing information,
     and milestone grouping.
   - Data flowing in: chapter structure constraints and progression ordering rules.
   - Responsibility boundary: this system defines where a level belongs; the
     chapter map system uses that information to present and unlock content.

7. **Save/Load and Settings Persistence System**
   - This system provides stable identifiers required to persist level completion
     and progression state.
   - Data flowing out: unique level IDs, chapter IDs, and content version markers
     if versioning is supported.
   - Data flowing in: persistence constraints for ID stability and migration safety.
   - Responsibility boundary: this system must guarantee stable authored identity;
     the persistence system stores player outcomes and unlocked progress separately
     from the authored level asset.

8. **Level Difficulty Curve and Content Pacing System**
   - This system acts as the content source for pacing and balancing decisions
     across the full level set.
   - Data flowing out: complete level definitions, goal complexity, obstacle usage,
     move budgets, and chapter ordering.
   - Data flowing in: pacing guidelines, difficulty targets, and milestone-specific
     content needs.
   - Responsibility boundary: this system stores individual level data; the pacing
     system reviews that data across many levels to shape progression and difficulty curves.

9. **Core HUD and Goal Feedback System**
   - This system provides the authored data the HUD must display to the player.
   - Data flowing out: goal definitions, move limit, level display metadata, and
     potentially icon references or localized labels.
   - Data flowing in: presentation constraints around what information must be
     surfaced clearly in-level.
   - Responsibility boundary: this system authors gameplay-relevant display data;
     the HUD system decides how that information is shown during play.

## Formulas

### Global Level Index

A level may need a globally unique sortable index in addition to its chapter-local
order. If used, it should be derived from authored chapter and level ordering
rather than entered manually.

```text
global_level_index = chapter_index * CHAPTER_LEVEL_BLOCK + level_order_index
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| chapter_index | int | 0 to N | level data | Zero-based or one-based chapter ordering, depending on project standard |
| CHAPTER_LEVEL_BLOCK | int | >= planned max levels per chapter | project constant | Reserved spacing for each chapter block |
| level_order_index | int | 0 to block size - 1 | level data | Level order inside the chapter |

**Expected output range**: 0 to total planned content capacity  
**Edge case**: `CHAPTER_LEVEL_BLOCK` must be greater than or equal to the maximum
number of levels allowed in a chapter, or collisions become possible.

### Star Threshold Validation Placeholder

If the approved star policy for the current milestone is move-based, star thresholds must be authored in ascending order and must never exceed the level's move limit. If the approved star policy is score-based or another metric, this formula is replaced by the corresponding ordered-threshold rule for that metric.

```text
if star_policy == move_based:
    0 < star_1_threshold <= star_2_threshold <= star_3_threshold <= move_limit
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| star_1_threshold | int | 1 to move_limit | level data | Minimum condition for 1-star success metric |
| star_2_threshold | int | star_1 to move_limit | level data | Minimum condition for 2-star success metric |
| star_3_threshold | int | star_2 to move_limit | level data | Minimum condition for 3-star success metric |
| move_limit | int | > 0 | level data | Maximum move count allowed in the level |

**Expected output range**: ordered integer thresholds within the legal move budget  
**Edge case**: If the game later uses score-based stars instead of move-based stars, the schema must preserve the star policy field and swap to score validation without redesigning the entire reward metadata payload.

### Weighted Random Fill Normalization

If a level supports partially authored boards with weighted random fill, all tile
spawn weights must normalize to a usable distribution.

```text
normalized_weight_i = raw_weight_i / total_weight
total_weight = sum(raw_weight_1 ... raw_weight_n)
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| raw_weight_i | float | >= 0 | level data | Authored spawn weight for a specific tile type |
| total_weight | float | > 0 | calculated | Sum of all raw weights in the active fill set |
| normalized_weight_i | float | 0.0 to 1.0 | calculated | Final spawn probability share for tile type i |

**Expected output range**: all normalized weights sum to 1.0  
**Edge case**: If all raw weights are zero, validation must fail. Runtime must not
attempt fallback guessing for a level-authored distribution.

### Goal Completion Rate

For UI, validation preview, or authoring debug purposes, a goal's completion rate
may be represented as a normalized progress value.

```text
goal_completion_rate = current_progress / target_value
clamped_goal_completion_rate = clamp(goal_completion_rate, 0, 1)
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| current_progress | int | 0 to target_value+ | runtime | Current progress made toward a goal |
| target_value | int | > 0 | level data | Required amount for that goal |
| goal_completion_rate | float | 0.0 to >1.0 | calculated | Raw completion ratio |
| clamped_goal_completion_rate | float | 0.0 to 1.0 | calculated | UI-safe completion ratio |

**Expected output range**: 0.0 to 1.0 after clamp  
**Edge case**: If `target_value` is zero because of invalid authored data, validation
must fail before runtime. Division by zero is never allowed.

## Edge Cases

| Scenario | Expected Behavior | Rationale |
|----------|------------------|-----------|
| A level asset is missing a required field | Validation fails and the level cannot be marked as playable | Required content must be complete before a level enters testable workflow |
| Two level assets share the same Level ID | Validation fails and both assets are flagged until the conflict is resolved | Progression, save/load, and results systems require stable unique identifiers |
| Board width or height is zero or negative | Validation fails immediately | A level cannot produce a legal runtime board without valid dimensions |
| An authored coordinate lies outside board bounds | Validation fails and highlights the invalid coordinate | Out-of-bounds placement is always a content error, not something runtime should silently correct |
| A cell is assigned multiple incompatible contents at level start | Validation fails unless the combination is explicitly supported by schema rules | Starting board ownership must be unambiguous to keep runtime logic deterministic |
| A level defines a goal type that the runtime goal system does not support | Validation fails | Unsupported goals must be caught at authoring time, not discovered during play |
| A goal target count is zero or negative | Validation fails unless that goal type explicitly allows boolean completion without count | Goal progress must be meaningful and evaluable |
| Move limit is zero or negative | Validation fails | A level must always define a legal number of moves |
| Star thresholds are unordered or exceed legal limits | Validation fails | Completion grading must be consistent and designer-readable |
| The initial board generates with no valid swap available | Structural validation fails only if the approved runtime contract forbids auto-reshuffle; otherwise the level receives a playability warning requiring review | This avoids pretending the authoring system can prove more than the runtime contract actually guarantees |
| The initial board auto-resolves excessive matches before the first player move | Structural validation does not fail by default; a playability warning is raised unless the runtime contract explicitly forbids this state | First-move readability is a quality concern, but not every case should block authoring unless the project standard says so |
| A partially authored board leaves random fill with no valid tile distribution | Validation fails | Runtime must not invent a distribution when the authored setup is incomplete or mathematically invalid |
| Obstacle placement blocks all paths to completing a goal | Raise a blocking playability review issue unless a deterministic solvability validator later proves it automatically | The current project scope should not imply a guaranteed theorem-level solver if one does not exist |
| Frozen ingredient placement conflicts with another non-stackable object on the same tile | Validation fails unless the schema explicitly allows the stack | Objective-linked objects need deterministic ownership and resolution order |
| Chapter ID or level order conflicts with progression ordering | Validation fails or raises a progression conflict warning | Campaign sequencing must remain stable for chapter-map progression |
| A level asset is edited after it has already been approved for a milestone | The asset is automatically moved out of approved state and must be revalidated | Approval only applies to the exact validated data revision |
| A saved player record references a level whose schema changed in a breaking way | The persistence layer must either migrate safely by version or treat the level as requiring progression resync in development builds | Level identity must be stable, and schema changes must not silently corrupt saved progress |
| A referenced obstacle, tile type, icon, or content asset is deleted or missing | Validation fails and the level is blocked from playtest | Broken references should be surfaced in the editor, not degrade silently in runtime |
| A level uses a board shape or content combination unsupported by current runtime systems | Validation fails and the level remains in Draft or Needs Revision | Authoring freedom must stay inside the capabilities of the playable build |
| A designer duplicates a level but forgets to change identity and ordering metadata | Validation warns or fails on duplicate identity fields | Duplication is expected workflow, so duplicate-safety checks must exist |

Validation classification notes:

1. Rows related to missing schema, illegal references, or impossible initialization
   are structural validation failures.
2. Rows related to fairness, first-move clarity, and likely solvability problems
   are playability review findings unless a stronger automated proof exists.

## Dependencies

| System | Direction | Nature of Dependency |
|--------|-----------|---------------------|
| Board Grid State System | This system depends on Board Grid State System | The authoring schema must match the legal runtime board model, including dimensions, cell occupancy rules, and board-shape constraints |
| Game Flow State System | Game Flow State System depends on this system | Game flow needs stable level identity, content references, and level-start data packages to enter playable stages |
| Level Goal and Move Limit System | This system provides data to Level Goal and Move Limit System | Goal definitions, target counts, and move budgets are authored here and evaluated there at runtime |
| Tile Swap and Match Resolution System | Tile Swap and Match Resolution System depends on this system | Runtime swap and resolution logic must begin from a valid authored starting board state |
| Cascade, Refill, and Determinism System | Cascade, Refill, and Determinism System depends on this system | Random fill allowances, spawn constraints, and board-start rules are authored here and consumed there |
| Power-Up Creation and Activation System | This system provides configuration to Power-Up Creation and Activation System | Level-specific power-up allowances or restrictions may be defined in authored level content |
| Obstacle and Special Tile System | This system provides data to Obstacle and Special Tile System | Obstacle types, placements, and level-specific setup originate in authored level data |
| Frozen Ingredient Objective System | This system provides data to Frozen Ingredient Objective System | Frozen ingredient placement and objective-linked metadata are authored here |
| Chapter Map Progression System | Chapter Map Progression System depends on this system | Chapter ID, level order, unlock position, and progression metadata must be stable and readable |
| Save/Load and Settings Persistence System | Save/Load and Settings Persistence System depends on this system | Persistent progression requires unique level IDs and stable content identity |
| Core HUD and Goal Feedback System | Core HUD and Goal Feedback System depends on this system indirectly | HUD surfaces authored goals, move limits, and display-friendly level metadata |
| Results, Retry, and Star Rating System | Results, Retry, and Star Rating System depends on this system | Completion grading and retry flow require star threshold data, level identity, and progression metadata |
| Level Difficulty Curve and Content Pacing System | Level Difficulty Curve and Content Pacing System depends on this system | Cross-level balancing and pacing analysis use authored board, goal, move, and obstacle data as source material |
| Onboarding and Tutorial Messaging System | Onboarding and Tutorial Messaging System depends on this system indirectly | Tutorial beats need access to authored level flags, expected teaching goals, and optional instructional metadata |
| Accessibility and Assist Options System | Soft dependency on this system | Accessibility review may require certain level metadata to remain display-safe and readable, but this system can function without direct runtime coupling |
| Audio Presentation System | Soft dependency on this system | Some level metadata may influence chapter or level presentation, but audio routing does not rely on this system structurally |

Dependency notes:

1. The most critical upstream dependency is `Board Grid State System`, because
   this system cannot define legal level content unless the runtime board schema
   is known.

2. The most critical downstream dependencies are:
   - `Tile Swap and Match Resolution System`
   - `Level Goal and Move Limit System`
   - `Chapter Map Progression System`
   - `Save/Load and Settings Persistence System`
   because all of them require stable, validated, designer-authored level data.

3. Dependencies on presentation systems are mostly indirect. UI, results, and
   tutorial layers do not define the level content model, but they rely on it
   being readable, stable, and expressive enough to present player-facing information.

4. This system has one hard contract requirement: authored level data must remain
   separate from runtime mutable state. Any dependent runtime system may read and
   instantiate authored data, but no dependent system may treat the authored asset
   itself as mutable session state.

## Tuning Knobs

| Parameter | Current Value | Safe Range | Effect of Increase | Effect of Decrease |
|-----------|--------------|------------|-------------------|-------------------|
| Board Width | 9 | 7-10 | Increases board complexity, combo opportunity space, and scan load; may reduce readability on mobile if too wide | Makes boards easier to read and faster to parse, but reduces puzzle variety and combo space |
| Board Height | 9 | 7-10 | Increases board depth, refill chain potential, and overall complexity | Reduces board complexity and cascade potential, making levels simpler and shorter-feeling |
| Move Limit | Per level authored | 10-40 | Makes levels more forgiving and reduces pressure; can flatten challenge if set too high | Increases tension and difficulty; if set too low, levels may feel unfair or overly luck-dependent |
| Goal Target Count | Per goal authored | Goal-dependent; must be achievable within move budget | Increases required effort and extends level tension | Makes levels easier and may reduce satisfaction if goals are completed too quickly |
| Allowed Tile Type Count | 5 | 4-6 | More tile types increase board complexity and reduce match frequency, making planning harder | Fewer tile types increase match density and combo frequency, making levels easier and juicier |
| Random Fill Weight Bias | Balanced distribution by default | All weights >= 0 and normalized | Favoring one tile type can support specific level goals or combo patterns, but may distort fairness | Lowering bias makes the board feel more neutral and less level-authored in character |
| Obstacle Density | Per level authored | Low to moderate for current scope | More obstacles increase difficulty, constrain board freedom, and create stronger authored challenge identity | Fewer obstacles make levels cleaner and easier to solve, but may reduce variety |
| Starting Handcrafted Layout Coverage | Partial authored layout by default | 0%-100% of relevant cells | More authored cells increase designer control and puzzle intentionality, but raise content creation cost | More random fill increases variety and speed of authoring, but reduces authored specificity |
| Star Threshold Strictness | Moderate by default | Must remain ordered and achievable | Higher thresholds push replay and mastery but may frustrate casual players | Lower thresholds make rewards easier to earn but reduce performance distinction |
| Chapter Difficulty Tag | Per chapter authored | Intro / Easy / Medium / Hard / Peak | Higher difficulty tags push supporting systems toward denser goals, tighter move budgets, and heavier obstacle use | Lower difficulty tags create gentler pacing and more onboarding-friendly progression |
| Tutorial Flag / Teaching Intent | Off by default except onboarding levels | Boolean or small enum set per level | More teaching flags increase onboarding clarity and pacing control, but can over-script progression | Fewer teaching flags create a freer campaign flow, but may weaken mechanic introduction clarity |
| Validation Strictness Mode | Strict in development | Warning-only to strict-fail, depending on rule category | Stricter validation catches content errors earlier, improving reliability but slowing rough iteration | Looser validation increases authoring speed early, but raises risk of invalid or inconsistent content entering playtest |

Tuning notes:

1. Most tuning knobs in this system are per-level authored parameters rather than
   global runtime balance values.

2. The primary purpose of these knobs is to let designers produce controlled
   variation across 50 handcrafted levels without changing gameplay code.

3. Some parameters, such as board size and allowed tile type count, should be
   treated as high-impact knobs and changed sparingly because they affect overall
   readability, feel, and tuning assumptions across multiple systems.

4. Validation Strictness Mode is a workflow knob, not a player-facing balance
   knob. It exists to support reliable content production during development.

5. If future tooling adds templates or difficulty presets, those presets should
   write into these authored knobs rather than bypassing them.
## Acceptance Criteria

- [ ] A designer can create a new level asset in Unity without modifying code.
- [ ] A level asset supports authored configuration for board size, playable cell layout, initial board setup, goals, move limit, and obstacle placement.
- [ ] A level asset can represent all currently planned level goal types for the sample: jelly clear, target collection, and frozen ingredient release.
- [ ] A level asset has a stable unique Level ID that can be consumed by progression, results, and persistence systems.
- [ ] A level asset can be placed into chapter order using explicit progression metadata rather than scene name or file path conventions.
- [ ] Validation catches missing required fields, duplicate Level IDs, out-of-bounds coordinates, illegal board dimensions, invalid goals, and invalid move limits before structured playtest.
- [ ] Validation surfaces content errors clearly enough that a designer can identify what is wrong without reading gameplay code.
- [ ] The authored data can initialize into a legal runtime board state through the board runtime systems.
- [ ] Runtime systems can read level data without mutating the authored source asset.
- [ ] A designer can duplicate an existing level asset, adjust its data, and create a new valid level without breaking identity or progression rules.
- [ ] The authoring format is practical for producing and maintaining the planned 50 handcrafted levels.
- [ ] The system supports partial handcrafted board setup plus controlled random fill, with the active mode explicitly defined in level data.
- [ ] Goal data, obstacle data, and progression metadata are structured clearly enough to support future editor tooling without redesigning the schema.
- [ ] Level data remains readable in the Unity authoring workflow and does not depend on hidden runtime-only assumptions to understand a level's intent.
- [ ] Performance: Loading and validating one level asset in editor/development workflow completes fast enough to support rapid iteration and should not create noticeable friction in ordinary level-authoring use.
- [ ] No hardcoded level content values are required in implementation for normal level creation and maintenance.

Acceptance notes:

1. The primary success condition for this system is not visual polish, but
   reliability, readability, and authoring speed.

2. This system is accepted when it enables sustainable handcrafted content
   production for the 50-level sample while keeping runtime puzzle systems fully
   data-driven.

3. If designers still need programmer intervention for normal level creation,
   the system has not met its intent even if the runtime technically works.

4. If runtime systems must special-case individual levels in code because the
   data model is too weak, the system has failed its architectural purpose.

## Open Questions

| Question | Owner | Deadline | Resolution |
|----------|-------|----------|-----------|
| What is the final runtime schema contract for Board Grid State System, including cell occupancy, stack rules, and board-shape representation? | User + future Board Grid State System design | Before implementation of this system | Provisional - must be finalized when Board Grid State System GDD is written |
| Should level assets use a single unified ScriptableObject schema for all levels, or support subtype variants for special level formats? | User + Codex | Before authoring more than the first 10 levels | Default assumption is a unified schema unless variation pressure proves otherwise |
| How much custom Unity editor tooling is required for the sample: Inspector-only workflow, custom editor windows, or grid painting tools? | User | Before large-scale content production | Not yet decided |
| Should partially authored boards support seeded deterministic random fill for repeatable debugging and balancing? | User + future Cascade, Refill, and Determinism System design | Before gameplay prototype hardening | Recommended, but not yet locked |
| How should star thresholds be authored in the final product: move-efficiency based, score-based, or another completion metric? | User + future Results, Retry, and Star Rating System design | Before results system design is finalized | Schema now assumes a star policy field, but the final metric is still undecided |
| Should the level data schema include optional teaching tags, hint markers, or onboarding metadata from the start, or wait until tutorial needs are clearer? | User + future Onboarding and Tutorial Messaging System design | Before onboarding system design | Lean default recommended; keep extensibility in schema |
| How should content versioning be handled if level assets change after persistence has already recorded player progress? | User + future Save/Load and Settings Persistence System design | Before persistence implementation | Not yet decided |
| Should obstacle configuration live entirely inside level assets, or should some obstacle parameters be referenced from shared obstacle definitions plus per-level overrides? | User + future Obstacle and Special Tile System design | Before obstacle-heavy content production | Shared definitions plus per-level placement is likely best, but not finalized |
| What is the minimum viable validation toolset for milestone one: automatic validation on save, manual validate button, or build-time validation pass? | User | Before first internal content production workflow is adopted | Not yet decided |
| Does the project need import/export support between ScriptableObject and JSON for debugging, version diffing, or future tools? | User | Before tooling scope expands | Optional, deferred unless a concrete workflow need appears |

Open question notes:

1. None of these questions block the conceptual design of the system, but several
   of them affect implementation detail and tooling scope.

2. The highest-priority unresolved question is the contract with `Board Grid State
   System`, because it determines how level-authored board data maps into runtime state.

3. Tooling questions should be answered conservatively. For this project, the
   default principle is to prefer a simple authoring workflow that can ship 50
   levels over a more ambitious editor-tool platform.

4. Questions that involve persistence, star rating, and obstacle schema should be
   resolved before those dependent systems enter implementation.





