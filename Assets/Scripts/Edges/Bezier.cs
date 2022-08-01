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
    public LineRenderer lineRenderer;
    public GameObject arrowHead;
    public Transform initialState;
    public Transform targetState;
    public TMP_Text symbolText;
    public float symbolOffsetDistance = 0.1f;
    public Color edgeColour;

    public int numGrabs = 0;

    private SphereCollider targetCollider;
    private List<char> symbols;
    private Vector3 controlPoint;
    private bool isLoop = false;

    List<Vector3> positions;

    void Start()
    {
        automataController = FindObjectOfType<AutomataController>();
        mainCamera = FindObjectOfType<Camera>();

        //SetColour(edgeColour);

        if (!lineRenderer)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        SetSymbol(symbolText.text);

        if (initialState == targetState)
        {
            isLoop = true;
        }

        targetCollider = targetState.GetComponentInChildren<SphereCollider>();
        arrowHead.SetActive(true);
    }

    void LateUpdate()
    {
        if(!isLoop)
        {
            CalculatePoints();
            DrawCurve();
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

            if (s2 != null && s2 != s1)
                s2.DeleteEdge(this);
        }
    }

    void CalculatePoints()
    {
        Vector3 directionToLine = (initialState.position - symbolText.transform.position) + (targetState.position - symbolText.transform.position);
        Vector3 curveMidpoint = symbolText.transform.position + symbolOffsetDistance * directionToLine.normalized;

        // Calculate control point position from symbol position
        controlPoint = 2 * curveMidpoint - 0.5f * (initialState.position + targetState.position);
        
        positions = new List<Vector3>();

        for (int i = 0; i < SEGMENT_COUNT; i++)
        {
            float t = i / (float)SEGMENT_COUNT;
            Vector3 pixel = CalculateQuadraticBezierPoint(t, initialState.position, controlPoint, targetState.position);

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
        Vector3 direction = (symbolText.transform.position - initialState.position);
        direction.Normalize();

        transform.position = targetState.position + 0.2f * direction;
        transform.LookAt(symbolText.transform.position);

        for (int i = 0; i <= SEGMENT_COUNT+1; i++)
        {
            float radian = ((float) i / SEGMENT_COUNT) * 2 * ((float)Math.PI) - 0.5f * ((float)Math.PI);
            float x = Mathf.Cos(radian);
            float z = Mathf.Sin(radian);

            x *= 0.1f;
            z *= 0.2f; // Oval rather than circle

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

        Vector3 symbolPoint = targetState.position + direction * (stateRadius + 0.35f);
        symbolText.transform.position = symbolPoint;
    }

    public void SetStates(Transform s1, Transform s2)
    {
        initialState = s1;
        targetState = s2;
    }

    public void SetSymbol(string symbol)
    {
        symbolText.SetText(symbol);
        symbols = new List<char>(symbol.Replace(",", "").ToCharArray());
    }

    public string GetSymbolText()
    {
        return symbolText.text;
    }

    public List<char> GetSymbolList()
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

    public void HideEdge()
    {
        arrowHead.gameObject.SetActive(false);
        symbolText.gameObject.SetActive(false);
        lineRenderer.enabled = false;
    }

    public void ShowEdge()
    {
        arrowHead.gameObject.SetActive(true);
        symbolText.gameObject.SetActive(true);
        lineRenderer.enabled = true;
    }

    public void SetLoop(bool loop)
    {
        isLoop = loop;
    }

    public bool IsLoop()
    {
        return isLoop;
    }
}