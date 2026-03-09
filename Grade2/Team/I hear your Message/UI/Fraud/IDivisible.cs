using System;

namespace A.Work.CHUH._01.Script.UI.Fraud
{
    public interface IDivisible
    {
        public event Action<int, int> OnSuccess;
        public event Action<int, int> OnFail;
    }
}