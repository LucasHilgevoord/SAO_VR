using PlayerInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class IndexInput : MonoBehaviour
{
	public InterfaceManager interfaceManager;
    public SteamVR_Action_Skeleton skeleton;
	public Hand leftHand;
	private float startFingerPosY;

	private float curTimeRemaining;
	private float maxTime = 3;
	private float minTriggerDistance = 0.2f;

	private bool gestureStarted;
	private bool openedInterface;

	private void Update()
	{
		CheckGesture();
	}

	private bool CheckCurlMin(float curl, float minCurl) { return curl <= minCurl; }
	private bool CheckCurlMax(float curl, float maxCurl) { return curl >= maxCurl; }

	private void CheckGesture()
	{
		if (leftHand.mainRenderModel == null)
			return;

		//Debug.Log(skeleton.ringCurl + " | " + skeleton.pinkyCurl + " | " + skeleton.thumbCurl);

		if (!openedInterface &&
			CheckCurlMin(skeleton.indexCurl, 0.05f) &&
			CheckCurlMin(skeleton.middleCurl, 0.15f) &&
			CheckCurlMax(skeleton.ringCurl, 0.5f) &&
			CheckCurlMax(skeleton.pinkyCurl, 0.5f) &&
			CheckCurlMax(skeleton.thumbCurl, 0.6f))
		{
			Debug.Log("Cool, gesture recognized");
			// Only check the start position once
			if (!gestureStarted)
			{
				Debug.Log("Gesture start pos set");
				gestureStarted = true;
				startFingerPosY = leftHand.mainRenderModel.GetBonePosition((int)SteamVR_Skeleton_JointIndexEnum.indexTip).y;
			}

			// Check if we reached the requested distance before the timer runs out
			if (curTimeRemaining > 0)
			{

				curTimeRemaining -= Time.deltaTime;

				if (Mathf.Abs(startFingerPosY - leftHand.mainRenderModel.GetBonePosition((int)SteamVR_Skeleton_JointIndexEnum.indexTip).y) > minTriggerDistance)
				{
					interfaceManager.ToggleCatogoryMenu(true);
					openedInterface = true;
					gestureStarted = false;
					curTimeRemaining = maxTime;
				}
			}
			else
			{
				Debug.Log("Time ran out!");
				curTimeRemaining = maxTime;
				gestureStarted = false;
			}
		}
		else
		{
			gestureStarted = false;
		}

		if (Input.GetKeyDown(KeyCode.G))
		{
			gestureStarted = false;
			openedInterface = false;
			interfaceManager.ToggleCatogoryMenu(false);
		}
	}
}
