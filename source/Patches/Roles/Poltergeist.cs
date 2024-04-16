using UnityEngine;
using System.Collections.Generic;
using TownOfUs.Extensions;
using TownOfUs.Modifiers.UnderdogMod;

namespace TownOfUs.Roles
{
    public class Poltergeist : Role
    {
        public RoleEnum formerRole = new RoleEnum();
        public bool Caught;
        public bool Revealed;
        public bool CompletedTasks;
        public bool Faded;

        public List<ArrowBehaviour> CrewArrows = new List<ArrowBehaviour>();

        public List<PlayerControl> PoltergeistTargets = new List<PlayerControl>();

        public List<ArrowBehaviour> PoltergeistArrows = new List<ArrowBehaviour>();

        public Poltergeist(PlayerControl player) : base(player)
        {
            Name = "Poltergeist";
            ImpostorText = () => "";
            TaskText = () => "Complete all your tasks to help impostors kill!";
            Color = Patches.Colors.Impostor;
            RoleType = RoleEnum.Poltergeist;
            Faction = Faction.Impostors;
            AddToRoleHistory(RoleType);
        }

        public void Fade()
        {
            Faded = true;
            Player.Visible = true;
            var color = new Color(1f, 1f, 1f, 0f);

            var maxDistance = ShipStatus.Instance.MaxLightRadius * GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod;

            if (PlayerControl.LocalPlayer == null)
                return;

            var distance = (PlayerControl.LocalPlayer.GetTruePosition() - Player.GetTruePosition()).magnitude;

            var distPercent = distance / maxDistance;
            distPercent = Mathf.Max(0, distPercent - 1);

            var velocity = Player.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude;
            color.a = 0.07f + velocity / Player.MyPhysics.GhostSpeed * 0.13f;
            color.a = Mathf.Lerp(color.a, 0, distPercent);

            if (Player.GetCustomOutfitType() != CustomPlayerOutfitType.PlayerNameOnly)
            {
                Player.SetOutfit(CustomPlayerOutfitType.PlayerNameOnly, new GameData.PlayerOutfit()
                {
                    ColorId = Player.GetDefaultOutfit().ColorId,
                    HatId = "",
                    SkinId = "",
                    VisorId = "",
                    PlayerName = " ",
                    PetId = ""
                });
            }
            Player.myRend().color = color;
            Player.nameText().color = Color.clear;
            Player.cosmetics.colorBlindText.color = Color.clear;
            Player.cosmetics.SetBodyCosmeticsVisible(false);
        }

        public float MaxTimer(bool IsUnderdog) => (IsUnderdog ? (PerformKill.LastImp() ? GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown - CustomGameOptions.UnderdogKillBonus : (PerformKill.IncreasedKC() ? GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown : GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + CustomGameOptions.UnderdogKillBonus)) : GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown) * (Utils.PoltergeistTasks() ? CustomGameOptions.PoltergeistKCdMult : 1f);

        public void SetKillTimer(bool IsUnderdog, PlayerControl player)
        {
            player.SetKillTimer(MaxTimer(IsUnderdog));
        }
    }
}