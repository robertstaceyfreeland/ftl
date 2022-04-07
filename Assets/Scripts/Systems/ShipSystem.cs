using System;
using System.Net.Http.Headers;
using UnityEngine;

public class ShipSystem : MonoBehaviour
{
    public Helm _Helm;
    private MiniMap _MiniMap;
    private PlayerShipHandler _PLayerShipHandler;

    private const float _STAMINA_CONSUMPTION = .05f;

    public enum SystemLevel
    {
        Level_0, Level_I, Level_II, Level_III
    }

    public SystemLevel _SystemLevel;

    public enum ModifierType
    {
        Attack, Defense, RateOfFire, BoardingPartyDelay, EnemyDoorResponseTime, PlayerDoorResponseTime, DoorLockTime, EnergyRecovery, PlayerHealth, PlayerStamina, RateOfHealing, OxygenFlow
    }
    
    private int[] _RatingLevel_I = new int[12];
    private int[] _RatingLevel_II = new int[12];
    private int[] _RatingLevel_III = new int[12];
    private int[] _RatingAutomatic = new int[12];
    private int[] _RatingImpaired = new int[12];
    private int[] _RatingManned = new int[12];

    public float _CurrentUpgrade = 0;
    private float _UpgradeLevel_I = 1000;
    private float _UpgradeLevel_II = 3000;
    private float _UpgradeLevel_III = 7000;
    private float _UpgradeConstant = 55;
    public bool _IsUpgradeLevel_I = false;
    public bool _IsUpgradeLevel_II = false;
    public bool _IsUpgradeLevel_III = false;

