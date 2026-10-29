# Alter custom trainer features (v0.2.0)

Create a local trainer in **Library → Create your own**, enter the game's process name, and connect while the game is running. **Add value** is for direct `Int32`, `Float`, and `Double` reads, writes, and freezes. **Custom logic** opens the in-app C# editor for features that need game-specific logic.

Custom features have two behaviors:

- **Toggle:** `On` runs when enabled, `Tick` runs about every 300 ms while enabled, and `Off` runs when disabled or Alter exits.
- **Action:** `On` runs once when **Apply** is clicked, followed by `Off` to clean up temporary patches.

The editor supplies `api` (a `TrainerApi`) and `value` (a `double` set in the editor). For example, an author who has found the game's health address could use this in **While On**:

```csharp
api.WriteFloat("Game.exe+0x1234", (float)value);
```

For a reversible byte patch, use **On**:

```csharp
api.Patch("Game.exe+0x5678", "90 90");
```

Alter saves the original bytes when `Patch` is called and restores them when the toggle turns off. Authors must confirm the correct address and replacement bytes for each game build.

Useful API methods include `Resolve`, `Pointer`, `FindPattern`, `ReadBytes`, `WriteBytes`, `ReadInt32`, `ReadFloat`, `ReadDouble`, their `Write` variants, `Patch`, `SavePosition`, and `LoadPosition`. `SavePosition`/`LoadPosition` use three float addresses and a named slot shared across custom features in the current Alter session.

Custom features are saved in JSON profiles. Imported C# code stays off until the user explicitly runs it, with a trust prompt. Code has the current Windows user's permissions. Only run profiles from authors you trust. FLiNG EXE import remains a separate external-trainer mode; it does not turn the EXE into an Alter custom feature.

This creator provides general building blocks; it does not discover game-specific cheat logic. Multiplier hooks, inventory grants, and other advanced effects need author-written code and live testing for each game/version. Global in-game hotkeys and a full Cheat Engine script interpreter are not included in v0.2.0.
