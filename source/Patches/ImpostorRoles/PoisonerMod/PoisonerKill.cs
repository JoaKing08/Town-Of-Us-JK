﻿using System.Linq;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using UnityEngine;
using UnityEngine.UI;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.Modifiers.AssassinMod;
using TownOfUs.ImpostorRoles.BlackmailerMod;
using TownOfUs.Extensions;
using TownOfUs.NeutralRoles.DoomsayerMod;
using TownOfUs.CrewmateRoles.SwapperMod;
using TownOfUs.Patches;
using System;
using TownOfUs.CrewmateRoles.VigilanteMod;
using TownOfUs.CrewmateRoles.ImitatorMod;
using Reactor.Utilities.Extensions;
using Reactor.Utilities;

namespace TownOfUs.ImpostorRoles.PoisonerMod
{
    public class PoisonerKill
    {
        public static void RpcMurderPlayer(PlayerControl player, PlayerControl poisoner)
        {
            PlayerVoteArea voteArea = Utils.IsMeeting ? MeetingHud.Instance.playerStates.First(
                x => x.TargetPlayerId == player.PlayerId
            ) : null;
            RpcMurderPlayer(voteArea, player, poisoner);
        }
        public static void RpcMurderPlayer(PlayerVoteArea voteArea, PlayerControl player, PlayerControl poisoner)
        {
            if (player.IsShielded())
            {
                var medic = player.GetMedic().Player.PlayerId;
                Utils.Rpc(CustomRPC.AttemptSound, medic, player.PlayerId);

                StopKill.BreakShield(medic, player.PlayerId, CustomGameOptions.ShieldBreaks);
            }
            else if (player.IsBarriered())
            {
                var cleric = player.GetCleric();
                cleric.BarrieredPlayer = null;
                Utils.Rpc(CustomRPC.Unbarrier, cleric.Player.PlayerId);
            }
            else if (!player.Is(RoleEnum.Pestilence) && !player.Is(RoleEnum.Famine) && !player.Is(RoleEnum.War) && !player.Is(RoleEnum.Death) && !player.IsProtected() && !player.IsVesting())
            {
                MurderPlayer(voteArea, player, poisoner);
                PoisKillCount(player, poisoner);
                Utils.Rpc(CustomRPC.PoisonKill, poisoner.PlayerId, player.PlayerId);
            }
        }

