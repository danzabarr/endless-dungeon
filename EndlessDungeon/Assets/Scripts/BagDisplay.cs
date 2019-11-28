using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class BagDisplay : MonoBehaviour
{
    [SerializeField]
    private ItemDisplay itemDisplayPrefab;

    private Inventory inventory;

    private Vector2 mousePosition;
    private Vector2Int mouseSlot;
    private ItemObject mouseItem;

    private Vector2Int swapSlot;
    private ItemObject swapItem;

    [SerializeField]
    private Image highlight;

    [SerializeField]
    private Color validHighlight;

    [SerializeField]
    private Color invalidHighlight;

    [SerializeField]
    private Color swapHighlight;

    public ItemDisplay Hover { get; private set; }

    private List<ItemDisplay> displayedItems = new List<ItemDisplay>();

    public Inventory Inventory
    {
        get => inventory;
        set
        {
            inventory = value;
            Refresh();
        }
    }

    public void Refresh()
    {
        displayedItems.Clear();
        foreach (Transform child in transform)
        {
            if (child == highlight.transform)
                continue;
            DestroyImmediate(child.gameObject);
        }


        if (inventory == null)
            return;

        int i = 0;

        foreach(ItemObject item in inventory)
        {
            if (item != null)
            {
                int x = i % inventory.XSize;
                int y = i / inventory.XSize;
                ItemDisplay id = Instantiate(itemDisplayPrefab, transform);
                id.Item = item;
                displayedItems.Add(id);
                SetChildTransform(id.transform as RectTransform, x, y, item.InventorySizeX, item.InventorySizeY);
            }
            i++;
        }
    }
    private void HideHighlight()
    {
        highlight.gameObject.SetActive(false);
    }

    private void ShowHighlight(Color color, int x, int y, int w, int h)
    {
        highlight.gameObject.SetActive(true);
        highlight.color = color;
        SetChildTransform(highlight.transform as RectTransform, x, y, w, h);
    }

    private void SetChildTransform(RectTransform child, int x, int y, int w, int h)
    {
        child.anchorMin = new Vector2
        (
            (float)x / inventory.XSize,
            (float)(inventory.YSize - y - h) / inventory.YSize
        );
        child.anchorMax = new Vector2
        (
            (float)(x + w) / inventory.XSize,
            (float)(inventory.YSize - y) / inventory.YSize
        );
    }

    public void Refresh(ItemObject item)
    {
        if (item == null)
            return;
        for (int i = 0; i < displayedItems.Count; i++)
        {
            ItemDisplay get = displayedItems[i];
            if (get && get.Item == item)
            {
                displayedItems.RemoveAt(i);
                DestroyImmediate(get.gameObject);
                return;
            }
        }

        if (inventory.Contains(item, out int x, out int y))
        {
            ItemDisplay id = Instantiate(itemDisplayPrefab, transform);
            id.Item = item;
            displayedItems.Add(id);
            SetChildTransform(id.transform as RectTransform, x, y, item.InventorySizeX, item.InventorySizeY);
        }
    }

    public void LateUpdate()
    {

        void Include(ItemObject item, int x, int y)
        {
            ItemDisplay id = Instantiate(itemDisplayPrefab, transform);
            id.Item = item;
            displayedItems.Add(id);
            SetChildTransform(id.transform as RectTransform, x, y, item.InventorySizeX, item.InventorySizeY);
        }

        void Delete(ItemObject item)
        {
            if (item == null)
                return;
            for (int i = 0; i < displayedItems.Count; i++)
            {
                ItemDisplay get = displayedItems[i];
                if (get && get.Item == item)
                {
                    displayedItems.RemoveAt(i);
                    DestroyImmediate(get.gameObject);
                    return;
                }
            }
        }

        RectTransform rectTransform = transform as RectTransform;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, null, out Vector2 localpoint);
        mousePosition = Rect.PointToNormalized(rectTransform.rect, localpoint);
        if (mousePosition.x > 0 && mousePosition.y > 0 && mousePosition.x < 1 && mousePosition.y < 1)
        {
            mouseSlot = new Vector2Int(Mathf.FloorToInt(mousePosition.x * inventory.XSize), Mathf.FloorToInt((1 - mousePosition.y) * inventory.YSize));
            mouseItem = inventory.Get(mouseSlot.x, mouseSlot.y, out int arrayX, out int arrayY);

            if (InventoryManager.Instance.HeldItem == null)
            {
                if (mouseItem != null)
                {
                    ShowHighlight(validHighlight, arrayX, arrayY, mouseItem.InventorySizeX, mouseItem.InventorySizeY);
                }
                else
                {
                    HideHighlight();
                }

                if (Input.GetMouseButtonDown(0))
                {
                    if (mouseItem != null)
                    {
                        InventoryManager.Instance.HeldItem = inventory.Remove(mouseSlot.x, mouseSlot.y);
                        Delete(InventoryManager.Instance.HeldItem);
                    }
                }
            }
            else
            {
                ItemObject heldItem = InventoryManager.Instance.HeldItem;

                int w = heldItem.InventorySizeX;
                int h = heldItem.InventorySizeY;
                swapSlot = new Vector2Int(Mathf.FloorToInt(mousePosition.x * inventory.XSize - w / 2f + 0.5f), Mathf.FloorToInt((1 - mousePosition.y) * inventory.YSize - h / 2f + 0.5f));

                if (swapSlot.x < 0 || swapSlot.y < 0 || swapSlot.x + w - 1 >= inventory.XSize || swapSlot.y + h - 1 >= inventory.YSize)
                {
                    swapSlot = new Vector2Int(-1, -1);
                    HideHighlight();
                }
                else if (inventory.Add(heldItem, swapSlot.x, swapSlot.y, out swapItem, out int swapX, out int swapY, false))
                {
                    if (swapItem == null)
                    {
                        ShowHighlight(validHighlight, swapSlot.x, swapSlot.y, w, h);

                        if (Input.GetMouseButtonDown(0))
                        {
                            if (inventory.Add(heldItem, swapSlot.x, swapSlot.y))
                            {
                                InventoryManager.Instance.HeldItem = null;
                                Include(heldItem, swapSlot.x, swapSlot.y);
                            }
                        }

                    }
                    else
                    {
                        ShowHighlight(swapHighlight, swapX, swapY, swapItem.InventorySizeX, swapItem.InventorySizeY);

                        if (Input.GetMouseButtonDown(0))
                        {
                            if (inventory.Add(heldItem, swapSlot.x, swapSlot.y))
                            {
                                InventoryManager.Instance.HeldItem = swapItem;
                                Include(heldItem, swapSlot.x, swapSlot.y);
                                Delete(swapItem);
                            }
                        }
                    }
                }
                else
                {
                    ShowHighlight(invalidHighlight, swapSlot.x, swapSlot.y, w, h);
                }
            }


            if (mouseItem == null)
            {
                Hover = null;
            }
            else
            {
                foreach (ItemDisplay id in displayedItems)
                    if (id.Item == mouseItem)
                    {
                        Hover = id;
                        break;
                    }
            }
        }
        else
        {
            Hover = null;
            mouseSlot = new Vector2Int(-1, -1);
            mouseItem = null;
            HideHighlight();
        }
    }
}
