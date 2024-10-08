using Reactor.Utilities;
using System;
using System.Collections;
using TownOfUs.Extensions;
using TownOfUs.ImpostorRoles.BomberMod;
using UnityEngine;
using TownOfUs.Modifiers.UnderdogMod;
using Object = UnityEngine.Object;

namespace TownOfUs.Roles.Modifiers
{
    public class Aftermath : Modifier
    {
        public Aftermath(PlayerControl player) : base(player)
        {
            Name = "Aftermath";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Force your killer to use their ability" : "Zmus twojego zab�jce do uzycia umiejetnosci";
            Color = Patches.Colors.Aftermath;
            ModifierType = ModifierEnum.Aftermath;
        }

        public static void ForceAbility(PlayerControl player, PlayerControl corpse)
        {
            if (!player.AmOwner) return;
            DeadBody db = null;
            var bodies = Object.FindObjectsOfType<DeadBody>();
            foreach (var body in bodies)
            {
                try
                {
                    if (body?.ParentId == corpse.PlayerId) { db = body; break; }
                }
                catch
                {
                }
            }
            Coroutines.Start(delay(player, corpse, db));
        }

        private static IEnumerator delay(PlayerControl player, PlayerControl corpse, DeadBody db)
        {
            yield return new WaitForSecondsRealtime(0.2f);
            var role = Role.GetRole(player);

            if (role is Blackmailer blackmailer)
            {
                blackmailer.Blackmailed?.myRend().material.SetFloat("_Outline", 0f);
                if (blackmailer.Blackmailed != null && blackmailer.Blackmailed.Data.IsImpostor())
                {
                    if (blackmailer.Blackmailed.GetCustomOutfitType() != CustomPlayerOutfitType.Camouflage &&
                        blackmailer.Blackmailed.GetCustomOutfitType() != CustomPlayerOutfitType.Swooper)
                        blackmailer.Blackmailed.nameText().color = Patches.Colors.Impostor;
                    else blackmailer.Blackmailed.nameText().color = Color.clear;
                }
                blackmailer.Blackmailed = player;

                Utils.Rpc(CustomRPC.Blackmail, player.PlayerId, player.PlayerId);
            }
            else if (role is Glitch glitch)
            {
                if (glitch.Player.GetCustomOutfitType() != CustomPlayerOutfitType.Morph) glitch.RpcSetMimicked(corpse);
            }
            else if (role is Escapist escapist)
            {
                if (escapist.EscapePoint != new Vector3(0f, 0f, 0f))
                {
                    Utils.Rpc(CustomRPC.Escape, PlayerControl.LocalPlayer.PlayerId, escapist.EscapePoint);
                    escapist.LastEscape = DateTime.UtcNow;
                    Escapist.Escape(escapist.Player);
                }
            }
            else if (role is Grenadier grenadier)
            {
                if (!grenadier.Enabled)
                {
                    Utils.Rpc(CustomRPC.FlashGrenade, PlayerControl.LocalPlayer.PlayerId);
                    grenadier.TimeRemaining = CustomGameOptions.GrenadeDuration;
                    grenadier.Flash();
                }
            }
            else if (role is Janitor janitor)
            {
                Utils.Rpc(CustomRPC.JanitorClean, PlayerControl.LocalPlayer.PlayerId, db.ParentId);

                Coroutines.Start(ImpostorRoles.JanitorMod.Coroutine.CleanCoroutine(db, janitor));
            }
            else if (role is Miner miner)
            {
                var position = PlayerControl.LocalPlayer.transform.position;
                if (CustomGameOptions.InstantVent)
                {
                    var id = ImpostorRoles.MinerMod.PlaceVent.GetAvailableId();
                    Utils.Rpc(CustomRPC.Mine, id, PlayerControl.LocalPlayer.PlayerId, position, position.z + 0.001f);
                    ImpostorRoles.MinerMod.PlaceVent.SpawnVent(id, miner, position, position.z + 0.001f);
                }
                else
                {
                    var ventPrefab = Object.FindObjectOfType<Vent>();
                    var vent = new GameObject("PlannedVent");
                    vent.transform.parent = ventPrefab.transform.parent;
                    var renderer = vent.AddComponent<SpriteRenderer>();
                    var sourceRenderer = ventPrefab.myRend;
                    var collider = vent.AddComponent<BoxCollider2D>();
                    var sourceCollider = ventPrefab.GetComponent<BoxCollider2D>();
                    renderer.sprite = sourceRenderer.sprite;
                    renderer.color = sourceRenderer.color * new Color(1f, 1f, 1f, 0.5f);
                    renderer.sortingLayerID = sourceRenderer.sortingLayerID;
                    renderer.sortingOrder = sourceRenderer.sortingOrder;
                    renderer.size = sourceRenderer.size;
                    collider.size = sourceCollider.size;
                    collider.offset = sourceCollider.offset;
                    collider.isTrigger = sourceCollider.isTrigger;
                    vent.transform.position = position + new Vector3(0f, 0f, 0.001f);
                    vent.transform.localScale = ventPrefab.transform.localScale;
                    miner.PlannedVents.Add(vent);
                }
                miner.LastMined = DateTime.UtcNow;
            }
            else if (role is Morphling morphling)
            {
                if (morphling.Player.GetCustomOutfitType() != CustomPlayerOutfitType.Morph)
                {
                    Utils.Rpc(CustomRPC.Morph, PlayerControl.LocalPlayer.PlayerId, corpse.PlayerId);
                    morphling.TimeRemaining = CustomGameOptions.MorphlingDuration;
                    if (morphling.SampledPlayer == null) morphling._morphButton.graphic.sprite = TownOfUs.MorphSprite;
                    morphling.SampledPlayer = corpse;
                    morphling.MorphedPlayer = corpse;
                    Utils.Morph(morphling.Player, corpse, true);
                }
            }
            else if (role is Swooper swooper)
            {
                if (swooper.Player.GetCustomOutfitType() != CustomPlayerOutfitType.Swooper)
                {
                    Utils.Rpc(CustomRPC.Swoop, PlayerControl.LocalPlayer.PlayerId);
                    swooper.TimeRemaining = CustomGameOptions.SwoopDuration;
                    swooper.Swoop();
                }
            }
            else if (role is Undertaker undertaker)
            {
                if (undertaker.CurrentlyDragging)
                {
                    Vector3 position = PlayerControl.LocalPlayer.transform.position;

                    if (Patches.SubmergedCompatibility.isSubmerged())
                    {
                        if (position.y > -7f)
                        {
                            position.z = 0.0208f;
                        }
                        else
                        {
                            position.z = -0.0273f;
                        }
                    }

                    position.y -= 0.3636f;

                    Utils.Rpc(CustomRPC.Drop, PlayerControl.LocalPlayer.PlayerId, position, position.z);

                    var body = undertaker.CurrentlyDragging;
                    undertaker.CurrentlyDragging = null;
                    body.transform.position = position;
                }

                Utils.Rpc(CustomRPC.Drag, PlayerControl.LocalPlayer.PlayerId, db.ParentId);
                undertaker.CurrentlyDragging = db;
                ImpostorRoles.UndertakerMod.KillButtonTarget.SetTarget(undertaker._dragDropButton, null, undertaker);
                undertaker._dragDropButton.graphic.sprite = TownOfUs.DropSprite;

            }
            else if (role is Venerer venerer)
            {
                if (!venerer.Enabled)
                {
                    Utils.Rpc(CustomRPC.Camouflage, PlayerControl.LocalPlayer.PlayerId, venerer.Kills);
                    venerer.TimeRemaining = CustomGameOptions.AbilityDuration;
                    venerer.KillsAtStartAbility = venerer.Kills;
                    venerer.Ability();
                }
            }
            else if (role is Bomber bomber)
            {
                bomber.Detonated = false;
                var pos = PlayerControl.LocalPlayer.transform.position;
                pos.z += 0.001f;
                bomber.DetonatePoint = pos;
                bomber.PlantButton.graphic.sprite = TownOfUs.DetonateSprite;
                bomber.TimeRemaining = CustomGameOptions.DetonateDelay;
                bomber.PlantButton.SetCoolDown(bomber.TimeRemaining, CustomGameOptions.DetonateDelay);
                if (PlayerControl.LocalPlayer.Is(ModifierEnum.Underdog))
                {
                    var lowerKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown - CustomGameOptions.UnderdogKillBonus;
                    var normalKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
                    var upperKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + CustomGameOptions.UnderdogKillBonus;
                    PlayerControl.LocalPlayer.SetKillTimer(((PerformKill.LastImp() ? lowerKC : (PerformKill.IncreasedKC() ? normalKC : upperKC)) * (Utils.PoltergeistTasks() ? CustomGameOptions.PoltergeistKCdMult : 1f)) + CustomGameOptions.DetonateDelay);
                }
                else PlayerControl.LocalPlayer.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown * (Utils.PoltergeistTasks() ? CustomGameOptions.PoltergeistKCdMult : 1f) + CustomGameOptions.DetonateDelay);
                DestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(null);
                bomber.Bomb = BombExtentions.CreateBomb(pos);
            }
            else if (role is Poisoner poisoner)
            {
                if (poisoner.PoisonedPlayer == null)
                {
                    Coroutines.Start(Utils.FlashCoroutine(Color.red));
                    NotificationPatch.Notification(Patches.TranslationPatches.CurrentLanguage == 0 ? "You Have Been Poisoned!" : "Zostales Otruty!", 1000 * CustomGameOptions.NotificationDuration);
                    poisoner.PoisonedPlayer = player;
                    poisoner.PoisonTime = DateTime.UtcNow;
                }
            }
            else if (role is Sniper sniper)
            {
                if (sniper.AimedPlayer == null) sniper.AimedPlayer = player;
                else
                {
                    if (sniper.AimedPlayer.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, sniper.AimedPlayer.PlayerId, (byte)sniper.RoleType, (byte)1);

                    if (!sniper.AimedPlayer.Is(RoleEnum.Pestilence) && !sniper.AimedPlayer.Is(RoleEnum.Famine) && !sniper.AimedPlayer.Is(RoleEnum.War) && !sniper.AimedPlayer.Is(RoleEnum.Death) && !sniper.AimedPlayer.IsShielded() && !sniper.AimedPlayer.IsVesting() && !sniper.AimedPlayer.IsOnAlert() && !sniper.AimedPlayer.IsProtected())
                    {
                        Utils.RpcMultiMurderPlayer(PlayerControl.LocalPlayer, sniper.AimedPlayer);
                        Utils.Rpc(CustomRPC.KillAbilityUsed, sniper.AimedPlayer.PlayerId);
                    }
                    sniper.AimedPlayer = null;
                    Utils.Rpc(CustomRPC.Shoot, player.PlayerId, sniper.AimedPlayer.Data.Disconnected ? byte.MaxValue : sniper.AimedPlayer.PlayerId);
                }
            }
        }
    }
}