namespace TownOfUs.Roles.Modifiers
{
    public class Famous : Modifier
    {
        public Famous(PlayerControl player) : base(player)
        {
            Name = "Famous";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Notice everyone that you died" : "Powiadom wszystkich ze zginales";
            Color = Patches.Colors.Famous;
            ModifierType = ModifierEnum.Famous;
        }
    }
}