        public static void MurderPlayer(PlayerControl player, PlayerControl poisoner, bool checkLover = true)
        {
            PlayerVoteArea voteArea = Utils.IsMeeting ? MeetingHud.Instance.playerStates.First(
                x => x.TargetPlayerId == player.PlayerId
            ) : null;
            MurderPlayer(voteArea, player, poisoner, checkLover);
        }
        public static void PoisKillCount(PlayerControl player, PlayerControl poisoner)
        {
            var pois = Role.GetRole<Poisoner>(poisoner);
            if (!Utils.IsMeeting) pois.Kills += 1;
        }
        public static void MurderPlayer(
            PlayerVoteArea voteArea,
            PlayerControl player,
            PlayerControl poisoner,
            bool checkLover = true
        )
        {
            if (voteArea == null && checkLover)
            {
                Utils.MurderPlayer(poisoner, player, false);
                return;
            }
            Coroutines.Start(Utils.FlashCoroutine(Patches.Colors.Impostor));
            NotificationPatch.Notification(Patches.TranslationPatches.CurrentLanguage == 0 ? $"{player.GetDefaultOutfit().PlayerName} Has Died From Poison!" : $"{player.GetDefaultOutfit().PlayerName} Zginal Przez Trucizne!", 1000 * CustomGameOptions.NotificationDuration);
            var hudManager = DestroyableSingleton<HudManager>.Instance;
            if (checkLover)
            {
                SoundManager.Instance.PlaySound(player.KillSfx, false, 0.8f);
            }
            var amOwner = player.AmOwner;
            if (amOwner)
            {
                Utils.ShowDeadBodies = true;
                hudManager.ShadowQuad.gameObject.SetActive(false);
                player.nameText().GetComponent<MeshRenderer>().material.SetInt("_Mask", 0);
                player.RpcSetScanner(false);
                ImportantTextTask importantTextTask = new GameObject("_Player").AddComponent<ImportantTextTask>();
                importantTextTask.transform.SetParent(AmongUsClient.Instance.transform, false);
                if (!GameOptionsManager.Instance.currentNormalGameOptions.GhostsDoTasks)
                {
                    for (int i = 0;i < player.myTasks.Count;i++)
                    {
                        PlayerTask playerTask = player.myTasks.ToArray()[i];
                        playerTask.OnRemove();
                        UnityEngine.Object.Destroy(playerTask.gameObject);
                    }

                    player.myTasks.Clear();
                    importantTextTask.Text = DestroyableSingleton<TranslationController>.Instance.GetString(
                        StringNames.GhostIgnoreTasks,
                        new Il2CppReferenceArray<Il2CppSystem.Object>(0)
                    );
                }
                else
                {
                    importantTextTask.Text = DestroyableSingleton<TranslationController>.Instance.GetString(
                        StringNames.GhostDoTasks,
                        new Il2CppReferenceArray<Il2CppSystem.Object>(0));
                }

                player.myTasks.Insert(0, importantTextTask);

                if (player.Is(RoleEnum.Swapper))
                {
                    var swapper = Role.GetRole<Swapper>(PlayerControl.LocalPlayer);
                    swapper.ListOfActives.Clear();
                    swapper.Buttons.Clear();
                    SwapVotes.Swap1 = null;
                    SwapVotes.Swap2 = null;
                    Utils.Rpc(CustomRPC.SetSwaps, sbyte.MaxValue, sbyte.MaxValue);
                    var buttons = Role.GetRole<Swapper>(player).Buttons;
                    foreach (var button in buttons)
                    {
                        button.SetActive(false);
                        button.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                    }
                }

                if (player.Is(RoleEnum.Imitator))
                {
                    var imitator = Role.GetRole<Imitator>(PlayerControl.LocalPlayer);
                    imitator.ListOfActives.Clear();
                    imitator.Buttons.Clear();
                    SetImitate.Imitate = null;
                    var buttons = Role.GetRole<Imitator>(player).Buttons;
                    foreach (var button in buttons)
                    {
                        button.SetActive(false);
                        button.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                    }
                }

                if (player.Is(RoleEnum.Vigilante))
                {
                    var retributionist = Role.GetRole<Vigilante>(PlayerControl.LocalPlayer);
                    ShowHideButtonsVigi.HideButtonsVigi(retributionist);
                }

                if (player.Is(AbilityEnum.Assassin))
                {
                    var assassin = Ability.GetAbility<Assassin>(PlayerControl.LocalPlayer);
                    ShowHideButtons.HideButtons(assassin);
                }

                if (player.Is(RoleEnum.Doomsayer))
                {
                    var doomsayer = Role.GetRole<Doomsayer>(PlayerControl.LocalPlayer);
                    ShowHideButtonsDoom.HideButtonsDoom(doomsayer);
                }

                if (player.Is(RoleEnum.Mayor))
                {
                    var mayor = Role.GetRole<Mayor>(PlayerControl.LocalPlayer);
                    mayor.RevealButton.Destroy();
                }

                if (player.Is(RoleEnum.Deputy))
                {
                    var demagogue0 = Role.GetRole<Deputy>(PlayerControl.LocalPlayer);
                    foreach (var button in demagogue0.ShootButtons) button.Value.Destroy();
                    demagogue0.ShootButtons.Clear();
                }
            }
            player.Die(DeathReason.Kill, false);
            if (checkLover && player.IsLover() && CustomGameOptions.BothLoversDie)
            {
                var otherLover = Objective.GetObjective<Lover>(player).OtherLover.Player;
                if (!otherLover.Is(RoleEnum.Pestilence) && !otherLover.Is(RoleEnum.Famine) && !otherLover.Is(RoleEnum.War) && !otherLover.Is(RoleEnum.Death)) MurderPlayer(otherLover, poisoner, false);
            }
            else if (checkLover && player.Is(FactionOverride.Recruit) && !player.Is(RoleEnum.Jackal) && CustomGameOptions.RecruistLifelink)
            {
                var otherRecruit = PlayerControl.AllPlayerControls.ToArray().First(x => x.PlayerId != player.PlayerId && x.Is(FactionOverride.Recruit) && !x.Is(RoleEnum.Jackal));
                if (!otherRecruit.Is(RoleEnum.Pestilence) && !otherRecruit.Is(RoleEnum.Famine) && !otherRecruit.Is(RoleEnum.War) && !otherRecruit.Is(RoleEnum.Death)) MurderPlayer(otherRecruit, poisoner, false);
            }
            else if (checkLover && player.Is(RoleEnum.JKNecromancer))
            {
                foreach (var undead in PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(FactionOverride.Undead) && !x.Is(RoleEnum.JKNecromancer)))
                {
                    MurderPlayer(undead, poisoner, false);
                }
            }

            var deadPlayer = new DeadPlayer
            {
                PlayerId = player.PlayerId,
                KillerId = player.PlayerId,
                KillTime = System.DateTime.UtcNow,
            };

