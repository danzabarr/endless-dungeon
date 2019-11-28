using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorMessageDisplay : MonoBehaviour
{
    public Text Text { get; private set; }

    private float timer;
    private new Coroutine animation;
    public float fadeInDuration, fadeOutDuration;
    public float displayDuration;
    private Color color;
    private bool shown;

    // Start is called before the first frame update
    void Start()
    {
        Text = GetComponent<Text>();
        color = Text.color;
        Text.color = Color.clear;
    }
    public void ErrorMessage(string text)
    {
        Text.text = text;
        timer = displayDuration;
        if (!shown)
            animation = StartCoroutine(AnimateTo(color, fadeInDuration));
        
        shown = true;
    }

    [ContextMenu("Test Message")]
    public void TestMessage()
    {
        ErrorMessage("Test");
    }

    public void FixedUpdate()
    {
        if (timer > 0)
            timer -= Time.deltaTime;
        
        if (timer <= 0 && shown)
        {
            timer = 0;
            if (animation != null) StopCoroutine(animation);
            animation = StartCoroutine(AnimateTo(Color.clear, fadeOutDuration));
            shown = false;
        }
    }

    IEnumerator AnimateTo(Color color, float time)
    {
        Color oldColor = Text.color;

        for (float i = 0; i <= time; i += Time.deltaTime)
        {
            Text.color = Color.Lerp(oldColor, color, i);
            yield return null;
        }
        
        Text.color = color;
    }
}
