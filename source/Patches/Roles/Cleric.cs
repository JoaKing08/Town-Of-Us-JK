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
    public class Cleric : Role
    {
        public DateTime LastBarrier;
        public PlayerControl ClosestPlayer;
        public PlayerControl BarrieredPlayer;

        public Cleric(PlayerControl player) : base(player)
        {
            Name = "Cleric";
            ImpostorText = () => "Protect Crew From Attacks!";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Barrier crewmates to destroy killers plan" : "Bron graczy by zniszczyc plany zabójców";
            Color = Patches.Colors.Cleric;
            RoleType = RoleEnum.Cleric;
            LastBarrier = DateTime.UtcNow;
            BarrieredPlayer = null;
            AddToRoleHistory(RoleType);
        }

        public float BarrierTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastBarrier;
            var num = CustomGameOptions.BarrierCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}
