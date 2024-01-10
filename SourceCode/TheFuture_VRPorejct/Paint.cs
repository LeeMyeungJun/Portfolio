using UnityEngine;

public abstract class Paint
{
    protected float detectionRadius = 25f;
    public abstract void BeginAction(Transform ShootPos);
    public abstract void Action(Transform ShootPos, Vector2 tumbValue, System.Action<float> OnColor = null, System.Action<Vector3[]> OnPosition = null);
    public abstract void EndAction(System.Action<Define.PaintType> OnEnd = null);
    public abstract void Init();
    public virtual float GetDetectionRadus() { return detectionRadius; }
}
