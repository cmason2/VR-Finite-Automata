using UnityEngine;
using System.Collections.Generic;
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


    void Start()
    {
        if (!lineRenderer)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
        
        lineRenderer.positionCount = SEGMENT_COUNT;
        //lineRenderer.material.color = Color.black;

        targetState = controlPoints[2];
    }

    void Update()
    {
        DrawCurve();
        DrawArrowHead();
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

    void DrawArrowHead()
    {
        Vector3[] positions = new Vector3[SEGMENT_COUNT];


        lineRenderer.GetPositions(positions);
        bool foundExternalPoint = false;
        Vector3 pointToCheck = Vector3.zero;

        // Check if last point of line is outside of destination sphere
        if (Vector3.Distance(positions[SEGMENT_COUNT-1], targetState.position) > (targetState.localScale.x / 2))
        {
            Debug.Log("Last point is outside sphere, you should increase number of line segments");
        }

            for (int i = SEGMENT_COUNT - 1; i >= 0; i--)
        {
            pointToCheck = positions[i];
            if (Vector3.Distance(pointToCheck, targetState.position) > (targetState.localScale.x/2))
            {
                foundExternalPoint = true;
                break;
            }
        }

        if (foundExternalPoint)
        {
            RaycastHit hit;

            Vector3 direction = targetState.position - pointToCheck;

            //Debug.DrawRay(pointToCheck, direction, Color.green);
            if (Physics.Raycast(pointToCheck, direction, out hit))
            {
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
}