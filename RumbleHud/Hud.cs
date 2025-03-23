using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using Il2CppRUMBLE.Players;
using UnityEngine.Rendering;
using Il2CppTMPro;

namespace RumbleHud
{
    class Hud
    {
        private static GameObject uiContainer = null;
        private static Canvas canvas = null;

        private static bool initialized = false;
        public static bool Initialized { get { return initialized; } }

        private static readonly int playerControllerLayerMask = LayerMask.NameToLayer("PlayerController");
        private const int playerControllerLayer = 8388608;

        private static Dictionary<string, PlayerUiElements> uiElementsByPlayer = new Dictionary<string, PlayerUiElements>();

        public static int PlayerUiElementsCount { get {  return uiElementsByPlayer.Count; } }

        private static GameObject previewRenderer = null;
        private static GameObject previewHead = null;

        public static string SelfPlayFabId { get; set; }

        public static void Initialize()
        {
            if (initialized) return;

            uiContainer = new GameObject();
            uiContainer.name = "RumbleHud_Canvas";
            uiContainer.AddComponent<Canvas>();

            canvas = uiContainer.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var canvasScaler = uiContainer.AddComponent<CanvasScaler>();

            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1008);

            uiContainer.AddComponent<GraphicRaycaster>();

            GameObject.DontDestroyOnLoad(uiContainer);
            GameObject.DontDestroyOnLoad(canvas);

            initialized = true;
        }

        public static void ToggleVisible()
        {
            if (uiContainer == null) return;

            uiContainer.active = !uiContainer.active;
        }

        public static void SetScale(float newScale)
        {
            Settings.Instance.HudScale = newScale;

            foreach (var playerUiElements in uiElementsByPlayer.Values)
            {
                playerUiElements.Container.transform.localScale = new Vector3(newScale, newScale, newScale);
                int offset = (int)((playerUiElements.Position / 2) * 150 * -1 * newScale);

                var rawImageTransform = playerUiElements.Background.GetComponent<RectTransform>();
                if (playerUiElements.IsRightAligned)
                {
                    // Anchor to top right.
                    rawImageTransform.anchoredPosition = new Vector3(0, -50 + offset, 0);
                }
                else
                {
                    rawImageTransform.anchoredPosition = new Vector3(0, -50 + offset, 0);
                }
            }
        }

        public static void LoadPreviewCharacter()
        {
            previewHead = GameObject.Find("--------------SCENE--------------/Gym_Production/Dressing Room/Preview Player Controller/Visuals/Skelington/Bone_Pelvis/Bone_Spine_A/Bone_Chest/Bone_Neck/Bone_Head");
            previewRenderer = GameObject.Find("--------------SCENE--------------/Gym_Production/Dressing Room/Preview Player Controller/Visuals/Renderer");

            if (previewRenderer != null && previewRenderer.layer != playerControllerLayerMask)
            {
                previewRenderer.layer = playerControllerLayerMask;
            }
        }

        public static bool PlayerHudExists(string playFabId)
        {
            return uiElementsByPlayer.ContainsKey(playFabId);
        }

