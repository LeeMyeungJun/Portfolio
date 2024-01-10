using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;

public class Brush : MonoBehaviour
{
    Paint paint = null;
    [SerializeField] MeshRenderer render;
    [SerializeField] Transform shootPos;
    [SerializeField] RayVisualizerEx ray;

    Paint[] paintRefDatas = new Paint[(int)Define.PaintType.END];
    Define.PaintType paintType = Define.PaintType.White;
    public Define.PaintType PaintType 
    { 
        get { return paintType; } 
        set
        {
            paint?.EndAction();
            paintType = value;
            paint = paintRefDatas[(int)value];
            brushMat.color = Define.ChangPaintTypeToColor(value);
            paint.Init();
            if(value != Define.PaintType.White)
                Managers.Sound.Play("SFX/GunReroadSound",this.transform.position);
            ray.ChangeRayColor(value);
        } 
    }
    Material brushMat;
    Vector2 tumbValue = Vector2.zero;

    public UnityEvent OnEnter = null;
    public UnityEvent OnExit = null;

    private void Awake()
    {
        for (int i = 0; i < (int)Define.PaintType.END; i++)
        {
            switch ((Define.PaintType)i)
            {
                case Define.PaintType.White:
                    paintRefDatas[i] = new WhitePaint();
                    break;
                case Define.PaintType.Red:
                    paintRefDatas[i] = new RedPaint();
                    break;
                case Define.PaintType.Green:
                    paintRefDatas[i] = new GreenPaint();
                    break;
                case Define.PaintType.Blue:
                    paintRefDatas[i] = new BluePaint();
                    break;
            }
        }
        brushMat = render.materials[0];
        PaintType = Define.PaintType.White;
        paint = paintRefDatas[(int)Define.PaintType.White];
    }

    public void SelectEnter(SelectEnterEventArgs eventArgs)
    {
        XRDirectInteractor interactor = eventArgs.interactorObject.transform.GetComponent<XRDirectInteractor>();
        if (interactor != null)
        {
            OnEnter?.Invoke();
        }
    }
    public void SelectExitEvent(SelectExitEventArgs eventArgs)
    {
        XRDirectInteractor interactor = eventArgs.interactorObject.transform.GetComponent<XRDirectInteractor>();
        if (interactor != null)
        {
            OnExit?.Invoke();
        }
    }

    public void Grab()
    {
        StopAllCoroutines();
        StartCoroutine(Action());
    }

    public void Release()
    {
        StopAllCoroutines();
        PaintType = Define.PaintType.White;
    }
    public void BeginAction()
    {
        paint.BeginAction(shootPos);
    }

    IEnumerator Action()
    {
        while (true)
        {
            paint.Action(shootPos, tumbValue ,(float WhiteDensity) => //WhiteDensity -> 1에가까울수록 하얀색빛이남
            {
                Color color = new Color(WhiteDensity, WhiteDensity, WhiteDensity) + Define.ChangPaintTypeToColor(PaintType);
                brushMat.color = color;
            },
            (Vector3[] bazierPos) => //GreenPaint사용
            {
                ray.SetBazierPositions(bazierPos);
            });
            yield return null;
        }
    }
    public void EndAction()
    {
        paint.EndAction((Define.PaintType type) => //WhiteDensity -> 1에가까울수록 하얀색빛이남
        {
            PaintType = type;
        }
        );
    }

    public void OnThumb(InputAction.CallbackContext context)
    {
        tumbValue = context.ReadValue<Vector2>();
    }
}
