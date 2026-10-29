# FLiNG EXE import (v0.2.0)

Use **Library → Add trainer** and select a FLiNG `.exe`. Alter scans the file locally for readable embedded option names and hotkeys. When found, it creates an editable profile automatically. The extractor was checked against Cyberpunk 2077 Plus 46 and S.T.A.L.K.E.R. 2 Plus 32. Packed or differently formatted EXEs may expose no readable list; they receive an empty profile ready for manual entry. Alter does not run the EXE during import.

Open the imported trainer to edit its name, version, option labels and categories; add, hide, or remove options; change the EXE path; and export the profile as JSON. To record a hotkey, open an option, click **Change key**, and press a key or combination. Press **Esc** to cancel capture.

**Launch trainer** starts the selected EXE only when clicked. **Send key** sends the saved hotkey only while the selected EXE is running. Alter does not extract or rewrite the FLiNG trainer's memory code, cannot read whether its cheats are active, and cannot change the EXE's own hotkeys. A changed hotkey in Alter must match the hotkey configured in the original trainer. Exported profiles contain a local EXE path; recipients need their own copy of the EXE and may need **Change EXE** after importing.

The imported EXE's cheat effects still need to be validated in the intended game.
