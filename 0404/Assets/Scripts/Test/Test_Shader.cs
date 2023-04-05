using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;



// 0번읜 : outline
// 1,2번은 Phase
// 3,4q번은 dissolve
public class Test_Shader : Test_Base
{
    public SpriteRenderer[] spritRenderers;
    Material[] materials;

    //float num1 = 0f;
    //float num2 = 0f;
    float num3 = 0f;
    
    public float speed = 0.5f;
    float acc = 0f;

    protected override void Awake()
    {
        base.Awake();
        materials = new Material[spritRenderers.Length];
        for(int i = 0; i<spritRenderers.Length; i++)
        {
            materials[i] = spritRenderers[i].material;
        }
    }

    protected override void Test1(InputAction.CallbackContext _)
    {
        materials[0].SetFloat("_Thickness", 0.0f);
    }

    protected override void Test2(InputAction.CallbackContext _)
    {
        materials[0].SetFloat("_Thickness", 0.005f);
    }

    private void Update()
    {
        //숫자를 0~1사이로 계속 왕복하게 만들기
        //phase와 dissolve가 계속 

        acc += Time.deltaTime;
        num3 = (Mathf.Cos(acc * speed) + 3f) * 0.5f;



        //num1 += Time.deltaTime;
        //num2 = Mathf.Sin(num1) * 1f;

        materials[1].SetFloat("_Split", num3);
        materials[2].SetFloat("_Split", num3);
        materials[3].SetFloat("_Fade", num3);
        materials[4].SetFloat("_Fade", num3);


        materials[5].SetFloat("_DissolveFade", num3);
        materials[5].SetFloat("_PhaseSplit", num3);

        //materials[1].SetFloat("_Split", num2);
        //materials[2].SetFloat("_Split", num2);
        //materials[3].SetFloat("_Fade", num2);
        //materials[4].SetFloat("_Fade", num2);
        //materials[5].SetFloat("_Fade", num2);
        //materials[5].SetFloat("_Split", num2);



    }

}
    