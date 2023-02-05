using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour, IDamageDealer
{
    [SerializeField] private int _attackDamage;
    [SerializeField] private float _attackDurationInSeconds;
    [SerializeField] private BoxCollider2D _attackCollider;

    private Coroutine _attackRoutine;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<HealthBehaviour>(out var healthBehaviour))
        {
            healthBehaviour.ReceiveDamage(DoDamage());
        }
    }

    public int DoDamage()
    {
        return _attackDamage;
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        
        _attackRoutine ??= StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        _attackCollider.enabled = true;

        yield return new WaitForSeconds(_attackDurationInSeconds);

        _attackCollider.enabled = false;
    }
}
