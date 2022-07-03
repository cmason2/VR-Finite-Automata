using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System;

[RequireComponent(typeof(LineRenderer))]
public class Bezier : MonoBehaviour
{
    private int SEGMENT_COUNT = 50;

    public Camera mainCamera;
    public AutomataController automataController;
    public Transform[] controlPoints;
    public LineRenderer lineRenderer;
    public GameObject arrowHead;
    private Transform initialState;
    private Transform targetState;
    private SphereCollider targetCollider;
    private List<string> symbols;
    public TMP_Text symbolText;
    public float symbolOffsetDistance = 0.1f;
    public Color edgeColour;
    
    public int numGrabs = 0;

    List<Vector3> positions;

    void Start()
    {
        automataController = FindObjectOfType<AutomataController>();
        mainCamera = FindObjectOfType<Camera>();

        SetColour(edgeColour);

        if (!lineRenderer)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        SetSymbol(symbolText.text);
        
        if (controlPoints[0] != null && controlPoints[2] != null)
        {
            initialState = controlPoints[0];
            targetState = controlPoints[2];
        }

        targetCollider = targetState.GetComponentInChildren<SphereCollider>();
        arrowHead.SetActive(true);
    }

    void Update()
    {
        if(initialState != targetState)
        {
            CalculatePoints();
            DrawCurve();
            DrawSymbol();
        }
        else
        {
            DrawCircle();
        }
    }

    private void OnDestroy()
    {
        if (initialState != null && targetState != null)
        {
            State s1 = initialState.GetComponent<State>();
            State s2 = targetState.GetComponent<State>();

            // Remove edge from list of connected edges on each state
            if (s1 != null)
            {
                automataController.DeleteTransition(s1, this);
                s1.DeleteEdge(this);
            }

            if (s2 != null)
                s2.DeleteEdge(this);
        }
    }

    void CalculatePoints()
    {
        positions = new List<Vector3>();

        for (int i = 0; i < SEGMENT_COUNT; i++)
        {
            float t = i / (float)SEGMENT_COUNT;
            Vector3 pixel = CalculateQuadraticBezierPoint(t, initialState.position, controlPoints[1].position, targetState.position);

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

    private void DrawCurve()
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

            // Move Arrowhead to correct position
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

    private void DrawCircle()
    {

        positions = new List<Vector3>();

        float stateRadius = targetCollider.transform.localScale.x / 2;
        Vector3 direction = (controlPoints[1].position - initialState.position);
        direction.Normalize();

        transform.position = targetState.position + 0.2f * direction;
        transform.LookAt(controlPoints[1].position);

        for (int i = 0; i <= SEGMENT_COUNT+1; i++)
        {
            float radian = ((float) i / SEGMENT_COUNT) * 2 * ((float)Math.PI) - 0.5f * ((float)Math.PI);
            float x = Mathf.Cos(radian);
            float z = Mathf.Sin(radian);

            //x *= circleRadius;
            //y *= circleRadius;

            x *= 0.1f;
            z *= 0.2f;

            //x += initialState.position.x + direction.x * (circleRadius + stateRadius);
            //y += stateRadius;

            //positions.Add(new Vector3(x, y, initialState.position.z));
            Vector3 point = new Vector3(x, 0, z);

            if (Vector3.Distance(transform.TransformPoint(point), initialState.position) > stateRadius)
            {
                positions.Add(point);
            }
        }

        Vector3 firstPoint = positions[0];
        Vector3 lastPoint = positions[positions.Count - 1];

        // Join start of curve to state
        Vector3 stateDirection = firstPoint - positions[1];
        float distance = stateDirection.magnitude;
        stateDirection.Normalize();
        positions.RemoveAt(0);

        arrowHead.transform.position = transform.TransformPoint(firstPoint + stateDirection * 0.02f);
        arrowHead.transform.rotation = Quaternion.LookRotation(transform.TransformVector(stateDirection));

        // Join end of curve to state
        stateDirection = lastPoint - positions[positions.Count - 2];
        distance = stateDirection.magnitude;
        stateDirection.Normalize();
        positions.Add(lastPoint + stateDirection * 0.02f);

        lineRenderer.positionCount = positions.Count;
        lineRenderer.SetPositions(positions.ToArray());

        //transform.root.SetParent(targetState.transform, false);
        //transform.LookAt(mainCamera.transform);

        Vector3 symbolPoint = targetState.position + direction * (stateRadius + 0.35f);
        symbolText.transform.position = symbolPoint;
    }

    void DrawSymbol()
    {
        Vector3 middlePoint = CalculateQuadraticBezierPoint(0.5f, initialState.position, controlPoints[1].position, targetState.position);
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
        //controlPoints[0] = s1;
        //controlPoints[2] = s2;
        initialState = s1;
        targetState = s2;
    }

    public void SetSymbol(string symbol)
    {
        symbolText.SetText(symbol);
        symbols = new List<string>();
        foreach (string sym in symbol.Split(','))
        {
            symbols.Add(sym);
        }
    }

    public string GetSymbolText()
    {
        return symbolText.text;
    }

    public List<string> GetSymbolList()
    {
        return symbols;
    }

    public State GetSourceState()
    {
        return initialState.GetComponent<State>();
    }

    public State GetTargetState()
    {
        return targetState.GetComponent<State>();
    }

    public void SetColour(Color colour)
    {
        lineRenderer.material.color = colour;
        arrowHead.GetComponentInChildren<Renderer>().material.color = colour;
        symbolText.color = colour;
    }
}