        public static void CreatePlayerUi(PlayerInfo playerInfo, int position)
        {
            bool isRightAligned = position % 2 == 1;
            int offset = (int)((position / 2) * 150 * -1 * Settings.Instance.HudScale);

            // BACKGROUND

            GameObject backgroundObject = new GameObject();
            backgroundObject.transform.parent = uiContainer.transform;
            backgroundObject.name = $"RumbleHud_{playerInfo.PlayFabId}_background";

            RawImage rawImage = backgroundObject.AddComponent<RawImage>();
            rawImage.texture = Resources.BackgroundTexture;
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
            nameObject.name = $"RumbleHud_{playerInfo.PlayFabId}_name";
            TextMeshProUGUI nameText = nameObject.AddComponent<TextMeshProUGUI>();

            nameText.font = Resources.TmpFont;
            nameText.text = playerInfo.Name;

            var nameTextTransform = nameText.GetComponent<RectTransform>();
            nameTextTransform.sizeDelta = new Vector2(300, 50);
            nameText.enableAutoSizing = true;
            nameText.fontSizeMax = 42;
            nameText.fontSizeMin = 16;

            if (isRightAligned)
            {
                // Anchor top right.
                nameTextTransform.anchorMin = new Vector2(1, 1);
                nameTextTransform.anchorMax = new Vector2(1, 1);
                nameTextTransform.pivot = new Vector2(1, 1);

                nameTextTransform.anchoredPosition = new Vector3(-100, -10);
                nameText.alignment = TextAlignmentOptions.TopRight;
            }
            else
            {
                // Anchor to top left.
                nameTextTransform.anchorMin = new Vector2(0, 1);
                nameTextTransform.anchorMax = new Vector2(0, 1);
                nameTextTransform.pivot = new Vector2(0, 1);

                nameTextTransform.anchoredPosition = new Vector3(100, -10);
                nameText.alignment = TextAlignmentOptions.TopLeft;
            }

            // BP

            GameObject bpObject = new GameObject();
            bpObject.transform.parent = backgroundObject.transform;
            bpObject.name = $"RumbleHud_{playerInfo.PlayFabId}_bp";
            TextMeshProUGUI bpText = bpObject.AddComponent<TextMeshProUGUI>();

            bpText.color = new Color(251f / 255, 1, 143f / 255);
            bpText.font = Resources.TmpFont;
            bpText.text = $"{playerInfo.BP} BP";
            bpText.enableAutoSizing = true;
            bpText.fontSizeMax = 36;
            bpText.fontSizeMin = 16;

            var bpTextTransform = bpText.GetComponent<RectTransform>();
            bpTextTransform.sizeDelta = new Vector2(125, 50);

            if (isRightAligned)
            {
                // Anchor to top left.
                bpTextTransform.anchorMin = new Vector2(0, 1);
                bpTextTransform.anchorMax = new Vector2(0, 1);
                bpTextTransform.pivot = new Vector2(0, 1);

                bpTextTransform.anchoredPosition = new Vector3(25, -10);

                bpText.alignment = TextAlignmentOptions.TopLeft;
            }
            else
            {
                // Anchor to top right.
                bpTextTransform.anchorMin = new Vector2(1, 1);
                bpTextTransform.anchorMax = new Vector2(1, 1);
                bpTextTransform.pivot = new Vector2(1, 1);

                bpTextTransform.anchoredPosition = new Vector3(-25, -10);

                bpText.alignment = TextAlignmentOptions.TopRight;
            }

            // HEALTH BAR

            GameObject healthBarObject = new GameObject();
            healthBarObject.transform.parent = backgroundObject.transform;
            healthBarObject.name = $"RumbleHud_{playerInfo.PlayFabId}_healthBackground";
            Image healthBar = healthBarObject.AddComponent<Image>();
            healthBar.color = Resources.HealthFull;

            var healthBarTransform = healthBar.GetComponent<RectTransform>();
            healthBarTransform.sizeDelta = new Vector2(340, 10);
            if (isRightAligned)
            {
                // Anchor to bottom left.
                healthBarTransform.anchorMin = new Vector2(0, 0);
                healthBarTransform.anchorMax = new Vector2(0, 0);
                healthBarTransform.pivot = new Vector2(0, 0);

                healthBarTransform.anchoredPosition = new Vector2(25, 20);
            }
            else
            {
                // Anchor to bottom right.
                healthBarTransform.anchorMin = new Vector2(1, 0);
                healthBarTransform.anchorMax = new Vector2(1, 0);
                healthBarTransform.pivot = new Vector2(1, 0);

                healthBarTransform.anchoredPosition = new Vector2(-25, 20);
            }

            //  HEALTH PIPS

            GameObject healthPipsObject = new GameObject();
            healthPipsObject.transform.parent = healthBarObject.transform;
            healthPipsObject.name = $"RumbleHud_{playerInfo.PlayFabId}_healthPips";
            RawImage healthPips = healthPipsObject.AddComponent<RawImage>();

            healthPips.texture = Resources.HealthPipsTexture;

            healthPips.uvRect = new Rect(0, 0, 20, 1);

            var healthPipsTransform = healthPips.GetComponent<RectTransform>();
            healthPipsTransform.sizeDelta = new Vector2(Resources.HealthPipsTexture.width * 20, Resources.HealthPipsTexture.height);

            if (isRightAligned)
            {
                // Anchor to middle right.
                healthPipsTransform.anchorMin = new Vector2(1, 0.5f);
                healthPipsTransform.anchorMax = new Vector2(1, 0.5f);
                healthPipsTransform.pivot = new Vector2(1, 0.5f);

                healthPipsTransform.anchoredPosition = new Vector2(0, 0);
            }
            else
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
            leftShiftStoneObject.name = $"RumbleHud_{playerInfo.PlayFabId}_leftShiftStone";
            RawImage leftShiftStone = leftShiftStoneObject.AddComponent<RawImage>();

            leftShiftStone.texture = Resources.GetShiftStoneTexture(playerInfo.ShiftStoneLeft);

            var leftShiftStoneTransform = leftShiftStoneObject.GetComponent<RectTransform>();

            leftShiftStoneTransform.sizeDelta = new Vector2(35, 35);

            if (isRightAligned)
            {
                // Anchor to bottom right.
                leftShiftStoneTransform.anchorMin = new Vector2(1, 0);
                leftShiftStoneTransform.anchorMax = new Vector2(1, 0);
                leftShiftStoneTransform.pivot = new Vector2(1, 0);

                leftShiftStoneTransform.anchoredPosition = new Vector2(-40 - 100, 10);
            }
            else
            {
                // Anchor to bottom left.
                leftShiftStoneTransform.anchorMin = new Vector2(0, 0);
                leftShiftStoneTransform.anchorMax = new Vector2(0, 0);
                leftShiftStoneTransform.pivot = new Vector2(0, 0);

                leftShiftStoneTransform.anchoredPosition = new Vector2(95, 10);
            }

            // RIGHT SHIFT STONE

            GameObject rightShiftStoneObject = new GameObject();
            rightShiftStoneObject.transform.parent = backgroundObject.transform;
            rightShiftStoneObject.name = $"RumbleHud_{playerInfo.PlayFabId}_rightShiftStone";
            RawImage rightShiftStone = rightShiftStoneObject.AddComponent<RawImage>();

            rightShiftStone.texture = Resources.GetShiftStoneTexture(playerInfo.ShiftStoneRight);

            var rightShiftStoneTransform = rightShiftStoneObject.GetComponent<RectTransform>();

            rightShiftStoneTransform.sizeDelta = new Vector2(35, 35);

            if (isRightAligned)
            {
                // Anchor to bottom right.
                rightShiftStoneTransform.anchorMin = new Vector2(1, 0);
                rightShiftStoneTransform.anchorMax = new Vector2(1, 0);
                rightShiftStoneTransform.pivot = new Vector2(1, 0);

                rightShiftStoneTransform.anchoredPosition = new Vector2(-95, 10);
            }
            else
            {
                // Anchor to bottom left.
                rightShiftStoneTransform.anchorMin = new Vector2(0, 0);
                rightShiftStoneTransform.anchorMax = new Vector2(0, 0);
                rightShiftStoneTransform.pivot = new Vector2(0, 0);

                rightShiftStoneTransform.anchoredPosition = new Vector2(40 + 100, 10);
            }

            // RENDER TEXTURE

            var renderTexture = new RenderTexture(100, 100, 16);
            renderTexture.Create();

            // PORTRAIT CAMERA

            GameObject portraitCameraObject = new GameObject();
            portraitCameraObject.name = $"RumbleHud_{playerInfo.PlayFabId}_portraitCamera";

            Camera portraitCamera = portraitCameraObject.AddComponent<Camera>();
            portraitCamera.targetTexture = renderTexture;
            portraitCamera.fieldOfView = 50;
            portraitCamera.cullingMask = playerControllerLayer;
            portraitCamera.clearFlags = CameraClearFlags.Depth;

            GameObject.DontDestroyOnLoad(portraitCameraObject);

            // PORTRAIT RAW IMAGE

            GameObject portraitImageObject = new GameObject();
            portraitImageObject.name = $"RumbleHud_{playerInfo.PlayFabId}_portrait";

            RawImage portraitImage = portraitImageObject.AddComponent<RawImage>();
            portraitImage.transform.parent = backgroundObject.transform;
            portraitImage.texture = renderTexture;

            var portraitImageTransform = portraitImage.GetComponent<RectTransform>();

            portraitImageTransform.sizeDelta = new Vector2(100, 100);

            if (isRightAligned)
            {
                // Anchor to top right.
                portraitImageTransform.anchorMin = new Vector2(1, 1);
                portraitImageTransform.anchorMax = new Vector2(1, 1);
                portraitImageTransform.pivot = new Vector2(1, 1);

                portraitImageTransform.anchoredPosition = new Vector2(0, 0);
            }
            else
            {
                // Anchor to top left.
                portraitImageTransform.anchorMin = new Vector2(0, 1);
                portraitImageTransform.anchorMax = new Vector2(0, 1);
                portraitImageTransform.pivot = new Vector2(0, 1);

                portraitImageTransform.anchoredPosition = new Vector2(0, 0);
            }

            var scale = Settings.Instance.HudScale;
            backgroundObject.transform.localScale = new Vector3(scale, scale, scale);

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
                HeadshotCamera = portraitCamera,
                renderTexture = renderTexture,
                Portrait = portraitImage,
                PortraitGenerated = -30,
                IsRightAligned = isRightAligned,
                PlayFabId = playerInfo.PlayFabId,
                Position = position
            };
        }

        public static void UpdatePlayerUi(PlayerInfo playerInfo)
        {
            var playerUiElements = uiElementsByPlayer[playerInfo.PlayFabId];

            if (playerUiElements == null) return;

            playerUiElements.BP.text = $"{playerInfo.BP} BP";

            Color healthBarColor = Resources.HealthFull;

            if (playerInfo.HP < 20)
            {
                healthBarColor = Resources.HealthHigh;
            }
            if (playerInfo.HP <= 10)
            {
                healthBarColor = Resources.HealthMedium;
            }
            if (playerInfo.HP <= 5)
            {
                healthBarColor = Resources.HealthLow;
            }

            playerUiElements.HealthBar.color = healthBarColor;

            var healthPipsTransform = playerUiElements.HealthPips.GetComponent<RectTransform>();
            playerUiElements.HealthPips.uvRect = new Rect(0, 0, playerInfo.HP, 1);

            healthPipsTransform.sizeDelta = new Vector2(
                Resources.HealthPipsTexture.width * playerInfo.HP,
                Resources.HealthPipsTexture.height);

            var leftShiftStoneTexture = Resources.GetShiftStoneTexture(playerInfo.ShiftStoneLeft);
            var rightShiftStoneTexture = Resources.GetShiftStoneTexture(playerInfo.ShiftStoneRight);

            playerUiElements.ShiftStoneLeft.texture = leftShiftStoneTexture;
            playerUiElements.ShiftStoneRight.texture = rightShiftStoneTexture;

            if (playerUiElements.PortraitGenerated > 0)
            {
                playerUiElements.HeadshotCamera.gameObject.SetActive(false);
            }

            if (playerUiElements.PortraitGenerated <= 0)
            {
                GameObject head = GetPlayerHead(playerInfo.PlayFabId, playerInfo.PlayerController);
                GameObject visuals = GetPlayerVisuals(playerInfo.PlayFabId, playerInfo.PlayerController);
                if (head != null)
                {
                    PointCamera(playerUiElements.HeadshotCamera, head, visuals, playerUiElements.IsRightAligned);
                    playerUiElements.PortraitGenerated++;
                }
            }
        }

        private static GameObject GetPlayerHead(string playFabId, PlayerController playerController)
        {
            if (playFabId == SelfPlayFabId) return previewHead;

            if (playerController == null) return null;

            var controllerObject = playerController.gameObject;

            if (!controllerObject.active) return null;
            // If the renderer aren't visible, also cancel.
            var skinnedMeshRenderer = controllerObject?.transform
                ?.GetChild(0)
                ?.GetChild(0)
                ?.GetComponent<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer == null) return null;
            if (!skinnedMeshRenderer.isVisible || !skinnedMeshRenderer.enabled)
            {
                return null;
            }

            // From the controller object, get the nose bone.
            // Controller > Visuals > Skelington > Pelvis > Spine > Chest > Neck > Head > Nose
            var noseTransform = controllerObject?.transform
                ?.GetChild(0) // Visuals
                ?.GetChild(1) // Skelington
                ?.GetChild(0) // Pelvis
                ?.GetChild(4) // Spine
                ?.GetChild(0) // Chest
                ?.GetChild(0) // Neck
                ?.GetChild(0) // Head
                ?.GetChild(9); // Nose

            return noseTransform?.gameObject;
        }

        private static GameObject GetPlayerVisuals(string playFabId, PlayerController playerController)
        {
            if (playFabId == SelfPlayFabId)
            {
                return GetPlayerHead(playFabId, null);
            };

            if (playerController == null) return null;

            var controllerObject = playerController.gameObject;

            if (!controllerObject.active) return null;

            return controllerObject?.transform?.GetChild(0).gameObject;
        }

        private static void PointCamera(Camera camera, GameObject head, GameObject visuals, bool facingLeft)
        {
            camera.transform.position = head.transform.position;

            // Get the rotation from Visuals.
            camera.transform.rotation = Quaternion.Euler(
                0,
                visuals.transform.rotation.eulerAngles.y,
                0);

            camera.transform.position += camera.transform.forward * 0.5f;
            camera.transform.Rotate(0, 180, 0);

            camera.transform.RotateAround(head.transform.position, camera.transform.up, facingLeft ? -30 : 30);
        }

        public static void ClearPlayerUi(bool includeSelfPlayer = false)
        {
            foreach (var playerUiElements in uiElementsByPlayer.Values)
            {
                // Don't clear self normally.
                // Currently, I only know how to generate self portrait while in the gym.
                if (playerUiElements.PlayFabId == SelfPlayFabId && !includeSelfPlayer)
                {
                    continue;
                }

                GameObject.Destroy(playerUiElements.Container);
                playerUiElements.HeadshotCamera.targetTexture = null;
                GameObject.Destroy(playerUiElements.HeadshotCamera.gameObject);

                uiElementsByPlayer.Remove(playerUiElements.PlayFabId);
            }
        }
    }
}
