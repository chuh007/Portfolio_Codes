using UnityEngine;

namespace _Work.CHUH.Code.Combat.Feedbacks
{
    public abstract class Feedback : MonoBehaviour
    {
        public abstract void CreateFeedback();

        public virtual void FinishFeedback()
        { }
    }
}