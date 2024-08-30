using System.Collections.Generic;
using UnityEngine;
using System;

namespace TownOfUs.Roles
{
    public class Medic : Role
    {
        public readonly List<GameObject> Buttons = new List<GameObject>();
        public Dictionary<int, string> LightDarkColors = new Dictionary<int, string>();
        public DateTime StartingCooldown { get; set; }
        public Medic(PlayerControl player) : base(player)
        {
            Name = "Medic";
            ImpostorText = () => "Create A Shield To Protect A Crewmate";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Protect a crewmate with a shield" : "Chron crewmate'a tarcza";
            Color = Patches.Colors.Medic;
            StartingCooldown = DateTime.UtcNow;
            RoleType = RoleEnum.Medic;
            AddToRoleHistory(RoleType);
            ShieldedPlayer = null;

            LightDarkColors.Add(0, "darker"); // Red
            LightDarkColors.Add(1, "darker"); // Blue
            LightDarkColors.Add(2, "darker"); // Green
            LightDarkColors.Add(3, "lighter"); // Pink
            LightDarkColors.Add(4, "lighter"); // Orange
            LightDarkColors.Add(5, "lighter"); // Yellow
            LightDarkColors.Add(6, "darker"); // Black
            LightDarkColors.Add(7, "lighter"); // White
            LightDarkColors.Add(8, "darker"); // Purple
            LightDarkColors.Add(9, "darker"); // Brown
            LightDarkColors.Add(10, "lighter"); // Cyan
            LightDarkColors.Add(11, "lighter"); // Lime
            LightDarkColors.Add(12, "darker"); // Maroon
            LightDarkColors.Add(13, "lighter"); // Rose
            LightDarkColors.Add(14, "lighter"); // Banana
            LightDarkColors.Add(15, "darker"); // Grey
            LightDarkColors.Add(16, "darker"); // Tan
            LightDarkColors.Add(17, "lighter"); // Coral
            LightDarkColors.Add(18, "darker"); // Watermelon
            LightDarkColors.Add(19, "darker"); // Chocolate
            LightDarkColors.Add(20, "lighter"); // Sky Blue
            LightDarkColors.Add(21, "lighter"); // Biege
            LightDarkColors.Add(22, "darker"); // Magenta
            LightDarkColors.Add(23, "lighter"); // Turquoise
            LightDarkColors.Add(24, "lighter"); // Lilac
            LightDarkColors.Add(25, "darker"); // Olive
            LightDarkColors.Add(26, "lighter"); // Azure
            LightDarkColors.Add(27, "darker"); // Plum
            LightDarkColors.Add(28, "darker"); // Jungle
            LightDarkColors.Add(29, "lighter"); // Mint
            LightDarkColors.Add(30, "lighter"); // Chartreuse
            LightDarkColors.Add(31, "darker"); // Macau
            LightDarkColors.Add(32, "darker"); // Tawny
            LightDarkColors.Add(33, "lighter"); // Gold
            LightDarkColors.Add(34, "lighter"); // Rainbow
            LightDarkColors.Add(35, "lighter"); // Ice
            LightDarkColors.Add(36, "lighter"); // Copper
            LightDarkColors.Add(37, "darker"); // Fortegreen
            LightDarkColors.Add(38, "darker"); // Ink Black
            LightDarkColors.Add(39, "darker"); // Ash Gray
            LightDarkColors.Add(40, "lighter"); // Snow White
            LightDarkColors.Add(41, "darker"); // Bloody Red
            LightDarkColors.Add(42, "darker"); // Sunset Orange
            LightDarkColors.Add(43, "lighter"); // Sunny Yellow
            LightDarkColors.Add(44, "lighter"); // Juicy Lime
            LightDarkColors.Add(45, "darker"); // Cactus Green
            LightDarkColors.Add(46, "lighter"); // Heaven Cyan
            LightDarkColors.Add(47, "darker"); // Ocean Blue
            LightDarkColors.Add(48, "darker"); // Galaxy Purple
            LightDarkColors.Add(49, "lighter"); // Neon Pink
            LightDarkColors.Add(50, "darker"); // Woody Brown
            LightDarkColors.Add(51, "lighter"); // Black & White
            LightDarkColors.Add(52, "lighter"); // Buggy Yellow
            LightDarkColors.Add(53, "lighter"); // Black & White
            LightDarkColors.Add(54, "darker"); // Bordeaux
            LightDarkColors.Add(55, "lighter"); // Lavender
            LightDarkColors.Add(56, "lighter"); // Pale Red
            LightDarkColors.Add(57, "lighter"); // Silver
            LightDarkColors.Add(58, "darker"); // Dell Green
            LightDarkColors.Add(59, "lighter"); // Azure Blue
            LightDarkColors.Add(60, "lighter"); // Pale Orange
            LightDarkColors.Add(61, "lighter"); // Corn
            LightDarkColors.Add(62, "darker"); // Husk Gold
            LightDarkColors.Add(63, "lighter"); // Lawn Green
            LightDarkColors.Add(64, "darker"); // Ming
            LightDarkColors.Add(65, "darker"); // Night Blue
            LightDarkColors.Add(66, "lighter"); // Spring Green
            LightDarkColors.Add(67, "darker"); // Grayscale
            LightDarkColors.Add(68, "lighter"); // Fire
            LightDarkColors.Add(69, "darker"); // Galaxy
        }
        public float StartTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - StartingCooldown;
            var num = 10000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public PlayerControl ClosestPlayer;
        public bool UsedAbility { get; set; } = false;
        public PlayerControl ShieldedPlayer { get; set; }
        public PlayerControl exShielded { get; set; }
    }
}