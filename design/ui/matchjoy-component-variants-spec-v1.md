# MatchJoy Component Variants Spec v1

> Status: Draft
> Last Updated: 2026-03-30
> Purpose: Define first-pass component variants for MatchJoy UI design.

## Goal

This document limits and standardizes the first wave of component variants.

The goal is reuse with control, not explosion of states.

## Variant Rules

### HUD Card Variants

Component:

- `Card/HUD`

Variants:

- `state=default`
- `state=pressure`

Meaning:

- `default` = stable early or mid-board state
- `pressure` = final stretch / high urgency state

### Board Shell Variants

Component:

- `Card/Board`

Variants:

- `state=ready`
- `state=scouting`
- `state=settling`
- `state=nomatch`

Meaning:

- `ready` = initial ready state
- `scouting` = selection and reading state
- `settling` = accepted swap resolution state
- `nomatch` = rejected swap feedback state

### Results Card Variants

Component:

- `Card/Results`

Variants:

- `state=win`
- `state=fail`

### Stage Pill Variants

Component:

- `Pill/BoardStage`

Variants:

- `label=ready`
- `label=scout`
- `label=settling`
- `label=nomatch`

### Status Badge Variants

Component:

- `Badge/Status`

Variants:

- `pressure=stable`
- `pressure=tight`
- `pressure=final`

### Star Row Variants

Component:

- `Reward/StarRow`

Variants:

- `stars=1`
- `stars=2`
- `stars=3`

## Variant Control Rules

1. First-pass variants must stay semantic, not cosmetic.
2. Do not create separate variants for tiny copy changes.
3. Prefer text/content swaps inside the same structural variant where possible.
4. Add new variants only when they represent a real product state.

## Figma Build Notes

When building variants:

- start with one master base
- duplicate into controlled semantic variants
- keep padding, typography, and structure stable across variants
- change only the properties that represent a real state change

## Unity Mapping Intent

Later, these variants should make it easier to map approved visual states into runtime states such as:

- board ready
- selection refresh
- resolve settle
- rejected swap
- win
- fail
- pressure window
