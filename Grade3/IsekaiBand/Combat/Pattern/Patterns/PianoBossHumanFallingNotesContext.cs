using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal sealed class PianoBossHumanFallingNotesContext : MonoBehaviour
    {
        private Vector2[] _landingPositions;

        public void Store(Vector2[] positions)
        {
            _landingPositions = positions;
        }

        public bool TryConsume(out Vector2[] positions)
        {
            positions = _landingPositions;
            _landingPositions = null;
            return positions != null && positions.Length > 0;
        }

        public void Clear()
        {
            _landingPositions = null;
        }

    }
}
