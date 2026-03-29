# MatchJoy Scene Wiring Guide

> **Status**: Draft
> **Last Updated**: 2026-03-29
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
- refresh only the cells whose visible state changed after interaction
- show thin per-cell transition feedback driven by view-side diff refresh
- stagger some of that feedback so refills and board changes feel directional
- keep selection responsiveness immediate while allowing accepted-swap resolution to present more sequentially
- include a very short swap preview on the accepted pair before the resolved board update lands
- let non-swap changed cells briefly clear out before their new resolved contents appear
- let HUD / result closure land closer to the end of the board presentation instead of always updating immediately
- let the view signal when its current presentation pass is actually done, so session/UI settlement can follow that event
- keep the transition rules themselves in a separate presentation-planning layer instead of burying all of them inside `BoardView`
- temporarily gate new board interactions while an accepted-swap presentation pass is still settling
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

Optional debug recommendation:
- enable `Log Diff Refreshes` if you want to study how many cells the view updates after each interaction
- the current `BoardView`/`BoardCellView` bridge also uses those same diff updates to trigger lightweight cell animations
- initial population and refill feedback now use small view-side delays so the board reads less like a hard snap
- accepted swap resolution now uses a more sequential presentation mode than plain selection refreshes
- the accepted pair can now show a very short preview move before the resolved state is revealed
- other changed cells in the resolved pass can now play a short clear-like fade before the new state appears
- move/result UI can now settle slightly later so board presentation reads first
- current UI settlement now follows board-presentation completion events rather than only a guessed delay
- transition decision logic can now be maintained separately from `BoardView` lifecycle/orchestration concerns
- accepted-swap presentation now temporarily blocks new board input until settlement completes

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
8. If `Log Diff Refreshes` is enabled, observe how many cells were updated after each interaction
9. Watch Console logs:
   - rejected swap logs should appear for bad swaps
   - accepted swap logs should show cleared count
10. Confirm the board refreshes only changed cells instead of blindly repainting every cell
11. Confirm changed cells now show lightweight visual feedback:
   - selection changes pulse
   - accepted swap pairs give a tiny directional preview before the board settles into resolved presentation
   - other changed cells can briefly fade/shrink out before their new contents appear
   - changed tile contents briefly scale
   - newly refilled cells fade in and drop into place
   - board changes and initial fill feel slightly staggered instead of fully simultaneous
   - accepted swap resolution should feel more staged than simple click-selection refreshes
12. Confirm the moves label and result panel now feel more aligned with the end of the board presentation instead of updating instantly at swap-commit time
13. Confirm repeated interactions do not cause stale delayed HUD/result updates from a previous presentation pass
14. Confirm rapid extra taps/swipes during accepted-swap presentation do not trigger overlapping new swaps
15. Optionally use `PrototypeSessionDriver -> Run Test Swap` to compare the debug path

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
7. `BoardView` compares current cell display state against the last rendered state and only redraws changed cells
8. `BoardCellView` consumes that diff and applies lightweight per-cell transition feedback
9. A presentation-planning layer decides which transition each changed cell should use
10. `LevelSessionController` can choose whether a given refresh is presented immediately or as a thin resolved sequence
11. For accepted swaps, `LevelSessionController` can also provide the resolved pair so the view can preview that exchange before showing the committed post-resolution state
12. `BoardView` tracks when all animated cells in the current presentation pass have finished
13. `HudPresenter` and `ResultsPresenter` can now be settled after that completion signal so UI closure better matches visible resolution timing
14. `LevelSessionController` temporarily gates further board input during this settlement window so presentation passes do not overlap

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
- there is now a thin animation bridge with light staggering, a separate resolved-sequence mode, a tiny swap preview, and a lightweight clear fade, but not a full staged swap / clear / fall timeline
- board/UI timing is now event-aligned at a coarse presentation-pass level, but still not driven by explicit per-phase timeline objects
- presentation planning is cleaner than before, but still not yet a full reusable timeline asset or standalone sequencer
- input is protected during settlement, but there is not yet a distinct gameplay state for "presentation busy"
- goal tracking is still very early and only partly representative
- `ReleaseFrozenIngredient` is stubbed
- `ClearJelly` is still placeholder-like in runtime terms
- result presentation is very thin
- diff refresh reduces redraw churn, but there is still no fully sequenced swap / clear / refill presentation with explicit intermediate board states

