using System;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

namespace TownOfUs.Roles
{
    public class Mystic : Role
    {
        public Dictionary<byte, ArrowBehaviour> BodyArrows = new Dictionary<byte, ArrowBehaviour>();
        public byte VisionPlayer;
        public List<byte> PlayersInteracted;
        public List<byte> InteractingPlayers;
        public bool UsedAbility;
        public DateTime LastVision;
        public PlayerControl ClosestPlayer;
        public Mystic(PlayerControl player) : base(player)
        {
            Name = "Mystic";
            ImpostorText = () => "Understand When And Where Kills Happen";
            TaskText = () => "Know When and Where Kills Happen";
            Color = Patches.Colors.Mystic;
            RoleType = RoleEnum.Mystic;
            UsedAbility = false;
            VisionPlayer = byte.MaxValue;
            PlayersInteracted = new List<byte>();
            InteractingPlayers = new List<byte>();
            LastVision = DateTime.UtcNow;
            AddToRoleHistory(RoleType);
        }

        public void DestroyArrow(byte targetPlayerId)
        {
            var arrow = BodyArrows.FirstOrDefault(x => x.Key == targetPlayerId);
            if (arrow.Value != null)
                Object.Destroy(arrow.Value);
            if (arrow.Value.gameObject != null)
                Object.Destroy(arrow.Value.gameObject);
            BodyArrows.Remove(arrow.Key);
        }

        public float VisionTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastVision;
            var num = CustomGameOptions.VisionCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}