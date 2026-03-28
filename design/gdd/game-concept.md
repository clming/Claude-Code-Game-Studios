# Game Concept: Jelly Candy Workshop

*Created: 2026-03-28*  
*Status: Draft*

---

## Elevator Pitch

> Jelly Candy Workshop is a mobile match-3 puzzle game where you swap candies
> and jelly pieces across a chapter map to clear jelly, collect sweets, and
> unlock frozen ingredients in a dreamy candy-house world.
>
> It aims to deliver a polished commercial-style match-3 experience in a
> scoped 50-level production sample, focused on teaching the studio workflow
> end to end.

---

## Core Identity

| Aspect | Detail |
| ---- | ---- |
| **Genre** | Mobile puzzle, match-3, level-based casual game |
| **Platform** | Mobile |
| **Target Audience** | Casual mobile puzzle players who enjoy polished, goal-driven progression |
| **Player Count** | Single-player |
| **Session Length** | 3-10 minute sessions |
| **Monetization** | None yet for prototype; designed in the style of a commercial F2P puzzle game |
| **Estimated Scope** | Medium (small production sample with 50 handcrafted levels) |
| **Comparable Titles** | Happy Match-style match-3 games, Candy Crush-style puzzle games, chapter-map mobile puzzlers |

---

## Core Fantasy

The player enters a dreamy candy-house world where every move creates bright,
pleasant chain reactions and progress feels tangible. The fantasy is not just
"matching pieces," but mastering a cheerful confectionery puzzle space where
smart swaps, satisfying combos, and chapter-by-chapter advancement create a
steady feeling of reward and momentum.

---

## Unique Hook

It is a polished commercial-style match-3 puzzle game, and also a deliberately
scoped "studio workflow training project" built to cover the full production
pipeline from concept, systems design, and level progression to implementation,
review, and sprint planning.

In player-facing terms, the hook is a dreamy candy workshop theme that combines
classic match-3 clarity with mixed level goals: clearing jelly, collecting
specific candies, and unlocking frozen ingredients.

---

## Player Experience Analysis (MDA Framework)

### Target Aesthetics (What the player FEELS)

| Aesthetic | Priority | How We Deliver It |
| ---- | ---- | ---- |
| **Sensation** (sensory pleasure) | 1 | Bright candy visuals, juicy matches, satisfying VFX/SFX, clear combo feedback |
| **Fantasy** (make-believe, role-playing) | 4 | Dreamy candy-house presentation and confectionery theme |
| **Narrative** (drama, story arc) | N/A | Minimal narrative, mostly implied through chapter progression |
| **Challenge** (obstacle course, mastery) | 2 | Escalating level goals, move limits, obstacle introduction, combo planning |
| **Fellowship** (social connection) | N/A | Not a focus in this scoped sample |
| **Discovery** (exploration, secrets) | 6 | New level mechanics, chapter progression, obstacle reveals |
| **Expression** (self-expression, creativity) | 7 | Limited expression through move choice and power-up usage |
| **Submission** (relaxation, comfort zone) | 3 | Short sessions, low-friction retries, familiar casual puzzle structure |

### Key Dynamics (Emergent player behaviors)

- Players scan the board for high-value swaps instead of only obvious matches.
- Players learn to preserve power pieces for stronger combo chains.
- Players adapt strategy based on mixed goals instead of playing every level the same way.
- Players build confidence through repeated short sessions and visible chapter progression.

### Core Mechanics (Systems we build)

1. Tile swapping and match resolution on a grid-based puzzle board
2. Cascades, refill, and combo generation
3. Goal-driven level completion with move limits
4. Power-up creation and activation (line clear, area blast, rainbow clear)
5. Chapter-map progression across 50 levels

---

## Player Motivation Profile

### Primary Psychological Needs Served

