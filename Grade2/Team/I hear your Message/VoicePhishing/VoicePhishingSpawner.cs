using System.Collections;
using System.Collections.Generic;
using Scripts.Cores;
using UnityEngine;
using Random = UnityEngine.Random;

namespace A.Work.CHUH._01.Script.VoicePhishing
{
    public class VoicePhishingSpawner : MonoBehaviour
    {
        [SerializeField] private Canvas spawnCanvas;
        [SerializeField] private VoicePhishing voicePhishingPrefab;
        [SerializeField] private PhishingDataList phishingDataList;
        [SerializeField] private DaySystem daySystem;
        [SerializeField] private float spawnCooldown;
        [SerializeField] private List<VoicePhishingSO> voicePhishingData;
        
        #region Test

        [ContextMenu("Spawn")]
        public void TestSpawn()
        {
            Spawn();
        }

        #endregion
        
        private void Start()
        {
            foreach (var data in phishingDataList.voicePhishingData)
            {
                voicePhishingData.Add(data);
            }

            GameTypeSingleton.Instance.OnGameTypeChanged += HandleGameTypeChange;
            StartCoroutine(TrySpawn());
        }

        private void HandleGameTypeChange(GameType type)
        {
            if (type == GameType.Game)
            {
                StartCoroutine(TrySpawn());
            }
        }

        private IEnumerator TrySpawn()
        {
            yield return new WaitForSeconds(spawnCooldown);
            if (GameTypeSingleton.Instance.GameType == GameType.Game)
                Spawn();
        }
        
        public void Spawn()
        {
            VoicePhishingSO data = voicePhishingData
                [Random.Range(0, voicePhishingData.Count)];
            voicePhishingData.Remove(data);
            VoicePhishing voicePhishing = Instantiate(voicePhishingPrefab, spawnCanvas.transform);
            voicePhishing.EndEvent += HandleEnd;
            voicePhishing.SetData(data, daySystem);
            voicePhishing.PopUp();
        }

        private void HandleEnd()
        {
            StartCoroutine(TrySpawn());
        }
        
        public void SetSpawnCooldown(float cooldown)
        {
            spawnCooldown = cooldown;
        }
    }
}