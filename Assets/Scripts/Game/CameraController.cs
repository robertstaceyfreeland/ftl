using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    bool _ShowDebug = true;

    [SerializeField]
    private Transform _Target;
    [SerializeField]
    private float _TracingSpeed = .05f;
    [SerializeField]
    private float _MouseLookSpeed = .1f;
    [SerializeField]
    private float _PanRange = 300;
    [SerializeField]
    private float _PanAcceleration = 3;
    [SerializeField] 
    private float _CameraDistanceMax = 250f;
    [SerializeField] 
    private float _CameraDistanceMin = 100f;
    [SerializeField] 
    private float _CameraDistance = 150f;
    [SerializeField] 
    private float _ScrollSpeed = 25f;

    private float _ZConstant = -100f;

    private void Awake()
    {

    }

    void Update()
    {
        _CameraDistance -= Input.GetAxis("Mouse ScrollWheel") * _ScrollSpeed;
        _CameraDistance = Mathf.Clamp(_CameraDistance, _CameraDistanceMin, _CameraDistanceMax);

        // set camera position
        Camera.main.orthographicSize = _CameraDistance;

        ShootRay();

    }

    void ShootRay()
    {
        
        Door myDoor;

        if (Input.GetMouseButtonDown(1))
        {
            //if (_ShowDebug) Debug.Log("ShootRay!");

            RaycastHit hit;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray,out hit, 10000f))
            {
                if (_ShowDebug) Debug.Log("Ray Hit: " + hit.transform.name);

                if (hit.transform != null)
                {
                    myDoor = hit.transform.GetComponentInChildren<Door>();

                    if (_ShowDebug) Debug.Log("My Door Name: " + myDoor.name);
                    myDoor.CloseDoor();
                }
            }        

        }
    }

    void FixedUpdate()
    {
        Vector2 _MousePosition;
        
        _MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        float _AdustX = Mathf.Clamp(((_MousePosition.x - _Target.position.x)* _PanAcceleration),-_PanRange,_PanRange);
        float _AdustY = Mathf.Clamp(((_MousePosition.y - _Target.position.y)* _PanAcceleration), -_PanRange, _PanRange);

        Vector3 _CurrentCameraPosition = transform.position;
        Vector3 _DesiredCameraPosition = new Vector3(_Target.position.x + Mathf.Lerp(0,_AdustX, _MouseLookSpeed), _Target.position.y + Mathf.Lerp(0, _AdustY, _MouseLookSpeed), _ZConstant);
        Vector3 _StepCameraPosition = new Vector3(Mathf.Lerp(_CurrentCameraPosition.x, _DesiredCameraPosition.x, _TracingSpeed), Mathf.Lerp(_CurrentCameraPosition.y, _DesiredCameraPosition.y, _TracingSpeed), _ZConstant);

        transform.position = _StepCameraPosition;
    }
}
