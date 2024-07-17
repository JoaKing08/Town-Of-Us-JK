using System;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.DetectiveMod
{
    public class BodyReport
    {
        public PlayerControl Killer { get; set; }
        public PlayerControl Reporter { get; set; }
        public PlayerControl Body { get; set; }
        public float KillAge { get; set; }

        public static string ParseBodyReport(BodyReport br)
        {
            if (br.KillAge > CustomGameOptions.DetectiveFactionDuration * 1000)
                return Patches.TranslationPatches.CurrentLanguage == 0 ?
                    $"<b>Body Report:</b> The corpse is <b>too old</b> to gain information from. (Killed <b>{Math.Round(br.KillAge / 1000)}</b>s ago)" :
                    $"<b>Raport Ciala:</b> Trup jest <b>za stary</b> by zdobyc jakiekolwiek informacje. (Zabito <b>{Math.Round(br.KillAge / 1000)}</b>s temu)";

            if (br.Killer.PlayerId == br.Body.PlayerId)
                return Patches.TranslationPatches.CurrentLanguage == 0 ?
                    $"<b>Body Report:</b> The kill appears to have been a <b>suicide</b>! (Killed <b>{Math.Round(br.KillAge / 1000)}</b>s ago)" :
                    $"<b>Raport Ciala:</b> Zabójstwo wyglada na <b>samobójstwo</b>! (Zabito <b>{Math.Round(br.KillAge / 1000)}</b>s temu)";

            var role = Role.GetRole(br.Killer);

            if (br.KillAge < CustomGameOptions.DetectiveRoleDuration * 1000)
                return Patches.TranslationPatches.CurrentLanguage == 0 ?
                    $"<b>Body Report:</b> The killer appears to be a <b>{role.ColorString}{role.Name}</color></b>! (Killed <b>{Math.Round(br.KillAge / 1000)}</b>s ago)" :
                    $"<b>Raport Ciala:</b> Zabójca wydaje sie byc <b>{role.ColorString}{role.Name}</color></b>! (Zabito <b>{Math.Round(br.KillAge / 1000)}</b>s temu)";

            if (br.Killer.Is(Faction.Crewmates))
                return Patches.TranslationPatches.CurrentLanguage == 0 ?
                    $"<b>Body Report:</b> The killer appears to be a <b><color=#00FFFFFF>Crewmate</color></b>! (Killed <b>{Math.Round(br.KillAge / 1000)}</b>s ago)" :
                    $"<b>Raport Ciala:</b> Zabójca wydaje sie byc <b><color=#00FFFFFF>Crewmate</color></b>! (Zabito <b>{Math.Round(br.KillAge / 1000)}</b>s temu)";

            else if (br.Killer.Is(Faction.NeutralKilling) || br.Killer.Is(Faction.NeutralBenign) || br.Killer.Is(Faction.NeutralEvil) || br.Killer.Is(Faction.NeutralChaos) || br.Killer.Is(Faction.NeutralApocalypse))
                return Patches.TranslationPatches.CurrentLanguage == 0 ?
                    $"<b>Body Report:</b> The killer appears to be a <b><color=#808080FF>Neutral</color> Role</b>! (Killed <b>{Math.Round(br.KillAge / 1000)}</b>s ago)" :
                    $"<b>Raport Ciala:</b> Zabójca wydaje sie byc <b>Rola <color=#808080FF>Neutral</color></b>! (Zabito <b>{Math.Round(br.KillAge / 1000)}</b>s temu)";

            else
                return Patches.TranslationPatches.CurrentLanguage == 0 ?
                    $"<b>Body Report:</b> The killer appears to be an <b><color=#FF0000FF>Impostor</color></b>! (Killed <b>{Math.Round(br.KillAge / 1000)}</b>s ago)" :
                    $"<b>Raport Ciala:</b> Zabójca wydaje sie byc <b><color=#FF0000FF>Impostor</color></b>! (Zabito <b>{Math.Round(br.KillAge / 1000)}</b>s temu)";
        }
    }
}
