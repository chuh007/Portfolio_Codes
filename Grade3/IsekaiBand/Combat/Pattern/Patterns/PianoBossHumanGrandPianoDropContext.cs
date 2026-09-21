using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal sealed class PianoBossHumanGrandPianoDropContext : MonoBehaviour
    {
        private Vector2 _landingPosition;
        private bool _hasLandingPosition;

        public void Store(Vector2 position)
        {
            _landingPosition = position;
            _hasLandingPosition = true;
        }

        public bool TryConsume(out Vector2 position)
        {
            position = _landingPosition;
            bool hasPosition = _hasLandingPosition;
            _hasLandingPosition = false;
            return hasPosition;
        }

        public void Clear()
        {
            _hasLandingPosition = false;
        }

    }
}
