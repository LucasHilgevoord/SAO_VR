using PlayerInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class InterfaceGesture : MonoBehaviour
{
	public InterfaceManager interfaceManager;
    public SteamVR_Action_Skeleton skeleton;
	public Hand leftHand;
	private float startFingerPosY;
	private float startFingerPosX;

	private float curTimeRemaining;
	private float maxTime = 3;
	private float minTriggerDistance = 0.2f;

	private bool appearGestureStarted;
	private bool dismissGestureStarted;
	private bool openedInterface;

	private void Start()
	{
		curTimeRemaining = maxTime;
	}

	private void Update()
	{
		CheckAppearGesture();
		CheckDismissGesture();
	}

	private bool CheckCurlMin(float curl, float minCurl) { return curl <= minCurl; }
	private bool CheckCurlMax(float curl, float maxCurl) { return curl >= maxCurl; }

	private void CheckAppearGesture()
	{
		if (leftHand.mainRenderModel == null)
			return;

		//Debug.Log(skeleton.indexCurl + " | " + skeleton.middleCurl + " | " + skeleton.ringCurl + " | " + skeleton.pinkyCurl + " | " + skeleton.thumbCurl);

		if (!openedInterface &&
			CheckCurlMin(skeleton.indexCurl, 0.05f) &&
			CheckCurlMin(skeleton.middleCurl, 0.15f) &&
			CheckCurlMax(skeleton.ringCurl, 0.5f) &&
			CheckCurlMax(skeleton.pinkyCurl, 0.5f) &&
			CheckCurlMax(skeleton.thumbCurl, 0.6f))
		{
			//Debug.Log("Cool, gesture recognized");
			// Only check the start position once
			if (!appearGestureStarted)
			{
				//Debug.Log("Gesture start pos set");
				appearGestureStarted = true;
				startFingerPosY = leftHand.mainRenderModel.GetBonePosition((int)SteamVR_Skeleton_JointIndexEnum.indexTip).y;
			}

			// Check if we reached the requested distance before the timer runs out
			if (curTimeRemaining > 0)
			{

				curTimeRemaining -= Time.deltaTime;

				float currentIndexTipY = leftHand.mainRenderModel.GetBonePosition((int)SteamVR_Skeleton_JointIndexEnum.indexTip).y;
				if (Mathf.Abs(startFingerPosY - currentIndexTipY) > minTriggerDistance && currentIndexTipY < startFingerPosY)
				{
					interfaceManager.ToggleCatogoryMenu(true);
					openedInterface = true;
					appearGestureStarted = false;
					curTimeRemaining = maxTime;
				}
			}
			else
			{
				Debug.Log("Time ran out!");
				curTimeRemaining = maxTime;
				//gestureStarted = false;
			}
		}
		else
		{
			appearGestureStarted = false;
		}

		if (Input.GetKeyDown(KeyCode.G))
		{
			appearGestureStarted = false;
			openedInterface = false;
			interfaceManager.ToggleCatogoryMenu(false);
		}
	}

	private void CheckDismissGesture()
	{
		if (openedInterface &&
			CheckCurlMin(skeleton.indexCurl, 0.1f) &&
			CheckCurlMin(skeleton.middleCurl, 0.1f) &&
			CheckCurlMin(skeleton.ringCurl, 0.1f) &&
			CheckCurlMin(skeleton.pinkyCurl, 0.1f) &&
			CheckCurlMin(skeleton.thumbCurl, 0.1f))
		{
			//Debug.Log("Closing gesture detected");
			// Only check the start position once
			if (!dismissGestureStarted)
			{
				//Debug.Log("Dismiss start pos set");
				dismissGestureStarted = true;
				startFingerPosX = leftHand.mainRenderModel.GetBonePosition((int)SteamVR_Skeleton_JointIndexEnum.indexTip).x;
			}

			if (curTimeRemaining > 0)
			{
				curTimeRemaining -= Time.deltaTime;

				float currentIndexTipX = leftHand.mainRenderModel.GetBonePosition((int)SteamVR_Skeleton_JointIndexEnum.indexTip).x;
				if (Mathf.Abs(startFingerPosX - currentIndexTipX) > minTriggerDistance)
				{
					//Debug.Log("GO AWAY BALLS");
					interfaceManager.ToggleCatogoryMenu(false);
					openedInterface = false;
					dismissGestureStarted = false;
					curTimeRemaining = maxTime;
				}
			}
			else
			{
				Debug.Log("Time ran out!");
				curTimeRemaining = maxTime;
			}
		}
	}
}
