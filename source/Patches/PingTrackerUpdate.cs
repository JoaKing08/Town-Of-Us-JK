using HarmonyLib;
using UnityEngine;

namespace TownOfUs
{
    //[HarmonyPriority(Priority.VeryHigh)] // to show this message first, or be overrided if any plugins do
    [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
    public static class PingTracker_Update
    {

        [HarmonyPostfix]
        public static void Postfix(PingTracker __instance)
        {
            var position = __instance.GetComponent<AspectPosition>();
            position.DistanceFromEdge = new Vector3(3.6f, 0.1f, 0);
            position.AdjustPosition();
            var host = GameData.Instance.GetHost();

            __instance.text.text =
                "<size=2><color=#00FF00FF>TownOfUs v" + TownOfUs.VersionString + "</color>, <color=#FFD000FF>JoaKing's addon v" + TownOfUs.ModesVersionString + "</color>\n" +
                $"Ping: {AmongUsClient.Instance.Ping}ms\n" +
                (!MeetingHud.Instance
                    ? "<color=#00FF00FF>Modded By: Donners &</color>\n" +
                    "<color=#00FF00FF>MyDragonBreath</color>\n" : "") +
                (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started
                    ? "<color=#00FF00FF>Formerly: Slushiegoose & Polus.gg</color>\n" : "") +
                (!MeetingHud.Instance
                    ? "<color=#FFD000FF>JoaKing's addon by: JoaKing</color>\n" : "") +
                (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started
                    ? $"Host: {host.PlayerName}" : "") +
                    "</size>";
        }
    }
}
