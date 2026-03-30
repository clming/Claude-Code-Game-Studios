# MatchJoy HUD Low-Fidelity Wireframe Spec v1

> Status: Draft
> Last Updated: 2026-03-30
> Purpose: Define the HUD card wireframe for the iPhone 17 portrait baseline.

## Frame Slot

Place inside the HUD card band from the low-fidelity layout spec.

Recommended outer frame:

- x: `20 pt`
- y: below top metadata band
- width: `353 pt`
- height: `132-156 pt`

## Internal Layout

### Row 1. Overline

Purpose:

- route metadata
- chapter framing

Placement:

- top-left aligned
- small, compact, single-line

Content:

- `WORKSHOP ROUTE / CHAPTER X`

### Row 2. Title Block

Purpose:

- identify level and order context

Placement:

- directly under overline
- left aligned

Content:

- level title or level number
- optional micro subtitle if needed

### Row 3. Goal Block

Purpose:

- communicate the active order contents

Placement:

- left-middle block
- medium-width text area

Content:

- `Goals X/Y`
- one concise progress line

Rule:

- this block should stay denser than the moves block
- it should not become the hero element

### Row 4. Moves Block

Purpose:

- provide the main operational number

Placement:

- right side
- vertically dominant in the card

Content:

- large move number
- small `moves left` label under it

Rule:

- this is the strongest element in the HUD card

### Row 5. Pace Block

Purpose:

- show quality / star-band forecast

Placement:

- directly under or adjacent to the moves block

Content:

- short pace label
- compact star forecast line

Rule:

- supports the move count, does not compete with it

### Row 6. Footer Ledger

Purpose:

- summarize progress in compact accounting form

Placement:

- bottom edge of the card
- full-width or near full-width

Content:

- spent moves
- completed goals

Rule:

- visually quieter than all above rows

## Visual Priority

1. move number
2. level title
3. goal block
4. pace block
5. footer ledger
6. overline

## Figma Construction Notes

Use these placeholder blocks in grayscale first:

- one thin overline bar
- one medium title block
- one medium goal text block
- one tall numeric block on the right
- one short pace strip
- one thin footer strip

## Do Not Do

- do not make the goal block wider than needed
- do not let the pace block become a second headline
- do not stack too many small labels above the title
