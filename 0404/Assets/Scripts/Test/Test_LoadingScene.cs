using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Test_LoadingScene : Test_Base
{
    AsyncOperation async;

    IEnumerator LoadScene()
    {
        async = SceneManager.LoadSceneAsync(1);
        async.allowSceneActivation= false;  //씬 전환을 즉시하지 않고 대기 시키기

        while (async.progress < 0.9f)
        {
            Debug.Log($"Progress : {async.progress}");
            yield return null;

        }

        Debug.Log("Loading Complete");

    }


    
    protected override void Test1(InputAction.CallbackContext _)
    {
        //동기방식(Sychronous)으로 불러온다.
        //SceneManager.LoadScene(1);

        //비동기방식으로 불러온다.
        //async =  SceneManager.LoadSceneAsync(1);
        //async.allowSceneActivation = false;     //씬 전환을 즉시 하지 않고 대기 시키기

        StartCoroutine(LoadScene());

    }

    protected override void Test2(InputAction.CallbackContext _)
    {
        async.allowSceneActivation = true;  //true가 되면 로딩 끝나면 바로 전환

    }

}
