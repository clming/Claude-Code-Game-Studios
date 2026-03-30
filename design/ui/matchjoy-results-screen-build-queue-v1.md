# MatchJoy Results Screen Build Queue v1

## Purpose
Define the immediate build queue for the two result screens after the InLevel hi-fi screen is stable.

## Build Order
1. Results Win hi-fi screen
2. Results Fail hi-fi screen

## Why This Order
Results Win should be built first because it establishes the aspirational reward tone of the product.
Results Fail should then be derived from the same card system so it feels related, not separate.

## Shared Requirements
Both screens must:
- reuse the same card shell logic established in the InLevel screen
- keep the candy workshop theme intact
- remain legible on iPhone 17 portrait
- support future FX overlays without layout changes

## Results Win Focus
Visual priorities:
- reward tone
- positive warmth
- star reveal space
- clear primary headline
- summary readability

Key supporting docs:
- @design/ui/matchjoy-results-win-screen-draw-guide-v1.md
- @design/ui/matchjoy-results-hifi-design-spec-v1.md
- @design/ui/matchjoy-motion-spec-v1.md
- @design/ui/matchjoy-fx-spec-v1.md

## Results Fail Focus
Visual priorities:
- softer setback tone
- readable failure explanation
- next-attempt guidance
- emotional drop without harsh punishment

Key supporting docs:
- @design/ui/matchjoy-results-fail-screen-draw-guide-v1.md
- @design/ui/matchjoy-results-hifi-design-spec-v1.md
- @design/ui/matchjoy-motion-spec-v1.md

## Review Rule
Do not begin export prep for results screens until the InLevel hi-fi frame has locked:
- HUD card language
- board shell language
- card edge treatment
- accent color logic
- typography hierarchy

## Export Targets
- design/ui/exports/matchjoy-results-win-hifi-review-v1.png
- design/ui/exports/matchjoy-results-fail-hifi-review-v1.png
