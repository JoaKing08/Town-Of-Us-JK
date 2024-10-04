using System.Collections.Generic;
using System.Linq;
using TownOfUs.Patches;
using UnityEngine;
using TownOfUs.Extensions;

namespace TownOfUs.Roles.Modifiers
{
    public class Cooperator : Objective
    {
        public Cooperator(PlayerControl player) : base(player)
        {
            Name = "Cooperator";
            SymbolName = "$";
            TaskText = () =>
                "You are in collaboration with " + OtherCooperator.Player.GetDefaultOutfit().PlayerName;
            Color = Colors.Cooperator;
            ObjectiveType = ObjectiveEnum.Cooperator;
        }

        public Cooperator OtherCooperator { get; set; }
        public int Num { get; set; }

        public override List<PlayerControl> GetTeammates()
        {
            var cooperatorTeam = new List<PlayerControl>
            {
                PlayerControl.LocalPlayer,
                OtherCooperator.Player
            };
            return cooperatorTeam;
        }

        public static void Gen(List<PlayerControl> canHaveModifiers)
        {
            List<PlayerControl> crewmates = new List<PlayerControl>();
            List<PlayerControl> nonkilling = new List<PlayerControl>();
            List<PlayerControl> impostors = new List<PlayerControl>();

            foreach(var player in canHaveModifiers)
            {
                if (player.Is(Faction.Impostors) || player.Is(Faction.NeutralKilling)
                    || player.Is(Faction.NeutralApocalypse))
                    impostors.Add(player);
                else if (player.Is(Faction.NeutralBenign) || player.Is(Faction.NeutralEvil)
                    || player.Is(Faction.NeutralChaos))
                    nonkilling.Add(player);
                if (player.Is(Faction.NeutralBenign) || player.Is(Faction.NeutralEvil)
                    || player.Is(Faction.NeutralChaos) || player.Is(Faction.Crewmates))
                    crewmates.Add(player);
            }

            if (crewmates.Count < 1 || (nonkilling.Count < 2 && (impostors.Count < 1 || CustomGameOptions.KillingCooperatorPrecent == 0))) return;

            var num = Random.RandomRangeInt(0, crewmates.Count);
            var firstCooperator = crewmates[num];
            if (nonkilling.Contains(firstCooperator)) nonkilling.Remove(firstCooperator);
            canHaveModifiers.Remove(firstCooperator);

            var cooperatingimp = Random.RandomRangeInt(0, 100);

            PlayerControl secondCooperator;
            if ((CustomGameOptions.KillingCooperatorPrecent > cooperatingimp) && impostors.Count > 0)
            {
                var num3 = Random.RandomRangeInt(0, impostors.Count);
                secondCooperator = impostors[num3];
            }
            else
            {
                if (nonkilling.Count > 0)
                {
                    var num3 = Random.RandomRangeInt(0, nonkilling.Count);
                    secondCooperator = nonkilling[num3];
                }
                else
                {
                    var num3 = Random.RandomRangeInt(0, impostors.Count);
                    secondCooperator = impostors[num3];
                }
            }
            canHaveModifiers.Remove(secondCooperator);

            Utils.Rpc(CustomRPC.SetCouple, firstCooperator.PlayerId, secondCooperator.PlayerId, (byte)ObjectiveEnum.Cooperator);
            var cooperator1 = new Cooperator(firstCooperator);
            var cooperator2 = new Cooperator(secondCooperator);

            cooperator1.OtherCooperator = cooperator2;
            cooperator2.OtherCooperator = cooperator1;
        }
    }
}