# Alter community catalog

Alter reads `community/catalog.json` locally and checks the same file at `https://raw.githubusercontent.com/an1dxsh/Alter/main/community/catalog.json`. The local catalog works offline. When the online catalog is available, Alter saves a local cache. The bundled catalog starts empty; the concept image's trainer cards and rankings are illustrative and are not shipped as fake entries.

To publish a trainer, fork Alter, add a catalog entry for a profile in your own GitHub repository, and open a pull request. A maintainer reviews the metadata and checksum before merging it. [GitHub Discussions](https://github.com/an1dxsh/Alter/discussions) remains the place for questions, feedback, and announcements.

Example entry:

```json
{
  "trainers": [
    {
      "id": "example-game-trainer",
      "name": "Example Game Trainer",
      "game": "Example Game",
      "creator": "CreatorName",
      "description": "Local single-player trainer profile",
      "version": "1.0.0",
      "updatedAt": "2026-10-29T00:00:00Z",
      "type": "Trainer",
      "repository": "https://github.com/CreatorName/Example-Game-Trainer",
      "profileUrl": "https://raw.githubusercontent.com/CreatorName/Example-Game-Trainer/main/ExampleGame.json",
      "sha256": "64 lowercase hexadecimal characters",
      "featured": false
    }
  ]
}
```

For a downloadable entry, `profileUrl` must be a GitHub HTTPS JSON profile and `sha256` must match that exact file. Alter verifies the hash before importing. Imported scripts start untrusted and disabled. Entries without a downloadable profile open their source repository instead.

The Community News panel reads `community/discussions.json`. The included GitHub Actions workflow uses a repository token to read public Discussions and refresh that file on a schedule or when discussions change. GitHub Releases supply fallback news cards until the workflow is installed and runs. The desktop app never asks users for a GitHub token. Trending, follows, uploads, reactions, and download counts need additional shared data; the interface does not invent them.

To activate live discussion cards, commit `.github/workflows/community-news.yml` and `community/discussions.json` to the `main` branch, then run **Refresh community news** once from the repository's Actions tab. The repository's Actions settings must permit the workflow's `contents: write` and `discussions: read` permissions. If those permissions are unavailable, Community News continues to show release notes.
