using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;

    float currentHealth = 0f;
    float damagePerHit = 20f;

    private void Awake()
    {
        ResetHealth();
    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }

    public float GetHealth()
    {
        return currentHealth;
    }

    private void ResetHealth()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage()
    {
        currentHealth -= damagePerHit;
    }
}
