#if USING_XRHANDS
using System.Collections.Generic;
using HurricaneVR.Framework.Shared;
using UnityEngine;
using UnityEngine.XR.Hands;

#endif

namespace HurricaneVR.Framework.ControllerInput
{
    public static class HVROpenXRFingerCurls
    {
#if USING_XRHANDS
        private static float minCurl = 0.15f;
        private static float maxCurl = 0.9f;

        private static XRHandSubsystem system;
        private static readonly List<XRHandSubsystem> systems = new List<XRHandSubsystem>();

        public static void Update()
        {
            if (system == null || !system.running)
            {
                SubsystemManager.GetSubsystems(systems);
                for (var i = 0; i < systems.Count; ++i)
                {
                    var handSubsystem = systems[i];
                    if (handSubsystem.running)
                    {
                        system = handSubsystem;
                        break;
                    }
                }
            }
        }

        public static bool TryGetCurls(HVRHandSide handSide, float[] curls)
        {
            if (system == null || !system.running)
                return false;

            XRHand hand = default;

            if (handSide == HVRHandSide.Left)
            {
                hand = system.leftHand;
            }
            else
            {
                hand = system.rightHand;
            }

            return hand.isTracked && TryCalculateFingerCurvatures(hand, curls);
        }

        static bool TryCalculateFingerCurvatures(XRHand hand, float[] curls)
        {
            Handedness handedness = hand.handedness;

            if (!TryCalculateFingerCurvature(hand, handedness, XRHandJointID.ThumbMetacarpal, XRHandJointID.ThumbProximal, XRHandJointID.ThumbDistal, XRHandJointID.ThumbTip, out curls[0]))
                return false;
            if (!TryCalculateFingerCurvature(hand, handedness, XRHandJointID.IndexMetacarpal, XRHandJointID.IndexProximal, XRHandJointID.IndexIntermediate, XRHandJointID.IndexDistal, out curls[1]))
                return false;
            if (!TryCalculateFingerCurvature(hand, handedness, XRHandJointID.MiddleMetacarpal, XRHandJointID.MiddleProximal, XRHandJointID.MiddleIntermediate, XRHandJointID.MiddleDistal, out curls[2]))
                return false;
            if (!TryCalculateFingerCurvature(hand, handedness, XRHandJointID.RingMetacarpal, XRHandJointID.RingProximal, XRHandJointID.RingIntermediate, XRHandJointID.RingDistal, out curls[3]))
                return false;
            if (!TryCalculateFingerCurvature(hand, handedness, XRHandJointID.LittleMetacarpal, XRHandJointID.LittleProximal, XRHandJointID.LittleIntermediate, XRHandJointID.LittleDistal, out curls[4]))
                return false;

            return true;
        }

        static bool TryCalculateFingerCurvature(XRHand hand, Handedness handName, XRHandJointID metacarpalId, XRHandJointID proximalId, XRHandJointID intermediateId, XRHandJointID distalId, out float curl)
        {
            var metacarpalJoint = hand.GetJoint(metacarpalId);
            var proximalJoint = hand.GetJoint(proximalId);
            var intermediateJoint = hand.GetJoint(intermediateId);
            var distalJoint = hand.GetJoint(distalId);

            if (metacarpalJoint.TryGetPose(out Pose metacarpalPose) &&
                proximalJoint.TryGetPose(out Pose proximalPose) &&
                intermediateJoint.TryGetPose(out Pose intermediatePose) &&
                distalJoint.TryGetPose(out Pose distalPose))
            {
                float curvature = CalculateCurvature(metacarpalPose, proximalPose, intermediatePose, distalPose);
                float remappedCurvature = Remap(curvature, minCurl, maxCurl, 0f, 1f);
                curl = remappedCurvature;
                return true;
            }

            curl = 0f;
            return false;
        }

        static float CalculateCurvature(Pose metacarpal, Pose proximal, Pose intermediate, Pose distal)
        {
            float angle1 = Vector3.Angle(metacarpal.position - proximal.position, proximal.position - intermediate.position);
            float angle2 = Vector3.Angle(proximal.position - intermediate.position, intermediate.position - distal.position);
            float normalizedAngle = (angle1 + angle2) / 180.0f; // 180 degrees being the max angle (fully extended)
            return Mathf.Clamp01(normalizedAngle);
        }

        static float Remap(float value, float from1, float to1, float from2, float to2)
        {
            value = Mathf.Clamp(value, minCurl, maxCurl);
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

#endif
    }
}