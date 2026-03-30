# MatchJoy Component Inventory Spec v1

> Status: Draft
> Last Updated: 2026-03-30
> Purpose: Define the first-pass reusable UI component inventory for MatchJoy.

## Goal

This inventory defines which UI pieces should become reusable components in Figma before Unity integration begins.

## Core Components

### 1. HUD Card

Component name:

- `Card/HUD/Base`

Contains:

- overline slot
- title slot
- goals slot
- moves slot
- pace slot
- footer slot
- accent strip

### 2. Board Shell

Component name:

- `Card/Board/Base`

Contains:

- overline slot
- title slot
- stage pill slot
- subtitle slot
- board frame slot
- status badge slot
- footnote slot

### 3. Results Card

Component name:

- `Card/Results/Base`

Contains:

- overline slot
- badge slot
- headline slot
- summary slot
- detail slot
- footer slot
- accent strip

## Supporting Components

### 4. Stage Pill

- `Pill/BoardStage/Base`

### 5. Status Badge

- `Badge/Status/Base`

### 6. Chapter Badge

- `Badge/Chapter/Base`

### 7. Star Row

- `Reward/StarRow/Base`

### 8. Goal Item

- `GoalItem/Base`

### 9. Pace Strip

- `PaceStrip/Base`

## Component Rules

1. Every component must have a stable base variant first.
2. Variant count should stay controlled in the first pass.
3. Do not turn every text block into its own component.
4. Components should map to real reusable UI modules, not arbitrary groups.

## Figma Page Target

These components belong on:

- `03_Components`

## Immediate Priority Order

Build in this order:

1. `Card/HUD/Base`
2. `Card/Board/Base`
3. `Card/Results/Base`
4. `Pill/BoardStage/Base`
5. `Badge/Status/Base`
6. `Reward/StarRow/Base`
