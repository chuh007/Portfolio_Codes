using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SoundSettingSaveLoad : MonoBehaviour
{
    public SoundData SoundData;

    private string path = Path.Combine(Application.dataPath, "soundData.json");

    private void Awake()
    {
        if (!File.Exists(path))
        {
            Debug.Log("파이리없는");
            string jsonData = JsonUtility.ToJson(SoundData, true);
            File.WriteAllText(path, jsonData);
        }
    }

    // 저장할때 호출
    public void SaveSoundDataToJson()
    {
        string jsonData = JsonUtility.ToJson(SoundData,true);
        File.WriteAllText(path, jsonData);
    }

    // awake든 start든 이거 호출해서 값 복구
    public void LoadSoundDataFromJson()
    {
        string jsonData = File.ReadAllText(path);
        SoundData = JsonUtility.FromJson<SoundData>(jsonData);
    }
}

[System.Serializable]
public class SoundData
{
    public float MasterSoundScale;
    public float SFXSoundScale;
    public float BGMSoundScale;

}
