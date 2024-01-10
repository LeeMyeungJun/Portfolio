using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PalettePaint : MonoBehaviour
{
    [SerializeField] Define.PaintType paintType;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] Material[] colorMats = new Material[3];

    public InputActionProperty changeColor;

    [SerializeField] bool isUseRed = false;
    [SerializeField] bool isUseGreen = false;
    [SerializeField] bool isUseBlue = false;

    public Define.PaintType GetPaintType() { return paintType; }

    private void OnEnable()
    {
        changeColor.action.performed -= OnChangeColor;
        changeColor.action.performed += OnChangeColor;
    }

    private void OnDisable()
    {
        changeColor.action.performed -= OnChangeColor;
    }

    private void Start()
    {
        SetMat();
    }

    public void OnChangeColor(InputAction.CallbackContext ctx)
    {
        Define.PaintType prevPaint = paintType;
        switch (paintType)
        {
            case Define.PaintType.White:
                if (isUseRed)
                    paintType = Define.PaintType.Red;
                break;
            case Define.PaintType.Red:
                if (isUseGreen)
                    paintType = Define.PaintType.Green;
                break;
            case Define.PaintType.Green:
                if (isUseBlue)
                    paintType = Define.PaintType.Blue;
                else
                    paintType = Define.PaintType.Red;
                break;
            case Define.PaintType.Blue:
                paintType = Define.PaintType.Red;
                break;
        }
        if (prevPaint == paintType)
            return;

        Managers.Sound.Play("SFX/ChangeColorSound", this.transform.position);
        SetMat();
    }
    public void OnChangeColor()
    {
        Define.PaintType prevPaint = paintType;
        switch (paintType)
        {
            case Define.PaintType.White:
                if (isUseRed)
                    paintType = Define.PaintType.Red;
                break;
            case Define.PaintType.Red:
                if (isUseGreen)
                    paintType = Define.PaintType.Green;
                break;
            case Define.PaintType.Green:
                if (isUseBlue)
                    paintType = Define.PaintType.Blue;
                else
                    paintType = Define.PaintType.Red;
                break;
            case Define.PaintType.Blue:
                paintType = Define.PaintType.Red;
                break;
        }
        if (prevPaint == paintType)
            return;

        Managers.Sound.Play("SFX/ChangeColorSound", this.transform.position);
        SetMat();
    }
    public void SetUsePaint(int index)
    {
        Managers.Sound.Play("SFX/GetItem", this.transform.position);
        switch ((Define.PaintType)index)
        {
            case Define.PaintType.Red:
                paintType = Define.PaintType.Red;
                isUseRed = true;
                break;
            case Define.PaintType.Green:
                paintType = Define.PaintType.Green;
                isUseGreen = true;
                break;
            case Define.PaintType.Blue:
                paintType = Define.PaintType.Blue;
                isUseBlue = true;
                break;
        }

        SetMat();
    }

    void SetMat()
    {
        if (paintType == Define.PaintType.END)
            paintType = Define.PaintType.Red;
        meshRenderer.gameObject.SetActive(true);
        switch (paintType)
        {
            case Define.PaintType.White:
                meshRenderer.gameObject.SetActive(false);
                break;
            case Define.PaintType.Red:
                meshRenderer.material = colorMats[0];
                break;
            case Define.PaintType.Green:
                meshRenderer.material = colorMats[1];
                break;
            case Define.PaintType.Blue:
                meshRenderer.material = colorMats[2];
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Brush _val = other.GetComponent<Brush>();
        if (_val)
        {
            _val.PaintType = paintType;
        }
    }
}
