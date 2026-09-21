using _Code.LCH._02.Scripts.Player;
using _Work.CHUH.Code.Core;
using _Work.CHUH.Code.Core.Events;
using _Work.CHUH.Code.Test;
using _Work.CHUH.Code.Enemies;
using Chuh007Lib.Bus;
using UnityEngine;

namespace _Work.CHUH.Code.StageSystem
{
    public class StageController : MonoBehaviour
    {
        private const int MaxBossBaseRewardCount = 4;
        private const int MaxBossBonusRewardCount = 1;
        private const float BossApproachingLeadTime = 5f;

        [SerializeField] private StageDataSenderSO senderSO;
        private StageDataSO _currentStageData;
        
        [Header("Stage Setup")]
        [SerializeField] private SpriteRenderer background;
        [SerializeField] private Transform player;
        [SerializeField, Min(0)] private int visibleChunkRadius = 1;

        [Header("Boss Arena")]
        [SerializeField] private Vector2 bossArenaSize = new Vector2(20f, 10f);
        [SerializeField, Min(0.1f)] private float bossArenaWallThickness = 1f;
        [SerializeField] private Sprite bossArenaWallSprite;
        [SerializeField] private Material bossArenaChainMaterial;
        [SerializeField] private Color bossArenaWallColor = Color.white;
        [SerializeField, Min(1f)] private float bossCameraOrthographicSize = 6f;
        [SerializeField, Min(0f)] private float bossCameraSizeTransitionDuration = 1f;

        [Header("Boss Music")]
        [SerializeField, Min(0f)] private float boss1BgmFadeOutDuration = 1.15f;
        
        private float _currentStageTime;
        
        private float _stageTime;
        private int _currentStageIndex = 0;
        private int _currentBossIndex = 0;
        private bool _bossApproachingWarningShown;

        private InfiniteMapChunkRenderer _mapRenderer;
        private InfiniteMapDecorationRenderer _decorationRenderer;
        private BossArenaController _bossArena;
        private StageBossEncounter _bossEncounter;

        private void Awake()
        {
            int wallLayer = LayerMask.NameToLayer("Wall");
            if (wallLayer < 0)
                wallLayer = 0;

            _mapRenderer = new InfiniteMapChunkRenderer(transform, background, ResolvePlayer, visibleChunkRadius);
            _decorationRenderer = new InfiniteMapDecorationRenderer(
                transform,
                background,
                ResolvePlayer,
                _mapRenderer.CanPlaceGroundDecoration,
                visibleChunkRadius);
            _bossArena = new BossArenaController(
                transform,
                background,
                wallLayer,
                bossArenaSize,
                bossArenaWallSprite,
                bossArenaChainMaterial,
                bossArenaWallThickness,
                bossArenaWallColor,
                bossCameraOrthographicSize,
                bossCameraSizeTransitionDuration);

            var bossMusic = new StageBossMusic(this, ResolvePlayer, () => boss1BgmFadeOutDuration);
            _bossEncounter = new StageBossEncounter(
                _bossArena, _decorationRenderer, ResolvePlayer, () => bossArenaSize, bossMusic);
        }

        private void OnDestroy()
        {
            _bossEncounter?.Dispose();
            _bossArena?.Dispose();
        }

        private void Start()
        {
            _currentStageData = senderSO.Data;
            SetupMap();
            _bossArena?.Prepare();
            
            Bus<MapSizeSetEvent>.Raise(new MapSizeSetEvent(_currentStageData.StageSize.x, _currentStageData.StageSize.y, true));
    
            if (_currentStageData.WaveList != null && _currentStageData.WaveList.Count > 0)
            {
                _currentStageIndex = 0;
                RaiseWaveChange();
            }
        }

        private void Update()
        {
            _mapRenderer?.Refresh();
            _decorationRenderer?.Refresh();

            if (_bossEncounter?.IsActive == true)
                return;

            if (_currentStageData == null || _currentStageIndex >= _currentStageData.WaveList.Count) return;

            _currentStageTime += Time.deltaTime * (TestDoubleMode.Instance.isOnDoubleMode ? 2f : 1f);
    
            if (_currentStageTime >= _currentStageData.WaveList[_currentStageIndex].ToNextWave + _stageTime)
            {
                _stageTime += _currentStageData.WaveList[_currentStageIndex].ToNextWave;
                NextWave();
            }
            
            if(_currentStageData.BossSpawnList.Count <= _currentBossIndex) return;
            BossSpawnData nextBoss = _currentStageData.BossSpawnList[_currentBossIndex];
            float warningTime = Mathf.Max(0f, nextBoss.spawnTime - BossApproachingLeadTime);
            if (nextBoss.showApproachingWarning
                && !_bossApproachingWarningShown
                && _currentStageTime >= warningTime)
            {
                _bossApproachingWarningShown = true;
                Bus<BossApproachingEvent>.Raise(new BossApproachingEvent());
            }

            if (_currentStageTime >= nextBoss.spawnTime)
            {
                int bossSpawnOrder = _currentBossIndex + 1;
                int rewardCount = GetBossRewardCount(bossSpawnOrder);
                Bus<EnemySpawnEvent>.Raise(new EnemySpawnEvent(
                    nextBoss.bossPoolItem,
                    rewardCount,
                    nextBoss.useDamageAttenuation));
                _currentBossIndex++;
                _bossApproachingWarningShown = false;
            }
        }

        private static int GetBossRewardCount(int bossSpawnOrder)
        {
            int baseRewardCount = Mathf.Clamp(((bossSpawnOrder - 1) / 2) + 1, 1, MaxBossBaseRewardCount);
            return Random.Range(baseRewardCount, baseRewardCount + MaxBossBonusRewardCount + 1);
        }

        private void NextWave()
        {
            _currentStageIndex++;
            if (_currentStageIndex < _currentStageData.WaveList.Count)
                RaiseWaveChange();
        }

        private void RaiseWaveChange()
        {
            Bus<WaveChangeEvent>.Raise(new WaveChangeEvent(
                _currentStageData,
                _currentStageData.WaveList[_currentStageIndex],
                _currentStageIndex));
        }

        private void SetupMap()
        {
            _mapRenderer?.Setup(_currentStageData);
            _decorationRenderer?.Setup(_currentStageData);
        }

        public void SetBossCameraOrthographicSize(float orthographicSize)
        {
            if (_bossArena == null || !_bossArena.IsActive)
                return;

            _bossArena.SetCameraOrthographicSize(orthographicSize);
        }

        private Transform ResolvePlayer()
        {
            if (player != null) return player;

            Player foundPlayer = FindAnyObjectByType<Player>();
            if (foundPlayer != null)
                player = foundPlayer.transform;

            return player;
        }
    }
}
