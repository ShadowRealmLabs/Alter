# Contributing to Alter

Alter v0.2.0 is an early release. Please keep changes local-first and avoid accounts, subscriptions, telemetry, or silently executed code in imported profiles.

1. Build with `./build.ps1` on Windows.
2. Test memory changes against a disposable local test process before testing any game.
3. Keep profile imports data-only. Do not execute Cheat Engine Lua or Auto Assembler scripts from an imported table.
4. Document game version and restart testing for any proposed trainer profile. A raw scan address alone is not a stable mapping.
5. Do not commit game binaries, third-party game artwork, personal saves, or local `settings.json` and `profiles/` data.

The current profile fields are described in [PROFILE_FORMAT.md](PROFILE_FORMAT.md). Open an issue with the game version and a reproducible case before proposing a game-specific fix.
