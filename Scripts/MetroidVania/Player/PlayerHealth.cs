using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] int maxHealth;
    [SerializeField] public int health;// { get; private set; }

    private void Start()
    {
        health = maxHealth;
    }

    public  void TakeDamage(int damage, bool isEnvironment)
    {
        if (isEnvironment)
            Respawn();

        if (health - damage > 0)
            health -= damage;
        else
        {
            health -= damage;
            Die();
        }
    }
    void IDamageable.TakeDamage(int damage)
    {
        if (health - damage > 0)
            health -= damage;
        else
            Die();
    }
    public void Die()
    {
        Debug.Log("Dead");
    }
    public void Respawn()
    {

    }
}
