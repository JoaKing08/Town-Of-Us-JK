using UnityEngine;
using System;
using TownOfUs.ImpostorRoles.BomberMod;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.Patches;

namespace TownOfUs.Roles
{
    public class Poisoner : Role

    {
        public KillButton _poisonButton;
        public DateTime PoisonTime { get; set; }
        public PlayerControl ClosestPlayer;
        public PlayerControl PoisonedPlayer;

        public Poisoner(PlayerControl player) : base(player)
        {
            Name = "Poisoner";
            ImpostorText = () => "Poison Players To Kill With No Evidence";
            TaskText = () => "Poison players to kill with no evidence";
            Color = Palette.ImpostorRed;
            RoleType = RoleEnum.Poisoner;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
        }
        public KillButton PoisonButton
        {
            get => _poisonButton;
            set
            {
                _poisonButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        public float PoisonTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - PoisonTime;
            var num = CustomGameOptions.PoisonDelay * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}