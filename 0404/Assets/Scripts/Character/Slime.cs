using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : PoolObject
{
    SpriteRenderer spriteRenderer;

    Material material;

    public float phaseDuration = 0.5f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        material = GetComponent<Renderer>().material;
    }
}
