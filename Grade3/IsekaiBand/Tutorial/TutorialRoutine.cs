using System;
using System.Collections;
using System.Collections.Generic;

namespace _Work.CHUH.Code.Tutorial
{
    internal static class TutorialRoutine
    {
        public static IEnumerator Run(IEnumerator lesson, Func<bool> isPaused)
        {
            var routines = new Stack<IEnumerator>();
            routines.Push(lesson);
            try
            {
                while (routines.Count > 0)
                {
                    if (isPaused())
                    {
                        yield return null;
                        continue;
                    }

                    IEnumerator current = routines.Peek();
                    if (!current.MoveNext())
                    {
                        (routines.Pop() as IDisposable)?.Dispose();
                        continue;
                    }

                    // 대기 조건을 포함한 중첩 단계도 회복 대사가 끝나기 전에는 진행하지 않는다.
                    if (current.Current is IEnumerator nested)
                        routines.Push(nested);
                    else
                        yield return current.Current;
                }
            }
            finally
            {
                while (routines.Count > 0)
                    (routines.Pop() as IDisposable)?.Dispose();
            }
        }
    }
}
