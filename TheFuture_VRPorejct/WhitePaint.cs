using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhitePaint : Paint
{
    public override void BeginAction(Transform ShootPos)
    {
    }
    public override void Action(Transform ShootPos, Vector2 tumbValue, System.Action<float> OnColor = null, System.Action<Vector3[]> OnPosition = null)
    {
    }

    public override void EndAction(Action<Define.PaintType> OnEnd = null)
    {
    }

    public override void Init()
    {
       
    }
}
