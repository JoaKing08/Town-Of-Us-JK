using HarmonyLib;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using System.Linq;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using TownOfUs.CrewmateRoles.AltruistMod;
using UnityEngine;

namespace TownOfUs.NeutralRoles.PirateMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))] // BBFDNCCEJHI
    public static class VotingComplete
    {
        public static void Postfix(MeetingHud __instance)
        {
            var pirate = (Pirate)Role.GetRoles(RoleEnum.Pirate).FirstOrDefault();
            if (pirate != null && pirate.DueledPlayer != null)
            {
                var dueled = Role.GetRole(pirate.DueledPlayer);
                pirate.DefenseButton.Destroy();
                dueled.DefenseButton.Destroy();
                if (pirate.Defense == dueled.Defense && !pirate.Player.Data.IsDead && !pirate.Player.Data.Disconnected && !dueled.Player.Data.IsDead && !dueled.Player.Data.Disconnected)
                {
                    if (PlayerControl.LocalPlayer == pirate.DueledPlayer)
                    {
                        Coroutines.Start(Utils.FlashCoroutine(Color.red));
                        NotificationPatch.Notification(Patches.TranslationPatches.CurrentLanguage == 0 ? "You Lost The Duel!" : "Przegrales Pojedynek!", 1000 * CustomGameOptions.NotificationDuration);
                    }
                    var voteArea = MeetingHud.Instance.playerStates.First(x => x.TargetPlayerId == pirate.DueledPlayer.PlayerId);
                    if (!pirate.DueledPlayer.Is(RoleEnum.Pestilence) && !pirate.DueledPlayer.Is(RoleEnum.Famine) && !pirate.DueledPlayer.Is(RoleEnum.War) && !pirate.DueledPlayer.Is(RoleEnum.Death))
                    {
                        pirate.DueledPlayer.Exiled();
                        voteArea.AmDead = true;
                        voteArea.Overlay.gameObject.SetActive(true);
                        voteArea.Overlay.color = Color.white;
                        voteArea.XMark.gameObject.SetActive(true);
                        voteArea.XMark.transform.localScale = Vector3.one;
                        SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.KillSfx, false, 0.8f);
                        if (pirate.DueledPlayer.Is(ObjectiveEnum.Lover) && CustomGameOptions.BothLoversDie)
                        {
                            var lover = Objective.GetObjective<Lover>(pirate.DueledPlayer).OtherLover.Player;
                            lover.Exiled();
                            voteArea = MeetingHud.Instance.playerStates.First(x => x.TargetPlayerId == lover.PlayerId);
                            voteArea.AmDead = true;
                            voteArea.Overlay.gameObject.SetActive(true);
                            voteArea.Overlay.color = Color.white;
                            voteArea.XMark.gameObject.SetActive(true);
                            voteArea.XMark.transform.localScale = Vector3.one;
                        }
                        if (pirate.DueledPlayer.Is(FactionOverride.Recruit) && CustomGameOptions.RecruistLifelink)
                        {
                            var recruit = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(x => x.Is(FactionOverride.Recruit) && !x.Is(RoleEnum.Jackal) && x.PlayerId != pirate.Player.PlayerId);
                            if (recruit != null)
                            {
                                recruit.Exiled();
                                voteArea = MeetingHud.Instance.playerStates.First(x => x.TargetPlayerId == recruit.PlayerId);
                                voteArea.AmDead = true;
                                voteArea.Overlay.gameObject.SetActive(true);
                                voteArea.Overlay.color = Color.white;
                                voteArea.XMark.gameObject.SetActive(true);
                                voteArea.XMark.transform.localScale = Vector3.one;
                            }
                        }
                        if (pirate.DueledPlayer.Is(RoleEnum.JKNecromancer))
                        {
                            foreach (var undead in PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(FactionOverride.Undead)))
                            {
                                undead.Exiled();
                                voteArea = MeetingHud.Instance.playerStates.First(x => x.TargetPlayerId == undead.PlayerId);
                                voteArea.AmDead = true;
                                voteArea.Overlay.gameObject.SetActive(true);
                                voteArea.Overlay.color = Color.white;
                                voteArea.XMark.gameObject.SetActive(true);
                                voteArea.XMark.transform.localScale = Vector3.one;
                            }
                        }
                        if (pirate.DueledPlayer.Is(RoleEnum.Godfather) && CustomGameOptions.MafiosoLifelink)
                        {
                            var mafioso = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(x => x.Is(RoleEnum.Mafioso));
                            if (mafioso != null && !mafioso.Data.IsDead && !mafioso.Data.Disconnected)
                            {
                                mafioso.Exiled();
                                voteArea = MeetingHud.Instance.playerStates.First(x => x.TargetPlayerId == mafioso.PlayerId);
                                voteArea.AmDead = true;
                                voteArea.Overlay.gameObject.SetActive(true);
                                voteArea.Overlay.color = Color.white;
                                voteArea.XMark.gameObject.SetActive(true);
                                voteArea.XMark.transform.localScale = Vector3.one;
                            }
                        }
                    }
                    pirate.DueledPlayer = null;
                    pirate.DuelsWon += 1;
                    if (pirate.Player == PlayerControl.LocalPlayer)
                    {
                        Coroutines.Start(Utils.FlashCoroutine(Color.green));
                        NotificationPatch.Notification(Patches.TranslationPatches.CurrentLanguage == 0 ? "Ya Won Th' Duel!" : "Wygrales Pojedynek!", 1000 * CustomGameOptions.NotificationDuration);
                    }
                        if (pirate.DuelsWon >= CustomGameOptions.PirateDuelsToWin)
                        {
                            pirate.WonByDuel = true;
                            if (!CustomGameOptions.NeutralEvilWinEndsGame)
                            {
                                KillButtonTarget.DontRevive = pirate.Player.PlayerId;
                                pirate.Player.Exiled();
                            }
                        }
                }
                else if (!pirate.Player.Data.IsDead && !pirate.Player.Data.Disconnected)
                {
                    if (pirate.Player == PlayerControl.LocalPlayer)
                    {
                        Coroutines.Start(Utils.FlashCoroutine(Color.red));
                        NotificationPatch.Notification(Patches.TranslationPatches.CurrentLanguage == 0 ? "Ya Lost Th' Duel!" : "Przegrales Pojedynek!", 1000 * CustomGameOptions.NotificationDuration);
                    }
                    if (pirate.DueledPlayer == PlayerControl.LocalPlayer)
                    {
                        Coroutines.Start(Utils.FlashCoroutine(Color.green));
                        NotificationPatch.Notification(Patches.TranslationPatches.CurrentLanguage == 0 ? "You Won The Duel!" : "Wygrales Pojedynek!", 1000 * CustomGameOptions.NotificationDuration);
                    }
                    pirate.DueledPlayer = null;
                }
            }
        }
    }
}