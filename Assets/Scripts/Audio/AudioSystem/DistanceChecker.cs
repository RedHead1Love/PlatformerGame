using UnityEngine;

namespace AudioSystem
{
    public sealed class DistanceChecker : IDistanceChecker
    {
        public bool IsWithinDistance(Vector3 sourcePosition, Vector3 targetPosition, float maxDistance)
        {
            float distance = CalculateDistance(sourcePosition, targetPosition);

            return distance <= maxDistance;
        }

        public float CalculateDistance(Vector3 sourcePosition, Vector3 targetPosition)
        {
            return Vector3.Distance(sourcePosition, targetPosition);
        }
    }
}