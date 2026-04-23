using System;
using System.Collections;
using _01Scripts.Core.EventSystem;
using _01Scripts.Enemies;
using _01Scripts.Entities;
using _01Scripts.Players;
using Chuh007Lib.Dependencies;
using Code.Core.GameSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace _01Scripts.Interact
{
    public class GhostObject : MonoBehaviour, IInteractable, ISavable
    {
        [SerializeField] private EntityFinderSO playerFinder;
        [SerializeField] private GameEventChannelSO uiChannel;
        [SerializeField] private Collider col;
        [FormerlySerializedAs("meshRenderer")] [SerializeField] private Renderer rendererCompo;
        [SerializeField] private EnemyDataCompo enemtDataCompo;
        
        public string Name { get; private set; }
        public bool IsInteracted { get; private set; }


        private Player _player;

        
        private void Awake()
        {
            enemtDataCompo.useThisData = false;
            Name = "???";
            _player = playerFinder.Target as Player;
        }

        public void Interact()
        {
            _player.GetCompo<CharacterMovement>().CanManualMovement = false;
            IsInteracted = true;
            enemtDataCompo.useThisData = true;
            uiChannel.AddListener<FadeCompleteEvent>(HandleFadeComplete);
            StartCoroutine(GotoBattle());
        }

        private IEnumerator GotoBattle()
        {
            FadeEvent fadeEvt = UIEvents.FadeEvent;
            fadeEvt.isFadeIn = false;
            fadeEvt.fadeTime = 0.5f;
            yield return new WaitForSeconds(0.5f);
            uiChannel.RaiseEvent(fadeEvt);

        }

        private void HandleFadeComplete(FadeCompleteEvent obj)
        {
            col.enabled = false;
            rendererCompo.enabled = false;
            uiChannel.RemoveListener<FadeCompleteEvent>(HandleFadeComplete);
            SceneManager.LoadScene("BattleScene");
        }

        [field: SerializeField] public SaveIdSO SaveID { get; private set; }

        [Serializable]
        public struct GhostObjectSaveData
        {
            public bool isInteracted;
        }
        
        public string GetSaveData()
        {
            GhostObjectSaveData data = new GhostObjectSaveData
            {
                isInteracted = IsInteracted,
            };
            return JsonUtility.ToJson(data);
        }

        public void RestoreData(string loadedData)
        {
            GhostObjectSaveData loadSaveData = JsonUtility.FromJson<GhostObjectSaveData>(loadedData);
            if (loadSaveData.isInteracted)
            {
                col.enabled = false;
                rendererCompo.enabled = false;
            }
        }
    }
}