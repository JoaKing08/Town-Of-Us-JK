using System.Collections.Generic;
using System.Linq;
using TMPro;
using TownOfUs.Patches;
using UnityEngine;
using TownOfUs.NeutralRoles.ExecutionerMod;
using TownOfUs.NeutralRoles.GuardianAngelMod;
using System;
using TownOfUs.CrewmateRoles.VampireHunterMod;

namespace TownOfUs.Roles
{
    public class Bodyguard : Role
    {
        public DateTime LastGuard;
        public PlayerControl ClosestPlayer;
        public PlayerControl GuardedPlayer;

        public Bodyguard(PlayerControl player) : base(player)
        {
            Name = "Bodyguard";
            ImpostorText = () => "Sacrifice Yourself To Protect Crew!";
            TaskText = () => "Protect crewmates to get killed instead of them";
            Color = Patches.Colors.Bodyguard;
            RoleType = RoleEnum.Bodyguard;
            LastGuard = DateTime.UtcNow;
            GuardedPlayer = null;
            AddToRoleHistory(RoleType);
        }

        public float GuardTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastGuard;
            var num = CustomGameOptions.GuardCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}
