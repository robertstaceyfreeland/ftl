using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshAnimator : MonoBehaviour
{
    public enum PlayerState { Idle, Walk, Roll}
    public enum PlayerDirection { Up, Down, Right, Left}
    public float _CurrentMovementMagnitude = 1;
    public PlayerState _PlayerState = PlayerState.Idle;
    private PlayerState _PreviousPlayerState = PlayerState.Idle;
    public PlayerDirection _PlayerDirection = PlayerDirection.Down;

    Mesh _Mesh;
    Vector3[] _Vertices;
    Vector3 _HeadPosition;
    Vector2[] uv;
    [SerializeField] public float _FeetMoveMultiplier = 15;

    //Head
    [SerializeField] float _HeadSize = 27.5f;
    [SerializeField] float _StartingHeadPositionX = -13.75f;
    [SerializeField] float _StartingHeadPositionY = 4.0f;
    float _HeadPositionY = -4;
    float _HeadPositionY_Max = 1;
    float _HeadPositionY_Min = -3;
    float _HeadPositionX = 7.5f;
    float _HeadPositionX_Max = 10f;
    float _HeadPositionX_Min = 10f;
    [SerializeField] float _HeadBobRangeY = 1;
    float _HeadBobRangeX = 0;
    [SerializeField] float _HeadBobSpeed = 5f;
    bool _HeadPositionMoveDown = true;
    bool _HeadPositionMoveRight = true;

    //Feet and Bools
    [SerializeField] float _FeetMoveRangeY = 4;
    [SerializeField] float _FeetStartingSpreadX = 5.0f;
    float _FeetPositionY_Max = 0;
    float _FeetPositionY_Min = 0;
    bool _RightFootPositionMoveDown = false;

    //Right Foot
    Vector3 _RightFootPosition;
    [SerializeField] float _RightFootStartingPositionY = -20;
    [SerializeField] float _RightFootStartingPositionX;
    [SerializeField] float _RightFootLeftRightPositionX;
    float _RightFootPositionY;
    float _RightFootPositionX;
    [SerializeField] float _RightFootSize = 10;

    //Left Foot
    Vector3 _LeftFootPosition;
    [SerializeField] float _LeftFootStartingPositionY = -20;
    [SerializeField] float _LeftFootStartingPositionX = 0;
    [SerializeField] float _LeftFootLeftRightPositionX;
    float _LeftFootPositionY;
    float _LeftFootPositionX = 15;
    [SerializeField] float _LeftFootSize =10.0f;

    [SerializeField] float _FeetShufflePositionY_Max;
    [SerializeField] float _FeetShufflePositionY_Min;
    [SerializeField] float _FeetShuffleRangeY = 1;

    //Body
    [SerializeField] float _BodySize = 30;
    [SerializeField] float _StartingBodyPositionY = -15;

    


    //New
    float _LeftFootPositionCloseX;
    float _RightFootPositionCloseX;

    //float _NextHeadBob = 0;

    private void Start()
    {
        _HeadPositionX = _StartingHeadPositionX;
        _HeadPositionY = _StartingHeadPositionY;

        _HeadPositionY_Max = _HeadPositionY + _HeadBobRangeY;
        _HeadPositionY_Min = _HeadPositionY - _HeadBobRangeY;

        _RightFootStartingPositionX = 0 + _FeetStartingSpreadX - (_RightFootSize/2);
        _RightFootPositionX = _RightFootStartingPositionX; 
        _RightFootPositionY = _RightFootStartingPositionY;
        _RightFootLeftRightPositionX = _RightFootStartingPositionX - (_FeetStartingSpreadX * .65f);

        _LeftFootStartingPositionX = 0 - _FeetStartingSpreadX - (_LeftFootSize / 2); 
        _LeftFootPositionX = _LeftFootStartingPositionX;
        _LeftFootPositionY = _LeftFootStartingPositionY;
        _LeftFootLeftRightPositionX = _LeftFootStartingPositionX + (_FeetStartingSpreadX*.65f);

        _FeetPositionY_Max = _RightFootStartingPositionY + _FeetMoveRangeY;
        _FeetPositionY_Min = _RightFootStartingPositionY - _FeetMoveRangeY;

        _FeetShufflePositionY_Max = _RightFootStartingPositionY + _FeetShuffleRangeY;
        _FeetShufflePositionY_Min = _RightFootStartingPositionY;



        _LeftFootPositionCloseX = 0;

        CreateAnimationMesh();


    }

    private void CreateAnimationMesh()
    {
        _Mesh = new Mesh();

        _Vertices = new Vector3[4 * 4];
        uv = new Vector2[4 * 4];
        int[] triangles = new int[6 * 4];


        #region // Render Body #1

        Vector3 bodyPosition = new Vector3((0-(_BodySize * .5f)), _StartingBodyPositionY);

        _Vertices[0] = bodyPosition + new Vector3(0, 0);
        _Vertices[1] = bodyPosition + new Vector3(0, _BodySize);
        _Vertices[2] = bodyPosition + new Vector3(_BodySize, _BodySize);
        _Vertices[3] = bodyPosition + new Vector3(_BodySize, 0);

        uv[0] = new Vector2(0, .5f);
        uv[1] = new Vector2(0, .8f);
        uv[2] = new Vector2(.2f, .8f);
        uv[3] = new Vector2(.2f, .5f);

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        triangles[3] = 0;
        triangles[4] = 2;
        triangles[5] = 3;
        
        #endregion

        #region  //Render Head #2

        _HeadPosition = new Vector3(_StartingHeadPositionX, _StartingHeadPositionY);

        //_HeadPositionX = _StartingHeadPositionX;
        //_HeadPositionY = _StartingHeadPositionY;

        _Vertices[4] = _HeadPosition + new Vector3(0, 0);
        _Vertices[5] = _HeadPosition + new Vector3(0, _HeadSize);
        _Vertices[6] = _HeadPosition + new Vector3(_HeadSize, _HeadSize);
        _Vertices[7] = _HeadPosition + new Vector3(_HeadSize, 0);
        
        uv[4] = new Vector2(0, .8f);
        uv[5] = new Vector2(0, 1f);
        uv[6] = new Vector2(.2f, 1f);
        uv[7] = new Vector2(.2f, .8f);

        triangles[6] = 4;
        triangles[7] = 5;
        triangles[8] = 6;

        triangles[9] = 4;
        triangles[10] = 6;
        triangles[11] = 7;

        _Mesh.vertices = _Vertices;
        _Mesh.uv = uv;
        _Mesh.triangles = triangles;

        #endregion

        #region  //Render Left Foot #3

        _LeftFootPosition = new Vector3(_LeftFootStartingPositionX, _LeftFootStartingPositionY);

        _Vertices[8] = _LeftFootPosition + new Vector3(0, 0);
        _Vertices[9] = _LeftFootPosition + new Vector3(0, _LeftFootSize);
        _Vertices[10] = _LeftFootPosition + new Vector3(_LeftFootSize, _LeftFootSize);
        _Vertices[11] = _LeftFootPosition + new Vector3(_LeftFootSize, 0);

        uv[8] =  new Vector2(.85f, .65f);
        uv[9] =  new Vector2(.85f, .75f);
        uv[10] = new Vector2(.95f, .75f);
        uv[11] = new Vector2(.95f, .65f);

        triangles[12] = 8;
        triangles[13] = 9;
        triangles[14] = 10;

        triangles[15] = 8;
        triangles[16] = 10;
        triangles[17] = 11;

        _Mesh.vertices = _Vertices;
        _Mesh.uv = uv;
        _Mesh.triangles = triangles;

        #endregion

        #region  //Render Right Foot #3


        _RightFootSize = 10f;
        _RightFootPosition = new Vector3(_RightFootStartingPositionX, _RightFootStartingPositionY); 

        _Vertices[12] = _RightFootPosition + new Vector3(0, 0);
        _Vertices[13] = _RightFootPosition + new Vector3(0, _RightFootSize);
        _Vertices[14] = _RightFootPosition + new Vector3(_RightFootSize, _RightFootSize);
        _Vertices[15] = _RightFootPosition + new Vector3(_RightFootSize, 0);

        uv[12] = new Vector2(.85f, .65f);
        uv[13] = new Vector2(.85f, .75f);
        uv[14] = new Vector2(.95f, .75f);
        uv[15] = new Vector2(.95f, .65f);

        triangles[18] = 12;
        triangles[19] = 13;
        triangles[20] = 14;

        triangles[21] = 12;
        triangles[22] = 14;
        triangles[23] = 15;

        #endregion

        _Mesh.vertices = _Vertices;
        _Mesh.uv = uv;
        _Mesh.triangles = triangles;

        GetComponent<MeshFilter>().mesh = _Mesh;
    }

    private void Update()
    {
        switch (_PlayerState)
        {
            case PlayerState.Idle:
                BobHead();
                ShuffleFeet();
                _PreviousPlayerState = PlayerState.Idle;
                break;
            case PlayerState.Walk:
                BobHead();
                MoveFeet();
                _PreviousPlayerState = PlayerState.Walk;
                break;
            case PlayerState.Roll:
                _PreviousPlayerState = PlayerState.Roll;
                break;
            default:
                break;
        }

        switch (_PlayerDirection)
        {
            case PlayerDirection.Up:
                MoveUpAnimations();
                break;
            case PlayerDirection.Down:
                MoveDownAnimations();
                break;
            case PlayerDirection.Right:
                MoveRightAnimations();
                break;
            case PlayerDirection.Left:
                MoveLeftAnimations();
                break;
            default:
                break;
        }
    }

    void BobHead()
    {
        
        if (_HeadPositionMoveDown)
        {
            _HeadPositionY += -_HeadBobSpeed * Time.deltaTime;
            if (_HeadPositionY < _HeadPositionY_Min)
            {
                _HeadPositionMoveDown = false;
            }
        }
        else
        {
            _HeadPositionY += _HeadBobSpeed * Time.deltaTime;
            if (_HeadPositionY > _HeadPositionY_Max)
            {
                _HeadPositionMoveDown = true;
            }
        }

        _HeadPosition = new Vector3(_HeadPositionX, _HeadPositionY);

        _Vertices[4] = _HeadPosition + new Vector3(0, 0);
        _Vertices[5] = _HeadPosition + new Vector3(0, _HeadSize);
        _Vertices[6] = _HeadPosition + new Vector3(_HeadSize, _HeadSize);
        _Vertices[7] = _HeadPosition + new Vector3(_HeadSize, 0);

        _Mesh.vertices = _Vertices;
    }

    void ShuffleFeet()
    {
        if (_PlayerDirection == PlayerDirection.Left || _PlayerDirection == PlayerDirection.Right)
        {
            _LeftFootPositionX = _LeftFootLeftRightPositionX;
            _RightFootPositionX = _RightFootLeftRightPositionX;
        }
        else
        {
            _LeftFootPositionX = _LeftFootStartingPositionX;
            _RightFootPositionX = _RightFootStartingPositionX;
        }

        if (_PreviousPlayerState != PlayerState.Idle)
        {
            //Reset Feet
            _RightFootPositionY = _RightFootStartingPositionY;
            _LeftFootPositionY = _LeftFootStartingPositionY;
        }

        if (_RightFootPositionMoveDown)
        {
            _RightFootPositionY -= 5f * Time.deltaTime;
            if (_RightFootPositionY <= _FeetShufflePositionY_Min)
            {
                _RightFootPositionMoveDown = false;
            }
        }
        else
        {
            _RightFootPositionY += 5f * Time.deltaTime;
            if (_RightFootPositionY > _FeetShufflePositionY_Max)
            {
                _RightFootPositionMoveDown = true;
            }
        }

        _LeftFootPosition = new Vector3(_LeftFootPositionX, _LeftFootStartingPositionY);
        _RightFootPosition = new Vector3(_RightFootPositionX, _RightFootPositionY);

        _Vertices[8] = _LeftFootPosition + new Vector3(0, 0);
        _Vertices[9] = _LeftFootPosition + new Vector3(0, _LeftFootSize);
        _Vertices[10] = _LeftFootPosition + new Vector3(_LeftFootSize, _LeftFootSize);
        _Vertices[11] = _LeftFootPosition + new Vector3(_LeftFootSize, 0);

        _Vertices[12] = _RightFootPosition + new Vector3(0, 0);
        _Vertices[13] = _RightFootPosition + new Vector3(0, _RightFootSize);
        _Vertices[14] = _RightFootPosition + new Vector3(_RightFootSize, _RightFootSize);
        _Vertices[15] = _RightFootPosition + new Vector3(_RightFootSize, 0);

        _Mesh.vertices = _Vertices;
    }

    void MoveFeet()
    {
        if (_PlayerDirection == PlayerDirection.Left || _PlayerDirection == PlayerDirection.Right)
        {
            _LeftFootPositionX = _LeftFootLeftRightPositionX;
            _RightFootPositionX = _RightFootLeftRightPositionX;
        }    
        else
        {
            _LeftFootPositionX = _LeftFootStartingPositionX;
            _RightFootPositionX = _RightFootStartingPositionX;
        }

        if (_RightFootPositionMoveDown)
        {
            _RightFootPositionY -= (_CurrentMovementMagnitude * _FeetMoveMultiplier) * Time.deltaTime;
            _LeftFootPositionY += (_CurrentMovementMagnitude * _FeetMoveMultiplier) * Time.deltaTime;

            if (_RightFootPositionY <= _FeetPositionY_Min)
            {
                _RightFootPositionMoveDown = false;
            }
        }
        else
        {
            _RightFootPositionY += (_CurrentMovementMagnitude * _FeetMoveMultiplier) * Time.deltaTime;
            _LeftFootPositionY -= (_CurrentMovementMagnitude * _FeetMoveMultiplier) * Time.deltaTime;
            
            if (_RightFootPositionY > _FeetPositionY_Max)
            {
                _RightFootPositionMoveDown = true;
            }
        }

        _RightFootPosition = new Vector3(_RightFootPositionX, _RightFootPositionY);
        _LeftFootPosition = new Vector3(_LeftFootPositionX, _LeftFootPositionY);

        _Vertices[8] = _LeftFootPosition + new Vector3(0, 0);
        _Vertices[9] = _LeftFootPosition + new Vector3(0, _LeftFootSize);
        _Vertices[10] = _LeftFootPosition + new Vector3(_LeftFootSize, _LeftFootSize);
        _Vertices[11] = _LeftFootPosition + new Vector3(_LeftFootSize, 0);

        _Vertices[12] = _RightFootPosition + new Vector3(0, 0);
        _Vertices[13] = _RightFootPosition + new Vector3(0, _RightFootSize);
        _Vertices[14] = _RightFootPosition + new Vector3(_RightFootSize, _RightFootSize);
        _Vertices[15] = _RightFootPosition + new Vector3(_RightFootSize, 0);

        _Mesh.vertices = _Vertices;
    }

    void MoveRightAnimations()
    {
        //Body
        uv[0] = new Vector2(0.6f, .5f);
        uv[1] = new Vector2(0.6f, .8f);
        uv[2] = new Vector2(.8f, .8f);
        uv[3] = new Vector2(.8f, .5f);

        //Head
        uv[4] = new Vector2(0.6f, .8f);
        uv[5] = new Vector2(0.6f, 1f);
        uv[6] = new Vector2(.8f, 1f);
        uv[7] = new Vector2(.8f, .8f);

        _Mesh.uv = uv;
    }

    void MoveLeftAnimations()
    {
        //Body
        uv[0] = new Vector2(0.2f, .5f);
        uv[1] = new Vector2(0.2f, .8f);
        uv[2] = new Vector2(.4f, .8f);
        uv[3] = new Vector2(.4f, .5f);

        //Head
        uv[4] = new Vector2(0.2f, .8f);
        uv[5] = new Vector2(0.2f, 1f);
        uv[6] = new Vector2(.4f, 1f);
        uv[7] = new Vector2(.4f, .8f);

        _Mesh.uv = uv;

        //Todo: Pickup where you left off
        //_LeftFootPositionX = _LeftFootPositionCloseX;
        //_RightFootPositionX = _RightFootPositionCloseX;

    }

    void MoveDownAnimations()
    {
        //Body
        uv[0] = new Vector2(0.0f, .5f);
        uv[1] = new Vector2(0.0f, .8f);
        uv[2] = new Vector2(.2f, .8f);
        uv[3] = new Vector2(.2f, .5f);

        //Head
        uv[4] = new Vector2(0.0f, .8f);
        uv[5] = new Vector2(0.0f, 1f);
        uv[6] = new Vector2(.2f, 1f);
        uv[7] = new Vector2(.2f, .8f);

        _Mesh.uv = uv;
    }

    void MoveUpAnimations()
    {
        //Body
        uv[0] = new Vector2(0.4f, .5f);
        uv[1] = new Vector2(0.4f, .8f);
        uv[2] = new Vector2(.6f, .8f);
        uv[3] = new Vector2(.6f, .5f);

        //Head
        uv[4] = new Vector2(0.4f, .8f);
        uv[5] = new Vector2(0.4f, 1f);
        uv[6] = new Vector2(.6f, 1f);
        uv[7] = new Vector2(.6f, .8f);
        
        _Mesh.uv = uv;
    }
}
