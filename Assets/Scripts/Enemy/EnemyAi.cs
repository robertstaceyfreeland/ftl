using UnityEngine;
using Pathfinding;
using System.Collections;

public class EnemyAi : MonoBehaviour
{
    public enum EnemyBulletType {Bullet_Alien_LaserPistol, Bullet_Alien_LaserRifle, Bullet_Alien_Claw}
    public enum EnemyAiState { Idle, ChasePlayer, AttackPlayer, ChaseHelm, AttackHelm };
    
    public EnemyBulletType _EnemyBulletType = EnemyBulletType.Bullet_Alien_LaserPistol;
    public EnemyAiState _CurrentEnemyAiState = EnemyAiState.ChasePlayer;

    public Transform _Muzzle;
    public Transform _Target;
    private MeshAnimator _PlayerMeshAnimator;
    private Rigidbody2D _Rigidbody2D;
    
    private Path _Path;
    private Seeker _Seeker;
    private int _CurrentWaypoint;
    private bool _EndOfPath = false;
    
    public float _RangeToTarget;
    public Transform _EnemyAimRotationTransform;
    public Transform _EnemyWeaponTransform;


    [SerializeField] private float _MoveSpeed = 75f;
    [SerializeField] private float _BulletLifespan = .1f;
    [SerializeField] private float _RateOfFire = .2f;
    [SerializeField] private float _StartAimingRange = 100;
    [SerializeField] private float _StopAimingRange = 150;
    [SerializeField] private float _NextWaypointDistance = 3f;
    [SerializeField] private LayerMask _LayerMask;

    private float NextFireTime = 0;
    ObjectPooler _ObjectPooler;

    void Start()
    {
        _Target = GameObject.FindObjectOfType<Player>().transform;
        
        _Seeker = GetComponent<Seeker>();
        _Rigidbody2D = GetComponent<Rigidbody2D>();
        _PlayerMeshAnimator = GetComponent<MeshAnimator>();

        _ObjectPooler = ObjectPooler.Instance;

        InvokeRepeating("UpdatePath", 0f, .5f);
    }

    void UpdatePath()
    {
        if (_Target == null) return;

        if (_Seeker.IsDone())
        {
            _Seeker.StartPath(_Rigidbody2D.position, _Target.position, OnPathComplete);
        }
    }

    public void EnemyAiStateMachine(EnemyAiState pAiState)
    {
        switch (pAiState)
        {
            case EnemyAiState.Idle:
                _CurrentEnemyAiState = EnemyAiState.Idle;
                break;
            case EnemyAiState.ChasePlayer:
                _CurrentEnemyAiState = EnemyAiState.ChasePlayer;
                break;
            case EnemyAiState.AttackPlayer:
                _CurrentEnemyAiState = EnemyAiState.AttackPlayer;
                break;
            case EnemyAiState.AttackHelm:
                _CurrentEnemyAiState = EnemyAiState.AttackHelm;
                break;
            default:
                break;
        }
    }

