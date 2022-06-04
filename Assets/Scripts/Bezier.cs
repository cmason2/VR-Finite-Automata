using UnityEngine;
using System.Collections.Generic;
using TMPro;

[RequireComponent(typeof(LineRenderer))]
public class Bezier : MonoBehaviour
{
    public Transform[] controlPoints;
    public LineRenderer lineRenderer;
    private int SEGMENT_COUNT = 50;
    public GameObject arrowHead;
    private Transform initialState;
    private Transform targetState;
    private SphereCollider targetCollider;
    public TMP_Text symbolText;
    public float symbolOffsetDistance = 0.1f;

    List<Vector3> positions;

    void Start()
    {
        if (!lineRenderer)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        initialState = controlPoints[0];
        targetState = controlPoints[2];
        targetCollider = targetState.GetComponentInChildren<SphereCollider>();
        arrowHead.SetActive(true);
    }

    void Update()
    {
        CalculatePoints();
        DrawCurve();
        DrawSymbol();
    }

    void CalculatePoints()
    {
        positions = new List<Vector3>();

        for (int i = 0; i < SEGMENT_COUNT; i++)
        {
            float t = i / (float)SEGMENT_COUNT;
            Vector3 pixel = CalculateQuadraticBezierPoint(t, controlPoints[0].position, controlPoints[1].position, controlPoints[2].position);

            // Check if any points on curve are inside the states' volume
            float stateRadius = targetCollider.transform.localScale.x / 2;
            if (Vector3.Distance(pixel, initialState.position) > stateRadius &&
                Vector3.Distance(pixel, targetState.position) > stateRadius)
            {
                positions.Add(pixel);
            }
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

    void DrawCurve()
    {
        if (positions.Count != 0)
        {
            Vector3 firstPoint = positions[0];
            Vector3 lastPoint = positions[positions.Count - 1];

            float stateRadius = targetCollider.transform.localScale.x / 2;

            // Join curve to initial state
            Vector3 direction = initialState.position - firstPoint;
            float distance = direction.magnitude;
            direction.Normalize();
            positions.Insert(0, firstPoint + direction * (distance - stateRadius));

            // Join curve to target state
            direction = targetState.position - lastPoint;
            distance = direction.magnitude;
            direction.Normalize();
            positions.Add(lastPoint + direction * (distance - stateRadius));
                
            // Move arrowhead to correct position
            //if (arrowHead == null)
            //    arrowHead = Instantiate(arrowHeadPrefab, positions[positions.Count - 1], Quaternion.Euler(direction));
            //else
            //    arrowHead.transform.SetPositionAndRotation(positions[positions.Count - 1], Quaternion.Euler(direction));

            arrowHead.transform.SetPositionAndRotation(positions[positions.Count - 1], Quaternion.Euler(direction));
            arrowHead.transform.LookAt(targetState.transform.position);

            // Draw the curve
            lineRenderer.positionCount = positions.Count;
            lineRenderer.SetPositions(positions.ToArray());

        }
        else
        {
            Debug.Log("Couldn't find point outside destination state");
        }
    }

    void DrawSymbol()
    {
        Vector3 middlePoint = CalculateQuadraticBezierPoint(0.5f, controlPoints[0].position, controlPoints[1].position, controlPoints[2].position);
        Vector3 direction = controlPoints[1].position - middlePoint;

        //if (direction.magnitude < symbolOffsetDistance)
        //{
        //    Debug.Log("Too Close!");
        //}
        direction.Normalize();

        symbolText.transform.position = middlePoint + (direction * symbolOffsetDistance);
    }

    public void SetStates(Transform s1, Transform s2)
    {
        controlPoints[0] = s1;
        controlPoints[2] = s2;
    }

    public void SetSymbol(string symbol)
    {
        symbolText.SetText(symbol);
    }

    public string GetSymbol()
    {
        return symbolText.text;
    }
}