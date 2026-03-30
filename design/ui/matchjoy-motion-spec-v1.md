# MatchJoy Motion Spec v1

> Status: Draft
> Last Updated: 2026-03-30
> Purpose: Define first-pass UI motion behavior for MatchJoy.

## Goal

This spec defines how key UI states should move.

Motion should feel:

- soft,
- elastic,
- warm,
- readable,
- intentionally restrained.

## Global Motion Rules

1. Motion must support clarity before spectacle.
2. No UI motion should overpower board readability.
3. Motion should use short, readable timings.
4. Avoid stacking multiple attention-grabbing motions at once.

## Timing Bands

### Micro

Use for:

- badge state changes
- pill snaps
- tiny icon reactions

Suggested duration:

- `0.12s - 0.18s`

### Standard

Use for:

- HUD pressure shift
- result summary reveal
- small card emphasis

Suggested duration:

- `0.20s - 0.32s`

### Major

Use for:

- result card entrance
- star reveal cadence

Suggested duration:

- `0.36s - 0.55s`

## Motion Beats

### 1. HUD Pressure Entry

Trigger:

- remaining moves enters pressure window

Behavior:

- accent strip warms up
- move count receives a soft emphasis pulse
- pace strip gets a short shimmer or contrast bump

Rule:

- only one pulse cycle, not looping

### 2. Board Stage Pill Change

Trigger:

- ready -> scouting -> settling -> nomatch transitions

Behavior:

- compact snap
- tiny overshoot
- quick settle

Rule:

- should feel responsive, not flashy

### 3. Board Pressure Shift

Trigger:

- board enters tighter move state

Behavior:

- shell accent warms subtly
- badge contrast increases slightly

Rule:

- no large board pulse

### 4. Results Card Entrance

Trigger:

- run resolves to win or fail

Behavior:

- card rises in
- eases into final position
- slight settle or gentle scale finish

Rule:

- keep it premium and concise

### 5. Star Reveal

Trigger:

- win card active

Behavior:

- one-by-one reveal
- slight stagger
- tiny glow support

Rule:

- reveal order must stay readable
- avoid noisy bounce chains

## Motion Do Not Do

- do not loop celebratory motion forever
- do not bounce entire cards repeatedly
- do not trigger multiple pulses across HUD, board, and results at the same time
- do not use slow floaty motion that delays readability

## Production Notes

Likely production tools:

- Figma Smart Animate for rough exploration
- Spine for production-ready UI motion candidates
- Unity for final runtime implementation
