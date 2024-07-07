using HarmonyLib;
using UnityEngine;
using System;
using Object = UnityEngine.Object;
using AmongUs.GameOptions;
using TownOfUs.Roles;

namespace TownOfUs.Patches
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudUpdate
    {
        private static GameObject ZoomButton;
        public static bool Zooming;
        private static Vector3 Pos;

        public static void Postfix(HudManager __instance)
        {
            if (!ZoomButton)
            {
                ZoomButton = Object.Instantiate(__instance.MapButton.gameObject, __instance.MapButton.transform.parent);
                ZoomButton.GetComponent<PassiveButton>().OnClick = new();
                ZoomButton.GetComponent<PassiveButton>().OnClick.AddListener(new Action(Zoom));
            }
            if ((PlayerControl.LocalPlayer.Is(RoleEnum.Aurial) && CustomGameOptions.AurialSeeThrough && (!ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>().IsActive || PlayerControl.LocalPlayer.Is(ModifierEnum.Torch))) || (PlayerControl.LocalPlayer.Is(RoleEnum.Lookout) && Role.GetRole<Lookout>(PlayerControl.LocalPlayer).Watching == true) || PlayerControl.LocalPlayer.Data.IsDead) DestroyableSingleton<HudManager>.Instance.ShadowQuad.gameObject.SetActive(false);
            else DestroyableSingleton<HudManager>.Instance.ShadowQuad.gameObject.SetActive(true);

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Aurial) && !PlayerControl.LocalPlayer.Data.IsDead && (!ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>().IsActive || PlayerControl.LocalPlayer.Is(ModifierEnum.Torch)) && !Utils.IsMeeting)
            {
                Camera.main.orthographicSize = 3f * CustomGameOptions.AurialVisionMultiplier;

                foreach (var cam in Camera.allCameras)
                {
                    if (cam?.gameObject.name == "UI Camera")
                        cam.orthographicSize = 3f * CustomGameOptions.AurialVisionMultiplier;
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Lookout) && Role.GetRole<Lookout>(PlayerControl.LocalPlayer).Watching == true && !Utils.IsMeeting)
            {
                Camera.main.orthographicSize = 3f * CustomGameOptions.WatchVisionMultiplier;
            }
            else if (Camera.main.orthographicSize != 12f)
            {
                Camera.main.orthographicSize = 3f;

                foreach (var cam in Camera.allCameras)
                {
                    if (cam?.gameObject.name == "UI Camera")
                        cam.orthographicSize = 3f;
                }
            }

            Pos = __instance.MapButton.transform.localPosition + new Vector3(0.02f, -0.66f, 0f);
            var dead = false;
            if (Utils.ShowDeadBodies && PlayerControl.LocalPlayer.Data.IsDead)
            {
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Haunter))
                {
                    var haunter = Role.GetRole<Haunter>(PlayerControl.LocalPlayer);
                    if (haunter.Caught) dead = true;
                }
                else if (PlayerControl.LocalPlayer.Is(RoleEnum.Phantom))
                {
                    var phantom = Role.GetRole<Phantom>(PlayerControl.LocalPlayer);
                    if (phantom.Caught) dead = true;
                }
                else if (PlayerControl.LocalPlayer.Is(RoleEnum.Poltergeist))
                {
                    var poltergeist = Role.GetRole<Poltergeist>(PlayerControl.LocalPlayer);
                    if (poltergeist.Caught) dead = true;
                }
                else dead = true;
            }
            ZoomButton.SetActive(!MeetingHud.Instance && dead && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started
                && GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.Normal);
            ZoomButton.transform.localPosition = Pos;
            ZoomButton.GetComponent<SpriteRenderer>().sprite = Zooming ? TownOfUs.ZoomPlusButton : TownOfUs.ZoomMinusButton;
        }

        public static void Zoom()
        {
            Zooming = !Zooming;
            var size = Zooming ? 12f : 3f;
            Camera.main.orthographicSize = size;

            foreach (var cam in Camera.allCameras)
            {
                if (cam?.gameObject.name == "UI Camera")
                    cam.orthographicSize = size;
            }

            ResolutionManager.ResolutionChanged.Invoke((float)Screen.width / Screen.height, Screen.width, Screen.height, Screen.fullScreen);
        }

        public static void ZoomStart()
        {
            var size = Zooming ? 12f : 3f;
            Camera.main.orthographicSize = size;

            foreach (var cam in Camera.allCameras)
            {
                if (cam?.gameObject.name == "UI Camera")
                    cam.orthographicSize = size;
            }

            ResolutionManager.ResolutionChanged.Invoke((float)Screen.width / Screen.height, Screen.width, Screen.height, Screen.fullScreen);
        }
    }
}