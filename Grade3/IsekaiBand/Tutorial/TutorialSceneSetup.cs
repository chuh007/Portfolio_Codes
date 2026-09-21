using _Code.LCH._02.Scripts.Player;
using _Work.CHUH.Code.StageSystem;
using _Work.CHUH.Code.UI;
using _Work.CHUH.Code.WaveSystem;
using UnityEngine;

namespace _Work.CHUH.Code.Tutorial
{
    internal static class TutorialSceneSetup
    {

        public static CraftingBenchToolkitController FindCraftingBench(CraftingBenchToolkitController assigned)
        {
            if (assigned != null) return assigned;
            CraftingBenchToolkitController[] benches = Object.FindObjectsByType<CraftingBenchToolkitController>(
                FindObjectsInactive.Include, FindObjectsSortMode.None);
            return benches.Length > 0 ? benches[0] : null;
        }

        public static void PrepareArena(Transform owner, Player player, float tutorialArenaSize)
        {
            SpriteRenderer background = null;
            SpriteRenderer[] renderers = Object.FindObjectsByType<SpriteRenderer>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);
            foreach (SpriteRenderer renderer in renderers)
            {
                if (renderer == null || renderer.gameObject.name != "StageBackGround")
                    continue;

                background = renderer;
                break;
            }

            var map = owner.GetComponent<TutorialInfiniteMap>();
            if (map == null)
                map = owner.gameObject.AddComponent<TutorialInfiniteMap>();
            map.Initialize(player, background, tutorialArenaSize);
        }

        public static void DisableNormalStageFlow()
        {
            foreach (StageController controller in Object.FindObjectsByType<StageController>(
                         FindObjectsInactive.Include,
                         FindObjectsSortMode.None))
            {
                controller.enabled = false;
            }

            foreach (WaveController controller in Object.FindObjectsByType<WaveController>(
                         FindObjectsInactive.Include,
                         FindObjectsSortMode.None))
            {
                controller.enabled = false;
            }
        }
    }
}
