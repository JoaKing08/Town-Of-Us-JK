using System;
using UnityEngine;
using TMPro;
using TownOfUs.Extensions;

namespace TownOfUs.Roles
{
    public class Lookout : Role
    {
        public bool Enabled;
        public DateTime LastWatched;
        public float TimeRemaining;

        public Lookout(PlayerControl player) : base(player)
        {
            Name = "Lookout";
            ImpostorText = () => "Watch The Crew";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "See what everyone is doing" : "Obserwuj co wszyscy robia";
            Color = Patches.Colors.Lookout;
            LastWatched = DateTime.UtcNow;
            RoleType = RoleEnum.Lookout;
            AddToRoleHistory(RoleType);
        }

        public bool Watching => TimeRemaining > 0f;

        public float WatchTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastWatched;
            ;
            var num = CustomGameOptions.WatchCooldown * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void StartWatch()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
        }


        public void EndWatch()
        {
            Enabled = false;
            LastWatched = DateTime.UtcNow;
        }

        public bool TryGetModifiedAppearance(out VisualAppearance appearance)
        {
            appearance = Player.GetDefaultAppearance();
            if (Watching) appearance.SpeedFactor = 0;
            return true;
        }
    }
}