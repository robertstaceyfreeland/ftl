using UnityEngine;
using Steamworks;
using ES3Types;
using System.Collections.Generic;

public class Achievements : MonoBehaviour
{
    public static Achievements Instance { get; private set; } //Singleton

    public List<MyAchievement> _MyAchievments = new List<MyAchievement>();

    public bool _IsFivePercentHealth = false;
    public bool _IsFivePercentHull = false;
    private bool _PlayerHealed = false;
    public bool _RanOutOfPower = false;
    public bool _ReloadedGame = false;
    public bool _EnemySuffocated = false;
    public bool _EnemyKilledByHands = false;

    public bool _PilotDamaged = false;
    public bool _MedicalDamaged = false;
    public bool _DoorDamaged = false;
    public bool _SensorDamaged = false;
    public bool _WeaponDamaged = false;
    public bool _ShieldDamaged = false;
    public bool _LifeSupportDamaged = false;
    public bool _EngineeringDamaged = false;

    private bool _IsPilotUpgraded = false;
    private bool _IsMedicalUpgraded = false;
    private bool _IsDoorUpgraded = false;
    private bool _IsSensorUpgraded = false;
    private bool _IsWeaponUpgraded = false;
    private bool _IsShieldUpgraded = false;
    private bool _IsLifeSupportUpgraded = false;
    private bool _IsEngineeringUpgraded = false;

    private int _CurrentLevel = 0;
    public int _BombBlastCount = 0;
    public int _BoardindPartyCount = 0;
    public float _HealingCount = 0;
    public int _KilledEnemiesCount = 0;


    private void Awake()
    {
        #region //Singleton

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        #endregion
    }

    private void Start()
    {
        LoadAchievements();
    }

    private void Update()
    {
        //1_3 Dodging Bomb Blast
        if (_BombBlastCount >= 10)
        {
            PostAchievement(3);
        }
    }

    private void EndOfBattleReset()
    {
        _BombBlastCount = 0;
        _BoardindPartyCount = 0;
        _HealingCount = 0;

        _RanOutOfPower = false;

        _PilotDamaged = false;
        _MedicalDamaged = false;
        _DoorDamaged = false;
        _SensorDamaged = false;
        _WeaponDamaged = false;
        _ShieldDamaged = false;
        _LifeSupportDamaged = false;
        _EngineeringDamaged = false;
        _IsFivePercentHull = false;
    }

    public void EndOfBattleAchievements(int pCurrentLevel)
    {
        _CurrentLevel = pCurrentLevel;
        if (_BoardindPartyCount <3)
        {
            PostAchievement(16);
        }

        if (pCurrentLevel >= 3)
        {
            PostAchievement(1);
        }

        if (_IsFivePercentHealth)
        {
            PostAchievement(2);
        }
        
        // 1_17 Every System Damaged During Battle
        if (
                _PilotDamaged &&
                _MedicalDamaged &&
                _DoorDamaged &&
                _SensorDamaged &&
                _WeaponDamaged &&
                _ShieldDamaged &&
                _LifeSupportDamaged &&
                _EngineeringDamaged
            )
        {
            PostAchievement(17);
        }

        if (_CurrentLevel >= 11)
        {
            //Survived all three alien races (1/8)
            PostAchievement(8);
        }

        if (_CurrentLevel >= 7)
        {
            //Reach Starbase Bravo without Healing
            PostAchievement(9);
        }
        if (_BoardindPartyCount >= 5)
        {
            PostAchievement(13);
        }
        if (_BoardindPartyCount <= 1)
        {
            PostAchievement(16);
        }

        if (_HealingCount >= 300)
        {
            PostAchievement(18);
        }
        if (_EnemySuffocated)
        {
            PostAchievement(26);
        }
        if (_EnemyKilledByHands)
        {
            PostAchievement(27);
        }

        

        EndOfSectorAchievements();
        EndOfBattleReset();
    }

    public void EnemyKilled()
    {
        _KilledEnemiesCount++;

        if (_KilledEnemiesCount >= 100)
        {
            PostAchievement(19);
        }
    }

