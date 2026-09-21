using _Work.CHUH.Code.WaveSystem;
using Reflex.Attributes;
using TMPro;
using UnityEngine;

namespace _Work.CHUH.Code.UI.Temp
{
    public class TimeText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        
        [Inject] private WaveController _waveController;
        
        private int _lastSecond = -1;
        
        private void Update()
        {
            int currentSecond = (int)_waveController.ElapsedTime;

            if (currentSecond != _lastSecond)
            {
                _lastSecond = currentSecond;
                int minute = (int)_waveController.ElapsedTime / 60;
                int second = (int)_waveController.ElapsedTime % 60;
                text.SetText($"{minute:D2}:{second:D2}");
            }
        }
    }
}