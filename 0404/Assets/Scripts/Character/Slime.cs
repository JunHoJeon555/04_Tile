using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : PoolObject
{
    Material material;


    private void Awake()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        GetComponent<Renderer>().material;
    }
}
