namespace Work.CUH.Code.Command
{
    /// <summary>
    /// 커맨드는 상세 동작은 모르나, 어떤 대상이 이러한 메서드를 갖고 있다 전도는 앎(추상화해서).
    /// 그걸 실행시키는 역할이 커맨드의 역할
    /// </summary>
    public interface ICommand
    {
        public bool CanExecute(); // 수행 가능한가?
        public void Execute(); // 실제 수행
        public void Undo();
    }
}