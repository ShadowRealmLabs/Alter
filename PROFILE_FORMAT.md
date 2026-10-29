# Alter profile format (v0.2.0)

A trainer is a UTF-8 JSON file. Import it through **Library → Add trainer**. Alter saves its own copy in the local `profiles` directory. Export a trainer from its page to share it.

Alter lets you choose a banner and cover from **Customize artwork** on any trainer or Library card. PNG, JPEG and BMP images under 3 MB are embedded as base64 in `bannerImage` and `coverImage` fields, so exported profiles carry the artwork with them.

```json
{
  "id": "example-local-id",
  "name": "Example Game",
  "process": "ExampleGame.exe",
  "version": "1",
  "cheats": [
    {
      "name": "Health",
      "category": "Player",
      "kind": "value",
      "address": "ExampleGame.exe+0x1234",
      "dataType": "Int32",
      "value": "100",
      "freeze": false
    }
  ]
}
```

Supported `dataType` values for value features: `Int32`, `Float`, `Double`. An `address` may be a decimal absolute address, a hexadecimal absolute address with `0x`, or an exact module filename plus hexadecimal/decimal offset. Imported profile IDs are replaced with a new local ID. v0.2.0 also accepts `script` features with `scriptMode` (`toggle` or `action`), `scriptOn`, `scriptOff`, `scriptTick`, and numeric `value` fields. Imported scripts are marked untrusted and never run automatically; see [CREATOR.md](CREATOR.md).

Addresses, types, and values must be tested against the intended game version. A raw absolute address often changes after a restart. The current format has no game build signature or automatic compatibility check.
