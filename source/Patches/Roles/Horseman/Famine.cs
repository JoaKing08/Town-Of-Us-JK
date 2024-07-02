using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using TownOfUs.Extensions;

namespace TownOfUs.Roles.Horseman
{
    public class Famine : Role
    {
        public Famine(PlayerControl owner) : base(owner)
        {
            Name = "Famine";
            Color = Patches.Colors.Famine;
            LastStarved = DateTime.UtcNow;
            RoleType = RoleEnum.Famine;
            AddToRoleHistory(RoleType);
            ImpostorText = () => "";
            TaskText = () => "Starve everyone to death!\nFake Tasks:";
            Faction = Faction.NeutralApocalypse;
        }
        public bool Announced;

        public PlayerControl ClosestPlayer;
        public DateTime LastStarved { get; set; }

        public float StarveTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastStarved;
            var num = CustomGameOptions.FamineCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}