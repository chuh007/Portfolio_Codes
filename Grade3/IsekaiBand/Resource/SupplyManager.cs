using System;
using _Code.LCH._02.Scripts.Bus;
using _Work.CHUH.Code.Core.Persistence;
using Chuh007Lib.Bus;
using UnityEngine;

namespace _Work.CHUH.Code.Resource
{
    public class SupplyManager : MonoBehaviour
    {
        private const float AutosaveInterval = 5f;
        private const int GameClearCoinReward = 200;
        private const int GameClearNoteReward = 2;

        public static SupplyManager Instance { get; private set; }

        [SerializeField] private int coin;
        [SerializeField] private int note;
        [SerializeField] private int runCoinEarned;
        [SerializeField] private int runMiddleBossNoteEarned;

        public int Coin => coin;
        public int Note => note;
        public int RunCoinEarned => runCoinEarned;
        public int RunNoteEarned => runMiddleBossNoteEarned;
        public int RunMiddleBossNoteEarned => runMiddleBossNoteEarned;
        public event Action<int> OnCoinChanged;
        public event Action<int> OnNoteChanged;
        public event Action<int> OnRunCoinEarnedChanged;
        public event Action<int> OnRunMiddleBossNoteEarnedChanged;

        private float _nextAutosaveTime;
        private bool _isRunCoinTracking;
        private bool _gameClearRewardGranted;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadResources();
            _nextAutosaveTime = Time.unscaledTime + AutosaveInterval;
            OnCoinChanged?.Invoke(coin);
            OnNoteChanged?.Invoke(note);
            Bus<MiddleBossNoteRewardEvent>.OnEvent += HandleMiddleBossNoteReward;
        }

        private void Update()
        {
            if (Time.unscaledTime < _nextAutosaveTime) return;

            _nextAutosaveTime = Time.unscaledTime + AutosaveInterval;
            if (PersistentProgressStore.HasUnsavedChanges)
                PersistentProgressStore.Flush();
        }

        private void OnApplicationPause(bool isPaused)
        {
            if (isPaused)
                PersistentProgressStore.Flush();
        }

        private void OnApplicationQuit()
        {
            PersistentProgressStore.Flush();
        }

        private void OnDestroy()
        {
            if (Instance != this) return;

            Bus<MiddleBossNoteRewardEvent>.OnEvent -= HandleMiddleBossNoteReward;
            PersistentProgressStore.Flush();
            Instance = null;
        }

        public void AddCoin(int amount)
        {
            if (amount <= 0) return;

            int previousCoin = coin;
            int nextCoin = ClampAddition(coin, amount);
            SetCoin(nextCoin);

            if (!_isRunCoinTracking)
                return;

            int gainedCoin = Mathf.Max(0, nextCoin - previousCoin);
            if (gainedCoin > 0)
                SetRunCoinEarned(ClampAddition(runCoinEarned, gainedCoin));
        }

        public void AddNote(int amount)
        {
            if (amount <= 0) return;

            int previousNote = note;
            int nextNote = ClampAddition(note, amount);
            SetNote(nextNote);

            if (!_isRunCoinTracking)
                return;

            int gainedNote = Mathf.Max(0, nextNote - previousNote);
            if (gainedNote > 0)
                SetRunMiddleBossNoteEarned(ClampAddition(runMiddleBossNoteEarned, gainedNote));
        }

        public bool TryUseCoin(int amount)
        {
            if (amount <= 0) return false;
            if (coin < amount) return false;

            SetCoin(coin - amount);
            return true;
        }

        public void UseCoin(int amount)
        {
            if (amount <= 0) return;

            SetCoin(coin - amount);
        }

        public void UseNote(int amount)
        {
            if (amount <= 0) return;

            SetNote(note - amount);
        }

        public bool TryUseNote(int amount)
        {
            if (amount <= 0) return false;
            if (note < amount) return false;

            SetNote(note - amount);
            return true;
        }

        public void SetCoin(int amount)
        {
            int newAmount = Mathf.Max(0, amount);
            if (coin == newAmount) return;

            coin = newAmount;
            PersistentProgressStore.SetResourceAmount(PersistentResourceIds.Coin, coin);
            OnCoinChanged?.Invoke(coin);
        }

        public void SetNote(int amount)
        {
            int newAmount = Mathf.Max(0, amount);
            if (note == newAmount) return;

            note = newAmount;
            PersistentProgressStore.SetResourceAmount(PersistentResourceIds.Note, note);
            OnNoteChanged?.Invoke(note);
        }

        public void BeginRunCoinTracking()
        {
            _isRunCoinTracking = true;
            _gameClearRewardGranted = false;
            SetRunCoinEarned(0);
            SetRunMiddleBossNoteEarned(0);
        }

        public void EndRunCoinTracking()
        {
            _isRunCoinTracking = false;
        }

        public bool TryGrantGameClearReward()
        {
            if (_gameClearRewardGranted)
                return false;

            _gameClearRewardGranted = true;
            AddCoin(GameClearCoinReward);
            AddNote(GameClearNoteReward);
            PersistentProgressStore.Flush();
            return true;
        }

        private void HandleMiddleBossNoteReward(MiddleBossNoteRewardEvent evt)
        {
            if (evt.NoteAmount <= 0)
                return;

            AddNote(evt.NoteAmount);

            PersistentProgressStore.Flush();
        }

        private void SetRunCoinEarned(int amount)
        {
            int newAmount = Mathf.Max(0, amount);
            if (runCoinEarned == newAmount) return;

            runCoinEarned = newAmount;
            OnRunCoinEarnedChanged?.Invoke(runCoinEarned);
        }

        private void SetRunMiddleBossNoteEarned(int amount)
        {
            int newAmount = Mathf.Max(0, amount);
            if (runMiddleBossNoteEarned == newAmount) return;

            runMiddleBossNoteEarned = newAmount;
            OnRunMiddleBossNoteEarnedChanged?.Invoke(runMiddleBossNoteEarned);
        }

        private void LoadResources()
        {
            if (PersistentProgressStore.TryGetResourceAmount(PersistentResourceIds.Coin, out int savedCoin))
                coin = savedCoin;
            else
                PersistentProgressStore.SetResourceAmount(PersistentResourceIds.Coin, coin);

            if (PersistentProgressStore.TryGetResourceAmount(PersistentResourceIds.Note, out int savedNote))
                note = savedNote;
            else
                PersistentProgressStore.SetResourceAmount(PersistentResourceIds.Note, note);
        }

        private static int ClampAddition(int currentAmount, int addedAmount)
        {
            return (int)Math.Min(int.MaxValue, (long)currentAmount + addedAmount);
        }
    }
}
