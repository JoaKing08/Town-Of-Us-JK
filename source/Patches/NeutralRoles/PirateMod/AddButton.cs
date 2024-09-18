using System;
using System.Linq;
using HarmonyLib;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using TownOfUs.Modifiers.AssassinMod;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace TownOfUs.NeutralRoles.PirateMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class AddDuelButton
    {
        public static Sprite DuelAttack0Sprite => TownOfUs.DuelAttack0Sprite;
        public static Sprite DuelAttack1Sprite => TownOfUs.DuelAttack1Sprite;
        public static Sprite DuelAttack2Sprite => TownOfUs.DuelAttack2Sprite;
        public static Sprite DuelDefend0Sprite => TownOfUs.DuelDefend0Sprite;
        public static Sprite DuelDefend1Sprite => TownOfUs.DuelDefend1Sprite;
        public static Sprite DuelDefend2Sprite => TownOfUs.DuelDefend2Sprite;

        public static void GenButtonPirate(Pirate role, int index)
        {
            var confirmButton = MeetingHud.Instance.playerStates[index].Buttons.transform.GetChild(0).gameObject;

            var newButton = Object.Instantiate(confirmButton, MeetingHud.Instance.playerStates[index].transform);
            var renderer = newButton.GetComponent<SpriteRenderer>();
            var passive = newButton.GetComponent<PassiveButton>();

            role.Defense = 0;
            renderer.sprite = DuelAttack0Sprite;
            newButton.transform.position = confirmButton.transform.position - new Vector3(0.75f, 0f, 0f);
            newButton.transform.localScale *= 0.8f;
            newButton.layer = 5;
            newButton.transform.parent = confirmButton.transform.parent.parent;

            passive.OnClick = new Button.ButtonClickedEvent();
            passive.OnClick.AddListener(ChangeAttack(role));
            role.DefenseButton = newButton;
        }

        public static void GenButtonDueled(Role role, int index)
        {
            var confirmButton = MeetingHud.Instance.playerStates[index].Buttons.transform.GetChild(0).gameObject;

            var newButton = Object.Instantiate(confirmButton, MeetingHud.Instance.playerStates[index].transform);
            var renderer = newButton.GetComponent<SpriteRenderer>();
            var passive = newButton.GetComponent<PassiveButton>();

            role.Defense = 0;
            renderer.sprite = DuelDefend0Sprite;
            newButton.transform.position = confirmButton.transform.position - new Vector3(0.75f, role.RoleType == RoleEnum.Mayor && !((Mayor)role).Revealed ? -0.15f : 0f, 0f);
            newButton.transform.localScale *= 0.8f;
            newButton.layer = 5;
            newButton.transform.parent = confirmButton.transform.parent.parent;

            passive.OnClick = new Button.ButtonClickedEvent();
            passive.OnClick.AddListener(ChangeDefense(role));
            role.DefenseButton = newButton;
        }

        private static Action ChangeAttack(Pirate role)
        {
            void Listener()
            {
                role.Defense += 1;
                if (role.Defense > 2) role.Defense = 0;
                Utils.Rpc(CustomRPC.ChangeDefence, PlayerControl.LocalPlayer.PlayerId, (byte)role.Defense);
                switch (role.Defense)
                {
                    case 0:
                        role.DefenseButton.GetComponent<SpriteRenderer>().sprite = DuelAttack0Sprite;
                        break;
                    case 1:
                        role.DefenseButton.GetComponent<SpriteRenderer>().sprite = DuelAttack1Sprite;
                        break;
                    case 2:
                        role.DefenseButton.GetComponent<SpriteRenderer>().sprite = DuelAttack2Sprite;
                        break;
                }
            }

            return Listener;
        }

        private static Action ChangeDefense(Role role)
        {
            void Listener()
            {
                role.Defense += 1;
                if (role.Defense > 2) role.Defense = 0;
                Utils.Rpc(CustomRPC.ChangeDefence, PlayerControl.LocalPlayer.PlayerId, (byte)role.Defense);
                switch (role.Defense)
                {
                    case 0:
                        role.DefenseButton.GetComponent<SpriteRenderer>().sprite = DuelDefend0Sprite;
                        break;
                    case 1:
                        role.DefenseButton.GetComponent<SpriteRenderer>().sprite = DuelDefend1Sprite;
                        break;
                    case 2:
                        role.DefenseButton.GetComponent<SpriteRenderer>().sprite = DuelDefend2Sprite;
                        break;
                }
            }

            return Listener;
        }
        public static void Postfix(MeetingHud __instance)
        {
            var localRole = Role.GetRole(PlayerControl.LocalPlayer);
            if (localRole.DefenseButton) localRole.DefenseButton.Destroy();

            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Pirate))
            {
                var role = Role.GetRole<Pirate>(PlayerControl.LocalPlayer);
                if (role.DueledPlayer == null || role.DueledPlayer.Data.IsDead || role.DueledPlayer.Data.Disconnected) return;
                role.MeetingStart = DateTime.UtcNow;
                role.notify = true;
                for (var i = 0; i < __instance.playerStates.Length; i++)
                    if (PlayerControl.LocalPlayer.PlayerId == __instance.playerStates[i].TargetPlayerId)
                    {
                        GenButtonPirate(role, i);
                    }
            }
            else if (PlayerControl.LocalPlayer.IsDueled())
            {
                var pirate = PlayerControl.LocalPlayer.GetPirate();
                if (pirate == null || pirate.Player.Data.IsDead || pirate.Player.Data.Disconnected) return;
                pirate.MeetingStart = DateTime.UtcNow;
                pirate.notify = true;
                var role = Role.GetRole(PlayerControl.LocalPlayer);
                for (var i = 0; i < __instance.playerStates.Length; i++)
                    if (PlayerControl.LocalPlayer.PlayerId == __instance.playerStates[i].TargetPlayerId)
                    {
                        GenButtonDueled(role, i);
                    }
            }
        }
    }
}