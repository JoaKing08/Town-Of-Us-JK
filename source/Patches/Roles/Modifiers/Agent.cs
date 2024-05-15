using System;

namespace TownOfUs.Roles.Modifiers
{
    public class ImpostorAgent : Objective
    {
        public ImpostorAgent(PlayerControl player) : base(player)
        {
            Name = "Agent (Imp)";
            SymbolName = "!";
            TaskText = () => "Use your <color=#CFFFFFFF>Crewmate</color> role to benefit <color=#FF0000FF>Impostors</color>.";
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
            TaskText = () => "Use your <color=#CFFFFFFF>Crewmate</color> role to benefit <color=#808080FF>Apocalypse</color>.";
            Color = Patches.Colors.ApocalypseAgent;
            ObjectiveType = ObjectiveEnum.ApocalypseAgent;
        }
    }
}