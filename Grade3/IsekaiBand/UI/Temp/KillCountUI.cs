using _Work.CHUH.Code.WaveSystem;
using Reflex.Attributes;
using TMPro;
using UnityEngine;

namespace _Work.CHUH.Code.UI.Temp
{
    public class KillCountUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        
        [Inject] private WaveController _waveController;

        private void Start()
        {
            _waveController.OnKillCountChanged += HandleKillCount;
        }

        private void HandleKillCount(int value)
        {
            text.SetText(value.ToString());
        }
    }
}