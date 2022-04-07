using UnityEngine;

public class HealingStation : MonoBehaviour
{
    public Helm _Helm;
    public SpriteRenderer _HealingPad;
    private float _StartingHealAmount = 1.25f;
    private float _CurrentHealAmount = 1.25f;
    private Player _Player;
    private float _NextHealTime = 0;
    private bool _HealPlayer = true;


    private void Start()
    {
        _Helm._DamageSystem.OnStoppedWorking += _DamageSystem_OnStoppedWorking;
        _Helm._DamageSystem.OnStartedWorking += _DamageSystem_OnStartedWorking;
        _Player = GameObject.FindObjectOfType<Player>();
    }

    private void Update()
    {
        if(_NextHealTime< Time.time)
        {
            _HealPlayer = true;
            _NextHealTime = Time.time + .25f;
        }
    }

    public void SetHealModifier(float pAmount)
    {
        _CurrentHealAmount = _StartingHealAmount + (pAmount);
    }

    private void _DamageSystem_OnStartedWorking(object sender, System.EventArgs e)
    {
        Color PadColor = Color.green;
        PadColor.a = .5f;

        _HealingPad.color = PadColor;
    }

    private void _DamageSystem_OnStoppedWorking(object sender, System.EventArgs e)
    {
        Color PadColor = Color.red;
        PadColor.a = .5f;
        _HealingPad.color = PadColor;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (_HealPlayer)
            {
                if (_Helm._IsWorking)
                {
                    _Player.Heal(_CurrentHealAmount);
                    _Player._HealthSystem.AddStamina(_CurrentHealAmount);

                    SoundManager.Instance.PlaySound(SoundManager.Sound.Medical, transform.position, SoundManager.AudioMixerGroupName.Sfx);
                }
                else
                {
                    _Player.Damage(_StartingHealAmount, false, false,false);
                    _Player._HealthSystem.SubtractStamina(_StartingHealAmount);

                    SoundManager.Instance.PlaySound(SoundManager.Sound.Human_Grunt, transform.position, SoundManager.AudioMixerGroupName.Player);
                }

                _HealPlayer = false;
            }
            
        }
    }


}
