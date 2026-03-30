# MatchJoy UI Asset Batch Plan v1

> Status: Draft
> Last Updated: 2026-03-30
> Purpose: Break the first-pass UI asset list into production batches that can be executed immediately.

## Goal

This plan converts the asset production list into actionable batches.

Each batch should have:

- a clear purpose,
- a clear asset list,
- a clear destination folder,
- a clear review checkpoint.

## Batch 01. Core Surfaces

Purpose:

- establish the three hero UI surfaces that define the product look

Assets:

1. HUD Card Background
2. Board Outer Frame
3. Board Inner Tray Base
4. Results Card Background
5. Board Stage Pill Base
6. Board Status Badge Base

Destination:

- `MatchJoyProj/Assets/MatchJoy/Art/UI/Panels/`
- `MatchJoyProj/Assets/MatchJoy/Art/UI/Badges/`

Review checkpoint:

- can the product look be recognized with only these six assets?

## Batch 02. Key Symbols And Accents

Purpose:

- add identity and readability support without over-polishing too early

Assets:

1. HUD Accent Strip
2. Results Accent Strip
3. Chapter Badge Frame
4. Star Glyph Set
5. Goal Icon Set

Destination:

- `Panels/`
- `Badges/`
- `Icons/`

Review checkpoint:

- do the UI cards now feel branded rather than generic?

## Batch 03. Secondary Polish

Purpose:

- add controlled richness once the main structure is approved

Assets:

1. HUD Divider Marks
2. Board Gloss Overlay
3. Results Sparkle Overlay
4. Corner Highlight Set
5. Sugar Dust Overlay

Destination:

- `Panels/`
- `Icons/`
- `FX/`

Review checkpoint:

- does the polish increase quality without harming readability?

## Batch 04. Motion / FX Support

Purpose:

- support first-pass motion and state punctuation

Assets:

1. Warm Glow Sprite
2. Gold Reward Glow Sprite
3. Pressure Glow Sprite
4. Small Spark Burst
5. Star Reveal Streak

Destination:

- `FX/`

Review checkpoint:

- are motion-support assets sufficient for first-pass UI animation tests?

## Production Rule

Do not start Batch 03 until Batch 01 and Batch 02 are approved.
Do not start Batch 04 until the static screens are visually approved.

## Immediate Start Recommendation

If starting art production right now, begin with:

- HUD Card Background
- Board Outer Frame
- Board Inner Tray Base
- Results Card Background

Those four assets will define most of the screen's visual identity.
