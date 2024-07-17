﻿using System;
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

            var colors = Patches.TranslationPatches.CurrentLanguage == 0 ? new Dictionary<int, string>
            {
                {0, "<color=#202020FF>darker"},// red
                {1, "<color=#202020FF>darker"},// blue
                {2, "<color=#202020FF>darker"},// green
                {3, "<color=#FFFFC0FF>lighter"},// pink
                {4, "<color=#FFFFC0FF>lighter"},// orange
                {5, "<color=#FFFFC0FF>lighter"},// yellow
                {6, "<color=#202020FF>darker"},// black
                {7, "<color=#FFFFC0FF>lighter"},// white
                {8, "<color=#202020FF>darker"},// purple
                {9, "<color=#202020FF>darker"},// brown
                {10, "<color=#FFFFC0FF>lighter"},// cyan
                {11, "<color=#FFFFC0FF>lighter"},// lime
                {12, "<color=#202020FF>darker"},// maroon
                {13, "<color=#FFFFC0FF>lighter"},// rose
                {14, "<color=#FFFFC0FF>lighter"},// banana
                {15, "<color=#202020FF>darker"},// gray
                {16, "<color=#202020FF>darker"},// tan
                {17, "<color=#FFFFC0FF>lighter"},// coral
                {18, "<color=#202020FF>darker"},// watermelon
                {19, "<color=#202020FF>darker"},// chocolate
                {20, "<color=#FFFFC0FF>lighter"},// sky blue
                {21, "<color=#FFFFC0FF>lighter"},// beige
                {22, "<color=#202020FF>darker"},// magenta
                {23, "<color=#FFFFC0FF>lighter"},// turquoise
                {24, "<color=#FFFFC0FF>lighter"},// lilac
                {25, "<color=#202020FF>darker"},// olive
                {26, "<color=#FFFFC0FF>lighter"},// azure
                {27, "<color=#202020FF>darker"},// plum
                {28, "<color=#202020FF>darker"},// jungle
                {29, "<color=#FFFFC0FF>lighter"},// mint
                {30, "<color=#FFFFC0FF>lighter"},// chartreuse
                {31, "<color=#202020FF>darker"},// macau
                {32, "<color=#202020FF>darker"},// tawny
                {33, "<color=#FFFFC0FF>lighter"},// gold
                {34, "<color=#FFFFC0FF>lighter"},// rainbow
                {35, "<color=#FFFFC0FF>lighter"},// ice
                {36, "<color=#FFFFC0FF>lighter"},// copper
                {37, "<color=#202020FF>darker"},// fortegreen
                {38, "<color=#202020FF>darker"},// ink black
                {39, "<color=#202020FF>darker"},// ash gray
                {40, "<color=#FFFFC0FF>lighter"},// snow white
                {41, "<color=#202020FF>darker"},// bloody red
                {42, "<color=#202020FF>darker"},// sunset orange
                {43, "<color=#FFFFC0FF>lighter"},// sunny yellow
                {44, "<color=#FFFFC0FF>lighter"},// juicy lime
                {45, "<color=#202020FF>darker"},// cactus green
                {46, "<color=#FFFFC0FF>lighter"},// heaven cyan
                {47, "<color=#202020FF>darker"},// ocean blue
                {48, "<color=#202020FF>darker"},// galaxy purple
                {49, "<color=#FFFFC0FF>lighter"},// neon pink
                {50, "<color=#202020FF>darker"},// woody brown
                {51, "<color=#FFFFC0FF>lighter"},// black & white
            } :
            new Dictionary<int, string>
            {
                { 0, "<color=#202020FF>ciemniejszy"},// red
                { 1, "<color=#202020FF>ciemniejszy"},// blue
                { 2, "<color=#202020FF>ciemniejszy"},// green
                { 3, "<color=#FFFFC0FF>jasniejszy"},// pink
                { 4, "<color=#FFFFC0FF>jasniejszy"},// orange
                { 5, "<color=#FFFFC0FF>jasniejszy"},// yellow
                { 6, "<color=#202020FF>ciemniejszy"},// black
                { 7, "<color=#FFFFC0FF>jasniejszy"},// white
                { 8, "<color=#202020FF>ciemniejszy"},// purple
                { 9, "<color=#202020FF>ciemniejszy"},// brown
                { 10, "<color=#FFFFC0FF>jasniejszy"},// cyan
                { 11, "<color=#FFFFC0FF>jasniejszy"},// lime
                { 12, "<color=#202020FF>ciemniejszy"},// maroon
                { 13, "<color=#FFFFC0FF>jasniejszy"},// rose
                { 14, "<color=#FFFFC0FF>jasniejszy"},// banana
                { 15, "<color=#202020FF>ciemniejszy"},// gray
                { 16, "<color=#202020FF>ciemniejszy"},// tan
                { 17, "<color=#FFFFC0FF>jasniejszy"},// coral
                { 18, "<color=#202020FF>ciemniejszy"},// watermelon
                { 19, "<color=#202020FF>ciemniejszy"},// chocolate
                { 20, "<color=#FFFFC0FF>jasniejszy"},// sky blue
                { 21, "<color=#FFFFC0FF>jasniejszy"},// beige
                { 22, "<color=#202020FF>ciemniejszy"},// magenta
                { 23, "<color=#FFFFC0FF>jasniejszy"},// turquoise
                { 24, "<color=#FFFFC0FF>jasniejszy"},// lilac
                { 25, "<color=#202020FF>ciemniejszy"},// olive
                { 26, "<color=#FFFFC0FF>jasniejszy"},// azure
                { 27, "<color=#202020FF>ciemniejszy"},// plum
                { 28, "<color=#202020FF>ciemniejszy"},// jungle
                { 29, "<color=#FFFFC0FF>jasniejszy"},// mint
                { 30, "<color=#FFFFC0FF>jasniejszy"},// chartreuse
                { 31, "<color=#202020FF>ciemniejszy"},// macau
                { 32, "<color=#202020FF>ciemniejszy"},// tawny
                { 33, "<color=#FFFFC0FF>jasniejszy"},// gold
                { 34, "<color=#FFFFC0FF>jasniejszy"},// rainbow
                { 35, "<color=#FFFFC0FF>jasniejszy"},// ice
                { 36, "<color=#FFFFC0FF>jasniejszy"},// copper
                { 37, "<color=#202020FF>ciemniejszy"},// fortegreen
                { 38, "<color=#202020FF>ciemniejszy"},// ink black
                { 39, "<color=#202020FF>ciemniejszy"},// ash gray
                { 40, "<color=#FFFFC0FF>jasniejszy"},// snow white
                { 41, "<color=#202020FF>ciemniejszy"},// bloody red
                { 42, "<color=#202020FF>ciemniejszy"},// sunset orange
                { 43, "<color=#FFFFC0FF>jasniejszy"},// sunny yellow
                { 44, "<color=#FFFFC0FF>jasniejszy"},// juicy lime
                { 45, "<color=#202020FF>ciemniejszy"},// cactus green
                { 46, "<color=#FFFFC0FF>jasniejszy"},// heaven cyan
                { 47, "<color=#202020FF>ciemniejszy"},// ocean blue
                { 48, "<color=#202020FF>ciemniejszy"},// galaxy purple
                { 49, "<color=#FFFFC0FF>jasniejszy"},// neon pink
                { 50, "<color=#202020FF>ciemniejszy"},// woody brown
                { 51, "<color=#FFFFC0FF>jasniejszy"},// black & white
            };
            var typeOfColor = colors[br.Killer.GetDefaultOutfit().ColorId];
            return Patches.TranslationPatches.CurrentLanguage == 0 ?
                $"<b>Body Report:</b> The killer appears to be a <b>{typeOfColor}</color> color</b>. (Killed <b>{Math.Round(br.KillAge / 1000)}</b>s ago)" :
                $"<b>Raport Ciala:</b> Zabójca wydaje sie <b>{typeOfColor}</color> kolor</b>. (Zabito <b>{Math.Round(br.KillAge / 1000)}</b>s temu)";
        }
    }
}
