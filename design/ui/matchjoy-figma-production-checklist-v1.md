# MatchJoy Figma Production Checklist v1

> Status: Draft
> Last Updated: 2026-03-30
> Purpose: Define the execution checklist for producing the first-pass Figma UI mockups.

## Goal

This checklist turns the existing UI art documentation into concrete Figma production steps.

It is intended for building the first-pass high-fidelity screens for:

- HUD
- Board shell
- Results card

on the iPhone 17 portrait baseline.

## File Setup

1. Create one Figma file for the playable UI slice.
2. Create the primary frame at `393 x 852`.
3. Name the main frame:
   - `MatchJoy / iPhone17 / HUD-Board-Results / v1`
4. Add safe-area guides for top and bottom device regions.
5. Add a `Notes` area off-canvas for decisions and review notes.

## Page Structure

Create these pages in the Figma file:

1. `00_References`
2. `01_LowFi`
3. `02_HiFi`
4. `03_Components`
5. `04_ExportPrep`

## Production Order

### Step 1. References

On `00_References`:

- place benchmark captures
- group by HUD / Board / Results
- annotate each with `Keep`, `Adapt`, or `Avoid`

### Step 2. Low-Fidelity Layout

On `01_LowFi`:

- create the approved iPhone 17 portrait frame
- block out the top metadata band
- block out the HUD card
- block out the board hero zone
- block out the bottom breathing band
- block out the centered results overlay

### Step 3. HUD HiFi

On `02_HiFi`:

- build the HUD card first
- define the overline, title, goals, moves, pace, and footer hierarchy
- test the card in grayscale before committing full color
- then apply the approved material and accent direction

### Step 4. Board HiFi

- build the board shell second
- lock the board frame proportions before decoration
- add stage pill and status badge only after the board frame reads clearly
- confirm the board still dominates the entire screen

### Step 5. Results HiFi

- build the results card third
- create one win variant
- create one fail variant
- keep both variants on one shared structure

### Step 6. Components

On `03_Components`:

- extract reusable cards
- extract pills
- extract badges
- extract typographic styles
- extract stars / small icons if created

### Step 7. Export Prep

On `04_ExportPrep`:

- duplicate approved frames
- label export-ready versions clearly
- remove temporary notes and construction marks from export variants

## Review Checklist

Before a first-pass Figma review, verify:

1. the board is still the hero
2. the move count is instantly readable
3. goals are understandable at a glance
4. the result card feels premium and centered
5. top and bottom safe areas are respected
6. chrome supports readability instead of competing with it

## Required Outputs

The first Figma production pass should export:

1. one full-screen HUD + Board composition
2. one win results composition
3. one fail results composition
4. one component page snapshot

## Handoff Note

After the first pass is approved, export tasks should move into:

- asset slicing / export planning
- Unity import prep
- runtime replacement planning
