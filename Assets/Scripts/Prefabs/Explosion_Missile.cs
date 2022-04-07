using UnityEngine;

public class Explosion_Missile : MonoBehaviour
{
    //Bullet is a general term for an object that
    //is used to transfer damage to another object.
    
    [SerializeField] private string _Name;
    [SerializeField] private float _Damage;
    [SerializeField] private AudioClip _AudioClip;

    private void Start()
    {
        //AudioSource.PlayClipAtPoint(_AudioClip, transform.position, 1.0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Explosion Trigger Enter Fired!");

        IDamageable[] _Damageable = collision.GetComponents<IDamageable>();

        for (int i = 0; i < _Damageable.Length; i++)
        {
            if (_Damageable[i] != null)
            {
                //Debug.Log(_Damageable[i].ToString());

                _Damageable[i].Damage(_Damage, false, false,true);
            }
        }
    }
}
