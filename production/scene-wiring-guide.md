# MatchJoy Scene Wiring Guide

> **Status**: Draft
> **Last Updated**: 2026-03-30
> **Purpose**: Help wire the current Sprint 1 prototype scripts into a working Unity scene step by step.

---

## Goal

At the end of this setup, you should have one Unity scene that can:

- create a level session from `LevelDefinition`
- build a runtime board
- render a visible grid with `BoardView`
- show remaining moves in `HudPresenter`
- show a simple result panel through `ResultsPresenter`
- present the runtime board as a readable product slice, with HUD context, board chrome, and a clearer result state
- let board chrome react not only to presentation stage but also to remaining-move pressure, so the center module better matches HUD urgency
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
- let each changed cell execute a lightweight internal presentation phase sequence instead of hardwiring all animation flow inside one branchy method
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

Optional additional UI children:
- `AccentBar` (`Image`)
- `OverlineLabel` (`Text`)
- `PanelBackground` (`Image`)
- `TitleLabel` (`Text`)
- `GoalLabel` (`Text`)
- `PaceLabel` (`Text`)
- `StateLabel` (`Text`)
- `FooterLabel` (`Text`)

Assign in `HudPresenter`:
- `Accent Bar` -> drag `AccentBar` if used
- `Overline Label` -> drag `OverlineLabel` if used
- `Panel Background` -> drag `PanelBackground` if used
- `Title Label` -> drag `TitleLabel` if used
- `Goal Label` -> drag `GoalLabel` if used
- `Moves Label` -> drag `MovesLabel`
- `Pace Label` -> drag `PaceLabel` if used
- `State Label` -> drag `StateLabel` if used
- `Footer Label` -> drag `FooterLabel` if used

### ResultsPanel Object

Create an empty child under Canvas:
- Name: `ResultsPanel`

Add component:
- `ResultsPresenter`

Create children:
- `HeadlineLabel`
- `DetailsLabel`

Optional additional children:
- `AccentBar` (`Image`)
- `OverlineLabel` (`Text`)
- `PanelBackground` (`Image`)
- `BadgeLabel` (`Text`)
- `SummaryLabel` (`Text`)
- `FooterLabel` (`Text`)

Assign in `ResultsPresenter`:
- `Root` -> drag `ResultsPanel`
- `Accent Bar` -> drag `AccentBar` if used
- `Overline Label` -> drag `OverlineLabel` if used
- `Panel Background` -> drag `PanelBackground` if used
- `Badge Label` -> drag `BadgeLabel` if used
- `Headline Label` -> drag `HeadlineLabel`
- `Summary Label` -> drag `SummaryLabel` if used
- `Details Label` -> drag `DetailsLabel`
- `Footer Label` -> drag `FooterLabel` if used

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
- `BoardCellView` execution is now organized as a lightweight sequence of presentation phases, which gives the bridge a cleaner path toward richer staged timelines
- accepted-swap presentation now temporarily blocks new board input until settlement completes
- `BoardView` can now optionally log presentation-pass start/completion in addition to diff counts
- lifecycle logging can now summarize how many cells used each transition type in the current pass
- lifecycle logging can now also summarize the lightweight cell presentation phases used in the current pass
- `BoardView` now remembers a pass-level summary for the latest presentation, so you can inspect recent bridge activity even after the Console has moved on
- `BoardView` now also keeps a short rolling history of recent presentation passes, so consecutive swaps can be compared without relying only on live Console output
- presentation passes now carry simple human-readable labels, so history/debug output is easier to match against concrete scene actions
- presentation summaries now also record approximate pass duration, so repeated tests can compare not only what happened but how long the bridge took to settle
- board render orchestration is now passed through a small render-request object instead of an ever-growing parameter list, which makes later presentation and UI expansion less invasive
- those render requests now also carry explicit presentation intent and board presentation stage, so summaries and future timeline logic do not need to infer meaning from label text alone
- `BoardView` now keeps its active presentation-pass runtime data in a dedicated internal context object, which reduces incidental state spread and makes later timeline expansion safer
- `BoardPresentationPlan` now produces a presentation instruction object instead of only a raw cell transition, and that instruction carries both intent and stage metadata for richer board-wide sequencing later
- each presentation pass now also builds a lightweight board-step plan (`Prepare -> ... -> Complete`) so pass history can describe board-level sequencing before a full sequencer exists
- board-step planning is now generated through a dedicated pass planner instead of being assembled ad hoc inside `BoardView`, which keeps the next sequencer step cleaner
- that pass planner now consumes an explicit planning context object, so later timeline hints can be added without stretching method signatures again
- that planning context now carries both transition counts and phase counts, which lets board-step planning distinguish interaction feedback from broader resolve work a little more cleanly
- it now also carries presentation mode and intent, so pass planning can react to request semantics directly instead of inferring everything from counts alone
- each board step now also carries a lightweight reason code, so pass history is starting to capture why a step exists, not just its coarse type
- board steps now also carry a lightweight completion mode (`Immediate` vs `AwaitAnimatedSettle`), which starts to model whether a future sequencer would advance instantly or wait for settle
- board steps now also carry an explicit sequence index, so pass history no longer relies only on list position to describe execution order
- board steps now also carry a lightweight expected-duration hint, so the pass model is beginning to express cadence as well as order
- the pass plan now also aggregates those step hints into an estimated total duration, so summary/history can compare planned cadence against actual settle time
- each presentation pass now also builds a lightweight board-step plan (`Prepare -> ... -> Complete`) so pass history can describe board-level sequencing before a full sequencer exists
- flow can now represent this bridge window explicitly as `Level Presentation Settling`

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
- `Board Panel Background` -> drag a surrounding `Image` if used
- `Board Accent` -> drag a thin accent `Image` if used
- `Board Overline Label` -> drag a top metadata `Text` if used
- `Board Stage Pill Label` -> drag a compact status `Text` if used
- `Board Title Label` -> drag a title `Text` if used
- `Board Subtitle Label` -> drag a subtitle `Text` if used
- `Board Status Badge Label` -> drag a right-side status `Text` if used
- `Board Footnote Label` -> drag a lower status `Text` if used
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
16. Optionally use additional `PrototypeSessionDriver` debug actions:
   - `Rebuild Session`
   - `Run Second Test Swap`
   - `Run Test Swap Then Immediate Second Swap`
   - `Log Presentation Bridge Summary`

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
10. `BoardCellView` turns that transition into a lightweight ordered phase sequence such as preview, wait, apply-state, and follow-up feedback
11. `LevelSessionController` can choose whether a given refresh is presented immediately or as a thin resolved sequence
12. For accepted swaps, `LevelSessionController` can also provide the resolved pair so the view can preview that exchange before showing the committed post-resolution state
13. `BoardView` tracks when all animated cells in the current presentation pass have finished
14. `HudPresenter` and `ResultsPresenter` can now be settled after that completion signal so UI closure better matches visible resolution timing
15. `LevelSessionController` temporarily gates further board input during this settlement window so presentation passes do not overlap
16. `GameFlowController` can now represent this post-commit bridge explicitly as `Level Presentation Settling`

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
- board/UI timing is now event-aligned at a coarse presentation-pass level, and cells now have a lightweight internal phase sequence, but the bridge is still not driven by a richer explicit board-wide timeline object
- pass summaries now include board-level step plans as scaffolding, but those steps are still descriptive planning data rather than a standalone runtime sequencer
- pass summaries now include board-level step plans as scaffolding, but those steps are still descriptive planning data rather than a standalone runtime sequencer
- presentation planning is cleaner than before, but still not yet a full reusable timeline asset or standalone sequencer
- input is protected during settlement and flow can now represent that bridge explicitly, but there is still not yet a richer per-phase timeline state model
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

