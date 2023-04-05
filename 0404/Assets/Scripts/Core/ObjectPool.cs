using System.Collections;
using System.Collections.Generic; //Generic은 들어갈 코드을 같은데 타입만 다르다
using UnityEngine;



//<>들어간 것은 제네릭 타입
//코드 내용은 동일한데 타입만 다를 경우 유용

//where T : Component
//T타입은 반드시 Component를 상속받아야한다.

public class ObjectPool<T> : MonoBehaviour where T : PoolObject
{
    //하나 
    //둘 끝나고 나서 되돌리는거 
    //셋 
    //ArrayList listA; // 값 , 참조 리스트에 다 넣을 수 있다. // 박싱 언박싱이 발생하여 사용하지 말자
    //List<int> list;  // 리스트에 int만 넣을 수 있다, // 제네릭 타입을 사용하는 것이 좋다.


    //풀에 생성해 놓을 오브젝트의 프리팹
    public GameObject originalPrefab;

    //풀의 크기, 처음에 생성하는 오브젝트 갯수. 갯수는 2^n으로 잡는게 좋다.
    public int poolSize = 64;

    //풀이 생성한 모든 오브젝트가 들어있는 배열
    T[] pool; //나중에는 2배로 늘어나면 새로 만들어서 그 변수를 넣어야한다.

    //사용 가능한 오브젝트들이 들어있는 큐
    Queue<T> readyQueue;  //Queue도 클래서여서 껍데기만 있는거임
    
    // 처음 만들어졌을 때 한번 실행될 코드
    public void Initialize() //일단 퍼블릭 먼저 테스트를 하기위함
    {

        if(pool == null)
        {
            //풀이 없으면 새로 만들고
            pool = new T[poolSize];
            readyQueue = new Queue<T>(poolSize); //선언 방식  //capacity를 poolSize만큼 잡고 생성
            //readyQueue.Count= 0; // 실제로 사용하는 갯수이다.
            //readyQueue.Capacity(); //미리만들어 놓은 노드의 갯수 //한번에 만드는것이 좋기 때문에 capatity가 좋을 것이다. 
    
           GenerateObject(0,poolSize, pool);//첫 번째 풀 생성
    
        }
        else
        {
            //있으면 풀안의 오브젝트를 모두 비활성화 시킨다.
            foreach (var obj in pool)
            {
                obj.gameObject.SetActive(false);
            }
        }
    
    
    }





    /// <summary>
    /// 오브젝트를 생성하고 배열에 추가하는 함수
    /// </summary>
    /// <param name="Start">새로 생성한 오브젝트가 들어가기 시작할 배열의 인덱스</param>
    /// <param name="end">새로 생성한 오브젝트가 마지막으로 들어가는 인덱스의 한칸 앞</param>
    /// <param name="newArray">새로 생성한 오브젝트가 들어갈 배열(풀)</param>
    void GenerateObject(int Start, int end, T[] newArray)
    {

        for(int i = Start; i <end; i++)                                //start 부터 end까지 반복한다.
        {

            GameObject obj = Instantiate(originalPrefab,transform);    //프리팹 생성하고 풀의 자식으로 설정
            obj.gameObject.name = $"{originalPrefab.name}_{i}";        //이름 변경
            T comp = obj.GetComponent<T>();                            //컴포넌트 찾고(PoolObject 타입이다.)
            //리턴타입이 void이고 파라메터가 없는 람다함수를 onDisable에 등록
            //델리게이트가 실행되면 readyQueue.Enque
            comp.onDisable += () => readyQueue.Enqueue(comp);
            
            newArray[i] = comp;                                   //풀 배열에 넣기     
            obj.SetActive(false);                                 //비활성화해서 안보이게 만고 레디큐에도 추가하기
        }
    }
    
    public T GetObject()
    {
        if( readyQueue.Count > 0)   //큐에 오브젝트가 있는지 확인
        {
            T obj = readyQueue.Dequeue(); //큐에 오브젝트가 있으면 큐에서 하나 꺼내고 
            obj.gameObject.SetActive(true); // 활성화 시킨 다음에
           
            
            return obj;                              //리턴
        }
        else
        {
            ExpandPool();
            return GetObject();                           // 새롭게 하나 요청

        }
    }

    public T GetObject(Transform spawnTransform)
    {
        if(readyQueue.Count > 0)
        {
            T obj = readyQueue.Dequeue();
            obj.transform.position = spawnTransform.position;
            obj.transform.rotation = spawnTransform.rotation;
            obj.transform.localScale = spawnTransform.localScale;
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            ExpandPool();
            return GetObject();

        }


    }
    /// <summary>
    /// 풀을 두배로 확장 시키는 함수
    /// </summary>
    private void ExpandPool()
    {
        Debug.LogWarning($"{this.gameObject.name} 풀 사이즈 증가");
        // 큐에 오브젝트가 없으면 풀을 두배로 늘린다.
        int newSize = poolSize * 2; //새 크기 설정
        T[] newPool = new T[newSize]; // 새 풀 설정
        for (int i = 0; i < poolSize; i++)
        {
            newPool[i] = pool[i];
        }
        GenerateObject(poolSize, newSize, newPool);

        pool = newPool;
        poolSize = newSize;
        
    }
}
