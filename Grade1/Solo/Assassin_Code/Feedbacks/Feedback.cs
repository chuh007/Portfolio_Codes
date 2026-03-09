using UnityEngine;

namespace Code.Feedbacks
{
    public abstract class Feedback : MonoBehaviour
    {
        public abstract void CreateFeedback();

        public virtual void FinishFeedback()
        { }
    }
}