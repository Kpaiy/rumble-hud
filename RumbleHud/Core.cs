using Il2CppInterop.Common;
using Il2CppPhoton.Realtime;
using Il2CppRUMBLE.Combat.ShiftStones;
using Il2CppRUMBLE.Managers;
using Il2CppRUMBLE.Players;
using Il2CppRUMBLE.Players.Subsystems;
using Il2CppSystem.Net.NetworkInformation;
using Il2CppSystem.Security.Cryptography;
using Il2CppTMPro;
using MelonLoader;
using System.ComponentModel;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[assembly: MelonInfo(typeof(RumbleHud.Core), "RumbleHud", "0.1.0", "Kpaiy", null)]
[assembly: MelonGame("Buckethead Entertainment", "RUMBLE")]

namespace RumbleHud
{
    public class Core : MelonMod
    {
        private PlayerManager playerManager = null;

        private List<PlayerInfo> playerInfos = null;

        private string settingsFilePath = @"UserData\RumbleHud.json";


        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("RumbleHud: Initialized.");

            try
            {
                Settings.FromXmlFile(settingsFilePath);
                LoggerInstance.Msg("RumbleHud: Loaded settings from file.");
            } catch
            {
                Settings.Initialize();
                LoggerInstance.Msg("RumbleHud: Unable to load settings. Using defaults.");
            }
        }

        public override void OnApplicationQuit()
        {
            // Save settings.
            Settings.Instance.ToXmlFile(settingsFilePath);

            base.OnApplicationQuit();
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (sceneName != "Gym") return;

            // Clear all player panels, including the self player.
            // This is a workaround to allow the player to regenerate their
            // own profile picture by re-entering the gym after modifying
            // their character.
            Hud.ClearPlayerUi(true);

            // Load the preview character from the dressing room.
            // We can use them to get the player's head, since the player
            // is headless.
            Hud.LoadPreviewCharacter();
        }

        public override void OnUpdate()
        {
            // Get the player manager if it isn't present.
            if (playerManager == null)
            {
                playerManager = GameObject.Find("Game Instance/Initializable/PlayerManager")?.GetComponent<PlayerManager>();
            }

            // Load the resources if not loaded.
            if (!Resources.Initialized)
            {
                Resources.LoadResources();
            }

            if (!Hud.Initialized)
            {
                Hud.Initialize();
            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                Hud.ToggleVisible();
            }

            if (Input.GetKeyDown(KeyCode.Equals))
            {
                Hud.SetScale(Settings.Instance.HudScale + 0.1f);
            }
            if (Input.GetKeyDown(KeyCode.Minus))
            {
                Hud.SetScale(Settings.Instance.HudScale - 0.1f);
            }

            // Update all player info.
            try
            {
                List<PlayerInfo> newPlayerInfos = new List<PlayerInfo>();

                var playerEnumerator = playerManager.AllPlayers.GetEnumerator();
                while (playerEnumerator.MoveNext())
                {
                    var current = playerEnumerator.Current;

                    string playfabId = current.Data.GeneralData.PlayFabMasterId;

                    // Strip Unity format indicators from the username
                    // TODO: Use TextMeshPro instead so they can be supported.
                    string strippedUsername = Regex.Replace(
                        current.Data.GeneralData.PublicUsername,
                        @"<[^>]*>", "");

                    if (playerManager.AllPlayers.Count == 1)
                    {
                        Hud.SelfPlayFabId = playfabId;
                    }

                    PlayerInfo currentPlayerInfo = new PlayerInfo
                    {
                        PlayFabId = playfabId,
                        Name = strippedUsername,
                        BP = current.Data.GeneralData.BattlePoints,
                        HP = current.Data.HealthPoints,
                        ShiftStoneLeft = (ShiftStones)current.Data.EquipedShiftStones[0],
                        ShiftStoneRight = (ShiftStones)current.Data.EquipedShiftStones[1],
                        PlayerController = current.Controller,
                    };

                    newPlayerInfos.Add(currentPlayerInfo);
                }

                playerInfos = newPlayerInfos;
            } catch (Exception ex)
            {
            }

            // Make new canvases if required, update if existing, for each player.
            try
            {
                // If the amount of player UIs is not equal to the amount of player infos,
                // nuke and rebuild.
                // TODO: Actually be smart about this.
                if (playerInfos.Count != Hud.PlayerUiElementsCount)
                {
                    Hud.ClearPlayerUi();
                }

                foreach (var playerInfo in playerInfos)
                {
                    if (!Hud.PlayerHudExists(playerInfo.PlayFabId))
                    {
                        Hud.CreatePlayerUi(playerInfo, Hud.PlayerUiElementsCount);

                        LoggerInstance.Msg($"RumbleHud: Created Element for player {playerInfo.Name}.");
                        continue;
                    }

                    try
                    {
                        Hud.UpdatePlayerUi(playerInfo);
                    } catch (Exception ex)
                    { }
                }
            } catch { }

            // base.OnUpdate();
        }
    }
}