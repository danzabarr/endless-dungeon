using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static EquipmentObject;

public class EquipmentDisplay : MonoBehaviour
{
    [SerializeField]
    private EquipmentManager equipment;

    [SerializeField]
    private ItemDisplay head, body, hands, feet, neck, finger, mainHand, offHand;

    private Slot hover = Slot.None;

    [SerializeField]
    private Color validColor;
    [SerializeField]
    private Color invalidColor;

    public ItemDisplay Hover { get; private set; }

    public EquipmentManager Equipment
    {
        get => equipment;
        set
        {
            equipment = value;
            Refresh();
        }
    }

    public void Start()
    {
        head.SlotSize = new Vector2Int(2, 2);
        body.SlotSize = new Vector2Int(2, 3);
        hands.SlotSize = new Vector2Int(2, 2);
        feet.SlotSize = new Vector2Int(2, 2);
        neck.SlotSize = new Vector2Int(1, 1);
        finger.SlotSize = new Vector2Int(1, 1);
        mainHand.SlotSize = new Vector2Int(2, 4);
        offHand.SlotSize = new Vector2Int(2, 4);

        Refresh();
    }

    public void Refresh()
    {
        head.Item = equipment.Head;
        body.Item = equipment.Body;
        hands.Item = equipment.Hands;
        feet.Item = equipment.Feet;
        neck.Item = equipment.Neck;
        finger.Item = equipment.Finger;
        mainHand.Item = equipment.MainHand;
        offHand.Item = equipment.OffHand;
    }

    public void LateUpdate()
    {
        Slot hover = Slot.None;
        ItemObject heldItem = InventoryManager.Instance.HeldItem;
        ItemDisplay hoverItem = null;

        void Set(Slot slot, EquipmentObject item)
        {
            switch (slot)
            {
                case Slot.None:
                    break;
                case Slot.Head:
                    equipment.Head = item;
                    head.Item = item;
                    break;
                case Slot.Body:
                    equipment.Body = item;
                    body.Item = item;
                    break;
                case Slot.Hands:
                    equipment.Hands = item;
                    hands.Item = item;
                    break;
                case Slot.Feet:
                    equipment.Feet = item;
                    feet.Item = item;
                    break;
                case Slot.Neck:
                    equipment.Neck = item;
                    neck.Item = item;
                    break;
                case Slot.Finger:
                    equipment.Finger = item;
                    finger.Item = item;
                    break;
                case Slot.MainHand:
                    equipment.MainHand = item;
                    mainHand.Item = item;
                    break;
                case Slot.OffHand:
                    equipment.OffHand = item;
                    offHand.Item = item;
                    break;
            }
        }

        bool MouseIsOver(RectTransform rectTransform)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, null, out Vector2 localpoint);
            Vector2 mousePosition = Rect.PointToNormalized(rectTransform.rect, localpoint);
            return mousePosition.x > 0 && mousePosition.y > 0 && mousePosition.x < 1 && mousePosition.y < 1;
        }

        

        void HoverOver(ItemDisplay item, Slot slot)
        {
            if (MouseIsOver(item.transform as RectTransform))
            {
                if (heldItem == null)
                {
                    if (item.Item != null)
                    {
                        item.ShowHighlight(validColor);
                        if (Input.GetMouseButtonDown(0))
                        {
                            InventoryManager.Instance.HeldItem = item.Item;
                            Set(slot, null);
                        }
                    }
                }
                else
                {
                    if (heldItem is EquipmentObject)
                    {
                        EquipmentObject heldEquipment = heldItem as EquipmentObject;

                        if (heldEquipment.ItemClass.Equippable(slot)/*TODO: Add other requirements*/)
                        {
                            item.ShowHighlight(validColor);
                            if (Input.GetMouseButtonDown(0))
                            {
                                InventoryManager.Instance.HeldItem = item.Item;
                                Set(slot, heldEquipment);
                            }
                        }
                        else
                        {
                            item.ShowHighlight(invalidColor);
                        }
                    }
                    else
                    {
                        item.ShowHighlight(invalidColor);
                    }
                }
                hover = slot;
                hoverItem = item;
            }
            else
            {
                item.HideHighlight();
            }
        }


        HoverOver(head, Slot.Head);
        HoverOver(body, Slot.Body);
        HoverOver(hands, Slot.Hands);
        HoverOver(feet, Slot.Feet);
        HoverOver(neck, Slot.Neck);
        HoverOver(finger, Slot.Finger);
        HoverOver(mainHand, Slot.MainHand);
        HoverOver(offHand, Slot.OffHand);

        this.hover = hover;
        Hover = hoverItem;
    }

    

}
