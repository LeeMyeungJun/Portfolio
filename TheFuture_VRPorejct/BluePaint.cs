using System.IO.Compression;
using UnityEngine;

public class BluePaint : Paint
{
    Zipline zipline = null;
    Zipline guideZipline = null;

    LayerMask environmentMask = LayerMask.GetMask("Environment", "InteractBlue");
    LayerMask detectionLayer = LayerMask.GetMask("InteractBlue");

    Transform camTrs;
    public override void BeginAction(Transform ShootPos)
    {
        if (Physics.Raycast(camTrs.position,Vector3.down, out RaycastHit downHit, 3f, environmentMask) &&
            Physics.Raycast(ShootPos.position, ShootPos.forward, out RaycastHit hit, detectionRadius, detectionLayer))
        {
            if (hit.transform.TryGetComponent<BluePaintObject>(out BluePaintObject Bpo) == false) return;

            if (zipline == null)
                zipline = Managers.Resource.Instantiate("ZiplineObject").GetComponent<Zipline>();

            Managers.Sound.Play(Sound.BluePaintInteract, ShootPos.position);

            Vector3 pos = downHit.point;
            if (downHit.transform.TryGetComponent<BluePaintObject>(out BluePaintObject downBpo)) 
                zipline.transform.position = downBpo.GetSpawnPos();
            else
            {
                Vector3 gunDir = new Vector3(ShootPos.forward.x + 0.5f, 0, ShootPos.forward.z - 0.5f);
                zipline.transform.position = pos + gunDir*0.5f;
            }

            Vector3 targetDir = (Bpo.GetSpawnPos() - pos).normalized;
            float angle = Vector3.Angle(Vector3.forward, targetDir);
            float signedAngle = Mathf.Sign(Vector3.Dot(Vector3.Cross(Vector3.forward, targetDir), Vector3.up)) * angle; // 외내적 다 왼손 좌표계 따라감
            zipline.transform.rotation = Quaternion.AngleAxis(signedAngle, Vector3.up);

            zipline.SetOtherZiplinePosition(Bpo.GetSpawnPos());
        }
    }

    public override void Action(Transform ShootPos, Vector2 tumbValue, System.Action<float> OnColor = null, System.Action<Vector3[]> OnPosition = null)
    {
        if (guideZipline == null)
            guideZipline = Managers.Resource.Instantiate("GuideZiplineObject").GetComponent<Zipline>();//1

        if (guideZipline.gameObject.activeSelf == false)
            guideZipline.gameObject.SetActive(true);

        //Guide
        if (Physics.Raycast(camTrs.position, Vector3.down, out RaycastHit downHit, 3f, environmentMask) &&
            Physics.Raycast(ShootPos.position, ShootPos.forward, out RaycastHit hit, detectionRadius, detectionLayer))
        {
            if (hit.transform.TryGetComponent<BluePaintObject>(out BluePaintObject Bpo) == false) return;

            Vector3 pos = downHit.point;
            if (downHit.transform.TryGetComponent<BluePaintObject>(out BluePaintObject downBpo))
                guideZipline.transform.position = downBpo.GetSpawnPos();
            else
            {
                Vector3 gunDir = new Vector3(ShootPos.forward.x + 0.5f, 0, ShootPos.forward.z - 0.5f);
                guideZipline.transform.position = pos + gunDir;
            }

            Vector3 targetDir = (Bpo.GetSpawnPos() - pos).normalized;
            float angle = Vector3.Angle(Vector3.forward, targetDir);
            float signedAngle = Mathf.Sign(Vector3.Dot(Vector3.Cross(Vector3.forward, targetDir), Vector3.up)) * angle;
            guideZipline.transform.rotation = Quaternion.AngleAxis(signedAngle, Vector3.up);

            guideZipline.SetOtherZiplinePosition(Bpo.GetSpawnPos());
        }
        else
        {
            guideZipline.gameObject.SetActive(false);
        }
    }

    public override void EndAction(System.Action<Define.PaintType> OnEnd = null)
    {
        if (guideZipline != null)
            guideZipline.gameObject.SetActive(false);

        OnEnd?.Invoke(Define.PaintType.White);
    }

    public override void Init() 
    {
        if (camTrs == null)
            camTrs = Camera.main.transform;

        detectionRadius = 30f;
    }
}
