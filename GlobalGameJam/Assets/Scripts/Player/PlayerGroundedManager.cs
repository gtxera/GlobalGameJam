using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerGroundedManager : MonoBehaviour
{
    [Header("Grounded Raycast")]
    [SerializeField] private float _extraRaycastSize;
    [SerializeField] private LayerMask _raycastLayerMask;
    
    public bool IsGrounded { get; private set; }

    private SpriteRenderer _spriteRenderer;
    private float _spriteHalfSize;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        var sprite = _spriteRenderer.sprite;
        _spriteHalfSize = (sprite.texture.height / sprite.pixelsPerUnit) / 2;
    }

    private bool IsGroundedRaycast()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector3.down), _spriteHalfSize + _extraRaycastSize, _raycastLayerMask);

        if (!hit) return false;

        return hit.transform.CompareTag("Ground");
    }
    
    private void FixedUpdate()
    {
        IsGrounded = IsGroundedRaycast();
    }
}
