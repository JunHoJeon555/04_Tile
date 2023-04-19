using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessManager : MonoBehaviour
{
    public float speed = 1f;
    float targetValue = 1.0f;


    /// <summary>
    /// 포스트 프로세스용 볼륨
    /// </summary>
    Volume postProcessVolume;
    
    /// <summary>
    /// 볼륨에 들어있을 비네트 효과를 사용하기 위한 클래스
    /// </summary>
    Vignette vignette;

    private void Awake()
    {
        postProcessVolume = GetComponent<Volume>();
        postProcessVolume.profile.TryGet<Vignette>(out vignette);       //찾기. 없으면 null이 설정되고 있으면 null 아닌 값
        //TryGet : 없으면 못가져올수도있다.

    }

    private void Start()
    {
        Player player = GameManager.Inst.Player;
        player.onLifeTimeChange += OnLifeTimeChange;        //플ㄹ[이어의 수명 변경 델리게이트에 함수 등록
        vignette.intensity.value = 0f;  //초기화
    }
    private void Update()
    {
        if (vignette.intensity.value > targetValue)      //슬라이더 위치가 목표치보다 클때
        {
            //slider.valuer가 줄어야한다.
            vignette.intensity.value -= Time.deltaTime * speed;
            if (vignette.intensity.value < targetValue)  // 줄였다가 목표치를 넘어섰을 때
            {
                vignette.intensity.value = Mathf.Max(0, targetValue);   //targetValue가 되거나 0

            }
        }
        else
        {
            //slider.value가 늘어야한다.
            vignette.intensity.value += speed * Time.deltaTime;
            if (vignette.intensity.value > targetValue)     //늘렸다가 목표치를 넘어섰을 때
            {
                vignette.intensity.value = Mathf.Min(1, targetValue);   //targetValue가 되거나 1 

            }
        }
    }
    private void OnLifeTimeChange(float ratio)
    {
        //vignette.intensity.value= 1.0f - ratio;             //수명 변한 때마다 비네트 정도 변경
        targetValue = 1.0f - ratio;
    }
}
