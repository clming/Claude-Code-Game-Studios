# MatchJoy Figma Naming And Export Spec v1

> Status: Draft
> Last Updated: 2026-03-30
> Purpose: Define naming, component structure, and export rules for first-pass Figma UI production.

## Naming Principles

Names should be:

- readable
- stable
- grouped by function
- easy to map back to Unity integration later

Avoid vague names like:

- `Frame 24`
- `Rectangle 81`
- `Group Copy 2`

## Frame Naming

Use this pattern:

- `MatchJoy / iPhone17 / ScreenName / Version`

Examples:

- `MatchJoy / iPhone17 / InLevel / v1`
- `MatchJoy / iPhone17 / ResultsWin / v1`
- `MatchJoy / iPhone17 / ResultsFail / v1`

## Component Naming

Use slash-based naming.

### Cards

- `Card/HUD/Base`
- `Card/Board/Base`
- `Card/Results/Base`

### Pills and Badges

- `Pill/BoardStage/Ready`
- `Pill/BoardStage/Settling`
- `Badge/Status/Base`
- `Badge/Chapter/Base`

### Typography

- `Text/HUD/Overline`
- `Text/HUD/Title`
- `Text/HUD/Moves`
- `Text/Board/Title`
- `Text/Results/Headline`
- `Text/Results/Summary`

### Icons

- `Icon/Goal/Base`
- `Icon/Move/Base`
- `Icon/Star/Filled`
- `Icon/Star/Empty`

## Layer Grouping

Within each screen frame, group layers in this order:

1. `Background`
2. `HUD`
3. `Board`
4. `ResultsOverlay`
5. `Guides`
6. `Notes`

Within each module, group in this order:

1. `Surface`
2. `Accent`
3. `Content`
4. `FX`

## Export Rules

### Review Exports

For stakeholder review:

- export full screens as PNG
- use `3x` export
- keep naming human-readable

Examples:

- `matchjoy-iphone17-inlevel-v1.png`
- `matchjoy-iphone17-results-win-v1.png`
- `matchjoy-iphone17-results-fail-v1.png`

### Asset Exports

For reusable pieces:

- export vectors as SVG where appropriate
- export panel textures or painted assets as PNG
- keep decorative overlays separated from base cards when possible

## Export Readiness Rules

A frame is export-ready only when:

1. temporary notes are hidden or removed
2. guide overlays are hidden
3. layer names are cleaned up
4. component instances are stable
5. visual state is the approved one

## Unity Mapping Intent

These names should later help map assets into runtime categories such as:

- HUD background
- board shell background
- results panel background
- stage pill
- status badge
- stars and icons

The goal is not one-to-one auto-binding yet.
The goal is to avoid chaos when art integration starts.
