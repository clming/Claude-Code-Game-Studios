# MatchJoy FX Spec v1

> Status: Draft
> Last Updated: 2026-03-30
> Purpose: Define first-pass UI FX usage for MatchJoy.

## Goal

This spec defines the visual effects layer that supports the first-pass UI system.

FX should make the UI feel alive and premium, but never noisy.

## FX Principles

1. FX supports hierarchy, it does not replace hierarchy.
2. FX should be warm and edible, not explosive or harsh.
3. Use sparse, readable accents instead of constant activity.
4. The board must remain readable under all FX states.

## FX Families

### 1. Warm Glow

Use for:

- subtle card emphasis
- board accent polish
- premium warmth on important surfaces

Asset candidates:

- warm glow sprite

### 2. Reward Glow

Use for:

- star reveal
- win card emphasis
- premium completion highlight

Asset candidates:

- gold glow sprite
- reward highlight streak

### 3. Pressure Glow

Use for:

- pressure-state accent support
- tighter move-window emphasis

Asset candidates:

- coral glow sprite

### 4. Sparkle / Dust

Use for:

- small card polish
- subtle reward support
- ambient confection feeling

Asset candidates:

- tiny sparkle set
- sugar dust overlay

### 5. Reward Burst Support

Use for:

- restrained win punctuation

Asset candidates:

- small spark burst
- soft star streak

## FX Placement Rules

### HUD

Allowed:

- tiny accent shimmer
- subtle pressure glow on the accent strip

Avoid:

- constant sparkle around move count

### Board
n
Allowed:

- soft shell gloss
- subtle pressure warmth
- tiny stage pill support glow

Avoid:

- FX layered over the playable grid in a way that blocks readability

### Results

Allowed:

- win glow around stars or accent strip
- subtle fail-state warm coral edge treatment
- tiny sparkle support on win

Avoid:

- large particle storm
- confetti overload in first pass

## FX Intensity Levels

### Low

Use for:

- persistent polish accents

### Medium

Use for:

- state change punctuation
- card entrances

### High

Use only for:

- win reveal accent moments

Rule:

- first-pass UI should mostly live in Low and Medium.

## Asset Output Notes

Likely exports:

- PNG sprite overlays
- small glow textures
- streak sprites
- sparkle clusters

## Approval Rule

FX should only be added after the core screen hierarchy is already approved in static form.
