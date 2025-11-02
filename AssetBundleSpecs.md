# Asset Bundle Specs

This mod stores its resources in an asset bundle, using a method outlined in the
Rumble modding Discord.

If you wish to create your own asset bundle for the mod, these are the assets my
mod is expecting to find.

| Name           | Type   | Description                            |
|----------------|--------|----------------------------------------|
| `TMP_GoodDogPlain` | `TMP_FontAsset` | The font used for player names and BP. |
| `HealthPip`    | `Texture2D` | This represents 1 unit of health. Expected resolution: 17x10. This texture includes the transparent area around the health pip. |
| `PlayerBackground` | `Texture2D` | This is the background for the player HUD. Expected resolution: 550x100. Changing the resolution could have undesirable effects on the positioning of left vs right aligned elements of the HUD. |
| `HostIcon` | `Texture2D` | Expected size: 50x50. This icon shows next to the player who is hosting the session. |
| `RoundWon` | `Texture2D` | Sprite to denote a won round, i.e. the green circle. Keep this as a square image. |
| `RoundWon` | `Texture2D` | Sprite to denote a not-won round, i.e. the empty circle. Keep this as a square image. |
| `ss_adamant` | `Texture2D` | Adamant shift stone icon. |
| `ss_charge` | `Texture2D` | Charge shift stone icon. |
| `ss_empty` | `Texture2D` | Icon to use when no shift stone equipped. |
| `ss_flow` | `Texture2D` | Flow shift stone icon. |
| `ss_guard` | `Texture2D` | Guard shift stone icon. |
| `ss_stubborn` | `Texture2D` | Stubborn shift stone icon. |
| `ss_surge` | `Texture2D` | Surge shift stone icon. |
| `ss_vigor` | `Texture2D` | Vigor shift stone icon. |
| `ss_volatile` | `Texture2D` | Volatile shift stone icon. |

**Note:** For all `Texture2D` assets, make sure the "Read/Write" property is checked. I'm fairly sure this is very important for me to access them in code.

All shift stone textures are expected to be roughly square in resolution.