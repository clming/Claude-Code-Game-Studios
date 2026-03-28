# Technical Preferences

<!-- Populated by /setup-engine. Updated as the user makes decisions throughout development. -->
<!-- All agents reference this file for project-specific standards and conventions. -->

## Engine & Language

- **Engine**: Unity 6.3 LTS
- **Language**: C#
- **Rendering**: URP
- **Physics**: Unity built-in physics (GameObject workflow)

## Naming Conventions

- **Classes**: PascalCase (e.g., `PlayerController`)
- **Variables**: Public fields/properties use PascalCase; private fields use `_camelCase`
- **Signals/Events**: PascalCase for C# events and callback-style methods (e.g., `HealthChanged`, `OnLevelCompleted`)
- **Files**: PascalCase matching primary class name (e.g., `BoardController.cs`)
- **Scenes/Prefabs**: PascalCase (e.g., `MainMenu`, `LevelBoard`, `CandyPiece`)
- **Constants**: PascalCase or UPPER_SNAKE_CASE for true compile-time constants

## Performance Budgets

- **Target Framerate**: [TO BE CONFIGURED - suggested default: 60 FPS]
- **Frame Budget**: [TO BE CONFIGURED - suggested default: 16.6 ms]
- **Draw Calls**: [TO BE CONFIGURED - suggested default: keep gameplay scenes lean and UI batching-friendly]
- **Memory Ceiling**: [TO BE CONFIGURED - suggested default: define after target device tier is chosen]

## Testing

- **Framework**: Unity Test Framework + NUnit
- **Minimum Coverage**: [TO BE CONFIGURED]
- **Required Tests**: Balance formulas, gameplay systems, networking (if applicable)

## Forbidden Patterns

<!-- Add patterns that should never appear in this project's codebase -->
- [None configured yet - add as architectural decisions are made]

## Allowed Libraries / Addons

<!-- Add approved third-party dependencies here -->
- Addressables
- [No additional libraries approved yet - add as dependencies are approved]

## Architecture Decisions Log

<!-- Quick reference linking to full ADRs in docs/architecture/ -->
- [No ADRs yet - use /architecture-decision to create one]