    private void LoadLevelData()
    {
        ModifierType _ModifierType;

        switch (_Helm._HelmType)
        {
            case Helm.HelmType.Shields:
                #region //Data
                _ModifierType = ModifierType.Attack;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.Defense;
                _RatingLevel_I[(int)_ModifierType] = 1;
                _RatingLevel_II[(int)_ModifierType] = 2;
                _RatingLevel_III[(int)_ModifierType] = 3;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = -3;
                _RatingManned[(int)_ModifierType] = 1;

                _ModifierType = ModifierType.RateOfFire;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.BoardingPartyDelay;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.EnemyDoorResponseTime;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.PlayerDoorResponseTime;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.DoorLockTime;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.EnergyRecovery;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.PlayerHealth;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.PlayerStamina;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.RateOfHealing;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.OxygenFlow;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;
                #endregion
                break;
            case Helm.HelmType.Weapons:
                #region //Data
                _ModifierType = ModifierType.Attack;
                _RatingLevel_I[(int)_ModifierType] = 1;
                _RatingLevel_II[(int)_ModifierType] = 2;
                _RatingLevel_III[(int)_ModifierType] = 2;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = -1;
                _RatingManned[(int)_ModifierType] = 1;

                _ModifierType = ModifierType.Defense;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.RateOfFire;
                _RatingLevel_I[(int)_ModifierType] = 2;
                _RatingLevel_II[(int)_ModifierType] = 3;
                _RatingLevel_III[(int)_ModifierType] = 4;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = -3;
                _RatingManned[(int)_ModifierType] = 1;

                _ModifierType = ModifierType.BoardingPartyDelay;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.EnemyDoorResponseTime;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.PlayerDoorResponseTime;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.DoorLockTime;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.EnergyRecovery;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.PlayerHealth;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.PlayerStamina;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.RateOfHealing;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.OxygenFlow;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;
                #endregion
                break;
            case Helm.HelmType.Medical:
                #region //Data
                _ModifierType = ModifierType.Attack;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.Defense;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.RateOfFire;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.BoardingPartyDelay;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.EnemyDoorResponseTime;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.PlayerDoorResponseTime;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.DoorLockTime;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.EnergyRecovery;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.PlayerHealth;
                _RatingLevel_I[(int)_ModifierType] = 20;
                _RatingLevel_II[(int)_ModifierType] = 50;
                _RatingLevel_III[(int)_ModifierType] = 75;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.PlayerStamina;
                _RatingLevel_I[(int)_ModifierType] = 20;
                _RatingLevel_II[(int)_ModifierType] = 50;
                _RatingLevel_III[(int)_ModifierType] = 75;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.RateOfHealing;
                _RatingLevel_I[(int)_ModifierType] = 1;
                _RatingLevel_II[(int)_ModifierType] = 2;
                _RatingLevel_III[(int)_ModifierType] = 3;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.OxygenFlow;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;
                #endregion
                break;
            case Helm.HelmType.Engineering:
                #region //Data
                _ModifierType = ModifierType.Attack;
                _RatingLevel_I[(int)_ModifierType] = 1;
                _RatingLevel_II[(int)_ModifierType] = 2;
                _RatingLevel_III[(int)_ModifierType] = 2;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = -1;
                _RatingManned[(int)_ModifierType] = 1;

                _ModifierType = ModifierType.Defense;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.RateOfFire;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.BoardingPartyDelay;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.EnemyDoorResponseTime;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.PlayerDoorResponseTime;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.DoorLockTime;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.EnergyRecovery;
                _RatingLevel_I[(int)_ModifierType] = 1;
                _RatingLevel_II[(int)_ModifierType] = 2;
                _RatingLevel_III[(int)_ModifierType] = 3;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = -2;
                _RatingManned[(int)_ModifierType] = 1;

                _ModifierType = ModifierType.PlayerHealth;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.PlayerStamina;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.RateOfHealing;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.OxygenFlow;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;
                #endregion
                break;
            case Helm.HelmType.Pilot:
                #region //Data
                _ModifierType = ModifierType.Attack;
                _RatingLevel_I[(int)_ModifierType] = 1;
                _RatingLevel_II[(int)_ModifierType] = 2;
                _RatingLevel_III[(int)_ModifierType] = 2;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = -1;
                _RatingManned[(int)_ModifierType] = 1;

                _ModifierType = ModifierType.Defense;
                _RatingLevel_I[(int)_ModifierType] = 1;
                _RatingLevel_II[(int)_ModifierType] = 2;
                _RatingLevel_III[(int)_ModifierType] = 3;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = -1;
                _RatingManned[(int)_ModifierType] = 1;

                _ModifierType = ModifierType.RateOfFire;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.BoardingPartyDelay;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.EnemyDoorResponseTime;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.PlayerDoorResponseTime;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.DoorLockTime;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.EnergyRecovery;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.PlayerHealth;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.PlayerStamina;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.RateOfHealing;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.OxygenFlow;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;
                #endregion
                break;
            case Helm.HelmType.Doors:
                #region //Data
                _ModifierType = ModifierType.Attack;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.Defense;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.RateOfFire;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.BoardingPartyDelay;
                _RatingLevel_I[(int)_ModifierType] = 5;
                _RatingLevel_II[(int)_ModifierType] = 10;
                _RatingLevel_III[(int)_ModifierType] = 15;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = -5;
                _RatingManned[(int)_ModifierType] = 5;

                _ModifierType = ModifierType.EnemyDoorResponseTime;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 1;
                _RatingLevel_III[(int)_ModifierType] = 2;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 3;
                _RatingManned[(int)_ModifierType] = 0; //TODO: This needs to be reviewed - Should Lock out Enemies when manned - I may do it somewhere else

                _ModifierType = ModifierType.PlayerDoorResponseTime;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 3;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.DoorLockTime;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 20;

                _ModifierType = ModifierType.EnergyRecovery;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.PlayerHealth;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.PlayerStamina;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.RateOfHealing;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.OxygenFlow;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;
                #endregion
                break;
            case Helm.HelmType.Sensor:
                #region //Data
                _ModifierType = ModifierType.Attack;
                _RatingLevel_I[(int)_ModifierType] = 1;
                _RatingLevel_II[(int)_ModifierType] = 2;
                _RatingLevel_III[(int)_ModifierType] = 3;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = -1;
                _RatingManned[(int)_ModifierType] = 1;

                _ModifierType = ModifierType.Defense;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.RateOfFire;
                _RatingLevel_I[(int)_ModifierType] = 1;
                _RatingLevel_II[(int)_ModifierType] = 1;
                _RatingLevel_III[(int)_ModifierType] = 2;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = -2;
                _RatingManned[(int)_ModifierType] = 1;

                _ModifierType = ModifierType.BoardingPartyDelay;
                _RatingLevel_I[(int)_ModifierType] = 5;
                _RatingLevel_II[(int)_ModifierType] = 10;
                _RatingLevel_III[(int)_ModifierType] = 15;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = -5;
                _RatingManned[(int)_ModifierType] = 5;

                _ModifierType = ModifierType.EnemyDoorResponseTime;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.PlayerDoorResponseTime;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.DoorLockTime;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.EnergyRecovery;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.PlayerHealth;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.PlayerStamina;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.RateOfHealing;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.OxygenFlow;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;
                #endregion
                break;
            case Helm.HelmType.LifeSupport:
                #region //Data
                _ModifierType = ModifierType.Attack;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.Defense;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.RateOfFire;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.BoardingPartyDelay;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.EnemyDoorResponseTime;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.PlayerDoorResponseTime;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.DoorLockTime;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.EnergyRecovery;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.PlayerHealth;
                _RatingLevel_I[(int)_ModifierType] = 5;
                _RatingLevel_II[(int)_ModifierType] = 10;
                _RatingLevel_III[(int)_ModifierType] = 20;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = -10;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.PlayerStamina;
                _RatingLevel_I[(int)_ModifierType] = 10;
                _RatingLevel_II[(int)_ModifierType] = 20;
                _RatingLevel_III[(int)_ModifierType] = 40;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 25;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.RateOfHealing;
                _RatingLevel_I[(int)_ModifierType] = 0;
                _RatingLevel_II[(int)_ModifierType] = 0;
                _RatingLevel_III[(int)_ModifierType] = 0;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = 0;
                _RatingManned[(int)_ModifierType] = 0;

                _ModifierType = ModifierType.OxygenFlow;
                _RatingLevel_I[(int)_ModifierType] = 1;
                _RatingLevel_II[(int)_ModifierType] = 2;
                _RatingLevel_III[(int)_ModifierType] = 3;
                _RatingAutomatic[(int)_ModifierType] = 0;
                _RatingImpaired[(int)_ModifierType] = -8;
                _RatingManned[(int)_ModifierType] = 05;
                #endregion
                break;
            default:
                break;
        }

        
    }

