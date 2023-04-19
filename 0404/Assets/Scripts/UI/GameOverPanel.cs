using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    /// <summary>
    /// 알파값 변경 속도
    /// </summary>
    public float alphaChangeSpeed = 1.0f;

    /// <summary>
    /// 패널 전체 알파 조정용
    /// </summary>
    CanvasGroup canvasGroup;

    /// <summary>
    /// 전체 플레이 시간 출력할 텍스트
    /// </summary>
    TextMeshProUGUI playTime;

    /// <summary>
    /// 전체 킬 수 출력할 텍스트
    /// </summary>
    TextMeshProUGUI killCount;

    /// <summary>
    /// 재시작용 버튼
    /// </summary>
    Button restart;

    private void Awake()
    {
        // 컴포넌트 찾기
        canvasGroup = GetComponent<CanvasGroup>();
        Transform child = transform.GetChild(1);
        playTime = child.GetComponent<TextMeshProUGUI>();
        child = transform.GetChild(2);
        killCount = child.GetComponent<TextMeshProUGUI>();
        restart = GetComponentInChildren<Button>();

        // 버튼에 함수 등록
        restart.onClick.AddListener(OnRestartClick);
    }

    private void Start()
    {
        StopAllCoroutines();
        Player player = GameManager.Inst.Player;
        player.onDie += OnPlayerDie;    // 플레이어 사망시 실행할 함수 등록
    }

    /// <summary>
    /// 플레이어가 죽었을 때 실행될 함수
    /// </summary>
    /// <param name="totalPlayTime">전체 플레이 시간</param>
    /// <param name="totalKillCount">전체 킬 수</param>
    private void OnPlayerDie(float totalPlayTime, int totalKillCount)
    {
        playTime.text = $"Total Play Time\n\r< {totalPlayTime:F1} Sec >";   // 출력 될 텍스트 변경
        killCount.text = $"Total Kill Count\n\r< {totalKillCount} Kill >";

        StartCoroutine(StartAlphaChange()); // 알파값 변경되도록 코루틴 실행
    }

    /// <summary>
    /// 알파 변경용 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator StartAlphaChange()
    {
        while (canvasGroup.alpha < 1.0f) // 알파값이 1 이상이 될때까지 반복
        {
            canvasGroup.alpha += Time.deltaTime * alphaChangeSpeed; // 알파값 증가
            yield return null;          // 다음 프레임까지 대기
        }
    }

    /// <summary>
    /// 버튼이 클릭되었을 때 실행될 함수
    /// </summary>
    private void OnRestartClick()
    {
        StartCoroutine(WaitUnloadAll());

    }

    /// <summary>
    /// 씬 전부 로딩 해제한 후. 로딩 씬으로 돌아가는 함수
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitUnloadAll()
    {
        MapManager mapManager = GameManager.Inst.MapManager;
        while (!mapManager.IsUnloadAll) // 로드되었던 모든씬이 해제될때까지 대기
        {
            yield return null;
        }
        SceneManager.LoadScene("LoadingScene"); // 로드해제가 끝나면 로딩씬으로 돌아가기
    }
}
