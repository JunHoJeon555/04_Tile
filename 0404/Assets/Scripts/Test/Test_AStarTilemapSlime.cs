using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class Test_AStarTilemapSlime : Test_Base
{
    public Tilemap background;
    public Tilemap obstacle;
    public Slime slime;

    GridMap map;

    private void Start()
    {
        map = new GridMap(background, obstacle);

        if (slime == null)
            slime = FindObjectOfType<Slime>();
        slime.Initialize(map, Vector3.zero);
        slime.PathLine.transform.SetParent(null);
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

    private void OnClick(InputAction.CallbackContext _)
    {
        // 마우스 좌클릭으로 시작위치 설정
        Vector2 screenPos = Mouse.current.position.ReadValue();         // 스크린 좌표 가져오기
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);   // 스크린 좌표를 월드좌표로 변경
        Vector2Int gridPos = map.WorldToGrid(worldPos);                 // 월드 좌표를 그리드 좌표로 변경
        if (!map.IsWall(gridPos) && !map.IsMonster(gridPos))            // 그 위치가 벽이나 몬스터가 아니면
        {
            slime.SetDestination(gridPos);  // 슬라임 목적지 설정
        }
    }
}
