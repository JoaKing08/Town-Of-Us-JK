using UnityEngine;
using System;
using TownOfUs.ImpostorRoles.BomberMod;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.Patches;
using System.Collections.Generic;

namespace TownOfUs.Roles
{
    public class Witch : Role

    {
        public DateTime LastControl { get; set; }
        public PlayerControl ClosestPlayer;
        public PlayerControl ControledPlayer;
        public List<byte> RevealedPlayers;

        public Witch(PlayerControl player) : base(player)
        {
            Name = "Witch";
            ImpostorText = () => "Control Players To Use Their Abilities";
            TaskText = () => "Control player to use their ability on wrong players";
            Color = Patches.Colors.Witch;
            RoleType = RoleEnum.Witch;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralEvil;
            RevealedPlayers = new List<byte>();
        }
        public float ControlTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastControl;
            var num = (ControledPlayer == null ? CustomGameOptions.ControlCooldown : CustomGameOptions.OrderCooldown) * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            var witchTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            witchTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = witchTeam;
        }
    }
}