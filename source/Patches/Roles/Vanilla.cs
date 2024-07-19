namespace TownOfUs.Roles
{
    public class Impostor : Role
    {
        public Impostor(PlayerControl player) : base(player)
        {
            Name = "Impostor";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Kill and sabotage" : "Zabijaj i sabotuj";
            ImpostorText = () => "Kill And Sabotage";
            Faction = Faction.Impostors;
            RoleType = RoleEnum.Impostor;
            AddToRoleHistory(RoleType);
            Color = Palette.ImpostorRed;
        }
    }

    public class Crewmate : Role
    {
        public Crewmate(PlayerControl player) : base(player)
        {
            Name = "Crewmate";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Do your tasks" : "Rób swoje zadania";
            ImpostorText = () => "Do Your Tasks";
            Faction = Faction.Crewmates;
            RoleType = RoleEnum.Crewmate;
            AddToRoleHistory(RoleType);
            Color = Patches.Colors.Crewmate;
        }
    }
}