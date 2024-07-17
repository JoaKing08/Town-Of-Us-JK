using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using Color = UnityEngine.Color;

namespace TownOfUs.CrewmateRoles.AurialMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class SeeAll
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Aurial)) return;

            Aurial s = Role.GetRole<Aurial>(PlayerControl.LocalPlayer);

            if (!PlayerControl.LocalPlayer.CanMove && !s.Loaded) return;

            var button = __instance.KillButton;
            button.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            //if (PlayerControl.LocalPlayer.IsControled()) Utils.Rpc(CustomRPC.ControlCooldown, (byte)s.RadiateTimer(), (byte)CustomGameOptions.RadiateCooldown);
            button.SetCoolDown(s.RadiateTimer(), CustomGameOptions.RadiateCooldown);

            var renderer = button.graphic;
            if (!button.isCoolingDown)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
            }

            if (!s.Loaded) s.Loaded = true;

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (PlayerControl.LocalPlayer == player) continue;

                if (s.NormalVision)
                {
                    continue;
                }

                if (s.SeeDelay() != 0f)
                {
                    if (player.Is(RoleEnum.Mayor))
                    {
                        var mayor = Role.GetRole<Mayor>(player);
                        if (!mayor.Revealed)
                        {
                            ColorChar(player, Color.clear);
                            continue;
                        }
                    }
                    else
                    {
                        ColorChar(player, Color.clear);
                        continue;
                    }
                }

                if (!Check(s, player))
                {
                    ColorChar(player, Color.white, Patches.TranslationPatches.CurrentLanguage == 0 ? "Unknown" : "Nieznany");
                    continue;
                }

                var faction = Role.GetRole(player).Faction;
                if (CustomGameOptions.AurialSeeRoles)
                    ColorChar(player, Role.GetRole(player).Color, player.Is(Faction.Impostors) ? "Impostor" : Role.GetRole(player).Name);
                else switch (faction)
                    {
                        default:
                            ColorChar(player, Color.white, "Unknown");
                            break;
                        case Faction.Crewmates:
                            ColorChar(player, Color.green, "Crewmate");
                            break;
                        case Faction.Impostors:
                            ColorChar(player, Color.red, "Impostor");
                            break;
                        case Faction.NeutralBenign:
                            if (CustomGameOptions.AurialDistinguishNeutrals) ColorChar(player, Color.Lerp(Color.cyan, Color.gray, 0.75f), "Neutral Benign"); else ColorChar(player, Color.gray, "Neutral");
                                break;
                        case Faction.NeutralEvil:
                            if (CustomGameOptions.AurialDistinguishNeutrals) ColorChar(player, Color.Lerp(Color.red, Color.gray, 0.75f), "Neutral Evil"); else ColorChar(player, Color.gray, "Neutral");
                            break;
                        case Faction.NeutralKilling:
                            if (CustomGameOptions.AurialDistinguishNeutrals) { if (player.Is(RoleEnum.Vampire) || player.Is(RoleEnum.JKNecromancer) || player.Is(RoleEnum.Jackal)) ColorChar(player, Color.Lerp(Color.yellow, Color.gray, 0.75f), "Neutral Proselyte"); else ColorChar(player, Color.Lerp(Color.blue, Color.gray, 0.75f), "Neutral Killing"); } else ColorChar(player, Color.gray, "Neutral");
                            break;
                        case Faction.NeutralChaos:
                            if (CustomGameOptions.AurialDistinguishNeutrals) ColorChar(player, Color.Lerp(Color.magenta, Color.gray, 0.75f), "Neutral Chaos"); else ColorChar(player, Color.gray, "Neutral");
                            break;
                        case Faction.NeutralApocalypse:
                            if (CustomGameOptions.GameMode == GameMode.Horseman || CustomGameOptions.AurialDistinguishNeutrals) ColorChar(player, Color.Lerp(Color.black, Color.gray, 0.75f), "Neutral Apocalypse"); else ColorChar(player, Color.gray, "Neutral");
                            break;
                    }
            }
        }

        public static bool Check(Aurial s, PlayerControl p)
        {
            if (p == null) return false;
            if (s.knownPlayerRoles.TryGetValue(p.PlayerId, out var count))
            {
                if (count >= CustomGameOptions.RadiateCount) return true;
            }
            return false;
        }

        public static void ColorChar(PlayerControl p, Color c, string colorblindText = null)
        {
            var fit = p.GetCustomOutfitType();
            if ((fit != CustomPlayerOutfitType.Aurial && fit != CustomPlayerOutfitType.Camouflage && fit != CustomPlayerOutfitType.Swooper) || (fit == CustomPlayerOutfitType.Aurial && p.myRend().color != c))
            {
                p.SetOutfit(CustomPlayerOutfitType.Aurial, new GameData.PlayerOutfit()
                {
                    ColorId = p.GetDefaultOutfit().ColorId,
                    HatId = "",
                    SkinId = "",
                    VisorId = "",
                    NamePlateId = p.GetDefaultOutfit().NamePlateId,
                    PlayerName = " ",
                    PetId = ""
                }); ;
                p.cosmetics.colorBlindText.color = Color.white;
                if (c != Color.clear) p.cosmetics.SetBodyColor(40);
                else p.cosmetics.colorBlindText.color = c;
                if (colorblindText != null)
                {
                    p.cosmetics.colorBlindText.text = colorblindText;
                }
                p.myRend().color = c;
                p.nameText().color = Color.clear;
            }
        }

        public static void AllToNormal()
        {
            foreach (var p in PlayerControl.AllPlayerControls)
            {
                Utils.Unmorph(p);
                p.cosmetics.colorBlindText.color = Color.white;
                p.myRend().color = Color.white;
            }
        }
    }
}
