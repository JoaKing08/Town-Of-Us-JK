using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System;
using System.Linq;
using TownOfUs.Extensions;
using UnityEngine;

namespace TownOfUs
{
    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.InitializeOptions))]
    public class EnableMapImps
    {
        private static void Prefix(ref GameSettingMenu __instance)
        {
            __instance.HideForOnline = new Il2CppReferenceArray<Transform>(0);
        }
    }

    [HarmonyPatch(typeof(ImpostorRole), nameof(ImpostorRole.CanUse))]
    public class ImpTasks
    {
        private static bool Prefix(ImpostorRole __instance, ref IUsable usable, ref bool __result)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.CultistSnitch) && !PlayerControl.LocalPlayer.Is(ModifierEnum.Tasker)) return true;
            __result = true;
            return false;
        }
    }
    [HarmonyPatch(typeof(ImpostorGhostRole), nameof(ImpostorGhostRole.CanUse))]
    public class DeadImpTasks
    {
        private static bool Prefix(ImpostorRole __instance, ref IUsable usable, ref bool __result)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Poltergeist)) return true;
            __result = true;
            return false;
        }
    }

    /*[HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    class OpenSaboMapPatch
    {
        static void Postfix(HudManager __instance)
        {
            __instance.MapButton.OnClick.RemoveAllListeners();
            __instance.MapButton.OnClick.AddListener((Action)(() =>
            {
                bool canSabo = (PlayerControl.LocalPlayer.Data.Role.IsImpostor || ((PlayerControl.LocalPlayer.Is(Faction.NeutralKilling) || PlayerControl.LocalPlayer.Is(Faction.NeutralKilling)) && (CustomGameOptions.AllowSaboNeutKillers == AllowSabotage.On || (CustomGameOptions.AllowSaboNeutKillers == AllowSabotage.AfterImpsDeath && !PlayerControl.AllPlayerControls.ToArray().Any(x => x.Data.IsImpostor() && !x.Data.IsDead && !x.Data.Disconnected))))) && !MeetingHud.Instance;
                if (MapBehaviour.Instance && MapBehaviour.Instance.gameObject.activeSelf)
                {
                    MapBehaviour.Instance.Close();
                    return;
                }
                if (__instance.IsIntroDisplayed)
                {
                    return;
                }
                if (!ShipStatus.Instance)
                {
                    return;
                }
                __instance.InitMap();
                if (canSabo)
                    MapBehaviour.Instance.ShowSabotageMap();
                else
                    MapBehaviour.Instance.ShowNormalMap();
            }));
        }
    }
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class DeadImpSabotage
    {
        private static bool Prefix(ImpostorRole __instance, ref IUsable usable, ref bool __result)
        {
            bool isImp = PlayerControl.LocalPlayer.Data.Role.IsImpostor;
            bool isNk = PlayerControl.LocalPlayer.Is(Faction.NeutralKilling) || PlayerControl.LocalPlayer.Is(Faction.NeutralApocalypse);
            if (!isImp && !isNk) return;
            bool impsLoseCritical = isImp && NeutralKiller.LoseCritSabo && MapOptions.Allimpsdead;
            bool nkCanSabo = nK && player.NeutralKillerCanSabo();
            bool loseCritical = impsLoseCritical || nkCanSabo;

            bool impsLoseDoors = isImp && NeutralKiller.LoseDoorSabo && MapOptions.Allimpsdead;

            if (MapBehaviour.Instance != null && MapBehaviour.Instance.IsOpen)
            {
                switch (GameOptionsManager.Instance.currentGameOptions.MapId)
                {
                    case 0:
                        // Map sabotage
                        GameObject minimapSabotage = GameObject.Find("Main Camera/Hud/ShipMap(Clone)/InfectedOverlay"); // Skeld
                        if (loseCritical)
                        {
                            minimapSabotage.transform.GetChild(4).gameObject.SetActive(false); // Sabotage o2
                            minimapSabotage.transform.GetChild(8).gameObject.SetActive(false); // Sabotage reactor
                        }
                        if (impsLoseDoors)
                        {
                            minimapSabotage.transform.GetChild(0).gameObject.SetActive(false); // Cafeteria Doors
                            minimapSabotage.transform.GetChild(2).gameObject.SetActive(false); // Medbay Doors
                            minimapSabotage.transform.GetChild(3).GetChild(0).gameObject.SetActive(false); // Electrical Doors
                            minimapSabotage.transform.GetChild(5).gameObject.SetActive(false); // Left Engine Doors
                            minimapSabotage.transform.GetChild(6).gameObject.SetActive(false); // Right Engine Doors
                            minimapSabotage.transform.GetChild(7).gameObject.SetActive(false); // Storage Doors
                            minimapSabotage.transform.GetChild(9).gameObject.SetActive(false); // Security Doors
                        }
                        break;
                    case 1:
                        GameObject minimapSabotageMira = GameObject.Find("Main Camera/Hud/HqMap(Clone)/InfectedOverlay"); // Mira
                        if (loseCritical)
                        {
                            minimapSabotageMira.transform.GetChild(2).gameObject.SetActive(false);
                            minimapSabotageMira.transform.GetChild(3).gameObject.SetActive(false);
                        }
                        break;
                    case 2:
                        GameObject minimapSabotagePolus = GameObject.Find("Main Camera/Hud/PbMap(Clone)/InfectedOverlay"); // Polus
                        if (loseCritical)
                        {
                            minimapSabotagePolus.transform.GetChild(6).GetChild(0).gameObject.SetActive(false);
                        }
                        if (impsLoseDoors)
                        {
                            minimapSabotagePolus.transform.GetChild(0).GetChild(1).gameObject.SetActive(false); // Sabotage reactor
                            minimapSabotagePolus.transform.GetChild(1).GetChild(1).gameObject.SetActive(false); // Sabotage reactor
                            minimapSabotagePolus.transform.GetChild(2).gameObject.SetActive(false); // Sabotage reactor
                            minimapSabotagePolus.transform.GetChild(3).gameObject.SetActive(false); // Sabotage reactor
                            minimapSabotagePolus.transform.GetChild(4).gameObject.SetActive(false); // Sabotage reactor
                            minimapSabotagePolus.transform.GetChild(5).gameObject.SetActive(false); // Sabotage reactor
                            minimapSabotagePolus.transform.GetChild(6).GetChild(1).gameObject.SetActive(false); // Sabotage reactor
                        }
                        break;
                    case 3:
                        GameObject minimapSabotageDleks = GameObject.Find("Main Camera/Hud/ShipMap(Clone)/InfectedOverlay"); // dlekS
                        if (loseCritical)
                        {
                            minimapSabotageDleks.transform.GetChild(4).gameObject.SetActive(false);
                            minimapSabotageDleks.transform.GetChild(8).gameObject.SetActive(false);
                        }
                        if (impsLoseDoors)
                        {
                            minimapSabotageDleks.transform.GetChild(0).gameObject.SetActive(false); // Cafeteria Doors
                            minimapSabotageDleks.transform.GetChild(2).gameObject.SetActive(false); // Medbay Doors
                            minimapSabotageDleks.transform.GetChild(3).GetChild(0).gameObject.SetActive(false); // Electrical Doors
                            minimapSabotageDleks.transform.GetChild(5).gameObject.SetActive(false); // Left Engine Doors
                            minimapSabotageDleks.transform.GetChild(6).gameObject.SetActive(false); // Right Engine Doors
                            minimapSabotageDleks.transform.GetChild(7).gameObject.SetActive(false); // Storage Doors
                            minimapSabotageDleks.transform.GetChild(9).gameObject.SetActive(false); // Security Doors
                        }
                        break;
                    case 4:
                        GameObject minimapSabotageAirship = GameObject.Find("Main Camera/Hud/AirshipMap(Clone)/InfectedOverlay"); // Airship
                        if (loseCritical)
                        {
                            minimapSabotageAirship.transform.GetChild(3).gameObject.SetActive(false);
                        }
                        if (impsLoseDoors)
                        {
                            minimapSabotageAirship.transform.GetChild(0).GetChild(1).gameObject.SetActive(false); // Comms Doors
                            minimapSabotageAirship.transform.GetChild(2).gameObject.SetActive(false); // MainHall Doors
                            minimapSabotageAirship.transform.GetChild(4).gameObject.SetActive(false); // Records Doors
                            minimapSabotageAirship.transform.GetChild(5).gameObject.SetActive(false); // Brig Doors
                            minimapSabotageAirship.transform.GetChild(6).gameObject.SetActive(false); // Kitchen Doors
                            minimapSabotageAirship.transform.GetChild(7).gameObject.SetActive(false); // Medbay Doors
                        }
                        break;
                }
            }
        }
    }*/
}