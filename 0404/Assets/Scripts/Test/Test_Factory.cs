using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Test_Factory : Test_Base
{
    public float maxX;
    public float maxY;

    List<Slime> slimes = new List<Slime>();

    protected override void Test1(InputAction.CallbackContext _)
    {
        Slime slime = Factory.Inst.GetSlime(transform);
        slime.Add(slime);
        Vetor3 pos = new(Random.Range(-maxX, maxX), Random.Range(-maxY, maxY));
    }


    protected override void Test2(InputAction.CallbackContext _)
    {
        while(slime.Count > 0)
        {
            Slime slime = slimes[0];
            slime.RemoveAt(0);
            slime.gameObject.SetActive(false);

        }
    }
}
