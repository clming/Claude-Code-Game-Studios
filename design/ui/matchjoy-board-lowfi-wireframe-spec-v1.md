# MatchJoy Board Low-Fidelity Wireframe Spec v1

> Status: Draft
> Last Updated: 2026-03-30
> Purpose: Define the board hero module wireframe for the iPhone 17 portrait baseline.

## Frame Slot

Place inside the board hero zone from the low-fidelity layout spec.

Recommended outer frame:

- x: `20 pt`
- width: `353 pt`
- target height: `430-470 pt`

## Internal Layout

### Row 1. Overline

Purpose:

- chapter / level metadata

Placement:

- top-left aligned
- light metadata role

### Row 2. Title + Stage Pill Row

Purpose:

- identify the board as the active work surface
- show the current interaction state

Placement:

- title on the left
- stage pill on the right

Content:

- board title
- stage pill like `BOARD READY` or `SETTLING`

Rule:

- title must read as the board module title
- pill must remain compact and capsule-like

### Row 3. Subtitle / Description Row

Purpose:

- provide one short line of board context

Placement:

- below title row
- left aligned

Content:

- board size summary
- one short status sentence

### Row 4. Board Frame

Purpose:

- hold the playable board
- dominate the center of the screen

Placement:

- centered in the module
- largest single rectangle on screen

Rule:

- the grid region should occupy most of this frame
- decorative framing should never shrink the playable grid too far

### Row 5. Status Badge

Purpose:

- summarize pace, pressure, and goal progress

Placement:

- upper-right or mid-right relative to board frame

Content:

- star-band pace
- pressure label
- goal completion compact summary

Rule:

- keep readable but secondary to the grid

### Row 6. Footnote

Purpose:

- show concise run state ledger

Placement:

- bottom-left under the board frame

Content:

- moves in reserve
- goals closed

## Visual Priority

1. board frame
2. board title
3. stage pill
4. status badge
5. subtitle
6. footnote
7. overline

## Figma Construction Notes

Use grayscale placeholders for:

- one thin overline label
- one bold title block
- one capsule pill on the right
- one subtitle strip
- one large square or near-square board frame
- one compact right-side badge block
- one thin footer strip

## Do Not Do

- do not let title and pill consume too much vertical space
- do not make the badge larger than the title row
- do not let the board frame lose hero status to chrome
