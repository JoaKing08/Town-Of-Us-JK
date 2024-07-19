namespace TownOfUs.Roles.Modifiers
{
    public class Diseased : Modifier
    {
        public Diseased(PlayerControl player) : base(player)
        {
            Name = "Diseased";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Killing you gives Impostors a high cooldown" : "Zabicie ciebie daje Impostorom wiekszy cooldown";
            Color = Patches.Colors.Diseased;
            ModifierType = ModifierEnum.Diseased;
        }
    }
}