    void FixedUpdate()
    {
        //_PlayerMeshAnimator._PlayerState = MeshAnimator.PlayerState.Walk;
        //_PlayerMeshAnimator._CurrentMovementMagnitude = 1;

        // vectorPath[] is a list of Vector3's making up the waypoints
        // _CurrentWaypoint is the index used with vectorPath[]
        // _EndOfPath is a simple bool indicating that the object is at the end of the path
        // _TargetsVector2 is the vector to the Target
        // _ThisVector2 is "This" object location
        // _RangeToTarget is the distance between this object and the target
        // _Path is an ASTAR object it holds the List<vectorPath> and other properties associated with the path

        // (Vector2)_Path.vectorPath[_CurrentWaypoint] is simple a waypoint
        // _Direction is the direction (vector) to the next waypoint
        // _Distance is the distance to the next waypoint
        // _Rotation provides the rotatation that can be applied to an objects transform allowing it to rotate

        Vector2 _DirectionToCurrentWaypoint;
        Vector2 _DirectionToTarget;
        Vector2 _TargetsPosition;
        Vector2 _ThisPosition;
        float _DistanceToCurrentWaypoint;
        //float _CurrentWaypointRotationAngle;

        if (_Target == null) return;
        
        //Store Positions (Target and THIS)
        _TargetsPosition = new Vector2(_Target.transform.position.x, _Target.transform.position.y);
        _ThisPosition = new Vector2(transform.position.x, transform.position.y);

        //Direction to Target
        _DirectionToTarget = (_TargetsPosition - _Rigidbody2D.position).normalized;

        //Calculates Range to Target
        _RangeToTarget = Vector2.Distance(_TargetsPosition, _ThisPosition);

        if (_CurrentEnemyAiState == EnemyAiState.AttackPlayer)
        {
            //See if we have line of sight
            RaycastHit2D _RayHitInfo = Physics2D.Raycast(_Muzzle.position, _DirectionToTarget, 500, _LayerMask);
            
            Debug.DrawRay(_Muzzle.position, _DirectionToTarget,Color.red);

            
            if (_RayHitInfo)
            {
                if (_RayHitInfo.transform.CompareTag("Player"))
                {
                    _Rigidbody2D.velocity = Vector3.zero;

                    Aim();
                    Attack();
                    _PlayerMeshAnimator._CurrentMovementMagnitude = 0;
                    _PlayerMeshAnimator._PlayerState = MeshAnimator.PlayerState.Idle;

                    if (_RangeToTarget >= _StopAimingRange) _CurrentEnemyAiState = EnemyAiState.ChasePlayer;
                }
                else
                {
                    _CurrentEnemyAiState = EnemyAiState.ChasePlayer;
                }
            }
            else
            {
                _CurrentEnemyAiState = EnemyAiState.ChasePlayer;
            }

        }


        if (_Path == null) return;

            if (_CurrentWaypoint >= _Path.vectorPath.Count)
            {
                _EndOfPath = true;
                return;
            }
            else
            {
                _EndOfPath = false;
            }
        

        if (_CurrentEnemyAiState == EnemyAiState.ChasePlayer)
        {
            //TODO: Fix this
            //SoundManager.PlaySound(SoundManager.Sound.Alien_Movement, transform.position,.1f);

            _PlayerMeshAnimator._CurrentMovementMagnitude = 5;
            _PlayerMeshAnimator._PlayerState = MeshAnimator.PlayerState.Walk;

            if (_RangeToTarget <= _StartAimingRange) _CurrentEnemyAiState = EnemyAiState.AttackPlayer;

            //Calculates Direction to Current Waypoint
            
            _DirectionToCurrentWaypoint = ((Vector2)_Path.vectorPath[_CurrentWaypoint] - _Rigidbody2D.position).normalized;
            _DistanceToCurrentWaypoint = Vector2.Distance(_Rigidbody2D.position, _Path.vectorPath[_CurrentWaypoint]);

            //Increments the waypoint if the end is reached
            if (_DistanceToCurrentWaypoint < _NextWaypointDistance) _CurrentWaypoint++;
                
            _Rigidbody2D.velocity = _DirectionToCurrentWaypoint * _MoveSpeed;
            
            Aim();
        }
    }

    private void Aim()
    {
        Vector2 _DirectionToTarget;
        Vector2 _TargetsPosition;
        Quaternion _RotationTowardsTarget;
        float _TargetAngle;

        _TargetsPosition = new Vector2(_Target.transform.position.x, _Target.transform.position.y);

        //Direction to Target
        _DirectionToTarget = (_TargetsPosition - _Rigidbody2D.position).normalized;

        //Rotates Gun Towards Target
        _TargetAngle = Mathf.Atan2(_DirectionToTarget.y, _DirectionToTarget.x) * Mathf.Rad2Deg;
        _RotationTowardsTarget = Quaternion.AngleAxis(_TargetAngle, Vector3.forward);
        _EnemyAimRotationTransform.rotation = _RotationTowardsTarget;

        if (_TargetAngle <= 45 && _TargetAngle > -45) _PlayerMeshAnimator._PlayerDirection = MeshAnimator.PlayerDirection.Right;
        if (_TargetAngle <= 135 && _TargetAngle > 45) _PlayerMeshAnimator._PlayerDirection = MeshAnimator.PlayerDirection.Up;
        if (_TargetAngle <= 180 && _TargetAngle > 135) _PlayerMeshAnimator._PlayerDirection = MeshAnimator.PlayerDirection.Left;
        if (_TargetAngle < -135 && _TargetAngle >= -180) _PlayerMeshAnimator._PlayerDirection = MeshAnimator.PlayerDirection.Left;
        if (_TargetAngle < -45 && _TargetAngle >= -135) _PlayerMeshAnimator._PlayerDirection = MeshAnimator.PlayerDirection.Down;

        Vector3 localScale = Vector3.one;
        if (_TargetAngle > 90 || _TargetAngle < -90)
        {
            localScale.y = -1f;
        }
        else
        {
            localScale.y = 1f;
        }

        _EnemyWeaponTransform.localScale = localScale;
    }

    private void OnPathComplete(Path pPath)
    {
        if (!pPath.error)
        {
            _Path = pPath;
            _CurrentWaypoint = 0;
        }
    }

    private void Attack()
    {
        if (NextFireTime <= Time.time)
        {
            GameObject _Bullet = _ObjectPooler.SpawnFromPool(_EnemyBulletType.ToString(), _Muzzle.position, _Muzzle.rotation);

            DisableAfterDelay(_Bullet, _BulletLifespan);

            NextFireTime = Time.time +_RateOfFire;
        }
    }

    private IEnumerator DisableAfterDelay(GameObject pObject, float pValue)
    {
        yield return new WaitForSeconds(pValue);

        pObject.SetActive(false);
    }

}
