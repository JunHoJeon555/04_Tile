using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
///싱글톤 : 객체를 하나만 가지는 디자인 패턴
/// </summary>


public class Singleton<T> : MonoBehaviour where T : Component 
{
    //static  : "정적" 이라는 단어로 많이 번역됨. 프로그램 실행 전에 메모리 주소가 결정되어있는 것에 붙임.
    //           멤버변수에 붙이면 클래스의 모든 객체에서 공용으로 사용할 수 있는 변수가 된다.
    //Dynamic : "동적"을 번역. 프로그램 실행 중에 프로그램 메모리 주소가 결정되는 것에 붙임  
    //

    //기본적으로 생성자가 만들어져있아야한다,
    

    /// <summary>
    /// 초기화를 진행한 표시를 나타내는 플래그
    /// </summary>
    private bool initialized = false;

    //설정 안되었다는 것을 표시하기 위한 상수
    const int NOT_SET = -1;

    // 게임의 메인씬의 인덱스
    private int mainSceneIndex = NOT_SET;

    //이미 종료처리에 들어갔는지 표시하기 위한 용도
    private static bool isShutDown = false;
    
    //싱글톤용 객체 다른 곳에서 접근 못하게 private으로 설정
    private static T instance;


    //싱글톤읽기 전용 프로퍼티. 이 프러퍼티로만 싱글톤 접근가능
    public static T Inst
    {
        get
        {
            if (isShutDown) //종료처리에 들어간 상황이다
            {
                Debug.LogWarning($"{typeof(T)} 싱글톤은 이미 삭제되었습니다."); // 이 코드가 나와면 사용한 곳에서 코드를 잘못 만든 것)
                return null;  //null처리
            }

            if (instance == null) //접근한 시점에서 instanse가 있는지 없는지 확인.
            {
                //없으면 만들어진적이 없다.

                T obj = FindObjectOfType<T>(); //에디터에서 만들어진 것이 있는지 확인
                if (obj == null)                                //null이면 에디터에서 만들어진것도 없다.
                {
                    GameObject gameObj = new GameObject();    //빈 오브젝트 생성 //MonoBehaviour가 아니여서 new 선언이 가능하다
                    gameObj.name = typeof(T).Name;            //이름 변경.
                    gameObj.AddComponent<T>();                //싱글톤을 컴포넌트로 추가.
                }

                instance = obj;                    //없어서 새로 만든 것이든 에디터가 만들어 놓아던 것이든 instance에 저장
                DontDestroyOnLoad(obj.gameObject); // 씬이 닫히더라도 게임 오브젝트가 삭제되지 않게 설정. 
                //얘는 scene이 그대로 넘어가도 얘는 그대로 유지하라.
            }
            return instance; // instance 리턴한다. (없었으면 새로 만들었고 있었으면 있던 것, 그래서 무조건 null이 
        }
    }

    private void Awake()
    {
        if (instance == null) //처음 불러들였을 때
        {
            //instance가 nulll이면 처음 생성 완료된 싱글톤 게임 오브젝트이다.(씬에 배치되어있는 게임 오브젝트)
            instance = this as T; //instance에 이 싱글톤 객체 기록
            DontDestroyOnLoad(instance.gameObject); //씬이 닫히더라도 게임 오브젝트가 삭제되지않게 설정
        }
        else
        {
            //instance가 null이 아닌면 이미 만들어진 싱글톤 게임 오브젝트가 있는 상황    //Singleton이 여러개 배정되었을 때
            if (instance != this) //혹시나 다른 곳에서 지정될 가능성이 있으니까. if로 내가 아니고 
            {
                //Awake되기 전에 다른 코드에서 프로퍼티를 통해서 접근했고 그래서 생성이 된상황
                Destroy(this.gameObject); //나중에 만들어진 

            }
        }

        
    }

    private void OnApplicationQuit()
    {
        isShutDown = true;
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSeneLoadded; //sceneLoaded도 static
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSeneLoadded;

    }
    /// <summary>
    /// 씬이 로드되면 호출이 되는 함수
    /// </summary>
    /// <param name="scene">로드된 씬</param>
    /// <param name="mode">로드 모드</param>
    private void OnSeneLoadded(Scene scene, LoadSceneMode mode)
    {
        if( !initialized)
        {
        PreInitialize();

        }
        Initialize();
    }

    /// <summary>
    /// 이 싱글톤이 처음 만들어 졌을 때 단 한번만 실행될 초기화 함수// 씬이 로드완료되고 나서 
    /// </summary>
    protected virtual void PreInitialize()
    {
        if (!initialized)                       //초기화 되지 않았을 때만 실행
        {
        initialized = true;                     //초기화 되었다고 표시해서 두번 실행되지 않게 하기
        Scene active = SceneManager.GetActiveScene();//현재 씬 가져와서
        mainSceneIndex = active.buildIndex;         //인덱스 저장

        }

    }
    /// <summary>
    /// 이 싱글톤이 만들어지고 씬이 로드될 때마다 실행될 초기화 함수
    /// </summary>
    protected virtual void Initialize() 
    {
    }

}

/// <summary>
/// 일반 싱글톤 예제
/// </summary>
// new 할때 뒤에 ()가 있다. 뉴할때 

public class TestSingleton
{

    //static변수를 만들어서 객체를 만들지 놓고 사용할 수 있게 만들기/
    private static TestSingleton instance; //instance; 객체 => new변수
    //다론 곳에서 instance를 수정하지 못하도록 읽지전용 프로퍼티 만들기
    public static TestSingleton Instance
    {
        get
        {
            if (instance == null) //처음 접근했을 때 new하기
            {
                instance = new TestSingleton();   //MonoBehaviour는 new를 받을 수 없다.?
            }
            return instance;  //항상 리턴 될 때 값을 존재한다.
        }
    }

    // 중복생성 방지 목적
    //private으로 생성자를 만들어 기본 public생성자가 생성되지 않게 막기
    private TestSingleton()
    {

    }

}
