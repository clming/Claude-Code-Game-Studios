# MatchJoy Scene Wiring Guide

> **Status**: Draft
> **Last Updated**: 2026-03-28
> **Purpose**: Help wire the current Sprint 1 prototype scripts into a working Unity scene step by step.

---

## Goal

At the end of this setup, you should have one Unity scene that can:

- create a level session from `LevelDefinition`
- build a runtime board
- render a visible grid with `BoardView`
- show remaining moves in `HudPresenter`
- show a simple result panel through `ResultsPresenter`
- let you click a first cell, click an adjacent second cell, and attempt a real swap
- let you drag or swipe from one cell toward a neighbor to attempt a direct swap
- optionally still run a debug swap from `PrototypeSessionDriver`

This is not the final scene architecture. It is the thinnest useful scene wiring
for learning the framework and validating the prototype spine.

---

## Scripts Used In This Wiring Pass

You will wire these existing scripts:

- `MatchJoy.Core.GameBootstrap`
- `MatchJoy.Flow.GameFlowController`
- `MatchJoy.Core.LevelSessionController`
- `MatchJoy.Input.BoardInputController`
- `MatchJoy.UI.BoardView`
- `MatchJoy.UI.BoardCellView`
- `MatchJoy.UI.HudPresenter`
- `MatchJoy.UI.ResultsPresenter`
- `MatchJoy.Debugging.PrototypeSessionDriver`
- `MatchJoy.Authoring.LevelDefinition` (ScriptableObject asset)

---

## Recommended Scene Objects

Create these scene objects in a new test scene.

### Root Objects

1. `Bootstrap`
2. `Canvas`
3. `DebugRoot`
4. `EventSystem`

`EventSystem` is required for cell click and swipe handling because `BoardCellView`
uses Unity UI pointer events.

---

## 1. Bootstrap Object

Create an empty GameObject:

- Name: `Bootstrap`

Add components:

- `GameFlowController`
- `GameBootstrap`
- `LevelSessionController`
- `BoardInputController`
- `PrototypeSessionDriver`

### References to Assign

#### `GameBootstrap`
- `Game Flow Controller` -> drag the `GameFlowController` component on `Bootstrap`

#### `BoardInputController`
- `Game Flow Controller` -> drag the `GameFlowController` component on `Bootstrap`

#### `LevelSessionController`
- `Game Flow Controller` -> drag the `GameFlowController` component on `Bootstrap`
- `Level Definition` -> drag your `LevelDefinition` asset
- `Hud Presenter` -> drag the `HudPresenter` component from `Canvas/HUD`
- `Results Presenter` -> drag the `ResultsPresenter` component from `Canvas/ResultsPanel`
- `Board View` -> drag the `BoardView` component from `Canvas/BoardLayer/BoardView`
- `Board Input Controller` -> drag the `BoardInputController` component on `Bootstrap`

#### `PrototypeSessionDriver`
- `Level Session Controller` -> drag the `LevelSessionController` component on `Bootstrap`
- leave test swap coordinates at default first

---

## 2. Canvas Setup

Create a Canvas if you do not already have one.

Suggested hierarchy:

```text
Canvas
  HUD
    MovesLabel
  BoardLayer
    BoardView
      CellRoot
      CellTemplate
        TileLabel
  ResultsPanel
    HeadlineLabel
    DetailsLabel
```

Use Unity's standard screen-space canvas for now.

### HUD Object

Create an empty child under Canvas:
- Name: `HUD`

Add component:
- `HudPresenter`

Create child UI Text:
- Name: `MovesLabel`

Assign in `HudPresenter`:
- `Moves Label` -> drag `MovesLabel`

### ResultsPanel Object

Create an empty child under Canvas:
- Name: `ResultsPanel`

Add component:
- `ResultsPresenter`

Create children:
- `HeadlineLabel`
- `DetailsLabel`

Assign in `ResultsPresenter`:
- `Root` -> drag `ResultsPanel`
- `Headline Label` -> drag `HeadlineLabel`
- `Details Label` -> drag `DetailsLabel`

Suggested initial state:
- disable `ResultsPanel` in hierarchy so it starts hidden

---

## 3. Board Layer Setup

The current `BoardView` uses `RectTransform`, `Image`, and `Text`, so keep it
inside the Canvas for this prototype pass.

### BoardLayer

Create an empty child under `Canvas`:
- Name: `BoardLayer`

Optionally add a `RectTransform` sized large enough for a 6x6 grid.

### BoardView Object

Create an empty GameObject under `BoardLayer`:
- Name: `BoardView`

Add component:
- `BoardView`

Give it a `RectTransform`.

