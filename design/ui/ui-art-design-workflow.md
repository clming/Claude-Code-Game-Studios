# UI Art Design Workflow

> Status: Active
> Last Updated: 2026-03-30
> Purpose: Define the studio-standard workflow for UI art design before UI code implementation.

## Decision

For this studio, UI work now follows:

1. Code foundation stabilizes.
2. UI art design starts.
3. UI visual direction is approved.
4. UI asset specs are defined.
5. UI implementation starts in Unity.

This replaces the previous habit of extending procedural placeholder UI too far.

## Primary Tools

### 1. Layout and System Design

- Figma Design
  - Main use: UI layout, component system, screen composition, annotations, handoff.
  - Why: collaborative design files, components, prototypes, and dev handoff.
  - Source: https://www.figma.com/design/

- FigJam
  - Main use: flow maps, mood boards, UI hierarchy boards, review boards.
  - Why: early exploration and review before high-fidelity design.
  - Source: https://help.figma.com/hc/en-us/articles/14563969806359-What-is-Figma

### 2. Reference Collection

- PureRef
  - Main use: mood boards, benchmark capture boards, shape-language boards, FX reference boards.
  - Why: persistent floating reference board for art production.
  - Source: https://www.pureref.com/

### 3. Raster UI Art

- Adobe Photoshop
  - Main use: paintovers, panel textures, badge polish, glow treatment, promo mockups, UI paint.
  - Why: strong raster editing and compositing workflow.
  - Source: https://www.adobe.com/products/photoshop/

### 4. Vector UI Art

- Adobe Illustrator
  - Main use: icons, badges, vector ornaments, scalable UI marks, shape kits.
  - Why: precise vector control for reusable UI assets.
  - Source: https://www.adobe.com/products/illustrator.html

### 5. UI / FX Motion

- Spine
  - Main use: 2D skeletal UI animation, reward popups, badge motion, elastic banners, lightweight character/mascot support.
  - Why: game-focused 2D animation workflow with runtime integration.
  - Source: https://esotericsoftware.com/

### 6. Runtime Integration

- Unity UI
  - Main use: runtime HUD, overlays, results panels, production integration.
  - Why: this project already uses Unity runtime UI and existing presenter/view scripts.
  - Source: https://docs.unity3d.com/2023.2/Documentation/Manual/UIElements.html

## Required Deliverables

Before large-scale UI code implementation, the following must exist:

1. UI mood board
2. UI benchmark board
3. Device artboard specification
4. Visual direction sheet
5. Component inventory
6. Screen layout wireframes
7. High-fidelity mockups for:
   - HUD
   - Board shell
   - Results panel
8. Asset production list
9. Motion and FX notes
10. Unity integration checklist

## Device Baseline

For MatchJoy, the default UI artboard baseline is now:

- device: iPhone 17
- orientation: portrait
- logical frame: `393 x 852 pt`
- export target: `1206 x 2622 px`
- density assumption: `@3x`

All first-pass UI design should be created against this baseline before secondary adaptations.

Mandatory layout rules:

- reserve top safe-area space for the Dynamic Island / status region
- reserve bottom safe-area space for the Home Indicator region
- do not place critical buttons, reward text, or primary HUD numbers flush to screen edges

## Reference Sources

References must come from four buckets.

### A. Internal Source of Truth

These define product intent and must be read first:

- `design/gdd/game-concept.md`
- `design/gdd/board-grid-state-system.md`
- `design/gdd/level-difficulty-curve-and-content-pacing-system.md`
- current runtime UI scripts under `MatchJoyProj/Assets/MatchJoy/Scripts/UI/`

### B. Benchmark Games

Use these to study hierarchy, polish, readability, reward language, and motion tone.

- Candy Crush franchise / King materials
  - Example source: https://www.king.com/corporate-and-media/posts/press-releases/match-bake-enjoy-candy-crush-launches-tasty-the-official-candy-crush-desserts-book/

- Royal Match official materials
  - Example source: https://www.royalmatch.com/en/collection

- Additional match-3 references should be collected from official store pages, trailers, and official websites only.

### C. Platform and Tool Guidance

Use official docs only:

- Figma official docs and product pages
- Unity official UI docs
- Adobe official product pages
- Spine official docs/site

### D. Studio-Owned Direction

Once a visual direction is chosen, the project's own design files become the top reference.

That means:

- Figma source file
- exported asset sheets
- design review notes
- approved motion samples

## Workflow

### Phase 1. Reference Gathering

Output:

