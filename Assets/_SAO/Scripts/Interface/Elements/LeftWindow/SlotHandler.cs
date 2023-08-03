using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotHandler : MonoBehaviour
{
    [SerializeField] private List<SlotObject> _slots;
    [SerializeField] private List<SlotObject> _selectedSlots;
    private float _slotAppearDelay = 0.1f;

    private void Start()
    {
        
    }

    private void Update()
    {
        if (InputHandler.Instance.wasKeyPressedThisFrame(UnityEngine.InputSystem.Key.N))
        {
            SelectSlots(new PlayerWindowSlot[] { new PlayerWindowSlot(SlotType.Skill)});
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

    public void SelectSlot(int typeIndex)
    {
        // convert the single int to a list of PlayerWindowSlot
        SelectSlots(new PlayerWindowSlot[] { new PlayerWindowSlot((SlotType)typeIndex, SlotSide.None)});
    }

    public void SelectSlots(int[][] slotTypes)
    {
        // Convert the int array to a PlayerWindowSlot array
        PlayerWindowSlot[] slots = new PlayerWindowSlot[slotTypes.Length];
        for (int i = 0; i < slotTypes.Length; i++)
        {
            slots[i] = new PlayerWindowSlot((SlotType)slotTypes[i][0], (SlotSide)slotTypes[i][1]);
        }
    }

    internal void SelectSlots(PlayerWindowSlot[] slotTypes)
    {   
        // Check which slots to highlight
        _selectedSlots = new List<SlotObject>();
        foreach (PlayerWindowSlot type in slotTypes)
        {
            Debug.Log(type.slotType);
            foreach (SlotObject slot in _slots)
            {
                if (_selectedSlots.Contains(slot)) { Debug.Log("contains"); continue; }
                if (slot.Type.slotType == type.slotType && (type.slotSide == SlotSide.None || slot.Type.slotSide == type.slotSide))
                {
                    Debug.Log("Added");
                    _selectedSlots.Add(slot);
                    continue;
                }
            }
        }

        StartCoroutine(SelectCoroutine());
    }

    private IEnumerator SelectCoroutine()
    {
        for (int i = 0; i < _slots.Count; i++)
        {
            if (!_selectedSlots.Contains(_slots[i]))
                _slots[i].Hide();

            if (!_selectedSlots.Contains(_slots[i + 1]))
                _slots[i + 1].Hide();
            
            i++;
            yield return new WaitForSeconds(_slotAppearDelay);
        }
        
        yield return new WaitForSeconds(.5f);
        foreach (SlotObject slot in _selectedSlots)
        {
            slot.Select();
        }
    }
}
