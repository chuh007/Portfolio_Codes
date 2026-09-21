namespace _Work.CHUH.Code.Tree.MetaUpgrade
{
    /// <summary>
    /// 각각 해당하는 스텟이 있다.
    /// </summary>
    public enum MetaUpgradeType
    {
        Damage, // 기본 딜증(공식 만들어야 할듯)
        Defence, // 방어력(최소뎀 1)
        MaxHealth, // 최대체력
        Heal, // 초당 힐
        Cooldown, // 쿨감(%)
        AttackRange, // 공격의 범위(보이는거랑 콜라이더 크기 키우는 식으로 해야할 듯.)
        ProjectileSpeed, // 투사체 속도
        ProjectileRuntime, // 투사체 지속시간
        ProjectileCount, // 투사체 개수
        MoveSpeed, // 이동속도
        Magnet, // 자력(먹는 범위)
        Luck, // 행운(공통 빌드 등급과 상자 보상 개수 확률 보정)
        GrowEXP, // 경험치 얻는 양 상승
        MoreGold, // 일반 적 골드 드롭확률 상승
        RerollCount, // 레벨업 카드 시작 리롤 횟수 추가
    }
}
