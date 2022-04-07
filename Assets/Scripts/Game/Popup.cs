using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CodeMonkey.Utils;

public class Popup : MonoBehaviour
{
    private TextMeshPro _TextMesh;
    private float _DisapearTimer;
    private const float _DisappearTimerMax = 1f;
    private Color _TextColor;
    private Vector3 _MoveVector;
    private static int _SortingOrder;


    public static Popup Create(Vector3 pPosition, int pDamageAmount, bool pIsCriticalHit)
    {
        Transform _DamagePopupTransform = Instantiate(MyAssets.i.myDamagePopup, pPosition, Quaternion.identity);
        Popup _Popup = _DamagePopupTransform.GetComponent<Popup>();
        _Popup.Setup(pDamageAmount,pIsCriticalHit);

        return _Popup;
    }

    private void Awake()
    {
        _TextMesh = transform.GetComponent<TextMeshPro>();
    }

    public void Setup(int pDamageAmount, bool pIsCriticalHit)
    {
        _SortingOrder++;
        _TextMesh.sortingOrder = _SortingOrder;

        if (pDamageAmount < 1) pDamageAmount = 1;

        _TextMesh.SetText(pDamageAmount.ToString());

        if (pIsCriticalHit)
        {
            _TextMesh.fontSize = 45;
            _TextColor = UtilsClass.GetColorFromString("FF2B00");
        }
        else
        {
            _TextMesh.fontSize = 36;
            _TextColor = UtilsClass.GetColorFromString("FFC500");
        }
        _TextMesh.color = _TextColor;
        _DisapearTimer = _DisappearTimerMax;

        _MoveVector = new Vector3(.7f, 1) * 60f;
    }

    private void Update()
    {

        float _MoveYSpeed = 20f;
        float _DisappearSpeed = 3f;

        transform.position += _MoveVector * Time.deltaTime;
        _MoveVector -= _MoveVector * 8f * Time.deltaTime;

        if (_DisapearTimer > (_DisappearTimerMax*.5))
        {
            float _IncreaseScaleAmount = 1f;
            transform.localScale += Vector3.one * _IncreaseScaleAmount * Time.deltaTime;
        }
        else
        {
            float _DecreaseScaleAmount = 1f;
            transform.localScale -= Vector3.one * _DecreaseScaleAmount * Time.deltaTime;
        }

        _DisapearTimer -= Time.deltaTime;
        if (_DisapearTimer < 0)
        {
            _TextColor.a -= _DisappearSpeed * Time.deltaTime;
            _TextMesh.color = _TextColor;
        }
        if (_TextColor.a <0)
        {
            Destroy(gameObject);
        }

    }
}
