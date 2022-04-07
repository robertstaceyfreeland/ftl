using System;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    //Core Objects
    public Player _Player;
    public HealthSystem _HealthSystem;
    private Rigidbody2D _PlayerRigidbody;
    public Transform _PlayerAimRotationTransform;
    public Transform _PlayerWeaponSpriteTransform;
    private MeshAnimator _PlayerMeshAnimator;
    public AudioSource _AudioSource_PlayerMovement;

    public LayerMask _LayerMask;
    private bool _IsDash = false;
    private float _DashTimer = 0;
    private float _MovementSpeed = 70f;
    Vector3 _LastMoveDirection;
    private bool _IsRecovered = true;
    private float _NextStepTime = 0;


    private void Start() 
    {
        _Player = GetComponent<Player>();
        _HealthSystem = _Player.GetHealthSystem();
        _PlayerRigidbody = GetComponentInParent<Rigidbody2D>();
        _PlayerMeshAnimator = GetComponent<MeshAnimator>();
        _AudioSource_PlayerMovement = GetComponent<AudioSource>();

        _HealthSystem.OnStaminaExhausted += HealthSystem_OnStaminaExhausted;
        _HealthSystem.OnStaminaChanged += HealthSystem_OnStaminaChanged;
        _HealthSystem.OnStaminaRecovered += HealthSystem_OnStaminaRecovered;
    }

    void Update()
    {
        if (_IsDash)
        {
            _MovementSpeed = 210;
            _DashTimer -= Time.deltaTime;
        }
        else
        {
            _MovementSpeed = 70;
        }

        if (_DashTimer <=0)
        {
            _IsDash = false;
            _DashTimer = 1;
        }
        
        
        HandleAim();

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (_HealthSystem.GetStaminaAmount() >= 20)
            {
                if (_IsDash) return;

                _IsDash = true;
                _DashTimer = .2f;
                _Player._HealthSystem.SubtractStamina(20);
                
            }
        }
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        float _MoveX = 0;
        float _MoveY = 0;

        Vector3 _MoveDirection;

        if (Input.GetKey(KeyCode.W))
        {
            _MoveY = 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            _MoveY = -1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            _MoveX = 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            _MoveX = -1;
        }

        _MoveDirection = (new Vector3(_MoveX, _MoveY));
        _MoveDirection.Normalize();

        if (_MoveX != 0 || _MoveY != 0) //(If Not Standing)
        {
            _PlayerMeshAnimator._PlayerState = MeshAnimator.PlayerState.Walk;
            _PlayerMeshAnimator._CurrentMovementMagnitude = Mathf.Max(Mathf.Abs(_MoveX), Mathf.Abs(_MoveY));
            
            _LastMoveDirection = _MoveDirection;

            //Save This
            //transform.position += _MoveDirection * _MovementSpeed * Time.deltaTime;

            _PlayerRigidbody.position += (Vector2)(_MoveDirection * _MovementSpeed * Time.fixedDeltaTime);

            //SoundManager.PlaySound(SoundManager.Sound.Human_Movement,transform.position, .1f);

            if (_NextStepTime < Time.time)
            {
                _AudioSource_PlayerMovement.Play();
                _NextStepTime = Time.time + .1f;
            }
        }
        else
        {
            _PlayerMeshAnimator._PlayerState = MeshAnimator.PlayerState.Idle;
            _PlayerMeshAnimator._CurrentMovementMagnitude = 0;
        }
    }

    private void HandleAim()
    {

        Vector2 _TargetDirection;
        Vector3 m_Velocity = Vector3.zero;

        //Handles Rotation
        _TargetDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float _TargetAngle = (Mathf.Atan2(_TargetDirection.y, _TargetDirection.x) * Mathf.Rad2Deg);
        Quaternion rotation = Quaternion.AngleAxis(_TargetAngle, Vector3.forward);
        _PlayerAimRotationTransform.rotation = rotation;

        //Handles Mesh Switching
        if (_TargetAngle <= 45 && _TargetAngle > -45) _PlayerMeshAnimator._PlayerDirection = MeshAnimator.PlayerDirection.Right;
        if (_TargetAngle <= 135 && _TargetAngle > 45) _PlayerMeshAnimator._PlayerDirection = MeshAnimator.PlayerDirection.Up;
        if (_TargetAngle <= 180 && _TargetAngle > 135) _PlayerMeshAnimator._PlayerDirection = MeshAnimator.PlayerDirection.Left;
        if (_TargetAngle < -135 && _TargetAngle >= -180) _PlayerMeshAnimator._PlayerDirection = MeshAnimator.PlayerDirection.Left;
        if (_TargetAngle < -45 && _TargetAngle >= -135) _PlayerMeshAnimator._PlayerDirection = MeshAnimator.PlayerDirection.Down;

        //Flips weapon when it crosses left/right boundry
        Vector3 localScale = Vector3.one;

        if (_TargetAngle > 90 || _TargetAngle < -90)
        {
            localScale.y = -1f;
        }
        else
        {
            localScale.y = 1f;
        }

        if (_PlayerWeaponSpriteTransform) _PlayerWeaponSpriteTransform.localScale = localScale;
    }

    //Events
    private void HealthSystem_OnStaminaChanged(object sender, EventArgs e)
    {
        //throw new NotImplementedException();
    }
    private void HealthSystem_OnStaminaRecovered(object sender, EventArgs e)
    {
        _IsRecovered = true;
    }
    private void HealthSystem_OnStaminaExhausted(object sender, EventArgs e)
    {
        _IsRecovered = false;
    }
}
