# Level Difficulty Curve and Content Pacing System

> **Status**: In Design
> **Author**: User + Codex
> **Last Updated**: 2026-03-28
> **Implements Pillar**: Clean Commercial Puzzle Structure; Scoped Learnable Production

## Overview

The Level Difficulty Curve and Content Pacing System defines how MatchJoy's
50-level sample is structured across time, challenge, mechanic introduction, and
chapter identity. It is responsible for sequencing goals, obstacles, power-up
complexity, move pressure, and teaching beats so the campaign feels deliberate,
fair, and progressively richer rather than repetitive or erratic.

This system does not replace per-level design. Individual level data still owns
board layouts, exact goals, and obstacle placements. Instead, this system owns
campaign-scale pacing rules: what should appear when, how quickly complexity may
increase, how chapters differentiate themselves, and how difficulty rises and
relaxes across the full sample.

For MVP/Vertical Slice planning, this system must answer:
- how the 50 levels are grouped into chapters
- what each chapter teaches or emphasizes
- when goals, obstacles, and power-ups are introduced
- how difficulty ramps, spikes, and recovers
- which content combinations are intentionally deferred

## Player Fantasy

This system supports the feeling that MatchJoy is thoughtfully guided and worth
continuing. The player should feel that new content arrives at a comfortable but
rewarding pace, that challenge rises with purpose, and that harder levels feel
like meaningful tests rather than random punishment.

The emotional target is momentum with trust. The player should sense that the
game is teaching them, then testing them, then rewarding mastery in cycles.
Each chapter should feel like a fresh step forward, not just more of the same.

## Detailed Design

### Core Rules

1. The 50-level sample must be paced as a handcrafted campaign rather than a
   random content bucket. Each level should serve a role in teaching, practice,
   escalation, combination testing, relief, or milestone challenge.

2. The default campaign structure for planning is `5 chapters x 10 levels`.
   Each chapter should have a distinct pacing purpose and a manageable content
   identity.

3. Every chapter should include, in some proportion:
   - onboarding or refresher levels
   - practice levels using recently introduced content
   - combination levels that mix previously learned demands
   - one or more pressure levels that test mastery more directly

4. New mechanical content should be introduced conservatively. For MVP/Vertical
   Slice pacing, the preferred rule is:
   - introduce one meaningful new concept at a time
   - give it at least 2-3 levels of readable practice before combining it with
     another new demand
   - avoid introducing a new obstacle, new goal type, and new power-up pattern
     all at once unless intentionally late in the sample

5. Difficulty should rise in waves, not a straight line. The campaign must allow
   brief recovery levels after spikes so players can consolidate understanding.

6. Move pressure is one of the main tuning levers, but it must not be the only
   difficulty tool. Difficulty should also vary through:
   - goal composition
   - board topology
   - obstacle density
   - target tile distribution pressure
   - power-up opportunity control

7. The sample's pacing should prioritize readability over content volume.
   If a planned feature or obstacle family would destabilize the campaign curve,
   it should be deferred instead of forcing it into the 50-level plan.

8. Power-up complexity should arrive after the player has internalized baseline
   swap/match readability. Early chapters should let power-ups feel rewarding,
   not required for basic comprehension.

9. Frozen ingredient goals and obstacle-heavy combinations should not dominate
   early pacing. They should appear only after the player has already formed a
   reliable understanding of standard goal play and board reading.

10. The pacing plan must explicitly define which content families are out of
    scope for the 50-level sample so later level production does not expand
    uncontrollably.

11. This system must produce campaign guidance that level authors can use
    directly. It should be specific enough to shape real level production, not
    just describe broad intentions.

### States and Transitions

This system describes campaign progression in pacing bands rather than runtime
state machine ownership.

| Pacing State | Entry Condition | Exit Condition | Behavior |
|-------------|----------------|----------------|----------|
| Introduction | A chapter or content family is first entering the campaign | Player has seen the concept in one or more readable levels | Uses low-complexity levels to teach a single new demand clearly |
| Reinforcement | A newly introduced concept has appeared and needs repetition | Player is ready for mixed-demand play | Repeats the concept in varied but still readable forms |
| Combination | Two or more previously taught demands are intentionally mixed | A stronger test level or chapter close is reached | Builds confidence through meaningful combinations |
| Pressure Test | Campaign wants to test recent learning under tighter constraints | Spike ends or chapter resolves | Uses stronger move pressure, board friction, or denser content interactions |
| Recovery | A recent spike has occurred and pacing needs relief | Player is ready for the next introduction or spike | Lowers one or more pressure factors without becoming trivial |
| Milestone Close | End-of-chapter or chapter-defining level grouping | Campaign advances into next chapter identity | Reinforces chapter mastery and creates transition energy |

