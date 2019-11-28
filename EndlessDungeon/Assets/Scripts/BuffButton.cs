using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffButton : MonoBehaviour
{
    [SerializeField]
    private Image icon;

    [SerializeField]
    private Shader iconShader;

    [SerializeField]
    private Text timerText;

    [SerializeField]
    private Text stacksText;

    [SerializeField]
    private Image border;

    private BuffInstance buff;

    public void Awake()
    {
        
    }

    public void OnDestroy()
    {
        Destroy(icon.material);
    }

    public void Init(BuffInstance buff, Color borderColor)
    {
        this.buff = buff;
        icon.material = new Material(icon.material);
        
        icon.material.SetTexture("_MainTex", buff.Icon);
        border.color = borderColor;
        if (buff.Permanent)
        {
            timerText.text = "";
            icon.material.SetFloat("_Fraction", 0);
        }
        stacksText.text = buff.Stacks > 1 ? (buff.Stacks + "") : "";
    }

    public void Update()
    {
        if (buff.Finished)
        {
            Destroy(gameObject);
            return;
        }

        if (!buff.Permanent)
        {
            timerText.text = FormatTimeRemaining(buff.SecondsElapsed, buff.SecondsMax);
            icon.material.SetFloat("_Fraction", buff.FractionElapsed);
        }

        stacksText.text = buff.Stacks > 1 ? (buff.Stacks + "") : "";
    }

    public static string FormatTimeRemaining(float secondsElapsed, float maxSeconds)
    {
        float secondsRemaining = Mathf.Max(0, maxSeconds - secondsElapsed);

        if (secondsRemaining >= 3600)
            return Mathf.CeilToInt(secondsRemaining / 3600) + " h";
        else if (secondsRemaining > 60)
            return Mathf.CeilToInt(secondsRemaining / 60) + " m";
        else
            return Mathf.CeilToInt(secondsRemaining) + " s";
    }
}
