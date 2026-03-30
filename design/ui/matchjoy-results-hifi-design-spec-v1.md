# MatchJoy Results High-Fidelity Design Spec v1

> Status: Draft
> Last Updated: 2026-03-30
> Purpose: Define the first-pass high-fidelity art direction for the results card.

## Design Intent

The results card should feel like a branded order completion card, not a generic popup.

On win, it should feel:

- warm,
- satisfying,
- premium,
- lightly celebratory.

On loss, it should feel:

- clear,
- branded,
- gently disappointing,
- not harsh or system-like.

## Material Direction

The result card should feel like:

- a polished receipt card,
- premium menu-card stock,
- lightly glossy confection packaging insert.

Preferred treatment:

- brighter card face than HUD
- subtle panel depth
- clearer central emphasis than the HUD card
- accent strip or crest area near the top

Avoid:

- full-screen dark overlay dominance
- loud casino burst treatment in the first pass
- generic system-dialog styling

## Shape Language

- centered rounded card
- soft top accent strip or header notch
- compact badge near top
- generous spacing around headline

## Typography Hierarchy

### Headline

Role:

- strongest text in the card

Style target:

- bold rounded display style
- clearly larger than every other text element

### Performance Summary

Role:

- second most important line

Style target:

- tighter, premium, concise

### Supporting Detail

Role:

- one short explanatory paragraph

Style target:

- readable but not dominant

### Footer

Role:

- progression or retry tone

Style target:

- compact and quiet

## Color Direction

Win:

- warm cream and honey gold base
- orange / syrup accent
- optional light sparkle or soft confetti treatment

Loss:

- warm cream base retained
- muted coral / baked strawberry accent

Rule:

- failure should not collapse into cold gray or generic red alert UI

## Decorative Elements

Allowed:

- subtle star row or star glyph area
- tiny syrup-glow accent
- soft candy sparkle treatment
- tasteful badge frame near top

Avoid:

- too many trophies or ribbons
- particle clutter behind text
- giant iconography crowding the headline

## Motion Direction

The results card should support:

- rise-and-settle entrance
- soft star reveal stagger on win
- light accent glow on state reveal

Avoid:

- long float-in delays
- repeated bouncing loops
- noisy reward explosions in first pass

## Asset Notes

Likely assets needed:

- result card background
- accent strip
- badge frame
- star glyphs or star icons
- optional small sparkle overlays

## Figma Build Notes

For the first high-fidelity pass:

- build the card silhouette first
- establish headline and summary hierarchy before adding stars
- keep the card visually premium in grayscale before adding warm color
- create win and loss variants from one shared structure
