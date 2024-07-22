using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;
using Reactor.Utilities;
using UnityEngine;
using TownOfUs.Extensions;

namespace TownOfUs.CrewmateRoles.HaunterMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
    public class CompleteTask
    {
        public static Sprite Sprite => TownOfUs.Arrow;
        public static void Postfix(PlayerControl __instance)
        {
            if (!__instance.Is(RoleEnum.Haunter)) return;
            var role = Role.GetRole<Haunter>(__instance);

            var taskinfos = __instance.Data.Tasks.ToArray();

            var tasksLeft = taskinfos.Count(x => !x.Complete);

            if (tasksLeft == CustomGameOptions.HaunterTasksRemainingAlert && !role.Caught)
            {
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Haunter))
                {
                    Coroutines.Start(Utils.FlashCoroutine(role.Color));
                    NotificationPatch.Notification(Patches.TranslationPatches.CurrentLanguage == 0 ? "Haunter Is Revealed!" : "Haunter Zostal Ujawniony!", 1000 * CustomGameOptions.NotificationDuration);
                }
                else if (PlayerControl.LocalPlayer.Data.IsImpostor() || (PlayerControl.LocalPlayer.Is(Faction.NeutralKilling) && CustomGameOptions.HaunterRevealsNeutrals))
                {
                    role.Revealed = true;
                    Coroutines.Start(Utils.FlashCoroutine(role.Color));
                    NotificationPatch.Notification(Patches.TranslationPatches.CurrentLanguage == 0 ? "Haunter Is Revealed!" : "Haunter Zostal Ujawniony!", 1000 * CustomGameOptions.NotificationDuration);
                    var gameObj = new GameObject();
                    var arrow = gameObj.AddComponent<ArrowBehaviour>();
                    gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                    var renderer = gameObj.AddComponent<SpriteRenderer>();
                    renderer.sprite = Sprite;
                    arrow.image = renderer;
                    gameObj.layer = 5;
                    role.ImpArrows.Add(arrow);
                }
            }

            if (tasksLeft == 0 && !role.Caught)
            {
                role.CompletedTasks = true;
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Haunter))
                {
                    Coroutines.Start(Utils.FlashCoroutine(Color.white));
                    NotificationPatch.Notification(Patches.TranslationPatches.CurrentLanguage == 0 ? "Haunter Finished Tasks!" : "Haunter Skonczyl Zadania!", 1000 * CustomGameOptions.NotificationDuration);
                }
                else if (PlayerControl.LocalPlayer.Data.IsImpostor() || (PlayerControl.LocalPlayer.Is(Faction.NeutralKilling) && CustomGameOptions.HaunterRevealsNeutrals))
                {
                    Coroutines.Start(Utils.FlashCoroutine(Color.white));
                    NotificationPatch.Notification(Patches.TranslationPatches.CurrentLanguage == 0 ? "Haunter Finished Tasks!" : "Haunter Skonczyl Zadania!", 1000 * CustomGameOptions.NotificationDuration);
                }
            }
        }
    }
}