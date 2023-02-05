using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathfinder : MonoBehaviour
{
    [SerializeField] private List<Transform> _pathTransforms;
    public List<Vector2> PathPoints = new List<Vector2>();
    
    void Start()
    {
        foreach (var pathTransform in _pathTransforms)
        {
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
