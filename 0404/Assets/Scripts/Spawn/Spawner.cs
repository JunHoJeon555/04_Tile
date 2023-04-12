using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class Spawner : MonoBehaviour
{
    /// <summary>
    /// 슬라임 스폰 간격
    /// </summary>
    public float interval = 1.0f;

    /// <summary>
    /// 동시에 유지되는 최대 슬라임 수
    /// </summary>
    public int capacity = 2;

    /// <summary>
    /// 스포너의 크기(transform의 position에서 부터의 크기)
    /// </summary>
    public Vector2 size;

    /// <summary>
    /// 마지막 스폰에서부터 경과한 시간
    /// </summary>
    float elapsed = 0.0f;

    /// <summary>
    /// 현재 스폰된 슬라임의 수
    /// </summary>
    int count = 0;

    /// <summary>
    /// 스폰 영역 중에서 벽이 아닌 지역
    /// </summary>
    List<Node> spawnAreaList;

    SpawnerManager manager;

    private void Start()
    {
        manager = GetComponentInParent<SpawnerManager>();
        spawnAreaList = manager.CalcSpawnArea(this);
    }

    private void Update()
    {
        if(count < capacity)
        {
            elapsed += Time.deltaTime;
            if(elapsed > interval)
            {
                Spawn();
                elapsed= 0.0f;
            }
        }

    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Vector3 basePos = new Vector3(Mathf.Floor(transform.position.x), Mathf.Floor(transform.position.y));
        Vector3 p0 = basePos;
        Vector3 p1 = basePos + Vector3.right*size.x;
        Vector3 p2 = basePos + new Vector3(size.x, size.y);
        Vector3 p3 = basePos + Vector3.up *size.y;

        Handles.color = Color.yellow;
        Handles.DrawLine(p0, p1, 5);
        Handles.DrawLine(p1, p2, 5);
        Handles.DrawLine(p2, p3, 5);    
        Handles.DrawLine(p3, p0, 5);



    }

#endif


    /// <summary>
    /// 몬스터를 한마리 스폰하는 함수
    /// </summary>
    void Spawn()
    {
        if(GetSpawnPosition(out Vector3 spawnPos))
        {
            Slime slime = Factory.Inst.GetSlime();
            count++;
            Transform oldParent = slime.transform.parent;
            slime.onDie += () =>
            {
                slime.transform.SetParent(oldParent);
                count--;
            };
            slime.transform.SetParent(transform);
            manager.SlimeInitialize(slime, spawnPos);
        }
        
    }

    /// <summary>
    /// 스폰할 위치를 구하는 함수
    /// </summary>
    /// <param name="spawnPos">스폰할 위치를 찾았을 때 스폰가능한 위치 중 하나를 돌려줌(월드좌표) 출력용 파라메터</param>
    /// <returns>true면찾은것, false면 스폰할 자리가 없는 것</returns>
    bool GetSpawnPosition(out Vector3 spawnPos)
    {
        bool result = false;
        List<Node> spawns = new List<Node>();
        foreach (var node in spawnAreaList)     //spawnAreaLis에서 몬스터가 없는 노드 구하기
        {
            if(node.gridType != Node.GridType.Monster)
            {
                spawns.Add(node);   
            }
        }

        if(spawns.Count > 0)        
        {

            //스폰가능한 지역이 최소 하나는 있었다. => 그 중에서 하나를 랜덤으로 고르기
            int index = Random.Range(0, spawns.Count);
            Node tartget = spawns[index];
            spawnPos = manager.GridToWorld(tartget.x, tartget.y);
            result= true;

        }
        else
        {
            //스폰 가능한 위치가 없다.
            spawnPos = Vector3.zero;
        }
        return result;

    } 
}