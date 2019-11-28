using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public StatsPanelTooltip tooltip;
    public string text;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (text == null || text.Length == 0)
            return;
        Vector2 position = transform.position;
        position.x -= 6;
        position.y += 20;
        tooltip.Show(text, position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (text == null || text.Length == 0)
            return;
        tooltip.Hide();
    }
}
