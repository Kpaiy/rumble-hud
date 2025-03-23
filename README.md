# RUMBLE HUD Mod

This mod adds a HUD to the RUMBLE display on your monitor, for use in streaming
RUMBLE or recording gameplay.

The mod will, for each player, display their:
  - Name
  - Battle Points
  - Current Health
  - Equipped Shift Stones
  - Player Portrait

## Controls

The mod currently uses the following controls:
  - `I` to toggle HUD on/off
  - `-`/`=` (near the backspace key) to decrease/increase HUD scale

The HUD scale currently saves to a settings file: `UserData/RumbleHud.xml`.
**Note:** Settings save on game quit, so RUMBLE crashing might mean your 
settings don't save.

## Demo Footage (Click to View)

[![RUMBLE HUD Demo](https://img.youtube.com/vi/MW8i_r3l8gQ/0.jpg)](https://www.youtube.com/watch?v=MW8i_r3l8gQ)

## Dependencies

This mod requires:
 - MelonLoader 0.7.0

## Installation Instructions

1. Extract the `Mods` and `UserData` folders into your RUMBLE install directory.

## Known Issues

- Incompatible with LIV camera
- Portrait generation for the player only occurs when entering the gym
  - To regenerate your portrait after changing your avatar, you must leave
      and re-enter the gym (e.g. go to park and back)
- Opponents can have their portrait taken while they're in awfully unflattering poses, e.g. full scorpion.
  - This is funny as hell, not fixing this
- In parks, if you're unlucky, other player portraits can be photobombed
  - This is also funny as hell, not fixing this

## Future Goals

- Implement host/client indicator
- Investigate LIV support
- Integrate with RumbleModUI to expose configuration options:
  - Hide or show HUD
  - Re-generate player portraits
  - Rearrange ordering of players on the HUD.
- Consider support for NameBending
- Score tracking
- Make shift stone icons pulse/shimmer when triggered/active
- Is it possible to use OSC or host a server? Something that could expose this information outside of RUMBLE, so someone familiar with OBS could make their own elements in OBS instead using this info?
  - This could be a workaround for supporting LIV for serious streaming setups.

## Support

Reach out to me on Discord (kpaiy) if you have issues or feature requests.
I can't guarantee fast response times, but I'll do what I can. ^-^

## Thanks

- **SDRAWKCABMIAY** for providing shift stone graphics.
- Testers:
  - **Jman**
  - **Rhymenocerous**
  - **Savitarian**