using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ReflectRays : MonoBehaviour
{
    const int Infinity = 999;

    int maxReflections = 100;
    int currentReflections = 0;

    [SerializeField]
    Vector2 startPoint, direction;
    List<Vector3> Points;
    int defaultRayDistance = 100;
    LineRenderer lr;

    // Use this for initialization
    void Start()
    {
        Points = new List<Vector3>();
        lr = transform.GetComponent<LineRenderer>();
    }

    private void Update()
    {
        var hitData = Physics2D.Raycast(startPoint, (direction - startPoint).normalized, defaultRayDistance);

        currentReflections = 0;
        Points.Clear();
        Points.Add(startPoint);

        if (hitData)
        {
            ReflectFurther(startPoint, hitData);
            Debug.Log("Hit");
        }
        else
        {
            Points.Add(startPoint + (direction - startPoint).normalized * Infinity);
            Debug.Log("Didnt Hit");
        }

        lr.positionCount = Points.Count;
        lr.SetPositions(Points.ToArray());
        //Debug.Log("Hit");
    }

    private void ReflectFurther(Vector2 origin, RaycastHit2D hitData)
    {
        if (currentReflections > maxReflections) return;

        Points.Add(hitData.point);
        currentReflections++;

        Vector2 inDirection = (hitData.point - origin).normalized;
        Vector2 newDirection = Vector2.Reflect(inDirection, hitData.normal);

        var newHitData = Physics2D.Raycast(hitData.point + (newDirection * 0.0001f), newDirection * 100, defaultRayDistance);
        if (newHitData)
        {
            ReflectFurther(hitData.point, newHitData);
        }
        else
        {
            Points.Add(hitData.point + newDirection * defaultRayDistance);
        }
    }
}