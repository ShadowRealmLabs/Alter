# Alter trainer creator roadmap

## Next: stable mappings

1. Add pointer-chain discovery and resolution.
2. Add byte-pattern searches with an expected-match count and game-version checks.
3. Test saved mappings after a full game restart and show the verification state in the UI.

## Creator features

- More scan modes: unknown initial value; changed, unchanged, increased, and decreased values; additional integer sizes and strings.
- Hotkeys, categories, notes, slider bounds, and configurable toggle behavior.
- Undo and restore of changed values, process-exit cleanup, and save/restore of trainer settings.
- A local test workflow that records the process build, the address expression, the before/after value, and the result.

## Compatibility and sharing

- Import more Cheat Engine table records, including pointer entries. Keep embedded scripts inactive until there is an explicit trust and execution design.
- Document a versioned, open Alter profile schema and compatibility checks.
- Add optional community discovery using user-selected file sources. The core library stays offline and local.
- Investigate FLiNG compatibility separately. A compiled FLiNG EXE does not expose its internal memory mappings through its visible option names.
- Investigate Proton/Linux support after the Windows engine and profile format are stable.

No subscription or account is planned for core trainer features.
