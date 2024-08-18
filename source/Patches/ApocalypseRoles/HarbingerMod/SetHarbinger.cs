using System;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using TownOfUs.Patches;
using TownOfUs.CrewmateRoles.AurialMod;
using TownOfUs.Patches.ScreenEffects;
using System.Linq;
using System.Collections.Generic;

namespace TownOfUs.ApocalypseRoles.HarbingerMod
{
    [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
    public static class AirshipExileController_WrapUpAndSpawn
    {
        public static void Postfix(AirshipExileController __instance) => SetHarbinger.ExileControllerPostfix(__instance);
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public class SetHarbinger
    {
        public static PlayerControl WillBeHarbinger;
        public static Vector2 StartPosition;

        public static void ExileControllerPostfix(ExileController __instance)
        {
            if (WillBeHarbinger == null) return;
            var exiled = __instance.exiled?.Object;
            if (!WillBeHarbinger.Data.IsDead && exiled.Is(Faction.NeutralApocalypse) && !exiled.IsLover()) WillBeHarbinger = exiled;
            if (WillBeHarbinger.Data.Disconnected) return;
            if (!WillBeHarbinger.Data.IsDead && WillBeHarbinger != exiled) return;

            if (!WillBeHarbinger.Is(RoleEnum.Harbinger))
            {
                var oldRole = Role.GetRole(WillBeHarbinger);
                var killsList = (oldRole.Kills, oldRole.CorrectKills, oldRole.IncorrectKills, oldRole.CorrectAssassinKills, oldRole.IncorrectAssassinKills);
                Role.RoleDictionary.Remove(WillBeHarbinger.PlayerId);
                if (PlayerControl.LocalPlayer == WillBeHarbinger)
                {
                    var role = new Harbinger(PlayerControl.LocalPlayer);
                    role.formerRole = oldRole.RoleType;
                    role.Kills = killsList.Kills;
                    role.CorrectKills = killsList.CorrectKills;
                    role.IncorrectKills = killsList.IncorrectKills;
                    role.CorrectAssassinKills = killsList.CorrectAssassinKills;
                    role.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
                    role.RegenTask();
                }
                else
                {
                    var role = new Harbinger(WillBeHarbinger);
                    role.formerRole = oldRole.RoleType;
                    role.Kills = killsList.Kills;
                    role.CorrectKills = killsList.CorrectKills;
                    role.IncorrectKills = killsList.IncorrectKills;
                    role.CorrectAssassinKills = killsList.CorrectAssassinKills;
                    role.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
                }

                Utils.RemoveTasks(WillBeHarbinger);
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Phantom) && !PlayerControl.LocalPlayer.Is(RoleEnum.Haunter) && !PlayerControl.LocalPlayer.Is(RoleEnum.Poltergeist)) WillBeHarbinger.MyPhysics.ResetMoveState();

                WillBeHarbinger.gameObject.layer = LayerMask.NameToLayer("Players");
            }

            WillBeHarbinger.gameObject.GetComponent<PassiveButton>().OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            WillBeHarbinger.gameObject.GetComponent<PassiveButton>().OnClick.AddListener((Action)(() => WillBeHarbinger.OnClick()));
            WillBeHarbinger.gameObject.GetComponent<BoxCollider2D>().enabled = true;

            if (PlayerControl.LocalPlayer != WillBeHarbinger) return;

            if (Role.GetRole<Harbinger>(PlayerControl.LocalPlayer).Caught) return;

            List<Vent> vents = new();
            var CleanVentTasks = PlayerControl.LocalPlayer.myTasks.ToArray().Where(x => x.TaskType == TaskTypes.VentCleaning).ToList();
            if (CleanVentTasks != null)
            {
                var ids = CleanVentTasks.Where(x => !x.IsComplete)
                                        .ToList()
                                        .ConvertAll(x => x.FindConsoles()[0].ConsoleId);

                vents = ShipStatus.Instance.AllVents.Where(x => !ids.Contains(x.Id)).ToList();
            }
            else vents = ShipStatus.Instance.AllVents.ToList();

            var startingVent = vents[Random.RandomRangeInt(0, vents.Count)];


            Utils.Rpc(CustomRPC.SetPos, PlayerControl.LocalPlayer.PlayerId, startingVent.transform.position.x, startingVent.transform.position.y + 0.3636f);
            var pos = new Vector2(startingVent.transform.position.x, startingVent.transform.position.y + 0.3636f);

            PlayerControl.LocalPlayer.transform.position = pos;
            PlayerControl.LocalPlayer.NetTransform.SnapTo(pos);
            PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(pos);
            PlayerControl.LocalPlayer.MyPhysics.RpcEnterVent(startingVent.Id);
        }

        public static void Postfix(ExileController __instance) => ExileControllerPostfix(__instance);

        [HarmonyPatch(typeof(Object), nameof(Object.Destroy), new Type[] { typeof(GameObject) })]
        public static void Prefix(GameObject obj)
        {
            if (!SubmergedCompatibility.Loaded || GameOptionsManager.Instance?.currentNormalGameOptions?.MapId != 6) return;
            if (obj.name?.Contains("ExileCutscene") == true) ExileControllerPostfix(ExileControllerPatch.lastExiled);
        }
    }
}