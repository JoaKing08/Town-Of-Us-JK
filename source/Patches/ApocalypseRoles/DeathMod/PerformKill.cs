using System;
using HarmonyLib;
using TownOfUs.Roles;
using AmongUs.GameOptions;
using TownOfUs.Roles.Horseman;
using UnityEngine;
using Reactor.Utilities;

namespace TownOfUs.ApocalypseRoles.DeathMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Death);
            if (!flag) return true;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            var role = Role.GetRole<Death>(PlayerControl.LocalPlayer);
            if (role.Player.inVent) return false;
            if (role.ApocalypseTimer() != 0) return false;
            if (Role.GetRole(PlayerControl.LocalPlayer).Roleblocked)
            {
                Coroutines.Start(Utils.FlashCoroutine(Color.white));
                role.Notification(Patches.TranslationPatches.CurrentLanguage == 0 ? "You Are Roleblocked!" : "Twoja Rola Zostala Zablokowana!", 1000 * CustomGameOptions.NotificationDuration);
                return false;
            }

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (role.FactionOverride == FactionOverride.None)
                {
                    if (!player.Is(Faction.NeutralApocalypse) && !player.Is(ObjectiveEnum.ApocalypseAgent))
                    {
                        Utils.RpcMultiMurderPlayer(PlayerControl.LocalPlayer, player);
                        Utils.Rpc(CustomRPC.KillAbilityUsed, player.PlayerId);
                    }
                }
                else
                {
                    if (!player.Is(role.FactionOverride))
                    {
                        Utils.RpcMultiMurderPlayer(PlayerControl.LocalPlayer, player);
                        Utils.Rpc(CustomRPC.KillAbilityUsed, player.PlayerId);
                    }
                }
            }
            return false;
        }
    }
}