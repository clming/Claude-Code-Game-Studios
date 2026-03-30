# MatchJoy iPhone 17 Low-Fidelity Layout Spec v1

> Status: Draft
> Last Updated: 2026-03-30
> Purpose: Define the first-pass low-fidelity layout for the playable single-level screen on the iPhone 17 portrait baseline.

## Frame Baseline

Design frame:

- device: iPhone 17
- orientation: portrait
- logical frame: `393 x 852 pt`
- export reference: `1206 x 2622 px`

This layout spec is the structural bridge between:

- UI art direction,
- benchmark / mood board work,
- and future high-fidelity screen mockups.

## Layout Objective

The screen should immediately communicate three things:

1. what order the player is working on,
2. what board state they should focus on,
3. how the run is progressing.

The board must remain the visual hero.
HUD and results should support it, not compete with it.

## Global Margin System

Recommended first-pass margin system:

- outer left/right margin: `20 pt`
- top safe spacing after device-safe region: `16 pt`
- vertical module gap: `16 pt`
- internal card padding: `16 pt`
- small internal gap: `8 pt`
- medium internal gap: `12 pt`

## Screen Zones

### Zone 1. Top Safe Metadata Band

Purpose:

- absorb the top device-safe region,
- provide breathing room,
- host lightweight route metadata if needed.

Recommended height budget:

- `44-60 pt` usable content band after safe area

Content priority:

- optional route / chapter micro-label
- optional background ornament only

Rule:

- do not place large counters or dense copy here

### Zone 2. HUD Card Band

Purpose:

- present the active order card,
- summarize moves, goals, and pace.

Recommended frame:

- x: `20 pt`
- width: `353 pt`
- target height: `132-156 pt`

Required internal structure:

1. overline row
2. title / level row
3. goal summary block
4. moves + pace emphasis block
5. footer ledger

Priority inside HUD:

- `Moves` is the largest operational element
- `Goals` is the second most important information block
- `Pace` supports decision-making but should not overpower moves
- `Footer` stays compact and ledger-like

### Zone 3. Board Hero Zone

Purpose:

- act as the visual center of the play screen,
- hold the main play interaction,
- communicate board state and urgency.

Recommended frame:

- x: `20 pt`
- width: `353 pt`
- target height: `430-470 pt`

Internal structure:

1. board overline
2. board title
3. stage pill
4. board subtitle
5. board frame
6. right-side status badge
7. footnote

Board emphasis rules:

- this is the largest module on the screen
- it must visually outweigh the HUD card
- it must not be squeezed by decorative chrome
- the playable grid should occupy the majority of this zone

Grid sizing rule:

- keep the board frame square or near-square where possible
- for a `6x6` prototype board, the playable region should feel generous, not inset too deeply

### Zone 4. Bottom Breathing / Utility Band

Purpose:

- preserve visual rest near the bottom safe area,
- leave room for later CTA or meta hooks if needed,
- prevent the board from feeling hard-stopped by the screen edge.

Recommended height budget:

- `36-72 pt`

Rule:

- do not overload this band in the first-pass layout

### Zone 5. Results Overlay Zone

Purpose:

- host the win / loss overlay card,
- appear centered without fighting the board framing.

Recommended overlay card:

- centered horizontally
- width: `313-337 pt`
- target height: `220-280 pt`
- vertical center slightly above exact middle for a lighter feel

Internal structure:

1. overline
2. badge
3. headline
4. performance summary
5. support detail
6. footer

Overlay rules:

- the card must remain comfortably inside safe readable space
- do not let the result panel kiss the top safe area
- do not let the bottom edge sit too close to the Home Indicator region

## Hierarchy Rules

### Visual Priority

1. Board hero zone
2. Moves count inside HUD
3. Goal progress block
4. Result headline when overlay is active
5. Pace / badges / footers

### Reading Order

At level start:

1. HUD title and moves
2. board title and board stage pill
3. playable grid

During interaction:

1. playable grid
2. board status pill / badge
3. moves and goals

During result state:

1. result headline
2. result summary
3. supporting detail
4. background board remains readable but secondary

## Wireframe Blocks

### HUD Card Wireframe

- top-left overline
- below it, chapter + level title
- mid-left goals block
- mid-right large moves count
- below moves, pace line
- bottom ledger footer

### Board Wireframe

- top-left overline
- below it, board title
- top-right stage pill
- below title, subtitle
- center large board frame
- upper-right or mid-right status badge
- bottom footnote

### Results Card Wireframe

- top center overline
- badge below or integrated near top
- large centered headline
- summary line under headline
- supporting paragraph under summary
- footer at bottom

## Recommended First-Pass Size Ratios

Use these only as first-pass guides:

- HUD card: about `17%` of total screen height
- Board hero zone: about `50-55%` of total screen height
- bottom breathing band: about `5-8%`
- results overlay: about `26-32%` of total screen height when active

## Do Not Do

- do not let HUD become taller than the board frame
- do not put dense text above the board in multiple stacked rows
- do not make the result card full-screen in the first-pass mockup
- do not crowd the bottom safe area with decorative elements
- do not let status badges visually overpower the main grid

## Deliverable From This Spec

The next design artifact should be:

- one grayscale low-fidelity wireframe for iPhone 17 portrait

It should include:

- HUD card placement
- board hero placement
- result overlay placement
- rough spacing labels
- safe-area notes

## Exit Criteria

This low-fidelity layout is approved when:

1. the board clearly reads as the hero,
2. moves and goals are readable at a glance,
3. the result overlay feels centered and premium,
4. no primary information collides with safe areas,
5. the screen can transition naturally into high-fidelity art exploration.
