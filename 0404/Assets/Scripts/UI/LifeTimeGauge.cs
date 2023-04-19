using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class LifeTimeGauge : MonoBehaviour
{
    Slider slider;

    public float speed = 1f;
    float targetValue =1.0f;
    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void Start()
    {
        Player player = GameManager.Inst.Player;
        player.onLifeTimeChange += OnLifeTimeChange;
        slider.value= 1;
        targetValue = 1f;
    }

    private void Update()
    {
        //slider.value가 targetValue쪽으로 변경되도록 실행
        if(slider.value > targetValue)      //슬라이더 위치가 목표치보다 클때
        {
            //slider.valuer가 줄어야한다.
            slider.value -=  Time.deltaTime * speed;
            if(slider.value < targetValue)  // 줄였다가 목표치를 넘어섰을 때
            {
                slider.value = Mathf.Max(0, targetValue);   //targetValue가 되거나 0

            }
            else
            {

            }
        }
        else
        {
            //slider.value가 늘어야한다.
            slider.value += speed * Time.deltaTime;
            if (slider.value > targetValue)     //늘렸다가 목표치를 넘어섰을 때
            {
                slider.value = Mathf.Min(1, targetValue);   //targetValue가 되거나 1 

            }
        }
    }


    /// <summary>
    /// 플레이어의 수명이 변경되면 실행되는 함수
    /// </summary>
    /// <param name="ratio">비율(0~1)</param>
    private void OnLifeTimeChange(float ratio)
    {
        //ratio = Mathf.MoveTowards(slider.value, ratio , speed * Time.deltaTime);
        targetValue = ratio;        //목표치만 변경
    }
}



//LifeTimeGauge. LifeTimeText, KillCountTxt, PostProcessManager의 변화가 천천히 일어나게 만들기
