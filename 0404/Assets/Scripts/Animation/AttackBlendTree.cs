using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//AttackBlendTree상태에  추가한 스크립트
public class AttackBlendTree : StateMachineBehaviour
{

    /// <summary>
    /// 미리 찾아놓은 플레이어
    /// </summary>
    Player player;

    private void Awake()
    {
        //플레이어찾기
        player = FindObjectOfType<Player>();
    }

    //OnstateExit는 트레지션이 끝날 때나 상태머신이 끝낼 때 호출된다/
    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log($"StateExit {player.gameObject.name}" );
        player.RestoreInputDir();   //플레이어의 이동방향 복원시키기
    }


    
}
