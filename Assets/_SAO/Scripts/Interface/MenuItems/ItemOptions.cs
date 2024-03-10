using DG.Tweening;
using PlayerInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class OptionButton {
    public Image image;
    public Button button;
    public Sprite selectedSprite;
    public Sprite unselectedSprites;
    internal bool isSelected;
}

public class ItemOptions : MonoBehaviour
{
    [SerializeField] private EquipmentMenuItem item;
    public RectTransform optionsRect;
    public OptionButton[] optionButtons;
    public Image background, activeBackground;

    private GameObject spawnedItem;

    /// <summary>
    /// Open the options overlay
    /// Called from the inspector
    /// </summary>
    public void OpenOptions()
    {
        Debug.Log("Open Options " + gameObject.name);

        // Check if it is equiped, if not, then reset the window
        if (!optionButtons[0].isSelected)
        {
            //resetWindow();
        }
        
        item.selectButton.interactable = false;
        optionsRect.gameObject.SetActive(true);
        optionsRect.DOAnchorPosX(0, 0.25f);
        
    }
    
    /// <summary>
    /// Close the options overlay
    /// Called from the inspector
    /// </summary>
    public void CloseOptions()
    {
        // Disable button interaction
        optionsRect.DOAnchorPosX(optionsRect.rect.width, 0.2f).OnComplete(() => {
            optionsRect.gameObject.SetActive(false);
            item.selectButton.interactable = true;
        });
    }

    private void resetWindow()
    {
        // Reset the buttons
        background.gameObject.SetActive(true);
        activeBackground.gameObject.SetActive(false);

        foreach (OptionButton button in optionButtons)
        {
            // Reset the sprite
            button.image.sprite = button.unselectedSprites;
            button.button.gameObject.SetActive(true);
            button.isSelected = false;
        }
    }

    public void Close()
    {
        item.Interact();
    }

    public void Equip()
    {
        // TODO: Does an item need to be removed from the inventory if it has been spawned? Or can you just recall it?
        if (!optionButtons[0].isSelected)
        {
            optionButtons[0].isSelected = true;
            SelectButton(0);

            if (item.equipmentData.objectPrefab)
            {
                ItemSpawner.Instance.SetSpawnPosition(new Vector3(-0.8f, 1.1f, -18.5f), new Vector3(-39.5f, 45, -84.5f));
                spawnedItem = ItemSpawner.Instance.SpawnItem(SpawnPosition.Position, item.equipmentData.objectPrefab);
            }
        } else
        {
            ItemSpawner.Instance.DespawnItem(spawnedItem);
            resetWindow();
        }
    }

    public void Delete()
    {
        // Disable the interaction
        item.Interact();
        
        // Delete the item
        item.RemoveItem();
    }

    private void SelectButton(int index)
    {
        background.gameObject.SetActive(false);
        activeBackground.gameObject.SetActive(true);
        
        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (i == index)
            {
                optionButtons[i].image.sprite = optionButtons[i].selectedSprite;
            }
            else
            {
                optionButtons[i].button.gameObject.SetActive(false);
            }
        }
    }

    private void OnDestroy()
    {
        DOTween.Kill(optionsRect);
    }
}
