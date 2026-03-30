# MatchJoy Results Fail HiFi Build Brief v1

## Goal
Build the first high-fidelity `Results Fail` screen for MatchJoy.

This screen should communicate setback clearly while preserving warmth and motivation.
It should feel like a softer production-game fail state, not a punishment screen.

## Screen Context
Screen type:
- Results Fail

Device baseline:
- iPhone 17 portrait
- 393 x 852 pt working frame
- 1206 x 2622 px review export

Primary purpose:
- communicate that the attempt ended
- preserve emotional safety
- encourage retry without flattening the product tone
- keep the same design family established by InLevel and Results Win

## Relationship To Other Screens
This screen must inherit the same product language as:
- InLevel
- Results Win

Carry over:
- card shell treatment
- typography hierarchy
- badge logic
- warm surface materials
- shared spacing rhythm

Adjust:
- lower energy than Results Win
- quieter glow treatment
- more explanatory summary tone
- softer accent emphasis

Avoid:
- red alarm-screen styling
- shame-oriented language
- punitive dark palettes
- making failure feel visually cheaper than success

## Visual Direction
Use the `Candy Workshop Counter` direction from:
- @design/ui/matchjoy-ui-art-direction-v1.md

Desired emotional read:
- gentle setback
- inviting retry
- still premium and crafted
- calm enough to keep the player in flow

## Build Order
### Layer 01 - Background And Tone
Create:
- warm but slightly quieter background field
- subtle card support glow
- minimal decorative atmosphere

Intent:
- reduce energy from the win screen without feeling cold or empty

### Layer 02 - Results Card Shell
Create:
- results fail card background
- softer accent strip
- overline area
- badge area
- headline zone
- summary zone
- footer / retry guidance area

Use docs:
- @design/ui/matchjoy-results-hifi-design-spec-v1.md
- @design/ui/matchjoy-visual-tokens-spec-v1.md

### Layer 03 - Outcome Zone
Create:
- failure-state icon or reduced star area
- calm feedback emphasis zone
- support for a retry cue

Rule:
- the outcome zone should read immediately but must not look harsh

### Layer 04 - Guidance Summary
Create:
- concise reason line
- move or goal shortfall summary
- a gentle encouragement line that implies another attempt

Use placeholder copy that feels supportive and game-ready.

### Layer 05 - Controlled Polish
Create:
- softer edge highlights
- limited sparkle or dust use
- quiet depth separation around the card

Rule:
- this screen should be polished, but clearly lower-energy than the win screen

## Required Text Slots
Prepare realistic placeholder text for the hi-fi mockup:
- Chapter 1
- Level 01 Incomplete
- Batch Needs Another Pass
- 2 goals left unfinished
- Out of moves this round
- Try the recipe again

Do not use lorem ipsum.

## Layout Priorities
Reading order:
1. fail state recognition
2. concise reason / summary
3. retry encouragement
4. supporting chapter or level context

Priority rule:
- the card is still the hero
- the summary must be easier to read than the decorative layer
- the screen should retain dignity and warmth

## Component Extraction Targets
After the screen is visually stable, extract or refine:
- Card/Results
- Badge/Chapter
- Badge/Status or fail-state outcome marker
- Accent/ResultsStrip

These should remain compatible with the same component system used by the win screen.

## Review Criteria
The screen is ready for review when:
- the fail state is clear within one glance
- the tone feels encouraging rather than punishing
- the results card still belongs to the same UI family as the win screen
- the summary reads quickly on mobile
- the screen feels calmer than win while still polished

## Export Requirement
Export one review image to:
- design/ui/exports/matchjoy-results-fail-hifi-review-v1.png

## Related Docs
- @design/ui/matchjoy-results-fail-screen-draw-guide-v1.md
- @design/ui/matchjoy-results-hifi-design-spec-v1.md
- @design/ui/matchjoy-component-variants-spec-v1.md
- @design/ui/matchjoy-motion-spec-v1.md
