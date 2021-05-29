using PlayerInterface;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

[Serializable]
public class HandGesture
{
	public SteamVR_Action_Skeleton skeleton;
	public Hand hand;
	internal bool appearGestureStarted;
	internal bool dismissGestureStarted;
	internal float curTimeRemaining;
}

public class InterfaceGesture : MonoBehaviour
{
	public InterfaceManager interfaceManager;
	public HandGesture leftHand, rightHand;
	private float startFingerPosY;
	private float startFingerPosX;

	private float maxTime = 3;
	private float minTriggerDistance = 0.2f;
	private bool openedInterface;

	private void Start()
	{
		leftHand.curTimeRemaining = maxTime;
		rightHand.curTimeRemaining = maxTime;
	}

	private void Update()
	{
		CheckAppearGesture(leftHand);
		CheckAppearGesture(rightHand);

		CheckDismissGesture(leftHand);
		CheckDismissGesture(rightHand);
	}

	private bool CheckCurlMin(float curl, float minCurl) { return curl <= minCurl; }
	private bool CheckCurlMax(float curl, float maxCurl) { return curl >= maxCurl; }

	private void CheckAppearGesture(HandGesture hand)
	{
		if (hand.hand.mainRenderModel == null)
			return;

		//Debug.Log(skeleton.indexCurl + " | " + skeleton.middleCurl + " | " + skeleton.ringCurl + " | " + skeleton.pinkyCurl + " | " + skeleton.thumbCurl);

		if (!openedInterface &&
			CheckCurlMin(hand.skeleton.indexCurl, 0.05f) &&
			CheckCurlMin(hand.skeleton.middleCurl, 0.15f) &&
			CheckCurlMax(hand.skeleton.ringCurl, 0.5f) &&
			CheckCurlMax(hand.skeleton.pinkyCurl, 0.5f) &&
			CheckCurlMax(hand.skeleton.thumbCurl, 0.6f))
		{
			//Debug.Log("Cool, gesture recognized");
			// Only check the start position once
			if (!hand.appearGestureStarted)
			{
				//Debug.Log("Gesture start pos set");
				hand.appearGestureStarted = true;
				startFingerPosY = hand.hand.mainRenderModel.GetBonePosition((int)SteamVR_Skeleton_JointIndexEnum.indexTip).y;
			}

			// Check if we reached the requested distance before the timer runs out
			if (hand.curTimeRemaining > 0)
			{

				hand.curTimeRemaining -= Time.deltaTime;

				float currentIndexTipY = hand.hand.mainRenderModel.GetBonePosition((int)SteamVR_Skeleton_JointIndexEnum.indexTip).y;
				if (Mathf.Abs(startFingerPosY - currentIndexTipY) > minTriggerDistance && currentIndexTipY < startFingerPosY)
				{
					interfaceManager.ToggleCatogoryMenu(true);
					openedInterface = true;
					hand.appearGestureStarted = false;
					hand.curTimeRemaining = maxTime;
				}
			}
			else
			{
				Debug.Log("Time ran out!");
				hand.curTimeRemaining = maxTime;
				//gestureStarted = false;
			}
		}
		else
		{
			hand.appearGestureStarted = false;
		}

		if (Input.GetKeyDown(KeyCode.G))
		{
			hand.appearGestureStarted = false;
			openedInterface = false;
			interfaceManager.ToggleCatogoryMenu(false);
		}
	}

	private void CheckDismissGesture(HandGesture hand)
	{
		if (openedInterface &&
			CheckCurlMin(hand.skeleton.indexCurl, 0.1f) &&
			CheckCurlMin(hand.skeleton.middleCurl, 0.1f) &&
			CheckCurlMin(hand.skeleton.ringCurl, 0.1f) &&
			CheckCurlMin(hand.skeleton.pinkyCurl, 0.1f) &&
			CheckCurlMin(hand.skeleton.thumbCurl, 0.1f))
		{
			//Debug.Log("Closing gesture detected");
			// Only check the start position once
			if (!hand.dismissGestureStarted)
			{
				//Debug.Log("Dismiss start pos set");
				hand.dismissGestureStarted = true;
				startFingerPosX = hand.hand.mainRenderModel.GetBonePosition((int)SteamVR_Skeleton_JointIndexEnum.indexTip).x;
			}

			if (hand.curTimeRemaining > 0)
			{
				hand.curTimeRemaining -= Time.deltaTime;

				float currentIndexTipX = hand.hand.mainRenderModel.GetBonePosition((int)SteamVR_Skeleton_JointIndexEnum.indexTip).x;
				if (Mathf.Abs(startFingerPosX - currentIndexTipX) > minTriggerDistance)
				{
					//Debug.Log("GO AWAY BALLS");
					interfaceManager.ToggleCatogoryMenu(false);
					openedInterface = false;
					hand.dismissGestureStarted = false;
					hand.curTimeRemaining = maxTime;
				}
			}
			else
			{
				Debug.Log("Time ran out!");
				hand.curTimeRemaining = maxTime;
			}
		}
	}
}
