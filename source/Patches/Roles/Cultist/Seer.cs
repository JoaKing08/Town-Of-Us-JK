using System;
using TMPro;

namespace TownOfUs.Roles.Cultist
{
    public class CultistSeer : Role
    {
        public int UsesLeft;
        public TextMeshPro UsesText;
        public bool ButtonUsable => UsesLeft != 0;

        public CultistSeer(PlayerControl player) : base(player)
        {
            Name = "Seer";
            ImpostorText = () => "Reveal If Other Players Have Been Converted";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Reveal if other players have been converted" : "Ujawniaj czy inni gracze zostali przekonwertowani";
            Color = Patches.Colors.Seer;
            LastInvestigated = DateTime.UtcNow;
            RoleType = RoleEnum.CultistSeer;
            AddToRoleHistory(RoleType);
            UsesLeft = CustomGameOptions.MaxReveals;
        }

        public PlayerControl ClosestPlayer;
        public DateTime LastInvestigated { get; set; }

        public float SeerTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastInvestigated;
            var num = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}