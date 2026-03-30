# MatchJoy Asset Production List v1

> Status: Draft
> Last Updated: 2026-03-30
> Purpose: Define the first-pass production list for UI art assets.

## Goal

This list identifies which assets must be produced for the first playable UI slice.

It covers:

- HUD assets
- Board assets
- Results assets
- shared support assets
- motion / FX support assets

## Asset Categories

### A. HUD Assets

1. `HUD Card Background`
   - type: painted panel surface
   - use: main order-card base

2. `HUD Accent Strip`
   - type: decorative accent strip
   - use: top card emphasis

3. `HUD Divider Marks`
   - type: tiny separators
   - use: goal / pace / ledger separation

4. `Move Icon`
   - type: icon
   - use: optional support for move count block

5. `Goal Icon Set`
   - type: icon family
   - use: optional goal markers

### B. Board Assets

1. `Board Outer Frame`
   - type: main shell frame
   - use: hero tray silhouette

2. `Board Inner Tray Base`
   - type: support surface
   - use: playable region backing

3. `Board Stage Pill Base`
   - type: pill background
   - use: ready / scout / settling / nomatch states

4. `Board Status Badge Base`
   - type: compact badge surface
   - use: pace / pressure / goals summary

5. `Board Gloss Overlay`
   - type: decorative overlay
   - use: shell polish

### C. Results Assets

1. `Results Card Background`
   - type: main panel surface
   - use: win / fail shared card base

2. `Results Accent Strip`
   - type: state accent
   - use: win / fail header treatment

3. `Results Badge Frame`
   - type: compact badge surface
   - use: chapter / level badge

4. `Star Glyph Set`
   - type: icon set
   - use: result reward display

5. `Results Sparkle Overlay`
   - type: decorative overlay
   - use: subtle reward polish

### D. Shared Support Assets

1. `Chapter Badge Base`
2. `Status Badge Base`
3. `Pill Base`
4. `Tiny Sparkle Set`
5. `Corner Highlight Set`
6. `Sugar Dust Overlay`

### E. Motion / FX Support Assets

1. `Warm Glow Sprite`
2. `Gold Reward Glow Sprite`
3. `Pressure Glow Sprite`
4. `Small Spark Burst`
5. `Star Reveal Streak`

## Production Priority

### Priority 1

- HUD Card Background
- Board Outer Frame
- Board Inner Tray Base
- Results Card Background
- Board Stage Pill Base
- Board Status Badge Base

### Priority 2

- HUD Accent Strip
- Results Accent Strip
- Chapter Badge Frame
- Star Glyph Set
- Goal Icon Set

### Priority 3

- gloss overlays
- sparkle overlays
- sugar dust overlays
- FX helper sprites

## Asset Ownership Intent

### Painted / Raster Candidates

Best for:

- card backgrounds
- board shell surfaces
- gloss overlays
- sparkle overlays

### Vector Candidates

Best for:

- stars
- badges
- pill shapes
- icons
- dividers

### Hybrid Candidates

Best for:

- result cards with vector structure plus raster polish
- board shell with vector silhouette plus raster highlights

## Approval Rule

Do not produce lower-priority polish assets until Priority 1 assets are approved.
