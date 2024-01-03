using UnityEngine;

public class RedPaint : Paint
{
    readonly int maxBullet = 5;
    protected int currentBullet = 5; //maxbullet
    protected int CurrentBullet
    {
        get => currentBullet;
        set
        {
            if (value < 0)
            {
                currentBullet = 0;

            }
            else if (value > maxBullet)
                currentBullet = maxBullet;
            else
                currentBullet = value;
        }
    }
    LayerMask mask = LayerMask.GetMask("Environment", "Monster","ProbObj");
    bool isFire = false;
    readonly float DelayFire = 0.3f;
    float elapsedTime = 99.9f;
    public override void BeginAction(Transform shootPos)
    {
        isFire = true;
    }
    
    public override void Action(Transform ShootPos, Vector2 tumbValue, System.Action<float> OnColor = null, System.Action<Vector3[]> OnPosition = null)
    {
        elapsedTime += Time.deltaTime;
        if (isFire == false) return;
        if (CurrentBullet <= 0) return;

        if(elapsedTime >= DelayFire)
        {
            elapsedTime = 0.0f;
            if (Physics.Raycast(ShootPos.position, ShootPos.forward, out RaycastHit hitInfo, detectionRadius, mask))
            {
                Managers.Sound.Play(Sound.RedPaintInteract, ShootPos.position);
                CurrentBullet--;
                OnColor?.Invoke(1.0f - (currentBullet / (float)maxBullet));
                Managers.Effect.Play("RedPaintExplosion", hitInfo.point + (ShootPos.position - hitInfo.point).normalized * 0.2f, Vector3.one);

                if(hitInfo.transform.CompareTag("Barrier"))
                {
                    if(hitInfo.transform.TryGetComponent<BossBarrier>(out BossBarrier bossBarrier))
                        bossBarrier.OnBlock(hitInfo);

                    if (hitInfo.transform.TryGetComponent<Barrier>(out Barrier barrier))
                        barrier.OnBlock(hitInfo);
                }
                else
                {
                    var hitObj = hitInfo.transform.GetComponent<HitableObject>();
                    hitObj?.Hit();
                }
            }
        }
       
    }

    public override void EndAction(System.Action<Define.PaintType> OnEnd = null)
    {
        isFire = false;
        if(CurrentBullet <= 0)
            OnEnd?.Invoke(Define.PaintType.White);
    }
    public override void Init()
    {
        CurrentBullet = maxBullet;
        detectionRadius = 100f;
    }
}
