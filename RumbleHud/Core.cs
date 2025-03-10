using Il2CppRUMBLE.Managers;
using Il2CppSystem.Security.Cryptography;
using Il2CppTMPro;
using MelonLoader;
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
        public string Name { get; set; }
        public int BP { get; set; }
        public int HP { get; set; }
        public string shiftStoneLeft { get; set; }
        public string shiftStoneRight { get; set; }
    }

    public class Core : MelonMod
    {
        private PlayerManager playerManager = null;
        private List<PlayerInfo> playerInfos = null;

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

            LoggerInstance.Msg($@"RumbleHud: ${canvas.pixelRect.ToString()}");

            GameObject myText = new GameObject();
            myText.transform.parent = uiContainer.transform;
            myText.name = "wibble";

            text = myText.AddComponent<Text>();
            text.font = font;
            text.text = "wobble";
            text.fontSize = 100;

            GameObject imageObject = new GameObject();
            imageObject.transform.parent = uiContainer.transform;
            imageObject.name = "background";

            RawImage image = imageObject.AddComponent<RawImage>();
            image.texture = background;
            image.SetNativeSize();

            var imageTransform = image.GetComponent<RectTransform>();
            // imageTransform.sizeDelta = new Vector2(background.width, background.height);
            imageTransform.anchorMin = new Vector2(0, 1);
            imageTransform.anchorMax = new Vector2(0, 1);
            imageTransform.pivot = new Vector2(0, 1); // Pivot on top left.
            imageTransform.anchoredPosition = new Vector3(0, -100, 0);

            LoggerInstance.Msg($@"RumbleHud: ${imageTransform.position}");
            LoggerInstance.Msg($@"RumbleHud: ${imageTransform.anchoredPosition}");

            // Text position
            var rectTransform = text.GetComponent<RectTransform>();
            rectTransform.localPosition = new Vector3(0, 0, 0);
            rectTransform.sizeDelta = new Vector2(400, 200);
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

            // Try to iterate over the list of players.
            try
            {
                var newPlayerInfos = new List<PlayerInfo>();

                var playerEnumerator = playerManager.AllPlayers.GetEnumerator();
                while (playerEnumerator.MoveNext())
                {
                    var current = playerEnumerator.Current;

                    PlayerInfo currentPlayerInfo = new PlayerInfo
                    {
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

            // base.OnUpdate();
        }

        public override void OnGUI()
        {
            base.OnGUI(); return;

            if (font == null || playerInfos == null) return;

            GUI.skin.font = font;
            base.OnUpdate();

            foreach (var playerInfo in playerInfos)
            {
                DrawPlayerHud(playerInfo, false);
                DrawPlayerHud(playerInfo, true);
            }
        }

        /**
         * Draws the HUD element for one player.
         */
        private void DrawPlayerHud(PlayerInfo playerInfo, bool rightSide, int yOffset = 0)
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