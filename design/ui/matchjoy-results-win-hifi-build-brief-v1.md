# MatchJoy Results Win HiFi Build Brief v1

## Goal
Build the first high-fidelity `Results Win` screen for MatchJoy.

This screen should feel like the emotional reward payoff of the same product family established in the InLevel gameplay screen.

## Screen Context
Screen type:
- Results Win

Device baseline:
- iPhone 17 portrait
- 393 x 852 pt working frame
- 1206 x 2622 px review export

Primary purpose:
- celebrate success clearly
- reinforce the candy workshop tone
- present the star outcome and summary without clutter
- feel rewarding without becoming noisy or casino-like

## Relationship To InLevel
This screen must inherit from the InLevel hi-fi screen, not reinvent the product look.

Carry over:
- card edge treatment
- corner highlight logic
- accent strip language
- typography hierarchy
- warm confectionery surface materials

Do not carry over:
- gameplay-density pressure
- over-busy board detail
- hard urgency colors

## Visual Direction
Use the `Candy Workshop Counter` direction from:
- @design/ui/matchjoy-ui-art-direction-v1.md

Desired emotional read:
- warm reward
- crafted success
- polished but approachable
- celebratory in a restrained premium-mobile way

Avoid:
- jackpot-machine styling
- confetti overload
- giant metallic trophies
- reward effects that bury the copy or stars

## Build Order
### Layer 01 - Background And Atmosphere
Create:
- softened warm background field
- subtle center glow behind the results card
- very light decorative sparkle support near the upper half only

Intent:
- support the card as the focal object

### Layer 02 - Results Card Shell
Create:
- results card background
- accent strip
- overline area
- badge area
- headline zone
- summary zone
- footer zone

Use docs:
- @design/ui/matchjoy-results-hifi-design-spec-v1.md
- @design/ui/matchjoy-visual-tokens-spec-v1.md

### Layer 03 - Reward Zone
Create:
- star row placement
- reward glow support layer
- space for score / outcome emphasis

Rule:
- stars should be the emotional peak of the screen
- stars must remain legible when static, before any motion is added

### Layer 04 - Supporting Summary
Create:
- concise completion summary block
- optional chapter / level context line
- next-step encouragement area

Use placeholder copy that feels like a shipped game, not a test scene.

### Layer 05 - Controlled Polish
Create:
- edge gleam details
- soft sugar sparkle accents
- controlled highlight around the star area
- subtle depth separation between card shell and background

Rule:
- polish should support celebration, not become the content itself

## Required Text Slots
Prepare realistic placeholder text for the hi-fi mockup:
- Chapter 1
- Level 01 Complete
- Fresh Batch Approved
- Three Stars
- Goals cleared with 7 moves left
- Ready for the next recipe

Do not use lorem ipsum.

## Layout Priorities
Reading order:
1. win state recognition
2. stars / reward zone
3. summary line
4. next-step encouragement

Priority rule:
- the results card is the hero
- the reward zone must sit above the summary in visual importance
- the screen should feel cleaner than the InLevel screen because gameplay density is gone

## Component Extraction Targets
After the screen is visually stable, extract or refine:
- Card/Results
- Reward/StarRow
- Badge/Chapter
- Accent/ResultsStrip

These must remain compatible with the shared component system.

## Review Criteria
The screen is ready for review when:
- it feels like a rewarding finish to the InLevel screen
- the stars read instantly as the focal outcome
- the card shell belongs to the same UI family as the HUD and board shell
- the copy remains readable at mobile viewing size
- decorative accents support celebration without visual chaos

## Export Requirement
Export one review image to:
- design/ui/exports/matchjoy-results-win-hifi-review-v1.png

## Related Docs
- @design/ui/matchjoy-results-win-screen-draw-guide-v1.md
- @design/ui/matchjoy-results-hifi-design-spec-v1.md
- @design/ui/matchjoy-component-variants-spec-v1.md
- @design/ui/matchjoy-motion-spec-v1.md
- @design/ui/matchjoy-fx-spec-v1.md
