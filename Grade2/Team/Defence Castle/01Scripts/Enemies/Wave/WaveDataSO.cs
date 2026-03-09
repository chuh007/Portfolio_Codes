using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Work.CHUH._01Scripts.Enemies.Wave
{
    [CreateAssetMenu(fileName = "WaveData", menuName = "SO/Wave/Data", order = 0)]
    public class WaveDataSO : ScriptableObject
    { 
        public List<WaveSO> waveList;
    }
}