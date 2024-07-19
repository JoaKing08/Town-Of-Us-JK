namespace TownOfUs.Roles.Modifiers
{
    public class Bait : Modifier
    {
        public Bait(PlayerControl player) : base(player)
        {
            Name = "Bait";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Killing you causes an instant self-report" : "Zabicie ciebie powoduje natychmiastowy self-report";
            Color = Patches.Colors.Bait;
            ModifierType = ModifierEnum.Bait;
        }
    }
}