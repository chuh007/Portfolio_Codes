using UnityEngine;

namespace _01Scripts.FSM
{
    [CreateAssetMenu(fileName = "StateData", menuName = "SO/StateData")]
    public class StateDataSO : ScriptableObject
    {
        public string stateName;
        public string className;
        public string animParamName;

        public int aniamtionHash;

        private void OnValidate()
        {
            aniamtionHash = Animator.StringToHash(animParamName);
        }
    }
}

