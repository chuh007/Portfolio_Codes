using _01Scripts.Core.EventSystem;
using _01Scripts.Entities;
using UnityEngine;

namespace _01Scripts.Core.GameSystem
{
    //모든 스크립트보다 빨리 실행돼.
    [DefaultExecutionOrder(-20)]
    public class SceneTransitionManager : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO uiChannel;
        [SerializeField] private EntityFinderSO playerFinder;

        private void Awake()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            Debug.Assert(player != null, "player does not exist in this scene");
            
            playerFinder.SetTarget(player.GetComponent<Entity>());
        }

        private void Start()
        {
            FadeEvent fadeEvt = UIEvents.FadeEvent;
            fadeEvt.isFadeIn = true;
            fadeEvt.fadeTime = 0.5f;
            
            uiChannel.RaiseEvent(fadeEvt);
        }
    }
}