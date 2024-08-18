using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace TownOfUs.ApocalypseRoles.HarbingerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class RepickHarbinger
    {
        private static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (PlayerControl.LocalPlayer != SetHarbinger.WillBeHarbinger) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(Faction.Crewmates))
            {
                var toChooseFromAlive = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.NeutralApocalypse) && !x.Is(ObjectiveEnum.Lover)).ToList();
                if (toChooseFromAlive.Count == 0)
                {
                    SetHarbinger.WillBeHarbinger = null;

                    Utils.Rpc(CustomRPC.SetHarbinger, byte.MaxValue);
                }
                else
                {
                    var rand2 = Random.RandomRangeInt(0, toChooseFromAlive.Count);
                    var pc2 = toChooseFromAlive[rand2];

                    SetHarbinger.WillBeHarbinger = pc2;

                    Utils.Rpc(CustomRPC.SetHarbinger, pc2.PlayerId);
                }
                return;
            }
            var toChooseFrom = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.NeutralApocalypse) && !x.Is(ObjectiveEnum.Lover) && x.Data.IsDead && !x.Data.Disconnected).ToList();
            if (toChooseFrom.Count == 0) return;
            var rand = Random.RandomRangeInt(0, toChooseFrom.Count);
            var pc = toChooseFrom[rand];

            SetHarbinger.WillBeHarbinger = pc;

            Utils.Rpc(CustomRPC.SetHarbinger, pc.PlayerId);
            return;
        }
    }
}