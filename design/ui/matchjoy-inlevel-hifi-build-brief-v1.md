# MatchJoy InLevel HiFi Build Brief v1

## Goal
Build the first high-fidelity `InLevel` gameplay screen for MatchJoy.

This brief is intended for direct execution in Figma using the established art direction, token system, and component rules.

## Screen Context
Screen type:
- InLevel gameplay screen

Device baseline:
- iPhone 17 portrait
- 393 x 852 pt working frame
- 1206 x 2622 px review export

Primary purpose:
- Present the board as the visual hero
- Keep goals and move pressure readable at a glance
- Make the session feel like a polished candy workshop product, not a debug prototype

## Visual Direction
Use the `Candy Workshop Counter` direction from:
- @design/ui/matchjoy-ui-art-direction-v1.md

The screen should feel:
- warm
- handcrafted
- confectionery themed
- readable at gameplay speed
- playful but not noisy

Avoid:
- flat utility-dashboard styling
- casino-like saturation overload
- hyper-glossy mobile ad aesthetics
- overdecorating the board zone

## Build Order
### Layer 01 - Background Base
Create:
- page background wash
- soft counter-surface gradient
- subtle vignette around the board zone

Intent:
- establish warmth and depth without stealing focus from gameplay

### Layer 02 - HUD Card
Create:
- top HUD card background
- accent strip
- chapter badge area
- title / overline / footer text slots
- goal progress row
- move pressure block

Use docs:
- @design/ui/matchjoy-hud-hifi-design-spec-v1.md
- @design/ui/matchjoy-visual-tokens-spec-v1.md

### Layer 03 - Board Shell
Create:
- board outer frame
- board inner tray
- stage pill
- board title block
- board status badge
- board footnote area

Use docs:
- @design/ui/matchjoy-board-hifi-design-spec-v1.md
- @design/ui/matchjoy-component-variants-spec-v1.md

### Layer 04 - Board Contents Placeholder
Create:
- playable grid placeholder
- blocked-cell placeholder treatment
- candy tile placeholder circles / rounded chips
- selected-cell sample state

Important:
- this is still a design-screen placeholder for composition
- keep tile shapes simple and aligned to the game board proportions already used in Unity

### Layer 05 - Secondary Polish
Create:
- corner gleam details
- subtle sugar dust near the outer shell only
- small highlight passes on card surfaces
- controlled shadow tuning between HUD and board zones

Rule:
- polishing should support hierarchy, not bury it

## Layout Targets
Use the low-fi page layout from:
- @design/ui/matchjoy-iphone17-lowfi-layout-spec-v1.md

Target reading order:
1. HUD summary
2. Board shell and board contents
3. stage / pressure status
4. small supporting footer text

Priority rule:
- the board must remain the main visual mass in the middle of the screen
- the HUD should read quickly and not become taller than necessary

## Required Text Slots
Prepare realistic placeholder text for the hi-fi mockup:
- Chapter 1
- Level 01
- Jelly Workshop
- Collect 18 red candies
- Moves 22
- Pace Stable
- Board Ready
- 9 x 9 candy tray

Do not use lorem ipsum.

## Component Extraction Targets
After the screen is visually stable, extract these components:
- Card/HUD
- Badge/Chapter
- GoalItem
- PaceStrip
- Card/Board
- Pill/BoardStage
- Badge/Status

These should become the first reusable component set in the Figma file.

## Review Criteria
The screen is ready for review when:
- the board is clearly the hero
- HUD and board belong to the same visual family
- decorative details do not interfere with fast reading
- pressure status is visible without dominating the screen
- the screen feels game-like even without final FX

## Export Requirement
Export one review image to:
- design/ui/exports/matchjoy-inlevel-hifi-review-v1.png

## Related Docs
- @design/ui/matchjoy-inlevel-screen-draw-guide-v1.md
- @design/ui/matchjoy-board-hifi-design-spec-v1.md
- @design/ui/matchjoy-hud-hifi-design-spec-v1.md
- @design/ui/matchjoy-visual-tokens-spec-v1.md
- @design/ui/matchjoy-component-inventory-spec-v1.md
