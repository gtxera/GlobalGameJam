using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthBehaviour : MonoBehaviour, IDamageReceiver
{
    [SerializeField] private int _maxHealth;
    private int _currentHealth;

    public int MaxHealth => _maxHealth;
    public int CurrentHealth => _currentHealth;

    [SerializeField] private UnityEvent _onDeath, _onDamage;
    
    private void Start()
    {
        _currentHealth = _maxHealth;
    }

    public void ReceiveDamage(int damage)
    {
        _currentHealth -= damage;
        _onDamage?.Invoke();
        
        if(_currentHealth <= 0) _onDeath?.Invoke();
    }
}
