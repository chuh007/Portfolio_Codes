using Code.CoreSystem;

namespace Work.CHUH._01Scripts.Enemies.Wave.GameEvents
{
    public struct WaveEndEvent : IEvent
    {
        public enum Reason
        {
            None,

            // 모든 적 처치 (클리어)
            AllEnemiesDefeated,

            // 패배
            PlayerDefeated,

            // 제한시간 초과 (필요할지 모르곘음)
            TimeLimitReached,

            /// 플레이어가 종료 (포기)
            PlayerRetreated,
        }

        public Reason EndReason;

        public WaveEndEvent(Reason reason)
        {
            EndReason = reason;
        }
    }
}