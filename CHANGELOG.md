# Changelog

## 0.2.0

- The hide/show weapons key now takes your weapons back out while swimming. Previously it could put them away but not bring them back.
- If a game update moves one of the swimming checks the mod removes, a warning is written to the BepInEx log instead of the mod silently doing nothing.
- The README now explains what the mod allows, and that stamina still works as usual.
- Added a CI check that the version matches in every file.
- Moved `package.sh` into `.github/scripts` and made it the same for every mod: the name, version and DLL now come from `manifest.json` and the `.csproj`.

## 0.1.0

- Equipment can now be used while in water.
