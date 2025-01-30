### Assets Setup
For standardized development, place game assets in `D:/GameDev/Assets/2DD2/`.
This is the recommended path to ensure consistent asset references across the team.

The folder structure should be:
- D:/GameDev/Assets/2DD2/
  - Sprites/
  - Sounds/
  - ...

#### Notes:
- A Python script is provided `Scripts\fix_tiled_paths.py` to help convert relative paths to absolute paths
- Run `python .\Scripts\fix_tiled_paths.py .` after adding new assets
- Before commit ensure all asset references in .tsx files starts with `D:/GameDev/Assets/2DD2/Sprites/...`