This is okay. The point of this scene is to verify the architecture path, not to
look production-ready yet.

---

## 9. Presentation Debug Guide

Use this quick guide when the current board-presentation bridge behaves in an
unexpected way during Play mode.

### Symptom: Selection highlight feels delayed or unresponsive

Check these first:

- Confirm the board is not currently settling after an accepted swap. During
  settlement, new board interactions are intentionally gated.
- Confirm `EventSystem` exists in the scene and the `Image` on `CellTemplate`
  still has `Raycast Target` enabled.
- Confirm you are not mistaking the accepted-swap presentation lock for input
  failure.

Most relevant scripts:

- `MatchJoy.Core.LevelSessionController`
- `MatchJoy.UI.BoardCellView`

### Symptom: Accepted swap plays, but HUD or result UI updates too early or too late

Check these first:

- Enable `Log Presentation Settlement` on `LevelSessionController`.
- Watch for:
  - settlement start log
  - settlement complete log
- If the board clearly finishes but the UI does not update, inspect whether
  `BoardView` is reaching its presentation completion callback.

Most relevant scripts:

- `MatchJoy.Core.LevelSessionController`
- `MatchJoy.UI.BoardView`

### Symptom: Swap preview is missing on the accepted pair

Check these first:

- Confirm the swap was actually accepted rather than rejected.
- Confirm the accepted path is using `ResolvedSequence` presentation mode.
- Confirm only the accepted source/target pair is expected to preview; other
  changed cells should not use the same motion.

Most relevant scripts:

- `MatchJoy.Core.LevelSessionController`
- `MatchJoy.UI.BoardPresentationPlan`

### Symptom: Changed cells jump directly to the new result without a clear-like transition

Check these first:

- Confirm the affected cells were previously occupied.
- Remember that new refilled cells use the refill path, not the clear-fade path.
- Inspect the transition plan logic for `TileChanged` vs `Refilled`.

Most relevant scripts:

- `MatchJoy.UI.BoardPresentationPlan`
- `MatchJoy.UI.BoardCellView`

### Symptom: Refilled cells appear, but do not feel directional

Check these first:

- Inspect `_refillRowStepDelay`
- Inspect `_refillSpawnOffsetCells`
- Confirm the cells are truly entering through the `Refilled` transition, not
  just `TileChanged`.

Most relevant scripts:

- `MatchJoy.UI.BoardPresentationPlan`
- `MatchJoy.UI.BoardCellView`

### Symptom: Rapid extra taps/swipes still seem to cause overlap

Check these first:

- Enable `Log Presentation Settlement`.
- Confirm ignored-swap logs appear during settlement.
- If overlap still appears visually, inspect whether an old presentation pass is
  still allowed to complete after a newer render token has begun.

Most relevant scripts:

- `MatchJoy.Core.LevelSessionController`
- `MatchJoy.UI.BoardView`

### Symptom: Board updates visually, but one or more cells seem stuck in the wrong look

Check these first:

- Enable `Log Diff Refreshes` on `BoardView`.
- Confirm the cell is actually counted as changed.
- Inspect whether the previous and next render snapshots really differ on:
  - occupied state
  - tile id
  - selection state

Most relevant scripts:

- `MatchJoy.UI.BoardView`
- `MatchJoy.UI.BoardPresentationPlan`

### Recommended Debug Toggle Pair

When diagnosing this bridge, the most useful temporary combination is:

- `BoardView -> Log Diff Refreshes`
- `LevelSessionController -> Log Presentation Settlement`

That pairing lets you see:

- when the board decided something changed
- when the session considered the presentation pass "busy"
- when UI settlement was allowed to finish

---

## 10. Recommended Next Step After Wiring

Once this scene is wired and visibly works, the next best implementation step is:

- deepen the `board animation bridge`

That means:
- keeping the current input path intact
- preserving diff-based refresh ownership inside the view layer
- evolving from per-cell diff feedback toward staged swap / clear / refill presentation without changing gameplay ownership
- eventually separating "visual timeline steps" from "final committed board snapshot" more explicitly
