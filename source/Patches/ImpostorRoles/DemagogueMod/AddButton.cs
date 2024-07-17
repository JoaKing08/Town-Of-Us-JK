using System;
using System.Linq;
using HarmonyLib;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using TownOfUs.Extensions;
using TownOfUs.Modifiers.AssassinMod;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace TownOfUs.ImpostorRoles.DemagogueMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class AddShootButton
    {
        public static Sprite KillSprite => TownOfUs.SmallKillSprite;

        public static void GenButton(Demagogue role, int index, byte targetId)
        {
            PluginSingleton<TownOfUs>.Instance.Log.LogMessage("Creating Meeting Kill Button");
            var confirmButton = MeetingHud.Instance.playerStates[index].Buttons.transform.GetChild(0).gameObject;

            var newButton = Object.Instantiate(confirmButton, MeetingHud.Instance.playerStates[index].transform);
            var renderer = newButton.GetComponent<SpriteRenderer>();
            var passive = newButton.GetComponent<PassiveButton>();

            renderer.sprite = KillSprite;
            newButton.transform.position = confirmButton.transform.position - new Vector3(1.25f, 0f, 0f);
            newButton.transform.localScale *= 0.8f;
            newButton.layer = 5;
            newButton.transform.parent = confirmButton.transform.parent.parent;
            var newText = Object.Instantiate(HudManager.Instance.KillButton.cooldownTimerText, newButton.transform);
            newText.gameObject.SetActive(true);
            newText.transform.localPosition = new Vector3(-0.14f, 0f, newButton.transform.localPosition.z - 1f);
            newText.transform.localScale = newText.transform.localScale * 0.65f;
            newText.alignment = TMPro.TextAlignmentOptions.Right;
            newText.fontStyle = TMPro.FontStyles.Bold;
            newText.text = CustomGameOptions.ChargesForMeetingKill + "";
            newText.color = role.Charges >= CustomGameOptions.ChargesForMeetingKill ? Color.green : Color.red;

            passive.OnClick = new Button.ButtonClickedEvent();
            passive.OnClick.AddListener(MeetingKill(role, targetId));
            PluginSingleton<TownOfUs>.Instance.Log.LogMessage("Action:" + MeetingKill(role, targetId));
            role.MeetingKillButtons.Add(targetId, (newButton, newText));
            PluginSingleton<TownOfUs>.Instance.Log.LogMessage("Button Created");
        }


        private static Action MeetingKill(Demagogue role, byte targetId)
        {
            void Listener()
            {
                if (MeetingHud.Instance.state == MeetingHud.VoteStates.Discussion) return;
                if (role.Charges < CustomGameOptions.ChargesForMeetingKill) return;
                if (Role.GetRole(PlayerControl.LocalPlayer).Roleblocked)
                {
                    Coroutines.Start(Utils.FlashCoroutine(Color.white));
                    role.Notification(Patches.TranslationPatches.CurrentLanguage == 0 ? "You Are Roleblocked!" : "Twoja Rola Zostala Zablokowana!", 1000 * CustomGameOptions.NotificationDuration);
                    return;
                }
                role.Charges -= (byte)CustomGameOptions.ChargesForMeetingKill;
                Utils.Rpc(CustomRPC.DemagogueCharges, role.Charges, role.Player.PlayerId);
                AddVoteButton.UpdateButton(role, MeetingHud.Instance);
                var target = Utils.PlayerById(targetId);
                Coroutines.Start(Utils.FlashCoroutine(Patches.Colors.Impostor));
                Role.GetRole(PlayerControl.LocalPlayer).Notification(Patches.TranslationPatches.CurrentLanguage == 0 ? $"Demagogue Has Convicted {target.GetDefaultOutfit().PlayerName}!" : $"Demagogue Skazal {target.GetDefaultOutfit().PlayerName}", 1000 * CustomGameOptions.NotificationDuration);
                DemagogueKill.RpcMurderPlayer(target, PlayerControl.LocalPlayer);
            }

            return Listener;
        }

        public static void Postfix(MeetingHud __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Demagogue))
            {
                var demagogue = (Demagogue)role;
                demagogue.MeetingKillButtons.Clear();
            }

            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Demagogue)) return;
            var demagoguerole = Role.GetRole<Demagogue>(PlayerControl.LocalPlayer);
            for (var i = 0; i < __instance.playerStates.Length; i++)
                if (Utils.PlayerById(__instance.playerStates[i].TargetPlayerId) != null && PlayerControl.LocalPlayer.PlayerId != __instance.playerStates[i].TargetPlayerId && !Utils.PlayerById(__instance.playerStates[i].TargetPlayerId).Data.IsDead && !Utils.PlayerById(__instance.playerStates[i].TargetPlayerId).Data.Disconnected && !(PlayerControl.LocalPlayer.Is(FactionOverride.None) && (((Utils.PlayerById(__instance.playerStates[i].TargetPlayerId).Is(Faction.Impostors) || (Utils.PlayerById(__instance.playerStates[i].TargetPlayerId).Is(RoleEnum.Undercover) && Utils.UndercoverIsImpostor())) && !Utils.CheckImpostorFriendlyFire()) || Utils.PlayerById(__instance.playerStates[i].TargetPlayerId).Is(ObjectiveEnum.ImpostorAgent))) && !(PlayerControl.LocalPlayer.Is(FactionOverride.Undead) && Utils.PlayerById(__instance.playerStates[i].TargetPlayerId).Is(FactionOverride.Undead)) && !(PlayerControl.LocalPlayer.Is(FactionOverride.Recruit) && Utils.PlayerById(__instance.playerStates[i].TargetPlayerId).Is(FactionOverride.Recruit) && (!Utils.PlayerById(__instance.playerStates[i].TargetPlayerId).Is(RoleEnum.Jackal) || CustomGameOptions.RecruistSeeJackal)))
                {
                    GenButton(demagoguerole, i, __instance.playerStates[i].TargetPlayerId);
                }
        }
    }
}