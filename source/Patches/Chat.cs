using System;
using HarmonyLib;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs
{
    public enum ChatType
    {
        VanillaChat,
        LoversChat,
        VampiresChat,
        RecruitsChat,
        UndeadChat,
        ImpostorsChat,
        ApocalypseChat
    }
    public static class ChatPatches
    {
        private static DateTime MeetingStartTime = DateTime.MinValue;

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
        public class MeetingStart
        {
            public static void Prefix(MeetingHud __instance)
            {
                MeetingStartTime = DateTime.UtcNow;
                CustomGameData.IsMeeting = true;
            }
        }
        [HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))]
        public static class AddChat
        {
            public static bool Prefix(ChatController __instance, [HarmonyArgument(0)] PlayerControl sourcePlayer, [HarmonyArgument(1)] ref string chatText)
            {
                if (__instance != HudManager.Instance.Chat) return true;
                var localPlayer = PlayerControl.LocalPlayer;
                if (localPlayer == null) return true;
                bool roleSeeMessage = false;
                bool meeting = DateTime.UtcNow - MeetingStartTime >= TimeSpan.FromSeconds(1);
                if (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started && !chatText.Contains('<')) switch (Role.GetRole(sourcePlayer).CurrentChat)
                    {
                        case ChatType.VanillaChat:
                            roleSeeMessage = meeting && (MeetingHud.Instance != null || LobbyBehaviour.Instance != null);
                            break;
                        case ChatType.LoversChat:
                            chatText = $"<b><color=#{Patches.Colors.Lovers.ToHtmlStringRGBA()}>Lovers Chat</color></b>\n" + chatText;
                            roleSeeMessage = localPlayer.LoverChat(meeting);
                            break;
                        case ChatType.VampiresChat:
                            chatText = $"<b><color=#{Patches.Colors.Vampire.ToHtmlStringRGBA()}>Vampire Chat</color></b>\n" + chatText;
                            roleSeeMessage = localPlayer.VampireChat(meeting);
                            break;
                        case ChatType.RecruitsChat:
                            chatText = $"<b><color=#{Patches.Colors.Jackal.ToHtmlStringRGBA()}>Recruit Chat</color></b>\n" + chatText;
                            roleSeeMessage = localPlayer.RecruitChat(meeting);
                            break;
                        case ChatType.UndeadChat:
                            chatText = $"<b><color=#{Patches.Colors.Necromancer.ToHtmlStringRGBA()}>Undead Chat</color></b>\n" + chatText;
                            roleSeeMessage = localPlayer.UndeadChat(meeting);
                            break;
                        case ChatType.ImpostorsChat:
                            chatText = $"<b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Impostor Chat</color></b>\n" + chatText;
                            roleSeeMessage = localPlayer.ImpostorChat(meeting);
                            break;
                        case ChatType.ApocalypseChat:
                            chatText = $"<b><color=#{new Color(0.25f, 0.35f, 0.25f, 1f).ToHtmlStringRGBA()}>Apocalypse Chat</color></b>\n" + chatText;
                            roleSeeMessage = localPlayer.ApocalypseChat(meeting);
                            break;
                    }
                else roleSeeMessage = MeetingHud.Instance != null || LobbyBehaviour.Instance != null;
                bool shouldSeeMessage = localPlayer.Data.IsDead || roleSeeMessage ||
                    sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId;
                /*sourcePlayer.Data.DefaultOutfit.PlayerName = "???";
                sourcePlayer.Data.DefaultOutfit.ColorId = 15;
                sourcePlayer.Data.DefaultOutfit.PetId = "";
                sourcePlayer.Data.DefaultOutfit.HatId = "";
                sourcePlayer.Data.DefaultOutfit.SkinId = "";
                sourcePlayer.Data.DefaultOutfit.VisorId = "";
                sourcePlayer.Data.DefaultOutfit.NamePlateId = "";
                sourcePlayer.Data.PlayerLevel = 0;*/
                return shouldSeeMessage;
            }
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public static class EnableChat
        {
            public static void Postfix(HudManager __instance)
            {
                if (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started && Role.GetRole(PlayerControl.LocalPlayer) != null)
                {
                    if (PlayerControl.LocalPlayer.Data.IsDead || !PlayerControl.LocalPlayer.Chat())
                    {
                        var role = Role.GetRole(PlayerControl.LocalPlayer);
                        if (PlayerControl.LocalPlayer.Data.IsDead & !__instance.Chat.isActiveAndEnabled)
                            __instance.Chat.SetVisible(true);
                        if (role.ChatButton != null) UnityEngine.Object.Destroy(role.ChatButton);
                        if (!CustomGameData.IsMeeting && !PlayerControl.LocalPlayer.Data.IsDead) __instance.Chat.SetVisible(false);
                        role.CurrentChat = ChatType.VanillaChat;
                    }
                    else
                    {
                        if (PlayerControl.LocalPlayer.Chat() & !__instance.Chat.isActiveAndEnabled)
                            __instance.Chat.SetVisible(true);
                        var role = Role.GetRole(PlayerControl.LocalPlayer);
                        if (role.ChatButton == null)
                        {
                            role.ChatButton = UnityEngine.Object.Instantiate(__instance.Chat.chatButton, __instance.Chat.backgroundImage.transform);
                            role.ChatButton.GetComponent<PassiveButton>().OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                            role.ChatButton.GetComponent<PassiveButton>().OnClick.AddListener((UnityEngine.Events.UnityAction)ChangeChat);
                        }
                        role.ChatButton.transform.localPosition = new Vector3(
                            2.65f,
                            -1.35f,
                            role.ChatButton.transform.localPosition.z);
                        if (role.CurrentChat == ChatType.VanillaChat)
                        {
                            __instance.Chat.backgroundImage.color = Color.white;
                        }
                        if (role.CurrentChat == ChatType.LoversChat)
                        {
                            __instance.Chat.backgroundImage.color = Patches.Colors.Lovers;
                            if (role.ChatButton.GetComponent<SpriteRenderer>().color == Color.white) role.ChatButton.GetComponent<SpriteRenderer>().color = Patches.Colors.Lovers;
                            else if (role.ChatButton.GetComponent<SpriteRenderer>().color == Color.green) role.ChatButton.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.green, Patches.Colors.Lovers, 0.75f);
                        }
                        if (role.CurrentChat == ChatType.VampiresChat)
                        {
                            __instance.Chat.backgroundImage.color = Patches.Colors.Vampire;
                            if (role.ChatButton.GetComponent<SpriteRenderer>().color == Color.white) role.ChatButton.GetComponent<SpriteRenderer>().color = Patches.Colors.Vampire;
                            else if (role.ChatButton.GetComponent<SpriteRenderer>().color == Color.green) role.ChatButton.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.green, Patches.Colors.Vampire, 0.75f);
                        }
                        if (role.CurrentChat == ChatType.RecruitsChat)
                        {
                            __instance.Chat.backgroundImage.color = Patches.Colors.Jackal;
                            if (role.ChatButton.GetComponent<SpriteRenderer>().color == Color.white) role.ChatButton.GetComponent<SpriteRenderer>().color = Patches.Colors.Jackal;
                            else if (role.ChatButton.GetComponent<SpriteRenderer>().color == Color.green) role.ChatButton.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.green, Patches.Colors.Jackal, 0.75f);
                        }
                        if (role.CurrentChat == ChatType.UndeadChat)
                        {
                            __instance.Chat.backgroundImage.color = Patches.Colors.Necromancer;
                            if (role.ChatButton.GetComponent<SpriteRenderer>().color == Color.white) role.ChatButton.GetComponent<SpriteRenderer>().color = Patches.Colors.Necromancer;
                            else if (role.ChatButton.GetComponent<SpriteRenderer>().color == Color.green) role.ChatButton.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.green, Patches.Colors.Necromancer, 0.75f);
                        }
                        if (role.CurrentChat == ChatType.ImpostorsChat)
                        {
                            __instance.Chat.backgroundImage.color = Patches.Colors.Impostor;
                            if (role.ChatButton.GetComponent<SpriteRenderer>().color == Color.white) role.ChatButton.GetComponent<SpriteRenderer>().color = Patches.Colors.Impostor;
                            else if (role.ChatButton.GetComponent<SpriteRenderer>().color == Color.green) role.ChatButton.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.green, Patches.Colors.Impostor, 0.75f);
                        }
                        if (role.CurrentChat == ChatType.ApocalypseChat)
                        {
                            __instance.Chat.backgroundImage.color = new Color(0.25f, 0.35f, 0.25f, 1f);
                            if (role.ChatButton.GetComponent<SpriteRenderer>().color == Color.white) role.ChatButton.GetComponent<SpriteRenderer>().color = new Color(0.25f, 0.35f, 0.25f, 1f);
                            else if (role.ChatButton.GetComponent<SpriteRenderer>().color == Color.green) role.ChatButton.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.green, new Color(0.25f, 0.35f, 0.25f, 1f), 0.75f);
                        }
                    }
                }
            }
        }
        public static void ChangeChat()
        {
            var role = Role.GetRole(PlayerControl.LocalPlayer);
            if (PlayerControl.LocalPlayer.Chat())
            {
                if (role.ChatButton == null)
                {
                    role.ChatButton = UnityEngine.Object.Instantiate(HudManager.Instance.Chat.chatButton, HudManager.Instance.Chat.backgroundImage.transform);
                    role.ChatButton.GetComponent<PassiveButton>().OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                    role.ChatButton.GetComponent<PassiveButton>().OnClick.AddListener((UnityEngine.Events.UnityAction)ChangeChat);
                }
                role.ChatButton.transform.localPosition = new Vector3(
                    2.65f,
                    -1.35f,
                    role.ChatButton.transform.localPosition.z);
                role.ChatButton.GetComponent<SpriteRenderer>().color = Color.green;
                var meeting = CustomGameData.IsMeeting;
                bool[] IsAllowed = new bool[]
                {
                meeting,
                PlayerControl.LocalPlayer.LoverChat(meeting),
                PlayerControl.LocalPlayer.VampireChat(meeting),
                PlayerControl.LocalPlayer.RecruitChat(meeting),
                PlayerControl.LocalPlayer.UndeadChat(meeting),
                PlayerControl.LocalPlayer.ImpostorChat(meeting),
                PlayerControl.LocalPlayer.ApocalypseChat(meeting),
                };
                switch (role.CurrentChat)
                {
                    case ChatType.VanillaChat:
                        if (IsAllowed[1])
                            role.CurrentChat = ChatType.LoversChat;
                        else if (IsAllowed[2])
                            role.CurrentChat = ChatType.VampiresChat;
                        else if (IsAllowed[3])
                            role.CurrentChat = ChatType.RecruitsChat;
                        else if (IsAllowed[4])
                            role.CurrentChat = ChatType.UndeadChat;
                        else if (IsAllowed[5])
                            role.CurrentChat = ChatType.ImpostorsChat;
                        else if (IsAllowed[6])
                            role.CurrentChat = ChatType.ApocalypseChat;
                        break;
                    case ChatType.LoversChat:
                        if (IsAllowed[2])
                            role.CurrentChat = ChatType.VampiresChat;
                        else if (IsAllowed[3])
                            role.CurrentChat = ChatType.RecruitsChat;
                        else if (IsAllowed[4])
                            role.CurrentChat = ChatType.UndeadChat;
                        else if (IsAllowed[5])
                            role.CurrentChat = ChatType.ImpostorsChat;
                        else if (IsAllowed[6])
                            role.CurrentChat = ChatType.ApocalypseChat;
                        else if (IsAllowed[0])
                            role.CurrentChat = ChatType.VanillaChat;
                        break;
                    case ChatType.VampiresChat:
                        if (IsAllowed[3])
                            role.CurrentChat = ChatType.RecruitsChat;
                        else if (IsAllowed[4])
                            role.CurrentChat = ChatType.UndeadChat;
                        else if (IsAllowed[5])
                            role.CurrentChat = ChatType.ImpostorsChat;
                        else if (IsAllowed[6])
                            role.CurrentChat = ChatType.ApocalypseChat;
                        else if (IsAllowed[0])
                            role.CurrentChat = ChatType.VanillaChat;
                        else if (IsAllowed[1])
                            role.CurrentChat = ChatType.LoversChat;
                        break;
                    case ChatType.RecruitsChat:
                        if (IsAllowed[4])
                            role.CurrentChat = ChatType.UndeadChat;
                        else if (IsAllowed[5])
                            role.CurrentChat = ChatType.ImpostorsChat;
                        else if (IsAllowed[6])
                            role.CurrentChat = ChatType.ApocalypseChat;
                        else if (IsAllowed[0])
                            role.CurrentChat = ChatType.VanillaChat;
                        else if (IsAllowed[1])
                            role.CurrentChat = ChatType.LoversChat;
                        else if (IsAllowed[2])
                            role.CurrentChat = ChatType.VampiresChat;
                        break;
                    case ChatType.UndeadChat:
                        if (IsAllowed[5])
                            role.CurrentChat = ChatType.ImpostorsChat;
                        else if (IsAllowed[6])
                            role.CurrentChat = ChatType.ApocalypseChat;
                        else if (IsAllowed[0])
                            role.CurrentChat = ChatType.VanillaChat;
                        else if (IsAllowed[1])
                            role.CurrentChat = ChatType.LoversChat;
                        else if (IsAllowed[2])
                            role.CurrentChat = ChatType.VampiresChat;
                        else if (IsAllowed[3])
                            role.CurrentChat = ChatType.RecruitsChat;
                        break;
                    case ChatType.ImpostorsChat:
                        if (IsAllowed[6])
                            role.CurrentChat = ChatType.ApocalypseChat;
                        else if (IsAllowed[0])
                            role.CurrentChat = ChatType.VanillaChat;
                        else if (IsAllowed[1])
                            role.CurrentChat = ChatType.LoversChat;
                        else if (IsAllowed[2])
                            role.CurrentChat = ChatType.VampiresChat;
                        else if (IsAllowed[3])
                            role.CurrentChat = ChatType.RecruitsChat;
                        else if (IsAllowed[4])
                            role.CurrentChat = ChatType.UndeadChat;
                        break;
                    case ChatType.ApocalypseChat:
                        if (IsAllowed[0])
                            role.CurrentChat = ChatType.VanillaChat;
                        else if (IsAllowed[1])
                            role.CurrentChat = ChatType.LoversChat;
                        else if (IsAllowed[2])
                            role.CurrentChat = ChatType.VampiresChat;
                        else if (IsAllowed[3])
                            role.CurrentChat = ChatType.RecruitsChat;
                        else if (IsAllowed[4])
                            role.CurrentChat = ChatType.UndeadChat;
                        else if (IsAllowed[5])
                            role.CurrentChat = ChatType.ImpostorsChat;
                        break;
                }
                Utils.Rpc(CustomRPC.SetChat, role.Player.PlayerId, (byte)role.CurrentChat);
            }
            else
            {
                role.CurrentChat = ChatType.VanillaChat;
                Utils.Rpc(CustomRPC.SetChat, role.Player.PlayerId, (byte)role.CurrentChat);
            }
        }
    }
}