using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightFlicker : MonoBehaviour
{
    private new Light light;
    private float intensity;
    private float delta;
    private float timer;
    public float intensityMinimum, intensityMaximum;
    [Range(0, 10)]
    public float speed = 1;
    void Awake()
    {
        light = GetComponent<Light>();
        intensity = light.intensity;
    }
    void Update()
    {
        speed = Mathf.Max(0, speed);
        timer += Time.deltaTime * speed;
        while (timer >= 1)
        {
            timer--;

            light.intensity = intensity;
            intensity = Random.Range(intensityMinimum, intensityMaximum);
            delta = intensity - light.intensity;
        }

        light.intensity += delta * Time.deltaTime * speed;
    }
}
