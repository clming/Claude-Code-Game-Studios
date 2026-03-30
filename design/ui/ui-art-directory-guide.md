# MatchJoy UI Art Directory Guide

> Status: Active
> Last Updated: 2026-03-30
> Purpose: Define where UI design files and final art assets should live.

## Design-Side Location

Store UI design work under:

- `design/ui/`

Use these subfolders:

- `design/ui/references/`
  - benchmark captures
  - PureRef exports
  - annotated reference boards

- `design/ui/figma/`
  - Figma exports
  - page snapshots
  - review PNGs

- `design/ui/exports/`
  - milestone review exports
  - delivery PNGs for discussion

The markdown planning and design-spec documents stay directly in:

- `design/ui/`

## Unity Runtime Art Location

Store final runtime UI art under:

- `MatchJoyProj/Assets/MatchJoy/Art/UI/`

Use these subfolders:

- `Panels/`
  - HUD card backgrounds
  - board shell panels
  - results card backgrounds

- `Icons/`
  - move icons
  - goal icons
  - star icons

- `Badges/`
  - chapter badge assets
  - status badges
  - stage pill graphics

- `FX/`
  - glow sprites
  - streaks
  - sparkles
  - overlay FX assets

- `Exports/`
  - temporary integration-ready exports before final sorting

## Working Rule

- design source and planning stay in `design/ui/`
- Unity-ready art stays in `Assets/MatchJoy/Art/UI/`
- do not mix design-source discussion files into the Unity asset tree
- do not place final runtime PNG/SVG assets into `design/ui/`
