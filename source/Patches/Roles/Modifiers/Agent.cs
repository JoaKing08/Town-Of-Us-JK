using System;

namespace TownOfUs.Roles.Modifiers
{
    public class ImpostorAgent : Objective
    {
        public ImpostorAgent(PlayerControl player) : base(player)
        {
            Name = "Agent (Imp)";
            SymbolName = "!";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Use your <color=#CFFFFFFF>Crewmate</color> role to benefit <color=#FF0000FF>Impostors</color>." : "Uzyj swojej roli <color=#CFFFFFFF>Crewmate</color> z korzyscia dla <color=#FF0000FF>Impostorów</color>.";
            Color = Patches.Colors.ImpostorAgent;
            ObjectiveType = ObjectiveEnum.ImpostorAgent;

        }
    }
    public class ApocalypseAgent : Objective
    {
        public ApocalypseAgent(PlayerControl player) : base(player)
        {
            Name = "Agent (Apoc)";
            SymbolName= "!";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Use your <color=#CFFFFFFF>Crewmate</color> role to benefit <color=#808080FF>Apocalypse</color>." : "Uzyj swojej roli <color=#CFFFFFFF>Crewmate</color> z korzyscia dla <color=#FF0000FF>Apocalypse</color>.";
            Color = Patches.Colors.ApocalypseAgent;
            ObjectiveType = ObjectiveEnum.ApocalypseAgent;
        }
    }
}