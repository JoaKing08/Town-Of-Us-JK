using TMPro;

namespace TownOfUs.Roles
{
    public class Engineer : Role
    {
        public Engineer(PlayerControl player) : base(player)
        {
            Name = "Engineer";
            ImpostorText = () => "Maintain Important Systems On The Ship";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? (CustomGameOptions.GameMode == GameMode.Cultist ? "Vent around" : "Vent around and fix sabotages") : (CustomGameOptions.GameMode == GameMode.Cultist ? "Wentuj dookola" : "Wentuj dookola i naprawiaj sabotaze");
            Color = Patches.Colors.Engineer;
            RoleType = RoleEnum.Engineer;
            AddToRoleHistory(RoleType);
            UsesLeft = CustomGameOptions.MaxFixes;
        }

        public int UsesLeft;
        public TextMeshPro UsesText;

        public bool ButtonUsable => UsesLeft != 0;
    }
}