using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsPanelTooltip : MonoBehaviour
{
    [SerializeField]
    private Text text;

    public void Show(string tooltip, Vector3 position)
    {
        text.text = tooltip;
        gameObject.transform.position = position;
        gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
