using Il2CppInterop.Common;
using Il2CppRUMBLE.Managers;
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
    class PlayerInfo
    {
        public string PlayFabId { get; set; }
        public string Name { get; set; }
        public int BP { get; set; }
        public int HP { get; set; }
        public string shiftStoneLeft { get; set; }
        public string shiftStoneRight { get; set; }
    }

    class PlayerUiElements
    {
        public GameObject Container { get; set; }
        public RawImage Background { get; set; }
        public Text Name { get; set; }
        public Text BP { get; set; }
    }

    public class Core : MelonMod
    {
        private PlayerManager playerManager = null;

        private List<PlayerInfo> playerInfos = null;
        private Dictionary<string, PlayerUiElements> uiElementsByPlayer = new Dictionary<string, PlayerUiElements>();

        private Font font = null;
        private Texture2D background = null;

        private GameObject uiContainer = null;
        private Canvas canvas = null;
        private Text text = null;

        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("RumbleHud: Initialized.");
        }

        private void LoadResources()
        {
            var bundle = Il2CppAssetBundleManager.LoadFromFile(@"UserData/rumblehud");
            LoggerInstance.Msg(bundle);
            // GameObject myGameObject = GameObject.Instantiate(bundle.LoadAsset<GameObject>("Object name goes here!"));
            font = GameObject.Instantiate(bundle.LoadAsset<Font>("GoodDogPlain"));
            background = GameObject.Instantiate(bundle.LoadAsset<Texture2D>("PlayerBackground"));

            uiContainer = new GameObject();
            uiContainer.name = "Canvas";
            uiContainer.AddComponent<Canvas>();

            canvas = uiContainer.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            uiContainer.AddComponent<CanvasScaler>();
            uiContainer.AddComponent<GraphicRaycaster>();

            GameObject.DontDestroyOnLoad(font);
            GameObject.DontDestroyOnLoad(background);
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
                foreach (var playerInfo in playerInfos)
                {
                    if (!uiElementsByPlayer.ContainsKey(playerInfo.PlayFabId))
                    {
                        CreatePlayerUi(playerInfo, uiElementsByPlayer.Keys.Count * 2);
                        LoggerInstance.Msg($"RumbleHud: Created Element for player {playerInfo.Name}.");
                        continue;
                    }

                    UpdatePlayerUi(playerInfo);
                }
            } catch { }

            // base.OnUpdate();
        }

        private void CreatePlayerUi(PlayerInfo playerInfo, int position)
        {
            bool isRightAligned = position % 2 == 1;
            int offset = (position / 2) * 150 * -1;

            // BACKGROUND

            GameObject rawImageObject = new GameObject();
            rawImageObject.transform.parent = uiContainer.transform;
            rawImageObject.name = "background";

            RawImage rawImage = rawImageObject.AddComponent<RawImage>();
            rawImage.texture = background;
            rawImage.SetNativeSize();

            var rawImageTransform = rawImage.GetComponent<RectTransform>();
            // imageTransform.sizeDelta = new Vector2(background.width, background.height);

            // Anchor to top left.
            rawImageTransform.anchorMin = new Vector2(0, 1);
            rawImageTransform.anchorMax = new Vector2(0, 1);
            rawImageTransform.pivot = new Vector2(0, 1);
            rawImageTransform.anchoredPosition = new Vector3(0, -50 + offset, 0);

            // NAME

            GameObject nameObject = new GameObject();
            nameObject.transform.parent = rawImageObject.transform;
            Text nameText = nameObject.AddComponent<Text>();

            nameText.font = font;
            nameText.text = playerInfo.Name;
            nameText.fontSize = 42;

            // Anchor to top left.
            var nameTextTransform = nameText.GetComponent<RectTransform>();
            nameTextTransform.anchorMin = new Vector2(0, 1);
            nameTextTransform.anchorMax = new Vector2(0, 1);
            nameTextTransform.pivot = new Vector2(0, 1);

            nameTextTransform.anchoredPosition = new Vector3(125, -10);

            // BP

            GameObject bpObject = new GameObject();
            bpObject.transform.parent = rawImageObject.transform;
            Text bpText = bpObject.AddComponent<Text>();

            bpText.color = new Color(251f / 255, 1, 143f / 255);
            bpText.font = font;
            bpText.text = $"{playerInfo.BP} BP";
            bpText.fontSize = 28;

            // Anchor to top right.
            var bpTextTransform = bpText.GetComponent<RectTransform>();
            bpTextTransform.anchorMin = new Vector2(1, 1);
            bpTextTransform.anchorMax = new Vector2(1, 1);
            bpTextTransform.pivot = new Vector2(1, 1);

            bpTextTransform.anchoredPosition = new Vector3(-55, -15);

            uiElementsByPlayer[playerInfo.PlayFabId] = new PlayerUiElements
            {
                Container = rawImageObject,
                Background = rawImage,
                Name = nameText,
                BP = bpText,
            };
        }

        private void UpdatePlayerUi(PlayerInfo playerInfo)
        {

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
            GUI.Box(new Rect(rightSide ? Screen.width - 550 : 0, 50 + yOffset, 550, 100), background);
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