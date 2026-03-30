# MatchJoy Screen Review Pack Spec v1

## Goal
Standardize the first review package for the three core MatchJoy UI art screens.

Scope:
- InLevel
- Results Win
- Results Fail

This document defines what should be exported, how versions should be labeled, and what must be visible in the first formal visual review.

## Review Pack Purpose
The review pack should allow the team to answer these questions quickly:
- Do the three screens belong to the same product family?
- Is the board still the gameplay hero in the InLevel screen?
- Are win and fail emotionally distinct without breaking the shared style?
- Is the visual language ready to move toward asset export and Unity integration?

## Required Review Images
Export these PNG review images:
- `matchjoy-inlevel-hifi-review-v1.png`
- `matchjoy-results-win-hifi-review-v1.png`
- `matchjoy-results-fail-hifi-review-v1.png`

Export location:
- `design/ui/exports/`

## Optional Supporting Review Images
If available, also export:
- one `component overview` board
- one `tokens overview` board
- one `asset batch preview` board

Suggested names:
- `matchjoy-components-review-v1.png`
- `matchjoy-tokens-review-v1.png`
- `matchjoy-asset-batch-preview-v1.png`

## Version Naming Rule
Use this format:
- `matchjoy-{screen-name}-{artifact-type}-v{number}.png`

Examples:
- `matchjoy-inlevel-hifi-review-v1.png`
- `matchjoy-results-win-hifi-review-v2.png`
- `matchjoy-results-fail-hifi-review-v3.png`

Rule:
- increase the version only when a new review round is intentionally prepared
- do not create multiple ad hoc suffixes like `final`, `final2`, `new`, or `latest`

## First Review Order
Recommended review sequence:
1. InLevel hi-fi screen
2. Results Win hi-fi screen
3. Results Fail hi-fi screen
4. shared component consistency check
5. asset export readiness check

Reason:
- InLevel defines the main product language
- Results Win should inherit that language and elevate reward
- Results Fail should inherit the same system while lowering emotional energy

## What Must Be Visible In Review
### InLevel
The exported screen must clearly show:
- HUD card
- board shell
- stage pill
- status badge
- board content placeholder composition
- supporting footer details

### Results Win
The exported screen must clearly show:
- reward card shell
- star zone
- win headline
- summary block
- supportive footer copy

### Results Fail
The exported screen must clearly show:
- fail card shell
- outcome zone
- concise summary block
- retry encouragement area
- calmer accent treatment than win

## Review Checklist
Use this checklist in the first visual review:
- The three screens share one clear visual family.
- Typography hierarchy is consistent.
- Card shell treatment is consistent across screens.
- Accent logic is consistent but adapts to context.
- InLevel is denser than the result screens in a deliberate way.
- Results Win feels more rewarding than Results Fail.
- Results Fail feels softer rather than punishing.
- Decorative details support hierarchy instead of competing with it.
- Safe areas are respected for iPhone 17 portrait.

## Approval Notes Format
When documenting review feedback, use this structure:
- `Screen:`
- `What works:`
- `Needs adjustment:`
- `Decision:`
- `Next export version:`

Example:
- `Screen: InLevel`
- `What works: board shell and HUD already feel unified`
- `Needs adjustment: stage pill contrast is too weak against the shell`
- `Decision: revise and re-export`
- `Next export version: v2`

## Exit Condition
The first review pack is complete when:
- all three core screens are exported
- naming is clean and versioned correctly
- each screen has at least one review note block
- the team can decide whether the visual direction is approved for asset production

## Related Docs
- @design/ui/matchjoy-inlevel-hifi-build-brief-v1.md
- @design/ui/matchjoy-results-win-hifi-build-brief-v1.md
- @design/ui/matchjoy-results-fail-hifi-build-brief-v1.md
- @design/ui/matchjoy-ui-art-production-sprint-v1.md
- @design/ui/matchjoy-figma-naming-and-export-spec-v1.md
