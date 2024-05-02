using HarmonyLib;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace TownOfUs.Patches
{
    //[HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    public static class AchievementPatch
    {
        private static Sprite BackgroundSprite => TownOfUs.AchievementPanelBackground;
        private static Sprite CloseButtonSprite => TownOfUs.CloseButton;
        private static Sprite OpenButtonSprite => TownOfUs.AchievementButton;
        public static bool achievementTabOpen = false;
        public static int currentAchievementTab = 0;
        public static GameObject achievementTab = new GameObject();
        public static GameObject achievementCloseButton = new GameObject();
        public static GameObject achievementOpenButton = new GameObject();
        static void Postfix(PingTracker __instance)
        {
            achievementTab = new GameObject("achievementTab_TownOfUs");
            achievementTab.transform.localScale = new Vector3(1f, 1f, 1f);

            var renderer = achievementTab.AddComponent<SpriteRenderer>();
            renderer.sprite = BackgroundSprite;


            achievementTabOpen = false;
            currentAchievementTab = 0;
            achievementCloseButton = GameObject.Instantiate(GameObject.Find("ExitGameButton"), achievementTab.transform);
            achievementTab.transform.position = new Vector3(0f, 0f, -10f);
            achievementCloseButton.transform.position = new Vector3(2f, 2f, -11f);
            achievementCloseButton.transform.localScale = new Vector3(0.3f, 1f, 1f);
            achievementCloseButton.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = CloseButtonSprite;
            achievementCloseButton.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = CloseButtonSprite;
            PassiveButton closePassiveButton = achievementCloseButton.AddComponent<PassiveButton>();
            //passiveButton.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            closePassiveButton.OnClick = new Button.ButtonClickedEvent();
            closePassiveButton.OnClick.AddListener((UnityEngine.Events.UnityAction)OnClickClose);
        }

        public static void OnClickClose()
        {
            achievementTabOpen = false;
            achievementTab.SetActive(false);
        }
    }
}
