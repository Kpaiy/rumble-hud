# Version 1.1.1

- Fix a bug with host text outline specification
  - As innocuous as this sounds, this was causing hella performance issues when
    loading hud elements while the HUD is toggled off. It would error out and
    retry while leaving vestiges of the first attempt, causing several
    duplicates to **TANK** performance.
- Increase logging (not too much, don't worry).

# Version 1.1.0

- Add host indicator to parks and matches.
  - It is hidden when there is only one player in a scene.
  - Press `O` on your keyboard to switch between the display modes for host indicator:
    - None
    - Text
    - Icon
    - Both

# Version 1.0.4

- Switch Text elements from `Text` type to `TextMeshProUGUI`.
  - This means that your coloured usernames now show in the HUD!

# Version 1.0.3

- Forget to rebuild the mod so no actual progress. >.<

# Version 1.0.2

Make text shrink to fit (within reason) to accommodate long usernames.

# Version 1.0.1

- Make BP text overflow instead of word wrap.

# Version 1.0.0

Initial release. Compared to testing version:
  - Implemented config file.
  - HUD scale saves to config file on exit.
  - Slightly increased BP text size.