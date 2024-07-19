namespace TownOfUs.Roles.Cultist
{
    public class CultistMystic : Role
    {
        public CultistMystic(PlayerControl player) : base(player)
        {
            Name = "Mystic";
            ImpostorText = () => "Understand When Someone Gets Converted";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Know when someone gets converted" : "Wiedz gdy ktos zostanie przekonwertowany";
            Color = Patches.Colors.Mystic;
            RoleType = RoleEnum.CultistMystic;
            AddToRoleHistory(RoleType);
        }
    }
}