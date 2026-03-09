using System;
using TransitionsPlus;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Work.CUH.Code.Transition
{
    public class TransitionSceneSupporter : MonoBehaviour
    {
        [SerializeField] private TransitionAnimator transitionAnimator;

        private void Awake()
        {
            transitionAnimator = GetComponent<TransitionAnimator>();
        }

        private void OnValidate()
        {
            if(transitionAnimator == null) return;
            transitionAnimator.sceneNameToLoad = SceneManager.GetActiveScene().name;
        }
    }
}