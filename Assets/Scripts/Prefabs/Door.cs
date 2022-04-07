using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Door : MonoBehaviour, IDoorControl
{
    bool _ShowDebug = false;

    public Oxygen _RoomIn;
    public Oxygen _RoomOut;
    //public DoorSystem _DoorSystem;
    private ShipSystem _DoorSystem;

    [SerializeField]
    
    private BoxCollider2D _BoxCollider;
    private Animator _Animator;
    private String _OpenDoorVariable = "OpenDoor";
    private String _SoundClipName = "SlidingDoor_01";
    private AudioSource _AudioSource;
    public Button _LockDoorOpenButton;
    public float _OxygenFlowThroughRate = .15f;

    //Door Operation Variables
    private float _BaseSystemResponseTime = 0;
    //private float _ModifiedSystemResponseTime = 0;
    private float _NextCloseDoorTime = 0;
    private float _NextOpenDoorTime = 0;
    private float _NextCycleDelayTime = 0;
    private float _AutoCloseDoorTime = 4;
    private float _DoorCycleDelay = 0.6f;
    public bool _IsOpen = false;
    public bool _IsLockedOpen = false;

    private void Start()
    {
        _Animator = GetComponent<Animator>();
        _BoxCollider = GetComponent<BoxCollider2D>();
        _AudioSource = GetComponent<AudioSource>();

        _DoorSystem = GameObject.Find("Helm_Door").GetComponent<ShipSystem>();

        ChangeButtonColor(Color.green);

        //Start with Doors Closed
        _Animator.SetBool(_OpenDoorVariable, false);
        _BoxCollider.enabled = true;
    }

    private void Update()
    {
        OxygenCheck();
        CloseDoor();
    }

    private float GetPlayerDoorResponseTime()
    {
        float _ModifiedPlayerDoorResponseTime;

        _ModifiedPlayerDoorResponseTime = _BaseSystemResponseTime;
        _ModifiedPlayerDoorResponseTime = _ModifiedPlayerDoorResponseTime + _DoorSystem.GetHelmModifier(ShipSystem.ModifierType.PlayerDoorResponseTime);
        _ModifiedPlayerDoorResponseTime = _ModifiedPlayerDoorResponseTime + _DoorSystem.GetSystemModifier(ShipSystem.ModifierType.PlayerDoorResponseTime);

        return _ModifiedPlayerDoorResponseTime;
    }

    private float GetEnemyDoorResponseTime()
    {
        float _ModifiedEnemyDoorResponseTime;

        _ModifiedEnemyDoorResponseTime = _BaseSystemResponseTime;
        
        //Door - Response
        _ModifiedEnemyDoorResponseTime = _ModifiedEnemyDoorResponseTime + _DoorSystem.GetHelmModifier(ShipSystem.ModifierType.PlayerDoorResponseTime);
        _ModifiedEnemyDoorResponseTime = _ModifiedEnemyDoorResponseTime + _DoorSystem.GetSystemModifier(ShipSystem.ModifierType.PlayerDoorResponseTime);

        //Door - Lock
        _ModifiedEnemyDoorResponseTime = _ModifiedEnemyDoorResponseTime + _DoorSystem.GetHelmModifier(ShipSystem.ModifierType.DoorLockTime);
        _ModifiedEnemyDoorResponseTime = _ModifiedEnemyDoorResponseTime + _DoorSystem.GetSystemModifier(ShipSystem.ModifierType.DoorLockTime);


        return _ModifiedEnemyDoorResponseTime;
    }

    private void OxygenCheck()
    {
        //if (_ShowDebug) Debug.Log("Door Check! Is Open = " + _IsOpen);

        float _RoomACurrentOxygen = 0;
        float _RoomBCurrentOxygen = 0;
        float _FlowRate = 0;
        
        if (_IsOpen)
        {
            _RoomACurrentOxygen = 0;
            _RoomBCurrentOxygen = 0;

            if (_RoomIn != null) _RoomACurrentOxygen = (_RoomIn.GetCurrentOxygenAmount()); 
            if (_RoomOut != null) _RoomBCurrentOxygen = (_RoomOut.GetCurrentOxygenAmount()); 

            _FlowRate = (Mathf.Abs(_RoomACurrentOxygen - _RoomBCurrentOxygen) * _OxygenFlowThroughRate * Time.deltaTime); 

            if (_RoomACurrentOxygen > _RoomBCurrentOxygen)
            {
                if (_RoomIn != null) _RoomIn.SubtractOxygen(_FlowRate);
                if (_RoomOut != null) _RoomOut.AddOxygen(_FlowRate);
            }
            else
            {
                if (_RoomOut != null) _RoomOut.SubtractOxygen(_FlowRate);
                if (_RoomIn != null) _RoomIn.AddOxygen(_FlowRate);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (_NextOpenDoorTime==0) _NextOpenDoorTime = Time.time + GetPlayerDoorResponseTime();
            OpenDoor();
        }

        if (collision.CompareTag("Enemy"))
        {
            if (_NextOpenDoorTime == 0) _NextOpenDoorTime = Time.time + GetEnemyDoorResponseTime();
            
            if (_NextOpenDoorTime - Time.time > 10) 
            {
                Achievements.Instance.DoorHeldByPlayer();
            }

            OpenDoor();
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _NextOpenDoorTime = 0;
    }

    public void OpenDoor()
    {
        if (_NextCycleDelayTime > Time.time) return;

        if (_IsOpen)
        {
            _NextCloseDoorTime = Time.time + _AutoCloseDoorTime;
            return;
        }
        
        if (_NextOpenDoorTime < Time.time)
        {
            //Open Door
            _Animator.speed = 1.25f;
            _Animator.SetBool(_OpenDoorVariable, true);
            _BoxCollider.enabled = false;
            _AudioSource.Play();
            _IsOpen = true;

            _NextCloseDoorTime = Time.time + _AutoCloseDoorTime;
            _NextOpenDoorTime = 0;
        }
    }

    public void CloseDoor()
    {
        if (_IsLockedOpen) return;
        if (!_IsOpen) return;

        if (_NextCloseDoorTime < Time.time)
        {
            _NextCycleDelayTime = Time.time + _DoorCycleDelay; 
            
            _Animator.SetBool(_OpenDoorVariable, false);
            _BoxCollider.enabled = true;
            _AudioSource.Play();
            _IsOpen = false;
        }
    }

    public void CloseAllDoors()
    {
        _IsLockedOpen = false;
        CloseDoor();
        ChangeButtonColor(Color.green);
    }

    public void LockOpenAllDoors()
    {
        if (_IsOpen)
        {
            _IsLockedOpen = true;
            ChangeButtonColor(Color.red);
        }
        else
        {
            //Open Door
            _Animator.speed = 1.25f;
            _Animator.SetBool(_OpenDoorVariable, true);
            _BoxCollider.enabled = false;
            _AudioSource.Play();
            _IsOpen = true;

            _NextCloseDoorTime = Time.time + _AutoCloseDoorTime;
            _NextOpenDoorTime = 0;

            _IsLockedOpen = true;
            ChangeButtonColor(Color.red);
        }
        
    }

    public void ToggleDoor()
    {

        if (_IsOpen)
        {
            CloseDoor();
        }
        else
        {
            OpenDoor();
        }
    }

    private void ChangeButtonColor(Color _Color)
    {
        ColorBlock _ButtonColors = _LockDoorOpenButton.colors;
        _ButtonColors.normalColor = _Color;
        _ButtonColors.highlightedColor = _Color;
        _ButtonColors.selectedColor = _Color;
        _ButtonColors.pressedColor = Color.yellow;
        _LockDoorOpenButton.colors = _ButtonColors;
    }

    public void ToggleDoorLockOpen()
    {
        if (_IsLockedOpen)
        {
            _IsLockedOpen = false;
            CloseDoor();
            ChangeButtonColor(Color.green);
        }
        else
        {
            OpenDoor();
            _IsLockedOpen = true;
            ChangeButtonColor(Color.red);
        }
    }
}
