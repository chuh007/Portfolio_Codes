using System.Collections;
using _Code.LCH._02.Scripts.Bus;
using _Code.LCH._02.Scripts.Player.Data;
using _Work.CHUH.Code.Core.Events;
using _Work.CHUH.Code.Enemies;
using _Work.CHUH.Code.UI;
using Chuh007Lib.Bus;
using Chuh007Lib.ObjectPool.RunTime;
using UnityEngine;

namespace _Work.CHUH.Code.Tutorial
{
    [DefaultExecutionOrder(-10000)]
    [DisallowMultipleComponent]
    public sealed class TutorialSequenceController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TutorialDialogueView dialogueView;
        [SerializeField] private PlayerCharacterDataSO pianoCharacter;
        [SerializeField] private PoolItemSO trainingEnemyPoolItem;
        [SerializeField] private CraftingBenchToolkitController craftingBench;

        [Header("Progress")]
        [SerializeField, Min(0.5f)] private float requiredMovementDistance = 3f;
        [SerializeField, Min(16f)] private float tutorialArenaSize = 40f;
        [SerializeField, Min(2.5f)] private float enemySpawnDistance = 4.5f;
        [SerializeField, Min(1)] private int maximumActiveEnemies = 2;
        [SerializeField, Min(0.1f)] private float enemyRespawnDelay = 0.75f;
        [SerializeField, Min(1)] private int combinedCombatKillTarget = 20;
        [SerializeField, Min(1)] private int combinedCombatMaxActiveEnemies = 6;
        [SerializeField, Min(2.5f)] private float combinedCombatSpawnDistance = 7.2f;
        [SerializeField] private string completionSceneName = "Main";

        private readonly TutorialProgress _progress = new();
        private TutorialTrainingEnemies _enemies;
        private TutorialContext _context;
        private Spawner _spawner;
        private TutorialRecoveryEvent _recovery;
        private Coroutine _tutorialRoutine;

        internal TutorialDialogueView Dialogue => dialogueView;
        internal CraftingBenchToolkitController CraftingBench => craftingBench;
        internal PoolItemSO TrainingEnemyPoolItem => trainingEnemyPoolItem;
        internal float RequiredMovementDistance => requiredMovementDistance;
        internal float EnemySpawnDistance => enemySpawnDistance;
        internal int MaximumActiveEnemies => maximumActiveEnemies;
        internal float EnemyRespawnDelay => enemyRespawnDelay;
        internal int CombinedCombatKillTarget => combinedCombatKillTarget;
        internal int CombinedCombatMaxActiveEnemies => combinedCombatMaxActiveEnemies;
        internal float CombinedCombatSpawnDistance => combinedCombatSpawnDistance;
        internal string CompletionSceneName => completionSceneName;

        private void Awake()
        {
            Time.timeScale = 1f;
            TutorialSceneSetup.DisableNormalStageFlow();
        }

        private void OnEnable()
        {
            Bus<DashEvent>.OnEvent += _progress.HandleDash;
            Bus<EnemyDeadEvent>.OnEvent += HandleEnemyDead;
            Bus<LevelUpEvent>.OnEvent += _progress.HandleLevelUp;
            Bus<CardSelectEvent>.OnEvent += _progress.HandleCardSelect;
        }

        private void Start()
        {
            if (dialogueView == null) dialogueView = GetComponent<TutorialDialogueView>();
            craftingBench = TutorialSceneSetup.FindCraftingBench(craftingBench);
            var player = new TutorialPlayerContext(this);
            _spawner = FindAnyObjectByType<Spawner>();
            _enemies = new TutorialTrainingEnemies(_spawner, this, player.Player);
            var combat = new TutorialCombatPractice(this, _enemies, _progress);
            _context = new TutorialContext(this, player, _enemies, combat, _progress);
            TutorialSceneSetup.PrepareArena(transform, player.Player, tutorialArenaSize);
            if (player.Attack != null && pianoCharacter != null) player.Attack.TryOverrideStartingWeapon(pianoCharacter);
            player.Movement?.LockMovement();
            _tutorialRoutine = StartCoroutine(RunTutorial());
        }

        private IEnumerator RunTutorial()
        {
            // 다른 컴포넌트의 Start 이후 일반 스폰을 중지하고 연습 전투를 시작한다.
            yield return null;
            if (_spawner != null) _spawner.enabled = false;
            if (!_context.Validate(pianoCharacter != null)) yield break;
            _recovery = new TutorialRecoveryEvent(this, dialogueView, _context.Player.Health);
            yield return TutorialRoutine.Run(TutorialFlow.Run(_context), () => _recovery.IsRecovering);
        }

        private void HandleEnemyDead(EnemyDeadEvent evt)
        {
            if (_enemies != null && _enemies.Remove(evt.Enemy)) _progress.RecordKill();
        }

        private void OnDisable()
        {
            if (_tutorialRoutine != null) StopCoroutine(_tutorialRoutine);
            _tutorialRoutine = null;
            _recovery?.Dispose();
            _recovery = null;
            dialogueView?.Hide();
            Bus<DashEvent>.OnEvent -= _progress.HandleDash;
            Bus<EnemyDeadEvent>.OnEvent -= HandleEnemyDead;
            Bus<LevelUpEvent>.OnEvent -= _progress.HandleLevelUp;
            Bus<CardSelectEvent>.OnEvent -= _progress.HandleCardSelect;
            if (craftingBench != null && craftingBench.IsVisible) craftingBench.Close();
        }

        private void OnDestroy() => Time.timeScale = 1f;

    }
}