### CellRoot

Create child object under `BoardView`:
- Name: `CellRoot`

Add component:
- `RectTransform`

This acts as the runtime parent for spawned cells.

### CellTemplate

Create child object under `BoardView`:
- Name: `CellTemplate`

Add components:
- `RectTransform`
- `Image`
- `BoardCellView`

Recommended extra component on `CellTemplate`:
- `CanvasRenderer` (usually added automatically with `Image`)

Create a child text object under `CellTemplate`:
- Name: `TileLabel`
- Add component: `Text`

Assign in `BoardCellView`:
- `Background` -> drag the `Image` on `CellTemplate`
- `Tile Label` -> drag `TileLabel`

Assign in `BoardView`:
- `Cell Root` -> drag `CellRoot`
- `Cell Template` -> drag `CellTemplate`

Important:
- keep `CellTemplate` active while wiring
- runtime code will hide it after first render
- make sure `CellTemplate` has a visible size so clones render clearly
- make sure the `Image` on `CellTemplate` has `Raycast Target` enabled so clicks and drags register

---

## 4. EventSystem

If your scene does not already have one, create:

- `GameObject -> UI -> Event System`

Without it, `BoardCellView` pointer handlers will not fire.

---

## 5. LevelDefinition Asset

Create one test asset:

- Right click in `MatchJoyProj/Assets/MatchJoy/ScriptableObjects/Levels`
- `Create -> MatchJoy -> Level Definition`

Suggested first test values:

- `Level Id`: `level_001`
- `Chapter Index`: `1`
- `Level Order`: `1`
- `Board Width`: `6`
- `Board Height`: `6`
- `Move Limit`: `12`

### Initial Tiles

Important note:
The current `LevelDefinition` expects `_initialTiles` as a flat int array.
So for now, in the Inspector, set array length to `board width * board height`.
For a `6 x 6` board, set length to `36`.

Use simple repeated tile ids like:

```text
0,1,2,3,4,0,
1,2,3,4,0,1,
2,3,4,0,1,2,
3,4,0,1,2,3,
4,0,1,2,3,4,
0,1,2,3,4,0
```

For goals, add one simple goal first:
- `Goal Type`: `CollectTile`
- `Target Count`: `3`
- `Target Tile Id`: `0`

This is better than `ClearJelly` for the first visual/debug pass, because the
current Sprint 1 runtime still uses clear-count as a placeholder for jelly logic.

---

## 6. First Validation Pass

After wiring, test in this order:

1. Press Play
2. Confirm the board appears
3. Confirm the moves label shows the move limit
4. Click one playable cell and confirm it highlights
5. Click an adjacent cell and watch the swap attempt resolve
6. Press Play again and drag from one cell toward a neighboring cell
7. Confirm the swipe path also attempts a swap
8. Watch Console logs:
   - rejected swap logs should appear for bad swaps
   - accepted swap logs should show cleared count
9. Confirm the board re-renders after the click-driven or swipe-driven swap
10. Optionally use `PrototypeSessionDriver -> Run Test Swap` to compare the debug path

---

## 7. What The Current Interaction Means

The prototype interaction loop is now:

1. `BoardCellView` receives a UI click or swipe
2. `BoardView` forwards either:
   - a clicked `BoardCoordinate`, or
   - a swipe request with `source -> target`
3. `LevelSessionController` decides whether this is:
   - a first selection
   - a second selection that can form a swap
   - a reselection
   - or a direct swipe swap attempt
4. `BoardInputController` owns the lightweight selection state for the click path
5. `SwapResolutionService` validates the swap
6. `CascadeResolver` clears/refills if accepted
7. `HudPresenter` and `ResultsPresenter` update after the state change

That flow is a good example of how this framework separates:
- scene input
- session orchestration
- gameplay rules
- presentation

---

## 8. Known Temporary Limitations

These are expected at this stage:

- swipe interaction is threshold-based, not polished gesture input yet
- click-to-select is still the safest fallback path
- there is no animation layer yet
- goal tracking is still very early and only partly representative
- `ReleaseFrozenIngredient` is stubbed
- `ClearJelly` is still placeholder-like in runtime terms
- result presentation is very thin
- the board is re-rendered whole after each swap instead of animating diffs

This is okay. The point of this scene is to verify the architecture path, not to
look production-ready yet.

---

## 9. Recommended Next Step After Wiring

Once this scene is wired and visibly works, the next best implementation step is:

- build the `board animation + diff refresh bridge`

That means:
- keeping the current input path intact
- replacing full board redraw behavior with targeted visual updates
- introducing simple swap / clear / refill presentation without changing gameplay ownership
