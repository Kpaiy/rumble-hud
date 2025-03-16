# RUMBLE HUD Mod

This mod adds a HUD to the RUMBLE display on your monitor, for use in streaming
RUMBLE or recording gameplay.

The mod will, for each player, display their:
  - Name
  - Battle Points
  - Current Health
  - Equipped Shift Stones
  - Player Portrait

## Demo Footage (Click to View)

[![RUMBLE HUD Demo](https://img.youtube.com/vi/MW8i_r3l8gQ/0.jpg)](https://www.youtube.com/watch?v=MW8i_r3l8gQ)

## Dependencies

This mod requires:
 - MelonLoader 0.7.0

## Installation Instructions

Once a release is available:
1. Extract the release files into your RUMBLE install directory.

## Known Issues

- Incompatible with LIV camera
- Portrait generation for the player only occurs when entering the gym
  - To regenerate your portrait after changing your avatar, you must leave
      and re-enter the gym (e.g. go to park and back)
- In parks, if you're unlucky, other player portraits can be photobombed
  - This is funny as hell, not fixing this

## Future Goals

- Implement support for Unity Rich Text (color tags in usernames)
- Investigate LIV support
- Integrate with RumbleModUI to expose configuration options:
  - Hide or show HUD
  - Re-generate player portraits
  - Rearrange ordering of players on the HUD.
- Consider support for NameBending
  - I am not familiar with this mod whatsoever.
- Score tracking

## Thanks

- **SDRAWKCABMIAY**: Provided shift stone sprites