using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject : MonoBehaviour
{/// <summary>
///  오브젝트 풀이 사용할 게임 오브젝트
/// </summary>


    //이 게임오브젝트가 비활성화 될 때 실행되는 델리게이트
    public Action onDisable;
    //게임 오브젝트가 비활성화 될 때 실행 
    protected virtual void OnDisable()
    {
        onDisable?.Invoke();  //이 델리게이트에 등록된 함수들 실행
    }
    /// <summary>
    /// 특정 시간 후에 비활성화 시키기 위한 델리게이트
    /// </summary>
    /// <param name="delay">딜레이시킬시간(default = 0)</param>
    /// <returns></returns>
    protected IEnumerator LifeOver(float delay = 0.0f)
    {
        yield return new WaitForSeconds(delay); // delay만큼 대기하고
        this.gameObject.SetActive(false);       // 비활성화 시키기        
    }
}
