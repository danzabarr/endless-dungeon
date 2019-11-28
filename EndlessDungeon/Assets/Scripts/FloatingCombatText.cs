using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform), typeof(Text))]
public class FloatingCombatText : MonoBehaviour
{
    public static readonly Color Green = new Color();
    public static readonly Color Red = new Color(.7f, 0.1f, .1f);
    public static readonly Color Gold = new Color();

    private RectTransform rectTransform;
    private Text text;

    public Vector3 worldOffset;
    public Vector2 screenOffset;
    public float life;
    public float gravity;
    public Vector3 velocityMin, velocityMax;
    public float dampening;
    
    private Vector3 position;
    private Vector3 velocity;
    public float sizeUp, sizeDown, sizeDampening;
    public float fade;

    public string Text { get => text.text; set => text.text = value; }
    public Color Color { get => text.color; set => text.color = value; }
    public float Size { get; set; }
    public Vector3 Position { get => position; set => position = value; }
    public void Awake()
    {
        rectTransform = transform as RectTransform;
        text = GetComponent<Text>();
        velocity = new Vector3
        (
            Random.Range(Mathf.Min(velocityMin.x, velocityMax.x), Mathf.Max(velocityMin.x, velocityMax.x)),
            Random.Range(Mathf.Min(velocityMin.y, velocityMax.y), Mathf.Max(velocityMin.y, velocityMax.y)),
            Random.Range(Mathf.Min(velocityMin.z, velocityMax.z), Mathf.Max(velocityMin.z, velocityMax.z))
        );
    }
    public void FixedUpdate()
    {
        if (life <= 0 || Size <= 0)
        {
            Destroy(gameObject);
            return;
        }

        position += velocity;
        position.y -= gravity;
        velocity *= dampening;

        float size = Size;
        size += sizeUp;
        sizeUp *= sizeDampening;
        size -= sizeDown;
        Size = Mathf.Max(1, size);

        text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.Clamp(text.color.a - fade, 0, 1));
        text.fontSize = Mathf.FloorToInt(Size);
        life -= Time.deltaTime;
    }
    public void LateUpdate()
    {
        Vector2 screen = FloatingCombatTextManager.Camera.WorldToScreenPoint(worldOffset + position);


        screen.x -= Screen.width / 2;
        screen.y -= Screen.height / 2;

        screen /= FloatingCombatTextManager.Canvas.scaleFactor;

        screen += screenOffset;
        rectTransform.anchoredPosition = screen;
    }
}