| Need | How This Game Satisfies It | Strength |
| ---- | ---- | ---- |
| **Autonomy** (freedom, meaningful choice) | Players choose swaps, power-up timing, and board priorities within each level | Supporting |
| **Competence** (mastery, skill growth) | Players improve at reading boards, planning combos, and solving mixed-goal levels efficiently | Core |
| **Relatedness** (connection, belonging) | Minimal in the current scope; theme creates warmth but not social connection | Minimal |

### Player Type Appeal (Bartle Taxonomy)

- [x] **Achievers** (goal completion, collection, progression) - How: Star goals, chapter unlocks, level completion, efficient clears
- [ ] **Explorers** (discovery, understanding systems, finding secrets) - How: Light system discovery through new obstacle and level goal combinations
- [ ] **Socializers** (relationships, cooperation, community) - How: Not a current focus
- [ ] **Killers/Competitors** (domination, PvP, leaderboards) - How: Not a current focus

### Flow State Design

- **Onboarding curve**: The opening levels teach swapping, simple matching, jelly clearing, and basic power-ups one concept at a time
- **Difficulty scaling**: New goals and obstacles are layered in gradually across chapters, increasing planning demands without overwhelming the player
- **Feedback clarity**: Strong visual and audio feedback shows match value, combo creation, and progress toward goals
- **Recovery from failure**: Levels are short and quickly replayable; failure should feel informative, not punishing

---

## Core Loop

### Moment-to-Moment (30 seconds)

The player scans the board, identifies a promising swap, creates a match,
triggers cascades or power-ups, and reassesses the updated board.

### Short-Term (5-15 minutes)

The player clears one or more levels by managing move limits and completing
mixed objectives such as clearing jelly, collecting target candies, and freeing
frozen ingredients.

### Session-Level (30-120 minutes)

A typical session includes clearing several levels, retrying a difficult one,
unlocking a new chapter node, and learning one new obstacle or power-up use case.

### Long-Term Progression

Over days of play, the player advances through a 50-level chapter map, faces
steadily more complex board states, and builds mastery over power-up usage and
goal prioritization.

### Retention Hooks

- **Curiosity**: New chapter layouts, new obstacle patterns, and upcoming mixed-goal levels
- **Investment**: Chapter progress, level completion streak, and visible map advancement
- **Social**: Not currently included
- **Mastery**: Better scores, more efficient clears, stronger understanding of combo opportunities

---

## Game Pillars

### Pillar 1: Juicy Match-3 Satisfaction

Every swap should feel responsive, readable, and rewarding.

*Design test*: If choosing between more spectacle and clearer feedback, prefer
the option that keeps board state understandable while still feeling juicy.

### Pillar 2: Clean Commercial Puzzle Structure

The game should feel like a polished mobile puzzle product, not just a loose prototype.

*Design test*: If choosing between a novel but messy system and a familiar but
well-executed level structure, choose the cleaner commercial structure.

### Pillar 3: Scoped Learnable Production

The project must remain small enough to complete as a 50-level training sample.

*Design test*: If a feature adds major scope without improving the core loop or
workflow-learning value, cut or defer it.

### Anti-Pillars (What This Game Is NOT)

- **NOT a direct commercial clone**: We can study reference products, but presentation and shipped assets must not copy copyrighted work
- **NOT a narrative-heavy adventure**: Story should support progression lightly, not dominate scope
- **NOT a meta-system-heavy live game**: No need for guilds, events, deep economy, or long-term live ops in this sample

---

## Inspiration and References

| Reference | What We Take From It | What We Do Differently | Why It Matters |
| ---- | ---- | ---- | ---- |
| Commercial match-3 mobile leaders | Clean board readability, move-limited levels, chapter-map pacing | Replace direct theming and assets with an original candy workshop wrapper | Validates the core market structure |
| Candy-themed puzzle games | Juicy visual feedback and satisfying candy identity | Use mixed goals anchored in a workshop/ingredient framing | Validates theme readability and player expectation |
| Casual chapter-map puzzlers | Short sessions and obvious progression | Scope the map tightly around 50 levels for a finishable sample | Keeps the project achievable |

