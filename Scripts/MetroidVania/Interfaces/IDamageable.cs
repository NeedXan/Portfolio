using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable : IPogoable, IHitable
{
    void TakeDamage(int damage);
    void Die();
}
