# Rules for agents

1. No (or minimal) comments. A comment may only be added if absolutely neccessary
2. Treat `DESIGN.md` as the current domain specification. Read it before changing
   domain code or tests, and update it in the same change when a domain decision,
   invariant, or externally observable behaviour changes.
3. Work specification-first:
   1. Clarify the relevant rule, scenario, and acceptance criteria in `DESIGN.md`.
   2. Express the behaviour with a focused test in
      `TE.DomainStorytellingApplied.Domain.Tests`.
   3. Implement the smallest domain change that makes the test pass.
   4. Run the smallest relevant test command, then `dotnet test` before completing
      a domain change.
4. Keep the proof of behaviour close to the domain. Tests should describe a
   business scenario using `Given_When_Then` names and assert observable outcomes,
   not private implementation details.
5. Preserve domain boundaries. Domain objects belong in
   `TE.DomainStorytellingApplied.Domain`; test cases belong in
   `TE.DomainStorytellingApplied.Domain.Tests`. Do not introduce UI, database,
   HTTP, framework, or infrastructure concerns into the domain project.
6. Model invalid state out where practical. Validate constructor and method inputs,
   enforce the invariants defined in `DESIGN.md`, and fail explicitly with a
   meaningful exception when an operation cannot be completed.
7. Keep this POC small. Add types and abstractions only when they represent a
   named domain concept, enforce a rule, or make a specified scenario clearer.
8. Do not infer rules that the specification does not state. Record unresolved
   decisions in `DESIGN.md` as open questions before implementing policy-dependent
   behaviour.
