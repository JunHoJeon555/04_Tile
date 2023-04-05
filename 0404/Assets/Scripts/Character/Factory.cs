using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 오브젝트 생성해주는 클래스
/// </summary>
public class Factory : Singleton<Factory>
{
    // 생성할 오브젝트의 풀들
    SlimePool slimePool;

    /// <summary>
    /// 이 싱글톤이 만들어질 때 처음 한번만 호출될 함수
    /// </summary>
    protected override void PreInitialize()
    {
        // 자식으로 붙어있는 풀들 다 찾아놓기
        slimePool = GetComponentInChildren<SlimePool>();        
    }

    /// <summary>
    /// 씬이 로드될 때마다 호출되는 초기화 함수
    /// </summary>
    protected override void Initialize()
    {
        slimePool?.Initialize();       // ?.은 null이 아니면 실행, null이면 아무것도 하지 않는다.        
    }

    /// <summary>
    /// Slime풀에서 Slime하나 꺼내는 함수
    /// </summary>
    /// <param name="parentT">기준 트랜스폼(이 트랜스폼의 위치, 회전, 스케일 사용)</param>
    /// <returns></returns>
    public Slime GetSlime(Transform parentT = null) => slimePool?.GetObject(parentT);

}
