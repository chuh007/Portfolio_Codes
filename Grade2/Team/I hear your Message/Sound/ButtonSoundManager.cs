using Ami.BroAudio;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace A.Work.CHUH._01.Script.Sound
{
    public class ButtonSoundManager : MonoBehaviour
    {
        [SerializeField] private SoundID buttonSound;
        
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
        
    }
}