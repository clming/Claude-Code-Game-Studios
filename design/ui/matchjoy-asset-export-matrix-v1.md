# MatchJoy Asset Export Matrix v1

> Status: Draft
> Last Updated: 2026-03-30
> Purpose: Define export formats and downstream handling for first-pass UI art assets.

## Goal

This matrix decides how each asset type should be exported and why.

## Export Matrix

### Card Backgrounds

- format: PNG
- reason: painted surfaces, gradients, highlight detail
- slicing note: evaluate 9-slice suitability where possible

### Board Shell Surfaces

- format: PNG
- reason: shell polish and highlight treatment likely raster-heavy
- slicing note: design with scalable frame logic in mind

### Pills and Badges

- format: SVG or PNG depending on treatment
- use SVG if shape is mostly clean vector
- use PNG if final look depends on painted highlight or texture

### Icons

- format: SVG preferred
- candidates: move icon, goal icons, stars, chapter markers

### Decorative Overlays

- format: PNG
- candidates: gloss, sparkles, sugar dust, reward burst overlays

### FX Helper Sprites

- format: PNG
- candidates: glow sprites, spark bursts, streaks

## Unity Integration Guidance

### Good SVG Candidates

- stars
- simple icons
- simple badge silhouettes
- divider marks

### Good PNG Candidates

- card faces
- shell textures
- gloss layers
- glow overlays
- sparkle sheets

## Naming Rule

Export names should be stable and lowercase.

Examples:

- `hud-card-bg-v1.png`
- `board-shell-outer-v1.png`
- `results-card-bg-v1.png`
- `icon-star-filled-v1.svg`
- `badge-status-base-v1.svg`

## Packaging Rule

Keep exports grouped by destination bucket:

- `hud/`
- `board/`
- `results/`
- `shared/`
- `fx/`

## First-Pass Export Scope

Do not export every decorative fragment early.

First-pass export scope should focus on:

1. key panel surfaces
2. core badges and pills
3. star and icon essentials
4. only minimal FX helpers
