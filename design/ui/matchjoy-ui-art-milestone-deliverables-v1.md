# MatchJoy UI Art Milestone Deliverables v1

> Status: Draft
> Last Updated: 2026-03-30
> Purpose: Define milestone deliverables for the first UI art phase of MatchJoy.

## Goal

This document turns the UI art workflow into a concrete milestone checklist.

It answers:

- what must exist,
- what counts as done,
- what should be reviewed,
- and what is required before Unity UI integration begins.

## Milestone Name

`UI Art Phase 1 - Playable Slice Visual Foundation`

## Required Deliverable Groups

### Group A. Direction And Planning Docs

Must exist:

- `ui-art-design-workflow.md`
- `matchjoy-ui-art-direction-v1.md`
- `matchjoy-ui-reference-source-pack-v1.md`
- `matchjoy-iphone17-artboard-spec.md`
- `matchjoy-iphone17-lowfi-layout-spec-v1.md`

Done when:

- the visual direction is documented
- the device baseline is fixed
- the layout baseline is fixed
- the approved reference source pack is established

### Group B. Benchmark And Moodboard

Must exist:

- benchmark board in PureRef or Figma
- mood board with 2-3 candidate routes
- one chosen route
- one rejected route summary

Done when:

- benchmark board clearly covers HUD / Board / Results
- candidate routes are visible and comparable
- one route is selected for production

### Group C. Low-Fidelity Screen Structure

Must exist:

- full-screen low-fidelity frame for iPhone 17 portrait
- HUD low-fidelity module spec
- Board low-fidelity module spec
- Results low-fidelity module spec

Done when:

- module hierarchy is approved
- board is clearly the visual hero
- safe areas are respected

### Group D. High-Fidelity Design Specs

Must exist:

- HUD hifi spec
- Board hifi spec
- Results hifi spec
- screen draw guides for InLevel / ResultsWin / ResultsFail

Done when:

- each core screen has enough detail to be drawn consistently
- material, hierarchy, and motion intent are documented

### Group E. Systemization Docs

Must exist:

- visual tokens spec
- component inventory spec
- component variants spec
- Figma production checklist
- Figma naming and export spec

Done when:

- reusable components are identified
- variants are controlled
- Figma production can proceed without naming chaos

### Group F. Production And Export Planning

Must exist:

- asset production list
- asset export matrix
- motion spec
- FX spec

Done when:

- asset types are categorized
- export format decisions are defined
- motion and FX rules are documented

## Required Figma Outputs

For UI Art Phase 1 to be considered complete, Figma must contain:

1. one approved `InLevel` frame
2. one approved `ResultsWin` frame
3. one approved `ResultsFail` frame
4. one `Components` page with first-pass reusable components
5. one `References` page with annotated source board

## Required Review Exports

Must export at minimum:

- `matchjoy-iphone17-inlevel-v1.png`
- `matchjoy-iphone17-results-win-v1.png`
- `matchjoy-iphone17-results-fail-v1.png`
- one component-page overview export

## Approval Gates

### Gate 1. Structural Approval

Checks:

- iPhone 17 portrait frame approved
- low-fidelity layout approved
- board still reads as the hero

### Gate 2. Visual Approval

Checks:

- chosen route is clear
- HUD / Board / Results look like the same product
- pressure state styling is readable

### Gate 3. System Approval

Checks:

- visual tokens are stable
- component naming and variants are stable
- Figma page organization is usable

### Gate 4. Production Approval

Checks:

- asset production list approved
- export matrix approved
- motion / FX direction approved

## Exit Condition

UI Art Phase 1 is complete only when:

1. the three core screens exist as approved high-fidelity frames,
2. the component page exists,
3. the export-ready review images exist,
4. the asset plan and motion plan are approved,
5. the team can begin Unity UI integration without inventing the visual system on the fly.
