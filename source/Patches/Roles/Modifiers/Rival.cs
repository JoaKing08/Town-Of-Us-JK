using System.Collections.Generic;
using System.Linq;
using TownOfUs.Patches;
using UnityEngine;
using TownOfUs.Extensions;

namespace TownOfUs.Roles.Modifiers
{
    public class Rival : Objective
    {
        public Rival(PlayerControl player) : base(player)
        {
            Name = "Rival";
            SymbolName = "♠";
            TaskText = () =>
                "You are the rivals with " + OtherRival.Player.GetDefaultOutfit().PlayerName;
            Color = Colors.Rival;
            ObjectiveType = ObjectiveEnum.Rival;
        }

        public Rival OtherRival { get; set; }
        public int Num { get; set; }

        public override List<PlayerControl> GetTeammates()
        {
            var cooperatorTeam = new List<PlayerControl>
            {
                PlayerControl.LocalPlayer,
                OtherRival.Player
            };
            return cooperatorTeam;
        }

        public static void Gen(List<PlayerControl> canHaveModifiers)
        {
            List<PlayerControl> crewmates = new List<PlayerControl>();

            foreach (var player in canHaveModifiers)
            {
                if (player.Is(Faction.Crewmates))
                    crewmates.Add(player);
            }

            if (crewmates.Count < 2) return;

            var num = Random.RandomRangeInt(0, crewmates.Count);
            var firstRival = crewmates[num];
            canHaveModifiers.Remove(firstRival);

            PlayerControl secondRival;

            var num3 = Random.RandomRangeInt(0, crewmates.Count);
            while (num3 == num)
            {
                num3 = Random.RandomRangeInt(0, crewmates.Count);
            }
            secondRival = crewmates[num3];
            canHaveModifiers.Remove(secondRival);

            Utils.Rpc(CustomRPC.SetCouple, firstRival.PlayerId, secondRival.PlayerId, (byte)ObjectiveEnum.Rival);
            var rival1 = new Rival(firstRival);
            var rival2 = new Rival(secondRival);

            rival1.OtherRival = rival2;
            rival2.OtherRival = rival1;
        }
    }
}