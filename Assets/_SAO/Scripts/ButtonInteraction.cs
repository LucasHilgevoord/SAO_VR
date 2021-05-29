using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using PlayerInterface;

public class ButtonInteraction : Interactable
{
	public MenuItem menuItem;
	protected override void OnHandHoverEnd(Hand hand)
	{
		base.OnHandHoverEnd(hand);
		menuItem.Interact();
	}
}
