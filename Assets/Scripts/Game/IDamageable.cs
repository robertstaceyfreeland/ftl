using UnityEngine;

public interface IDamageable
{
    void Damage(float Amount, bool IsBullet, bool NoDamageToPlayer, bool IsExplosion);
    void LowOxygen(float Amount);
}
