﻿using HarmonyLib;
using UnityEngine;

namespace TownOfUs.RainbowMod
{
    [HarmonyPatch(typeof(PlayerTab))]
    public static class PlayerTabPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(PlayerTab.OnEnable))]
        public static void OnEnablePostfix(PlayerTab __instance)
        {
            for (int i = 0; i < __instance.ColorChips.Count; i++)
            {
                var colorChip = __instance.ColorChips[i];
                colorChip.transform.localScale *= 0.6f;
                var x = __instance.XRange.Lerp((i % 8) / 7f) + 0.25f;
                var y = __instance.YStart - (i / 7.5f) * 0.3625f;
                colorChip.transform.localPosition = new Vector3(x, y, -1f);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(PlayerTab.Update))]
        public static void UpdatePostfix(PlayerTab __instance)
        {
            for (int i = 0; i < __instance.ColorChips.Count; i++)
            {
                if (RainbowUtils.IsRainbow(i))
                {
                    __instance.ColorChips[i].Inner.SpriteColor = RainbowUtils.Rainbow;
                }
                else if (RainbowUtils.IsGrayscale(i))
                {
                    __instance.ColorChips[i].Inner.SpriteColor = RainbowUtils.Grayscale;
                }
                else if (RainbowUtils.IsFire(i))
                {
                    __instance.ColorChips[i].Inner.SpriteColor = RainbowUtils.Fire;
                }
                else if (RainbowUtils.IsGalaxy(i))
                {
                    __instance.ColorChips[i].Inner.SpriteColor = RainbowUtils.Galaxy;
                }
            }

        }
    }
}
