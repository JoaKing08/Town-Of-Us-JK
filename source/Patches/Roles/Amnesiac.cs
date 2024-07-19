using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

namespace TownOfUs.Roles
{
    public class Amnesiac : Role
    {
        public Dictionary<byte, ArrowBehaviour> BodyArrows = new Dictionary<byte, ArrowBehaviour>();
        public bool SpawnedAs = true;
        public bool WasSwapped = false;

        public Amnesiac(PlayerControl player) : base(player)
        {
            Name = "Amnesiac";
            ImpostorText = () => "Remember A Role Of A Deceased Player";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? (WasSwapped ? "Your role was stolen. Now remember a new role!" : SpawnedAs ? "Find a dead body to remember a role" : "Your target was killed. Now remember a new role!") : (WasSwapped ? "Twoja rola zostala podmieniona. Teraz zapamietaj nowa!" : SpawnedAs ? "Znajdz martwe cialo by zapamietac role" : "Tw�j cel zostal zabity. Teraz zapamietaj nowa role!");
            Color = Patches.Colors.Amnesiac;
            RoleType = RoleEnum.Amnesiac;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralBenign;
        }

        public DeadBody CurrentTarget;

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            var amnesiacTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            amnesiacTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = amnesiacTeam;
        }

        public void DestroyArrow(byte targetPlayerId)
        {
            var arrow = BodyArrows.FirstOrDefault(x => x.Key == targetPlayerId);
            if (arrow.Value != null)
                Object.Destroy(arrow.Value);
            if (arrow.Value.gameObject != null)
                Object.Destroy(arrow.Value.gameObject);
            BodyArrows.Remove(arrow.Key);
        }
    }
}