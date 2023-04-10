using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class Test_AStar_Tilemap : Test_Base
{
    public Tilemap background;  // 배경용 타일맵(가장 커야 한다.)
    public Tilemap obstacle;    // 장애물 표시용 타일맵

    public PathLine pathLine;   // 경로를 그려줄 라인

    public Transform start;     // 시작 위치
    public Transform end;       // 도착 위치

    GridMap map;                // 타일맵으로 만들어진 그리드맵

    private void Start()
    {
        map = new GridMap(background, obstacle);    // 타일맵으로 그리드맵 생성
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        inputActions.Test.Click.performed += OnLClick;      // 클릭입력이랑 함수 연결
        inputActions.Test.RClick.performed += OnRClick;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        inputActions.Test.RClick.performed -= OnRClick;     // 연결한 함수 해제
        inputActions.Test.Click.performed -= OnLClick;
    }

    private void OnLClick(InputAction.CallbackContext _)
    {
        // 마우스 좌클릭으로 시작위치 설정
        Vector2 screenPos = Mouse.current.position.ReadValue();         // 스크린 좌표 가져오기
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);   // 스크린 좌표를 월드좌표로 변경
        Vector2Int gridPos = map.WorldToGrid(worldPos);                 // 월드 좌표를 그리드 좌표로 변경
        if (!map.IsWall(gridPos) && !map.IsMonster(gridPos))            // 그 위치가 벽이나 몬스터가 아니면
        {
            Vector2 finalPos = map.GridToWorld(gridPos);                // 그리드 좌표를 다시 월드로 변경(각 칸의 한 가운대로 설정)
            start.position = finalPos;                                  // 위치 실제로 변경
        }
    }

    private void OnRClick(InputAction.CallbackContext _)
    {
        // 마우스 우클릭으로 도착위치 설정
        Vector2 screenPos = Mouse.current.position.ReadValue();         // 좌클릭과 똑같음
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        Vector2Int gridPos = map.WorldToGrid(worldPos);
        if (!map.IsWall(gridPos) && !map.IsMonster(gridPos))
        {
            Vector2 finalPos = map.GridToWorld(gridPos);
            end.position = finalPos;
        }
    }

    protected override void Test1(InputAction.CallbackContext _)
    {
        Vector2Int startGrid = map.WorldToGrid(start.position);
        Debug.Log($"Start : {startGrid}");
        Vector2Int endGrid = map.WorldToGrid(end.position);
        Debug.Log($"End : {endGrid}");

        List<Vector2Int> path = AStar.PathFind(map, startGrid, endGrid);    // 경로 구하고
        pathLine.DrawPath(map, path);                                       // 경로 그리기
    }

    private void Test_GetTileMapInfos()
    {
        // 타일맵의 크기
        Debug.Log($"Background : {background.size}");
        Debug.Log($"Obstacle : {obstacle.size}");

        // 타일맵의 원점(그리드좌표)
        Debug.Log($"Background origin : {background.origin}");
        Debug.Log($"Obstacle origin : {obstacle.origin}");

        // 타일맵 전체 순회하기
        for (int y = background.cellBounds.yMin; y < background.cellBounds.yMax; y++)
        {
            for (int x = background.cellBounds.xMin; x < background.cellBounds.xMax; x++)
            {
                TileBase tile = obstacle.GetTile(new(x, y));    // 특정 위치의 타일 가져오기
                if (tile != null)
                {
                    Debug.Log($"Obstacle Pos : ({x},{y})");
                }
            }
        }
    }
}
