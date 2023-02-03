using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerGroundedManager))]
[RequireComponent(typeof(PlayerWallInteraction))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _maxMovementSpeed;

    [Header("De/Acceleration")]
    [SerializeField] private int _timeToAccelerateInFrames;
    [SerializeField] private int _timeToDecelerateInFrames;

    [Header("Wall Jump")]
    [SerializeField] private int _wallJumpLockTimeInFrames;

    private Rigidbody2D _rb2d;
    private PlayerGroundedManager _playerGroundedManager;
    private PlayerWallInteraction _playerWallInteraction;
    private PlayerInput _playerInput;

    private float _currentSpeed;
    private float _currentDirection;

    private float _accelerationSpeed;
    private float _decelerationSpeed;

    private Coroutine _accelerationCoroutine;
    private Coroutine _decelerationCoroutine;

    private bool _hasWallJump;

    private void Start()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _playerWallInteraction = GetComponent<PlayerWallInteraction>();
        _playerGroundedManager = GetComponent<PlayerGroundedManager>();
        _playerInput = GetComponent<PlayerInput>();

        _accelerationSpeed = _maxMovementSpeed / _timeToAccelerateInFrames;
        _decelerationSpeed = _maxMovementSpeed / _timeToDecelerateInFrames;
    }

    public void Movement(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _currentDirection = context.ReadValue<Vector2>().x;

            if(_currentDirection == 0)
            {
                _decelerationCoroutine = StartCoroutine(Deceleration(Mathf.Sign(_currentSpeed)));
                return;
            }

            if (_accelerationCoroutine != null) StopCoroutine(_accelerationCoroutine);

            _accelerationCoroutine = StartCoroutine(Acceleration(_currentDirection));
        }

        if(context.canceled)
        {
            if (_playerWallInteraction.IsCollidingWithWall) return;

            _decelerationCoroutine = StartCoroutine(Deceleration(_currentDirection));
            _currentDirection = 0;
        }
    }

    private IEnumerator Acceleration(float direction)
    {
        if (_currentSpeed != 0)
        {
            if (_decelerationCoroutine != null) StopCoroutine(_decelerationCoroutine);

            _decelerationCoroutine = StartCoroutine(Deceleration(direction));
            yield return _decelerationCoroutine;
        }

        for(int i = 0; i < _timeToAccelerateInFrames; i++)
        {
            _currentSpeed += _accelerationSpeed * direction;

            if (Mathf.Abs(_currentSpeed) > Mathf.Abs(_maxMovementSpeed))
            {
                _currentSpeed = _maxMovementSpeed * direction;

                _accelerationCoroutine = null;

                yield break;
            }

            yield return new WaitForFixedUpdate();
        }

        _accelerationCoroutine = null;
    }

    private IEnumerator Deceleration(float direction)
    {
        for(int i = 0; i < _timeToDecelerateInFrames; i++)
        {
            _currentSpeed -= _decelerationSpeed * direction;

            if((_currentSpeed < 0 && direction > 0) || (_currentSpeed > 0 && direction < 0))
            {
                _currentSpeed = 0;

                _decelerationCoroutine = null;

                yield break;
            }

            yield return new WaitForFixedUpdate();
        }

        _decelerationCoroutine = null;
    }

    private void FixedUpdate()
    {
        if (_playerWallInteraction.IsCollidingWithWall && !_playerGroundedManager.IsGrounded)
        {
            if(_currentDirection == _playerWallInteraction.ContactDirection) 
            {
                _rb2d.velocity = new Vector2(0, _rb2d.velocity.y);
                _hasWallJump = true;
                return;
            }
            
        }

        else
        {
            _hasWallJump = false;
        }

        _rb2d.velocity = new Vector2(_currentSpeed, _rb2d.velocity.y);
    }

    public void WallJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_playerWallInteraction.IsCollidingWithWall && _hasWallJump)
            {
                _currentDirection = -_playerWallInteraction.ContactDirection;
                _currentSpeed = _maxMovementSpeed * _currentDirection;
                _hasWallJump = false;
                StartCoroutine(WallJumpLock());
            }
        }
    }

    private IEnumerator WallJumpLock()
    {
        for (int i = 0; i < _wallJumpLockTimeInFrames; i++)
        {
            yield return new WaitForFixedUpdate();
        }

        var currentInput = _playerInput.actions["movement"].ReadValue<Vector2>().x;

        if (currentInput != _currentDirection)
        {
            _currentDirection = currentInput;
            _accelerationCoroutine = StartCoroutine(Acceleration(_currentDirection));
        }
    }
}
