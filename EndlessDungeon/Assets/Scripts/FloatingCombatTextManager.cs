using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingCombatTextManager : MonoBehaviour
{
    private static FloatingCombatTextManager instance;
    [SerializeField]
    private FloatingCombatText prefab;

    private static Camera main;
    private static Canvas canvas;
    public static Camera Camera => main;
    public static Canvas Canvas => canvas;
    public void Awake()
    {
        instance = this;
        main = Camera.main;
        canvas = GetComponentInParent<Canvas>();
    }

    public static void Add(Vector3 position, float damage, float size, Color color)
    {
        Add(position, FormatDamage(damage), size, color);
    }

    public static string FormatDamage(float damage)
    {
        if (damage > 999999999 || damage < -999999999)
        {
            return damage.ToString("0,,,.###B");
        }
        else
        if (damage > 999999 || damage < -999999)
        {
            return damage.ToString("0,,.##M");
        }
        else
        if (damage > 999 || damage < -999)
        {
            return damage.ToString("0,.#K");
        }
        else
        {
            return Mathf.CeilToInt(damage).ToString();
        }
    }

    public static void Add(Vector3 position, string text, float size, Color color)
    {
        FloatingCombatText fct = Instantiate(instance.prefab, instance.transform);
        fct.Position = position;
        fct.Text = text;
        fct.Size = size;
        fct.Color = color;
    }
}
