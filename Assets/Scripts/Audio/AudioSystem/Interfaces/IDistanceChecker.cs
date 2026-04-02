using UnityEngine;

namespace AudioSystem
{
    public interface IDistanceChecker
    {
        bool IsWithinDistance(Vector3 sourcePosition, Vector3 targetPosition, float maxDistance);
        float CalculateDistance(Vector3 sourcePosition, Vector3 targetPosition);
    }
}