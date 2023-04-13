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
        PathLine pathLine = comp.PathLine;
        pathLine.gameObject.name = $"PathLine_{index}";
        pathLine.transform.SetParent(pathLines);
        //pathLine.gameObject.SetActive(false);
    }

}
