using UnityEngine;
using System;
using TownOfUs.ImpostorRoles.BomberMod;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.Patches;
using System.Linq;
using Reactor.Utilities.Extensions;

namespace TownOfUs.Roles
{
    public class Sniper : Role

    {
        public KillButton _snipeButton;
        public DateTime LastAim { get; set; }
        public PlayerControl ClosestPlayer;
        public PlayerControl AimedPlayer;

        public Sniper(PlayerControl player) : base(player)
        {
            Name = "Sniper";
            ImpostorText = () => "Kill Players From Far Far Away";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Aim to kill crewmetes from far away" : "Wyceluj by zabijac graczy z daleka";
            Color = Palette.ImpostorRed;
            RoleType = RoleEnum.Sniper;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
        }
        public KillButton SnipeButton
        {
            get => _snipeButton;
            set
            {
                _snipeButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        public float AimTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastAim;
            var num = CustomGameOptions.AimCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}