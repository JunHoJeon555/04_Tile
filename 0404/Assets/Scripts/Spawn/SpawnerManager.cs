using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpawnerManager : MonoBehaviour
{
    Player player;
    public Player Player => player;

    GridMap gridMap;
    Tilemap background;
    Tilemap obstacle;

    Spawner[] spawners;

    //public GridMap GridMap => gridMap;

    private void Awake()
    {
        Transform grid = transform.parent;
        Transform tilemap = grid.GetChild(0);
        background = tilemap.GetComponent<Tilemap>();
        tilemap = grid.GetChild(1);
        obstacle = tilemap.GetComponent<Tilemap>();

        gridMap = new GridMap(background, obstacle);

        spawners = GetComponentsInChildren<Spawner>();
    }

    private void Start()
    {
        player = GameManager.Inst.Player;
    }

    /// <summary>
    /// 스폰가능한 영역을 미리 찾아 놓는 함수
    /// </summary>
    /// <param name="spawner">스폰 가능한 영역을 찾을 스포너</param>
    /// <returns>스포너의 영역 중에 스폰 가능한 지역의 리스트</returns>
    public List<Node> CalcSpawnArea(Spawner spawner)
    {
        List<Node> nodes = new List<Node>();
        Vector2Int min = gridMap.WorldToGrid(spawner.transform.position);  //p0
        Vector2Int max = gridMap.WorldToGrid(spawner.transform.position + (Vector3)spawner.size);//p2+1

        for(int y = min.y; y<max.y; y++)
        {
            for(int x = min.x; x<max.x; x++)
            {
                if (gridMap.IsSpawnable(x, y))              //스폰 가능한 위치면
                {
                    nodes.Add (gridMap.GetNode(x, y));      //리스트에 추가
                }
            }
        }

        return nodes;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Vector3 GridToWorld(int x, int y)
    {
        Vector2Int gridPos = new Vector2Int(x, y);
        return gridMap.GridToWorld(gridPos);
    }

    /// <summary>
    /// 슬라임 초기화
    /// </summary>
    /// <param name="slime">초기화할 슬라임</param>
    /// <param name="pos">슬라임 </param>
    public void SlimeInitialize(Slime slime, Vector3 pos)
    {
        slime.Initialize(gridMap, pos);
    }



    /// <summary>
    /// 스크린좌표에 있는 노드를 리턴하는 함수
    /// </summary>
    /// <param name="screenPos">마우스의 스크린 좌표</param>
    /// <returns>해당 위치에 있는 노드</returns>
    public Node GetNode(Vector2 screenPos)
    {
        Camera.main.ScreenToWorldPoint(screenPos);
        return gridMap.GetNode(screenPos);
    }
}
