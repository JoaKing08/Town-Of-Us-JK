using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace TownOfUs.RainbowMod
{
    [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString),
        new[] { typeof(StringNames), typeof(Il2CppReferenceArray<Il2CppSystem.Object>) })]
    public class PatchColours
    {
        public static bool Prefix(ref string __result, [HarmonyArgument(0)] StringNames name)
        {
            var newResult = (int)name switch
            {
                999983 => "Watermelon",
                999984 => "Chocolate",
                999985 => "Sky Blue",
                999986 => "Beige",
                999987 => "Magenta",
                999988 => "Turquoise",
                999989 => "Lilac",
                999990 => "Olive",
                999991 => "Azure",
                999992 => "Plum",
                999993 => "Jungle",
                999994 => "Mint",
                999995 => "Chartreuse",
                999996 => "Macau",
                999997 => "Tawny",
                999998 => "Gold",
                999999 => "Rainbow",
                1000000 => "Ice",
                1000001 => "Copper",
                1000002 => "Fortegreen",
                1000003 => "Ink Black",
                1000004 => "Ash Gray",
                1000005 => "Snow White",
                1000006 => "Bloody Red",
                1000007 => "Sunset Orange",
                1000008 => "Sunny Yellow",
                1000009 => "Juicy Lime",
                1000010 => "Cactus Green",
                1000011 => "Heaven Cyan",
                1000012 => "Ocean Blue",
                1000013 => "Galaxy Purple",
                1000014 => "Neon Pink",
                1000015 => "Woody Brown",
                1000016 => "Black & White",
                _ => null
            };
            if (newResult != null)
            {
                __result = newResult;
                return false;
            }

            return true;
        }
    }
}
