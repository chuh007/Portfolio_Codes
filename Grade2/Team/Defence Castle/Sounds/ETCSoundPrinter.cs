using Ami.BroAudio;
using Chipmunk.GameEvents;
using UnityEngine;
using UnityEngine.SceneManagement;
using Work.CHUH._01Scripts.Enemies.Wave.GameEvents;


#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
#endif

namespace Work.CHUH._01Scripts.Sounds
{
    public class ETCSoundPrinter : MonoBehaviour
    {
        [SerializeField] private SoundID bgmSound;
        [SerializeField] private SoundID waveStartSound;
        [SerializeField] private SoundID victorySound;
        [SerializeField] private SoundID defeatSound;
        [SerializeField] private SoundID buttonSound;
        
        private void Awake()
        {
            EventBus<WaveStartEvent>.OnEvent += HandleWaveStart;
            EventBus<WaveEndEvent>.OnEvent += HandleWaveEnd;
        }

        private void Start()
        {
            BroAudio.Play(bgmSound);
        }
        
        public void PlayButtonSound()
        {
            BroAudio.Play(buttonSound);
        }


#if UNITY_EDITOR
        [ContextMenu("Auto Button Sound Setting")]
        private void AutoButtonSound()
        {
            var scene = SceneManager.GetActiveScene();
            var roots = scene.GetRootGameObjects();
            
            foreach (var root in roots)
            {
                var buttons = root.GetComponentsInChildren<Button>(true);
                foreach (var btn in buttons)
                {
                    UnityEventTools.AddPersistentListener(btn.onClick, PlayButtonSound);
                    EditorUtility.SetDirty(btn);
                }
            }
            
            EditorSceneManager.MarkSceneDirty(scene);
        }
    #endif
        
        private void HandleWaveStart(WaveStartEvent evt)
        {
            BroAudio.Play(waveStartSound);
        }
        
        private void HandleWaveEnd(WaveEndEvent evt)
        {
            if (evt.EndReason == WaveEndEvent.Reason.AllEnemiesDefeated)
                BroAudio.Play(victorySound);
            else
                BroAudio.Play(defeatSound);
        }
    }
}