using _Work.CHUH.Code.Enemies;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class LobRockLanding
    {
        private readonly MiddleBossLobRockPatternSO _pattern;

        public LobRockLanding(MiddleBossLobRockPatternSO pattern) => _pattern = pattern;

        public Vector2 GetLandingPosition(Enemy owner)
        {
            if (owner.target != null)
                return owner.target.transform.position;

            return owner.transform.position;
        }

        public Vector2[] CreateLandingPositions(Vector2 aimedPosition)
        {
            int count = Mathf.Max(0, _pattern.RandomRockCount) + 1;
            Vector2[] positions = new Vector2[count];
            positions[0] = aimedPosition;

            for (int i = 1; i < count; i++)
                positions[i] = aimedPosition + GetRandomLandingOffset();

            Shuffle(positions);
            return positions;
        }

        public Vector2 GetRandomLandingOffset()
        {
            float radius = Mathf.Max(0f, _pattern.RandomLandingRadius);
            float minDistance = Mathf.Min(Mathf.Max(0f, _pattern.RandomLandingMinDistance), radius);
            if (radius <= 0f)
                return Vector2.zero;

            for (int i = 0; i < 8; i++)
            {
                Vector2 offset = UnityEngine.Random.insideUnitCircle * radius;
                if (offset.magnitude >= minDistance)
                    return offset;
            }

            return UnityEngine.Random.insideUnitCircle.normalized * minDistance;
        }

        public static void Shuffle(Vector2[] positions)
        {
            for (int i = positions.Length - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                (positions[i], positions[j]) = (positions[j], positions[i]);
            }
        }
    }
}
