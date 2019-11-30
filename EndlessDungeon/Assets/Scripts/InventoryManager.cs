using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    private static InventoryManager instance;
    public static InventoryManager Instance => instance;

    [SerializeField]
    private GameObject inventoryPanel, statsPanel;

    [SerializeField]
    private EquipmentDisplay equipment;

    [SerializeField]
    private BagDisplay bag;

    [SerializeField]
    private RawImage heldItemImage;
    private ItemObject heldItem;

    [SerializeField]
    private ItemTooltip itemTooltip;

    private EventSystem eventSystem;

    public ItemObject HeldItem
    {
        get => heldItem;
        set
        {
            heldItem = value;
            if (heldItem)
            {
                heldItemImage.texture = heldItem.Icon;
                //heldItemImage.rectTransform.anchorMin = new Vector2(0, 1f - heldItem.InventorySizeY / 20f);
                //heldItemImage.rectTransform.anchorMax = new Vector2(heldItem.InventorySizeX / 12.5f, 1);
                heldItemImage.rectTransform.sizeDelta = heldItem.InventorySize * 64;

                heldItemImage.enabled = true;
                PositionHeldItem();
            }
            else
            {
                heldItemImage.enabled = false;
                heldItemImage.texture = null;
            }
        }
    }

    public void Awake()
    {
        instance = this;
        heldItemImage.enabled = false;
        eventSystem = EventSystem.current;
    }


    private void PositionHeldItem()
    {
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(bag.transform.parent.transform as RectTransform, Input.mousePosition, null, out Vector2 localpoint);
        heldItemImage.rectTransform.position = Input.mousePosition;
        Cursor.visible = heldItem == null;
    }

    public void LateUpdate()
    {
        PositionHeldItem();
        
    }
    public void Update()
    {
        if (heldItem != null)
        {
            itemTooltip.SetVisible(false);
        }
        else if (bag.Hover && bag.Hover.Item)
        {
            itemTooltip.Inspect(bag.Hover);
            itemTooltip.SetVisible(true);
        }
        else if (equipment.Hover && equipment.Hover.Item)
        {
            itemTooltip.Inspect(equipment.Hover);
            itemTooltip.SetVisible(true);
        }
        else
        {
            itemTooltip.SetVisible(false);
        }


        if (Input.GetKeyDown(KeyCode.I))
        {
            bool active = inventoryPanel.gameObject.activeSelf;
            inventoryPanel.SetActive(!active);
        }

        if (heldItem != null && Input.GetMouseButtonDown(0) && !eventSystem.IsPointerOverGameObject())
        {
            heldItem.transform.position = Player.Instance.GetCastPosition();
            heldItem.transform.rotation = Player.Instance.transform.rotation * Quaternion.Euler(0, 0, 90);
            heldItem.gameObject.SetActive(true);
            heldItem.gameObject.layer = LayerMask.NameToLayer("Default");
            heldItem.Rigidbody.isKinematic = false;
            heldItem.Rigidbody.useGravity = true;
            heldItem.Rigidbody.AddForce(Vector3.up * 10, ForceMode.VelocityChange);
            Level.Instance.EnableItem(heldItem);
            HeldItem = null;
        }
    }
}