    public void DoorHeldByPlayer()
    {
        PostAchievement(23);
    }

    public void ShipSystemLevelIII(Helm.HelmType pHelmType)
    {
        switch (pHelmType)
        {
            case Helm.HelmType.Shields:
                _IsShieldUpgraded = true;
                PostAchievement(28);
                break;
            case Helm.HelmType.Weapons:
                _IsWeaponUpgraded = true;
                PostAchievement(11);
                break;
            case Helm.HelmType.Medical:
                _IsMedicalUpgraded = true;
                PostAchievement(5);
                break;
            case Helm.HelmType.Engineering:
                _IsEngineeringUpgraded = true;
                PostAchievement(25);
                break;
            case Helm.HelmType.Pilot:
                _IsPilotUpgraded = true;
                PostAchievement(24);
                break;
            case Helm.HelmType.Doors:
                _IsDoorUpgraded = true;
                PostAchievement(14);
                break;
            case Helm.HelmType.Sensor:
                _IsSensorUpgraded = true;
                PostAchievement(12);
                break;
            case Helm.HelmType.LifeSupport:
                _IsLifeSupportUpgraded = true;
                PostAchievement(10);
                break;
            default:
                break;
        }
        CheckForAllShipSystems();
    }

    

    private void CheckForAllShipSystems()
    {
        if (
                _IsPilotUpgraded &&
                _IsMedicalUpgraded &&
                _IsDoorUpgraded &&
                _IsSensorUpgraded &&
                _IsWeaponUpgraded &&
                _IsShieldUpgraded &&
                _IsLifeSupportUpgraded &&
                _IsEngineeringUpgraded
            )
        {
            PostAchievement(15);

            //Reach Starbase Alpha Fully Upgraded (1/6)
            if (_CurrentLevel <= 4) PostAchievement(6);
        }

    }

    private void EndOfSectorAchievements()
    {
        if (_CurrentLevel == 2)
        {
            PostAchievement(30);
        }
        if (_CurrentLevel == 6)
        {
            PostAchievement(31);
        }
        if (_CurrentLevel == 10)
        {
            PostAchievement(32);
        }
        if (_CurrentLevel == 13)
        {
            PostAchievement(33);
        }
        if (_IsFivePercentHull)
        {
            PostAchievement(7);
        }
    }

    public void EndOfGameAchievements()
    {
        if (_RanOutOfPower != false)
        {
            PostAchievement(4);
        }

        if (
                _IsPilotUpgraded ||
                _IsMedicalUpgraded ||
                _IsDoorUpgraded ||
                _IsSensorUpgraded ||
                _IsWeaponUpgraded ||
                _IsShieldUpgraded ||
                _IsLifeSupportUpgraded ||
                _IsEngineeringUpgraded
            )
        {
            //Not Eligible
        }
        else
        {
            PostAchievement(22);
        }

        if (!_ReloadedGame)
        {
            PostAchievement(25);
        }

        PostAchievement(29);
    }

    public void PostAchievement(int Value)
    {
        try
        {
            if (_MyAchievments[Value].Achieved == false)
            {
                SteamUserStats.SetAchievement(_MyAchievments[Value].Name);
                SteamUserStats.StoreStats();

                _MyAchievments[Value].Achieved = true;
            }
            
        }
        catch { Debug.LogWarning("Achievment Not Saved"); }
    }

    public bool IsAchievementAchieved(int Value)
    {
        return _MyAchievments[Value].Achieved;
    }

    public string GetAchievementName(int Value)
    {
        try
        {
            return _MyAchievments[Value].Name;
        }
        catch { return string.Empty; }
    }

