using System.Collections.Generic;
using _Work.CHUH.Code.Core;
using Chuh007Lib.Bus;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace _Work.CHUH.Code.StageSystem
{
    /// <summary>
    /// 메인 화면에 넣는거 생각중
    /// 여기서 StageData 선택해서 씬 넘겨서 하는 느낌
    /// </summary>
    public class StageSelector : MonoBehaviour
    {
        [SerializeField] private List<StageDataSO> stageDataList = new List<StageDataSO>();
        [SerializeField] private StageDataSenderSO stageDataSenderSO;
        
        private int _currentStageIndex = 0;
        
#if UNITY_EDITOR
        private void Update()
        {
            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                stageDataSenderSO.Data = stageDataList[_currentStageIndex];
                SceneManager.LoadScene("EnemyTestScene");
            }
        }
#endif
    }
}