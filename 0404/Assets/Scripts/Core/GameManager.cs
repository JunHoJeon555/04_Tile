using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    /// <summary>
    /// 게임에 존재하는 플레이어
    /// </summary>
    Player player;
    public Player Player => player;

    /// <summary>
    /// 게임의 전체 맵을 관리하는 매니저
    /// </summary>
    MapManager mapManager;
    public MapManager MapManager => mapManager;

    protected override void PreInitialize()
    {
        base.PreInitialize();

        mapManager = GetComponent<MapManager>();
        mapManager.PreInitialize();
    }

    protected override void Initialize()
    {
        base.Initialize();
        player = FindObjectOfType<Player>();
        mapManager.Initialize();
    }

}