    private void LoadAchievements()
    {
        MyAchievement _MyAchievement = new MyAchievement();

        _MyAchievement.Id = 0;
        _MyAchievement.Name = "ACHIEVEMENT_01";
        _MyAchievement.Achieved = false;
        _MyAchievments.Add(_MyAchievement);
        _MyAchievement.Id = 1;
        _MyAchievement.Name = "NEW_ACHIEVEMENT_1_1";
        _MyAchievement.Achieved = false;
        _MyAchievments.Add(_MyAchievement);
        _MyAchievement.Id = 2;
        _MyAchievement.Name = "NEW_ACHIEVEMENT_1_2";
        _MyAchievement.Achieved = false;
        _MyAchievments.Add(_MyAchievement);
        _MyAchievement.Id = 3;
        _MyAchievement.Name = "NEW_ACHIEVEMENT_1_3";
        _MyAchievement.Achieved = false;
        _MyAchievments.Add(_MyAchievement);
        _MyAchievement.Id = 4;
        _MyAchievement.Name = "NEW_ACHIEVEMENT_1_4";
        _MyAchievement.Achieved = false;
        _MyAchievments.Add(_MyAchievement);
        _MyAchievement.Id = 5;
        _MyAchievement.Name = "NEW_ACHIEVEMENT_1_5";
        _MyAchievement.Achieved = false;
        _MyAchievments.Add(_MyAchievement);
        _MyAchievement.Id = 6;
        _MyAchievement.Name = "NEW_ACHIEVEMENT_1_6";
        _MyAchievement.Achieved = false;
        _MyAchievments.Add(_MyAchievement);
        _MyAchievement.Id = 7;
        _MyAchievement.Name = "NEW_ACHIEVEMENT_1_7";
        _MyAchievement.Achieved = false;
        _MyAchievments.Add(_MyAchievement);
        _MyAchievement.Id = 8;
        _MyAchievement.Name = "NEW_ACHIEVEMENT_1_8";
        _MyAchievement.Achieved = false;
        _MyAchievments.Add(_MyAchievement);
        _MyAchievement.Id = 9;
        _MyAchievement.Name = "NEW_ACHIEVEMENT_1_9";
        _MyAchievement.Achieved = false;
        _MyAchievments.Add(_MyAchievement);
        _MyAchievement.Id = 10;
        _MyAchievement.Name = "NEW_ACHIEVEMENT_1_10";
        _MyAchievement.Achieved = false;
        _MyAchievments.Add(_MyAchievement);
        _MyAchievement.Id = 11;
        _MyAchievement.Name = "NEW_ACHIEVEMENT_1_11";
        _MyAchievement.Achieved = false;
        _MyAchievments.Add(_MyAchievement);
        _MyAchievement.Id = 12;
        _MyAchievement.Name = "NEW_ACHIEVEMENT_1_12";
        _MyAchievement.Achieved = false;
        _MyAchievments.Add(_MyAchievement);
        _MyAchievement.Id = 13;
        _MyAchievement.Name = "NEW_ACHIEVEMENT_1_13";
        _MyAchievement.Achieved = false;
        _MyAchievments.Add(_MyAchievement);
        _MyAchievement.Id = 14;
        _MyAchievement.Name = "NEW_ACHIEVEMENT_1_14";
        _MyAchievement.Achieved = false;
        _MyAchievments.Add(_MyAchievement);
        _MyAchievement.Id = 15;
        _MyAchievement.Name = "NEW_ACHIEVEMENT_1_15";
        _MyAchievement.Achieved = false;
        _MyAchievments.Add(_MyAchievement);
        _MyAchievement.Id = 16;
        _MyAchievement.Name = "NEW_ACHIEVEMENT_1_16";
        _MyAchievement.Achieved = false;
        _MyAchievments.Add(_MyAchievement);
        _MyAchievement.Id = 17;
        _MyAchievement.Name = "NEW_ACHIEVEMENT_1_17";
        _MyAchievement.Achieved = false;
        _MyAchievments.Add(_MyAchievement);
        _MyAchievement.Id = 18;
        _MyAchievement.Name = "NEW_ACHIEVEMENT_1_18";
        _MyAchievement.Achieved = false;
        _MyAchievments.Add(_MyAchievement);
        _MyAchievement.Id = 19;
        _MyAchievement.Name = "NEW_ACHIEVEMENT_1_19";
        _MyAchievement.Achieved = false;
        _MyAchievments.Add(_MyAchievement);
        _MyAchievement.Id = 20;
        _MyAchievement.Name = "NEW_ACHIEVEMENT_1_20";
        _MyAchievement.Achieved = false;
        _MyAchievments.Add(_MyAchievement);
        _MyAchievement.Id = 21;
        _MyAchievement.Name = "NEW_ACHIEVEMENT_1_21";
        _MyAchievement.Achieved = false;
        _MyAchievments.Add(_MyAchievement);
        _MyAchievement.Id = 22;
        _MyAchievement.Name = "NEW_ACHIEVEMENT_1_22";
        _MyAchievement.Achieved = false;
        _MyAchievments.Add(_MyAchievement);
        _MyAchievement.Id = 23;
        _MyAchievement.Name = "NEW_ACHIEVEMENT_1_23";
        _MyAchievement.Achieved = false;
        _MyAchievments.Add(_MyAchievement);
        _MyAchievement.Id = 24;
        _MyAchievement.Name = "NEW_ACHIEVEMENT_1_24";
        _MyAchievement.Achieved = false;
        _MyAchievments.Add(_MyAchievement);
        _MyAchievement.Id = 25;
        _MyAchievement.Name = "NEW_ACHIEVEMENT_1_25";
        _MyAchievement.Achieved = false;
        _MyAchievments.Add(_MyAchievement);
        _MyAchievement.Id = 26;
        _MyAchievement.Name = "NEW_ACHIEVEMENT_1_26";
        _MyAchievement.Achieved = false;
        _MyAchievments.Add(_MyAchievement);
        _MyAchievement.Id = 27;
        _MyAchievement.Name = "NEW_ACHIEVEMENT_1_27";
        _MyAchievement.Achieved = false;
        _MyAchievments.Add(_MyAchievement);
        _MyAchievement.Id = 28;
        _MyAchievement.Name = "NEW_ACHIEVEMENT_1_28";
        _MyAchievement.Achieved = false;
        _MyAchievments.Add(_MyAchievement);
        _MyAchievement.Id = 29;
        _MyAchievement.Name = "NEW_ACHIEVEMENT_1_29";
        _MyAchievement.Achieved = false;
        _MyAchievments.Add(_MyAchievement);
        _MyAchievement.Id = 30;
        _MyAchievement.Name = "NEW_ACHIEVEMENT_1_30";
        _MyAchievement.Achieved = false;
        _MyAchievments.Add(_MyAchievement);
        _MyAchievement.Id = 31;
        _MyAchievement.Name = "NEW_ACHIEVEMENT_1_31";
        _MyAchievement.Achieved = false;
        _MyAchievments.Add(_MyAchievement);
        _MyAchievement.Id = 32;
        _MyAchievement.Name = "NEW_ACHIEVEMENT_2_0";
        _MyAchievement.Achieved = false;
        _MyAchievments.Add(_MyAchievement);
        _MyAchievement.Id = 33;
        _MyAchievement.Name = "NEW_ACHIEVEMENT_2_1";
        _MyAchievement.Achieved = false;
        _MyAchievments.Add(_MyAchievement);
        _MyAchievement.Id = 34;
        _MyAchievement.Name = "NEW_ACHIEVEMENT_2_2";
        _MyAchievement.Achieved = false;
        _MyAchievments.Add(_MyAchievement);

        for (int i = 0; i < _MyAchievments.Count; i++)
        {
            Debug.Log("ID: " + _MyAchievments[i].Id);
            Debug.Log("Name: " + _MyAchievments[i].Name);
            Debug.Log("Achieve? " + _MyAchievments[i].Achieved);
        }
    }

    public void SaveData()
    {
        ES3.Save<List<MyAchievement>>(this.name + "MyAchievement", _MyAchievments);
    }

    public void LoadData()
    {
        _MyAchievments = ES3.Load<List<MyAchievement>>(this.name + "MyAchievement");
    }

}

public class MyAchievement
{
    public int Id;
    public string Name;
    public bool Achieved;
}
