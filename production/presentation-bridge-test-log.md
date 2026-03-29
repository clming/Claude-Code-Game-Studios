# MatchJoy Presentation Bridge Test Log

> **Status**: Active Working Note
> **Last Updated**: 2026-03-29
> **Purpose**: Capture structured observations while validating the current board presentation bridge in Unity.

---

## How To Use

Run the current prototype scene in Unity and record findings here while testing:

- accepted swap presentation
- rejected swap behavior
- selection responsiveness
- refill readability
- HUD/result timing
- settlement input gating

Use one short entry per observed issue or confirmation. Keep each note tied to a
specific scene action so we can reproduce it quickly.

---

## Recommended Debug Toggles

For the most useful console trace during testing, enable:

- `BoardView -> Log Diff Refreshes`
- `BoardView -> Log Presentation Lifecycle`
- `LevelSessionController -> Log Presentation Settlement`

Optional:

- use `PrototypeSessionDriver` context menu actions for deterministic checks
- use `Log Presentation Bridge Summary` if Console scroll makes it hard to catch the most recent pass details live
- use `Log Last Presentation Pass` if you only need the latest step/stage/transition/phase summary without dumping the rest of the session state
- use `Log Presentation History` if you are comparing multiple attempts and want to see whether later passes differ from earlier ones
- use `Log Compact Presentation History` if you want a one-line scan of several recent passes
- use `Clear Presentation History` before a fresh validation loop so the next history dump only contains the current run
- use the pass labels in those summaries to match history entries back to concrete actions such as initial build, selection refresh, rejected swap refresh, or accepted swap resolve
- use the recorded pass duration to compare whether one validation run is settling noticeably slower or faster than another
- use `Log Last Pass Test Log Snippet` when you want a Markdown starter block that can be pasted directly into the observation section below
- use `Run Presentation Validation Snapshot` when you want one bundled dump that captures the current bridge configuration, recent history, a test-log starter, and the current board
- use `Run Validation Snapshot After Test Swap` when you want a fresh one-swap validation bundle based on the primary configured swap
- use `Run Validation Snapshot After Second Test Swap` when you want the same kind of fresh validation bundle for the secondary configured swap
- use `Run Settlement Gate Validation Snapshot` when you want a focused bundle for the overlapping-input / PB-08 path
- use `Log Suggested Last Pass Test Log Snippet` when you want the latest pass converted into a Markdown starter with a best-effort matching test ID

---

## Test Matrix

| ID | Scenario | Expected Result | Status | Notes |
|----|----------|-----------------|--------|-------|
| PB-01 | Click-to-select only | Selection highlight appears immediately | Untested | |
| PB-02 | Rejected swap | No resolved sequence; board remains readable | Untested | |
| PB-03 | Accepted swap | Swap preview appears before resolved sequence | Untested | |
| PB-04 | Non-swap changed cells | Clear-like fade occurs before new contents appear | Untested | |
| PB-05 | Refilled cells | New cells feel directional and readable | Untested | |
| PB-06 | HUD timing | Moves label updates near presentation end | Untested | |
| PB-07 | Result timing | Victory/failure panel appears after presentation settles | Untested | |
| PB-08 | Settlement gating | Extra taps/swipes during presentation are ignored | Untested | |
| PB-09 | Rapid repeated testing | No stale presentation completion or stale UI settlement | Untested | |
| PB-10 | Session rebuild | Rebuilt board starts in a clean visual state | Untested | |

Status suggestions:

- `Pass`
- `Fail`
- `Partial`
- `Untested`

---

## Observation Template

Copy this block for each meaningful finding:

```md
### PB-XX - Short Title

- Scene action:
- Expected:
- Actual:
- Repro frequency:
- Console evidence:
- Most likely layer:
- Next fix idea:
```

Most likely layer suggestions:

- `Input / selection`
- `LevelSessionController`
- `BoardView`
- `BoardPresentationPlan`
- `BoardCellView`
- `HUD / Results`

---

## Current Findings

### Placeholder

- Scene action: Not recorded yet.
- Expected: Fill this in after first live validation pass.
- Actual: Fill this in after first live validation pass.
- Repro frequency: Unknown.
- Console evidence: None yet.
- Most likely layer: Unknown.
- Next fix idea: Start with PB-03, PB-05, PB-06, and PB-08 because they cover the current bridge most directly.

---

## Fast Repro Checklist

Use this order if you want one quick validation loop:

1. Press Play and confirm the board appears cleanly.
2. Click one cell and confirm selection is immediate.
3. Try one clearly bad swap and confirm rejection readability.
4. Try one accepted swap and watch for:
   - swap preview
   - clear fade
   - refill drop
   - HUD timing
5. During accepted presentation, click rapidly and confirm no extra swap starts.
6. If possible, force a win/loss edge and confirm result timing follows board settlement.

---

## Notes For Follow-Up

- If most failures point to transition choice, inspect `BoardPresentationPlan`.
- If timing feels wrong but transition choice is right, inspect `BoardCellView`.
- If board timing is right but HUD/results feel off, inspect `LevelSessionController`.
- If logs imply correct timing but observed behavior still feels messy, the next step is likely a dedicated UI/polish pass rather than more bridge architecture.
