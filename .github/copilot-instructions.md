# UltimateRunningPlanner Copilot Instructions

Scope
- Use these instructions when performing code reviews, suggestions, or automated refactors.

Primary goals
- Enforce strong OOP and design principles.
- Prioritize clean, maintainable, testable C# code that fits C# 13 / .NET 9 idioms.

Rules and focus areas
- SOLID first:
  - Single Responsibility: prefer small, focused types and methods.
  - Open/Closed: prefer extensibility via interfaces or protected extension points.
  - Liskov: ensure derived behavior remains substitutable.
  - Interface Segregation: prefer many narrow interfaces to large monoliths.
  - Dependency Inversion: depend on abstractions and use DI-friendly patterns.

- Clean code and readability:
  - Use descriptive, intention-revealing names for types, members, and parameters.
  - Keep methods short and single-purpose; extract helpers with meaningful names.
  - Avoid long parameter lists; suggest parameter objects or builder patterns when needed.
  - Prefer immutability / readonly fields for state where appropriate.
  - Avoid public mutable fields; prefer properties with deliberate accessor visibility.
  - Use nullable reference types and explicit null checks for public APIs.

- Async & concurrency:
  - Use async/await when I/O-bound; name async methods with the "Async" suffix.
  - Include CancellationToken on cancellable async APIs.
  - Avoid shared mutable static state and race conditions; recommend thread-safe constructs.

- Dependency & abstraction guidance:
  - Prefer interfaces and abstraction for I/O, time, and external dependencies to facilitate testing.
  - Prefer composition over inheritance; avoid deep inheritance hierarchies.

Operational notes
- Do not suggest features incompatible with project C#/.NET targets.
- Do not change target framework.
