using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AStarTilemap_BlockAbdWait : Test_Base
{
    SpawnerManager spawnerManager;

    private void Start()
    {
        spawnerManager = FindObjectOfType<SpawnerManager>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        inputActions.Test.Click.performed += OnClick;
    }

    protected override void OnDisable()
    {
        inputActions.Test.Click.performed -= OnClick;
        base.OnDisable();
    }

    private void OnClick(UnityEngine.InputSystem.InputAction.CallbackContext _)
    {
        Vector2 screenPos = Mouse.current.position.ReadValue();
        Node node = spawnerManager.GetNode(screenPos);
        Debug.Log($"Node type : {node.gridType}");
    }
}
