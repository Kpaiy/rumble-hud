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

## Future Goals

- Code cleanup; everything is in one C# file at the moment.
- Integrate with RumbleModUI to expose configuration options:
  - Hide or show HUD
  - Re-generate player portraits
  - Rearrange ordering of players on the HUD.
- Investigate LIV support
- Score tracking

## Thanks

- **SDRAWKCABMIAY**: Provided shift stone sprites