using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_Seemless : Test_Base
{
    public int x;
    public int y;

    protected override void Test1(InputAction.CallbackContext _)
    {
        MapManager mapManager = GameManager.Inst.MapManager;
        mapManager.Test_LoadScene(x, y);
    }

    protected override void Test2(InputAction.CallbackContext _)
    {
        MapManager mapManager = GameManager.Inst.MapManager;
        mapManager.Test_UnloadScene(x, y);
    }
}
