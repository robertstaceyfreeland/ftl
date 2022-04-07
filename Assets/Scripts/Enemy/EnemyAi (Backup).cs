using UnityEngine;
using Pathfinding;

public class EnemyAi_Backup : MonoBehaviour
{
    public Transform _Muzzle;
    public GameObject _ProjectilePrefab;

    [SerializeField] private float _RotationSpeed = 1f;
    [SerializeField] private float _MoveSpeed = 3000f;
    [SerializeField] private float _BulletForce = 200f;
    [SerializeField] private float _BulletLifespan = .1f;
    //[SerializeField] private bool _IsRanged = false;
    [SerializeField] private float _RateOfFire = .2f;
    [SerializeField] private float _ChaseRange = 100;
    [SerializeField] private float _Range = 150;
    [SerializeField] private Transform _Target;
    [SerializeField] private float _NextWaypointDistance = 3f;

    private bool _InRange = false;
    private Path _Path;
    private int _CurrentWaypoint;
    private bool _EndOfPath = false;
    private Seeker _Seeker;
    private Rigidbody2D _Rigidbody2D;
    private float _Elapsed = 0;

    void Start()
    {
        _Target = GameObject.FindObjectOfType<Player>().transform;
        
        _Seeker = GetComponent<Seeker>();
        _Rigidbody2D = GetComponent<Rigidbody2D>();
        
        InvokeRepeating("UpdatePath", 0f, .5f);
    }

    void UpdatePath()
    {
        if (_Seeker.IsDone())
        {
            _Seeker.StartPath(_Rigidbody2D.position, _Target.position, OnPathComplete);
        }
    }

    private void Update()
    {
        
    }

    void FixedUpdate()
    {
        Vector2 _Direction;
        float _Distance;

        _Elapsed += Time.deltaTime;

        if (_Path != null)
        {
            if (_CurrentWaypoint >= _Path.vectorPath.Count)
            {
                _EndOfPath = true;
                return;
            }
            else
            {
                _EndOfPath = false;
            }


            Vector2 _TargetsVector2 = new Vector2(_Target.transform.position.x, _Target.transform.position.y);
            Vector2 _ThisVector2 = new Vector2(transform.position.x, transform.position.y);
            float _RangeToTarget = Vector2.Distance(_TargetsVector2, _ThisVector2);

            _Direction = ((Vector2)_Path.vectorPath[_CurrentWaypoint] - _Rigidbody2D.position).normalized;
            _Distance = Vector2.Distance(_Rigidbody2D.position, _Path.vectorPath[_CurrentWaypoint]);

            if (_Distance < _NextWaypointDistance) _CurrentWaypoint++;


            //if (_IsRanged == true)
            //{
            if (_RangeToTarget <= _Range)
            {
                if (_Elapsed >= _RateOfFire)
                {
                    Attack();
                    _Elapsed = 0;
                }
            }

            if (_RangeToTarget > _ChaseRange)
            {
                float _TargetAngle = Mathf.Atan2(_Direction.y, _Direction.x) * Mathf.Rad2Deg - 90f;
                Quaternion rotation = Quaternion.AngleAxis(_TargetAngle, Vector3.forward);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, _RotationSpeed * Time.deltaTime);

                _Rigidbody2D.AddForce(_Direction * _MoveSpeed * Time.deltaTime, ForceMode2D.Impulse);
            }
            else
            {
                _Direction = (_TargetsVector2 - _Rigidbody2D.position).normalized;

                float _TargetAngle = Mathf.Atan2(_Direction.y, _Direction.x) * Mathf.Rad2Deg - 90f;
                Quaternion rotation = Quaternion.AngleAxis(_TargetAngle, Vector3.forward);
                transform.rotation = rotation;
            }
        }
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
        
            GameObject _Bullet = Instantiate(_ProjectilePrefab, _Muzzle.position, _Muzzle.rotation);
            Rigidbody2D _BulletRigidbody = _Bullet.GetComponent<Rigidbody2D>();
            _BulletRigidbody.AddForce(_Muzzle.up * _BulletForce, ForceMode2D.Impulse);
            Destroy(_Bullet, _BulletLifespan);
        
    }
}
