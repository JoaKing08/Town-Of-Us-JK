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

namespace TownOfUs.CrewmateRoles.DeputyMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class AddShootButton
    {
        public static Sprite RevealSprite => TownOfUs.DeputySprite;

        public static void GenButton(Deputy role, int index, byte targetId)
        {
            PluginSingleton<TownOfUs>.Instance.Log.LogMessage("Creating Shoot Button");
            var confirmButton = MeetingHud.Instance.playerStates[index].Buttons.transform.GetChild(0).gameObject;

            var newButton = Object.Instantiate(confirmButton, MeetingHud.Instance.playerStates[index].transform);
            var renderer = newButton.GetComponent<SpriteRenderer>();
            var passive = newButton.GetComponent<PassiveButton>();

            renderer.sprite = RevealSprite;
            newButton.transform.position = confirmButton.transform.position - new Vector3(0.75f, role.Player.IsDueled() ? 0.15f : 0f, 0f);
            newButton.transform.localScale *= 0.8f;
            newButton.layer = 5;
            newButton.transform.parent = confirmButton.transform.parent.parent;

            passive.OnClick = new Button.ButtonClickedEvent();
            passive.OnClick.AddListener(Shoot(role, targetId));
            PluginSingleton<TownOfUs>.Instance.Log.LogMessage("Action:" + Shoot(role, targetId));
            role.ShootButtons.Add(newButton);
            PluginSingleton<TownOfUs>.Instance.Log.LogMessage("Button Created");
        }


        private static Action Shoot(Deputy role, byte targetId)
        {
            void Listener()
            {
                if (Role.GetRole(PlayerControl.LocalPlayer).Roleblocked)
                {
                    Coroutines.Start(Utils.FlashCoroutine(Color.white));
                    role.Notification("You Are Roleblocked!", 1000 * CustomGameOptions.NotificationDuration);
                    return;
                }
                foreach (var button in role.ShootButtons)
                {
                    button.Destroy();
                }
                role.ShootButtons.Clear();
                var target = Utils.PlayerById(targetId);
                Coroutines.Start(Utils.FlashCoroutine(Patches.Colors.Deputy));
                role.Notification($"Deputy Has Shot {target.GetDefaultOutfit().PlayerName}!", 1000 * CustomGameOptions.NotificationDuration);
                role.Revealed = true;
                if (!target.Is(RoleEnum.Pestilence) && !target.Is(RoleEnum.Famine) && !target.Is(RoleEnum.War) && !target.Is(RoleEnum.Death))
                {
                    target.Exiled();
                    SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.KillSfx, false, 0.8f);
                    if (target.Is(ObjectiveEnum.Lover) && CustomGameOptions.BothLoversDie)
                    {
                        Objective.GetObjective<Lover>(target).OtherLover.Player.Exiled();
                    }
                    if (target.Is(FactionOverride.Recruit) && CustomGameOptions.RecruistLifelink)
                    {
                        var recruit = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(x => x.Is(FactionOverride.Recruit) && !x.Is(RoleEnum.Jackal) && x.PlayerId != targetId);
                        if (recruit != null) recruit.Exiled();
                    }
                    if (target.Is(RoleEnum.JKNecromancer))
                    {
                        foreach (var undead in PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(FactionOverride.Undead)))
                        {
                            undead.Exiled();
                        }
                    }
                    if (CustomGameOptions.MisfireKillsDeputy && target.Is(Faction.Crewmates) && target.Is(FactionOverride.None) && !target.Is(ObjectiveEnum.ImpostorAgent) && !target.Is(ObjectiveEnum.ApocalypseAgent))
                    {
                        role.Player.Exiled();
                        if (role.Player.Is(ObjectiveEnum.Lover) && CustomGameOptions.BothLoversDie)
                        {
                            Objective.GetObjective<Lover>(role.Player).OtherLover.Player.Exiled();
                        }
                        if (role.Player.Is(FactionOverride.Recruit) && CustomGameOptions.RecruistLifelink)
                        {
                            var recruit = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(x => x.Is(FactionOverride.Recruit) && !x.Is(RoleEnum.Jackal) && x.PlayerId != role.Player.PlayerId);
                            if (recruit != null) recruit.Exiled();
                        }
                    }
                }
                if (PlayerControl.LocalPlayer.IsDueled()) role.DefenseButton.transform.position -= new Vector3(0f, 0.15f, 0f);
                Utils.Rpc(CustomRPC.DeputyShoot, role.Player.PlayerId, targetId);
            }

            return Listener;
        }

        public static void RemoveAssassin(Deputy deputy)
        {
            PlayerVoteArea voteArea = MeetingHud.Instance.playerStates.First(
                x => x.TargetPlayerId == deputy.Player.PlayerId);
            if (PlayerControl.LocalPlayer.Is(AbilityEnum.Assassin))
            {
                var assassin = Ability.GetAbility<Assassin>(PlayerControl.LocalPlayer);
                ShowHideButtons.HideTarget(assassin, voteArea.TargetPlayerId);
                voteArea.NameText.transform.localPosition += new Vector3(-0.2f, -0.1f, 0f);
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Doomsayer))
            {
                var doomsayer = Role.GetRole<Doomsayer>(PlayerControl.LocalPlayer);
                var (cycleBack, cycleForward, guess, guessText) = doomsayer.Buttons[voteArea.TargetPlayerId];
                if (cycleBack == null || cycleForward == null) return;
                cycleBack.SetActive(false);
                cycleForward.SetActive(false);
                guess.SetActive(false);
                guessText.gameObject.SetActive(false);

                cycleBack.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                cycleForward.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                guess.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                doomsayer.Buttons[voteArea.TargetPlayerId] = (null, null, null, null);
                doomsayer.Guesses.Remove(voteArea.TargetPlayerId);
                voteArea.NameText.transform.localPosition += new Vector3(-0.2f, -0.1f, 0f);
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Vigilante))
            {
                var vigilante = Role.GetRole<Vigilante>(PlayerControl.LocalPlayer);
                var (cycleBack, cycleForward, guess, guessText) = vigilante.Buttons[voteArea.TargetPlayerId];
                if (cycleBack == null || cycleForward == null) return;
                cycleBack.SetActive(false);
                cycleForward.SetActive(false);
                guess.SetActive(false);
                guessText.gameObject.SetActive(false);

                cycleBack.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                cycleForward.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                guess.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                vigilante.Buttons[voteArea.TargetPlayerId] = (null, null, null, null);
                vigilante.Guesses.Remove(voteArea.TargetPlayerId);
                voteArea.NameText.transform.localPosition += new Vector3(-0.2f, -0.1f, 0f);
            }
            return;
        }

        public static void Postfix(MeetingHud __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Deputy))
            {
                var deputy = (Deputy)role;
                foreach (var button in deputy.ShootButtons) button.Destroy();
                deputy.ShootButtons.Clear();
            }

            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Deputy)) return;
            var deputyrole = Role.GetRole<Deputy>(PlayerControl.LocalPlayer);
            if (deputyrole.Revealed) return;
            for (var i = 0; i < __instance.playerStates.Length; i++)
                if (PlayerControl.LocalPlayer.PlayerId != __instance.playerStates[i].TargetPlayerId && deputyrole.Targets.Contains(__instance.playerStates[i].TargetPlayerId))
                {
                    GenButton(deputyrole, i, __instance.playerStates[i].TargetPlayerId);
                }
        }
    }
}