State transition rules:

1. A new chapter generally begins with `Introduction` or light `Reinforcement`.
2. New content should not move straight from `Introduction` into intense
   `Pressure Test` without passing through `Reinforcement`.
3. `Combination` levels should appear only after their component demands have
   already been taught in isolation.
4. `Pressure Test` levels should be followed by either `Recovery` or chapter-end
   payoff, not by another unrelated spike immediately.
5. `Milestone Close` should summarize the chapter's identity rather than adding
   a large amount of brand-new systemic complexity.

### Interactions with Other Systems

1. **MatchJoy Authoring System**
   - Receives chapter-level pacing guidance and uses it to shape authored level
     metadata, level tags, and production expectations.
   - Responsibility boundary: pacing sets campaign guidance; authoring stores
     level-specific implementation data.

2. **Level Goal and Move Limit System**
   - Uses pacing guidance to decide when mixed goals, tighter move budgets, and
     more complex success conditions should appear.
   - Responsibility boundary: goal system defines exact rules; pacing system
     defines when complexity should rise.

3. **Obstacle and Special Tile System**
   - Receives staged introduction windows and density expectations.
   - Responsibility boundary: obstacle system defines obstacle behavior; pacing
     decides when obstacle families enter the campaign and how dominant they are.

4. **Power-Up Creation and Activation System**
   - Receives expectations for when power-up reliance is introduced and how much
     combo complexity is appropriate per chapter.
   - Responsibility boundary: power-up system defines mechanics; pacing decides
     exposure cadence.

5. **Frozen Ingredient Objective System**
   - Receives guidance on when frozen ingredient goals should enter the campaign
     and how strongly they should shape level identity.
   - Responsibility boundary: ingredient system defines completion semantics;
     pacing decides rollout timing.

6. **Chapter Map Progression System**
   - Uses chapter splits and milestone levels to create visible campaign rhythm.
   - Responsibility boundary: chapter map presents the structure; pacing defines
     why that structure exists.

7. **Results, Retry, and Star Rating System**
   - Informs how star thresholds and difficulty expectations should scale across
     the campaign.
   - Responsibility boundary: results define evaluation output; pacing defines
     expected challenge trends.

## Formulas

### Chapter Index Mapping

