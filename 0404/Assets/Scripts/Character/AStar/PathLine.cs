using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 경로를 시각적으로 보여준 주는 역할
/// </summary>
public class PathLine : MonoBehaviour
{
    /// <summary>
    /// 경로를 그리는 라인 랜더러
    /// </summary>
    LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    /// <summary>
    /// 경로를 실제로 그리는 함수
    /// </summary>
    /// <param name="map">월드 좌표를 구하는데 필요한 맵</param>
    /// <param name="path">맵의 그리드 단위로 구해진 경로</param>
    public void DrawPath(GridMap map, List<Vector2Int> path)
    {
        // 맵이 있고 경로가 있고 PathLine이 활성화 되어있을 때만 그리기
        if (map != null && path != null && gameObject.activeSelf)
        {
            lineRenderer.positionCount = path.Count;    // 경로 갯수에 맞게 라인랜더러의 위치 갯수 설정

            int index = 0;
            foreach (var node in path)   // 모든 경로를 순회하면서
            {
                Vector2 worldPos = map.GridToWorld(node);   // 노드의 월드 좌표 구하고
                Vector3 localPos = (Vector3)worldPos - lineRenderer.transform.position; // 로컬 좌표로 변환
                lineRenderer.SetPosition(index, localPos);  // 로컬좌표를 라인랜더러에 설정
                index++;                                    // 인덱스 증가
            }
        }
        else
        {
            lineRenderer.positionCount = 0;     // 카운트를 0으로 해서 안보이게 만들기
        }
    }

    /// <summary>
    /// 그리는 경로를 초기화 하는 함수. 비활성화도 실행
    /// </summary>
    public void ClearPath()
    {
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 0;
        }
        gameObject.SetActive(false);
    }
}
