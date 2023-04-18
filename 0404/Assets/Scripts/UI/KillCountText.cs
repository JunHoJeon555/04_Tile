using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;

public class KillCountText : MonoBehaviour
{
    TextMeshProUGUI TextMeshProUGUI;
    public int point;

    private void Awake()
    {
        TextMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        point= 0;
        Player player = GameManager.Inst.Player;
        player.onKill += onKill;
        TextMeshProUGUI.text = $"Kill Point : {point}";
    }

    private void onKill(int point)
    {
        TextMeshProUGUI.text = $"Kill Point : {point}";
    }
}
