using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : PoolObject
{
    //일반 변수들---------------------------------------------------------------------------------

    /// <summary>
    /// 슬라임이 활동 중인지 아닌지 표시하는 변수
    /// </summary>
    bool isActivate = false;
    
    /// <summary>
    /// 위치 확안용 프로퍼티 (그리드 좌표)
    /// </summary>
    Vector2Int Position => map.WorldToGrid(transform.position);

    public Action onDie;

    // 이동 관련 변수들 ----------------------------------------------------------------------------
    
    /// <summary>
    /// 이동속도
    /// </summary>
    public float moveSpeed = 2.0f;

    /// <summary>
    /// 이 슬라임이 있는 그리드 맵
    /// </summary>
    GridMap map;


    /// <summary>
    /// 슬라임이 이동할 경로
    /// </summary>
    List<Vector2Int> path;

    /// <summary>
    /// 슬라임이 이동할 경로를 그리는 클래스
    /// </summary>
    PathLine pathLine;

    /// <summary>
    /// 경로 그리는 클레
    /// </summary>
    public PathLine PathLine => pathLine;

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

    Action OnGoalArrive;

    // 컴포넌트들
    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        pathLine = GetComponentInChildren<PathLine>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        mainMaterial = spriteRenderer.material;

        onPhaseEnd += () =>
        {
            isActivate = true;  //페이즈가 끝나면 isActivate를 활성화
        };
        onDissolveEnd += Die;   // 디졸브가 끝나면 죽게 만들기

        OnGoalArrive += () =>
        {
            //현재 위치가 다시 나와도 상관 없을 때
            //SetDestination(map.GetRandomMovablePosition());      

            Vector2Int pos;
            do
            {
                pos = map.GetRandomMovablePosition();

            } while (pos == Position);  //랜덤으로 가져온 위치가 현재 위치랑 다른위치일 때까지 반복

            SetDestination(pos);        //지정된 위치로 이동하기

        };
    
    
    }

    private void OnEnable()
    {
        isActivate = false;
        ResetShaderProperties();            // 스폰 될 때 셰이더 프로퍼티 초기화
        StartCoroutine(StartPhase());       // 페이즈 시작 0
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
                                  //활성화 끄기
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
        if (isActivate)
        {
            isActivate = false;
            StartCoroutine(StartDissolve());

        }
    }

    /// <summary>
    /// 사망 처리용 함수. Dissolve가 끝날 때 실행됨.
    /// </summary>
    void Die()
    {
        onDie?.Invoke();
        onDie = null;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 슬라임 초기화용 함수
    /// </summary>
    /// <param name="gridMap">그리드 맵</param>
    /// <param name="pos">시작 위치의 그리드 좌표</param>
    public void Initialize(GridMap gridMap, Vector3 pos)
    {
        map = gridMap;  //맵 저장
        transform.position = map.GridToWorld(map.WorldToGrid(pos)); //시작 위치에 배치
    }

    /// <summary>
    /// 목적지를 지정하는 함수
    /// </summary>
    /// <param name="goal">목적지의 그리드 좌표</param>
    public void SetDestination(Vector2Int goal)
    {
        path = AStar.PathFind(map, Position, goal); //길찾기해서 경로 저장하기
        pathLine.DrawPath(map, path);               //경로 따라서 그리기
    }

    /// <summary>
    /// Update에서 실행되는 함수. 이동처리.
    /// </summary>
    private void MoveUpdate()
    {if(isActivate) 
        {
        if (path != null && path.Count > 0) //path가 있고 path의 갯수가 보다 크다
        {
            Vector2Int destGrid = path[0];  //path의 [0]번째를 중간 목적지로 설정 

            Vector3 dest = map.GridToWorld(destGrid);   //중간 목적지의 월드 좌표 계산
            Vector3 dir = dest - transform.position;    //방향 결정

            if (dir.sqrMagnitude < 0.001f)              //남은 거리 확인
            {
                //거의 도착한 상태
                transform.position = dest;              // 중간 도착지점으로 위치 옮기기
                path.RemoveAt(0);                       // path의 0번째 제거
            }
            else
            {
                // 아직 거리가 남아있는 상태
                transform.Translate(Time.deltaTime * moveSpeed * dir.normalized);   // 중간 지점까지 계속 이동
            }
        }
        else
        {
            //path 따라서 도착
            OnGoalArrive?.Invoke();
        }
        }
    
    }
}
