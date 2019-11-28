using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[ExecuteAlways]
public class AbilityButton : MonoBehaviour
{
    public float transitionDuration;
    private Color onCooldownTint;
    private Color onCooldownOverlay;

    public Image background, icon, pressed;
    public Text bindingIndicator;
    public TextMeshProUGUI cooldownIndicator;

    private bool onCooldown;

    public void Awake()
    {
        onCooldownTint = icon.material.GetColor("_Tint");
        onCooldownOverlay = icon.material.GetColor("_Overlay");
        icon.material = new Material(icon.material);
        icon.material.SetFloat("_Fraction", 1);
        icon.material.SetColor("_Tint", Color.white);
        icon.material.SetColor("_Overlay", Color.white);
    }

    public void OnDestroy()
    {
        Destroy(icon.material);
    }

    public void SetBinding(KeyCode key)
    {
        string str = key.ToString();

        if (key == KeyCode.Mouse0)
            str = "LM";
        if (key == KeyCode.Mouse1)
            str = "RM";

        bindingIndicator.text = str;
    }

    private Coroutine transition;

    public void SetCooldown(float seconds, float maxSeconds)
    {
        //cooldownOverlay.gameObject.SetActive(maxSeconds > 0);
        bool wasOnCooldown = this.onCooldown;
        bool onCooldown = seconds > 0 && maxSeconds > 0;
        this.onCooldown = onCooldown;

        if (!onCooldown)
            cooldownIndicator.text = "";
        else if (seconds < 1)
            cooldownIndicator.text = (Mathf.CeilToInt(seconds * 10) / 10f) + "";
        else if (seconds < 60)
            cooldownIndicator.text = Mathf.CeilToInt(seconds).ToString("0");
        else
            cooldownIndicator.text = string.Format("{0:00}:{1:00}", Mathf.CeilToInt(seconds) / 60, Mathf.CeilToInt(seconds) % 60);

        if (onCooldown)
        {
            icon.material.SetFloat("_Fraction", Mathf.Clamp(1.0f - seconds / maxSeconds, 0, 1));
            //icon.material.SetColor("_Tint", new Color(1, 1, 1, 0.8f));
            if (!wasOnCooldown)
            {
                if (transition != null) StopCoroutine(transition);
                transition = StartCoroutine(TintTo(onCooldownTint, onCooldownOverlay, transitionDuration));
            }
        }
        else 
        {
            icon.material.SetFloat("_Fraction", 1);
            //icon.material.SetColor("_Tint", Color.white);
            if (wasOnCooldown)
            {
                if (transition != null) StopCoroutine(transition);
                transition = StartCoroutine(TintTo(Color.white, Color.white, transitionDuration));
            }
        }
    }

    public void SetPressed(bool pressed)
    {
        this.pressed.gameObject.SetActive(pressed);
    }

    IEnumerator TintTo(Color tint, Color overlay, float time)
    {
        Color oldTint = icon.material.GetColor("_Tint");
        Color oldOverlay = icon.material.GetColor("_Overlay");
        for (float i = 0; i <= time; i += Time.deltaTime)
        {
            icon.material.SetColor("_Tint", Color.Lerp(oldTint, tint, i / time));
            icon.material.SetColor("_Overlay", Color.Lerp(oldOverlay, overlay, i / time));
            yield return null;
        }
        icon.material.SetColor("_Tint", tint);
        icon.material.SetColor("_Overlay", overlay);
    }

    public void SetAbility(Ability ability)
    {
        if (ability == null)
        {
            //icon.gameObject.SetActive(false);
            cooldownIndicator.gameObject.SetActive(false);
            icon.gameObject.SetActive(false);
        }
        else
        {
            //icon.gameObject.SetActive(true);
            cooldownIndicator.gameObject.SetActive(true);
            icon.gameObject.SetActive(true);
            icon.material.SetTexture("_MainTex", ability.Icon);
            SetCooldown(ability.Cooldown, ability.Cooldown);
            //icon.texture = ability.Icon;
        }
    }
}
