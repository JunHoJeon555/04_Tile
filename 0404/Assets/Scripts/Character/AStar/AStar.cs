using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AStar
{
    /// <summary>
    /// A* 알고리즘으로 길을 탐색하는 함수
    /// </summary>
    /// <param name="gridMap">길찾기를 진행할 맵</param>
    /// <param name="start">시작 그리드 좌표</param>
    /// <param name="end">도착 그리드 좌표</param>
    /// <returns>시작->도착으로 가는 경로.(길찾기에 실패했을 경우 null)</returns>
    public static List<Vector2Int> PathFind(GridMap gridMap, Vector2Int start, Vector2Int end)
    {
        
        const float sideDistance = 1.0f;        // 옆 칸으로 가는데 걸리는 거리
        const float diagonalDistance = 1.4f;    // 대각선 옆 칸으로 가는데 걸리는 거리

        gridMap.ClearData();            // 그리드 맵에 들어있는 노드들 초기화(반복 사용때문에 처리)
        List<Vector2Int> path = null;   // 최종 경로가 들어갈 리스트

        // 시작지점과 도착지점이 맵안에 있을 때만 진행
        if (gridMap.IsValidPosition(start) && gridMap.IsValidPosition(end))
        {
            List<Node> open = new List<Node>();     // open리스트(앞으로 탐색할 노드의 리스트)
            List<Node> close = new List<Node>();    // close리스트(탐색이 완료된 노드의 리스트)

            // A* 시작하기
            Node current = gridMap.GetNode(start);  // 시작 노드를 가져와서
            current.G = 0;                          // G,H 설정
            current.H = GetHeuristic(current, end);
            open.Add(current);                      // open리스트에 추가

            // A* 핵심 루틴 시작
            while (open.Count > 0)     // open리스트가 빌때까지 반복
            {
                open.Sort();            // open리스트에 있는 노드들을 f값기준으로 정렬
                current = open[0];      // f값이 가장 큰 노드를 current로 지정
                open.RemoveAt(0);       // open리스트에서 current로 지정된 노드 제거

                if (current != end)      // current가 도착지점인지 확인
                {
                    // 도착지점이 아니면
                    close.Add(current);         // close리스트에 current 추가

                    // current 주변 8방향에 있는 노드들을 open리스트에 넣거나 G값 갱신하기
                    for (int y = -1; y < 2; y++)
                    {
                        for (int x = -1; x < 2; x++)
                        {
                            Node node = gridMap.GetNode(current.x + x, current.y + y);  // current 주변에 있는 노드 중 1개

                            // 스킵할 노드는 스킵
                            if (node == null) continue;         // 주변 위치가 맵 밖일 경우
                            if (node == current) continue;      // current인 경우
                            if (node.gridType == Node.GridType.Wall) continue;  // 벽인 경우
                            if (close.Exists((x) => x == node)) continue;       // close리스트에 있을 경우
                            bool isDiagonal = Mathf.Abs(x) == Mathf.Abs(y); // 대각선이면 true
                            if (isDiagonal &&                   // 대각선이고 대각선 양옆의 노드 중 하나가 벽일 경우
                                (gridMap.GetNode(current.x + x, current.y).gridType == Node.GridType.Wall
                                 || gridMap.GetNode(current.x, current.y + y).gridType == Node.GridType.Wall))
                                continue;

                            float distance;
                            if (isDiagonal)
                                distance = diagonalDistance;    // 대각선상에 있는 노드면 1.4로 거리 설정
                            else
                                distance = sideDistance;        // 옆에 있는 노드면 1로 거리 설정

                            // node의 G값이 current를 거쳐서 node로 가는 G값보다 크면 갱신
                            if (node.G > current.G + distance)
                            {
                                if (node.parent == null)        // open리스트에 없었을 경우
                                {
                                    node.H = GetHeuristic(node, end);   // 휴리스틱 값 설정
                                    open.Add(node);                     // open리스트에 추가
                                }
                                node.G = current.G + distance;  // G값 갱신
                                node.parent = current;          // current를 거쳐서 왔다고 설정
                            }
                        }
                    }
                }
                else
                {
                    break;  // 도착지점이면 루틴 종료
                }
            }

            // 마무리 작업(path 만들기)
            if (current == end)
            {
                // 도착지점에 도달한 경우만 path 구성
                path = new List<Vector2Int>();      // 리스트 생성
                Node result = current;
                while (result != null)               // current에서 계속 타고 올라가기
                {
                    path.Add(new Vector2Int(result.x, result.y));   // 리스트에 노드 위치 추가
                    result = result.parent;
                }
                path.Reverse();                     // 도착 -> 시작으로 되어있는 리스트를 뒤집기
            }
        }

        return path;    // 경로 리턴
    }

    /// <summary>
    /// 휴리스틱 값 계산
    /// </summary>
    /// <param name="current">현재 노드</param>
    /// <param name="end">도착지점의 그리드 좌표</param>
    /// <returns>current에서 end까지의 예상 거리</returns>
    static float GetHeuristic(Node current, Vector2Int end)
    {
        
        return Mathf.Abs(current.x - end.x) + Mathf.Abs(current.y - end.y);
    }
}