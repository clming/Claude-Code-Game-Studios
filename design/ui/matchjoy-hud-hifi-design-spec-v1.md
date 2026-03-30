# MatchJoy HUD High-Fidelity Design Spec v1

> Status: Draft
> Last Updated: 2026-03-30
> Purpose: Define the first-pass high-fidelity art direction for the HUD order card.

## Design Intent

The HUD should read like a premium confectionery order ticket.

It is not a flat mobile top bar.
It should feel:

- light,
- layered,
- crafted,
- operational,
- warm.

## Material Direction

The HUD card surface should feel like:

- laminated bakery ticket stock,
- slightly frosted,
- softly rounded,
- gently lifted from the background.

Preferred treatment:

- warm cream base
- soft caramel edge shading
- very subtle top highlight
- minimal internal shadow for depth

Avoid:

- hard metallic bevels
- dark heavy shadow stacks
- flat white placeholder cards

## Shape Language

- rounded outer rectangle
- medium-large corner radius
- one accent strip or accent bead near top edge
- compact rounded modules for pace or tags

## Typography Hierarchy

### Level Title

Role:

- strongest text after move count

Style target:

- rounded bold display face
- warm brown text

### Moves Count

Role:

- largest operational information on the card

Style target:

- heavier, larger numeric emphasis
- contrast stronger than all supporting labels

### Goal Text

Role:

- dense, readable support text

Style target:

- clean sans
- smaller than title and moves
- slightly darker than footer text

### Footer Ledger

Role:

- quiet support text

Style target:

- compact
- low contrast relative to title and moves

## Color Direction

Base colors:

- cream card body
- baked sugar beige support surfaces
- caramel brown typography

Accent colors:

- syrup orange accent strip
- strawberry coral for high-pressure state
- honey gold for stable-positive state

Pressure state rule:

- when move pressure rises, the accent should shift warmer and stronger
- the entire card should not turn red; only accent and emphasis should intensify

## Decorative Elements

Allowed:

- subtle candy-wrapper corner gloss
- thin syrup accent bar
- tiny icon separator for goals or pace
- light decorative sparkles in very small amounts

Avoid:

- ornate filigree
- dense candy clutter behind text
- large mascots inside the card

## Motion Direction

The HUD should support:

- gentle pulse when entering pressure window
- soft numeric emphasis when moves change
- lightweight shimmer on pace update

Avoid:

- large bounce on every move
- persistent glowing animation loops

## Asset Notes

Likely assets needed:

- HUD card background
- top accent strip
- optional move icon
- optional goal icon family
- small divider marks

## Figma Build Notes

When creating the first high-fidelity frame:

- start from the approved low-fidelity layout
- apply one polished card background
- add one accent strip treatment
- establish title, goal, moves, pace, and footer styles as reusable text styles
- do not add extra decorative sub-panels unless hierarchy remains clear
