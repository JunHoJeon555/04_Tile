using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    /// <summary>
    /// 다음에 로딩할 씬의 이름
    /// </summary>
    public string nextSceneName = "14_LoadingSample";

    
    
    /// <summary>
    /// 비동기 명령 처리용
    /// </summary>
    AsyncOperation async;
   

   

    /// <summary>
    /// 글자변환용 코루틴
    /// </summary>
    IEnumerator loadingTextCoroutine;
    
    
    /// <summary>
    /// 로딩바의 value가 목표로 하는 값 
    /// </summary>
    float loadRatio = 0f;

    /// <summary>
    /// 로딩바가 증가하는 속도
    /// </summary>
    public float loadingBarSpeed = 1f;

    /// <summary>
    /// 로딩완료되었음을 표시하는 변수 (true일 때 로딩 완료)
    /// </summary>
    bool loadingComplete = false;

    //컴포넌트와 인풋 액션
    PlayerInputActions inputActions;
    Slider slider;
    TextMeshProUGUI loadingText;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }


    private void OnEnable()
    {
        inputActions.UI.Enable();
        inputActions.UI.AnyKey.performed += press;
        inputActions.UI.Click.performed += press;
    }
    private void OnDisable()
    {
        inputActions.UI.Click.performed-= press;
        inputActions.UI.AnyKey.performed-= press;
        inputActions.UI.Disable();
    }

    private void Start()
    {
        slider = FindObjectOfType<Slider>();
        loadingText = FindObjectOfType<TextMeshProUGUI>();

        loadingTextCoroutine = LodingTextProgress();
        StartCoroutine(LoadScene());
        StartCoroutine(loadingTextCoroutine);
    }

    private void Update()
    {
        if(slider.value < loadRatio)
        {
            slider.value += (Time.deltaTime * loadingBarSpeed);
        }
        else
        {
            slider.value = loadRatio;
        }
    }

    private void press(InputAction.CallbackContext context)
    {
        
    }


    IEnumerator LoadScene()
    {
        slider.value = 0f;
        loadRatio = 0f;
        async = SceneManager.LoadSceneAsync(nextSceneName);
        async.allowSceneActivation= false;

        while (loadRatio < 1f)
        {
            loadRatio =async.progress +0.1f;
            yield return null;  
        }
        yield return new WaitForSeconds((loadRatio-slider.value)/loadingBarSpeed);

        StopCoroutine(loadingTextCoroutine);
        loadingComplete= true;
        loadingText.text = "Loading\n Complete.";
    }

    IEnumerator LodingTextProgress()
    {

        while (loadRatio < 1f)
        {
            loadRatio = async.progress + 0.1f;

            loadingText.text = "Loading" + ".";
            yield return new WaitForSeconds(0.3f);
            loadingText.text = "Loading" + ". .";
            yield return new WaitForSeconds(0.3f);
            loadingText.text = "Loading" + ". . .";
            yield return null;
        }
        
        
        yield return null;


    }
}
