# MatchJoy iPhone 17 Artboard Spec

> Status: Draft
> Last Updated: 2026-03-30
> Purpose: Lock the primary device baseline for UI art design and exports.

## Primary Device Baseline

All first-pass MatchJoy UI art should be designed for:

- device: iPhone 17
- orientation: portrait
- logical frame: `393 x 852 pt`
- export target: `1206 x 2622 px`
- scale model: `@3x`

## Why This Baseline

This gives the project:

- a modern tall-phone portrait canvas,
- a stable review size,
- a clear mobile-first hierarchy target,
- a realistic production frame for casual puzzle UI.

## Figma Setup

Create the main design frame as:

- width: `393`
- height: `852`
- unit: points-equivalent design space

Recommended frame naming:

- `MatchJoy / iPhone17 / HUD-Board-Results / v1`

## Export Rules

When exporting high-fidelity review images:

- target pixel size: `1206 x 2622 px`
- export scale: `3x`
- format:
  - PNG for review images
  - SVG for vector icon candidates where appropriate

## Safe Area Guidance

Design must respect:

- top safe area for Dynamic Island / system region
- bottom safe area for Home Indicator gesture area

Practical rule:

- do not place critical HUD copy or primary buttons flush against the top
- do not place CTA-like controls flush against the bottom edge
- keep major result text comfortably inside the central readable field

## Layout Zones

Recommended vertical zoning for first-pass mockups:

1. Top metadata band
   - chapter
   - level
   - route metadata

2. HUD card band
   - moves
   - goals
   - pace

3. Board hero zone
   - board shell
   - board labels
   - state badges

4. Result overlay zone
   - centered modal card
   - should not collide with top and bottom safe areas

## Review Rule

Every high-fidelity UI review must answer:

1. is the board still the visual hero on `393 x 852`?
2. are move count and goals legible at a glance?
3. does the result panel feel centered and premium without crowding?
4. do top and bottom regions remain safe on a tall phone?

## Secondary Adaptation

Only after the iPhone 17 portrait baseline is approved should the team adapt for:

- shorter portrait phones
- wider Android portrait phones
- tablet layouts
