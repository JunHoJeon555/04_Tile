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

    private void Awake()
    {
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        

        Player player = GameManager.Inst.Player;
        player.onLifeTimeChange += OnLifeTimeChange;

        maxLifeTime = player.maxLifeTime;


    }

    private void OnLifeTimeChange(float ratio)
    {
        textMeshProUGUI.text = $"{(ratio * maxLifeTime):f2} Sec";
  
    }
}
