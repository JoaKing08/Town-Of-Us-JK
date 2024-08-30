using System;
using System.Collections.Generic;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.MedicMod
{
    public class DeadPlayer
    {
        public byte KillerId { get; set; }
        public byte PlayerId { get; set; }
        public DateTime KillTime { get; set; }
        public bool KillerVented { get; set; }
        public KillerDirection KillerRunTo { get; set; }
        public bool KillerEscapeAbility { get; set; }
        public bool KillerKillAbility { get; set; }
        public Vector3 BodyPosition { get; set; }
        public RoleEnum KillersRole { get; set; }
        public Faction KillersFaction { get; set; }
    }

    //body report class for when medic reports a body
    public class BodyReport
    {
        public PlayerControl Killer { get; set; }
        public PlayerControl Reporter { get; set; }
        public PlayerControl Body { get; set; }
        public float KillAge { get; set; }

        public static string ParseBodyReport(BodyReport br, Medic role)
        {
            //System.Console.WriteLine(br.KillAge);
            if (br.KillAge > CustomGameOptions.MedicReportColorDuration * 1000)
                return Patches.TranslationPatches.CurrentLanguage == 0 ?
                    $"<b>Body Report:</b> The corpse is <b>too old</b> to gain information from. (Killed <b>{Math.Round(br.KillAge / 1000)}</b>s ago)" :
                    $"<b>Raport Ciala:</b> Trup jest <b>za stary</b> aby zdobyc jakiekolwiek informacje. (Zabito <b>{Math.Round(br.KillAge / 1000)}</b>s temu)";

            if (br.Killer.PlayerId == br.Body.PlayerId)
                return Patches.TranslationPatches.CurrentLanguage == 0 ?
                    $"<b>Body Report:</b> The kill appears to have been a <b>suicide</b>! (Killed <b>{Math.Round(br.KillAge / 1000)}</b>s ago)" :
                    $"<b>Raport Ciala:</b> Zabójstwo wyglada na <b>samobójstwo</b>! (Zabito <b>{Math.Round(br.KillAge / 1000)}</b>s temu)";

            if (br.KillAge < CustomGameOptions.MedicReportNameDuration * 1000)
                return Patches.TranslationPatches.CurrentLanguage == 0 ?
                    $"<b>Body Report:</b> The killer appears to be <b>{br.Killer.Data.PlayerName}</b>! (Killed <b>{Math.Round(br.KillAge / 1000)}</b>s ago)" :
                    $"<b>Raport Ciala:</b> Zabójca wydaje sie byc <b>{br.Killer.Data.PlayerName}</b>! (Zabito <b>{Math.Round(br.KillAge / 1000)}</b>s temu)";

            var typeOfColor = role.LightDarkColors[br.Killer.GetDefaultOutfit().ColorId] == "lighter" ? (Patches.TranslationPatches.CurrentLanguage == 0 ? "<color=#FFFFC0FF>lighter" : "<color=#FFFFC0FF>jasniejszy") : (Patches.TranslationPatches.CurrentLanguage == 0 ? "<color=#202020FF>darker" : "<color=#202020FF>ciemniejszy");
            return Patches.TranslationPatches.CurrentLanguage == 0 ?
                $"<b>Body Report:</b> The killer appears to be a <b>{typeOfColor}</color> color</b>. (Killed <b>{Math.Round(br.KillAge / 1000)}</b>s ago)" :
                $"<b>Raport Ciala:</b> Zabójca wydaje sie <b>{typeOfColor}</color> kolor</b>. (Zabito <b>{Math.Round(br.KillAge / 1000)}</b>s temu)";
        }
    }
}
