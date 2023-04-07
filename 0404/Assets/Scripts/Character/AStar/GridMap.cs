using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


//그리드 좌표 (0,0) = 월드 좌표(0.5f, 0.5f);
//그리드 좌표의 한칸의 간격은 1이다.

public class GridMap
{
    /// <summary>
    /// 이 맵에 있는 모든 노드
    /// </summary>
    Node[] nodes;

    /// <summary>
    /// 맵의 가로 길이
    /// </summary>
    int width;

    /// <summary>
    ///맵의 세로 길이
    /// </summary>
    int height;

    /// <summary>
    /// 위치 입력이 잘못되었다는 것을 표시하기 위한 상수
    /// </summary>
    public const int Error_Not_Valid_Position = -1;

    public GridMap(int width, int height)
    {
        //World의(0,0,0)에 만든다고 가정

        this.width = width;
        this.height = height;

        //C#의 다차원배열은 함수호출 형식으로 처리가 되기 때문에 속도가 느리다.
        //Node[,] test = new Node[height,width];
        //test[2,1]
        nodes = new Node[height*width];
        
        for(int y = 0; y <height; y++)
        {
            for(int x= 0; x <width; x++)
            {
                int index = GridToIndex(x, y);
                nodes[index] = new Node(x, y);

            }
        }
    
    }

    /// <summary>
    /// 특정 위치에 있는 노드를 돌려주는 함수
    /// </summary>
    /// <param name="x">X좌표</param>
    /// <param name="y">Y좌표</param>
    /// <returns>x,y에 있는 노드. 좌표가 잘못되었을 경우 null </returns>
    public Node GetNode(int x, int y)
    {
        int index = GridToIndex(x, y);
        Node result = null;
        if(index != Error_Not_Valid_Position)
        {
            result = nodes[index];

        }
        return result;
    }

    /// <summary>
    /// 특정위치에 있는 노드를 돌려주는 함수
    /// </summary>
    /// <param name="gridPos">그리드 좌표</param>
    /// <returns>x,y에 있는 노드. 좌표가 잘못되었을 경우 null </returns>
    public Node GetNode(Vector2Int gridPos)
    {
        return GetNode(gridPos.x, gridPos.y);
    }

    /// <summary>
    /// 특정위치에 있는 노드를 돌려주는 함수
    /// </summary>
    /// <param name="worldPos">월드 좌표</param>
    /// <returns> 월드 좌표에 있는 노드. 좌표가 잘못되었을 경우 null</returns>
    public Node GetNode(Vector3 worldPos)
    {
        return GetNode(WorldToGrid(worldPos));
    }

    /// <summary>
    /// 모든 노드의 A* 계산용 데이터를 초기화하는 함수
    /// </summary>
    public void ClearData()
    {
        foreach(var node in nodes)
        {
            node.ClearData();
        }
    }


    //유틸리티 함수들--------------------------------------------------------------------------------


    /// <summary>
    /// 월드 좌표를 그리드 좌표로 변경해주는 함수
    /// </summary>
    /// <param name="Pos"월드 좌표</param>
    /// <returns> </returns>
    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        return new Vector2Int((int)worldPos.x, (int)worldPos.y);
    }

    /// <summary>
    /// 그리드좌표를 월드좌표로 변경해주는 함수
    /// </summary>
    /// <param name="girdPos">그리드 좌표</param>
    /// <returns></returns>
    public Vector2 GridTOWorld(Vector2Int gridPos)
    {
        return new Vector2(gridPos.x + 0.5f, gridPos.y + 0.5f);
    }

    /// <summary>
    /// 그리드 좌표를 인덱스로 변경해주는 함수
    /// </summary>
    /// <param name="x">X좌표</param>
    /// <param name="y">Y좌표</param>
    /// <returns>변환된 인덱스값</returns>
    public int GridToIndex(int x, int y)
    {
        // 왼쪽 위가 원점일 때
        // index = x+y *width; 
        //x = 0  y=4 * 

        //왼쪽 아래 원점일 떄
        //x + (height - 1 -y )* width; 

        int index = Error_Not_Valid_Position;
        if (IsValidPosition(x, y))
        {
            index = x + (height - 1 - y) * width;
        }
        
        return index;

    }



    /// <summary>
    /// 입력받은 위치가 맵 내부인지 아닌지 확인하는 함수
    /// </summary>
    /// <param name="x"> x좌표</param>
    /// <param name="y"> y좌표</param>
    /// <returns> 맵 내부면 true, 맵 밖이면 false </returns>
    public bool IsValidPosition(int x, int y)
    {
        return x  >=0 && x < width && y >=0 && y < height;
    }

    /// <summary>
    /// 입력받은 위치가 맵 내부인지 아닌지 확인하는 함수
    /// </summary>
    /// <param name="gridPos"> 확인할 좌표</param>
    /// <returns> 맵 내부면 true 맵 외부면 false</returns>
    public bool IsValidPosition(Vector2Int gridPos)
    {
        return IsValidPosition(gridPos.x, gridPos.y); 
    }

    /// <summary>
    /// 입력받은 위치가 벽인지 아닌지 확인하는 함수 
    /// </summary>
    /// <param name="x"> x 좌표</param>
    /// <param name="y">y좌표</param>
    /// <returns>벽이면true 아니면 false</returns>
    public bool IsWall(int x, int y)
    {
        Node node = GetNode(x, y);
        return node != null && node.gridType == Node.GridType.Wall;
    }

    /// <summary>
    /// 입력받은 위치가 벽인지 아닌지 확인하는 함수
    /// </summary>
    /// <param name="gridPos">그리드 좌표</param>
    /// <returns>벽이면true 아니면 false</returns>
    public bool IsWall(Vector2Int gridPos)
    {

        return IsWall(gridPos.x, gridPos.y);
    }

    /// <summary>
    ///  입력받은 위치가 몬스터인지 아닌지 확인하는 함수
    /// </summary>
    /// <param name="x">x좌표</param>
    /// <param name="y">y좌표</param>
    /// <returns></returns>
    public bool IsMonster(int x, int y)
    {
        Node node = GetNode(x, y);
        return node != null && node.gridType == Node.GridType.Monster;

    }

    /// <summary>
    /// 입력받은 위치가 몬스터인지 아닌지 확인하는 함수
    /// </summary>
    /// <param name="gridPos">그리드좌표</param>
    /// <returns>몬스터이면 true, 아니면 false</returns>
    public bool IsMonster(Vector2Int gridPos)
    {
        return IsMonster(gridPos.x, gridPos.y);
    }


}
