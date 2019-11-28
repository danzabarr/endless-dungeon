using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDisplay : MonoBehaviour
{
    [SerializeField]
    private RawImage image;
    [SerializeField]
    private Image background;
    private ItemObject item;
    private Vector2Int slotSize;

    public void ShowHighlight(Color color)
    {
        if (!background)
            return;
        background.enabled = true;
        background.color = color;
    }

    public void HideHighlight()
    {
        if (!background)
            return;
        background.enabled = false;
    }

    public Vector2Int SlotSize
    {
        get => slotSize;
        set
        {
            slotSize = value;
            ResizeItemImage();
        }
    }
    
    private void ResizeItemImage()
    {
        if (image == null)
            return;
        if (item == null)
            return;
        if (slotSize == Vector2Int.zero)
            return;

        float x = ((slotSize.x - item.InventorySizeX) / 2f) / slotSize.x;
        float y = ((slotSize.y - item.InventorySizeY) / 2f) / slotSize.y;

        image.rectTransform.anchorMin = new Vector2(x, y);
        image.rectTransform.anchorMax = new Vector2(1 - x, 1 - y);
    }

    public ItemObject Item
    {
        get => item;
        set
        {
            item = value;
            if (item == null)
            {
                image.enabled = false;
                image.texture = null;
            }

            else
            {
                image.enabled = true;
                image.texture = value.Icon;
                ResizeItemImage();
            }
        }
    }

    
}
