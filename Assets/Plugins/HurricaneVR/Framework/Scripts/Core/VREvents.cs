using System;
using HurricaneVR.Framework.Core.Grabbers;
using HurricaneVR.Framework.Core.HandPoser;
using UnityEngine.Events;

namespace HurricaneVR.Framework.Core
{
    [Serializable]
    public class VRHandGrabberEvent : UnityEvent<HVRHandGrabber, HVRGrabbable>
    {
    }

    [Serializable]
    public class VRSocketEvent : UnityEvent<HVRSocket, HVRGrabbable>
    {
    }

    [Serializable]
    public class VRGrabberEvent : UnityEvent<HVRGrabberBase, HVRGrabbable>
    {
    }

    [Serializable]
    public class VRGrabbableEvent : UnityEvent<HVRGrabbable>
    {
    }

    [Serializable]
    public class VRHandPoseEvent : UnityEvent<HVRHandPoser>
    {
    }

    [Serializable]
    public class DialSteppedEvent : UnityEvent<int>
    {
    }

    [Serializable]
    public class DialTurnedEvent : UnityEvent<float, float, float>
    {
    }

    [Serializable]
    public class LeverMovedEvent : UnityEvent<float, float, float>
    {
    }

    [Serializable]
    public class LeverSteppedEvent : UnityEvent<int>
    {
    }

    [Serializable]
    public class HVRDistanceReleaseEvent : UnityEvent<HVRDistanceReleaseArgs>
    {
    }

    [Serializable]
    public class HVRDistanceGrabEvent : UnityEvent<HVRDistanceGrabArgs>
    {
    }

    public struct HVRDistanceGrabArgs
    {
        public HVRForceGrabber ForceGrabber;
        public HVRGrabbable Grabbable;

        public HVRDistanceGrabArgs(HVRForceGrabber forceGrabber, HVRGrabbable grabbable)
        {
            ForceGrabber = forceGrabber;
            Grabbable = grabbable;
        }
    }

    public struct HVRDistanceReleaseArgs
    {
        public HVRForceGrabber ForceGrabber;
        public HVRGrabbable Grabbable;
        public bool WasHandGrabbed;

        public HVRDistanceReleaseArgs(HVRForceGrabber forceGrabber, HVRGrabbable grabbable, bool wasHandGrabbed)
        {
            ForceGrabber = forceGrabber;
            Grabbable = grabbable;
            WasHandGrabbed = wasHandGrabbed;
        }
    }
}