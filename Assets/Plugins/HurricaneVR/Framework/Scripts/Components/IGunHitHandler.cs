using UnityEngine;

namespace HurricaneVR.Framework.Components
{
    public interface IGunHitHandler
    {
        void HandleHit(HVRDamageProvider damageProvider, RaycastHit hit, Vector3 direction);
    }
}