using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : PoolObject
{
    // 이동 관련 변수들 ----------------------------------------------------------------------------
    public float moveSpeed = 2.0f;

    GridMap map;

    List<Vector2Int> path;

    PathLine pathLine;
    public PathLine PathLine => pathLine;

    Vector2Int Position => map.WorldToGrid(transform.position);

    // 셰이더용 변수들 -----------------------------------------------------------------------------
    /// <summary>
    /// 페이즈 전체 진행 시간
    /// </summary>
    public float phaseDuration = 0.5f;

    /// <summary>
    /// 디졸브 전체 진행 시간
    /// </summary>
    public float dissolveDuration = 1.0f;

    /// <summary>
    /// 외각선 두께용 상수
    /// </summary>
    const float Outline_Thickness = 0.005f;

    /// <summary>
    /// 페이즈가 끝날 때 실행되는 델리게이트
    /// </summary>
    Action onPhaseEnd;

    /// <summary>
    /// 디졸브가 끝날 때 실행되는 델리게이트(기본적으로 Die 함수 연결되어있음)
    /// </summary>
    Action onDissolveEnd;

    /// <summary>
    /// 이 게임 오브젝트의 머티리얼
    /// </summary>
    Material mainMaterial;

    // 컴포넌트들
    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        pathLine = GetComponentInChildren<PathLine>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        mainMaterial = spriteRenderer.material;

        onDissolveEnd += Die;   // 디졸브가 끝나면 죽게 만들기
    }

    private void OnEnable()
    {
        ResetShaderProperties();            // 스폰 될 때 셰이더 프로퍼티 초기화
        StartCoroutine(StartPhase());       // 페이즈 시작
    }

    private void Update()
    {
        MoveUpdate();
    }

    /// <summary>
    /// 셰이더 프로퍼티 초기화 하는 함수
    /// </summary>
    private void ResetShaderProperties()
    {
        mainMaterial.SetFloat("_OutlineThickness", 0.0f);   // 아웃라인 안보이게 하기

        mainMaterial.SetFloat("_PhaseSplit", 1.0f);         // 페이즈 선 위치 초기화
        mainMaterial.SetFloat("_PhaseThickness", 0.1f);     // 페이즈 선 두께 설정해서 선 보이게 만들기

        mainMaterial.SetFloat("_DissolveFade", 1.0f);       // 디졸브 fade 상태 초기화
    }

    /// <summary>
    /// 스폰될 때 실행될 코루틴. 페이즈 시작함.
    /// </summary>
    /// <returns></returns>
    IEnumerator StartPhase()
    {
        float timeElipsed = 0.0f;                       // 진행 시간 초기화
        float phaseNormalize = 1.0f / phaseDuration;    // split 값을 0~1사이로 정규화하기 위해 미리 계산(나누기 횟수 줄이기 위한 용도)

        while (timeElipsed < phaseDuration)             // 진행시간이 목표시간에 도달할 때까지 진행
        {
            timeElipsed += Time.deltaTime;              // 진행시간 계속 증가

            mainMaterial.SetFloat("_PhaseSplit", 1 - (timeElipsed * phaseNormalize));   // split 값 감소

            yield return null;                          // 다음 프레임까지 대기
        }

        mainMaterial.SetFloat("_PhaseThickness", 0.0f); // 페이즈 선 안보이게 하기
        mainMaterial.SetFloat("_PhaseSplit", 0.0f);     // 다 진행된 상태로 설정

        onPhaseEnd?.Invoke();                           // 페이즈가 끝났음을 알림
    }

    /// <summary>
    /// 플레이어에게 공격 당했을 때 실행될 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator StartDissolve()
    {
        float timeElipsed = 0.0f;                       // 진행 시간 초기화
        float dissolveNormalize = 1.0f / dissolveDuration;  // fade 값을 0~1사이로 정규화하기 위해 미리 계산(나누기 횟수 줄이기 위한 용도)

        while (timeElipsed < dissolveDuration)          // 진행시간이 목표시간에 도달할 때까지 진행
        {
            timeElipsed += Time.deltaTime;              // 진행시간 계속 증가

            mainMaterial.SetFloat("_DissolveFade", 1 - (timeElipsed * dissolveNormalize));   // fade 값 감소

            yield return null;                          // 다음 프레임까지 대기
        }

        mainMaterial.SetFloat("_DissolveFade", 0.0f);   // 다 진행된 상태로 설정

        onDissolveEnd?.Invoke();                        // 디졸브가 끝났음을 알림
    }

    /// <summary>
    /// 아웃라인 표시용 함수
    /// </summary>
    /// <param name="isShow">true면 아웃라인을 표시, false면 아웃라인 끄기</param>
    public void ShowOutline(bool isShow = true)
    {
        if (isShow)
        {
            mainMaterial.SetFloat("_OutlineThickness", Outline_Thickness);
        }
        else
        {
            mainMaterial.SetFloat("_OutlineThickness", 0.0f);
        }
    }

    /// <summary>
    /// 플레이어에게 공격 당하면 실행되는 함수
    /// </summary>
    public void OnAttacked()
    {
        StartCoroutine(StartDissolve());
    }

    /// <summary>
    /// 사망 처리용 함수. Dissolve가 끝날 때 실행됨.
    /// </summary>
    void Die()
    {
        gameObject.SetActive(false);
    }


    public void Initialize(GridMap gridMap, Vector3 pos)
    {
        map = gridMap;
        transform.position = map.GridToWorld(map.WorldToGrid(pos));
    }

    public void SetDestination(Vector2Int goal)
    {
        path = AStar.PathFind(map, Position, goal);
        pathLine.DrawPath(map, path);
    }

    private void MoveUpdate()
    {
        if (path != null && path.Count > 0)
        {
            Vector2Int destGrid = path[0];

            Vector3 dest = map.GridToWorld(destGrid);
            Vector3 dir = dest - transform.position;

            if (dir.sqrMagnitude < 0.001f)
            {
                transform.position = dest;
                path.RemoveAt(0);
            }
            else
            {
                transform.Translate(Time.deltaTime * moveSpeed * dir.normalized);
            }
        }
    }
}