    private void CalculateUpgrade()
    {
        _CurrentUpgrade = Mathf.Clamp(_CurrentUpgrade, 0, _UpgradeLevel_III);

        if (_CurrentUpgrade < _UpgradeLevel_I)
        {
            _IsUpgradeLevel_I = false;
            _IsUpgradeLevel_II = false;
            _IsUpgradeLevel_III = false;

            _Helm.SetUpgradeSymbols(_IsUpgradeLevel_I, _IsUpgradeLevel_II, _IsUpgradeLevel_III);
            _Helm.SetUpgradeFill(_CurrentUpgrade / _UpgradeLevel_I);
            _SystemLevel = SystemLevel.Level_0;
            return;
        }

        if (_CurrentUpgrade < _UpgradeLevel_II)
        {
            if (_IsUpgradeLevel_I == false)
            {
                SoundManager.Instance.PlaySound(SoundManager.Sound.Acknowledgement, transform.position, SoundManager.AudioMixerGroupName.UI);
            }

            _IsUpgradeLevel_I = true;
            _IsUpgradeLevel_II = false;
            _IsUpgradeLevel_III = false;

            _Helm.SetUpgradeSymbols(_IsUpgradeLevel_I, _IsUpgradeLevel_II, _IsUpgradeLevel_III);

            _Helm.SetUpgradeFill((_CurrentUpgrade - _UpgradeLevel_I) / (_UpgradeLevel_II - _UpgradeLevel_I));
            _SystemLevel = SystemLevel.Level_I;
            return;
        }

        if (_CurrentUpgrade < _UpgradeLevel_III)
        {
            if (_IsUpgradeLevel_II == false)
            {
                SoundManager.Instance.PlaySound(SoundManager.Sound.Acknowledgement, transform.position, SoundManager.AudioMixerGroupName.UI);
            }

            _IsUpgradeLevel_I = true;
            _IsUpgradeLevel_II = true;
            _IsUpgradeLevel_III = false;

            _Helm.SetUpgradeSymbols(_IsUpgradeLevel_I, _IsUpgradeLevel_II, _IsUpgradeLevel_III);

            _Helm.SetUpgradeFill((_CurrentUpgrade - _UpgradeLevel_II) / (_UpgradeLevel_III - _UpgradeLevel_II));
            _SystemLevel = SystemLevel.Level_II;
            return;
        }

        if (_CurrentUpgrade >= _UpgradeLevel_III)
        {
            if (_IsUpgradeLevel_III == false)
            {
                SoundManager.Instance.PlaySound(SoundManager.Sound.Acknowledgement, transform.position, SoundManager.AudioMixerGroupName.UI);
                Achievements.Instance.ShipSystemLevelIII(_Helm._HelmType);
            }

            _IsUpgradeLevel_I = true;
            _IsUpgradeLevel_II = true;
            _IsUpgradeLevel_III = true;

            _Helm.SetUpgradeSymbols(_IsUpgradeLevel_I, _IsUpgradeLevel_II, _IsUpgradeLevel_III);

            _Helm.SetUpgradeFill(1);
            _SystemLevel = SystemLevel.Level_III;
            return;
        }

    }

