using System;

namespace TownOfUs.Roles.Modifiers
{
    public class ImpostorAgent : Modifier
    {
        public ImpostorAgent(PlayerControl player) : base(player)
        {
            Name = "Agent (Imp)";
            TaskText = () => "Use your <color=#CFFFFFFF>Crewmate</color> role to benefit <color=#FF0000FF>Impostors</color>.";
            Color = Patches.Colors.ImpostorAgent;
            ModifierType = ModifierEnum.ImpostorAgent;
        }
    }
    public class ApocalypseAgent : Modifier
    {
        public ApocalypseAgent(PlayerControl player) : base(player)
        {
            Name = "Agent (Apoc)";
            TaskText = () => "Use your <color=#CFFFFFFF>Crewmate</color> role to benefit <color=#808080FF>Apocalypse</color>.";
            Color = Patches.Colors.ApocalypseAgent;
            ModifierType = ModifierEnum.ApocalypseAgent;
        }
    }
}