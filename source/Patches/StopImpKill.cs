using HarmonyLib;
using TownOfUs.Extensions;
using AmongUs.GameOptions;
using TownOfUs.Modifiers.UnderdogMod;
using TownOfUs.Roles;
using UnityEngine;
using Reactor.Utilities;
using System.Linq;

namespace TownOfUs
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class StopImpKill
    {
        [HarmonyPriority(Priority.First)]
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            if (!PlayerControl.LocalPlayer.Data.IsImpostor()) return true;
            var target = __instance.currentTarget;
            if (target == null) return true;
            if (target.Is(ObjectiveEnum.ImpostorAgent) || ((target.Data.IsImpostor() || target.Is(RoleEnum.Undercover)) && Utils.UndercoverIsImpostor()) && !Utils.CheckImpostorFriendlyFire()) return true;
            if (!__instance.isActiveAndEnabled || __instance.isCoolingDown) return true;
            if (Role.GetRole(PlayerControl.LocalPlayer).Roleblocked)
            {
                Coroutines.Start(Utils.FlashCoroutine(Color.white));
                return false;
            }
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek)
            {
                if (!target.inVent) Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, target);
                return false;
            }
            if (target.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, target.PlayerId, (byte)RoleEnum.Impostor, (byte)0);
            var interact = Utils.Interact(PlayerControl.LocalPlayer, target, true);
            if (interact[4] == true)
            {
                PlayerControl.LocalPlayer.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown * (Utils.PoltergeistTasks() ? CustomGameOptions.PoltergeistKCdMult : 1f));
                return false;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Warlock))
            {
                var warlock = Role.GetRole<Warlock>(PlayerControl.LocalPlayer);
                if (warlock.Charging)
                {
                    warlock.UsingCharge = true;
                    warlock.ChargeUseDuration = warlock.ChargePercent * CustomGameOptions.ChargeUseDuration / 100f;
                    if (warlock.ChargeUseDuration == 0f) warlock.ChargeUseDuration += 0.01f;
                }
                PlayerControl.LocalPlayer.SetKillTimer(0.01f);
            }
            else if (interact[0] == true)
            {
                if (PlayerControl.LocalPlayer.Is(ModifierEnum.Underdog))
                {
                    var lowerKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown - CustomGameOptions.UnderdogKillBonus;
                    var normalKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
                    var upperKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + CustomGameOptions.UnderdogKillBonus;
                    PlayerControl.LocalPlayer.SetKillTimer((PerformKill.LastImp() ? lowerKC : (PerformKill.IncreasedKC() ? normalKC : upperKC)) * (Utils.PoltergeistTasks() ? CustomGameOptions.PoltergeistKCdMult : 1f));
                }
                else PlayerControl.LocalPlayer.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown * (Utils.PoltergeistTasks() ? CustomGameOptions.PoltergeistKCdMult : 1f));
                return false;
            }
            else if (interact[1] == true)
            {
                PlayerControl.LocalPlayer.SetKillTimer(CustomGameOptions.ProtectKCReset + 0.01f);
                return false;
            }
            else if (interact[2] == true)
            {
                PlayerControl.LocalPlayer.SetKillTimer(CustomGameOptions.VestKCReset + 0.01f);
                return false;
            }
            else if (interact[3] == true)
            {
                PlayerControl.LocalPlayer.SetKillTimer(0.01f);
                return false;
            }
            return false;
        }
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public class HudManagerUpdate
        {

            public static void Postfix(HudManager __instance)
            {
                if (PlayerControl.AllPlayerControls.Count <= 1) return;
                if (PlayerControl.LocalPlayer == null) return;
                if (PlayerControl.LocalPlayer.Data == null) return;
                if (!PlayerControl.LocalPlayer.Data.IsImpostor()) return;

                //if (PlayerControl.LocalPlayer.IsControled()) Utils.Rpc(CustomRPC.ControlCooldown, (byte)PlayerControl.LocalPlayer.killTimer, (byte)GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);

                var notImpostor = PlayerControl.AllPlayerControls.ToArray().Where(
                    player => !player.Is(ObjectiveEnum.ImpostorAgent) && !(((player.Data.IsImpostor() || player.Is(RoleEnum.Undercover)) && Utils.UndercoverIsImpostor()) && !Utils.CheckImpostorFriendlyFire())
                ).ToList();
                var target = new PlayerControl();

                Utils.SetTarget(ref target, __instance.KillButton, GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance], notImpostor);
                __instance.KillButton.SetTarget(target);
            }
        }
    }
}