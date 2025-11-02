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
        public TextMeshProUGUI HostText {  get; set; }
        public RawImage HostIcon {  get; set; }
        public Camera HeadshotCamera { get; set; }
        public RenderTexture RenderTexture { get; set; }
        public RawImage Portrait { get; set; }
        public int PortraitGenerated { get; set; }
        public bool IsRightAligned { get; set; }
        public string PlayFabId { get; set; }
        public int Position { get; set; }
        public RawImage FirstRound { get; set; }
        public RawImage SecondRound { get; set; }
        public int RoundsWon { get; set; }
    }
}