    private void Update()
    {
        float _StaminaUsed = 0;

        switch (_Helm.GetHelmState())
        {
            case Helm.HelmState.Manned:
                _CurrentUpgrade += _UpgradeConstant * Time.deltaTime;
                if (_IsUpgradeLevel_I) { _PLayerShipHandler.UseStamina(_STAMINA_CONSUMPTION * 2 * Time.deltaTime); break; }
                if (_IsUpgradeLevel_II) { _PLayerShipHandler.UseStamina(_STAMINA_CONSUMPTION * 4 * Time.deltaTime); break; }
                if (_IsUpgradeLevel_III) { _PLayerShipHandler.UseStamina(_STAMINA_CONSUMPTION * 8 * Time.deltaTime); break; }
                _StaminaUsed = (_STAMINA_CONSUMPTION * 1.25f * Time.deltaTime);
                break;
            case Helm.HelmState.Automatic:
                if (_IsUpgradeLevel_I) { _PLayerShipHandler.UseStamina(_STAMINA_CONSUMPTION * 2 * Time.deltaTime); break; }
                if (_IsUpgradeLevel_II) { _PLayerShipHandler.UseStamina(_STAMINA_CONSUMPTION * 4 * Time.deltaTime); break; }
                if (_IsUpgradeLevel_III) { _PLayerShipHandler.UseStamina(_STAMINA_CONSUMPTION * 8 * Time.deltaTime); break; }
                _StaminaUsed = (_STAMINA_CONSUMPTION * 1.0f * Time.deltaTime);
                break;
            case Helm.HelmState.Impaired:
                if (_IsUpgradeLevel_I)
                {
                    _CurrentUpgrade -= (_UpgradeConstant * .5f) * Time.deltaTime;
                    
                }
                if (_IsUpgradeLevel_II)
                {
                    _CurrentUpgrade -= (_UpgradeConstant * 1f) * Time.deltaTime;
                    
                }
                if (_IsUpgradeLevel_III)
                {
                    _CurrentUpgrade -= (_UpgradeConstant * 2f) * Time.deltaTime;
                }
                
                _CurrentUpgrade -= (_UpgradeConstant * .25f) * Time.deltaTime;

                break;
            default:
                _StaminaUsed = 0;
                break;
        }

        if (_StaminaUsed > _PLayerShipHandler._HealthSystem.GetStaminaAmount())
        {
            if (_IsUpgradeLevel_I) { _Helm.Damage((.15f * Time.deltaTime), false, false,false); return; }
            if (_IsUpgradeLevel_II) { _Helm.Damage((.20f * Time.deltaTime), false, false, false); return; }
            if (_IsUpgradeLevel_III) { _Helm.Damage((.40f * Time.deltaTime), false, false, false); return; }
            _Helm.Damage((.10f * Time.deltaTime), false, false, false);
        }

        _PLayerShipHandler.UseStamina(_STAMINA_CONSUMPTION * 1 * Time.deltaTime);

        CalculateUpgrade();
    }