If you need one level deeper of diagnosis, also enable:

- `BoardView -> Log Presentation Lifecycle`

That adds:

- presentation pass start
- whether the pass completed immediately or after animated cells settled
- a clearer separation between "diff detected" and "presentation finished"
- a quick summary of transition types used in that pass, such as `SelectionPulse`,
  `TileChanged`, or `Refilled`
- a quick summary of cell presentation phases used in that pass, such as
  `SwapPreview`, `ApplyVisualState`, or `RefillDrop`

### Useful PrototypeSessionDriver Actions

These are the most useful Inspector-side debug actions for the current bridge:

- `Rebuild Session`
  Resets the board and logs the rebuilt state.
- `Run Test Swap`
  Runs the primary configured swap and logs whether it was accepted.
- `Run Second Test Swap`
  Runs an alternate configured swap so you can compare behaviors quickly.
- `Run Test Swap Then Immediate Second Swap`
  Specifically stress-tests whether accepted-swap settlement is blocking
  overlapping follow-up input.
- `Log Presentation Bridge Summary`
  Prints current session readiness, settlement state, bridge timing settings,
  the latest presentation pass summary, and the current board snapshot in one place.
- `Log Last Presentation Pass`
  Prints only the most recently recorded presentation-pass summary, which is useful when you want to inspect the bridge without the full board/session dump.
- `Log Presentation History`
  Prints the recent rolling history of presentation-pass summaries, which is useful when comparing several swaps or checking whether settlement behavior changed across attempts.
  Recent entries now also include a simple label such as an initial build, selection refresh, or accepted-swap resolve.
- `Log Compact Presentation History`
  Prints the same recent history in a one-line-per-pass format, which is easier to scan during repeated validation loops.
  Each line now also includes the approximate duration of that pass.
- `Clear Presentation History`
  Clears the remembered pass history so a new validation run starts from a clean slate.
- `Log Last Pass Test Log Snippet`
  Prints the most recent pass as a Markdown-ready observation snippet, so it can be pasted into the presentation test log with less manual rewriting.
- `Run Presentation Validation Snapshot`
  Prints a compact validation bundle in one go: current presentation settings, compact recent history, the latest Markdown-ready log snippet, and the current board snapshot.
- `Run Validation Snapshot After Test Swap`
  Clears presentation history, runs the primary configured test swap, and then prints the bundled validation snapshot for that run.
- `Run Validation Snapshot After Second Test Swap`
  Clears presentation history, runs the secondary configured test swap, and then prints the bundled validation snapshot for that run.
- `Run Settlement Gate Validation Snapshot`
  Clears presentation history, runs the overlapping-input stress check, and then prints a focused validation bundle for the settlement-gating path.
- `Log Suggested Last Pass Test Log Snippet`
  Prints a Markdown-ready test-log starter with a best-effort suggested test ID based on the latest pass label.
- `Run Core Presentation Validation Suite`
  Runs a small high-value validation loop for the current bridge: one primary swap pass and one settlement-gate pass, with history/snippet output for each.

---

## 10. Recommended Next Step After Wiring

Once this scene is wired and visibly works, the next best implementation step is:

- deepen the `board animation bridge`

That means:
- keeping the current input path intact
- preserving diff-based refresh ownership inside the view layer
- evolving from per-cell diff feedback toward staged swap / clear / refill presentation without changing gameplay ownership
- eventually separating "visual timeline steps" from "final committed board snapshot" more explicitly

For live validation, use:

- `production/presentation-bridge-test-log.md`
