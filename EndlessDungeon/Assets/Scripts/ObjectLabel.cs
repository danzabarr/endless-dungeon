using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectLabel : FollowWorldTransform
{
    [SerializeField]
    private CanvasGroup canvasGroup;
    [SerializeField]
    private Text labelText;
    [SerializeField]
    private Image labelBackground;
    [SerializeField]
    private Image barFill;
    [SerializeField]
    private Image barBackground;

    [SerializeField]
    private Color regular, hover;

    private float opacity;
    private float color;

    [SerializeField]
    private float fadeInSpeed, fadeOutSpeed;
    public bool Hover { set => color = value ? 2 : 0; }
    public bool LabelEnabled { set => labelBackground.gameObject.SetActive(value); }
    public string LabelText { set => labelText.text = value; }
    public int LabelTextFontSize { set => labelText.fontSize = value; }
    public Color LabelTextColor { set => labelText.color = value; }
    public Vector3 LabelWorldOffset { set => worldOffset = value; }
    public Vector2 LabelScreenOffset { set => screenOffset = value; }
    public bool LabelSupportRichText { set => labelText.supportRichText = value; }
    public bool BarEnabled { set => barBackground.gameObject.SetActive(value); }
    public Color BarFillColor { set => barFill.color = value; }
    public float BarFillAmount { set => barFill.rectTransform.anchorMax = new Vector2(Mathf.Clamp(value, 0, 1), 1); }
    public bool SeparateRectangles { get; set; }
    public bool ZSort { get; set; }
    
    /*
    public void Update()
    {

        if (focus)
            opacity = Mathf.Lerp(opacity, 1, Time.deltaTime * fadeInSpeed);
        else
            opacity = Mathf.Lerp(opacity, 0, Time.deltaTime * fadeOutSpeed);

        if (opacity < .001f)
            opacity = 0;
        if (opacity > .999f)
            opacity = 1;

        canvasGroup.alpha = opacity;


        background.color = Color.Lerp(regular, hover, color);
        color -= Time.deltaTime * fadeOutSpeed;
        color = Mathf.Clamp(color, 0, 1);
    }
    */
}
