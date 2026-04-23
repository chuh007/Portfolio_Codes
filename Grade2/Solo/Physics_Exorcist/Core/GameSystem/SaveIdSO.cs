using UnityEngine;

namespace Code.Core.GameSystem
{
    [CreateAssetMenu(fileName = "SaveIdSO", menuName = "SO/System/SaveId", order = 0)]
    public class SaveIdSO : ScriptableObject
    {
        public int saveID;
        public string saveName;
        [TextArea]
        public string description; //나중에 나를 위한 설명자료
    }
}