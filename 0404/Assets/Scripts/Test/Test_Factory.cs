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
        slimes.Add(slime);
        Vector3 pos = new(Random.Range(-maxX, maxX), Random.Range(-maxY, maxY));
        slime.transform.position= pos;
    }


    protected override void Test2(InputAction.CallbackContext _)
    {
        while(slimes.Count > 0)
        {
            Slime slime = slimes[0];
            slimes.RemoveAt(0);
            slime.gameObject.SetActive(false);

        }
    }
}
