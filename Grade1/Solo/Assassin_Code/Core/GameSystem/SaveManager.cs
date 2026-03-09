using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.EventSystems;
using UnityEngine;

namespace Code.Core.GameSystem
{
    [Serializable]
    public struct SaveData
    {
        public int saveID;
        public string data;
    }
    
    [Serializable]
    public struct DataCollection
    {
        public List<SaveData> dataList;
    }
    
    public class SaveManager : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO systemChannel;
        [SerializeField] private string saveDataKey = "savedGame";
        
        private List<SaveData> unUsedData = new List<SaveData>();

        private void Awake()
        {
            systemChannel.AddListener<SaveGameEvent>(HandleSaveGame);
            systemChannel.AddListener<LoadGameEvent>(HandleLoadGame);
        }

        private void OnDestroy()
        {
            systemChannel.RemoveListener<SaveGameEvent>(HandleSaveGame);
            systemChannel.RemoveListener<LoadGameEvent>(HandleLoadGame);
        }

        #region SaveGame Logic

        private void HandleSaveGame(SaveGameEvent saveEvt)
        {
            if (saveEvt.isSaveToFile == false)
                SaveGameToPrefs();
        }

        private void SaveGameToPrefs()
        {
            string dataJson = GetDataToSave();
            PlayerPrefs.SetString(saveDataKey, dataJson);
            Debug.Log(dataJson);
        }

        private string GetDataToSave()
        {
            IEnumerable<ISavable> savableObjects =
                FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ISavable>();
            
            List<SaveData> saveDataList = new List<SaveData>();

            foreach (ISavable savable in savableObjects)
            {
                saveDataList.Add(new SaveData{saveID = savable.SaveID.saveID, data = savable.GetSaveData()});
            }
            
            saveDataList.AddRange(unUsedData);
            DataCollection saveDataCollection = new DataCollection { dataList = saveDataList };
            
            return JsonUtility.ToJson(saveDataCollection);
        }

        #endregion

        #region Load Logic

        private void HandleLoadGame(LoadGameEvent loadEvt)
        {
            if (loadEvt.isLoadFromFile == false)
                LoadFromPrefs();
        }

        private void LoadFromPrefs()
        {
            string loadedJson = PlayerPrefs.GetString(saveDataKey, string.Empty);
            RestoreData(loadedJson);
        }

        private void RestoreData(string loadedJson)
        {
            //해당 씬에서 데이터를 받아줄 오브젝트를 다 가져온다.
            IEnumerable<ISavable> savableObjects 
                = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ISavable>();
            
            DataCollection collection = string.IsNullOrEmpty(loadedJson) 
                ? new DataCollection() : JsonUtility.FromJson<DataCollection>(loadedJson);

            unUsedData.Clear(); //현재 가지고 있는 미사용데이터는 클리어.

            if (collection.dataList != null) //로드된 데이터가 존재한다면
            {
                foreach (SaveData saveData in collection.dataList)
                {
                    ISavable target = savableObjects.FirstOrDefault(savable => savable.SaveID.saveID == saveData.saveID);

                    if (target != default)
                    {
                        target.RestoreData(saveData.data);
                    }
                    else
                    {
                        unUsedData.Add(saveData);
                    }
                }
            }
        }

        #endregion


        #region Clear DebugCode

        [ContextMenu("Clear Prefs data")]
        private void ClearPrefs()
        {
            PlayerPrefs.DeleteKey(saveDataKey);
        }

        [ContextMenu("Clear file data")]
        private void ClearFileData()
        {
            //나중에 처리할께.
        }

        #endregion
    }
}