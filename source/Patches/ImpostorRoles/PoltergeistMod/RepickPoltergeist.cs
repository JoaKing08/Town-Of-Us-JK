using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.PoltergeistMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class RepickPoltergeist
    {
        private static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (PlayerControl.LocalPlayer != SetPoltergeist.WillBePoltergeist) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(Faction.Impostors))
            {
                var toChooseFromAlive = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Impostors) && !x.Is(ObjectiveEnum.Lover) && !x.Data.Disconnected).ToList();
                if (toChooseFromAlive.Count == 0)
                {
                    SetPoltergeist.WillBePoltergeist = null;

                    Utils.Rpc(CustomRPC.SetPoltergeist, byte.MaxValue);
                }
                else
                {
                    var rand2 = Random.RandomRangeInt(0, toChooseFromAlive.Count);
                    var pc2 = toChooseFromAlive[rand2];

                    SetPoltergeist.WillBePoltergeist = pc2;

                    Utils.Rpc(CustomRPC.SetPoltergeist, pc2.PlayerId);
                }
                return;
            }
            var toChooseFrom = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Impostors) && !x.Is(ObjectiveEnum.Lover) && x.Data.IsDead && !x.Data.Disconnected).ToList();
            if (toChooseFrom.Count == 0) return;
            var rand = Random.RandomRangeInt(0, toChooseFrom.Count);
            var pc = toChooseFrom[rand];

            SetPoltergeist.WillBePoltergeist = pc;

            Utils.Rpc(CustomRPC.SetPoltergeist, pc.PlayerId);
            return;
        }
    }
}