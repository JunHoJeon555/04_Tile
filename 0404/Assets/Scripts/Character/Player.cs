using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    //수명 관련---------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// 플레이어의 최대 수명
    /// </summary>
    public float maxLifeTime = 10.0f;

    /// <summary>
    /// 플레이어의 현재 수명
    /// </summary>
    float lifeTime;
    
    /// <summary>
    /// 수명확인 및 설정용 프로퍼티
    /// </summary>
    public float LifeTime
    {
        get => lifeTime;
        set
        {
            lifeTime = value;
            if(lifeTime < 0.0f && !isDead)
            {
                Die();      //살아있는데 수명이 0이하면 사망
                
            }
            else
            {
                lifeTime = Mathf.Clamp(value, 0.0f, maxLifeTime); //최소0, 최대 maxLifeTime로 클램프
            }
            onLifeTimeChange?.Invoke(lifeTime/maxLifeTime);
        }
    }

    /// <summary>
    /// 플레이어 수명이 변경될 때 실행 될 델리게이트, (파라메터 : 비율)
    /// </summary>
    public Action<float> onLifeTimeChange;

    public Action<int> onKill;


    /// <summary>
    /// 전체 플레이 시간
    /// </summary>
    float totalPlayTime;

    /// <summary>
    /// 잡은 슬라임 수 
    /// </summary>
    int killCount = 0;

    /// <summary>
    /// 플레이어의 생존 여부
    /// </summary>
    bool isDead = false;

    /// <summary>
    /// 플레이어가 죽었을 때 실행될 델리게이트. 전체 플레이 시간과 킬 카운트넘겨줌
    /// </summary>
    public Action<float, int> onDie;

    //이동 관련---------------------------------------------
    /// <summary>
    /// 플레이어의 이동속도
    /// </summary>
    public float speed = 3.0f;


    /// <summary>
    /// 플레이어의 입력방향
    /// </summary>
    Vector2 inputDir;

    /// <summary>
    /// 공격했을 때 저장해 ㄶ은 원래 이동방향
    /// </summary>
    Vector2 oldInputDir;
    bool isMove = false;

    /// <summary>
    /// 플레이어가 현재 위치하고 있는 맵의 그리드 좌표
    /// </summary>
    Vector2Int currentMap;

    Vector2Int CurrentMap
    {
        get => currentMap;
        set
        {
            if (value != currentMap)            //맵을 이동했을 때만
            {
                currentMap = value;             //변경하고
                onMapMoved?.Invoke(currentMap); //델리게이트 실행
            }
        }
    }

    /// <summary>
    /// 맵이 변경되었을 때 실행될 델리게이트(파라메터 : 진입한 맵의 그리드 좌표)
    /// </summary>
    public Action<Vector2Int> onMapMoved;

    //공격관련--------------------------------

    bool isAttacking = false;
    bool isAttackValid = false;

    Transform attackAreaCenter;

    /// <summary>
    /// 공격쿨타임
    /// </summary>
    public float attackCoolTime = 1f;

    /// <summary>
    /// 현재 공격쿨타임
    /// </summary>
    float currentAttackCoolTime = 0.0f;

    /// <summary>
    /// 플레이어의 공격 범위 안에 들어와 있는 모든 슬라임
    /// </summary>
    List<Slime> attackTargetList;

    //기타-------------------------

    //컴포넌트들
    Animator anim;
    Rigidbody2D rigid;
    //입력 인풋 액샨
    PlayerInputActions inputActions;

    MapManager mapManager;


    private void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        inputActions = new PlayerInputActions();

        attackAreaCenter = transform.GetChild(0);

        attackTargetList = new List<Slime>(4);
        EnemySensor sensor = attackAreaCenter.GetComponentInChildren<EnemySensor>();    // 센서 찾고
        sensor.onEnemyEnter += (slime) =>
        {
            // 센서 안에 슬라임이 들어오면 
            if (isAttackValid)
            {
                // 공격이 유효한 상태면 바로 죽이기
                slime.OnAttacked();
            }
            else
            {
                // 공격이 아직 유효하지 않으면 리스트에 담아 놓기
                attackTargetList.Add(slime);    // 리스트에 추가하고
                slime.ShowOutline(true);        // 아웃라인 표시
            }
        };
        sensor.onEnemyExit += (slime) =>
        {
            // 센서에서 슬라임이 나가면
            attackTargetList.Remove(slime); // 리스트에서 제거하고
            slime.ShowOutline(false);       // 아웃라인 끄기
        };
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnStop;
        inputActions.Player.Attack.performed += OnAttack;
    }



    private void OnDisable()
    {
        inputActions.Player.Attack.performed -= OnAttack;
        inputActions.Player.Move.canceled -= OnStop;
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Disable();
    }

    private void Start()
    {
        mapManager = GameManager.Inst.MapManager;
        LifeTime = maxLifeTime;
    }

    private void Update()
    {
        currentAttackCoolTime -= Time.deltaTime;//무조건 쿨타임 감소시키기
        LifeTime -=Time.deltaTime;
        totalPlayTime += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        //transform.Translate(Time.fixedDeltaTime * speed * inputDir);

        rigid.MovePosition(rigid.position + Time.fixedDeltaTime * speed * inputDir);

        CurrentMap = mapManager.WorldToGrid(rigid.position);

        //Debug.Log(mapManager.WorldToGrid(rigid.position));
    }



    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        if (isAttacking)
        {
            oldInputDir = input; //공격 중일 때는 백업해 놓은 값만 변경
        }
        else
        {
            inputDir = input;    //입력 방향 저장
            anim.SetFloat("InputX", inputDir.x);        //애니메인션 파라메터 설정
            anim.SetFloat("InputY", inputDir.y);
            AttackAreaRotate(inputDir);
        }

        
        isMove = true;                              //이동 중이라고 표시
        anim.SetBool("IsMove", isMove);

        //공격 영역 중심축 회전하기



    }


    private void OnStop(InputAction.CallbackContext context)
    {
        inputDir = Vector2.zero;        //입력방향 (0,0)으로 설정
        isMove = false;                 //멈췄다고 표시

        anim.SetBool("IsMove", isMove); //애니메이션 파라메터 설정 

    }

    private void OnAttack(InputAction.CallbackContext context)
    {

        if (currentAttackCoolTime < 0)
        {
            isAttacking = true;
            oldInputDir = inputDir;                 //입력방향 백업
            inputDir = Vector2.zero;                //입력방향 초기화(안움직이게 만드는 목적)
            anim.SetTrigger("Attack");              //공격애니메이션 재생
            currentAttackCoolTime = attackCoolTime; //쿨타임초기화
        }
    }


    /// <summary>
    /// 공격이 효과가 있을 때 실행
    /// 애니메이션 이벤트로 실행할 함수.
    /// </summary>
    public void AttackValid()
    {
        isAttackValid = true;

        while (attackTargetList.Count > 0)       // 리스트에 슬라임이 남아있으면 계속 반복
        {
            Slime slime = attackTargetList[0];  // 하나를 꺼내서
            attackTargetList.RemoveAt(0);
            slime.OnAttacked();                 // 공격하기
        }
    }



    /// <summary>
    /// 공격 효과가 없어졌을 때 실행
    /// 애니메이션 이벤트로 실행할 함수.
    /// </summary>
    public void AttackNotValid()
    {
        isAttackValid = false;
    }




    /// <summary>
    /// 백업해 놓은입력방향 복원하는 함수
    /// </summary>
    public void RestoreInputDir()
    {
        if (isMove)                 //아직 이동 중일 때만 
        {
            inputDir = oldInputDir; //입력방향 복원
            anim.SetFloat("InputX", inputDir.x);
            anim.SetFloat("InputY", inputDir.y);
            
            AttackAreaRotate(inputDir);
        }
        isAttacking = false;            //이동 중이든 아니든 무조건 false로 초기화
    }

    private void AttackAreaRotate(Vector2 input)
    {
        // 공격 영역 중심축 회전하기
        if (input.y < 0)            // 아래로 이동
        {
            attackAreaCenter.rotation = Quaternion.identity;
        }
        else if (input.y > 0)       // 위로 이동
        {
            attackAreaCenter.rotation = Quaternion.Euler(0, 0, 180.0f);
        }
        else if (input.x > 0)       // 오른쪽으로 이동
        {
            attackAreaCenter.rotation = Quaternion.Euler(0, 0, 90.0f);  // 반시계방향으로 90도
        }
        else if (input.x < 0)       //왼쪽으로 이동
        {
            attackAreaCenter.rotation = Quaternion.Euler(0, 0, -90.0f); // 시계방향으로 90도
        }
        else
        {
            attackAreaCenter.rotation = Quaternion.identity;    // 중립
        }
    }
    
    /// <summary>
    /// 플레이어가 죽으면 실행되는 함수
    /// </summary>
    void Die()
    {
        lifeTime = 0.0f;            //수명은 0으로

        isDead= true;               //죽었다고 표시
        onDie?.Invoke(totalPlayTime, killCount);        //죽었다고 알림
            
    }


    /// <summary>
    /// 수명 추가해주는 함수
    /// </summary>
    /// <param name="time">추가되는 수명</param>
    public void AddLifeTime(float time)
    {
        LifeTime += time;
    }

    /// <summary>
    /// 킬카운트 1증가 시키는 함수
    /// </summary>
    public void AddKillCount()
    {
        
        
        killCount++;
        onKill?.Invoke(killCount);
    }

}
