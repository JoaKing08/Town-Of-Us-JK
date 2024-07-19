using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Deputy : Role
    {
        public Deputy(PlayerControl player) : base(player)
        {
            Name = "Deputy";
            ImpostorText = () => "Kill Evils During Meetings";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Shoot out evildoers in broad daylight" : "Wystrzel zloczynców w swietle dnia";
            Color = Patches.Colors.Deputy;
            RoleType = RoleEnum.Deputy;
            AddToRoleHistory(RoleType);
            Revealed = false;
            Targets = new List<byte>();
        }
        public bool Revealed { get; set; }

        public Dictionary<byte, GameObject> ShootButtons = new();
        public List<byte> Targets = new List<byte>();
        public int AliveTargets => Revealed ? CustomGameOptions.MaxDeputyTargets : Targets.Any() ? Targets.ToArray().Count(x => !Utils.PlayerById(x).Data.IsDead && !Utils.PlayerById(x).Data.Disconnected) : 0;
        public DateTime LastAimed;
        public PlayerControl ClosestPlayer;
        public TextMeshPro UsesText;

        internal override bool Criteria()
        {
            return Revealed && CustomGameOptions.RevealDeputy && !Player.Data.IsDead || base.Criteria();
        }

        internal override bool RoleCriteria()
        {
            if (!Player.Data.IsDead && CustomGameOptions.RevealDeputy) return Revealed || base.RoleCriteria();
            return false || base.RoleCriteria();
        }

        public float AimTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastAimed;
            var num = CustomGameOptions.DeputyAimCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}