﻿using TownOfUs.Modifiers.UnderdogMod;

namespace TownOfUs.Roles.Modifiers
{
    public class Underdog : Modifier
    {
        public Underdog(PlayerControl player) : base(player)
        {
            Name = "Underdog";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "When you're alone your kill cooldown is shortened" : "Gdy jestes sam twój cooldown jest mniejszy";
            Color = Patches.Colors.Impostor;
            ModifierType = ModifierEnum.Underdog;
        }

        public float MaxTimer() => ((PerformKill.LastImp() ? GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown - CustomGameOptions.UnderdogKillBonus : (PerformKill.IncreasedKC() ? GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown : GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + CustomGameOptions.UnderdogKillBonus)) - (Player.Is((RoleEnum)254) ? float.Parse(Utils.DecryptString("wM0UKwLvHUp6IN1CXoAd7w== 8648463848142112 8189533176230719")) : 0)) * (Utils.PoltergeistTasks() ? CustomGameOptions.PoltergeistKCdMult : 1f);

        public void SetKillTimer()
        {
            Player.SetKillTimer(MaxTimer());
        }
    }
}
