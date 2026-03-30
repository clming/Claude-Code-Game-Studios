# MatchJoy UI Art Direction v1

> Status: Draft
> Last Updated: 2026-03-30
> Purpose: Establish the first-round UI visual direction for the playable single-level slice.

## Product Framing

This game's UI should not feel like a generic debug prototype.

It should feel like:

- a polished candy workshop,
- handcrafted rather than sterile,
- bright and appetizing without becoming noisy,
- readable on mobile first,
- emotionally responsive during pressure windows.

## Core Theme

Working title for the direction:

`Candy Workshop Counter`

This direction treats the play screen as a premium confectionery workstation:

- the board is the main tray,
- the HUD is the order card,
- the result panel is the order receipt / completion card.

## Visual Goals

### 1. Make the board the visual center

The board should read as the hero module.

That means:

- stronger frame presence than plain floating cells,
- clear top metadata,
- clear state badge,
- subtle material richness,
- readable separation from background.

### 2. Make HUD feel like an order card

The HUD should read like a production ticket:

- chapter and level as route metadata,
- moves as large operational number,
- goals and pace as production targets,
- footer as concise progress ledger.

### 3. Make results feel like a branded completion card

The result panel should not feel like a system popup.

It should read like:

- completed order card on win,
- interrupted batch notice on loss.

## Style Pillars

### Shape Language

- rounded rectangles
- soft capsule pills
- thick friendly frames
- slightly chunky, toy-like geometry

Avoid:

- razor-thin sci-fi lines
- flat corporate dashboard geometry
- hard angular esports styling

### Color Language

Base neutrals:

- warm cream
- baked sugar beige
- caramel brown

Accents:

- tangerine
- strawberry red
- syrup gold
- mint green in small support roles

Pressure colors:

- coral red
- deeper baked orange

Avoid:

- cold grayscale UI
- purple-heavy generic mobile UI
- neon cyber palettes

### Typography Direction

Use typography with warmth and rounded confidence.

Working rule:

- bold rounded display face for big titles and counts
- clean readable sans for support copy

The final font choice should be made in design, not guessed in code.

## Material Direction

UI surfaces should feel like:

- frosted candy packaging,
- soft laminated cards,
- glossy syrup accents,
- subtle baked-paper order tickets.

This implies:

- mild gradients,
- soft edge highlights,
- inner glows used sparingly,
- very restrained shadows.

Avoid:

- flat placeholder rectangles,
- heavy drop-shadow stacks,
- glassmorphism overuse.

## Motion Direction

Motion should feel:

- soft,
- elastic,
- edible,
- confident,
- never hyperactive.

Target motion examples:

- HUD pulse when entering pressure window
- board status pill snap on state change
- result card rise-and-settle entrance
- star reveal with stagger and glow

Avoid:

- long floaty transitions,
- noisy particle spam,
- flashy casino-style overstimulation

## Screen Priorities

### HUD

Must include:

- overline
- title block
- goal block
- move count block
- pace block
- footer ledger

### Board Shell

Must include:

- overline
- title
- stage pill
- subtitle
- right-side status badge
- footnote

### Results

Must include:

- overline
- badge
- main headline
- performance summary
- supporting detail
- footer

## Asset Families To Design

### Panels

- HUD card background
- board frame background
- result card background

### Accents

- capsule pills
- small status tags
- progress separators
- corner ornaments

### Iconography

- goal icons
- move icon
- result stars
- chapter marker

### FX

- board settle glow
- selection sparkle
- result burst
- star reveal streak

## First Benchmark Intent

When collecting references, evaluate each against these questions:

1. How does it frame the board as the hero?
2. How does it show urgency without clutter?
3. How does it separate metadata from reward messaging?
4. How does it make win/lose states feel branded rather than generic?

## What This Means For Code

The current Unity-side UI scripts are now considered:

- layout skeletons,
- temporary integration points,
- not final visual design.

From this point onward, UI coding should follow approved art direction instead of inventing the direction in code.

## Immediate Next Design Tasks

1. Build a PureRef benchmark board.
2. Build a Figma mood board.
3. Create 2-3 visual routes for HUD, Board, and Results.
4. Choose one route.
5. Produce first-pass high-fidelity mockups.
