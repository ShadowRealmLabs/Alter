# Alter v0.2.0

Alter is a free, open-source, local Windows trainer hub. This release focuses on the Community Hub and makes it easier to discover, create, and share trainer profiles.

## What's new

- A redesigned Community Hub with a deeper purple look, icon navigation, search, filters, and a cleaner empty state.
- GitHub-backed catalog support with a local offline cache. Published trainer profiles can be imported after a SHA-256 checksum check; scripts remain untrusted until reviewed.
- Community News cards for GitHub Discussions, with GitHub Releases as a fallback. The included workflow generates the public discussion feed after it is added to the repository.
- A **Submit Trainer** path through GitHub pull requests.
- A Settings update checker that compares this version against GitHub Releases and links to a newer release.

## Before you start

Extract the Windows ZIP into a writable folder and run `Alter.exe`. The Library starts empty; create a trainer or import a supported profile. Internet access is optional for local trainer creation and use. Community discovery, news, and update checks require it.

The community catalog starts empty until reviewed submissions are merged. This release does not include a game-specific cheat, automatic updater, full Cheat Engine interpreter, or native FLiNG code conversion.

For full details, see `README.md`, `CHANGELOG.md`, and `COMMUNITY_CATALOG.md`.
