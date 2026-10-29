# Alter v0.2.0

Alter is a local Windows trainer hub and early trainer creator. There is no account or subscription. Trainers, extensions and settings are stored in the folder where you extract Alter.

## Run

Extract the complete Windows ZIP into a folder you can write to, then run `Alter.exe`. Keep `assets/` beside it. The Library starts empty. Create a trainer in **Library → Create your own**, or import an Alter JSON profile, a supported Cheat Engine `.CT` table, or a FLiNG trainer EXE with **Add trainer**.

## Available now

- Create, import, export, edit and remove local trainer profiles.
- Attach to a running Windows process by executable name. Scan exact `Int32`, `Float` and `Double` values, then read, write or freeze a saved address or `Module.exe+0xOffset`.
- Create a game-specific feature in the C# editor using the local `TrainerApi` for memory reads, writes, pattern searches, reversible patches and position slots. Imported scripts require a trust prompt before execution. See [CREATOR.md](CREATOR.md).
- Import simple direct-address numeric entries from Cheat Engine tables. Pointer entries and embedded table scripts are skipped.
- Import readable option names and hotkeys from some FLiNG EXEs into an editable external trainer page. The original EXE is still required to run those cheats. See [FLING_IMPORT.md](FLING_IMPORT.md).
- Customize trainer banner and cover art, view recently used trainers, and add local HTTPS resource extensions.
- Choose UI accent, density, startup page, news visibility, confirmations and tray behavior in Settings. Settings checks GitHub Releases against v0.2.0 and links to a newer release when available.
- Browse the Community Hub catalog, release notes and discussion updates when online. Local trainer profiles remain usable offline. See [COMMUNITY_CATALOG.md](COMMUNITY_CATALOG.md).

## Current limits

No game-specific cheat is bundled or verified. A raw scan result can move after a game restart, so authors must test stable mappings against each game build. First Scan checks at most 1 GiB of readable memory, retains at most 500,000 matches and displays the first 100. There is no pointer scanner, full Cheat Engine interpreter, native FLiNG code conversion, Proton support or automatic game-version compatibility check. External trainer status cannot be read from the original EXE.

Imported profiles can write to another process. Read their addresses and code before enabling features, and use Alter with games you are permitted to modify.

## Build and contribute

On Windows with .NET Framework 4.x and WPF assemblies, run `./build.ps1` in PowerShell. Source is included in the source archive. Profile details are in [PROFILE_FORMAT.md](PROFILE_FORMAT.md), extension details in [EXTENSIONS.md](EXTENSIONS.md), and planned work in [ROADMAP.md](ROADMAP.md).

The C# source and documentation use the MIT license in [LICENSE.txt](LICENSE.txt). The supplied Alter brand images in `assets/` have separate ownership. Third-party game artwork is omitted from the public packages; Home and Community show built-in banner treatments. See [ASSETS.md](ASSETS.md).
