using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : MonoBehaviour
{

    [SerializeField]
    private float life;

    // Update is called once per frame
    void Update()
    {
        life -= Time.deltaTime;
        if (life <= 0)
            Destroy(gameObject);

    }
}
