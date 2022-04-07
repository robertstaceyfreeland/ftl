using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerWeapons : MonoBehaviour
{
    private PlayerMovement _PlayerMovement;
    public Weapon _CurrentWeapon;
    public GameObject _WeaponRig;
    public Weapon[] _PlayerWeaponsStash;
    private int _CurrentWeaponIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        _PlayerMovement = GetComponent<PlayerMovement>();
        
        GetNewWeapon(0);
       
    }

    // Update is called once per frame
    void Update()
    {
        SwitchWeapon();

        if (Input.GetMouseButton(0) && !IsMouseOverUi())
        {
            _CurrentWeapon.Fire();
        }
    }

    private void SwitchWeapon() //Player Input for Switching Weapons
    {

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            int _WeaponIndex = 0;

            if (_CurrentWeaponIndex == _WeaponIndex) 
            {
                return;
            }

            GetNewWeapon(_WeaponIndex);

            _CurrentWeaponIndex = _WeaponIndex;
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            int _WeaponIndex = 1;

            if (_CurrentWeaponIndex == _WeaponIndex)
            {
                return;
            }

            GetNewWeapon(_WeaponIndex);

            _CurrentWeaponIndex = _WeaponIndex;
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            int _WeaponIndex = 2;

            if (_CurrentWeaponIndex == _WeaponIndex)
            {
                return;
            }

            GetNewWeapon(_WeaponIndex);

            _CurrentWeaponIndex = _WeaponIndex;
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            int _WeaponIndex = 3;


            if (_CurrentWeaponIndex == _WeaponIndex)
            {
                return;
            }

            GetNewWeapon(_WeaponIndex);

            _CurrentWeaponIndex = _WeaponIndex;
            return;
        }
    }

    private void GetNewWeapon(int pWeaponStorageIndex)
    {
        try
        {
            _CurrentWeapon._SpriteRenderer.enabled = false;
        }
        catch { }

        _CurrentWeapon = _PlayerWeaponsStash[pWeaponStorageIndex];

        _CurrentWeapon._SpriteRenderer.enabled = true;

        _PlayerMovement._PlayerWeaponSpriteTransform = _CurrentWeapon._WeaponSpriteImage.transform;
    }

    private bool IsMouseOverUi() //Checks to see if mouse is over GUI
    {
        try
        {
            return EventSystem.current.IsPointerOverGameObject();
        }
        catch { return false; }
    }
}