- PureRef board
- benchmark screenshots
- categorized notes

Tasks:

- collect 3-5 benchmark games
- isolate HUD patterns
- isolate board framing patterns
- isolate result/reward panel patterns
- isolate icon/badge/button treatments
- isolate FX language

### Phase 2. Visual Direction

Output:

- direction board with 2-3 candidate routes

Each route must define:

- color system
- shape language
- material style
- typography direction
- icon treatment
- lighting / glow treatment
- motion tone

### Phase 3. Screen Design

Output:

- low-fidelity wireframes
- then high-fidelity UI mockups

Mandatory screens:

- in-level HUD
- board shell
- results panel

All of those screens must first be reviewed on the iPhone 17 portrait baseline frame.

### Phase 4. Asset Planning

Output:

- asset list by type

Examples:

- panel backgrounds
- accent bars
- badges
- button states
- icons
- candy UI ornaments
- result stars
- glow strips
- reward burst FX

### Phase 5. Motion / FX Planning

Output:

- motion spec sheet

Must define:

- selection feedback
- match resolve feedback
- reward panel entrance
- star reveal
- win/lose accent treatment

### Phase 6. Unity Integration

Only after previous phases are approved:

- export assets
- assign slicing/import settings
- replace placeholder colors and text-only chrome
- wire prefab/layout hierarchy
- integrate motion and FX

## Approval Gates

The team should not move forward until each gate is approved.

1. Reference board approved
2. Visual direction approved
3. Screen mockups approved
4. Asset list approved
5. Motion spec approved
6. Unity integration approved

## Immediate Project Next Step

For MatchJoy / Jelly Candy Workshop, the next correct step is:

1. create the benchmark board,
2. create the first visual direction sheet,
3. design first-pass high-fidelity mockups for HUD, Board, and Results,
4. then return to Unity integration.

## Current Layout Entry Doc

@design/ui/matchjoy-iphone17-artboard-spec.md

@design/ui/matchjoy-iphone17-lowfi-layout-spec-v1.md

@design/ui/matchjoy-ui-reference-source-pack-v1.md

## Low-Fidelity Module Specs

@design/ui/matchjoy-hud-lowfi-wireframe-spec-v1.md

@design/ui/matchjoy-board-lowfi-wireframe-spec-v1.md

@design/ui/matchjoy-results-lowfi-wireframe-spec-v1.md

## High-Fidelity Module Specs

@design/ui/matchjoy-hud-hifi-design-spec-v1.md

@design/ui/matchjoy-board-hifi-design-spec-v1.md

@design/ui/matchjoy-results-hifi-design-spec-v1.md

## Figma Execution Docs

@design/ui/matchjoy-figma-production-checklist-v1.md

@design/ui/matchjoy-figma-naming-and-export-spec-v1.md

## Screen Draw Guides

@design/ui/matchjoy-inlevel-screen-draw-guide-v1.md

@design/ui/matchjoy-results-win-screen-draw-guide-v1.md

@design/ui/matchjoy-results-fail-screen-draw-guide-v1.md

## Visual Tokens

@design/ui/matchjoy-visual-tokens-spec-v1.md

## Component System Docs

@design/ui/matchjoy-component-inventory-spec-v1.md

@design/ui/matchjoy-component-variants-spec-v1.md

## Asset Production Docs

@design/ui/matchjoy-asset-production-list-v1.md

@design/ui/matchjoy-asset-export-matrix-v1.md

## Motion And FX Docs

@design/ui/matchjoy-motion-spec-v1.md

@design/ui/matchjoy-fx-spec-v1.md

## Milestone Deliverables

@design/ui/matchjoy-ui-art-milestone-deliverables-v1.md

## Figma Structure Templates

@design/ui/matchjoy-figma-page-template-spec-v1.md

@design/ui/matchjoy-figma-initial-frame-checklist-v1.md

## Directory Guide

@design/ui/ui-art-directory-guide.md

## Asset Batch Execution Docs

@design/ui/matchjoy-ui-asset-batch-plan-v1.md

@design/ui/matchjoy-ui-asset-start-order-v1.md

## Production Sprint Docs

@design/ui/matchjoy-ui-art-production-sprint-v1.md


## Screen Build Briefs

@design/ui/matchjoy-inlevel-hifi-build-brief-v1.md

@design/ui/matchjoy-results-win-hifi-build-brief-v1.md

@design/ui/matchjoy-results-fail-hifi-build-brief-v1.md


## Review Pack Docs

@design/ui/matchjoy-screen-review-pack-spec-v1.md

