using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LifeTimeText : MonoBehaviour
{
    //시간 변화에 따라 남아있는 플레이어의 수명 출력하기

    TextMeshProUGUI textMeshProUGUI;
    float maxLifeTime;

    public float speed = 1f;
    float targetValue;
    float currentValue;


    private void Awake()
    {
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        

        Player player = GameManager.Inst.Player;
        player.onLifeTimeChange += OnLifeTimeChange;

        maxLifeTime = player.maxLifeTime;


        targetValue = maxLifeTime;
        currentValue = maxLifeTime;


    }
    //private void Update()
    //{
    //    if (currentValue > targetValue)      //슬라이더 위치가 목표치보다 클때
    //    {
    //        //slider.valuer가 줄어야한다.
    //        currentValue -= Time.deltaTime * speed;
    //        if (currentValue < targetValue)  // 줄였다가 목표치를 넘어섰을 때
    //        {
    //            currentValue = Mathf.Max(0, targetValue);   //targetValue가 되거나 0

    //        }
    //        else
    //        {

    //        }
    //    }
    //    else
    //    {
    //        //slider.value가 늘어야한다.
    //        currentValue += speed * Time.deltaTime;
    //        if (currentValue > targetValue)     //늘렸다가 목표치를 넘어섰을 때
    //        {
    //            currentValue = Mathf.Min(1, targetValue);   //targetValue가 되거나 1 

    //        }
    //    }
        
    //}
    private void OnLifeTimeChange(float ratio)
    {
        textMeshProUGUI.text = $"{(maxLifeTime*ratio):f2} Sec";
        //targetValue = maxLifeTime * ratio;  //목표시간 설정
  
    }
}
