using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D))] 
[RequireComponent(typeof(PlayerGroundedManager))]
[RequireComponent(typeof(PlayerWallInteraction))]
public class PlayerJump : MonoBehaviour
{
    [Header("Ground Jump")] 
    [SerializeField] private float _maxGroundJumpHeight;
    [SerializeField] private float _minGroundJumpHeight;
    [SerializeField] private float _timeToReachGroundJumpApexInSeconds;
    
    [Header("Double Jump")]
    [SerializeField] private float _maxDoubleJumpHeight;
    [SerializeField] private float _minDoubleJumpHeight;

    [Header("Wall Jump")]
    [SerializeField] private float _maxWallJumpHeight;
    [SerializeField] private float _minWallJumpHeight;

    [Header("Balancing")] 
    [SerializeField] private float _coyoteTimeInFrames;
    [SerializeField] private int _inputGraceTimeInFrames;

    private Rigidbody2D _rb2d;
    private PlayerGroundedManager _playerGroundedManager;
    private PlayerWallInteraction _playerWallInteraction;

    private float _groundJumpVelocity;
    private float _doubleJumpVelocity;
    private float _wallJumpVelocity;
    private float _terminatedGroundJumpVelocity;
    private float _terminatedDoubleJumpVelocity;
    private float _terminatedWallJumpVelocity;

    private Coroutine _coyoteTimeCoroutine;
    private bool _coyoteTimeExpired;
    private bool _hasCoyoteTime;

    private bool _groundJumpTerminated;
    private bool _doubleJumpTerminated;
    private bool _wallJumpTerminated;
    private bool _canGroundJump;
    private bool _canDoubleJump;
    private bool _canWallJump;

    private void Start()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _playerGroundedManager = GetComponent<PlayerGroundedManager>();
        _playerWallInteraction = GetComponent<PlayerWallInteraction>();

        var gravityScale = 2 * _maxGroundJumpHeight / (_timeToReachGroundJumpApexInSeconds * _timeToReachGroundJumpApexInSeconds);
        _rb2d.gravityScale = gravityScale;
        
        _groundJumpVelocity = Mathf.Sqrt(2 * gravityScale * _maxGroundJumpHeight);
        _terminatedGroundJumpVelocity = Mathf.Sqrt(_groundJumpVelocity * _groundJumpVelocity + 2 * -gravityScale * (_maxGroundJumpHeight - _minGroundJumpHeight));

        _doubleJumpVelocity = Mathf.Sqrt(2 * gravityScale * _maxDoubleJumpHeight);
        _terminatedDoubleJumpVelocity = Mathf.Sqrt(_doubleJumpVelocity * _doubleJumpVelocity + 2 * -gravityScale * (_maxDoubleJumpHeight - _minDoubleJumpHeight));

        _wallJumpVelocity = Mathf.Sqrt(2 * gravityScale * _maxWallJumpHeight);
        _terminatedWallJumpVelocity = Mathf.Sqrt(_wallJumpVelocity * _wallJumpVelocity + 2 * -gravityScale * (_maxWallJumpHeight - _minWallJumpHeight));
    }

    public void JumpViaInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_canWallJump)
            {
                _rb2d.velocity = new Vector2(_rb2d.velocity.x, _wallJumpVelocity);
                _hasCoyoteTime = false;
                _canGroundJump = false;
            }

            else if (_canGroundJump)
            {
                _rb2d.velocity = new Vector2(_rb2d.velocity.x, _groundJumpVelocity);
                _hasCoyoteTime = false;
                _canGroundJump = false;
                if(_coyoteTimeCoroutine != null) StopCoroutine(_coyoteTimeCoroutine);
            }
            
            else if (_canDoubleJump)
            {
                _rb2d.velocity = new Vector2(_rb2d.velocity.x, _doubleJumpVelocity);
                _canDoubleJump = false;
            }
            
            else StartCoroutine(InputGraceTime());
        }
        
        else if (context.canceled)
        {
            if (!_groundJumpTerminated)
            {
                var velocityY = Mathf.Min(_rb2d.velocity.y, _terminatedGroundJumpVelocity);
                _rb2d.velocity = new Vector2(_rb2d.velocity.x, velocityY);
                _groundJumpTerminated = true;
            }
            
            else if (!_doubleJumpTerminated)
            {
                var velocityY = Mathf.Min(_rb2d.velocity.y, _terminatedDoubleJumpVelocity);
                _rb2d.velocity = new Vector2(_rb2d.velocity.x, velocityY);
                _doubleJumpTerminated = true;
            }
        }
    }

    private IEnumerator InputGraceTime()
    {
        for (var i = 0; i < _inputGraceTimeInFrames; i++)
        {
            if (_playerGroundedManager.IsGrounded)
            {
                JumpViaGraceTime();
                yield break;
            }

            yield return new WaitForFixedUpdate();
        }
    }

    private void JumpViaGraceTime()
    {
        _rb2d.velocity = new Vector2(_rb2d.velocity.x, _groundJumpVelocity);
    }

    private void FixedUpdate()
    {
        HandleGroundDetectionAndCoyoteTime();
        HandleWallDetection();
    }

    private void HandleGroundDetectionAndCoyoteTime()
    {
        if (_playerGroundedManager.IsGrounded)
        {
            _groundJumpTerminated = false;
            _doubleJumpTerminated = false;

            _canGroundJump = true;
            _canDoubleJump = true;

            _coyoteTimeExpired = false;
            _hasCoyoteTime = true;
        }

        else
        {
            if (_coyoteTimeCoroutine == null && !_coyoteTimeExpired && _hasCoyoteTime)
            {
                _coyoteTimeCoroutine = StartCoroutine(CoyoteTime());
            }

            if (_coyoteTimeExpired)
            {
                _canGroundJump = false;
            }
        }
    }

    private void HandleWallDetection()
    {
        if (_playerWallInteraction.IsCollidingWithWall && !_playerGroundedManager.IsGrounded)
        {
            _canWallJump = true;
            return;
        }

        _canWallJump = false;
    }

    private IEnumerator CoyoteTime()
    {
        for (var i = 0; i < _coyoteTimeInFrames; i++)
        {
            yield return new WaitForFixedUpdate();
        }

        _canGroundJump = false;
        _coyoteTimeExpired = true;

        _coyoteTimeCoroutine = null;
    }
}
