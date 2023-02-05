using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathfinder : MonoBehaviour
{
    [SerializeField] private List<Transform> _pathTransforms;
    public List<Vector2> PathPoints { get; private set; }
    
    void Start()
    {
        foreach (var pathTransform in _pathTransforms)
        {
            PathPoints.Add(pathTransform.position);
        }
    }
}
