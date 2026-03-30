# MatchJoy Visual Tokens Spec v1

> Status: Draft
> Last Updated: 2026-03-30
> Purpose: Define the first-pass visual token system for MatchJoy UI art and later Unity integration.

## Goal

These tokens are the shared visual language for:

- Figma styles
- component construction
- later Unity UI skin integration

They are not final forever, but they are the first stable version.

## Color Tokens

### Base Surfaces

- `surface/cream/base` = warm cream main card surface
- `surface/cream/soft` = lighter cream for brighter panels
- `surface/sugar/beige` = baked sugar beige for support surfaces
- `surface/caramel/deep` = deep caramel for rim or edge emphasis

### Text

- `text/title/warm-dark` = primary warm brown for titles
- `text/body/caramel` = standard support copy color
- `text/support/muted` = footer and low-emphasis support copy
- `text/inverse/light` = light text for dark or saturated pills when needed

### Accent

- `accent/syrup/orange` = main active accent
- `accent/honey/gold` = positive / premium highlight accent
- `accent/coral/pressure` = pressure-state accent
- `accent/strawberry/fail` = fail-state accent
- `accent/mint/support` = occasional support accent only

### FX

- `fx/glow/warm` = subtle warm glow
- `fx/glow/gold` = premium reward glow
- `fx/glow/coral` = pressure glow

## Radius Tokens

- `radius/card/lg` = primary HUD / Board / Results card radius
- `radius/card/md` = secondary internal card radius
- `radius/pill/full` = stage pills and small status capsules
- `radius/badge/md` = compact badge radius

Working numeric suggestion for first pass:

- card large: `28`
- card medium: `20`
- badge medium: `16`
- pill full: `999`

## Stroke Tokens

- `stroke/card/soft` = subtle edge definition for cards
- `stroke/frame/warm` = board frame support line
- `stroke/pill/accent` = stage pill outline when needed

Rule:

- strokes should support silhouette clarity, not dominate it

## Shadow Tokens

- `shadow/card/soft` = main floating-card shadow
- `shadow/card/tight` = compact card or badge shadow
- `shadow/frame/inner` = board inner separation shadow

Rule:

- shadows must stay restrained and warm
- avoid large dark generic mobile shadows

## Highlight Tokens

- `highlight/top/soft` = top-edge card sheen
- `highlight/frame/syrup` = board frame gloss
- `highlight/reward/gold` = result reward accent highlight

## Typography Tokens

### Display

- `type/display/lg` = result headline
- `type/display/md` = board title
- `type/display/sm` = HUD title / numeric emphasis support

### Numeric

- `type/numeric/hero` = move count
- `type/numeric/support` = smaller status numerics

### Body

- `type/body/md` = standard support copy
- `type/body/sm` = subtitle and footnote copy
- `type/body/xs` = overline and ledger copy

## Spacing Tokens

- `space/4`
- `space/8`
- `space/12`
- `space/16`
- `space/20`
- `space/24`
- `space/32`

These should be enough for first-pass Figma layout consistency.

## Usage Mapping

### HUD

- main card uses `surface/cream/base`
- accent strip uses `accent/syrup/orange`
- move count uses `type/numeric/hero`
- footer uses `type/body/xs` or `type/body/sm`

### Board

- shell body uses `surface/cream/base`
- rim uses `surface/caramel/deep`
- stage pill uses `accent/syrup/orange` or `accent/coral/pressure`
- badge uses `surface/sugar/beige`

### Results

- win card uses `surface/cream/soft` + `accent/honey/gold`
- fail card uses `surface/cream/soft` + `accent/strawberry/fail`
- headline uses `type/display/lg`

## Figma Setup Notes

Create these as:

- color styles
- text styles
- effect styles
- radius/stroke values documented in the component page notes

## Unity Mapping Notes

Later, Unity should map these tokens into:

- sprite backgrounds
- text style guides
- color constants
- panel and badge skins

The token names should stay stable even if values are tuned.
