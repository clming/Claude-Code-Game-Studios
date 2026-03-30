# MatchJoy Figma Page Template Spec v1

> Status: Draft
> Last Updated: 2026-03-30
> Purpose: Define what each Figma page should contain for the first-pass MatchJoy UI art file.

## Goal

This document standardizes the structure of the Figma file so production stays organized.

## Page 00_References

Purpose:

- store benchmark captures
- store annotations
- store route comparison samples

Must contain:

1. `Benchmarks / HUD`
2. `Benchmarks / Board`
3. `Benchmarks / Results`
4. `Keep / Adapt / Avoid` note area
5. one shortlist area for selected route traits

Rule:

- no final UI production should happen on this page

## Page 01_LowFi

Purpose:

- hold grayscale structure work
- validate spacing and hierarchy before polishing

Must contain:

1. one iPhone 17 baseline frame
2. one full low-fidelity screen layout
3. low-fidelity HUD module study
4. low-fidelity Board module study
5. low-fidelity Results module study

Rule:

- keep grayscale and low-detail

## Page 02_HiFi

Purpose:

- hold the approved first-pass high-fidelity screens

Must contain:

1. `InLevel / v1`
2. `ResultsWin / v1`
3. `ResultsFail / v1`
4. optional alternates placed to the side, not mixed into main review flow

Rule:

- this page is for review-ready visual compositions

## Page 03_Components

Purpose:

- hold reusable UI modules and styles

Must contain:

1. cards
2. pills
3. badges
4. star row
5. icon starters
6. token notes and style references

Rule:

- components must be built from approved screen decisions

## Page 04_ExportPrep

Purpose:

- prepare export-ready duplicates
- isolate review exports from working frames

Must contain:

1. approved `InLevel` export frame
2. approved `ResultsWin` export frame
3. approved `ResultsFail` export frame
4. component overview export frame

Rule:

- no notes, guides, or construction clutter on export frames
