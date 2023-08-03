using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotHandler : MonoBehaviour
{
    [SerializeField] private List<SlotObject> _slots;
    private float _slotAppearDelay = 0.1f;

    private void Start()
    {
        
    }

    private void Update()
    {
        if (InputHandler.Instance.wasKeyPressedThisFrame(UnityEngine.InputSystem.Key.N))
        {
            
        }
    }
    
    internal void ShowSlots()
    {
        StartCoroutine(ShowSlotsCoroutine());
    }

    private IEnumerator ShowSlotsCoroutine()
    {
        for (int i = 0; i < _slots.Count; i++)
        {
            // Show two at the time
            _slots[i].Show();
            _slots[i + 1].Show();
            i++;
            yield return new WaitForSeconds(_slotAppearDelay);
        }
    }

    private void HighlightSlots(Slot[] slotTypes)
    {
        List<SlotObject> selectedSlots = new List<SlotObject>();
        foreach (Slot type in slotTypes)
        {
            foreach (SlotObject slot in _slots)
            {
                if (selectedSlots.Contains(slot)) { continue; }

                if (slot.Type.slotType == type.slotType && (slot.Type.slotSide == SlotSide.None || slot.Type.slotSide == type.slotSide))
                {
                    selectedSlots.Add(slot);
                    continue;
                }
            }
        }
    }
}
