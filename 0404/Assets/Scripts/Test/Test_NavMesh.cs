using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Test_NavMesh : Test_Base
{

    NavMeshAgent agent;

    protected override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
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


    private void OnClick(InputAction.CallbackContext context)
    {
        Vector2 screenPos = Mouse.current.position.ReadValue(); // 마우스가 클릭된 스크린 좌표
        //Debug.Log(screenPos);
        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            //hit.point;  // 내가 클릭(피킹)한 월드 좌표

            agent.SetDestination(hit.point);

            //agent.Warp()              //순간이동
            //agent.speed               //이동속도
            //agent.acceleration        //가속도
            //agent.angularSpeed        //회전속도
            //agent.radius              //에이전트의 반지름
            //agent.avoidancePriority   //회피우선 순위
            //agent.remainingDistance   //남아있는 이동거리
            //agent.stoppingDistance;   //도착한 것으로 인정하는 거리
            //agent.pathPending;        //에이전트가 경로를 계산 중이라 정지상태일 때 true.
            //agent.autoRepath;         //경로가 사용하지 못하게 되면 자동으로 경로 재계산

        }
    }
}
