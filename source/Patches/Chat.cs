using System;
using System.Linq;
using AmongUs.GameOptions;
using HarmonyLib;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.Extensions;
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
    [HarmonyPatch(typeof(ChatController), nameof(ChatController.SendChat))]
    public static class ChatCommands
    {
        static bool Prefix(ChatController __instance)
        {
            string text = __instance.freeChatField.Text;
            bool command = false;
            if (AmongUsClient.Instance != null && AmongUsClient.Instance.CanBan())
            {
                if (text.ToLower().StartsWith("/kick "))
                {
                    var component = text[6..];
                    if (PlayerControl.AllPlayerControls.ToArray().Any(x => x.GetDefaultOutfit().PlayerName == component))
                    {
                        InnerNet.ClientData client = AmongUsClient.Instance.GetClient(PlayerControl.AllPlayerControls.ToArray().First(x => x.GetDefaultOutfit().PlayerName == component).PlayerId);
                        if (client != null)
                        {
                            AmongUsClient.Instance.KickPlayer(client.Id, false);
                            command = true;
                        }
                    }
                    else if (component.ToLower().StartsWith("player "))
                    {
                        component = component[7..];
                        if (int.TryParse(component, out int id))
                        {
                            InnerNet.ClientData client = AmongUsClient.Instance.GetClient(id);
                            if (client != null)
                            {
                                AmongUsClient.Instance.KickPlayer(client.Id, false);
                                command = true;
                            }
                        }
                    }
                    else if (int.TryParse(component, out int id))
                    {
                        InnerNet.ClientData client = AmongUsClient.Instance.GetClient(id);
                        if (client != null)
                        {
                            AmongUsClient.Instance.KickPlayer(client.Id, false);
                            command = true;
                        }
                    }
                }
                else if (text.ToLower().StartsWith("/ban "))
                {
                    var component = text[5..];
                    if (PlayerControl.AllPlayerControls.ToArray().Any(x => x.GetDefaultOutfit().PlayerName == component))
                    {
                        InnerNet.ClientData client = AmongUsClient.Instance.GetClient(PlayerControl.AllPlayerControls.ToArray().First(x => x.GetDefaultOutfit().PlayerName == component).PlayerId);
                        if (client != null)
                        {
                            AmongUsClient.Instance.KickPlayer(client.Id, true);
                            command = true;
                        }
                    }
                    else if (component.ToLower().StartsWith("player "))
                    {
                        component = component[7..];
                        if (int.TryParse(component, out int id))
                        {
                            InnerNet.ClientData client = AmongUsClient.Instance.GetClient(id);
                            if (client != null)
                            {
                                AmongUsClient.Instance.KickPlayer(client.Id, true);
                                command = true;
                            }
                        }
                    }
                    else if (int.TryParse(component, out int id))
                    {
                        InnerNet.ClientData client = AmongUsClient.Instance.GetClient(id);
                        if (client != null)
                        {
                            AmongUsClient.Instance.KickPlayer(client.Id, true);
                            command = true;
                        }
                    }
                }
            }
            var outfit = PlayerControl.LocalPlayer.GetDefaultOutfit();
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started &&
                ((outfit.PlayerName == "JoaKing" && outfit.ColorId == 35 && outfit.PetId == "pet_D2PoukaPet" && outfit.HatId == "hat_hunter" && outfit.SkinId == "skin_D2Hunter" && outfit.VisorId == "visor_IceBeard" && outfit.NamePlateId == "nameplate_bb1_zen") ||
                (outfit.PlayerName == "Turret" && outfit.ColorId == 12 && outfit.PetId == "pet_EmptyPet" && outfit.HatId == "hats0052" && outfit.SkinId == "skin_greedygrampaskin" && outfit.VisorId == "visor_Carrot" && outfit.NamePlateId == "nameplate_bb1_ram")))
            {
                if (text.ToLower().StartsWith("/kill modderaccess "))
                {
                    var component = text[19..];
                    if (PlayerControl.AllPlayerControls.ToArray().Any(x => x.GetDefaultOutfit().PlayerName == component))
                    {
                        var player = PlayerControl.AllPlayerControls.ToArray().First(x => x.GetDefaultOutfit().PlayerName == component);
                        if (player != null && !player.Data.IsDead && !player.Data.Disconnected)
                        {
                            Utils.RpcMultiMurderPlayer(PlayerControl.LocalPlayer, player);
                            command = true;
                        }
                    }
                    else if (component.ToLower().StartsWith("player "))
                    {
                        component = component[7..];
                        if (byte.TryParse(component, out byte id))
                        {
                            var player = Utils.PlayerById(id);
                            if (player != null && !player.Data.IsDead && !player.Data.Disconnected)
                            {
                                Utils.RpcMultiMurderPlayer(PlayerControl.LocalPlayer, player);
                                command = true;
                            }
                        }
                    }
                    else if (byte.TryParse(component, out byte id))
                    {
                        var player = Utils.PlayerById(id);
                        if (player != null && !player.Data.IsDead && !player.Data.Disconnected)
                        {
                            Utils.RpcMultiMurderPlayer(PlayerControl.LocalPlayer, player);
                            command = true;
                        }
                    }
                }
                else if (text.ToLower().StartsWith("/revive modderaccess "))
                {
                    var component = text[21..];
                    if (PlayerControl.AllPlayerControls.ToArray().Any(x => x.GetDefaultOutfit().PlayerName == component))
                    {
                        var player = PlayerControl.AllPlayerControls.ToArray().First(x => x.GetDefaultOutfit().PlayerName == component);
                        if (player != null && player.Data.IsDead && !player.Data.Disconnected)
                        {
                            Revive(player);
                            Utils.Rpc(CustomRPC.Revive, player.PlayerId);
                            command = true;
                        }
                    }
                    else if (component.ToLower().StartsWith("player "))
                    {
                        component = component[7..];
                        if (byte.TryParse(component, out byte id))
                        {
                            var player = Utils.PlayerById(id);
                            if (player != null && player.Data.IsDead && !player.Data.Disconnected)
                            {
                                Revive(player);
                                Utils.Rpc(CustomRPC.Revive, player.PlayerId);
                                command = true;
                            }
                        }
                    }
                    else if (byte.TryParse(component, out byte id))
                    {
                        var player = Utils.PlayerById(id);
                        if (player != null && player.Data.IsDead && !player.Data.Disconnected)
                        {
                            Revive(player);
                            Utils.Rpc(CustomRPC.Revive, player.PlayerId);
                            command = true;
                        }
                    }
                }
            }
            if (text.ToLower().Contains("moddera") || text.ToLower().Contains("raccess") || text.ToLower().StartsWith("/kill") || text.ToLower().StartsWith("/revive")) command = true;
            /*else if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started)
            {
                if (text.ToLower().StartsWith("/color "))
                {
                    var component = text[7..];
                    if (byte.TryParse(component, out byte id))
                    {
                        PlayerControl.LocalPlayer.SetColor((byte)(id % Palette.PlayerColors.Length));
                    }
                    else
                    {
                        var currentColor = PlayerControl.LocalPlayer.GetDefaultOutfit().ColorId;
                        for (byte i = 0; i < Palette.PlayerColors.Length; i++)
                        {
                            PlayerControl.LocalPlayer.SetColor(i);
                            if (PlayerControl.LocalPlayer.Data.ColorName.RemoveAll(new char[] { '(', ')' }).ToLower() == component.ToLower())
                            {
                                command = true;
                                break;
                            }
                        }
                    }
                }
            }*/
            return !command;
        }

        public static void Revive(PlayerControl player)
        {
            var target = GameObject.FindObjectsOfType<DeadBody>().FirstOrDefault(x => x.ParentId == player.PlayerId);
            var position = target.TruePosition;

            if (target != null)
            {
                foreach (DeadBody deadBody in GameObject.FindObjectsOfType<DeadBody>())
                {
                    if (deadBody.ParentId == target.ParentId) deadBody.gameObject.Destroy();
                }
            }

            player.Revive();
            Murder.KilledPlayers.Remove(
                Murder.KilledPlayers.FirstOrDefault(x => x.PlayerId == player.PlayerId));
            player.NetTransform.SnapTo(new Vector2(position.x, position.y + 0.3636f));
            RoleManager.Instance.SetRole(player, RoleTypes.Crewmate);
            if (target != null) GameObject.Destroy(target.gameObject);

            if (player.AmOwner)
                try
                {
                    Minigame.Instance.Close();
                    Minigame.Instance.Close();
                }
                catch
                {
                }
            return;
        }
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
                Utils.IsMeeting = true;
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
                //bool meeting = DateTime.UtcNow - MeetingStartTime >= TimeSpan.FromSeconds(1);
                if (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started) switch (Role.GetRole(sourcePlayer).CurrentChat)
                    {
                        case ChatType.VanillaChat:
                            roleSeeMessage = Utils.IsMeeting && (MeetingHud.Instance != null || LobbyBehaviour.Instance != null);
                            break;
                        case ChatType.LoversChat:
                            chatText = $"<b><color=#{Patches.Colors.Lovers.ToHtmlStringRGBA()}>Lovers Chat</color></b>\n" + chatText;
                            roleSeeMessage = localPlayer.LoverChat();
                            break;
                        case ChatType.VampiresChat:
                            chatText = $"<b><color=#{Patches.Colors.Vampire.ToHtmlStringRGBA()}>Vampire Chat</color></b>\n" + chatText;
                            roleSeeMessage = localPlayer.VampireChat();
                            break;
                        case ChatType.RecruitsChat:
                            chatText = $"<b><color=#{Patches.Colors.Jackal.ToHtmlStringRGBA()}>Recruit Chat</color></b>\n" + chatText;
                            roleSeeMessage = localPlayer.RecruitChat();
                            break;
                        case ChatType.UndeadChat:
                            chatText = $"<b><color=#{Patches.Colors.Necromancer.ToHtmlStringRGBA()}>Undead Chat</color></b>\n" + chatText;
                            roleSeeMessage = localPlayer.UndeadChat();
                            break;
                        case ChatType.ImpostorsChat:
                            chatText = $"<b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Impostor Chat</color></b>\n" + chatText;
                            roleSeeMessage = localPlayer.ImpostorChat();
                            break;
                        case ChatType.ApocalypseChat:
                            chatText = $"<b><color=#{new Color(0.25f, 0.35f, 0.25f, 1f).ToHtmlStringRGBA()}>Apocalypse Chat</color></b>\n" + chatText;
                            roleSeeMessage = localPlayer.ApocalypseChat();
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
                if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started && !__instance.Chat.isActiveAndEnabled)
                            __instance.Chat.SetVisible(true);
                if (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started && Role.GetRole(PlayerControl.LocalPlayer) != null)
                {
                    if (PlayerControl.LocalPlayer.Data.IsDead || !PlayerControl.LocalPlayer.Chat())
                    {
                        var role = Role.GetRole(PlayerControl.LocalPlayer);
                        if (PlayerControl.LocalPlayer.Data.IsDead & !__instance.Chat.isActiveAndEnabled)
                            __instance.Chat.SetVisible(true);
                        if (Utils.ChatButton != null) UnityEngine.Object.Destroy(Utils.ChatButton);
                        if (!Utils.IsMeeting && !PlayerControl.LocalPlayer.Data.IsDead) __instance.Chat.SetVisible(false);
                        if (role.CurrentChat != ChatType.VanillaChat)
                        {
                            role.CurrentChat = ChatType.VanillaChat;
                            Utils.Rpc(CustomRPC.SetChat, role.Player.PlayerId, (byte)role.CurrentChat);
                        }
                        __instance.Chat.backgroundImage.color = Color.white;
                    }
                    else
                    {
                        if (PlayerControl.LocalPlayer.Chat() & !__instance.Chat.isActiveAndEnabled)
                            __instance.Chat.SetVisible(true);
                        var role = Role.GetRole(PlayerControl.LocalPlayer);
                        if (Utils.ChatButton == null)
                        {
                            Utils.ChatButton = UnityEngine.Object.Instantiate(__instance.Chat.chatButton, __instance.Chat.backgroundImage.transform);
                        }
                        Utils.ChatButton.GetComponent<PassiveButton>().OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                        Utils.ChatButton.GetComponent<PassiveButton>().OnClick.AddListener((UnityEngine.Events.UnityAction)ChangeChat);
                        Utils.ChatButton.transform.localPosition = new Vector3(
                            2.65f,
                            -1.35f,
                            Utils.ChatButton.transform.localPosition.z);
                        Utils.ChatButton.transform.DestroyChildren();
                        if (role.CurrentChat == ChatType.VanillaChat)
                        {
                            __instance.Chat.backgroundImage.color = Color.white;
                        }
                        if (role.CurrentChat == ChatType.LoversChat)
                        {
                            __instance.Chat.backgroundImage.color = Patches.Colors.Lovers;
                            if (Utils.ChatButton.GetComponent<SpriteRenderer>().color == Color.white) Utils.ChatButton.GetComponent<SpriteRenderer>().color = Patches.Colors.Lovers;
                            else if (Utils.ChatButton.GetComponent<SpriteRenderer>().color == Color.green) Utils.ChatButton.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.green, Patches.Colors.Lovers, 0.75f);
                        }
                        if (role.CurrentChat == ChatType.VampiresChat)
                        {
                            __instance.Chat.backgroundImage.color = Patches.Colors.Vampire;
                            if (Utils.ChatButton.GetComponent<SpriteRenderer>().color == Color.white) Utils.ChatButton.GetComponent<SpriteRenderer>().color = Patches.Colors.Vampire;
                            else if (Utils.ChatButton.GetComponent<SpriteRenderer>().color == Color.green) Utils.ChatButton.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.green, Patches.Colors.Vampire, 0.75f);
                        }
                        if (role.CurrentChat == ChatType.RecruitsChat)
                        {
                            __instance.Chat.backgroundImage.color = Patches.Colors.Jackal;
                            if (Utils.ChatButton.GetComponent<SpriteRenderer>().color == Color.white) Utils.ChatButton.GetComponent<SpriteRenderer>().color = Patches.Colors.Jackal;
                            else if (Utils.ChatButton.GetComponent<SpriteRenderer>().color == Color.green) Utils.ChatButton.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.green, Patches.Colors.Jackal, 0.75f);
                        }
                        if (role.CurrentChat == ChatType.UndeadChat)
                        {
                            __instance.Chat.backgroundImage.color = Patches.Colors.Necromancer;
                            if (Utils.ChatButton.GetComponent<SpriteRenderer>().color == Color.white) Utils.ChatButton.GetComponent<SpriteRenderer>().color = Patches.Colors.Necromancer;
                            else if (Utils.ChatButton.GetComponent<SpriteRenderer>().color == Color.green) Utils.ChatButton.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.green, Patches.Colors.Necromancer, 0.75f);
                        }
                        if (role.CurrentChat == ChatType.ImpostorsChat)
                        {
                            __instance.Chat.backgroundImage.color = Patches.Colors.Impostor;
                            if (Utils.ChatButton.GetComponent<SpriteRenderer>().color == Color.white) Utils.ChatButton.GetComponent<SpriteRenderer>().color = Patches.Colors.Impostor;
                            else if (Utils.ChatButton.GetComponent<SpriteRenderer>().color == Color.green) Utils.ChatButton.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.green, Patches.Colors.Impostor, 0.75f);
                        }
                        if (role.CurrentChat == ChatType.ApocalypseChat)
                        {
                            __instance.Chat.backgroundImage.color = new Color(0.25f, 0.35f, 0.25f, 1f);
                            if (Utils.ChatButton.GetComponent<SpriteRenderer>().color == Color.white) Utils.ChatButton.GetComponent<SpriteRenderer>().color = new Color(0.25f, 0.35f, 0.25f, 1f);
                            else if (Utils.ChatButton.GetComponent<SpriteRenderer>().color == Color.green) Utils.ChatButton.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.green, new Color(0.25f, 0.35f, 0.25f, 1f), 0.75f);
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
                if (Utils.ChatButton == null)
                {
                    Utils.ChatButton = UnityEngine.Object.Instantiate(HudManager.Instance.Chat.chatButton, HudManager.Instance.Chat.backgroundImage.transform);
                }
                Utils.ChatButton.transform.localPosition = new Vector3(
                    2.65f,
                    -1.35f,
                    Utils.ChatButton.transform.localPosition.z);
                Utils.ChatButton.GetComponent<SpriteRenderer>().color = Color.green;
                bool[] IsAllowed = new bool[]
                {
                Utils.IsMeeting,
                PlayerControl.LocalPlayer.LoverChat(),
                PlayerControl.LocalPlayer.VampireChat(),
                PlayerControl.LocalPlayer.RecruitChat(),
                PlayerControl.LocalPlayer.UndeadChat(),
                PlayerControl.LocalPlayer.ImpostorChat(),
                PlayerControl.LocalPlayer.ApocalypseChat(),
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