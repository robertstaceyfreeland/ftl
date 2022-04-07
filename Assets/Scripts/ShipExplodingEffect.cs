using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UIElements;

public class ShipExplodingEffect : MonoBehaviour
{
    [SerializeField] private UnityEngine.Experimental.Rendering.Universal.Light2D _Light2D;
    private bool _IncreaseIntesity =true;
    float _Intensity = 0;

    // Start is called before the first frame update
    void Start()
    {
        _Light2D.intensity = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (_IncreaseIntesity)
        {
            _Light2D.intensity = _Intensity += (1f * Time.deltaTime);
            if (_Intensity >= 1) _IncreaseIntesity = false;

        }
        else
        {
            if (_Intensity>0) _Light2D.intensity = _Intensity -= (1f * Time.deltaTime);
        }
        
    }
}
