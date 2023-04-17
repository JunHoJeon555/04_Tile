using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimePool : ObjectPool<Slime>
{
    Transform pathLines;

    public override void Initialize()
    {
        pathLines = transform.GetChild(0);
        base.Initialize();
    }

    protected override void OnGenerateObject(Slime comp, int index)
    {
        comp.Pool = comp.transform.parent;  // 풀의 트렌스폼 설정

        PathLine pathLine = comp.PathLine;
        pathLine.gameObject.name = $"PathLine_{index}";
        pathLine.transform.SetParent(pathLines);
        //pathLine.gameObject.SetActive(false);
    }
}
