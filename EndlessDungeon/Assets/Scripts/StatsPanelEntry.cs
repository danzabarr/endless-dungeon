using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsPanelEntry : MonoBehaviour
{
    [SerializeField]
    private Text variableText, valueText;
    [SerializeField]
    private TooltipHover tooltipHover;
    private Vector2 statValue = Vector2.zero;
    private string format = "";
    private bool vectorValue;
    public TooltipHover TooltipHover => tooltipHover;

    public string Name
    {
        get => variableText.text;
        set => variableText.text = value;
    }

    public float FloatValue
    {
        get => statValue.x;
        set
        {
            vectorValue = false;
            statValue.x = value;
            valueText.text = string.Format(format, statValue.x);
        }
    }

    public Vector2 VectorValue
    {
        get => statValue;
        set
        {
            vectorValue = true;
            statValue = value;
            valueText.text = string.Format(format, statValue.Min(), statValue.Max());
        }
    }

    public string Format
    {
        get => format;
        set
        {
            format = value;
            if (vectorValue)
                valueText.text = string.Format(format, statValue.Min(), statValue.Max());
            else 
                valueText.text = string.Format(format, statValue.x);
        }
    }
}
