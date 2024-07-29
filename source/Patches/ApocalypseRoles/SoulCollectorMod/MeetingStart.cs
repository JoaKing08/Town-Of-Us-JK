using HarmonyLib;
using TownOfUs.Roles;
using System;
using System.Linq;
using TownOfUs.CrewmateRoles.OracleMod;
using Reactor.Utilities.Extensions;
using TownOfUs.Roles.Horseman;

namespace TownOfUs.ApocalypseRoles.SoulCollectorMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingStart
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.SoulCollector)) return;
            var role = Role.GetRole<SoulCollector>(PlayerControl.LocalPlayer);
            if (PlayerControl.AllPlayerControls.ToArray().Count(x => (x.Is(Faction.NeutralKilling) || x.Is(Faction.Impostors) || x.Is(Faction.NeutralApocalypse)) && !x.Is(RoleEnum.SoulCollector) && !x.Is(RoleEnum.Baker) && !x.Is(RoleEnum.Famine) && !x.Is(RoleEnum.Arsonist) && !x.Is(RoleEnum.Plaguebearer) && !x.Data.Disconnected && !x.Data.IsDead) == 0)
            {
                role.ReapedSouls += 1;
                DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, Patches.TranslationPatches.CurrentLanguage == 0 ? $"Because you know there will be no more bodies, you collect soul from underground." : "Poniewaz wiesz ze nie bedzie wiecej cial, zabierasz dusze z podziemia.");
            }
            if (DestroyableSingleton<HudManager>.Instance && CustomGameOptions.SoulsNeeded > role.ReapedSouls)
            {
                DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, Patches.TranslationPatches.CurrentLanguage == 0 ? $"<b>{CustomGameOptions.SoulsNeeded - role.ReapedSouls}</b> more souls to reap remaining." : $"Pozostalo <b>{CustomGameOptions.SoulsNeeded - role.ReapedSouls}</b> dusz do zebrania.");
                if (!Utils.UndercoverIsApocalypse()) Utils.Rpc(CustomRPC.SendChatInfo, (byte)RoleEnum.SoulCollector, role.Player.PlayerId, (byte)(CustomGameOptions.SoulsNeeded - role.ReapedSouls));
            }
        }
    }
}