```text
chapter_index = floor((global_level_number - 1) / levels_per_chapter) + 1
chapter_local_index = ((global_level_number - 1) mod levels_per_chapter) + 1
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| global_level_number | int | 1 to 50 | campaign plan | Global level order across the sample |
| levels_per_chapter | int | > 0 | campaign plan | Planned levels per chapter |
| chapter_index | int | 1 to chapter count | calculated | Which chapter the level belongs to |
| chapter_local_index | int | 1 to levels_per_chapter | calculated | Position of the level inside its chapter |

### Difficulty Band Tag

```text
difficulty_band in {Intro, Easy, Medium, Hard, Peak}
```

This is an authored campaign guidance tag rather than a numerical difficulty
score. It exists to keep pacing communication readable during production.

## Edge Cases

| Scenario | Expected Behavior | Rationale |
|----------|------------------|-----------|
| A new obstacle is introduced in the same level as a new goal type and a new pressure model | Defer one or more elements unless this is an intentional late-sample challenge | Too many simultaneous novelties hurt learnability |
| Several levels in a row use the same goal pattern with only cosmetic changes | Pacing review flags repetition risk | Content cadence should feel varied and purposeful |
| A chapter ends without a clear identity or milestone feel | Pacing review flags weak chapter close | Chapter structure should feel visible, not arbitrary |
| Difficulty spike occurs with no recovery window afterward | Pacing review flags curve instability | Sustained spikes feel punishing and reduce trust |
| A late chapter still spends too many levels reteaching baseline content | Pacing review flags under-escalation | Later content should use the campaign budget efficiently |
| Frozen ingredient goals appear before players understand simpler goal loops | Pacing review flags sequencing problem | Objective complexity should build on established comprehension |
| Power-ups are required for success before players have internalized how they are created | Pacing review flags premature reliance | Reward systems should not become hidden requirements too early |

## Dependencies

| System | Direction | Nature of Dependency |
|--------|-----------|---------------------|
| MatchJoy Authoring System | This system coordinates with MatchJoy Authoring System | Needs a place for pacing guidance to become real level-production data |
| Chapter Map Progression System | Chapter Map Progression System depends on this system structurally | Uses chapter splits and milestone rhythm defined here |
| Level Goal and Move Limit System | This system influences Level Goal and Move Limit System | Sets expected complexity cadence across the campaign |
| Obstacle and Special Tile System | This system influences Obstacle and Special Tile System | Sets rollout and density expectations |
| Power-Up Creation and Activation System | This system influences Power-Up Creation and Activation System | Sets when special-piece complexity should feel central |
| Frozen Ingredient Objective System | This system influences Frozen Ingredient Objective System | Sets introduction timing and usage weight |
| Results, Retry, and Star Rating System | Soft dependency in MVP | Uses pacing expectations to interpret star-threshold trends sensibly |

Dependency notes:

1. This system is one of the main safeguards against scope creep and content
   repetition in the 50-level sample.
2. It should guide production early, before too many handcrafted levels exist,
   because late pacing corrections are expensive.
3. It is intentionally campaign-scale guidance, not a replacement for per-level
   tuning work.

## Tuning Knobs

| Parameter | Current Value | Safe Range | Effect of Increase | Effect of Decrease |
|-----------|--------------|------------|-------------------|-------------------|
| Levels Per Chapter | 10 | 5-15 | Strengthens chapter identity and longer thematic arcs | Creates more frequent chapter turnover |
| New-Concept Spacing | 2-3 reinforcement levels | 1-4 | Slows novelty rate but improves learnability | Speeds novelty but increases confusion risk |
| Recovery Frequency | Moderate | Low to high | Makes pacing gentler and more trust-building | Makes the campaign feel harsher and more relentless |
| Obstacle Density Growth Rate | Conservative | Low to moderate | Makes later chapters more board-friction-heavy | Keeps campaign cleaner but may reduce variation |
| Power-Up Reliance Growth Rate | Conservative | Low to moderate | Makes later levels more combo-driven | Keeps core matching more primary throughout |

## Visual/Audio Requirements

| Event | Visual Feedback | Audio Feedback | Priority |
|-------|----------------|---------------|----------|
| Chapter identity shift | New chapter framing/theme emphasis | Optional chapter transition cue | Medium |
| Milestone level completion | Stronger chapter-close framing | Chapter milestone cue | Medium |
| Difficulty spike signaling | Optional subtle expectation-setting before hard levels | None required in MVP | Low |

## UI Requirements

| Information | Display Location | Update Frequency | Condition |
|-------------|-----------------|-----------------|-----------|
| Chapter titles/identity | Chapter map | On chapter entry / map view | When chapter context is shown |
| Milestone level distinction | Chapter map node treatment | On map load | For chapter-closing or key levels |
| Difficulty/internal pacing tags | Designer-facing tools/docs, not player HUD | During production | For content planning and review |

## Acceptance Criteria

- [ ] The 50-level sample has an explicit chapter structure with a readable campaign curve.
- [ ] Each chapter has a defined teaching/emphasis identity.
- [ ] New goals, obstacles, and power-up complexity are introduced in a controlled sequence.
- [ ] Difficulty rises in waves with identifiable recovery moments.
- [ ] Frozen ingredient goals and obstacle-heavy play are introduced after simpler foundations are established.
- [ ] The pacing guidance is specific enough to inform real level production decisions.
- [ ] The campaign plan explicitly identifies out-of-scope content for the sample.
- [ ] Chapter boundaries support map readability and production planning.
- [ ] Performance: not applicable as a runtime-critical system; success is measured by production guidance clarity and reduced rework.
- [ ] The system does not attempt to replace per-level tuning with vague campaign slogans.

## Open Questions

| Question | Owner | Deadline | Resolution |
|----------|-------|----------|-----------|
| Should the 50-level sample be locked to `5 x 10`, or is a `4 x 12 + finale`-style structure materially better for pacing? | User | Before broad level production | `5 x 10` recommended, not yet fully locked |
| Which exact obstacle family should be the first non-jelly friction layer in the campaign? | User + obstacle tuning pass | Before obstacle-heavy content planning | Not yet decided |
| How early should frozen ingredient goals appear: Chapter 2, Chapter 3, or later? | User + content pacing pass | Before detailed chapter planning | Not yet decided |
| Should one chapter be explicitly designed as a power-up mastery chapter, or should power-up emphasis stay distributed? | User + power-up tuning pass | Before late-sample content planning | Not yet decided |

Open question notes:

1. None of these questions block the existence of a solid pacing framework.
2. The main goal is to prevent the 50-level sample from becoming a collection of
   disconnected handcrafted boards.
