﻿using Il2CppInterop.Common;
using Il2CppRUMBLE.Combat.ShiftStones;
using Il2CppRUMBLE.Managers;
using Il2CppRUMBLE.Players.Subsystems;
using Il2CppSystem.Net.NetworkInformation;
using Il2CppSystem.Security.Cryptography;
using Il2CppTMPro;
using MelonLoader;
using System.ComponentModel;
using System.Reflection.Metadata;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[assembly: MelonInfo(typeof(RumbleHud.Core), "RumbleHud", "0.1.0", "Kpaiy", null)]
[assembly: MelonGame("Buckethead Entertainment", "RUMBLE")]

namespace RumbleHud
{
    // First element is left hand, second element is right hand.
    enum ShiftStones
    {
        Empty = -1,
        Adamant = 0,
        Charge = 1,
        Flow = 2,
        Guard = 3,
        Stubborn = 4,
        Surge = 5,
        Vigor = 6,
        Volatile = 7
    }

    class PlayerInfo
    {
        public string PlayFabId { get; set; }
        public string Name { get; set; }
        public int BP { get; set; }
        public int HP { get; set; }
        public ShiftStones ShiftStoneLeft { get; set; }
        public ShiftStones ShiftStoneRight { get; set; }
    }

    class PlayerUiElements
    {
        public GameObject Container { get; set; }
        public RawImage Background { get; set; }
        public Text Name { get; set; }
        public Text BP { get; set; }
        public Image HealthBar { get; set; }
        public RawImage HealthPips { get; set; }
        public RawImage ShiftStoneLeft { get; set; }
        public RawImage ShiftStoneRight { get; set; }
    }

    public class Core : MelonMod
    {
        private Il2CppAssetBundle bundle;

        private PlayerManager playerManager = null;

        private List<PlayerInfo> playerInfos = null;
        private Dictionary<string, PlayerUiElements> uiElementsByPlayer = new Dictionary<string, PlayerUiElements>();

        private Font font = null;
        private Texture2D backgroundTexture = null;
        private Texture2D healthPipsTexture = null;

        private Dictionary<ShiftStones, string> shiftStoneResourceNames = new Dictionary<ShiftStones, string>()
        {
            {ShiftStones.Empty, "ss_empty"},
            {ShiftStones.Adamant, "ss_adamant"},
            {ShiftStones.Charge, "ss_charge"},
            {ShiftStones.Flow, "ss_flow"},
            {ShiftStones.Guard, "ss_guard"},
            {ShiftStones.Stubborn, "ss_stubborn"},
            {ShiftStones.Surge, "ss_surge"},
            {ShiftStones.Vigor, "ss_vigor"},
            {ShiftStones.Volatile, "ss_volatile"}
        };
        private Dictionary<ShiftStones, Texture2D> shiftStoneTextures = new Dictionary<ShiftStones, Texture2D>();

        private GameObject uiContainer = null;
        private Canvas canvas = null;

        private readonly Color healthLow = new Color(151f / 255, 74f / 255, 69f / 255);
        private readonly Color healthMedium = new Color(139f / 255, 132f / 255, 66f / 255);
        private readonly Color healthHigh = new Color(96f / 255, 142f / 255, 83f / 255);
        private readonly Color healthFull = new Color(124f / 255, 150f / 255, 171f / 255);

        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("RumbleHud: Initialized.");
        }

        private void LoadShiftStoneTexture(Il2CppAssetBundle bundle, ShiftStones shiftStone)
        {
            var resourceName = shiftStoneResourceNames[shiftStone];
            var shiftStoneTexture = GameObject.Instantiate(bundle.LoadAsset<Texture2D>(resourceName));
            shiftStoneTextures[shiftStone] = shiftStoneTexture;
            GameObject.DontDestroyOnLoad(shiftStoneTexture);
            LoggerInstance.Msg($"RumbleHud: Loaded texture for {shiftStone}.");
        }

