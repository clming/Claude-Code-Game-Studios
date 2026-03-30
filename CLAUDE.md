# Claude Code Game Studios -- Game Studio Agent Architecture

Indie game development managed through 48 coordinated Claude Code subagents.
Each agent owns a specific domain, enforcing separation of concerns and quality.

## Technology Stack

- **Engine**: Unity 6.3 LTS
- **Language**: C#
- **Version Control**: Git with trunk-based development
- **Build System**: Unity Build Pipeline
- **Asset Pipeline**: Unity Asset Import Pipeline + Addressables

> **Note**: Engine-specialist agents exist for Godot, Unity, and Unreal with
> dedicated sub-specialists. Use the set matching your engine.

## Project Structure

@.claude/docs/directory-structure.md

## Engine Version Reference

@docs/engine-reference/unity/VERSION.md

## Technical Preferences

@.claude/docs/technical-preferences.md

## Coordination Rules

@.claude/docs/coordination-rules.md

## Collaboration Protocol

**User-driven collaboration, not autonomous execution.**
Every task follows: **Question -> Options -> Decision -> Draft -> Approval**

- Agents MUST ask "May I write this to [filepath]?" before using Write/Edit tools
- Agents MUST show drafts or summaries before requesting approval
- Multi-file changes require explicit approval for the full changeset
- No commits without user instruction

See `docs/COLLABORATIVE-DESIGN-PRINCIPLE.md` for full protocol and examples.

> **First session?** If the project has no engine configured and no game concept,
> run `/start` to begin the guided onboarding flow.

## UI Workflow Priority

### Hard Rule

For game projects, `UI art design` must come before `UI code implementation`.

Required order:

1. Core gameplay and code foundation stabilize.
2. UI art design begins and a first-round visual direction is approved.
3. Component design, art specs, and motion direction are defined.
4. Only then should UI code implementation and asset integration proceed.

Default anti-patterns to avoid:

- Expanding placeholder procedural UI into de facto final UI.
- Treating plain-color debug panels and text-only layouts as final direction.
- Continuing large-scale HUD/Board/Results coding after the visual phase should have started.

### Allowed Use of Procedural Placeholder UI

Procedural placeholder UI is only acceptable to:

- validate information hierarchy,
- validate interaction timing,
- validate gameplay-to-presentation flow.

Once those are stable, the workflow must switch to:

- UI art design,
- visual direction approval,
- asset specification,
- then UI engineering and integration.

### Default Agent Behavior

When the project reaches the UI phase, agents should default to:

1. producing the UI art workflow,
2. producing a visual direction document,
3. documenting tools, references, and deliverables,
4. then moving into Figma/design production,
5. and only after that implementing UI in Unity.

If the user has not explicitly asked to keep writing UI code, agents should not skip the UI art phase.

## UI Design Entry Docs

@design/ui/ui-art-design-workflow.md

@design/ui/matchjoy-ui-art-direction-v1.md

## Coding Standards

@.claude/docs/coding-standards.md

## Context Management

@.claude/docs/context-management.md
