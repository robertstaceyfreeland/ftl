using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    public enum MiniMapMode { PlayerShip, Dogfight, EnemyShip, Blackout }
    
    private PlayerShipHandler _PlayerShipHandler;
    private Camera _CameraMinimap;
    Vector3 _DesiredMiniMapLocation = Vector3.zero;
    float _DesiredMiniMapSize = 0;

    [SerializeField] private Image _BrokenOverlay;
    [SerializeField] private Button _ButtonIntruder;
    private Image _ButtonImage;

    private const float _CAMERA_SPEED = 2.5f;

    void Start()
    {
        _CameraMinimap = GameObject.Find("MinimapCamera").GetComponent<Camera>();

        _BrokenOverlay.enabled = false;

        _ButtonImage = _ButtonIntruder.GetComponent<Image>();

        _ButtonImage.color = Color.grey;

        _PlayerShipHandler = GameObject.FindObjectOfType<PlayerShipHandler>();
    }

    private void Update()
    {
        _CameraMinimap.transform.position = Vector3.Lerp(_CameraMinimap.transform.position, _DesiredMiniMapLocation, (_CAMERA_SPEED * Time.deltaTime));
        _CameraMinimap.orthographicSize = Mathf.Lerp(_CameraMinimap.orthographicSize, _DesiredMiniMapSize, (_CAMERA_SPEED * Time.deltaTime));
    }

    public void SetMiniMapCamera(MiniMapMode pMiniMapMode)
    {
        switch (pMiniMapMode)
        {
            case MiniMapMode.PlayerShip:
                _DesiredMiniMapLocation = new Vector3(0, 0, -500);
                _DesiredMiniMapSize = 1000;
                break;
            case MiniMapMode.Dogfight:
                _DesiredMiniMapLocation = new Vector3(825, 750, -500);
                _DesiredMiniMapSize = 1800;
                break;
            case MiniMapMode.EnemyShip:
                _DesiredMiniMapLocation = new Vector3(750, 750, -500);
                _DesiredMiniMapSize = 1500;
                break;
            case MiniMapMode.Blackout:
                _DesiredMiniMapLocation = new Vector3(5000, 5000, -500);
                _DesiredMiniMapSize = 1500;
                break;
            default:
                break;
        }
    }

    public void SoundRedAlert(bool pValue)
    {
        if (pValue)
        {
            _ButtonImage.color = Color.red;
        }
        else
        {
            _ButtonImage.color = Color.grey;
        }
        
    }
    
    public void ToggleIntruderButton()
    {
        if (_ButtonImage.color == Color.grey)
        {
            _PlayerShipHandler._EnemyIntruderAlertLevel = PlayerShipHandler.EnemyIntruderAlertLevel.Off;
            //Do Nothing - No Intruders
        }

        else
        {
            if (_ButtonImage.color == Color.red)
            {
                _ButtonImage.color = Color.yellow;
                _PlayerShipHandler._EnemyIntruderAlertLevel = PlayerShipHandler.EnemyIntruderAlertLevel.Yellow;
            }
            else
            {
                _ButtonImage.color = Color.red;
                _PlayerShipHandler._EnemyIntruderAlertLevel = PlayerShipHandler.EnemyIntruderAlertLevel.Red;
            }
        }

    }

    public void MiniMapDisabled(bool value)
    {
        _BrokenOverlay.enabled = value;
    }
}