        private void LoadResources()
        {
            bundle = Il2CppAssetBundleManager.LoadFromFile(@"UserData/rumblehud");
            // GameObject myGameObject = GameObject.Instantiate(bundle.LoadAsset<GameObject>("Object name goes here!"));
            font = GameObject.Instantiate(bundle.LoadAsset<Font>("GoodDogPlain"));
            backgroundTexture = GameObject.Instantiate(bundle.LoadAsset<Texture2D>("PlayerBackground"));
            healthPipsTexture = GameObject.Instantiate(bundle.LoadAsset<Texture2D>("HealthPip"));

            uiContainer = new GameObject();
            uiContainer.name = "Canvas";
            uiContainer.AddComponent<Canvas>();

            canvas = uiContainer.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var canvasScaler = uiContainer.AddComponent<CanvasScaler>();

            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1008);

            uiContainer.AddComponent<GraphicRaycaster>();

            // TODO: Do this iteratively, you fool.
            LoadShiftStoneTexture(bundle, ShiftStones.Empty);
            LoadShiftStoneTexture(bundle, ShiftStones.Adamant);
            LoadShiftStoneTexture(bundle, ShiftStones.Charge);
            LoadShiftStoneTexture(bundle, ShiftStones.Flow);
            LoadShiftStoneTexture(bundle, ShiftStones.Guard);
            LoadShiftStoneTexture(bundle, ShiftStones.Stubborn);
            LoadShiftStoneTexture(bundle, ShiftStones.Surge);
            LoadShiftStoneTexture(bundle, ShiftStones.Vigor);
            LoadShiftStoneTexture(bundle, ShiftStones.Volatile);

