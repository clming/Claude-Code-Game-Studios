# MatchJoy UI Art Production Sprint v1

## Goal
Turn the current UI art design package into a first-pass production sprint for the playable slice.

Scope is limited to the three critical screens:
- InLevel
- Results Win
- Results Fail

Target device baseline:
- iPhone 17 portrait
- 393 x 852 pt working frame
- 1206 x 2622 px review export

## Sprint Outcome
At the end of this sprint, the team should have:
- One Figma file with the required page structure
- Low-fi and hi-fi frames for the three core screens
- The first production-ready component set
- The first export batch for Unity integration
- A review package for visual approval

## Sprint Structure
The sprint is organized into five working blocks.

### Block 01 - Figma File Setup
Deliverables:
- Create the Figma file using the page structure in @design/ui/matchjoy-figma-page-template-spec-v1.md
- Create the initial iPhone 17 portrait frames listed in @design/ui/matchjoy-figma-initial-frame-checklist-v1.md
- Add the approved references and benchmark board captures

Exit condition:
- The file structure is stable and ready for design work.

### Block 02 - Core Screen Layout
Deliverables:
- Build the InLevel low-fi frame
- Build the Results Win low-fi frame
- Build the Results Fail low-fi frame
- Confirm module proportions against @design/ui/matchjoy-iphone17-lowfi-layout-spec-v1.md

Exit condition:
- Screen-level information hierarchy is readable before decorative styling.

### Block 03 - Hi-Fi Screen Pass
Deliverables:
- Apply the visual direction from @design/ui/matchjoy-ui-art-direction-v1.md
- Apply tokens from @design/ui/matchjoy-visual-tokens-spec-v1.md
- Build hi-fi versions of InLevel, Results Win, and Results Fail
- Match composition and detail level described in the draw guides

Exit condition:
- All three screens read as one consistent game UI family.

### Block 04 - Component And Asset Production
Deliverables:
- Build the first component set from @design/ui/matchjoy-component-inventory-spec-v1.md
- Apply variants from @design/ui/matchjoy-component-variants-spec-v1.md
- Produce Batch 01 and Batch 02 assets from @design/ui/matchjoy-ui-asset-batch-plan-v1.md
- Name exports using @design/ui/matchjoy-figma-naming-and-export-spec-v1.md

Exit condition:
- The design file contains reusable components and the first exportable asset batch.

### Block 05 - Review And Handoff Pack
Deliverables:
- Export review PNGs for InLevel, Results Win, and Results Fail
- Export the minimum viable visual batch listed in @design/ui/matchjoy-ui-asset-start-order-v1.md
- Package motion notes from @design/ui/matchjoy-motion-spec-v1.md and FX notes from @design/ui/matchjoy-fx-spec-v1.md
- Confirm milestone gates in @design/ui/matchjoy-ui-art-milestone-deliverables-v1.md

Exit condition:
- The team has a reviewable visual package and a clean first handoff set.

## Working Order
Recommended sequence:
1. Set up the file and references.
2. Build low-fi InLevel first.
3. Build hi-fi InLevel next.
4. Reuse that visual language for Results Win.
5. Adapt Results Fail from the same component system.
6. Build shared components after the first hi-fi pass is visually stable.
7. Export the minimum viable asset batch.

## Daily Focus Plan
### Day 01
- Set up the Figma file structure
- Create the required frames
- Place benchmark and mood board material

### Day 02
- Finish low-fi InLevel
- Finish low-fi Results Win
- Finish low-fi Results Fail

### Day 03
- Build hi-fi InLevel
- Lock the HUD, board shell, and board badge look

### Day 04
- Build hi-fi Results Win
- Build hi-fi Results Fail
- Normalize card language across all screens

### Day 05
- Build reusable components
- Produce the first export batch
- Export review boards

## First Review Checklist
Use this review before exporting assets:
- The three screens share one visual language.
- HUD, board shell, and results cards feel like parts of the same product.
- The screen remains readable inside iPhone 17 portrait safe areas.
- The board remains the primary focal area during gameplay.
- Win and fail are visually distinct without feeling like different games.
- Decorative detail does not crowd the gameplay area.

## Immediate Start Recommendation
If the team starts work right now, use this order:
1. Build the InLevel hi-fi frame first.
2. Extract `Card/HUD`, `Card/Board`, and `Pill/BoardStage` from that frame.
3. Build the Results Win frame second.
4. Build the Results Fail frame third.
5. Export `hud-card-bg-v1`, `board-shell-outer-v1`, `board-tray-inner-v1`, and `results-card-bg-v1` as the first runtime art package.

## Related Docs
- @design/ui/ui-art-design-workflow.md
- @design/ui/matchjoy-ui-art-direction-v1.md
- @design/ui/matchjoy-iphone17-artboard-spec.md
- @design/ui/matchjoy-figma-production-checklist-v1.md
- @design/ui/matchjoy-ui-asset-batch-plan-v1.md
- @design/ui/matchjoy-ui-asset-start-order-v1.md
