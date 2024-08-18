using System.Collections.Generic;
using TownOfUs.Extensions;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Harbinger : Role
    {
        public RoleEnum formerRole = new RoleEnum();
        public bool Caught;
        public bool Revealed;
        public bool CompletedTasks;
        public bool Faded;

        public List<ArrowBehaviour> CrewArrows = new List<ArrowBehaviour>();

        public List<PlayerControl> HarbingerTargets = new List<PlayerControl>();

        public List<ArrowBehaviour> HarbingerArrows = new List<ArrowBehaviour>();

        public Harbinger(PlayerControl player) : base(player)
        {
            Name = "Harbinger";
            ImpostorText = () => "";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Complete all your tasks to hasten the apocalypse!" : "Skoncz swoje zadania by przyspieszyc apokalipse!";
            Color = Patches.Colors.Harbinger;
            RoleType = RoleEnum.Harbinger;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralApocalypse;
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
                    PetId = " "
                });
            }
            Player.myRend().color = color;
            Player.nameText().color = Color.clear;
            Player.cosmetics.colorBlindText.color = Color.clear;
            Player.cosmetics.SetBodyCosmeticsVisible(false);
        }
    }
}