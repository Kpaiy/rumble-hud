using Il2CppRUMBLE.Players;

namespace RumbleHud
{
    class PlayerInfo
    {
        public string PlayFabId { get; set; }
        public string Name { get; set; }
        public int BP { get; set; }
        public int HP { get; set; }
        public ShiftStones ShiftStoneLeft { get; set; }
        public ShiftStones ShiftStoneRight { get; set; }
        public PlayerController PlayerController { get; set; }
    }
}
