using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ScreenScaleSaveLoad : MonoBehaviour
{
    public ScreenScaleData ScreenScaleData;

    private string path = Path.Combine(Application.dataPath, "ScreenScaleData.json");

    private void Awake()
    {
        if(!File.Exists(path))
        {
            Debug.Log("파이리없는");
            string jsonData = JsonUtility.ToJson(ScreenScaleData, true);
            File.WriteAllText(path, jsonData);
        }
    }

    // 저장할때 호출
    public void SaveKeyDataToJson()
    {
        string jsonData = JsonUtility.ToJson(ScreenScaleData,true);
        File.WriteAllText(path, jsonData);
    }

    // awake든 start든 이거 호출해서 값 복구
    public void LoadKeyDataFromJson()
    {
        string jsonData = File.ReadAllText(path);
        ScreenScaleData = JsonUtility.FromJson<ScreenScaleData>(jsonData);
    }
}

[System.Serializable]
public class ScreenScaleData
{
    public int width;
    public int height;
    public int ScreenMode;
}
