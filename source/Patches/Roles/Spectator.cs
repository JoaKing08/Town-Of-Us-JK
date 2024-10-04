using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

namespace TownOfUs.Roles
{
    public class Spectator : Role
    {
        public Spectator(PlayerControl player) : base(player)
        {
            Name = "Spectator";
            ImpostorText = () => "You Are Spectating";
            TaskText = () => "You are spectating";
            Color = UnityEngine.Color.white;
            RoleType = RoleEnum.Spectator;
            Faction = Faction.Spectators;
        }
    }
}