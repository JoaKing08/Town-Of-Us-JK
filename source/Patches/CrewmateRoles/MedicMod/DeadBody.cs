using System;
using System.Collections.Generic;
using TownOfUs.Extensions;
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

        public static string ParseBodyReport(BodyReport br)
        {
            //System.Console.WriteLine(br.KillAge);
            if (br.KillAge > CustomGameOptions.MedicReportColorDuration * 1000)
                return
                    $"Body Report: The corpse is too old to gain information from. (Killed {Math.Round(br.KillAge / 1000)}s ago)";

            if (br.Killer.PlayerId == br.Body.PlayerId)
                return
                    $"Body Report: The kill appears to have been a suicide! (Killed {Math.Round(br.KillAge / 1000)}s ago)";

            if (br.KillAge < CustomGameOptions.MedicReportNameDuration * 1000)
                return
                    $"Body Report: The killer appears to be {br.Killer.Data.PlayerName}! (Killed {Math.Round(br.KillAge / 1000)}s ago)";

            var colors = new Dictionary<int, string>
            {
                {0, "darker"},// red
                {1, "darker"},// blue
                {2, "darker"},// green
                {3, "lighter"},// pink
                {4, "lighter"},// orange
                {5, "lighter"},// yellow
                {6, "darker"},// black
                {7, "lighter"},// white
                {8, "darker"},// purple
                {9, "darker"},// brown
                {10, "lighter"},// cyan
                {11, "lighter"},// lime
                {12, "darker"},// maroon
                {13, "lighter"},// rose
                {14, "lighter"},// banana
                {15, "darker"},// gray
                {16, "darker"},// tan
                {17, "lighter"},// coral
                {18, "darker"},// watermelon
                {19, "darker"},// chocolate
                {20, "lighter"},// sky blue
                {21, "lighter"},// beige
                {22, "darker"},// magenta
                {23, "lighter"},// turquoise
                {24, "lighter"},// lilac
                {25, "darker"},// olive
                {26, "lighter"},// azure
                {27, "darker"},// plum
                {28, "darker"},// jungle
                {29, "lighter"},// mint
                {30, "lighter"},// chartreuse
                {31, "darker"},// macau
                {32, "darker"},// tawny
                {33, "lighter"},// gold
                {34, "lighter"},// rainbow
                {35, "lighter"},// ice
                {36, "lighter"},// copper
                {37, "darker"},// fortegreen
                {38, "darker"},// ink black
                {39, "darker"},// ash gray
                {40, "lighter"},// snow white
                {41, "darker"},// bloody red
                {42, "darker"},// sunset orange
                {43, "lighter"},// sunny yellow
                {44, "lighter"},// juicy lime
                {45, "darker"},// cactus green
                {46, "lighter"},// heaven cyan
                {47, "darker"},// ocean blue
                {48, "darker"},// galaxy purple
                {49, "lighter"},// neon pink
                {50, "darker"},// woody brown
                {51, "lighter"},// black & white
            };
            var typeOfColor = colors[br.Killer.GetDefaultOutfit().ColorId];
            return
                $"Body Report: The killer appears to be a {typeOfColor} color. (Killed {Math.Round(br.KillAge / 1000)}s ago)";
        }
    }
}
