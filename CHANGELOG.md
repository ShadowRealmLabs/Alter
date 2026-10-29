# Changelog

## v0.2.0 · 2026-10-29

- Redesigned the Community Hub with larger icon tabs, custom dark filters, deeper purple accents, and clearer empty states.
- Added a GitHub-backed trainer catalog with local caching. Community profile imports verify SHA-256 checksums; imported scripts remain untrusted until reviewed.
- Added Community News cards for GitHub Discussions. The included GitHub Actions workflow can publish discussion titles and excerpts into the public feed; GitHub Releases provide fallback news.
- Added **Submit Trainer** links to the GitHub pull request flow and kept discussions available for community conversation.
- Added an update checker in Settings. It compares the installed version with published GitHub Releases, shows when a newer release exists, and links to that release page.
- Refined the Home and Community banners and kept the app fully local for trainer creation and library use.

### Current limits

The public community catalog starts empty until reviewed entries are merged. Discussion cards require the included workflow to be committed and run on GitHub. The update checker links to releases; it does not install updates. No game-specific trainer is bundled or verified.

## v0.1.0-alpha · 2026-10-29

- Introduced the local trainer library, import/export, artwork, and recently used trainers.
- Added exact-value scans, direct memory values, and user-authored C# trainer features.
- Added partial Cheat Engine table import and external FLiNG option import.
- Added local resource extensions, community links, gaming headlines, UI settings, and tray controls.
