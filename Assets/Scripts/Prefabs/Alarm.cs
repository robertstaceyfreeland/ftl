using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Alarm : MonoBehaviour
{

    [SerializeField] private UnityEngine.Experimental.Rendering.Universal.Light2D _Light2D;
    float _NextCycleTime = 0;
    float _LightStartOffset = 0;
    float _LightCycleOnOffset = 0;
    float _LightCycleOffOffset = 0;
    private PlayerShipHandler _PlayerShipHandler;
    


    // Start is called before the first frame update
    void Start()
    {
        _LightStartOffset =  UnityEngine.Random.Range(0, 3);
        _LightCycleOnOffset = UnityEngine.Random.Range(0, .5f);
        _LightCycleOffOffset = UnityEngine.Random.Range(0, .2f);
        _NextCycleTime = Time.time + _LightStartOffset;

        _PlayerShipHandler = GameObject.FindObjectOfType<PlayerShipHandler>();


        
    }

    // Update is called once per frame
    void Update()
    {
        if(_PlayerShipHandler._EnemyShipAlertLevel == PlayerShipHandler.EnemyShipAlertLevel.Red)
        {
            if (_NextCycleTime < Time.time)
            {
                if (_Light2D.enabled == true)
                {

                    _Light2D.color = Color.red;
                    _Light2D.enabled = false;
                    _NextCycleTime = Time.time + 1 + _LightCycleOffOffset;
                }
                else
                {
                    //Debug.Log("Turning Light On");
                    _Light2D.enabled = true;
                    _NextCycleTime = Time.time + 3 + _LightCycleOnOffset;
                }
            }
        }
        else if(_PlayerShipHandler._EnemyShipAlertLevel == PlayerShipHandler.EnemyShipAlertLevel.Yellow)
        {
            if (_NextCycleTime < Time.time)
            {
                if (_Light2D.enabled == true)
                {
                    _Light2D.color = Color.yellow;
                    _Light2D.enabled = false;
                    _NextCycleTime = Time.time + 1 + _LightCycleOffOffset;
                }
                else
                {
                    _Light2D.enabled = true;
                    _NextCycleTime = Time.time + 3 + _LightCycleOnOffset;
                }
            }
        }
        else
        {
            //TODO: This Needs to be reviewed (I took out a light but not sure it was being used)
            //_Light2D.enabled = false;
        }

        
    }
}
