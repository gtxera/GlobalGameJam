using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class OnCollisionDamage : MonoBehaviour, IDamageDealer
{
    [SerializeField] private int _damage;
    [SerializeField] private UnityEvent _onDamageEvent;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerInput>(out _))
        {
            var healthBehaviour = collision.gameObject.GetComponent<HealthBehaviour>();
            healthBehaviour.ReceiveDamage(DoDamage());
            _onDamageEvent?.Invoke();
        }
    }

    public int DoDamage()
    {
        return _damage;
    }
}
