using AmongUs.GameOptions;
using HarmonyLib;
using Reactor.Utilities;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using UnityEngine;

namespace TownOfUs
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.Start))]
    public static class KillButtonAwake
    {
        public static void Prefix(KillButton __instance)
        {
            __instance.transform.Find("Text_TMP").gameObject.SetActive(false);
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class KillButtonSprite
    {
        private static Sprite Fix => TownOfUs.EngineerFix;
        private static Sprite Medic => TownOfUs.MedicSprite;
        private static Sprite Seer => TownOfUs.SeerSprite;
        private static Sprite Douse => TownOfUs.DouseSprite;
        private static Sprite Revive => TownOfUs.ReviveSprite;
        private static Sprite Alert => TownOfUs.AlertSprite;
        private static Sprite Remember => TownOfUs.RememberSprite;
        private static Sprite Track => TownOfUs.TrackSprite;
        private static Sprite Transport => TownOfUs.TransportSprite;
        private static Sprite Mediate => TownOfUs.MediateSprite;
        private static Sprite Vest => TownOfUs.VestSprite;
        private static Sprite Protect => TownOfUs.ProtectSprite;
        private static Sprite Infect => TownOfUs.InfectSprite;
        private static Sprite Trap => TownOfUs.TrapSprite;
        private static Sprite Inspect => TownOfUs.InspectSprite;
        private static Sprite Swoop => TownOfUs.SwoopSprite;
        private static Sprite Observe => TownOfUs.ObserveSprite;
        private static Sprite Bite => TownOfUs.BiteSprite;
        private static Sprite Stake => TownOfUs.StakeSprite;
        private static Sprite Confess => TownOfUs.ConfessSprite;
        private static Sprite Radiate => TownOfUs.RadiateSprite;
        private static Sprite Bread => TownOfUs.BreadSprite;
        private static Sprite Starve => TownOfUs.StarveSprite;
        private static Sprite Reap => TownOfUs.ReapSprite;
        private static Sprite Apocalypse => TownOfUs.ApocalypseSprite;
        private static Sprite Duel => TownOfUs.DuelSprite;
        private static Sprite Knight => TownOfUs.KnightSprite;
        private static Sprite Inquire => TownOfUs.InquireSprite;
        private static Sprite Drink => TownOfUs.DrinkSprite;
        private static Sprite Bug => TownOfUs.BugSprite;
        private static Sprite Control => TownOfUs.ControlSprite;
        private static Sprite Order => TownOfUs.OrderSprite;
        private static Sprite SoulSwap => TownOfUs.SoulSwapSprite;
        private static Sprite Investigate => TownOfUs.InvestigateSprite;
        private static Sprite Watch => TownOfUs.WatchSprite;
        private static Sprite Revive2 => TownOfUs.Revive2Sprite;
        private static Sprite Nothing => TownOfUs.NothingSprite;

        private static Sprite Kill;


        public static void Postfix(HudManager __instance)
        {
            if (__instance.KillButton == null) return;

            if (!Kill) Kill = __instance.KillButton.graphic.sprite;

            var flag = false;
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer) || PlayerControl.LocalPlayer.Is(RoleEnum.CultistSeer))
            {
                __instance.KillButton.graphic.sprite = Seer;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Medic))
            {
                __instance.KillButton.graphic.sprite = Medic;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Arsonist))
            {
                __instance.KillButton.graphic.sprite = Douse;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Altruist))
            {
                __instance.KillButton.graphic.sprite = Revive;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Veteran))
            {
                __instance.KillButton.graphic.sprite = Alert;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Amnesiac))
            {
                __instance.KillButton.graphic.sprite = Remember;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Tracker))
            {
                __instance.KillButton.graphic.sprite = Track;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Transporter))
            {
                __instance.KillButton.graphic.sprite = Transport;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Medium))
            {
                __instance.KillButton.graphic.sprite = Mediate;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Survivor))
            {
                __instance.KillButton.graphic.sprite = Vest;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.GuardianAngel))
            {
                __instance.KillButton.graphic.sprite = Protect;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Plaguebearer))
            {
                __instance.KillButton.graphic.sprite = Infect;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Engineer) && CustomGameOptions.GameMode != GameMode.Cultist)
            {
                __instance.KillButton.graphic.sprite = Fix;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Trapper))
            {
                __instance.KillButton.graphic.sprite = Trap;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Detective))
            {
                __instance.KillButton.graphic.sprite = Inspect;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Chameleon))
            {
                __instance.KillButton.graphic.sprite = Swoop;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Doomsayer))
            {
                __instance.KillButton.graphic.sprite = Observe;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Vampire))
            {
                __instance.KillButton.graphic.sprite = Bite;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.VampireHunter))
            {
                __instance.KillButton.graphic.sprite = Stake;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Oracle))
            {
                __instance.KillButton.graphic.sprite = Confess;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Aurial))
            {
                __instance.KillButton.graphic.sprite = Radiate;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Baker))
            {
                __instance.KillButton.graphic.sprite = Bread;
                __instance.KillButton.buttonLabelText.gameObject.SetActive(true);
                __instance.KillButton.buttonLabelText.text = "Bread";
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Famine))
            {
                __instance.KillButton.graphic.sprite = Starve;
                __instance.KillButton.buttonLabelText.gameObject.SetActive(true);
                __instance.KillButton.buttonLabelText.text = "Starve";
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.SoulCollector))
            {
                __instance.KillButton.graphic.sprite = Reap;
                __instance.KillButton.buttonLabelText.gameObject.SetActive(true);
                __instance.KillButton.buttonLabelText.text = "Reap";
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Death))
            {
                __instance.KillButton.graphic.sprite = Apocalypse;
                __instance.KillButton.buttonLabelText.gameObject.SetActive(true);
                __instance.KillButton.buttonLabelText.text = "Apocalypse";
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Pirate))
            {
                __instance.KillButton.graphic.sprite = Duel;
                __instance.KillButton.buttonLabelText.gameObject.SetActive(true);
                __instance.KillButton.buttonLabelText.text = "Duel";
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Inspector))
            {
                __instance.KillButton.graphic.sprite = Inspect;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Monarch))
            {
                __instance.KillButton.graphic.sprite = Knight;
                __instance.KillButton.buttonLabelText.gameObject.SetActive(true);
                __instance.KillButton.buttonLabelText.text = "Knight";
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Inquisitor))
            {
                __instance.KillButton.graphic.sprite = Inquire;
                __instance.KillButton.buttonLabelText.gameObject.SetActive(true);
                __instance.KillButton.buttonLabelText.text = "Inquire";
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.TavernKeeper))
            {
                __instance.KillButton.graphic.sprite = Drink;
                __instance.KillButton.buttonLabelText.gameObject.SetActive(true);
                __instance.KillButton.buttonLabelText.text = "Drink";
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Spy) && CustomGameOptions.GameMode != GameMode.Cultist)
            {
                __instance.KillButton.graphic.sprite = Bug;
                __instance.KillButton.buttonLabelText.gameObject.SetActive(true);
                __instance.KillButton.buttonLabelText.text = "Bug";
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Witch))
            {
                __instance.KillButton.graphic.sprite = Role.GetRole<Witch>(PlayerControl.LocalPlayer).ControledPlayer == null ? Control : Order;
                __instance.KillButton.buttonLabelText.gameObject.SetActive(true);
                __instance.KillButton.buttonLabelText.text = Role.GetRole<Witch>(PlayerControl.LocalPlayer).ControledPlayer == null ? "Control" : "Order";
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.CursedSoul))
            {
                __instance.KillButton.graphic.sprite = SoulSwap;
                __instance.KillButton.buttonLabelText.gameObject.SetActive(true);
                __instance.KillButton.buttonLabelText.text = "Soul Swap";
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Investigator))
            {
                __instance.KillButton.graphic.sprite = Investigate;
                __instance.KillButton.buttonLabelText.gameObject.SetActive(true);
                __instance.KillButton.buttonLabelText.text = "Investigate";
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Lookout))
            {
                __instance.KillButton.graphic.sprite = Watch;
                __instance.KillButton.buttonLabelText.gameObject.SetActive(true);
                __instance.KillButton.buttonLabelText.text = "Watch";
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.JKNecromancer))
            {
                __instance.KillButton.graphic.sprite = Revive2;
                flag = true;
            }
            else
            {
                __instance.KillButton.graphic.sprite = Kill;
                __instance.KillButton.buttonLabelText.gameObject.SetActive(true);
                __instance.KillButton.buttonLabelText.text = "Kill";
                flag = PlayerControl.LocalPlayer.Is(RoleEnum.Sheriff) || PlayerControl.LocalPlayer.Is(RoleEnum.Pestilence) ||
                    PlayerControl.LocalPlayer.Is(RoleEnum.Werewolf) || PlayerControl.LocalPlayer.Is(RoleEnum.Juggernaut) ||
                    CustomGameOptions.GameMode == GameMode.Teams || PlayerControl.LocalPlayer.Is(RoleEnum.SoloKiller) ||
                    PlayerControl.LocalPlayer.Is(RoleEnum.Berserker) || PlayerControl.LocalPlayer.Is(RoleEnum.War) ||
                    PlayerControl.LocalPlayer.Is(RoleEnum.SerialKiller) || PlayerControl.LocalPlayer.Is(RoleEnum.Jackal);
            }
            if (!PlayerControl.LocalPlayer.Is(Faction.Impostors) &&
                GameOptionsManager.Instance.CurrentGameOptions.GameMode != GameModes.HideNSeek)
            {
                __instance.KillButton.transform.localPosition = new Vector3(0f, 1f, 0f);
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Engineer) || PlayerControl.LocalPlayer.Is(RoleEnum.Glitch)
                 || PlayerControl.LocalPlayer.Is(RoleEnum.Pestilence) || PlayerControl.LocalPlayer.Is(RoleEnum.Juggernaut)
                 || PlayerControl.LocalPlayer.Is(RoleEnum.Vampire) || PlayerControl.LocalPlayer.Is(RoleEnum.Berserker)
                 || PlayerControl.LocalPlayer.Is(RoleEnum.War) || PlayerControl.LocalPlayer.Is(RoleEnum.SerialKiller)
                 || PlayerControl.LocalPlayer.Is(RoleEnum.JKNecromancer) || PlayerControl.LocalPlayer.Is(RoleEnum.Jackal)
                 || PlayerControl.LocalPlayer.Is(RoleEnum.Baker) || PlayerControl.LocalPlayer.Is(RoleEnum.Famine)
                 || PlayerControl.LocalPlayer.Is(RoleEnum.SoulCollector) || PlayerControl.LocalPlayer.Is(RoleEnum.Death)
                 || PlayerControl.LocalPlayer.Is(RoleEnum.Plaguebearer))
            {
                __instance.ImpostorVentButton.transform.localPosition = new Vector3(-2f, 0f, 0f);
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Werewolf) || PlayerControl.LocalPlayer.Is(RoleEnum.SoloKiller))
            {
                __instance.ImpostorVentButton.transform.localPosition = new Vector3(-1f, 1f, 0f);
            }
            else if (CustomGameOptions.GameMode == GameMode.Teams)
            {
                __instance.ImpostorVentButton.transform.localPosition = new Vector3(-1f, 0f, 0f);
            }
            if (CustomGameOptions.GameMode == GameMode.Teams) __instance.ReportButton.transform.localPosition = new Vector3(100f, 100f, 0f);

            bool KillKey = Rewired.ReInput.players.GetPlayer(0).GetButtonDown("Kill");
            var controller = ConsoleJoystick.player.GetButtonDown(8);
            if ((KillKey || controller) && __instance.KillButton != null && flag && !PlayerControl.LocalPlayer.Data.IsDead)
                __instance.KillButton.DoClick();

            var role = Role.GetRole(PlayerControl.LocalPlayer);
            bool AbilityKey = Rewired.ReInput.players.GetPlayer(0).GetButtonDown("ToU imp/nk");
            if (role?.ExtraButtons != null && AbilityKey && !PlayerControl.LocalPlayer.Data.IsDead)
                role?.ExtraButtons[0]?.DoClick();

            if (Modifier.GetModifier<ButtonBarry>(PlayerControl.LocalPlayer)?.ButtonUsed == false &&
                Rewired.ReInput.players.GetPlayer(0).GetButtonDown("ToU bb/disperse/mimic") &&
                !PlayerControl.LocalPlayer.Data.IsDead)
            {
                Modifier.GetModifier<ButtonBarry>(PlayerControl.LocalPlayer).ButtonButton.DoClick();
            }
            else if (Modifier.GetModifier<Disperser>(PlayerControl.LocalPlayer)?.ButtonUsed == false &&
                     Rewired.ReInput.players.GetPlayer(0).GetButtonDown("ToU bb/disperse/mimic") &&
                     !PlayerControl.LocalPlayer.Data.IsDead)
            {
                Modifier.GetModifier<Disperser>(PlayerControl.LocalPlayer).DisperseButton.DoClick();
            }
        }

        [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.Update))]
        class AbilityButtonUpdatePatch
        {
            static void Postfix()
            {
                if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started)
                {
                    HudManager.Instance.AbilityButton.gameObject.SetActive(false);
                    return;
                }
                else if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek)
                {
                    HudManager.Instance.AbilityButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsImpostor());
                    return;
                }
                var ghostRole = false;
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Haunter))
                {
                    var haunter = Role.GetRole<Haunter>(PlayerControl.LocalPlayer);
                    if (!haunter.Caught) ghostRole = true;
                }
                else if (PlayerControl.LocalPlayer.Is(RoleEnum.Phantom))
                {
                    var phantom = Role.GetRole<Phantom>(PlayerControl.LocalPlayer);
                    if (!phantom.Caught) ghostRole = true;
                }
                HudManager.Instance.AbilityButton.gameObject.SetActive(!ghostRole && Utils.ShowDeadBodies && !MeetingHud.Instance);
            }
        }
    }
}
