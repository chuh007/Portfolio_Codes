using UnityEngine;

namespace _01Scripts.Combat.Feedback
{
    public abstract class Feedback : MonoBehaviour
    {
        public abstract void CreateFeedback();

        public virtual void FinishFeedback()
        { }
    }
}
