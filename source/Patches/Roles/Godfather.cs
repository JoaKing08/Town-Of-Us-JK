using UnityEngine;
using System;
using TownOfUs.ImpostorRoles.BomberMod;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.Patches;
using System.Linq;
using Reactor.Utilities.Extensions;

namespace TownOfUs.Roles
{
    public class Godfather : Role

    {
        public KillButton _recruitButton;
        public DateTime LastRecruit { get; set; }
        public PlayerControl ClosestPlayer;
        public bool Recruited { get; set; }
        public bool CanKill => Recruited && !PlayerControl.AllPlayerControls.ToArray().Any(x => x.Is(RoleEnum.Mafioso) && !x.Data.IsDead && !x.Data.Disconnected);

        public Godfather(PlayerControl player) : base(player)
        {
            Name = "Godfather";
            ImpostorText = () => "Order Your Dirty Work To Others";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Recruit a player to kill instead of you." : "Zrekrutuj gracza by zabijal zamiast ciebie";
            Color = Palette.ImpostorRed;
            RoleType = RoleEnum.Godfather;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
        }
        public KillButton RecruitButton
        {
            get => _recruitButton;
            set
            {
                _recruitButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        public float RecruitTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastRecruit;
            var flag2 = -(float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (-(float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}