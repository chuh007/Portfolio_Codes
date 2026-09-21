using System.Collections.Generic;
using System.Linq;
using _Work.CHUH.Code.Tree.MetaUpgrade;
using _Work.CHUH.Code.Tree.UI;
using _Work.CHUH.Code.Tree.Upgrade;
using UnityEditor;
using UnityEngine;

using static _Work.CHUH.Code.Tree.Editor.PermanentUpgradeTreeIcons;
using static _Work.CHUH.Code.Tree.Editor.PermanentUpgradeTreeSpec;

namespace _Work.CHUH.Code.Tree.Editor
{
    internal static class PermanentUpgradeTreeCatalog
    {
        private const float HealRewardValue = 0.1f;

        public static readonly NodeSpec[] NodeSpecs =
        {
            N("root", "N00_FirstRehearsal", "첫 리허설", P(10), 0, 1, DamageIcon,
                C("power1", "survival1", "growth1"), U(MetaUpgradeType.Damage, 0.03f, true, "피해량 +3%")),

            N("power1", "N10_PowerChord", "정확한 템포", P(20), 1, 0, DamageIcon,
                C("power2"), U(MetaUpgradeType.Cooldown, 0.03f, true, "재사용 대기시간 효율 +3%")),
            N("survival1", "N11_StableSetup", "기초 체력", P(20), 1, 1, HealthIcon,
                C("survival2"), U(MetaUpgradeType.MaxHealth, 10f, false, "최대 체력 +10")),
            N("growth1", "N12_Fanbase", "연습 노트", P(20), 1, 2, ExpIcon,
                C("growth2"), U(MetaUpgradeType.GrowEXP, 0.05f, true, "경험치 획득량 +5%")),

            N("power2", "N20_FastPicking", "고속 피킹", P(30), 2, 0, MoveIcon,
                C("power3"), U(MetaUpgradeType.ProjectileSpeed, 0.12f, true, "투사체 속도 +12%")),
            N("survival2", "N22_SteelStamina", "초급 수집망", P(30), 2, 1, MagnetIcon,
                C("survival3"), U(MetaUpgradeType.Magnet, 0.12f, true, "경험치 획득 범위 +12%")),
            N("growth2", "N24_LuckyMerch", "첫 수익", P(30), 2, 2, ExpIcon,
                C("growth3"), U(MetaUpgradeType.MoreGold, 0.05f, true, "일반 적 골드 드롭 확률 +5%")),

            N("power3", "N21_WideStage", "더블 트랙", P(40), 3, 0, ProjectileIcon,
                C("merge1"), U(MetaUpgradeType.ProjectileCount, 1f, false, "투사체 수 +1")),
            N("survival3", "N23_RecoveryRoutine", "호흡 조절", P(40), 3, 1, HealthIcon,
                C("merge1"), U(MetaUpgradeType.Heal, HealRewardValue, false, "초당 회복 +0.1")),
            N("growth3", "N25_RapidGrowth", "경험치 수집망", P(40), 3, 2, ExpIcon,
                C("merge1"), U(MetaUpgradeType.Magnet, 0.12f, true, "경험치 획득 범위 +12%")),

            N("merge1", "N30_EncoreFirepower", "합주 시작", P(60), 4, 1, DamageIcon,
                C("merge2"), U(MetaUpgradeType.Damage, 0.08f, true, "피해량 +8%")),
            N("merge2", "N31_UnbreakableStage", "무대 적응", P(70), 5, 1, HealthIcon,
                C("merge3"), U(MetaUpgradeType.MaxHealth, 25f, false, "최대 체력 +25")),
            N("merge3", "N32_StarRoad", "운명의 조명", P(80), 6, 1, ExpIcon,
                C("power4", "survival4", "growth4"), U(MetaUpgradeType.Luck, 2f, false, "행운 +2")),

            N("power4", "N70_LongResonance", "긴 여운", P(90), 7, 0, ProjectileIcon,
                C("power5"), U(MetaUpgradeType.ProjectileRuntime, 0.10f, true, "투사체 지속시간 +10%")),
            N("survival4", "N71_ReinforcedHealth", "강화 체력", P(90), 7, 1, HealthIcon,
                C("survival5"), U(MetaUpgradeType.MaxHealth, 15f, false, "최대 체력 +15")),
            N("growth4", "N72_FastGrowth", "빠른 성장", P(90), 7, 2, ExpIcon,
                C("growth5"), U(MetaUpgradeType.GrowEXP, 0.10f, true, "경험치 획득량 +10%")),

            N("power5", "N80_WideStage", "넓은 무대", P(105), 8, 0, ProjectileIcon,
                C("power6"), U(MetaUpgradeType.AttackRange, 0.12f, true, "공격 범위 +12%")),
            N("survival5", "N81_ThickGuard", "확장 수집망", P(105), 8, 1, MagnetIcon,
                C("survival6"), U(MetaUpgradeType.Magnet, 0.12f, true, "경험치 획득 범위 +12%")),
            N("growth5", "N82_StrongMagnet", "숙련된 수집", P(105), 8, 2, ExpIcon,
                C("growth6"), U(MetaUpgradeType.Magnet, 0.12f, true, "경험치 획득 범위 +12%")),

            N("power6", "N90_DoubleTrack", "더블 트랙 II", P(120), 9, 0, ProjectileIcon,
                C("power7"), U(MetaUpgradeType.ProjectileCount, 1f, false, "투사체 수 +1")),
            N("survival6", "N91_FirstEncore", "첫 앙코르", P(120), 9, 1, RerollIcon,
                C("survival7"), U(MetaUpgradeType.RerollCount, 1f, false, "시작 리롤 횟수 +1")),
            N("growth6", "N92_TourProfit", "흥행 수익", P(120), 9, 2, ExpIcon,
                C("growth7"), U(MetaUpgradeType.MoreGold, 0.08f, true, "일반 적 골드 드롭 확률 +8%")),

            N("power7", "N100_PowerChord", "파워 코드", P(140), 10, 0, DamageIcon,
                C("headliner"), U(MetaUpgradeType.Damage, 0.12f, true, "피해량 +12%")),
            N("survival7", "N101_StageBody", "무대 체질", P(140), 10, 1, MoveIcon,
                C("headliner"), U(MetaUpgradeType.MoveSpeed, 0.08f, true, "이동속도 +8%")),
            N("growth7", "N102_StarLuck", "스타의 운", P(140), 10, 2, ExpIcon,
                C("headliner"), U(MetaUpgradeType.Luck, 2f, false, "행운 +2")),

            N("headliner", "N40_Headliner", "헤드라이너", 1, 11, 1, DamageIcon,
                C("power8", "survival8", "growth8"), U(MetaUpgradeType.Cooldown, 0.10f, true, "재사용 대기시간 효율 +10%"), TreeNodeCostCurrency.Note),

            N("power8", "N120_RapidTempo", "폭발적인 템포", P(180), 12, 0, DamageIcon,
                C("power9"), U(MetaUpgradeType.Cooldown, 0.05f, true, "재사용 대기시간 효율 +5%")),
            N("survival8", "N121_RoadTraining", "투어 체력", P(180), 12, 1, HealthIcon,
                C("survival9"), U(MetaUpgradeType.MaxHealth, 25f, false, "최대 체력 +25")),
            N("growth8", "N122_ExpCollector", "경험치 수집 장비", P(180), 12, 2, ExpIcon,
                C("growth9"), U(MetaUpgradeType.Magnet, 0.15f, true, "경험치 획득 범위 +15%")),

            N("power9", "N130_AmplifiedStrike", "증폭된 연주", P(220), 13, 0, ProjectileIcon,
                C("power10"), U(MetaUpgradeType.ProjectileSpeed, 0.15f, true, "투사체 속도 +15%")),
            N("survival9", "N131_SteelGuard", "경험치 유도 장치", P(220), 13, 1, MagnetIcon,
                C("survival10"), U(MetaUpgradeType.Magnet, 0.15f, true, "경험치 획득 범위 +15%")),
            N("growth9", "N132_SecondChance", "두 번째 선택", P(220), 13, 2, ExpIcon,
                C("growth10"), U(MetaUpgradeType.RerollCount, 1f, false, "시작 리롤 횟수 +1")),

            N("power10", "N140_HeavySound", "헤비 사운드", P(260), 14, 0, DamageIcon,
                C("milestone1"), U(MetaUpgradeType.Damage, 0.15f, true, "피해량 +15%")),
            N("survival10", "N141_RecoveryBreath", "회복의 호흡", P(260), 14, 1, HealthIcon,
                C("milestone1"), U(MetaUpgradeType.Heal, HealRewardValue, false, "초당 회복 +0.1")),
            N("growth10", "N142_FastLearning", "빠른 학습", P(260), 14, 2, ExpIcon,
                C("milestone1"), U(MetaUpgradeType.GrowEXP, 0.15f, true, "경험치 획득량 +15%")),

            N("milestone1", "N150_BreakthroughConcert", "돌파 콘서트", 1, 15, 1, DamageIcon,
                C("power11", "survival11", "growth11"), U(MetaUpgradeType.Damage, 0.20f, true, "피해량 +20%"), TreeNodeCostCurrency.Note),

            N("power11", "N160_GrandStage", "대형 무대", P(320), 16, 0, ProjectileIcon,
                C("power12"), U(MetaUpgradeType.AttackRange, 0.15f, true, "공격 범위 +15%")),
            N("survival11", "N161_TourBody", "단련된 체력", P(320), 16, 1, HealthIcon,
                C("survival12"), U(MetaUpgradeType.MaxHealth, 35f, false, "최대 체력 +35")),
            N("growth11", "N162_MerchExpansion", "굿즈 확장", P(320), 16, 2, ExpIcon,
                C("growth12"), U(MetaUpgradeType.MoreGold, 0.10f, true, "일반 적 골드 드롭 확률 +10%")),

            N("power12", "N170_EndlessResonance", "끝없는 공명", P(380), 17, 0, ProjectileIcon,
                C("power13"), U(MetaUpgradeType.ProjectileRuntime, 0.15f, true, "투사체 지속시간 +15%")),
            N("survival12", "N171_HeavyArmor", "광역 경험치장", P(380), 17, 1, MagnetIcon,
                C("survival13"), U(MetaUpgradeType.Magnet, 0.20f, true, "경험치 획득 범위 +20%")),
            N("growth12", "N172_WideExpField", "넓은 경험치 영역", P(380), 17, 2, ExpIcon,
                C("growth13"), U(MetaUpgradeType.Magnet, 0.20f, true, "경험치 획득 범위 +20%")),

            N("power13", "N180_TripleTrack", "트리플 트랙", P(440), 18, 0, ProjectileIcon,
                C("milestone2"), U(MetaUpgradeType.ProjectileCount, 1f, false, "투사체 수 +1")),
            N("survival13", "N181_HealingRhythm", "치유의 리듬", P(440), 18, 1, HealthIcon,
                C("milestone2"), U(MetaUpgradeType.Heal, HealRewardValue, false, "초당 회복 +0.1")),
            N("growth13", "N182_EncoreChoice", "앙코르 선택", P(440), 18, 2, ExpIcon,
                C("milestone2"), U(MetaUpgradeType.RerollCount, 1f, false, "시작 리롤 횟수 +1")),

            N("milestone2", "N190_DomeTour", "돔 투어", 1, 19, 1, HealthIcon,
                C("power14", "survival14", "growth14"), U(MetaUpgradeType.MaxHealth, 100f, false, "최대 체력 +100"), TreeNodeCostCurrency.Note),

            N("power14", "N200_SonicBoom", "소닉 붐", P(520), 20, 0, DamageIcon,
                C("power15"), U(MetaUpgradeType.Damage, 0.18f, true, "피해량 +18%")),
            N("survival14", "N201_LightFootwork", "가벼운 발놀림", P(520), 20, 1, MoveIcon,
                C("survival15"), U(MetaUpgradeType.MoveSpeed, 0.10f, true, "이동속도 +10%")),
            N("growth14", "N202_AcceleratedGrowth", "가속 성장", P(520), 20, 2, ExpIcon,
                C("growth15"), U(MetaUpgradeType.GrowEXP, 0.18f, true, "경험치 획득량 +18%")),

            N("power15", "N210_PerfectTempo", "완벽한 템포", P(600), 21, 0, DamageIcon,
                C("power16"), U(MetaUpgradeType.Cooldown, 0.08f, true, "재사용 대기시간 효율 +8%")),
            N("survival15", "N211_FortressStage", "경험치 흡입 장치", P(600), 21, 1, MagnetIcon,
                C("survival16"), U(MetaUpgradeType.Magnet, 0.25f, true, "경험치 획득 범위 +25%")),
            N("growth15", "N212_StarIntuition", "스타의 직감", P(600), 21, 2, ExpIcon,
                C("growth16"), U(MetaUpgradeType.Luck, 3f, false, "행운 +3")),

            N("power16", "N220_LightningPerformance", "번개 같은 연주", P(700), 22, 0, ProjectileIcon,
                C("power17"), U(MetaUpgradeType.ProjectileSpeed, 0.20f, true, "투사체 속도 +20%")),
            N("survival16", "N221_VitalPulse", "생명의 박동", P(700), 22, 1, HealthIcon,
                C("survival17"), U(MetaUpgradeType.Heal, HealRewardValue, false, "초당 회복 +0.1")),
            N("growth16", "N222_ExpVacuum", "경험치 진공", P(700), 22, 2, ExpIcon,
                C("growth17"), U(MetaUpgradeType.Magnet, 0.25f, true, "경험치 획득 범위 +25%")),

            N("power17", "N230_WallOfSound", "사운드의 벽", P(800), 23, 0, ProjectileIcon,
                C("milestone3"), U(MetaUpgradeType.ProjectileCount, 2f, false, "투사체 수 +2")),
            N("survival17", "N231_GiantStamina", "거인의 체력", P(800), 23, 1, HealthIcon,
                C("milestone3"), U(MetaUpgradeType.MaxHealth, 50f, false, "최대 체력 +50")),
            N("growth17", "N232_MultipleChances", "다중 선택", P(800), 23, 2, ExpIcon,
                C("milestone3"), U(MetaUpgradeType.RerollCount, 2f, false, "시작 리롤 횟수 +2")),

            N("milestone3", "N240_WorldTour", "월드 투어", 1, 24, 1, ExpIcon,
                C("power18", "survival18", "growth18"), U(MetaUpgradeType.GrowEXP, 0.35f, true, "경험치 획득량 +35%"), TreeNodeCostCurrency.Note),

            N("power18", "N250_LegendarySound", "전설의 사운드", P(950), 25, 0, DamageIcon,
                C("power19"), U(MetaUpgradeType.Damage, 0.22f, true, "피해량 +22%")),
            N("survival18", "N251_DiamondGuard", "궁극의 수집망", P(950), 25, 1, MagnetIcon,
                C("survival19"), U(MetaUpgradeType.Magnet, 0.30f, true, "경험치 획득 범위 +30%")),
            N("growth18", "N252_GlobalMerch", "글로벌 굿즈", P(950), 25, 2, ExpIcon,
                C("growth19"), U(MetaUpgradeType.MoreGold, 0.15f, true, "일반 적 골드 드롭 확률 +15%")),

            N("power19", "N260_StadiumStage", "스타디움 무대", P(1100), 26, 0, ProjectileIcon,
                C("power20"), U(MetaUpgradeType.AttackRange, 0.20f, true, "공격 범위 +20%")),
            N("survival19", "N261_ImmortalBody", "불멸의 체력", P(1100), 26, 1, HealthIcon,
                C("survival20"), U(MetaUpgradeType.MaxHealth, 65f, false, "최대 체력 +65")),
            N("growth19", "N262_FortunesFavor", "행운의 총애", P(1100), 26, 2, ExpIcon,
                C("growth20"), U(MetaUpgradeType.Luck, 4f, false, "행운 +4")),

            N("power20", "N270_MasterTempo", "마스터 템포", P(1250), 27, 0, DamageIcon,
                C("power21"), U(MetaUpgradeType.Cooldown, 0.10f, true, "재사용 대기시간 효율 +10%")),
            N("survival20", "N271_EndlessRecovery", "끝없는 회복", P(1250), 27, 1, HealthIcon,
                C("survival21"), U(MetaUpgradeType.Heal, HealRewardValue, false, "초당 회복 +0.1")),
            N("growth20", "N272_ExpHorizon", "경험치의 지평선", P(1250), 27, 2, ExpIcon,
                C("growth21"), U(MetaUpgradeType.Magnet, 0.30f, true, "경험치 획득 범위 +30%")),

            N("power21", "N280_OrchestraBarrage", "오케스트라 포화", P(1450), 28, 0, ProjectileIcon,
                C("legend"), U(MetaUpgradeType.ProjectileCount, 2f, false, "투사체 수 +2")),
            N("survival21", "N281_StageAscension", "무대 초월", P(1450), 28, 1, MoveIcon,
                C("legend"), U(MetaUpgradeType.MoveSpeed, 0.15f, true, "이동속도 +15%")),
            N("growth21", "N282_UnlimitedChoices", "무한한 선택지", P(1450), 28, 2, ExpIcon,
                C("legend"), U(MetaUpgradeType.RerollCount, 2f, false, "시작 리롤 횟수 +2")),

            N("legend", "N290_LegendaryStage", "전설의 무대", P(2500), 29, 1, DamageIcon,
                C(), U(MetaUpgradeType.Cooldown, 0.25f, true, "재사용 대기시간 효율 +25%"))
        };
    }
}
