using AmongUs.GameOptions;
using HarmonyLib;
using Reactor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs
{
    public class SpectatorPatch
    {
        public static KillButton SpectateButton = null;
        public static KillButton NextButton = null;
        public static KillButton ObserveButton = null;
        public static KillButton BackButton = null;
        public static List<byte> Spectators = new();
        public static byte SpectatedPlayer = byte.MaxValue;
        public static PlayerControl SpectateTarget = null;
        public static void SpectateNext(bool backwards = false)
        {
            if (backwards)
            {
                if (SpectatedPlayer == byte.MaxValue || !PlayerControl.AllPlayerControls.ToArray().Any(x => !x.IsSpectator() && x.PlayerId < SpectatedPlayer))
                {
                    for (int i = 127; i >= 0; i--)
                    {
                        var player = Utils.PlayerById((byte)i);
                        if (player != null && !player.IsSpectator())
                        {
                            SpectatedPlayer = (byte)i;
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = SpectatedPlayer - 1; i >= 0; i--)
                    {
                        var player = Utils.PlayerById((byte)i);
                        if (player != null && !player.IsSpectator())
                        {
                            SpectatedPlayer = (byte)i;
                            break;
                        }
                    }
                }
            }
            else
            {
                if (SpectatedPlayer == byte.MaxValue || !PlayerControl.AllPlayerControls.ToArray().Any(x => !x.IsSpectator() && x.PlayerId > SpectatedPlayer))
                {
                    for (int i = 0; i < 128; i++)
                    {
                        var player = Utils.PlayerById((byte)i);
                        if (player != null && !player.IsSpectator())
                        {
                            SpectatedPlayer = (byte)i;
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = SpectatedPlayer + 1; i < 128; i++)
                    {
                        var player = Utils.PlayerById((byte)i);
                        if (player != null && !player.IsSpectator())
                        {
                            SpectatedPlayer = (byte)i;
                            break;
                        }
                    }
                }
            }
        }
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public class HudUpdate
        {
            public static void Postfix(HudManager __instance)
            {
                if (SpectateButton == null)
                {
                    SpectateButton = GameObject.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    SpectateButton.graphic.enabled = true;
                    SpectateButton.gameObject.SetActive(false);
                }

                SpectateButton.graphic.sprite = TownOfUs.HauntSprite;
                SpectateButton.transform.localPosition = new Vector3(-1f, 0f, 0f);
                SpectateButton.buttonLabelText.gameObject.SetActive(true);
                SpectateButton.buttonLabelText.text = SpectateTarget == null ? (PlayerControl.LocalPlayer.IsSpectator() ? "Unspectate" : "Spactate") : (SpectateTarget.IsSpectator() ? "Unassign Spectator" : "Assign Spactator");
                SpectateButton.gameObject.SetActive(AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started
                    && AmongUsClient.Instance.AmHost);
                if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started
                    && AmongUsClient.Instance.AmHost)
                {
                    Utils.SetTarget(ref SpectateTarget, SpectateButton, GameOptionsData.KillDistances[0]);
                }
                SpectateButton.graphic.color = Palette.EnabledColor;
                SpectateButton.graphic.material.SetFloat("_Desat", 0f);
                SpectateButton.buttonLabelText.color = Palette.EnabledColor;
                SpectateButton.buttonLabelText.material.SetFloat("_Desat", 0f);
                SpectateButton.SetCoolDown(0f, 1f);

                if (NextButton == null)
                {
                    NextButton = GameObject.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    NextButton.graphic.enabled = true;
                    NextButton.gameObject.SetActive(false);
                }

                NextButton.graphic.sprite = TownOfUs.Arrow;
                NextButton.transform.localPosition = new Vector3(0f, 0f, 0f);
                NextButton.gameObject.SetActive(AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started
                    && !MeetingHud.Instance && PlayerControl.LocalPlayer.IsSpectator() && SpectatedPlayer != byte.MaxValue);
                NextButton.graphic.color = Palette.EnabledColor;
                NextButton.graphic.material.SetFloat("_Desat", 0f);
                NextButton.SetCoolDown(0f, 1f);

                if (ObserveButton == null)
                {
                    ObserveButton = GameObject.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    ObserveButton.graphic.enabled = true;
                    ObserveButton.gameObject.SetActive(false);
                }

                ObserveButton.graphic.sprite = SpectatedPlayer == byte.MaxValue ? TownOfUs.HauntSprite : TownOfUs.X;
                ObserveButton.transform.localPosition = SpectatedPlayer == byte.MaxValue ? new Vector3(0f, 0f, 0f) : new Vector3(-1f, 0f, 0f);
                ObserveButton.buttonLabelText.gameObject.SetActive(true);
                ObserveButton.buttonLabelText.text = SpectatedPlayer == byte.MaxValue ? "Spectate" : "";
                ObserveButton.gameObject.SetActive(AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started
                    && !MeetingHud.Instance && PlayerControl.LocalPlayer.IsSpectator());
                ObserveButton.graphic.color = Palette.EnabledColor;
                ObserveButton.graphic.material.SetFloat("_Desat", 0f);
                ObserveButton.buttonLabelText.color = Palette.EnabledColor;
                ObserveButton.buttonLabelText.material.SetFloat("_Desat", 0f);
                ObserveButton.SetCoolDown(0f, 1f);

                if (BackButton == null)
                {
                    BackButton = GameObject.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    BackButton.graphic.enabled = true;
                    BackButton.gameObject.SetActive(false);
                }

                BackButton.graphic.sprite = TownOfUs.InvertedArrow;
                BackButton.transform.localPosition = new Vector3(-2f, 0f, 0f);
                BackButton.gameObject.SetActive(AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started
                    && !MeetingHud.Instance && PlayerControl.LocalPlayer.IsSpectator() && SpectatedPlayer != byte.MaxValue);
                BackButton.graphic.color = Palette.EnabledColor;
                BackButton.graphic.material.SetFloat("_Desat", 0f);
                BackButton.SetCoolDown(0f, 1f);

                if (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started && PlayerControl.LocalPlayer.IsSpectator())
                {
                    __instance.ReportButton.canInteract = false;
                    __instance.ReportButton.gameObject.SetActive(false);
                    __instance.UseButton.canInteract = false;
                    __instance.UseButton.gameObject.SetActive(false);
                    __instance.UseButton.transform.localPosition = new Vector3(-100, 0f, 0f);
                    __instance.PetButton.canInteract = false;
                    __instance.PetButton.gameObject.SetActive(false);
                    __instance.AbilityButton.canInteract = false;
                    __instance.AbilityButton.gameObject.SetActive(false);
                }
                else
                {
                    __instance.UseButton.transform.localPosition = new Vector3(0, 0f, 0f);
                }
                foreach (var playerId in Spectators)
                {
                    var player = Utils.PlayerById(playerId);
                    var data = player?.Data;
                    if (data != null && AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started)
                    {
                        player.nameText().color = Color.gray;
                    }
                }
                if (PlayerControl.LocalPlayer.IsSpectator() && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started)
                {
                    var camera = GameObject.FindGameObjectWithTag("MainCamera");
                    var followerCamera = camera.GetComponent<FollowerCamera>();
                    if (SpectatedPlayer != byte.MaxValue)
                    {
                        var player = Utils.PlayerById(SpectatedPlayer);
                        followerCamera.Target = player;
                        if (!player.Data.IsDead)
                        {
                            foreach (var ghost in PlayerControl.AllPlayerControls.ToArray().Where(x => x.Data.IsDead))
                            {
                                ghost.Visible = false;
                            }
                        }
                        else
                        {
                            PlayerControl.LocalPlayer.Visible = false;
                        }
                    }
                    else
                    {
                        followerCamera.Target = PlayerControl.LocalPlayer;
                        foreach (var ghost in PlayerControl.AllPlayerControls.ToArray().Where(x => x.Data.IsDead && PlayerControl.LocalPlayer.Data.IsDead))
                        {
                            ghost.Visible = true;
                        }
                    }
                }
            }
        }
        [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
        public class ButtonClick
        {
            public static bool Prefix(KillButton __instance)
            {
                if (__instance == SpectateButton)
                {
                    if (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started || !AmongUsClient.Instance.AmHost) return false;
                    if (SpectateTarget == null)
                    {
                        if (PlayerControl.LocalPlayer.IsSpectator())
                        {
                            Spectators.Remove(PlayerControl.LocalPlayer.PlayerId);
                            Utils.Rpc(CustomRPC.AssignSpectator, PlayerControl.LocalPlayer.PlayerId, false);
                        }
                        else
                        {
                            Spectators.Add(PlayerControl.LocalPlayer.PlayerId);
                            Utils.Rpc(CustomRPC.AssignSpectator, PlayerControl.LocalPlayer.PlayerId, true);
                        }
                    }
                    else
                    {
                        if (SpectateTarget.IsSpectator())
                        {
                            Spectators.Remove(SpectateTarget.PlayerId);
                            Utils.Rpc(CustomRPC.AssignSpectator, SpectateTarget.PlayerId, false);
                        }
                        else
                        {
                            Spectators.Add(SpectateTarget.PlayerId);
                            Utils.Rpc(CustomRPC.AssignSpectator, SpectateTarget.PlayerId, true);
                        }
                    }
                }
                else if (__instance == ObserveButton)
                {
                    if (!(AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started
                    && !MeetingHud.Instance && PlayerControl.LocalPlayer.IsSpectator())) return false;
                    if (SpectatedPlayer == byte.MaxValue)
                    {
                        SpectateNext();
                    }
                    else
                    {
                        if (Utils.PlayerById(SpectatedPlayer) != null) PlayerControl.LocalPlayer.TeleportRpc(Utils.PlayerById(SpectatedPlayer).transform.position);
                        SpectatedPlayer = byte.MaxValue;
                    }
                }
                else if (__instance == NextButton)
                {
                    if (!(AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started
                    && !MeetingHud.Instance && PlayerControl.LocalPlayer.IsSpectator() && SpectatedPlayer != byte.MaxValue)) return false;
                    SpectateNext();
                }
                else if (__instance == BackButton)
                {
                    if (!(AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started
                    && !MeetingHud.Instance && PlayerControl.LocalPlayer.IsSpectator() && SpectatedPlayer != byte.MaxValue)) return false;
                    SpectateNext(true);
                }
                return true;
            }
        }
    }
}