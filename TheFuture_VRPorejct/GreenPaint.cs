using System.Collections.Generic;
using UnityEngine;


public class GreenPaint : Paint
{
    private Transform target;
    private Rigidbody rb;
    private Vector3 offset;

    LayerMask detectionLayer = LayerMask.GetMask("InteractMoveable");

    private Collider[] colliders = new Collider[15];
    private List<KeyValuePair<MeshRenderer, int>> prevDetectObjects = new List<KeyValuePair<MeshRenderer, int>>();
    private float rotSpeed = 90f;
    private float ySpeed = 2.0f;
    private float xSpeed = 3.0f;
    GameObject auraParticle;
    public override void BeginAction(Transform ShootPos)
    {
        if (Physics.Raycast(ShootPos.position, ShootPos.forward, out RaycastHit hitInfo, detectionRadius, detectionLayer))
        {
            offset = hitInfo.transform.position - ShootPos.position;
            target = hitInfo.transform;
            if (target.TryGetComponent<Rigidbody>(out Rigidbody rig))
            {
                rb = rig;
                rig.isKinematic = true;
            }
            Managers.Sound.Play(Sound.GreenPaintInteract, ShootPos.position);
        }
        else
        {
            if (target != null)
            {
                if (target.TryGetComponent<Rigidbody>(out Rigidbody rig))
                    rig.isKinematic = false;
                target = null;
                rb = null;
                offset = Vector3.zero;
            }
        }
    }

    Vector3 previousPos = Vector3.zero;
    Vector3[] bazierPos = new Vector3[4];
    public override void Action(Transform ShootPos, Vector2 tumbValue, System.Action<float> OnColor = null, System.Action<Vector3[]> OnPosition = null)
    {
        DetectObjectsWithChangeMaterial(ShootPos);
        auraParticle.transform.position = ShootPos.position + Vector3.down * 0.5f;
        if (target == null || target.gameObject.activeSelf == false)
        {
            for (int i = 0; i < bazierPos.Length; i++)
                bazierPos[i] = Vector3.zero;
            OnPosition?.Invoke(bazierPos);
            target = null;
            return;
        }

        previousPos = target.position;

        //이동  offset 방향값으로 가중치주는거
        Vector3 magnitudeDis = offset + offset.normalized * tumbValue.y * ySpeed * Time.deltaTime;
        float distance = Vector3.Magnitude(magnitudeDis);
        if (distance <= detectionRadius + 0.1f)// Z로 사정거리 되는지  당기는거랑 미는거 
        {
            offset += offset.normalized * tumbValue.y * ySpeed * Time.deltaTime;
            //플레이어 각도에의한 포지션
            float dist = Vector3.Magnitude(offset);
            dist = Mathf.Clamp(dist, 0.3f, detectionRadius);
            Vector3 nextPosition = dist * ShootPos.forward + ShootPos.position;
            Vector3 bazierDir = (nextPosition - target.position).normalized;
            float bazierMagnitude = (nextPosition - target.position).magnitude;

            //도착장소 벽인지 체크 벽아니면 이동 벽이면 offset 계산
            if (Physics.Raycast(target.position, bazierDir, out RaycastHit hit,
                0.1f, LayerMask.GetMask("Environment")))
            {
                offset = target.position - ShootPos.position;
            }
            else
            {
                //target.position = nextPosition;
                if (bazierMagnitude < 0.1f)
                {
                    for (int i = 0; i < bazierPos.Length; i++)
                        bazierPos[i] = Vector3.zero;
                    target.position = nextPosition;
                }
                else
                {
                    Vector3 dir = (nextPosition - ShootPos.position).normalized;
                    float magnitude = (nextPosition - ShootPos.position).magnitude;

                    bazierPos[0] = ShootPos.position;
                    bazierPos[1] = ShootPos.position + dir * magnitude * 0.65f + bazierDir * 1.2f;
                    bazierPos[2] = ShootPos.position + dir * magnitude * 0.8f + bazierDir * 0.4f;
                    bazierPos[3] = target.position;

                    target.position += bazierDir * xSpeed * Time.deltaTime;
                }



                OnPosition?.Invoke(bazierPos);
            }
        }

        //회전
        Quaternion rot = target.rotation;
        float newYRotation = target.rotation.eulerAngles.y + tumbValue.x * Time.deltaTime * rotSpeed;
        target.transform.rotation = Quaternion.Euler(target.rotation.eulerAngles.x, newYRotation, target.rotation.eulerAngles.z);
    }

    void DetectObjectsWithChangeMaterial(Transform ShootPos)
    {
        ResetColor();

        if (target != null)
        {
            MeshRenderer[] mrs = target.transform.GetComponentsInChildren<MeshRenderer>();
            foreach (var item in mrs)
            {
                item.materials = Managers.Mat.ChangeMaterials(item.transform.GetInstanceID(), item.materials, "Green_Glow", "Green_line");
            }
            return;
        }
        //범위에있는거 색넣기
        int count = Physics.OverlapSphereNonAlloc(ShootPos.position, detectionRadius, colliders, detectionLayer);

        for (int i = 0; i < count; i++)
        {

            MeshRenderer[] mrs = colliders[i].transform.GetComponentsInChildren<MeshRenderer>();
            foreach (var item in mrs)
            {
                item.materials = Managers.Mat.ChangeMaterials(item.transform.GetInstanceID(), item.materials, "Orange_Glow", "Orange_line");
                prevDetectObjects.Add(KeyValuePair.Create(item, item.transform.GetInstanceID()));
            }

        }

        //총 사거리에 딱맞는녀석 색넣기
        if (Physics.Raycast(ShootPos.position, ShootPos.forward, out RaycastHit hitInfo, detectionRadius, detectionLayer))
        {
            MeshRenderer[] mrs = hitInfo.transform.GetComponentsInChildren<MeshRenderer>();
            foreach (var item in mrs)
            {
                item.materials = Managers.Mat.ChangeMaterials(item.transform.GetInstanceID(), item.materials, "Green_Glow", "Green_line");
            }
        }
    }

    void ResetColor()
    {
        foreach (var item in prevDetectObjects)
        {
            if (item.Key != null)
                item.Key.materials = Managers.Mat.GetBaseMaterial(item.Value);
        }
        prevDetectObjects.Clear();
        if(target != null)
        {
            MeshRenderer[] mrs = target.transform.GetComponentsInChildren<MeshRenderer>();
            foreach (var item in mrs)
            {
                item.materials = Managers.Mat.GetBaseMaterial(item.transform.GetInstanceID());
            }
        }
        
    }
    public override void EndAction(System.Action<Define.PaintType> OnEnd = null)
    {
        if (target != null)
        {

            Vector3 forceDir = (bazierPos[2] - bazierPos[3]).normalized;
            float force = (bazierPos[2] - bazierPos[3]).magnitude;
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.AddForce(forceDir * force * 150f);
                rb = null;
            }
            
            offset = Vector3.zero;
        }
        ResetColor();
        target = null;
        auraParticle?.gameObject.SetActive(false);
        OnEnd?.Invoke(Define.PaintType.White);
    }
    public override void Init()
    {
        if (auraParticle == null)
        {
            auraParticle = Managers.Resource.Instantiate("Effect/GreenPaintAura");
        }

        auraParticle.gameObject.SetActive(true);
        detectionRadius = 5f;
    }
}
