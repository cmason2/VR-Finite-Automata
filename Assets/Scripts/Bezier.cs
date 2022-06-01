using UnityEngine;
using System.Collections.Generic;
using TMPro;

[RequireComponent(typeof(LineRenderer))]
public class Bezier : MonoBehaviour
{
    public Transform[] controlPoints;
    public LineRenderer lineRenderer;
    private int SEGMENT_COUNT = 50;
    public GameObject arrowHeadPrefab;
    private LineRenderer arrowHeadLine;
    private GameObject arrowHead;
    private Transform targetState;
    private SphereCollider targetCollider;
    public TMP_Text symbolText;
    public float symbolOffsetDistance = 0.1f;

    Vector3[] arrPositions;


    void Start()
    {
        if (!lineRenderer)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        arrPositions = new Vector3[SEGMENT_COUNT];

        //lineRenderer.positionCount = SEGMENT_COUNT;
        //lineRenderer.material.color = Color.black;

        targetState = controlPoints[2];
        targetCollider = targetState.GetComponentInChildren<SphereCollider>();
    }

    void Update()
    {
        CalculatePoints();
        DrawCurve();
        DrawSymbol();
    }

    void CalculatePoints()
    {
        for (int i = 0; i < SEGMENT_COUNT; i++)
        {
            float t = i / (float)SEGMENT_COUNT;
            Vector3 pixel = CalculateQuadraticBezierPoint(t, controlPoints[0].position, controlPoints[1].position, controlPoints[2].position);
            arrPositions[i] = pixel;
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
        List<Vector3> positions = new List<Vector3>(arrPositions);
        bool foundExternalPoint = false;
        Vector3 pointToCheck = Vector3.zero;

        // Check if last point of line is outside of destination sphere
        float stateRadius = targetCollider.transform.localScale.x / 2;
        if (Vector3.Distance(positions[SEGMENT_COUNT-1], targetState.position) > stateRadius)
        {
            Debug.Log("Last point is outside sphere, you should increase number of line segments");
        }

        // Find first point outside destination sphere, delete points inside
        for (int i = SEGMENT_COUNT - 1; i >= 0; i--)
        {
            pointToCheck = positions[i];
            if (Vector3.Distance(pointToCheck, targetState.position) > stateRadius)
            {
                foundExternalPoint = true;
                break;
            }
            positions.RemoveAt(i);
        }

        if (foundExternalPoint)
        {
            RaycastHit hit;

            Vector3 direction = targetState.position - pointToCheck;

            //Debug.DrawRay(pointToCheck, direction, Color.green);
            if (Physics.Raycast(pointToCheck, direction, out hit))
            {
                // Redraw line with internal points removed
                positions.Add(hit.point);
                lineRenderer.positionCount = positions.Count;
                lineRenderer.SetPositions(positions.ToArray());
                Debug.Log(positions.ToArray());

                if (arrowHead == null)
                    arrowHead = Instantiate(arrowHeadPrefab, hit.point, Quaternion.Euler(direction));
                else
                    arrowHead.transform.SetPositionAndRotation(hit.point, Quaternion.Euler(direction));

                arrowHead.transform.LookAt(targetState.transform.position);
                //if (arrowHeadLine == null)
                //{
                //    Vector3[] arrowPoints = new Vector3[3];
                //    arrowPoints[0] = Vector3.forward;
                //    arrowPoints[1] = hit.point;
                //    arrowPoints[2] = Vector3.forward;

                //    arrowHeadLine = gameObject.AddComponent<LineRenderer>();
                //    arrowHeadLine.SetPositions(arrowPoints);
                //}

            }
        }
        else
        {
            Debug.Log("Couldn't find point outside destination state");
        }
    }

    void DrawSymbol()
    {
        Vector3 middlePoint = lineRenderer.GetPosition(SEGMENT_COUNT / 2);

        Vector3 direction = controlPoints[1].position - middlePoint;

        //if (direction.magnitude < symbolOffsetDistance)
        //{
        //    Debug.Log("Too Close!");
        //}
        direction.Normalize();

        symbolText.transform.position = middlePoint + (direction * symbolOffsetDistance);
    }
}