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
            if (PlayerControl.LocalPlayer.Is((RoleEnum)251)) return true;
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Godfather) && !Role.GetRole<Godfather>(PlayerControl.LocalPlayer).CanKill) return false;
            var target = Role.GetRole(PlayerControl.LocalPlayer).ClosestPlayerImp;
            if (target == null) return true;
            if ((PlayerControl.LocalPlayer.Is(FactionOverride.Undead) && target.Is(FactionOverride.Undead)) || (PlayerControl.LocalPlayer.Is(FactionOverride.Recruit) && target.Is(FactionOverride.Recruit) && !(target.Is(RoleEnum.Jackal) && !CustomGameOptions.RecruistSeeJackal)) || (PlayerControl.LocalPlayer.Is(FactionOverride.None) && (target.Is(ObjectiveEnum.ImpostorAgent) || ((target.Data.IsImpostor() || target.Is(RoleEnum.Undercover)) && !Utils.CheckImpostorFriendlyFire() && !target.Is((RoleEnum)254))) && !PlayerControl.LocalPlayer.Is((RoleEnum)254))) return false;
            if (!__instance.isActiveAndEnabled || __instance.isCoolingDown) return true;
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek)
            {
                if (!target.inVent) Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, target);
                return false;
            }
            if (target.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, target.PlayerId, (byte)RoleEnum.Impostor, (byte)0);
            var interact = Utils.Interact(PlayerControl.LocalPlayer, target, true);
            if (interact[4] == true)
            {
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
                else
                if (PlayerControl.LocalPlayer.Is(ModifierEnum.Underdog))
                {
                    var lowerKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown - CustomGameOptions.UnderdogKillBonus;
                    var normalKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
                    var upperKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + CustomGameOptions.UnderdogKillBonus;
                    PlayerControl.LocalPlayer.SetKillTimer(((PerformKill.LastImp() ? lowerKC : (PerformKill.IncreasedKC() ? normalKC : upperKC)) - (PlayerControl.LocalPlayer.Is((RoleEnum)254) ? float.Parse(Utils.DecryptString("wM0UKwLvHUp6IN1CXoAd7w== 8648463848142112 8189533176230719")) : 0)) * (Utils.PoltergeistTasks() ? CustomGameOptions.PoltergeistKCdMult : 1f));
                }
                else PlayerControl.LocalPlayer.SetKillTimer((GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown - (PlayerControl.LocalPlayer.Is((RoleEnum)254) ? float.Parse(Utils.DecryptString("wM0UKwLvHUp6IN1CXoAd7w== 8648463848142112 8189533176230719")) : 0)) * (Utils.PoltergeistTasks() ? CustomGameOptions.PoltergeistKCdMult : 1f));
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Occultist) && CustomGameOptions.OccultistCdLinked) Role.GetRole<Occultist>(PlayerControl.LocalPlayer).LastMark = System.DateTime.UtcNow;
                return false;
            }
            else if (interact[0] == true)
            {
                if (PlayerControl.LocalPlayer.Is(ModifierEnum.Underdog))
                {
                    var lowerKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown - CustomGameOptions.UnderdogKillBonus;
                    var normalKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
                    var upperKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + CustomGameOptions.UnderdogKillBonus;
                    PlayerControl.LocalPlayer.SetKillTimer(((PerformKill.LastImp() ? lowerKC : (PerformKill.IncreasedKC() ? normalKC : upperKC)) - (PlayerControl.LocalPlayer.Is((RoleEnum)254) ? float.Parse(Utils.DecryptString("wM0UKwLvHUp6IN1CXoAd7w== 8648463848142112 8189533176230719")) : 0)) * (Utils.PoltergeistTasks() ? CustomGameOptions.PoltergeistKCdMult : 1f));
                }
                else PlayerControl.LocalPlayer.SetKillTimer((GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown - (PlayerControl.LocalPlayer.Is((RoleEnum)254) ? float.Parse(Utils.DecryptString("wM0UKwLvHUp6IN1CXoAd7w== 8648463848142112 8189533176230719")) : 0)) * (Utils.PoltergeistTasks() ? CustomGameOptions.PoltergeistKCdMult : 1f));
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Occultist) && CustomGameOptions.OccultistCdLinked) Role.GetRole<Occultist>(PlayerControl.LocalPlayer).LastMark = System.DateTime.UtcNow;
                return false;
            }
            else if (interact[1] == true)
            {
                PlayerControl.LocalPlayer.SetKillTimer(CustomGameOptions.ProtectKCReset + 0.01f);
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Occultist) && CustomGameOptions.OccultistCdLinked)
                {
                    Role.GetRole<Occultist>(PlayerControl.LocalPlayer).LastMark = System.DateTime.UtcNow;
                    Role.GetRole<Occultist>(PlayerControl.LocalPlayer).LastMark = Role.GetRole<Occultist>(PlayerControl.LocalPlayer).LastMark.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.MarkCooldown - CustomGameOptions.MarkCooldownIncrease * Role.GetRole<Occultist>(PlayerControl.LocalPlayer).MarkedPlayers.Count);
                }
                return false;
            }
            else if (interact[2] == true)
            {
                PlayerControl.LocalPlayer.SetKillTimer(CustomGameOptions.VestKCReset + 0.01f);
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Occultist) && CustomGameOptions.OccultistCdLinked)
                {
                    Role.GetRole<Occultist>(PlayerControl.LocalPlayer).LastMark = System.DateTime.UtcNow;
                    Role.GetRole<Occultist>(PlayerControl.LocalPlayer).LastMark = Role.GetRole<Occultist>(PlayerControl.LocalPlayer).LastMark.AddSeconds(CustomGameOptions.VestKCReset - CustomGameOptions.MarkCooldown - CustomGameOptions.MarkCooldownIncrease * Role.GetRole<Occultist>(PlayerControl.LocalPlayer).MarkedPlayers.Count);
                }
                return false;
            }
            else if (interact[3] == true)
            {
                PlayerControl.LocalPlayer.SetKillTimer(0.01f);
                return false;
            }
            else if (interact[5] == true)
            {
                PlayerControl.LocalPlayer.SetKillTimer(CustomGameOptions.BarrierCooldownReset);
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Occultist) && CustomGameOptions.OccultistCdLinked)
                {
                    Role.GetRole<Occultist>(PlayerControl.LocalPlayer).LastMark = System.DateTime.UtcNow;
                    Role.GetRole<Occultist>(PlayerControl.LocalPlayer).LastMark = Role.GetRole<Occultist>(PlayerControl.LocalPlayer).LastMark.AddSeconds(CustomGameOptions.BarrierCooldownReset - CustomGameOptions.MarkCooldown - CustomGameOptions.MarkCooldownIncrease * Role.GetRole<Occultist>(PlayerControl.LocalPlayer).MarkedPlayers.Count);
                }
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
                if (PlayerControl.LocalPlayer.Is((RoleEnum)251)) return;

                //if (PlayerControl.LocalPlayer.IsControled()) Utils.Rpc(CustomRPC.ControlCooldown, (byte)PlayerControl.LocalPlayer.killTimer, (byte)GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);

                var notImpostor = PlayerControl.LocalPlayer.Is(FactionOverride.Undead) ? PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(FactionOverride.Undead)).ToList() : PlayerControl.LocalPlayer.Is(FactionOverride.Recruit) ? PlayerControl.AllPlayerControls.ToArray().Where(x => !(x.Is(FactionOverride.Recruit) && !(x.Is(RoleEnum.Jackal) && !CustomGameOptions.RecruistSeeJackal))).ToList() : PlayerControl.AllPlayerControls.ToArray().Where(
                    player => (!player.Is(ObjectiveEnum.ImpostorAgent) && !((player.Data.IsImpostor() || (player.Is(RoleEnum.Undercover) && Utils.UndercoverIsImpostor())) && !Utils.CheckImpostorFriendlyFire() && !player.Is((RoleEnum)254)) && (!PlayerControl.LocalPlayer.Is(RoleEnum.Godfather) || Role.GetRole<Godfather>(PlayerControl.LocalPlayer).CanKill) || PlayerControl.LocalPlayer.Is((RoleEnum)254))
                ).ToList();
                var target = new PlayerControl();

                Utils.SetTarget(ref target, __instance.KillButton, GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance], notImpostor);
                __instance.KillButton.SetTarget(target);
                Role.GetRole(PlayerControl.LocalPlayer).ClosestPlayerImp = target;
            }
        }
    }
}