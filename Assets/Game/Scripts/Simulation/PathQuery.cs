using UnityEngine;

namespace Game.Simulation
{
    public static class PathQuery
    {
        public static bool TryFindBlocker(ObstacleSet obstacles, PathAxis axis, float fromDistance, float halfWidth,
            out int index, out float contactDistance)
        {
            index = -1;
            contactDistance = float.MaxValue;
            float reach = halfWidth + obstacles.Radius;
            for (int i = 0; i < obstacles.Count; i++)
            {
                if (!TryGetCorridorOffset(obstacles, axis, i, fromDistance, reach, out float along, out float across))
                    continue;

                float contact = along - Mathf.Sqrt(reach * reach - across * across);
                if (contact < axis.Length && contact < contactDistance)
                {
                    contactDistance = contact;
                    index = i;
                }
            }

            return index >= 0;
        }

        public static bool TryFindImpact(ObstacleSet obstacles, PathAxis axis, float fromDistance, float halfWidth,
            float shotRadius, float launchDistance, out int index, out float impactDistance)
        {
            index = -1;
            impactDistance = float.MaxValue;
            float reach = halfWidth + obstacles.Radius;
            float shotReach = shotRadius + obstacles.Radius;
            for (int i = 0; i < obstacles.Count; i++)
            {
                if (!TryGetCorridorOffset(obstacles, axis, i, fromDistance, reach, out float along, out float across))
                    continue;

                if (along - Mathf.Sqrt(reach * reach - across * across) >= axis.Length)
                    continue;

                float impact = Mathf.Abs(across) < shotReach
                    ? along - Mathf.Sqrt(shotReach * shotReach - across * across)
                    : along;
                impact = Mathf.Max(impact, launchDistance);
                if (impact < impactDistance)
                {
                    impactDistance = impact;
                    index = i;
                }
            }

            return index >= 0;
        }

        private static bool TryGetCorridorOffset(ObstacleSet obstacles, PathAxis axis, int index, float fromDistance,
            float reach, out float along, out float across)
        {
            along = 0f;
            across = 0f;
            if (!obstacles.IsAlive(index))
                return false;

            Vector2 position = obstacles.PositionOf(index);
            along = axis.Along(position);
            if (along <= fromDistance)
                return false;

            across = axis.Across(position);
            return Mathf.Abs(across) < reach;
        }
    }
}
