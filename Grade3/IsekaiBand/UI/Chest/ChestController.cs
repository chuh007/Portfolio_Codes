using System;
using System.Collections;
using _Work.CHUH.Code.Audio;
using _Work.CHUH.Code.Core.Events;
using Chuh007Lib.Bus;
using UnityEngine;
using UnityEngine.UI;

namespace _Work.CHUH.Code.UI.Chest
{
    public class ChestController : MonoBehaviour
    {
        [SerializeField] private ChestOpenCutScene chestOpenCutScene;
        [SerializeField] private LootRevealUI chestUpgradeUI;
        [SerializeField] private Button backButton;
        private int _upgradeCount;
        private bool _isOpening;
        private bool _canClose;
        private bool _canClickBackButton;
        private bool _isTimePaused;
        private float _previousTimeScale = 1f;
        private Coroutine _upgradeOpenRoutine;
        private Coroutine _chestDropRoutine;
        
        private void Awake()
        {
            Bus<UpgradeChestEvent>.OnEvent += HandleChestOn;
            backButton.onClick.AddListener(ClickBtn);
            UiButtonSoundFeedback.ExcludeUguiButton(backButton);
            chestOpenCutScene.OnChestUpgrade += HandleUpgrade;
            chestUpgradeUI.OnOpenCompleted += HandleUpgradeOpened;
            chestUpgradeUI.OnCloseRequested += Close;
            gameObject.SetActive(false);
        }
        
        private void OnDestroy()
        {
            Bus<UpgradeChestEvent>.OnEvent -= HandleChestOn;
            chestOpenCutScene.OnChestUpgrade -= HandleUpgrade;
            chestUpgradeUI.OnOpenCompleted -= HandleUpgradeOpened;
            chestUpgradeUI.OnCloseRequested -= Close;
            ResumeGameTime();
        }
        
        private void ClickBtn()
        {
            if (!_canClickBackButton) return;

            if (_canClose)
            {
                Close();
                return;
            }

            if (_isOpening) return;

            _canClickBackButton = false;
            SetBackButtonEnabled(false);
            _isOpening = true;
            Bus<SoundPlayEvent>.Raise(new SoundPlayEvent(
                SoundKeys.ChestOpen,
                SoundType.SFX));
            chestOpenCutScene.OpenChest();
        }
        
        private void HandleUpgrade()
        {
            if (_upgradeOpenRoutine != null)
                StopCoroutine(_upgradeOpenRoutine);

            _upgradeOpenRoutine = StartCoroutine(OpenUpgradeAfterDelay());
        }

        private IEnumerator OpenUpgradeAfterDelay()
        {
            yield return new WaitForSecondsRealtime(1f);

            _upgradeOpenRoutine = null;
            _isOpening = false;
            chestUpgradeUI.OpenChestUpgradeUI(_upgradeCount);
        }

        private void HandleUpgradeOpened()
        {
            _canClose = true;
            _canClickBackButton = true;
            SetBackButtonEnabled(true);
        }
        
        private void HandleChestOn(UpgradeChestEvent evt)
        {
            PauseGameTime();
            _upgradeCount = evt.GetUpgradeCount;
            _isOpening = false;
            _canClose = false;
            _canClickBackButton = false;
            SetBackButtonEnabled(false);
            StopUpgradeOpenRoutine();
            StopChestDropRoutine();
            chestOpenCutScene.ResetChest();
            chestUpgradeUI.Close();
            gameObject.SetActive(true);
            _chestDropRoutine = StartCoroutine(ChestDropRoutine());
        }

        private void Close()
        {
            _isOpening = false;
            _canClose = false;
            _canClickBackButton = false;
            SetBackButtonEnabled(false);
            StopUpgradeOpenRoutine();
            StopChestDropRoutine();
            chestUpgradeUI.Close();
            gameObject.SetActive(false);
            ResumeGameTime();
        }

        private IEnumerator ChestDropRoutine()
        {
            yield return chestOpenCutScene.PlayDropIn();

            _chestDropRoutine = null;
            _canClickBackButton = true;
            SetBackButtonEnabled(true);
        }

        private void StopUpgradeOpenRoutine()
        {
            if (_upgradeOpenRoutine == null) return;

            StopCoroutine(_upgradeOpenRoutine);
            _upgradeOpenRoutine = null;
        }

        private void StopChestDropRoutine()
        {
            if (_chestDropRoutine == null) return;

            StopCoroutine(_chestDropRoutine);
            _chestDropRoutine = null;
            chestOpenCutScene.ResetDropInPosition();
        }

        private void SetBackButtonEnabled(bool enabled)
        {
            if (backButton != null)
                backButton.enabled = enabled;
        }

        private void PauseGameTime()
        {
            if (_isTimePaused) return;

            _previousTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            _isTimePaused = true;
        }

        private void ResumeGameTime()
        {
            if (!_isTimePaused) return;

            Time.timeScale = _previousTimeScale;
            _isTimePaused = false;
        }
    }
}
