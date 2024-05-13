using System;
using HarmonyLib;
using Reactor.Utilities;
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
    public static class Chat
    {
        private static DateTime MeetingStartTime = DateTime.MinValue;

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
        public class MeetingStart
        {
            public static void Prefix(MeetingHud __instance)
            {
                MeetingStartTime = DateTime.UtcNow;
            }
        }
        [HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))]
        public static class AddChat
        {
            public static bool Prefix(ChatController __instance, [HarmonyArgument(0)] PlayerControl sourcePlayer, [HarmonyArgument(1)] string chatText)
            {
                if (__instance != HudManager.Instance.Chat) return true;
                var localPlayer = PlayerControl.LocalPlayer;
                if (localPlayer == null) return true;
                Boolean shouldSeeMessage = localPlayer.Data.IsDead || localPlayer.IsLover() ||
                    sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId;
                if (DateTime.UtcNow - MeetingStartTime < TimeSpan.FromSeconds(1))
                {
                    return shouldSeeMessage;
                }
                return MeetingHud.Instance != null || LobbyBehaviour.Instance != null || shouldSeeMessage;
            }
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public static class EnableChat
        {
            public static void Postfix(HudManager __instance)
            {
                if (PlayerControl.LocalPlayer.LoverChat() & !__instance.Chat.isActiveAndEnabled)
                    __instance.Chat.SetVisible(true);
                /*var role = Role.GetRole(PlayerControl.LocalPlayer);
                if (!role.ChatControllers.ContainsKey(ChatType.VanillaChat))
                {
                    role.ChatControllers.Add(ChatType.VanillaChat, __instance.Chat);
                }
                if (!role.ChatControllers.ContainsKey(ChatType.LoversChat))
                {
                    var chat = UnityEngine.Object.Instantiate(__instance.Chat);
                    chat.backgroundImage.color = Patches.Colors.Lovers;
                    role.ChatControllers.Add(ChatType.LoversChat, chat);
                }
                if (!role.ChatControllers.ContainsKey(ChatType.VampiresChat))
                {
                    var chat = UnityEngine.Object.Instantiate(__instance.Chat);
                    chat.backgroundImage.color = Patches.Colors.Vampire;
                    role.ChatControllers.Add(ChatType.VampiresChat, chat);
                }
                if (!role.ChatControllers.ContainsKey(ChatType.RecruitsChat))
                {
                    var chat = UnityEngine.Object.Instantiate(__instance.Chat);
                    chat.backgroundImage.color = Patches.Colors.Jackal;
                    role.ChatControllers.Add(ChatType.RecruitsChat, chat);
                }
                if (!role.ChatControllers.ContainsKey(ChatType.UndeadChat))
                {
                    var chat = UnityEngine.Object.Instantiate(__instance.Chat);
                    chat.backgroundImage.color = Patches.Colors.Necromancer;
                    role.ChatControllers.Add(ChatType.UndeadChat, chat);
                }
                if (!role.ChatControllers.ContainsKey(ChatType.ImpostorsChat))
                {
                    var chat = UnityEngine.Object.Instantiate(__instance.Chat);
                    chat.backgroundImage.color = Patches.Colors.Impostor;
                    role.ChatControllers.Add(ChatType.ImpostorsChat, chat);
                }
                if (!role.ChatControllers.ContainsKey(ChatType.ApocalypseChat))
                {
                    var chat = UnityEngine.Object.Instantiate(__instance.Chat);
                    chat.backgroundImage.color = new Color(0.25f, 0.25f, 0.25f, 1f);
                    role.ChatControllers.Add(ChatType.ApocalypseChat, chat);
                }

                if (PlayerControl.LocalPlayer.LoverChat())
                {
                    if (role.ChatControllers[ChatType.LoversChat].chatButton.GetComponent<SpriteRenderer>().color == Color.white) __instance.Chat.chatButton.GetComponent<SpriteRenderer>().color = Patches.Colors.Lovers;
                    else if (role.ChatControllers[ChatType.LoversChat].chatButton.GetComponent<SpriteRenderer>().color == Color.green) __instance.Chat.chatButton.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.green, Patches.Colors.Lovers, 0.75f);
                    role.ChatControllers[ChatType.LoversChat].SetVisible(true);
                }
                else
                {
                    role.ChatControllers[ChatType.LoversChat].SetVisible(false);
                }
                if (PlayerControl.LocalPlayer.VampireChat())
                {
                    if (role.ChatControllers[ChatType.VampiresChat].chatButton.GetComponent<SpriteRenderer>().color == Color.white) __instance.Chat.chatButton.GetComponent<SpriteRenderer>().color = Patches.Colors.Vampire;
                    else if (role.ChatControllers[ChatType.VampiresChat].chatButton.GetComponent<SpriteRenderer>().color == Color.green) __instance.Chat.chatButton.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.green, Patches.Colors.Vampire, 0.75f);
                    role.ChatControllers[ChatType.VampiresChat].SetVisible(true);
                }
                else
                {
                    role.ChatControllers[ChatType.VampiresChat].SetVisible(false);
                }
                if (PlayerControl.LocalPlayer.RecruitChat())
                {
                    if (role.ChatControllers[ChatType.RecruitsChat].chatButton.GetComponent<SpriteRenderer>().color == Color.white) __instance.Chat.chatButton.GetComponent<SpriteRenderer>().color = Patches.Colors.Jackal;
                    else if (role.ChatControllers[ChatType.RecruitsChat].chatButton.GetComponent<SpriteRenderer>().color == Color.green) __instance.Chat.chatButton.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.green, Patches.Colors.Jackal, 0.75f);
                    role.ChatControllers[ChatType.RecruitsChat].SetVisible(true);
                }
                else
                {
                    role.ChatControllers[ChatType.RecruitsChat].SetVisible(false);
                }
                if (PlayerControl.LocalPlayer.UndeadChat())
                {
                    if (role.ChatControllers[ChatType.UndeadChat].chatButton.GetComponent<SpriteRenderer>().color == Color.white) __instance.Chat.chatButton.GetComponent<SpriteRenderer>().color = Patches.Colors.Necromancer;
                    else if (role.ChatControllers[ChatType.UndeadChat].chatButton.GetComponent<SpriteRenderer>().color == Color.green) __instance.Chat.chatButton.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.green, Patches.Colors.Necromancer, 0.75f);
                    role.ChatControllers[ChatType.UndeadChat].SetVisible(true);
                }
                else
                {
                    role.ChatControllers[ChatType.UndeadChat].SetVisible(false);
                }
                if (PlayerControl.LocalPlayer.ImpostorChat())
                {
                    if (role.ChatControllers[ChatType.ImpostorsChat].chatButton.GetComponent<SpriteRenderer>().color == Color.white) __instance.Chat.chatButton.GetComponent<SpriteRenderer>().color = Patches.Colors.Impostor;
                    else if (role.ChatControllers[ChatType.ImpostorsChat].chatButton.GetComponent<SpriteRenderer>().color == Color.green) __instance.Chat.chatButton.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.green, Patches.Colors.Impostor, 0.75f);
                    role.ChatControllers[ChatType.ImpostorsChat].SetVisible(true);
                }
                else
                {
                    role.ChatControllers[ChatType.ImpostorsChat].SetVisible(false);
                }
                if (PlayerControl.LocalPlayer.ApocalypseChat())
                {
                    if (role.ChatControllers[ChatType.ApocalypseChat].chatButton.GetComponent<SpriteRenderer>().color == Color.white) __instance.Chat.chatButton.GetComponent<SpriteRenderer>().color = new Color(0.25f, 0.25f, 0.25f, 1f);
                    else if (role.ChatControllers[ChatType.ApocalypseChat].chatButton.GetComponent<SpriteRenderer>().color == Color.green) __instance.Chat.chatButton.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.green, new Color(0.25f, 0.25f, 0.25f, 1f), 0.75f);
                    role.ChatControllers[ChatType.ApocalypseChat].SetVisible(true);
                }
                else
                {
                    role.ChatControllers[ChatType.ApocalypseChat].SetVisible(false);
                }

                var pos = new Vector3(0f, 0f, 0f);
                foreach (var entry in role.ChatControllers)
                {
                    var id = entry.Key;
                    var chat = entry.Value;
                    if (chat.isActiveAndEnabled && id != ChatType.VanillaChat)
                    {
                        pos += new Vector3(-1f, 0f, 0f);
                        chat.transform.position = pos;
                    }
                }*/
            }
        }
    }
}