            Murder.KilledPlayers.Add(deadPlayer);
            if (voteArea == null) return;
            if (voteArea.DidVote) voteArea.UnsetVote();
            voteArea.AmDead = true;
            voteArea.Overlay.gameObject.SetActive(true);
            voteArea.Overlay.color = Color.white;
            voteArea.XMark.gameObject.SetActive(true);
            voteArea.XMark.transform.localScale = Vector3.one;

            var meetingHud = MeetingHud.Instance;
            if (amOwner)
            {
                meetingHud.SetForegroundForDead();
            }

            var blackmailers = Role.AllRoles.Where(x => x.RoleType == RoleEnum.Blackmailer && x.Player != null).Cast<Blackmailer>();
            foreach (var role in blackmailers)
            {
                if (role.Blackmailed != null && voteArea.TargetPlayerId == role.Blackmailed.PlayerId)
                {
                    if (BlackmailMeetingUpdate.PrevXMark != null && BlackmailMeetingUpdate.PrevOverlay != null)
                    {
                        voteArea.XMark.sprite = BlackmailMeetingUpdate.PrevXMark;
                        voteArea.Overlay.sprite = BlackmailMeetingUpdate.PrevOverlay;
                        voteArea.XMark.transform.localPosition = new Vector3(
                            voteArea.XMark.transform.localPosition.x - BlackmailMeetingUpdate.LetterXOffset,
                            voteArea.XMark.transform.localPosition.y - BlackmailMeetingUpdate.LetterYOffset,
                            voteArea.XMark.transform.localPosition.z);
                    }
                }
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Vigilante) && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                var vigi = Role.GetRole<Vigilante>(PlayerControl.LocalPlayer);
                ShowHideButtonsVigi.HideTarget(vigi, voteArea.TargetPlayerId);
            }

            if (PlayerControl.LocalPlayer.Is(AbilityEnum.Assassin) && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                var assassin = Ability.GetAbility<Assassin>(PlayerControl.LocalPlayer);
                ShowHideButtons.HideTarget(assassin, voteArea.TargetPlayerId);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Doomsayer) && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                var doom = Role.GetRole<Doomsayer>(PlayerControl.LocalPlayer);
                ShowHideButtonsDoom.HideTarget(doom, voteArea.TargetPlayerId);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Deputy) && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                var dep = Role.GetRole<Deputy>(PlayerControl.LocalPlayer);
                if (!dep.Revealed)
                {
                    dep.ShootButtons[voteArea.TargetPlayerId].Destroy();
                    dep.ShootButtons.Remove(voteArea.TargetPlayerId);
                }
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Demagogue) && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                var dem = Role.GetRole<Demagogue>(PlayerControl.LocalPlayer);
                if (dem.MeetingKillButtons.ContainsKey(voteArea.TargetPlayerId))
                {
                    dem.MeetingKillButtons[voteArea.TargetPlayerId].button.Destroy();
                    dem.MeetingKillButtons[voteArea.TargetPlayerId].text.Destroy();
                    dem.MeetingKillButtons.Remove(voteArea.TargetPlayerId);
                }
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Swapper) && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                var swapper = Role.GetRole<Swapper>(PlayerControl.LocalPlayer);
                var button = swapper.Buttons[voteArea.TargetPlayerId];
                if (button.GetComponent<SpriteRenderer>().sprite == TownOfUs.SwapperSwitch)
                {
                    swapper.ListOfActives[voteArea.TargetPlayerId] = false;
                    if (SwapVotes.Swap1 == voteArea) SwapVotes.Swap1 = null;
                    if (SwapVotes.Swap2 == voteArea) SwapVotes.Swap2 = null;
                    Utils.Rpc(CustomRPC.SetSwaps, sbyte.MaxValue, sbyte.MaxValue);
                }
                button.SetActive(false);
                button.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                swapper.Buttons[voteArea.TargetPlayerId] = null;
            }

            foreach (var playerVoteArea in meetingHud.playerStates)
            {
                if (playerVoteArea.VotedFor != player.PlayerId) continue;
                playerVoteArea.UnsetVote();
                var voteAreaPlayer = Utils.PlayerById(playerVoteArea.TargetPlayerId);
                if (voteAreaPlayer.Is(RoleEnum.Prosecutor))
                {
                    var pros = Role.GetRole<Prosecutor>(voteAreaPlayer);
                    pros.ProsecuteThisMeeting = false;
                }
                if (!voteAreaPlayer.AmOwner) continue;
                meetingHud.ClearVote();
            }

            if (AmongUsClient.Instance.AmHost) meetingHud.CheckForEndVoting();

            AddHauntPatch.AssassinatedPlayers.Add(player);
        }
    }
}