    private void Start()
    {
        _Helm = GetComponent<Helm>();
        _PLayerShipHandler = GameObject.FindObjectOfType<PlayerShipHandler>();

        _MiniMap = GameObject.FindObjectOfType<MiniMap>();

        LoadLevelData();

        _Helm._DamageSystem.OnHealthChanged += _DamageSystem_OnHealthChanged;
        _Helm._DamageSystem.OnStartedWorking += _DamageSystem_OnStartedWorking;
        _Helm._DamageSystem.OnStoppedWorking += _DamageSystem_OnStoppedWorking;

        GameHandler.Instance.OnSaveAllData += Instance_OnSaveAllData;

        LoadData();
    }

    private void Instance_OnSaveAllData(object sender, EventArgs e)
    {
        SaveData();
    }

    public void SaveData()
    {
        ES3.Save(_Helm._HelmType.ToString() + "_SystemLevel", _SystemLevel);
        ES3.Save(_Helm._HelmType.ToString() + "_CurrentUpgrade", _CurrentUpgrade);
        ES3.Save(_Helm._HelmType.ToString() + "_IsUpgradeLevel_I", _IsUpgradeLevel_I);
        ES3.Save(_Helm._HelmType.ToString() + "_IsUpgradeLevel_II", _IsUpgradeLevel_II);
        ES3.Save(_Helm._HelmType.ToString() + "_IsUpgradeLevel_III", _IsUpgradeLevel_III);
    }

    public void LoadData()
    {
        try
        {
            _SystemLevel = ES3.Load<SystemLevel>(_Helm._HelmType.ToString() + "_SystemLevel");
            _CurrentUpgrade = ES3.Load<float>(_Helm._HelmType.ToString() + "_CurrentUpgrade");
            _IsUpgradeLevel_I = ES3.Load<bool>(_Helm._HelmType.ToString() + "_IsUpgradeLevel_I");
            _IsUpgradeLevel_II = ES3.Load<bool>(_Helm._HelmType.ToString() + "_IsUpgradeLevel_II");
            _IsUpgradeLevel_III = ES3.Load<bool>(_Helm._HelmType.ToString() + "_IsUpgradeLevel_III");
        }
        catch { }
    }

    public int GetHelmModifier(ModifierType pModifierType)
    {
        switch (_Helm.GetHelmState())
        {
            case Helm.HelmState.Manned:
                return _RatingManned[(int)pModifierType];
            case Helm.HelmState.Automatic:
                return _RatingAutomatic[(int)pModifierType];
            case Helm.HelmState.Impaired:
                return _RatingImpaired[(int)pModifierType];
            default:
                Debug.LogWarning("Helm State Not Found!");
                return 0;
        }
    }

    public int GetSystemModifier(ModifierType pModifierType)
    {
        switch (_SystemLevel)
        {
            case SystemLevel.Level_I:
                return _RatingLevel_I[(int)pModifierType];
            case SystemLevel.Level_II:
                return _RatingLevel_II[(int)pModifierType];
            case SystemLevel.Level_III:
                return _RatingLevel_III[(int)pModifierType];
            default:
                return 0;
        }
    }

    //Events
    private void _DamageSystem_OnStoppedWorking(object sender, System.EventArgs e)
    {
        if(_Helm._HelmType == Helm.HelmType.Sensor) _MiniMap.MiniMapDisabled(true);
    }
    private void _DamageSystem_OnStartedWorking(object sender, System.EventArgs e)
    {
        if (_Helm._HelmType == Helm.HelmType.Sensor) _MiniMap.MiniMapDisabled(false);
    }
    private void _DamageSystem_OnHealthChanged(object sender, System.EventArgs e)
    {
        //
    }
}
