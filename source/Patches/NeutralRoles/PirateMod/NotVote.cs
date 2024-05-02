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
                if (pirate.Defense == dueled.Defense && !pirate.Player.Data.IsDead && !pirate.Player.Data.Disconnected)
                {
                    if (PlayerControl.LocalPlayer == pirate.DueledPlayer)
                    {
                        Coroutines.Start(Utils.FlashCoroutine(Color.red));
                        dueled.Notification("You Lost The Duel!", 1000 * CustomGameOptions.NotificationDuration);
                    }
                        if (pirate.DueledPlayer.Is(RoleEnum.Pestilence) || pirate.DueledPlayer.Is(RoleEnum.Famine) || pirate.DueledPlayer.Is(RoleEnum.War) || pirate.DueledPlayer.Is(RoleEnum.Death))
                    {
                        pirate.DueledPlayer = null;
                    }
                    else
                    {
                        if (PlayerControl.LocalPlayer == pirate.DueledPlayer)
                        {
                            KillButtonTarget.DontRevive = pirate.DueledPlayer.PlayerId;
                        }
                        pirate.DueledPlayer.Exiled();
                        pirate.DueledPlayer = null;
                    }
                    pirate.DuelsWon += 1;
                    if (pirate.Player == PlayerControl.LocalPlayer)
                    {
                        Coroutines.Start(Utils.FlashCoroutine(Color.green));
                        pirate.Notification("Ya Won Th' Duel!", 1000 * CustomGameOptions.NotificationDuration);
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
                        pirate.Notification("Ya Lost Th' Duel!", 1000 * CustomGameOptions.NotificationDuration);
                    }
                    if (pirate.DueledPlayer == PlayerControl.LocalPlayer)
                    {
                        Coroutines.Start(Utils.FlashCoroutine(Color.green));
                        dueled.Notification("You Won The Duel!", 1000 * CustomGameOptions.NotificationDuration);
                    }
                    pirate.DueledPlayer = null;
                }
            }
        }
    }
}