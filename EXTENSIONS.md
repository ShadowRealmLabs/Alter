# Local resource extensions

Open **Extensions → Create extension** to add a named HTTPS resource link. The extension is stored as JSON in the local `extensions` folder. You can import a JSON extension, edit it, open its link, or remove it from the page.

This first extension type is a resource card. It does not execute code or add trainer cheats. Trainer profiles are created in Library.

Example:

```json
{"name":"Community guide","description":"An example resource","author":"Alter community","url":"https://example.com/guide"}
```

Community updates are read from `community-updates.txt`; the Community changelog is read from `CHANGELOG.md`. Edit those local files to update the page in a future build.
