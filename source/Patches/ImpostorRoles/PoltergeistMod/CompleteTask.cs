using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;
using Reactor.Utilities;
using UnityEngine;
using TownOfUs.Extensions;

namespace TownOfUs.ImpostorRoles.PoltergeistMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
    public class CompleteTask
    {
        public static Sprite Sprite => TownOfUs.Arrow;
        public static void Postfix(PlayerControl __instance)
        {
            if (!__instance.Is(RoleEnum.Poltergeist)) return;
            var role = Role.GetRole<Poltergeist>(__instance);

            var taskinfos = __instance.Data.Tasks.ToArray();

            var tasksLeft = taskinfos.Count(x => !x.Complete);

            if (tasksLeft == CustomGameOptions.PoltergeistTasksRemainingAlert && !role.Caught)
            {
                role.Revealed = true;
                Coroutines.Start(Utils.FlashCoroutine(role.Color));
                NotificationPatch.Notification(Patches.TranslationPatches.CurrentLanguage == 0 ? "Poltergeist Is Revealed!" : "Poltergeist Zostal Ujawniony!", 1000 * CustomGameOptions.NotificationDuration);
                var gameObj = new GameObject();
                var arrow = gameObj.AddComponent<ArrowBehaviour>();
                gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                var renderer = gameObj.AddComponent<SpriteRenderer>();
                renderer.sprite = Sprite;
                arrow.image = renderer;
                gameObj.layer = 5;
                role.CrewArrows.Add(arrow);
            }

            if (tasksLeft == 0 && !role.Caught)
            {
                role.CompletedTasks = true;
                if (PlayerControl.LocalPlayer.Data.IsImpostor())
                {
                    PlayerControl.LocalPlayer.SetKillTimer(PlayerControl.LocalPlayer.killTimer / 2);
                }
                Coroutines.Start(Utils.FlashCoroutine(role.Color));
                NotificationPatch.Notification(Patches.TranslationPatches.CurrentLanguage == 0 ? "Poltergeist Finished Tasks!" : "Poltergeist Skonczyl Zadania!", 1000 * CustomGameOptions.NotificationDuration);
            }
        }
    }
}