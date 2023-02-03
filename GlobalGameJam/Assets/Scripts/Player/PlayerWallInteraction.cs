using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallInteraction : MonoBehaviour
{
    [SerializeField] private float _extraRaycastSize;
    [SerializeField] private LayerMask _layerMask;
    
    public bool IsCollidingWithWall { get; private set; }
    public int ContactDirection { get; private set; }

    private Vector3 _raycastOffsetLeftUp;
    private Vector3 _raycastOffsetLeftDown;
    private Vector3 _raycastOffsetRightUp;
    private Vector3 _raycastOffsetRightDown;

    private void Start()
    {
        var colliderSize = GetComponent<BoxCollider2D>().size;
        _raycastOffsetLeftUp = new Vector2(-colliderSize.x / 2, colliderSize.y / 2);
        _raycastOffsetLeftDown = new Vector2(-colliderSize.x / 2, -colliderSize.y / 2);
        _raycastOffsetRightUp = new Vector2(colliderSize.x / 2, colliderSize.y / 2);
        _raycastOffsetRightDown = new Vector2(colliderSize.x / 2, -colliderSize.y / 2);
    }

    private struct WallRaycastResults
    {
        public bool Collided;
        public int CollisionDirection;
    }

    private void FixedUpdate()
    {
        var raycastResults = WallRaycast();
        IsCollidingWithWall = raycastResults.Collided;
        ContactDirection = raycastResults.CollisionDirection;
        
    }

    private WallRaycastResults WallRaycast()
    {
        RaycastHit2D leftHitUp = (Physics2D.Raycast(transform.position + _raycastOffsetLeftUp, transform.TransformDirection(Vector3.left), _extraRaycastSize, _layerMask));
        RaycastHit2D leftHitDown = (Physics2D.Raycast(transform.position + _raycastOffsetLeftDown, transform.TransformDirection(Vector3.left), _extraRaycastSize,_layerMask));
        RaycastHit2D rightHitUp = (Physics2D.Raycast(transform.position + _raycastOffsetRightUp, transform.TransformDirection(Vector3.right), _extraRaycastSize,_layerMask));
        RaycastHit2D rightHitDown = (Physics2D.Raycast(transform.position + _raycastOffsetRightDown, transform.TransformDirection(Vector3.right), _extraRaycastSize,_layerMask));

        if (leftHitUp && leftHitDown)
        {
            if (leftHitUp.transform.CompareTag("Ground") && leftHitDown.transform.CompareTag("Ground")) return new WallRaycastResults() { Collided = true, CollisionDirection = -1 }; 
        }
        
        else if (rightHitUp && rightHitDown)
        {
            if (rightHitUp.transform.CompareTag("Ground") && rightHitDown.transform.CompareTag("Ground")) return new WallRaycastResults() { Collided = true, CollisionDirection = 1 }; 
        }

        return new WallRaycastResults(){Collided = false, CollisionDirection = 0}; 
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position + _raycastOffsetLeftUp, transform.position + _raycastOffsetLeftUp + _extraRaycastSize * transform.TransformDirection(Vector3.left));
        Gizmos.DrawLine(transform.position + _raycastOffsetLeftDown, transform.position + _raycastOffsetLeftDown + _extraRaycastSize * transform.TransformDirection(Vector3.left));
        Gizmos.DrawLine(transform.position + _raycastOffsetRightUp, transform.position + _raycastOffsetRightUp + _extraRaycastSize * transform.TransformDirection(Vector3.right));
        Gizmos.DrawLine(transform.position + _raycastOffsetRightDown, transform.position + _raycastOffsetRightDown + _extraRaycastSize * transform.TransformDirection(Vector3.right));
    }
}
