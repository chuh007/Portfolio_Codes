using UnityEngine;
#if UNITY_EDITOR
using _Work.CHUH.Code.Enemies.Boss;
using UnityEngine.InputSystem;
#endif

namespace _Code.LCH._02.Scripts.Level
{
    public class LevelSystemCompo : MonoBehaviour
    {
        [SerializeField] private LevelDataSO levelData;

        [Header("디버그")]
        [SerializeField] private bool  enableDebugKeys = true;
        [SerializeField] private float debugExpAmount  = 50f;

        public LevelSystem LevelSystem { get; private set; }

        private void Awake()
        {
            LevelSystem = new LevelSystem(levelData);
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (!enableDebugKeys) return;
            if (Keyboard.current == null) return;
            
            if (Keyboard.current.lKey.wasPressedThisFrame
                && UnityEngine.Object.FindFirstObjectByType<BossPhaseController>() == null)
            {
                LevelSystem.AddExp(debugExpAmount);
                Debug.Log($"[Debug] +{debugExpAmount} EXP, Lv.{LevelSystem.CurrentLevel}, {LevelSystem.CurrentExp:F1}/{LevelSystem.RequiredExp:F1}");
            }
            
            if (Keyboard.current.kKey.wasPressedThisFrame)
            {
                float needed = LevelSystem.RequiredExp - LevelSystem.CurrentExp + 1f;
                LevelSystem.AddExp(needed);
                Debug.Log($"[Debug] 레벨업! Lv.{LevelSystem.CurrentLevel}");
            }
            
            if (Keyboard.current.jKey.wasPressedThisFrame)
            {
                LevelSystem.AddExp(debugExpAmount * 20f);
                Debug.Log($"[Debug] +{debugExpAmount * 20f} EXP, Lv.{LevelSystem.CurrentLevel}");
            }
        }
#endif
    }
}
