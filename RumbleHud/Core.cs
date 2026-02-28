using Il2CppInterop.Common;
using Il2CppPhoton.Pun;
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

[assembly: MelonInfo(typeof(RumbleHud.Core), "RumbleHud", "1.6.0", "Kpaiy", null)]
[assembly: MelonGame("Buckethead Entertainment", "RUMBLE")]

namespace RumbleHud
{
    public class Core : MelonMod
    {
        private PlayerManager playerManager = null;

        private List<PlayerInfo> playerInfos = null;

        private string settingsFilePath = @"UserData\RumbleHud.xml";

        private string currentScene = "Loader";
        private bool roundInProgress = false;


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
            currentScene = sceneName;

            if (sceneName != "Gym") return;

            // Clear all player panels, including the self player.
            Hud.ClearPlayerUi(true);

            Hud.ResetRoundTracking();
        }

        private bool IsHost(int playerCount, string playFabId, PlayerController playerController)
        {
            // Don't bother showing host when there's only one player.
            if (playerCount == 1) return false;

            // Use the conventional check.
            if (playerCount == 2)
            {
                bool selfIsHost = PhotonNetwork.IsMasterClient;
                bool playerIsSelf = playFabId == Hud.SelfPlayFabId;

                // Either we are checking if we are the host, or if
                // the opponent is not us and we are not the host.
                return (selfIsHost && playerIsSelf) || (!selfIsHost && !playerIsSelf);
            }

            // We're in a park, check the PlayerController's PhotonView for an ID of 1 (creator).
            // NOTE: This doesn't work for matches, because the ID is not reassigned on rematch.
            var photonView = playerController?.gameObject?.GetComponent<PhotonView>();

            // Safety.
            if (photonView == null) return false;

            return photonView.OwnerActorNr == 1;
        }

        private void CalculateRoundTracking(List<PlayerInfo> playerInfos)
        {
            // Iterate over players. Increment the round counter for the one with non-zero health.
            foreach (var playerInfo in playerInfos)
            {
                if (playerInfo.HP != 0)
                {
                    Hud.IncrementRound(playerInfo.PlayFabId);
                }
            }
        }

        public override void OnUpdate()
        {
            // Get the player manager if it isn't present.
            if (playerManager == null)
            {
                playerManager = GameObject.Find("Game Instance/Initializable/PlayerManager")?.GetComponent<PlayerManager>();

                // Try again next Update.
                if (playerManager == null) return;
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

            if (!Settings.Instance.LockControls)
            {
                if (Input.GetKeyDown(KeyCode.I))
                {
                    Hud.ToggleVisible();
                }

                // Cycle host display mode.
                if (Input.GetKeyDown(KeyCode.O))
                {
                    // TODO: Surely there's a better way.
                    switch (Settings.Instance.HostIndicator)
                    {
                        case HostIndicatorOptions.None:
                            Settings.Instance.HostIndicator = HostIndicatorOptions.Text;
                            break;
                        case HostIndicatorOptions.Text:
                            Settings.Instance.HostIndicator = HostIndicatorOptions.Icon;
                            break;
                        case HostIndicatorOptions.Icon:
                            Settings.Instance.HostIndicator = HostIndicatorOptions.Both;
                            break;
                        case HostIndicatorOptions.Both:
                            Settings.Instance.HostIndicator = HostIndicatorOptions.None;
                            break;
                    }
                }

                // Regenerate portraits.
                if (Input.GetKeyDown(KeyCode.P))
                {
                    Hud.RegeneratePortraits();
                }

                if (Input.GetKeyDown(KeyCode.Equals))
                {
                    Hud.SetScale(Settings.Instance.HudScale + 0.1f);
                }
                if (Input.GetKeyDown(KeyCode.Minus))
                {
                    Hud.SetScale(Settings.Instance.HudScale - 0.1f);
                }
            }

            Hud.SetRoundsVisible(
                playerManager?.AllPlayers?.Count == 2
                && (
                    currentScene == "Map0"
                    || currentScene == "Map1"
                ));

            // Update all player info.
            try
            {
                List<PlayerInfo> newPlayerInfos = new List<PlayerInfo>();

                bool allPlayersFullHp = true;
                bool anyPlayerDead = false;
                var playerEnumerator = playerManager?.AllPlayers?.GetEnumerator();
                if (playerEnumerator == null) {
                    return;
                }
                while (playerEnumerator.MoveNext())
                {
                    var current = playerEnumerator.Current;

                    int hp = current.Data.HealthPoints;
                    if (hp == 0)
                    {
                        anyPlayerDead = true;
                    }
                    if (hp != 20)
                    {
                        allPlayersFullHp = false;
                    }


                    string playFabId = current.Data.GeneralData.PlayFabMasterId;

                    // Record our own PlayFabId
                    if (playerManager.AllPlayers.Count == 1)
                    {
                        Hud.SelfPlayFabId = playFabId;
                    }

                    bool isHost = IsHost(playerManager.AllPlayers.Count, playFabId, current.Controller);

                    PlayerInfo currentPlayerInfo = new PlayerInfo
                    {
                        PlayFabId = playFabId,
                        Name = current.Data.GeneralData.PublicUsername,
                        BP = current.Data.GeneralData.BattlePoints,
                        HP = current.Data.HealthPoints,
                        ShiftStoneLeft = (ShiftStones)current.Data.EquipedShiftStones[0],
                        ShiftStoneRight = (ShiftStones)current.Data.EquipedShiftStones[1],
                        PlayerController = current.Controller,
                        IsHost = isHost,
                    };

                    newPlayerInfos.Add(currentPlayerInfo);
                }

                if (allPlayersFullHp)
                {
                    roundInProgress = true;
                    Hud.ResetRoundsIfWinnerPresent();
                }
                if (anyPlayerDead)
                {
                    if (roundInProgress)
                    {
                        CalculateRoundTracking(newPlayerInfos);
                    }

                    roundInProgress = false;
                }

                playerInfos = newPlayerInfos;
            } catch (Exception ex)
            {
                // MelonLogger.Error(ex);
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

                    if (Settings.Instance.HideSolo)
                    {
                        Hud.SetVisible(playerInfos.Count != 1);
                    }
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
                    {
                        // LoggerInstance.Error(ex);
                    }
                }
            } catch (Exception ex) {
                // LoggerInstance.Error(ex);
            }

            // base.OnUpdate();
        }
    }
}