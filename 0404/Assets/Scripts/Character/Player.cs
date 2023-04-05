using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
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

    bool isAttacking = false;
 
    /// <summary>
    /// 공격쿨타임
    /// </summary>
    public float attackCoolTime = 1f;
    
    /// <summary>
    /// 현재 공격쿨타임
    /// </summary>
    float currentAttackCoolTime = 0.0f;

    Animator anim;
    Rigidbody2D rigid;

    PlayerInputActions inputActions;

    

    private void Awake()
    {
        anim= GetComponent<Animator>();
        inputActions = new PlayerInputActions();
        rigid= GetComponent<Rigidbody2D>();
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
        inputActions.Player.Attack.performed-= OnAttack;
        inputActions.Player.Move.canceled -= OnStop;
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Disable();
    }

    private void Update()
    {
        currentAttackCoolTime -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        transform.Translate(Time.fixedDeltaTime * speed * inputDir);     
        
        rigid.MovePosition(rigid.position + Time.fixedDeltaTime *speed * inputDir);
        
        
    }
  


    private void OnMove(InputAction.CallbackContext context)
    {
        if (isAttacking)
        {
            oldInputDir = context.ReadValue<Vector2>(); //공격 중일 때는 백업해 놓은 값만 변경
        }
        else
        {
            inputDir = context.ReadValue<Vector2>();    //입력 방향 저장
            anim.SetFloat("InputX", inputDir.x);        //애니메인션 파라메터 설정
            anim.SetFloat("InputY", inputDir.y);
        }
       
            isMove = true;                              //이동 중이라고 표시
            anim.SetBool("IsMove", isMove);
        

    }


    private void OnStop(InputAction.CallbackContext context)
    {
        inputDir = Vector2.zero;        //입력방향 (0,0)으로 설정
        isMove = false;                 //멈췄다고 표시
        
        anim.SetBool("IsMove", isMove); //애니메이션 파라메터 설정 
        
    }

    private void OnAttack(InputAction.CallbackContext context)
    {

        if(currentAttackCoolTime < 0)
        {
            isAttacking = true;
            oldInputDir = inputDir;                 //입력방향 백업
            inputDir = Vector2.zero;                //입력방향 초기화(안움직이게 만드는 목적)
            anim.SetTrigger("Attack");              //공격애니메이션 재생
            currentAttackCoolTime = attackCoolTime; //쿨타임초기화
        }
      

        
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
        }
        isAttacking = false;            //이동 중이든 아니든 무조건 false로 초기화
    }

}
