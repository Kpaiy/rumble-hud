using UnityEngine.UI;
using UnityEngine;
using Il2CppTMPro;

namespace RumbleHud
{
    class PlayerUiElements
    {
        public GameObject Container { get; set; }
        public RawImage Background { get; set; }
        public TextMeshProUGUI Name { get; set; }
        public TextMeshProUGUI BP { get; set; }
        public Image HealthBar { get; set; }
        public RawImage HealthPips { get; set; }
        public RawImage ShiftStoneLeft { get; set; }
        public RawImage ShiftStoneRight { get; set; }
        public Camera HeadshotCamera { get; set; }
        public RenderTexture renderTexture { get; set; }
        public RawImage Portrait { get; set; }
        public int PortraitGenerated { get; set; }
        public bool IsRightAligned { get; set; }
        public string PlayFabId { get; set; }
        public int Position { get; set; }
    }
}
