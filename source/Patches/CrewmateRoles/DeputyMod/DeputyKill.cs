using System.Linq;
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
using TownOfUs.NeutralRoles.PirateMod;

namespace TownOfUs.CrewmateRoles.DeputyMod
{
    public class DeputyKill
    {
        public static void RpcMurderPlayer(PlayerControl player, PlayerControl deputy)
        {
            PlayerVoteArea voteArea = MeetingHud.Instance.playerStates.First(
                x => x.TargetPlayerId == player.PlayerId
            );
            RpcMurderPlayer(voteArea, player, deputy);
        }
        public static void RpcMurderPlayer(PlayerVoteArea voteArea, PlayerControl player, PlayerControl deputy)
        {
            if (!player.Is(RoleEnum.Pestilence) && !player.Is(RoleEnum.Famine) && !player.Is(RoleEnum.War) && !player.Is(RoleEnum.Death))
            {
                MurderPlayer(voteArea, player, deputy);
                DepKillCount(player, deputy);
            }
            Utils.Rpc(CustomRPC.DeputyShoot, deputy.PlayerId, player.PlayerId);
        }

        public static void MurderPlayer(PlayerControl player, PlayerControl deputy, bool checkLover = true)
        {
            PlayerVoteArea voteArea = MeetingHud.Instance.playerStates.First(
                x => x.TargetPlayerId == player.PlayerId
            );
            MurderPlayer(voteArea, player, deputy, checkLover);
        }
        public static void DepKillCount(PlayerControl player, PlayerControl deputy)
        {
            var dep = Role.GetRole<Deputy>(deputy);
            if (deputy.Is(FactionOverride.None) && !deputy.Is(ObjectiveEnum.ImpostorAgent) && !deputy.Is(ObjectiveEnum.ApocalypseAgent))
            {
                if (player.Is(Faction.Crewmates) && player.Is(FactionOverride.None) && !player.Is(ObjectiveEnum.Lover) && !player.Is(ObjectiveEnum.ImpostorAgent) && !player.Is(ObjectiveEnum.ApocalypseAgent))
                {
                    if (CustomGameOptions.MisfireKillsDeputy) MurderPlayer(deputy, deputy);
                    dep.IncorrectKills += 1;
                }
                else dep.CorrectKills += 1;
            }
            else
            {
                dep.Kills += 1;
            }
        }
        public static void MurderPlayer(
            PlayerVoteArea voteArea,
            PlayerControl player,
            PlayerControl deputy,
            bool checkLover = true
        )
        {
            var hudManager = DestroyableSingleton<HudManager>.Instance;
            if (checkLover && player.PlayerId != deputy.PlayerId)
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
                    var deputy0 = Role.GetRole<Deputy>(PlayerControl.LocalPlayer);
                    foreach (var button in deputy0.ShootButtons) button.Value.Destroy();
                    deputy0.ShootButtons.Clear();
                }

                if (player.Is(RoleEnum.Demagogue))
                {
                    var demagogue = Role.GetRole<Demagogue>(PlayerControl.LocalPlayer);
                    foreach (var entry in demagogue.MeetingKillButtons)
                    {
                        entry.Value.text.Destroy();
                        entry.Value.button.Destroy();
                    }
                    demagogue.MeetingKillButtons.Clear();
                }
            }
            player.Die(DeathReason.Kill, false);
            if (checkLover && player.IsLover() && CustomGameOptions.BothLoversDie)
            {
                var otherLover = Objective.GetObjective<Lover>(player).OtherLover.Player;
                if (!otherLover.Is(RoleEnum.Pestilence) && !otherLover.Is(RoleEnum.Famine) && !otherLover.Is(RoleEnum.War) && !otherLover.Is(RoleEnum.Death)) MurderPlayer(otherLover, deputy, false);
            }
            else if (checkLover && player.IsCooperator() && CustomGameOptions.BothCooperatorsDie)
            {
                var otherCooperator = Objective.GetObjective<Cooperator>(player).OtherCooperator.Player;
                if (!otherCooperator.Is(RoleEnum.Pestilence) && !otherCooperator.Is(RoleEnum.Famine) && !otherCooperator.Is(RoleEnum.War) && !otherCooperator.Is(RoleEnum.Death)) MurderPlayer(otherCooperator, deputy, false);
            }
            else if (checkLover && player.Is(FactionOverride.Recruit) && !player.Is(RoleEnum.Jackal) && CustomGameOptions.RecruistLifelink)
            {
                var otherRecruit = PlayerControl.AllPlayerControls.ToArray().First(x => x.PlayerId != player.PlayerId && x.Is(FactionOverride.Recruit) && !x.Is(RoleEnum.Jackal));
                if (!otherRecruit.Is(RoleEnum.Pestilence) && !otherRecruit.Is(RoleEnum.Famine) && !otherRecruit.Is(RoleEnum.War) && !otherRecruit.Is(RoleEnum.Death)) MurderPlayer(otherRecruit, deputy, false);
            }
            else if (player.Is(RoleEnum.Godfather) && CustomGameOptions.MafiosoLifelink)
            {
                var mafioso = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(x => x.Is(RoleEnum.Mafioso));
                if (mafioso != null && !mafioso.Data.IsDead && !mafioso.Data.Disconnected) MurderPlayer(mafioso, deputy, false);
            }
            else if (checkLover && player.Is(RoleEnum.JKNecromancer))
            {
                foreach (var undead in PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(FactionOverride.Undead) && !x.Is(RoleEnum.JKNecromancer)))
                {
                    MurderPlayer(undead, deputy, false);
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

            if (player.Is(RoleEnum.Pirate) || player.IsDueled())
            {
                ShowHideButtonsPirate.HideButtons();
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Deputy) && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                var dep = Role.GetRole<Deputy>(PlayerControl.LocalPlayer);
                if (!dep.Revealed && dep.ShootButtons.ContainsKey(voteArea.TargetPlayerId))
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
