using UnityEngine;

namespace Foxberry
{
    public interface ICoinPhysicsHandler
    {
        void Initialize(LayerMask groundLayer);

        void StopPhysics();
    }
}
