# IMPLEMENTATION RULES (Codex Runbook)

## Execution order
- Follow PLAN.md in order.
- Do exactly one milestone per PR.

## Diff discipline
- Keep diffs small and focused.
- Do not do unrelated refactors, formatting, renames, or cleanup.

## Validation gate (required)
After completing a milestone:
- Run project validation (play mode check + any tests).
- Fix failures before moving on.

## Plan updates (required)
After each milestone:
- Update PLAN.md progress (checkbox + short note in Progress log).
