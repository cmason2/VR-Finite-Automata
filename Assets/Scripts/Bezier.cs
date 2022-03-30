using UnityEngine;
using System.Collections.Generic;
[RequireComponent(typeof(LineRenderer))]
public class Bezier : MonoBehaviour
{
    public Transform[] controlPoints;
    public LineRenderer lineRenderer;
    private int SEGMENT_COUNT = 50;

    void Start()
    {
        if (!lineRenderer)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
        
        lineRenderer.positionCount = SEGMENT_COUNT;
        lineRenderer.material.color = Color.black;
    }

    void Update()
    {

        DrawCurve();

    }

    void DrawCurve()
    {
        for (int i = 0; i < SEGMENT_COUNT; i++)
        {
            float t = i / (float)SEGMENT_COUNT;
            Vector3 pixel = CalculateQuadraticBezierPoint(t, controlPoints[0].position, controlPoints[1].position, controlPoints[2].position);
            lineRenderer.SetPosition((i), pixel);
        }
    }

    Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;

        return p;
    }
}