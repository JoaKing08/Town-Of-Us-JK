using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;
using Reactor.Utilities;
using UnityEngine;
using TownOfUs.Extensions;

namespace TownOfUs.ApocalypseRoles.HarbingerMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
    public class CompleteTask
    {
        public static Sprite Sprite => TownOfUs.Arrow;
        public static void Postfix(PlayerControl __instance)
        {
            if (!__instance.Is(RoleEnum.Harbinger)) return;
            var role = Role.GetRole<Harbinger>(__instance);

            var taskinfos = __instance.Data.Tasks.ToArray();

            var tasksLeft = taskinfos.Count(x => !x.Complete);

            if (tasksLeft == CustomGameOptions.HarbingerTasksRemainingAlert && !role.Caught)
            {
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Harbinger))
                {
                    Coroutines.Start(Utils.FlashCoroutine(role.Color));
                    NotificationPatch.Notification(Patches.TranslationPatches.CurrentLanguage == 0 ? "Harbinger Is Revealed!" : "Harbinger Zostal Ujawniony!", 1000 * CustomGameOptions.NotificationDuration);
                }
                else
                {
                    role.Revealed = true;
                    Coroutines.Start(Utils.FlashCoroutine(role.Color));
                    NotificationPatch.Notification(Patches.TranslationPatches.CurrentLanguage == 0 ? "Harbinger Is Revealed!" : "Harbinger Zostal Ujawniony!", 1000 * CustomGameOptions.NotificationDuration);
                    var gameObj = new GameObject();
                    var arrow = gameObj.AddComponent<ArrowBehaviour>();
                    gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                    var renderer = gameObj.AddComponent<SpriteRenderer>();
                    renderer.sprite = Sprite;
                    arrow.image = renderer;
                    gameObj.layer = 5;
                    role.CrewArrows.Add(arrow);
                }
            }

            if (tasksLeft == 0 && !role.Caught)
            {
                role.CompletedTasks = true;
                Coroutines.Start(Utils.FlashCoroutine(role.Color));
                NotificationPatch.Notification(Patches.TranslationPatches.CurrentLanguage == 0 ? "Harbinger Finished Tasks!" : "Harbinger Skonczyl Zadania!", 1000 * CustomGameOptions.NotificationDuration);
            }
        }
    }
}