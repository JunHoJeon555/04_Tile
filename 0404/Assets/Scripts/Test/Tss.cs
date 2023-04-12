using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Tss : MonoBehaviour
{
    public Transform goal;
    private NavMeshAgent agent;
    private float distance;

    private void Start()
    {
        agent= GetComponent<NavMeshAgent>();
        agent.destination = goal.position;

        distance = agent.remainingDistance;
    }

    private void Update()
    {

        
        //distance = agent.remainingDistance;
        //도착한 것으로 인정하는 거리
    }
}