            GameObject.DontDestroyOnLoad(font);
            GameObject.DontDestroyOnLoad(backgroundTexture);
            GameObject.DontDestroyOnLoad(healthPipsTexture);
            GameObject.DontDestroyOnLoad(uiContainer);
            GameObject.DontDestroyOnLoad(canvas);
        }

        public override void OnUpdate()
        {
            // Get the player manager if it isn't present.
            if (playerManager == null)
            {
                playerManager = GameObject.Find("Game Instance/Initializable/PlayerManager")?.GetComponent<PlayerManager>();
            }

            // Load the resources if not loaded.
            if (font == null)
            {
                LoadResources();
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

                    PlayerInfo currentPlayerInfo = new PlayerInfo
                    {
                        PlayFabId = playfabId,
                        Name = current.Data.GeneralData.PublicUsername,
                        BP = current.Data.GeneralData.BattlePoints,
                        HP = current.Data.HealthPoints,
                        ShiftStoneLeft = (ShiftStones)current.Data.EquipedShiftStones[0],
                        ShiftStoneRight = (ShiftStones)current.Data.EquipedShiftStones[1],
                    };

                    newPlayerInfos.Add(currentPlayerInfo);
                }

                playerInfos = newPlayerInfos;
            } catch (Exception ex)
            {
                // LoggerInstance.Msg(ex.ToString());
            }

            // Make new canvases if required, update if existing, for each player.
            try
            {
                // If the amount of player UIs is not equal to the amount of player infos,
                // nuke and rebuild.
                // TODO: Actually be smart about this.
                if (playerInfos.Count != uiElementsByPlayer.Count)
                {
                    ClearPlayerUi();
                }

                foreach (var playerInfo in playerInfos)
                {
                    if (!uiElementsByPlayer.ContainsKey(playerInfo.PlayFabId))
                    {
                        CreatePlayerUi(playerInfo, uiElementsByPlayer.Keys.Count);
                        LoggerInstance.Msg($"RumbleHud: Created Element for player {playerInfo.Name}.");
                        continue;
                    }

                    try
                    {
                        UpdatePlayerUi(playerInfo);
                    } catch (Exception ex)
                    {
                        LoggerInstance.Error(ex.ToString());
                    }
                }
            } catch { }

            // base.OnUpdate();
        }

        private void CreatePlayerUi(PlayerInfo playerInfo, int position)
        {
            bool isRightAligned = position % 2 == 1;
            int offset = (position / 2) * 150 * -1;

            // BACKGROUND

            GameObject backgroundObject = new GameObject();
            backgroundObject.transform.parent = uiContainer.transform;
            backgroundObject.name = "background";

            RawImage rawImage = backgroundObject.AddComponent<RawImage>();
            rawImage.texture = backgroundTexture;
            rawImage.SetNativeSize();

            var rawImageTransform = rawImage.GetComponent<RectTransform>();
            if (isRightAligned)
            {
                // Anchor to top right.
                rawImageTransform.anchorMin = new Vector2(1, 1);
                rawImageTransform.anchorMax = new Vector2(1, 1);
                rawImageTransform.pivot = new Vector2(1, 1);
                rawImageTransform.anchoredPosition = new Vector3(0, -50 + offset, 0);

                // Flip texture.
                rawImage.uvRect = new Rect(0, 0, -1, 1);
            }
            else
            {
                // Anchor to top left.
                rawImageTransform.anchorMin = new Vector2(0, 1);
                rawImageTransform.anchorMax = new Vector2(0, 1);
                rawImageTransform.pivot = new Vector2(0, 1);
                rawImageTransform.anchoredPosition = new Vector3(0, -50 + offset, 0);
            }

            // NAME

            GameObject nameObject = new GameObject();
            nameObject.transform.parent = backgroundObject.transform;
            Text nameText = nameObject.AddComponent<Text>();

            nameText.font = font;
            nameText.text = playerInfo.Name;
            nameText.fontSize = 42;
            nameText.horizontalOverflow = HorizontalWrapMode.Overflow;

            var nameTextTransform = nameText.GetComponent<RectTransform>();
            nameTextTransform.sizeDelta = new Vector2(300, 50);
            nameText.resizeTextForBestFit = true;
            nameText.resizeTextMaxSize = 42;
            if (isRightAligned)
            {
                // Anchor top right.
                nameTextTransform.anchorMin = new Vector2(1, 1);
                nameTextTransform.anchorMax = new Vector2(1, 1);
                nameTextTransform.pivot = new Vector2(1, 1);

                nameTextTransform.anchoredPosition = new Vector3(-125, -10);
                nameText.alignment = TextAnchor.UpperRight;
            }
            else
            {
                // Anchor to top left.
                nameTextTransform.anchorMin = new Vector2(0, 1);
                nameTextTransform.anchorMax = new Vector2(0, 1);
                nameTextTransform.pivot = new Vector2(0, 1);

                nameTextTransform.anchoredPosition = new Vector3(125, -10);
            }

            // BP

            GameObject bpObject = new GameObject();
            bpObject.transform.parent = backgroundObject.transform;
            Text bpText = bpObject.AddComponent<Text>();

            bpText.color = new Color(251f / 255, 1, 143f / 255);
            bpText.font = font;
            bpText.text = $"{playerInfo.BP} BP";
            bpText.fontSize = 28;

            var bpTextTransform = bpText.GetComponent<RectTransform>();

            if (isRightAligned)
            {
                // Anchor to top left.
                bpTextTransform.anchorMin = new Vector2(0, 1);
                bpTextTransform.anchorMax = new Vector2(0, 1);
                bpTextTransform.pivot = new Vector2(0, 1);

                bpTextTransform.anchoredPosition = new Vector3(55, -15);
                bpText.alignment = TextAnchor.UpperRight;
            }
            else
            {
                // Anchor to top right.
                bpTextTransform.anchorMin = new Vector2(1, 1);
                bpTextTransform.anchorMax = new Vector2(1, 1);
                bpTextTransform.pivot = new Vector2(1, 1);

                bpTextTransform.anchoredPosition = new Vector3(-55, -15);
            }

            // HEALTH BAR

            GameObject healthBarObject = new GameObject();
            healthBarObject.transform.parent = backgroundObject.transform;
            Image healthBar = healthBarObject.AddComponent<Image>();
            healthBar.color = healthFull;

            var healthBarTransform = healthBar.GetComponent<RectTransform>();
            healthBarTransform.sizeDelta = new Vector2(340, 10);
            if (isRightAligned)
            {
                // Anchor to bottom left.
                healthBarTransform.anchorMin = new Vector2(0, 0);
                healthBarTransform.anchorMax = new Vector2(0, 0);
                healthBarTransform.pivot = new Vector2(0, 0);

                healthBarTransform.anchoredPosition = new Vector2(100, 20);
            } else
            {
                // Anchor to bottom right.
                healthBarTransform.anchorMin = new Vector2(1, 0);
                healthBarTransform.anchorMax = new Vector2(1, 0);
                healthBarTransform.pivot = new Vector2(1, 0);

                healthBarTransform.anchoredPosition = new Vector2(-100, 20);
            }

            //  HEALTH PIPS
            
            GameObject healthPipsObject = new GameObject();
            healthPipsObject.transform.parent = healthBarObject.transform;
            RawImage healthPips = healthPipsObject.AddComponent<RawImage>();

            healthPips.texture = healthPipsTexture;

            healthPips.uvRect = new Rect(0, 0, 20, 1);

            var healthPipsTransform = healthPips.GetComponent<RectTransform>();
            healthPipsTransform.sizeDelta = new Vector2(healthPipsTexture.width * 20, healthPipsTexture.height);

            if (isRightAligned)
            {
                // Anchor to middle right.
                healthPipsTransform.anchorMin = new Vector2(1, 0.5f);
                healthPipsTransform.anchorMax = new Vector2(1, 0.5f);
                healthPipsTransform.pivot = new Vector2(1, 0.5f);

                healthPipsTransform.anchoredPosition = new Vector2(0, 0);
            } else
            {
                // Anchor to middle left.
                healthPipsTransform.anchorMin = new Vector2(0, 0.5f);
                healthPipsTransform.anchorMax = new Vector2(0, 0.5f);
                healthPipsTransform.pivot = new Vector2(0, 0.5f);

                healthPipsTransform.anchoredPosition = new Vector2(0, 0);
            }

            // LEFT SHIFT STONE

            GameObject leftShiftStoneObject = new GameObject();
            leftShiftStoneObject.transform.parent = backgroundObject.transform;
            RawImage leftShiftStone = leftShiftStoneObject.AddComponent<RawImage>();

            leftShiftStone.texture = shiftStoneTextures[playerInfo.ShiftStoneLeft];

            var leftShiftStoneTransform = leftShiftStoneObject.GetComponent<RectTransform>();

            leftShiftStoneTransform.sizeDelta = new Vector2(35, 35);

            if (isRightAligned)
            {
                // Anchor to bottom right.
                leftShiftStoneTransform.anchorMin = new Vector2(1, 0);
                leftShiftStoneTransform.anchorMax = new Vector2(1, 0);
                leftShiftStoneTransform.pivot = new Vector2(1, 0);

                leftShiftStoneTransform.anchoredPosition = new Vector2(-55, 10);
            } else
            {
                // Anchor to bottom left.
                leftShiftStoneTransform.anchorMin = new Vector2(0, 0);
                leftShiftStoneTransform.anchorMax = new Vector2(0, 0);
                leftShiftStoneTransform.pivot = new Vector2(0, 0);

                leftShiftStoneTransform.anchoredPosition = new Vector2(10, 10);
            }

            // RIGHT SHIFT STONE

            GameObject rightShiftStoneObject = new GameObject();
            rightShiftStoneObject.transform.parent = backgroundObject.transform;
            RawImage rightShiftStone = rightShiftStoneObject.AddComponent<RawImage>();

            rightShiftStone.texture = shiftStoneTextures[playerInfo.ShiftStoneRight];

            var rightShiftStoneTransform = rightShiftStoneObject.GetComponent<RectTransform>();

            rightShiftStoneTransform.sizeDelta = new Vector2(35, 35);

            if (isRightAligned)
            {
                // Anchor to bottom right.
                rightShiftStoneTransform.anchorMin = new Vector2(1, 0);
                rightShiftStoneTransform.anchorMax = new Vector2(1, 0);
                rightShiftStoneTransform.pivot = new Vector2(1, 0);

                rightShiftStoneTransform.anchoredPosition = new Vector2(-10, 10);
            }
            else
            {
                // Anchor to bottom left.
                rightShiftStoneTransform.anchorMin = new Vector2(0, 0);
                rightShiftStoneTransform.anchorMax = new Vector2(0, 0);
                rightShiftStoneTransform.pivot = new Vector2(0, 0);

                rightShiftStoneTransform.anchoredPosition = new Vector2(55, 10);
            }


            uiElementsByPlayer[playerInfo.PlayFabId] = new PlayerUiElements
            {
                Container = backgroundObject,
                Background = rawImage,
                Name = nameText,
                BP = bpText,
                HealthBar = healthBar,
                HealthPips = healthPips,
                ShiftStoneLeft = leftShiftStone,
                ShiftStoneRight = rightShiftStone,
            };
        }

        private void UpdatePlayerUi(PlayerInfo playerInfo)
        {
            var playerUiElements = uiElementsByPlayer[playerInfo.PlayFabId];

            if (playerUiElements == null) return;

            playerUiElements.BP.text = $"{playerInfo.BP} BP";

            Color healthBarColor = healthFull;
            
            if (playerInfo.HP < 20)
            {
                healthBarColor = healthHigh;
            }
            if (playerInfo.HP <= 10)
            {
                healthBarColor = healthMedium;
            }
            if (playerInfo.HP <= 5)
            {
                healthBarColor = healthLow;
            }
            
            playerUiElements.HealthBar.color = healthBarColor;

            var healthPipsTransform = playerUiElements.HealthPips.GetComponent<RectTransform>();
            playerUiElements.HealthPips.uvRect = new Rect(0, 0, playerInfo.HP, 1);

            healthPipsTransform.sizeDelta = new Vector2(healthPipsTexture.width * playerInfo.HP, healthPipsTexture.height);

            if (shiftStoneTextures[playerInfo.ShiftStoneLeft] == null)
            {
                // LoggerInstance.Error("Null shift stone texture"); return;
                LoadShiftStoneTexture(bundle, playerInfo.ShiftStoneLeft);
            }
            if (shiftStoneTextures[playerInfo.ShiftStoneRight] == null)
            {
                LoadShiftStoneTexture(bundle, playerInfo.ShiftStoneRight);
            }
            playerUiElements.ShiftStoneLeft.texture = shiftStoneTextures[playerInfo.ShiftStoneLeft];
            playerUiElements.ShiftStoneRight.texture = shiftStoneTextures[playerInfo.ShiftStoneRight];
        }

        private void ClearPlayerUi()
        {
            foreach (var playerUiElements in uiElementsByPlayer.Values)
            {
                GameObject.Destroy(playerUiElements.Container);
            }
            uiElementsByPlayer.Clear();
        }

        /**
         * Draws the HUD element for one player.
         * TODO: KILL THIS.
         */
        private void DrawPlayerHud__OLD(PlayerInfo playerInfo, bool rightSide, int yOffset = 0)
        {
            GUIStyle textRightAlign = new GUIStyle();
            textRightAlign.alignment = TextAnchor.MiddleRight;

            // Background box
            /*
            GUI.DrawTexture(
                new Rect(rightSide ? Screen.width - 550 : 0, 50 + yOffset, 550, 100),
                background,
                ScaleMode.StretchToFill,
                true,
                1); */
            GUI.Box(new Rect(rightSide ? Screen.width - 550 : 0, 50 + yOffset, 550, 100), backgroundTexture);
            if (rightSide)
            {
                GUI.Label(new Rect(Screen.width - 550 + 25, 50 + yOffset, 550 - 125, 50), $"<color=white>{playerInfo.Name}</color>", textRightAlign);
                GUI.Label(new Rect(Screen.width - 550 + 25, 50 + yOffset, 550 - 125, 50), $@"<color=#fbe18fff>{playerInfo.BP} BP</color>");
            } else
            {
                GUI.Label(new Rect(125, 50 + yOffset, 400, 50), $"<color=white>{playerInfo.Name}</color>");
                GUI.Label(new Rect(125, 50 + yOffset, 400, 50), $@"<color=#fbe18fff>{playerInfo.BP} BP</color>", textRightAlign);
            }
            
            // GUI.Label(new Rect(125, 50, 400, 50), $@"<color=#e2c991ff>{playerInfo.BP} BP</color>", textRightAlign);
        }
    }
}