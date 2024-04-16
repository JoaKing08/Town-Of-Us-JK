using System;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

namespace TownOfUs.Roles
{
    public class CursedSoul : Role
    {
        public bool SpawnedAs = true;
        public bool WasSwapped = false;
        public PlayerControl ClosestPlayer;
        public DateTime LastSwapped { get; set; }

        public CursedSoul(PlayerControl player) : base(player)
        {
            Name = "Cursed Soul";
            ImpostorText = () => "Soul Swap To Get Role";
            TaskText = () => WasSwapped ? "Your role was stolen. Now soul swap for new role!" : SpawnedAs ? "Soul swap with player to get new role" : "Your target was killed. Now soul swap for new role!";
            Color = Patches.Colors.CursedSoul;
            RoleType = RoleEnum.CursedSoul;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralBenign;
            LastSwapped = DateTime.UtcNow;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__36 __instance)
        {
            var cursedSoulTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            cursedSoulTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = cursedSoulTeam;
        }

        public float SoulSwapTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastSwapped;
            var num = CustomGameOptions.SoulSwapCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}