**Non-game inspirations**: Candy-shop displays, pastel dessert packaging, toy-like UI clarity, confectionery color palettes.

---

## Target Player Profile

| Attribute | Detail |
| ---- | ---- |
| **Age range** | 12-40 |
| **Gaming experience** | Casual to mid-core mobile players |
| **Time availability** | Short daily sessions, usually 3-10 minutes at a time |
| **Platform preference** | Mobile |
| **Current games they play** | Match-3 puzzle games, light progression mobile games, short-session casual titles |
| **What they're looking for** | Satisfying puzzle clears, short-session progress, polished feedback, accessible challenge |
| **What would turn them away** | Confusing boards, cluttered UI, unfair randomness, overly punishing difficulty spikes |

---

## Technical Considerations

| Consideration | Assessment |
| ---- | ---- |
| **Recommended Engine** | Unity - strong 2D tooling, mature mobile workflow, suitable for puzzle board logic and UI-heavy production |
| **Key Technical Challenges** | Board state management, deterministic match resolution, power-up interaction rules, level data authoring, responsive UI flow |
| **Art Style** | 2D stylized |
| **Art Pipeline Complexity** | Low to Medium (placeholder or custom 2D assets, UI-heavy) |
| **Audio Needs** | Moderate - repeated but high-quality puzzle feedback, combo cues, menu sounds |
| **Networking** | None |
| **Content Volume** | 50 handcrafted levels, several goal types, a small set of obstacles and 3 core power-up families |
| **Procedural Systems** | Minimal; board initialization and refill logic only, not procedural campaign generation |

---

## Risks and Open Questions

### Design Risks

- Core match feel may seem too derivative if presentation does not differentiate enough
- Mixed-goal levels may become visually noisy if objective communication is weak

### Technical Risks

- Board resolution edge cases may create bugs in cascades, power-up chains, or refill logic
- Level authoring may become slow without a clean data format or editor workflow

### Market Risks

- The genre is crowded, so this project should be treated as a workflow-learning sample rather than a commercial differentiation bet
- If art and polish stay placeholder too long, the game may feel generic despite solid mechanics

### Scope Risks

- Adding too many obstacles or meta systems could easily expand the project beyond a clean 50-level sample
- Attempting to reproduce too much of a live-service puzzle game structure would reduce completion chances

### Open Questions

- Which obstacle set is enough for 50 levels without bloating scope? We should answer this during systems decomposition and early level planning
- Do we need a light booster/pre-level item system for the sample, or is core level progression enough? Prototype and scope-check before committing

---

## MVP Definition

**Core hypothesis**: Players find the candy-themed mixed-goal match-3 loop satisfying enough to play through repeated short sessions.

**Required for MVP**:
1. A working match-3 board with swaps, valid-match detection, cascades, and refill
2. At least one goal-completion level loop with move limits
3. Core power-ups: line clear, area blast, rainbow clear

**Explicitly NOT in MVP**:

- Large meta progression systems
- Live ops, monetization, or social features

### Scope Tiers (if budget/time shrinks)

| Tier | Content | Features | Timeline |
| ---- | ---- | ---- | ---- |
| **MVP** | 10 levels | Core board loop, one chapter slice, basic goals, core power-ups | 2-4 weeks |
| **Vertical Slice** | 15-20 levels | Core loop, polished UI flow, mixed goals, representative obstacle set | 4-8 weeks |
| **Alpha** | 50 levels | Full planned content, chapter map, all scoped systems, rough balancing | 8-14 weeks |
| **Full Vision** | 50 polished levels | Full sample polish, tuned progression, complete content presentation | 12-20 weeks |

---

## Next Steps

- [ ] Confirm Unity project setup (`/setup-engine`)
- [ ] Create game pillars document
- [ ] Decompose the concept into systems (`/map-systems`)
- [ ] Define the first architecture decision for board/data structure
- [ ] Prototype the core loop (swap -> match -> clear -> cascade -> refill)
- [ ] Validate the prototype feel with a playtest pass
- [ ] Plan the first sprint
